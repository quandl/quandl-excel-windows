using System;
using System.Collections.Generic;
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
        Dataset selectedDataset = null;
        string lastFilterText = "";
        int perPageCount = 50;
        int currentPage = 1;
        int totalNumberOfDisplayedItems= 0;
        int totalPageCount = 1;
        int pageSteps = 10;

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
            btnFirstPage.IsEnabled = !(currentPage == 1);
            btnPrevPage.IsEnabled = !(currentPage == 1);
            btnNextPage.IsEnabled = !(currentPage == totalPageCount);
            btnLastPage.IsEnabled = !(currentPage == totalPageCount);
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
            string code = StateControl.Instance.DataCode;

            this.Dispatcher.Invoke(() =>
            {
                UpdateResultsLabel(false);
                DisablePaginationControls();
            });

            if (StateControl.Instance.ChainType == StateControl.ChainTypes.TimeSeries)
            {
                txtFilterResults.IsEnabled = true;
                var datasets = await Web.SearchDatasetsAsync(code, query, currentPage, perPageCount);

                this.Dispatcher.Invoke(() =>
                {
                    if (!this.IsLoaded)
                    {
                        return;
                    }
                    lvDatasets.ItemsSource = datasets.Datasets;
                    totalPageCount = (int)datasets.Meta.TotalPages;
                    totalNumberOfDisplayedItems = lvDatasets.Items.Count;
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
            lblDatasetsDatatablesResults.Content = loaded ? $"Showing {totalNumberOfDisplayedItems} results." : "Loading...";
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            string currentText = txtFilterResults.Text;
            if (currentText == lastFilterText) return;
            GetDatasetsDatatablesFromAPI(currentText);
            lastFilterText = currentText;
        }

        public async void GetDatasetFromAPI(string code)
        {
            DatasetResponse dataset = await Web.SearchDatasetAsync(code);
            selectedDataset = dataset.Dataset;
        }

        private void lvDatasets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StateControl.Instance.DatasetOrDatatable.Clear();
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
                    StateControl.Instance.DatasetOrDatatable.Add(selectedDataset);
                });
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    StateControl.Instance.DatasetOrDatatable.Clear();
                    // TODO: implement Datatable selection
                    //       - make API query to get specific dataset
                    //       - save it to state control
                });
            }
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPageCount) {
                currentPage++;
                GetDatasetsDatatablesFromAPI();
            }
        }

        private void btnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                GetDatasetsDatatablesFromAPI();
            }
        }

        private void btnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            // this button no longer goes to the first page.  instead, it will jump back 'x' number of pages.
            currentPage = (currentPage <= pageSteps) ? 1 : currentPage - pageSteps;
            GetDatasetsDatatablesFromAPI();
        }

        private void btnLastPage_Click(object sender, RoutedEventArgs e)
        {
            // this button no longer goes to the first page.  instead, it will jump forward 'x' number of pages.
            currentPage = (currentPage >= totalPageCount - pageSteps) ? totalPageCount : currentPage + pageSteps;
            GetDatasetsDatatablesFromAPI();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GetDatasetsDatatablesFromAPI();
            DebounceSearchFilter();
        }
    }
}