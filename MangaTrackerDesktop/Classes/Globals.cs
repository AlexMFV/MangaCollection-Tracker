using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaTrackerDesktop
{
    public static class Globals
    {
        public static Mangas ALL_MANGAS = new Mangas();
        public static Mangas MANGA_LIST = new Mangas();
        public static FavMangas ALL_FAVMANGAS = new FavMangas();
        public static FavMangas FAVMANGAS_LIST = new FavMangas();
        //public static Volumes ALL_VOLUMES = new Volumes();
        public static string APPDATA_DIR = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static int NUM_PARTITION = 1000;
    }
}
