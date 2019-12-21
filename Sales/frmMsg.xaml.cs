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

namespace Sales
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    public partial class frmMsg : Window
    {
        public frmMsg()
        {
            InitializeComponent();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
                this.Close();
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            glb_function.blMsg = true;
            this.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            glb_function.blMsg = false;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double ltop = lblMsg.Margin.Bottom + 10;

            if (stpOk.Visibility == Visibility.Visible)
            {
                this.Height = lblMsg.ActualHeight + stpOk.ActualHeight + 100;
                stpOk.Margin = new Thickness(stpOk.Margin.Left, ltop, stpOk.Margin.Right, stpOk.Margin.Bottom);
                btnOk.Focus();

            }

            else
            {
                this.Height = lblMsg.ActualHeight + stpYesNO.ActualHeight + 120;
                stpYesNO.Margin = new Thickness(stpYesNO.Margin.Left, ltop, stpYesNO.Margin.Right, stpYesNO.Margin.Bottom);

                btnYes.Focus();
            }


            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }
}
