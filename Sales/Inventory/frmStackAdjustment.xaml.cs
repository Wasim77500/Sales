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
    public partial class frmStackAdjustment : Window
    {
        public frmStackAdjustment()
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

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private bool CheckEntries()
        {

            if(lstBranches.SelectedIndex==-1)
            {
                glb_function.MsgBox("الرجاء اختيار الفرع");
                lstBranches.Focus();
                return false;
            }
            if (lstWarehouses.SelectedIndex == -1)
            {
                glb_function.MsgBox("الرجاء اختيار المخزن");
                lstWarehouses.Focus();
                return false;
            }

            if(dgvStockAdjustment.Items.Count<=0)
            {
                glb_function.MsgBox("الرجاء ادخال صنف واحد على الاقل للتعديل");
                return false;
            }

            return true;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckEntries())
                return;

            ConnectionToMySQL cnnSave = new ConnectionToMySQL();
            DataTable dt = cnnSave.GetDataTable("(select ifnull(max(b.pkid),0)+1 from stockadjust_header b)");
            txtPkid.Text = dt.Rows[0][0].ToString();
            dt = cnnSave.GetDataTable("(select ifnull(max(b.stockadj_no),0)+1 from stockadjust_header b where branch_id="+lstBranches.SelectedValue.ToString()+" and warehosue_id="+lstWarehouses.SelectedValue.ToString()+")");
            txtStockAdjNo.Text = dt.Rows[0][0].ToString();
            int icheck = cnnSave.TranDataToDB("insert into stockadjust_header " +
                          " values(" + txtPkid.Text + "" +
                          ",'فعال'" +
                          ",SYSDATE() " +
                          "," + glb_function.glb_strUserId +
                          "," + txtStockAdjNo.Text +
                        "," + lstBranches.SelectedValue.ToString() +
                           "," + lstWarehouses.SelectedValue.ToString() +
                          ",'" + txtStockAdjNote.Text.Trim() + "'" +
                        
                         ")");

            if (icheck <= 0)
            {

                glb_function.MsgBox("حدث خطأ اثناء عملية حفظ بيانات التعديل المخزني");
                return;
            }

            for (int i = 0; i < dgvStockAdjustment.Items.Count; i++)
            {
                icheck = cnnSave.TranDataToDB("insert into stockadjust_details values ((select ifnull(max(b.pkid),0)+1 from stockadjust_details b)" +
                    "," + txtPkid.Text +
                    "," + glb_function.GetCellValue(ref dgvStockAdjustment,clmItemId.DisplayIndex,i) +
                     "," + glb_function.GetCellValue(ref dgvStockAdjustment, clmRequiredQty.DisplayIndex, i) +
                    ")");

                if (icheck <= 0)
                {
                    cnnSave.glb_RollbackTransaction();
                    glb_function.MsgBox("حدث خطأ اثناء عملية حفظ بيانات الصنف");
                    return;
                }

                icheck = cnnSave.TranDataToDB("insert into item_trans values ((select ifnull(max(b.pkid),0)+1 from item_trans b)" +
                   ",SYSDATE() " +
                   "," + glb_function.glb_strUserId +
                    "," + glb_function.GetCellValue(ref dgvStockAdjustment, clmItemId.DisplayIndex, i) +
                     "," + lstWarehouses.SelectedValue.ToString() +
                       "," + glb_function.GetCellValue(ref dgvStockAdjustment, clmRequiredQty.DisplayIndex, i) +
                   "," + txtPkid.Text +
                   ",'تعديل مخزني'"+
                   ",''"+
                  
                  
                   ")");

                if (icheck <= 0)
                {
                    cnnSave.glb_RollbackTransaction();
                    glb_function.MsgBox("حدث خطأ اثناء عملية حفظ بيانات حركة الاصناف");
                    return;
                }

            }



            cnnSave.glb_commitTransaction();
            glb_function.MsgBox("تمت العملية بنجاح");
            GetData(txtPkid.Text);
        }
        private void GetData(string strPkid)
        {
            btnSave.IsEnabled = false;
        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            new glb_function().clearItems(this);
            PrepareForm();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if(txtItemId.Text =="")
            {
                glb_function.MsgBox("الرجاء اختيار الصنف");
                return;
            }
            if(nmbQty.Value ==0)
            {
                glb_function.MsgBox("الرجاء ادخال الكمية");
                return;
            }
            if(nmbQty.Value + nmbQtyOnHand.Value <0)
            {
                glb_function.MsgBox("لايمكن ادخال كمية اقل من المخزون");
                return;
            }
            for (int i = 0; i < dgvStockAdjustment.Items.Count; i++)
            {
                if(txtItemId.Text ==glb_function.GetCellValue(ref dgvStockAdjustment ,clmItemId.DisplayIndex,i))
                {
                    glb_function.MsgBox("تمت ادخال هذا الصنف من قبل");
                    return;
                }
            }
            StockDetails newRow = new StockDetails();
            newRow.clmPKid = "";
            newRow.clmItemId = txtItemId.Text ;
            newRow.clmItemNo = txtItemNo.Text ;
            newRow.clmItemName = txtItemName.Text ;
            newRow.clmQtyOnHand =nmbQtyOnHand.Value.ToString();
            newRow.clmRequiredQty = nmbQty.Value.ToString();

            dgvStockAdjustment.Items.Add(newRow);

            txtItemId.Text = "";
            txtItemNo.Text = "";
            txtItemName.Text = "";
            nmbQty.Value = 0;
            nmbQtyOnHand.Value = 0;
        }
        class StockDetails
        {
            public string clmPKid { get; set; }
            public string clmItemId { get; set; }
            public string clmItemNo { get; set; }
            public string clmItemName { get; set; }
            public string clmQtyOnHand { get; set; }
            public string clmRequiredQty { get; set; }
        }
        private void btnFindItem_Click(object sender, RoutedEventArgs e)
        {
            if(lstWarehouses.SelectedIndex==-1)
            {
                glb_function.MsgBox("الرجاء اختيار المخزن");
                lstWarehouses.Focus();
                return;
            }
            Inventory.frmFindItem frm = new frmFindItem();
            frm.ShowDialog();
            if (frm.strPKid != "")
                GetItemData(frm.strPKid);
        }
        private void GetItemData(string strItemPkId)
        {
           

            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtItem = cnn.GetDataTable("select pkid,itemno,itemname," +
                "(select ifnull(sum(qty) ,0) from item_trans where item_id=i.pkid and warehouse_id="+lstWarehouses.SelectedValue.ToString()+") qty " +
                "from items i where pkid=" + strItemPkId);
            txtItemId.Text = strItemPkId;
            txtItemNo.Text = dtItem.Rows[0]["itemno"].ToString();
            txtItemName.Text = dtItem.Rows[0]["itemname"].ToString();
            nmbQtyOnHand.Value = Convert.ToDouble(dtItem.Rows[0]["qty"].ToString());




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



        }
        private void FillData()
        {
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtCurr = cnn.GetDataTable("SELECT pkid,branch_name FROM branches order by pkid");

            lstBranches.ItemsSource = dtCurr.DefaultView;
            lstBranches.SelectedValuePath = "pkid";
            lstBranches.DisplayMemberPath = "branch_name";

            


        }

        private void lstBranches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstBranches.SelectedIndex == -1)
                return;

            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtWarehouse = cnn.GetDataTable("SELECT pkid,warehouse_name FROM warehouse where branch_id="+lstBranches.SelectedValue.ToString());

            lstWarehouses.ItemsSource = dtWarehouse.DefaultView;
            lstWarehouses.SelectedValuePath = "pkid";
            lstWarehouses.DisplayMemberPath = "warehouse_name";


        }
    }
}
