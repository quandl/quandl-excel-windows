using System;
using System.Windows.Forms;
using Quandl.Shared;

namespace Quandl.Excel.Addin.Controls
{
    public partial class QuandlSettings : UserControl
    {
        private const int OneDay = 1;
        private const int SevenDays = 7;
        public delegate void AuthTokenChanged();

        public delegate void AutoUpdateSettingsChanged();
        public event AuthTokenChanged SettingsAuthTokenChanged;
        public event AutoUpdateSettingsChanged SettingsAutoUpdateChanged;
        public QuandlSettings()
        {
            InitializeComponent();
            apiKeyTextBox.Text = QuandlConfig.ApiKey;
            autoUpdate.Checked = QuandlConfig.AutoUpdate;
            updateFrequency.SelectedIndex = UpdateFrequencyToComboBoxIndex(QuandlConfig.AutoUpdateFrequency);
        }

        private void autoUpdate_CheckedChanged(object sender, EventArgs e)
        {
        }

        protected virtual void OnSettingsAuthTokenChanged()
        {
            SettingsAuthTokenChanged?.Invoke();
        }

        protected virtual void OnSettingsAutoUpdateChanged()
        {
            SettingsAutoUpdateChanged?.Invoke();
        }

        private void saveSettings_Click(object sender, EventArgs e)
        {
            bool autoUpdateChanged = (QuandlConfig.AutoUpdate != autoUpdate.Checked);
            var freqInDays = ComboBoxIndexToUpdateFrequency(updateFrequency.SelectedIndex);
            bool autoUpdateFreqChanged = (QuandlConfig.AutoUpdateFrequency != freqInDays);
           
            QuandlConfig.AutoUpdate = autoUpdate.Checked;
            QuandlConfig.ApiKey = apiKeyTextBox.Text;
            QuandlConfig.AutoUpdateFrequency = freqInDays;

            if (autoUpdateChanged || autoUpdateFreqChanged)
            {
                OnSettingsAutoUpdateChanged();
            }

            if (QuandlConfig.ApiKey != apiKeyTextBox.Text)
            {
                OnSettingsAuthTokenChanged();
            }
        }

        public void UpdateApiKeyTextBox()
        {
            apiKeyTextBox.Text = QuandlConfig.ApiKey;
        }

        private int ComboBoxIndexToUpdateFrequency(int index)
        {
            // combobox index 0: no background refresh freq (default)
            // combobox index 1: refresh every 24 hours
            // combobox index 2: refresh after every 7 days
            switch (index)
            {
                case 1:
                    return OneDay;
                case 2:
                    return SevenDays;
                default:
                    return 0;
            }
        }

        private int UpdateFrequencyToComboBoxIndex(int frequency)
        {
            // combobox index 0: no background refresh freq (default)
            // combobox index 1: refresh every 24 hours
            // combobox index 2: refresh after every 7 days
            switch (frequency)
            {
                case OneDay:
                    return 1;
                case SevenDays:
                    return 2;
                default:
                    return 0;
            }
        }
    }
}
