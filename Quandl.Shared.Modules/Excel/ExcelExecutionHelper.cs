using Quandl.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quandl.Shared.Excel
{
    /// <summary>
    /// Single logic to auto-retry any failed attempts to contact Excel
    /// </summary>
    public static class ExcelExecutionHelper
    {
        private static readonly TimeSpan RetryWaitTime = TimeSpan.FromMilliseconds(500);
        private const int MaximumRetries = 5;

        public static void ExecuteWithAutoRetry(Action action)
        {
            ExecuteWithAutoRetry(action, MaximumRetries, RetryWaitTime);

        }
        public static void ExecuteWithAutoRetry(Action action, int retryCount, TimeSpan retryWait)
        {
            if (retryCount <= 1)
            {
                retryCount = 1;
            }
            while(retryCount>0)
            {
                try
                {
                    action.Invoke();
                    return;
                }
                catch (COMException e)
                {
                    retryCount--;
                    // Excel is locked atm. Need to wait till its free
                    if (e.HResult == Exception.RPC_E_SERVERCALL_RETRYLATER || e.HResult == Exception.VBA_E_IGNORE ||
                        e.HResult == Exception.UNSPECIFIED_1)
                    {
                        // do not sleep on the last retry
                        if (retryCount > 0)
                        {
                            Thread.Sleep(retryWait);
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            // if we are here, action has not been executed
            Logger.log(new System.Exception("Could not execute Excel action."), new Dictionary<string, string> { { "StackTrace", new System.Diagnostics.StackTrace().ToString() }, { "Retries", retryCount.ToString() } });

        }
    }
}
