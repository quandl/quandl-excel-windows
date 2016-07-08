using System.Collections.Generic;
using System.ComponentModel;
using Quandl.Shared.models;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    internal class StateControl : INotifyPropertyChanged
    {
        // The different types of chains
        public enum ChainTypes
        {
            TimeSeries,
            Datatables
        }

        // Singleton state to be shared between different forms
        private static StateControl instance;

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

        public int currentStep;

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
        }

        public static StateControl Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StateControl();
                }
                return instance;
            }
        }

        public string UdfFormula { get; internal set; }

        public string DataCode { get; internal set; }
        public List<string> DataSetTableSelection { get; internal set; } = new List<string>();
        public List<List<string>> Columns { get; internal set; } = new List<List<string>>();
        public List<DataSetTableFilter> Filters { get; internal set; } = new List<DataSetTableFilter>();

        public ChainTypes chainType { get; internal set; } = ChainTypes.Datatables;

        public DatatableCollectionResponse datatableCollection { get; internal set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Reset()
        {
            UdfFormula = "";
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
            NotifyPropertyChanged("DataCode");
        }

        public string[] GetStepList()
        {
            return chainType == ChainTypes.TimeSeries ? timeSeriesChain : datatableChain;
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateFormula()
        {
            // If the DataCode has been nullified or blanked out simply erase the formula
            if (string.IsNullOrEmpty(DataCode))
            {
                UdfFormula = "";
                NotifyPropertyChanged("UdfFormula");
                return;
            }

            // At least the DataCode has been given
            UdfFormula = $"=QDATA({DataCode}";

            // Close off the formula
            UdfFormula += ")";

            // Updated Property
            NotifyPropertyChanged("UdfFormula");
        }
    }
}