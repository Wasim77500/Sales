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

namespace Sales.Files
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    public partial class frmFindUser : Window
    {
        public string strPKid;
        public frmFindUser()
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

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            GetUsersData();
        }
        class User
        {
            public string clmSwid { get; set; }
            public string clmUserLogin { get; set; }
            public string clmUserName { get; set; }


        }
        private void GetUsersData()
        {
            dgvUser.Items.Clear();
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            System.Data.DataTable dtUsers = cnn.GetDataTable("select pkid,UserFullName,UserLoginName from sales.users " +
                                " where UserLoginName like '%" + txtUserLogin.Text + "%' and UserFullName like '%" + txtUserName.Text + "%' order  by UserFullName");

            List<User> lst = new List<User>();
            for (int i = 0; i < dtUsers.Rows.Count; i++)
            {


                User NewUser = new User();
                NewUser.clmSwid = dtUsers.Rows[i]["pkid"].ToString();
                NewUser.clmUserName = dtUsers.Rows[i]["UserFullName"].ToString();
                NewUser.clmUserLogin = dtUsers.Rows[i]["UserLoginName"].ToString();

                dgvUser.Items.Add(NewUser);


                //First Way  //  dgvUser.Items.Add(new {clmSwid= dtUsers.Rows[i]["USERID"].ToString(), clmUserName= dtUsers.Rows[i]["USERNAME"].ToString() , clmUserLogin= dtUsers.Rows[i]["LOGIN_NAME"].ToString() });
                //2.second
                // 2. lst.Add(new User { clmSwid=dtUsers.Rows[i]["USERID"].ToString(), clmUserName= dtUsers.Rows[i]["USERNAME"].ToString(), clmUserLogin= dtUsers.Rows[i]["LOGIN_NAME"].ToString() });
            }
            //2.dgvUser.ItemsSource = lst;

        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (dgvUser.SelectedItems.Count > 0)
            {
                TextBlock x = dgvUser.Columns[clmSwid.DisplayIndex].GetCellContent(dgvUser.Items[dgvUser.SelectedIndex]) as TextBlock;
                strPKid = x.Text.Trim();

                this.Close();
            }
            else
                strPKid = "";
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            strPKid = "";
            this.Close();
        }

        private void dgvUser_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnOk_Click(null, null);
        }
    }
}
