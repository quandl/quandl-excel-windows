using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;
using Quandl.Excel.Addin.Controls;
using Quandl.Shared;

namespace Quandl.Excel.Addin
{
    using System.Windows.Forms;
    using System.Windows.Forms.Integration;
    public partial class Toolbar
    {
        public static Form frm = new Form();

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

        private void refresh_Click(object sender, RibbonControlEventArgs e)
        {
            var activeWorkBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            FunctionUpdater.RecalculateQuandlFunctions(activeWorkBook);
        }

        private void udfBuilder_Click(object sender, RibbonControlEventArgs e)
        {
            ElementHost host = new ElementHost();
            host.Child = new UI.UDF_Builder.WizardGuide();
            host.Dock = DockStyle.Fill;
            host.AutoSize = true;

            UserControl uc = new UserControl();
            uc.Controls.Add(host);
            uc.Dock = DockStyle.Fill;
            uc.AutoSize = true;
            uc.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            frm.Controls.Clear();
            frm.Controls.Add(uc);
            frm.Dock = DockStyle.Fill;
            frm.AutoSize = true;
            frm.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            frm.MaximizeBox = false;
            frm.Icon = global::Quandl.Excel.Addin.Properties.Resources.Quandl_Icon;
            frm.TopMost = true;
            frm.Show();

            UI.UDF_Builder.WizardGuide child = ((UI.UDF_Builder.WizardGuide)host.Child);
            child.stepFrame.Height = 480;
            child.stepFrame.Width = 640;
        }
    }
}
