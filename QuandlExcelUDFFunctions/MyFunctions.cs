using System;
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

            Excel.Range currentCell = Application.ActiveCell;

            ProcessParams p = new ProcessParams();
            p.range = currentCell;
            p.response = TestFunctions.pullSomeData(stockName);

            Thread t = new Thread(DumpFromThread);
            t.Start(p);
            return stockName;
        }

        private static void DumpFromThread(Object p)
        {
            ProcessParams pp = p as ProcessParams;
            JObject o = pp.response;

            pp.range[0,2].Value2 = o["dataset_data"]["column_names"][0];
            pp.range[0, 3].Value2 = o["dataset_data"]["column_names"][1];

            for (int i = 1; i < 10; i++)
            {
                pp.range[i, 2] = o["dataset_data"]["data"][i-1][0].ToString();
                pp.range[i, 3] = o["dataset_data"]["data"][i-1][1].ToString();
            }

        }


        public class ProcessParams
        {
            public Excel.Range range { get; set; }
            public JObject response { get; set; }
        }

    }
}
