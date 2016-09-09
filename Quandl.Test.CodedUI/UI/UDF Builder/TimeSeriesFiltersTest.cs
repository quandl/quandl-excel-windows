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
using Quandl.Shared.Models;
using System.Linq;

namespace Quandl.Test.CodedUI.UI.UDF_Builder
{
    [CodedUITest]
    public class TimeSeriesFiltersTest
    {
        public UIMap UIMap => _map ?? (_map = new UIMap());
        private UIMap _map;
        private Dataset _dataset;
        private string _filterDateFormat;
        private string _udfDateFormat;

        public TimeSeriesFiltersTest()
        {
            _filterDateFormat = "dd-MMM-yyyy";
            _udfDateFormat = "yyyy-MM-dd";

            _dataset = new Dataset
            {
                DatabaseCode = "EOD",
                DatasetCode = "AAPL",
                Name = "Apple Inc. (AAPL) Stock Prices, Dividends and Splits"
            };
        }

        #region Additional test attributes

        [TestInitialize()]
        public void MyTestInitialize()
        {
            Playback.PlaybackSettings.DelayBetweenActions = 10;
            Playback.PlaybackSettings.MatchExactHierarchy = true;
            Playback.PlaybackSettings.SearchInMinimizedWindows = false;
            Playback.PlaybackSettings.SmartMatchOptions = SmartMatchOptions.None;
            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.Disabled;
            UIMap.ClearRegistryApiKey();
            UIMap.OpenExcelAndWorksheet();
            UIMap.OpenLoginPage();
            UIMap.LoginWithApiKey();
            CompleteStep1(_dataset.DatabaseCode);
            CompleteStep2(_dataset, _dataset.Name);
            CompleteStep3();
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            UIMap.ClearRegistryApiKey();
        }

        #endregion

        private void CompleteStep1(string databaseCode)
        {
            UIMap.InputDatabaseCode(databaseCode);
            UIMap.NextButton().WaitForControlEnabled();
            UIMap.ClickNextButton();
        }

        private void CompleteStep2(DataHolderDefinition dataHolder, string filterText = null)
        {
            if (filterText != null)
            {
                UIMap.FilterDatasetsDatatables(filterText);
            }
            UIMap.SelectDatasetOrDatatableByName(dataHolder.Name.Replace(",", "\\,"));
            UIMap.NextButton().WaitForControlEnabled();
            UIMap.ClickNextButton();
        }

        public void CompleteStep3(List<DataColumn> columns = null)
        {
            if (columns != null)
            {
                columns.ForEach(column => UIMap.SelectColumn(column));
            }

            UIMap.ClickNextButton();
        }

        private string QSeriesUDF(List<DateTime> dates = null)
        {
            if (dates == null || dates.Count == 0)
            {
                return $"=QSERIES(\"{_dataset.Code}\")";
            }
            else if (dates.Count == 1)
            {
                return $"=QSERIES(\"{_dataset.Code}\",\"{dates.First().ToString(_udfDateFormat)}\")";
            }
            else
            {
                List<string> strDates = dates.Select(date => { return date.ToString(_udfDateFormat); }).ToList();
                string datesArray = $"{{\"{string.Join("\",\"", strDates)}\"}}";
                return $"=QSERIES(\"{_dataset.Code}\",{datesArray})";
            }
        }

        [TestMethod]
        public void SelectAllHistoricalDateRangeFilter()
        {
            string expectedUDF = QSeriesUDF();

            UIMap.SelectDatasetDateRangeFilter("All Historical", "All");
            UIMap.AssertCorrectUDFSignature(expectedUDF);
        }

        [TestMethod]
        public void SelectSingleDateFilter()
        {
            DateTime date = new DateTime(2016, 1, 1);

            UIMap.SelectDatasetDateRangeFilter("Single Date", "Single");
            UIMap.SelectDatasetDateFromFilter(date.ToString(_filterDateFormat));

            string expectedUDF = QSeriesUDF(new List<DateTime>() { date });
            UIMap.AssertCorrectUDFSignature(expectedUDF);
        }

        [TestMethod]
        public void SelectPeriodRangeDateFilter()
        {
            DateTime fromDate = new DateTime(2016, 1, 1);
            DateTime toDate = DateTime.Today;

            UIMap.SelectDatasetDateRangeFilter("Period Range", "Range");
            UIMap.SelectDatasetDateFromFilter(fromDate.ToString(_filterDateFormat));
            UIMap.SelectDatasetDateToFilter(toDate.ToString(_filterDateFormat));

            string expectedUDF = QSeriesUDF(new List<DateTime> { fromDate, toDate });
            UIMap.AssertCorrectUDFSignature(expectedUDF);
        }
    }
}
