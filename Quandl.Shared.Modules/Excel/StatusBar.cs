using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using Quandl.Shared.Helpers;

namespace Quandl.Shared.Excel
{
    public class StatusBar : IStatusBar
    {
        private const int RetryWaitTimeMs = 500;
        private const int MaximumRetries = 10;

        private Application _application;

        public StatusBar()
        {
            try
            {
                try
                {
                    // There is a potential issue where this will get the `background` excel if one is running. In which case you may not see status messages display.
                    _application = (Application)Marshal.GetActiveObject("Excel.Application");
                }
                catch (COMException)
                {
                    _application = new Application();
                }
            }
            catch (System.Exception e)
            {
                Logger.log(e, null, Logger.LogType.NOSENTRY);
            }
        }

        // Be sure to cleanup any references to excel COM objects that may exist.
        ~StatusBar()
        {
            _application = null;
        }

        public void AddException(System.Exception error)
        {
            AddMessage("⚠ Error : " + error.Message);
        }

        // Thread the status bar updates to prevent the main application thread from locking waiting to update the status bar.
        public void AddMessage(string msg)
        {
            try
            {
                Logger.log(msg, null, Logger.LogType.STATUS);
                AddMessageWithoutThreading(msg);
            } 
            catch (System.Exception e)
            {
                Logger.log(e, null, Logger.LogType.NOSENTRY);
            }
        }

        private void AddMessageWithoutThreading(string msg, int retryCount = MaximumRetries)
        {
            // Fail out after maximum retries.
            if (retryCount == 0)
            {
                Logger.log(new System.Exception("Could not update status bar."), new Dictionary<string, string> { { "Message", msg }, { "Retries", MaximumRetries.ToString() } });
                return;
            }

            // Try to display the message otherwise retry or just fail out.
            try
            {
                _application.StatusBar = msg;
            }
            catch (COMException e)
            {
                // Excel is locked atm. Need to wait till its free
                if (e.HResult == Exception.RPC_E_SERVERCALL_RETRYLATER || e.HResult == Exception.VBA_E_IGNORE || e.HResult == Exception.UNSPECIFIED_1)
                {
                    Thread.Sleep(RetryWaitTimeMs);
                    AddMessageWithoutThreading(msg, retryCount - 1);
                    return;
                }
                throw;
            }
        }
    }
}
