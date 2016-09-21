using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace Quandl.Excel.Console
{
    public class SetupHelp
    {
        private const string AddinPartialPackageString = "Quandl";
        private const string AddinPackageString = "Quandl.Excel.UDF.Functions-AddIn.xll";

        private static readonly string OpenValue = $@"/R ""{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Quandl\Quandl.Excel.UDF.Functions-AddIn.xll""";

        public static string ValueKeyName { get; } = "OPEN";

        public static void RegisterExcelAddin()
        {
            foreach (var subKey in AddinRegisterKeys())
            {
                RemoveAvailableOpenOption(subKey); // Remove any existing old keys
                SetAvailableOpenOption(subKey); // Add in any new keys
            }
        }

        // For any existing excel add-ins, if they are selected then option of corresponding OPEN options will be used
        // For excel addin OPEN options follow the path OPEN, OPEN1, OPEN2 ... find the first available options
        private static void SetAvailableOpenOption(string subKey)
        {
            var keys = OpenSubKeys(subKey);
            foreach (var key in keys)
            {
                var result = CheckQuandlAddinRegistry(subKey, key);
                if (result == KeySearchResult.Exist)
                {
                    return;
                }
            }

            // Figure out the last OPEN integer and add that in.
            var values = keys.Select(k =>
            {
                var destrung = Regex.Replace(k, "[^0-9]", "");
                return string.IsNullOrWhiteSpace(destrung) ? 0 : Convert.ToInt32(destrung);
            }).ToList();

            var maxKey = values.Any() ? $"{values.Max() + 1}" : "";
            SetRegistryKeyValue(subKey, $"{ValueKeyName}{maxKey}", OpenValue);
        }

        private static void RemoveAvailableOpenOption(string subKey)
        {
            foreach (var key in OpenSubKeys(subKey))
            {
                var result = CheckQuandlAddinRegistry(subKey, key);
                if (result != KeySearchResult.Exist) continue;
                RemoveRegistryKeyValue(subKey, key);
            }
        }

        private static void RemoveRegistryKeyValue(string subKey, string key)
        {
            Registry.CurrentUser.OpenSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree)?.DeleteValue(key);
        }

        private static void SetRegistryKeyValue(string subKey, string key, object value,
            RegistryValueKind regValueKing = RegistryValueKind.String)
        {
            var keyPath = Registry.CurrentUser.CreateSubKey(subKey);
            if (keyPath == null) return;
            keyPath.SetValue(key, value, regValueKing);
            keyPath.Close();
        }

        private static KeySearchResult CheckQuandlAddinRegistry(string subKey, string keyName)
        {
            var keyPath = Registry.CurrentUser.OpenSubKey(subKey);
            var value = keyPath?.GetValue(keyName);
            if (value == null) return KeySearchResult.NotExist;
            return value.ToString().Contains(AddinPartialPackageString)
                ? KeySearchResult.Exist
                : KeySearchResult.NotFoundYet;
        }

        private static List<string> OpenSubKeys(string subkey)
        {
            var openSubKey = Registry.CurrentUser.OpenSubKey(subkey);
            if (openSubKey == null) return new List<string>();
            var excelInstallations = openSubKey.GetValueNames();
            var keys = new List<string>(excelInstallations);
            keys = keys.Where(k => Regex.IsMatch(k, "^OPEN\\d*$")).ToList();
            return keys;
        }

        private static IEnumerable<string> AddinRegisterKeys()
        {
            var openSubKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Office");
            if (openSubKey == null) return new List<string>();
            var excelInstallations = openSubKey.GetSubKeyNames();
            var keys = new List<string>(excelInstallations);
            keys = keys.Where(k => Regex.IsMatch(k, "^\\d+.\\d+$")).Where(k => double.Parse(k) >= 14)
                .Select(k => $@"SOFTWARE\Microsoft\Office\{k}\Excel\Options").ToList();
            return keys;
        }

        private static void ClearSettings()
        {
            string RegistrySubKey = @"SOFTWARE";
            var appKeyPath = Registry.CurrentUser.CreateSubKey(RegistrySubKey);
            try
            {
                appKeyPath.DeleteSubKeyTree("Quandl");
            }
            catch (System.ArgumentException e)
            {
                // Catch exceptiont that the key does not exist, do nothing
            }
            finally
            {
                appKeyPath.Close();
            }
        }

        public static void UnRegisterExcelAddin()
        {
            foreach (var subKey in AddinRegisterKeys())
            {
                RemoveAvailableOpenOption(subKey);
            }
            ClearSettings();
        }

        private enum KeySearchResult
        {
            Exist,
            NotExist,
            NotFoundYet
        }
    }
}