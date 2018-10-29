using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using Quandl.Shared.Helpers;

namespace Quandl.Shared.Excel
{
   /// <summary>
   /// Handle status bar changes through <see cref="IHostService"/> adapter
   /// </summary>
    public class StatusBar : IStatusBar
    {
        


        private readonly IHostService _hostService;
        public StatusBar(IHostService hostService)
        {
            _hostService = hostService;
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
                ExcelExecutionHelper.ExecuteWithAutoRetry(()=>_hostService.SetStatusBar(msg));
            } 
            catch (System.Exception e)
            {
                Logger.log(e, null, Logger.LogType.NOSENTRY);
            }
        }
        
    }
}
