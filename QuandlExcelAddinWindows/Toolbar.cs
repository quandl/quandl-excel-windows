using System.Windows;
using Microsoft.Office.Tools.Ribbon;
using Quandl.Excel.Addin.UI;
using Quandl.Excel.Addin.UI.Settings;
using Quandl.Excel.Addin.UI.UDF_Builder;
using Quandl.Shared;
using Quandl.Shared.Errors;
using Brushes = System.Windows.Media.Brushes;

namespace Quandl.Excel.Addin
{
    public partial class Toolbar
    {
        private readonly WizardGuide _guideChild = new WizardGuide();
        private TaskPaneControl _builderPane;
        private TaskPaneControl _settingsPane;
        private Shared.Helpers.Updater _updater = new Shared.Helpers.Updater();

        private void Ribbon2_Load(object sender, RibbonUIEventArgs e)
        {
            SetExecutionToggleIcon();
        }

        private void AboutButton_Click(object sender, RibbonControlEventArgs e)
        {
            new TaskPaneControl(new About(), "About").Show(400, 600);
        }

        private void openQuandlSettings_Click(object sender, RibbonControlEventArgs e)
        {
            if (_settingsPane == null)
            {
                _settingsPane = new TaskPaneControl(new Settings(), "Settings");
            }
            _settingsPane.Show(400, 600);
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

        private void btnCheckUpdate_Click(object sender, RibbonControlEventArgs e)
        {
            new TaskPaneControl(new Update(_updater), "New Updates Available!").Show(450, 640);
        }

        private void CheckUpdate()
        {
            if (_updater.UpdateAvailable)
            {
                btnCheckUpdate.Visible = true;
            }
        }
    }
}