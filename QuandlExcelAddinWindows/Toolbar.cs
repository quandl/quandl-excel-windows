using System.Windows;
using Microsoft.Office.Tools.Ribbon;
using Quandl.Shared;
using System.Collections.Generic;

namespace Quandl.Excel.Addin
{
    using Controls;
    using UI;
    public partial class Toolbar
    {

        private void Ribbon2_Load(object sender, RibbonUIEventArgs e)
        {
        }

        private void AboutButton_Click(object sender, RibbonControlEventArgs e)
        {
            new Quandl.Excel.Addin.Controls.AboutForm().Show();
        }

        private void openQuandlSettings_Click(object sender, RibbonControlEventArgs e)
        {
            var quandlSettings = new QuandlSettings();
            // allows toolbar to handle auth token changed events
            quandlSettings.SettingsAuthTokenChanged += Globals.ThisAddIn.OnAuthTokenChangedEvent;
            quandlSettings.SettingsAutoUpdateChanged += Globals.ThisAddIn.OnAutoUpdateChangedEvent;

            // allows quandl settings pane to handle login changed events
            Globals.ThisAddIn.LoginChangedEvent += quandlSettings.UpdateApiKeyTextBox;

            var taskPane = new TaskPaneControl(quandlSettings, "Quandl Settings");
            taskPane.Show();
        }

        private void udfBuilder_Click(object sender, RibbonControlEventArgs e)
        {
            UI.UDF_Builder.WizardGuide child = new UI.UDF_Builder.WizardGuide();
            var taskPane = new TaskPaneControl(child, " ");
            taskPane.Show();
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
