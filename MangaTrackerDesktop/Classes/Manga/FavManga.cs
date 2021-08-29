using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MangaTrackerDesktop
{
    public class FavManga
    {
        private int id;
        private string title;
        private string titleTrimmed;
        private int volumes;
        private int main_volumes;
        private int volumes_owned;
        private string volume_stat;
        private string image;

        public FavManga() { }

        public FavManga(int id, string title, int volumes, int volumes_owned, int mainVolumes, string image)
        {
            this.id = id;
            this.title = title;

            if (this.title.Length > 35)
                this.titleTrimmed = title.Substring(0, 35);
            else
                this.titleTrimmed = this.title;

            this.volumes = volumes;
            this.volumes_owned = volumes_owned;
            this.main_volumes = mainVolumes;
            //this.Volume_stat = $"{this.volumes_owned} / {this.volumes} volumes";
            this.Volume_stat = $"{this.volumes_owned} / {this.main_volumes} volumes";
            this.image = image;
        }

        public int Id { get => id; set => id = value; }
        public string Title { get => title; set => title = value; }
        public int Volumes { get => volumes; set => volumes = value; }
        public int Volumes_owned { get => volumes_owned; set => volumes_owned = value; }
        public string Image { get => image; set => image = value; }
        public string Volume_stat { get => volume_stat; set => volume_stat = value; }
        public string TitleTrimmed { get => titleTrimmed; set => titleTrimmed = value; }
        public int Main_volumes { get => main_volumes; set => main_volumes = value; }
    }
}
