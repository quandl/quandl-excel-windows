using ExcelDna.Integration;
using MSExcel = Microsoft.Office.Interop.Excel;
using Quandl.Shared;
using Quandl.Shared.Excel;
using Quandl.Shared.Models;
using Quandl.Excel.UDF.Functions.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Quandl.Shared.Helpers;
using Microsoft.Office.Interop.Excel;

namespace Quandl.Excel.UDF.Functions.UserDefinedFunctions
{
    public static class Datatable
    {
        private const int RowPullCountMax = 1000;

        /**
         * The following is a very tricky setup. To avoid cells being re-calculated over and over again we need to mark a UDF as non-volatile. Due to the way
         * in which non-volatility works just marking it as non-volatile off the bat is not enough. We need to mark it on each run. Furthermore to make that
         * type of call can only be done from a Macro UDF. Therefore to achieve the desired scenario we do the following:
         * 
         * 1. Mark the UDF as a Macro and Volatile to begin with.
         * 2. When UDF runs immediately mark it as non-volatile
         * 3. When outputting the data run this in a Queued macro function so that it does not influence the running calculation thread. 
         */
        [ExcelFunction("Pull in Quandl data via the API",
            Name = "QTABLE",
            Category = "Financial",
            IsMacroType = true,
            IsExceptionSafe = false,
            IsThreadSafe = false,
            IsVolatile = true)]
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
            // Need to reset cell volatility on each run-through
            Tools.SetCellVolatile(false);
            // Get the current cell formula.
            var reference = (ExcelReference)XlCall.Excel(XlCall.xlfCaller);

            // Prevent the formula from running should it be blocked.
            if (QuandlConfig.PreventCurrentExecution)
            {
                return Locale.English.AutoDownloadTurnedOff;
            }
            return Process(reference, rawQuandlCode, rawColumns, argName1, argValue1, argName2, argValue2, argName3, argValue3, argName4, argValue4, argName5, argValue5, argName6, argValue6);
        }

        /// <summary>
        ///   Converts a filter value from a list of objects to a list of string values
        /// </summary>
        /// <param name="filter"></param>
        private static void CreateFilterValue(ref object filter)
        {
            if (filter is ExcelReference || filter is ExcelMissing || filter is string || filter == null) return;

            IEnumerable enumerableFilterValues = filter as IEnumerable;
            filter = enumerableFilterValues.Cast<string>().ToList();
        }

        private static string Process(ExcelReference currentFormulaCellReference, object rawQuandlCode, object rawColumns, object argName1, object argValue1, object argName2, object argValue2, object argName3, object argValue3, object argName4, object argValue4, object argName5, object argValue5, object argName6, object argValue6)
        {
            Common.StatusBar.AddMessage(Locale.English.UdfRetrievingData);
            var queryParams = new DatatableParams();
            try
            {
                // Parse out all the parameters specified in the UDF.
                var quandlCode = Tools.GetStringValue(rawQuandlCode);
                var columns = Tools.GetArrayOfValues(rawColumns).Select(s => ((string)s).ToLower()).ToList();

                // Add all the query parameters
                if (!string.IsNullOrEmpty(QuandlConfig.ApiKey))
                {
                    queryParams.AddInternalParam("api_key", QuandlConfig.ApiKey);
                }
                if (columns.Count > 0)
                {
                    queryParams.AddInternalParam("qopts.columns", columns);
                }

                CreateFilterValue(ref argValue1);
                CreateFilterValue(ref argValue2);
                CreateFilterValue(ref argValue3);
                CreateFilterValue(ref argValue4);
                CreateFilterValue(ref argValue5);
                CreateFilterValue(ref argValue6);

                // The user query or additional qopts params
                queryParams.AddParam(Tools.GetStringValue(argName1), Tools.GetStringValue(argValue1));
                queryParams.AddParam(Tools.GetStringValue(argName2), Tools.GetStringValue(argValue2));
                queryParams.AddParam(Tools.GetStringValue(argName3), Tools.GetStringValue(argValue3));
                queryParams.AddParam(Tools.GetStringValue(argName4), Tools.GetStringValue(argValue4));
                queryParams.AddParam(Tools.GetStringValue(argName5), Tools.GetStringValue(argValue5));
                queryParams.AddParam(Tools.GetStringValue(argName6), Tools.GetStringValue(argValue6));

                // If the user has not added in any query parameters warn them that its probably not a good idea to continue forward.
                if (!ShouldContinueWithoutParams(queryParams.UserParamsGiven))
                {
                    return Locale.English.AdditionalQueryParamsPleaseAdd;
                }

                // Pull the metadata first to get the first column name. This is not very efficient as it makes another call just to get one field.
                Common.StatusBar.AddMessage(Locale.English.UdfRetrievingData);
                queryParams.AddInternalParam("qopts.per_page", 1);
                var task = new Web().GetDatatableData(quandlCode, queryParams.QueryParams);
                var firstCellString = task.Result.Columns[0].Name;

                // Reset to pull x rows at a time.
                queryParams.AddInternalParam("qopts.per_page", RowPullCountMax);

                // Pull the data
                var retriever = new RetrieveAndWriteData(quandlCode, queryParams, currentFormulaCellReference);
                var thready = new Thread(retriever.fetchData);
                thready.Priority = ThreadPriority.Normal;
                thready.IsBackground = true;
                thready.Start();

                // Begin the reaping thread. This is necessary to kill off and formula that are functioning for a long time.
                FunctionGrimReaper.AddNewThread(thready);

                return Utilities.ValidateEmptyData(firstCellString);
            }
            catch (DatatableParamError e)
            {
                Logger.log(e, AdditionalInfo(queryParams));
                return e.Message;
            }
            catch (System.Exception e)
            {
                return Common.HandlePotentialQuandlError(e, true, AdditionalInfo(queryParams));
            }
        }

        // Spawn a msg box to ask the user if they want to continue even if they don't have any query params given.
        // This is run in a task thread to avoid deadlock issues within the main excel thread.
        private static bool ShouldContinueWithoutParams(bool paramsGiven)
        {
            var shouldContinue = Task.Factory.StartNew(() =>
            {
                if (QuandlConfig.LongRunningQueryWarning && !paramsGiven)
                {
                    DialogResult continueAnyways = MessageBox.Show(
                        Locale.English.AdditionalQueryParamsRequiredDesc,
                        Locale.English.AdditionalQueryParamsRequiredTitle,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (continueAnyways == DialogResult.No)
                    {
                        return false;
                    }
                }
                return true;
            });
            return shouldContinue.Result;
        }

        private static Dictionary<string, string> AdditionalInfo(DatatableParams queryParams)
        {
            var d1 = new Dictionary<string, string>() { { "UDF", "QTABLE" } };
            var d2 = queryParams.QueryParams.Select(entry => new KeyValuePair<string, string>(entry.Key, Utilities.ObjectToHumanString(entry.Value)));
            return d1.Concat(d2).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.First().Value);
        }

        internal class RetrieveAndWriteData 
        {
            private string _quandlCode;
            private DatatableParams _datatableParams;
            private ExcelReference _currentCellReference;
            private MSExcel.Range _currentCellRange;
            ~RetrieveAndWriteData()
            {
                ReleaseRange();
            }

            void ReleaseRange()
            {
                try
                {
                    if (_currentCellRange != null)
                    {
                        Marshal.ReleaseComObject(_currentCellRange);
                        _currentCellRange = null;
                    }
                }
                catch // suppress any exception
                {

                }
                GC.SuppressFinalize(this); // we only need to run this function once

            }
            public RetrieveAndWriteData(string quandlCode, DatatableParams datatableParams, ExcelReference currentCellReference)
            {
                this._quandlCode = quandlCode;
                this._datatableParams = datatableParams;
                this._currentCellReference = currentCellReference;
                this._currentCellRange = Tools.ReferenceToRange(currentCellReference);
            }

            private static readonly Mutex populationMutex
                = new Mutex();
            public void fetchData()
            {
                int currentRow = 0;
                string nextCursorId = null;
                bool? confirmedOverwrite = null;
                try
                {
                    do
                    {
                        // Fetch rows
                        var task = new Web().GetDatatableData(_quandlCode, _datatableParams.QueryParams);
                        var results = task.Result;

                        // Inform the user whats going on.
                        currentRow += results.Data.DataPoints.Count();
                        Common.StatusBar.AddMessage(
                            Locale.English.UdfRetrievingDataMoreDetails.Replace("{currentRow}", currentRow.ToString()));

                        // Process fetched rows
                        var processedData = new ResultsData(results.Data.DataPoints,
                            results.Columns.Select(c => c.Code).ToList());
                        // async processing ends here.
                        // however looks like this function executes synchronously

                        // Get table metadata
                        var metaDataTask = new Web().GetDatatableMetadata(_quandlCode);
                        var metaDataResults = metaDataTask.Result;

                        // Write fetch rows out to the sheet. If this is the first iteration save the value to display in the formula cell.
                        SheetHelper excelWriter = new SheetHelper(processedData, false, false, true, false, metaDataResults);
                        if (nextCursorId == null)
                        {
                            excelWriter = new SheetHelper(processedData, true, true, true, false, metaDataResults);
                        }

                        // Bail out if the worksheet no longer exists.
                        if (!WorksheetStillExists(_currentCellRange))
                        {
                            return;
                        }

                        // If the user already accepted to overwrite data then set that.
                        excelWriter.ConfirmedOverwrite = confirmedOverwrite;

                        // Write data and save state of whether to continue overwriting.
                        // this logic currently executes from background thread, so use locking
                        bool mutexAcquired;
                        try
                        {
                            mutexAcquired = populationMutex.WaitOne();

                        }
                        catch
                        {
                            mutexAcquired = false;
                        }
                        try
                        {
                            excelWriter.PopulateData(_currentCellRange);
                        }
                        finally
                        {
                            if (mutexAcquired)
                            {
                                try
                                {
                                    populationMutex.ReleaseMutex();
                                }
                                catch
                                {
                                }
                            }
                        }
                        

                        // Bail out if the user said no to overwriting data;
                        confirmedOverwrite = excelWriter.ConfirmedOverwrite;
                        if (excelWriter.ConfirmedOverwrite == false)
                        {
                            Common.StatusBar.AddMessage(Locale.English.WarningOverwriteNotAccepted);
                            return;
                        }

                        // Update the query params for next run if their is a cursor given and then increment the range where new data should go.
                        if (!string.IsNullOrWhiteSpace(results.Data.Cursor))
                        {
                            var headerOffset = 0;
                            if (nextCursorId == null)
                            {
                                headerOffset = 1;
                            }

                            nextCursorId = results.Data.Cursor;
                            _datatableParams.AddInternalParam("qopts.cursor_id", results.Data.Cursor);
                            MSExcel.Worksheet currentWorksheet = null;
                            MSExcel.Range wsCells = null;
                            try
                            {
                                currentWorksheet = _currentCellRange.Worksheet;
                                wsCells = currentWorksheet.Cells;
                                var swapCell =
                                    wsCells[_currentCellRange.Row + headerOffset + results.Data.DataPoints.Count,
                                        _currentCellRange.Column];
                                Marshal.ReleaseComObject(_currentCellRange);
                                _currentCellRange = swapCell;
                            }
                            finally
                            {
                                if (currentWorksheet != null)
                                {
                                    Marshal.ReleaseComObject(currentWorksheet);
                                }

                                if (wsCells != null)
                                {
                                    Marshal.ReleaseComObject(wsCells);
                                }
                            }
                        }
                        else
                        {
                            nextCursorId = null;
                        }
                    } while (!string.IsNullOrWhiteSpace(nextCursorId));

                    Common.StatusBar.AddMessage(Locale.English.UdfDataRetrievalSuccess);
                    Common.StatusBar.AddMessage(Locale.English.UdfDataWritingSuccess);
                }
                catch (COMException e)
                {
                    // Most likely the worksheet no longer exists so bail out. These two codes seem to occur during those scenarios.
                    if (e.HResult == Shared.Excel.Exception.BAD_REFERENCE ||
                        e.HResult == Shared.Excel.Exception.VBA_E_IGNORE)
                    {
                        return;
                    }

                    Common.HandlePotentialQuandlError(e, false, AdditionalInfo(_datatableParams));
                    Common.StatusBar.AddMessage(Locale.English.UdfCompleteError);
                }
                catch (ThreadAbortException)
                {
                    return; // Safe to ignore aborting threads. Assume user forcibly stopped the UDF.
                }
                catch (System.Exception e)
                {
                    Common.HandlePotentialQuandlError(e, false, AdditionalInfo(_datatableParams));
                    Common.StatusBar.AddMessage(Locale.English.UdfCompleteError);
                }
                finally
                {
                    ReleaseRange(); 
                }
            }

            private bool WorksheetStillExists(MSExcel.Range range)
            {
                if (range == null)
                {
                    return false;
                }

                MSExcel.Worksheet ws = null;
                try
                {
                    ws = range.Worksheet;
                    return ws != null;
                }
                finally
                {
                    if (ws != null)
                    {
                        Marshal.ReleaseComObject(ws);
                    }
                }
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