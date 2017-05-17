using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Unofficial_Rooster_Teeth_App
{
    /// <summary>
    /// Shows all of the shows from a selected website
    /// </summary>
    public sealed partial class Shows : Page
    {
        List<ShowsCode> ShowList = new List<ShowsCode>();
        public static ShowsCode SelectedShow = new ShowsCode();
        public Shows()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            FillList();
            this.InitializeComponent();
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

        /// <summary>
        /// This fills the list of shows (this does take a few seconds to load everything however)
        /// </summary>
        public async void FillList()
        {
            ShowList = await ShowsCode.ShowScraper(MainPage.SelectedRTSite.SiteURL);
            RTShowsList.ItemsSource = ShowList;
        }

        /// <summary>
        /// This updates the displayed values to show what is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RTShowsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedShow = (ShowsCode)RTShowsList.SelectedItem;
            ShowText.Text = SelectedShow.Name;
            ShowImage.Source = new BitmapImage(new Uri(SelectedShow.Image));
            ContinueButton.IsEnabled = true;
            
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
            Frame.Navigate(typeof(Episodes));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
