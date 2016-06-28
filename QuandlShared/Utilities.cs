using Newtonsoft.Json;
using System;
using System.Collections;
using Quandl.Shared.QuandlException;

namespace Quandl.Shared
{
    public class Utilities
    {
        private static string excelVersion = null;

        public static string GetExcelVersionNumber
        {
            get
            {
                if (excelVersion != null)
                {
                    return excelVersion;
                }

                // This is expensive so only call it once.
                Microsoft.Office.Interop.Excel.Application appVersion = new Microsoft.Office.Interop.Excel.Application();
                appVersion.Visible = false;
                excelVersion = appVersion.Version.ToString();
                return excelVersion;
            }
        }

        public static ArrayList GetMatchedListByOrder(ArrayList columnNames, ArrayList columnNamesList, ArrayList dataList)
        {
            ArrayList result = new ArrayList();
            ArrayList indexList = new ArrayList();

            if (columnNames == null || columnNames.Count == 0)
            {
                columnNames = columnNamesList;
            }
            else
            {
                // add date column first
                columnNames = PrependToList(columnNames, "DATE");
            }

            result.Add(columnNames);

            foreach (string column in columnNames)
            {
                int index = columnNamesList.IndexOf(column.ToUpper());
                if (index >= 0)
                {
                    indexList.Add(index);
                }
            }

            foreach (ArrayList list in dataList)
            {
                result.Add(SubList(indexList, list));
            }

            return result;
        }

        public static ArrayList GetValuesFromString(String excelFormulaArray)
        {
            ArrayList values = new ArrayList();
            values.Add(excelFormulaArray.ToUpper());
            return values;
        }

        public static ArrayList ListToUpper(ArrayList list)
        {
            ArrayList result = new ArrayList();
            foreach (string s in list)
            {
                result.Add(s.ToUpper());
            }
            return result;
        }

        public static ArrayList SubList(ArrayList indexList, ArrayList list)
        {
            ArrayList result = new ArrayList();
            foreach (int i in indexList)
            {
                result.Add(list[i]);
            }
            return result;
        }

        public static string ValidateEmptyData(string quandl_data)
        {
            if (quandl_data == null || quandl_data.Equals(""))
            {
                throw new QuandlDataNotFoundException();
            }

            return quandl_data;
        }

        private static ArrayList PrependToList(ArrayList list, string item)
        {
            ArrayList result = new ArrayList();
            result.Add(item);
            result.AddRange(list);
            return result;
        }
    }
}
