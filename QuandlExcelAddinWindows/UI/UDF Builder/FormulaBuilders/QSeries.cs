using Quandl.Shared;
using Quandl.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quandl.Excel.Addin.UI.UDF_Builder.FormulaBuilders
{
    class QSeries : Base
    {
        private StateControl.TimeSeriesFilterSorts TimeSeriesSortFilter => _stateControl.TimeSeriesSortFilter;

        private StateControl.TimeSeriesFilterTransformations TimeSeriesTransformationFilter
            => _stateControl.TimeSeriesTransformationFilter;

        private int? TimeSeriesLimitFilter => _stateControl.TimeSeriesLimitFilter;

        private DateTime? EndDate => _stateControl.EndDate;

        private DateTime? StartDate => _stateControl.StartDate;

        private bool IncludeHeaders => _stateControl.IncludeHeaders;

        private StateControl.TimeSeriesFilterTypes DateTypeFilter => _stateControl.DateTypeFilter;

        public QSeries(StateControl stateControl) : base(stateControl)
        {
        }

        public override string ToUdf()
        {
            var formulaComponents = new List<string>();

            AddQuandlCodeAndColumns(formulaComponents);
            AddDateFilters(formulaComponents);
            AddCollapseFilters(formulaComponents);
            AddSortFilters(formulaComponents);
            AddTransformationFilters(formulaComponents);
            AddLimitFilters(formulaComponents);
            AddHeaderOptions(formulaComponents);

            // Close off the formula
            return $"=QSERIES({string.Join(",", formulaComponents.Select(n => n.ToString()).ToArray()).TrimEnd(',')})";
        }

        private void AddQuandlCodeAndColumns(List<string> formulaComponents)
        {
            // No columns selected. Only dealing with codes.
            if (Columns.Count == 0)
            {
                formulaComponents.Add(QuandlCodes.Count() > 1
                    ? $"{{{string.Join(",", QuandlCodes.Select(qc => $"\"{qc}\""))}}}"
                    : $"\"{QuandlCodes[0]}\"");
            }

            // Some columns have been selected.
            else if (Columns.Count >= 1)
            {
                var columns = string.Join(",", Columns.Select(c => $"\"{CodeFromColumn(c)}\"").ToArray());
                if (Columns.Count == 1)
                {
                    formulaComponents.Add(columns);
                }
                else
                {
                    formulaComponents.Add($"{{{columns}}}");
                }
            }
        }

        private string CodeFromColumn(DataColumn column)
        {
            return $"{_stateControl.CodeFromDataHolder(column.Parent)}/{column.Code}";
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
            if (DateTypeFilter == StateControl.TimeSeriesFilterTypes.Single && StartDate != null)
            {
                formulaComponents.Add(StringFromDate(StartDate));
            }
            else if (DateTypeFilter == StateControl.TimeSeriesFilterTypes.Range && StartDate != null && EndDate != null)
            {
                formulaComponents.Add(
                    $"{{{string.Join(",", StringFromDate(StartDate), StringFromDate(EndDate))}}}");
            }
            else if (DateTypeFilter == StateControl.TimeSeriesFilterTypes.Range && StartDate != null)
            {
                formulaComponents.Add(
                    $"{{{string.Join(",", StringFromDate(StartDate), "\"\"")}}}");
            }
            else if (DateTypeFilter == StateControl.TimeSeriesFilterTypes.Range && EndDate != null)
            {
                formulaComponents.Add(
                    $"{{{string.Join(",", "\"\"", StringFromDate(EndDate))}}}");
            }
            else if (_stateControl.TimeseriesFilterAfter("date"))
            {
                formulaComponents.Add("");
            }
        }

        private string StringFromDate(DateTime? date)
        {
            return $"\"{((DateTime)date).ToString(Utilities.DateFormat)}\"";
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

        private void AddLimitFilters(List<string> formulaComponents)
        {
            // Add limit
            if ((TimeSeriesLimitFilter != null || TimeSeriesLimitFilter > 0) && DateTypeFilter != StateControl.TimeSeriesFilterTypes.Single)
            {
                formulaComponents.Add($"{TimeSeriesLimitFilter}");
            }
            else if (_stateControl.TimeseriesFilterAfter("limit"))
            {
                formulaComponents.Add("");
            }
        }

        private void AddHeaderOptions(List<string> formulaComponents)
        {
            // Add limit
            if (!IncludeHeaders)
            {
                formulaComponents.Add(false.ToString());
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

    }
}
