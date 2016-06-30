using System.Windows.Controls;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    /// Interaction logic for TimeSeriesFilters.xaml
    /// </summary>
    public partial class TimeSeriesFilters : UserControl, WizardUIBase
    {
        public string getTitle()
        {
            return "Customize Time Series Data";
        }

        public TimeSeriesFilters()
        {
            InitializeComponent();
        }
    }
}
