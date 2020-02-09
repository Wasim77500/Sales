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
    public partial class frmLogin : Window
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckEntries())
                return;

            ConnectionToMySQL cnn = new ConnectionToMySQL();
            System.Data.DataTable MyDataTable;
            MyDataTable = cnn.GetDataTable("SELECT pkid,UserLoginName,UserFullName,password,notes,branch_id FROM users Where userLoginEncry = '" + new glb_function().Encrypt(txtUsername.Text, true) + "' And Password = '" + new glb_function().Encrypt(txtPassword.Password, true) + "'");


            if (MyDataTable != null && MyDataTable.Rows.Count != 0)
            {


                glb_function.glb_strUserName = MyDataTable.Rows[0]["UserFullName"].ToString();
                glb_function.glb_strUserId = MyDataTable.Rows[0]["pkid"].ToString();
                glb_function.glb_strBranchPkid= MyDataTable.Rows[0]["branch_id"].ToString();



                this.Close();

            }
            else
            {
                // new glb_function().MsgBox("خطأ في اسم المستخدم او كلمة السر!", "تسجيل الدخول");
                glb_function.MsgBox("خطأ في اسم المستخدم او كلمة السر!", "تسجيل الدخول");
                txtPassword.Focus();
            }
        }
        private bool CheckEntries()
        {

            if (txtUsername.Text.Trim() == "")
            {
                glb_function.MsgBox("الرجاء ادخال اسم المستخدم", "رسالة خطأ");
                txtUsername.Focus();
                return false;
            }

            if (txtPassword.Password.Trim() == "")
            {
                glb_function.MsgBox("الرجاء ادخال كلمة السر", "رسالة خطأ");
                txtPassword.Focus();
                return false;
            }

            return true;


        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnLogin_Click(null, null);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtUsername.Focus();
        }

      

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            txtPassword.Focus();
        }
    }
}
