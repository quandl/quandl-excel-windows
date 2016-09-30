using Quandl.Shared.Models;
using System.Collections.Generic;

namespace Quandl.Test.CodedUI.Helpers
{
    public partial class CodedUITestHelpers
    {
        public static UIMap UIMap => _map ?? (_map = new UIMap());
        private static UIMap _map;
        private static Dataset _dataset;
        private static Datatable _datatable;
        private static List<DataColumn> _datasetColumns;
        private static List<DataColumn> _datatableColumns;
        private Dictionary<string, string> _filterOptions;

        public static Dataset SampleDataset()
        {
            if (_dataset == null)
            {
                _dataset = new Dataset
                {
                    DatabaseCode = "EOD",
                    DatasetCode = "AAPL",
                    Name = "Apple Inc. (AAPL) Stock Prices, Dividends and Splits"
                };
            }

            return _dataset;
        }

        public static Dataset FreeDataset()
        {
            if (_dataset == null)
            {
                _dataset = new Dataset
                {
                    DatabaseCode = "WIKI",
                    DatasetCode = "FMC",
                    Name = "FMC Corp. (FMC) Prices, Dividends, Splits and Trading Volume"
                };
            }

            return _dataset;
        }

        public static Dataset PremiumDataset()
        {
            if (_dataset == null)
            {
                _dataset = new Dataset
                {
                    DatabaseCode = "SF1",
                    DatasetCode = "SYPR_CURRENTRATIO_MRQ",
                    Name = "SYPRIS SOLUTIONS INC (NASDAQ:SYPR) - Current Ratio (Most Recent - Quarterly)"
                };
            }

            return _dataset;
        }

        public static Datatable SampleDatatable()
        {
            if (_datatable == null)
            {
                _datatable = new Datatable
                {
                    VendorCode = "ZACKS",
                    DatatableCode = "CP",
                    Name = "Zacks Company Profiles"
                };
            }

            return _datatable;
        }

        public static Datatable PremiumDatatable()
        {
            if (_datatable == null)
            {
                _datatable = new Datatable
                {
                    VendorCode = "ZACKS",
                    DatatableCode = "HDM",
                    Name = "Zacks Historical Daily Maintenance"
                };
            }

            return _datatable;
        }

        public static List<DataColumn> SampleDatasetColumns()
        {
            if (_datasetColumns == null)
            {
                _datasetColumns = new List<DataColumn>
                {
                    new DataColumn() { Name = "Volume", Parent = _dataset },
                    new DataColumn() { Name = "Open",   Parent = _dataset },
                    new DataColumn() { Name = "Close",  Parent = _dataset }
                };
            }

            return _datasetColumns;
        }

        public static List<DataColumn> DateOpenHighLowCloseExDividendDatasetColumns()
        {
            if (_datasetColumns == null)
            {
                _datasetColumns = new List<DataColumn>
                {
                    new DataColumn() { Name = "Date", Parent = _dataset },
                    new DataColumn() { Name = "Open", Parent = _dataset },
                    new DataColumn() { Name = "High", Parent = _dataset },
                    new DataColumn() { Name = "Low",  Parent = _dataset },
                    new DataColumn() { Name = "Close", Parent = _dataset },
                    new DataColumn() { Name = "Ex-Dividend", Parent = _dataset }
                };
            }

            return _datasetColumns;
        }

        public static List<DataColumn> DateValueColumns()
        {
            if (_datasetColumns == null)
            {
                _datasetColumns = new List<DataColumn>
                {
                    new DataColumn() { Name = "Date", Parent = _dataset },
                    new DataColumn() { Name = "Value", Parent = _dataset }
                };
            }

            return _datasetColumns;
        }

        public static List<DataColumn> SampleDatatableColumns()
        {
            if (_datasetColumns == null)
            {
                _datasetColumns = new List<DataColumn>
                {
                    new DataColumn() { Name = "ticker",         Parent = _datatable },
                    new DataColumn() { Name = "exchange",       Parent = _datatable },
                    new DataColumn() { Name = "address_line_1", Parent = _datatable },
                    new DataColumn() { Name = "city",           Parent = _datatable }
                };
            }

            return _datasetColumns;
        }

        public static List<DataColumn> ActionTypeMtickerStatusDatatableColumns()
        {
            if (_datasetColumns == null)
            {
                _datasetColumns = new List<DataColumn>
                {
                    new DataColumn() { Name = "action_type", Parent = _datatable },
                    new DataColumn() { Name = "m_ticker", Parent = _datatable },
                    new DataColumn() { Name = "status", Parent = _datatable },
                };
            }

            return _datasetColumns;
        }

        public static List<DataColumn> MtickerDatatableColumns()
        {
            if (_datasetColumns == null)
            {
                _datasetColumns = new List<DataColumn>
                {
                    new DataColumn() { Name = "m_ticker", Parent = _datatable },
                };
            }

            return _datasetColumns;
        }

        public static Dictionary<string, string> filtersAllHistRdiff()
        {
            var filterOptions = new Dictionary<string, string>();
            filterOptions.Add("date from", null);
            filterOptions.Add("single date", null);
            filterOptions.Add("transformation", "Row-on-row % change (rdiff)");
            filterOptions.Add("transformation code", "RDiff");
            filterOptions.Add("frequency", null);
            filterOptions.Add("sort", null);
            filterOptions.Add("limit", null);
            return filterOptions;
        }

        public static Dictionary<string, string> filtersAllHistQuarterlyDiff()
        {
            var filterOptions = new Dictionary<string, string>();
            filterOptions.Add("date from", null);
            filterOptions.Add("single date", null);
            filterOptions.Add("transformation", "Row-on-row change (diff)");
            filterOptions.Add("transformation code", "Diff");
            filterOptions.Add("frequency", "Quarterly");
            filterOptions.Add("frequency code", "Quarter");
            filterOptions.Add("sort", null);
            filterOptions.Add("limit", null);
            return filterOptions;
        }

        public static Dictionary<string, string> filtersDateRangetMonthlyDiff()
        {

            var filterOptions = new Dictionary<string, string>();
            filterOptions.Add("date from", "2016, 1, 1");
            filterOptions.Add("date to", "2016, 2, 1");
            filterOptions.Add("single date", null);
            filterOptions.Add("transformation", "Row-on-row change (diff)");
            filterOptions.Add("transformation code", "Diff");
            filterOptions.Add("frequency", "Monthly");
            filterOptions.Add("frequency code", "Month");
            filterOptions.Add("sort", null);
            filterOptions.Add("limit", null);
            return filterOptions;
        }

        public static Dictionary<string, string> filtersSingleDateDailyCum()
        {

            var filterOptions = new Dictionary<string, string>();
            filterOptions.Add("date from", null);
            filterOptions.Add("single date", "2016, 2, 1");
            filterOptions.Add("transformation", "Cumulative sum");
            filterOptions.Add("transformation code", "Cumul");
            filterOptions.Add("frequency", "Daily");
            filterOptions.Add("frequency code", "Day");
            filterOptions.Add("sort", null);
            filterOptions.Add("limit", null);
            return filterOptions;
        }

        public static Dictionary<string, string> selectStockUsMfOne()
        {
            var browseOptions = new Dictionary<string, string>();
            browseOptions.Add("data type", "Stock Data");
            browseOptions.Add("region", "United States");
            browseOptions.Add("category", "Stock Prices End of Day, Current and Historical");
            browseOptions.Add("database", "Free WIKI Wiki EOD Stock Prices");
            return browseOptions;
        }

        public static Dictionary<string, string> selectStockUsSfone()
        {
            var browseOptions = new Dictionary<string, string>();
            browseOptions.Add("data type", "Stock Data");
            browseOptions.Add("region", "United States");
            browseOptions.Add("category", "Fundamentals and Financial Ratios");
            browseOptions.Add("database", "Premium SF1 Core US Fundamentals Data");
            return browseOptions;
        }
    }
}
