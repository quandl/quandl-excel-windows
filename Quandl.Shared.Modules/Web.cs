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


        public static async Task<DatasetsResponse> SearchDatasetsAsync(string databaseCode, string query, int page,
            int perPage)
        {
            var queryParams = new Dictionary<string, object>
            {
                {"database_code", databaseCode},
                {"page", page.ToString()},
                {"per_page", perPage.ToString()},
                {"query", query}
            };
            return await RequestAsync<DatasetsResponse>("datasets", CallTypes.Data, queryParams);
        }

        public static async Task<DatasetResponse> SearchDatasetAsync(string datasetCode)
        {
            string relativeUrl = $"datasets/{datasetCode}/metadata";
            return await RequestAsync<DatasetResponse>(relativeUrl, CallTypes.Data);
        }

        public static async Task<BrowseCollection> BrowseAsync()
        {
            var queryParams = new Dictionary<string, object>
            {
                {"keys[]", "browse"}
            };

            var resp =
                await RequestAsync<NamedContentCollection>("named_contents", CallTypes.Search, queryParams, null);
            var namedContent = resp.NamedContents.FirstOrDefault();
            var browseJson = namedContent.HtmlContent;
            var browse = JsonConvert.DeserializeObject<BrowseCollection>(browseJson, JsonSettings());
            return browse;
        }

        public static async Task<Dataset> GetDatasetData(string quandlCode, Dictionary<string, object> queryParams)
        {
            var relativeUrl = "datasets/" + quandlCode + "/data";
            var resp = await RequestAsync<DataArray>(relativeUrl, CallTypes.Data, queryParams);
            var dataset = new Dataset {Data = resp, Columns = resp.Columns};
            dataset.DatabaseCode = quandlCode.Split('/')[0];
            dataset.DatasetCode = quandlCode.Split('/')[1];
            return dataset;
        }

        public static async Task<DatasetMetaResponse> GetDatasetMetadata(string quandlCode)
        {
            var relativeUrl = "datasets/" + quandlCode + "/metadata";
            var resp = await RequestAsync<DatasetMetaResponse>(relativeUrl, CallTypes.Data);
            return resp;
        }

        public static async Task<Datatable> GetDatatableData(string quandlCode, Dictionary<string, object> queryParams)
        {
            var relativeUrl = "datatables/" + quandlCode;
            var resp = await RequestAsync<DataArray>(relativeUrl, CallTypes.Data, queryParams);
            var datatable = new Datatable { Data = resp, Columns = resp.Columns };
            datatable.VendorCode = quandlCode.Split('/')[0];
            datatable.DatatableCode = quandlCode.Split('/')[1];
            return datatable;
        }

        public static JObject Post(string requestUri, string body)
        {
            var client = QuandlApiWebClient();
            var response = client.UploadString(requestUri, body);
            return JObject.Parse(response);
        }

        public static async Task<T> GetResponseJson<T>(string requestParams)
        {
            var client = new WebClient();
            var requestUri = Settings.Default.BaseUrl + requestParams;
            var resp = await client.DownloadStringTaskAsync(requestUri);
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new SnakeCaseMappingResolver()
            };
            return JsonConvert.DeserializeObject<T>(resp, settings);
        }

        public static async Task<T> GetModelByIds<T>(string type, List<string> ids) where T : class, new()
        {
            if (ids.Count.Equals(0))
            {
                return new T();
            }
            var queryParams = new Dictionary<string, object> {{"ids", ids}};
            var callType = type.Equals("databases") ? CallTypes.Data : CallTypes.Search;
            return await RequestAsync<T>(type, callType, queryParams);
        }

        public static async Task<T> GetDatabase<T>(string code)
        {
            var relativeUrl = "databases/" + code;
            return await RequestAsync<T>(relativeUrl, CallTypes.Data, null);
        }

        public static async Task<T> GetDatatableCollection<T>(string code)
        {
            var relativeUrl = "datatable_collections/" + code;
            return await RequestAsync<T>(relativeUrl, CallTypes.Search, null);
        }

        public static async Task<DatatableMetadata> GetDatatableMetadata(string code)
        {
            string relativeUrl = "datatables/" + code + "/metadata";
            return await RequestAsync<DatatableMetadata>(relativeUrl, CallTypes.Data, null);
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
            var query = queryValues.Select(x => key + "[]=" + Uri.EscapeDataString(x));
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

                if (!string.IsNullOrEmpty(QuandlConfig.ApiKey) &&
                    (headers == null || !headers.ContainsKey("X-API-Token")))
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
                    relativeUrl = relativeUrl + "?" + StringifyQueryParams(queryParams);
                }

                var resp = await client.GetAsync(relativeUrl);
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    throw new QuandlErrorBase(resp.StatusCode);
                }

                var data = await resp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(data, JsonSettings());
            }
        }

        private static string StringifyQueryParams(Dictionary<string, object> queryParams)
        {
            var queryArr = new List<string>();

            foreach (var queryParam in queryParams)
            {
                if (queryParam.Value == null)
                {
                    continue;
                }
                if (queryParam.Value is IList && queryParam.Value.GetType().IsGenericType)
                {
                    queryArr.Add(ConvertListToQueryString(queryParam.Key, (List<string>) queryParam.Value));
                }
                else
                {
                    queryArr.Add(queryParam.Key + "=" + Uri.EscapeDataString(queryParam.Value.ToString()));
                }
            }

            return string.Join("&", queryArr);
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