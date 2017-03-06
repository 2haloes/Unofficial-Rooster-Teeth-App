using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Unofficial_Rooster_Teeth_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        List<string> QualityList = new List<string>();
        ApplicationDataContainer SettingsValues;
        public Settings()
        {
            this.InitializeComponent();
            ComboBoxData();
            // This loads the app settings
            SettingsValues = ApplicationData.Current.LocalSettings;
            QualityComboBox.SelectedItem = SettingsValues.Values["Quality"];
        }

        public void ComboBoxData()
        {
            QualityList.Add("Auto");
            QualityList.Add("240P");
            QualityList.Add("360P");
            QualityList.Add("480P");
            QualityList.Add("720P");
            QualityList.Add("1080P");
            QualityComboBox.ItemsSource = QualityList;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsValues.Values["Quality"] = QualityComboBox.SelectedItem;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
