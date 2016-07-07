using System.Collections.Generic;
using System.Collections.ObjectModel;
using Quandl.Shared.models;

namespace Quandl.Excel.Addin.ViewData
{
    public class Category
    {
        public string Header { get; set; }
        public ObservableCollection<SubCategory> SubCategories { get; set; }

        public Category()
        {
            SubCategories = new ObservableCollection<SubCategory>();
        }
    }

    public class SubCategory
    {
        public string Header { get; set; }
        public ObservableCollection<Detail> Details { get; set; }

        public SubCategory()
        {
            Details = new ObservableCollection<Detail>();
        }
    }

    public class Detail
    {
        public string Header { get; set; }

        public List<OrderedResourceIds> OrderList { get; set; }

        public List<Data> DataList;

        public Detail(string header, List<OrderedResourceIds> orderList)
        {
            Header = header;
            OrderList = orderList;
            DataList = new List<Data>();
        }
    }


    public class Data
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool Premium { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public string DataType { get; set; }

        public Data(int id, string code, bool premium, string type)
        {
            Id = id;
            Code = code;
            Premium = premium;
            Type = type;
            if (premium)
            {
                DataType = "Premium";
            }
            else
            {
                DataType = "Free";
            }

        }
    }
}