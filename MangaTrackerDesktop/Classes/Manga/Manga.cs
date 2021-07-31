using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MangaTrackerDesktop
{
    public class Manga
    {
        /* COMMON */
        private int id;
        private string gid;
        private string type;
        private string name;
        private string precision;
        private string vintage;

        /* DETAILS */
        private string imgURL;
        private string jpTitle; //Japanese Title
        private List<string> genres;
        private string plotSummary; //Japanese Title
        private string jpSite; //Japanese Website
        private string enSite; //English Website
        private double rating;
        private int rating_votes;

        //public List<MangaReleases>; //Dates, links and names of releases
        //public List<MangaNews> //Dates, links and description

        private List<Author> authors;

        public int Id { get => id; set => id = value; }
        public string Gid { get => gid; set => gid = value; }
        public string Type { get => type; set => type = value; }
        public string Name { get => name; set => name = value; }
        public string Precision { get => precision; set => precision = value; }
        public string Vintage { get => vintage; set => vintage = value; }
        public string ImgURL { get => imgURL; set => imgURL = value; }
        public string JpTitle { get => jpTitle; set => jpTitle = value; }
        public List<string> Genres { get => genres; set => genres = value; }
        public string PlotSummary { get => plotSummary; set => plotSummary = value; }
        public string JpSite { get => jpSite; set => jpSite = value; }
        public string EnSite { get => enSite; set => enSite = value; }
        public List<Author> Authors { get => authors; set => authors = value; }
        public double Rating { get => rating; set => rating = value; }
        public int Rating_Votes { get => rating_votes; set => rating_votes = value; }

        public Manga() { }

        public Manga(int id, string gid, string type, string name, string precision, string vintage)
        {
            Id = id;
            Gid = gid;
            Type = type;
            Name = name;
            Precision = precision;
            Vintage = vintage;
        }

        public Manga(int id, string gid, string type, string name, string precision, string vintage, string imgURL, string jpTitle, List<string> genres, string plotSummary, string jpSite, string enSite, List<Author> authors, double rating, int ratingvotes) : this(id, gid, type, name, precision, vintage)
        {
            this.imgURL = imgURL;
            this.jpTitle = jpTitle;
            this.genres = genres;
            this.plotSummary = plotSummary;
            this.jpSite = jpSite;
            this.enSite = enSite;
            this.authors = authors;
            this.rating = rating;
            this.rating_votes = ratingvotes;
        }
    }
}
