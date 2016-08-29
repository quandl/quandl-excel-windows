using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Quandl.Excel.Addin.UI.Helpers;
using Quandl.Shared;

namespace Quandl.Excel.Addin.UI.Settings
{
    /// <summary>
    ///     Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        private const int SaveIconFadeTimeMs = 250;
        private const int SaveIconWaitTimeMs = 750;

        public QuandlConfig.AutoUpdateFrequencies AutoUpdateFrequency;

        public Settings()
        {
            InitializeComponent();

            Loaded += delegate
            {
                ApiKeyTextBox.Text = QuandlConfig.ApiKey;
                LongRunningWarningTextBox.IsChecked = QuandlConfig.LongRunningQueryWarning;
                OverwriteWarningTextBox.IsChecked = QuandlConfig.OverwriteDataWarning;
                AutoUpdateComboBox.SelectedValue = QuandlConfig.AutoUpdateFrequency;
                ScollEnabledCheckBox.IsChecked = QuandlConfig.ScrollOnInsert;

                BindingHelper.SetItemSourceViaEnum(AutoUpdateComboBox, typeof(QuandlConfig.AutoUpdateFrequencies));
            };
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            FadeImage(SaveStatus, new TimeSpan(0, 0, 0, 0, SaveIconFadeTimeMs), new TimeSpan(0, 0, 0, 0, SaveIconWaitTimeMs), new TimeSpan(0, 0, 0, 0, SaveIconFadeTimeMs), SaveSettings);            
        }

        private void SaveSettings()
        {
            QuandlConfig.ApiKey = ApiKeyTextBox.Text;
            QuandlConfig.AutoUpdateFrequency = (QuandlConfig.AutoUpdateFrequencies)AutoUpdateComboBox.SelectedValue;
            QuandlConfig.LongRunningQueryWarning = (bool)LongRunningWarningTextBox.IsChecked;
            QuandlConfig.OverwriteDataWarning = (bool)OverwriteWarningTextBox.IsChecked;
            QuandlConfig.ScrollOnInsert = ScollEnabledCheckBox.IsEnabled;
        }

        private void FadeImage(Image image, TimeSpan fadeInTime, TimeSpan waitTime, TimeSpan fadeOutTime, Action action)
        {
            var fadeInAnimation = new DoubleAnimation(1d, fadeInTime);
            var waitAnimation = new DoubleAnimation(1d, waitTime);
            var fadeOutAnimation = new DoubleAnimation(0d, fadeOutTime);
            var originalVisibility = image.Visibility;
            var originalOpactiy = image.Opacity;

            fadeInAnimation.Completed += (o, e) => { action(); image.BeginAnimation(OpacityProperty, waitAnimation); };
            waitAnimation.Completed += (o, e) => { image.BeginAnimation(OpacityProperty, fadeOutAnimation); };
            fadeOutAnimation.Completed += (o, e) =>
            {
                image.Visibility = originalVisibility;
                image.Opacity = originalOpactiy;
            };

            image.Opacity = 0;
            image.Visibility = Visibility.Visible;
            image.BeginAnimation(OpacityProperty, fadeInAnimation);
        }
    }
}