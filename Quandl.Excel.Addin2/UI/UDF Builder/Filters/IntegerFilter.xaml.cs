using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using Quandl.Excel.Addin.UI.Helpers;

namespace Quandl.Excel.Addin.UI.UDF_Builder.Filters
{
    /// <summary>
    /// Interaction logic for IntegerFilter.xaml
    /// </summary>
    public partial class IntegerFilter : UserControl
    {
        public IntegerFilter(string identifier, FilterHelper filterHelper)
        {
            InitializeComponent();
            Identifier = identifier;
            FilterHelper = filterHelper;
            Filterlabel.Content = string.Format(Properties.Resources.DatatableFilterIntegerNumber, Identifier);
        }

        public FilterHelper FilterHelper { get; set; }

        public string Identifier { get; set; }

        private void IntegerFilterInput_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[\d,]$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        public Filter Filter
        {
            get
            {
                string input = IntegerFilterInput.Text.ToString();
                return  new Filter
                {
                    Name = Identifier,
                    Value = $"\"{input}\""
                };
            }
        }

        private void IntegerFilterInput_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            FilterHelper.PropertyChanged(Filter);
        }
    }
}
