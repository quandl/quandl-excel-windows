using System.Windows;

namespace Quandl.Excel.Addin.UI.Helpers
{
    public class DataCodeColumn : DependencyObject
    {
        public DataCodeColumn(DataCodeCollection dcc, string dataName, string columnName)
        {
            SetValue(CheckedItemHelper.ParentProperty, dcc);
            Name = $"{dataName} - {columnName}";
            QuandlCode = dcc.QuandlCode;
            ColumnName = columnName;
        }

        public string Name { get; }
        public string QuandlCode { get; }
        public string ColumnName { get; }
    }
}