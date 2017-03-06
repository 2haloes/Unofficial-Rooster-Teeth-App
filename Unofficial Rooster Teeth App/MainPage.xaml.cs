using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Unofficial_Rooster_Teeth_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<RTSites> RTList;
        public static RTSites SelectedRTSite = new RTSites();
        public MainPage()
        {
            RTList = new List<RTSites>();
            RTList.Add(new RTSites("Rooster Teeth", "https://roosterteeth.com/rt-favicon.png", "http://roosterteeth.com/show"));
            RTList.Add(new RTSites("Achievement \n Hunter", "https://achievementhunter.roosterteeth.com/ah-favicon.png", "http://achievementhunter.roosterteeth.com/show"));
            RTList.Add(new RTSites("Funhaus", "https://funhaus.roosterteeth.com/fh-favicon.png", "http://funhaus.roosterteeth.com/show"));
            RTList.Add(new RTSites("ScrewAttack", "https://screwattack.roosterteeth.com/sa-favicon.png", "http://screwattack.roosterteeth.com/show"));
            RTList.Add(new RTSites("Game Attack", "https://gameattack.roosterteeth.com/ga-favicon.png", "http://gameattack.roosterteeth.com/show"));
            RTList.Add(new RTSites("The Know", "https://theknow.roosterteeth.com/tk-favicon.png", "http://theknow.roosterteeth.com/show"));
            RTList.Add(new RTSites("Cow Chop", "https://cowchop.roosterteeth.com/cc-favicon.png", "http://cowchop.roosterteeth.com/show"));
            ApplicationDataContainer SettingsValues = ApplicationData.Current.LocalSettings;
            if (SettingsValues.Values["Quality"] == null)
            {
                SettingsValues.Values["Quality"] = "Auto";
                SettingsValues.Values["Username"] = "";
                SettingsValues.Values["Password"] = "";
            }
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            this.InitializeComponent();
            SiteImage.Source = new BitmapImage(new Uri ("https://roosterteeth.com/rt-favicon.png"));
            RTSitesList.ItemsSource = RTList;
        }

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

            public RTSites(string Name, string Image, string SiteURL)
            {
                this.Name = Name;
                this.Image = Image;
                this.SiteURL = SiteURL;
            }
            public RTSites() { }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Shows));
        }
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Settings));
        }
    }
}
