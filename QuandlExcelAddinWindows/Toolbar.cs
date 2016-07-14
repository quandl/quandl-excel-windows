using Microsoft.Office.Tools.Ribbon;
using Quandl.Excel.Addin.Controls;
using Quandl.Excel.Addin.UI;
using Quandl.Excel.Addin.UI.Settings;
using Quandl.Excel.Addin.UI.UDF_Builder;
using Quandl.Shared;

namespace Quandl.Excel.Addin
{
    public partial class Toolbar
    {
        private readonly WizardGuide _guideChild = new WizardGuide();
        private TaskPaneControl _builderPane;
        private TaskPaneControl _settingsPane;

        private void Ribbon2_Load(object sender, RibbonUIEventArgs e)
        {
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