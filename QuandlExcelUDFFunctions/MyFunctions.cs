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
            [ExcelArgument("is the quandl database code")] string stockName)
        {
            ProcessParams processParams = new ProcessParams();
            processParams.cell = Application.ActiveCell;
            string code = "ZFB/AAPL";
            string indicator = "_TOT_REVNU_Q";
            processParams.data = TestFunctions.pullRecentStockData(code + indicator, new string[] { "TOT_REVNU", "PER_END_DATE" }, 1);
            processParams.code = code;

            Thread t = new Thread(DumpFromThread);
            t.Start(processParams);

            return "success";
        }

        private static void DumpFromThread(Object processParams)
        {
            ProcessParams p = processParams as ProcessParams;
            TestFunctions.populateData(p.code, p.cell, p.data,1);
     
        }

        public class ProcessParams
        {
            public Excel.Range cell { get; set; }
            public ArrayList data { get; set; }

            public string code { get; set; }
        }

    }
}
