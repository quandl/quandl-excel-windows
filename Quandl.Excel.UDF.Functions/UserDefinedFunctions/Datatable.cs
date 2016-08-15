using ExcelDna.Integration;
using Microsoft.Office.Interop.Excel;
using Quandl.Shared.Models;
using System.Linq;
using System;
using static Quandl.Excel.UDF.Functions.UserDefinedFunctions.Timeseries;
using System.Collections.Generic;
using Quandl.Shared;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Quandl.Shared.Excel;
using System.Diagnostics;
using System.Resources;

namespace Quandl.Excel.UDF.Functions.UserDefinedFunctions
{
    public static class Datatable
    {
        private const int RowPullCountMax = 1000;

        [ExcelFunction("Pull in Quandl data via the API", Name = "QTABLE", IsMacroType = true, Category = "Financial")]
        public static string Qtable(
            [ExcelArgument("A single Quandl code", Name = "Quandl Code", AllowReference = true)] object rawQuandlCode,
            [ExcelArgument("(optional) A list of columns to fetch", Name = "Columns", AllowReference = true)] object rawColumns,
            [ExcelArgument("(optional) The name of filter 1", AllowReference = true)] object argName1,
            [ExcelArgument("(optional) The value of filter 1", AllowReference = true)] object argValue1,
            [ExcelArgument("(optional) The name of filter 2", AllowReference = true)] object argName2,
            [ExcelArgument("(optional) The value of filter 2", AllowReference = true)] object argValue2,
            [ExcelArgument("(optional) The name of filter 3", AllowReference = true)] object argName3,
            [ExcelArgument("(optional) The value of filter 3", AllowReference = true)] object argValue3,
            [ExcelArgument("(optional) The name of filter 4", AllowReference = true)] object argName4,
            [ExcelArgument("(optional) The value of filter 4", AllowReference = true)] object argValue4,
            [ExcelArgument("(optional) The name of filter 5", AllowReference = true)] object argName5,
            [ExcelArgument("(optional) The value of filter 5", AllowReference = true)] object argValue5,
            [ExcelArgument("(optional) The name of filter 6", AllowReference = true)] object argName6,
            [ExcelArgument("(optional) The value of filter 6", AllowReference = true)] object argValue6)
        {
            // Get the current cell formula.
            var reference = (ExcelReference)XlCall.Excel(XlCall.xlfCaller);
            Range currentFormulaCell = Tools.ReferenceToRange(reference);

            // Prevent the formula from running should it be blocked.
            if (QuandlConfig.PreventCurrentExecution)
            {
                return Locale.English.AutoDownloadTurnedOff;
            }

            // Begin the reaping thread. This is necessary to kill off and formula that are functioning for a long time.
            FunctionGrimReaper.BeginTheReaping(currentFormulaCell.Application);

            return Process(currentFormulaCell, rawQuandlCode, rawColumns, argName1, argValue1, argName2, argValue2, argName3, argValue3, argName4, argValue4, argName5, argValue5, argName6, argValue6);
        }

        private static string Process(Range currentFormulaCell, object rawQuandlCode, object rawColumns, object argName1, object argValue1, object argName2, object argValue2, object argName3, object argValue3, object argName4, object argValue4, object argName5, object argValue5, object argName6, object argValue6)
        {
            try
            {
                // Parse out all the parameters specified in the UDF.
                var quandlCode = Tools.GetStringValue(rawQuandlCode);
                var columns = Tools.GetArrayOfValues(rawColumns).Select(s => s.ToUpper()).ToList();

                // Add all the query parameters
                var queryParams = new DatatableParams();
                if (!string.IsNullOrEmpty(QuandlConfig.ApiKey))
                {
                    queryParams.AddInternalParam("api_key", QuandlConfig.ApiKey);
                }
                if (columns.Count > 0)
                {
                    queryParams.AddInternalParam("qopts.columns", columns);
                }

                // The user query or additional qopts params
                queryParams.AddParam(Tools.GetStringValue(argName1), argValue1);
                queryParams.AddParam(Tools.GetStringValue(argName2), argValue2);
                queryParams.AddParam(Tools.GetStringValue(argName3), argValue3);
                queryParams.AddParam(Tools.GetStringValue(argName4), argValue4);
                queryParams.AddParam(Tools.GetStringValue(argName5), argValue5);
                queryParams.AddParam(Tools.GetStringValue(argName6), argValue6);

                // If the user has not added in any query parameters warn them that its probably not a good idea to continue forward.
                if (!queryParams.UserParamsGiven && !QuandlConfig.IgnoreMissingFormulaParams)
                {
                    DialogResult continueAnyways = MessageBox.Show(
                        Locale.English.AdditionalQueryParamsRequiredDesc,
                        Locale.English.AdditionalQueryParamsRequiredTitle,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (continueAnyways == DialogResult.No)
                    {
                        return Locale.English.AdditionalQueryParamsPleaseAdd;
                    }
                }

                // Pull the metadata first to get the first column name. This is not very efficient as it makes another call just to get one field.
                queryParams.AddInternalParam("qopts.per_page", 1);
                var task = Web.GetDatatableData(quandlCode, queryParams.QueryParams);
                task.Wait();
                var firstCellString = task.Result.Columns[0].Name;

                // Reset to pull x rows at a time.
                queryParams.AddInternalParam("qopts.per_page", RowPullCountMax);

                // Pull the data
                var retriever = new RetrieveAndWriteData(quandlCode, queryParams, (Range)currentFormulaCell);
                var thready = new Thread(retriever.fetchData);
                thready.Start();
                FunctionGrimReaper.AddNewThread(thready);

                return Utilities.ValidateEmptyData(firstCellString);
            }
            catch (DatatableParamError e)
            {
                return e.Message;
            }
        }

        internal class RetrieveAndWriteData
        {
            private string quandlCode;
            private DatatableParams datatableParams;
            private Range currentRange;

            public RetrieveAndWriteData(string quandlCode, DatatableParams datatableParams, Range currentRange)
            {
                this.quandlCode = quandlCode;
                this.datatableParams = datatableParams;
                this.currentRange = currentRange;
            }

            public void fetchData()
            {
                string next_cursor_id = null;

                try
                {
                    do
                    {
                        // Fetch rows
                        var task = Web.GetDatatableData(quandlCode, datatableParams.QueryParams);
                        task.Wait();
                        var results = task.Result;

                        // Process fetched rows
                        var processedData = new ResultsData(results.Data.DataPoints, results.Columns.Select(c => c.Code).ToList());

                        // Write fetch rows out to the sheet. If this is the first iteration save the value to display in the formula cell.
                        SheetHelper excelWriter = new SheetHelper(currentRange, processedData, false, true);
                        if (next_cursor_id == null)
                        {
                            excelWriter = new SheetHelper(currentRange, processedData, true, true);
                        }

                        // Bail out if the worksheet no longer exists.
                        if (!WorksheetStillExists())
                        {
                            return;
                        }

                        excelWriter.PopulateData();

                        // Update the query params for next run if their is a cursor given and then increment the range where new data should go.
                        if (results.Data.Cursor != null)
                        {
                            var header_offset = 0;
                            if (next_cursor_id == null)
                            {
                                header_offset = 1;
                            }

                            next_cursor_id = results.Data.Cursor;
                            datatableParams.AddInternalParam("qopts.cursor_id", results.Data.Cursor);

                            var worksheet = currentRange.Worksheet;
                            currentRange = (Range)worksheet.Cells[currentRange.Row + header_offset + results.Data.DataPoints.Count - 1, currentRange.Column];
                        }
                    } while (!string.IsNullOrWhiteSpace(next_cursor_id));
                }
                catch (COMException e)
                {
                    // Most likely the worksheet no longer exists so bail out. These two codes seem to occur during those scenarios.
                    if (e.HResult == -2146827864 || e.HResult == -2146777998)
                    {
                        return;
                    }
                    throw;
                }
                catch (ThreadAbortException)
                {
                    return; // Safe to ignore aborting threads
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                }
            }

            private bool WorksheetStillExists()
            {
                return !(currentRange == null || currentRange.Worksheet == null);
            }
        }

        internal class DatatableParamError : ArgumentException
        {
            public DatatableParamError(string message) : base(message)
            {
            }
        }


        internal class DatatableParams
        {
            public readonly string[] invalidArgNames = new string[] { "qopts.columns", "qopts.per_page", "api_key", "qopts.cursor_id" };

            public Dictionary<string, object> QueryParams { get; } = new Dictionary<string, object>();

            public bool UserParamsGiven = false;

            public DatatableParams() { }

            internal void AddParam(string key, object value)
            {
                if (invalidArgNames.Contains<string>(key))
                {
                    throw new DatatableParamError(Locale.English.DatatableParamInvalid.Replace("{key}", key));
                }
                else if (string.IsNullOrWhiteSpace(key) && (value == null || value is ExcelMissing))
                {
                    return;
                }
                else if (string.IsNullOrWhiteSpace(key) && (value != null || !(value is ExcelMissing)))
                {
                    throw new DatatableParamError(Locale.English.DatatableParamWithoutKey.Replace("{value}", (string)value));
                }
                else if (!string.IsNullOrWhiteSpace(key) && (value == null || value is ExcelMissing))
                {
                    throw new DatatableParamError(Locale.English.DatatableParamNullValue.Replace("{key}", key));
                }

                UserParamsGiven = true;

                AddInternalParam(key, value);
            }

            internal void AddInternalParam(string key, object value)
            {
                if (QueryParams.ContainsKey(key))
                {
                    QueryParams[key] = value;
                }
                else
                {
                    QueryParams.Add(key, value);
                }
            }
        }
    }
}