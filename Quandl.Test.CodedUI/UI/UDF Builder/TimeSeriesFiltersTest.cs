using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quandl.Shared.Models;
using Quandl.Test.CodedUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quandl.Test.CodedUI.UI.UDF_Builder
{
    [CodedUITest]
    public class TimeSeriesFiltersTest
    {
        private UIMap UIMap;
        private Dataset _dataset;
        private string _filterDateFormat;
        private string _udfDateFormat;

        public TimeSeriesFiltersTest()
        {
            UIMap = CodedUITestHelpers.UIMap;
            _dataset = CodedUITestHelpers.SampleDataset();
            _filterDateFormat = "dd-MMM-yyyy";
            _udfDateFormat = "yyyy-MM-dd";
        }

        #region Additional test attributes

        [TestInitialize()]
        public void MyTestInitialize()
        {
            CodedUITestHelpers.CompleteStep1(_dataset.DatabaseCode);
            CodedUITestHelpers.CompleteStep2(_dataset, _dataset.Name);
            CodedUITestHelpers.CompleteStep3(null);
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            UIMap.ClearRegistryApiKey();
        }

        #endregion

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
