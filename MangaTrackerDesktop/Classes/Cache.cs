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

            for(int idx = 0; idx < mangaCol.Count; idx++)
            {
                //From Accounts Collection To JSON To File
                using (StreamWriter file = File.CreateText(cacheDir + "\\" + $"mangaDB-{mangaCol[idx][0].Id}-{mangaCol[idx][mangaCol[idx].Count-1].Id}.json"))
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    JArray arr = new JArray();

                    foreach (Manga manga in mangaCol[idx])
                    {
                        JObject toAdd = new JObject();

                        foreach(var prop in manga.GetType().GetProperties())
                        {
                            if(prop.Name == "Id")
                                toAdd.Add(prop.Name, (int)prop.GetValue(manga));
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

                            foreach(JObject obj in arr)
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

        public static bool isDBCorrupted()
        {
            LoadOrderedDB();
            int count = LoadChecksum();

            if (Globals.MANGAS == null || Globals.MANGAS.Count != count)
                return true;
            return false;
        }

        public static Mangas OrderMangasByName(Mangas mangas)
        {
            if(mangas != null)
                return mangas.ToObject(mangas.ToList().OrderBy(item => item.Name).ToList());
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
            Globals.MANGAS = OrderMangasByName(LoadMangaList());
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
            foreach(PropertyInfo prop in manga.GetType().GetProperties())
            {
                switch (prop.Name)
                {
                    case "Id": prop.SetValue(manga, int.Parse(toLoad.Property(prop.Name).Value.ToString())); break;
                    //case "Genres": prop.SetValue(manga, (List<string>)toLoad.Property(prop.Name).Values()); break;
                    case "Genres": break;
                    //case "Author": prop.SetValue(manga, (List<Author>)toLoad.Property(prop.Name).Values()); break;
                    case "Authors": break;
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
