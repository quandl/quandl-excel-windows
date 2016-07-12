using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using Quandl.Shared;
using static Quandl.Excel.Addin.UI.UDF_Builder.StateControl;

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

            // Wait for the UI thread to become idle before rendering. Not this can have disastrous performance implications if used incorrectly.
            Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);

            Instance.Reset();

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

        private string[] steps => Instance.GetStepList();

        private int currentStep => Instance.CurrentStep;

        private void PrepareFormEvents()
        {
            QuandlConfig.Instance.LoginChanged += LoginOrSearch;
            Instance.PropertyChanged += delegate { AllowMovementToNextStep(); };
        }

        private void AllowMovementToNextStep()
        {
            nextButton.IsEnabled = currentStep > shownStep || Instance.CanMoveForward();
        }

        private void nextButton_click(object sender, RoutedEventArgs e)
        {
            // Bail out if the user has reached a future step but is showing an older step
            if (currentStep > shownStep)
            {
                shownStep++;
                ShowStep(shownStep);
            }
            else
            {
                Instance.NextStep();
                ChangeToCurrentStep();
            }
        }

        private void prevButton_click(object sender, RoutedEventArgs e)
        {
            shownStep--;
            ShowStep(shownStep);
        }

        private async void LoginOrSearch()
        {
            try
            {
                var loggedIn = await QuandlConfig.ApiKeyValid();
                Dispatcher.Invoke(() =>
                {
                    if (loggedIn)
                    {
                        foreach (UIElement child in currentStepGrid.Children)
                        {
                            child.Visibility = Visibility.Visible;
                        }

                        ChangeToCurrentStep();
                    }
                    else
                    {
                        var loginXaml = new Uri("../Settings/Login.xaml", UriKind.Relative);
                        stepFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
                        stepFrame.Source = loginXaml;
                        currentStepGrid.Children[0].Visibility = Visibility.Hidden;
                        currentStepGrid.Children[2].Visibility = Visibility.Hidden;
                    }
                    Focus();
                });
            }
            catch (Exception exp)
            {
                Globals.ThisAddIn.UpdateStatusBar(exp);
            }
        }

        private void ChangeToCurrentStep()
        {
            // Show the current step form in the wizard
            ShowStep(currentStep);
        }

        private void ShowForm()
        {
            var stepXaml = new Uri(steps[shownStep] + ".xaml", UriKind.Relative);
            stepFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            stepFrame.Source = stepXaml;
        }

        private void ShowStep(int stepNumber)
        {
            // Update the shown step
            shownStep = stepNumber;

            // Show the correct user form in the wizard
            ShowForm();

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
                    title = stepObject.GetTitle();
                }

                // Step button
                var stepLink = new Button();
                stepLink.Content = stepObject.GetShortTitle();
                stepLink.Padding = new Thickness(10);
                var step = i; // Need to duplicate the value to avoid issues with referencing a changing 'i'
                stepLink.Click += delegate { ShowStep(step); };
                stepBreadcrumb.Children.Add(stepLink);

                // Separator between step buttons
                if (i != currentStep)
                {
                    var sep = new Label();
                    sep.Content = "-";
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

            AllowMovementToNextStep();
        }
    }
}