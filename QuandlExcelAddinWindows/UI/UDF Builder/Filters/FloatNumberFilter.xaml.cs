using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using Quandl.Excel.Addin.UI.Helpers;

namespace Quandl.Excel.Addin.UI.UDF_Builder.Filters
{
    /// <summary>
    /// Interaction logic for FloatNumberFilter.xaml
    /// </summary>
    public partial class FloatNumberFilter : UserControl
    {
        public FloatNumberFilter(string identifier, FilterHelper filterHelper)
        {
            InitializeComponent();
            Identifier = identifier;
            FilterHelper = filterHelper;
            FilterLabel.Content = string.Format(Properties.Settings.Default.DatatableFilterFloatNumber, Identifier);
        }

        public FilterHelper FilterHelper { get; set; }

        public string Identifier { get; set; }

        private void TextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Validate float number
            //Regex regex = new Regex(@"^[+-]?\d*\.\d+$|^[+-]?\d+(\.\d*)?$");
            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        public Filter Filter
        {
            get
            {
                string input = InputTextBox.Text.ToString();
                return  new Filter
                {
                    Name = Identifier,
                    Value = $"\"{input}\""
                };
            }
        }

        private void InputTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            FilterHelper.PropertyChanged(Filter);
        }
    }
}
