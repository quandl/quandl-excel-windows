using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

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
                // Set the title of the form
                var type = Type.GetType("Quandl.Excel.Addin.UI.UDF_Builder." + steps[i]);
                WizardUIBase stepObject = (WizardUIBase)Activator.CreateInstance(type);
                Toolbar.frm.Text = stepObject.getTitle();

                // Step button
                Button stepLink = new Button();
                stepLink.Content = "Step " + (i+1).ToString();
                stepLink.IsEnabled = (i != this.currentStep);
                stepLink.Padding = new Thickness(10);
                int step = i; // Need to duplicate the value to avoid issues with referencing a changing 'i'
                stepLink.Click += delegate
                {
                    this.currentStep = step;
                    this.changeToStep();
                };
                this.stepBreadcrumb.Children.Add(stepLink);

                // Seperator between step buttons
                if (i != this.currentStep)
                {
                    TextBox sep = new TextBox();
                    sep.Text = ">";
                    sep.IsEnabled = false;
                    sep.Padding = new Thickness(0,10,0,10);
                    this.stepBreadcrumb.Children.Add(sep);
                }
            }

            // Set some common styling elements
            foreach (Control child in this.stepBreadcrumb.Children)
            {
                child.HorizontalAlignment = HorizontalAlignment.Left;
                child.FontSize = 15;
                child.Margin = new Thickness(0);
                child.BorderThickness = new Thickness(0);
                child.Background = System.Windows.Media.Brushes.Transparent;
            }
        }
    }
}