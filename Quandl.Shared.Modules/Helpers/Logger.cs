using System;
using System.IO;
using SharpRaven;
using SharpRaven.Data;
using System.Diagnostics;
using System.Collections.Generic;
using Quandl.Shared.Properties;
using MoreLinq;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Quandl.Shared.Helpers
{
    public class Logger
    {
        public enum LogType {FULL, STATUS, NOSENTRY, ERROR};

        public const string LogPath = @"Nasdaq\DataLink\logs";

        private const bool ENABLE_SENTRY_LOG = true;
        private const bool ENABLE_DISK_LOG = true;

        private const string FullLogPrefix = "addin";
        private const string StatusLogPrefix = "status";
        private const string ErrorLogPrefix = "error";

        private static Mutex mut = new Mutex();

        public static string getLogPath()
        {
            var path = Path.Combine(Environment.CurrentDirectory, LogPath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public static void log(string message, Dictionary<string, string> additionalData = null, LogType t = LogType.NOSENTRY)
        {
            if (additionalData == null)
            {
                additionalData = new Dictionary<string, string>() { };
            }

            // Write to disk logging if applicable
            if (ENABLE_DISK_LOG)
            {
                LogToDisk(message, additionalData, t);
            }
        }

        public static void log(Exception e, Dictionary<string, string> additionalData = null, LogType t = LogType.FULL)
        {
            Trace.WriteLine(e.Message);
            Trace.WriteLine(e.StackTrace);

            if (additionalData == null)
            {
                additionalData = new Dictionary<string, string>() { };
            }

            // Add the stack trace into the additional data if one is available
            if (e.StackTrace != null && e.StackTrace.Length > 0) {
                additionalData["StackTrace"] = e.StackTrace;
            }

            // Write to sentry logging if applicable
            if (ENABLE_SENTRY_LOG && ( t != LogType.NOSENTRY || t != LogType.STATUS))
            {
                LogToSentry(e, additionalData);
            }

            log(e.Message, additionalData, LogType.ERROR);
        }

        // Attempt to write to sentry but if it does not work then continue on.
        private static async void LogToSentry(Exception exception, Dictionary<string, string> additionalData)
        {
            try
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
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                Trace.WriteLine(e.StackTrace);
            }
        }

        private static void SetSentryData(Exception exception, string key, string value)
        {
            if (key != null && !exception.Data.Contains(key))
                exception.Data.Add(key, value);
        }

        // Attempt to write to disk but if it does not work then continue on.
        // Use a mutex to only allow writing from one thread at a time.
        private static void LogToDisk(String message, Dictionary<string, string> additionalData, LogType t = LogType.FULL)
        {
            var prefix = FullLogPrefix;
            if (t == LogType.STATUS)
            {
                LogToDisk(message, additionalData, LogType.FULL);
                prefix = StatusLogPrefix;
            }
            else if (t == LogType.ERROR)
            {
                LogToDisk(message, additionalData, LogType.FULL);
                prefix = ErrorLogPrefix;
            }

            mut.WaitOne();
            try
            {
                var path = getLogPath();
                Directory.CreateDirectory(path);
                using (StreamWriter w = File.AppendText($"{path}/{prefix}-{DateTime.UtcNow.ToString("yyyy-MM-ddTHH-00-00Z")}.txt"))
                {
                    var now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ssZ");
                    w.WriteLine($"{now} : {message}");
                    additionalData.ToList().ForEach((key, val) =>
                       w.WriteLine($"{now} : {key} {val}")
                    );
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                Trace.WriteLine(e.StackTrace);
            }
            mut.ReleaseMutex();
        }
    }
}
