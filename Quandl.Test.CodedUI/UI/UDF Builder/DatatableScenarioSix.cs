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
    ///Page 1 - Database(Step 1): look for ZFA using search.
    ///Page 2 - Data(step 2): select HDM.
    ///Page 3 - Columns(step 3): Select the following columns mticker.
    ///Page 4 - Filters(step 4): Select these filteres: mticker : FTXD.
    ///Page 5 - Placement(step 5): include headers.
    ///Verify UDF signature: =QTABLE(\"ZACKS/HDM\",\"m_ticker\",\"m_ticker\",\"FTXD\")
    ///Click insert.
    ///</remarks>

    [CodedUITest]
    public class DatatableScenarioSix
    {
        public UIMap UIMap => map ?? (map = new UIMap());
        private UIMap map;
        private static Datatable _datatable;
        private static List<DataColumn> _datatableColumns;

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
        public void DatatableTestCases6()
        {
            _datatable = CodedUITestHelpers.PremiumDatatable();
            _datatableColumns = CodedUITestHelpers.MtickerDatatableColumns();
            var filters = CodedUITestHelpers.filtersAllHistRdiff();
            var expectedUDF = "=QTABLE(\"ZACKS/HDM\",\"m_ticker\",\"m_ticker\",\"FTXD\")";
            CodedUITestHelpers.CompleteStep1("ZFA");
            CodedUITestHelpers.CompleteStep2(_datatable, null);
            CodedUITestHelpers.CompleteStep3(_datatableColumns);
            UIMap.EnterMtickerDatatableFilters("FTXD");
            UIMap.ClickNextButton();
            CodedUITestHelpers.CompleteStep5(false);
            UIMap.AssertCorrectUDFSignature(expectedUDF);
            UIMap.ClickInsertButton();
        }
    }
}
