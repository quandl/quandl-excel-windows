using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using PropertyChanged;
using Quandl.Excel.Addin.UI.Helpers;
using Quandl.Shared.Models;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    [ImplementPropertyChanged]
    public class StateControl : INotifyPropertyChanged
    {
        // The different types of chains
        public enum ChainTypes
        {
            TimeSeries,
            Datatables
        }

        // Dataset filter options
        public enum TimeSeriesFilterCollapse
        {
            [Description("None (default)")] Default,
            [Description("Daily")] Daily,
            [Description("Weekly")] Week,
            [Description("Monthly")] Month,
            [Description("Quarterly")] Quarter,
            [Description("Annual")] Annual
        }

        public enum TimeSeriesFilterSorts
        {
            [Description("Default")] Default,
            [Description("Ascending")] Ascending,
            [Description("Descending")] Descending
        }

        public enum TimeSeriesFilterTransformations
        {
            [Description("None (default)")] Default,
            [Description("Row-on-row change (diff)")] Diff,
            [Description("Row-on-row % change (rdiff)")] RDiff,
            [Description("Latest value as % increment (rdiff_from)")] RDiffFrom,
            [Description("Cumulative sum (cumul)")] Cumulative,
            [Description("Scale series to start at 100 (normalize)")] Normalize
        }

        public enum TimeSeriesFilterTypes
        {
            [Description("Single Date")] Single,
            [Description("Period Range")] Range,
            [Description("All Historical")] All
        }

        // Singleton state to be shared between different forms
        private static StateControl _instance;

        // The chain of forms for time series
        private static readonly string[] timeSeriesChain =
        {
            "DatabaseSelection",
            "DatasetDatatableSelection",
            "ColumnSelection",
            "TimeSeriesFilters",
            "FormulaInserter"
        };

        // The chain of forms for time series
        private static readonly string[] datatableChain =
        {
            "DatabaseSelection",
            "DatasetDatatableSelection",
            "ColumnSelection",
            "DatatableFilters",
            "FormulaInserter"
        };

        public StateControl()
        {
            Reset();
            PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName != "UdfFormula")
                {
                    UpdateFormula();
                }
            };
            Columns.CollectionChanged += delegate { UpdateFormula(); };
        }

        public static StateControl Instance => _instance ?? (_instance = new StateControl());

        public int CurrentStep { get; internal set; }

        public string UdfFormula { get; set; }

        public string DataCode { get; internal set; }
        public List<string> DataSetTableSelection { get; internal set; } = new List<string>();

        public Provider Provider { get; internal set; }

        public bool ValidateCode { get; internal set; } = false;

        public IList<DataCodeCollection> AvailableCodeColumns { get; set; } =
            new ObservableCollection<DataCodeCollection>();

        public ObservableCollection<DataCodeColumn> Columns { get; } = new ObservableCollection<DataCodeColumn>();

        // Dataset Filters
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSeriesFilterTypes DateTypeFilter { get; set; }
        public TimeSeriesFilterCollapse TimeSeriesCollapseFilter { get; set; }
        public TimeSeriesFilterTransformations TimeSeriesTransformationFilter { get; set; }
        public TimeSeriesFilterSorts TimeSeriesSortFilter { get; set; }
        public int? TimeSeriesLimitFilter { get; set; }

        public ChainTypes ChainType { get; internal set; } = ChainTypes.Datatables;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Reset()
        {
            UdfFormula = "";
            CurrentStep = 0;
            ChainType = ChainTypes.Datatables;
            DataCode = null;
            DataSetTableSelection = new List<string>();
            Columns.Clear();

            // Reset Dataset Filters
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
            DateTypeFilter = TimeSeriesFilterTypes.All;
            TimeSeriesCollapseFilter = TimeSeriesFilterCollapse.Default;
            TimeSeriesTransformationFilter = TimeSeriesFilterTransformations.Default;
            TimeSeriesSortFilter = TimeSeriesFilterSorts.Default;
            TimeSeriesLimitFilter = null;

            // The following is only a sample while the real data is unavailable
            var dataCodeCollection = new DataCodeCollection("NSE", "National Stock Exchange");
            AvailableCodeColumns.Clear();
            AvailableCodeColumns.Add(dataCodeCollection);
            dataCodeCollection.Columns.Add(new DataCodeColumn(dataCodeCollection, dataCodeCollection.Name,
                "Date"));
            dataCodeCollection.Columns.Add(new DataCodeColumn(dataCodeCollection, dataCodeCollection.Name,
                "High"));
            dataCodeCollection.Columns.Add(new DataCodeColumn(dataCodeCollection, dataCodeCollection.Name,
                "Low"));
            dataCodeCollection.Columns.Add(new DataCodeColumn(dataCodeCollection, dataCodeCollection.Name,
                "Open"));
            dataCodeCollection.Columns.Add(new DataCodeColumn(dataCodeCollection, dataCodeCollection.Name,
                "Close"));
        }

        public void ChangeCode(string dataCode, ChainTypes ct)
        {
            Reset(); // Reset the chain because the code has been chained.
            ChainType = ct;
            DataCode = dataCode;
        }

        // Move forward rules
        // Step 1: DataCode has been entered
        // Step 2: At least on dataset/datatable has been selected allowing the creation of a Quandl Code
        // Step 3: (Optional) Column selection
        // Step 4: (Optional) Filter selection
        // Step 5: (Optional) Insert UDF formula
        public bool CanMoveForward()
        {
            return (CurrentStep == 0 &&  IsValidateDataCode()) ||
                   (CurrentStep == 1 && DataSetTableSelection.Count > 0) ||
                   (CurrentStep >= 2);
        }

        public void NextStep()
        {
            if (CanMoveForward())
            {
                CurrentStep++;
            }
        }

        public string[] GetStepList()
        {
            return ChainType == ChainTypes.TimeSeries ? timeSeriesChain : datatableChain;
        }

        private bool IsValidateDataCode()
        {
            return ValidateCode && !string.IsNullOrEmpty(DataCode);
        }

        private void UpdateFormula()
        {
            // If the DataCode has been nullified or blanked out simply erase the formula
            if (string.IsNullOrEmpty(DataCode))
            {
                UdfFormula = "";
                return;
            }

            // At least the DataCode has been given
            UdfFormula = new FormulaBuilder(this).ToUdf();
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool TimeseriesFilterAfter(string filterType)
        {
            switch (filterType)
            {
                case "date":
                    return TimeSeriesCollapseFilter != TimeSeriesFilterCollapse.Default ||
                           TimeseriesFilterAfter("collapse");
                case "collapse":
                    return TimeSeriesSortFilter != TimeSeriesFilterSorts.Default || TimeseriesFilterAfter("sort");
                case "sort":
                    return TimeSeriesTransformationFilter !=
                           TimeSeriesFilterTransformations.Default || TimeseriesFilterAfter("transformation");
                case "transformation":
                    return !(TimeSeriesLimitFilter == null || TimeSeriesLimitFilter <= 0) ||
                           TimeseriesFilterAfter("limit");
                default:
                    return false;
            }
        }
    }
}
