using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Unofficial_Rooster_Teeth_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
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

        public async void FillList()
        {
            ShowList = await ShowsCode.ShowScraper(MainPage.SelectedRTSite.SiteURL);
            RTShowsList.ItemsSource = ShowList;
        }

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
