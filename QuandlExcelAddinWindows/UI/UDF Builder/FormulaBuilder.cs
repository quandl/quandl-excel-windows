using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Quandl.Excel.Addin.UI.Helpers;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    internal class FormulaBuilder
    {
        private readonly StateControl _stateControl;

        public FormulaBuilder(StateControl stateControl)
        {
            _stateControl = stateControl;
        }

        private ObservableCollection<DataCodeColumn> Columns => _stateControl.Columns;
        private string DataCode => _stateControl.DataCode;

        private StateControl.TimeSeriesFilterCollapse TimeSeriesCollapseFilter => _stateControl.TimeSeriesCollapseFilter
            ;

        private StateControl.TimeSeriesFilterSorts TimeSeriesSortFilter => _stateControl.TimeSeriesSortFilter;

        private StateControl.TimeSeriesFilterTransformations TimeSeriesTransformationFilter
            => _stateControl.TimeSeriesTransformationFilter;

        private int? TimeSeriesLimitFilter => _stateControl.TimeSeriesLimitFilter;

        private ObservableCollection<DataCodeColumn> DataCodeColumns => _stateControl.Columns;

        private DateTime EndDate => _stateControl.EndDate;

        private DateTime StartDate => _stateControl.StartDate;

        private StateControl.TimeSeriesFilterTypes DateTypeFilter => _stateControl.DateTypeFilter;

        public string ToUdf() => _stateControl.ChainType == StateControl.ChainTypes.TimeSeries
            ? ProduceQdataFormula()
            : ProduceQtableFormula();

        private string ProduceQdataFormula()
        {
            var formulaComponents = new List<string>();

            AddQuandlCodeAndColumns(formulaComponents);
            AddDateFilters(formulaComponents);
            AddCollapseFilters(formulaComponents);
            AddSortFilters(formulaComponents);
            AddTransformationFilters(formulaComponents);
            AddLimitFilters(formulaComponents);

            // Close off the formula
            return $"=QDATA({string.Join(",", formulaComponents.Select(n => n.ToString()).ToArray())})";
        }

        private void AddQuandlCodeAndColumns(List<string> formulaComponents)
        {
            if (DataCodeColumns.Count == 1)
            {
                formulaComponents.Add($"\"{DataCode}/{Columns[0].ColumnName}\"");
            }
            else if (Columns.Count > 1)
            {
                formulaComponents.Add(
                    $"{{{string.Join(",", Columns.Select(n => $"\"{DataCode}/{n.ColumnName}\"".ToString()).ToArray())}}}");
            }
            else
            {
                formulaComponents.Add($"\"{DataCode}\"");
            }
        }

        private void AddLimitFilters(List<string> formulaComponents)
        {
            // Add limit
            if (TimeSeriesLimitFilter != null || TimeSeriesLimitFilter > 0)
            {
                formulaComponents.Add(
                    $"{TimeSeriesLimitFilter}");
            }
        }

        private void AddCollapseFilters(List<string> formulaComponents)
        {
            if (TimeSeriesCollapseFilter != StateControl.TimeSeriesFilterCollapse.Default)
            {
                formulaComponents.Add($"\"{CollapseToStringConverter(TimeSeriesCollapseFilter)}\"");
            }
            else if (_stateControl.TimeseriesFilterAfter("collapse"))
            {
                formulaComponents.Add("");
            }
        }

        private void AddDateFilters(List<string> formulaComponents)
        {
            if (DateTypeFilter == StateControl.TimeSeriesFilterTypes.Single)
            {
                formulaComponents.Add($"\"{StartDate.ToString("yyyy-M-d")}\"");
            }
            else if (DateTypeFilter == StateControl.TimeSeriesFilterTypes.Range)
            {
                formulaComponents.Add(
                    $"{{{string.Join(",", $"\"{StartDate.ToString("yyyy-M-d")}\"", $"\"{EndDate.ToString("yyyy-M-d")}\"")}}}");
            }
            else if (_stateControl.TimeseriesFilterAfter("date"))
            {
                formulaComponents.Add("");
            }
        }

        private void AddSortFilters(List<string> formulaComponents)
        {
            if (TimeSeriesSortFilter != StateControl.TimeSeriesFilterSorts.Default)
            {
                formulaComponents.Add($"\"{SortToStringConverter(TimeSeriesSortFilter)}\"");
            }
            else if (_stateControl.TimeseriesFilterAfter("sort"))
            {
                formulaComponents.Add("");
            }
        }

        private void AddTransformationFilters(List<string> formulaComponents)
        {
            if (TimeSeriesTransformationFilter != StateControl.TimeSeriesFilterTransformations.Default)
            {
                formulaComponents.Add(
                    $"\"{TransformationToStringConverter(TimeSeriesTransformationFilter)}\"");
            }
            else if (_stateControl.TimeseriesFilterAfter("transformation"))
            {
                formulaComponents.Add("");
            }
        }

        private string TransformationToStringConverter(
            StateControl.TimeSeriesFilterTransformations timeSeriesTransformationFilter)
        {
            switch (timeSeriesTransformationFilter)
            {
                case StateControl.TimeSeriesFilterTransformations.Diff:
                    return "diff";
                case StateControl.TimeSeriesFilterTransformations.RDiff:
                    return "rdiff";
                case StateControl.TimeSeriesFilterTransformations.RDiffFrom:
                    return "rdiff_from";
                case StateControl.TimeSeriesFilterTransformations.Cumulative:
                    return "cumul";
                case StateControl.TimeSeriesFilterTransformations.Normalize:
                    return "normalize";
            }
            throw new ArgumentException("Unknown transformation type specified.");
        }

        private string CollapseToStringConverter(StateControl.TimeSeriesFilterCollapse timeSeriesCollapseFilter)
        {
            switch (timeSeriesCollapseFilter)
            {
                case StateControl.TimeSeriesFilterCollapse.Daily:
                    return "daily";
                case StateControl.TimeSeriesFilterCollapse.Week:
                    return "weekly";
                case StateControl.TimeSeriesFilterCollapse.Month:
                    return "monthly ";
                case StateControl.TimeSeriesFilterCollapse.Quarter:
                    return "quarterly";
                case StateControl.TimeSeriesFilterCollapse.Annual:
                    return "annual";
            }
            throw new ArgumentException("Unknown collapse type specified.");
        }

        private string SortToStringConverter(StateControl.TimeSeriesFilterSorts timeSeriesSortFilter)
        {
            switch (timeSeriesSortFilter)
            {
                case StateControl.TimeSeriesFilterSorts.Ascending:
                    return "asc";
                case StateControl.TimeSeriesFilterSorts.Descending:
                    return "desc";
            }
            throw new ArgumentException("Unknown sort type specified.");
        }

        private string ProduceQtableFormula()
        {
            var formulaComponents = new List<string>();
            formulaComponents.Add($"\"{DataCode}\"");

            if (Columns.Count == 1)
            {
                formulaComponents.Add($"\"{Columns[0].ColumnName}\"");
            }
            else if (Columns.Count > 1)
            {
                formulaComponents.Add(
                    $"{{{string.Join(",", Columns.Select(n => $"\"{n.ColumnName}\"".ToString()).ToArray())}}}");
            }

            // Close off the formula
            return $"=QTABLE({string.Join(",", formulaComponents.Select(n => n.ToString()).ToArray())})";
        }
    }
}