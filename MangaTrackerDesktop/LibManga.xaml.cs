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
        Volumes vols = new Volumes();

        public LibManga(object _content, Frame _frame, FavManga _manga)
        {
            InitializeComponent();
            this.manga = _manga;
            this.frame = _frame;
            this.prevContent = _content;
            this.m = Cache.LoadSingleManga(manga.Id);

            vols = Cache.LoadVolumeInfos(this.manga.Id);

            LoadComboBox();
            LoadMangaInfo();
            LoadReleases();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.frame.Content = this.prevContent;
        }

        public void LoadMangaInfo()
        {

        }

        public void LoadComboBox()
        {
            cbbStatus.Items.Clear();
            cbbStatus.Items.Add(Status.owned);
            cbbStatus.Items.Add(Status.ordered);
            cbbStatus.Items.Add(Status.otw);
            cbbStatus.Items.Add(Status.preorder);
            cbbStatus.Items.Add(Status.none);
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
                            AddVolumeItem(rel);

                    if (this.chkHC.IsChecked == true)
                        if (rel.IsHC)
                            AddVolumeItem(rel);

                    if (this.chkOB.IsChecked == true)
                        if (rel.IsOB)
                            AddVolumeItem(rel);

                    if (this.chkBS.IsChecked == true)
                        if (rel.IsBS)
                            AddVolumeItem(rel);

                    if (this.chkOther.IsChecked == true)
                        if (rel.IsOther)
                            AddVolumeItem(rel);
                }
            }
        }

        public void AddVolumeItem(Release rel)
        {
            ListViewItem item = new ListViewItem();
            item.Content = rel.Title;
            item.Tag = rel.Id;
            Volume newVol = vols.GetByID(rel.Id);
            if(newVol is not null)
                ChangeColor(item, newVol.Status);
            lstVolReleases.Items.Add(item);
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

        private void cbbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cbbStatus.SelectedIndex != -1 && lstVolReleases.SelectedItems.Count > 0)
            {
                foreach(ListViewItem item in lstVolReleases.SelectedItems)
                {
                    ChangeColor(item);
                    if(!vols.Contains((int)item.Tag))
                    {
                        Release rel = m.Releases.GetByID((int)item.Tag);
                        Volume vol = new Volume();
                        vol.Id = rel.Id;
                        vol.Status = (string)cbbStatus.SelectedItem;
                        vols.Add(vol);
                    }
                    else
                    {
                        Volume vol = vols.GetByID((int)item.Tag);
                        vol.Status = (string)cbbStatus.SelectedItem;
                        vols.Update(vol);
                    }
                }
                Cache.SaveVolumeInfos(vols, manga.Id);
                cbbStatus.SelectedIndex = -1;
                lstVolReleases.SelectedItems.Clear();
            }
        }

        public void ChangeColor(ListViewItem item)
        {
            switch (cbbStatus.SelectedValue)
            {
                case Status.owned:
                    item.Foreground = Brushes.GreenYellow;
                    break;
                case Status.otw:
                    item.Foreground = Brushes.Orange;
                    break;
                case Status.preorder:
                    item.Foreground = Brushes.DodgerBlue;
                    break;
                case Status.ordered:
                    item.Foreground = Brushes.Tomato;
                    break;
                default: item.Foreground = Brushes.White; break;
            }
        }

        public void ChangeColor(ListViewItem item, string status)
        {
            switch (status)
            {
                case Status.owned:
                    item.Foreground = Brushes.GreenYellow;
                    break;
                case Status.otw:
                    item.Foreground = Brushes.Orange;
                    break;
                case Status.preorder:
                    item.Foreground = Brushes.DodgerBlue;
                    break;
                case Status.ordered:
                    item.Foreground = Brushes.Tomato;
                    break;
                default: item.Foreground = Brushes.White; break;
            }
        }
    }
}
