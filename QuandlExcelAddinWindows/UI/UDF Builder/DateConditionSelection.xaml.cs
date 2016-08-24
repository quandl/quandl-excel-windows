﻿using System;
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
        public DateConditionSelection(string identifier, FilterHelper filterHelper)
        {
            InitializeComponent();
      
            Identifier = identifier;
            FilterHelper = filterHelper;
            InitList();
        }

        public FilterHelper FilterHelper { get; set; }

        public string Identifier { get; set; }

        private void InitList()
        {
            List<Operator> listData = new List<Operator>();
            listData.Add(new Operator {Value = "lt", Label = "<"});
            listData.Add(new Operator { Value = "lte", Label = "<=" });
            listData.Add(new Operator { Value = "gt", Label = ">" });
            listData.Add(new Operator { Value = "gte", Label = ">=" });
            ConditionBox.SelectedValue = "lt";
            ConditionBox.ItemsSource = listData;
            ConditionBox.DisplayMemberPath = "Label";
            ConditionBox.SelectedValuePath = "Value";
        }

        public Filter[] Value
        {
            get
            {
                var input = DateBox.SelectedDate == null ? "" : String.Format("{0:yyyy-MM-dd}", DateBox.SelectedDate);
                return new Filter[1] { new Filter
                {
                    Name = $"{Identifier}.{ConditionBox.SelectedValue}",
                    Value = $"\"{input}\""
                }};
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
            if (!Value[0].Value.Replace("\"", "").Equals(""))
            {
                FilterHelper.PropertyChanged(Value);
            }
        }

        private void ConditionBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DateBox_OnSelectedDateChanged(sender, e);
        }
    }

    public delegate void EventHander(object sender, SelectionChangedEventArgs selectionChangedEventArgs);
}