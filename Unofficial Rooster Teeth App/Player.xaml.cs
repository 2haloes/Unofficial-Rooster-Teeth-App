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
            string VideoURL = "";
            this.InitializeComponent();
            VideoURL = LoadPlayer();
            RTPlayer.Source = new Uri(VideoURL);
            RTPlayer.AutoPlay = true;
            
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
                    if ((string)SettingsValues.Values["Quality"] != "Auto")
                    {
                        PageSource = PageSource.Replace("index", "NewHLS-" + SettingsValues.Values["Quality"]);
                    }
                    
                    return PageSource;
                }
            }
        }
    }
}
