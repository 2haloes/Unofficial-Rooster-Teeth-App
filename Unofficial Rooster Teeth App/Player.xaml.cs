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
        public Player()
        {
            string VideoURL = "";
            this.InitializeComponent();
            VideoURL = LoadPlayer().Result;
            VideoURL = Task.Factory.StartNew( async () => await LoadPlayer()).Result.Result;
            RTPlayer.Source = new Uri(VideoURL);
            RTPlayer.AutoPlay = true;
            
        }

        public async Task<string> LoadPlayer()
        {
            using (var wc = new HttpClient())
            {
                HttpResponseMessage response = await wc.GetAsync(new Uri(Episodes.SingleEpisode.PageURL));
                using (HttpContent content = response.Content)
                {
                    string PageSource = await content.ReadAsStringAsync();
                    PageSource = PageSource.Remove(0, PageSource.IndexOf("http://wpc.1765A.taucdn"));
                    PageSource = PageSource.Substring(0, PageSource.IndexOf("'"));
                    PageSource = PageSource.Replace("index", "NewHLS-480P");
                    //MediaPlayerElement RTPlay = new MediaPlayerElement()
                    //{
                    //    Name = "RTPlay",
                    //    Stretch = Stretch.Uniform,
                    //    Source = MediaSource.CreateFromUri(new Uri(PageSource)),
                    //    AutoPlay = true,
                    //    AreTransportControlsEnabled = true
                    //};
                    //MainGrid.Children.Add(RTPlay);
                    return PageSource;
                }
            }
        }
    }
}
