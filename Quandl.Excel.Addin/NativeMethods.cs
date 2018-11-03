using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Quandl.Excel.Addin
{
    static class NativeMethods
    {
        [DllImport("user32.dll", EntryPoint = "FindWindowW")]
        public static extern IntPtr FindWindowW([In] [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
            [In] [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MoveWindow([In] IntPtr hWnd, int X, int Y, int nWidth, int nHeight,
            [MarshalAs(UnmanagedType.Bool)] bool bRepaint);

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        public static System.Drawing.Rectangle GetWindowRectangle(IntPtr handle)
        {
            RECT rect;
            if (GetWindowRect(handle, out rect))
            {
                return new System.Drawing.Rectangle
                {
                    X = rect.Left,
                    Y = rect.Top,
                    Width = rect.Right - rect.Left,
                    Height = rect.Bottom - rect.Top
                };
            }

            return new System.Drawing.Rectangle();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }
    }
}
