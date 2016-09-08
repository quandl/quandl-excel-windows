using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Quandl.Shared.Excel
{
    /**
     * The purpose of this class is to forcibly kill of any of the data pulling threads under certain conditions. 
     */
    public class FunctionGrimReaper
    {
        private const int ReapingWaitMs = 1000;

        public static FunctionGrimReaper Instance => _instance ?? (_instance = new FunctionGrimReaper());

        private static Application _application;
        private static Mutex ThreadAccess = new Mutex();
        private static Thread _reaper;

        private static List<Thread> _runningThreads = new List<Thread>();
        private static FunctionGrimReaper _instance;

        private static IStatusBar StatusBar => new StatusBar();

        public static void AddNewThread(Thread t, Application application)
        {
            // Add a new thread to be reaped.
            ThreadAccess.WaitOne();
            _runningThreads.Add(t);
            ThreadAccess.ReleaseMutex();

            // Start reaping if not already started.
            BeginTheReaping(application);
        }

        public static void EndReaping()
        {
            _application = null;
            if (_reaper != null)
            {
                _reaper.Abort();
            }
        }

        private static void BeginTheReaping(Application application)
        {
            // If a reaper is already running abort it first before creating a new one.
            EndReaping();

            // Set the application
            _application = application;

            // Create a new background low priority reaper.
            _reaper = new Thread(Reap);
            _reaper.IsBackground = true;
            _reaper.Priority = ThreadPriority.BelowNormal;
            _reaper.Start();
        }

        private static void Reap()
        {
            // Loop forever to kill off threads when requested.
            var running = true;
            while (running)
            {
                // If we should not reap then simply wait a given period and check again.
                if (!ShouldReap())
                {
                    Thread.Sleep(ReapingWaitMs);
                    continue;
                }
                else
                {
                    running = !ReapThreads();
                }
            }

            // Remove this iteration of the reaper so a new one can be created.
            EndReaping();
        }

        // Figure out if we need to reap existing execution threads.
        // 1. The user asked to stop execution
        // 2. All running threads have finished.
        private static bool ShouldReap()
        {
            return QuandlConfig.StopCurrentExecution || _runningThreads.Where(t => t.IsAlive).Count() == 0;
        }

        private static bool ReapThreads()
        {
            ThreadAccess.WaitOne();

            try
            {
                // Add a message indicating the formula's are stopping.
                StatusBar.AddMessage(Locale.English.DownloadStopping);

                // Kill and remove from the queue all running threads.
                _runningThreads.ForEach(t => t.Abort());
                _runningThreads.Clear();

                // All threads are killed so reset registry option.
                QuandlConfig.StopCurrentExecution = false;

                // Add a message to indicate the formula's have stopped.
                StatusBar.AddMessage(Locale.English.DownloadStopped);
            }
            catch(Exception e)
            {
                StatusBar.AddException(e);
                Utilities.LogToSentry(e);
                return false;
            }
            finally
            {
                ThreadAccess.ReleaseMutex();
            }

            // All threads reaped.
            return true;
        }
    }
}
