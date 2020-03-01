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

namespace Sales.Inventory
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    public partial class frmItems : Window
    {
        public frmItems()
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

        private bool CheckEntries()
        {
            if (txtItemNo.Text.Trim() == "")
            {
                glb_function.MsgBox("الرجاء ادخال رقم الصنف");
                txtItemNo.Focus();
                return false;
            }
            if (txtItemName.Text.Trim() == "")
            {
                glb_function.MsgBox("الرجاء ادخال اسم الصنف");
                txtItemName.Focus();
                return false;
            }

            if (lstItemType.Text == "")
            {
                glb_function.MsgBox("الرجاء اختيار نوع الصنف");
                lstItemType.Focus();
                return false;
            }
            
            if(lstCostCurrency.SelectedIndex==-1)
            {
                glb_function.MsgBox("الرجاء اختيار عملة الشراء");
                lstCostCurrency.Focus();
                return false;
            }
            if (lstSellingPriceCurrency.SelectedIndex == -1)
            {
                glb_function.MsgBox("الرجاء اختيار عملة البيع");
                lstSellingPriceCurrency.Focus();
                return false;
            }



            return true;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckEntries())
                return;

            ConnectionToMySQL cnnSave = new ConnectionToMySQL();
            DataTable dt = cnnSave.GetDataTable("(select ifnull(max(b.pkid),0)+1 from Items b)");
            txtPkid.Text = dt.Rows[0][0].ToString();
            int icheck = cnnSave.TranDataToDB("insert into Items " +
                          " values(" + txtPkid.Text + "" +
                          ",'فعال'" +
                          ",SYSDATE() " +
                          "," + glb_function.glb_strUserId +
                          ",'" + lstItemGroup.Text.Trim() + "'" +
                          ",'" + txtItemNo.Text.Trim() + "'" +
                           ",'" + txtItemName.Text.Trim() + "'" +
                           ",'" + lstItemType.Text.Trim() + "'" +
                           "," + lstCostCurrency.SelectedValue.ToString() +
                            "," + nmbCostValue.Value.ToString() +
                            "," + nmbCostValue_yr.Value.ToString() +
                             "," + lstSellingPriceCurrency.SelectedValue.ToString() +
                             "," + nmbSellingPriceValue.Value.ToString() +
                             "," + nmbSellingPrice_yr.Value.ToString() +
                             "," + (ckbSellingPriceEditable.IsChecked ==true?"1":"0") +
                          ",'" + txtItemNote.Text.Trim() + "'" +
                            "," +(lstVendor.SelectedIndex==-1?"null": lstVendor.SelectedValue.ToString() )+
                         ")");

            if (icheck <= 0)
            {

                glb_function.MsgBox("حدث خطأ اثناء عملية حفظ بيانات الصنف");
                return;
            }





            cnnSave.glb_commitTransaction();
            glb_function.MsgBox("تمت العملية بنجاح");
            GetData(txtPkid.Text);

        }
        private void GetData(string strPk)
        {

            btnNew_Click(null, null);
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtWarehouse = cnn.GetDataTable("select pkid, stat, created_date, created_user, itemgroup, "+
                                        " itemno, itemname, itemtype, cost_currency, cost_Value, "+
                                        " cost_Value_yr, sellingPrice_currency, sellingPrice_value, "+
                                        " sellingPrice_yr, sellingPrice_editable, itemnote,vendor_id "+
                                        " from items " +
                                       "where pkid= " + strPk);


            txtPkid.Text = dtWarehouse.Rows[0]["pkid"].ToString();
            lstItemGroup.Text = dtWarehouse.Rows[0]["itemgroup"].ToString();
            txtItemNo.Text = dtWarehouse.Rows[0]["itemno"].ToString();
            txtItemName.Text = dtWarehouse.Rows[0]["itemname"].ToString();
            lstItemType.Text = dtWarehouse.Rows[0]["itemtype"].ToString();
            lstCostCurrency.SelectedValue = dtWarehouse.Rows[0]["cost_currency"].ToString();
            nmbCostValue.Value = Convert.ToDouble(dtWarehouse.Rows[0]["cost_Value"].ToString());
            nmbCostValue_yr.Value= Convert.ToDouble(dtWarehouse.Rows[0]["cost_Value_yr"].ToString());
            lstSellingPriceCurrency.SelectedValue= dtWarehouse.Rows[0]["sellingPrice_currency"].ToString();
            nmbSellingPriceValue.Value = Convert.ToDouble(dtWarehouse.Rows[0]["sellingPrice_value"].ToString());
            nmbSellingPrice_yr.Value = Convert.ToDouble(dtWarehouse.Rows[0]["sellingPrice_yr"].ToString());
            ckbSellingPriceEditable.IsChecked = (dtWarehouse.Rows[0]["sellingPrice_editable"].ToString() == "1" ? true : false);
            txtItemNote.Text = dtWarehouse.Rows[0]["itemnote"].ToString();
            lstVendor.SelectedValue = dtWarehouse.Rows[0]["vendor_id"].ToString();

            if (UserTemplate.HasPrivilege("btnUpdate"))
                btnUpdate.IsEnabled = true;

            btnSave.IsEnabled = false;

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
          
         
            int icheck = cnnSave.TranDataToDB("insert into Items set " +

                          " created_user=" + glb_function.glb_strUserId +
                          ",itemgroup='" + lstItemGroup.Text.Trim() + "'" +
                          ",itemno='" + txtItemNo.Text.Trim() + "'" +
                           ",itemname'" + txtItemName.Text.Trim() + "'" +
                           ",itemtype='" + lstItemType.Text.Trim() + "'" +
                           ",cost_currency=" + lstCostCurrency.SelectedValue.ToString() +
                            ",cost_Value=" + nmbCostValue.Value.ToString() +
                            ",cost_Value_yr=" + nmbCostValue_yr.Value.ToString() +
                             ",sellingPrice_currency=" + lstSellingPriceCurrency.SelectedValue.ToString() +
                             ",sellingPrice_value=" + nmbSellingPriceValue.Value.ToString() +
                             ",sellingPrice_yr=" + nmbSellingPrice_yr.Value.ToString() +
                             ",sellingPrice_editable=" + (ckbSellingPriceEditable.IsChecked == true ? "1" : "0") +
                          ",itemnote='" + txtItemNote.Text.Trim() + "'" +
                            ",vendor_id=" + (lstVendor.SelectedIndex == -1 ? "null" : lstVendor.SelectedValue.ToString()) +
                         " where pkid="+txtPkid.Text);

            if (icheck <= 0)
            {

                glb_function.MsgBox("حدث خطأ اثناء عملية تعديل بيانات الصنف");
                return;
            }





            cnnSave.glb_commitTransaction();
            glb_function.MsgBox("تمت العملية بنجاح");
            GetData(txtPkid.Text);

        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            new glb_function().clearItems(this);

            PrepareForm();
        }
        private void PrepareForm()
        {
            FillData();
            ckbSellingPriceEditable.IsChecked = false;
            if (UserTemplate.HasPrivilege("btnSave"))
                btnSave.IsEnabled = true;
            else
                btnSave.IsEnabled = false;

            btnUpdate.IsEnabled = false;
        }
        private void FillData()
        {
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtCurr = cnn.GetDataTable("SELECT pkid,curr_name FROM currency order by pkid");

            lstCostCurrency.ItemsSource = dtCurr.DefaultView;
            lstCostCurrency.SelectedValuePath = "pkid";
            lstCostCurrency.DisplayMemberPath = "curr_name";

            lstSellingPriceCurrency.ItemsSource = dtCurr.DefaultView;
            lstSellingPriceCurrency.SelectedValuePath = "pkid";
            lstSellingPriceCurrency.DisplayMemberPath = "curr_name";

           DataTable dtGroup= cnn.GetDataTable("SELECT distinct itemgroup FROM items");
            
            lstItemGroup.ItemsSource = dtGroup.DefaultView;

            lstItemGroup.DisplayMemberPath = "itemgroup";

            DataTable dtType = cnn.GetDataTable("SELECT distinct itemtype FROM items");

            lstItemType.ItemsSource = dtType.DefaultView;

            lstItemType.DisplayMemberPath = "itemtype";

            DataTable dtVendor = cnn.GetDataTable("SELECT pkid ,vendorname FROM vendors order by vendorname");

            lstVendor.ItemsSource = dtVendor.DefaultView;
            lstVendor.SelectedValuePath = "pkid";
            lstVendor.DisplayMemberPath = "vendorname";

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new UserTemplate().GetPrivForThisForm(this);
           

            PrepareForm();
        }

       

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            frmFindItem frm = new frmFindItem();
            frm.ShowDialog();
            if (frm.strPKid != "")
                GetData(frm.strPKid);
        }
    }
}
