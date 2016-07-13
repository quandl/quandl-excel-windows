using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Quandl.Shared;
using Quandl.Shared.Models;
using Quandl.Shared.Models.Browse;
using Category = Quandl.Shared.Models.Browse.Category;
using SubCategory = Quandl.Shared.Models.Browse.SubCategory;
using Quandl.Shared.Errors;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    ///     Interaction logic for DatabaseSelection.xaml
    /// </summary>
    public partial class DatabaseSelection : UserControl, WizardUIBase
    {
        private List<ViewData> _allItems;
        static int VALIDATION_DELAY = 1200;
        private System.Threading.Timer _timer = null;

        public DatabaseSelection()
        {
            InitializeComponent();
            DataContext = StateControl.Instance;
            PopulateTreeView();
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
            var items = await Web.BrowseAsync();
            Categories categories = new Categories();

            foreach (var item in items.Items)
            {
                Category category = new Category { Name = item.Name };
                categories.Add(category);
                foreach (var subItem in item.Items)
                {
                    var subCategory = new SubCategory { Name = subItem.Name };
                    category.SubCategories.Add(subCategory);
                    foreach (var detailItem in subItem.Items)
                    {
                        var detail = new LeafCategory(detailItem.Name, detailItem.OrderedResourceIds);
                        subCategory.LeafCategories.Add(detail);
                    }
                }
            }
            BrowseData.ItemsSource = categories;
        }

        private async void PopulateList(object current)
        {
            _allItems = new List<ViewData>();
            var cur = (LeafCategory)current;
            DatabaseCollectionResponse dbCollection = await GetAllDatabase(cur);
            DatatableCollectionsResponse dtcCollection = await GetAllDatatable(cur);
            SetDataList(cur, dbCollection.Providers, dtcCollection.Providers);
            AllDatabaseList.ItemsSource = _allItems;
            PremiumDatabaseList.ItemsSource = PremiumItems();
            FreeDatabaseList.ItemsSource = FreeItems();
        }

        private void SetDataList(LeafCategory current, List<Provider> dbProviders, List<Provider> dtcProviders)
        {
            var i = 0;
            var j = 0;
            foreach (var item in current.OrderList)
            {
                var type = item.Type;
                Provider provider = null;
                ViewData viewData = null;
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
                    viewData = provider.ToViewData(type);
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
                        AllDatabaseList.SelectedValue = null;
                        break;
                    case 1:
                        PremiumDatabaseList.ItemsSource = PremiumItems();
                        PremiumDatabaseList.SelectedValue = null;
                        break;
                    case 2:
                        FreeDatabaseList.ItemsSource = FreeItems();
                        FreeDatabaseList.SelectedValue = null;
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
            CleanValidationError();

            if (AllDatabaseList.SelectedValue != null)
            {
                var selectedItem = (ViewData)AllDatabaseList.SelectedValue;
                SetSelection(selectedItem);
            }
            else if (PremiumDatabaseList.SelectedValue != null)
            {
                var selectedItem = (ViewData)PremiumDatabaseList.SelectedValue;
                SetSelection(selectedItem);
            }
            else if (FreeDatabaseList.SelectedValue != null)
            {
                var selectedItem = (ViewData)FreeDatabaseList.SelectedValue;
                SetSelection(selectedItem);
            }
            StateControl.Instance.DataCode = DatabaseCodeBox.Text.Trim();
        }

        private void SetSelection(ViewData selectedItem)
        {
            DatabaseCodeBox.Text = selectedItem.Code;
            StateControl.Instance.ValidateCode = true;
            SetChainType(selectedItem);
        }

        private void SetChainType(ViewData selectedItem)
        {
            if (selectedItem.Type.Equals("database"))
            {
                StateControl.Instance.ChainType = StateControl.ChainTypes.TimeSeries;
                StateControl.Instance.Provider = (Provider)selectedItem.DataSource;
            }
            else if (selectedItem.Type.Equals("datatable-collection"))
            {
                StateControl.Instance.ChainType = StateControl.ChainTypes.Datatables;
            }
        }

        // stackoverflow.com/questions/8001450/c-sharp-wait-for-user-to-finish-typing-in-a-text-box
        private void DatabaseCodeBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox origin = (TextBox)sender;
            if (!origin.IsFocused)
                return;

            DisposeTimer();
            _timer = new System.Threading.Timer(TimerElapsed, origin.Text, VALIDATION_DELAY, VALIDATION_DELAY);

        }

        private void TimerElapsed(Object text)
        {
            ValidateDataCode(text as string);
            DisposeTimer();
        }

        private void ValidateDataCode(string code)
        {
            Dispatcher.Invoke(async () =>
            {
                var isDatatableExist = await ValidateDatatable(code);
                var isDatabaseExist = await ValidateDatbase(code);

                if (!isDatatableExist && !isDatabaseExist)
                {
                    StateControl.Instance.ValidateCode = false;
                    ShowValidationError(code);
                }
                else
                {
                    StateControl.Instance.ValidateCode = true;
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
            ErrorMessage.Content = string.Empty;
        }

        private async Task<bool> ValidateDatatable(string code)
        {
            try
            {
                DatatableCollectionResponse dtc = await Web.GetDatatableCollection<DatatableCollectionResponse>(code);
                StateControl.Instance.ChangeCode(code, StateControl.ChainTypes.Datatables);
                StateControl.Instance.Provider = dtc.Provider;
                return true;
            }
            catch (QuandlErrorBase)
            {
                return false;
            }
        }

        private async Task<bool> ValidateDatbase(string code)
        {
            try
            {
                DatabaseResponse db = await Web.GetDatabase<DatabaseResponse>(code);
                StateControl.Instance.ChangeCode(code, StateControl.ChainTypes.TimeSeries);
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
            return await Web.GetModelByIds<DatabaseCollectionResponse>(type + "s", GetListIds(leafCategory, type));
        }

        private async Task<DatatableCollectionsResponse> GetAllDatatable(LeafCategory leafCategory)
        {
            var type = "datatable-collection";
            return await Web.GetModelByIds<DatatableCollectionsResponse>(type.Replace("-", "_") + "s", GetListIds(leafCategory, type));
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
