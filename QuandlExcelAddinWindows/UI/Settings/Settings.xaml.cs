using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using GongSolutions.Wpf.DragDrop.Utilities;
using Quandl.Excel.Addin.UI.Helpers;
using Quandl.Shared;

namespace Quandl.Excel.Addin.UI.Settings
{
    /// <summary>
    ///     Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public QuandlConfig.AutoUpdateFrequencies AutoUpdateFrequency;
        public TaskPaneControl ParentControl { get; set; }

        public Settings()
        {
            InitializeComponent();
            Loaded += delegate
            {
                SetSettings();
                BindingHelper.SetItemSourceViaEnum(AutoUpdateComboBox, typeof(QuandlConfig.AutoUpdateFrequencies));
            };
        }

        public void SetSettings()
        {
            ApiKeyTextBox.Text = QuandlConfig.ApiKey;
            LongRunningWarningTextBox.IsChecked = QuandlConfig.LongRunningQueryWarning;
            OverwriteWarningTextBox.IsChecked = QuandlConfig.OverwriteDataWarning;
            AutoUpdateComboBox.SelectedValue = QuandlConfig.AutoUpdateFrequency;
            ScollEnabledCheckBox.IsChecked = QuandlConfig.ScrollOnInsert;
        }

        private void SaveSettings()
        {
            var keyInput = ApiKeyTextBox.Text.Trim();
            if (QuandlConfig.ApiKey != keyInput)
            {
                QuandlConfig.Instance.LoginChanged += ValideKey;
                QuandlConfig.ApiKey = keyInput;
            }
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
            var result = MessageBox.Show(Properties.Settings.Default.SettingsSaveWarning,
                                         Properties.Settings.Default.SettingsSaveConfirmation, 
                                         MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                SaveSettings();
            }
            Close();
        }

        private void Close()
        {
            if (ParentControl != null) ParentControl.Close();
        }

        private async void ValideKey()
        {
            try
            {
                await QuandlConfig.ApiKeyValid();
            }
            catch (Exception exp)
            {
                Globals.ThisAddIn.UpdateStatusBar(exp);
                Utilities.LogToSentry(exp);
            }
        }

    }
}