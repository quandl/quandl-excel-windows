using System.Collections;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;

namespace Quandl.Shared
{
    public class Utilities
    {
        public const string DateFormat = "yyyy-MM-dd";

        private static string excelVersion;

        public static string GetExcelVersionNumber
        {
            get
            {
                if (excelVersion != null)
                {
                    return excelVersion;
                }

                // This is expensive so only call it once.
                var appVersion = new Application();
                appVersion.Visible = false;
                excelVersion = appVersion.Version;
                return excelVersion;
            }
        }

        public static ArrayList GetMatchedListByOrder(ArrayList columnNames, ArrayList columnNamesList,
            ArrayList dataList)
        {
            var result = new ArrayList();
            var indexList = new ArrayList();

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
                var index = columnNamesList.IndexOf(column.ToUpper());
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

        public static List<string> GetValuesFromString(string excelFormulaArray)
        {
            var values = new List<string>();
            values.Add(excelFormulaArray.ToUpper());
            return values;
        }

        public static List<string> ListToUpper(List<string> list)
        {
            var result = new List<string>();
            foreach (var s in list)
            {
                result.Add(s.ToUpper());
            }
            return result;
        }

        public static ArrayList SubList(ArrayList indexList, ArrayList list)
        {
            var result = new ArrayList();
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
            var result = new ArrayList();
            result.Add(item);
            result.AddRange(list);
            return result;
        }
    }
}