using System;
using Microsoft.Office.Interop.Excel;
using System.Timers;
using System.Threading.Tasks;

namespace Quandl.Shared.Excel
{
    public class StatusBar
    {
        private static Timer _statusTimer;
        private Application application;
        private readonly int TIMER_DELAY = 20000;

        public StatusBar(Application application)
        {
            this.application = application;
        }

        public void AddMessage(string msg)
        {
            var oldStatusBarVisibility = application.DisplayStatusBar;
            application.DisplayStatusBar = true;
            application.StatusBar = msg;

            // Clean up an old timers;
            if (_statusTimer != null)
            {
                _statusTimer.Stop();
                _statusTimer.Close();
            }

            // Create a new timer to show the error temporarily
            _statusTimer = new Timer(TIMER_DELAY);
            _statusTimer.Elapsed += async (sender, e) => await Task.Run(() =>
            {
                application.StatusBar = false;
                application.DisplayStatusBar = oldStatusBarVisibility;
            });
            _statusTimer.Start();
        }

        public void AddException(Exception error)
        {
            AddMessage("⚠ Quandl plugin error: " + error.Message);
        }
    }
}
