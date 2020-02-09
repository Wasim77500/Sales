using System;
using System.Collections.Generic;
using System.Data;
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

namespace Sales.Accounts
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    public partial class frmAccount : Window
    {
        string strAccShortNo = "";
        public frmAccount()
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckEntries())
                return;

            if (txtPkid.Text.Trim() == "")

                AddAccount();
            else
                UpdateAccount();

            
        }
        private bool CheckEntries()
        {
            
            if (txtAccName.Text.Trim() == "")
            {
                glb_function.MsgBox("الرجاء ادخال اسم الحساب");
                txtAccName.Focus();
                return false;
            }
           

            return true;
        }

        private void  AddAccount()
        {
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            int icheck = cnn.TranDataToDB("insert into accounts values(" +
                "(SELECT ifnull(max(b.pkid),0)+1 FROM accounts b)" +
                ",'فعال'" +
                ",sysdate()" +
                "," + glb_function.glb_strUserId+
                ",'"+ txtAccNo.Text + "'" +
                ",'" + txtAccName.Text.Trim() + "'" +
                ",'" + txtParentId.Text.Trim() + "'" +
                ",'" + txtAccNote.Text.Trim() + "'" +
                ",'" + txtLevel.Text.Trim() + "'" +
                ",'" + strAccShortNo.Trim() + "'" +
                ")");

            if(icheck <=0)
            {
                glb_function.MsgBox("حدث خطأ اثناء الإضافة");
                return;
            }

            cnn.glb_commitTransaction();
            glb_function.MsgBox("تمت عملية الإضافة بنجاح");

            txtAccNo.Text = "";
            txtAccName.Text = "";
            txtAccNote.Text = "";
            GetAccNo();
        }
        private void UpdateAccount()
        {
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            int icheck = cnn.TranDataToDB("update accounts set " +
                  " Acc_Name='" + txtAccName.Text.Trim() + "'" +
                  ",notes='" + txtAccNote.Text.Trim() + "'" +
                  " where pkid="+txtPkid.Text );

            if(icheck<=0)
            {
                glb_function.MsgBox("حدث خطأ اثناء عملية التعديل");
                return;

            }
            cnn.glb_commitTransaction();
            glb_function.MsgBox("تمت عملية التعديل بنجاح");





        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           if(txtPkid.Text .Trim()=="")
            {
                GetAccNo();

            }
          
           
        }

        private void GetAccNo()
        {
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtAcc = cnn.GetDataTable("SELECT ifnull(max(a.acc_short_no),0)+1 FROM accounts a where parent_id=" + txtParentId.Text);
            strAccShortNo = dtAcc.Rows[0][0].ToString();
            txtAccNo.Text = "";
            if (txtLevel.Text == "1")
            {
                txtAccNo.Text = strAccShortNo.PadLeft(2, '0');
            }
            else if (txtLevel.Text == "2")
            {
                txtAccNo.Text = txtParentAccNo.Text.Trim() + strAccShortNo.PadLeft(2, '0');
            }
            if (txtLevel.Text == "3")
            {
                txtAccNo.Text = txtParentAccNo.Text.Trim() + "-" + strAccShortNo.PadLeft(3, '0');
            }
        }
    }
}
