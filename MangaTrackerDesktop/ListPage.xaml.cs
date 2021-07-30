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
using System.Xml;

namespace MangaTrackerDesktop
{
    /// <summary>
    /// Interaction logic for ListPage.xaml
    /// </summary>
    public partial class ListPage : Page
    {
        int per_page = 500;
        int maxPages;
        int currPage = 0;
        Frame frame;

        public ListPage(Frame _frame)
        {
            InitializeComponent();
            this.frame = _frame;
            InitialSteps();
        }

        async public void InitialSteps()
        {
            if (Cache.isDBCorrupted())
            {
                await DatabaseFullReindex();
            }

            FillListWithMangas(0);
            FillPagesLabel();
        }

        public void FillPagesLabel()
        {
            maxPages = (int)Math.Ceiling((double)Globals.MANGA_LIST.Count / per_page);
            nupPage.Maximum = maxPages;
            lblPages.Content = $"Page {currPage + 1} / {maxPages}";
        }

        async public Task<bool> DatabaseFullReindex()
        {
            MessageBoxResult res = MessageBox.Show("The database needs to be reindexed, this may take some time!", "Database Reindex",
                MessageBoxButton.OK, MessageBoxImage.Information);

            if (res == MessageBoxResult.OK)
            {
                //Loading Screen

                string fullDatabase = "https://www.animenewsnetwork.com/encyclopedia/reports.xml?id=155&type=manga&nlist=all";
                Mangas fullDB = API.RequestAPIMangas(fullDatabase);
                List<Manga> mangaList = fullDB.ToList().OrderBy(x => x.Id).ToList();

                fullDB = new Mangas();
                for (int i = 0; i < mangaList.Count; i++)
                    fullDB.Add(mangaList[i]);

                List<Mangas> splittedDB = fullDB.Partition(Globals.NUM_PARTITION);
                Cache.SaveMangaListPartitioned(splittedDB);
                Cache.LoadOrderedDB();

                return true;
            }

            return false;
        }

        async public void FillListWithMangas(int page)
        {
            for (int i = (per_page * page); i < (per_page * page + per_page); i++)
            {
                if (i >= Globals.MANGA_LIST.Count)
                    return;

                ListViewItem newItem = new ListViewItem();
                newItem.Content = Globals.MANGA_LIST[i].Name;
                newItem.Tag = Globals.MANGA_LIST[i].Id;
                lstMangas.Items.Add(newItem);
            }
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (currPage > 0)
            {
                this.currPage--;
                UpdatePage();
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (currPage + 1 < maxPages)
            {
                this.currPage++;
                UpdatePage();
            }
        }

        private void btnGoTo_Click(object sender, RoutedEventArgs e)
        {
            if ((int)nupPage.Value - 1 != currPage)
            {
                this.currPage = (int)nupPage.Value - 1;
                UpdatePage();
            }
        }

        public void UpdatePage()
        {
            lstMangas.Items.Clear();
            FillListWithMangas(currPage);
            FillPagesLabel();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtSearchTitle.Text == "")
                {
                    currPage = 0;
                    Globals.MANGA_LIST = Globals.ALL_MANGAS;
                    UpdatePage();
                }
                else
                {
                    currPage = 0;
                    Globals.MANGA_LIST = Globals.ALL_MANGAS.ToObject(Globals.ALL_MANGAS.ToList().Where(x => x.Name.ToLower().Contains(txtSearchTitle.Text.ToLower())).ToList());
                    UpdatePage();
                }
            }
        }

        private void lstMangas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.frame.Content = new MangaInfo(frame.Content, frame, int.Parse(((ListViewItem)lstMangas.Items[lstMangas.SelectedIndex]).Tag.ToString()));
        }
    }
}
