using ExcelDna.Integration;
using System;
using System.Collections;
using Quandl.Shared;

namespace QuandlFunctions
{
    public static class Tools
    {
        public static string GetStringValue(Object referenceOrString)
        {
            if (referenceOrString is String)
            {
                return referenceOrString.ToString();
            }
            else if (referenceOrString is ExcelReference)
            {
                return GetValueFromSingleCell((ExcelReference)referenceOrString);
            }
            else
            {
                return null;
            }
        }

        public static string GetDateValue(Object referenceOrString)
        {
            if (referenceOrString is String)
            {
                return referenceOrString.ToString();
            }
            else if (referenceOrString is ExcelReference)
            {
                return GetDateValueFromSingleCell((ExcelReference)referenceOrString);
            }
            else
            {
                return null;
            }
        }

        public static dynamic ReferenceToRange(ExcelReference xlref)
        {
            dynamic app = ExcelDnaUtil.Application;
            return app.Range[XlCall.Excel(XlCall.xlfReftext, xlref,
    true)];
        }

        public static ArrayList GetArrayOfValues(Object referenceOrString)
        {
            if (referenceOrString is Object[,])
            {
                return GetValuesFromObjectArray((Object[,])referenceOrString);
            }
            else if (referenceOrString is String)
            {
                return Utilities.GetValuesFromString((String)referenceOrString);
            }
            else if (referenceOrString is ExcelReference)
            {
                return GetValuesFromCellRange((ExcelReference)referenceOrString);
            }
            else
            {
                return new ArrayList();
            }
        }
        public static ArrayList GetValuesFromObjectArray(Object[,] arr)
        {
            ArrayList return_values = new ArrayList();
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    if (!(arr[i, j] is ExcelMissing))
                    {
                        return_values.Add(arr[i, j].ToString());
                    }
                }
            }
            return return_values;
        }

        public static ArrayList GetValuesFromCellRange(ExcelReference excelReference)
        {
            if (IsSingleCell(excelReference))
            {
                ArrayList return_value = new ArrayList();
                return_value.Add(excelReference.GetValue().ToString());
                return return_value;
            }
            else
            {
                return GetValuesFromObjectArray((Object[,])excelReference.GetValue());
            }
            
        }

        public static string GetValueFromSingleCell(ExcelReference excelReference)
        {
            return (string)excelReference.GetValue();
        }

        public static string GetDateValueFromSingleCell(ExcelReference excelReference)
        {
            var date = DateTime.FromOADate(Convert.ToDouble(excelReference.GetValue()));
            return date.ToString("yyyyMMdd");
        }

        public static bool IsSingleCell(ExcelReference er)
        {
            if (er.ColumnFirst == er.ColumnLast && er.RowFirst == er.RowLast)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
