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
        public ImageSource Imagem { get; set; }
        public string Name { get; set; }
        public int Progress { get; set; }
        public int Chapters { get; set; }
        public int Volumes { get; set; }
    }
}
