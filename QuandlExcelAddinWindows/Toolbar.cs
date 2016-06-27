using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;
using Quandl.Excel.Addin.Controls;
using Quandl.Shared;

namespace Quandl.Excel.Addin
{
    using Microsoft.Office.Core;
    using System;
    using System.Collections;
    using System.Drawing;
    using System.IO;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Forms.Integration;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using UI.UDF_Builder;
    public partial class Toolbar
    {

        private void Ribbon2_Load(object sender, RibbonUIEventArgs e)
        {
        }

        private async void AboutButton_Click(object sender, RibbonControlEventArgs e)
        {
            var columns = new ArrayList()
            {
                "Date", "Open", "High"
            };
            //ArrayList list = Web.PullRecentStockData("NSE/OIL", columns, 1);
            //var datasets = await Web.SearchDatasetsAsync("NSE", "oil");
            var databases = await Web.SearchDatabasesAsync("NSE");
            new Quandl.Excel.Addin.Controls.AboutForm().Show();
        }

        private void openQuandlSettings_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.SettingsPane_Show(this);
        }

        private void udfBuilder_Click(object sender, RibbonControlEventArgs e)
        {
            UI.UDF_Builder.WizardGuide child = new UI.UDF_Builder.WizardGuide();
            Globals.ThisAddIn.ShowCustomPane(child, " ");
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
