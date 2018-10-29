﻿using System;
using System.Collections.Generic;
using System.Globalization;
using ExcelDna.Integration;
using Quandl.Shared;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace Quandl.Excel.UDF.Functions.Helpers
{
    public class Tools
    {
        public static void SetCellVolatile(bool value)
        {
            XlCall.Excel(XlCall.xlfVolatile, value);
        }

        public static bool GetBoolValue(object referenceOrString)
        {
            return GetStringValue(referenceOrString).ToLower() == "true";
        }

        public static int? GetIntValue(object referenceOrString)
        {
            if (referenceOrString is double)
                return (int)(double)referenceOrString;
            if (referenceOrString is int)
                return (int)referenceOrString;

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
            else if (referenceOrString is List<string>)
            {
                return string.Join(",", (List<string>)referenceOrString);
            }
            else if (referenceOrString is ExcelReference)
            {
                bool isSingleCell = IsSingleCell((ExcelReference)referenceOrString);

                if (isSingleCell)
                {
                    return GetValueFromSingleCell((ExcelReference)referenceOrString);
                }
                else
                {
                    List<string> rangeOfCells = GetValuesFromCellRange((ExcelReference)referenceOrString).ConvertAll(obj => obj.ToString());
                    return string.Join(",", rangeOfCells);
                }
            }
            else
            {
                return null;
            }
        }

        public static dynamic ReferenceToRange(ExcelReference xlref)
        {
            var app = (Microsoft.Office.Interop.Excel._Application) ExcelDnaUtil.Application;
            return app.Range[XlCall.Excel(XlCall.xlfReftext, xlref, true)];
            
        }

        public static void ExecuteOnForegroundThread(ExcelAction action)
        {
            ExcelAsyncUtil.QueueAsMacro(action);
        }

        public static List<object> GetArrayOfValues(object referenceOrString)
        {
            if (referenceOrString is object[,])
                return GetValuesFromObjectArray((object[,])referenceOrString);

            if (referenceOrString is string)
                return Utilities.GetValuesFromString((string)referenceOrString).Select(s => (object)s).ToList();

            if (referenceOrString is ExcelReference)
                return GetValuesFromCellRange((ExcelReference)referenceOrString);

            return new List<object>();
        }

        public static List<DateTime?> GetArrayOfDates(object referenceOrString)
        {
            if (referenceOrString is object[,])
            {
                return GetValuesFromObjectArray((object[,])referenceOrString).Select(GetDateValue).ToList();
            }
            else if (referenceOrString is ExcelReference)
            {
                var reference = (ExcelReference)referenceOrString;
                if (!IsSingleCell(reference))
                {
                    object[,] dates = (object[,])reference.GetValue();
                    var startDate = GetDateValue(dates[0, 0]);
                    var endDate = GetDateValue(dates[dates.GetLength(0) - 1, dates.GetLength(1) - 1]);
                    return new List<DateTime?>() { startDate, endDate };
                }
                else
                {
                    var date = GetDateValue(referenceOrString);
                    if (date != null)
                    {
                        return new List<DateTime?>() { date };
                    }
                }
            }
            else
            {
                var date = GetDateValue(referenceOrString);
                if (date != null)
                {
                    return new List<DateTime?>() { date };
                }
            }
            return new List<DateTime?>() { };
        }

        private static DateTime? GetDateValue(object referenceOrString)
        {
            if (referenceOrString is ExcelReference)
            {
                return GetDateValueFromPrimitive(((ExcelReference)referenceOrString).GetValue());
            }
            if (referenceOrString is Range)
            {
                return GetDateValueFromPrimitive(((Range)referenceOrString).Value2);
            }
            return GetDateValueFromPrimitive(referenceOrString);
        }

        public static List<object> GetValuesFromObjectArray(object[,] arr)
        {
            var returnValues = new List<object>();
            for (var i = 0; i < arr.GetLength(0); i++)
            {
                for (var j = 0; j < arr.GetLength(1); j++)
                {
                    if (!(arr[i, j] is ExcelMissing))
                    {
                        returnValues.Add(arr[i, j]);
                    }
                }
            }
            return returnValues;
        }

        public static List<object> GetValuesFromCellRange(ExcelReference excelReference)
        {
            if (IsSingleCell(excelReference))
            {
                var returnValue = new List<object>();
                returnValue.Add(excelReference.GetValue());
                return returnValue;
            }
            return GetValuesFromObjectArray((object[,])excelReference.GetValue());
        }

        public static string GetValueFromSingleCell(ExcelReference excelReference)
        {
            // This is a very basic way of detecting date formatted cell. Checking for the existence of a y as most date formats will contain at least one 'y' for the year.
            if (XlCall.Excel(XlCall.xlfGetCell, 7, excelReference).ToString().Contains("y"))
            {
                return GetDateValueFromDouble((double)excelReference.GetValue()).ToString(Utilities.DateFormat);
            }
            else
            {
                return excelReference.GetValue().ToString();
            }
        }

        public static DateTime? GetDateValueFromPrimitive(object date)
        {
            if (date == null || date is ExcelDna.Integration.ExcelMissing)
            {
                return null;
            }
            if (date is string)
            {
                return GetDateValueFromString((string)date);
            }
            if (date is double)
            {
                return GetDateValueFromDouble((double)date);
            }
            throw new ArgumentException("Could not determine date type.");
        }

        public static DateTime? GetDateValueFromString(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return null;
            }
            try
            {
                return DateTime.ParseExact(date, Utilities.DateFormat, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                return DateTime.Parse(date);
            }
        }

        public static DateTime GetDateValueFromDouble(double date)
        {
            return DateTime.FromOADate(Convert.ToDouble(date));
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