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
    /// Interaction logic for Library.xaml
    /// </summary>
    public partial class Library : Page
    {
        Frame frame;
        object prevContent;

        public Library(object _object, Frame _frame)
        {
            InitializeComponent();
            this.frame = _frame;
            this.prevContent = _object;

            if(Globals.FAVMANGAS_LIST.Count > 0)
                lstLibrary.ItemsSource = Globals.FAVMANGAS_LIST;
        }

        public ImageSource GetIMG(string url)
        {
            var image = new Image();

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(url, UriKind.Absolute);
            bitmap.EndInit();

            image.Source = bitmap;
            return image.Source;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.frame.Content = this.prevContent;
        }

        private void txtSearchLib_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtSearchLib.Text == "")
                {
                    Globals.FAVMANGAS_LIST = Globals.ALL_FAVMANGAS;
                    lstLibrary.ItemsSource = Globals.FAVMANGAS_LIST;
                }
                else
                {
                    Globals.FAVMANGAS_LIST = Globals.ALL_FAVMANGAS.ToObject(Globals.ALL_FAVMANGAS.ToList().Where(x => x.Title.ToLower().Contains(txtSearchLib.Text.ToLower())).ToList());
                    lstLibrary.ItemsSource = Globals.FAVMANGAS_LIST;
                }
            }
        }

        private void lstLibrary_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstLibrary.SelectedIndex != -1)
                this.frame.Content = new LibManga(this.frame.Content, this.frame, Globals.FAVMANGAS_LIST[lstLibrary.SelectedIndex]);
        }
    }
}
