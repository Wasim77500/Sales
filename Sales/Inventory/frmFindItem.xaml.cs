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

namespace Sales.Inventory
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    public partial class frmFindItem : Window
    {
        public string strPKid = "";
        public string strWhere = "";
        public frmFindItem()
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
            strPKid = "";
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (dgvJournalData.SelectedItems.Count > 0)
            {
                TextBlock x = dgvJournalData.Columns[clmPKid.DisplayIndex].GetCellContent(dgvJournalData.Items[dgvJournalData.SelectedIndex]) as TextBlock;
                strPKid = x.Text.Trim();

                this.Close();
            }
            else
                strPKid = "";
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            dgvJournalData.Items.Clear();
            
          

             ConnectionToMySQL cnn = new ConnectionToMySQL();
            System.Data.DataTable dtJournal = cnn.GetDataTable("select pkid, stat, created_date, created_user, " +
                                       "  itemgroup, itemno, itemname, itemtype, " +
                                       "  cost_currency, cost_Value, cost_Value_yr, " +
                                       "  sellingPrice_currency, sellingPrice_value, sellingPrice_yr, sellingPrice_editable, " +
                                       "  itemnote, vendor_id  " +
                                       "  from items " +
                                       " where concat(concat(concat(itemgroup, concat(itemno, concat(ifnull(cost_currency, ''), ifnull(itemnote, '')))), itemtype), itemname)  like '%" + txtSearchField .Text.Trim()+ "%' "+
                                         strWhere +
                                       "");

            ItemData newRow;
            for (int i = 0; i < dtJournal.Rows.Count; i++)
            {
                
                newRow = new ItemData();
                newRow.clmPKid = dtJournal.Rows[i]["pkid"].ToString();
                newRow.clmItemGroup = dtJournal.Rows[i]["itemgroup"].ToString();
                newRow.clmItemNo = dtJournal.Rows[i]["itemno"].ToString();
                newRow.clmItemName =  dtJournal.Rows[i]["itemname"].ToString();
                newRow.clmItemType = dtJournal.Rows[i]["itemtype"].ToString();
                newRow.clmItemNote = dtJournal.Rows[i]["itemnote"].ToString();
               

                dgvJournalData.Items.Add(newRow);

            }

        }
        class ItemData
        {
          public   string clmPKid { get; set; }
            public string clmItemGroup { get; set; }
            public string clmItemNo { get; set; }
            public string clmItemName { get; set; }
            public string clmItemType { get; set; }
            public string clmItemNote { get; set; }
           

        }

        private void dgvJournalData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnOk_Click(null, null);
        }
    }
}
