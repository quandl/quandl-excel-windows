using System;
using Microsoft.Office.Interop.Excel;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace Quandl.Shared.Excel
{
    public class StatusBar
    {
        private static System.Timers.Timer _statusTimer;
        private Application application;
        private readonly int TIMER_DELAY = 20000;

        // Retry wait if excel is busy
        private const int RetryWaitTimeMs = 500;

        // Try really hard to get the instance of the status bar from the application.
        public static StatusBar Instance(object application)
        {
            try
            {
                return new StatusBar((Microsoft.Office.Interop.Excel.Application)application);
            }
            catch (COMException e)
            {
                // The excel RPC server is busy. We need to wait and then retry (RPC_E_SERVERCALL_RETRYLATER)
                if (e.HResult == -2147417846)
                {
                    Thread.Sleep(RetryWaitTimeMs);
                    return Instance(application);
                }

                throw;
            }
        }

        public StatusBar(Application application)
        {
            this.application = application;
        }

        public void AddMessage(string msg)
        {
            var oldStatusBarVisibility = application.DisplayStatusBar;
            application.StatusBar = msg;

            // Clean up an old timers;
            if (_statusTimer != null)
            {
                _statusTimer.Stop();
                _statusTimer.Close();
            }

            // Create a new timer to show the error temporarily
            _statusTimer = new System.Timers.Timer(TIMER_DELAY);
            _statusTimer.Elapsed += async (sender, e) => await Task.Run(() =>
            {
                ResetToDefault();
            });
            _statusTimer.Start();
        }

        public void AddException(Exception error)
        {
            AddMessage("⚠ Quandl plugin error: " + error.Message);
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
