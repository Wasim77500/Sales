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

namespace Sales.Purchases
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    public partial class frmVendors : Window
    {
        public frmVendors()
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

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckEntries())
                return;

            ConnectionToMySQL cnnSave = new ConnectionToMySQL();
          
            int icheck = cnnSave.TranDataToDB("update vendors set " +
                        
                          " created_user=" + glb_function.glb_strUserId +
                          ",vendorno='" + txtVendorNo.Text.Trim() + "'" +
                          ",vendorname='" + txtVendorName.Text.Trim() + "'" +
                           ",addess1='" + txtAddress1.Text.Trim() + "'" +
                           ",addess2='" + txtAddress2.Text.Trim() + "'" +
                            ",tel1='" + txtTel1.Text.Trim() + "'" +
                           ",tel2='" + txtTel2.Text.Trim() + "'" +
                          ",country='" + txtCountry.Text.Trim() + "'" +
                          ",acc_id=" + (lstAccNo.SelectedIndex == -1 ? "null" : lstAccNo.SelectedValue.ToString()) +
                           ",vendornote'" + txtVendorNote.Text.Trim() + "'" +
                         " where pkid="+txtPkid.Text );

            if (icheck <= 0)
            {

                glb_function.MsgBox("حدث خطأ اثناء عملية تعديل بيانات المورد");
                return;
            }





            cnnSave.glb_commitTransaction();
            glb_function.MsgBox("تمت العملية بنجاح");
            GetData(txtPkid.Text);

        }
        private bool CheckEntries()
        {
            if (txtVendorNo.Text.Trim() == "")
            {
                glb_function.MsgBox("الرجاء ادخال رقم المورد");
                txtVendorNo.Focus();
                return false;
            }
            if (txtVendorName.Text.Trim() == "")
            {
                glb_function.MsgBox("الرجاء ادخال اسم المورد");
                txtVendorName.Focus();
                return false;
            }

          



            return true;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckEntries())
                return;

            ConnectionToMySQL cnnSave = new ConnectionToMySQL();
            DataTable dt = cnnSave.GetDataTable("(select ifnull(max(b.pkid),0)+1 from vendors b)");
            txtPkid.Text = dt.Rows[0][0].ToString();
            int icheck = cnnSave.TranDataToDB("insert into vendors " +
                          " values(" + txtPkid.Text + "" +
                          ",'فعال'" +
                          ",SYSDATE() " +
                          "," + glb_function.glb_strUserId +
                          ",'" + txtVendorNo.Text.Trim() + "'" +
                          ",'" + txtVendorName.Text.Trim() + "'" +
                           ",'" + txtAddress1.Text.Trim() + "'" +
                           ",'" + txtAddress2.Text.Trim() + "'" +
                            ",'" + txtTel1.Text.Trim() + "'" +
                           ",'" + txtTel2.Text.Trim() + "'" +
                          ",'" + txtCountry.Text.Trim() + "'" +                         
                          "," + (lstAccNo.SelectedIndex == -1 ? "null" : lstAccNo.SelectedValue.ToString()) +
                           ",'" + txtVendorNote.Text.Trim() + "'" +
                         ")");

            if (icheck <= 0)
            {

                glb_function.MsgBox("حدث خطأ اثناء عملية حفظ بيانات المورد");
                return;
            }





            cnnSave.glb_commitTransaction();
            glb_function.MsgBox("تمت العملية بنجاح");
            GetData(txtPkid.Text);
        }
        private void GetData(string strPkid)
        {
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtVendor = cnn.GetDataTable("select pkid, stat, created_date, created_user, "+
                               " vendorno, vendorname, addess1, addess2, tel1, tel2, country, "+
                                " acc_id, vendornote "+
                               " from vendors where pkid="+strPkid);

            txtPkid.Text = dtVendor.Rows[0]["pkid"].ToString();
            txtVendorNo.Text = dtVendor.Rows[0]["vendorno"].ToString();
            txtVendorName.Text = dtVendor.Rows[0]["vendorname"].ToString();
            txtAddress1.Text = dtVendor.Rows[0]["addess1"].ToString();
            txtAddress2.Text = dtVendor.Rows[0]["addess2"].ToString();
            txtTel1.Text = dtVendor.Rows[0]["tel1"].ToString();
            txtTel2.Text = dtVendor.Rows[0]["tel2"].ToString();
            txtCountry.Text = dtVendor.Rows[0]["country"].ToString();
            lstAccNo.SelectedValue = dtVendor.Rows[0]["acc_id"].ToString();
            txtVendorNote.Text = dtVendor.Rows[0]["vendornote"].ToString();

            if (UserTemplate.HasPrivilege("btnUpdate"))
                btnUpdate.IsEnabled = true;

            btnSave.IsEnabled = false;

        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            new glb_function().clearItems(this);

            PrepareForm();
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            frmFindVendor frm = new frmFindVendor();
            frm.ShowDialog();
            if (frm.strPKid != "")
                GetData(frm.strPKid);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new UserTemplate().GetPrivForThisForm(this);


            PrepareForm();
        }
        private void PrepareForm()
        {
            FillData();
            if (UserTemplate.HasPrivilege("btnSave"))
                btnSave.IsEnabled = true;
            else
                btnSave.IsEnabled = false;

            btnUpdate.IsEnabled = false;
        }
        private void FillData()
        {
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtVendor = cnn.GetDataTable("SELECT pkid,acc_no "+
                       " FROM accounts "+
                       " where level = 3 and parent_id in (select pkid from accounts where acc_name = 'الموردين') ");

            lstAccNo.ItemsSource = dtVendor.DefaultView;
            lstAccNo.SelectedValuePath = "pkid";
            lstAccNo.DisplayMemberPath = "acc_no";

        }

    }
}
