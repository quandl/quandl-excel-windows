using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExcelDna.Integration;
using Microsoft.Office.Interop.Excel;
using Quandl.Shared;
using Quandl.Shared.Models;

namespace Quandl.Excel.UDF.Functions.UserDefinedFunctions
{
    public static class Timeseries
    {
        [ExcelFunction("Pull time series data from the Quandl time series API", Name = "QSERIES", IsMacroType = true,
            Category = "Financial")]
        public static string Qseries(
            [ExcelArgument(Name = "quandlCode",
                Description = "Single or multiple Quandl codes with optional columns references", AllowReference = true)
            ] object rawQuandlCodeColumns,
            [ExcelArgument(Name = "dateRange", Description = "(optional) The date or range of dates to filter on")] object rawDates = null,
            [ExcelArgument(Name = "collapse", Description = "(optional) How to collapse the data", AllowReference = true
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
            // Parse out all the data.
            var quandlCodeColumns = Tools.GetArrayOfValues(rawQuandlCodeColumns).Select(s => s.ToUpper()).ToList();
            var dates = GetDatesFromFormula(rawDates);
            var collapse = Tools.GetStringValue(rawCollapse);
            var orderAsc = Tools.GetStringValue(rawOrder).ToLower() == "asc";
            var transformation = Tools.GetStringValue(rawTransformation);
            var limit = Tools.GetIntValue(rawLimit);
            var includeHeader = string.IsNullOrEmpty(rawHeader) || Tools.GetBoolValue(rawHeader);

            // Get the current cell formula.
            var reference = (ExcelReference) XlCall.Excel(XlCall.xlfCaller);
            Range currentFormulaCell = Tools.ReferenceToRange(reference);

            // Pull the data and place it in the cells
            ResultsData results = null;
            try
            {
                results = RetrieveData(quandlCodeColumns, dates, collapse, transformation, limit);
            }
            catch (DatasetParamError e)
            {
                return e.Message;
            }

            // Sort out and display the data
            var sortedResults = new ResultsData(results.SortedData("date", orderAsc), results.Headers);
            var reorderColumns = sortedResults.ExpandAndReorderColumns(quandlCodeColumns);
            var excelWriter = new ExcelHelp(currentFormulaCell, reorderColumns, includeHeader);
            return Utilities.ValidateEmptyData(excelWriter.PopulateData());
        }

        private static List<DateTime?> GetDatesFromFormula(object dates)
        {
            var dateRange = Tools.GetArrayOfValues(dates);
            if (dateRange.Count == 0)
            {
                return new List<DateTime?>();
            }

            return dateRange.Select(Tools.GetDateValueFromString).ToList();
        }

        private static ResultsData RetrieveData(List<string> quandlCodeColumns,
            List<DateTime?> dates, string collapse, string transformation, int? limit)
        {
            var datasets = new Dictionary<string, DatasetParams>();
            var datasetsWithoutColumns = new List<string>();

            foreach (var quandlCodeColumn in quandlCodeColumns)
            {
                var splitString = quandlCodeColumn.Split("/".ToCharArray(),
                    StringSplitOptions.RemoveEmptyEntries);

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
            var tasks = datasets.Select(dsp => Web.GetData(dsp.Value.Code, dsp.Value.QueryParams));
            var fetchTask = Task.WhenAll(tasks);
            fetchTask.Wait();

            // Create a bunch of results which we can combine to one giant table
            var combinedResults = new ResultsData(new List<List<object>>(), new List<string>());
            foreach (var qcc in fetchTask.Result.Select((x, i) => new {Value = x, Index = i}))
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

        internal class DatasetParamError : ArgumentException
        {
            public DatasetParamError(string message) : base(message)
            {
            }
        }

        internal class DatasetParams
        {
            private readonly string _collapse;
            private readonly string[] _collapseFilters = {"daily", "weekly", "monthly", "quarterly", "annual"};
            private readonly List<DateTime?> _dates;
            private readonly int? _limit;
            private readonly string _transformation;
            private readonly string[] _transformationFilters = {"diff", "rdiff", "rdiff_from", "cumul", "normalize"};
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

                    // Column Names
                    queryParams.Add("column_index", Columns);

                    // Convert dates
                    if (_dates.Count == 2 && _dates[0] != null && _dates[1] != null)
                    {
                        queryParams.Add("start_date", ((DateTime) _dates[0]).ToString(Utilities.DateFormat));
                        queryParams.Add("end_date", ((DateTime) _dates[1]).ToString(Utilities.DateFormat));
                    }
                    else if (_dates.Count == 2 && _dates[0] != null && _dates[1] == null)
                    {
                        queryParams.Add("start_date", ((DateTime) _dates[0]).ToString(Utilities.DateFormat));
                    }
                    else if (_dates.Count == 2 && _dates[0] == null && _dates[1] != null)
                    {
                        queryParams.Add("end_date", ((DateTime) _dates[1]).ToString(Utilities.DateFormat));
                    }
                    else if (_dates.Count == 1 && _dates[0] != null)
                    {
                        queryParams.Add("start_date", ((DateTime) _dates[0]).ToString(Utilities.DateFormat));
                        queryParams.Add("end_date", ((DateTime) _dates[0]).ToString(Utilities.DateFormat));
                    }
                    else if (_dates.Count != 0)
                    {
                        throw new DatasetParamError(
                            "Invalid date filters specified. Please ensure a maximum of two dates are given and they are in the format YYYY-MM-DD.");
                    }

                    // Collapse filters
                    if (_collapseFilters.Contains(_collapse))
                    {
                        queryParams.Add("collapse", _collapse);
                    }
                    else if (!string.IsNullOrEmpty(_collapse))
                    {
                        throw new DatasetParamError(
                            $"Invalid collapse parameter given : {_collapse}");
                    }

                    // Transformation filters
                    if (_transformationFilters.Contains(_transformation))
                    {
                        queryParams.Add("transform", _transformation);
                    }
                    else if (!string.IsNullOrEmpty(_transformation))
                    {
                        throw new DatasetParamError(
                            $"Invalid transformation parameter given : {_transformation}");
                    }

                    // Convert limits
                    if (_limit > 0)
                    {
                        queryParams.Add("limit", _limit);
                    }
                    else if (_limit != null)
                    {
                        throw new DatasetParamError("Limit must be above zero or not specified.");
                    }

                    return queryParams;
                }
            }
        }
    }
}