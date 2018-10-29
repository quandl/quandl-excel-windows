using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using GongSolutions.Wpf.DragDrop.Utilities;
using Quandl.Excel.Addin.UI.Helpers;
using Quandl.Shared;
using Quandl.Shared.Helpers;
using System.Threading.Tasks;

namespace Quandl.Excel.Addin.UI.Settings
{
    /// <summary>
    ///     Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public QuandlConfig.AutoUpdateFrequencies AutoUpdateFrequency;
        //public TaskPaneControl ParentControl { get; set; }

        public Settings()
        {
            InitializeComponent();
            Loaded += delegate
            {
                //QuandlConfig.ApiKeyChanged += 
                Reset();
                BindingHelper.SetItemSourceViaEnum(AutoUpdateComboBox, typeof(QuandlConfig.AutoUpdateFrequencies));
            };
            this.Unloaded += Settings_Unloaded;
        }

        private void Settings_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
        
        public void Reset()
        {
            ApiKeyTextBox.Text = QuandlConfig.ApiKey;
            LongRunningWarningTextBox.IsChecked = QuandlConfig.LongRunningQueryWarning;
            OverwriteWarningTextBox.IsChecked = QuandlConfig.OverwriteDataWarning;
            AutoUpdateComboBox.SelectedValue = QuandlConfig.AutoUpdateFrequency;
            ScollEnabledCheckBox.IsChecked = QuandlConfig.ScrollOnInsert;
        }

        private void SaveSettings(string apiKey)
        {
            QuandlConfig.ApiKey = apiKey;
            QuandlConfig.AutoUpdateFrequency = (QuandlConfig.AutoUpdateFrequencies)AutoUpdateComboBox.SelectedValue;
            QuandlConfig.LongRunningQueryWarning = (bool)LongRunningWarningTextBox.IsChecked;
            QuandlConfig.OverwriteDataWarning = (bool)OverwriteWarningTextBox.IsChecked;
            QuandlConfig.ScrollOnInsert = ScollEnabledCheckBox.IsChecked.Value;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmSave();
        }

        private void CandelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ConfirmSave()
        {
            var result = MessageBox.Show(Properties.Resources.SettingsSaveWarning,
                                         Properties.Resources.SettingsSaveConfirmation, 
                                         MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var key = ApiKeyTextBox.Text.Trim();
                if (string.IsNullOrEmpty(key))
                {
                    SaveSettings(key);
                    Close();
                }
                else
                {
                    Task.Run(() => ValidateApiKey(key));
                }
            }
            else
            {
                Close();
            }
        }

        async Task ValidateApiKey(string apiKey)
        {
            if (await QuandlConfig.ApiKeyValid(apiKey))
            {
                Dispatcher.Invoke(() => {
                    SaveSettings(apiKey);
                    Close();
                });
            }
            else
            {
                DisplayErrorMessage(Properties.Resources.InvalidApiKeyEntered);
            }
        }
        private void DisplayErrorMessage(string message)
        {
            // Necessary since the display message may be on a different thread
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message,
                    AddinModule.CurrentInstance.AddinName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            });
        }

        private void Close()
        {
            MainLogic.Instance.TaskPaneUpdater.Hide<SettingsControlHost>();
        }

        

    }
}