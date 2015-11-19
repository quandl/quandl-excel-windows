using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Linq;

namespace Quandl.Shared
{
    public class Utilities
    {
        public static ArrayList GetMatchedListByOrder(ArrayList columnNames, ArrayList columnNamesList, ArrayList dataList)
        {
            if (columnNames == null)
            {
                return dataList;
            }

            ArrayList result = new ArrayList();
            ArrayList indexList = new ArrayList();
            // addd date column first
            indexList.Add(0);
            foreach(string column in columnNames)
            {
               int index = columnNamesList.IndexOf(column.ToUpper());
               if (index > 0)
               {
                    indexList.Add(index);
               }
            }

            foreach(ArrayList list in dataList)
            {
                result.Add(SubList(indexList, list));
            }
            return result;
        }



        public static string AuthToken(string accountName, string pass)
        {
            var obj = new { user = new { account = accountName, password = pass } };
            var payload = JsonConvert.SerializeObject(obj);
            var requestUri = Properties.Settings.Default.BaseUrl + "users/token_auth";
            var res = Web.Post(requestUri, payload);
            return res["user"]["api_key"].ToObject<string>();
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

        private static ArrayList SubList(ArrayList indexList, ArrayList list)
        {
            ArrayList result = new ArrayList();
            foreach(int i in indexList)
            {
                result.Add(list[i]);
            }
            return result;
        }
    }
}
