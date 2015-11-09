using System;
using System.Collections;
using System.Net;
using System.Linq;
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

        public static void populateLatestStockData(string[] quandlCodes, ArrayList columnNames, Excel.Range activeCells)
        {
            // Header
            Excel.Range firstActiveCell = activeCells.get_Offset(0, 0);

            // Data
            int i = 1;
            foreach(string quandlCode in quandlCodes)
            {
                ArrayList convertedData = pullRecentStockData(quandlCode, columnNames, 1);
                populateData(quandlCode.ToUpper(), firstActiveCell, convertedData, i);
                i++;
            }
        }
        
        public static ArrayList pullRecentStockData(string code, ArrayList columnNames, int limit )
        {
            string requestUri = apiUri + "datasets/" + code + "/data.json?limit=" + limit.ToString() + "&api_key=56LY1VVcCDFj1u3J48Kw";
            JObject response =  getResponseJson(requestUri);

            ArrayList columnsList = response["dataset_data"]["column_names"].ToObject<ArrayList>();
            ArrayList columnsUppercase = new ArrayList(); 
            foreach (string column in columnsList)
            {
                columnsUppercase.Add(column.ToUpper());
            }
            ArrayList dataList = response["dataset_data"]["data"][0].ToObject<ArrayList>();
            ArrayList data = new ArrayList();

            int i = 0;
            foreach (string columnName in columnNames)
            {
                int index = columnsUppercase.IndexOf(columnName);
                if (index >= 0)
                {
                    data.Add(dataList[index]);
                }
                else
                {
                    data.Add("");
                }

                i++;
            }

            ArrayList result = new ArrayList();
            result.Add(new ArrayList(columnNames));
            result.Add(data);

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

        public static string[] stringToArray(string input)
        {
            char[] delimiter = new char[] { '[', ',', ']','\'' };
            string[] words = input.Split(delimiter);
            return words.Where( x => !string.IsNullOrEmpty(x)).ToArray();
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
