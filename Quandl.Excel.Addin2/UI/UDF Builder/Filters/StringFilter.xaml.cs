using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Quandl.Excel.Addin.UI.Helpers;

namespace Quandl.Excel.Addin.UI.UDF_Builder.Filters
{
    /// <summary>
    /// Interaction logic for StringFilter.xaml
    /// </summary>
    public partial class StringFilter : UserControl
    {
 
        public StringFilter(string identifier, FilterHelper filterHelper)
        {
            InitializeComponent();
            FilterHelper = filterHelper;
            Identifier = identifier;
            Filterlabel.Content = string.Format(Properties.Resources.DatatableFilterIntegerNumber, Identifier);
        }

        public FilterHelper FilterHelper { get; set; }

        public string Identifier { get; set; }

        public Filter Filter
        {
            get
            {
                string input = StringFilterInput.Text.ToString();
                string[] filterValues = input.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                return  new Filter
                {
                    Name = Identifier,
                    Value = filterValues.Length == 1 ? $"\"{filterValues.First()}\"" : $"{{{string.Join(",",filterValues.Select(n => $"\"{n}\"").ToArray())}}}"
                };
            }
        }

        private void StringFilterInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterHelper.PropertyChanged(Filter);
        }
    }
}
