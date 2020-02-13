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
    public partial class frmFormTemplet : Window
    {
        public frmFormTemplet()
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new UserTemplate().GetPrivForThisForm(this);
            PrepareForm();
        }
        private void PrepareForm()
        {
            btnUpdate.IsEnabled = false;
            btnSave.IsEnabled = true;
            lstListType.Visibility = Visibility.Hidden;
            FillListType();

        }
        private void FillListType()
        {
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtFill = cnn.GetDataTable("select pkid, list_name from LIST_TYPE");
            lstListType.ItemsSource = dtFill.DefaultView;
            lstListType.SelectedValuePath = "pkid";
            lstListType.DisplayMemberPath = "list_name";

            lstListType.SelectedIndex = -1;
        }
        private void btnFindTemplet_Click(object sender, RoutedEventArgs e)
        {
            frmFindForms frm = new frmFindForms();

            frm.ShowDialog();

            if (frm.strPkid.Trim() != "" && frm.strPkid != null)
            {
                GetData(frm.strPkid);
            }
        }

        private void lstControlType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // string text = (sender as ComboBox).SelectedItem as string;
            //or 
            if (lstControlType.SelectedIndex == -1)
                return;
            string text = (e.AddedItems[0] as ComboBoxItem).Content as string;
            // string text = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;

            if (text == "")
            {
                lstListType.Visibility = Visibility.Hidden;
                return;
            }

            lstListType.Visibility = Visibility.Hidden;

            if (text == "Free List" || text == "Free MultiChoices")
            {
                frmFreeList frm = new frmFreeList();
                frm.ShowDialog();
                txtFreeListId.Text = frm.strFreeListName;

                if (txtFreeListId.Text == "")
                    lstControlType.SelectedIndex = -1;
            }
            else if (text == "List" || text == "MultiChoices")
            {
                lstListType.Visibility = Visibility.Visible;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckDetailsEntries())
                return;


            clsGrid newRow = new clsGrid();
            newRow.clmSwid = "";
            newRow.clmFieldType = lstControlType.Text;
            newRow.clmFieldNameAr = txtArName.Text;
            newRow.clmFieldNameEn = txtEnName.Text;
            newRow.clmNote = txtPROPERITY_NOTE.Text;
            newRow.clmListTypeId = (lstListType.SelectedIndex == -1 ? "null" : lstListType.SelectedValue.ToString());
            newRow.clmListType = lstListType.Text;
            newRow.clmFreeList = txtFreeListId.Text;

            dgvProperities.Items.Add(newRow);



            txtArName.Text = "";
            txtEnName.Text = "";
            txtPROPERITY_NOTE.Text = "";
            lstListType.SelectedIndex = -1;
            lstListType.Text = "";
            txtFreeListId.Text = "";
        }
        class clsGrid
        {
            public string clmSwid { get; set; }
            public string clmFieldType { get; set; }
            public string clmFieldNameAr { get; set; }
            public string clmFieldNameEn { get; set; }
            public string clmNote { get; set; }
            public string clmListTypeId { get; set; }
            public string clmListType { get; set; }
            public string clmFreeList { get; set; }

        }
        private void btnDeleteProprity_Click(object sender, RoutedEventArgs e)
        {
            if (glb_function.MsgBox("هل تريد الحذف بالفعل؟", "", true) == false)
                return;

            if (glb_function.GetCellValue(ref dgvProperities, clmSwid.DisplayIndex, dgvProperities.SelectedIndex) == "")
            {
                dgvProperities.Items.Remove(dgvProperities.CurrentItem);
            }
            else
            {
                ConnectionToMySQL cnn = new ConnectionToMySQL();
                int icheck = cnn.TranDataToDB("delete from FORMs_de where pkid=" + glb_function.GetCellValue(ref dgvProperities, clmSwid.DisplayIndex, dgvProperities.SelectedIndex));
                if (icheck <= 0)
                {
                    glb_function.MsgBox("حدثت مشكلة اثناء الحذف");
                    return;
                }
                cnn.glb_commitTransaction();
                dgvProperities.Items.Remove(dgvProperities.CurrentItem);
            }
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            new glb_function().clearItems(this);
            PrepareForm();
        }
        private bool CheckEntries()
        {




            if (txtAR_NAME.Text == "")
            {

                glb_function.MsgBox("الرجاء ادخال اسم الشاشة بالعربي");
                txtAR_NAME.Focus();
                return false;
            }


            //txtFormInProgram
            if (txtEN_NAME.Text == "")
            {

                glb_function.MsgBox("الرجاء ادخال اسم الشاشة في البرنامج");
                txtEN_NAME.Focus();
                return false;
            }







            return true;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckEntries())
                return;

            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtTemplte = cnn.GetDataTable("select ifnull(max(b.pkid),0)+1 from FORMS_hd b");

            txtSwid.Text = dtTemplte.Rows[0][0].ToString();






            int icheck = cnn.TranDataToDB("insert into FORMS_hd values(" + txtSwid.Text + ",'" + txtAR_NAME.Text + "','" + txtEN_NAME.Text + "','" + (ckbMULTI_FORM_FOR_USER.IsChecked == true ? "1" : "0") + "','" + ((ListBoxItem)lstFormType.Items[lstFormType.Items.IndexOf(lstFormType.SelectedItem)]).Tag.ToString() + "','" + lstFormType.Text.Trim() + "')");
            if (icheck <= 0)
            {
                glb_function.MsgBox("حدث خطأ اثناء العملية");
                return;
            }




            for (int i = 0; i < dgvProperities.Items.Count; i++)
            {
                if (glb_function.GetCellValue(ref dgvProperities, clmSwid.DisplayIndex, i) == "")
                {


                    icheck = cnn.TranDataToDB("insert into FORMs_de values((select ifnull(max(b.pkid),0)+1 from FORMs_de b)," + txtSwid.Text + ",'" + glb_function.GetCellValue(ref dgvProperities, clmFieldType.DisplayIndex, i) + "','" + glb_function.GetCellValue(ref dgvProperities, clmFieldNameEn.DisplayIndex, i) + "','" + glb_function.GetCellValue(ref dgvProperities, clmFieldNameAr.DisplayIndex, i) + "'," + glb_function.GetCellValue(ref dgvProperities, clmListTypeId.DisplayIndex, i) + ",'" + glb_function.GetCellValue(ref dgvProperities, clmFreeList.DisplayIndex, i) + "','" + glb_function.GetCellValue(ref dgvProperities, clmNote.DisplayIndex, i) + "')");

                    if (icheck <= 0)
                    {
                        cnn.glb_RollbackTransaction();
                        glb_function.MsgBox("حدث خطأ اثناء العملية");
                        return;
                    }
                }
            }

            cnn.glb_commitTransaction();
            glb_function.MsgBox("تمت العملية بنجاح");
            GetData(txtSwid.Text.Trim());



        }
        private void GetData(string strPkid)
        {
            new glb_function().clearItems(this);
            PrepareForm();
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtLocation = cnn.GetDataTable("select * from forms_hd where pkid=" + strPkid);

            txtSwid.Text = strPkid;
            txtAR_NAME.Text = dtLocation.Rows[0]["AR_NAME"].ToString();


            txtEN_NAME.Text = dtLocation.Rows[0]["EN_NAME"].ToString();


            ckbMULTI_FORM_FOR_USER.IsChecked = (dtLocation.Rows[0]["MULTI_FORM_FOR_USER"].ToString() == "1" ? true : false);

            lstFormType.Text = dtLocation.Rows[0]["form_type_AR"].ToString();

            GetProperitiesData();

            btnSave.IsEnabled = false;
            btnUpdate.IsEnabled = true;
        }
        private void GetProperitiesData()
        {
            dgvProperities.Items.Clear();
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtPorp = cnn.GetDataTable("select pkid, control_type, en_name, ar_name, " +
                       "  list_id, (select l.list_name from list_type l where l.pkid = list_id) listtype, " +
                         " free_list_name, properity_note " +
                         " from forms_de " +
                        "  where header_id = " + txtSwid.Text);

            clsGrid newRow;
            for (int i = 0; i < dtPorp.Rows.Count; i++)
            {
                newRow = new clsGrid();

                newRow.clmSwid = dtPorp.Rows[i]["pkid"].ToString();
                newRow.clmFieldType = dtPorp.Rows[i]["control_type"].ToString();
                newRow.clmFieldNameEn = dtPorp.Rows[i]["en_name"].ToString();
                newRow.clmFieldNameAr = dtPorp.Rows[i]["ar_name"].ToString();
                newRow.clmNote = dtPorp.Rows[i]["properity_note"].ToString();
                newRow.clmListTypeId = dtPorp.Rows[i]["list_id"].ToString();
                newRow.clmListType = dtPorp.Rows[i]["listtype"].ToString();
                newRow.clmFreeList = dtPorp.Rows[i]["free_list_name"].ToString();

                dgvProperities.Items.Add(newRow);

            }
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckEntries())
                return;
            DataTable dtTemplte = new DataTable();
            ConnectionToMySQL cnn = new ConnectionToMySQL();







            int icheck = cnn.TranDataToDB("update FORMS_hd set AR_NAME='" + txtAR_NAME.Text + "',EN_NAME='" + txtEN_NAME.Text + "',MULTI_FORM_FOR_USER='" + (ckbMULTI_FORM_FOR_USER.IsChecked == true ? "1" : "0") + "',form_type_en='" + ((ListBoxItem)lstFormType.Items[lstFormType.Items.IndexOf(lstFormType.SelectedItem)]).Tag.ToString() + "',form_type_ar='" + lstFormType.Text.Trim() + "' where pkid=" + txtSwid.Text);
            if (icheck <= 0)
            {
                glb_function.MsgBox("حدث خطأ اثناء العملية");
                return;
            }
            for (int i = 0; i < dgvProperities.Items.Count; i++)
            {
                if (glb_function.GetCellValue(ref dgvProperities, clmSwid.DisplayIndex, i) == "")
                {


                    icheck = cnn.TranDataToDB("insert into FORMs_de values((select ifnull(max(b.pkid),0)+1 from FORMs_de b)," + txtSwid.Text + ",'" + glb_function.GetCellValue(ref dgvProperities, clmFieldType.DisplayIndex, i) + "','" + glb_function.GetCellValue(ref dgvProperities, clmFieldNameEn.DisplayIndex, i) + "','" + glb_function.GetCellValue(ref dgvProperities, clmFieldNameAr.DisplayIndex, i) + "'," + glb_function.GetCellValue(ref dgvProperities, clmListTypeId.DisplayIndex, i) + ",'" + glb_function.GetCellValue(ref dgvProperities, clmFreeList.DisplayIndex, i) + "','" + glb_function.GetCellValue(ref dgvProperities, clmNote.DisplayIndex, i) + "')");

                    if (icheck <= 0)
                    {
                        cnn.glb_RollbackTransaction();
                        glb_function.MsgBox("حدث خطأ اثناء العملية");
                        return;
                    }
                }
            }

            cnn.glb_commitTransaction();
            glb_function.MsgBox("تمت العملية بنجاح");



            GetData(txtSwid.Text.Trim());
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool CheckDetailsEntries()
        {




            if (lstControlType.Text == "")
            {

                glb_function.MsgBox("الرجاء اختيار نوع الحقل");
                lstControlType.Focus();
                return false;
            }


            if (lstControlType.Text == "List")
            {
                if (lstListType.Text == "")
                {
                    glb_function.MsgBox("الرجاء اختيار القائمة");
                    lstListType.Focus();
                    return false;
                }
            }
            else if (lstControlType.Text == "Free List")
            {

                if (txtFreeListId.Text == "")
                {
                    glb_function.MsgBox("الرجاء ادخال القائمة الحرة");
                    lstControlType.Focus();
                    return false;
                }
            }
            //txtArName
            if (txtArName.Text == "")
            {

                glb_function.MsgBox("الرجاء ادخال الاسم بالعربي");
                txtArName.Focus();
                return false;
            }

            //txtEnName
            if (txtEnName.Text == "")
            {

                glb_function.MsgBox("الرجاء ادخال الاسم بالانجليزي");
                txtEnName.Focus();
                return false;
            }




            return true;
        }
    }
}
