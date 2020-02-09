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

namespace Sales.Files
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    public partial class frmFreeList : Window
    {
        private string strtemp;
        public string strFreeListName;
        public frmFreeList()
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

        private void lstLIST_NAME_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //strFreeListName = "";
            //if (lstLIST_NAME.Text == null)
            //    return;

            if (lstLIST_NAME.SelectedIndex == -1)
                return;
            //string text = (e.AddedItems[0] as ComboBoxItem).Content as string;
            // string text= ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string; 
            //  string text = lstLIST_NAME.Text;
            string text = lstLIST_NAME.Items[lstLIST_NAME.Items.IndexOf(lstLIST_NAME.SelectedItem)].ToString();
            if (text != "")
            {

                if (strtemp == "")
                {
                    //strtemp = lstLIST_NAME.Text.ToString();
                    //new glb_function().clearItems(this);
                    //lstLIST_NAME.Text = strtemp;
                    dgvFreeList.Items.Clear();
                    txtDISPLAY_MEMBER.Text = "";
                    txtSwid.Text = "";
                    txtVALUE_MEMBER.Text = "";
                    GetData(text);
                    strtemp = "";

                }


            }


            strFreeListName = lstLIST_NAME.Text;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtVALUE_MEMBER.Text.Trim() == "")
            {
                glb_function.MsgBox("الرجاء ادخال رقم التعريف");
                txtVALUE_MEMBER.Focus();
                return;
            }


            if (txtDISPLAY_MEMBER.Text.Trim() == "")
            {
                glb_function.MsgBox("الرجاء ادخال القيمة");
                txtDISPLAY_MEMBER.Focus();
                return;
            }




            dgvFreeList.Items.Clear();
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtGetid = cnn.GetDataTable("select ifnull(max(b.pkid),0)+1 from FREE_LIST b");
            txtSwid.Text = dtGetid.Rows[0][0].ToString();
            int icheck = cnn.TranDataToDB("insert into FREE_LIST values(" + txtSwid.Text + ",'" + lstLIST_NAME.Text + "','" + txtDISPLAY_MEMBER.Text + "','" + txtVALUE_MEMBER.Text + "')");

            if (icheck <= 0)
            {
                glb_function.MsgBox("حدث خطأ اثناء عملية الاضافة");
                return;
            }
            cnn.glb_commitTransaction();
            GetData(lstLIST_NAME.Text);
            glb_function.MsgBox("تمت العملية بنجاح.");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            strtemp = "";
           
            new glb_function().clearItems(this);
            FillList();
        }
        private void GetData(string strListName)
        {

            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtGetFreeListData = cnn.GetDataTable("select pkid,list_name,display_member,value_member from FREE_LIST t" +
                            " where list_name = '" + strListName + "'");
            dgvFreeList.Items.Clear();
            lstLIST_NAME.Text = strListName;
            clsFreeList newRow;
            for (int i = 0; i < dtGetFreeListData.Rows.Count; i++)
            {
                newRow = new clsFreeList();

                newRow.clmSwid = dtGetFreeListData.Rows[i]["pkid"].ToString();
                newRow.clmFieldNo = dtGetFreeListData.Rows[i]["value_member"].ToString();
                newRow.clmFieldName = dtGetFreeListData.Rows[i]["display_member"].ToString();

                dgvFreeList.Items.Add(newRow);



            }


        }
        class clsFreeList
        {
            public string clmSwid { get; set; }
            public string clmFieldNo { get; set; }
            public string clmFieldName { get; set; }
        }
        private void FillList()
        {
            ConnectionToMySQL myconn = new ConnectionToMySQL();
            DataTable MyDataTable;
            MyDataTable = myconn.GetDataTable("Select distinct list_name From  free_list ");
            if (MyDataTable != null)
            {


                //lstLIST_NAME.ItemsSource = MyDataTable.DefaultView;


                //lstLIST_NAME.DisplayMemberPath = "list_name".ToUpper();

                for (int i = 0; i < MyDataTable.Rows.Count; i++)
                {
                    lstLIST_NAME.Items.Add(MyDataTable.Rows[i]["list_name"].ToString());
                    //  lstLIST_NAME.Items.Insert(0, "Copenhagen");
                }


                lstLIST_NAME.SelectedIndex = -1;

            }

            this.DataContext = this;

        }
    }
}
