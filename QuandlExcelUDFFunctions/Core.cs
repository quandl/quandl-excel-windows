using System.Collections;
using System.Collections.Generic;
using ExcelDna.Integration;
using Microsoft.Office.Interop.Excel;
using Quandl.Shared;

namespace Quandl.Excel.UDF.Functions
{
    public static class Core
    {
        [ExcelFunction(Description = "Quandl QDATA function pull single value", IsMacroType = true)]
        public static string QDATA(
            [ExcelArgument("is the quandl code", AllowReference = true)] object excelQuandlCode,
            [ExcelArgument("is the prefered column", AllowReference = true)] object excelColumnName = null,
            [ExcelArgument("is the date", AllowReference = true)] object excelDate = null
            )
        {
            // translate input parameters from string value or excel references
            var quandlCode = Tools.GetStringValue(excelQuandlCode);
            var columnName = Tools.GetStringValue(excelColumnName);
            var date = Tools.GetDateValue(excelDate);

            var task = Web.PullSingleValue(quandlCode, columnName, date);
            task.Wait();
            var data = task.Result;
            return Utilities.ValidateEmptyData(data);
        }

        [ExcelFunction(Description = "Quandl hQDATA function pull single value", IsMacroType = true)]
        public static string hQDATA(
            [ExcelArgument("is the quandl code", AllowReference = true)] object excelQuandlCode,
            [ExcelArgument("is the prefered column", AllowReference = true)] object excelStartDate,
            [ExcelArgument("is the start date", AllowReference = true)] object excelEndDate,
            [ExcelArgument("is the end date", AllowReference = true)] object excelColumnNames
            )
        {
            var reference = (ExcelReference) XlCall.Excel(XlCall.xlfCaller);
            Range currentFormulaCell = Tools.ReferenceToRange(reference);

            // translate input parameters from string value or excel references
            var quandlCode = Tools.GetStringValue(excelQuandlCode);
            var startDate = Tools.GetDateValue(excelStartDate);
            var endDate = Tools.GetDateValue(excelEndDate);
            var columnNames = Utilities.ListToUpper(Tools.GetArrayOfValues(excelColumnNames));

            // TODO: convert columnNames to List<string>
            var task = Web.PullHistoryData(quandlCode, startDate, endDate, new List<string>());
            task.Wait();
            var list = task.Result;
            // TODO: support List<List<object>>
            return Utilities.ValidateEmptyData(ExcelHelp.PopulateData(currentFormulaCell, new ArrayList()));
        }


        [ExcelFunction(Description = "Quandl mQDATA function", IsMacroType = true)]
        public static string mQDATA(
            [ExcelArgument("is the quandl database code", AllowReference = true)] object excelQuandlCodes,
            [ExcelArgument("are the quandl column name list", AllowReference = true)] object excelColumnNames
            )
        {
            var reference = (ExcelReference) XlCall.Excel(XlCall.xlfCaller);
            Range currentFormulaCell = Tools.ReferenceToRange(reference);

            // translate input parameters from string value or excel references
            var quandlCodes = Tools.GetArrayOfValues(excelQuandlCodes);
            var columnNames = Utilities.ListToUpper(Tools.GetArrayOfValues(excelColumnNames));

            var value = "Failed"; // default return value
            var i = 0;
            foreach (var quandlCode in quandlCodes)
            {
                var dataTask = Web.PullRecentStockData(quandlCode, columnNames, 1);
                dataTask.Wait();
                var data = dataTask.Result;
                // Remove column name list which is not required by this function.
                data.RemoveAt(0);
                // keep data of active cell which have mQDATA formula  
                if (i == 0)
                {
                    value = data[0][1].ToString();
                }
                // populate data for each quandl code
                ExcelHelp.PopulateData(currentFormulaCell, quandlCode, data, i);
                i++;
            }

            return Utilities.ValidateEmptyData(value);
        }
    }
}