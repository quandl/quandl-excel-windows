using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quandl.Shared.errors;
using Quandl.Shared.models;
using Quandl.Shared.Properties;

namespace Quandl.Shared
{
    public class Web
    {
        public static async Task<User> WhoAmI(string api_key)
        {
            var queryHeaders = new Dictionary<string, string>
            {
                {"X-API-Token", api_key}
            };

            var userResponse = await RequestAsync<UserResponse>("users/me", null, queryHeaders);
            return userResponse.user;
        }

        public static async Task<DatabaseCollection> SearchDatabasesAsync(string query)
        {
            var queryParams = new Dictionary<string, string>
            {
                {"per_page", "10"},
                {"query", query}
            };
            return await RequestAsync<DatabaseCollection>("databases", queryParams);
        }

        public static async Task<DatasetCollection> SearchDatasetsAsync(string databaseCode, string query)
        {
            var requestUri = Settings.Default.BaseUrl + "datasets?database_code=" + databaseCode + "&per_page=10&query=" +
                             query;

            var queryParams = new Dictionary<string, string>
            {
                {"database_code", databaseCode},
                {"per_page", "10"},
                {"query", query}
            };
            return await RequestAsync<DatasetCollection>("datasets", queryParams);
        }

        public static ArrayList PullRecentStockData(string quandlCode, ArrayList columnNames, int limit)
        {
            var extraUri = "&limit=" + limit;
            return GetMatchedData(quandlCode, columnNames, extraUri);
        }

        public static ArrayList PullHistoryData(string quandlCode, string startDate, string endDate,
            ArrayList columnNames)
        {
            var extraUri = "";

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
            var response = QuandlAPICall(quandlCode, extrUri);

            var columnsList = response["dataset_data"]["column_names"].ToObject<ArrayList>();
            var columnsUppercase = new ArrayList();

            foreach (string column in columnsList)
            {
                columnsUppercase.Add(column.ToUpper());
            }
            var list = response["dataset_data"]["data"].ToObject<ArrayList>();
            var dataList = new ArrayList();
            foreach (JArray j in list)
            {
                dataList.Add(j.ToObject<ArrayList>());
            }

            return Utilities.GetMatchedListByOrder(columnNames, columnsUppercase, dataList);
        }

        public static string PullSingleValue(string code, string columnName = null, string date = null)
        {
            var api_key = QuandlConfig.ApiKey;
            var requestUri = Settings.Default.BaseUrl + "datasets/" + code + "/data.json?limit=1" + "&api_key=" +
                             api_key;
            if (date != null)
            {
                requestUri += "&start_date=" + date + "&end_date=" + date;
            }
            var response = GetResponseJson(requestUri);

            var columnsList = response["dataset_data"]["column_names"].ToObject<ArrayList>();
            var columnsUppercase = new ArrayList();

            foreach (string column in columnsList)
            {
                columnsUppercase.Add(column.ToUpper());
            }
            var dataList = response["dataset_data"]["data"][0].ToObject<ArrayList>();
            var data = "";

            var index = 1;
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

        private static JObject GetResponseJson(string requestUri)
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
            var requestUri = Settings.Default.BaseUrl + "datasets/" + quandlCode + "/data.json?" + extraUri;
            var client = QuandlApiWebClient();
            var resp = client.DownloadString(requestUri);
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new SnakeCaseMappingResolver()
            };
            var foo = JsonConvert.DeserializeObject<DatasetDataResponse>(resp, settings);
            return JObject.Parse(resp);
        }

        private static WebClient QuandlApiWebClient(string type = "(Search/Guide)")
        {
            var client = new WebClient
            {
                Headers =
                {
                    [HttpRequestHeader.Accept] = "application/json",
                    [HttpRequestHeader.ContentType] = "application/json",
                    [HttpRequestHeader.UserAgent] = $"QuandlExcelAddIn/3.0 {type}",
                    ["Request-Source"] = "excel",
                    ["Request-Platform"] = Utilities.GetExcelVersionNumber,
                    ["Request-Version"] = "3.0beta"
                }
            };
            if (!string.IsNullOrEmpty(QuandlConfig.ApiKey))
            {
                client.Headers["X-API-Token"] = QuandlConfig.ApiKey;
            }

            return client;
        }

        private static async Task<T> RequestAsync<T>(string relativeUrl, Dictionary<string, string> queryParams = null,
            Dictionary<string, string> headers = null)
        {
            string type = "(Search/Guide)";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Settings.Default.BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.UserAgent.Clear();
                client.DefaultRequestHeaders.Add("User-Agent", $"QuandlExcelAddIn/3.0 {type}");
                client.DefaultRequestHeaders.Add("Request-Platform", Utilities.GetExcelVersionNumber);
                client.DefaultRequestHeaders.Add("Request-Version", "3.0alpha");
                client.DefaultRequestHeaders.Add("Request-Source", "excel");
                if (!string.IsNullOrEmpty(QuandlConfig.ApiKey) && (headers == null || !headers.ContainsKey("X-API-Token")))
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
                    relativeUrl = relativeUrl + "?" + query;
                }

                HttpResponseMessage resp = null;
                resp = await client.GetAsync(relativeUrl);
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    throw new QuandlErrorBase(resp.StatusCode);
                } 

                var data = await resp.Content.ReadAsStringAsync();
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new SnakeCaseMappingResolver()
                };
                return JsonConvert.DeserializeObject<T>(data, settings);
            }
        }
    }
}