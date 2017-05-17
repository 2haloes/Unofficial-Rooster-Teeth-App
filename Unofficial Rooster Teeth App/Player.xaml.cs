using System;
using System.Collections.Generic;
using System.Net.Http;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Unofficial_Rooster_Teeth_App
{
    /// <summary>
    /// The page that loads the video, it also logs into the website if needed
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
            // Checks if the video is FIRST only
            if (Episodes.SingleEpisode.SponserImage == "★")
            {
                VideoURL = LoadPlayerAdvanced();
            }
            else
            {
                // This will be improved to swap to th advanced load player if needed
                // e.g. members (not just first) only
                VideoURL = LoadPlayer();
            }
            if (VideoURL == "Sponser Error")
            {
                MessageDialog errorDisplay = new MessageDialog("This is a FIRST member only video. If you are a FIRST member, please save your details in the settings screen");
                SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
                this.Frame.GoBack();
            }
            else if (VideoURL == "URL Error")
            {
                MessageDialog errorDisplay = new MessageDialog("An error has occured loading the video. Please report this including what video you were trying to load at https://github.com/2haloes/Unofficial-Rooster-Teeth-App/issues");
                SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
                this.Frame.GoBack();
            }
            // Sets up the details for the MediaElement (Which is the media player)
            // NOTE, if anyone wants to port this to WPF/Winforms, the videoplayer would need something exturnal as there isn't support for .ts playlists
            RTPlayer.Source = new Uri(VideoURL);
            RTPlayer.PosterSource = new BitmapImage(new Uri(Episodes.SingleEpisode.Image));
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

        /// <summary>
        /// Loading for basic videos
        /// </summary>
        /// <returns></returns>
        public string LoadPlayer()
        {
            using (var wc = new HttpClient())
            {
                HttpResponseMessage response = wc.GetAsync(new Uri(Episodes.SingleEpisode.PageURL)).Result;
                using (HttpContent content = response.Content)
                {
                    // This finds the video URL and (unless the video quality is on auto) sets the quality of the stream loaded
                    string PageSource = content.ReadAsStringAsync().Result;
                    // This checks if the video stream URL exists and if it does, it takes it from the rest of the page
                    // if not, it trys to login before trying again
                    if (PageSource.IndexOf("http://wpc.1765A.taucdn") == -1)
                    {
                        return LoadPlayerAdvanced();
                    }
                    PageSource = PageSource.Remove(0, PageSource.IndexOf("http://wpc.1765A.taucdn"));
                    PageSource = PageSource.Substring(0, PageSource.IndexOf("'"));
                    // This checks if the video is old or new (see OldOrNew method)
                    // before using the information to get the URI of the video stream
                    bool NewHLS = OldOrNew(PageSource);
                    // If auto video qulaity is selected then the index playlist file is used,
                    // it results in a lower quality video but it should load any videos if they have issues
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

        /// <summary>
        /// Logs into the RT website before attempting to load the video
        /// </summary>
        /// <returns></returns>
        public string LoadPlayerAdvanced()
        {
            // Programmer note. outside of the code to get the episodes, this was probably the most annoying thing to program
            // it didn't help that this was the second attempt at a program with this functionallity. This does work (even if it goes to the error page)
            using (var wc = new HttpClient())
            {
                // This gets the token from the login page
                string Token;
                HttpResponseMessage GetTokenResponce = wc.GetAsync(new Uri("http://www.roosterteeth.com/login")).Result;
                using (HttpContent content = GetTokenResponce.Content)
                {
                    string LoginPage = content.ReadAsStringAsync().Result;
                    LoginPage = LoginPage.Remove(0, (LoginPage.IndexOf("token") + 28));
                    Token = LoginPage.Substring(0, LoginPage.IndexOf("\""));
                }
                // This gets the user info from the PasswordVault
                string ResourceName = "Unofficial Rooster";
                PasswordCredential Credential = null;
                IReadOnlyList<PasswordCredential> credentialList;
                PasswordVault Vault = new PasswordVault();
                try
                {
                    credentialList = Vault.FindAllByResource(ResourceName);
                    Credential = credentialList[0];
                }
                catch (Exception)
                {

                }
                Dictionary<string, string> LoginDictionary = new Dictionary<string, string>()
                {
                    { "username", Credential.UserName },
                    { "password",  Vault.Retrieve(ResourceName, Credential.UserName).Password},
                    { "_token", Token }

                };
                // This POSTs the RT login page
                HttpContent LoginPost = new FormUrlEncodedContent(LoginDictionary);
                var postResponse = wc.PostAsync("http://www.roosterteeth.com/login", LoginPost).Result;
                postResponse.EnsureSuccessStatusCode();
                // If it was successful, it should have the username of the user in the HTML (as is displays the username within the page)
                // This is annoyingly the only real check that can be made
                if (postResponse.Content.ReadAsStringAsync().Result.Contains(Credential.UserName))
                {
                    return "Sponser Error";
                }
                // Everything here is the same as the LoadPlayer method and comments there also apply here
                HttpResponseMessage response = wc.GetAsync(new Uri(Episodes.SingleEpisode.PageURL)).Result;
                using (HttpContent content = response.Content)
                {
                    string PageSource = content.ReadAsStringAsync().Result;
                    if (PageSource.IndexOf("http://wpc.1765A.taucdn") == -1)
                    {
                        return "URL Error";
                    }
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

        /// <summary>
        /// Checks if the video is in the old or new format (The new format uses NewHLS in the video stream names)
        /// </summary>
        /// <param name="PageURL"></param>
        /// <returns></returns>
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

        private void RTPlayer_Finished(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
            RTPlayer.IsFullWindow = false;
            this.Frame.GoBack();
        }
    }
}
