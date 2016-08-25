using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Quandl.Shared;
using Quandl.Shared.Errors;

namespace Quandl.Excel.Addin.UI.Settings
{
    /// <summary>
    ///     Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        public Login()
        {
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
                    if (await QuandlConfig.ApiKeyValid(apiKey.Text))
                    {
                        QuandlConfig.ApiKey = apiKey.Text;
                    }
                    else
                    {
                        DisplayErrorMessage("Invalid api key specified.");
                    }
                }
                else if (!string.IsNullOrWhiteSpace(email.Text) && !string.IsNullOrWhiteSpace(password.Text))
                {
                    QuandlConfig.AuthenticateWithCredentials(new Web(), email.Text, password.Text);
                }
                else
                {
                    DisplayErrorMessage("Please input your login credentials.");
                }
            }
            catch (QuandlErrorBase exp)
            {
                if (exp.StatusCode == HttpStatusCode.BadRequest)
                {
                    DisplayErrorMessage("Incorrect credentials inputted.");
                }
                else
                {
                    DisplayErrorMessage("Something went wrong. Please try again later.");
                }
            }
            catch (Exception exp)
            {
                DisplayErrorMessage("Something went wrong. Please try again later.");
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
    }
}