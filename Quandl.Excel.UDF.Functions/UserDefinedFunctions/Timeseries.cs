using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExcelDna.Integration;
using Microsoft.Office.Interop.Excel;
using Quandl.Shared;
using Quandl.Shared.Models;
using Quandl.Shared.Excel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Quandl.Excel.UDF.Functions.UserDefinedFunctions
{
    public static class Timeseries
    {
        private static Dictionary<string, DatasetMeta> datasetMetadata = new Dictionary<string, DatasetMeta>();
        private static StatusBar StatusBar => StatusBarInstance();


        [ExcelFunction("Pull time series data from the Quandl time series API", Name = "QSERIES", IsMacroType = true,
            Category = "Financial")]
        public static string Qseries(
            [ExcelArgument(Name = "quandlCode",
                Description = "Single or multiple Quandl codes with optional columns references", AllowReference = true)
            ] object rawQuandlCodeColumns,
            [ExcelArgument(Name = "dateRange", Description = "(optional) The date or range of dates to filter on", AllowReference = true)] object rawDates = null,
            [ExcelArgument(Name = "frequency", Description = "(optional) Change the sampling frequency of the returned data", AllowReference = true
                )] string rawCollapse = null,
            [ExcelArgument(Name = "order", Description = "(optional) Order the data is returned in",
                AllowReference = true)] string rawOrder = null,
            [ExcelArgument(Name = "transformation", Description = "(optional) How the data is to be transformed",
                AllowReference = true)] string rawTransformation = null,
            [ExcelArgument(Name = "limit", Description = "(optional) Limit the number of rows returned",
                AllowReference = true)] object rawLimit = null,
            [ExcelArgument(Name = "headers",
                Description = "(optional) Default: true - Whether the resulting data will include a header row",
                AllowReference = true)] string rawHeader = null
            )
        {
            // Prevent the formula from running should it be blocked.
            if (QuandlConfig.PreventCurrentExecution)
            {
                return Locale.English.AutoDownloadTurnedOff;
            }

            try
            {
                // Parse out all the parameters specified in the UDF.
                var quandlCodeColumns = Tools.GetArrayOfValues(rawQuandlCodeColumns).Select(s => ((string)s).ToUpper()).ToList();
                var dates = Tools.GetArrayOfDates(rawDates);
                var collapse = Tools.GetStringValue(rawCollapse);
                var orderAsc = Tools.GetStringValue(rawOrder).ToLower() == "asc";
                var transformation = Tools.GetStringValue(rawTransformation);
                var limit = Tools.GetIntValue(rawLimit);
                var includeHeader = string.IsNullOrEmpty(rawHeader) || Tools.GetBoolValue(rawHeader);

                // Get the current cell formula.
                var reference = (ExcelReference)XlCall.Excel(XlCall.xlfCaller);
                Range currentFormulaCell = Tools.ReferenceToRange(reference);

                // Begin the reaping thread. This is necessary to kill off and formula that are functioning for a long time.
                FunctionGrimReaper.BeginTheReaping(currentFormulaCell.Application);

                // Update status
                StatusBar.AddMessage(Locale.English.UdfRetrievingData);

                // Pull the data
                ResultsData results = null;
                try
                {
                    results = RetrieveData(quandlCodeColumns, dates, collapse, transformation, limit);
                }
                catch (DatasetParamError e)
                {
                    return e.Message;
                }

                // Sort out the data and place it in the cells
                var sortedResults = new ResultsData(results.SortedData("date", orderAsc), results.Headers);
                var reorderColumns = sortedResults.ExpandAndReorderColumns(SanitizeColumnNames(quandlCodeColumns));
                var excelWriter = new SheetHelper(currentFormulaCell, reorderColumns, includeHeader, true);

                if (excelWriter.ConfirmedOverwrite == false)
                {
                    StatusBar.AddMessage(Locale.English.WarningOverwriteNotAccepted);
                }

                var firstCellMsg = Utilities.ValidateEmptyData(excelWriter.PopulateData());
                StatusBar.AddMessage(Locale.English.UdfCompleteSuccess);
                return firstCellMsg;
            }
            catch (Exception e)
            {
                StatusBar.AddMessage(Locale.English.UdfCompleteError);
                Trace.WriteLine(e.Message);
                Utilities.LogToSentry(e, "Qtable", $"|rawQuandlCodeColumns:{rawQuandlCodeColumns.ToString()}" +
                                                   $"|rawDates:{rawDates.ToString()}" +
                                                   $"|rawCollapse:{rawCollapse.ToString()}" +
                                                   $"|rawOrder:{rawOrder.ToString()}" +
                                                   $"|rawTransformation:{rawTransformation.ToString()}" +
                                                   $"|rawLimit:{rawLimit.ToString()}" +
                                                   $"|rawHeader:{rawHeader.ToString()}");
                throw;
            }
        }

        private static ResultsData RetrieveData(List<string> quandlCodeColumns,
            List<DateTime?> dates, string collapse, string transformation, int? limit)
        {
            var datasets = new Dictionary<string, DatasetParams>();
            var datasetsWithoutColumns = new List<string>();

            var uniqueQuandlCodes = GetDatasetQuandlCodes(quandlCodeColumns);
            GetDatasetMetadata(uniqueQuandlCodes);

            quandlCodeColumns = SanitizeColumnNames(quandlCodeColumns);

            foreach (var quandlCodeColumn in quandlCodeColumns)
            {
                var splitString = SplitQuandlCode(quandlCodeColumn);

                // Quandl code and column (ex: NSE/OIL/HIGH)
                if (splitString.Length == 3)
                {
                    var quandlCode = string.Join("/", splitString[0], splitString[1]);
                    if (!datasets.ContainsKey(quandlCode))
                    {
                        datasets[quandlCode] = new DatasetParams(quandlCode, dates, collapse, transformation, limit);
                    }
                    datasets[quandlCode].Columns.Add(splitString[2]);
                }
                // Quandl code only (ex: NSE/OIL)
                else if (splitString.Length == 2)
                {
                    var quandlCode = string.Join("/", splitString[0], splitString[1]);
                    if (!datasets.ContainsKey(quandlCode))
                    {
                        datasets[quandlCode] = new DatasetParams(quandlCode, dates, collapse, transformation, limit);
                    }
                    datasetsWithoutColumns.Add(quandlCode);
                }
                // Invalid format
                else
                {
                    throw new DatasetParamError($"Invalid Quandl code: {quandlCodeColumn}");
                }
            }

            // If any datasets without columns have been specified remove any customized columns that users specified to ensure all columns are pulled.
            datasetsWithoutColumns.ForEach(qc => datasets[qc].Columns.Clear());

            // Fetch all the data at the same time.
            var tasks = datasets.Select(dsp => Web.GetDatasetData(dsp.Value.Code, dsp.Value.QueryParams));
            var fetchTask = Task.WhenAll(tasks);
            fetchTask.Wait();

            // Create a bunch of results which we can combine to one giant table
            var combinedResults = new ResultsData(new List<List<object>>(), new List<string>());
            foreach (var qcc in fetchTask.Result.Select((x, i) => new { Value = x, Index = i }))
            {
                var dataset = qcc.Value;
                var columns =
                    dataset.Columns.Select(
                        c => c.Code.ToUpper() == "DATE" ? c.Code : $"{dataset.Code}/{c.Code}".ToUpper()).ToList();
                var newResults = new ResultsData(dataset.Data.DataPoints, columns);
                combinedResults = combinedResults.Combine(newResults);
            }

            return combinedResults;
        }

        // Try really hard to get the instance of the status bar from the application.
        public static StatusBar StatusBarInstance()
        {
            try
            {
                return new StatusBar((Microsoft.Office.Interop.Excel.Application)ExcelDnaUtil.Application);
            }
            catch (COMException e)
            {
                // The excel RPC server is busy. We need to wait and then retry (RPC_E_SERVERCALL_RETRYLATER)
                if (e.HResult == -2147417846 || e.HResult == -2146777998)
                {
                    Thread.Sleep(Datatable.RetryWaitTimeMs);
                    return StatusBarInstance();
                }

                throw;
            }
        }

        private static List<string> GetDatasetQuandlCodes(List<string> qCodes)
        {
            HashSet<string> codes = new HashSet<string>();
            foreach (var code in qCodes)
            {
                var codeArr = SplitQuandlCode(code);
                codes.Add($"{codeArr[0]}/{codeArr[1]}");
            }

            return codes.ToList();
        }

        private static void GetDatasetMetadata(List<string> datasetCodes)
        {
            foreach (var code in datasetCodes)
            {
                if (!datasetMetadata.ContainsKey(code))
                {
                    // perform a metadata api query based on the dataset code
                    var fetchTask = Task.WhenAll(Web.GetDatasetMetadata(code));
                    fetchTask.Wait();

                    var metadata = fetchTask.Result.First().Metadata;
                    datasetMetadata.Add(code, metadata);
                }
            }
        }

        private static List<string> SanitizeColumnNames(List<string> quandlCodes)
        {
            List<string> convertedQuandlCodes = new List<string>();

            foreach (var code in quandlCodes)
            {
                var codeFragments = SplitQuandlCode(code);
                var datasetCode = $"{codeFragments[0]}/{codeFragments[1]}";
                if (codeFragments.Count() == 3)
                {
                    int result;
                    if (int.TryParse(codeFragments[2], out result))
                    {
                        codeFragments[2] = datasetMetadata[datasetCode].Columns[result];
                    }
                }

                var newQuandlCode = string.Join("/", codeFragments).ToUpper();
                convertedQuandlCodes.Add(newQuandlCode);
            }

            return convertedQuandlCodes;
        }

        private static string[] SplitQuandlCode(string code)
        {
            return code.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        internal class DatasetParamError : ArgumentException
        {
            public DatasetParamError(string message) : base(message)
            {
            }
        }

        internal class DatasetParams
        {
            private readonly string _collapse;
            private readonly string[] _collapseFilters = { "daily", "weekly", "monthly", "quarterly", "annual" };
            private readonly List<DateTime?> _dates;
            private readonly int? _limit;
            private readonly string _transformation;
            private readonly string[] _transformationFilters = { "diff", "rdiff", "rdiff_from", "cumul", "normalize" };
            public readonly string Code;

            public readonly List<string> Columns;

            public DatasetParams(string code, List<DateTime?> dates, string collapse, string transformation, int? limit)
            {
                Code = code;
                Columns = new List<string>();
                _dates = dates;
                _collapse = collapse;
                _transformation = transformation;
                _limit = limit;
            }

            public Dictionary<string, object> QueryParams
            {
                get
                {
                    var queryParams = new Dictionary<string, object>();

                    // Add column names specified by the user. Remove date as thats returned by default anyways
                    queryParams.Add("column_index", Columns);
                    if (queryParams["column_index"] is List<string>)
                    {
                        queryParams["column_index"] = ((List<string>)queryParams["column_index"]).Where(s => s != "DATE").ToList();
                    }

                    // Convert dates
                    if (_dates.Count == 2 && _dates[0] != null && _dates[1] != null)
                    {
                        queryParams.Add("start_date", ((DateTime)_dates[0]).ToString(Utilities.DateFormat));
                        queryParams.Add("end_date", ((DateTime)_dates[1]).ToString(Utilities.DateFormat));
                    }
                    else if (_dates.Count == 2 && _dates[0] != null && _dates[1] == null)
                    {
                        queryParams.Add("start_date", ((DateTime)_dates[0]).ToString(Utilities.DateFormat));
                    }
                    else if (_dates.Count == 2 && _dates[0] == null && _dates[1] != null)
                    {
                        queryParams.Add("end_date", ((DateTime)_dates[1]).ToString(Utilities.DateFormat));
                    }
                    else if (_dates.Count == 1 && _dates[0] != null)
                    {
                        queryParams.Add("start_date", ((DateTime)_dates[0]).ToString(Utilities.DateFormat));
                        queryParams.Add("end_date", ((DateTime)_dates[0]).ToString(Utilities.DateFormat));
                    }
                    else if (_dates.Count != 0)
                    {
                        throw new DatasetParamError(Locale.English.DatasetParamsInvalidDateFilters);
                    }

                    // Collapse filters
                    if (_collapseFilters.Contains(_collapse))
                    {
                        queryParams.Add("collapse", _collapse);
                    }
                    else if (!string.IsNullOrEmpty(_collapse))
                    {
                        throw new DatasetParamError(Locale.English.DatasetParamsInvalidCollapse.Replace("{collapse}", _collapse));
                    }

                    // Transformation filters
                    if (_transformationFilters.Contains(_transformation))
                    {
                        queryParams.Add("transform", _transformation);
                    }
                    else if (!string.IsNullOrEmpty(_transformation))
                    {
                        throw new DatasetParamError(Locale.English.DatasetParamsInvalidTransformation.Replace("{transformation}", _transformation));
                    }

                    // Convert limits
                    if (_limit > 0)
                    {
                        queryParams.Add("limit", _limit);
                    }
                    else if (_limit != null)
                    {
                        throw new DatasetParamError(Locale.English.DatasetParamsLimitZeroOrBelow);
                    }

                    return queryParams;
                }
            }
        }
    }
}
