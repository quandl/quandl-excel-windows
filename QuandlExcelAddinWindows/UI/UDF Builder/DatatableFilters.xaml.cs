using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Quandl.Excel.Addin.UI.Helpers;
using Quandl.Excel.Addin.UI.UDF_Builder.Filters;
using Quandl.Shared;
using Quandl.Shared.Models;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    ///     Interaction logic for DatatableFilters.xaml
    /// </summary>
    public partial class DatatableFilters : UserControl, WizardUIBase
    {
        private ObservableCollection<DataHolderDefinition> AvailableDataHolders
             => StateControl.Instance.AvailableDataHolders;
        public DatatableFilters()
        {
            InitializeComponent();
            DataContext = StateControl.Instance;
            SetDatatableFilters();
        }


        public string GetTitle()
        {
            return "Customize data table data";
        }

        public string GetShortTitle()
        {
            return "Filters";
        }

        private void SetDatatableFilters()
        {
            Dispatcher.Invoke(async () =>
            {
                if (StateControl.Instance.AvailableDataHolders != null)
                {
                    Datatable dt = StateControl.Instance.AvailableDataHolders[0] as Datatable;
                    DatatableMetadata items = await new Web().GetDatatableMetadata(dt.Code);
                    foreach (string filter in items.datatable.Filters)
                    {
                        var column = items.datatable.Columns.FirstOrDefault(x => x.Name.Equals(filter));
                        PopulateFilters(column.Name, column.Type.ToLower());
  
                    }
                }
            });
        }

        private void PopulateFilters(string name, string type)
        {
            if (type.StartsWith("bigdecimal"))
            {
                type = "bigdecimal";
            }
            switch (type)
            {
                case "string":
                    FiltersGroup.Children.Add(new StringFilter(name, new FilterHelper()));
                    break;
                case "date":
                    FiltersGroup.Children.Add(new DateFilter(name, new FilterHelper()));
                    break;
                case "integer":
                    FiltersGroup.Children.Add(new IntegerFilter(name, new FilterHelper()));
                    break;
                case "bigcecimal":
                    FiltersGroup.Children.Add(new FloatNumberFilter(name, new FilterHelper()));
                    break;
            }
            
        }

    }
}