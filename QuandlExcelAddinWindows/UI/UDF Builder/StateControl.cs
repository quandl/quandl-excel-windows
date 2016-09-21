using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using PropertyChanged;
using Quandl.Shared.Models;
using Quandl.Excel.Addin.UI.UDF_Builder.Filters;
using Quandl.Excel.Addin.UI.UDF_Builder.FormulaBuilders;

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
            PropertyChanged += OnPropertyChanged;
            Columns.CollectionChanged += delegate { UpdateFormula(); };
            AvailableDataHolders.CollectionChanged += delegate { OnPropertyChanged("DatasetOrDatatable"); };
        }

        public static StateControl Instance => _instance ?? (_instance = new StateControl());

        public int CurrentStep { get; internal set; }

        public string UdfFormula { get; set; }

        public IList<string> QuandlCodes => AvailableDataHolders.Select(CodeFromDataHolder).ToList();

        public Provider Provider { get; internal set; }
        public ChainTypes ChainType { get; internal set; } = ChainTypes.Datatables;

        public ObservableCollection<DataHolderDefinition> AvailableDataHolders { get; internal set; } =
            new ObservableCollection<DataHolderDefinition>();

        public String PreviousDataHolderCode { get; set; }

        public ObservableCollection<DataColumn> Columns { get; } = new ObservableCollection<DataColumn>();

        public StackPanel FiltersGroup { get; set; } = null;

        // Dataset Filters
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSeriesFilterTypes DateTypeFilter { get; set; }
        public TimeSeriesFilterCollapse TimeSeriesCollapseFilter { get; set; }
        public TimeSeriesFilterTransformations TimeSeriesTransformationFilter { get; set; }
        public TimeSeriesFilterSorts TimeSeriesSortFilter { get; set; }
        public int? TimeSeriesLimitFilter { get; set; }

        public Hashtable DatatableFilters { get; } = new Hashtable();

        public bool IsFilterChanged { get; set; }

        // Generic Options
        public bool IncludeHeaders { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "UdfFormula")
            {
                UpdateFormula();
            }
        }

        public void Reset(int step = 0)
        {
            AvailableDataHolders.Clear();
            Columns.Clear();
            UdfFormula = "";
            CurrentStep = step;
            // reset provider at step 0 or 1, need keep provider selected at step 1
            if (step == 0 || step == 1)
            {
                ChainType = ChainTypes.Datatables;
                Provider = null;
            }
            IncludeHeaders = true;

            // Reset Dataset Filters
            StartDate = null;
            EndDate = null;
            DateTypeFilter = TimeSeriesFilterTypes.All;
            TimeSeriesCollapseFilter = TimeSeriesFilterCollapse.Default;
            TimeSeriesTransformationFilter = TimeSeriesFilterTransformations.Default;
            TimeSeriesSortFilter = TimeSeriesFilterSorts.Default;
            TimeSeriesLimitFilter = null;

            // Reset Datatable Filters
            DatatableFilters.Clear();
        }

        public void ChangeCode(Provider provider, ChainTypes ct)
        {
            Reset(); // Reset the chain because the code has been chained.
            ChainType = ct;
            Provider = provider;
        }

        // Move forward rules
        // Step 1: DataCode has been entered
        // Step 2: At least on dataset/datatable has been selected allowing the creation of a Quandl Code
        // Step 3: (Optional) Column selection
        // Step 4: (Optional) Filter selection
        // Step 5: (Optional) Insert UDF formula
        public bool CanMoveForward()
        {
            return (CurrentStep == 0 && Provider != null) ||
                   (CurrentStep == 1 && AvailableDataHolders.Count > 0) ||
                   (CurrentStep >= 2 && CurrentStep + 1 < GetStepList().Length);
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

        private void UpdateFormula()
        {
            // If the DataCode has been nullified or blanked out simply erase the formula
            if (Provider == null || AvailableDataHolders.Count == 0)
            {
                UdfFormula = "";
                return;
            }

            // At least the DataCode has been given
            UdfFormula = (ChainType == ChainTypes.Datatables) ? (new QTable(this)).ToUdf() : (new QSeries(this)).ToUdf();
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
                case "limit":
                    return !IncludeHeaders ||
                           TimeseriesFilterAfter("headers");
                default:
                    return false;
            }
        }

        public string CodeFromDataHolder(DataHolderDefinition dh)
        {
            return ChainType == ChainTypes.TimeSeries
                ? ((Dataset) dh).Code
                : ((Datatable) dh).Code;
        }
    }
}
