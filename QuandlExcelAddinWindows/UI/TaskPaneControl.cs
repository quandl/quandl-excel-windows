using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Microsoft.Office.Core;
using Quandl.Shared;
using CustomTaskPane = Microsoft.Office.Tools.CustomTaskPane;

namespace Quandl.Excel.Addin.UI
{
    internal class TaskPaneControl
    {
        private readonly UserControl control;
        private readonly System.Windows.Controls.UserControl userControl;
        private readonly string name;
        private readonly float _scalingFactor = Utilities.WindowsScalingFactor();

        private CustomTaskPane taskPane;

        public TaskPaneControl(UserControl userControl, string name)
        {
            control = userControl;
            this.name = name;
        }

        public TaskPaneControl(System.Windows.Controls.UserControl userControl, string name)
        {
            var controlHost = new TaskPaneWpfControlHost();
            this.userControl = userControl;
            controlHost.WpfElementHost.HostContainer.Children.Add(userControl);
            control = controlHost;
            this.name = name;
        }

        [DllImport("user32.dll", EntryPoint = "FindWindowW")]
        public static extern IntPtr FindWindowW([In] [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
            [In] [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MoveWindow([In] IntPtr hWnd, int X, int Y, int nWidth, int nHeight,
            [MarshalAs(UnmanagedType.Bool)] bool bRepaint);

        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                var bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public void Show()
        {
            ShowAsTaskPane();
        }

        public void Close()
        {
            if (taskPane != null)
            {
                taskPane.Visible = false;
            }
        }

        private void ShowAsTaskPane()
        {
            if (taskPane == null)
            {
                taskPane = Globals.ThisAddIn.AddCustomTaskPane(control, name);
                // Code for re-sizing the task pane when switching dock types.
                taskPane.DockPositionChanged += delegate
                {
                    var timer = new System.Timers.Timer(100);
                    timer.AutoReset = false;
                    timer.Elapsed += (sender, e) =>
                    {
                        if (taskPane.Width < 700)
                        {
                            taskPane.Width = 700;
                        }
                        if (taskPane.Height < 540)
                        {
                            taskPane.Height = 540;
                        }
                    };
                    timer.Start();
                };
            }

            if (!taskPane.Visible)
            {
                taskPane.DockPosition = MsoCTPDockPosition.msoCTPDockPositionFloating;
                taskPane.DockPositionRestrict = MsoCTPDockPositionRestrict.msoCTPDockPositionRestrictNoHorizontal;
                taskPane.Width = (int)(700 * _scalingFactor);
                taskPane.Height = (int)(540 * _scalingFactor);
                taskPane.Visible = true;
                if (name == "Settings")
                {
                    ((Settings.Settings)userControl).SetSettings();
                }
            }
            
            // Set it to the center of the screen
            var screen = Screen.FromControl(control);
            SetCustomPanePositionWhenFloating(taskPane, screen.Bounds.Width/2 - taskPane.Width/2,
                screen.Bounds.Height/2 - taskPane.Height/2);
        }

        // http://stackoverflow.com/questions/6916402/c-excel-addin-cant-reposition-floating-custom-task-pane
        private void SetCustomPanePositionWhenFloating(CustomTaskPane customTaskPane, int x, int y)
        {
            var oldDockPosition = customTaskPane.DockPosition;
            var oldVisibleState = customTaskPane.Visible;

            customTaskPane.DockPosition = MsoCTPDockPosition.msoCTPDockPositionFloating;
            customTaskPane.Visible = true; //The task pane must be visible to set its position
            
            var window = FindWindowW("MsoCommandBar", customTaskPane.Title); //MLHIDE
            if (window == null) return;

            MoveWindow(window, x, y, customTaskPane.Width, customTaskPane.Height, true);

            customTaskPane.Visible = oldVisibleState;
            customTaskPane.DockPosition = oldDockPosition;
        }
    }
}