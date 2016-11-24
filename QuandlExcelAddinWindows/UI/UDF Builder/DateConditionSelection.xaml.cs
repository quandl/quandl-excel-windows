using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Quandl.Excel.Addin.UI.Helpers;
using Quandl.Excel.Addin.UI.UDF_Builder.Filters;
using Quandl.Shared;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    /// Interaction logic for ConditionFilter.xaml
    /// </summary>
    public partial class DateConditionSelection : UserControl
    {

        private List<Operator> listData = new List<Operator> { new Operator { Value = "lt", Label = "<" },
                                                               new Operator { Value = "lte", Label = "<=" },
                                                               new Operator { Value = "gt", Label = ">" },
                                                               new Operator { Value = "gte", Label = ">=" } };

        public DateConditionSelection(string identifier, FilterHelper filterHelper)
        {
            InitializeComponent();
      
            Identifier = identifier;
            FilterHelper = filterHelper;
            dateSingleSelector();
            
        }

        public FilterHelper FilterHelper { get; set; }

        public string Identifier { get; set; }

        public void dateRangeSelector()
        {
            setComboboxList(listData, "lt");
        }

        public void dateSingleSelector()
        {
            List<Operator> listPlusEquals = new List<Operator> { new Operator { Value = "eq", Label = "=" } };
            listPlusEquals.AddRange(listData);
            setComboboxList(listPlusEquals, "eq");
        }

        private void setComboboxList(List<Operator> listData, string selected)
        {
            ConditionBox.ItemsSource = listData;
            ConditionBox.SelectedValue = selected;
            ConditionBox.DisplayMemberPath = "Label";
            ConditionBox.SelectedValuePath = "Value";
        }

        public Filter Filter
        {
            get
            {
                var input = DateBox.SelectedDate == null ? "" : String.Format("{0:yyyy-MM-dd}", DateBox.SelectedDate);
                string filterName = "";
                if((string)ConditionBox.SelectedValue == "eq")
                {
                    filterName = $"{Identifier}";
                }
                else
                {
                    filterName = $"{Identifier}.{ConditionBox.SelectedValue}";
                }
                return  new Filter {
                                        Name = filterName,
                                        Value = $"\"{input}\""
                                    };
            }
        }

        public class Operator
        {
            public string Value { get; set; }
            public string Label { get; set; }
        }

        public event EventHander SelectedDateChanged = delegate {};

        private void DateBox_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
               SelectedDateChanged(sender, e);
            }
            catch (Exception exception)
            {
                if (!(exception is QuandlFromDateIsGreaterThanEndDateException ||
                    exception is QuandlDateCanNotBlankException))
                {
                    Utilities.LogToSentry(exception);
                    throw;
                }
            }
            FilterHelper.PropertyChanged(Filter);
        }

        private void ConditionBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DateBox_OnSelectedDateChanged(sender, e);
        }
    }

    public delegate void EventHander(object sender, SelectionChangedEventArgs selectionChangedEventArgs);
}
