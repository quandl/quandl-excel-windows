using Quandl.Shared.Helpers;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

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

        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Logger.getLogPath());
        }
    }
}
