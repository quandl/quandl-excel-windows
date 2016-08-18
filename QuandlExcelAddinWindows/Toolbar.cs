using Microsoft.Office.Tools.Ribbon;
using Quandl.Excel.Addin.Controls;
using Quandl.Excel.Addin.UI;
using Quandl.Excel.Addin.UI.Settings;
using Quandl.Excel.Addin.UI.UDF_Builder;
using Quandl.Shared;
using Quandl.Shared.Errors;

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
            _builderPane.Show();
        }

        private void refreshWorkbook_Click(object sender, RibbonControlEventArgs e)
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

        private void refreshWorksheet_Click(object sender, RibbonControlEventArgs e)
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

        private void refreshMulti_Click(object sender, RibbonControlEventArgs e)
        {
            refreshWorksheet_Click(sender, e);
        }

        private void btnStopAll_Click(object sender, RibbonControlEventArgs e)
        {
            QuandlConfig.StopCurrentExecution = true;
        }

        private void btnExecutionToggle_Click(object sender, RibbonControlEventArgs e)
        {
            var prevent = QuandlConfig.PreventCurrentExecution;
            if (prevent)
            {
                QuandlConfig.PreventCurrentExecution = false;
            }
            else
            {
                QuandlConfig.PreventCurrentExecution = true;
            }
            SetExecutionToggleIcon();
        }

        public void SetExecutionToggleIcon()
        {
            var prevent = QuandlConfig.PreventCurrentExecution;
            if (prevent)
            {
                btnExecutionToggle.OfficeImageId = "SkipOccurrence";
            }
            else
            {
                btnExecutionToggle.OfficeImageId = "FileStartWorkflow";
            }
        }

        public void CloseBuilder()
        {
            _builderPane.Close();
        }
    }
}