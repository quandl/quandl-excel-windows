using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    class StateControl
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

        public int currentStep = 0;

        public string DataCode { get; internal set; }
        public List<string> DataSetTableSelection { get; internal set; } = new List<string>();
        public List<List<string>> Columns { get; internal set; } = new List<List<string>>();
        public List<DataSetTableFilter> Filters { get; internal set; } = new List<DataSetTableFilter>();

        private ChainTypes chainType = ChainTypes.Datatables;

        public StateControl()
        {
            this.reset();
        }

        public void reset()
        {
            currentStep = 0;
            chainType = ChainTypes.Datatables;
            DataCode = null;
            DataSetTableSelection = new List<string>();
            Columns = new List<List<string>>();
            Filters = new List<DataSetTableFilter>();
        }

        public void changeCode(string dataCode, bool timeSeries)
        {

        }

        public string[] getStepList()
        {
            if (this.chainType == ChainTypes.TimeSeries)
            {
                return timeSeriesChain;
            }
            else
            {
                return datatableChain;
            }
        }
    }
}
