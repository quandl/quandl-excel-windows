using System;
using System.Collections;
using System.Collections.Generic;
using ExcelDna.Integration;
using Quandl.Shared;

namespace Quandl.Excel.UDF.Functions
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

        public static List<string> GetArrayOfValues(Object referenceOrString)
        {
            if (referenceOrString is Object[,])
            {
                return GetValuesFromObjectArray((Object[,])referenceOrString);
            }
            else if (referenceOrString is string)
            {
                return Utilities.GetValuesFromString((string)referenceOrString);
            }
            else if (referenceOrString is ExcelReference)
            {
                return GetValuesFromCellRange((ExcelReference)referenceOrString);
            }
            else
            {
                return new List<string>();
            }
        }
        public static List<string> GetValuesFromObjectArray(Object[,] arr)
        {
            var return_values = new List<string>();
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

        public static List<string> GetValuesFromCellRange(ExcelReference excelReference)
        {
            if (IsSingleCell(excelReference))
            {
                List<string> return_value = new List<string>();
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
