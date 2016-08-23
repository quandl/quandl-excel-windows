using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quandl.Shared.Excel
{
    public class FunctionGrimReaper
    {
        private const int ReapingWaitMs = 300;

        public static FunctionGrimReaper Instance => _instance ?? (_instance = new FunctionGrimReaper());

        private static Application _application;
        private static Mutex ThreadAccess = new Mutex();
        private static Thread _reaper;

        private static List<Thread> _runningThreads = new List<Thread>();
        private static FunctionGrimReaper _instance;

        public static void AddNewThread(Thread t)
        {
            ThreadAccess.WaitOne();
            _runningThreads.Add(t);
            ThreadAccess.ReleaseMutex();
        }

        public static void BeginTheReaping(Application application)
        {
            _application = application;
            if(_reaper != null)
            {
                return;
            }

            _reaper = new Thread(Reap);
            _reaper.Start();
        }

        private static void Reap()
        {
            // Loop forever to kill off threads when requested.
            while (true)
            {
                // Figure out if we need to reap existing execution threads.
                var shouldReap = QuandlConfig.StopCurrentExecution;

                // If we should not reap then simply wait a given period and check again.
                if (!shouldReap)
                {
                    Thread.Sleep(ReapingWaitMs);
                    continue;
                }
                else
                {
                    ReapThreads();
                }
            }
        }

        private static void ReapThreads()
        {
            ThreadAccess.WaitOne();

            try
            {
                // Add a message indicating the formula's are stopping.
                var statusBar = new Shared.Excel.StatusBar(_application);
                statusBar.AddMessage("Stopping all Quandl data downloads.");

                // Kill and remove from the queue all running threads.

                _runningThreads.ForEach(t =>
                {
                    t.Abort();
                });
                _runningThreads.Clear();

                // All threads are killed so reset registry option.
                QuandlConfig.StopCurrentExecution = false;

                // Add a message to indicate the formula's have stopped.
                statusBar.AddMessage("Quandl downloads stopped.");
            }
            catch(Exception e)
            {
                var statusBar = new Shared.Excel.StatusBar(_application);
                statusBar.AddException(e);
                Utilities.LogToSentry(e);
            }
            finally
            {
                ThreadAccess.ReleaseMutex();
            }
        }
    }
}
