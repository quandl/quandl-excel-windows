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

        private Dictionary<string, string> _filterOptions;

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
            CodedUITestHelpers.SetupCodedUITest();
            CodedUITestHelpers.CompleteStep1(_dataset.DatabaseCode);
            CodedUITestHelpers.CompleteStep2(_dataset, _dataset.Name);
            CodedUITestHelpers.CompleteStep3(null);

            ResetFilterOptions();
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            CodedUITestHelpers.CompleteCodedUITest();
        }

        #endregion

        private void ResetFilterOptions()
        {
            _filterOptions = new Dictionary<string, string>();
            _filterOptions.Add("dataset_code", _dataset.Code);
            _filterOptions.Add("date_range", "");
            _filterOptions.Add("frequency", "");
            _filterOptions.Add("sort", "");
            _filterOptions.Add("transformation", "");
            _filterOptions.Add("limit", "");
        }

        private string QSeriesUDF(List<DateTime> dates = null)
        {
            List<string> udfParams = new List<string>();
            udfParams.Add($"\"{_dataset.Code}\"");

            if (dates == null || dates.Count == 0)
            {
                udfParams.Add(null);
            }
            else if (dates != null && dates.Count == 1)
            {
                udfParams.Add($"\"{dates.First().ToString(_udfDateFormat)}\"");
            }
            else if (dates != null && dates.Count == 2)
            {
                List<string> formattedDates = dates.Select(date => { return date.ToString(_udfDateFormat); }).ToList();
                udfParams.Add($"{{\"{string.Join("\",\"", formattedDates)}\"}}");
            }

            string qseries = string.Join(",", udfParams.Where(param => !String.IsNullOrEmpty(param)).ToList());
            return $"=QSERIES({qseries})";
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

        [TestMethod]
        public void SelectFrequencyFilter()
        {
            _filterOptions["frequency"] = "Quarter";

            UIMap.SelectFrequencyFilter("Quarterly", _filterOptions["frequency"]);
            UIMap.AssertCorrectUDFSignature();
        }

        [TestMethod]
        public void SelectTransformationFilter()
        {
            _filterOptions["transforamtion"] = "Diff";

            UIMap.SelectTransformationFilter("Row-on-row change (diff)", _filterOptions["transformation"]);
        }

        [TestMethod]
        public void SelectSortFilter()
        {
            _filterOptions["sort"] = "Ascending";

            UIMap.SelectSortFilter("Ascending", _filterOptions["sort"]);
        }

        [TestMethod]
        public void SelectLimitFilter()
        {
            _filterOptions["limit"] = "10";

            UIMap.SelectLimitFilter(_filterOptions["limit"]);
        }
    }
}
