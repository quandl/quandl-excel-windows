using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Quandl.Excel.Addin.UI.Helpers;
using Quandl.Shared;
using ComboBox = System.Windows.Controls.ComboBox;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using UserControl = System.Windows.Controls.UserControl;

namespace Quandl.Excel.Addin.UI.UDF_Builder.Filters
{
    /// <summary>
    /// Interaction logic for DateFilter.xaml
    /// </summary>
    public partial class DateFilter : UserControl
    {
        private DateConditionSelection _dateFrom = null;
        private DateConditionSelection _dateTo = null;

        public DateFilter(string identifier, FilterHelper filterHelper)
        {
            InitializeComponent();
            Identifier = identifier;
            FilterHelper = filterHelper;
            PopulateDateCondition();
            Filterlabel.Content = string.Format(Properties.Resources.DatatableFilterDateRange, Identifier);
        }

        public FilterHelper FilterHelper { get; set; }

        public string Identifier { get; set; }

        private void PopulateDateCondition()
        {
            _dateFrom = new DateConditionSelection(Identifier, new FilterHelper());
            _dateFrom.HorizontalAlignment = HorizontalAlignment.Left;
            _dateFrom.Margin = new Thickness(10,0,10,30);
            _dateFrom.SelectedDateChanged += Date_OnSelectedDateChanged;

            _dateTo = new DateConditionSelection(Identifier, new FilterHelper());
            _dateTo.dateRangeSelector();
            _dateTo.HorizontalAlignment = HorizontalAlignment.Left;
            _dateTo.Margin = new Thickness(180, 0, 10, 30);
            _dateTo.IsEnabled = false;
            _dateTo.SelectedDateChanged += Date_OnSelectedDateChanged;

            DateRangeGroup.Children.Add(_dateFrom);
            DateRangeGroup.Children.Add(_dateTo);
        }

        private void DateTypeSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedText = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
            if (selectedText == "Single Date")
            {
                CleanValidationMessage();
                _dateTo.IsEnabled = false;
                _dateFrom.dateSingleSelector();
            }
            else if (selectedText == "Period Range")
            {
                _dateTo.IsEnabled = true;
                _dateFrom.dateRangeSelector();
            } 
        }

        private void Date_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!ValidateDate(_dateFrom, _dateTo))
            {
                ClearDates();
            }
            else if(_dateFrom.DateBox.Text != "" && _dateTo.DateBox.Text != "")
            {
                CleanValidationMessage();
            }
        }

        private bool ValidateDate(DateConditionSelection dateFrom, DateConditionSelection dateTo)
        {
            try
            {
                string dateFromComparator = dateFrom.ConditionBox.Text;
                string dateToComparator = dateTo.ConditionBox.Text;
                string dateFromString = dateFrom.Filter.Value.Replace("\"", "");
                string dateToString = dateTo.Filter.Value.Replace("\"", "");
                if (dateFromString.Equals("") || dateToString.Equals(("")))
                {
                    return true;
                }
                
                DateTime df = Convert.ToDateTime(dateFromString);
                DateTime dt = Convert.ToDateTime(dateToString);
                
                if (df.CompareTo(dt) >= 0)
                {
                    string tempStr = dateFromComparator;
                    dateFromComparator = dateToComparator;
                    dateToComparator = tempStr;
                }
                if (dateFromComparator == "<" && dateToComparator == ">")
                {
                    throw new QuandlFromDateIsGreaterThanEndDateException();
                }
            }
            catch (Exception exception)
            {
                ValidationMessage.Content = exception.Message;
                return false;
            }
            return true;
        }

        private void ClearDates()
        {
            _dateFrom.DateBox.Text = "";
            _dateTo.DateBox.Text = "";
        }

        private void CheckDateFiltersEmpty()
        {
            var datatabelFilters = StateControl.Instance.DatatableFilters;
            if (_dateFrom.DateBox.Text.Equals(""))
            {
                datatabelFilters.Remove(_dateFrom.FilterHelper.Id);
            }
            if (_dateTo.DateBox.Text.Equals(""))
            {
                datatabelFilters.Remove(_dateTo.FilterHelper.Id);
            }
        }

        private void CleanValidationMessage()
        {
            ValidationMessage.Content = String.Empty;
        }
    }
}
