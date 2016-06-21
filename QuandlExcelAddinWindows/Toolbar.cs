using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;
using Quandl.Excel.Addin.Controls;
using Quandl.Shared;

namespace Quandl.Excel.Addin
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Forms.Integration;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    public partial class Toolbar
    {
        public static Window frm;

        private void Ribbon2_Load(object sender, RibbonUIEventArgs e)
        {
            UpdateLoginLabel();
            Globals.ThisAddIn.AuthTokenChangedEvent += UpdateLoginLabel;
            Globals.ThisAddIn.LoginChangedEvent += UpdateLoginLabel;
        }

        private void GetDataButton_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.TaskPane_Show();
        }

        private void AboutButton_Click(object sender, RibbonControlEventArgs e)
        {
            new Quandl.Excel.Addin.Controls.AboutForm().Show();
        }

        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
            Form dataSelection = new Form();
            DataTaskPane taskPane = new DataTaskPane(Globals.ThisAddIn.ActiveCells);
            dataSelection.Controls.Clear();
            dataSelection.Controls.Add(taskPane);
            dataSelection.AutoSize = true;
            dataSelection.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            dataSelection.Show();
        }

        private void login_Click(object sender, RibbonControlEventArgs e)
        {
            var loggedIn = string.IsNullOrEmpty(QuandlConfig.ApiKey);
            if (loggedIn)
            {
                var loginForm = new LoginForm();
                loginForm.LoginChanged += Globals.ThisAddIn.OnLoginChangedEvent;
                loginForm.Show();
            }
            else
            {
                QuandlConfig.ApiKey = "";
                Globals.ThisAddIn.OnLoginChangedEvent();

            }
        }

        public void UpdateLoginLabel()
        {
            login.Label = string.IsNullOrEmpty(QuandlConfig.ApiKey) ? "Login" : "Logout";
        }

        private void openQuandlSettings_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.SettingsPane_Show(this);
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
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

        private void udfBuilder_Click(object sender, RibbonControlEventArgs e)
        {
            if (frm == null)
            {
                frm = new Window()
                {
                    SizeToContent = SizeToContent.WidthAndHeight,
                    ResizeMode = ResizeMode.CanResizeWithGrip,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    MinHeight = 480,
                    MinWidth = 640
                };
            }

            UI.UDF_Builder.WizardGuide child = new UI.UDF_Builder.WizardGuide();
            frm.Content = child;

            frm.Icon = BitmapToImageSource(Properties.Resources.Quandl_Icon.ToBitmap());
            frm.ShowDialog();
        }

        private void refreshWorkbook_Click(object sender, RibbonControlEventArgs e)
        {
            var activeWorkBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            FunctionUpdater.RecalculateQuandlFunctions(activeWorkBook);
        }

        private void refreshWorksheet_Click(object sender, RibbonControlEventArgs e)
        {
            var activeSheet = Globals.ThisAddIn.Application.ActiveSheet;
            FunctionUpdater.RecalculateQuandlFunctions(activeSheet);
        }

        private void refreshMulti_Click(object sender, RibbonControlEventArgs e)
        {
            refreshWorkbook_Click(sender, e);
        }
    }
}
