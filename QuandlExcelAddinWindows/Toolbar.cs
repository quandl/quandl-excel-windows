using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Office.Tools.Ribbon;
using Octokit;
using Quandl.Excel.Addin.Controls;
using Quandl.Excel.Addin.UI;
using Quandl.Excel.Addin.UI.Settings;
using Quandl.Excel.Addin.UI.UDF_Builder;
using Quandl.Shared;
using Quandl.Shared.Errors;
using Brushes = System.Windows.Media.Brushes;
using MessageBox = System.Windows.MessageBox;

namespace Quandl.Excel.Addin
{
    public partial class Toolbar
    {
        private readonly WizardGuide _guideChild = new WizardGuide();
        private TaskPaneControl _builderPane;
        private TaskPaneControl _settingsPane;
        private string _releaseBody;
        private string _releaseTag;
        private string _fullFile;

        private void Ribbon2_Load(object sender, RibbonUIEventArgs e)
        {
            SetExecutionToggleIcon();
        }

        private void AboutButton_Click(object sender, RibbonControlEventArgs e)
        {
            new AboutForm().Show();
        }

        private void openQuandlSettings_Click(object sender, RibbonControlEventArgs e)
        {
            if (_settingsPane == null)
            {
                _settingsPane = new TaskPaneControl(new Settings(), "Settings");
            }
            _settingsPane.Show();
        }

        private void udfBuilder_Click(object sender, RibbonControlEventArgs e)
        {
            if (_builderPane == null)
            {
                _builderPane = new TaskPaneControl(_guideChild, "Quandl Formula Builder");
            }
            _guideChild.Reset();
            _guideChild.Background = Brushes.White;
            _guideChild.Margin = new Thickness(0);
            _guideChild.Padding = new Thickness(0);
            _builderPane.Show();
        }

        private void btnStopAll_Click(object sender, RibbonControlEventArgs e)
        {
            QuandlConfig.StopCurrentExecution = true;
        }

        public void SetExecutionToggleIcon()
        {
            if (QuandlConfig.PreventCurrentExecution)
            {
                btnFormulaToggleSplit.Image = Properties.Resources.formulas_disabled;
                btnEnableFormula.Enabled = true;
                btnDisableFormula.Enabled = false;
            }
            else
            {
                btnFormulaToggleSplit.Image = Properties.Resources.formulas_enabled;
                btnEnableFormula.Enabled = false;
                btnDisableFormula.Enabled = true;
            }
            if (QuandlConfig.CheckUpdateAtStart)
                CheckUpdate();
        }

        public void CloseBuilder()
        {
            _builderPane.Close();
        }

        private void btnRefreshWorkSheet_Click(object sender, RibbonControlEventArgs e)
        {
            var activeSheet = Globals.ThisAddIn.Application.ActiveSheet;

            try
            {
                FunctionUpdater.RecalculateQuandlFunctions(activeSheet);
            }
            catch (MissingFormulaException ex)
            {
                Globals.ThisAddIn.UpdateStatusBar(ex);
            }
        }

        private void btnRefreshWorkbook_Click(object sender, RibbonControlEventArgs e)
        {
            var activeWorkBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            try
            {
                FunctionUpdater.RecalculateQuandlFunctions(activeWorkBook);
            }
            catch (MissingFormulaException ex)
            {
                Globals.ThisAddIn.UpdateStatusBar(ex);
            }
        }

        private void btnEnableFormula_Click(object sender, RibbonControlEventArgs e)
        {
            QuandlConfig.PreventCurrentExecution = !QuandlConfig.PreventCurrentExecution;
            SetExecutionToggleIcon();
        }

        private void btnDisableFormula_Click(object sender, RibbonControlEventArgs e)
        {
            QuandlConfig.PreventCurrentExecution = !QuandlConfig.PreventCurrentExecution;
            SetExecutionToggleIcon();
        }

        private void btnUpgrade_Click(object sender, RibbonControlEventArgs e)
        {
            if (IsUpdateAvailable())
            {
                btnCheckUpdate.Image = Quandl.Excel.Addin.Properties.Resources.update_available;
                GetLastestUpdate();
            }
        }

        private void GetLastestUpdate()
        {
            MessageBoxResult result = MessageBox.Show(Properties.Settings.Default.CheckUpdatesDownloadConfirmation,
                                                      Properties.Settings.Default.CheckUpdatesDownloadConfirmationTitle,
                                                      MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                //var dialog = new FolderBrowserDialog();
                //DialogResult dialogResult = dialog.ShowDialog();
                //if (dialogResult == DialogResult.OK)
                //{
                //    string requestPath = Properties.Settings.Default.CheckUpdateDownloadLink;
                //    string fullFile = dialog.SelectedPath + "\\" + GetFileNameFrom(new Uri(requestPath));

                //    DownloadSync(requestPath, fullFile);
                //}

                string requestPath = Properties.Settings.Default.CheckUpdateDownloadLink;
                _fullFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}" + "\\TMP\\"  + GetFileNameFrom(new Uri(requestPath));
                string fullFile = @"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}" + "\\TMP\\" + GetFileNameFrom(new Uri(requestPath));

                DownloadSync(requestPath, fullFile);


            }
        }

        public void DownloadSync(string requestPath, string fileName)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Download_Completed);
            webClient.DownloadFileAsync(new Uri(requestPath), fileName);
        }

        private void Download_Completed(object s, AsyncCompletedEventArgs e)
        {
            MessageBox.Show(Properties.Settings.Default.CheckUpdateDownloadComplete);

            //Process.Start(_fullFile);
            //Process.Start($"C:\\Users\\Developer\\Downloads\\Quandl-Excel-Addin-latest.exe");
            ThreadPool.QueueUserWorkItem(
                delegate { Process.Start($"C:\\Users\\Developer\\Downloads\\Quandl-Excel-Addin-latest.exe"); });
            Utilities.Application.Quit();
        }

        private bool CheckUpdate()
        {
            if (IsUpdateAvailable())
            {
                btnCheckUpdate.Image = Properties.Resources.update_available;
                btnCheckUpdate.ScreenTip = Properties.Settings.Default.CheckUpdateNewUpatateAvailable;
                return true;
            }
            else
            {
                btnCheckUpdate.Image = Properties.Resources.update_check;
                btnCheckUpdate.ScreenTip = Properties.Settings.Default.CheckUpdateNoUpatateAvailable;
                return false;
            }

        }

        private string GetFileNameFrom(Uri uri)
        {
            return Path.GetFileName(uri.LocalPath);
        }

        private bool IsUpdateAvailable()
        {
            var client = new GitHubClient(new ProductHeaderValue("Quandl-Excel-Addin"));
            var releases = client.Repository.Release.GetAll("quandl", "quandl-python");
            var latest = releases.Result[0];
            _releaseBody = latest.Body;
            _releaseTag = latest.TagName;
            var result = latest.Id > Utilities.GithubReleaseId && !latest.Prerelease && !latest.Draft;

            Random r = new Random();
            int n = r.Next();
            return n % 2 == 0;
        }

        private void btnCheckUpdate_Click(object sender, RibbonControlEventArgs e)
        {
            bool isUpdateAvailable = CheckUpdate();
            if (isUpdateAvailable)
            {
                GetLastestUpdate();
            }
            else
            {
                MessageBox.Show(Properties.Settings.Default.CheckUpdateNoUpatateAvailable);
            }
        }

        private void btnViewChangeLog_Click(object sender, RibbonControlEventArgs e)
        {
            //TODO use releasse body
            MessageBox.Show(_releaseBody, String.Format(Properties.Settings.Default.CheckUpdatesReleaseNoteTitle, _releaseTag));
        }

        private void btnViewAll_Click(object sender, RibbonControlEventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Settings.Default.CheckUpdatesReleaseUrl);
        }
    }
}