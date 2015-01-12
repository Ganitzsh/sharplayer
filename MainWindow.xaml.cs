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

namespace MediaPlayer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchMedia(object sender, TextChangedEventArgs e)
        {
            //((MainWindowViewModel)this.DataContext).SearchMedia(((TextBox)sender).Text); -> Du coup ça marche plus
            // Check if it can be done better...
         //   this.libraryList.ItemsSource = null;
           // this.libraryList.ItemsSource = ((MainWindowViewModel)this.DataContext).MediaPlayer.DisplayableMediaList;
        }
    }
}
