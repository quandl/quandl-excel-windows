using ExcelDna.Integration;
using Quandl.Shared;
using Quandl.Shared.Errors;
using Quandl.Shared.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Quandl.Excel.UDF.Functions
{
    public class Common
    {
        // Retry wait if excel is busy
        private const int RetryWaitTimeMs = 500;
        private const int MaximumRetries = 20;

        public static IStatusBar StatusBar => StatusBarInstance();

        public static string HandleQuandlError(QuandlErrorBase e, bool reThrow = true, Dictionary<string, string> additionalData = null)
        {
            if (!string.IsNullOrWhiteSpace((e).ErrorCode))
                {
                StatusBar.AddException(e);
                return e.Message;
            }

            // We couldn't figure out how to handle it. Log and explode.
            Trace.WriteLine(e.Message);
            Shared.Utilities.LogToSentry(e, additionalData);

            throw e;
        }

        public static void CheckNoApiKey(string errorCode)
        {
            if ((errorCode == "QEPx05" || errorCode == "QEPx04") && QuandlConfig.ApiKey == "")
            {
                System.Windows.Forms.MessageBox.Show(Locale.English.MessageBoxText, Locale.English.MessageBoxTitle);
            }
        }

        public static string HandlePotentialQuandlError(Exception e, bool reThrow = true, Dictionary<string, string> additionalData = null)
        {

            // If it's detected as a quandl error handle it but don't send out sentry message.
            if (e.GetType() == typeof(QuandlErrorBase))
            {
                return HandleQuandlError((QuandlErrorBase)e, reThrow, additionalData);
            }

            var innerException = e.InnerException;
            if (innerException != null && innerException.GetType() == typeof(QuandlErrorBase))
            {
                QuandlErrorBase exBase = (QuandlErrorBase)e.InnerException.GetBaseException();
                CheckNoApiKey(exBase.ErrorCode);
                return HandleQuandlError((QuandlErrorBase)innerException, reThrow, additionalData);
            }

            // We couldn't figure out how to handle it. Log and explode.
            Trace.WriteLine(e.Message);
            Shared.Utilities.LogToSentry(e, additionalData);
            
            if (reThrow)
            {
                throw e;
            }
            return null;
        }

        // Try really hard to get the instance of the status bar from the application.
        public static IStatusBar StatusBarInstance(int retryCount = MaximumRetries)
        {
            // Ran out of retries
            if (retryCount == 0)
            {
                return new NullStatusBar();
            }

            // Normal status bar access
            try
            {
                return new StatusBar();
            }
            catch (COMException e)
            {
                // The excel RPC server is busy. We need to wait and then retry (RPC_E_SERVERCALL_RETRYLATER)
                if (e.HResult == -2147417846 || e.HResult == -2146777998)
                {
                    Thread.Sleep(RetryWaitTimeMs);
                    return StatusBarInstance(retryCount - 1);
                }

                Utilities.LogToSentry(e);
                return new NullStatusBar();
            }
        }
        
        public static void SetCellVolatile(bool value)
        {
            XlCall.Excel(XlCall.xlfVolatile, value);
        }
    }
}
