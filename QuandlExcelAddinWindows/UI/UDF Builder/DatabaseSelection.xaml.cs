using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Quandl.Shared;
using Quandl.Shared.Errors;
using Quandl.Shared.Models;
using Quandl.Shared.Models.Browse;


namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    ///     Interaction logic for DatabaseSelection.xaml
    /// </summary>
    public partial class DatabaseSelection : UserControl, WizardUIBase
    {
        private static readonly int VALIDATION_DELAY = 1200;
        private List<ViewData> _allItems;
        private Timer _timer;
        private static Provider Provider => StateControl.Instance.Provider;

        public DatabaseSelection()
        {
            InitializeComponent();
            DataContext = StateControl.Instance;
            SetDataCodeBox();
            PopulateTreeView();
        }

        private void SetDataCodeBox()
        {
            if ( Provider != null)
            {
                DatabaseCodeBox.Text = Provider.Code;
            }
        }

        public string GetTitle()
        {
            return "Browse Databases or Enter a Database Code";
        }

        public string GetShortTitle()
        {
            return "Database";
        }

        private async void PopulateTreeView()
        {
            LoadingState.IsBusy = true;
            var items = await new Web().BrowseAsync();
            var categories = new Categories();

            foreach (var item in items.Items)
            {
                var category = new Category {Name = item.Name};
                categories.Add(category);
                foreach (var subItem in item.Items)
                {
                    var subCategory = new SubCategory {Name = subItem.Name};
                    category.SubCategories.Add(subCategory);
                    foreach (var detailItem in subItem.Items)
                    {
                        // remove intraday data
                        if (detailItem.Name != null && !detailItem.Name.ToLower().Contains("intraday"))
                        {
                            var detail = new LeafCategory(detailItem.Name, detailItem.OrderedResourceIds);
                            subCategory.LeafCategories.Add(detail);
                        }
    
                    }
                }
            }
            Dispatcher.Invoke(() => { BrowseData.ItemsSource = categories;
                                        LoadingState.IsBusy = false;
            });
        }

        private async void PopulateList(object current)
        {
            LoadingState.IsBusy = true;
            _allItems = new List<ViewData>();
            var cur = (LeafCategory) current;
            var dbCollection = await GetAllDatabase(cur);
            var dtcCollection = await GetAllDatatable(cur);
            SetDataList(cur, dbCollection.Providers, dtcCollection.Providers);

            Dispatcher.Invoke(() =>
            {
                AllDatabaseList.ItemsSource = _allItems;
                PremiumDatabaseList.ItemsSource = PremiumItems();
                FreeDatabaseList.ItemsSource = FreeItems();
                LoadingState.IsBusy = false;
            });
        }

        private void SetDataList(LeafCategory current, List<Provider> dbProviders, List<Provider> dtcProviders)
        {
            var i = 0;
            var j = 0;
            foreach (var item in current.OrderList)
            {
                var type = item.Type;
                Provider provider;
                switch (type)
                {
                    case "database":
                        provider = dbProviders[i];
                        i++;
                        break;
                    case "datatable-collection":
                        provider = dtcProviders[j];
                        j++;
                        break;
                    default:
                        provider = null;
                        break;
                }
                if (provider != null)
                {
                    var viewData = provider.ToViewData(type);
                    if (viewData != null)
                    {
                        _allItems.Add(viewData);
                    }
                }
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var current = BrowseData.SelectedItem;
            if (current.GetType().Name.ToLower() == "leafcategory")
            {
                PopulateList(current);
            }
            else
            {
                ResetSelections();
            }
        }

        private void ResetSelections()
        {
            AllDatabaseList.ItemsSource = null;
            PremiumDatabaseList.ItemsSource = null;
            FreeDatabaseList.ItemsSource = null;
            TabControl.SelectedIndex = 0;
            DatabaseCodeBox.Text = string.Empty;
            _allItems = null;
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_allItems != null)
            {
                switch (TabControl.SelectedIndex)
                {
                    case 0:
                        AllDatabaseList.ItemsSource = _allItems;
                        //Reset others
                        PremiumDatabaseList.SelectedValue = null;
                        FreeDatabaseList.SelectedValue = null;
                        break;
                    case 1:
                        PremiumDatabaseList.ItemsSource = PremiumItems();
                        //Reset others;
                        AllDatabaseList.SelectedValue = null;
                        FreeDatabaseList.SelectedValue = null;
                        break;
                    case 2:
                        FreeDatabaseList.ItemsSource = FreeItems();
                        //Reset others
                        AllDatabaseList.SelectedValue = null;
                        PremiumDatabaseList.SelectedValue = null;
                        break;
                }
            }
        }

        private List<ViewData> PremiumItems()
        {
            return _allItems.Where(x => x.Premium).ToList();
        }

        private List<ViewData> FreeItems()
        {
            return _allItems.Where(x => !x.Premium).ToList();
        }

        private void DatabaseList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;

            CleanValidationError();

            if (AllDatabaseList.SelectedValue != null)
            {
                var selectedItem = (ViewData) AllDatabaseList.SelectedValue;
                SetSelection(selectedItem);
            }
            else if (PremiumDatabaseList.SelectedValue != null)
            {
                var selectedItem = (ViewData) PremiumDatabaseList.SelectedValue;
                SetSelection(selectedItem);
            }
            else if (FreeDatabaseList.SelectedValue != null)
            {
                var selectedItem = (ViewData) FreeDatabaseList.SelectedValue;
                SetSelection(selectedItem);
            }
        }

        private void SetSelection(ViewData selectedItem)
        {
            Dispatcher.Invoke(() => { DatabaseCodeBox.Text = selectedItem.Code; });
            SetChainType(selectedItem);
        }

        private void SetChainType(ViewData selectedItem)
        {
            if (selectedItem.Type.Equals("database"))
            {
                StateControl.Instance.ChangeCode((Provider) selectedItem.DataSource, StateControl.ChainTypes.TimeSeries);
            }
            else if (selectedItem.Type.Equals("datatable-collection"))
            {
                StateControl.Instance.ChangeCode((Provider) selectedItem.DataSource, StateControl.ChainTypes.Datatables);
            }
        }

        // stackoverflow.com/questions/8001450/c-sharp-wait-for-user-to-finish-typing-in-a-text-box
        private void DatabaseCodeBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var origin = (TextBox) sender;
            if (!origin.IsFocused)
                return;

            DisposeTimer();
            _timer = new Timer(TimerElapsed, origin.Text, VALIDATION_DELAY, VALIDATION_DELAY);
        }

        private void TimerElapsed(object text)
        {
            ValidateDataCode(text as string);
            DisposeTimer();
        }

        private void ValidateDataCode(string code)
        {
            Dispatcher.Invoke(async () =>
            {
                bool isDatatableExist = await ValidateDatatable(code);
                bool isDatabaseExist = false;
                if (!isDatatableExist)
                {
                    isDatabaseExist = await ValidateDatabase(code);
                }
                 

                if (!isDatatableExist && !isDatabaseExist)
                {
                    ShowValidationError(code);
                }
                else
                {
                    CleanValidationError();
                }
            });
        }

        private void DisposeTimer()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }


        private void ShowValidationError(string code)
        {
            ErrorMessage.Content = string.Format(Properties.Settings.Default.DataCodeValidationMessage, code);
        }

        private void CleanValidationError()
        {
            Dispatcher.Invoke(() => { ErrorMessage.Content = string.Empty; });
        }

        private async Task<bool> ValidateDatatable(string code)
        {
            try
            {
                var dtc = await new Web().GetDatatableCollection<DatatableCollectionResponse>(code);
                StateControl.Instance.ChangeCode(dtc.Provider, StateControl.ChainTypes.Datatables);
                return true;
            }
            catch (QuandlErrorBase)
            {
                return false;
            }
        }

        private async Task<bool> ValidateDatabase(string code)
        {
            try
            {
                var response = await new Web().GetDatabase<DatabaseResponse>(code);
                StateControl.Instance.ChangeCode(response.Provider, StateControl.ChainTypes.TimeSeries);
                return true;
            }
            catch (QuandlErrorBase)
            {
                return false;
            }
        }

        private async Task<DatabaseCollectionResponse> GetAllDatabase(LeafCategory leafCategory)
        {
            var type = "database";
            return await new Web().GetModelByIds<DatabaseCollectionResponse>(type + "s", GetListIds(leafCategory, type));
        }

        private async Task<DatatableCollectionsResponse> GetAllDatatable(LeafCategory leafCategory)
        {
            var type = "datatable-collection";
            return
                await
                    new Web().GetModelByIds<DatatableCollectionsResponse>(type.Replace("-", "_") + "s",
                        GetListIds(leafCategory, type));
        }

        private List<string> GetListIds(LeafCategory leafCategory, string type)
        {
            var ids = new List<string>();
            foreach (var ol in leafCategory.OrderList)
            {
                if (ol.Type.Equals(type))
                {
                    ids.Add(ol.Id.ToString());
                }
            }
            return ids;
        }
    }

    public class Categories : ObservableCollection<Category>
    {
    }
}