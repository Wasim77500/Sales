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
    public partial class frmJournal : Window
    {
        bool bLoad = false;
        public frmJournal()
        {
            bLoad = true;
            InitializeComponent();
            FillData();
            dtpJourDate.SelectedDate = DateTime.Now;
            bLoad = false;
        }
        private void FillData()
        {
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtCurr = cnn.GetDataTable("SELECT pkid,curr_name FROM currency order by pkid");
            lstCurrency.ItemsSource = dtCurr.DefaultView;
            lstCurrency.SelectedValuePath = "pkid";
            lstCurrency.DisplayMemberPath = "curr_name";





            DataTable dtAcc = cnn.GetDataTable("SELECT pkid,acc_no,acc_name FROM accounts where level=3 order by acc_no");
            lstAccNo.ItemsSource = dtAcc.DefaultView;
            lstAccNo.SelectedValuePath = "pkid";
            lstAccNo.DisplayMemberPath = "acc_no";

            lstAccName.ItemsSource = dtAcc.DefaultView;
            lstAccName.SelectedValuePath = "pkid";
            lstAccName.DisplayMemberPath = "acc_name";
            lstAccNo.SelectedIndex = -1;
            lstAccName.SelectedIndex = -1;
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if(lstCurrency.SelectedIndex ==-1)
            {
                glb_function.MsgBox("الرجاء اختيار العملة");
                lstCurrency.Focus();
                return;
            }
            if (lstAccNo.SelectedIndex == -1)
            {
                glb_function.MsgBox("الرجاء تحديد الحساب");
                lstAccNo.Focus();
                return;
            }
            if (lstAccName.SelectedIndex == -1)
            {
                glb_function.MsgBox("الرجاء تحديد الحساب");
                lstAccName.Focus();
                return;
            }
            if (nmbCurrValue.Value <=0)
            {
                glb_function.MsgBox("الرجاء ادخال المبلغ");
                
                return;
            }

            if (txtCreditTotal.Text.Trim() == "")
                txtCreditTotal.Text = "0";
            if (txtDeptTotal.Text.Trim() == "")
                txtDeptTotal.Text = "0";

            clsJourDetail newRow = new clsJourDetail();
            newRow.clmPKid = "";
            newRow.clmDept = nmbDept.Value.ToString() ;
            newRow.clmCredit = nmbCredit.Value .ToString();
            newRow.clmCurrValue = nmbCurrValue.Value.ToString();
            newRow.clmCurrencyId = lstCurrency.SelectedValue.ToString();
            newRow.clmCurrencyName = lstCurrency.Text ;
            newRow.clmAccId = lstAccName.SelectedValue.ToString() ;
            newRow.clmAccNo =lstAccNo.Text ;
            newRow.clmAccName = lstAccName.Text ;
            newRow.clmJourNote = txtJourDetail.Text.Trim();
            newRow.clmExchangeRate = nmbExchangeRate.Value.ToString();
            dgvJourDetails.Items.Add(newRow);

           

            txtCreditTotal.Text = (Convert.ToDouble(txtCreditTotal.Text.Trim()) + Convert.ToDouble(newRow.clmCredit)).ToString();
            txtDeptTotal.Text = (Convert.ToDouble(txtDeptTotal.Text.Trim()) + Convert.ToDouble(newRow.clmDept)).ToString();

            if (txtCreditTotal.Text.Trim() == txtDeptTotal.Text.Trim())
            {

                txtDeptTotal.Background = Brushes.LawnGreen;
                txtCreditTotal.Background = Brushes.LawnGreen;
            }
            else
            {
                txtDeptTotal.Background = Brushes.Red;
                txtCreditTotal.Background = Brushes.Red;
            }
          
            ClearDetail();

        }
      
        private void ClearDetail()
        {
           
            nmbCurrValue.Value = 0;
            lstAccNo.SelectedIndex = -1;
            lstAccName.SelectedIndex = -1;
            txtJourDetail.Text = "";
            nmbDept.IsEnabled = true;
            nmbCredit.IsEnabled = true;

        }
        class clsJourDetail
        {
            public string clmPKid { get; set; }
            public string clmDept { get; set; }
            public string clmCredit { get; set; }
            public string clmCurrValue { get; set; }
            public string clmCurrencyId { get; set; }
            public string clmCurrencyName { get; set; }
            public string clmAccId { get; set; }
            public string clmAccNo { get; set; }
            public string clmAccName { get; set; }
            public string clmJourNote { get; set; }
            public string clmExchangeRate { get; set; }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lstAccNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            if (lstAccNo.SelectedValue == null || lstAccNo.SelectedIndex==-1)
                return;
            

             lstAccName.SelectedValue = lstAccNo.SelectedValue;
           
        }

        private void lstAccName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstAccName.SelectedValue == null || lstAccName.SelectedIndex == -1)
                return;


            lstAccNo.SelectedValue = lstAccName.SelectedValue;
        }

        private void lstAccNo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (lstAccNo.SelectedIndex == -1)
            {
                lstAccNo.Text = "";
                lstAccName.Text = "";
            }
              
        }

        private void lstAccName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (lstAccName.SelectedIndex == -1)
            {
                lstAccNo.Text = "";
                lstAccName.Text = "";
            }
        }
        int iDecimalPlace = 0;
        private void lstCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            nmbCredit.Value = 0;
            nmbDept.Value = 0;
            nmbCurrValue.Value = 0;
            iDecimalPlace = 0;
            nmbExchangeRate.Value = 1;
            if (lstCurrency.SelectedIndex == -1)
                return;

            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtCurr = cnn.GetDataTable("SELECT pkid,curr_smpl,curr_decimal,CURR_CHANGE_Rate FROM currency where pkid="+lstCurrency.SelectedValue.ToString());

            nmbExchangeRate.Value = Convert.ToDouble(dtCurr.Rows[0]["CURR_CHANGE_Rate"].ToString());
            iDecimalPlace= Convert.ToInt32(dtCurr.Rows[0]["curr_decimal"].ToString());
        }

      

        private void nmbDept_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (bLoad)
                return;

            double d =(double) (nmbDept.Value / nmbExchangeRate.Value);
            nmbCredit.IsEnabled = false;
            nmbCurrValue.Value = Math.Round(d, iDecimalPlace);
        }

        private void nmbCredit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (bLoad)
                return;

            double d = (double)(nmbCredit.Value / nmbExchangeRate.Value);
            nmbDept.IsEnabled = false;
            nmbCurrValue.Value = Math.Round(d, iDecimalPlace);
        }

        private void nmbCurrValue_LostFocus(object sender, RoutedEventArgs e)
        {
          
        }

        private void nmbCurrValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
          
            if(nmbDept.IsEnabled ==true)
            {
                nmbDept.Value = nmbCurrValue.Value * nmbExchangeRate.Value;

                if (nmbDept.Value > 0)
                    nmbCredit.IsEnabled = false;
                else
                    nmbCredit.IsEnabled = true;
            }
            else
            {
                nmbCredit.Value = nmbCurrValue.Value * nmbExchangeRate.Value;
                if (nmbCredit.Value > 0)
                    nmbDept.IsEnabled = false;
                else
                    nmbDept.IsEnabled = true;
            }
                
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(dgvJourDetails.Items.Count<=0)
            {
                glb_function.MsgBox("الرجاء ادخال البيانات التفصيلية للقيد");
                return;
            }
            if(txtDeptTotal.Text.Trim()!=txtCreditTotal.Text .Trim())
            {
                glb_function.MsgBox("مبلغ المدين لا يساوي الدائن");
                return;
            }

            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtKeys = cnn.GetDataTable("select (select  ifnull(max( convert(  substring(jour_no,instr(jour_no,'-')+1),signed)),0)+1 FROM sales.journal_header where Branch_id="+glb_function.glb_strBranchPkid+" and trans_name='سند قيد') IssueNo,(select ifnull(max(pkid),0)+1 from sales.journal_header) pkid");
            txtPkid.Text = dtKeys.Rows[0]["pkid"].ToString();
           
            string strIssueNo = dtKeys.Rows[0]["IssueNo"].ToString();

            txtJourNo.Text = glb_function.glb_strBranchPkid + "-" + strIssueNo;

            int icheck = 0;

            icheck = cnn.TranDataToDB("insert into journal_header values (" +txtPkid.Text+
                ",'فعال'" +
                ",sysdate()" +
                "," +glb_function.glb_strUserId+
                "," + glb_function.glb_strBranchPkid +
                ",'"+txtJourNo.Text .Trim()+"'" +
                ",'سند قيد'" +
                "," + strIssueNo +
                ",str_to_date('" + dtpJourDate.SelectedDate.Value.ToString("dd/MM/yyyy")+ "','%d/%m/%Y')" +
                ",'" + txtHeaderNote.Text.Trim() + "'" +
                ")");
            if(icheck <=0)
            {
                glb_function.MsgBox("حدث خطأ اثناء حفظ البيانات الأساسية");
                return;
            }

            for (int i = 0; i < dgvJourDetails.Items.Count; i++)
            {
                double dMainValue = 0;
                double dCurrValue = 0;

             if(Convert.ToDouble(   glb_function.GetCellValue(ref dgvJourDetails, clmDept.DisplayIndex, i))>0)
                {
                    dMainValue = Convert.ToDouble(glb_function.GetCellValue(ref dgvJourDetails, clmDept.DisplayIndex, i)) ;
                    dCurrValue= Convert.ToDouble(glb_function.GetCellValue(ref dgvJourDetails, clmCurrValue.DisplayIndex, i)) ;
                }
             else
                {
                    dMainValue = Convert.ToDouble(glb_function.GetCellValue(ref dgvJourDetails, clmCredit.DisplayIndex, i)) * -1;
                    dCurrValue = Convert.ToDouble(glb_function.GetCellValue(ref dgvJourDetails, clmCurrValue.DisplayIndex, i)) * -1;
                }

                icheck = cnn.TranDataToDB("insert into journal_details values ((select ifnull(max(b.pkid),0)+1 from journal_details b )" +
               ",'فعال'" +
               ",sysdate()" +
               "," + glb_function.glb_strUserId +
                "," + txtPkid.Text +
                "," + glb_function.GetCellValue(ref dgvJourDetails,clmCurrencyId.DisplayIndex,i) +
                "," + glb_function.GetCellValue(ref dgvJourDetails, clmAccId.DisplayIndex, i) +
                "," + dMainValue +
                "," + dCurrValue +
                "," + glb_function.GetCellValue(ref dgvJourDetails, clmExchangeRate.DisplayIndex, i) +
                ",'"+ glb_function.GetCellValue(ref dgvJourDetails, clmJourNote.DisplayIndex, i) + "'" +
               ")");
                if (icheck <= 0)
                {
                    cnn.glb_RollbackTransaction();
                    glb_function.MsgBox("حدث خطأ اثناء حفظ التفاصيل");
                    return;
                }

            }
           

            cnn.glb_commitTransaction();

            if (glb_function.MsgBox("تمت عملية الحفظ بنجاح" + "\n" + "هل تريد طباعة سند القيد؟", "", true) == false)
                return;



            //SELECT worker_fname, worker_mname, worker_lname, worker_sirname," +
            //        "Passport_No,Passport_Place_Of_Birth_Id,date_format(Passport_Date_Of_Birth,'%Y-%m-%d')," +
            //        "date_format(Passport_Date_Of_Issue,'%Y-%m-%d'),date_format(Passport_Date_Of_Expiry,'%Y-%m-%d')" +






        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            new glb_function().clearItems(this);
            txtCreditTotal.Text = "0";
            txtDeptTotal.Text = "0";
            txtDeptTotal.Background = txtCreditTotal.Background = Brushes.LawnGreen;

        }

        
    }
}
