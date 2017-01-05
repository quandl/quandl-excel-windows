using System;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using Quandl.Shared.Helpers;

namespace Quandl.Shared.Excel
{
    public class DelayedStatusBar : IStatusBar
    {
        private const int MsgAutoRemovalTimerMs = 30000;
        private const int RetryWaitTimeMs = 1000;
        private const int MaximumRetries = 10;

        private static System.Timers.Timer _statusTimer;
        private Application application;

        public DelayedStatusBar()
        {
            try
            {
                application = (Application)Marshal.GetActiveObject("Excel.Application");
            }
            catch (COMException)
            {
                application = new Application();
            }
        }

        // Be sure to cleanup any references to excel COM objects that may exist.
        ~DelayedStatusBar()
        {
            Cleanup();
        }

        // Thread the status bar updates to prevent the main application thread from locking waiting to update the status bar.
        public void AddMessage(string msg)
        {
            var addMsgThread = new Thread(() => AddMessageWithoutThreading(msg));
            addMsgThread.Priority = ThreadPriority.Lowest;
            addMsgThread.IsBackground = true;
            addMsgThread.Start();
        }

        public void AddException(System.Exception error)
        {
            AddMessage("⚠ Error : " + error.Message);
        }

        private void Cleanup()
        {
            application = null;
            MsgTimerShutdown();
        }

        private void AddMessageWithoutThreading(string msg, int retryCount = MaximumRetries)
        {
            // Fail out after maximum retries.
            if (retryCount == 0)
            {
                Logger.log(new System.Exception("Could not update status bar."), new Dictionary<string, string> { { "Message", msg }, { "Retries", MaximumRetries.ToString() } });
                return;
            }

            try
            {
                application.StatusBar = msg;

                // Clean up an old timers;
                MsgTimerShutdown();

                // Create a new timer to show the error temporarily
                _statusTimer = new System.Timers.Timer(MsgAutoRemovalTimerMs);
                _statusTimer.AutoReset = false;
                _statusTimer.Elapsed += (sender, e) => ResetToDefault();
                _statusTimer.Start();
            }
            catch (COMException e)
            {
                // Excel is locked atm. Need to wait till its free
                if (e.HResult == Exception.RPC_E_SERVERCALL_RETRYLATER || e.HResult == Quandl.Shared.Excel.Exception.VBA_E_IGNORE)
                {
                    Thread.Sleep(RetryWaitTimeMs);
                    AddMessageWithoutThreading(msg, retryCount - 1);
                    return;
                }
                throw;
            }
            catch (NullReferenceException e)
            {
                Logger.log(e);
            }
        }

        private void MsgTimerShutdown()
        {
            if (_statusTimer != null)
            {
                _statusTimer.Stop();
                _statusTimer.Close();
                _statusTimer.Dispose();
            }
        }

        private void ResetToDefault(int retryCount = MaximumRetries)
        {
            if (retryCount == 0)
            {
                return;
            }

            try
            {
                application.StatusBar = false;
                MsgTimerShutdown();
            }
            catch (COMException e)
            {
                // Basically the system is paused due to a user making an update somewhere. Please wait and retry again.
                if (e.HResult == Quandl.Shared.Excel.Exception.VBA_E_IGNORE)
                {
                    Thread.Sleep(RetryWaitTimeMs);
                    ResetToDefault(retryCount - 1);
                    return;
                }
                throw;
            }
        }
    }
}
