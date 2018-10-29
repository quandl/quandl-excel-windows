using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Quandl.Shared;
using Quandl.Shared.Models;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    ///     Interaction logic for DatasetDatatableSelection.xaml
    /// </summary>
    public partial class DatasetDatatableSelection : WizardUIBase
    {
        private const int _timerDebounce = 400;

        private readonly int pageSteps = 10;
        private readonly int perPageCount = 50;
        private int _currentPage = 1;
        private string _lastFilterText = "";
        private Datatable _selectedDatatable;
        private int _totalNumberOfDisplayedItems;
        private int _totalPageCount = 1;
        private bool _selectionCodeTriggered = false;

        private DispatcherTimer _timer = new DispatcherTimer();

        public DatasetDatatableSelection()
        {
            InitializeComponent();
            SelectedDataHolderTextBox.DataContext = this;

            Loaded += delegate
            {
                if (StateControl.Instance.ChainType != StateControl.ChainTypes.TimeSeries)
                {
                    txtFilterResults.Visibility = Visibility.Collapsed;
                    PaginationButtons.Visibility = Visibility.Collapsed;
                    lvDatasetsDatatables.SelectionMode = SelectionMode.Single;
                }
            };

            this.Unloaded += delegate
            {
                _timer.Stop();
            };
        }

        private ObservableCollection<DataHolderDefinition> AvailableDataHolders
            => StateControl.Instance.AvailableDataHolders;

        public ObservableCollection<DataColumn> Columns
            => StateControl.Instance.Columns;

        public string GetTitle()
        {
            return "Choose dataset or data table";
        }

        public string GetShortTitle()
        {
            return "Data";
        }

        private void DebounceSearchFilter()
        {
            _timer.Interval = TimeSpan.FromMilliseconds(_timerDebounce);
            _timer.Tick += timer_Tick;
            _timer.Start();
        }

        private void UpdatePaginationControls()
        {
            btnFirstPage.IsEnabled = !(_currentPage == 1);
            btnPrevPage.IsEnabled = !(_currentPage == 1);
            btnNextPage.IsEnabled = !(_currentPage == _totalPageCount);
            btnLastPage.IsEnabled = !(_currentPage == _totalPageCount);
        }

        private void DisablePaginationControls()
        {
            btnFirstPage.IsEnabled = false;
            btnPrevPage.IsEnabled = false;
            btnNextPage.IsEnabled = false;
            btnLastPage.IsEnabled = false;
            UpdateResultsLabel(false);
        }

        public void DisplaySelectedCodes()
        {
            SelectedDataHolderTextBox.Text = string.Join(",", StateControl.Instance.QuandlCodes);
        }

        public async void GetDatasetsDatatablesFromAPI(string query = "")
        {
            var code = StateControl.Instance.Provider.Code;

            _selectionCodeTriggered = true;

            Dispatcher.Invoke(() =>
            {
                UpdateResultsLabel(false);
                DisablePaginationControls();
            });

            if (StateControl.Instance.ChainType == StateControl.ChainTypes.TimeSeries)
            {
                txtFilterResults.IsEnabled = true;
                var datasets = await new Web().SearchDatasetsAsync(code, query, _currentPage, perPageCount);

                Dispatcher.Invoke(() =>
                {
                    if (!IsLoaded)
                    {
                        return;
                    }
                    lvDatasetsDatatables.ItemsSource = datasets.Datasets;
                    _totalPageCount = (int)datasets.Meta.TotalPages;
                    _totalNumberOfDisplayedItems = lvDatasetsDatatables.Items.Count;
                    UpdateResultsLabel();
                    UpdatePaginationControls();
                });
            }
            else
            {
                txtFilterResults.IsEnabled = false;
                lvDatasetsDatatables.ItemsSource = StateControl.Instance.Provider.GetDatatables();
                _totalNumberOfDisplayedItems = lvDatasetsDatatables.Items.Count;
                UpdateResultsLabel();
            }

            RestoreDatasetSelection();
            _selectionCodeTriggered = false;
        }

        private void UpdateResultsLabel(bool loaded = true)
        {
            lblDatasetsDatatablesResults.Content = loaded
                ? $"Showing {_totalNumberOfDisplayedItems} results."
                : "Loading...";
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            var currentText = txtFilterResults.Text;
            if (currentText == _lastFilterText) return;
            _currentPage = 1;
            GetDatasetsDatatablesFromAPI(currentText);
            _lastFilterText = currentText;
        }

        private async void SetDatatableFromAPI(string code)
        {
            DatatableMetadata datatableMetadata = await new Web().GetDatatableMetadata(code);

            _selectedDatatable = (Datatable)lvDatasetsDatatables.SelectedItem;
            _selectedDatatable.Columns = datatableMetadata.datatable.Columns;

            AvailableDataHolders.Add(_selectedDatatable);
        }

        private void lvDatasetsDatatables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // if this event is triggered by code, then don't need to update data in the following.
            if (_selectionCodeTriggered)
            {
                return;
            }

            if (StateControl.Instance.ChainType == StateControl.ChainTypes.TimeSeries)
            {
                Dispatcher.Invoke(() =>
                {
                    UpdateDataHolders();
                });
            }
            else
            {
                PopulateDatatableList();
            }

            Dispatcher.Invoke(DisplaySelectedCodes);
        }

        private void UpdateDataHolders()
        {
            // Using Set operations to get all un-selected items and selected items
            string[] dataCodeHolders = GetStringListFromCollection(AvailableDataHolders);
            IEnumerable<string> items = GetStringListFromCollection(lvDatasetsDatatables.Items);
            IEnumerable<string> selectedItems = GetStringListFromCollection(lvDatasetsDatatables.SelectedItems);
            IEnumerable<string> unSelectedItems = items.Except(selectedItems);
            IEnumerable<string> removedItems = dataCodeHolders.Intersect(unSelectedItems);

            // Clean all items which is un-selected
            foreach (var code in removedItems)
            {
                RemoveDataHolder(code);
            }

            // Select all items which is selected
            foreach (Dataset d in lvDatasetsDatatables.SelectedItems)
            {
                if (!dataCodeHolders.Contains(d.Code))
                {
                    AvailableDataHolders.Add(d);
                }
            }
        }

        private string[] GetStringListFromCollection(IEnumerable e)
        {
            var list = new ArrayList();
            foreach (Dataset d in e)
            {
                list.Add(d.Code);
            }
            return list.ToArray(typeof(string)) as string[];
        }

        private void RemoveDataHolder(string code)
        {
            var d = AvailableDataHolders.SingleOrDefault(x => ((Dataset)x).Code.Equals(code));
            if ( d != null)
            {
                AvailableDataHolders.Remove(d);
            }       
        }

        private void PopulateDatatableList()
        {
            Dispatcher.Invoke(() =>
            {
                Datatable selectedItem = (Datatable)lvDatasetsDatatables.SelectedItem;
                SetDatatableFromAPI(selectedItem.Code);
            });
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPageCount)
            {
                _currentPage++;
                GetDatasetsDatatablesFromAPI();
            }
        }

        private void btnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                GetDatasetsDatatablesFromAPI();
            }
        }

        private void btnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            // this button no longer goes to the first page.  instead, it will jump back 'x' number of pages.
            _currentPage = _currentPage <= pageSteps ? 1 : _currentPage - pageSteps;
            GetDatasetsDatatablesFromAPI();
        }

        private void btnLastPage_Click(object sender, RoutedEventArgs e)
        {
            // this button no longer goes to the first page.  instead, it will jump forward 'x' number of pages.
            _currentPage = _currentPage >= _totalPageCount - pageSteps ? _totalPageCount : _currentPage + pageSteps;
            GetDatasetsDatatablesFromAPI();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GetDatasetsDatatablesFromAPI();
            DebounceSearchFilter();
            DisplaySelectedCodes();
        }

        /// <summary>
        /// Fixes the issue with the scrollable section of datasets/datatables not scrolling when using a trackpad
        /// scroll motion while the cursor is over the listview.
        /// 
        /// http://stackoverflow.com/a/16235785/5034313
        /// </summary>
        private void ListViewScrollViewerWrapper_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void RestoreDatasetSelection()
        {
            if (AvailableDataHolders.Count == 0)
            {
                return;
            }

            _selectionCodeTriggered = true;
            foreach (var item in lvDatasetsDatatables.Items)
            {
                foreach (var data in AvailableDataHolders)
                {
                    if (((Dataset)data).Code == ((Dataset)item).Code)
                    {
                        lvDatasetsDatatables.SelectedItems.Add(item);
                    }
                }
            }
            _selectionCodeTriggered = false;
        }
    }
}