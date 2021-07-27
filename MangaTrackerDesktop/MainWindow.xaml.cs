﻿using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.Collections;
using GraphQL.Common.Response;
using GraphQL.Common.Request;
using System.Xml.Serialization;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using System.Xml;

namespace MangaTrackerDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        bool write = false;
        Mangas mangas = new Mangas();
        MangaProperties props = MangaProperties.NONE;
        bool haltOp = false;

        public MainWindow()
        {
            InitializeComponent();
            InitialSteps();
        }

        async public void InitialSteps()
        {
            if (Cache.isDBCorrupted())
            {
                await DatabaseFullReindex();
            }

            FillListWithMangas();
        }

        public Mangas RequestAPIMangas(string _url)
        {
            XmlTextReader reader = new XmlTextReader(_url);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: ProcessXML(reader.Name); continue;
                    case XmlNodeType.Text: AddMangaCommon(reader.Value); break;
                    case XmlNodeType.EndElement: ResumeOps(reader.Name); break;
                    default: write = false; break;
                }
            }

            return mangas;
        }

        public void ProcessXML(string type)
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

        public void ResumeOps(string name)
        {
            switch (name)
            {
                case "args": haltOp = false; break;
            }
        }

        public void AddMangaCommon(string value)
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

        async public Task<bool> DatabaseFullReindex()
        {
            MessageBoxResult res = MessageBox.Show("The database needs to be reindexed, this may take some time!", "Database Reindex",
                MessageBoxButton.OK, MessageBoxImage.Information);

            if (res == MessageBoxResult.OK)
            {
                //Loading Screen

                string fullDatabase = "https://www.animenewsnetwork.com/encyclopedia/reports.xml?id=155&type=manga&nlist=all";
                Mangas fullDB = RequestAPIMangas(fullDatabase);
                List<Manga> mangaList = fullDB.ToList().OrderBy(x => x.Id).ToList();

                fullDB = new Mangas();
                for(int i = 0; i < mangaList.Count; i++)
                    fullDB.Add(mangaList[i]);

                List<Mangas> splittedDB = fullDB.Partition(Globals.NUM_PARTITION);
                Cache.SaveMangaListPartitioned(splittedDB);
                Cache.LoadOrderedDB();

                return true;
            }

            return false;
        }

        async public void FillListWithMangas()
        {
            for(int i = 0; i < Globals.MANGAS.Count; i++)
                lstMangas.Items.Add(Globals.MANGAS[i].Name);
        }
    }
}