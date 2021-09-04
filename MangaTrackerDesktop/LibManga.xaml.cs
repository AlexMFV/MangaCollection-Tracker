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

        public LibManga(object _content, Frame _frame, FavManga _manga)
        {
            InitializeComponent();
            this.manga = _manga;
            this.frame = _frame;
            this.prevContent = _content;

            PreLoadManga();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.frame.Content = this.prevContent;
        }

        public void PreLoadManga()
        {

        }
    }
}
