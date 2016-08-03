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
        public QuandlConfig.AutoUpdateFrequencies AutoUpdateFrequency;

        public Settings()
        {
            InitializeComponent();

            Loaded += delegate
            {
                ApiKeyTextBox.Text = QuandlConfig.ApiKey;
                BindingHelper.SetItemSourceViaEnum(AutoUpdateComboBox, typeof(QuandlConfig.AutoUpdateFrequencies));
            };
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            QuandlConfig.ApiKey = ApiKeyTextBox.Text;
            QuandlConfig.AutoUpdateFrequency = (QuandlConfig.AutoUpdateFrequencies) AutoUpdateComboBox.SelectedValue;
            FadeImage(SaveStatus, new TimeSpan(0, 0, 0, 0, 250), new TimeSpan(0, 0, 0, 2), new TimeSpan(0, 0, 0, 0, 250));
        }

        public static void FadeImage(Image image, TimeSpan fadeInTime, TimeSpan waitTime, TimeSpan fadeOutTime)
        {
            var fadeInAnimation = new DoubleAnimation(1d, fadeInTime);
            var waitAnimation = new DoubleAnimation(1d, waitTime);
            var fadeOutAnimation = new DoubleAnimation(0d, fadeOutTime);
            var originalVisibility = image.Visibility;
            var originalOpactiy = image.Opacity;

            fadeInAnimation.Completed += (o, e) => { image.BeginAnimation(OpacityProperty, waitAnimation); };
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