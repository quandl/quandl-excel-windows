using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;
using Quandl.Shared.Errors;
using Quandl.Shared.Properties;

namespace Quandl.Shared
{
    public class QuandlConfig
    {
        public delegate void LoginChangedHandler();

        public enum AutoUpdateFrequencies
        {
            [Description("Disabled")]
            Disabled = -1,
            [Description("One Day")]
            OneDay = 1,
            [Description("Seven Days")]
            SevenDays = 7
        }

        private const string RegistrySubKey = @"SOFTWARE\Quandl\Excel Add-in";

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

        public static bool IgnoreMissingFormulaParams
        {
            get { return GetRegistry<bool>("IgnoreMissingFormulaParams"); }
            set
            {
                SetRegistryKeyValue("IgnoreMissingFormulaParams", value, RegistryValueKind.DWord);
            }
        }

        public static bool StopCurrentExecution
        {
            get { return GetRegistry<bool>("StopCurrentExecution"); }
            set
            {
                SetRegistryKeyValue("StopCurrentExecution", value, RegistryValueKind.DWord);
            }
        }

        public static bool PreventCurrentExecution {
            get { return GetRegistry<bool>("PreventExecution"); }
            set
            {
                SetRegistryKeyValue("PreventExecution", value, RegistryValueKind.DWord);
            }
        }

        private AutoUpdateFrequencies autoUpdateFrequency
        {
            get { return GetRegistry<AutoUpdateFrequencies>("AutoUpdateFrequency"); }
            set
            {
                SetRegistryKeyValue("AutoUpdateFrequency", value, RegistryValueKind.DWord);
                OnAutoUpdateFrequencyChanged();
            }
        }

        // In days: 1 day or 7 days or never (0 days)
        public static AutoUpdateFrequencies AutoUpdateFrequency
        {
            get { return Instance.autoUpdateFrequency; }
            set { Instance.autoUpdateFrequency = value; }
        }

        public static int AutoUpdateFrequencyDays => (int)AutoUpdateFrequency;

        private string apiKey
        {
            get { return GetRegistry<string>("ApiKey"); }
            set
            {
                SetRegistryKeyValue("ApiKey", value);
                OnLoginChanged();
            }
        }

        public static string ApiKey
        {
            get { return Instance.apiKey; }
            set { Instance.apiKey = value; }
        }

        public static bool AutoUpdate => AutoUpdateFrequency != 0;

        public event LoginChangedHandler LoginChanged;
        public event LoginChangedHandler AutoUpdateFrequencyChanged;

        public static async Task<bool> ApiKeyValid(string apiKey = null)
        {
            if (apiKey == null)
            {
                apiKey = ApiKey;
            }

            if (string.IsNullOrEmpty(apiKey))
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
            var requestUri = Settings.Default.BaseUrl + "users/token_auth";
            var res = Web.Post(requestUri, payload);
            Instance.apiKey = res["user"]["api_key"].ToObject<string>();
        }

        public static void Reset()
        {
            Registry.CurrentUser.DeleteSubKeyTree(RegistrySubKey);
        }

        private static void SetRegistryKeyValue(string key, object value,
            RegistryValueKind regValueKing = RegistryValueKind.String)
        {
            var appKeyPath = Registry.CurrentUser.CreateSubKey(RegistrySubKey);
            appKeyPath.SetValue(key, value, regValueKing);
            appKeyPath.Close();
        }

        private static T GetRegistry<T>(string key)
        {
            var quandlRootKey = Registry.CurrentUser.OpenSubKey(RegistrySubKey);
            if (quandlRootKey != null)
            {
                if (typeof(T) == typeof(bool))
                {
                    return (T)(object)((int)quandlRootKey.GetValue(key, default(int)) == 1);
                }
                else
                {
                    return (T)quandlRootKey.GetValue(key, default(T));
                }
            }

            return default(T);
        }

        protected virtual void OnLoginChanged()
        {
            LoginChanged?.Invoke();
        }

        protected virtual void OnAutoUpdateFrequencyChanged()
        {
            AutoUpdateFrequencyChanged?.Invoke();
        }
    }
}