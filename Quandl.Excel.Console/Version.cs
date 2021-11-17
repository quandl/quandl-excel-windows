using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Quandl.Excel.Console
{
    public class Version
    {
        private static string AddinPackageString = "Quandl.Excel.UDF.Functions";
        private static string BaseValue = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Nasdaq\DataLink Addin";
        private string ValueFor32 = $@"/R ""{BaseValue}\{AddinPackageString}32.xll""";
        private string ValueFor64 = $@"/R ""{BaseValue}\{AddinPackageString}64.xll""";
        private string ExcelExecutable = "excel.exe";
        private string[] ExcelInstallFolders = new string[8]
        {
            @"C:\Program Files (x86)\Microsoft Office\Office{0}\",
            @"C:\Program Files (x86)\Microsoft Office\root\Office{0}\",
            @"C:\Program Files (x86)\Microsoft Office {0}\Office{0}\",
            @"C:\Program Files (x86)\Microsoft Office {0}\root\Office{0}\",
            @"C:\Program Files\Microsoft Office {0}\Office{0}\",
            @"C:\Program Files\Microsoft Office {0}\root\Office{0}\",
            @"C:\Program Files\Microsoft Office\Office{0}\",
            @"C:\Program Files\Microsoft Office\root\Office{0}\"

        };
        internal string _excelVersion = null;

        public Version(string excelVersion)
        {
            _excelVersion = excelVersion;
        }

        public string AddinPath
        {
            get
            {
                string value;
                if (Is64BitOperatingSystem())
                {
                    value = IsExcel32BitInstall(_excelVersion) ? ValueFor32 : ValueFor64;

                }
                else
                {
                    value = ValueFor32;
                }
                return value;
            }
        }

        private bool IsExcel32BitInstall(string version)
        {

            string folder = null;
            string file = string.Empty;
            foreach (var f in ExcelInstallFolders)
            {
                folder = string.Format(f, version.Replace(".0", ""));
                file = FindExistingExcelFromFolder(folder);
                if (file != string.Empty)
                    break;
            }

            if (file != string.Empty)
            {
                return Introspection.GetPEFormat(file) == Introspection.PEFormat.PE32;
            }


            return false;
        }


        private string FindExistingExcelFromFolder(string folder)
        {
            string result = string.Empty;

            if (Directory.Exists(folder))
            {
                var file = Directory.EnumerateFiles(folder).ToArray().FirstOrDefault(x => x.ToLower().EndsWith(ExcelExecutable));
                if (file != null)
                    result = file;
            }
            
            return result;
        }

        #region Is64BitOperatingSystem (IsWow64Process)
        // http://1code.codeplex.com/SourceControl/changeset/view/39074#842775
        internal static bool Is64BitOperatingSystem()
        {
            if (IntPtr.Size == 8)  // 64-bit programs run only on Win64
            {
                return true;
            }
            else  // 32-bit programs run on both 32-bit and 64-bit Windows
            {
                // Detect whether the current process is a 32-bit process 
                // running on a 64-bit system.
                bool flag;
                return ((DoesWin32MethodExist("kernel32.dll", "IsWow64Process") &&
                    IsWow64Process(GetCurrentProcess(), out flag)) && flag);
            }
        }

        static bool DoesWin32MethodExist(string moduleName, string methodName)
        {
            IntPtr moduleHandle = GetModuleHandle(moduleName);
            if (moduleHandle == IntPtr.Zero)
            {
                return false;
            }
            return (GetProcAddress(moduleHandle, methodName) != IntPtr.Zero);
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule,
            [MarshalAs(UnmanagedType.LPStr)]string procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);

        #endregion
    }
}
