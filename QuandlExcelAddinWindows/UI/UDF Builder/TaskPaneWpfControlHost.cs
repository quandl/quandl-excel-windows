using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Quandl.Excel.Addin.UI.UDF_Builder
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
