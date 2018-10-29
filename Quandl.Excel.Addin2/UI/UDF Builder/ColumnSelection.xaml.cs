using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Quandl.Shared.Models;
using Quandl.Shared.Helpers;

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
            AvailableColumnsTreeView.DataContext = StateControl.Instance.AvailableDataHolders;

            CheckedItemHelper.CheckedChanged += delegate { AddRemoveCheckedItems(); };

            // Set the parent of each column
            foreach (var dh in StateControl.Instance.AvailableDataHolders)
                foreach (var column in dh.Columns)
                {
                    column.Content = UseMnemonics(column.Name);
                    column.Parent = dh;
                    column.SetValue(CheckedItemHelper.ParentProperty, dh);
                }
        }

        public string GetTitle()
        {
            return "Choose columns";
        }

        public string GetShortTitle()
        {
            return "Columns";
        }

        // http://stackoverflow.com/questions/7861699/cannot-see-underscore-in-wpf-content
        private string UseMnemonics(string name)
        {
            return name.Replace("_", "__");
        }

        private void ButtonAddAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var ddc in StateControl.Instance.AvailableDataHolders)
            {
                CheckedItemHelper.SetIsChecked(ddc, true);
            }
        }

        private void ButtonRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var ddc in StateControl.Instance.AvailableDataHolders)
            {
                CheckedItemHelper.SetIsChecked(ddc, false);
            }
        }

        private void AddRemoveCheckedItems()
        {
            foreach (var ddc in StateControl.Instance.AvailableDataHolders)
            {
                foreach (var column in ddc.Columns)
                {
                    var chkd = (bool)CheckedItemHelper.GetIsChecked(column);
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
            var copy = SelectedColumnOrderListBox.SelectedItems.Cast<DataColumn>().ToList();
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