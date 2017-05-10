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
        public enum LogType {FULL, STATUS, NOSENTRY};

        public const string LogPath = @"Quandl\Excel\logs";

        private const bool ENABLE_SENTRY_LOG = true;
        private const bool ENABLE_DISK_LOG = true;

        private const string FullLogPrefix = "quandl";
        private const string StatusLogPrefix = "status";

        private static Mutex mut = new Mutex();

        public static void log(string message, Dictionary<string, string> additionalData = null, LogType t = LogType.FULL)
        {
            log(new Exception(message), additionalData, t);
        }

        public static void log(Exception e, Dictionary<string, string> additionalData = null, LogType t = LogType.FULL)
        {
            Trace.WriteLine(e.Message);
            Trace.WriteLine(e.StackTrace);

            // Add the stack trace into the additional data if one is available
            if (additionalData == null)
            {
                additionalData = new Dictionary<string, string>() { };
            }
            if(e.StackTrace != null && e.StackTrace.Length > 0) {
                additionalData["StackTrace"] = e.StackTrace;
            }

            // Write to sentry logging if applicable
            // TODO it look like in some strange case excel status case exception with normal status info
            if (ENABLE_SENTRY_LOG && ( t != LogType.NOSENTRY || t != LogType.STATUS || !e.Message.ToLower().Contains("successfully") || !e.Message.ToLower().Contains("retrieving data")))
            {
                LogToSentry(e, additionalData);
            }

            // Write to disk logging if applicable
            if (ENABLE_DISK_LOG)
            {
                LogToDisk(e, additionalData, t);
            }
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
        private static void LogToDisk(Exception exception, Dictionary<string, string> additionalData, LogType t = LogType.FULL)
        {
            mut.WaitOne();
            try
            {
                var prefix = FullLogPrefix;
                if (t == LogType.STATUS)
                {
                    prefix = StatusLogPrefix;
                }

                Directory.CreateDirectory(LogPath);
                using (StreamWriter w = File.AppendText($"{LogPath}/{prefix}-{DateTime.UtcNow.ToString("yyyy-MM-ddTHH-00-00Z")}.txt"))
                {
                    var now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ssZ");
                    w.WriteLine($"{now} : {exception.Message}");
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
