using System.Collections;
using System.Collections.Generic;
using Quandl.Excel.Addin.UI.UDF_Builder.Filters;
using Quandl.Excel.Addin.UI.UDF_Builder;

namespace Quandl.Excel.Addin.UI.Helpers
{
    public class FilterHelper
    {
        public FilterHelper()
        {
            Id = System.Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public void PropertyChanged(Filter[] value )
        {
            Hashtable df = StateControl.Instance.DatatableFilters;
            if (df.ContainsKey(Id))
            {
                if (value != null && (value[0].Value.Trim().Replace("\"","").Equals("") || value[0].Value.Trim().Equals("{}")))
                {
                   df.Remove(Id);
                }
                else
                {
                    df[Id] = value;
                }
                
            }
            else
            {
                df.Add(Id, value);
            }
            StateControl.Instance.IsFilterChanged = StateControl.Instance.IsFilterChanged ? false : true;
        }
    }
}
