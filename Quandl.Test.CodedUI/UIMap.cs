namespace Quandl.Test.CodedUI
{
    using System.Drawing;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.WinControls;
    using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mouse = Microsoft.VisualStudio.TestTools.UITesting.Mouse;
    using Microsoft.Win32;
    using System.Collections.Generic;
    public partial class UIMap
    {
        #region WPF Components

        private UIItemCustom1 ExcelClient()
        {
            return UIQuandlFormulaBuilderWindow.UIWpfElementHostWindow.UIWpfElementHostClient.UIItemPane.UIItemCustom;
        }

        private UIItemCustom11 ExcelClient1()
        {
            return UIQuandlFormulaBuilderWindow.UIWpfElementHostWindow.UIWpfElementHostClient.UIItemPane.UIItemCustom1;
        }

        private UIItemCustom6 ExcelClient2()
        {
            return UIQuandlFormulaBuilderWindow1.UIWpfElementHostWindow.UIWpfElementHostClient.UIItemPane.UIItemCustom;
        }

        private WpfButton NextButton()
        {
            return ExcelClient().UINextButton;
        }

        private WpfTabPage GetSelectedTab()
        {
            var tabList = ExcelClient1().UITabControlTabList;
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
            var tab = ExcelClient1().UITabControlTabList.UIPremiumTabPage;
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
            var blankWorkbookListItem = this.UIExcelWindow.UIFeaturedList.UIBlankworkbookListItem;
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
            var txtEmailAddress = ExcelClient().UIQuandlExcelAddincompPane.UIEmailEdit;
            var txtPassword = ExcelClient().UIQuandlExcelAddincompPane.UIPasswordEdit;
            var btnLogin = ExcelClient().UIQuandlExcelAddincompPane.UILoginButton;

            string username = "qa_admin@quandl.com";
            string password = "***REMOVED***";

            txtEmailAddress.Text = username;
            txtPassword.Text = password;
            Mouse.Click(btnLogin);
        }

        public void AssertCorrectUDFSignature(string UDF)
        {
            var txtUDFSignature = ExcelClient().UIUdfOutputEdit;

            Assert.AreEqual(UDF, txtUDFSignature.Text, "Generated QSERIES function not correct");
        }

        public void AssertSelectedDatabaseCode(string databaseCode)
        {
            var txtDatabaseCode = ExcelClient().UIQuandlExcelAddincompPane.UIDatabaseCodeBoxEdit;

            Assert.AreEqual(databaseCode, txtDatabaseCode.Text);
        }

        public void SelectBrowseCategory(string leaf1, string leaf2, string leaf3)
        {
            var treeBranch1 = ExcelClient1().UIBrowseDataTree.UIStockDataTreeItem;
            var treeBranch2 = treeBranch1.UIUnitedStatesTreeItem;
            var treeBranch3 = treeBranch2.UIStockPricesEndofDayCTreeItem;
            var lstDatabase = ExcelClient1().UITabControlTabList.UIAllTabPage.UIAllDatabaseListList;

            treeBranch1.SearchProperties[WpfTreeItem.PropertyNames.Name] = leaf1;
            treeBranch2.SearchProperties[WpfTreeItem.PropertyNames.Name] = leaf2;
            treeBranch3.SearchProperties[WpfTreeItem.PropertyNames.Name] = leaf3;

            lstDatabase.Container = GetSelectedTab();

            SelectBrowseCategory();
        }

        public void SelectBrowseFilter(string tabName)
        {
            var tab = SetSelectedTab(tabName);
            var databaseList = ExcelClient1().UITabControlTabList.UIAllTabPage.UIAllDatabaseListList;

            databaseList.SearchProperties[WpfList.PropertyNames.AutomationId] = $"{tabName}DatabaseList";
            databaseList.Container = tab;

            SelectBrowseFilter();
        }

        public void SelectDatabase(string databaseName, string filter = "All")
        {
            var tab = ExcelClient2().UITabControlTabList.UIAllTabPage;
            var databaseList = ExcelClient2().UITabControlTabList.UIAllTabPage.UIAllDatabaseListList;

            tab.SearchProperties[WpfTabPage.PropertyNames.Name] = filter;

            databaseList.SearchProperties[WpfList.PropertyNames.AutomationId] = $"{filter}DatabaseList";
            databaseList.SelectedItemsAsString = databaseName;
        }

        public List<string> GetDatabaseList(string filter = "All")
        {
            var tab = ExcelClient2().UITabControlTabList.UIAllTabPage;
            var databaseList = ExcelClient2().UITabControlTabList.UIAllTabPage.UIAllDatabaseListList;

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
            var txtDatabaseCode = ExcelClient().UIQuandlExcelAddincompPane.UIDatabaseCodeBoxEdit;

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
            var txtDatabaseCode = ExcelClient1().UIEODFBisnotavalidcodeText.UIEODFBisnotavalidcodeText1;

            txtDatabaseCode.WaitForControlReady();
            Assert.AreEqual(txtDatabaseCode.DisplayText, $"\"{code}\" is not a valid code.");
            Assert.AreNotEqual(AssertNextButtonEnabledExpectedValues.UINextButtonEnabled, uINextButton.Enabled, "Next button not enabled");
        }
    }
}
