using Microsoft.Office.Tools;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace Quandl.Excel.Addin.UI
{
    class TaskPaneControl
    {
        [DllImport("user32.dll", EntryPoint = "FindWindowW")]
        public static extern System.IntPtr FindWindowW([System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string lpClassName, [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool MoveWindow([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, int X, int Y, int nWidth, int nHeight, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)] bool bRepaint);

        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private CustomTaskPane taskPane;
        private UserControl control;
        private string name;

        public TaskPaneControl(UserControl userControl, string name)
        {
            this.control = userControl;
            this.name = name;
        }

        public TaskPaneControl(System.Windows.Controls.UserControl userControl, string name)
        {
            var controlHost = new TaskPaneWpfControlHost();
            controlHost.WpfElementHost.HostContainer.Children.Add(userControl);
            this.control = controlHost;
            this.name = name;
        }

        public void Show(bool asWindow = false)
        {
            if (asWindow)
            {
                ShowAsWindow();
            }
            else
            {
                ShowAsTaskPane();
            }
        }

        private void ShowAsWindow()
        {
            var window = new Window()
            {
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.CanResizeWithGrip,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                MinHeight = 480,
                MinWidth = 640
            };

            window.Content = control;
            window.Icon = TaskPaneControl.BitmapToImageSource(Quandl.Excel.Addin.Properties.Resources.Quandl_Icon.ToBitmap());
            window.ShowDialog();
        }

        private void ShowAsTaskPane()
        {
            if (taskPane == null)
            {
                taskPane = Globals.ThisAddIn.AddCustomTaskPane(control, name);
            }

            if (!taskPane.Visible)
            {
                taskPane.DockPosition = Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionFloating;
                taskPane.DockPositionRestrict = Microsoft.Office.Core.MsoCTPDockPositionRestrict.msoCTPDockPositionRestrictNoHorizontal;
                taskPane.Width = 640;
                taskPane.Height = 480;
                taskPane.Visible = true;
            }

            // Set it to the center of the screen
            var screen = Screen.FromControl(control);
            SetCustomPanePositionWhenFloating(taskPane, screen.Bounds.Width / 2 - taskPane.Width / 2, screen.Bounds.Height / 2 - taskPane.Height / 2);
        }

        // http://stackoverflow.com/questions/6916402/c-excel-addin-cant-reposition-floating-custom-task-pane
        private void SetCustomPanePositionWhenFloating(CustomTaskPane customTaskPane, int x, int y)
        {
            var oldDockPosition = customTaskPane.DockPosition;
            var oldVisibleState = customTaskPane.Visible;

            customTaskPane.DockPosition = Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionFloating;
            customTaskPane.Visible = true; //The task pane must be visible to set its position

            var window = FindWindowW("MsoCommandBar", customTaskPane.Title); //MLHIDE
            if (window == null) return;

            MoveWindow(window, x, y, customTaskPane.Width, customTaskPane.Height, true);

            customTaskPane.Visible = oldVisibleState;
            customTaskPane.DockPosition = oldDockPosition;
        }
    }
}
