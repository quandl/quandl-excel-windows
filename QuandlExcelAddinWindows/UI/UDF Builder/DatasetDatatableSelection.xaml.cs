using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using Quandl.Shared;
using Quandl.Shared.Models;
using Quandl.Shared.Models.Browse;

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
            return "Choose Your Dataset or Data Table";
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
                lvDatasets.ItemsSource = StateControl.Instance.Provider.ToDatatablesViewData(); ;
                _totalNumberOfDisplayedItems = lvDatasets.Items.Count;
                UpdateResultsLabel();
            }
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
            GetDatasetsDatatablesFromAPI(currentText);
            _lastFilterText = currentText;
        }

        private async void SetDatatableFromAPI(string code)
        {
            DatatableMetadata dtm = await new Web().GetDatatableMetadata(code);
            AvailableDataHolders.Clear();
            _selectedDatatable = dtm.datatable;
            AvailableDataHolders.Add(_selectedDatatable);
        }

        private void lvDatasetsDatatables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AvailableDataHolders.Clear();
            Columns.Clear();
            if (lvDatasets.SelectedItem == null)
            {
                return;
            }
            var selectedDataset = new Dataset();
            if (StateControl.Instance.ChainType == StateControl.ChainTypes.TimeSeries)
            {
                Dispatcher.Invoke(() =>
                {
                    selectedDataset = (Dataset)lvDatasets.SelectedItem;
                    AvailableDataHolders.Add(selectedDataset);
                });
            }
            else
            {
                PopulateDatatableList();
            }

            Dispatcher.Invoke(DisplaySelectedCodes);
        }

        private void PopulateDatatableList()
        {
            Dispatcher.Invoke(() =>
            {
                ViewData selectedDatatable = (ViewData)lvDatasets.SelectedItem;
                var code = selectedDatatable.Code;
                var codes = GetCodes(code);
                _selectedDatatable = new Datatable { VendorCode = codes.Item1, DatatableCode = codes.Item2 };
                SetDatatableFromAPI(selectedDatatable.Code);
                AvailableDataHolders.Add(_selectedDatatable);
            });
        }

        private Tuple<string, string> GetCodes(string fullCode)
        {
            if (fullCode != null)
            {
                string[] r = fullCode.Split(Convert.ToChar("/"));
                return Tuple.Create(r[0], r[1]);
            }
            return null;
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
    }
}