using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Quandl.Shared;
using Quandl.Shared.Errors;
using System.Windows.Media;

namespace Quandl.Excel.Addin.UI.Settings
{
    /// <summary>
    ///     Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        private VisualBrush _buttonHint;

        public Login()
        {
            InitializePasswordBoxHint();
            InitializeComponent();
            
            Loaded += delegate
            {
                errorLabel.Visibility = Visibility.Hidden;
                apiKey.Text = QuandlConfig.ApiKey;

                // If we are loading the login form we assume whatever api key was entered is invalid
                if (!string.IsNullOrWhiteSpace(apiKey.Text))
                {
                    DisplayErrorMessage("Invalid api key specified.");
                }
            };
        }

        private async void loginButton_click(object sender, RoutedEventArgs e)
        {
            loginForm.IsEnabled = false;
            errorLabel.Visibility = Visibility.Hidden;

            try
            {
                // save this to config
                if (!string.IsNullOrWhiteSpace(apiKey.Text))
                {
                    var key = apiKey.Text.Trim();
                    if (await QuandlConfig.ApiKeyValid(key))
                    {
                        QuandlConfig.ApiKey = key;
                    }
                    else
                    {
                        DisplayErrorMessage(Properties.Settings.Default.SettingsInValidApiKey);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(email.Text) && !string.IsNullOrWhiteSpace(password.Password))
                {
                    QuandlConfig.AuthenticateWithCredentials(new Web(), email.Text.Trim(), password.Password.Trim());
                }
                else
                {
                    DisplayErrorMessage(Properties.Settings.Default.SettingsIncorrectUsernameOrPassword);
                }
            }
            catch (QuandlErrorBase exp)
            {
                if (exp.StatusCode == HttpStatusCode.BadRequest)
                {
                    DisplayErrorMessage(Properties.Settings.Default.SettingsIncorrectCredentials);
                }
                else
                {
                    DisplayErrorMessage(Properties.Settings.Default.SettingsSomethingWrongTryLater);
                }
            }
            catch (Exception exp)
            {
                DisplayErrorMessage(Properties.Settings.Default.SettingsIncorrectUsernameOrPassword);
                Globals.ThisAddIn.UpdateStatusBar(exp);
                // For debug purposes only. This should not make it to production.

                Utilities.LogToSentry(exp);
            }
        }

        private void DisplayErrorMessage(string message)
        {
            // Necessary since the display message may be on a different thread
            Dispatcher.Invoke(() =>
            {
                loginForm.IsEnabled = true;
                errorLabel.Content = message;
                errorLabel.Visibility = Visibility.Visible;
                Globals.ThisAddIn.UpdateStatusBar(new Exception(message));
            });
        }

        private void registerButton_click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.quandl.com/?modal=register");
        }

        private void InitializePasswordBoxHint()
        {
             if (_buttonHint == null)
            {
                _buttonHint = new VisualBrush();
                _buttonHint.AlignmentX = AlignmentX.Left;
                _buttonHint.AlignmentY = AlignmentY.Center;
                _buttonHint.Stretch = Stretch.None;
                _buttonHint.TileMode = TileMode.None;

                var lb = new Label();
                lb.Content = "Your password";
                lb.Background = Brushes.White;
                lb.Foreground = Brushes.LightGray;
                lb.Padding = new Thickness(5, 5, 5, 5);
                lb.Width = 200;

                _buttonHint.Visual = lb;
            }
        }

        private void password_Initialized(object sender, EventArgs e)
        {
            password.Background = _buttonHint;
        }

        private void password_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            password.Background = (!password.IsKeyboardFocused && password.Password.Length == 0) 
                                ? (Brush) _buttonHint 
                                : Brushes.White;
        }
    }
}
