using System.Collections.Generic;
using System.ComponentModel;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    class StateControl : INotifyPropertyChanged
    {
        // Singleton state to be shared between different forms
        private static StateControl instance;
        public static StateControl Instance {
            get
            {
                if (instance == null)
                {
                    instance = new StateControl();
                }
                return instance;
            }
        }

        // The different types of chains
        public enum ChainTypes { TimeSeries, Datatables };

        // The chain of forms for time series
        private static readonly string[] timeSeriesChain = {
            "DatabaseSelection",
            "DatasetDatatableSelection",
            "ColumnSelection",
            "TimeSeriesFilters",
            "FormulaInserter"
        };

        // The chain of forms for time series
        private static readonly string[] datatableChain = {
            "DatabaseSelection",
            "DatasetDatatableSelection",
            "ColumnSelection",
            "DatatableFilters",
            "FormulaInserter"
        };

        public event PropertyChangedEventHandler PropertyChanged;

        public int currentStep = 0;

        public string DataCode { get; internal set; }
        public List<string> DataSetTableSelection { get; internal set; } = new List<string>();
        public List<List<string>> Columns { get; internal set; } = new List<List<string>>();
        public List<DataSetTableFilter> Filters { get; internal set; } = new List<DataSetTableFilter>();

        public ChainTypes chainType { get; internal set; } = ChainTypes.Datatables;

        public StateControl()
        {
            Reset();
        }

        public void Reset()
        {
            currentStep = 0;
            chainType = ChainTypes.Datatables;
            DataCode = null;
            DataSetTableSelection = new List<string>();
            Columns = new List<List<string>>();
            Filters = new List<DataSetTableFilter>();
        }

        public void ChangeCode(string dataCode, ChainTypes ct)
        {
            Reset(); // Reset the chain because the code has been chained.
            chainType = ct;
            DataCode = dataCode;
            OnPropertyChanged("DataCode");
        }

        public string[] GetStepList()
        {
            return (this.chainType == ChainTypes.TimeSeries) ? timeSeriesChain : datatableChain;
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }
}
