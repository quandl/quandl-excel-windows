using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;

namespace Quandl.Test.CodedUI.UI.UDF_Builder
{
    [CodedUITest]
    public class DatasetDatatableSelectionTest
    {
        public UIMap UIMap => map ?? (map = new UIMap());
        private UIMap map;

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
        public void SelectDataset()
        {
            PassStepOne("EOD");

            UIMap.FilterDatasetsDatatables("Facebook");
            UIMap.SelectDatasetOrDatatableByName("Facebook Inc. (FB) Stock Prices\\, Dividends and Splits");
            UIMap.AssertCorrectDatasetDatatableCode("EOD/FB");
            UIMap.AssertCorrectUDFSignature("=QSERIES(\"EOD/FB\")");
        }

        [TestMethod]
        public void SelectDatatable()
        {
            PassStepOne("ZCP");

            UIMap.InputDatabaseCode("ZCP");
            UIMap.NextButton().WaitForControlEnabled();
            UIMap.ClickNextButton();
            UIMap.SelectDatasetOrDatatableByName("Zacks Master Table");
            UIMap.AssertCorrectDatasetDatatableCode("ZACKS/MT");
            UIMap.AssertCorrectUDFSignature("=QTABLE(\"ZACKS/MT\")");
        }

        [TestMethod]
        public void SelectDifferentDatatables()
        {
            UIMap.SelectDatasetOrDatatableByName("Zacks Master Table");
            UIMap.AssertCorrectDatasetDatatableCode("ZACKS/MT");
            UIMap.AssertCorrectUDFSignature("=QTABLE(\"ZACKS/MT\")");
            UIMap.SelectDatasetOrDatatableByName("Zacks Company Profiles");
            UIMap.AssertCorrectDatasetDatatableCode("ZACKS/CP");
            UIMap.AssertCorrectUDFSignature("=QTABLE(\"ZACKS/CP\")");
        }

        /// <summary>
        /// The SelectDatasetsFromVariousPages() method will test the functionality of the pagination buttons to
        /// multiple pages, and ensure that while changing pages, the selected code remains the same and the UDF
        /// signature will also remain intact until such time that a new dataset is selected.
        /// </summary>
        [TestMethod]
        public void SelectDatasetsFromVariousPages()
        {
            string selectedCode = "";

            PassStepOne("EOD");

            UIMap.AssertFirstPageButtonEnabled(false);
            UIMap.AssertPreviousPageButtonEnabled(false);
            UIMap.AssertNextPageButtonEnabled(true);
            UIMap.AssertLastPageButtonEnabled(true);
            UIMap.SelectDatasetOrDatatableByIndex(5);

            selectedCode = GetSelectedDatasetCode();
 
            UIMap.ClickDatasetPageButton(">>");
            UIMap.AssertFirstPageButtonEnabled(true);
            UIMap.AssertPreviousPageButtonEnabled(true);
            UIMap.AssertCorrectDatasetDatatableCode(selectedCode);
            UIMap.AssertCorrectUDFSignature($"=QSERIES(\"{selectedCode}\")");

            UIMap.SelectDatasetOrDatatableByIndex(8);
            selectedCode = GetSelectedDatasetCode();

            UIMap.ClickDatasetPageButton("<");
            UIMap.ClickDatasetPageButton("<<");
            UIMap.AssertFirstPageButtonEnabled(false);
            UIMap.AssertPreviousPageButtonEnabled(false);
            UIMap.AssertCorrectDatasetDatatableCode(selectedCode);
            UIMap.AssertCorrectUDFSignature($"=QSERIES(\"{selectedCode}\")");
        }

        private void PassStepOne(string databaseCode)
        {
            UIMap.InputDatabaseCode(databaseCode);
            UIMap.NextButton().WaitForControlEnabled();
            UIMap.ClickNextButton();
        }

        private string GetSelectedDatasetCode()
        {
            string selectedCode = UIMap.GetSelectedDatasetDatatableCode();
            UIMap.AssertCorrectDatasetDatatableCode(selectedCode);
            UIMap.AssertCorrectUDFSignature($"=QSERIES(\"{selectedCode}\")");
            return selectedCode;
        }
    }
}
