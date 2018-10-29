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

namespace Quandl.Excel.Addin.UI
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ChangeLog : UserControl
    {
        public ChangeLog(string message)
        {
            InitializeComponent();
            DocumentFlow.IsToolBarVisible = false;
            var md = new Markdown.Xaml.Markdown();

            Content = md.Transform(message);
        }
    }

}
