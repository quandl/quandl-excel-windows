using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Quandl.Test.CodedUI.UI.UDF_Builder
{
    [CodedUITest]
    public class DatabaseSelectionTest
    {
        public UIMap UIMap => map ?? (map = new UIMap());
        private UIMap map;

        #region Additional test attributes

        [ClassInitialize]
        public static void TestClassInitialize(TestContext context)
        {
            string username = context.Properties["username"].ToString();
            string password = context.Properties["password"].ToString();
            string api_key  = context.Properties["api_key"].ToString();
        }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            Playback.PlaybackSettings.DelayBetweenActions = 10;
            UIMap.ClearRegistryApiKey();
            UIMap.OpenExcelAndWorksheet();
            UIMap.OpenLoginPage();
            UIMap.LoginWithApiKey();
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            UIMap.ClearRegistryApiKey();
        }

        #endregion

        private class DatabaseFilter
        {
            public string Name { get; set; }
            public int Length { get; set; }
        }

        [TestMethod]
        public void SelectDatabaseFromAllDatabaseFilter()
        {
            UIMap.SelectBrowseCategory("Stock Data", "United States", "Fundamentals and Financial Ratios");
            UIMap.SelectDatabase("Premium MF1 Mergent Global Fundamentals Data");
            UIMap.AssertSelectedDatabaseCode("MF1");
        }

        [TestMethod]
        public void SelectDatatableCollectionsFromAllDatabaseFilter()
        {
            UIMap.SelectBrowseCategory("Stock Data", "United States", "Fundamentals and Financial Ratios");
            UIMap.SelectDatabase("Premium ZCP Zacks Company Profiles");
            UIMap.AssertSelectedDatabaseCode("ZCP");
        }

        [TestMethod]
        public void SelectDatabaseFromPremiumDatabaseFilter()
        {
            UIMap.SelectBrowseCategory("Currency Data", "Bitcoin", "Spot Exchange Rates");
            UIMap.SelectBrowseFilter("Premium");
            UIMap.SelectDatabase("Premium BNC1 BNC Liquid Index", "Premium");
            UIMap.AssertSelectedDatabaseCode("BNC1");
            UIMap.SelectDatabase("Premium BNC2 BNC Digital Currency Indexed EOD", "Premium");
            UIMap.AssertSelectedDatabaseCode("BNC2");
        }

        [TestMethod]
        public void SelectDatabaseFromFreeDatabaseFilter()
        {
            UIMap.SelectBrowseCategory("Futures Data", "Europe", "Individual Contracts");
            UIMap.SelectBrowseFilter("Free");
            UIMap.SelectDatabase("Free EUREX EUREX Futures Data", "Free");
            UIMap.AssertSelectedDatabaseCode("EUREX");
            UIMap.SelectDatabase("Free LIFFE LIFFE Futures Data", "Free");
            UIMap.AssertSelectedDatabaseCode("LIFFE");
            UIMap.SelectDatabase("Free EEX European Energy Exchange", "Free");
            UIMap.AssertSelectedDatabaseCode("EEX");
        }

        [TestMethod]
        public void SelectDatabaseFromMultipleDatabaseFilters()
        {
            UIMap.SelectBrowseCategory("Futures Data", "Europe", "Individual Contracts");

            List<string> databaseList;
            List<DatabaseFilter> filters = new List<DatabaseFilter> {
                new DatabaseFilter { Name = "Premium", Length = 4 },
                new DatabaseFilter { Name = "Free", Length = 3 },
                new DatabaseFilter { Name = "All", Length = 7 },
            };

            foreach (DatabaseFilter filter in filters)
            {
                UIMap.SelectBrowseFilter(filter.Name);
                databaseList = UIMap.GetDatabaseList(filter.Name);
                Assert.AreEqual(databaseList.Count, filter.Length);
            }
        }

        [TestMethod]
        public void SelectDatabaseByValidDatabaseCode()
        {
            string databaseCode = "EOD";
            UIMap.InputDatabaseCode(databaseCode);
            UIMap.AssertValidDatabaseCode();
        }

        [TestMethod]
        public void SelectDatabaseWithInvalidDatabaseCode()
        {
            string databaseCode = "EOD/FB";
            UIMap.InputDatabaseCode(databaseCode);
            UIMap.AssertInvalidDatabaseCode(databaseCode);
        }
    }
}
