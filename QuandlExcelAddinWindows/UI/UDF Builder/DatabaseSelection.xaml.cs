using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Quandl.Shared;
using Quandl.Shared.Models;
using Quandl.Shared.Models.ViewData;
using Category = Quandl.Shared.Models.ViewData.Category;
using SubCategory = Quandl.Shared.Models.ViewData.SubCategory;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    ///     Interaction logic for DatabaseSelection.xaml
    /// </summary>
    public partial class DatabaseSelection : UserControl, WizardUIBase
    {
        private List<Data> allItems;

        public DatabaseSelection()
        {
            InitializeComponent();
            DataContext = StateControl.Instance;
            PopulateTreeView();
            GetDatabase("WIKI");
        }

        public async void GetDatabase(string code)
        {
            var provider = await Web.GetDatabase(code);
            return;
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
            var categories = new Categories();

            foreach (var item in items.Items)
            {
                {
                    var category = new Category {Header = item.Name};
                    foreach (var subItem in item.Items)
                    {
                        var subCategory = new SubCategory {Header = subItem.Name};
                        category.SubCategories.Add(subCategory);
                        foreach (var detailItem in subItem.Items)
                        {
                            var detail = new Detail(detailItem.Name, detailItem.OrderedResourceIds);
                            subCategory.Details.Add(detail);
                        }
                    }
                    categories.Add(category);
                }
            }

            BrowseData.ItemsSource = categories;
        }


        private async void PopulateList(object current)
        {
            allItems = new List<Data>();
            var cur = (Detail) current;
            var databaseCollection = await GetAllDatabase(cur);
            var datatableCollectionsResponse = await GetAllDatatable(cur);

            var dtcCount = 0;
            var dtCount = 0;
            foreach (var listItem in cur.OrderList)
            {
                OldDatabase db = null;
                OldDatatableCollection dbc = null;
                Data data = null;
                var type = listItem.Type;
                if (type.Equals("database") && databaseCollection.Databases != null)
                {
                    db = databaseCollection.Databases[dtcCount];
                    data = db.ToData(type);
                    dtcCount++;
                }
                else
                {
                    if (datatableCollectionsResponse.DatatableCollections != null)
                    {
                        dbc = datatableCollectionsResponse.DatatableCollections[dtCount];
                        data = dbc.ToData(type);
                        dtCount++;
                    }
                }

                if (data != null)
                {
                    allItems.Add(data);
                }
            }

            AllDatabaseList.ItemsSource = allItems;
            PremiumDatabaseList.ItemsSource = PremiumItems();
            FreeDatabaseList.ItemsSource = FreeItems();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var current = BrowseData.SelectedItem;
            if (current.GetType().Name.ToLower() == "detail")
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
            allItems = null;
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (allItems != null)
            {
                switch (TabControl.SelectedIndex)
                {
                    case 0:
                        AllDatabaseList.ItemsSource = allItems;
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

        private List<Data> PremiumItems()
        {
            return allItems.Where(x => x.Premium).ToList();
        }

        private List<Data> FreeItems()
        {
            return allItems.Where(x => !x.Premium).ToList();
        }

        private void DatabaseList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CleanValidationError();

            if (AllDatabaseList.SelectedValue != null)
            {
                var selectedItem = (Data) AllDatabaseList.SelectedValue;
                DatabaseCodeBox.Text = selectedItem.Code;
                SetChainType(selectedItem);
            }
            else if (PremiumDatabaseList.SelectedValue != null)
            {
                var selectedItem = (Data) PremiumDatabaseList.SelectedValue;
                DatabaseCodeBox.Text = selectedItem.Code;
                SetChainType(selectedItem);
            }
            else if (FreeDatabaseList.SelectedValue != null)
            {
                var selectedItem = (Data) FreeDatabaseList.SelectedValue;
                DatabaseCodeBox.Text = selectedItem.Code;
                SetChainType(selectedItem);
            }
            StateControl.Instance.DataCode = DatabaseCodeBox.Text.Trim();
        }

        private void SetChainType(Data selectedItem)
        {
            if (selectedItem.Type.Equals("database"))
            {
                StateControl.Instance.chainType = StateControl.ChainTypes.TimeSeries;
            }
            else if (selectedItem.Type.Equals("datatable-collection"))
            {
                StateControl.Instance.chainType = StateControl.ChainTypes.Datatables;
            }
        }

        private async void DatabaseCodeBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var code = ((TextBox) sender).Text;
            var hasError = false;

            try
            {
                var dc = await Web.GetDatatableCollection(code);
                StateControl.Instance.ChangeCode(code, StateControl.ChainTypes.Datatables);
                StateControl.Instance.datatableCollection = dc;
            }
            catch (Exception)
            {
                try
                {
                    await Web.GetDatabase(code);
                    StateControl.Instance.ChangeCode(code, StateControl.ChainTypes.TimeSeries);
                }
                catch (Exception)
                {
                    hasError = true;
                }
            }


            if (hasError.Equals(false))
            {
                CleanValidationError();
            }
            else
            {
                ShowValidationError(code);
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

        private async Task<OldDatabaseCollection> GetAllDatabase(Detail detail)
        {
            var type = "database";
            return await Web.GetModelByIds<OldDatabaseCollection>(type + "s", GetListIds(detail, type));
        }

        private async Task<OldDatatableCollectionsResponse> GetAllDatatable(Detail detail)
        {
            var type = "datatable_collection";
            return await Web.GetModelByIds<OldDatatableCollectionsResponse>(type, GetListIds(detail, type));
        }

        private List<string> GetListIds(Detail detail, string type)
        {
            var ids = new List<string>();
            foreach (var ol in detail.OrderList)
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