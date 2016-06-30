using System.Windows.Controls;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    /// Interaction logic for DatatableFilters.xaml
    /// </summary>
    public partial class DatatableFilters : UserControl, WizardUIBase
    {
        public string getTitle()
        {
            return "Filter Data";
        }

        public DatatableFilters()
        {
            InitializeComponent();
        }
    }
}
