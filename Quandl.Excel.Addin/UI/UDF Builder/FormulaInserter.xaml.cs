using System.Windows.Controls;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Quandl.Shared.Excel;
using System;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    ///     Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class FormulaInserter : UserControl, WizardUIBase
    {
        private DisplayLogic display;
        public FormulaInserter()
        {
            InitializeComponent();
            var logic = MainLogic.Instance;
            Loaded +=
                delegate
                {
                    if (display == null)
                    {
                        var instance = MainLogic.Instance;
                        if (instance != null)
                        {
                            display = new DisplayLogic(instance, t => SelectedCellTextbox.Text = t);
                        }
                    }
                    if (StateControl.Instance.ChainType != StateControl.ChainTypes.TimeSeries)
                    {
                        IncludeHeaders.Visibility = System.Windows.Visibility.Collapsed;
                        Dates.Visibility = System.Windows.Visibility.Collapsed;
                        Transpose.Visibility = System.Windows.Visibility.Collapsed;
                    }
                };
            Unloaded += delegate
            {
                if (display != null)
                {
                    display.Dispose();
                    display = null;
                }
            };
        }

        sealed class DisplayLogic : IDisposable
        {
            private readonly MainLogic _logic;
            private readonly Action<string> _setText;
            public DisplayLogic(MainLogic logic, Action<string> setText)
            {
                _logic = logic;
                _setText = setText;
                _logic.SelectionChanged += _logic_SelectionChanged;
                UpdateText();
            }

            public void Dispose()
            {
                _logic.SelectionChanged -= _logic_SelectionChanged;
            }
            private void _logic_SelectionChanged(object sender, EventArgs e)
            {
                UpdateText();
            }

            public void UpdateText()
            {
                string setText = null;
                try
                {
                    setText = _logic.SelectedCellReference();
                }
                catch (System.Exception ex)
                {
                    setText = ex.Message;
                }

                _setText.Invoke(setText ?? "Please select one cell to insert your formula into.");
            }
        }
       

        public string GetTitle()
        {
            return "Choose where to place the data";
        }

        public string GetShortTitle()
        {
            return "Placement";
        }

    }
}