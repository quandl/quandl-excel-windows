using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Quandl.Excel.Console
{
    public class Version
    {
        private static string AddinPackageString = "Quandl.Excel.UDF.Functions-AddIn.xll";
        private static string BaseValue = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Quandl";
        private string ValueFor32 = $@"/R ""{BaseValue}\{AddinPackageString}""";
        private string ValueFor64 = $@"/R ""{BaseValue}\64bit\{AddinPackageString}""";
        private string Excel32InstallFolder = @"C:\Program Files (x86)\Microsoft Office\Office{0}\EXCEL.EXE";
        private string AlternativeFolder = @"C:\Program Files (x86)\Microsoft Office\root\Office{0}\EXCEL.EXE";
        internal string _excelVersion = null;

        public Version(string excelVersion)
        {
            _excelVersion = excelVersion;
        }

        public string OpenValue
        {
            get
            {
                string value;
                if (Is64BitOperatingSystem())
                {
                    value = IsExcel32bitInstall(_excelVersion) ? ValueFor32 : ValueFor64;

                }
                else
                {
                    value = ValueFor32;
                }
                return value;
            }
        }

        private bool IsExcel32bitInstall(string version)
        {
            try
            {
                if (File.Exists(string.Format(Excel32InstallFolder, version.Replace(".0", ""))))
                {
                    return true;
                }
                else
                {
                    return File.Exists(string.Format(AlternativeFolder, version.Replace(".0", "")));
                };
            }
            catch
            {
                return false;
            }
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
