using System;
using System.Net;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Office.Tools.Ribbon;

namespace quandl_excel_windows
{
    public partial class Ribbon1
    {
        ArrayList columnList = null;

        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {
            columnList = this.getWorkingColumns();
        }

        private void download_button_Click(object sender, RibbonControlEventArgs e)
        {
            Excel.Worksheet sheet = ((Excel.Worksheet)Globals.ThisAddIn.Application.ActiveSheet);
     
           string database_code = getDatabaseCode(sheet);
            int i = 2;
            int j = 2;
           foreach(string ticker in getTicker(sheet))
           {
                foreach (string indicator in getIndicators())
                {
                    string code = database_code + "/" + ticker + "_" + indicator + "_Q";
                    string requestUri = "https://www.quandl.com/api/v3/datasets/" + code + "/data.json?limit=1&api_key=56LY1VVcCDFj1u3J48Kw";
                    JObject o = getResponseJson(requestUri);
                    sheet.get_Range(String.Concat(columnList[j - 1], i)).Value2 = code;
                    sheet.get_Range(String.Concat(columnList[j], i)).Value2 = o["dataset_data"]["column_names"][0];
                    sheet.get_Range(String.Concat(columnList[j], i+1)).Value2 = o["dataset_data"]["column_names"][1];
                    sheet.get_Range(String.Concat(columnList[j+1], i)).Value2 = o["dataset_data"]["data"][0][0].ToString();
                    sheet.get_Range(String.Concat(columnList[j+1], i+1)).Value2 = o["dataset_data"]["data"][0][1].ToString();
                    i += 2;
                }
            }





            /*foreach (Excel.Range cell in activeWorksheet.get_Range("B1:C1").Cells)
            {
                String code = database_code + "/" + cell.Value;
                
                
                String r = String.Concat(this.columnList[j], 2);
                activeWorksheet.get_Range(r).Value2 = code;

                activeWorksheet.get_Range(String.Concat(columnList[j], 3)).Value2 = o["dataset_data"]["column_names"][0];
                activeWorksheet.get_Range(String.Concat(columnList[j + 1], 3)).Value2 = o["dataset_data"]["column_names"][1];

                int i = 4;
                foreach (Newtonsoft.Json.Linq.JToken s in o["dataset_data"]["data"])
                {
                    activeWorksheet.get_Range(String.Concat(columnList[j], i)).Value2 = s[0].ToString();
                    activeWorksheet.get_Range(String.Concat(columnList[j + 1], i)).Value2 = s[1].ToString();
                    i++;
                }
                j = j + 2;
            }*/

        }

        private JObject getResponseJson(String requestUri)
        {
            WebClient client = new WebClient();
            client.Headers["User-Agent"] = "excel quandl new add-in";
            return JObject.Parse(client.DownloadString(requestUri));
        }

        private string getDatabaseCode(Excel.Worksheet sheet)
        {
            return "ZFB";
            //return sheet.get_Range("A1").Value;
        }

        private string[] getTicker(Excel.Worksheet sheet)
        {
            //return sheet.get_Range("A2:A5").Value;
            return new string[] { "AAPL", "MSFT", "IBM", "FB" };
        }

        private string[] getIndicators()
        {
            return new string[] { "TOT_REVNU", "COST_GOOD_SOLD", "GROSS_PROFIT" }; 
        }

        private ArrayList getWorkingColumns()
        {
            ArrayList columnList = new ArrayList();
            columnList.Add("A");
            columnList.Add("B");
            columnList.Add("C");
            columnList.Add("D");
            columnList.Add("E");
            columnList.Add("F");
            columnList.Add("G");
            columnList.Add("H");
            columnList.Add("I");
            columnList.Add("J");
            columnList.Add("K");
            columnList.Add("L");
            columnList.Add("M");
            columnList.Add("N");
            columnList.Add("O");
            columnList.Add("P");
            columnList.Add("Q");
            return columnList;
        }

        private void About_Click(object sender, RibbonControlEventArgs e)
        {
            new quandl_excel_windows.Controls.AboutForm().Show();
        }
    }
}
