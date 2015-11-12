using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Quandl.Shared
{
    public class QuandlConfig
    {
        private const string RegistrySubKey = @"SOFTWARE\Quandl\ExcelAddin";

        public static string ApiKey
        {
            get { return GetRegistry<string>("ApiKey"); }
            set { SetRegistryKeyValue("ApiKey", value); }
        }

        public static bool AutoUpdate
        {
            get { return Convert.ToBoolean(GetRegistry<int>("AutoUpdate")); }
            set { SetRegistryKeyValue("AutoUpdate", Convert.ToInt32(value), RegistryValueKind.DWord); }
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
    }
}
