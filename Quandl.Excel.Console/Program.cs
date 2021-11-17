using System;
using System.IO;

namespace Quandl.Excel.Console
{
    internal class Program
    {
        // Installation package will handle the case which disable second re-install
        // RegisterExcelAddIn function will handle the case when the Quandl excel add-in registry key created before install
        private static void Main(string[] args)
        {
            try
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
            }
            catch (Exception e)
            {
                using (StreamWriter w = File.AppendText(GetTempPath() + @".\quandl_excel_install_error_log.txt"))
                {
                    Log(e.Message, w);
                    return;
                }
            }
            

            throw new Exception("Could not identify command to use. Please use either `register` or `unregister`.");
        }
        private static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
            w.WriteLine("  :{0}", logMessage);
            w.WriteLine("-------------------------------");
        }
        private static string GetTempPath()
        {
            string path = System.Environment.GetEnvironmentVariable("TEMP");
            if (!path.EndsWith("\\")) path += "\\";
            return path;
        }
    }
}