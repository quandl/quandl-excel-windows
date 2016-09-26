using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quandl.Shared.Models;
using Quandl.Test.CodedUI.Helpers;
using System.Collections.Generic;
using ExcelApp = Microsoft.Office.Interop.Excel;

namespace Quandl.Test.CodedUI.UI.UDF_Builder
{
    /// <summary>
    /// Scenario one in the Excel Test Case Sheet
    /// </summary
    ///<remarks>
    ///Page 1 - Database(Step 1): look for ZFA using search.
    ///Page 2 - Data(step 2): select HDM.
    ///Page 3 - Columns(step 3): Select the following columns mticker.
    ///Page 4 - Filters(step 4): No filters.
    ///Page 5 - Placement(step 5): include headers.
    ///Verify UDF signature: =QTABLE(\"ZACKS/HDM\",\"m_ticker\")
    ///Click insert.
    ///</remarks>

    [CodedUITest]
    public class DatatableScenarioSix
    {
        public UIMap UIMap => map ?? (map = new UIMap());
        private UIMap map;
        private static Datatable _datatable;
        private static List<DataColumn> _datatableColumns;
        private ExcelApp.Application _excelApp;
        private ExcelApp.Worksheet _worksheet;

        #region Additional test attributes

        [TestInitialize()]
        public void MyTestInitialize()
        {
            List <object> testPre = CodedUITestHelpers.PrepareTest();
            _excelApp = (ExcelApp.Application)testPre[0];
            _worksheet = (ExcelApp.Worksheet)testPre[1];
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            UIMap.ClearRegistryApiKey();
            _excelApp.DisplayAlerts = false;
            _excelApp.Quit();
        }

        #endregion  
        [TestMethod]
        public void DatatableTestCases6()
        {
            UIMap.OpenLoginPage();
            UIMap.LoginWithApiKey();
            _datatable = CodedUITestHelpers.PremiumDatatable();
            _datatableColumns = CodedUITestHelpers.MtickerDatatableColumns();
            var filters = CodedUITestHelpers.filtersAllHistRdiff();
            var expectedUDF = "=QTABLE(\"ZACKS/HDM\",\"m_ticker\")";
            CodedUITestHelpers.CompleteStep1("ZFA");
            CodedUITestHelpers.CompleteStep2(_datatable, null);
            CodedUITestHelpers.CompleteStep3(_datatableColumns);
            UIMap.ClickNextButton();
            CodedUITestHelpers.CompleteStep5(false);
            UIMap.AssertCorrectUDFSignature(expectedUDF);
            UIMap.ClickInsertButton();
            System.Threading.Thread.Sleep(3000);
            var actualCellHeader = _worksheet.Cells[1, 1].Value2;
            UIMap.AssertCorrectCellValue(actualCellHeader, "M_TICKER");
            var actualCellValue = _worksheet.Cells[2, 1].Value2;
            var numberOfAttempt = 0;
            while (numberOfAttempt <= 5)
            {
                if (actualCellValue != null)
                {
                    UIMap.AssertCorrectCellValue(actualCellHeader, "M_TICKER");
                    break;
                }

                else
                {
                    numberOfAttempt += 1;
                    System.Threading.Thread.Sleep(3000);
                }
            }
        }
    }
}
