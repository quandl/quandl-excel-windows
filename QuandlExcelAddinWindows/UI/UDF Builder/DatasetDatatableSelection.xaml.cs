using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Quandl.Shared;
using Quandl.Shared.Models;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    ///     Interaction logic for DatasetDatatableSelection.xaml
    /// </summary>
    public partial class DatasetDatatableSelection : UserControl, WizardUIBase
    {
        Dataset _selectedDataset = null;
        string _lastFilterText = "";
        readonly int perPageCount = 50;
        readonly int pageSteps = 10;
        int _currentPage = 1;
        int _totalNumberOfDisplayedItems= 0;
        int _totalPageCount = 1;

        private ObservableCollection<DataHolderDefinition> AvailableDataHolders
            => StateControl.Instance.AvailableDataHolders;

        public DatasetDatatableSelection()
        {
            InitializeComponent();
            this.DataContext = StateControl.Instance;
        }

        public string GetTitle()
        {
            return "Choose Your Dataset or Data Table";
        }

        public string GetShortTitle()
        {
            return "Data";
        }

        private void DebounceSearchFilter()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(400);
            timer.Tick += timer_Tick;
            timer.Start();
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

        public async void GetDatasetsDatatablesFromAPI(string query = "")
        {
            string code = StateControl.Instance.Provider.Code;

            this.Dispatcher.Invoke(() =>
            {
                UpdateResultsLabel(false);
                DisablePaginationControls();
            });

            if (StateControl.Instance.ChainType == StateControl.ChainTypes.TimeSeries)
            {
                txtFilterResults.IsEnabled = true;
                var datasets = await Web.SearchDatasetsAsync(code, query, _currentPage, perPageCount);

                this.Dispatcher.Invoke(() =>
                {
                    if (!this.IsLoaded)
                    {
                        return;
                    }
                    lvDatasets.ItemsSource = datasets.Datasets;
                    _totalPageCount = (int)datasets.Meta.TotalPages;
                    _totalNumberOfDisplayedItems = lvDatasets.Items.Count;
                    UpdateResultsLabel();
                    UpdatePaginationControls();
                });
            }
            else
            {
                txtFilterResults.IsEnabled = false;
                // TODO: use statecontrol's datatableCollection to populate the list view with datatables
                UpdateResultsLabel();
            }
        }

        private void UpdateResultsLabel(bool loaded = true)
        {
            lblDatasetsDatatablesResults.Content = loaded ? $"Showing {_totalNumberOfDisplayedItems} results." : "Loading...";
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            string currentText = txtFilterResults.Text;
            if (currentText == _lastFilterText) return;
            GetDatasetsDatatablesFromAPI(currentText);
            _lastFilterText = currentText;
        }

        public async void GetDatasetFromAPI(string code)
        {
            DatasetResponse dataset = await Web.SearchDatasetAsync(code);
            _selectedDataset = dataset.Dataset;
        }

        private void lvDatasets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AvailableDataHolders.Clear();
            if (lvDatasets.SelectedItem == null)
            {
                return;
            }
            Dataset selectedDataset = new Dataset();
            Datatable selectedDatatable = new Datatable();
            if (StateControl.Instance.ChainType == StateControl.ChainTypes.TimeSeries)
            {
                this.Dispatcher.Invoke(() =>
                {
                    selectedDataset = (Dataset)lvDatasets.SelectedItem;
                });

                GetDatasetFromAPI(selectedDataset.Code);

                this.Dispatcher.Invoke(() =>
                {
                    AvailableDataHolders.Add(selectedDataset);
                });
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    AvailableDataHolders.Clear();
                    // TODO: implement Datatable selection
                    //       - make API query to get specific dataset
                    //       - save it to state control
                });
            }
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPageCount) {
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
            _currentPage = (_currentPage <= pageSteps) ? 1 : _currentPage - pageSteps;
            GetDatasetsDatatablesFromAPI();
        }

        private void btnLastPage_Click(object sender, RoutedEventArgs e)
        {
            // this button no longer goes to the first page.  instead, it will jump forward 'x' number of pages.
            _currentPage = (_currentPage >= _totalPageCount - pageSteps) ? _totalPageCount : _currentPage + pageSteps;
            GetDatasetsDatatablesFromAPI();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GetDatasetsDatatablesFromAPI();
            DebounceSearchFilter();
        }
    }
}