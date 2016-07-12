using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Quandl.Excel.Addin.UI.Helpers;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    ///     Interaction logic for ColumnSelection.xaml
    /// </summary>
    public partial class ColumnSelection : UserControl, WizardUIBase
    {
        public ColumnSelection()
        {
            InitializeComponent();

            SelectedColumnOrderListBox.DataContext = StateControl.Instance.Columns;
            AvailableColumnsTreeView.DataContext = StateControl.Instance.AvailableCodeColumns;

            CheckedItemHelper.CheckedChanged += delegate { AddRemoveCheckedItems(); };
        }

        public string GetTitle()
        {
            return "Choose Your Columns";
        }

        public string GetShortTitle()
        {
            return "Columns";
        }

        private void ButtonAddAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var ddc in StateControl.Instance.AvailableCodeColumns)
            {
                CheckedItemHelper.SetIsChecked(ddc, true);
            }
        }

        private void ButtonRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var ddc in StateControl.Instance.AvailableCodeColumns)
            {
                CheckedItemHelper.SetIsChecked(ddc, false);
            }
        }

        private void AddRemoveCheckedItems()
        {
            foreach (var ddc in StateControl.Instance.AvailableCodeColumns)
            {
                foreach (var column in ddc.Columns)
                {
                    var chkd = (bool) CheckedItemHelper.GetIsChecked(column);
                    if (chkd && !StateControl.Instance.Columns.Contains(column))
                    {
                        StateControl.Instance.Columns.Add(column);
                    }
                    else if (!chkd && StateControl.Instance.Columns.Contains(column))
                    {
                        StateControl.Instance.Columns.Remove(column);
                    }
                }
            }
        }

        private void ButtonRemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            var copy = SelectedColumnOrderListBox.SelectedItems.Cast<DataCodeColumn>().ToList();
            foreach (var column in copy)
            {
                CheckedItemHelper.SetIsChecked(column, false);
            }
        }

        private void SelectedColumnOrderListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonRemoveSelected.IsEnabled = SelectedColumnOrderListBox.SelectedItems.Count > 0;
        }
    }
}