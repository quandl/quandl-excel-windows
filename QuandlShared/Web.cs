using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quandl.Shared.Errors;
using Quandl.Shared.models;
using Quandl.Shared.Properties;

namespace Quandl.Shared
{
    public class Web
    {
        private enum CallTypes { Search, Data };

        public static async Task<User> WhoAmI(string api_key)
        {
            var queryHeaders = new Dictionary<string, string>
            {
                {"X-API-Token", api_key}
            };

            var userResponse = await RequestAsync<UserResponse>("users/me", CallTypes.Search, null, queryHeaders);
            return userResponse.user;
        }

        public static async Task<DatabaseCollection> SearchDatabasesAsync(string query)
        {
            var queryParams = new Dictionary<string, object>
            {
                {"per_page", "10"},
                {"query", query}
            };
            return await RequestAsync<DatabaseCollection>("databases", CallTypes.Search, queryParams);
        }


        public static async Task<DatasetCollection> SearchDatasetsAsync(string databaseCode, string query)
        {
            var queryParams = new Dictionary<string, object>
            {
                {"database_code", databaseCode},
                {"per_page", "10"},
                {"query", query}
            };
            return await RequestAsync<DatasetCollection>("datasets", CallTypes.Search, queryParams);
        }

        public static async Task<BrowseCollection> BrowseAsync()
        {
            var headers = new Dictionary<string, string>
            {
                //{"Request-Source", "next"},
                {"X-Requested-With", "XMLHttpRequest"}
            };

            var queryParams = new Dictionary<string, object>
            {
                {"keys[]", "browse"}
            };

            var resp = await RequestAsync<NamedContentCollection>("named_contents", CallTypes.Search, queryParams, headers);
            var namedContent = resp.NamedContents.FirstOrDefault();
            var browseJson = namedContent.HtmlContent;
            var browse = JsonConvert.DeserializeObject<BrowseCollection>(browseJson, JsonSettings());
            return browse;
        }

        public async static Task<List<List<object>>> PullRecentStockData(string quandlCode, List<string> columnNames, int limit)
        {
            var queryParams = new Dictionary<string, object>
            {
                { "limit", limit.ToString() },
                { "column_index", columnNames }
            };
            return await GetMatchedData(quandlCode, queryParams);
        }

        public async static Task<List<List<object>>> PullHistoryData(string quandlCode, string startDate, string endDate, List<string> columnNames)
        {

            var queryParams = new Dictionary<string, object>
            {
                { "start_date", startDate },
                { "end_date", endDate },
                { "column_index", columnNames }
            };
            return await GetMatchedData(quandlCode, queryParams);
        }

        private async static Task<List<List<object>>> GetMatchedData(string quandlCode, Dictionary<string, object> queryParams)
        {
            var relativeUrl = "datasets/" + quandlCode + "/data";
            var resp = await RequestAsync<DatasetDataResponse>(relativeUrl, CallTypes.Data, queryParams);
            return resp.DatasetData.Data;
        }

        public async static Task<string> PullSingleValue(string code, string columnName, string date)
        {
            var data = "";
            var queryParams = new Dictionary<string, object>
            {
                { "limit", "1" }
            };

            if (date != null)
            {
                queryParams["start_date"] = date;
                queryParams["end_date"] = date;
            }
            queryParams["column_index"] = columnName;

            var relativeUrl = "datasets/" + code + "/data";
            var resp = await RequestAsync<DatasetDataResponse>(relativeUrl, CallTypes.Data, queryParams);
            var dataRow = resp.DatasetData.Data.FirstOrDefault();
            if (dataRow.Count > 1)
            {
                data = dataRow[1].ToString();
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

        public static async Task<T> GetResponseJson<T>(string requestParams)
        {
            var client = new WebClient
            {
                Headers =
                {
                    [HttpRequestHeader.Accept] = "application/json",
                    [HttpRequestHeader.ContentType] = "application/json",
                    ["Request-Source"] = "next",
                    ["X-Requested-With"] = "XMLHttpRequest",
                }
            };
            string requestUri = Settings.Default.BaseUrl + requestParams;
            var resp = await client.DownloadStringTaskAsync(requestUri);
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new SnakeCaseMappingResolver()
                
            };
            settings.NullValueHandling = NullValueHandling.Ignore;
            return JsonConvert.DeserializeObject<T>(resp, settings);
        }

        public static async Task<Database> GetDatabase(string code)
        {
            string requestParams = "databases/" + code;
            return await Web.GetResponseJson<Database>(requestParams);
        }

        public static async Task<DatatableCollection> GetDatatableCollection(string code)
        {
            string requestParams = "datatable_collections/" + code;
            return await Web.GetResponseJson<DatatableCollection>(requestParams);
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

        private static WebClient QuandlApiWebClient(CallTypes callType = CallTypes.Data)
        {
            var client = new WebClient
            {
                Headers =
                {
                    [HttpRequestHeader.Accept] = "application/json",
                    [HttpRequestHeader.ContentType] = "application/json",
                    [HttpRequestHeader.UserAgent] = $"QuandlExcelAddIn/3.0 {CallTypeMapper(callType)}",
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

        private static string ConvertListToQueryString(string key, List<string> queryValues)
        {
            var query = queryValues.Select(x => key + "[]=" + x);
            var queryString = string.Join("&", query); 
            return queryString;
        }

        private static async Task<T> RequestAsync<T>(string relativeUrl, CallTypes callType = CallTypes.Data, Dictionary<string, object> queryParams = null,
            Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Settings.Default.BaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.UserAgent.Clear();
                client.DefaultRequestHeaders.UserAgent.ParseAdd($"QuandlExcelAddIn/3.0 {CallTypeMapper(callType)}");
                client.DefaultRequestHeaders.Add("Request-Platform", Utilities.GetExcelVersionNumber);
                client.DefaultRequestHeaders.Add("Request-Version", "3.0beta");
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
                    var queryString = "?";
                    foreach (var queryParam in queryParams)
                    {
                        if (queryParam.Value == null)
                        {
                            continue;
                        }
                        if (queryParam.Value is IList && queryParam.Value.GetType().IsGenericType)
                        {
                            queryString += ConvertListToQueryString(queryParam.Key, (List<string>)queryParam.Value) + "&";
                        }
                        else
                        {
                            queryString += queryParam.Key + "=" + queryParam.Value.ToString() + "&";
                        }
                    }
                    relativeUrl = relativeUrl + queryString;
                }

                HttpResponseMessage resp = null;
                resp = await client.GetAsync(relativeUrl);
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    throw new QuandlErrorBase(resp.StatusCode);
                } 

                var data = await resp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(data, JsonSettings());
            }
        }

        private static JsonSerializerSettings JsonSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new SnakeCaseMappingResolver()
            };
        }
        private static string CallTypeMapper(CallTypes callType)
        {
            return (callType == CallTypes.Data ? "(Data)" : "(Search/Guide)");
        }
    }
}