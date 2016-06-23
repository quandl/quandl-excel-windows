using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

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

        public static bool ApiKeyValid() {
            return !String.IsNullOrEmpty(ApiKey);
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
