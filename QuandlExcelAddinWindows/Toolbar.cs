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
        private TaskPaneControl builderPane;
        private TaskPaneControl settingsPane;

        private void Ribbon2_Load(object sender, RibbonUIEventArgs e)
        {
        }

        private void AboutButton_Click(object sender, RibbonControlEventArgs e)
        {
            new AboutForm().Show();
        }

        private void openQuandlSettings_Click(object sender, RibbonControlEventArgs e)
        {
            if (settingsPane == null)
            {
                var child = new Settings();
                settingsPane = new TaskPaneControl(child, "Settings");
                settingsPane.Show();
            }
            else
            {
                settingsPane.Show();
            }
        }

        private void udfBuilder_Click(object sender, RibbonControlEventArgs e)
        {
            if (builderPane == null)
            {
                var child = new WizardGuide();
                builderPane = new TaskPaneControl(child, "Quandl UDF Builder");
                builderPane.Show();
            }
            else
            {
                builderPane.Show();
            }
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