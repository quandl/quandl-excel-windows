using System.Collections.Generic;

namespace Quandl.Shared.models
{
    public class DatasetData
    {
        public int? Limit { get; set; }
        public string Transform { get; set; }
        public int? ColumnIndex { get; set; }
        public List<string> ColumnNames { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Frequency { get; set; }
        public List<List<object>> Data { get; set; }
        public string Collapse { get; set; }
        public string Order { get; set; }
    }

    public class DatasetDataResponse
    {
        public DatasetData DatasetData { get; set; }
    }
}