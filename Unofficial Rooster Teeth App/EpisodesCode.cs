using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Unofficial_Rooster_Teeth_App
{
    public class EpisodesCode
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string Runtime { get; set; }
        public string UploadTime { get; set; }
        public string SponserImage { get; set; }
        public string PageURL { get; set; }
        public int Season { get; set; }

        public EpisodesCode(string Name, string Image, string Runtime, string UploadTime, string SponserImage, string PageURL, int Season)
        {
            this.Name = Name;
            this.Image = Image;
            this.Runtime = Runtime;
            this.UploadTime = UploadTime;
            this.SponserImage = SponserImage;
            this.PageURL = PageURL;
            this.Season = Season;
        }

        public EpisodesCode() { }

        public static async Task<List<List<EpisodesCode>>> ExtractEpisodesCode(string PageURL)
        {
            string Webpage;
            List<List<EpisodesCode>> AllEpisodesCode = new List<List<EpisodesCode>>();
            using (var wc = new HttpClient())
            {
                HttpResponseMessage response = await wc.GetAsync(new Uri(PageURL));
                using (HttpContent content = response.Content)
                {
                    Webpage = await content.ReadAsStringAsync();
                }
            }
            // Reverse seasons from count (1 = 12 etc.)
            AllEpisodesCode = await FromShowPage(Webpage);
            return AllEpisodesCode;
        }

        private async static Task<List<List<EpisodesCode>>> FromShowPage(string Webpage)
        {
            #region Variables
            List<List<EpisodesCode>> AllEpisodesCode = new List<List<EpisodesCode>>();
            List<EpisodesCode> SeasonEpisodesCode = new List<EpisodesCode>();
            List<string[]> EpisodesCodeArray = new List<string[]>();
            List<string[]> SeasonsArray = new List<string[]>();
            List<string> PageList = new List<string>();
            string[] SeasonsBlock;
            string[] EpisodeBlocks;
            int checkchar = 0;
            int season = 1;
            checkchar = Webpage.IndexOf("tab-content-episodes");
            Webpage = Webpage.Remove(0, checkchar);
            #endregion
            #region Get episode/season blocks
            // Split into seasons into blocks of episodes
            SeasonsBlock = Webpage.Split(new string[] { "ul class='grid-blocks'" }, StringSplitOptions.None);
            season = SeasonsBlock.Count();
            foreach (var SeasonString in SeasonsBlock)
            {
                // If there isn't a link to a full season that goes somewhere else, get the epsodes from the current page
                // (This is for each season is seasons can be of different lengths)
                if ((checkchar = SeasonString.IndexOf("pull-right")) == -1)
                {
                    EpisodeBlocks = SeasonString.Split(new string[] { "</li>" }, StringSplitOptions.None);
                    EpisodesCodeArray = new List<string[]>();
                    foreach (string item in EpisodeBlocks)
                    {
                        EpisodesCodeArray.Add(item.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None));
                    }
                }
                else
                {
                    // Cuts the url of the season page
                    string tempString = "";
                    PageList = new List<string>();
                    tempString = SeasonString.Remove(0, (checkchar + 18));
                    tempString = tempString.Remove(tempString.IndexOf(">") - 1) +"?page=";
                    int pageNum = 1;
                    string pagestring = "";
                    // Downloads the page and checks if there is content, when there isn't then it exits the loop
                    do
                    {
                        using (var wc = new HttpClient())
                        {
                            HttpResponseMessage response = await wc.GetAsync(new Uri(tempString + pageNum));
                            using (HttpContent content = response.Content)
                            {
                                PageList.Add(await content.ReadAsStringAsync());
                            }
                        }
                        checkchar = PageList[pageNum - 1].IndexOf("timestamp");
                        pageNum++;
                    } while (checkchar != -1);
                    PageList.RemoveAt(PageList.Count - 1);
                    for (int i = 0; i < PageList.Count; i++)
                    {
                        PageList[i] = PageList[i].Remove(0, PageList[i].IndexOf("grid-blocks"));
                        pagestring += PageList[i].Substring(0, PageList[i].IndexOf("pagination"));
                    }
                    EpisodeBlocks = pagestring.Split(new string[] { "</li>" }, StringSplitOptions.None);
                    EpisodesCodeArray = new List<string[]>();
                    foreach (string item in EpisodeBlocks)
                    {
                        EpisodesCodeArray.Add(item.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None));
                    }
                }
                #endregion
                foreach (string[] EpisodeItem in EpisodesCodeArray)
                    {
                        string PageURL = null;
                        string Name = null;
                        string Image = null;
                        string Info = null;
                        string SponserImage = "";
                        string Runtime = null;

                        foreach (string stringitem in EpisodeItem)
                        {
                            if (stringitem.IndexOf("<a ") != -1)
                            {
                                PageURL = stringitem.Remove(0, stringitem.IndexOf('"') + 1);
                                PageURL = PageURL.Remove(PageURL.IndexOf('"'));
                            }
                            else if (stringitem.IndexOf("<img") != -1)
                            {
                                Image = stringitem.Remove(0, stringitem.IndexOf('"') + 3);
                                Image = Image.Remove(Image.IndexOf('"'));
                                Image = "https://" + Image;
                            }
                            else if (stringitem.IndexOf("<p class=\"name\"") != -1)
                            {
                                Name = stringitem.Remove(0, stringitem.IndexOf('>') + 1);
                                Name = Name.Remove(Name.IndexOf('<'));
                            }
                            else if (stringitem.IndexOf("<p class=\"post-stamp\"") != -1)
                            {
                                Info = stringitem.Remove(0, stringitem.IndexOf('>') + 1);
                                Info = Info.Remove(Info.IndexOf('<'));
                            }
                            else if (stringitem.IndexOf("icon ion-star") != -1)
                            {
                                SponserImage = "★";
                            }
                            else if (stringitem.IndexOf("ion-play") != -1)
                            {
                                Runtime = stringitem.Remove(0, stringitem.IndexOf("ion-play") + 15);
                                Runtime = Runtime.Remove(Runtime.IndexOf('<'));
                            }
                        }

                        if (Name == null || Runtime == null)
                        {
                            break;
                        }
                        SeasonEpisodesCode.Add(new EpisodesCode(Name, Image, Runtime, Info, SponserImage, PageURL, season));
                    }
                    AllEpisodesCode.Add(SeasonEpisodesCode);
                if (AllEpisodesCode[AllEpisodesCode.Count - 1].Count == 0)
                {
                    AllEpisodesCode.RemoveAt(AllEpisodesCode.Count - 1);
                }
                    SeasonEpisodesCode = new List<EpisodesCode>();
                    season--;
                }
            return AllEpisodesCode;
        }

        private static List<List<EpisodesCode>> FromSeasonPage(string Webpage)
        {
            List<List<EpisodesCode>> AllEpisodesCode = new List<List<EpisodesCode>>();
            List<string> AllLinks = new List<string>();
            List<int> LinkIndexs = new List<int>();
            int index = Webpage.IndexOf("pull");
            string tempString = Webpage.Substring(index + 18);
            while (index != -1)
            {
                AllLinks.Add(tempString.Remove(tempString.IndexOf(">") - 1));
                index = Webpage.IndexOf("pull", index + "pull".Length);
                tempString = Webpage.Substring(index + 18);
            }
            
            return AllEpisodesCode;
        }
    }
}
