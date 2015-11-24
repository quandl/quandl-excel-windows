using Microsoft.Win32;

namespace Quandl.Shared
{
    public class SetupHelp
    {
        private static string[] AddinRegisterKey = new string[] { @"SOFTWARE\Microsoft\Office\14.0\Excel\Options", @"SOFTWARE\Microsoft\Office\15.0\Excel\Options", @"SOFTWARE\Microsoft\Office\16.0\Excel\Options" };
        private const string OpenValue = @"/R ""C:\Program Files (x86)\Quandl Inc\Quandl-Excel-Addin\Quandl.Excel.UDF.Functions-AddIn.xll""";
        private const string AddinPackageString = "Quandl.Excel.UDF.Functions-AddIn.xll";
        private const int DEFAULT_NUMBER_OF_USER_SELECTED_ADDIN = 1000;
        private enum KeySearchResult { Exist, NotExist, NotFoundYet }

        public static void RegisterExcelAddin()
        {
            foreach (string subKey in AddinRegisterKey)
            {
                SetAvailableOpenOption(subKey);
            }
        }

        // For any existing excel add-ins, if they are selected then option of corresponding OPEN options will be used
        // For excel addin OPEN options follow the path OPEN, OPEN1, OPEN2 ... find the first available options
        private static void SetAvailableOpenOption(string subKey)
        {
            string option = "OPEN";
            KeySearchResult result = CheckQuandlAddinRegistry(subKey, option);
            if (result == KeySearchResult.NotExist)
            {
                SetRegistryKeyValue(subKey, option, OpenValue);
            }
            else if (result != KeySearchResult.Exist)
            {
                for (int i = 1; i <= DEFAULT_NUMBER_OF_USER_SELECTED_ADDIN; i++)
                {
                    option = option + i;
                    result = CheckQuandlAddinRegistry(subKey, option.ToString());
                    if (result == KeySearchResult.NotExist)
                    {
                        SetRegistryKeyValue(subKey, option, OpenValue);
                        break;
                    }
                    else if (result == KeySearchResult.Exist)
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        private static void SetRegistryKeyValue(string subKey, string key, object value, RegistryValueKind regValueKing = RegistryValueKind.String)
        {
            Microsoft.Win32.RegistryKey keyPath = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(subKey);
            keyPath.SetValue(key, value, regValueKing);
            keyPath.Close();
        }

        private static KeySearchResult CheckQuandlAddinRegistry(string subKey, string keyName)
        {
            RegistryKey keyPath = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(subKey);
            if (keyPath == null)
            {
                return KeySearchResult.NotExist;
            }
            else
            {
                object value = keyPath.GetValue(keyName);
                if (value != null)
                {
                    if (value.ToString().Contains(AddinPackageString))
                    {
                        return KeySearchResult.Exist;
                    }
                    else
                    {
                        return KeySearchResult.NotFoundYet;
                    }
                }
                else
                {
                    return KeySearchResult.NotExist;
                }

            }
        }

    }
}
