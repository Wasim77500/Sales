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
    public partial class frmChangePassword : Window
    {
        public frmChangePassword()
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ConnectionToMySQL conn = new ConnectionToMySQL();
            System.Data.DataTable dtLogin = conn.GetDataTable("select pkid from sales.users " +
                " where UserFullName='" + glb_function.glb_strUserName.Trim() + "' and PASSWORD='" + new glb_function().Encrypt(txtOldPassword.Password.Trim(), true) + "'");

            if (dtLogin != null && dtLogin.Rows.Count != 0)
            {

                if (txtPassword.Password.Trim() != txtConfirmPass.Password.Trim())
                {
                    glb_function.MsgBox("كلمة السر غير متطابقة");
                    return;
                }

                int icheck = conn.TranDataToDB("update  sales.users set PASSWORD='" + new glb_function().Encrypt(txtPassword.Password.Trim(), true) + "' where pkid=" + glb_function.glb_strUserId);
                if (icheck <= 0)
                {

                    conn.glb_RollbackTransaction();
                    glb_function.MsgBox("حدث خطأ عند تغير كلمة السر");
                    return;
                }

                conn.glb_commitTransaction();
                glb_function.MsgBox("تمت عملية تغير كلمة السر بنجاح" + "\n" + "الرجاء تسجيل الدخول للتاكد");

                this.Close();
            }
            else
            {
                glb_function.MsgBox("الرجاء التاكد من كلمة السر السابقة", "رسالة نظام");
                return;
            }
        }
    }
}
