using System.Windows.Controls;

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
        }

        public string GetTitle()
        {
            return "Data Placement";
        }

        public string GetShortTitle()
        {
            return "Placement";
        }
    }
}