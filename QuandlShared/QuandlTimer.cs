using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Quandl.Shared
{
    public class QuandlTimer
    {
        private static QuandlTimer _instance;

        private QuandlTimer() {}

        public static QuandlTimer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new QuandlTimer();
                }
                return _instance;
            }
        }

        private Timer qTimer;
        private const int RetryIntervalSeconds = 20;

        public void SetupAutoRefreshTimer(ElapsedEventHandler eventHandler)
        {
            // clean up any previous timer
            DisableUpdateTimer();

            // Don't update with timer
            if (QuandlConfig.AutoUpdateFrequency <= 0)
            {
                return;
            }

            qTimer = new Timer();
            qTimer.Elapsed += eventHandler;
            qTimer.Interval = TimeOutInterval();
            qTimer.Enabled = true;
        }

        public void DisableUpdateTimer()
        {
            if (qTimer == null) return;
            qTimer.Enabled = false;
            qTimer.Dispose();
            qTimer = null;
        }

        public void SetTimeoutInterval(bool configureRetryInterval)
        {
            var interval = TimeOutInterval();
            if (configureRetryInterval)
            {
                interval = RetryTimeoutInterval();
            }
            SetInterval(interval);
        }

        private void SetInterval(double interval)
        {
            if (qTimer.Interval.Equals(interval)) return;
            qTimer.Stop();
            qTimer.Interval = interval;
            qTimer.Start();
        }

        private double TimeOutInterval()
        {
            return TimeSpan.FromDays(QuandlConfig.AutoUpdateFrequency).TotalMilliseconds;
        }

        private double RetryTimeoutInterval()
        {
            return TimeSpan.FromSeconds(RetryIntervalSeconds).TotalMilliseconds;
        }
    }
}
