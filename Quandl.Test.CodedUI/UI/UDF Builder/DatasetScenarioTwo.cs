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
    ///Page 1 - Database(Step 1): look for wiki using browse.
    ///Page 2 - Data(step 2): Search for FMC Corp. (FMC) Prices, Dividends, Splits and Trading Volume and select it.
    ///Page 3 - Columns(step 3): Select the following columns Date Open High Low Close Ex-Dividend.
    ///Page 4 - Filters(step 4): Select these filteres: All historical , Transformation: row-on-row change, frequency quarterly.
    ///Page 5 - Placement(step 5): include headers.
    ///Verify UDF signature: =QSERIES({\"WIKI/FMC/DATE\",\"WIKI/FMC/OPEN\",\"WIKI/FMC/HIGH\",\"WIKI/FMC/LOW\",\"WIKI/FMC/CLOSE\",\"WIKI/FMC/EX-DIVIDEND\"},,\"quarterly\",,\"diff\")
    ///Click insert.
    ///</remarks>

    [CodedUITest]
    public class DatasetScenarioTwo
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
        public void DatasetTestCases2()
        {
            var browseOptions = CodedUITestHelpers.selectStockUsMfOne();
            _dataset = CodedUITestHelpers.FreeDataset();
            _datasetColumns = CodedUITestHelpers.DateOpenHighLowCloseExDividendDatasetColumns();
            var filters = CodedUITestHelpers.filtersAllHistQuarterlyDiff();
            var expectedUDF = "=QSERIES({\"WIKI/FMC/DATE\",\"WIKI/FMC/OPEN\",\"WIKI/FMC/HIGH\",\"WIKI/FMC/LOW\",\"WIKI/FMC/CLOSE\",\"WIKI/FMC/EX-DIVIDEND\"},,\"quarterly\",,\"diff\")";
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
