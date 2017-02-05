using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class Episodes : Page
    {
        List<List<EpisodesCode>> AllSeasons = new List<List<EpisodesCode>>();
        List<EpisodesCode> SingleSeason = new List<EpisodesCode>();
        EpisodesCode SingleEpisode = new EpisodesCode();
        public Episodes()
        {
            this.InitializeComponent();
            FillList();
        }

        public async void FillList()
        {
            AllSeasons = await EpisodesCode.ExtractEpisodesCode(Shows.SelectedShow.ShowURL);
            List<int> SeasonList = new List<int>();
            for (int i = 0; i != AllSeasons.Count; i++)
            {
                SeasonList.Add(i + 1);
            }
            AllSeasons.Reverse();
            RTEpisodeSeason.ItemsSource = SeasonList;
        }

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

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void RTEpisodeSeason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RTEpisodesList.SelectedItem = new EpisodesCode();
            SingleEpisode = new EpisodesCode();
            EpisodeText.Text = "";
            EpisodeImage.Source = null;
            FirstMember.Text = "";
            ContinueButton.IsEnabled = false;
            RTEpisodesList.ItemsSource = AllSeasons[(int)RTEpisodeSeason.SelectedItem - 1];
        }
    }
}
