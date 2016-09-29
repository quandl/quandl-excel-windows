using System.Drawing;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;
using Quandl.Excel.Addin.Controls;
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
            _guideChild.Margin = new System.Windows.Thickness(0);
            _guideChild.Padding = new System.Windows.Thickness(0);
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
                btnFormulaToggleSplit.Image = Quandl.Excel.Addin.Properties.Resources.formulas_disabled;
                btnEnableFormula.Enabled = true;
                btnDisableFormula.Enabled = false;
            }
            else
            {
                btnFormulaToggleSplit.Image = Quandl.Excel.Addin.Properties.Resources.formulas_enabled;
                btnEnableFormula.Enabled = false;
                btnDisableFormula.Enabled = true;
            }
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
                FunctionUpdater.RefreshConfirmation = null;
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
                FunctionUpdater.RefreshConfirmation = null;
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
    }
}