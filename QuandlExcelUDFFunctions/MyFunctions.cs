using System;
using System.Collections;
using System.Threading;

using ExcelDna.Integration;
using Excel = Microsoft.Office.Interop.Excel;
using Newtonsoft.Json.Linq;

using Quandl.Shared;
using System.Reflection;

namespace QuandlFunctions
{
    public static class MyFunctions
    {
        static Excel.Application Application = ExcelDnaUtil.Application as Excel.Application;

        [ExcelFunction(Description = "My first .NET function", IsMacroType = true)]
        public static string QDATA(
            [ExcelArgument("is the quandl database code", AllowReference = true)] Object excelQuandlCodes,
            [ExcelArgument("are the quandl column name list", AllowReference = true)] Object excelColumnNames
            )
        {
            ProcessParams processParams = new ProcessParams();
            ExcelReference reference = (ExcelReference)XlCall.Excel(XlCall.xlfCaller);
            Excel.Range currentFormulaCell = ReferenceToRange(reference);
            processParams.cell = currentFormulaCell;

            ArrayList quandlCodes = getArrayOfValues(excelQuandlCodes);
            ArrayList columnNamesMixedCase = getArrayOfValues(excelColumnNames);
            ArrayList columnNames = new ArrayList();
            foreach (string columnName in columnNamesMixedCase)
            {
                columnNames.Add(columnName.ToUpper());
            }

            int i = 1;
            foreach(string quandlCode in quandlCodes)
            {
                processParams.data = TestFunctions.pullRecentStockData(quandlCode, columnNames, 1);
                processParams.code = quandlCode.ToUpper();
                processParams.rowCount = i;
                i++;

                // Threading only necessary for class 1 type excel functions.
                DumpFromThread(processParams);
                //Thread t = new Thread(DumpFromThread);
                //t.Start(processParams);
            }

            return "success";
        }

        private static dynamic ReferenceToRange(ExcelReference xlref)
        {
            dynamic app = ExcelDnaUtil.Application;
            return app.Range[XlCall.Excel(XlCall.xlfReftext, xlref,
    true)];
        }

        private static ArrayList getArrayOfValues(Object referenceOrString)
        {
            if (referenceOrString is Object[,])
            {
                return getValuesFromObjectArray((Object[,])referenceOrString);
            }
            else if (referenceOrString is String)
            {
                return getValuesFromString((String)referenceOrString);
            }
            else if (referenceOrString is ExcelReference)
            {
                return getValuesFromCellRange((ExcelReference)referenceOrString);
            }
            else
            {
                return new ArrayList();
            }
        }
        private static ArrayList getValuesFromObjectArray(Object[,] arr)
        {
            ArrayList values = new ArrayList();
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    if (!(arr[i, j] is ExcelMissing))
                    {
                        values.Add(arr[i, j].ToString());
                    }
                }
            }
            return values;
        }

        private static ArrayList getValuesFromString(String excelFormulaArray)
        {
            ArrayList values = new ArrayList();
            values.Add(excelFormulaArray.ToUpper());
            return values;
        }

        private static ArrayList getValuesFromCellRange(ExcelReference excelReference)
        {
            return getValuesFromObjectArray((Object[,])excelReference.GetValue());
        }

        private static void DumpFromThread(Object processParams)
        {
            ProcessParams p = processParams as ProcessParams;
            TestFunctions.populateData(p.code, p.cell, p.data,p.rowCount);
     
        }

        public class ProcessParams
        {
            public Excel.Range cell { get; set; }
            public ArrayList data { get; set; }

            public string code { get; set; }
            public int rowCount { get; set; }
        }

    }
}
