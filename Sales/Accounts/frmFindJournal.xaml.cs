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

namespace Sales.Accounts
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    public partial class frmFindJournal : Window
    {
        public string strPKid = "";
        public string strWhere = "";
        public frmFindJournal()
        {
            InitializeComponent();
            dtpFrom.SelectedDate = DateTime.Now;
            dtpTo.SelectedDate = DateTime.Now;
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

        private void ckbSelectDate_Unchecked(object sender, RoutedEventArgs e)
        {
            gbDate.IsEnabled = false;
        }

        private void ckbSelectDate_Checked(object sender, RoutedEventArgs e)
        {
            gbDate.IsEnabled = true;
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
            string strCheckDate = "";
            if(ckbSelectDate.IsChecked==true )
                strCheckDate = " and jour_date between str_to_date('"+dtpFrom.SelectedDate.Value.ToString("dd/MM/yyyy")+ "', '%d/%m/%Y') and str_to_date('" + dtpTo.SelectedDate.Value.ToString("dd/MM/yyyy") + "', '%d/%m/%Y')";
            

             ConnectionToMySQL cnn = new ConnectionToMySQL();
            System.Data.DataTable dtJournal = cnn.GetDataTable("SELECT h.pkid,Branch_id, jour_no, trans_name, trans_id, date_format(jour_date,'%d/%m/%Y') jour_date, jour_note, Person, " +
                                       " curr_id, acc_id, main_value, jour_value, exchange_Rate, jour_details, "+
                                       " a.acc_no, a.acc_name "+
                                       "  FROM sales.journal_header h "+
                                       " join sales.journal_details d on(h.Pkid = d.header_id) "+
                                       " join sales.accounts a on(a.pkid = d.acc_id) "+
                                       " where concat(concat(concat(jour_no, concat(d.jour_details, concat(ifnull(jour_note, ''), ifnull(Person, '')))), a.acc_no), a.acc_name)  like '%"+ txtSearchField .Text.Trim()+ "%' "+
                                         strWhere + strCheckDate+
                                       "");

            JouranalData newRow;
            for (int i = 0; i < dtJournal.Rows.Count; i++)
            {
                
                newRow = new JouranalData();
                newRow.clmPKid = dtJournal.Rows[i]["pkid"].ToString();
                newRow.clmJourNo = dtJournal.Rows[i]["jour_no"].ToString();
                newRow.clmJourDate = dtJournal.Rows[i]["jour_date"].ToString();
                newRow.clmCurrValue =(Convert.ToDouble ( dtJournal.Rows[i]["jour_value"].ToString())>0 ? dtJournal.Rows[i]["jour_value"].ToString(): (Convert.ToDouble(dtJournal.Rows[i]["jour_value"].ToString())*-1).ToString());
                newRow.clmAccNo = dtJournal.Rows[i]["acc_no"].ToString();
                newRow.clmAccName = dtJournal.Rows[i]["acc_name"].ToString();
                newRow.clmJourNote = dtJournal.Rows[i]["jour_note"].ToString();

                dgvJournalData.Items.Add(newRow);

            }

        }
        class JouranalData
        {
          public   string clmPKid { get; set; }
            public string clmJourNo { get; set; }
            public string clmJourDate { get; set; }
            public string clmCurrValue { get; set; }
            public string clmAccNo { get; set; }
            public string clmAccName { get; set; }
            public string clmJourNote { get; set; }

        }

        private void dgvJournalData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnOk_Click(null, null);
        }
    }
}
