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
using System.Windows.Shapes;
using System.Device.Location;
namespace Sales
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        private GeoCoordinateWatcher Watcher = null;
        public Window2()
        {
            InitializeComponent();
        }

        private void btnGetImage_Click(object sender, RoutedEventArgs e)
        {

            //BitmapImage bmpImage = new BitmapImage();
            ////Dim mapURL As String = "http://maps.googleapis.com/maps/api/staticmap?" & _
            ////            "size=500x400&markers=size:mid%7Ccolor:red%7C" & _
            ////            location & "&zoom=" & zoom & _
            ////            "&maptype=" & mapType & "&sensor=false"
            //string mapURL = "https://www.google.com/maps/place/%D8%AC%D8%A7%D9%85%D8%B9+%D8%A7%D9%84%D8%B5%D8%A7%D9%84%D8%AD+Al+Saleh+Mosque%E2%80%AD/@15.329556,44.202559,15.29z/data=!4m5!3m4!1s0x0:0xcfbf0a7488f8bdab!8m2!3d15.3256671!4d44.2080493";


            //bmpImage.BeginInit();
            //bmpImage.UriSource = new Uri(mapURL);
            //bmpImage.EndInit();

            //imgMap.Source = bmpImage;

            //this.UpdateLayout();
            //  System.Diagnostics.Process.Start("https://www.google.com/maps/place/%D8%AC%D8%A7%D9%85%D8%B9+%D8%A7%D9%84%D8%B5%D8%A7%D9%84%D8%AD+Al+Saleh+Mosque%E2%80%AD/@15.329556,44.202559,15.29z/data=!4m5!3m4!1s0x0:0xcfbf0a7488f8bdab!8m2!3d15.3256671!4d44.2080493");
            wbBrows.Source = new System.Uri("https://www.google.com/maps/place/%D8%AC%D8%A7%D9%85%D8%B9+%D8%A7%D9%84%D8%B5%D8%A7%D9%84%D8%AD+Al+Saleh+Mosque%E2%80%AD/@15.329556,44.202559,15.29z/data=!4m5!3m4!1s0x0:0xcfbf0a7488f8bdab!8m2!3d15.3256671!4d44.2080493");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

    }
}
