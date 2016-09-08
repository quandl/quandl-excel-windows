using System;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;

namespace Quandl.Shared.Excel
{
    public class StatusBar : IStatusBar
    {
        private const int RetryWaitTimeMs = 1000;
        private const int MaximumRetries = 10;

        private Application _application;

        public StatusBar()
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

        // Be sure to cleanup any references to excel COM objects that may exist.
        ~StatusBar()
        {
            _application = null;
        }

        // Thread the status bar updates to prevent the main application thread from locking waiting to update the status bar.
        public void AddMessage(string msg)
        {
            AddMessageWithoutThreading(msg);
        }

        public void AddException(Exception error)
        {
            AddMessage("⚠ Error : " + error.Message);
        }

        private void AddMessageWithoutThreading(string msg, int retryCount = MaximumRetries)
        {
            // Fail out after maximum retries.
            if (retryCount == 0)
            {
                Utilities.LogToSentry(new Exception("Could not update status bar."), new Dictionary<string, string> { { "Message", msg }, { "Retries", MaximumRetries.ToString() } });
                return;
            }

            try
            {
                _application.StatusBar = msg;
            }
            catch (COMException e)
            {
                // Excel is locked atm. Need to wait till its free
                if (e.HResult == -2147417846 || e.HResult == -2146777998 || e.HResult == -2146827284)
                {
                    Thread.Sleep(RetryWaitTimeMs);
                    AddMessageWithoutThreading(msg, retryCount - 1);
                    return;
                }
                throw;
            }
            catch (NullReferenceException e)
            {
                Utilities.LogToSentry(e);
            }
        }
    }
}
