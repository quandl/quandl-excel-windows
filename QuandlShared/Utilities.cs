using System;
using System.Collections;
using System.Collections.Generic;
using Quandl.Shared.models;
using System.Linq;
using System.Threading.Tasks;

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

        public static List<string> GetValuesFromString(string excelFormulaArray)
        {
            var values = new List<string>();
            values.Add(excelFormulaArray.ToUpper());
            return values;
        }

        public static List<string> ListToUpper(List<string> list)
        {
            var result = new List<string>();
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

        public static async Task<bool> ValidateDataCode(string code)
        {
            try
            {
                Database db = await Web.GetDatabase(code);
           
                if (db == null)
                {
                    DatatableCollection dc = await Web.GetDatatableCollection(code);
                    if (dc == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

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
