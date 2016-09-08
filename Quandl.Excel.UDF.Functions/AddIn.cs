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
            // This registers the intellisense server. ATM this plugin is slightly buggy and prone to crashes.
            IntelliSenseServer.Register();

            // Reset the stop execution function incase excel crashed last time.
            QuandlConfig.StopCurrentExecution = false;
        }

        public void AutoClose()
        {
            FunctionGrimReaper.EndReaping();
        }
    }
}