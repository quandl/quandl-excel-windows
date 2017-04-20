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
        private Shared.Helpers.Updater _updater;
        public Update(Shared.Helpers.Updater updater)
        {
            var md = new Markdown.Xaml.Markdown();
            
            _updater = updater;
            
            InitializeComponent();

            var contents = _updater.latestRelease.Name + "\r\n" + _updater.latestRelease.Body;

            DocFlow.Document = md.Transform(contents);
            DocFlow.Document.Background = Brushes.White;
            DocFlow.Document.PagePadding = new Thickness(5);
            DocFlow.Document.FontFamily = new FontFamily("Arial");
            DocFlow.Document.FontSize = 14;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // _updater.GetLastestUpdate(); // Download the latest update to their computer.
            Process.Start("https://www.quandl.com/tools/excel");
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
