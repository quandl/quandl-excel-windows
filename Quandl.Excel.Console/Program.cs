using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quandl.Shared;


namespace Quandl.Excel.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 1 && args[0].Equals("r"))
            {
                ExcelHelp.RegisterExcelAddin();
            }
        }
    }
}
