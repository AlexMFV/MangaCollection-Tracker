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
    /// Interaction logic for LibManga.xaml
    /// </summary>
    public partial class LibManga : Page
    {
        int id;
        Frame frame;
        object prevContent;
        FavManga manga;
        Manga m;

        public LibManga(object _content, Frame _frame, FavManga _manga)
        {
            InitializeComponent();
            this.manga = _manga;
            this.frame = _frame;
            this.prevContent = _content;
            this.m = Cache.LoadSingleManga(manga.Id);

            LoadMangaInfo();
            LoadReleases();
            LoadReleasesStatus();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.frame.Content = this.prevContent;
        }

        public void LoadMangaInfo()
        {

        }

        public void LoadReleases()
        {
            if (lstVolReleases is not null)
            {
                lstVolReleases.Items.Clear();
                foreach (Release rel in m.Releases)
                {
                    if (this.chkGN.IsChecked == true)
                        if (rel.IsGN)
                            lstVolReleases.Items.Add(rel.Title);

                    if (this.chkHC.IsChecked == true)
                        if (rel.IsHC)
                            lstVolReleases.Items.Add(rel.Title);

                    if (this.chkOB.IsChecked == true)
                        if (rel.IsOB)
                            lstVolReleases.Items.Add(rel.Title);

                    if (this.chkBS.IsChecked == true)
                        if (rel.IsBS)
                            lstVolReleases.Items.Add(rel.Title);

                    if (this.chkOther.IsChecked == true)
                        if (rel.IsOther)
                            lstVolReleases.Items.Add(rel.Title);
                }
            }
        }

        //Green - Owned, Yellow - On order, Red - On the way
        public void LoadReleasesStatus()
        {

        }

        private void TabItem_MouseEnter(object sender, MouseEventArgs e)
        {
            ((TextBlock)e.Source).Foreground = Brushes.LightBlue;
        }

        private void TabItem_MouseLeave(object sender, MouseEventArgs e)
        {
            ((TextBlock)e.Source).Foreground = Brushes.White;
        }

        private void chk_Checked(object sender, RoutedEventArgs e)
        {
            LoadReleases();
        }

        //private void tabVolumes_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    tabStats.Visibility = Visibility.Hidden;
        //    tabPrices.Visibility = Visibility.Hidden;
        //    tabVolumes.Visibility = Visibility.Visible;
        //}
        //
        //private void tabPrices_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    tabStats.Visibility = Visibility.Hidden;
        //    tabPrices.Visibility = Visibility.Visible;
        //    tabVolumes.Visibility = Visibility.Hidden;
        //}
        //
        //private void tabStats_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    tabStats.Visibility = Visibility.Visible;
        //    tabPrices.Visibility = Visibility.Hidden;
        //    tabVolumes.Visibility = Visibility.Hidden;
        //}
    }
}
