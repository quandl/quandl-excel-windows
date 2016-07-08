using System;
using System.Collections.Generic;
using ExcelDna.Integration;
using Quandl.Shared;

namespace Quandl.Excel.UDF.Functions
{
    public static class Tools
    {
        public static string GetStringValue(object referenceOrString)
        {
            if (referenceOrString is string)
            {
                return referenceOrString.ToString();
            }
            if (referenceOrString is ExcelReference)
            {
                return GetValueFromSingleCell((ExcelReference) referenceOrString);
            }
            return null;
        }

        public static string GetDateValue(object referenceOrString)
        {
            if (referenceOrString is string)
            {
                return referenceOrString.ToString();
            }
            if (referenceOrString is ExcelReference)
            {
                return GetDateValueFromSingleCell((ExcelReference) referenceOrString);
            }
            return null;
        }

        public static dynamic ReferenceToRange(ExcelReference xlref)
        {
            dynamic app = ExcelDnaUtil.Application;
            return app.Range[XlCall.Excel(XlCall.xlfReftext, xlref,
                true)];
        }

        public static List<string> GetArrayOfValues(object referenceOrString)
        {
            if (referenceOrString is object[,])
            {
                return GetValuesFromObjectArray((object[,]) referenceOrString);
            }
            if (referenceOrString is string)
            {
                return Utilities.GetValuesFromString((string) referenceOrString);
            }
            if (referenceOrString is ExcelReference)
            {
                return GetValuesFromCellRange((ExcelReference) referenceOrString);
            }
            return new List<string>();
        }

        public static List<string> GetValuesFromObjectArray(object[,] arr)
        {
            var return_values = new List<string>();
            for (var i = 0; i < arr.GetLength(0); i++)
            {
                for (var j = 0; j < arr.GetLength(1); j++)
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
                var return_value = new List<string>();
                return_value.Add(excelReference.GetValue().ToString());
                return return_value;
            }
            return GetValuesFromObjectArray((object[,]) excelReference.GetValue());
        }

        public static string GetValueFromSingleCell(ExcelReference excelReference)
        {
            return (string) excelReference.GetValue();
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
            return false;
        }
    }
}