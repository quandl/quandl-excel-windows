using System;
using System.Net;
using Newtonsoft.Json.Linq;

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
    }
}
