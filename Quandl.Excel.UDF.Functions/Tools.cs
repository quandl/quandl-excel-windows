using System;
using System.Collections.Generic;
using System.Globalization;
using ExcelDna.Integration;
using Quandl.Shared;

namespace Quandl.Excel.UDF.Functions
{
    public class Tools
    {
        public static bool GetBoolValue(object referenceOrString)
        {
            return GetStringValue(referenceOrString).ToLower() == "true";
        }

        public static int? GetIntValue(object referenceOrString)
        {
            if (referenceOrString is double)
                return (int) (double) referenceOrString;
            if (referenceOrString is int)
                return (int) referenceOrString;

            var cellValue = GetStringValue(referenceOrString);
            if (cellValue == null)
                return null;
            return int.Parse(GetStringValue(referenceOrString));
        }


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
            var returnValues = new List<string>();
            for (var i = 0; i < arr.GetLength(0); i++)
            {
                for (var j = 0; j < arr.GetLength(1); j++)
                {
                    if (!(arr[i, j] is ExcelMissing))
                    {
                        returnValues.Add(arr[i, j].ToString());
                    }
                }
            }
            return returnValues;
        }

        public static List<string> GetValuesFromCellRange(ExcelReference excelReference)
        {
            if (IsSingleCell(excelReference))
            {
                var returnValue = new List<string>();
                returnValue.Add(excelReference.GetValue().ToString());
                return returnValue;
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

        public static DateTime? GetDateValueFromString(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return null;
            }
            return DateTime.ParseExact(date, Utilities.DateFormat, CultureInfo.InvariantCulture);
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