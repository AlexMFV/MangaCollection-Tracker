using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaTrackerDesktop
{
    public class Release
    {
        private int id;
        private string title;
        private DateTime release_date;
        private bool isGN;

        [JsonProperty("Id")]
        public int Id { get => id; set => id = value; }
        [JsonProperty("Title")]
        public string Title { get => title; set => title = value; }
        [JsonProperty("Release_date")]
        public DateTime Release_date { get => release_date; set => release_date = value; }
        [JsonProperty("IsGN")]
        public bool IsGN { get => isGN; set => isGN = value; }

        public Release() { }

        public Release(int id, string title, DateTime release_date, int urlId, bool isgn)
        {
            this.Id = id;
            this.Title = title;
            this.Release_date = release_date;
            this.IsGN = isGN;
        }
    }
}
