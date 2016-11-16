﻿using Microsoft.Office.Interop.Excel;
using MoreLinq;
using Quandl.Shared.Properties;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Quandl.Shared
{
    public class Utilities
    {
        public const string ReleaseVersion = "3.0 Beta 1.0";
        public const string ReleaseSource = "excel";
        public const string DateFormat = "yyyy-MM-dd";
        public enum UserRoles
        {
            User,
            Admin,
            Platinum,
            Customer
        };

        private const bool ENABLE_SENTRY_LOG = true;
        private const int WinDefaultDpi = 96;
        public static string ExcelVersionNumber { get; set; }

        public static async void LogToSentry(System.Exception exception, Dictionary<string, string> additionalData = null)
        {
            Trace.WriteLine(exception.Message);

            if (ENABLE_SENTRY_LOG)
            {
                SetSentryData(exception, "Excel-Version", Utilities.ExcelVersionNumber);
                SetSentryData(exception, "Addin-Release-Version", Utilities.ReleaseVersion);
                SetSentryData(exception, "X-API-Token", QuandlConfig.ApiKey);
                if (additionalData != null)
                {
                    additionalData.ForEach(k => SetSentryData(exception, k.Key, k.Value));
                }
                var ravenClient = new RavenClient(Settings.Default.SentryUrl);
                await ravenClient.CaptureAsync(new SentryEvent(exception));
            }
        }

        public static void LogToSentry(Exception exception)
        {
            LogToSentry(exception, null);
        }

        public static ArrayList GetMatchedListByOrder(ArrayList columnNames, ArrayList columnNamesList,
            ArrayList dataList)
        {
            var result = new ArrayList();
            var indexList = new ArrayList();

            if (columnNames == null || columnNames.Count == 0)
            {
                columnNames = columnNamesList;
            }
            else
            {
                // add date column first
                columnNames = PrependToList(columnNames, "DATE");
            }

            result.Add(columnNames);

            foreach (string column in columnNames)
            {
                var index = columnNamesList.IndexOf(column.ToUpper());
                if (index >= 0)
                {
                    indexList.Add(index);
                }
            }

            foreach (ArrayList list in dataList)
            {
                result.Add(SubList(indexList, list));
            }

            return result;
        }

        public static List<string> GetValuesFromString(string excelFormulaArray)
        {
            var values = new List<string>();
            values.Add(excelFormulaArray.ToUpper());
            return values;
        }

        public static List<string> ListToUpper(List<string> list)
        {
            var result = new List<string>();
            foreach (var s in list)
            {
                result.Add(s.ToUpper());
            }
            return result;
        }

        public static ArrayList SubList(ArrayList indexList, ArrayList list)
        {
            var result = new ArrayList();
            foreach (int i in indexList)
            {
                result.Add(list[i]);
            }
            return result;
        }

        public static string ObjectToHumanString(object obj)
        {
            var value = obj.ToString();

            try
            {
                if (obj is List<string>)
                {
                    value = string.Join(", ", ((List<string>)obj).Select(s => s.ToString()));
                }
                else if (obj is object[,])
                {
                    string[] to = ((object[,])obj).Cast<string>().ToArray();
                    value = string.Join(", ", to);
                }
            }
            catch (Exception)
            {
                // bail since we really don't know what to do at this point.
            }
            return value;
        }

        public static string ValidateEmptyData(string quandl_data)
        {
            if (string.IsNullOrWhiteSpace(quandl_data))
            {
                throw new QuandlDataNotFoundException();
            }

            return quandl_data;
        }

        /*
         * g is a graphics object in a window. We just need to get the 
         * current windows dpi settings so any window will do.
         * http://stackoverflow.com/questions/5977445/how-to-get-windows-display-settings
         * 
         * 96(WinDefaultDpi) is the default dpi for Windows
         * https://blogs.msdn.microsoft.com/fontblog/2005/11/08/where-does-96-dpi-come-from-in-windows/
         * 
         * According to https://msdn.microsoft.com/en-us/library/system.drawing.graphics.fromhwnd(v=vs.110).aspx
         * you should .Dispose of the graphics object when you are done with it.
         * 
         */
        public static float WindowsScalingFactor()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            float factor = g.DpiX / WinDefaultDpi;
            g.Dispose();
            return factor;
        }

        public static string GetUserRole(JToken user)
        {
            UserRoles userRole;
            string userRoleStr = user["user_role"].ToString().ToLower();
            switch (userRoleStr)
            {
                case "user":
                    userRole = UserRoles.Admin;
                    break;
                case "customer":
                    userRole = UserRoles.Customer;
                    break;
                case "premium":
                    userRole = UserRoles.Platinum;
                    break;
                case "admin":
                    userRole = UserRoles.Admin;
                    break;
                default:
                    userRole = UserRoles.User;
                    break;
            }
            return userRole.ToString();
        }

        private static ArrayList PrependToList(ArrayList list, string item)
        {
            var result = new ArrayList();
            result.Add(item);
            result.AddRange(list);
            return result;
        }

        private static void SetSentryData(Exception exception, string key, string value)
        {
            if (key != null && !exception.Data.Contains(key))
                exception.Data.Add(key, value);
        }
    }
}