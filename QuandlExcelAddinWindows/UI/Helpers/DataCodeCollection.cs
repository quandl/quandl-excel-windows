using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Quandl.Excel.Addin.UI.Helpers
{
    public class DataCodeCollection : DependencyObject
    {
        public DataCodeCollection(string quandlCode, string name)
        {
            QuandlCode = quandlCode;
            Name = name;
            Columns = new ObservableCollection<DataCodeColumn>();
        }

        public string QuandlCode { get; }
        public string Name { get; }
        public IList<DataCodeColumn> Columns { get; }
    }
}