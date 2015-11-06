using Microsoft.Office.Tools.Ribbon;
using Quandl.Excel.Addin.Controls;
using Quandl.Shared;

namespace Quandl.Excel.Addin
{
    using System.Windows;
    using System.Windows.Forms;

    public partial class Toolbar
    {
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
    }
}
