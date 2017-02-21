using Quandl.Shared;
using System.Drawing;
using System.Windows.Forms;

namespace Quandl.Excel.UDF.Functions.UI
{
    public partial class confirmOverwrite : Form
    {
        public confirmOverwrite()
        {
            InitializeComponent();
            warningPicture.Image = SystemIcons.Warning.ToBitmap();
        }

        private void checkShow_CheckedChanged(object sender, System.EventArgs e)
        {
            if(checkShow.Checked && QuandlConfig.OverwriteDataWarning )
            {
                QuandlConfig.OverwriteDataWarning = false;
            }
        }
    }
}
