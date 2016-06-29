using System.Collections;
using System.Collections.Generic;
using Quandl.Shared.QuandlException;

namespace Quandl.Shared
{
    public class ExcelHelp
    {
        public static string PopulateData(Microsoft.Office.Interop.Excel.Range activeCell, ArrayList dataList)
        {
  
            // populate column names for hqdata
            ArrayList columnsList = (ArrayList)dataList[0];
            string result = "";
            for (int i = 0; i < columnsList.Count; i++)
            {
                if (i == 0)
                {
                    result = columnsList[i].ToString();
                }
                else
                {
                  activeCell[1, i+1].Value2 = columnsList[i].ToString();
                }
                
            }

            // populate data
            for (int i = 1; i < dataList.Count; i++)
            {
                int j = 1;
                foreach(var data in (ArrayList)dataList[i])
                {
                    activeCell[i + 1, j].Value2 = data.ToString();
                    j++; 
                }
            }
            return result;
        }
        public async static void PopulateLatestStockData(string[] quandlCodes, List<string> columnNames, Microsoft.Office.Interop.Excel.Range activeCells)
        {
            // Header
            Microsoft.Office.Interop.Excel.Range firstActiveCell = activeCells.get_Offset(0, 0);

            // Data
            int i = 1;
            foreach (string quandlCode in quandlCodes)
            {
                List<List<object>> convertedData = await Web.PullRecentStockData(quandlCode, columnNames, 1);
                PopulateData(quandlCode.ToUpper(), firstActiveCell, convertedData, i);
                i++;
            }
        }


        public static void PopulateData(string code, Microsoft.Office.Interop.Excel.Range activeCell, List<List<object>> data, int rowCount)
        {
            List<object> columns = data[0];
            List<object> dataList = data[1];

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
            
            for (int i = 1; i < list.Count; i++)
            {
                if (rowCount != 0 || i != 1)
                {
                    currentFormulaCell[rowCount + 1, i ].Value2 = list[i];
                }
  
            }

        }

    }

}

