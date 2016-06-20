using System;
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
    /// Interaction logic for WizardGuide.xaml
    /// </summary>
    public partial class WizardGuide : UserControl
    {
        private string[] steps = new string[] {
            "DatabaseSelection",
            "DatasetDatatableSelection"
        };
        private int currentStep = 0;

        public WizardGuide()
        {
            this.Initialized += delegate { this.changeToStep(); };
            InitializeComponent();
        }

        private void nextButton_click(object sender, RoutedEventArgs e)
        {
            currentStep++;
            this.changeToStep();
        }

        private void prevButton_click(object sender, RoutedEventArgs e)
        {
            currentStep--;
            this.changeToStep();
        }

        private void changeToStep()
        {
            // Show the correct user form in the wizard
            Uri stepXaml = new Uri(steps[this.currentStep] + ".xaml", UriKind.Relative);
            this.stepFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            this.stepFrame.Source = stepXaml;

            // Enable the appropriate navigation buttons
            this.nextButton.IsEnabled = true;
            this.prevButton.IsEnabled = true;
            if (currentStep == 0)
            {
                this.prevButton.IsEnabled = false;
            }
            else if (currentStep == steps.Length - 1)
            {
                this.nextButton.IsEnabled = false;
            }

            // Build up the breadcrumb bar
            this.stepBreadcrumb.Children.Clear();
            for (int i = 0; i <= this.currentStep; i++)
            {
                var type = Type.GetType("Quandl.Excel.Addin.UI.UDF_Builder." + steps[i]);
                var myObject = (UserControl)Activator.CreateInstance(type);
                string name = myObject.GetType().Name;

                Button stepLink = new Button();
                stepLink.Content = name;
                stepLink.FontSize = 15;
                stepLink.IsEnabled = (i != this.currentStep);
                int step = i;
                stepLink.Click += delegate
                {
                    this.currentStep = step;
                    this.changeToStep();
                };
                this.stepBreadcrumb.Children.Add(stepLink);

                if (i != this.currentStep)
                {
                    TextBox sep = new TextBox();
                    sep.Text = ">";
                    sep.FontSize = 15;
                    sep.IsEnabled = false;
                    this.stepBreadcrumb.Children.Add(sep);
                }
            }
        }
    }
}