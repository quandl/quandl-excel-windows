using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Quandl.Shared;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    ///     Interaction logic for WizardGuide.xaml
    /// </summary>
    public partial class WizardGuide : UserControl
    {
        private int shownStep;

        public WizardGuide()
        {
            InitializeComponent();

            StateControl.Instance.Reset();

            // Async check that the user is logged in our not
            Loaded += async delegate
            {
                PrepareFormEvents();
                try
                {
                    var validKey = await QuandlConfig.ApiKeyValid();
                    LoginOrSearch();
                }
                catch (Exception exp)
                {
                    Globals.ThisAddIn.UpdateStatusBar(exp);
                }
            };
        }

        private void PrepareFormEvents()
        {
            QuandlConfig.Instance.LoginChanged += async delegate
            {
                var validKey = await QuandlConfig.ApiKeyValid();
                LoginOrSearch();
            };

            StateControl.Instance.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "DataCode")
                {
                    changeToStep();
                }
            };
        }

        private string[] steps => StateControl.Instance.GetStepList();

        private int currentStep => StateControl.Instance.currentStep;

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

        private async void LoginOrSearch()
        {
            try
            {
                var loggedIn = await QuandlConfig.ApiKeyValid();
                if (loggedIn)
                {
                    foreach (UIElement child in currentStepGrid.Children)
                    {
                        child.Visibility = Visibility.Visible;
                    }

                    DataContext = StateControl.Instance;
                    changeToStep();
                }
                else
                {
                    var loginXaml = new Uri("../Settings/Login.xaml", UriKind.Relative);
                    stepFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
                    stepFrame.Source = loginXaml;
                    currentStepGrid.Children[0].Visibility = Visibility.Hidden;
                    currentStepGrid.Children[2].Visibility = Visibility.Hidden;
                }
                this.Focus();
            }
            catch (Exception exp)
            {
                Globals.ThisAddIn.UpdateStatusBar(exp);
            }
        }

        private void changeToStep()
        {
            // Show the current step form in the wizard
            showStep(currentStep);
        }

        private void showForm()
        {
            var stepXaml = new Uri(steps[shownStep] + ".xaml", UriKind.Relative);
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
            var title = "";
            stepBreadcrumb.Children.Clear();
            for (var i = 0; i <= currentStep; i++)
            {
                // Set the title of the form
                var type = Type.GetType("Quandl.Excel.Addin.UI.UDF_Builder." + steps[i]);
                var stepObject = (WizardUIBase) Activator.CreateInstance(type);

                // Should this be the title shown
                if (i == stepNumber)
                {
                    title = stepObject.getTitle();
                }

                // Step button
                var stepLink = new Button();
                stepLink.Content = "Step " + (i + 1);
                stepLink.Padding = new Thickness(10);
                var step = i; // Need to duplicate the value to avoid issues with referencing a changing 'i'
                stepLink.Click += delegate { showStep(step); };
                stepBreadcrumb.Children.Add(stepLink);

                // Separator between step buttons
                if (i != currentStep)
                {
                    var sep = new Label();
                    sep.Content = "\\";
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
                child.Background = Brushes.Transparent;
            }

            // Add in the title
            var titleBox = new TextBox();
            titleBox.Text = title;
            titleBox.BorderThickness = new Thickness(0);
            titleBox.Background = Brushes.Transparent;
            titleBox.HorizontalContentAlignment = HorizontalAlignment.Right;
            titleBox.VerticalContentAlignment = VerticalAlignment.Center;

            stepBreadcrumb.Children.Add(titleBox);

            // Highlight the current step
            var stepElement = (Control) stepBreadcrumb.Children[(stepNumber + 1)*2 - 2];
            stepElement.Background = Brushes.AliceBlue;
        }
    }
}