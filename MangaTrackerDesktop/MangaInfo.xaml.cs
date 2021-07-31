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

        public MangaInfo(object _content, Frame _frame, int _id)
        {
            InitializeComponent();
            this.prevContent = _content;
            this.id = _id;
            this.frame = _frame;

            //Check if a file exists for this title (save the file with the id of the title)
            //If it has no data
            //Request from api with ID
            //Else
            //Read data from files

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
    }
}
