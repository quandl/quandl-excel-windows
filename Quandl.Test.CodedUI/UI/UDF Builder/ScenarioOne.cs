using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quandl.Shared.Models;
using Quandl.Test.CodedUI.Helpers;

namespace Quandl.Test.CodedUI.UI.UDF_Builder
{
    /// <summary>
    /// Scenario one in the Excel Test Case Sheet
    /// </summary
    ///<remarks>
    ///Page 1 - Database(Step 1) :Enter Wiki in search.
    ///Page 2 - Data(step 2): Search for FMC Corp. (FMC) Prices, Dividends, Splits and Trading Volume and select it.
    ///Page 3 - Columns(step 3): Don't select columns.
    ///Page 4 - Filters(step 4): Select these filteres: All historical , Transformation: row-on-row %change.
    ///Page 5 - Placement(step 5): include headers.
    ///Verify UDF signature: =QSERIES(\"WIKI/FMC\",,,,\"rdiff\")
    ///Click insert.
    ///</remarks>

    [CodedUITest]
    public class ScenarioOne
    {
        public UIMap UIMap => map ?? (map = new UIMap());
        private UIMap map;
        private Dataset _dataset;

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
        public void TestCases1()
        {
            _dataset = CodedUITestHelpers.FreeDataset();
            var filters = CodedUITestHelpers.filtersAllHistRdiff();
            var expectedUDF = "=QSERIES(\"WIKI/FMC\",,,,\"rdiff\")";
            CodedUITestHelpers.CompleteStep1("WIKI");
            CodedUITestHelpers.CompleteStep2(_dataset, _dataset.Name);
            CodedUITestHelpers.CompleteStep3(null);
            CodedUITestHelpers.CompleteStep4(filters);
            CodedUITestHelpers.CompleteStep5(false);
            UIMap.AssertCorrectUDFSignature(expectedUDF);
            UIMap.ClickInsertButton();
        }
    }
}
