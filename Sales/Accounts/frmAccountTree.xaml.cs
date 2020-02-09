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
    public partial class frmAccountTree : Window
    {
        DataTable dtPrepareAccTree;
        public frmAccountTree()
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

        private void tvAccounts_Loaded(object sender, RoutedEventArgs e)
        {
            FillAccountTree();
        }
        private void FillAccountTree()
        {

            dtPrepareAccTree = new DataTable();
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            //dtPrepareAccTree = cnn.GetDataTable("select nvl(the_value,0) from DEFAULT_VALUES where VALUE_NAME='الرتبة'");

            // int iLevelNo =Convert.ToInt16(dtPrepareAccTree.Rows[0][0].ToString());
            dtPrepareAccTree.Rows.Clear();

            //dtPrepareAccTree = cnn.GetDataTable("select swid, created_date, stat, acc_no, acc_type, acc_name, acc_level, acc_class, acc_note, acc_code, nvl(acc_parent,0) as acc_parent, created_user, acc_subject,ACC_REPORT,ACC_NATURE  from ACCOUNTS t " +
                                  //  " order by acc_no ");
            dtPrepareAccTree = cnn.GetDataTable("SELECT pkid,stat,created_date,created_user,acc_no,acc_name,ifnull(parent_id,0) parent_id ,notes,level,Acc_short_no " +
                                " FROM sales.accounts order by acc_no");

            //tvAccTreeMain.Nodes.Clear();
          tvAccounts.Items.Clear();
            PopulateTreeViewMain(0, null);



        }
        private void PopulateTreeViewMain(int parentId, TreeViewItem parentNode)

        {


            TreeViewItem childNode;


            foreach (DataRow dr in dtPrepareAccTree.Select("[parent_id]=" + parentId))

            {


                TreeViewItem t = new TreeViewItem();
                t.Header = GetTreeView(dr["acc_no"].ToString() + " - " + dr["acc_name"].ToString());
                // t.Name = dr["acc_no"].ToString();

                //  t.Tag = dtPrepareAccTree.Rows.IndexOf(dr);
                t.Tag = dr["acc_name"].ToString();
                t.ToolTip = dr["pkid"].ToString();

                if (parentNode == null)
                {
                    //  tvAccTreeMain.Nodes.Add(t);
                    tvAccounts.Items.Add(t);
                    childNode = t;

                }
                else
                {

                    parentNode.Items.Add(t);

                    childNode = t;

                }

                PopulateTreeViewMain(Convert.ToInt32(dr["pkid"].ToString()), childNode);

            }

        }

        private StackPanel GetTreeView(string text)
        {


            //item.IsExpanded = false;

            // create stack panel
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;


            TextBlock lbl = new TextBlock();
            lbl.Text = text;
            lbl.VerticalAlignment = VerticalAlignment.Center;
            // lbl.Foreground= new SolidColorBrush(Colors.Green); //هذه طريقة لتغيير لون الخط


            ////طريقة اخرى لتغيير لون الخط
            ////Color color = (Color)ColorConverter.ConvertFromString("#FFDFD991");
            //lbl.Foreground = new SolidColorBrush(color);
            // Add into stack


            stack.Children.Add(lbl);

            // assign stack to header

            return stack;
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            frmAccount frm = new frmAccount();
            frm.txtPkid.Text = txtPkid.Text;
            frm.txtAccNo.Text = txtAccNo.Text;
            frm.txtAccName.Text = txtAccName.Text;
            frm.txtAccNote.Text = txtAccNote.Text;
            frm.txtLevel.Text = (Convert.ToInt32(txtLevel.Text)).ToString();
            frm.txtParentId.Text = txtParentId.Text;
            frm.txtParentAccName.Text = txtAccParentName.Text;
            frm.txtParentAccNo.Text = txtParentAccNo.Text;

            frm.ShowDialog();
            FillAccountTree();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            new glb_function().clearItems(this);
            PrepareForm();

            // ClearTreeSelection. ClearTreeViewSelection(tvAccounts);
            FillAccountTree();
        }
       

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            frmAccount frm = new frmAccount();

            frm.txtPkid.Text  ="";
            if(txtLevel.Text !="3")
            {
                frm.txtLevel.Text = (Convert.ToInt32(txtLevel.Text) + 1).ToString();
                frm.txtParentId.Text = txtPkid.Text;
                frm.txtParentAccName.Text = txtAccName.Text;
                frm.txtParentAccNo.Text = txtAccNo.Text;
            }
            else
            {
                frm.txtLevel.Text = (Convert.ToInt32(txtLevel.Text) ).ToString();
                frm.txtParentId.Text = txtParentId.Text;
                frm.txtParentAccName.Text = txtAccParentName.Text;
                frm.txtParentAccNo.Text = txtParentAccNo.Text;
            }
            

            frm.ShowDialog();
            btnNew_Click(null, null);

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void tvAccounts_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = (e.NewValue as TreeViewItem);
            if (item == null)
                return;

            string str = item.ToolTip.ToString();

            // txtAccName.Text = item.Tag.ToString();

            foreach (DataRow dr in dtPrepareAccTree.Select("[pkid]=" + str))

            {


                txtPkid.Text = str;
                txtAccNo.Text = dr["acc_no"].ToString();
                txtAccName.Text = dr["acc_name"].ToString();
                txtAccNote.Text = dr["notes"].ToString();
                txtParentId.Text = dr["parent_id"].ToString();
                txtLevel.Text = dr["level"].ToString();
                // dr["Acc_short_no"].ToString() 

                if(txtParentId.Text !="0")
                    foreach (DataRow drParent in dtPrepareAccTree.Select("[pkid]=" + txtParentId.Text))
                    {
                        txtParentAccNo.Text = drParent["acc_no"].ToString();
                        txtAccParentName.Text = drParent["acc_name"].ToString();
                    }



            }
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeViewItem item in tvAccounts.Items)
            {
                if (!(item.Tag.ToString().Contains(txtSearchAccName.Text.Trim())))
                {
                    item.Foreground = Brushes.Black;
                    RecurseItem(item, txtSearchAccName.Text.Trim());
                }

                else
                {
                    // MessageBox.Show("tr");
                    // item.Background  = Brushes.Red;
                    item.Foreground = Brushes.Red;
                    item.IsExpanded = true;
                    item.BringIntoView();
                    if (item.Items.Count > 0)
                        RecurseItem(item, txtSearchAccName.Text.Trim());
                }

            }
        }
        private void RecurseItem(TreeViewItem item, string strSearch)
        {
            foreach (TreeViewItem subItem in item.Items)
            {
                string strHeader = subItem.Tag.ToString();
                // compare header and return that item if you find it
                if (!(subItem.Tag.ToString().Contains(strSearch)))
                {
                    subItem.Foreground = Brushes.Black;
                    RecurseItem(subItem, strSearch);
                }

                else
                {
                    subItem.Foreground = Brushes.Red;



                    subItem.IsExpanded = true; ;
                    subItem.BringIntoView();
                    if (subItem.Items.Count > 0)
                        RecurseItem(subItem, strSearch);
                }

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PrepareForm();  
        }
        private void PrepareForm()
        {
            txtLevel.Text = "0";
            txtParentId.Text = "0";
        }
    }
}
