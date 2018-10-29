using System;
using ExcelDna.Integration;
using ExcelDna.IntelliSense;
using Quandl.Shared;
using Quandl.Shared.Excel;

namespace Quandl.Excel.UDF.Functions
{
    public class AddIn : IExcelAddIn
    {
        public void AutoOpen()
        {
            System.Diagnostics.Debug.Assert(false);

            Shared.Helpers.HttpHelper.EnableTlsSupport();
            Shared.Globals.Instance.HostService = new ExcelDnaHostService();
            // This registers the intellisense server. ATM this plugin is slightly buggy and prone to crashes.
            IntelliSenseServer.Register();

            // Reset the stop execution function incase excel crashed last time.
            QuandlConfig.StopCurrentExecution = false;

            
        }

        class ExcelDnaHostService : IHostService
        {
            public void SetStatusBar(string message)
            {
                ((dynamic) ExcelDnaUtil.Application).StatusBar = message;
            }
        }

        public void AutoClose()
        {
            FunctionGrimReaper.EndReaping();
        }
    }
}