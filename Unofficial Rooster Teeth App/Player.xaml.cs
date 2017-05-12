using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
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
    public sealed partial class Player : Page
    {
        ApplicationDataContainer SettingsValues = ApplicationData.Current.LocalSettings;
        public Player()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            string VideoURL = "";
            this.InitializeComponent();
            VideoURL = LoadPlayer();
            RTPlayer.Source = new Uri(VideoURL);
            RTPlayer.AutoPlay = true;
            
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

        public string LoadPlayer()
        {
            using (var wc = new HttpClient())
            {
                HttpResponseMessage response = wc.GetAsync(new Uri(Episodes.SingleEpisode.PageURL)).Result;
                using (HttpContent content = response.Content)
                {
                    string PageSource = content.ReadAsStringAsync().Result;
                    PageSource = PageSource.Remove(0, PageSource.IndexOf("http://wpc.1765A.taucdn"));
                    PageSource = PageSource.Substring(0, PageSource.IndexOf("'"));
                    bool NewHLS = OldOrNew(PageSource);
                    if ((string)SettingsValues.Values["Quality"] != "Auto")
                    {
                        if (NewHLS)
                        {
                            PageSource = PageSource.Replace("index", "NewHLS-" + SettingsValues.Values["Quality"]);
                        }
                        else
                        {
                            if ((string)SettingsValues.Values["Quality"] == "360P")
                            {
                                // NOTE: This loads in 360P quality according to the index file
                                // I don't know why
                                PageSource = PageSource.Replace("index", "480P");
                            }
                            else
                            {
                                PageSource = PageSource.Replace("index", (string)SettingsValues.Values["Quality"]);
                            }
                        }
                    }
                    return PageSource;
                }
            }
        }

        public bool OldOrNew(string PageURL)
        {
            bool NewHLS;
            using (var wc = new HttpClient())
            {
                HttpResponseMessage response = wc.GetAsync(new Uri(PageURL)).Result;
                using (HttpContent content = response.Content)
                {
                    NewHLS = content.ReadAsStringAsync().Result.Contains("NewHLS");
                }
            }
            return NewHLS;
        }
    }
}
