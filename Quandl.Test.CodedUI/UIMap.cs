namespace Quandl.Test.CodedUI
{
    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Win32;
    using Shared.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Mouse = Microsoft.VisualStudio.TestTools.UITesting.Mouse;

    public partial class UIMap
    {
        #region Coded UI Test Components

        private UIItemPane1 ExcelClient()
        {
            return UIQuandlFormulaBuilderWindow.UIWpfElementHostWindow.UIWpfElementHostClient.UIItemPane;
        }

        private UIItemCustom1 ExcelClient1()
        {
            return UIQuandlFormulaBuilderWindow.UIWpfElementHostWindow.UIWpfElementHostClient.UIItemPane.UIItemCustom;
        }

        private UIItemCustom11 ExcelClient2()
        {
            return UIQuandlFormulaBuilderWindow.UIWpfElementHostWindow.UIWpfElementHostClient.UIItemPane.UIItemCustom1;
        }

        private UIItemCustom7 ExcelClient3()
        {
            return UIQuandlFormulaBuilderWindow1.UIWpfElementHostWindow.UIWpfElementHostClient.UIItemPane.UIItemCustom;
        }

        public UIItemCustom21 ExcelClient4()
        {
            return UIQuandlFormulaBuilderWindow1.UIWpfElementHostWindow.UIWpfElementHostClient.UIItemPane.UIItemCustom2;
        }

        public UIItemPane1 ExcelClient5()
        {
            return UIQuandlFormulaBuilderWindow.UIWpfElementHostWindow.UIWpfElementHostClient.UIItemPane;
        }

        public UIItemCustom12 ExcelClient6()
        {
            return UIQuandlFormulaBuilderWindow1.UIWpfElementHostWindow.UIWpfElementHostClient.UIItemPane.UIItemCustom1;
        }

        public WpfButton NextButton()
        {
            return ExcelClient1().UINextButton;
        }

        private WpfTabPage GetSelectedTab()
        {
            var tabList = ExcelClient2().UITabControlTabList;
            var tabPage = tabList.UIAllTabPage;

            string name = "";
            switch (tabList.SelectedIndex)
            {
                case 0:
                    name = "All";
                    break;
                case 1:
                    name = "Premium";
                    break;
                case 2:
                    name = "Free";
                    break;
            }

            tabPage.SearchProperties[WpfTabPage.PropertyNames.Name] = name;
            return tabPage;
        }

        private WpfTabPage SetSelectedTab(string name)
        {
            var tab = ExcelClient2().UITabControlTabList.UIPremiumTabPage;
            tab.SearchProperties[WpfTabPage.PropertyNames.Name] = name;
            return tab;
        }

        #endregion

        public void OpenExcelAndLoginUsingApiKey()
        {
            OpenExcelAndWorksheet();
            OpenLoginPage();
            LoginWithApiKey();
            AssertLoggedIn();
        }

        public void ClearRegistryApiKey()
        {
            string RegistrySubKey = @"SOFTWARE\Quandl\Excel Add-in";
            var appKeyPath = Registry.CurrentUser.CreateSubKey(RegistrySubKey);
            appKeyPath.SetValue("ApiKey", "", RegistryValueKind.String);
            appKeyPath.Close();
        }

        public void OpenExcelAndWorksheet()
        {
            var blankWorkbookListItem = UIExcelWindow.UIFeaturedList.UIBlankworkbookListItem;
            string ExePath = "C:\\Program Files (x86)\\Microsoft Office\\root\\Office16\\EXCEL.EXE";
            string AlternateExePath = "%ProgramFiles%\\Microsoft Office\\root\\Office16\\EXCEL.EXE";

            ApplicationUnderTest excelApplication = ApplicationUnderTest.Launch(ExePath, AlternateExePath);
            Mouse.Click(blankWorkbookListItem);
        }

        public void OpenLoginPage()
        {
            var tabQuandl = this.UIExcelWindow.UIItemWindow.UIRibbonClient.UIQuandlTabPage;
            var uiDownloadButton = this.UIExcelWindow.UIItemWindow.UIDataToolBar.UIDownloadButton;

            Mouse.Click(tabQuandl);
            Mouse.Click(uiDownloadButton);
        }

        public void LoginWithUsername()
        {
            var txtEmailAddress = ExcelClient1().UIQuandlExcelAddincompPane.UIEmailEdit;
            var txtPassword = ExcelClient1().UIQuandlExcelAddincompPane.UIPasswordEdit;
            var btnLogin = ExcelClient1().UIQuandlExcelAddincompPane.UILoginButton;

            string username = "qa_admin@quandl.com";
            string password = "***REMOVED***";

            txtEmailAddress.Text = username;
            txtPassword.Text = password;
            Mouse.Click(btnLogin);
        }

        public void AssertCorrectUDFSignature(string signature)
        {
            var txtUDFSignature = ExcelClient1().UIUdfOutputEdit;

            Assert.AreEqual(signature, txtUDFSignature.Text, "Generated QSERIES function not correct");
        }

        public void AssertSelectedDatabaseCode(string databaseCode)
        {
            var txtDatabaseCode = ExcelClient1().UIQuandlExcelAddincompPane.UIDatabaseCodeBoxEdit;

            Assert.AreEqual(databaseCode, txtDatabaseCode.Text);
        }

        public void SelectBrowseCategory(string leaf1, string leaf2, string leaf3)
        {
            var treeBranch1 = ExcelClient2().UIBrowseDataTree.UIStockDataTreeItem;
            var treeBranch2 = treeBranch1.UIUnitedStatesTreeItem;
            var treeBranch3 = treeBranch2.UIStockPricesEndofDayCTreeItem;
            var lstDatabase = ExcelClient2().UITabControlTabList.UIAllTabPage.UIAllDatabaseListList;

            treeBranch1.SearchProperties[WpfTreeItem.PropertyNames.Name] = leaf1;
            treeBranch2.SearchProperties[WpfTreeItem.PropertyNames.Name] = leaf2;
            treeBranch3.SearchProperties[WpfTreeItem.PropertyNames.Name] = leaf3;

            lstDatabase.Container = GetSelectedTab();

            SelectBrowseCategory();
        }

        public void SelectBrowseFilter(string tabName)
        {
            var tab = SetSelectedTab(tabName);
            var databaseList = ExcelClient2().UITabControlTabList.UIAllTabPage.UIAllDatabaseListList;

            databaseList.SearchProperties[WpfList.PropertyNames.AutomationId] = $"{tabName}DatabaseList";
            databaseList.Container = tab;

            SelectBrowseFilter();
        }

        public void SelectDatabase(string databaseName, string filter = "All")
        {
            var tab = ExcelClient3().UITabControlTabList.UIAllTabPage;
            var databaseList = ExcelClient3().UITabControlTabList.UIAllTabPage.UIAllDatabaseListList;

            tab.SearchProperties[WpfTabPage.PropertyNames.Name] = filter;

            databaseList.SearchProperties[WpfList.PropertyNames.AutomationId] = $"{filter}DatabaseList";
            databaseList.SelectedItemsAsString = databaseName;
        }

        public List<string> GetDatabaseList(string filter = "All")
        {
            var tab = ExcelClient3().UITabControlTabList.UIAllTabPage;
            var databaseList = ExcelClient3().UITabControlTabList.UIAllTabPage.UIAllDatabaseListList;

            tab.SearchProperties[WpfTabPage.PropertyNames.Name] = filter;
            databaseList.SearchProperties[WpfList.PropertyNames.AutomationId] = $"{filter}DatabaseList";

            List<string> values = new List<string>();

            var items = databaseList.GetProperty("Items") as UITestControlCollection;
            foreach (WpfListItem item in items)
            {
                values.Add(item.DisplayText);
            }

            return values;
        }

        public void InputDatabaseCode(string databaseCode)
        {
            var txtDatabaseCode = ExcelClient1().UIQuandlExcelAddincompPane.UIDatabaseCodeBoxEdit;

            txtDatabaseCode.Text = databaseCode;
        }

        public void AssertValidDatabaseCode()
        {
            // Verify that the 'Enabled' property of 'next' button equals 'True'
            NextButton().WaitForControlEnabled();
            Assert.AreEqual(AssertNextButtonEnabledExpectedValues.UINextButtonEnabled, NextButton().Enabled, "Next button not enabled");
        }

        public void AssertInvalidDatabaseCode(string code)
        {
            var uINextButton = NextButton();
            var txtDatabaseCode = ExcelClient2().UIEODFBisnotavalidcodeText.UIEODFBisnotavalidcodeText1;

            txtDatabaseCode.WaitForControlReady();
            Assert.AreEqual(txtDatabaseCode.DisplayText, $"\"{code}\" is not a valid code.");
            Assert.AreNotEqual(AssertNextButtonEnabledExpectedValues.UINextButtonEnabled, uINextButton.Enabled, "Next button not enabled");
        }

        public void FilterDatasetsDatatables(string filter)
        {
            var txtDatasetsFilter = ExcelClient1().UIQuandlExcelAddincompPane.UITxtFilterResultsEdit;

            txtDatasetsFilter.Text = filter;
        }

        public void AssertCorrectDatasetDatatableCode(string datasetDatatableCode)
        {
            var txtDatasetDatatableCode = ExcelClient5().UIStepTwoPaneCustom.UIDatabaseCodeBoxEdit.Text;

            Assert.AreEqual(datasetDatatableCode, txtDatasetDatatableCode);
        }

        public void AssertFirstPageButtonEnabled(bool enabled = true)
        {
            var btnFirstPage = ExcelClient1().UIQuandlExcelAddincompPane.UIItemButton;

            if (enabled)
            {
                btnFirstPage.WaitForControlEnabled();
            }
            Assert.AreEqual(enabled, btnFirstPage.Enabled);
        }

        public void AssertPreviousPageButtonEnabled(bool enabled = true)
        {
            var btnPrevPage = ExcelClient1().UIQuandlExcelAddincompPane.UIItemButton1;

            if (enabled)
            {
                btnPrevPage.WaitForControlEnabled();
            }
            Assert.AreEqual(enabled, btnPrevPage.Enabled);
        }

        public void AssertNextPageButtonEnabled(bool enabled = true)
        {
            var btnNextPage = ExcelClient1().UIQuandlExcelAddincompPane.UIItemButton2;

            if (enabled)
            {
                btnNextPage.WaitForControlEnabled();
            }
            Assert.AreEqual(enabled, btnNextPage.Enabled);
        }

        public void AssertLastPageButtonEnabled(bool enabled = true)
        {
            var btnLastPage = ExcelClient1().UIQuandlExcelAddincompPane.UIItemButton3;

            if (enabled)
            {
                btnLastPage.WaitForControlEnabled();
            }
            Assert.AreEqual(enabled, btnLastPage.Enabled);
        }

        public void ClickDatasetPageButton(string page)
        {
            var button = new WpfButton();

            switch (page)
            {
                case "<<":
                    button = ExcelClient1().UIQuandlExcelAddincompPane.UIItemButton;
                    break;
                case "<":
                    button = ExcelClient1().UIQuandlExcelAddincompPane.UIItemButton1;
                    break;
                case ">>":
                    button = ExcelClient1().UIQuandlExcelAddincompPane.UIItemButton3;
                    break;
                default:
                    button = ExcelClient1().UIQuandlExcelAddincompPane.UIItemButton2;
                    break;
            }

            Mouse.Click(button);
        }

        public void SelectDatasetOrDatatableByName(string name)
        {
            Playback.PlaybackSettings.MatchExactHierarchy = false;

            var stepTwoPane = ExcelClient4().UIDatasetsDatatablesPane;
            var datasetsScrollViewer = new WpfScrollViewer(stepTwoPane);
            var datasetsListView = new UIDatasetsDatatablesLiList(datasetsScrollViewer);

            datasetsListView.SearchProperties[WpfList.PropertyNames.AutomationId] = "DatasetsDatatablesListView";
            datasetsListView.SelectedItemsAsString = name;

            Playback.PlaybackSettings.MatchExactHierarchy = true;
        }

        public void SelectDatasetOrDatatableByIndex(int index)
        {
            var stepTwoPane = ExcelClient4().UIDatasetsDatatablesPane;
            var datasetsScrollViewer = new WpfScrollViewer(stepTwoPane);
            var datasetsListView = new UIDatasetsDatatablesLiList(datasetsScrollViewer);

            datasetsListView.SearchProperties[WpfList.PropertyNames.AutomationId] = "DatasetsDatatablesListView";
            datasetsListView.SelectedIndices = new int[] { index };
        }

        public string GetSelectedDatasetDatatableCode()
        {
            return UIQuandlFormulaBuilderWindow.UIWpfElementHostWindow.UIWpfElementHostClient.UIItemPane.UIStepTwoPaneCustom.UIDatabaseCodeBoxEdit.Text;
        }

        public void SelectColumn(DataColumn column)
        {
            var rootItem = ExcelClient().UIStepThreePaneCustom.UIColumnsTreeViewTree.UIColumnsTreeRootItem;
            rootItem.SearchProperties[UITestControl.PropertyNames.Name] = column.Parent.Name;

            var listItem = rootItem.UIColumnsTreeListItem1;
            listItem.SearchProperties[UITestControl.PropertyNames.Name] = $"{column.Parent.Name} - {column.Name}";
            listItem.SearchConfigurations.Add(SearchConfiguration.ExpandWhileSearching);

            var checkBox = listItem.UIColumnListItemCheckBox;
            checkBox.SearchProperties[WpfControl.PropertyNames.AutomationId] = column.Name;
            checkBox.SearchConfigurations.Add(SearchConfiguration.ExpandWhileSearching);

            checkBox.Checked = !checkBox.Checked;
        }

        public void ClickAddAllColumnsButton()
        {
            var btnAddAllColumns = ExcelClient().UIQuandlExcelAddincompPane.UIStepThreePaneCustom.UIAddAllButton;

            Mouse.Click(btnAddAllColumns);
        }

        public void ClickRemoveAllColumnsButton()
        {
            var btnRemoveAllColumns = ExcelClient().UIQuandlExcelAddincompPane.UIStepThreePaneCustom.UIRemoveAllButton;

            Mouse.Click(btnRemoveAllColumns);
        }

        public void AssertColumnAddedToSelection(DataColumn column)
        {
            string columnLongName = $"{column.Parent.Name} - {column.Name}";
            var columnList = ExcelClient().UIStepThreePaneCustom.UISelectedColumnsList;
            var columnListItem = columnList.UISelectedColumnListItem;
            columnListItem.SearchProperties[UITestControl.PropertyNames.Name] = columnLongName;

            Assert.AreEqual(true, columnListItem.Exists);
            Assert.AreEqual(columnLongName, columnListItem.DisplayText);
        }

        public void AssertNumberOfColumnsSelected(int count)
        {
            var columnList = ExcelClient().UIStepThreePaneCustom.UISelectedColumnsList;

            Assert.AreEqual(count, CountSelectedColumns());
        }

        public List<DataColumn> GetAllAvailableColumns()
        {
            var columnCollection = ExcelClient().UIStepThreePaneCustom.UIColumnsTreeViewTree.UIColumnsTreeRootItem
                                                .UIColumnTreeListItem2.FindMatchingControls();

            List<DataColumn> columns = new List<DataColumn>();
            foreach (var column in columnCollection)
            {
                string columnName = column.Name.Split(new string[] { " - " }, StringSplitOptions.None).Last();
                columns.Add(new DataColumn { Name = columnName });
            }
            return columns;
        }

        public int CountAvailableColumns()
        {
            var columnCollection = ExcelClient().UIStepThreePaneCustom.UIColumnsTreeViewTree.UIColumnsTreeRootItem
                                                .UIColumnTreeListtemCollection.FindMatchingControls();

            return columnCollection.Count;
        }

        public int CountSelectedColumns()
        {
            var selectedColumns = ExcelClient().UIStepThreePaneCustom.UISelectedColumnsList
                                               .UISelectedColumnListItems.FindMatchingControls();

            return selectedColumns.Count;
        }

        public void ClickNextButton()
        {
            WpfButton btnNext = ExcelClient().UIItemCustom.UINextButton;

            Mouse.Click(btnNext);
        }

        public void SelectDatasetDateRangeFilter(string description, string value)
        {
            var lstDateRangeFilter = ExcelClient().UIItemCustom.UIQuandlExcelAddincompPane.UIDateRangeTypeFilterComboBox;

            lstDateRangeFilter.SelectedItem = $"{{ Description = {description}, value = {value} }}";
        }

        public void SelectDatasetDateFromFilter(string date)
        {
            var dateFromPicker = ExcelClient().UIItemCustom11.UIDateFromFilterDatePicker;

            dateFromPicker.DateAsString = date;
        }

        public void SelectDatasetDateToFilter(string date)
        {
            var dateToFilter = ExcelClient6().UIQuandlExcelAddincompPane.UIDateToFilterDatePicker;

            dateToFilter.DateAsString = date;
        }

        public void SelectFrequencyFilter(string description, string value)
        {
            var frequencyComboBox = ExcelClient().UIItemCustom.UIQuandlExcelAddincompPane.UIFrequencyFilterComboBox;

            frequencyComboBox.SelectedItem = $"{{ Description = {description}, value = {value} }}";
        }

        public void SelectLimitFilter(string limit)
        {
            var limitTextBox = ExcelClient().UIItemCustom.UIQuandlExcelAddincompPane.UIAutoSelectTextBoxEdit;

            limitTextBox.Text = limit;
        }

        public void SelectSortFilter(string description, string value)
        {
            var sortComboBox = ExcelClient().UIItemCustom.UIQuandlExcelAddincompPane.UISortFilterComboBox;

            sortComboBox.SelectedItem = $"{{ Description = {description}, value = {value} }}";
        }

        public void SelectTransformationFilter(string description, string value)
        {
            var transformationComboBox = ExcelClient().UIItemCustom.UIQuandlExcelAddincompPane.UITransformationFilterComboBox;

            transformationComboBox.SelectedItem = $"{{ Description = {description}, value = {value} }}";
        }
    }

    public class WpfScrollViewer : WpfPane
    {
        public WpfScrollViewer(UITestControl container) : base(container)
        {
            SearchProperties[WpfPane.PropertyNames.ClassName] = "Uia.ScrollViewer";
            SearchProperties[WpfPane.PropertyNames.AutomationId] = "ListViewScrollViewerWrapper";
            WindowTitles.Add("Quandl Formula Builder");
        }

        public WpfList UIDatasetList
        {
            get
            {
                if (datasetsList == null)
                {
                    datasetsList = new WpfList();
                    datasetsList.SearchProperties[WpfList.PropertyNames.AutomationId] = "DatasetsDatatablesListView";
                }
                return datasetsList;
            }
        }
        private WpfList datasetsList;
    }
}
