using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quandl.Shared.Errors;
using Quandl.Shared.Models;
using Quandl.Shared.Models.Browse;
using Quandl.Shared.Properties;

namespace Quandl.Shared
{
    public class Web
    {
        public static async Task<OldUser> WhoAmI(string api_key)
        {
            var queryHeaders = new Dictionary<string, string>
            {
                {"X-API-Token", api_key}
            };

            var userResponse = await RequestAsync<OldUserResponse>("users/me", CallTypes.Search, null, queryHeaders);
            return userResponse.user;
        }

        public static async Task<OldDatabaseCollection> SearchDatabasesAsync(string query)
        {
            var queryParams = new Dictionary<string, object>
            {
                {"per_page", "10"},
                {"query", query}
            };
            return await RequestAsync<OldDatabaseCollection>("databases", CallTypes.Search, queryParams);
        }


        public static async Task<DatasetsResponse> SearchDatasetsAsync(string databaseCode, string query, int page, int perPage)
        {
            var queryParams = new Dictionary<string, object>
            {
                { "database_code", databaseCode },
                { "page", page.ToString() },
                { "per_page", perPage.ToString() },
                { "query", query }
            };
            return await RequestAsync<DatasetsResponse>("datasets", CallTypes.Data, queryParams);
        }

        public static async Task<DatasetResponse> SearchDatasetAsync(string datasetCode)
        {
            string relativeUrl = $"datasets/{datasetCode}";
            return await RequestAsync<DatasetResponse>(relativeUrl, CallTypes.Data);
        }

        public static async Task<BrowseCollection> BrowseAsync()
        { 
            var queryParams = new Dictionary<string, object>
            {
                {"keys[]", "browse"}
            };

            var resp = await RequestAsync<OldNamedContentCollection>("named_contents", CallTypes.Search, queryParams, null);
            var namedContent = resp.NamedContents.FirstOrDefault();
            var browseJson = namedContent.HtmlContent;
            var browse = JsonConvert.DeserializeObject<BrowseCollection>(browseJson, JsonSettings());
            return browse;
        }

        public static async Task<List<List<object>>> PullRecentStockData(string quandlCode, List<string> columnNames,
            int limit)
        {
            var queryParams = new Dictionary<string, object>
            {
                {"limit", limit.ToString()},
                {"column_index", columnNames}
            };
            return await GetMatchedData(quandlCode, queryParams);
        }

        public static async Task<List<List<object>>> PullHistoryData(string quandlCode, string startDate, string endDate,
            List<string> columnNames)
        {
            var queryParams = new Dictionary<string, object>
            {
                {"start_date", startDate},
                {"end_date", endDate},
                {"column_index", columnNames}
            };
            return await GetMatchedData(quandlCode, queryParams);
        }

        private static async Task<List<List<object>>> GetMatchedData(string quandlCode,
            Dictionary<string, object> queryParams)
        {
            var relativeUrl = "datasets/" + quandlCode + "/data";
            var resp = await RequestAsync<DatasetDataResponse>(relativeUrl, CallTypes.Data, queryParams);
            return resp.DatasetData.Data;
        }

        public static async Task<string> PullSingleValue(string code, string columnName, string date)
        {
            var data = "";
            var queryParams = new Dictionary<string, object>
            {
                {"limit", "1"}
            };

            if (date != null)
            {
                queryParams["start_date"] = date;
                queryParams["end_date"] = date;
            }
            queryParams["column_index"] = columnName;

            var relativeUrl = "datasets/" + code + "/data";
            var resp = await RequestAsync<DatasetDataResponse>(relativeUrl, CallTypes.Search, queryParams);
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
            var client = new WebClient();
            string requestUri = Settings.Default.BaseUrl + requestParams;
            var resp = await client.DownloadStringTaskAsync(requestUri);
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new SnakeCaseMappingResolver()
                
            };
            return JsonConvert.DeserializeObject<T>(resp, settings);
        }

        public static async Task<T> GetModelByIds<T>(string type, List<string> ids) where T: class, new()
        {
            if (ids.Count.Equals(0))
            {
                return new T();
            }
            var queryParams = new Dictionary<string, object>
            {
                { "ids", ids }
            };
            CallTypes callType;
            if (type.Equals("databases"))
            {
                callType = CallTypes.Data;
            }
            else
            {
                callType = CallTypes.Search;
            }
            return await RequestAsync<T>(type, callType, queryParams);
        }

        public static async Task<T> GetDatabase<T>(string code)
        {
            string relativeUrl = "databases/" + code;
            return await RequestAsync<T>(relativeUrl, CallTypes.Data, null);

        }

        public static async Task<T> GetDatatableCollection<T>(string code)
        {
            string relativeUrl = "datatable_collections/" + code;
            return await RequestAsync<T>(relativeUrl, CallTypes.Search, null);
        }

        /*
        public static async Task<DatatableMetadata> GetDatatableMetadata(string vendorCode, string datatableCode)
        {
            string relativeUrl = "datatables/" + vendorCode + "/" + datatableCode + "/metadata";
            return await RequestAsync<DatatableMetadata>(relativeUrl, CallTypes.Data, null);
        }
        */

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

        private static async Task<T> RequestAsync<T>(string relativeUrl, CallTypes callType = CallTypes.Data,
            Dictionary<string, object> queryParams = null,
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
                            queryString += ConvertListToQueryString(queryParam.Key, (List<string>) queryParam.Value) +
                                           "&";
                        }
                        else
                        {
                            queryString += queryParam.Key + "=" + queryParam.Value + "&";
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
            return callType == CallTypes.Data ? "(Data)" : "(Search/Guide)";
        }

        private enum CallTypes
        {
            Search,
            Data
        }
    }
}