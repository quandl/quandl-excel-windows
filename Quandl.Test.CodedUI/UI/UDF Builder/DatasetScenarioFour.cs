using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quandl.Shared.Models;
using Quandl.Test.CodedUI.Helpers;
using System.Collections.Generic;

namespace Quandl.Test.CodedUI.UI.UDF_Builder
{
    /// <summary>
    /// Scenario one in the Excel Test Case Sheet
    /// </summary
    ///<remarks>
    ///Page 1 - Database(Step 1): look for sf1 using browse.
    ///Page 2 - Data(step 2): SYPRIS SOLUTIONS INC (NASDAQ:SYPR) - Current Ratio (Most Recent - Quarterly.
    ///Page 3 - Columns(step 3): Select the following columns Date, Value.
    ///Page 4 - Filters(step 4): Select these filteres: All historical , Transformation: row-on-row %change.
    ///Page 5 - Placement(step 5): include headers.
    ///Verify UDF signature: =QSERIES({\"SF1/SYPR_CURRENTRATIO_MRQ/DATE\",\"SF1/SYPR_CURRENTRATIO_MRQ/VALUE\"},,,,\"rdiff\")
    ///Click insert.
    ///</remarks>

    [CodedUITest]
    public class DatasetScenarioFour
    {
        public UIMap UIMap => map ?? (map = new UIMap());
        private UIMap map;
        private Dataset _dataset;
        private static List<DataColumn> _datasetColumns;

        #region Additional test attributes

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
        [TestMethod]
        public void DatasetTestCases4()
        {
            var browseOptions = CodedUITestHelpers.selectStockUsSfone();
            _dataset = CodedUITestHelpers.PremiumDataset();
            _datasetColumns = CodedUITestHelpers.DateValueColumns();
            var filters = CodedUITestHelpers.filtersAllHistRdiff();
            var expectedUDF = "=QSERIES({\"SF1/SYPR_CURRENTRATIO_MRQ/DATE\",\"SF1/SYPR_CURRENTRATIO_MRQ/VALUE\"},,,,\"rdiff\")";
            CodedUITestHelpers.CompleteBrowseStep1(browseOptions);
            CodedUITestHelpers.CompleteStep2(_dataset, _dataset.Name);
            CodedUITestHelpers.CompleteStep3(_datasetColumns);
            CodedUITestHelpers.CompleteStep4(filters);
            CodedUITestHelpers.CompleteStep5(false);
            UIMap.AssertCorrectUDFSignature(expectedUDF);
            UIMap.ClickInsertButton();
        }
    }
}

