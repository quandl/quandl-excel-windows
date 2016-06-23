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
        }

        private void loginButton_click(object sender, RoutedEventArgs e)
        {
            try
            {
                // save this to config
                errorLabel.Visibility = Visibility.Hidden;
                if (!string.IsNullOrWhiteSpace(apiKey.Text))
                {
                    QuandlConfig.ApiKey = apiKey.Text;
                }
                else if (!string.IsNullOrWhiteSpace(email.Text) && !string.IsNullOrWhiteSpace(password.Text)) 
                {
                    QuandlConfig.ApiKey = Shared.Utilities.AuthToken(email.Text, password.Text);
                }
                else
                {
                    errorLabel.Content = @"Please input your login credentials.";
                    errorLabel.Visibility = Visibility.Visible;
                }
            }
            catch (WebException exp)
            {
                var response = exp.Response as HttpWebResponse;
                if (response != null && response.StatusCode == (HttpStatusCode)422)
                {
                    errorLabel.Content = @"Incorrect credentials inputted.";
                }
                else
                {
                    errorLabel.Content = @"Something went wrong. Please try again later.";
                }

                errorLabel.Visibility = Visibility.Visible;
            }
        }

        private void registerButton_click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.quandl.com/?modal=register");
        }
    }
}
