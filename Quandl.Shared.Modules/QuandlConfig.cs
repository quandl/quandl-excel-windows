using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;
using Quandl.Shared.Errors;
using System.Linq;
using System.Security.RightsManagement;

namespace Quandl.Shared
{
    public class QuandlConfig
    {
        public delegate void LoginChangedHandler();

        public enum AutoUpdateFrequencies
        {
            [Description("Disabled")]
            Disabled = -1,
            [Description("On Workbook Open")]
            WorkbookOpen = 1
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

        public static bool ScrollOnInsert
        {
            get { return RegistryKeyExists("ScrollOnInsert") ? GetRegistry<bool>("ScrollOnInsert") : false; }
            set
            {
                SetRegistryKeyValue("ScrollOnInsert", value, RegistryValueKind.DWord);
            }
        }

        public static bool LongRunningQueryWarning
        {
            get { return RegistryKeyExists("LongRunningQueryWarning") ? GetRegistry<bool>("LongRunningQueryWarning") : true; }
            set
            {
                SetRegistryKeyValue("LongRunningQueryWarning", value, RegistryValueKind.DWord);
            }
        }

        public static bool OverwriteDataWarning
        {
            get { return RegistryKeyExists("OverwriteDataWarning") ? GetRegistry<bool>("OverwriteDataWarning") : true; }
            set
            {
                SetRegistryKeyValue("OverwriteDataWarning", value, RegistryValueKind.DWord);
            }
        }

        public static bool CheckUpdateAtStart
        {
            get { return RegistryKeyExists("CheckUpdateAtStart") ? GetRegistry<bool>("CheckUpdateAtStart") : true; }
            set
            {
                SetRegistryKeyValue("CheckUpdateAtStar", value, RegistryValueKind.DWord);
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

        public static bool PreventCurrentExecution
        {
            get { return GetRegistry<bool>("PreventExecution"); }
            set
            {
                SetRegistryKeyValue("PreventExecution", value, RegistryValueKind.DWord);
            }
        }

        private AutoUpdateFrequencies autoUpdateFrequency
        {
            get { return RegistryKeyExists("AutoUpdateFrequency") ? GetRegistry<AutoUpdateFrequencies>("AutoUpdateFrequency") : AutoUpdateFrequencies.Disabled; }
            set
            {
                SetRegistryKeyValue("AutoUpdateFrequency", value, RegistryValueKind.DWord);
                
            }
        }

        private const string EnableFloatingTaskPaneNameKey = nameof(EnableFloatingTaskPane);
        public static bool EnableFloatingTaskPane
        {
            get
            {
                try
                {
                    if (RegistryKeyExists(EnableFloatingTaskPaneNameKey))
                    {
                        return GetRegistry<bool>(EnableFloatingTaskPaneNameKey);
                    }
                }
                catch
                {
                }
                return false;
            }
        }
        public static AutoUpdateFrequencies AutoUpdateFrequency
        {
            get { return Instance.autoUpdateFrequency; }
            set { Instance.autoUpdateFrequency = value; }
        }

        private string apiKey
        {
            get { return GetRegistry<string>("ApiKey"); }
            set { SetRegistryKeyValue("ApiKey", value); }
        }

        private string apiHost
        {
            get { return GetRegistry<string>("ApiHost"); }
            set { SetRegistryKeyValue("ApiHost", value); }
        }

        private string userRole
        {
            get { return GetRegistry<string>("UserRole"); }
            set
            {
                SetRegistryKeyValue("UserRole", value);
            }
        }

        public static string ApiKey
        {
            get { return Instance.apiKey; }
            set { Instance.apiKey = value; }
        }

        public static string ApiHost
        {
            get { return Instance.apiHost; }
            set { Instance.apiHost = value; }
        }

        public static string UserRole
        {
            get { return Instance.userRole; }
            set { Instance.userRole = value; }
        }

        private static bool RegistryKeyExists(string key)
        {
            var quandlRootKey = Registry.CurrentUser.OpenSubKey(RegistrySubKey);
            return quandlRootKey != null && quandlRootKey.GetValueNames().Contains(key);
        }

        
        
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
                var user = await new Web().WhoAmI(apiKey);
                var isValid = (user != null) && (user.ApiKey == apiKey);
                Instance.userRole = isValid ? user.UserRole : "";
                return isValid;
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

        public static void AuthenticateWithCredentials(Web web, string accountName, string pass)
        {
            var obj = new { user = new { account = accountName, password = pass } };
            var payload = JsonConvert.SerializeObject(obj);
            var res = web.Authenticate(payload);
            Instance.apiKey = res["user"]["api_key"].ToObject<string>();
            Instance.userRole = Utilities.GetUserRole(res["user"]);
        }

        public bool IsOnlyUser()
        {
            return Instance.apiKey == null ||
                   Instance.apiKey.Trim().Equals("") ||
                   Instance.userRole == null ||
                   string.Equals(Instance.userRole, Utilities.UserRoles.User.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }

        public static void Reset()
        {
            Registry.CurrentUser.DeleteSubKeyTree(RegistrySubKey);
        }

        public static void DeleteApiHost()
        {
            DeleteRegistryKeyValue("ApiHost");
        }

        public static void DeleteApiKey()
        {
            DeleteRegistryKeyValue("ApiKey");
        }

        private static void DeleteRegistryKeyValue(string key)
        {
            using (var appKeyPath = Registry.CurrentUser.CreateSubKey(RegistrySubKey))
            {
                appKeyPath.DeleteValue(key, false);
            }
        }

        private static void SetRegistryKeyValue(string key, object value,
            RegistryValueKind regValueKing = RegistryValueKind.String)
        {
            using (var appKeyPath = Registry.CurrentUser.CreateSubKey(RegistrySubKey))
            {
                appKeyPath.SetValue(key, value, regValueKing);
            }
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

     

     
    }
}
