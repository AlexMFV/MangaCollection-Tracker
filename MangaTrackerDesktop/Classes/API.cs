using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MangaTrackerDesktop
{
    public static class API
    {
        static bool write = false;
        static Mangas mangas = new Mangas();
        static Manga manga = new Manga();
        static MangaProperties props = MangaProperties.NONE;
        static MangaPropertyInfo propInfo = MangaPropertyInfo.NONE;
        static bool haltOp = false;

        public static Mangas RequestAPIMangas(string _url)
        {
            XmlTextReader reader = new XmlTextReader(_url);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: ProcessCommonXML(reader.Name); continue;
                    case XmlNodeType.Text: AddMangaCommon(reader.Value); break;
                    case XmlNodeType.EndElement: ResumeOps(reader.Name); break;
                    default: write = false; break;
                }
            }

            return mangas;
        }

        public static Manga RequestMangaInfo(string _url, Manga _manga)
        {
            manga = _manga;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = true;

            XmlReader reader = XmlReader.Create(_url, settings);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: ProcessDetailXML(reader.Name, reader); continue;
                    case XmlNodeType.Text: AddMangaDetail(reader.Value); break;
                    case XmlNodeType.EndElement: ResumeOps(reader.Name); break;
                    default: write = false; propInfo = MangaPropertyInfo.NONE; break;
                }
            }

            return manga;
        }

        public static void ProcessCommonXML(string type)
        {
            switch (type)
            {
                case "id": mangas.Add(new Manga()); props = MangaProperties.ID; write = true; break;
                case "gid": props = MangaProperties.GID; write = true; break;
                case "type": props = MangaProperties.TYPE; write = true; break;
                case "name": props = MangaProperties.NAME; write = true; break;
                case "precision": props = MangaProperties.PRECISION; write = true; break;
                case "vintage": props = MangaProperties.VINTAGE; write = true; break;
                case "args": haltOp = !haltOp; break;
                default: write = false; props = MangaProperties.NONE; break;
            }
        }

        public static void ProcessDetailXML(string type, XmlReader reader)
        {
            switch (type)
            {
                case "info": SetInfoTag(reader); break;
                //case "news": props = MangaProperties.NEWS; write = true; break;
                //case "release": props = MangaProperties.RELEASE; write = true; break;
                //case "ratings": props = MangaProperties.RATINGS; write = true; break;
                //case "staff": props = MangaProperties.STAFF; write = true; break; //Task or person also available
                default: write = false; propInfo = MangaPropertyInfo.NONE; break;
            }
        }

        public static void SetInfoTag(XmlReader reader)
        {
            switch (reader.GetAttribute("type"))
            {
                case "Picture":
                    manga.ImgURL = reader.GetAttribute("src"); break;
                case "Alternative title":
                    if (reader.GetAttribute("lang") == "JA")
                        propInfo = MangaPropertyInfo.ALT_TITLE;
                    write = true;
                    break;
                case "Plot Summary":
                    propInfo = MangaPropertyInfo.PLOTSUMMARY; write = true; break;
                case "Official website":
                    if (manga.EnSite == "" && reader.GetAttribute("lang") == "EN")
                        manga.EnSite = reader.GetAttribute("href");
                    else
                        if (manga.JpSite == "" && reader.GetAttribute("lang") == "JA")
                            manga.JpSite = reader.GetAttribute("href");
                    break;
            }
        }

        public static void ResumeOps(string name)
        {
            switch (name)
            {
                case "args": haltOp = false; break;
            }
        }

        public static void AddMangaCommon(string value)
        {
            if (write && !haltOp)
            {
                switch (props)
                {
                    case MangaProperties.ID: mangas[mangas.Count - 1].Id = int.Parse(value); break;
                    case MangaProperties.GID: mangas[mangas.Count - 1].Gid = value; break;
                    case MangaProperties.TYPE: mangas[mangas.Count - 1].Type = value; break;
                    case MangaProperties.NAME: mangas[mangas.Count - 1].Name = value; break;
                    case MangaProperties.PRECISION: mangas[mangas.Count - 1].Precision = value; break;
                    case MangaProperties.VINTAGE: mangas[mangas.Count - 1].Vintage = value; break;
                    default: break;
                }
            }
        }

        public static void AddMangaDetail(string value)
        {
            if (write && !haltOp)
            {
                switch (propInfo)
                {
                    case MangaPropertyInfo.ALT_TITLE: manga.JpTitle = value; break;
                    case MangaPropertyInfo.PLOTSUMMARY: manga.PlotSummary = value; break;
                    default: break;
                }
            }
        }
    }
}
