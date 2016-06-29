using System;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Net;
using Quandl.Shared.errors;

namespace Quandl.Shared
{
    public class QuandlConfig
    {
        private const string RegistrySubKey = @"SOFTWARE\Quandl\ExcelAddin";

        private static QuandlConfig instance;
        public static QuandlConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new QuandlConfig();
                }
                return instance;
            }
        }

        public delegate void LoginChangedHandler();
        public event LoginChangedHandler LoginChanged;

        private string apiKey {
            get { return GetRegistry<string>("ApiKey"); }
            set { SetRegistryKeyValue("ApiKey", value); OnLoginChanged(); }
        }

        public static string ApiKey
        {
            get { return Instance.apiKey; }
            set { Instance.apiKey = value; }
        }

        public static async Task<bool> ApiKeyValid(string apiKey = null) {
            if (apiKey == null)
            {
                apiKey = ApiKey;
            }

            if (String.IsNullOrEmpty(apiKey))
            {
                return false;
            }

            try
            {
                var user = await Web.WhoAmI(apiKey);
                return user != null && user.ApiKey == apiKey;
            }
            catch (QuandlErrorBase exp)
            {
                if (exp.StatusCode == HttpStatusCode.BadRequest)
                {
                    return false;
                }
                throw exp; // Not what we were expecting so throw an error.
            }
        }

        public static void AuthenticateWithCredentials(string accountName, string pass)
        {
            var obj = new { user = new { account = accountName, password = pass } };
            var payload = JsonConvert.SerializeObject(obj);
            var requestUri = Quandl.Shared.Properties.Settings.Default.BaseUrl + "users/token_auth";
            var res = Web.Post(requestUri, payload);
            Instance.apiKey = res["user"]["api_key"].ToObject<string>();
        }

        public static bool AutoUpdate
        {
            get { return Convert.ToBoolean(GetRegistry<int>("AutoUpdate")); }
            set { SetRegistryKeyValue("AutoUpdate", Convert.ToInt32(value), RegistryValueKind.DWord); }
        }

        // In days: 1 day or 7 days or never (0 days)
        public static int AutoUpdateFrequency
        {
            get { return GetRegistry<int>("AutoUpdateFrequency"); }
            set { SetRegistryKeyValue("AutoUpdateFrequency", value, RegistryValueKind.DWord); }
        }

        public static void Reset()
        {
            Registry.CurrentUser.DeleteSubKeyTree(RegistrySubKey);
        }

        private static void SetRegistryKeyValue(string key, object value, RegistryValueKind regValueKing = RegistryValueKind.String)
        {
            var appKeyPath = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RegistrySubKey);
            var apiSubKey = appKeyPath.CreateSubKey(key);
            apiSubKey.SetValue(key, value, regValueKing);
            apiSubKey.Close();
        }

        private static T GetRegistry<T>(string key)
        {
            var quandlRootKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegistrySubKey);
            if (quandlRootKey != null)
            {
                var subKey = quandlRootKey.OpenSubKey(key);
                if (subKey != null)
                {
                    return (T)subKey.GetValue(key, default(T));
                }
            }

            return default(T);
        }

        protected virtual void OnLoginChanged()
        {
            LoginChanged?.Invoke();
        }
    }
}
