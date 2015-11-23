using System.Collections;
using Quandl.Shared;
using System;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;

namespace Quandl.Shared
{
    public class ExcelHelp
    {
        private static string[] AddinRegisterKey = new string[] { @"SOFTWARE\Microsoft\Office\14.0\Excel\Options", @"SOFTWARE\Microsoft\Office\15.0\Excel\Options", @"SOFTWARE\Microsoft\Office\16.0\Excel\Options" };
        private const string OpenValue = @"/R ""C:\Program Files (x86)\Quandl Inc\Quandl-Excel-Addin\Quandl.Excel.UDF.Functions-AddIn.xll""";
        private const string AddinPackageString = "Quandl.Excel.UDF.Functions-AddIn.xll";

        public static void RegisterExcelAddin()
        {
            foreach(string subKey in AddinRegisterKey)
            {
                SetAvailableOpenOption(subKey);
            }
        }

        public static string PopulateData(Microsoft.Office.Interop.Excel.Range activeCell, ArrayList dataList)
        {
            string result = "";
            for (int i = 0; i < dataList.Count; i++)
            {
                int j = 0;
                foreach(var data in (ArrayList)dataList[i])
                {
                    if (i == 0 && j == 1)
                    {
                        result = data.ToString();
                    }
                    else
                    {
                       activeCell[i + 1, j].Value2 = data.ToString();
                    }
                    j++; 
                }
            }
            return result;
        }
        public static void PopulateLatestStockData(string[] quandlCodes, ArrayList columnNames, Microsoft.Office.Interop.Excel.Range activeCells)
        {
            // Header
            Microsoft.Office.Interop.Excel.Range firstActiveCell = activeCells.get_Offset(0, 0);

            // Data
            int i = 1;
            foreach (string quandlCode in quandlCodes)
            {
                ArrayList convertedData = Web.PullRecentStockData(quandlCode, columnNames, 1);
                PopulateData(quandlCode.ToUpper(), firstActiveCell, convertedData, i);
                i++;
            }
        }


        public static void PopulateData(string code, Microsoft.Office.Interop.Excel.Range activeCell, ArrayList data, int rowCount)
        {
            ArrayList columns = (ArrayList)data[0] as ArrayList;
            ArrayList dataList = (ArrayList)data[1] as ArrayList;

            if (rowCount == 1)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    activeCell[rowCount, i + 2].Value2 = columns[i];
                }

            }

            activeCell[rowCount + 1][1].Value2 = code;
            for (int i = 0; i < dataList.Count; i++)
            {
                activeCell[rowCount + 1, i + 2].Value2 = dataList[i];

            }

        }

        public static void PopulateData(Microsoft.Office.Interop.Excel.Range currentFormulaCell, string quandlCode, ArrayList dataList, int rowCount)
        {
            Microsoft.Office.Interop.Excel.Range firstCell = currentFormulaCell.get_Offset(rowCount, -1 );
            ArrayList list = (ArrayList)dataList[0];
            
            for (int i = 0; i < list.Count; i++)
            {
                if (rowCount != 0 || i != 1)
                {
                    currentFormulaCell[rowCount + 1, i ].Value2 = list[i];
                }
  
            }

        }


        private static void SetAvailableOpenOption(string subKey)
        {
            string option = "OPEN";
            string result = CheckQuandlAddinRegistry(subKey, option);
            if (result == null)
            {
                SetRegistryKeyValue(subKey, option, OpenValue);
            }
            else if (result != "")
            {
                for(int i = 1; i <= 1000; i++)
                {
                    option = option + i;
                    result = CheckQuandlAddinRegistry(subKey, option.ToString());
                    if (result == null)
                    {
                        SetRegistryKeyValue(subKey, option, OpenValue);
                        break;
                    }
                    else if ( result == "")
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

        private static string CheckQuandlAddinRegistry(string subKey, string keyName)
        {
            RegistryKey keyPath = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(subKey);
            if (keyPath == null)
            {
                return null;
            }
            else
            {
                object value = keyPath.GetValue(keyName);
                if (value != null)
                {
                    if (value.ToString().Contains(AddinPackageString))
                    {
                        return "";
                    }
                    else
                    {
                        return value.ToString();
                    }
                }
                else
                {
                    return null;
                }

            }
        }
    }

}

