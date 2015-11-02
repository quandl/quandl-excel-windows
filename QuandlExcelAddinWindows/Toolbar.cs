using Microsoft.Office.Tools.Ribbon;

namespace Quandl.Excel.Addin
{
    using System.Windows;
    using System.Windows.Forms;

    public partial class Toolbar
    {

        private void Ribbon2_Load(object sender, RibbonUIEventArgs e)
        {
        }

        private void GetDataButton_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.TaskPane.Visible = true;
        }

        private void AboutButton_Click(object sender, RibbonControlEventArgs e)
        {
            new Quandl.Excel.Addin.Controls.AboutForm().Show();
        }

        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
            Form dataSelection = new Form();
            dataSelection.Controls.Add(new DataTaskPane());
            dataSelection.AutoSize = true;
            dataSelection.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            dataSelection.Show();
        }
    }
}
