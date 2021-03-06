﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Unofficial_Rooster_Teeth_App
{
    public class ShowsCode
    {
        public string ShowURL { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Info { get; set; }

        public ShowsCode(string ShowURL, string Name, string Image, string Info)
        {
            this.ShowURL = ShowURL;
            this.Name = Name;
            this.Image = Image;
            this.Info = Info;
        }

        public ShowsCode() { }

        public static async Task<List<ShowsCode>> ShowScraper(string SiteURL)
        {
            string webpage;
            List<string[]> ShowArrays = new List<string[]>();
            List<ShowsCode> AllShows = new List<ShowsCode>();
            string[] ShowBlocks;
            // Downloads the page for the shows (each RT site has a different domain but a similar page)
            using (var wc = new HttpClient())
            {
                HttpResponseMessage response = await wc.GetAsync(new Uri(SiteURL));
                using (HttpContent content = response.Content)
                {
                    webpage = await content.ReadAsStringAsync();
                }
            }
            int checkchar = 0;
            // Finds where the data for the shows are and cuts it from the string
            checkchar = webpage.IndexOf("<h2>New");
            checkchar = checkchar + 61;
            webpage = webpage.Remove(0, checkchar);
            webpage = webpage.Remove(webpage.IndexOf("<!-- =============== BEGIN FOOTER =============== -->") - 3);
            ShowBlocks = webpage.Split(new string[] { "</li>" }, StringSplitOptions.None);
            foreach (string item in ShowBlocks)
            {
                // The last result is not a show and is ignored, will likely replace with a better solution later on
                if (item == ShowBlocks[ShowBlocks.Count() - 1])
                {

                }
                else
                {
                    ShowArrays.Add(item.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None));
                }
            }

            // This takes:
            // The name, image and amount of Episodes & Seasons for display
            // and the URL to download next (If that show is selected)
            foreach (string[] item in ShowArrays)
            {
                string ShowURL = null;
                string Name = null;
                string Image = null;
                string Info = null;

                foreach (string stringitem in item)
                {
                    if (stringitem.IndexOf("<a") != -1)
                    {
                        ShowURL = stringitem.Remove(0, stringitem.IndexOf('"') + 1);
                        ShowURL = ShowURL.Remove(ShowURL.IndexOf('"'));
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
                }
                AllShows.Add(new ShowsCode(ShowURL, Name, Image, Info));
            }
            return AllShows;
        }
    }
}
