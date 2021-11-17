using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Quandl.Excel.Addin.UI
{
    /// <summary>
    /// Interaction logic for Update.xaml
    /// </summary>
    public partial class Update : UserControl
    {
        private System.Func<Shared.Helpers.Updater> _updater;
        public Update(System.Func<Shared.Helpers.Updater> updater)
        {
            
            
            _updater = updater;
            InitializeComponent();

            UpdateContent();

        }

        public void UpdateContent()
        {
            var md = new Markdown.Xaml.Markdown();
            var checkRelease = _updater.Invoke()?.latestRelease;
            var contents = checkRelease==null ? 
                Properties.Resources.CheckUpdateNoUpdateAvailable
                : checkRelease.Name + "\r\n" + checkRelease.Body;

            DocFlow.Document = md.Transform(contents);
            DocFlow.Document.Background = Brushes.White;
            DocFlow.Document.PagePadding = new Thickness(5);
            DocFlow.Document.FontFamily = new FontFamily("Arial");
            DocFlow.Document.FontSize = 14;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // _updater.GetLastestUpdate(); // Download the latest update to their computer.
            Process.Start("https://data.nasdaq.com/tools/excel");
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void All_Release_Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/quandl/quandl-excel-windows/releases");
        }
    }
}
