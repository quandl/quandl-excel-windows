using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quandl.Shared.models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Quandl.Shared
{
    public class Web
    {
        public static async Task<DatabaseCollection> SearchDatabasesAsync(string query)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "per_page", "10" },
                { "query", query }
            };
            var resp = await RequestAsync<DatabaseCollection>("databases", queryParams);
            return resp;
        }
        public static async Task<DatasetCollection> SearchDatasetsAsync(string databaseCode, string query)
        {
            string requestUri = Properties.Settings.Default.BaseUrl + "datasets?database_code=" + databaseCode + "&per_page=10&query=" + query;

            var queryParams = new Dictionary<string, string>
            {
                { "database_code", databaseCode },
                { "per_page", "10" },
                { "query", query }
            };
            var resp = await RequestAsync<DatasetCollection>("datasets", queryParams);
            return resp;
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
            string requestUri = Quandl.Shared.Properties.Settings.Default.BaseUrl + "datasets/" + code + "/data.json?limit=1" + "&api_key=" + api_key;
            if (date != null)
            {
                requestUri += "&start_date=" + date + "&end_date=" + date;
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
            var resp = client.DownloadString(requestUri);
            return JObject.Parse(resp);
        }

        private static T GetResponseJson<T>(string requestUri)
        {
            var client = QuandlApiWebClient();
            var resp = client.DownloadString(requestUri);
            return JsonConvert.DeserializeObject<T>(resp);
        }

        private static JObject QuandlAPICall(string quandlCode, string extraUri)
        {
            string requestUri = Quandl.Shared.Properties.Settings.Default.BaseUrl + "datasets/" + quandlCode + "/data.json?" + extraUri;
            var client = QuandlApiWebClient();
            var resp = client.DownloadString(requestUri);
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new SnakeCaseMappingResolver()
            };
            var foo = JsonConvert.DeserializeObject<DatasetDataResponse>(resp, settings);
            return JObject.Parse(resp);
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

        private static async Task<T> RequestAsync<T>(string relativeUrl, Dictionary<string, string> queryParams = null, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Quandl.Shared.Properties.Settings.Default.BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Request-Version", "3.0alpha");
                client.DefaultRequestHeaders.Add("Request-Source", "excel");
                if (!string.IsNullOrEmpty(QuandlConfig.ApiKey))
                {
                    client.DefaultRequestHeaders.Add("X-API-Token", QuandlConfig.ApiKey);
                }

                if (headers != null)
                {
                    foreach (var h in headers)
                    {
                        client.DefaultRequestHeaders.Add(h.Key, h.Value);
                    }
                }

                if (queryParams != null)
                {
                    var query = HttpUtility.ParseQueryString(string.Empty);
                    foreach (var queryParam in queryParams)
                    {
                        query[queryParam.Key] = queryParam.Value;
                    }
                    relativeUrl = relativeUrl + "?" + query.ToString();
                }

                HttpResponseMessage resp = null;

                try
                {
                    resp = await client.GetAsync(relativeUrl);
                    resp.EnsureSuccessStatusCode();
                }
                catch(HttpRequestException e)
                {
                    Console.WriteLine("Hello world!");
                }

                string data = await resp.Content.ReadAsStringAsync();
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new SnakeCaseMappingResolver()
                };
                return JsonConvert.DeserializeObject<T>(data, settings);
            }
        }
    }
}
