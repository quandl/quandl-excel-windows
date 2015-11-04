using System;
using System.Collections;
using System.Net;
using Newtonsoft.Json.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace Quandl.Shared
{
    public class TestFunctions
    {
        public static String apiUri = "https://www.quandl.com/api/v3/";

        public static JObject SearchDatabases(string query)
        {
            string requestUri = apiUri + "databases.json?per_page=10&query=" + query;
            return getResponseJson(requestUri);
        }
        public static JObject SearchDatasets(string databaseCode, string query)
        {
            string requestUri = apiUri + "datasets.json?database_code=" + databaseCode + "&per_page=10&query=" + query;
            return getResponseJson(requestUri);
        }

        public static JObject pullSomeData(string stockName)
        {
            string code = stockName;
            string requestUri = apiUri + "datasets/" + code + "/data.json?limit=10&api_key=56LY1VVcCDFj1u3J48Kw";
            return getResponseJson(requestUri);
        }

        private static JObject getResponseJson(String requestUri)
        {
            WebClient client = new WebClient();
            client.Headers["User-Agent"] = "excel quandl new add-in";
            client.Headers["Request-Source"] = "excel";
            //  client.Headers["Request-Platform"] = GetExcelVersionNumber().ToString();
            client.Headers["Request-Version"] = "3.0alpha";
            return JObject.Parse(client.DownloadString(requestUri));
        }

        //public static int GetExcelVersionNumber()
        //{
        //    Excel.Application excelApp = new Excel.Application();

        //    string versionName = excelApp.Version;
        //    int length = versionName.IndexOf('.');
        //    versionName = versionName.Substring(0, length);

        //    // int.parse needs to be done using US Culture.
        //    return int.Parse(versionName, CultureInfo.GetCultureInfo("en-US"));
        //}


        public static ArrayList pullRecentStockData(string code,string[] columnNames, int limit )
        {
            string requestUri = "https://www.quandl.com/api/v3/datasets/" + code + "/data.json?limit=" + limit.ToString() + "&api_key=56LY1VVcCDFj1u3J48Kw";
            JObject response =  getResponseJson(requestUri);

            string[] columns = response["dataset_data"]["column_names"].ToObject<string[]>();
            ArrayList columnsList = response["dataset_data"]["column_names"].ToObject<ArrayList>();
            Object[] data = response["dataset_data"]["data"][0].ToObject<Object[]>();
            ArrayList dataList = response["dataset_data"]["data"][0].ToObject<ArrayList>();

            int i = 0;
            foreach (string column in columns)
            {
                
                int index = Array.IndexOf(columnNames, column);
                if (index < 0)
                {
                    columnsList.Remove(columns[i]);
                    dataList.Remove(data[i]);
                }

                i++;
            }

            ArrayList result = new ArrayList();
            result.Add(columnsList);
            result.Add(dataList);

            if (columnNames.Length != 0 && columnsList.Count != columnNames.Length)
            {
                throw new Exception("data not found!");
            }

            return result;
        }

        public static void populateData(string code, Excel.Range activeCell, ArrayList data, int rowCount)
        {
            ArrayList columns = (ArrayList)data[0] as ArrayList;
            ArrayList dataList = (ArrayList)data[1] as ArrayList;

            if (rowCount == 1)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    activeCell[rowCount, i + 2].Value2 = columns[i];
                }

            }

            activeCell[rowCount + 1][1].Value2 = code;
            for (int i = 0; i < dataList.Count; i++)
            {
                activeCell[rowCount+1, i + 2].Value2 = dataList[i];
             
            }

        }

        private static string[] convertToArray(JToken tokens)
        {
   
            ArrayList result = new ArrayList();

            for (int i = 0; ; i++)
            {
                result.Add((string)tokens[i]);
                if (tokens[i].Equals(tokens.Last)) { break; };
            }
            return (String[]) result.ToArray(typeof(string));
        }

        //public static int GetExcelVersionNumber()
        //{
        //    Excel.Application excelApp = new Excel.Application();

        //    string versionName = excelApp.Version;
        //    int length = versionName.IndexOf('.');
        //    versionName = versionName.Substring(0, length);

        //    // int.parse needs to be done using US Culture.
        //    return int.Parse(versionName, CultureInfo.GetCultureInfo("en-US"));
        //}
    }
}
