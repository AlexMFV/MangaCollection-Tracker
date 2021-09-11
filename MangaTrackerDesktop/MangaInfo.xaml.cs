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

namespace MangaTrackerDesktop
{
    /// <summary>
    /// Interaction logic for MangaInfo.xaml
    /// </summary>
    public partial class MangaInfo : Page
    {
        object prevContent;
        int id;
        Frame frame;
        Manga manga = new Manga();
        int listID = -1;

        public MangaInfo(object _content, Frame _frame, int _id)
        {
            InitializeComponent();
            this.prevContent = _content;
            this.id = _id;
            this.frame = _frame;

            //Check if manga is added to favourites (change button text)
            listID = Globals.FAVMANGAS_LIST.GetID(id);
            if (listID != -1)
                ChangeFavouriteButton();

            //this.manga = Globals.ALL_MANGAS.GetByID(this.id);
            this.manga = Cache.LoadSingleManga(this.id);

            if (manga == null)
            {
                this.manga = Globals.ALL_MANGAS.GetByID(this.id);
                GetMangaData();
            }

            LoadMangaInfo();
        }

        public void LoadMangaInfo()
        {
            if (manga != null)
            {
                if(manga.ImgURL != "")
                    imgImage.Source = new BitmapImage(new Uri(manga.ImgURL));
                lblTitle.Content = manga.Name;
                lblType.Content = $"Type: {manga.Type}";
                lblJpnName.Content = $"Original Name: {manga.JpTitle}";
                lblFirstPub.Content = $"Publication Date: {manga.Vintage}";
                txtPlotSum.Text = manga.PlotSummary;
                lblRatingScore.Content = $"{Math.Round(manga.Rating, 2)}/10";
                lblRatingVotes.Content = $"{manga.Rating_Votes} users";
            }
            else
            {
                MessageBox.Show("There was a problem loading this title, please try again or re-index the database!", "Error", MessageBoxButton.OK);
                this.frame.Content = this.prevContent;
            }
        }

        async public Task<bool> GetMangaData()
        {
            try
            {
                string requestUrl = $"https://cdn.animenewsnetwork.com/encyclopedia/api.xml?manga={this.id}";
                Manga loadedManga = API.RequestMangaInfo(requestUrl, manga);
                Cache.SaveSingleManga(manga);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.frame.Content = this.prevContent;
        }

        public void ChangeFavouriteButton()
        {
            btnFav.Content = "Remove from Library";
            btnFav.Tag = "Rem";
        }

        private void btnFav_Click(object sender, RoutedEventArgs e)
        {
            if ((string)btnFav.Tag == "Add")
            {
                Globals.FAVMANGAS_LIST.Add(API.ConvertToFavorite(this.manga));
                listID = Globals.FAVMANGAS_LIST.Count - 1;
                Cache.SaveFavManga(Globals.FAVMANGAS_LIST[Globals.FAVMANGAS_LIST.Count - 1]);
                ChangeFavouriteButton();
            }
            else
            {
                if ((string)btnFav.Tag == "Rem")
                {
                    MessageBoxResult res = MessageBox.Show("Are you sure you want to remove this manga from your library?", "Confirmation", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                    {
                        Globals.FAVMANGAS_LIST.RemoveAt(listID);
                        Cache.RemoveFavManga(this.id);
                        listID = -1;
                        btnFav.Content = "Add to Library";
                        btnFav.Tag = "Add";
                        return;
                    }
                }
            }
        }
    }
}
