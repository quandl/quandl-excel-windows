using System;
using Microsoft.Office.Interop.Excel;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace Quandl.Shared.Excel
{
    public class StatusBar
    {
        private const int MsgAutoRemovalTimerMs = 30000;
        private const int RetryWaitTimeMs = 1000;
        private const int MaximumRetries = 50;

        private static System.Timers.Timer _statusTimer;
        private Application application;

        public StatusBar(Application application)
        {
            this.application = application;
        }

        // Thread the status bar updates to prevent the main application thread from locking waiting to update the status bar.
        public void AddMessage(string msg)
        {
            new Thread(() => AddMessageWithoutThreading(msg, 0)).Start();
        }

        private void AddMessageWithoutThreading(string msg, int retries)
        {
            // Fail out after maximum retries.
            if (retries == MaximumRetries)
            {
                Shared.Utilities.LogToSentry(new Exception("Could not update status bar."), new System.Collections.Generic.Dictionary<string, string> { { "Message", msg }, { "Retries", retries.ToString() } });
                return;
            }

            try
            {
                application.StatusBar = msg;

                // Clean up an old timers;
                if (_statusTimer != null)
                {
                    _statusTimer.Stop();
                    _statusTimer.Close();
                }

                // Create a new timer to show the error temporarily
                _statusTimer = new System.Timers.Timer(MsgAutoRemovalTimerMs);
                _statusTimer.Elapsed += async (sender, e) => await Task.Run(() =>
                {
                    ResetToDefault();
                });
                _statusTimer.Start();
            }
            catch (COMException e)
            {
                // Excel is locked atm. Need to wait till its free
                if (e.HResult == -2147417846 || e.HResult == -2146777998)
                {
                    Thread.Sleep(RetryWaitTimeMs);
                    AddMessageWithoutThreading(msg, retries + 1);
                    return;
                }
                throw;
            }
            catch (NullReferenceException e)
            {
                Utilities.LogToSentry(e);
            }
        }

        public void AddException(Exception error)
        {
            AddMessage("⚠ Error : " + error.Message);
        }

        public void ResetToDefault()
        {
            try
            {
                application.StatusBar = false;
            }
            catch (COMException e)
            {
                // Basically the system is paused due to a user making an update somewhere. Please wait and retry again.
                if (e.HResult == -2146777998)
                {
                    Thread.Sleep(RetryWaitTimeMs);
                    ResetToDefault();
                    return;
                }
                throw;
            }
        }
    }
}
