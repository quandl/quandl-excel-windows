using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using MoreLinq;
using System.Linq;
using System.Threading;

namespace Quandl.Shared.Helpers
{
    public class Logger
    {
        public enum LogType {FULL, STATUS, NOSENTRY, ERROR};

        public const string LogPath = @"Quandl\Excel\logs";

        private const bool ENABLE_SENTRY_LOG = true;
        private const bool ENABLE_DISK_LOG = true;

        private const string FullLogPrefix = "quandl";
        private const string StatusLogPrefix = "status";
        private const string ErrorLogPrefix = "error";

        private static Mutex mut = new Mutex();

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

            log(e.Message, additionalData, LogType.ERROR);
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
                Directory.CreateDirectory(LogPath);
                using (StreamWriter w = File.AppendText($"{LogPath}/{prefix}-{DateTime.UtcNow.ToString("yyyy-MM-ddTHH-00-00Z")}.txt"))
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
