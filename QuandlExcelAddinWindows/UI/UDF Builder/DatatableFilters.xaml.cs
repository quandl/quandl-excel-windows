using System;
using Quandl.Excel.Addin.UI.Helpers;
using Quandl.Excel.Addin.UI.UDF_Builder.Filters;
using Quandl.Shared;
using Quandl.Shared.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    ///     Interaction logic for DatatableFilters.xaml
    /// </summary>
    public partial class DatatableFilters : UserControl, WizardUIBase
    {
        private ObservableCollection<DataHolderDefinition> AvailableDataHolders
             => StateControl.Instance.AvailableDataHolders;

        private String PreviousDataHolderCode
        {
            get { return StateControl.Instance.PreviousDataHolderCode; }
            set { StateControl.Instance.PreviousDataHolderCode = value; }
        }

        private StackPanel filterGroup
        {
            get { return StateControl.Instance.FiltersGroup; }
            set { StateControl.Instance.FiltersGroup = value; }
        }

        private readonly int _preLoadElement = 2;

        public DatatableFilters()
        {
            InitializeComponent();
            if (ShouldSetFilters() )
            {
                SetDatatableFilters();
            }
            else
            {
                RestoreFiltersGroup(FiltersGroup, filterGroup);
            }
            filterGroup = FiltersGroup;
        }

        public string GetTitle()
        {
            return "Customize data table data";
        }

        public string GetShortTitle()
        {
            return "Filters";
        }

        private bool ShouldSetFilters()
        {
            if (AvailableDataHolders[0] == null)
            {
                return false;
            }
            else
            {
                Datatable dt = AvailableDataHolders[0] as Datatable;
                return dt.Code != PreviousDataHolderCode ||
                        filterGroup == null ||
                        filterGroup.Children.Count == _preLoadElement;
            }
        }

        private void RestoreFiltersGroup(StackPanel current, StackPanel previous)
        {
            if (current.Children.Count == _preLoadElement)
            {
                UIElement[] allChildren = new UIElement[previous.Children.Count];
                previous.Children.CopyTo(allChildren, 0);
                int i = 0;
                foreach (UIElement c in allChildren)
                {
                    if (i >= _preLoadElement)
                    {
                        previous.Children.Remove(c);
                        current.Children.Add(c);
                    }
                    i++;
                }
            }
        }

        private void SetDatatableFilters()
        {
            Dispatcher.Invoke(async () =>
            {
                FilterHelper.Reset();
                if (AvailableDataHolders[0] != null)
                {
                    Datatable dt = AvailableDataHolders[0] as Datatable;
                    DatatableMetadata items = await new Web().GetDatatableMetadata(dt.Code);
                    PreviousDataHolderCode = dt.Code;

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