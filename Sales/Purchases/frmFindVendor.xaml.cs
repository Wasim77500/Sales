﻿using System;
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

namespace Sales.Purchases
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    public partial class frmFindVendor : Window
    {
        public string strPKid = "";
        public string strWhere = "";
        public frmFindVendor()
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
            System.Data.DataTable dtJournal = cnn.GetDataTable("select pkid, stat, created_date, created_user, "+
                                       " vendorno, vendorname, addess1, addess2, tel1, tel2, country, "+
                                       " acc_id, vendornote "+
                                       " from vendors " +
                                       " where concat(concat(concat(vendorno, concat(vendorname, concat(ifnull(addess1, ''), ifnull(addess2, '')))), tel1), tel2)  like '%" + txtSearchField .Text.Trim()+ "%' "+
                                         strWhere +
                                       "");

            ItemData newRow;
            for (int i = 0; i < dtJournal.Rows.Count; i++)
            {
                
                newRow = new ItemData();
                newRow.clmPKid = dtJournal.Rows[i]["pkid"].ToString();
                newRow.clmVendorNo = dtJournal.Rows[i]["vendorno"].ToString();
                newRow.clmVendorName = dtJournal.Rows[i]["vendorname"].ToString();
                newRow.clmAddress =  dtJournal.Rows[i]["addess1"].ToString();
                newRow.clmTel = dtJournal.Rows[i]["tel1"].ToString();
                newRow.clmVendorNote = dtJournal.Rows[i]["vendornote"].ToString();
               

                dgvJournalData.Items.Add(newRow);

            }

        }
        class ItemData
        {
          public   string clmPKid { get; set; }
            public string clmVendorNo { get; set; }
            public string clmVendorName{ get; set; }
            public string clmAddress { get; set; }
            public string clmTel { get; set; }
            public string clmVendorNote { get; set; }
           

        }

        private void dgvJournalData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnOk_Click(null, null);
        }
    }
}
