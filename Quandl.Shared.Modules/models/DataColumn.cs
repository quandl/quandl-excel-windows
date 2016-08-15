using System.Windows;
using Quandl.Excel.Addin.UI.Helpers;

namespace Quandl.Shared.Models
{
    public class DataColumn : DependencyObject
    {
        public string Name { get; set; }

        public string Code => Name.ToUpper();

        public DataHolderDefinition Parent { get; set; }

        public ProviderType ProviderType { get; set; }

        public string Type { get; set; }

        public string LongName => ParentProperty != null ? $"{ParentProperty.Name} - {Name}" : Name;

        private DataHolderDefinition ParentProperty => (DataHolderDefinition) GetValue(CheckedItemHelper.ParentProperty)
            ;
    }
}