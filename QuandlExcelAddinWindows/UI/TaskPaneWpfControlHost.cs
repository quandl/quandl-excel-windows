using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Quandl.Excel.Addin.UI
{
    public partial class TaskPaneWpfControlHost : UserControl
    {
        public TaskPaneWpfControlHost()
        {
            InitializeComponent();
        }

        public ElementHost WpfElementHost
        {
            get
            {
                return this.wpfElementHost;
            }
        }
    }
}
