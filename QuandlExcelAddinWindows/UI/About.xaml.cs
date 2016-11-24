using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Quandl.Excel.Addin.UI
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    /// 

    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string Version { get { return Shared.Utilities.ReleaseVersion; } }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
