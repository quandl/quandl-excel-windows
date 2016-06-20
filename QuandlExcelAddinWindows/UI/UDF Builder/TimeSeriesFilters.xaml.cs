﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    /// Interaction logic for TimeSeriesFilters.xaml
    /// </summary>
    public partial class TimeSeriesFilters : UserControl, WizardUIBase
    {
        public string getTitle()
        {
            return "Customize Time Series Data";
        }

        public TimeSeriesFilters()
        {
            InitializeComponent();
        }
    }
}
