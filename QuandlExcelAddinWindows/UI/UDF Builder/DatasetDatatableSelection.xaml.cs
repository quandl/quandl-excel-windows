using System.Windows.Controls;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    /// Interaction logic for DatasetDatatableSelection.xaml
    /// </summary>
    public partial class DatasetDatatableSelection : UserControl, WizardUIBase
    {
        public string getTitle()
        {
            return "Choose your dataset or data table";
        }

        public DatasetDatatableSelection()
        {
            InitializeComponent();
            this.DataContext = StateControl.Instance;
        }
    }
}
