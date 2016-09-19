using System.Windows.Controls;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Diagnostics;

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
                    if (StateControl.Instance.ChainType != StateControl.ChainTypes.TimeSeries)
                    {
                        IncludeHeaders.Visibility = System.Windows.Visibility.Collapsed; 
                    }
                };
        }

        public string GetTitle()
        {
            return "Choose where to place the data";
        }

        public string GetShortTitle()
        {
            return "Placement";
        }

        private void DisplayRangeSelection(Range target)
        {
            SelectedCellTextbox.Text = "Please select one cell to insert your formula into.";

            try
            {
                if (target != null && target.Worksheet != null)
                {
                    SelectedCellTextbox.Text = $"{target.Worksheet.Name}!{target.Cells[1, 1].Address}";
                }
            }
            catch (COMException ex)
            {
                // Ignore no cells being selected error.
                if (ex.HResult == -2146827864)
                {
                    Trace.WriteLine(ex.Message);
                    return;
                }
                throw;
            }
        }
    }
}