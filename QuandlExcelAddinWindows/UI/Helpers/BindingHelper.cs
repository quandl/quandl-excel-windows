using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace Quandl.Excel.Addin.UI.Helpers
{
    internal class BindingHelper
    {
        public static void SetItemSourceViaEnum(ComboBox comboBox, Type type)
        {
            comboBox.ItemsSource = Enum.GetValues(type)
                .Cast<Enum>()
                .Select(value => new
                {
                    (Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()),
                        typeof(DescriptionAttribute)) as DescriptionAttribute)?.Description,
                    value
                })
                .OrderBy(item => item.value)
                .ToList();
        }
    }
}