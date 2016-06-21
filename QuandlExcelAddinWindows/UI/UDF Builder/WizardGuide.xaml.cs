using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private int currentStep
        {
            get {
                return StateControl.Instance.currentStep;
            }
        }
        private int shownStep = 0;

        public WizardGuide()
        {
            Initialized += delegate { changeToStep(); };
            InitializeComponent();

            // Initialize the new control
            StateControl.Instance.reset();
            DataContext = StateControl.Instance;
            StateControl.Instance.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "DataCode")
                {
                    changeToStep();
                }
            };
        }

        private void nextButton_click(object sender, RoutedEventArgs e)
        {
            // Bail out if the user has reached a future step but is showing an older step
            if (currentStep > shownStep)
            {
                shownStep++;
                showStep(shownStep);
            }
            else
            {
                StateControl.Instance.currentStep++;
                changeToStep();
            }
        }

        private void prevButton_click(object sender, RoutedEventArgs e)
        {
            shownStep--;
            showStep(shownStep);
        }

        private void changeToStep()
        {
            // Show the current step form in the wizard
            showStep(currentStep);
        }

        private void showForm()
        {
            Uri stepXaml = new Uri(steps[shownStep] + ".xaml", UriKind.Relative);
            stepFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            stepFrame.Source = stepXaml;
        }

        private void showStep(int stepNumber)
        {
            // Update the shown step
            shownStep = stepNumber;

            // Show the correct user form in the wizard
            showForm();

            // Enable the appropriate navigation buttons
            nextButton.IsEnabled = true;
            prevButton.IsEnabled = true;
            if (stepNumber == 0)
            {
                prevButton.IsEnabled = false;
            }
            else if (stepNumber == steps.Length - 1)
            {
                nextButton.IsEnabled = false;
            }

            // Build up the breadcrumb bar
            stepBreadcrumb.Children.Clear();
            for (int i = 0; i <= currentStep; i++)
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
                    showStep(step);
                };
                stepBreadcrumb.Children.Add(stepLink);

                // Separator between step buttons
                if (i != currentStep)
                {
                    Label sep = new Label();
                    sep.Content = "\\";
                    //sep.IsEnabled = false;
                    sep.Padding = new Thickness(0, 10, 0, 10);
                    stepBreadcrumb.Children.Add(sep);
                }
            }

            // Set some common styling elements
            foreach (Control child in stepBreadcrumb.Children)
            {
                child.HorizontalAlignment = HorizontalAlignment.Left;
                child.FontSize = 15;
                child.Margin = new Thickness(0);
                child.BorderThickness = new Thickness(0);
                child.Background = System.Windows.Media.Brushes.Transparent;
            }

            // Highlight the current step
            Control stepElement = (Control)stepBreadcrumb.Children[((stepNumber+1)*2)-2];
            stepElement.Background = System.Windows.Media.Brushes.AliceBlue;
        }
    }
}