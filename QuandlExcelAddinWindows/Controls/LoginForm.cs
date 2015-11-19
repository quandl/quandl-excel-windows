using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Quandl.Shared;

namespace Quandl.Excel.Addin.Controls
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            loginButton.Click += LoginButton_Click;
            errorLabel.ForeColor = Color.Red;
            errorLabel.Hide();
        }

        public delegate void LoginChangedHandler();

        public event LoginChangedHandler LoginChanged;

        private void LoginButton_Click(object sender, EventArgs e)
        {
            var userName = accountName.Text;
            var pass = password.Text;
            try
            {
                // save this to config
                // dismiss login box?
                errorLabel.Hide();
                var apiKey = Shared.Utilities.AuthToken(userName, pass);
                QuandlConfig.ApiKey = apiKey;
                OnLoginChanged();
                Close();
            }
            catch (WebException exp)
            {
                var response = exp.Response as HttpWebResponse;
                if (response != null && response.StatusCode == (HttpStatusCode)422)
                {
                    errorLabel.Text = @"Incorrect credentials inputted.";
                }
                else
                {
                    errorLabel.Text = @"Something went wrong. Please try again later.";
                }
               
                errorLabel.Show();
            }
        }

        protected virtual void OnLoginChanged()
        {
            LoginChanged?.Invoke();
        }
    }
}
