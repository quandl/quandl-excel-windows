using Quandl.Excel.Addin.UI.UDF_Builder.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quandl.Excel.Addin.UI.UDF_Builder.FormulaBuilders
{
    class QTable : Base
    {
        private StateControl.ChainTypes ChainType => _stateControl.ChainType;

        private StateControl.TimeSeriesFilterTypes DateTypeFilter => _stateControl.DateTypeFilter;

        private Hashtable DatatableFilters => _stateControl.DatatableFilters;

        public QTable(StateControl stateControl) : base(stateControl)
        {
        }

        public override string ToUdf()
        {
            var formulaComponents = new List<string>();
            formulaComponents.Add($"\"{QuandlCodes[0]}\"");

            AddDatatableColumns(formulaComponents);
            AddDatatableFilters(formulaComponents);

            return $"=QTABLE({string.Join(",", formulaComponents.Select(n => n.ToString()).ToArray())})";
        }

        private void AddDatatableColumns(List<string> formulaComponents)
        {
            if (Columns.Count == 1)
            {
                formulaComponents.Add($"\"{Columns[0].Name}\"");
            }
            else if (Columns.Count > 1)
            {
                formulaComponents.Add(
                    $"{{{string.Join(",", Columns.Select(n => $"\"{n.Name}\"").ToArray())}}}");
            }
            else if (DatatableFilters.Count > 0)
            {
                formulaComponents.Add("");
            }
        }

        private void AddDatatableFilters(List<string> formulaComponents)
        {
            if (DatatableFilters != null && DatatableFilters.Count > 0)
            {
                foreach (DictionaryEntry item in DatatableFilters)
                {
                    Filter f = item.Value as Filter;
                    formulaComponents.Add($"\"{f.Name}\",{f.Value}");
                }
            }
        }
    }
}
