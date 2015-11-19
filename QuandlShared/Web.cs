using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Net;

namespace Quandl.Shared
{
    public class Web
    {
        public static JObject SearchDatabases(string query)
        {
            string requestUri = Properties.Settings.Default.BaseUrl + "databases?per_page=10&query=" + query;
            return GetResponseJson(requestUri);
        }
        public static JObject SearchDatasets(string databaseCode, string query)
        {
            string requestUri = Properties.Settings.Default.BaseUrl + "datasets?database_code=" + databaseCode + "&per_page=10&query=" + query;
            return GetResponseJson(requestUri);
        }

        public static ArrayList PullRecentStockData(string quandlCode, ArrayList columnNames, int limit)
        {
            string extraUri = "&limit=" + limit;
            return GetMatchedData(quandlCode, columnNames, extraUri);
        }

        public static ArrayList PullHistoryData(string quandlCode, string startDate, string endDate, ArrayList columnNames)
        {

            string extraUri = "";

            if (startDate != null && endDate != null)
            {
                extraUri = "start_date=" + startDate + "&end_date=" + endDate;
            }
            else if (startDate != null)
            {
                extraUri = "start_date=" + startDate;
            }
            return GetMatchedData(quandlCode, columnNames, extraUri);
           
        }

        private static ArrayList GetMatchedData(string quandlCode, ArrayList columnNames, string extrUri)
        {
            JObject response = QuandlAPICall(quandlCode, extrUri);

            ArrayList columnsList = response["dataset_data"]["column_names"].ToObject<ArrayList>();
            ArrayList columnsUppercase = new ArrayList();

            foreach (string column in columnsList)
            {
                columnsUppercase.Add(column.ToUpper());
            }
            ArrayList list = response["dataset_data"]["data"].ToObject<ArrayList>();
            ArrayList dataList = new ArrayList();
            foreach (JArray j in list)
            {
                dataList.Add(j.ToObject<ArrayList>());
            }

            return Utilities.GetMatchedListByOrder(columnNames, columnsUppercase, dataList);
        }

        public static string PullSingleValue(string code, string columnName = null, string date = null)
        {
            string api_key = QuandlConfig.ApiKey;
            string requestUri = Properties.Settings.Default.BaseUrl + "datasets/" + code + "/data.json?limit=1" + "&api_key=" + api_key; ;
            if (date != null)
            {
                requestUri += "&start_date=" + date;
            }
            JObject response = GetResponseJson(requestUri);

            ArrayList columnsList = response["dataset_data"]["column_names"].ToObject<ArrayList>();
            ArrayList columnsUppercase = new ArrayList();

            foreach (string column in columnsList)
            {
                columnsUppercase.Add(column.ToUpper());
            }
            ArrayList dataList = response["dataset_data"]["data"][0].ToObject<ArrayList>();
            string data = "";

            int index = 1;
            if (columnName != null)
            {
                index = columnsUppercase.IndexOf(columnName.ToUpper());
            }

            if (index >= 0)
            {
                data = dataList[index].ToString();
            }

            return data;

        }

        public static JObject Post(string requestUri, string body)
        {
            var client = QuandlApiWebClient();
            var response = client.UploadString(requestUri, body);
            return JObject.Parse(response);
        }

        private static JObject GetResponseJson(String requestUri)
        {
            var client = QuandlApiWebClient();
            return JObject.Parse(client.DownloadString(requestUri));
        }

        private static JObject QuandlAPICall(string quandlCode, string extraUri)
        {
            string api_key = QuandlConfig.ApiKey;
            string requestUri = Properties.Settings.Default.BaseUrl + "datasets/" + quandlCode + "/data.json?" + extraUri;
            var client = QuandlApiWebClient();
            return JObject.Parse(client.DownloadString(requestUri));
        }

        private static WebClient QuandlApiWebClient()
        {
            var client = new WebClient
            {
                Headers =
                {
                    ["User-Agent"] = "excel quandl new add-in",
                    ["Request-Source"] = "excel",
                    [HttpRequestHeader.ContentType] = "application/json",
                    [HttpRequestHeader.Accept] = "application/json"
            }
            };
            if (!string.IsNullOrEmpty(QuandlConfig.ApiKey))
            {
                client.Headers["X-API-Token"] = QuandlConfig.ApiKey;
            }
            //  client.Headers["Request-Platform"] = GetExcelVersionNumber().ToString();
            client.Headers["Request-Version"] = "3.0alpha";

            return client;
        }

    }
}
