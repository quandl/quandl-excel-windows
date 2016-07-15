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
        private DateConditionSelection DateFrom = null;
        private DateConditionSelection DateTo = null;

        public DateFilter(string identifier, FilterHelper filterHelper)
        {
            InitializeComponent();
            Identifier = identifier;
            FilterHelper = filterHelper;
            PopulateDateCondition();
            Filterlabel.Content = string.Format(Properties.Settings.Default.DatatableFilterDateRange, Identifier);
        }

        public FilterHelper FilterHelper { get; set; }

        public string Identifier { get; set; }

        private void PopulateDateCondition()
        {
            DateFrom = new DateConditionSelection(Identifier, new FilterHelper());
            DateFrom.HorizontalAlignment = HorizontalAlignment.Left;
            DateFrom.Margin = new Thickness(10,0,10,30);
     
            DateTo = new DateConditionSelection(Identifier, new FilterHelper());
            DateTo.HorizontalAlignment = HorizontalAlignment.Left;
            DateTo.Margin = new Thickness(180, 0, 10, 30);
            DateTo.IsEnabled = false;
            DateTo.SelectedDateChanged += new EventHander(DateTo_OnSelectedDateChanged);

            DateRangeGroup.Children.Add(DateFrom);
            DateRangeGroup.Children.Add(DateTo);
        }

        private void DateTypeSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedText = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
            if (selectedText == "Single Date")
            {
                CleanValidationMessage();
                DateTo.IsEnabled = false;
            }
            else if (selectedText == "Period Range")
            {
                DateTo.IsEnabled = true;
            } 
        }

        private void DateTo_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateDate(DateFrom, DateTo);
        }

        private void ValidateDate(DateConditionSelection dateFrom, DateConditionSelection dateTo)
        {
            try
            {
                string dateFromString = DateFrom.Value[0].Value.Replace("\"", "");
                string dateToString = DateTo.Value[0].Value.Replace("\"", "");
                if (dateFromString.Equals("") || dateToString.Equals(("")))
                {
                    throw new QuandlDateCanNotBlankException();
                }
                
                DateTime df = Convert.ToDateTime(dateFromString);
                DateTime dt = Convert.ToDateTime(dateToString);

                if (df.CompareTo(dt) <= 0)
                {
                    CleanValidationMessage();
                }
                else
                {
                    throw new QuandlFromDateIsGreaterThanEndDateException();
                }
                
            }
            catch (Exception exception)
            {
                ValidationMessage.Content = exception.Message;
                throw;
            }
   
        }

        private void CleanValidationMessage()
        {
            ValidationMessage.Content = String.Empty;
        }
    }
}
