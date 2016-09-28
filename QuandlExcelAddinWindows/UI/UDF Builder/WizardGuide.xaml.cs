using Quandl.Shared;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using static Quandl.Excel.Addin.UI.UDF_Builder.StateControl;

namespace Quandl.Excel.Addin.UI.UDF_Builder
{
    /// <summary>
    ///     Interaction logic for WizardGuide.xaml
    /// </summary>
    public partial class WizardGuide : UserControl
    {
        private int _shownStep;

        public WizardGuide()
        {
            InitializeComponent();

            // Wait for the UI thread to become idle before rendering. Not this can have disastrous performance implications if used incorrectly.
            Dispatcher.Invoke(new System.Action(() => { }), DispatcherPriority.ContextIdle, null);

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
                    Utilities.LogToSentry(exp);
                }
            };
        }

        private string[] steps => Instance.GetStepList();

        private int currentStep => Instance.CurrentStep;

        public void Reset()
        {
            ShowLoadingState();
            Instance.Reset();
            LoginOrSearch();
        }

        private void PrepareFormEvents()
        {
            QuandlConfig.Instance.LoginChanged += LoginOrSearch;
            Instance.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName != "UdfFormula")
                {
                    AllowMovementToNextStep(currentStep);
                }
                if (e.PropertyName == "CurrentStep" && _shownStep == 0)
                {
                    ShowStep(_shownStep);
                }
            };
        }

        private void AllowMovementToNextStep(int stepNumber)
        {
            Dispatcher.Invoke(() =>
            {
                // Reset everything to defaults
                nextButton.IsEnabled = true;
                prevButton.IsEnabled = true;
                nextButton.Visibility = Visibility.Visible;
                insertButton.Visibility = Visibility.Collapsed;
                nextButton.IsDefault = true;

                // Determine which buttons to show/hide and make visible
                if (stepNumber == 0)
                {
                    prevButton.IsEnabled = false;
                }

                if (stepNumber == steps.Length - 1)
                {
                    nextButton.Visibility = Visibility.Collapsed;
                    insertButton.Visibility = Visibility.Visible;
                    insertButton.IsDefault = true;
                }
                else
                {
                    nextButton.IsEnabled = currentStep > _shownStep || Instance.CanMoveForward();
                }
            });
        }

        private void nextButton_click(object sender, RoutedEventArgs e)
        {
            // Bail out if the user has reached a future step but is showing an older step
            if (currentStep > _shownStep)
            {
                _shownStep++;
                ShowStep(_shownStep);
            }
            else
            {
                Instance.NextStep();
                ChangeToCurrentStep();
            }
        }

        private void prevButton_click(object sender, RoutedEventArgs e)
        {
            _shownStep--;
            ShowStep(_shownStep);
            nextButton.Focus();
        }

        private void ShowLoadingState()
        {
            var loginXaml = new Uri("Loading.xaml", UriKind.Relative);
            stepFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            stepFrame.Source = loginXaml;
            currentStepGrid.Children[0].Visibility = Visibility.Collapsed;
            currentStepGrid.Children[2].Visibility = Visibility.Collapsed;
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
                        currentStepGrid.Children[0].Visibility = Visibility.Collapsed;
                        currentStepGrid.Children[2].Visibility = Visibility.Collapsed;
                    }
                    Focus();
                });
            }
            catch (Exception exp)
            {
                Globals.ThisAddIn.UpdateStatusBar(exp);
                Utilities.LogToSentry(exp);
            }
        }

        private void ChangeToCurrentStep()
        {
            // Show the current step form in the wizard
            ShowStep(currentStep);
        }

        private void ShowForm()
        {
            var stepXaml = new Uri(steps[_shownStep] + ".xaml", UriKind.Relative);
            stepFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            stepFrame.Source = stepXaml;
        }

        private void ShowStep(int stepNumber)
        {
            // Update the shown step
            _shownStep = stepNumber;

            // Show the correct user form in the wizard
            ShowForm();

            // Build up the breadcrumb bar
            var title = "";
            stepBreadcrumb.Children.Clear();
            for (var i = 0; i <= currentStep; i++)
            {
                // Set the title of the form
                var type = Type.GetType("Quandl.Excel.Addin.UI.UDF_Builder." + steps[i]);
                var stepObject = (WizardUIBase)Activator.CreateInstance(type);

                // Should this be the title shown
                if (i == stepNumber)
                {
                    title = stepObject.GetTitle();
                }

                // Step button
                var stepLink = new Button();
                stepLink.Content = stepObject.GetShortTitle();
                stepLink.Padding = new Thickness(10);
                stepLink.Margin = new Thickness(0, 0, 2, 0);

                var step = i; // Need to duplicate the value to avoid issues with referencing a changing 'i'
                stepLink.Click += delegate { ShowStep(step); };
                stepBreadcrumb.Children.Add(stepLink);
                stepBreadcrumb.Margin = new Thickness(10);
            }

            // Set some common styling elements
            foreach (Control child in stepBreadcrumb.Children)
            {
                child.HorizontalAlignment = HorizontalAlignment.Left;
                child.FontSize = 15;
                child.BorderThickness = new Thickness(0);
                child.Foreground = Brushes.Black;
                child.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xB1, 0XDB, 0XFF));
            }

            // Add in the title
            var titleBox = new Label();
            titleBox.Content = title;
            titleBox.BorderThickness = new Thickness(0);
            titleBox.Background = Brushes.Transparent;
            titleBox.HorizontalContentAlignment = HorizontalAlignment.Right;
            titleBox.VerticalContentAlignment = VerticalAlignment.Center;

            stepBreadcrumb.Children.Add(titleBox);

            // Highlight the current step
            var stepElement = (Control)stepBreadcrumb.Children[(stepNumber)];
            stepElement.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x0B, 0X8F, 0XFF));
            stepElement.Foreground = Brushes.White;

            AllowMovementToNextStep(stepNumber);
        }

        private void InsertButton_OnClickButton_click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Globals.ThisAddIn.ActiveCells != null)
                {
                    Globals.ThisAddIn.ActiveCells.Cells[1, 1].Value2 = Instance.UdfFormula;
                    Globals.Ribbons.Ribbon2.CloseBuilder(); // Wish there was a cleaner way to close this off.
                }
            }
            catch (COMException ex)
            {
                // Ignore no cells being selected error.
                if (ex.HResult == -2146827864)
                {
                    Trace.WriteLine(ex.Message);
                    return;
                }
                throw;
            }
        }
    }
}