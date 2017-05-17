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
    /// Shows all episodes and seasons of a show
    /// </summary>
    public sealed partial class Episodes : Page
    {
        public string EpisodeURL;
        public static string EpisodeImgURL;
        List<List<EpisodesCode>> AllSeasons = new List<List<EpisodesCode>>();
        List<EpisodesCode> SingleSeason = new List<EpisodesCode>();
        public static EpisodesCode SingleEpisode = new EpisodesCode();
        public Episodes()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            this.InitializeComponent();
            FillList();
        }

        /// <summary>
        /// Used to go back to the shows page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            // Pulls the episode and seasons list from the website
            AllSeasons = await EpisodesCode.ExtractEpisodesCode(Shows.SelectedShow.ShowURL);
            List<int> SeasonList = new List<int>();
            for (int i = 0; i != AllSeasons.Count; i++)
            {
                SeasonList.Add(i + 1);
            }
            // Reverses the season count to start at 1
            AllSeasons.Reverse();
            RTEpisodeSeason.ItemsSource = SeasonList;
        }

        /// <summary>
        /// Displays information about the episode (Name, thumbnail, if it is FIRST only and the runtime
        /// Also enables the button to continue to the video player
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RTEpisodesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SingleEpisode = (EpisodesCode)RTEpisodesList.SelectedItem;
            if (RTEpisodesList.SelectedItem != null)
            {
                EpisodeText.Text = SingleEpisode.Name;
                EpisodeImage.Source = new BitmapImage(new Uri(SingleEpisode.Image));
                FirstMember.Text = SingleEpisode.SponserImage;
                Runtime.Text = SingleEpisode.Runtime;
                ContinueButton.IsEnabled = true;
            }
        }
        
        /// <summary>
        /// Loads the video player
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
            Frame.Navigate(typeof(Player));
        }

        /// <summary>
        /// Goes to the shows page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
            Frame.GoBack();
        }

        /// <summary>
        /// Resets the displayed data and loads the data of the newly selected season
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RTEpisodeSeason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RTEpisodesList.SelectedItem = new EpisodesCode();
            SingleEpisode = new EpisodesCode();
            EpisodeText.Text = "";
            EpisodeImage.Source = null;
            FirstMember.Text = "";
            Runtime.Text = "";
            ContinueButton.IsEnabled = false;
            RTEpisodesList.ItemsSource = AllSeasons[(int)RTEpisodeSeason.SelectedItem - 1];
        }
    }
}
