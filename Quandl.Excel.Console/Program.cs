using Quandl.Shared;

namespace Quandl.Excel.Console
{
    class Program
    {
        // Installation package will handl the case which disable second re-install
        // RegisterExcelAddiin function will handle the case when the quandl excel add-in registry key created before install
        static void Main(string[] args)
        {
            if (args.Length >= 1 && args[0].Equals("r"))
            {
                SetupHelp.RegisterExcelAddin();
            }
        }
    }
}
