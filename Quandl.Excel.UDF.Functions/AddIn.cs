using System;
using ExcelDna.Integration;
using ExcelDna.IntelliSense;
using Quandl.Shared;

namespace Quandl.Excel.UDF.Functions
{
    public class AddIn : IExcelAddIn
    {
        public void AutoOpen()
        {
            IntelliSenseServer.Register(); // The registers the intellisense server. ATM this is buggy and prone to crashes.
        }

        public void AutoClose()
        {
        }
    }
}