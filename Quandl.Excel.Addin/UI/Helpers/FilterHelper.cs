using System.Collections;
using Quandl.Excel.Addin.UI.UDF_Builder.Filters;
using Quandl.Excel.Addin.UI.UDF_Builder;

namespace Quandl.Excel.Addin.UI.Helpers
{
    public class FilterHelper
    {
        public FilterHelper()
        {
            Id = FilterHelper._count.ToString();
            FilterHelper._count++;
        }

        private static int _count = 0;
        public string Id { get; set; }
        public string Name { get; set; }

        public void PropertyChanged(Filter filter )
        {
            if (filter == null) return;
            Hashtable datatableFilters = StateControl.Instance.DatatableFilters;
            string filterValue = filter.Value.Trim();
            if (datatableFilters.ContainsKey(Id))
            {
                if (filterValue.Replace("\"","").Equals("") || filterValue.Equals("{}"))
                {
                   datatableFilters.Remove(Id);
                }
                else
                {
                    datatableFilters[Id] = filter;
                }
                
            }
            else if(filterValue.Replace("\"", "") != "")
            {
                datatableFilters.Add(Id, filter);
            }
            StateControl.Instance.IsFilterChanged = !StateControl.Instance.IsFilterChanged;
        }

        public static void Reset()
        {
            _count = 0;
            StateControl.Instance.DatatableFilters.Clear();
        }
    }
}
