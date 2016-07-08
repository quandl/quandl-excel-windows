using System.Windows.Controls;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    ///     Interaction logic for DatasetDatatableSelection.xaml
    /// </summary>
    public partial class DatasetDatatableSelection : UserControl, WizardUIBase
    {
        public DatasetDatatableSelection()
        {
            InitializeComponent();
            DataContext = StateControl.Instance;
        }

        public string GetTitle()
        {
            return "Choose your dataset or data table";
        }

        public string GetShortTitle()
        {
            return "Data";
        }
    }
}