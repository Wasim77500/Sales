using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sales
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataTable dtUserForm;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (RibbonTab tab in FindLogicalChildren<RibbonTab>(this.RibbonWin))
            {
                tab.Visibility = Visibility.Hidden;
                foreach (RibbonButton button in FindLogicalChildren<RibbonButton>(tab))
                {

                    button.Visibility = Visibility.Hidden;

                    button.MinWidth = 100;

                    if (button.CommandParameter != null)
                        button.Label = button.CommandParameter.ToString();
                    if (button.Content != null)
                        button.LargeImageSource = new BitmapImage(new Uri(@"" + (button.Content.ToString()), UriKind.Relative));
                    button.Margin = new Thickness(5, 0, 0, 0);
                }

            }



            ConnectionToMySQL myconn = new ConnectionToMySQL();
          //  DataTable MyDataTable;

            new frmLogin().ShowDialog();

            if (glb_function.glb_strUserId == "")
                return;

            // glb_function.glb_strUserName = "وسيم الاغبري";


            if (glb_function.glb_strUserName == "وسيم الأغبري")
            {
                foreach (RibbonTab tab in FindLogicalChildren<RibbonTab>(this.RibbonWin))
                {
                    tab.Visibility = Visibility.Visible;
                    foreach (RibbonButton button in FindLogicalChildren<RibbonButton>(tab))
                    {

                        button.Visibility = Visibility.Visible;

                        button.MinWidth = 100;

                        if (button.CommandParameter != null)
                            button.Label = button.CommandParameter.ToString();
                        if (button.Content != null)
                            button.LargeImageSource = new BitmapImage(new Uri(@"" + (button.Content.ToString()), UriKind.Relative));
                        button.Margin = new Thickness(5, 0, 0, 0);
                    }

                }


                tabFile.IsSelected = true;
                tabFile.UpdateLayout();
                RibbonWin.UpdateLayout();
                return;
            }


            dtUserForm = myconn.GetDataTable("select u.pkid,m.form_arabic_name,m.form_eng_name,m.form_type_en from Sales.templet_header m,Sales.user_templet u " +
                                          "  where m.pkid = u.templet_id and u.user_id= " + glb_function.glb_strUserId);
            CreateToolIcons(this);




        }
        private void CreateToolIcons(Control myControl)
        {

            Ribbon rpt = new Ribbon();
            RibbonTab rr = new RibbonTab();
            glb_function checkpriv = new glb_function();
            rbLogout.IsEnabled = true;

            foreach (RibbonTab tab in FindLogicalChildren<RibbonTab>(this.RibbonWin))
            {
                tab.Visibility = Visibility.Visible;

                foreach (RibbonButton button in FindLogicalChildren<RibbonButton>(tab))
                {
                    if (button.Tag == null || button.Tag.ToString() == "")
                    {
                        button.Visibility = Visibility.Visible;
                        button.Tag = "";
                    }

                    else if (!IsUserHasThisForm(button.Tag.ToString().Trim()))
                    {
                        button.Visibility = Visibility.Hidden;
                        button.MinWidth = 0;
                        button.Width = 0;
                        button.Label = "";
                        button.LargeImageSource = null;
                        button.Margin = new Thickness(0);
                    }

                    else
                        button.Visibility = Visibility.Visible;

                }

            }
            tabFile.IsSelected = true;
            tabFile.UpdateLayout();
            RibbonWin.UpdateLayout();


         
        }

        private bool IsUserHasThisForm(string strFormName)
        {
            for (int i = 0; i < dtUserForm.Rows.Count; i++)
            {
                if (dtUserForm.Rows[i]["form_eng_name"].ToString() == strFormName)
                    return true;
            }
            return false;
        }
        public static IEnumerable<T> FindLogicalChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                foreach (object rawChild in LogicalTreeHelper.GetChildren(depObj))
                {
                    if (rawChild is DependencyObject)
                    {
                        DependencyObject child = (DependencyObject)rawChild;
                        if (child is T)
                        {
                            yield return (T)child;
                        }

                        foreach (T childOfChild in FindLogicalChildren<T>(child))
                        {
                            yield return childOfChild;
                        }
                    }
                }
            }
        }


        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {


            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
        private void rbDefineTemplet_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rbAddUser_Click(object sender, RoutedEventArgs e)
        {
          
            new frmUsers().ShowDialog();
        }

        private void rbDefineForms_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rbLogout_Click(object sender, RoutedEventArgs e)
        {
            Window_Loaded(null, null);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
