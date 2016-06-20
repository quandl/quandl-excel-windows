
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
    }
}
