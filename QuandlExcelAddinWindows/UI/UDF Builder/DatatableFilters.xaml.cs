using System.Windows.Controls;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    ///     Interaction logic for DatatableFilters.xaml
    /// </summary>
    public partial class DatatableFilters : UserControl, WizardUIBase
    {
        public DatatableFilters()
        {
            InitializeComponent();
        }

        public string GetTitle()
        {
            return "Filter Data";
        }

        public string GetShortTitle()
        {
            return "Filters";
        }
    }
}