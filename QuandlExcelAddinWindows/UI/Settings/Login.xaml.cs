using Quandl.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Quandl.Shared.errors;

namespace Quandl.Excel.Addin.UI.Settings
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
            errorLabel.Visibility = Visibility.Hidden;
            apiKey.Text = QuandlConfig.ApiKey;

            if (!string.IsNullOrWhiteSpace(apiKey.Text))
            {
                displayErrorMessage(@"Invalid api key specified.");
            }
        }

        private async void loginButton_click(object sender, RoutedEventArgs e)
        {
            try
            {
                // save this to config
                errorLabel.Visibility = Visibility.Hidden;
                if (!string.IsNullOrWhiteSpace(apiKey.Text))
                {
                    if (await QuandlConfig.ApiKeyValid(apiKey.Text))
                    {
                        QuandlConfig.ApiKey = apiKey.Text;
                    }
                    else
                    {
                        displayErrorMessage(@"Invalid api key specified.");
                    }
                }
                else if (!string.IsNullOrWhiteSpace(email.Text) && !string.IsNullOrWhiteSpace(password.Text)) 
                {
                    QuandlConfig.AuthenticateWithCredentials(email.Text, password.Text);
                }
                else
                {
                    displayErrorMessage(@"Please input your login credentials.");
                }
            }
            catch (QuandlErrorBase exp)
            {
                if (exp.StatusCode == HttpStatusCode.BadRequest)
                {
                    displayErrorMessage(@"Incorrect credentials inputted.");
                }
                else
                {
                    displayErrorMessage(@"Something went wrong. Please try again later.");
                }
            }
        }

        private void displayErrorMessage(string message)
        {
            errorLabel.Content = message;
            errorLabel.Visibility = Visibility.Visible;
        }

        private void registerButton_click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.quandl.com/?modal=register");
        }
    }
}
