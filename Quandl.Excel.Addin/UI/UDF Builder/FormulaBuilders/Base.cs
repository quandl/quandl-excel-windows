using System.Collections.Generic;
using System.Collections.ObjectModel;
using Quandl.Shared.Models;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    internal abstract class Base
    {
        protected readonly StateControl _stateControl;

        protected StateControl.TimeSeriesFilterCollapse TimeSeriesCollapseFilter => _stateControl.TimeSeriesCollapseFilter;
        protected ObservableCollection<DataColumn> Columns => _stateControl.Columns;
        protected IList<string> QuandlCodes => _stateControl.QuandlCodes;

        public Base(StateControl stateControl)
        {
            _stateControl = stateControl;
        }

        public abstract string ToUdf();
    }
}