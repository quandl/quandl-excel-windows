using System.Collections;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;

namespace Quandl.Shared
{
    public class ExcelHelp
    {
        public static string PopulateData(Range activeCell, ArrayList dataList)
        {
            // populate column names for hqdata
            var columnsList = (ArrayList) dataList[0];
            var result = "";
            for (var i = 0; i < columnsList.Count; i++)
            {
                if (i == 0)
                {
                    result = columnsList[i].ToString();
                }
                else
                {
                    activeCell[1, i + 1].Value2 = columnsList[i].ToString();
                }
            }

            // populate data
            for (var i = 1; i < dataList.Count; i++)
            {
                var j = 1;
                foreach (var data in (ArrayList) dataList[i])
                {
                    activeCell[i + 1, j].Value2 = data.ToString();
                    j++;
                }
            }
            return result;
        }

        public static async void PopulateLatestStockData(string[] quandlCodes, List<string> columnNames,
            Range activeCells)
        {
            // Header
            var firstActiveCell = activeCells.get_Offset(0, 0);

            // Data
            var i = 1;
            foreach (var quandlCode in quandlCodes)
            {
                var convertedData = await Web.PullRecentStockData(quandlCode, columnNames, 1);
                PopulateData(quandlCode.ToUpper(), firstActiveCell, convertedData, i);
                i++;
            }
        }


        public static void PopulateData(string code, Range activeCell, List<List<object>> data, int rowCount)
        {
            var columns = data[0];
            var dataList = data[1];

            if (rowCount == 1)
            {
                for (var i = 0; i < columns.Count; i++)
                {
                    activeCell[rowCount, i + 2].Value2 = columns[i];
                }
            }

            activeCell[rowCount + 1][1].Value2 = code;
            for (var i = 0; i < dataList.Count; i++)
            {
                activeCell[rowCount + 1, i + 2].Value2 = dataList[i];
            }
        }

        public static void PopulateData(Range currentFormulaCell, string quandlCode, List<List<object>> dataList,
            int rowCount)
        {
            var firstCell = currentFormulaCell.get_Offset(rowCount, -1);
            var list = dataList[0];

            for (var i = 1; i < list.Count; i++)
            {
                if (rowCount != 0 || i != 1)
                {
                    currentFormulaCell[rowCount + 1, i].Value2 = list[i];
                }
            }
        }
    }
}