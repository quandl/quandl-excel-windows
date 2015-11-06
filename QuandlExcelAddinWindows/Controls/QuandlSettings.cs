using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Quandl.Shared;

namespace Quandl.Excel.Addin.Controls
{
    public partial class QuandlSettings : UserControl
    {
        public delegate void AuthTokenChanged();
        public event AuthTokenChanged SettingsAuthTokenChanged;
        public QuandlSettings()
        {
            InitializeComponent();
            apiKeyTextBox.Text = QuandlConfig.ApiKey;
            autoUpdate.Checked = QuandlConfig.AutoUpdate;
        }

        private void autoUpdate_CheckedChanged(object sender, EventArgs e)
        {
        }

        protected virtual void OnSettingsAuthTokenChanged()
        {
            SettingsAuthTokenChanged?.Invoke();
        }

        private void saveSettings_Click(object sender, EventArgs e)
        {
            QuandlConfig.AutoUpdate = autoUpdate.Checked;
            QuandlConfig.ApiKey = apiKeyTextBox.Text;
            OnSettingsAuthTokenChanged();
        }

        public void UpdateApiKeyTextBox()
        {
            apiKeyTextBox.Text = QuandlConfig.ApiKey;
        }
    }
}
