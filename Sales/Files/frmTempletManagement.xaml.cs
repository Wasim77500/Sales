using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class frmTempletManagement : Window
    {
        public frmTempletManagement()
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

        private void btnFindForm_Click(object sender, RoutedEventArgs e)
        {
            dgvForms.Items.Clear();
            dgProperities.Items.Clear();
            ConnectionToMySQL cnn = new ConnectionToMySQL();


            DataTable dtLocationData = cnn.GetDataTable("select pkid,ar_name,en_name,multi_form_for_user,form_type_ar,form_type_En from sales.forms_hd where ar_name like '%" +
                                 txtFormArName.Text + "%'  ");
            clsHeaderData newRow;
            for (int i = 0; i < dtLocationData.Rows.Count; i++)
            {
                newRow = new clsHeaderData();
                newRow.clmPkId = dtLocationData.Rows[i]["pkid"].ToString();
                newRow.clmFormArName = dtLocationData.Rows[i]["ar_name"].ToString();
                newRow.clmFormEnName = dtLocationData.Rows[i]["en_name"].ToString();
                newRow.clmMultiForm = dtLocationData.Rows[i]["multi_form_for_user"].ToString();
                newRow.clmFormTypeAr = dtLocationData.Rows[i]["form_type_ar"].ToString();
                newRow.clmFormTypeEn = dtLocationData.Rows[i]["form_type_En"].ToString();
                dgvForms.Items.Add(newRow);


            }
        }
        class clsHeaderData
        {
            public string clmPkId { get; set; }
            public string clmFormArName { get; set; }
            public string clmFormEnName { get; set; }
            public string clmMultiForm { get; set; }
            public string clmFormTypeAr { get; set; }
            public string clmFormTypeEn { get; set; }


        }
        private void btnShowTemp_Click(object sender, RoutedEventArgs e)
        {
            frmViewEditTemplet frm = new frmViewEditTemplet();
            frm.txtFormName.Text = glb_function.GetCellValue(ref dgvForms, clmFormArName.DisplayIndex, dgvForms.SelectedIndex);
            frm.txtFormEnName.Text = glb_function.GetCellValue(ref dgvForms, clmFormEnName.DisplayIndex, dgvForms.SelectedIndex);
            frm.txtFormId.Text = glb_function.GetCellValue(ref dgvForms, clmPkId.DisplayIndex, dgvForms.SelectedIndex);
            frm.txtForm_type.Text = glb_function.GetCellValue(ref dgvForms, clmFormTypeEn.DisplayIndex, dgvForms.SelectedIndex);
            frm.ShowDialog();
        }

        private void txtValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if (dgProperities.SelectedIndex == -1)
                return;
            DataGridRow row = dgProperities.ItemContainerGenerator.ContainerFromIndex(dgProperities.SelectedIndex) as DataGridRow;
            ContentPresenter CP = dgProperities.Columns[clmPropertyValue.DisplayIndex].GetCellContent(row) as ContentPresenter;
            TextBox DP = glb_function.FindVisualChild<TextBox>(CP);
            string strTest = DP.Text.Trim();
            DP.IsReadOnly = true;

            DP.Text = "";
            clsFormDetails UpdateRow = (clsFormDetails)dgProperities.Items[dgProperities.SelectedIndex];
            UpdateRow.clmPropertyValue = strTest;

            UpdateRow.clmRealValue = strTest;

            int iSelectedIndex = dgProperities.SelectedIndex;

            dgProperities.Items[iSelectedIndex] = UpdateRow;
            dgProperities.Items.Refresh();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckEntries())
                return;


            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtTemplte = cnn.GetDataTable("select ifnull(max(b.pkid),0)+1 from sales.TEMPLET_HEADER b");

            txtPkId.Text = dtTemplte.Rows[0][0].ToString();

            int icheck = cnn.TranDataToDB("insert into sales.TEMPLET_HEADER values(" + txtPkId.Text + "," +
                        glb_function.glb_strUserId + ",sysdate(),'" + txtTempletName.Text + "','" +
                        glb_function.GetCellValue(ref dgvForms, clmFormEnName.DisplayIndex, dgvForms.SelectedIndex) + "','" +
                        glb_function.GetCellValue(ref dgvForms, clmFormArName.DisplayIndex, dgvForms.SelectedIndex) + "','" +
                        glb_function.GetCellValue(ref dgvForms, clmMultiForm.DisplayIndex, dgvForms.SelectedIndex) + "','" +
                        glb_function.GetCellValue(ref dgvForms, clmFormTypeEn.DisplayIndex, dgvForms.SelectedIndex) + "','" +
                        glb_function.GetCellValue(ref dgvForms, clmFormTypeAr.DisplayIndex, dgvForms.SelectedIndex) + "')");
            if (icheck <= 0)
            {
                glb_function.MsgBox("حدث خطأ اثناء عملية الحفظ");
                return;
            }
            int iProSwid = 0;
            dtTemplte.Clear();
            dtTemplte = cnn.GetDataTable("select ifnull(max(b.pkid),0)+1 from sales.TEMPLET_DETAILS b");
            iProSwid = Convert.ToInt16(dtTemplte.Rows[0][0].ToString());

            for (int i = 0; i < dgProperities.Items.Count; i++)
            {



                icheck = cnn.TranDataToDB("insert into sales.TEMPLET_DETAILS values(" + iProSwid.ToString() +
                         "," + txtPkId.Text +
                         ",'" +
                        glb_function.GetCellValue(ref dgProperities, clmcontrol_type.DisplayIndex, i) + "','" +
                         glb_function.GetCellValue(ref dgProperities, clmen_name.DisplayIndex, i) + "','" +
                         glb_function.GetCellValue(ref dgProperities, clmPropertyName.DisplayIndex, i) + "','" +
                        glb_function.GetCellValueControl(ref dgProperities, "TextBox", clmPropertyValue.DisplayIndex, i) + "'," +
                         glb_function.glb_strUserId + ",sysdate(),'" +
                         (glb_function.GetCellValue(ref dgProperities, clmRealValue.DisplayIndex, i) == "" ? glb_function.GetCellValueControl(ref dgProperities, "TextBox", clmPropertyValue.DisplayIndex, i) : glb_function.GetCellValue(ref dgProperities, clmRealValue.DisplayIndex, i)) + "')");
                iProSwid++;

                if (icheck <= 0)
                {
                    cnn.glb_RollbackTransaction();
                    glb_function.MsgBox("حدث خطأ اثناء عملية الحفظ");
                    return;
                }

            }
            cnn.glb_commitTransaction();
            glb_function.MsgBox("تمت عملية الحفظ بنجاح");




            dgProperities.Items.Clear();
            txtTempletName.Text = "";
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dgvForms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvForms.Items.Count > 0)
                if (dgvForms.SelectedItems.Count > 0)
                {

                    //To get Selected Column in datagrid
                    DataGridCellInfo cellInfo = dgvForms.CurrentCell;
                    DataGridColumn column = cellInfo.Column;
                    //************************************
                    if (column.DisplayIndex == clmShow.DisplayIndex)
                    {

                        return;
                    }


                    string strHeaderPkid = glb_function.GetCellValue(ref dgvForms, clmPkId.DisplayIndex, dgvForms.SelectedIndex);

                    GetDetailData(strHeaderPkid);
                }
        }
        private void GetDetailData(string strHeaderPkid)
        {
            dgProperities.Items.Clear();
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtProp = cnn.GetDataTable("select pkid,control_type,en_name,ar_name,list_id,free_list_name from Sales.FORMs_de t " +
                      " where t.header_id = " + strHeaderPkid + " order by pkid");
            clsFormDetails newRow;

            for (int i = 0; i < dtProp.Rows.Count; i++)
            {
                newRow = new clsFormDetails();
                newRow.clmDetailPkId = dtProp.Rows[i]["pkid"].ToString();
                newRow.clmPropertyName = dtProp.Rows[i]["ar_name"].ToString();
                newRow.clmPropertyValue = "";
                newRow.clmcontrol_type = dtProp.Rows[i]["control_type"].ToString();
                newRow.clmlist_id = dtProp.Rows[i]["list_id"].ToString();
                newRow.clmfree_list_name = dtProp.Rows[i]["free_list_name"].ToString();
                newRow.clmen_name = dtProp.Rows[i]["en_name"].ToString();

                dgProperities.Items.Add(newRow);

            }

        }

        class clsFormDetails
        {
            public string clmDetailPkId { get; set; }
            public string clmPropertyName { get; set; }
            public string clmPropertyValue { get; set; }
            public string clmcontrol_type { get; set; }
            public string clmlist_id { get; set; }
            public string clmfree_list_name { get; set; }
            public string clmen_name { get; set; }
            public string clmRealValue { get; set; }
        }

        private void dgProperities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvForms.Items.Count > 0)
                if (dgProperities.SelectedIndex >= 0)
                {


                    if (glb_function.GetCellValue(ref dgProperities, clmcontrol_type.DisplayIndex, dgProperities.SelectedIndex) == "Enability")
                    {
                        frmlstEditor frm = new frmlstEditor();
                        frm.lstEditor.Items.Add("تفعيل");
                        frm.lstEditor.Items.Add("اغلاق");
                        frm.ShowDialog();




                        clsFormDetails UpdateRow = (clsFormDetails)dgProperities.Items[dgProperities.SelectedIndex];
                        UpdateRow.clmPropertyValue = frm.lstEditor.Text;
                        if (frm.lstEditor.Text == "تفعيل")
                            UpdateRow.clmRealValue = "1";
                        else
                            UpdateRow.clmRealValue = "0";
                        int iSelectedIndex = dgProperities.SelectedIndex;

                        dgProperities.Items[iSelectedIndex] = UpdateRow;
                        dgProperities.Items.Refresh();



                    }

                    else if (glb_function.GetCellValue(ref dgProperities, clmcontrol_type.DisplayIndex, dgProperities.SelectedIndex) == "Visability")
                    {
                        frmlstEditor frm = new frmlstEditor();
                        frm.lstEditor.Items.Add("اظهار");
                        frm.lstEditor.Items.Add("اخفاء");
                        frm.ShowDialog();




                        clsFormDetails UpdateRow = (clsFormDetails)dgProperities.Items[dgProperities.SelectedIndex];
                        UpdateRow.clmPropertyValue = frm.lstEditor.Text;
                        if (frm.lstEditor.Text == "اظهار")
                            UpdateRow.clmRealValue = "1";
                        else
                            UpdateRow.clmRealValue = "0";
                        int iSelectedIndex = dgProperities.SelectedIndex;

                        dgProperities.Items[iSelectedIndex] = UpdateRow;
                        dgProperities.Items.Refresh();



                    }
                    else if (glb_function.GetCellValue(ref dgProperities, clmcontrol_type.DisplayIndex, dgProperities.SelectedIndex) == "Text")
                    {
                        DataGridRow row = dgProperities.ItemContainerGenerator.ContainerFromIndex(dgProperities.SelectedIndex) as DataGridRow;
                        ContentPresenter CP = dgProperities.Columns[clmPropertyValue.DisplayIndex].GetCellContent(row) as ContentPresenter;
                        TextBox DP = glb_function.FindVisualChild<TextBox>(CP);
                        DP.IsReadOnly = false;


                    }
                    else if (glb_function.GetCellValue(ref dgProperities, clmcontrol_type.DisplayIndex, dgProperities.SelectedIndex) == "Free List")
                    {
                        frmlstEditor frm = new frmlstEditor();
                        ConnectionToMySQL cnn = new ConnectionToMySQL();
                        DataTable dtFreeList = cnn.GetDataTable("select display_member,value_member from Sales.free_list " +
                        " where list_name = '" + glb_function.GetCellValue(ref dgProperities, clmFreeListName.DisplayIndex, dgProperities.SelectedIndex) + "'");

                        frm.lstEditor.ItemsSource = dtFreeList.DefaultView;
                        frm.lstEditor.SelectedValuePath = "value_member".ToUpper();
                        frm.lstEditor.DisplayMemberPath = "display_member".ToUpper();
                        frm.ShowDialog();




                        clsFormDetails UpdateRow = (clsFormDetails)dgProperities.Items[dgProperities.SelectedIndex];
                        UpdateRow.clmPropertyValue = frm.lstEditor.Text;

                        UpdateRow.clmRealValue = frm.lstEditor.SelectedValue.ToString();

                        int iSelectedIndex = dgProperities.SelectedIndex;

                        dgProperities.Items[iSelectedIndex] = UpdateRow;
                        dgProperities.Items.Refresh();



                    }
                    else if (glb_function.GetCellValue(ref dgProperities, clmcontrol_type.DisplayIndex, dgProperities.SelectedIndex) == "List")
                    {
                        frmlstEditor frm = new frmlstEditor();
                        ConnectionToMySQL cnn = new ConnectionToMySQL();
                        DataTable dtListTable = cnn.GetDataTable("select  select_statement from Sales.LIST_TYPE t " +
                             " where pkid =  '" + glb_function.GetCellValue(ref dgProperities, clmListId.DisplayIndex, dgProperities.SelectedIndex) + "'");

                        DataTable dtGetList = cnn.GetDataTable(dtListTable.Rows[0][0].ToString());





                        frm.lstEditor.ItemsSource = dtGetList.DefaultView;
                        frm.lstEditor.SelectedValuePath = "pkid".ToUpper();
                        frm.lstEditor.DisplayMemberPath = "name".ToUpper();
                        frm.ShowDialog();




                        clsFormDetails UpdateRow = (clsFormDetails)dgProperities.Items[dgProperities.SelectedIndex];
                        UpdateRow.clmPropertyValue = frm.lstEditor.Text;

                        UpdateRow.clmRealValue = frm.lstEditor.SelectedValue.ToString();

                        int iSelectedIndex = dgProperities.SelectedIndex;

                        dgProperities.Items[iSelectedIndex] = UpdateRow;
                        dgProperities.Items.Refresh();



                    }
                    else if (glb_function.GetCellValue(ref dgProperities, clmcontrol_type.DisplayIndex, dgProperities.SelectedIndex) == "MultiChoices")
                    {
                        frmMultilstEditor frm = new frmMultilstEditor();
                        ConnectionToMySQL cnn = new ConnectionToMySQL();
                        DataTable dtListTable = cnn.GetDataTable("select  select_statement from Sales.LIST_TYPE t " +
                             " where pkid =  '" + glb_function.GetCellValue(ref dgProperities, clmListId.DisplayIndex, dgProperities.SelectedIndex) + "'");

                        DataTable dtGetList = cnn.GetDataTable(dtListTable.Rows[0][0].ToString());





                        //frm.lstEditor.ItemsSource = dtGetList.DefaultView;
                        //frm.lstEditor.SelectedValuePath = "pkid".ToUpper();
                        //frm.lstEditor.DisplayMemberPath = "name".ToUpper();
                        frm.myList = new ObservableCollection<frmMultilstEditor.MultiList>();

                        for (int i = 0; i < dtGetList.Rows.Count; i++)
                        {
                            string strText = dtGetList.Rows[i]["name"].ToString();
                            int iValue = Convert.ToUInt16(dtGetList.Rows[i]["pkid"].ToString());
                            frm.myList.Add(new frmMultilstEditor.MultiList { TheText = strText, TheValue = iValue });
                        }
                        frm.DataContext = frm;
                        frm.lstEditor.UpdateLayout();

                        if (glb_function.GetCellValue(ref dgProperities, clmRealValue.DisplayIndex, dgProperities.SelectedIndex) != "")
                        {
                            string strMult = glb_function.GetCellValue(ref dgProperities, clmRealValue.DisplayIndex, dgProperities.SelectedIndex);

                            string iindexx = "";

                            while (strMult != "")
                            {


                                iindexx = (strMult.Substring(0, strMult.IndexOf(";")));
                                strMult = strMult.Substring(strMult.IndexOf(";") + 1);

                                for (int i = 0; i < frm.lstEditor.Items.Count; i++)
                                {
                                    frmMultilstEditor.MultiList o = (frmMultilstEditor.MultiList)frm.lstEditor.Items[i];
                                    if (o.TheValue.ToString() == iindexx)
                                        o.IsSelected = true;
                                }


                            }
                        }


                        frm.ShowDialog();
                        string strPropertyValue = "";
                        string strPropRealValue = "";
                        for (int i = 0; i < frm.lstEditor.Items.Count; i++)
                        {
                            frmMultilstEditor.MultiList o = (frmMultilstEditor.MultiList)frm.lstEditor.Items[i];


                            if (o.IsSelected == true)
                            {

                                strPropertyValue += o.TheText + ";";
                                strPropRealValue += o.TheValue + ";";


                            }


                        }




                        clsFormDetails UpdateRow = (clsFormDetails)dgProperities.Items[dgProperities.SelectedIndex];
                        UpdateRow.clmPropertyValue = strPropertyValue;

                        UpdateRow.clmRealValue = strPropRealValue;

                        int iSelectedIndex = dgProperities.SelectedIndex;

                        dgProperities.Items[iSelectedIndex] = UpdateRow;
                        dgProperities.Items.Refresh();

                    }
                    else if (glb_function.GetCellValue(ref dgProperities, clmcontrol_type.DisplayIndex, dgProperities.SelectedIndex) == "Free MultiChoices")
                    {
                        frmMultilstEditor frm = new frmMultilstEditor();
                        ConnectionToMySQL cnn = new ConnectionToMySQL();
                        DataTable dtFreeList = cnn.GetDataTable("select display_member,value_member from Sales.free_list " +
                        " where list_name = '" + glb_function.GetCellValue(ref dgProperities, clmFreeListName.DisplayIndex, dgProperities.SelectedIndex) + "'");


                        frm.myList = new ObservableCollection<frmMultilstEditor.MultiList>();

                        for (int i = 0; i < dtFreeList.Rows.Count; i++)
                        {
                            string strText = dtFreeList.Rows[i]["name"].ToString();
                            int iValue = Convert.ToUInt16(dtFreeList.Rows[i]["pkid"].ToString());
                            frm.myList.Add(new frmMultilstEditor.MultiList { TheText = strText, TheValue = iValue });
                        }
                        frm.DataContext = frm;
                        frm.lstEditor.UpdateLayout();

                        if (glb_function.GetCellValue(ref dgProperities, clmRealValue.DisplayIndex, dgProperities.SelectedIndex) != "")
                        {
                            string strMult = glb_function.GetCellValue(ref dgProperities, clmRealValue.DisplayIndex, dgProperities.SelectedIndex);

                            string iindexx = "";

                            while (strMult != "")
                            {


                                iindexx = (strMult.Substring(0, strMult.IndexOf(";")));
                                strMult = strMult.Substring(strMult.IndexOf(";") + 1);

                                for (int i = 0; i < frm.lstEditor.Items.Count; i++)
                                {
                                    frmMultilstEditor.MultiList o = (frmMultilstEditor.MultiList)frm.lstEditor.Items[i];
                                    if (o.TheValue.ToString() == iindexx)
                                        o.IsSelected = true;
                                }


                            }
                        }


                        frm.ShowDialog();
                        string strPropertyValue = "";
                        string strPropRealValue = "";
                        for (int i = 0; i < frm.lstEditor.Items.Count; i++)
                        {
                            frmMultilstEditor.MultiList o = (frmMultilstEditor.MultiList)frm.lstEditor.Items[i];


                            if (o.IsSelected == true)
                            {

                                strPropertyValue += o.TheText + ";";
                                strPropRealValue += o.TheValue + ";";


                            }


                        }




                        clsFormDetails UpdateRow = (clsFormDetails)dgProperities.Items[dgProperities.SelectedIndex];
                        UpdateRow.clmPropertyValue = strPropertyValue;

                        UpdateRow.clmRealValue = strPropRealValue;

                        int iSelectedIndex = dgProperities.SelectedIndex;

                        dgProperities.Items[iSelectedIndex] = UpdateRow;
                        dgProperities.Items.Refresh();
                    }
                }
        }

        private bool CheckEntries()
        {



            int iError = 0;
            if (txtTempletName.Text.Trim() == "")
            {
                glb_function.MsgBox("الرجاء ادخال اسم النموذج");
                txtTempletName.Focus();
                return false;
            }




            for (int i = 0; i < dgProperities.Items.Count; i++)
            {
                if (glb_function.GetCellValueControl(ref dgProperities, "TextBox", clmPropertyValue.DisplayIndex, i) == "")
                {
                    glb_function.MsgBox("الرجاء اختيار قيمة لكل الخصائص");
                    return false;
                }
            }



            if (iError == 1)
                return false;

            return true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new UserTemplate().GetPrivForThisForm(this);
        }
    }
}
