using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MangaTrackerDesktop
{
    class Cache
    {
        public static void SaveMangaListPartitioned(List<Mangas> mangaCol)
        {
            string cacheDir = Path.Combine(Globals.APPDATA_DIR, "alexmfv", "mangaTracker", "mangaDB");

            if (!Directory.Exists(cacheDir))
                Directory.CreateDirectory(cacheDir);

            for(int idx = 0; idx < mangaCol.Count; idx++)
            {
                //From Accounts Collection To JSON To File
                using (StreamWriter file = File.CreateText(cacheDir + "\\" + $"mangaDB-{mangaCol[idx][0].Id}-{mangaCol[idx][mangaCol.Count-1].Id}.json"))
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    JArray arr = new JArray();

                    foreach (Manga manga in mangaCol[idx])
                    {
                        JObject toAdd = new JObject();

                        foreach(var prop in manga.GetType().GetProperties())
                            toAdd.Add(prop.Name, (string)prop.GetValue(manga));

                        //toAdd.Add("id", manga.Id);
                        //toAdd.Add("gid", manga.Gid);
                        //toAdd.Add("type", manga.Type);
                        //toAdd.Add("name", manga.Name);
                        //toAdd.Add("precision", manga.Precision);
                        //toAdd.Add("vintage", manga.Vintage);

                        arr.Add(toAdd);
                    }

                    arr.WriteTo(writer);
                }
            }
        }
    }
}
