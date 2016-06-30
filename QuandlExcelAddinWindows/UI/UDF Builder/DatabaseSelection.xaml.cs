using System.Windows.Controls;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{

    /// <summary>
    /// Interaction logic for DatabaseSelection.xaml
    /// </summary>
    public partial class DatabaseSelection : UserControl, WizardUIBase
    {
        public string getTitle()
        {
            return "Browse Databases or Enter a Database Code";
        }

        public DatabaseSelection()
        {
            InitializeComponent();
            this.DataContext = StateControl.Instance;
        }

        private void dataCode_LostFocus(object sender, System.EventArgs e)
        {
            StateControl.Instance.ChangeCode(((TextBox)sender).Text, StateControl.ChainTypes.TimeSeries);
        }
    }
}
