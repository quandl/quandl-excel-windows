using ExcelDna.Integration;

namespace Quandl.Excel.UDF.Functions.UserDefinedFunctions
{
    public static class Datatable
    {
        [ExcelFunction("Pull in Quandl data via the API", Name = "QTABLE", IsMacroType = true, Category = "Financial")]
        public static string Qtable(
            [ExcelArgument("A single Quandl code", AllowReference = true)] object quandlCode,
            [ExcelArgument("(optional) A list of columns to fetch", AllowReference = true)] object columns,
            [ExcelArgument("(optional) The name of filter 1", AllowReference = false)] object argName1,
            [ExcelArgument("(optional) The value of filter 1", AllowReference = true)] object argValue1,
            [ExcelArgument("(optional) The name of filter 2", AllowReference = false)] object argName2,
            [ExcelArgument("(optional) The value of filter 2", AllowReference = true)] object argValue2,
            [ExcelArgument("(optional) The name of filter 3", AllowReference = false)] object argName3,
            [ExcelArgument("(optional) The value of filter 3", AllowReference = true)] object argValue3,
            [ExcelArgument("(optional) The name of filter 4", AllowReference = false)] object argName4,
            [ExcelArgument("(optional) The value of filter 4", AllowReference = true)] object argValue4,
            [ExcelArgument("(optional) The name of filter 5", AllowReference = false)] object argName5,
            [ExcelArgument("(optional) The value of filter 5", AllowReference = true)] object argValue5,
            [ExcelArgument("(optional) The name of filter 6", AllowReference = false)] object argName6,
            [ExcelArgument("(optional) The value of filter 6", AllowReference = true)] object argValue6)
        {
            return "Not yet implemented";
        }
    }
}