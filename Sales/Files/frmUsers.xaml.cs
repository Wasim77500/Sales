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
    public partial class frmUsers : Window
    {
        public frmUsers()
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
            frmFindUser frm = new frmFindUser();
            frm.ShowDialog();

            if (frm.strPKid != "")
            {
                GetData(frm.strPKid);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if(txtPkid.Text .Trim()=="")
            {
                glb_function.MsgBox("الرجاء اختيار مستخدم");
                return;
            }

            if (txtUserName.Text.Trim() == "")
            {
                glb_function.MsgBox("الرجاء ادخال اسم المستخدم");
                txtUserName.Focus();
                return;
            }
            if (txtUserLogin.Text.Trim() == "")
            {
                glb_function.MsgBox("الرجاء ادخال رمز الدخول");
                txtUserLogin.Focus();
                return;
            }
            if (lstBranches.SelectedIndex == -1)
            {
                glb_function.MsgBox("الرجاء اختيار الفرع");
                lstBranches.Focus();
                return;
            }



            ConnectionToMySQL cnnSave = new ConnectionToMySQL();
          
            int icheck = cnnSave.TranDataToDB("update sales.users " +
                          " set UserLoginName='" + txtUserLogin.Text.Trim() + "',userLoginEncry='" + new glb_function().Encrypt(txtUserLogin.Text.Trim(), true) + "',UserFullName='" + txtUserName.Text.Trim() +
                          "',Notes='" + txtNote.Text.Trim() + "',branch_id=" + lstBranches.SelectedValue.ToString() + " where pkid="+txtPkid.Text );

            if (icheck <= 0)
            {

                glb_function.MsgBox("حدث خطأ اثناء عملية حفظ بيانات المستخدم");
                return;
            }


            icheck = cnnSave.TranDataToDB("delete from sales.USER_TEMPLET where user_id=" + txtPkid.Text);
            if (icheck < 0)
            {
                cnnSave.glb_RollbackTransaction();
                glb_function.MsgBox("حدث خطأ اثناء عملية حفظ بيانات المستخدم");
                return;
            }
            GetTempletToSave();

            while (strSelectedTemp != "")
            {
                string strTemp = strSelectedTemp.Substring(0, strSelectedTemp.IndexOf(';'));
                icheck = cnnSave.TranDataToDB("insert into sales.USER_TEMPLET values ((select ifnull(max(b.pkid),0)+1 from sales.USER_TEMPLET b), " + txtPkid.Text + "," + strTemp + ")");

                if (icheck <= 0)
                {
                    cnnSave.glb_RollbackTransaction();
                    glb_function.MsgBox("حدث خطأ اثناء عملية التعديل بيانات المستخدم");
                    return;
                }

                strSelectedTemp = strSelectedTemp.Substring(strSelectedTemp.IndexOf(';') + 1);
            }

            cnnSave.glb_commitTransaction();
            glb_function.MsgBox("تمت العملية بنجاح");

            UpdateHistory();

            GetData(txtPkid.Text);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtUserName.Text.Trim() == "")
            {
                glb_function.MsgBox("الرجاء ادخال اسم المستخدم");
                txtUserName.Focus();
                return;
            }
            if (txtUserLogin.Text.Trim() == "")
            {
                glb_function.MsgBox("الرجاء ادخال رمز الدخول");
                txtUserLogin.Focus();
                return;
            }
            if(lstBranches.SelectedIndex==-1)
            {
                glb_function.MsgBox("الرجاء اختيار الفرع");
                lstBranches.Focus();
                return;
            }

            ConnectionToMySQL cnnSave = new ConnectionToMySQL();
            DataTable dt = cnnSave.GetDataTable("(select ifnull(max(b.pkid),0)+1 from sales.users b)");
            txtPkid.Text = dt.Rows[0][0].ToString();
            int icheck = cnnSave.TranDataToDB("insert into sales.users " +
                          " values(" + txtPkid.Text + ",'فعال',SYSDATE() ," +glb_function.glb_strUserId+
                          ",'"+txtUserLogin.Text.Trim()+ "','" + new glb_function().Encrypt(txtUserLogin.Text.Trim(), true) + "','" + txtUserName.Text.Trim() +
                          "','" + new glb_function().Encrypt(txtPassword.Password.Trim(), true) + "','"+txtNote.Text .Trim()+"'," + lstBranches.SelectedValue.ToString()+ ")");

            if (icheck <= 0)
            {

                glb_function.MsgBox("حدث خطأ اثناء عملية حفظ بيانات المستخدم");
                return;
            }


            icheck = cnnSave.TranDataToDB("delete from sales.USER_TEMPLET where user_id=" + txtPkid.Text);
            if (icheck < 0)
            {
                cnnSave.glb_RollbackTransaction();
                glb_function.MsgBox("حدث خطأ اثناء عملية حفظ بيانات المستخدم");
                return;
            }
            GetTempletToSave();

            while (strSelectedTemp != "")
            {
                string strTemp = strSelectedTemp.Substring(0, strSelectedTemp.IndexOf(';'));
                icheck = cnnSave.TranDataToDB("insert into sales.USER_TEMPLET values ((select ifnull(max(b.pkid),0)+1 from sales.USER_TEMPLET b), " + txtPkid.Text + "," + strTemp + ")");

                if (icheck <= 0)
                {
                    cnnSave.glb_RollbackTransaction();
                    glb_function.MsgBox("حدث خطأ اثناء عملية حفظ بيانات المستخدم");
                    return;
                }

                strSelectedTemp = strSelectedTemp.Substring(strSelectedTemp.IndexOf(';') + 1);
            }

            cnnSave.glb_commitTransaction();
            glb_function.MsgBox("تمت العملية بنجاح");
            GetData(txtPkid.Text);
        }



        System.Data.DataTable dtUserData;
        private void GetData(string strId)
        {
            new glb_function().clearItems(this);
            tvPrivsTree_Loaded(null, null);
            FillBranches();
            ConnectionToMySQL cnn = new ConnectionToMySQL();
             dtUserData = cnn.GetDataTable("SELECT pkid,userloginname,userLoginEncry,UserFullName,notes,branch_id  FROM sales.users" +
                                   " where pkid='" + strId + "'");

            if (dtUserData != null && dtUserData.Rows.Count > 0)
            {
                txtPkid.Text = dtUserData.Rows[0]["pkid"].ToString();
              
                txtUserName.Text = dtUserData.Rows[0]["UserFullName"].ToString();
                
                txtUserLogin.Text = dtUserData.Rows[0]["userloginname"].ToString();

                txtNote.Text = dtUserData.Rows[0]["notes"].ToString();

                lstBranches.SelectedValue =Convert.ToInt32( dtUserData.Rows[0]["branch_id"].ToString());

                DataTable dtTemplet = cnn.GetDataTable("select * from sales.USER_TEMPLET where user_id=" + txtPkid.Text);

                for (int i = 0; i < dtTemplet.Rows.Count; i++)
                {
                    foreach (TreeViewItem item1 in tvPrivsTree.Items)
                        foreach (TreeViewItem item in item1.Items)
                        {

                            {
                                foreach (TreeViewItem subItem in item.Items)
                                {
                                    DependencyObject CP = subItem as DependencyObject;

                                    CheckBox subItemCkb = glb_function.FindVisualChild<CheckBox>(CP);


                                    if (subItemCkb.ToolTip.ToString() == dtTemplet.Rows[i]["templet_id"].ToString())
                                        subItemCkb.IsChecked = true;

                                }
                            }
                        }
                }




                if (UserTemplate.HasPrivilege("btnUpdate"))
                    btnUpdate.IsEnabled = true;

                
                
                btnSave.IsEnabled = false ;

            }
        }
        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            new glb_function().clearItems(this);
            tvPrivsTree_Loaded(null, null);
            PrepareForm();
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (txtPkid.Text.Trim() == "")
            {
                glb_function.MsgBox("الرجاء اختيار مستخدم");
                return;
            }

            glb_function.MsgBox("سيتم مسح عمل اعادة تعيين لكلمة السر" + "\n" + "كلمة السر الجديدة :" + "123");
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            int icheck = cnn.TranDataToDB("update sales.users set PASSWORD='" + new glb_function().Encrypt("123", true) + "'" +
                                            " where pkid=" + txtPkid.Text.Trim() + "");

            if (icheck >= 0)
            {
                cnn.glb_commitTransaction();
                glb_function.MsgBox("تمت عملية تغيير كلمة السر بنجاح");

            }
            else
                glb_function.MsgBox("حدث خطأ اثناء تغير كلمة السر");
        }

        private void tvPrivsTree_Loaded(object sender, RoutedEventArgs e)
        {
            FillTempletTree();
            tvPrivsTree.UpdateLayout();
            foreach (TreeViewItem item1 in tvPrivsTree.Items)
            {
                item1.IsExpanded = false;
                foreach (TreeViewItem item in item1.Items)
                    item.IsExpanded = false;
            }
        }
        private void FillTempletTree()
        {



            tvPrivsTree.Items.Clear();

            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtTemplet = cnn.GetDataTable("select distinct t.form_type_ar,t.form_type_en " +
                           " from sales.TEMPLET_HEADER t " +
                           " ");

            foreach (DataRow row in dtTemplet.Rows)
            {
                TreeViewItem Node = new TreeViewItem();
                Node.FontSize = 16;
                Node.FontWeight = FontWeights.Bold;
                Node.Header = row["form_type_ar"].ToString();
                Node.ToolTip = "Parent";
                Node.Tag = row["form_type_en"].ToString();
                tvPrivsTree.Items.Add(Node);
                AddFormNode((string)row["form_type_en"], Node);
            }


        }

        private void AddFormNode(string strform_type, TreeViewItem node)
        {
            node.IsExpanded = true;
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtTemplet = cnn.GetDataTable("select distinct t.form_arabic_name,t.form_eng_name " +
                           " from sales.TEMPLET_HEADER t " +
                           " where form_type_en = '" + strform_type + "'");

            foreach (DataRow row in dtTemplet.Rows)
            {
                TreeViewItem Node = new TreeViewItem();
                Node.FontSize = 16;
                Node.FontWeight = FontWeights.Bold;
                Node.Header = row["form_arabic_name"].ToString();
                Node.ToolTip = "Parent";
                Node.Tag = row["form_eng_name"].ToString();
                node.Items.Add(Node);
                AddChildNodes((string)row["form_eng_name"], Node);
            }


        }

        private void AddChildNodes(string strFormName, TreeViewItem node)
        {
            node.IsExpanded = true;
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtTemplet = cnn.GetDataTable("select pkid, templet_name,multi_form_for_user  " +
                       " from sales.TEMPLET_HEADER t " +
                       " where form_eng_name = '" + strFormName + "' ");

            foreach (DataRow row in dtTemplet.Rows)
            {
                TreeViewItem childNode = new TreeViewItem();
                childNode.Header = SetTreeView(row["templet_name"].ToString(), false, row["multi_form_for_user"].ToString(), strFormName, row["pkid"].ToString());//row["form_name"].ToString();


                //childNode.ShowCheckBox = true;
                childNode.Tag = row["pkid"].ToString();


                node.Items.Add(childNode);
            }


        }
        private StackPanel SetTreeView(string text, bool checkStat, string strMultForm, string strParentName, string strChildPkId)
        {


            //item.IsExpanded = false;

            // create stack panel
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;


            TextBlock lbl = new TextBlock();
            CheckBox ckb = new CheckBox();
            ckb.IsChecked = checkStat;
            ckb.VerticalAlignment = VerticalAlignment.Center;
            ckb.FlowDirection = FlowDirection.LeftToRight;
            ckb.Tag = strParentName;
            ckb.IsThreeState = (strMultForm == "1" ? true : false);
            ckb.ToolTip = strChildPkId;

            ckb.Checked += Ckb_Checked;


            lbl.Text = text;
            lbl.VerticalAlignment = VerticalAlignment.Center;
            lbl.Margin = new Thickness(5, 0, 0, 0);

            lbl.FontWeight = FontWeights.Bold;
            //lbl.FontStyle = FontStyles.Italic;
            lbl.FontSize = 16;

            // lbl.Foreground= new SolidColorBrush(Colors.Green); //هذه طريقة لتغيير لون الخط


            ////طريقة اخرى لتغيير لون الخط
            ////Color color = (Color)ColorConverter.ConvertFromString("#FFDFD991");
            //lbl.Foreground = new SolidColorBrush(color);
            // Add into stack

            stack.Children.Add(ckb);
            stack.Children.Add(lbl);

            // assign stack to header

            return stack;
        }

        private void Ckb_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox ckbTemp = ((CheckBox)sender);
            if ((ckbTemp.IsThreeState) == false)
            {
                foreach (TreeViewItem item1 in tvPrivsTree.Items)
                    foreach (TreeViewItem item in item1.Items)
                    {
                        if (item.ToolTip.ToString() == "Parent" && ckbTemp.Tag.ToString() == item.Tag.ToString())
                        {
                            foreach (TreeViewItem subItem in item.Items)
                            {
                                DependencyObject CP = subItem as DependencyObject;

                                CheckBox subItemCkb = glb_function.FindVisualChild<CheckBox>(CP);


                                if (ckbTemp.ToolTip.ToString() != subItemCkb.ToolTip.ToString())
                                    subItemCkb.IsChecked = false;

                            }
                        }
                    }
            }

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
            new UserTemplate().GetPrivForThisForm(this);
            PrepareForm();
        }
        private void PrepareForm()
        {
            if(UserTemplate.HasPrivilege("btnSave"))
              btnSave.IsEnabled = true;

            btnUpdate.IsEnabled = false;

            FillBranches();
        }

        private void FillBranches()
        {
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtBrnch = cnn.GetDataTable("SELECT pkid,Branch_name FROM sales.branches ");
            lstBranches.ItemsSource = dtBrnch.DefaultView;
            lstBranches.SelectedValuePath = "pkid";
            lstBranches.DisplayMemberPath = "Branch_name";
            
        }
        private void GetTempletToSave()
        {
            strSelectedTemp = "";
            foreach (TreeViewItem item in tvPrivsTree.Items)
            {
                if (item.HasItems == true)
                    SaveTemplet(item);
            }
        }

        string strSelectedTemp = "";
        private void SaveTemplet(TreeViewItem root)
        {

            foreach (TreeViewItem ctrl in root.Items)
            {
                if (ctrl.HasItems == true)
                    SaveTemplet(ctrl);
                else
                {

                    DependencyObject CP = ctrl as DependencyObject;

                    CheckBox subItemCkb = glb_function.FindVisualChild<CheckBox>(CP);
                    if (subItemCkb.IsChecked == true)
                        strSelectedTemp += subItemCkb.ToolTip.ToString() + ";";
                }

            }
        }

        private void UpdateHistory()
        {


           if( txtPkid.Text != dtUserData.Rows[0]["pkid"].ToString())
            {
                glb_function.MsgBox("الرجاء التأكد من عملية التحديث");
                return;
            }
            bool bCheckUpdateHistory = true;
            // insert into sales.USER_TEMPLET values((select ifnull(max(b.pkid),0)+1 from sales.USER_TEMPLET b),
            if (txtUserName.Text.Trim() != dtUserData.Rows[0]["UserFullName"].ToString().Trim())
                if (!glb_function.UpdateHistory("users", "UserFullName", txtPkid.Text, dtUserData.Rows[0]["UserFullName"].ToString().Trim(), txtUserName.Text.Trim(), "اسم المستخدم"))
                    bCheckUpdateHistory = false;



            if (txtUserLogin.Text.Trim() != dtUserData.Rows[0]["userloginname"].ToString().Trim())
                if (!glb_function.UpdateHistory("users", "userloginname", txtPkid.Text, dtUserData.Rows[0]["userloginname"].ToString().Trim(), txtUserLogin.Text.Trim(), "اسم الدخول"))
                    bCheckUpdateHistory = false;

           if(txtNote.Text.Trim() != dtUserData.Rows[0]["notes"].ToString())
                if (!glb_function.UpdateHistory("users", "notes", txtPkid.Text, dtUserData.Rows[0]["notes"].ToString().Trim(), txtNote.Text.Trim(), "ملاحظات"))
                    bCheckUpdateHistory = false;

           if( lstBranches.SelectedValue.ToString() !=dtUserData.Rows[0]["branch_id"].ToString())
                if (!glb_function.UpdateHistory("users", "branch_id", txtPkid.Text, dtUserData.Rows[0]["branch_id"].ToString().Trim(), lstBranches.SelectedValue.ToString().Trim(), "الفرع"))
                    bCheckUpdateHistory = false;



            if (bCheckUpdateHistory == false)
                glb_function.MsgBox("حدث خطأ عند ادخال التعديل الى بيانات التتبع");

        }
    }
}
