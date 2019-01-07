using Microsoft.Win32;

namespace Quandl.Shared
{
    public class SetupHelp
    {
        private const string OpenValue =
            @"/R ""C:\Program Files (x86)\Quandl Inc\Quandl-Excel-Addin\Quandl.Excel.UDF.Functions-AddIn-packed.xll""";

        private const string AddinPackageString = "Quandl.Excel.UDF.Functions-AddIn-packed.xll";
        private const int DEFAULT_NUMBER_OF_USER_SELECTED_ADDIN = 1000;

        private static readonly string[] AddinRegisterKey =
        {
            @"SOFTWARE\Microsoft\Office\14.0\Excel\Options",
            @"SOFTWARE\Microsoft\Office\15.0\Excel\Options",
            @"SOFTWARE\Microsoft\Office\16.0\Excel\Options"
        };

        public static void RegisterExcelAddin()
        {
            foreach (var subKey in AddinRegisterKey)
            {
                SetAvailableOpenOption(subKey);
            }
        }

        // For any existing excel add-ins, if they are selected then option of corresponding OPEN options will be used
        // For excel addin OPEN options follow the path OPEN, OPEN1, OPEN2 ... find the first available options
        private static void SetAvailableOpenOption(string subKey)
        {
            var option = "OPEN";
            var result = CheckQuandlAddinRegistry(subKey, option);
            if (result == KeySearchResult.NotExist)
            {
                SetRegistryKeyValue(subKey, option, OpenValue);
            }
            else if (result != KeySearchResult.Exist)
            {
                for (var i = 1; i <= DEFAULT_NUMBER_OF_USER_SELECTED_ADDIN; i++)
                {
                    option = option + i;
                    result = CheckQuandlAddinRegistry(subKey, option);
                    if (result == KeySearchResult.NotExist)
                    {
                        SetRegistryKeyValue(subKey, option, OpenValue);
                        break;
                    }
                    if (result == KeySearchResult.Exist)
                    {
                        break;
                    }
                }
            }
        }

        private static void SetRegistryKeyValue(string subKey, string key, object value,
            RegistryValueKind regValueKing = RegistryValueKind.String)
        {
            using (var keyPath = Registry.CurrentUser.CreateSubKey(subKey))
            {
                keyPath.SetValue(key, value, regValueKing);
            }
        }

        private static KeySearchResult CheckQuandlAddinRegistry(string subKey, string keyName)
        {
            var keyPath = Registry.CurrentUser.OpenSubKey(subKey);
            if (keyPath == null)
            {
                return KeySearchResult.NotExist;
            }
            var value = keyPath.GetValue(keyName);
            if (value != null)
            {
                if (value.ToString().Contains(AddinPackageString))
                {
                    return KeySearchResult.Exist;
                }
                return KeySearchResult.NotFoundYet;
            }
            return KeySearchResult.NotExist;
        }

        private enum KeySearchResult
        {
            Exist,
            NotExist,
            NotFoundYet
        }
    }
}