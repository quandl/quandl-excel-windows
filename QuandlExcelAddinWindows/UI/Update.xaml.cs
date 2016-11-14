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

            DocFlow.Document = md.Transform(_updater.latestRelease.Body);
            DocFlow.Document.Background = Brushes.White;
            DocFlow.Document.PagePadding = new Thickness(5);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _updater.GetLastestUpdate();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
