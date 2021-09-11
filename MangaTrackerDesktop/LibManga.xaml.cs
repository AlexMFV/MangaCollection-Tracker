using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.ComponentModel;

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
        bool listLoading = false;
        bool noise = false;

        public LibManga(object _content, Frame _frame, FavManga _manga)
        {
            InitializeComponent();
            this.manga = _manga;
            this.frame = _frame;
            this.prevContent = _content;
            this.m = Cache.LoadSingleManga(manga.Id);

            vols = Cache.LoadVolumeInfos(this.manga.Id);

            LoadStatus();
            LoadComboBox();
            LoadMangaInfo();
            LoadReleases();
            LoadVolumes();
        }        
       
        public void LoadMangaInfo()
        {
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.frame.Content = this.prevContent;
        }

        public ObservableCollection<string> Steps
        {
            get;
            set;
        }

        public void LoadStatus()
        {
            Steps = new ObservableCollection<string>();
            Steps.Add(Status.preorder);
            Steps.Add(Status.ordered);
            Steps.Add(Status.otw);
            Steps.Add(Status.owned);
            this.DataContext = this;
        }

        public void LoadComboBox()
        {
            cbbStatus.Items.Clear();
            cbbStatus.Items.Add(Status.none);
            cbbStatus.Items.Add(Status.preorder);
            cbbStatus.Items.Add(Status.ordered);
            cbbStatus.Items.Add(Status.otw);
            cbbStatus.Items.Add(Status.owned);

            cbbStatus.SelectedIndex = 0;
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

        public void LoadVolumes()
        {
            if (lstVolPrices is not null)
            {
                listLoading = true;
                lstVolPrices.Items.Clear();
                foreach (Release rel in m.Releases)
                {
                    Volume vol = vols.GetByID(rel.Id);
                    if (vol is not null && vol.Status != Status.none)
                        AddVolumeItem(rel, vol);
                }
            }
            listLoading = false;
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

        public void AddVolumeItem(Release rel, Volume newVol)
        {
            ListViewItem item = new ListViewItem();
            item.Content = rel.Title;
            item.Tag = rel.Id;
            ChangeColor(item, newVol.Status);
            lstVolPrices.Items.Add(item);
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
            if (!noise)
            {
                if (cbbStatus.SelectedIndex != -1 && lstVolReleases.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem item in lstVolReleases.SelectedItems)
                    {
                        ChangeColor(item);
                        Release rel = m.Releases.GetByID((int)item.Tag);
                        if (!vols.Contains((int)item.Tag))
                        {
                            Volume vol = new Volume();
                            vol.Id = rel.Id;
                            vol.Status = (string)cbbStatus.SelectedItem;
                            vol.Type = GetVolumeType(rel);
                            vols.Add(vol);
                        }
                        else
                        {
                            Volume vol = vols.GetByID((int)item.Tag);
                            vol.Status = (string)cbbStatus.SelectedItem;
                            vol.Type = GetVolumeType(rel);
                            vols.Update(vol);
                        }
                    }

                    UpdateProgressBarStatus((string)cbbStatus.SelectedValue);

                    manga.Volumes_owned = vols.GetOwnedSCVolumes();

                    Cache.SaveVolumeInfos(vols, manga.Id);
                    Cache.SaveFavManga(manga);
                    //lstVolReleases.SelectedItems.Clear();
                    //cbbStatus.SelectedIndex = 0;
                }
            }
            
            noise = false;
        }

        public string GetVolumeType(Release rel)
        {
            if (rel.IsBS)
                return "BS";

            if (rel.IsGN)
                return "GN";

            if (rel.IsHC)
                return "HC";

            if (rel.IsOB)
                return "OB";

            return "Other";
        }

        public void UpdateProgressBarStatus(string status)
        {
            //ProgressBar
            switch (status)
            {
                case Status.preorder:
                    pbStatus.Progress = 20;
                    this.Resources["LineColor"] = Brushes.RoyalBlue; //DodgerBlue
                    this.Resources["BallColor"] = Brushes.RoyalBlue; //DodgerBlue
                    break;
                case Status.ordered:
                    pbStatus.Progress = 40;
                    this.Resources["LineColor"] = Brushes.Firebrick; //Tomato
                    this.Resources["BallColor"] = Brushes.Firebrick; //Tomato
                    break;
                case Status.otw:
                    pbStatus.Progress = 60;
                    this.Resources["LineColor"] = Brushes.DarkOrange; //Orange
                    this.Resources["BallColor"] = Brushes.DarkOrange; //Orange
                    break;
                case Status.owned:
                    pbStatus.Progress = 80;
                    this.Resources["LineColor"] = Brushes.LimeGreen; //GreenYellow
                    this.Resources["BallColor"] = Brushes.LimeGreen; //GreenYellow
                    break;
                default: pbStatus.Progress = 0;
                    this.Resources["LineColor"] = (LinearGradientBrush)FindResource("wizardBarBrush"); //Change to resources
                    this.Resources["BallColor"] = (LinearGradientBrush)FindResource("wizardBarBrush"); //Change to resources
                    break;
            }
        }

        public void ChangeColor(ListViewItem item)
        {
            switch (cbbStatus.SelectedValue)
            {
                case Status.preorder:
                    item.Foreground = Brushes.DodgerBlue;
                    break;
                case Status.ordered:
                    item.Foreground = Brushes.Tomato;
                    break;
                case Status.otw:
                    item.Foreground = Brushes.Orange;
                    break;
                case Status.owned:
                    item.Foreground = Brushes.GreenYellow;
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

        public void ChangeColor(Label item, string status)
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

        private void txt_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                lstVolPrices.Focus();
                ((MahApps.Metro.Controls.NumericUpDown)sender).Focus();
            }
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.OldValue == false && (bool)e.NewValue == true)
            {
                LoadVolumes();
                if (lstVolPrices.Items.Count > 0)
                    lstVolPrices.SelectedIndex = 0;
            }
        }

        private void lstVolPrices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!listLoading && lstVolPrices.SelectedItems.Count == 1)
            {
                Volume vol = vols.GetByID((int)((ListViewItem)lstVolPrices.SelectedItem).Tag);
                txtPrice.Value = vol.VolPrice;
                txtShip.Value = vol.ShipPrice;
                txtCosts.Value = vol.AddCosts;
                calBuy.DisplayDate = vol.BuyDate == DateTime.MinValue ? DateTime.Now : vol.BuyDate;
                calBuy.SelectedDate = calBuy.DisplayDate;
                calArrive.DisplayDate = vol.ArrivalDate == DateTime.MinValue ? DateTime.Now : vol.ArrivalDate;
                calArrive.SelectedDate = calArrive.DisplayDate;
                lblVolStatus.Content = vol.Status;
                ChangeColor((Label)lblVolStatus, vol.Status);
            }
        }

        private void lstVolReleases_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstVolReleases.SelectedItems.Count > 0)
            {
                if (lstVolReleases.SelectedItems.Count > 1)
                {
                    if (cbbStatus.SelectedIndex != -1)
                    {
                        noise = true;
                        cbbStatus.SelectedIndex = -1;
                    }
                }
                else
                {
                    Volume vol = vols.GetByID((int)((ListViewItem)lstVolReleases.SelectedItem).Tag);
                    if (vol != null)
                    {
                        UpdateProgressBarStatus(vol.Status);
                        if(cbbStatus.SelectedItem != vol.Status)
                        {
                            noise = true;
                            cbbStatus.SelectedItem = vol.Status;
                        }
                    }
                    else
                    {
                        UpdateProgressBarStatus(Status.none);
                        if(cbbStatus.SelectedIndex != 0)
                        {
                            noise = true;
                            cbbStatus.SelectedIndex = 0;
                        }
                    }
                }
            }
        }

        private void txt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (lstVolPrices.SelectedItems.Count > 0)
            {
                int idx = vols.GetIndex((int)((ListViewItem)lstVolPrices.SelectedItem).Tag);

                vols[idx].VolPrice = (double)txtPrice.Value;
                vols[idx].ShipPrice = (double)txtShip.Value;
                vols[idx].AddCosts = (double)txtCosts.Value;
                vols[idx].BuyDate = (DateTime)calBuy.SelectedDate;
                vols[idx].ArrivalDate = (DateTime)calArrive.SelectedDate;

                Cache.SaveVolumeInfos(vols, manga.Id);
            }
        }
    }
}
