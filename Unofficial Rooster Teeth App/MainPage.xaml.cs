using System;
using System.Collections.Generic;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Unofficial_Rooster_Teeth_App
{
    /// <summary>
    /// The starting page, displaying the different webstes the information can be taken from
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<RTSites> RTList;
        public static RTSites SelectedRTSite = new RTSites();
        public MainPage()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            RTList = new List<RTSites>();
            FillLists();
            ApplicationDataContainer SettingsValues = ApplicationData.Current.LocalSettings;
            // Checks settings for first time use, this ensures no errors later on 
            // (Either though loadings the settings menu or trying to load a video)
            if (SettingsValues.Values["Quality"] == null)
            {
                SettingsValues.Values["Quality"] = "Auto";
                SettingsValues.Values["Username"] = "";
            }
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            this.InitializeComponent();
            // Loads a default icon 
            SiteImage.Source = new BitmapImage(new Uri ("https://roosterteeth.com/rt-favicon.png"));
            RTSitesList.ItemsSource = RTList;
        }

        /// <summary>
        /// Used for mobile devices (As the back button is hidden) and the only way back is to exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            Application.Current.Exit();
        }

        /// <summary>
        /// Updates the displayed data when a site is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RTSitesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedRTSite = (RTSites)RTSitesList.SelectedItem;
            SiteText.Text = SelectedRTSite.Name;
            SiteImage.Source = new BitmapImage(new Uri(SelectedRTSite.Image));
            ContinueButton.IsEnabled = true;
        }


        public class RTSites
        {
            public string Name { get; set; }
            public string Image { get; set; }
            public string SiteURL { get; set; }
            /// <summary>
            /// Holds the basic details for each Rooster Teeth based website
            /// </summary>
            /// <param name="Name"></param>
            /// <param name="Image"></param>
            /// <param name="SiteURL"></param>
            public RTSites(string Name, string Image, string SiteURL)
            {
                this.Name = Name;
                this.Image = Image;
                this.SiteURL = SiteURL;
            }
            public RTSites() { }
        }

        /// <summary>
        /// Fills the list with every Rooster Teeth based website I don't know if automatically detecting them is possible
        /// This is one of the few major hardcoded parts of the program (The other being logging in as a sponser)
        /// </summary>
        public void FillLists()
        {
            RTList.Add(new RTSites("Rooster Teeth", "https://roosterteeth.com/rt-favicon.png", "http://roosterteeth.com/show"));
            RTList.Add(new RTSites("Achievement \n Hunter", "https://achievementhunter.roosterteeth.com/ah-favicon.png", "http://achievementhunter.roosterteeth.com/show"));
            RTList.Add(new RTSites("Funhaus", "https://funhaus.roosterteeth.com/fh-favicon.png", "http://funhaus.roosterteeth.com/show"));
            RTList.Add(new RTSites("ScrewAttack", "https://screwattack.roosterteeth.com/sa-favicon.png", "http://screwattack.roosterteeth.com/show"));
            RTList.Add(new RTSites("Game Attack", "https://gameattack.roosterteeth.com/ga-favicon.png", "http://gameattack.roosterteeth.com/show"));
            RTList.Add(new RTSites("The Know", "https://theknow.roosterteeth.com/tk-favicon.png", "http://theknow.roosterteeth.com/show"));
            RTList.Add(new RTSites("Cow Chop", "https://cowchop.roosterteeth.com/cc-favicon.png", "http://cowchop.roosterteeth.com/show"));
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
            Frame.Navigate(typeof(Shows));
        }
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
            Frame.Navigate(typeof(Settings));
        }
    }
}
