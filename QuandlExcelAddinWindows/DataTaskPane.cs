using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Quandl.Shared;
using Newtonsoft.Json.Linq;

namespace Quandl.Excel.Addin
{
    using System.Threading;
    using System.Windows.Documents;

    public partial class DataTaskPane : UserControl
    {
        private String databaseCode;

        public DataTaskPane()
        {
            InitializeComponent();

            this.listBox1.DisplayMember = "Name";
            this.listBox1.ValueMember = "Value";
            this.listBox1.DoubleClick += ListBox1_DoubleClick;

            this.listBox2.DisplayMember = "Name";
            this.listBox2.ValueMember = "Value";
            this.listBox2.DoubleClick += ListBox2_DoubleClick;

            this.listBox3.DisplayMember = "Name";
            this.listBox3.ValueMember = "Value";
        }

        private void ListBox2_DoubleClick(object sender, EventArgs e)
        {
            ListBox box = (ListBox)sender;
            dynamic item = box.SelectedItem;
            if (this.listBox2.Items.Contains(item))
            {
                this.listBox2.Items.Remove(item);
                AvailableColumns_Recalculate();
            }
        }

        private void ListBox1_DoubleClick(object sender, EventArgs e)
        {
            ListBox box = (ListBox)sender;
            dynamic item = box.SelectedItem;
            if (!this.listBox2.Items.Contains(item))
            {
                this.listBox2.Items.Add(item);
                AvailableColumns_Recalculate();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            this.databaseCode = ((TextBox)sender).Text.ToUpper();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
            this.button2.Enabled = false;

            String query = this.textBox1.Text;
            JObject data = TestFunctions.SearchDatasets(this.databaseCode, (string)query);
            foreach (JObject dataset in data["datasets"])
            {
                this.listBox1.Items.Add(new { Value = dataset["dataset_code"].ToObject<String>(), Name = dataset["name"].ToObject<String>(), Extras = dataset["column_names"] });
            }

            this.button2.Enabled = true;
        }

        private void AvailableColumns_Recalculate()
        {
            ArrayList newColumns = new ArrayList();
            foreach (dynamic item in this.listBox2.Items)
            {
                newColumns.AddRange(item.Extras.ToObject<ArrayList>());
            }

            foreach (String columnName in newColumns)
            {
                object columnSelection = new { Value = columnName, Name = columnName };

                if (!this.listBox3.Items.Contains(columnSelection))
                {
                    this.listBox3.Items.Add(columnSelection);
                }
            }

            ArrayList currentItems = new ArrayList(this.listBox3.Items);
            foreach (dynamic columnSelection in currentItems)
            {
                if (!newColumns.Contains(columnSelection.Value))
                {
                    this.listBox3.Items.Remove(columnSelection);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.listBox2.Items.Count == 0 || this.listBox3.Items.Count == 0 || this.listBox3.SelectedItems.Count == 0)
            {
                this.textBox3.Text = "You must select at least one Dataset and one Column To display.";
            }
        }
    }
}
