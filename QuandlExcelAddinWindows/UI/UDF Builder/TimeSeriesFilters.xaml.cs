using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Quandl.Excel.Addin.UI.Helpers;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    ///     Interaction logic for TimeSeriesFilters.xaml
    /// </summary>
    public partial class TimeSeriesFilters : UserControl, WizardUIBase
    {

        public TimeSeriesFilters()
        {
            InitializeComponent();
            Loaded += delegate
            {
                SetDateSelection();
                UpdateDateVisibility();
            };
        }

        public string GetTitle()
        {
            return "Customize Time Series Data";
        }

        public string GetShortTitle()
        {
        return "Filters";
        }
        private void ComboBoxDateSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDateVisibility();
        }

        private void SetDateSelection()
        {
            BindingHelper.SetItemSourceViaEnum(ComboBoxDateSelection, typeof(StateControl.TimeSeriesFilterTypes));
            BindingHelper.SetItemSourceViaEnum(ComboBoxFrequency, typeof(StateControl.TimeSeriesFilterCollapse));
            BindingHelper.SetItemSourceViaEnum(ComboBoxTransformation, typeof(StateControl.TimeSeriesFilterTransformations));
            BindingHelper.SetItemSourceViaEnum(ComboBoxSort, typeof(StateControl.TimeSeriesFilterSorts));
        }

        private void UpdateDateVisibility()
        {
            if (DatePickerStart == null || DatePickerEnd == null || LabelDateTo == null ||
                ComboBoxDateSelection.SelectedValue == null)
            {
                return;
            }

            var selectedItem = (StateControl.TimeSeriesFilterTypes) ComboBoxDateSelection.SelectedValue;
            switch (selectedItem)
            {
                case StateControl.TimeSeriesFilterTypes.All:
                    DateRow.Height = new GridLength(0);
                    DatePickerStart.Visibility = Visibility.Hidden;
                    DatePickerEnd.Visibility = Visibility.Hidden;
                    LabelDateTo.Visibility = Visibility.Hidden;
                    break;
                case StateControl.TimeSeriesFilterTypes.Range:
                    DateRow.Height = GridLength.Auto;
                    DatePickerStart.Visibility = Visibility.Visible;
                    DatePickerEnd.Visibility = Visibility.Visible;
                    LabelDateTo.Visibility = Visibility.Visible;
                    break;
                default:
                    DateRow.Height = GridLength.Auto;
                    DatePickerStart.Visibility = Visibility.Visible;
                    DatePickerEnd.Visibility = Visibility.Hidden;
                    LabelDateTo.Visibility = Visibility.Hidden;
                    break;
            }
        }
    }
}