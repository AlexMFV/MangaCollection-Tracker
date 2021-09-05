using ControlzEx.Standard;
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
        private bool isGN; //Graphic Novel
        private bool isOB; //Omnibus
        private bool isHC; //Hardcover
        private bool isBS; //Box Set
        private bool isOther; //Other releases

        [JsonProperty("Id")]
        public int Id { get => id; set => id = value; }
        [JsonProperty("Title")]
        public string Title { get => title; set => title = value; }
        [JsonProperty("Release_date")]
        public DateTime Release_date { get => release_date; set => release_date = value; }
        [JsonProperty("IsGN")]
        public bool IsGN { get => isGN; set => isGN = value; }
        [JsonProperty("IsOB")]
        public bool IsOB { get => isOB; set => isOB = value; }
        [JsonProperty("IsHC")]
        public bool IsHC { get => isHC; set => isHC = value; }
        [JsonProperty("IsBS")]
        public bool IsBS { get => isBS; set => isBS = value; }
        [JsonProperty("IsOther")]
        public bool IsOther { get => isOther; set => isOther = value; }

        public Release() { }

        public Release(int id, string title, DateTime release_date, int urlId, bool isgn = false, bool isob = false, bool ishc = false, bool isbs = false, bool isother = false)
        {
            this.Id = id;
            this.Title = title;
            this.Release_date = release_date;
            this.IsGN = isgn;
            this.IsOB = isob;
            this.IsHC = ishc;
            this.IsBS = isbs;
            this.IsOther = isother;
        }

        public void SetReleaseType(bool isgn = false, bool isob = false, bool ishc = false, bool isbs = false, bool isother = false)
        {
            this.IsGN = isgn;
            this.IsOB = isob;
            this.IsHC = ishc;
            this.IsBS = isbs;
            this.IsOther = isother;
        }
    }
}
