using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Quandl.Excel.Addin.ViewData;
using Quandl.Shared;
using Quandl.Shared.models;

using Category = Quandl.Excel.Addin.ViewData.Category;
using SubCategory = Quandl.Excel.Addin.ViewData.SubCategory;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{

    /// <summary>
    /// Interaction logic for DatabaseSelection.xaml
    /// </summary>
    public partial class DatabaseSelection : UserControl, WizardUIBase
    {
        private List<Data> allItems;

        public string getTitle()
        {
            return "Browse Databases or Enter a Database Code";
        }

        public DatabaseSelection()
        {
            InitializeComponent();
            DataContext = StateControl.Instance;
            PopulateTreeView();
        }

        private async void PopulateTreeView()
        {
            BrowseCollection items = await Web.BrowseAsync();
            Categories categories = new Categories();

            foreach (var item in items.Items)
            {
                {
                    Category category = new Category() { Header = item.Name };
                    foreach (var subItem in item.Items)
                    {
                        SubCategory subCategory = new SubCategory() { Header = subItem.Name };
                        category.SubCategories.Add(subCategory);
                        foreach (var detailItem in subItem.Items)
                        {
                            Detail detail = new Detail(detailItem.Name, detailItem.OrderedResourceIds);
                            subCategory.Details.Add(detail);
                        }
                    }
                    categories.Add(category);
                }
            }

            BrowseData.ItemsSource = categories;
        }


        private async void PopulateList(Object current)
        {
            allItems = new List<Data>();
            Detail cur = (Detail)current;
            DatabaseCollection databaseCollection = await GetAllDatabase(cur);
            DatatableCollectionsResponse datatableCollectionsResponse = await GetAllDatatable((Detail)cur);

            int dtcCount = 0;
            int dtCount = 0;
            foreach (var listItem in cur.OrderList)
            {
                Database db = null;
                DatatableCollection dbc = null;
                Data data = null;
                var type = listItem.Type;
                if (type.Equals("database") && databaseCollection.Databases != null)
                {
                    db = databaseCollection.Databases[dtcCount];
                    dtcCount++;
                    data = new Data(db.Id, db.DatabaseCode, db.Premium, type);
                    data.Name = db.Name;
                    data.Description = db.Description;
                }
                else
                {
                    if (datatableCollectionsResponse.DatatableCollections != null)
                    {
                        dbc = datatableCollectionsResponse.DatatableCollections[dtCount];
                        data = new Data(dbc.Id, dbc.Code, dbc.Premium, type);
                        data.Name = dbc.Name;
                        data.Description = dbc.Description;
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

            object current = BrowseData.SelectedItem;
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
                Data selectedItem = (Data)AllDatabaseList.SelectedValue;
                DatabaseCodeBox.Text = selectedItem.Code;
                SetChainType(selectedItem);
            }
            else if (PremiumDatabaseList.SelectedValue != null)
            {
                Data selectedItem = (Data)PremiumDatabaseList.SelectedValue;
                DatabaseCodeBox.Text = selectedItem.Code;
                SetChainType(selectedItem);
            }
            else if (FreeDatabaseList.SelectedValue != null)
            {
                Data selectedItem = (Data)FreeDatabaseList.SelectedValue;
                DatabaseCodeBox.Text = selectedItem.Code;
                SetChainType(selectedItem);
            }
            StateControl.Instance.DataCode = DatabaseCodeBox.Text.Trim();
            StateControl.Instance.SelectionType = StateControl.SelectionTypes.Automatic;
        }

        private void SetChainType(ViewData.Data selectedItem)
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
            string code = ((TextBox)sender).Text;
            DatatableCollection dc = null;
            bool hasError = false;

            try
            {
                await Web.GetDatabase(code);
                StateControl.Instance.ChangeCode(code, StateControl.ChainTypes.TimeSeries);
            }
            catch (Exception)
            {
                try
                {
                    dc = await Web.GetDatatableCollection(code);
                    StateControl.Instance.ChangeCode(code, StateControl.ChainTypes.Datatables);
                    StateControl.Instance.datatableCollection = dc;
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
            ErrorMessage.Content = String.Format(Properties.Settings.Default.DataCodeValidationMessage, code);
        }

        private void CleanValidationError()
        {
            ErrorMessage.Content = String.Empty;
        }

        private void DatabaseCodeBox_OnMouseEnter(object sender, MouseEventArgs e)
        {
            StateControl.Instance.SelectionType = StateControl.SelectionTypes.Manual;
        }

        private async Task<DatabaseCollection> GetAllDatabase(Detail detail)
        {
            string type = "database";
            return await Web.GetModelByIds<DatabaseCollection>(type + "s", GetListIds(detail, type));
        }

        private async Task<DatatableCollectionsResponse> GetAllDatatable(Detail detail)
        {
            string type = "datatable_collection";
            return await Web.GetModelByIds<DatatableCollectionsResponse>(type, GetListIds(detail, type));
        }

        private List<string> GetListIds(Detail detail, string type)
        {
            List<string> ids = new List<string>();
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
