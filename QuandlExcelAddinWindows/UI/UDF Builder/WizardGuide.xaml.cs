using System;
using System.Collections.Generic;
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
        private string[] steps { get
            {
               return StateControl.Instance.getStepList();
            }
        }
        private int currentStep = 0;
        private int shownStep = 0;

        public WizardGuide()
        {
            this.Initialized += delegate { this.changeToStep(); };
            InitializeComponent();

            // Initialize the new control
            StateControl.Instance.reset();
            this.DataContext = StateControl.Instance;
        }

        private void nextButton_click(object sender, RoutedEventArgs e)
        {
            // Bail out if the user has reached a future step but is showing an older step
            if (currentStep > shownStep)
            {
                shownStep++;
                this.showStep(this.shownStep);
            }
            else
            {
                currentStep++;
                this.changeToStep();
            }
        }

        private void prevButton_click(object sender, RoutedEventArgs e)
        {
            shownStep--;
            this.showStep(this.shownStep);
        }

        private void changeToStep()
        {
            // Show the current step form in the wizard
            this.showStep(this.currentStep);
        }

        private void showForm()
        {
            Uri stepXaml = new Uri(steps[this.shownStep] + ".xaml", UriKind.Relative);
            this.stepFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            this.stepFrame.Source = stepXaml;
        }

        private void showStep(int stepNumber)
        {
            // Update the shown step
            this.shownStep = stepNumber;

            // Show the correct user form in the wizard
            this.showForm();

            // Enable the appropriate navigation buttons
            this.nextButton.IsEnabled = true;
            this.prevButton.IsEnabled = true;
            if (stepNumber == 0)
            {
                this.prevButton.IsEnabled = false;
            }
            else if (stepNumber == steps.Length - 1)
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
                stepLink.Content = "Step " + (i + 1).ToString();
                //stepLink.IsEnabled = (i != this.currentStep);
                stepLink.Padding = new Thickness(10);
                int step = i; // Need to duplicate the value to avoid issues with referencing a changing 'i'
                stepLink.Click += delegate
                {
                    this.showStep(step);
                };
                this.stepBreadcrumb.Children.Add(stepLink);

                // Separator between step buttons
                if (i != this.currentStep)
                {
                    Label sep = new Label();
                    sep.Content = "\\";
                    //sep.IsEnabled = false;
                    sep.Padding = new Thickness(0, 10, 0, 10);
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

            // Highlight the current step
            Control stepElement = (Control)this.stepBreadcrumb.Children[((stepNumber+1)*2)-2];
            stepElement.Background = System.Windows.Media.Brushes.AliceBlue;
        }
    }
}