using System;
using System.Collections;
using System.Collections.Generic;
using ExcelDna.Integration;
using Quandl.Shared;

namespace Quandl.Excel.UDF.Functions
{
    public static class Core
    {
        [ExcelFunction(Description = "Quandl QDATA function pull single value", IsMacroType = true)]
        public static string QDATA(
            [ExcelArgument("is the quandl code", AllowReference = true)] Object excelQuandlCode,
            [ExcelArgument("is the prefered column", AllowReference = true)] Object excelColumnName = null,
            [ExcelArgument("is the date", AllowReference = true)] Object excelDate = null
            )
        {
            // tranlaste input parameters from string value or excel references
            string quandlCode = Tools.GetStringValue(excelQuandlCode);
            string columnName = Tools.GetStringValue(excelColumnName);
            string date = Tools.GetDateValue(excelDate);

            return Utilities.ValidateEmptyData(Web.PullSingleValue(quandlCode, columnName, date));
        }

        [ExcelFunction(Description = "Quandl hQDATA function pull single value", IsMacroType = true)]
        public static string hQDATA(
            [ExcelArgument("is the quandl code", AllowReference = true)] Object excelQuandlCode,
            [ExcelArgument("is the prefered column", AllowReference = true)] Object excelStartDate,
            [ExcelArgument("is the start date", AllowReference = true)] Object excelEndDate,
            [ExcelArgument("is the end date", AllowReference = true)] Object excelColumnNames
            )
        {
            ExcelReference reference = (ExcelReference)XlCall.Excel(XlCall.xlfCaller);
            Microsoft.Office.Interop.Excel.Range currentFormulaCell = Tools.ReferenceToRange(reference);

            // tranlaste input parameters from string value or excel references
            string quandlCode = Tools.GetStringValue(excelQuandlCode);
            string startDate = Tools.GetDateValue(excelStartDate);
            string endDate = Tools.GetDateValue(excelEndDate);
            ArrayList columnNames = Utilities.ListToUpper(Tools.GetArrayOfValues(excelColumnNames));

            // TODO: convert columnNames to List<string>
            var task = Web.PullHistoryData(quandlCode, startDate, endDate, new List<string>());
            task.Wait();
            var list = task.Result;
            // TODO: support List<List<object>>
            return Utilities.ValidateEmptyData(ExcelHelp.PopulateData(currentFormulaCell, new ArrayList()));
        }


        [ExcelFunction(Description = "Quandl mQDATA function", IsMacroType = true)]
        public static string mQDATA(
            [ExcelArgument("is the quandl database code", AllowReference = true)] Object excelQuandlCodes,
            [ExcelArgument("are the quandl column name list", AllowReference = true)] Object excelColumnNames
            )
        {
            ExcelReference reference = (ExcelReference)XlCall.Excel(XlCall.xlfCaller);
            Microsoft.Office.Interop.Excel.Range currentFormulaCell = Tools.ReferenceToRange(reference);

            // tranlaste input parameters from string value or excel references
            ArrayList quandlCodes = Tools.GetArrayOfValues(excelQuandlCodes);
            ArrayList columnNames = Utilities.ListToUpper(Tools.GetArrayOfValues(excelColumnNames));
        
            
            string value = "Failed"; // default return value
            int i = 0;
            foreach (string quandlCode in quandlCodes)
            {
                ArrayList list = Web.PullRecentStockData(quandlCode, columnNames, 1);
                // Remove column name list which is not required by this function.
                list.RemoveAt(0);
                // keep data of active cell which have mQDATA formula  
                if (i == 0)
                {
                    value = ((ArrayList)list[0])[1].ToString();
                }
                // populate data for each quandl code
                ExcelHelp.PopulateData(currentFormulaCell, quandlCode, list, i);
                i++;
            }

            return Utilities.ValidateEmptyData(value);
        }

    }
}
