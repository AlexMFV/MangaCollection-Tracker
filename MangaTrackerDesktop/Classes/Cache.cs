using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using System.Reflection;
using Microsoft.SqlServer.Management.Assessment.Configuration;

namespace MangaTrackerDesktop
{
    class Cache
    {
        public static void SaveMangaListPartitioned(List<Mangas> mangaCol)
        {
            string cacheDir = Path.Combine(Globals.APPDATA_DIR, "alexmfv", "mangaTracker", "mangaDB");

            if (Directory.Exists(cacheDir))
            {
                foreach (string file in Directory.GetFiles(cacheDir))
                    File.Delete(file);
                Directory.Delete(cacheDir);
            }

            Directory.CreateDirectory(cacheDir);

            int aux = 0;

            for (int idx = 0; idx < mangaCol.Count; idx++)
            {
                //From Accounts Collection To JSON To File
                using (StreamWriter file = File.CreateText(cacheDir + "\\" + $"mangaDB-{mangaCol[idx][0].Id}-{mangaCol[idx][mangaCol[idx].Count - 1].Id}.json"))
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    JArray arr = new JArray();

                    foreach (Manga manga in mangaCol[idx])
                    {
                        JObject toAdd = new JObject();

                        foreach (var prop in manga.GetType().GetProperties())
                        {
                            if (prop.Name == "Releases")
                                continue;

                            if (prop.Name == "Id" || prop.Name == "Rating_Votes")
                                toAdd.Add(prop.Name, (int)prop.GetValue(manga));
                            else
                                if (prop.Name == "Rating")
                                    toAdd.Add(prop.Name, (double)prop.GetValue(manga));
                                else
                                    toAdd.Add(prop.Name, (string)prop.GetValue(manga));
                        }

                        //toAdd.Add("id", manga.Id);
                        //toAdd.Add("gid", manga.Gid);
                        //toAdd.Add("type", manga.Type);
                        //toAdd.Add("name", manga.Name);
                        //toAdd.Add("precision", manga.Precision);
                        //toAdd.Add("vintage", manga.Vintage);

                        aux++;
                        arr.Add(toAdd);
                    }

                    arr.WriteTo(writer);
                }
            }

            SaveChecksum(aux);
        }

        public static Mangas LoadMangaList()
        {
            string cacheDir = Path.Combine(Globals.APPDATA_DIR, "alexmfv", "mangaTracker", "mangaDB");

            Mangas collection = new Mangas();

            if (Directory.Exists(cacheDir))
            {
                if (Directory.GetFiles(cacheDir).Length > 0)
                {
                    //Get all the files inside the directory
                    foreach (string file in Directory.GetFiles(cacheDir, "mangaDB*"))
                    {
                        string content = File.ReadAllText(file);
                        if (content != "")
                        {
                            JArray arr = JArray.Parse(content);

                            foreach (JObject obj in arr)
                            {
                                Manga manga = new Manga();
                                manga = LoadMangaProperties(manga, obj);
                                collection.Add(manga);
                            }
                        }
                        //else //Maybe show an error to the user to reindex the database
                        //{
                        //    isCorrupted = true;
                        //    File.Delete(file);
                        //}
                    }

                    return collection;
                }
            }

            return null;
        }

        public static FavMangas LoadFavMangaList()
        {
            string cacheDir = Path.Combine(Globals.APPDATA_DIR, "alexmfv", "mangaTracker", "favDB");

            FavMangas collection = new FavMangas();

            if (Directory.Exists(cacheDir))
            {
                if (Directory.GetFiles(cacheDir).Length > 0)
                {
                    //Get all the files inside the directory
                    foreach (string file in Directory.GetFiles(cacheDir, "fav*"))
                    {
                        string content = File.ReadAllText(file);
                        if (content != "")
                        {
                            JObject obj = JObject.Parse(content);
                            FavManga manga = new FavManga();
                            manga.Id = int.Parse(obj.Property("id").Value.ToString());
                            manga.Title = obj.Property("title").Value.ToString();
                            manga.TitleTrimmed = obj.Property("titleTrimmed").Value.ToString();
                            manga.Volumes = int.Parse(obj.Property("volumes").Value.ToString());
                            manga.Volumes_owned = int.Parse(obj.Property("volumesOwned").Value.ToString());
                            manga.Volume_stat = obj.Property("volumeStat").Value.ToString();
                            manga.Rating = double.Parse(obj.Property("rating").Value.ToString());
                            manga.Image = obj.Property("imgUrl").Value.ToString();
                            collection.Add(manga);
                        }
                    }

                    return collection;
                }
            }

            return new FavMangas();
        }

        public static void SaveSingleManga(Manga manga)
        {
            string cacheDir = Path.Combine(Globals.APPDATA_DIR, "alexmfv", "mangaTracker", "titlesDB");

            if (!Directory.Exists(cacheDir))
                Directory.CreateDirectory(cacheDir);

            using (StreamWriter file = File.CreateText(cacheDir + "\\" + $"title_{manga.Id}.json"))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                JObject obj = new JObject();
                obj.Add("id", manga.Id);
                obj.Add("gid", manga.Gid);
                obj.Add("type", manga.Type);
                obj.Add("name", manga.Name);
                obj.Add("precision", manga.Precision);
                obj.Add("vintage", manga.Vintage);
                obj.Add("imgurl", manga.ImgURL); //Change this to image later (for offline use)
                obj.Add("jptitle", manga.JpTitle);
                obj.Add("plot", manga.PlotSummary);
                obj.Add("jpsite", manga.JpSite);
                obj.Add("ensite", manga.EnSite);
                obj.Add("releases", JsonConvert.SerializeObject(manga.Releases));
                obj.Add("rating", manga.Rating);
                obj.Add("rating_votes", manga.Rating_Votes);
                obj.WriteTo(writer);
            }
        }

        public static void SaveVolumeInfos(Volumes vols, int seriesID)
        {
            string cacheDir = Path.Combine(Globals.APPDATA_DIR, "alexmfv", "mangaTracker", "volumesDB");
            string filepath = Path.Combine(cacheDir, $"vols_{seriesID}.json");

            if (!Directory.Exists(cacheDir))
                Directory.CreateDirectory(cacheDir);

            Directory.CreateDirectory(cacheDir);

            using (StreamWriter file = File.CreateText(filepath))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                JArray arr = new JArray();

                foreach (Volume vol in vols)
                {
                    JObject obj = new JObject();

                    obj.Add("id", vol.Id);
                    obj.Add("price", vol.VolPrice);
                    obj.Add("shipping", vol.ShipPrice);
                    obj.Add("costs", vol.AddCosts);
                    obj.Add("buy", vol.BuyDate);
                    obj.Add("arrival", vol.ArrivalDate);
                    obj.Add("status", vol.Status);

                    arr.Add(obj);
                }

                arr.WriteTo(writer);
            }
        }

        public static Volumes LoadVolumeInfos(int seriesID)
        {
            string cacheDir = Path.Combine(Globals.APPDATA_DIR, "alexmfv", "mangaTracker", "volumesDB");
            Volumes collection = new Volumes();

            if (Directory.Exists(cacheDir))
            {
                if (Directory.GetFiles(cacheDir).Length > 0)
                {
                    //Get all the files inside the directory
                    string[] files = Directory.GetFiles(cacheDir, $"vols_{seriesID}.json");

                    if (files.Length > 0)
                    {
                        string content = File.ReadAllText(files[0]);
                        if (content != "")
                        {
                            JArray arr = JArray.Parse(content);

                            foreach (JObject obj in arr)
                            {
                                Volume vol = new Volume();
                                vol.Id = int.Parse(obj.Property("id").Value.ToString());
                                vol.VolPrice = double.Parse(obj.Property("price").Value.ToString());
                                vol.ShipPrice = double.Parse(obj.Property("shipping").Value.ToString());
                                vol.AddCosts = double.Parse(obj.Property("costs").Value.ToString());
                                vol.BuyDate = DateTime.Parse(obj.Property("buy").Value.ToString());
                                vol.ArrivalDate = DateTime.Parse(obj.Property("arrival").Value.ToString());
                                vol.Status = obj.Property("status").Value.ToString();
                                collection.Add(vol);
                            }
                        }
                    }
                }
                return collection;
            }

            return new Volumes();
        }

        public static void SaveFavManga(FavManga manga)
        {
            string cacheDir = Path.Combine(Globals.APPDATA_DIR, "alexmfv", "mangaTracker", "favDB");

            if (!Directory.Exists(cacheDir))
                Directory.CreateDirectory(cacheDir);

            using (StreamWriter file = File.CreateText(cacheDir + "\\" + $"fav_{manga.Id}.json"))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                JObject obj = new JObject();
                obj.Add("id", manga.Id);
                obj.Add("title", manga.Title);
                obj.Add("titleTrimmed", manga.TitleTrimmed);
                obj.Add("volumes", manga.Volumes);
                obj.Add("volumesOwned", manga.Volumes_owned);
                obj.Add("volumeStat", manga.Volume_stat);
                obj.Add("mainVolumes", manga.Main_volumes);
                obj.Add("rating", manga.Rating);
                obj.Add("imgUrl", manga.Image); //Change this to image later (for offline use)
                obj.WriteTo(writer);
            }
        }

        public static Manga LoadSingleManga(int _id)
        {
            string cacheDir = Path.Combine(Globals.APPDATA_DIR, "alexmfv", "mangaTracker", "titlesDB");

            if (!Directory.Exists(cacheDir))
                Directory.CreateDirectory(cacheDir);

            string file = cacheDir + $"\\title_{_id}.json";
            Manga manga = new Manga();

            if (File.Exists(file))
            {
                string content = File.ReadAllText(file);
                if (content != "")
                {
                    JObject obj = JObject.Parse(content);
                    manga.Id = int.Parse(obj.Property("id").Value.ToString());
                    manga.Gid = obj.Property("gid").Value.ToString();
                    manga.Type = obj.Property("type").Value.ToString();
                    manga.Name = obj.Property("name").Value.ToString();
                    manga.Precision = obj.Property("precision").Value.ToString();
                    manga.Vintage = obj.Property("vintage").Value.ToString();
                    manga.ImgURL = obj.Property("imgurl").Value.ToString();
                    manga.JpTitle = obj.Property("jptitle").Value.ToString();
                    manga.PlotSummary = obj.Property("plot").Value.ToString();
                    manga.JpSite = obj.Property("jpsite").Value.ToString();
                    manga.EnSite = obj.Property("ensite").Value.ToString();

                    JArray arr = JsonConvert.DeserializeObject<JArray>(obj.Property("releases").Value.ToString());
                    Releases rels = new Releases();
                    foreach(JObject obj2 in arr)
                    {
                        Release rel = new Release();
                        rel.Id = int.Parse(obj2.Property("Id").Value.ToString());
                        rel.Title = obj2.Property("Title").Value.ToString();
                        rel.Release_date = DateTime.Parse(obj2.Property("Release_date").Value.ToString());
                        rel.IsGN = bool.Parse(obj2.Property("IsGN").Value.ToString());
                        rel.IsOB = bool.Parse(obj2.Property("IsOB").Value.ToString());
                        rel.IsHC = bool.Parse(obj2.Property("IsHC").Value.ToString());
                        rel.IsBS = bool.Parse(obj2.Property("IsBS").Value.ToString());
                        rel.IsOther = bool.Parse(obj2.Property("IsOther").Value.ToString());
                        rels.Add(rel);
                    }

                    manga.Releases = rels;

                    manga.Rating = double.Parse(obj.Property("rating").Value.ToString());
                    manga.Rating_Votes = int.Parse(obj.Property("rating_votes").Value.ToString());
                    return manga;
                }
            }

            return null;
        }

        public static FavManga LoadFavManga(int _id)
        {
            string cacheDir = Path.Combine(Globals.APPDATA_DIR, "alexmfv", "mangaTracker", "favDB");

            if (!Directory.Exists(cacheDir))
                Directory.CreateDirectory(cacheDir);

            string file = cacheDir + $"\\fav_{_id}.json";
            FavManga manga = new FavManga();

            if (File.Exists(file))
            {
                string content = File.ReadAllText(file);
                if (content != "")
                {
                    JObject obj = JObject.Parse(content);
                    manga.Id = int.Parse(obj.Property("id").Value.ToString());
                    manga.Title = obj.Property("title").Value.ToString();
                    manga.TitleTrimmed = obj.Property("titleTrimmed").Value.ToString();
                    manga.Volumes = int.Parse(obj.Property("volumes").Value.ToString());
                    manga.Volumes_owned = int.Parse(obj.Property("volumesOwned").Value.ToString());
                    manga.Main_volumes = int.Parse(obj.Property("mainVolumes").Value.ToString());
                    manga.Rating = double.Parse(obj.Property("rating").Value.ToString());
                    manga.Volume_stat = obj.Property("volumeStat").Value.ToString();
                    manga.Image = obj.Property("imgUrl").Value.ToString();
                    return manga;
                }
            }

            return null;
        }

        public static void RemoveFavManga(int id)
        {
            string cacheDir = Path.Combine(Globals.APPDATA_DIR, "alexmfv", "mangaTracker", "favDB");
            string filepath = Path.Combine(cacheDir, $"fav_{id}.json");

            if (File.Exists(filepath))
                File.Delete(filepath);
        }

        public static bool isDBCorrupted()
        {
            LoadOrderedDB();
            LoadOrderedFavManga();
            int count = LoadChecksum();

            if (Globals.ALL_MANGAS == null || Globals.ALL_MANGAS.Count != count)
                return true;

            Globals.MANGA_LIST = Globals.ALL_MANGAS;
            return false;
        }

        public static Mangas OrderMangasByName(Mangas mangas)
        {
            if (mangas != null)
                return mangas.ToObject(mangas.ToList().OrderBy(item => item.Name).ToList());
            return null;
        }

        public static FavMangas OrderMangasByName(FavMangas mangas)
        {
            if (mangas != null)
                return mangas.ToObject(mangas.ToList().OrderBy(item => item.Title).ToList());
            return null;
        }

        private static int LoadChecksum()
        {
            string cacheDir = Path.Combine(Globals.APPDATA_DIR, "alexmfv", "mangaTracker", "mangaDB");

            if (!Directory.Exists(cacheDir))
                Directory.CreateDirectory(cacheDir);

            string file = cacheDir + "\\chksum.json";

            if (File.Exists(file))
            {
                string content = File.ReadAllText(file);
                if (content != "")
                {
                    JObject obj = JObject.Parse(content);
                    int value = int.Parse(obj.Property("num").Value.ToString());
                    return value;
                }
            }

            return -1;
        }

        public static void LoadOrderedDB()
        {
            Globals.ALL_MANGAS = OrderMangasByName(LoadMangaList());
            Globals.MANGA_LIST = Globals.ALL_MANGAS;
        }

        public static void LoadOrderedFavManga()
        {
            Globals.ALL_FAVMANGAS = OrderMangasByName(LoadFavMangaList());
            Globals.FAVMANGAS_LIST = Globals.ALL_FAVMANGAS;
        }

        private static void SaveChecksum(int _num)
        {
            string cacheDir = Path.Combine(Globals.APPDATA_DIR, "alexmfv", "mangaTracker", "mangaDB");

            if (!Directory.Exists(cacheDir))
                Directory.CreateDirectory(cacheDir);

            using (StreamWriter file = File.CreateText(cacheDir + "\\" + $"chksum.json"))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                JObject obj = new JObject();
                obj.Add("num", _num);
                obj.WriteTo(writer);
            }
        }

        private static Manga LoadMangaProperties(Manga manga, JObject toLoad)
        {
            foreach (PropertyInfo prop in manga.GetType().GetProperties())
            {
                switch (prop.Name)
                {
                    case "Id": prop.SetValue(manga, int.Parse(toLoad.Property(prop.Name).Value.ToString())); break;
                    //case "Genres": prop.SetValue(manga, (List<string>)toLoad.Property(prop.Name).Values()); break;
                    case "Genres": break;
                    //case "Author": prop.SetValue(manga, (List<Author>)toLoad.Property(prop.Name).Values()); break;
                    case "Authors": break;
                    case "Rating":
                        if(toLoad[prop.Name] != null) prop.SetValue(manga, double.Parse(toLoad.Property(prop.Name).Value.ToString()));
                        break;
                    case "Rating_Votes":
                        if (toLoad[prop.Name] != null) prop.SetValue(manga, int.Parse(toLoad.Property(prop.Name).Value.ToString()));
                        break;
                    case "Releases": break;
                    default: prop.SetValue(manga, toLoad.Property(prop.Name).Value.ToString()); break;
                }
            }

            return manga;

            //manga.Id = int.Parse(toLoad.Property("Id").Value.ToString());
            //manga.Gid = toLoad.Property("noteText").Value.ToString();
            //manga.Type = toLoad.Property("noteTitle").Value.ToString();
            //manga.Name = Parser.ToColor(toLoad.Property("noteColor").Value.ToString());
            //manga.Precision = Parser.ToColor(toLoad.Property("titleColor").Value.ToString());
            //manga.Vintage = DateTime.Parse(toLoad.Property("dateCreated").Value.ToString());
            //manga.ImgURL = toLoad.Property("baseFont").Value.ToString();
            //manga.JpTitle = int.Parse(toLoad.Property("baseFontSize").Value.ToString());
            //manga.Genres = toLoad.Property("baseFontColor").Value.ToString();
            //manga.PlotSummary = int.Parse(toLoad.Property("x").Value.ToString());
            //manga.JpSite = int.Parse(toLoad.Property("y").Value.ToString());
            //manga.EnSite = int.Parse(toLoad.Property("width").Value.ToString());
            //manga.Authors = int.Parse(toLoad.Property("height").Value.ToString());
        }
    }
}
