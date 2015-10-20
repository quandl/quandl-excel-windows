using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ExcelDna.Integration;
using Excel = Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            string code = stockName + "_" + "TOT_REVNU" + "_Q";
            string requestUri = "https://www.quandl.com/api/v3/datasets/" + code + "/data.json?limit=10&api_key=56LY1VVcCDFj1u3J48Kw";
            JObject o = getResponseJson(requestUri);
         
            ProcessParams p = new ProcessParams();
            p.range = currentCell;
            p.response = o;

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

        private static JObject getResponseJson(String requestUri)
        {
            WebClient client = new WebClient();
            client.Headers["User-Agent"] = "excel quandl new add-in";
            return JObject.Parse(client.DownloadString(requestUri));
        }

        public class ProcessParams
        {
            public Excel.Range range { get; set; }
            public JObject response { get; set; }
        }

    }
}
