using ExcelDna.Integration;

namespace Quandl.Excel.UDF.Functions.UserDefinedFunctions
{
    public static class Timeseries
    {
        [ExcelFunction("Pull time series data from the Quandl time series API", Name = "QSERIES", IsMacroType = true,
            Category = "Financial")]
        public static string Qseries(
            [ExcelArgument(Name = "quandlCode",
                Description = "Single or multiple Quandl codes with optional columns references", AllowReference = true)
            ] object
                quandlCode,
            [ExcelArgument(Name = "dateRange", Description = "(optional) The date or range of dates to filter on",
                AllowReference = true)] object dates =
                null,
            [ExcelArgument(Name = "collapse", Description = "(optional) How to collapse the data", AllowReference = true
                )] string collapse = null,
            [ExcelArgument(Name = "order", Description = "(optional) Order the data is returned in",
                AllowReference = true)] string sort = null,
            [ExcelArgument(Name = "transformation", Description = "(optional) How the data is to be transformed",
                AllowReference = true)] string transformation
                = null,
            [ExcelArgument(Name = "limit", Description = "(optional) Limit the number of rows returned",
                AllowReference = true)] object limit = null,
            [ExcelArgument(Name = "headers",
                Description = "(optional) Default: true - Whether the resulting data will include a header row",
                AllowReference = true)] bool header = true
            )
            {
                return "Not yet implemented";
            }
        }
    }