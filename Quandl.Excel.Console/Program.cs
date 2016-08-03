using System;

namespace Quandl.Excel.Console
{
    internal class Program
    {
        // Installation package will handle the case which disable second re-install
        // RegisterExcelAddIn function will handle the case when the Quandl excel add-in registry key created before install
        private static void Main(string[] args)
        {
            if (args.Length == 0) return;
            if (args[0].Equals("register"))
            {
                SetupHelp.RegisterExcelAddin();
                return;
            }
            if (args[0].Equals("unregister"))
            {
                SetupHelp.UnRegisterExcelAddin();
                return;
            }

            throw new Exception("Could not identify command to use. Please use either `register` or `unregister`.");
        }
    }
}