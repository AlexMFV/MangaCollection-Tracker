﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaTrackerDesktop
{
    public static class Globals
    {
        public static Mangas MANGAS = new Mangas();
        public static string APPDATA_DIR = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static int NUM_PARTITION = 1000;
    }
}