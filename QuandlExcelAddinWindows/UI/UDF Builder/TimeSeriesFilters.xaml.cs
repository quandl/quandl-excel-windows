using System.Windows.Controls;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    ///     Interaction logic for TimeSeriesFilters.xaml
    /// </summary>
    public partial class TimeSeriesFilters : UserControl, WizardUIBase
    {
        public TimeSeriesFilters()
        {
            InitializeComponent();
        }

        public string GetTitle()
        {
            return "Customize Time Series Data";
        }

        public string GetShortTitle()
        {
            return "Filters";
        }
    }
}