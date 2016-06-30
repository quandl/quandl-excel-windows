using System.Windows.Controls;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    /// Interaction logic for ColumnSelection.xaml
    /// </summary>
    public partial class ColumnSelection : UserControl, WizardUIBase
    {
        public string getTitle()
        {
            return "Choose Your Columns";
        }

        public ColumnSelection()
        {
            InitializeComponent();
        }
    }
}
