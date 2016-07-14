using System.Windows.Controls;
using Microsoft.Office.Interop.Excel;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    ///     Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class FormulaInserter : UserControl, WizardUIBase
    {
        public FormulaInserter()
        {
            InitializeComponent();

            Loaded +=
                delegate
                {
                    DisplayRangeSelection(Globals.ThisAddIn.ActiveCells);
                    Globals.ThisAddIn.ActiveCellChangedEvent +=
                        delegate(Range target) { DisplayRangeSelection(target); };
                };
        }

        public string GetTitle()
        {
            return "Data Placement";
        }

        public string GetShortTitle()
        {
            return "Placement";
        }

        private void DisplayRangeSelection(Range target)
        {
            SelectedCellTextbox.Text = target != null
                ? $"{target.Worksheet.Name}!{target.Cells[1, 1].Address}"
                : "No cells selected";
        }
    }
}