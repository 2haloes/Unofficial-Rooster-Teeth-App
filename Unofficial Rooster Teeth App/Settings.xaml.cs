using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Core;
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
    /// Used to access and change settings such as the user's account and video quality
    /// </summary>
    public sealed partial class Settings : Page
    {
        PasswordVault Vault;
        string ResourceName;
        PasswordCredential Credential;
        List<string> QualityList = new List<string>();
        ApplicationDataContainer SettingsValues;
        public Settings()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            this.InitializeComponent();
            ComboBoxData();
            // This loads the app settings
            SettingsValues = ApplicationData.Current.LocalSettings;
            SetupLocker();
            QualityComboBox.SelectedItem = SettingsValues.Values["Quality"];
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                e.Handled = true;
                SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
                this.Frame.GoBack();
            }
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

        public void SetupLocker()
        {
            ResourceName = "Unofficial Rooster";
            Credential = null;
            IReadOnlyList<PasswordCredential> credentialList;
            Vault = new PasswordVault();
            try
            {
                credentialList = Vault.FindAllByResource(ResourceName);
                Credential = credentialList[0];
                UserNameTextBox.Text = Credential.UserName;
                PasswordTextBox.Password = Vault.Retrieve(ResourceName, Credential.UserName).Password;
            }
            catch (Exception)
            {
                
            }
        }
        public void UpdateVault()
        {
            Vault.Remove(Credential);
            Vault.Add(new PasswordCredential(ResourceName, UserNameTextBox.Text, PasswordTextBox.Password));
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsValues.Values["Quality"] = QualityComboBox.SelectedItem;
            UpdateVault();
            SettingsValues.Values["Username"] = UserNameTextBox.Text;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
            Frame.GoBack();
        }
    }
}
