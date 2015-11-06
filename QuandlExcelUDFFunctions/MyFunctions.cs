using System;
using System.Collections;
using System.Threading;

using ExcelDna.Integration;
using Excel = Microsoft.Office.Interop.Excel;
using Newtonsoft.Json.Linq;

using Quandl.Shared;

namespace QuandlFunctions
{
    public static class MyFunctions
    {
        static Excel.Application Application = ExcelDnaUtil.Application as Excel.Application;
        [ExcelFunction(Description = "My first .NET function")]
        public static string qdata(
            [ExcelArgument("is the quandl database code")] string databaseCode,
            [ExcelArgument("are the quandl stock code list")] string stockCode,
            [ExcelArgument("are the quandl stock indicator")] string indicator,
            [ExcelArgument("are the quandl column name list")] string columnName
            )
        {
            string[] stockCodes = TestFunctions.stringToArray(stockCode);
            string[] columnNames = TestFunctions.stringToArray(columnName);
            ProcessParams processParams = new ProcessParams();

            processParams.cell = Application.ActiveCell;

            int i = 1;
            foreach(string stock in stockCodes)
            {
                string fullCode = databaseCode + "/" + stock;
                string code = fullCode +"_" + indicator + "_q";
                processParams.data = TestFunctions.pullRecentStockData(code, columnNames, 1);
                processParams.code = fullCode;
                processParams.rowCount = i;
                i++;
                Thread t = new Thread(DumpFromThread);
                t.Start(processParams);
            }
            

            return "success";
        }

        private static void DumpFromThread(Object processParams)
        {
            ProcessParams p = processParams as ProcessParams;
            TestFunctions.populateData(p.code, p.cell, p.data,p.rowCount);
     
        }

        public class ProcessParams
        {
            public Excel.Range cell { get; set; }
            public ArrayList data { get; set; }

            public string code { get; set; }
            public int rowCount { get; set; }
        }

    }
}
