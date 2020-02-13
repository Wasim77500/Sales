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
    public partial class frmCashPayments : Window
    {
        bool bLoad = false;
        string strCasherAccId = "";
        public frmCashPayments()
        {
            bLoad = true;
            InitializeComponent();
           
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

       
      
        
      

      

        private void lstAccNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            if (lstAccNo.SelectedValue == null || lstAccNo.SelectedIndex==-1)
                return;

            
             lstAccName.SelectedValue = lstAccNo.SelectedValue;
            lstAccNo.SelectedValue = lstAccName.SelectedValue;
        }

        private void lstAccName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstAccName.SelectedValue == null || lstAccName.SelectedIndex == -1)
                return;

     
            lstAccNo.SelectedValue = lstAccName.SelectedValue;
            lstAccName.SelectedValue = lstAccNo.SelectedValue;
            
            
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
            nmbDept.Value = 0;
           
            nmbCurrValue.Value = 0;
            iDecimalPlace = 0;
            nmbExchangeRate.Value = 1;
            if (lstCurrency.SelectedIndex == -1 || lstCurrency.SelectedValue ==null)
                return;

            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtCurr = cnn.GetDataTable("SELECT pkid,curr_smpl,curr_decimal,CURR_CHANGE_Rate FROM currency where pkid="+lstCurrency.SelectedValue.ToString());

            nmbExchangeRate.Value = Convert.ToDouble(dtCurr.Rows[0]["CURR_CHANGE_Rate"].ToString());
            iDecimalPlace= Convert.ToInt32(dtCurr.Rows[0]["curr_decimal"].ToString());
            nmbCurrValue.FormatString = "N" + iDecimalPlace.ToString();

        }

      

      

        private void nmbCredit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (bLoad)
                return;

            double d = (double)(nmbDept.Value / nmbExchangeRate.Value);
           
            nmbCurrValue.Value = Math.Round(d, iDecimalPlace);
        }

        private void nmbCurrValue_LostFocus(object sender, RoutedEventArgs e)
        {
          
        }

        private void nmbCurrValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {



            nmbDept.Value = nmbCurrValue.Value * nmbExchangeRate.Value;
               
            
                
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
           

            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtKeys = cnn.GetDataTable("select (select  ifnull(max( convert(  substring(jour_no,instr(jour_no,'-')+1),signed)),0)+1 FROM sales.journal_header where Branch_id="+glb_function.glb_strBranchPkid+ " and trans_name='سند صرف') IssueNo,(select ifnull(max(pkid),0)+1 from sales.journal_header) pkid");
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
                ",'سند صرف'" +
                "," + strIssueNo +
                ",str_to_date('" + dtpJourDate.SelectedDate.Value.ToString("dd/MM/yyyy")+ "','%d/%m/%Y')" +
                ",'" + txtHeaderNote.Text.Trim() + "'" +
                 ",'" + txtPerson.Text.Trim() + "'" +
                ")");
            if(icheck <=0)
            {
                glb_function.MsgBox("حدث خطأ اثناء حفظ البيانات الأساسية");
                return;
            }

            //حفظ بيانات المدين 
            //
            double dMainValue =(double)nmbDept.Value;
                double dCurrValue =(double) nmbCurrValue.Value;
                icheck = cnn.TranDataToDB("insert into journal_details values ((select ifnull(max(b.pkid),0)+1 from journal_details b )" +
               ",'فعال'" +
               ",sysdate()" +
               "," + glb_function.glb_strUserId +
                "," + txtPkid.Text +
                "," + lstCurrency.SelectedValue.ToString() +
                "," + lstAccNo.SelectedValue.ToString()   +
                "," + dMainValue +
                "," + dCurrValue +
                "," + nmbExchangeRate.Value.ToString() +
                ",''" +
               ")");
                if (icheck <= 0)
                {
                    cnn.glb_RollbackTransaction();
                    glb_function.MsgBox("حدث خطأ اثناء حفظ التفاصيل");
                    return;
                }

           
            //حفظ بيانات الدائن 
            //يكون الصندوق دائن في سند الصرف ويميزه انه اصغر من الصفر
            dMainValue = (double)nmbDept.Value * -1;
             dCurrValue = (double)nmbCurrValue.Value * -1;
            icheck = cnn.TranDataToDB("insert into journal_details values ((select ifnull(max(b.pkid),0)+1 from journal_details b )" +
           ",'فعال'" +
           ",sysdate()" +
           "," + glb_function.glb_strUserId +
            "," + txtPkid.Text +
            "," + lstCurrency.SelectedValue.ToString() +
            "," + strCasherAccId +
            "," + dMainValue +
            "," + dCurrValue +
            "," + nmbExchangeRate.Value.ToString() +
            ",''" +
           ")");
            if (icheck <= 0)
            {
                cnn.glb_RollbackTransaction();
                glb_function.MsgBox("حدث خطأ اثناء حفظ التفاصيل");
                return;
            }



            cnn.glb_commitTransaction();
            GetData(txtPkid.Text);
            if (glb_function.MsgBox("تمت عملية الحفظ بنجاح" + "\n" + "هل تريد طباعة سند القبض؟", "", true) == false)
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

            PrepareForm();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new UserTemplate().GetPrivForThisForm(this);
            strCasherAccId = txtAccNo.Text.Trim();
            if(strCasherAccId=="" || strCasherAccId==null)
            {
                glb_function.MsgBox("لايوجد صندوق محدد لهذا النموذج");
                this.Close();
            }
            PrepareForm();
        }
       private void  PrepareForm()
        {
            FillData();
            if (UserTemplate.HasPrivilege("btnSave"))
                btnSave.IsEnabled = true;

            btnUpdate.IsEnabled = false;
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

            ConnectionToMySQL cnn = new ConnectionToMySQL();
           

          

            int icheck = 0;

            icheck = cnn.TranDataToDB("update journal_header set " +              
                "created_user=" + glb_function.glb_strUserId +
                ",jour_date=str_to_date('" + dtpJourDate.SelectedDate.Value.ToString("dd/MM/yyyy") + "','%d/%m/%Y')" +
                ",jour_note='" + txtHeaderNote.Text.Trim() + "'" +
                 ",Person='" + txtPerson.Text.Trim() + "'" +
                " where pkid=" +txtPkid.Text );
            if (icheck <= 0)
            {
                glb_function.MsgBox("حدث خطأ اثناء تعديل البيانات الأساسية");
                return;
            }

            icheck = cnn.TranDataToDB("delete from journal_details where header_id= "+txtPkid.Text );
            if (icheck <= 0)
            {
                cnn.glb_commitTransaction();
                glb_function.MsgBox("حدث خطأ اثناء حذف البيانات السابقة");
                return;
            }
            //حفظ بيانات المدين 
            //يكون الصندوق مدين في سند القبض ويميزه انه اكبر من الصفر
            double dMainValue = (double)nmbDept.Value;
            double dCurrValue = (double)nmbCurrValue.Value;
            icheck = cnn.TranDataToDB("insert into journal_details values ((select ifnull(max(b.pkid),0)+1 from journal_details b )" +
           ",'فعال'" +
           ",sysdate()" +
           "," + glb_function.glb_strUserId +
            "," + txtPkid.Text +
            "," + lstCurrency.SelectedValue.ToString() +
            "," + lstAccNo.SelectedValue.ToString()   +
            "," + dMainValue +
            "," + dCurrValue +
            "," + nmbExchangeRate.Value.ToString() +
            ",''" +
           ")");
            if (icheck <= 0)
            {
                cnn.glb_RollbackTransaction();
                glb_function.MsgBox("حدث خطأ اثناء تعديل التفاصيل");
                return;
            }

            //حفظ بيانات الدائن 
            //يكون الصندوق الدائن في سند القبض ويميزه انه اصغر من الصفر
            dMainValue = (double)nmbDept.Value * -1;
            dCurrValue = (double)nmbCurrValue.Value * -1;
            icheck = cnn.TranDataToDB("insert into journal_details values ((select ifnull(max(b.pkid),0)+1 from journal_details b )" +
           ",'فعال'" +
           ",sysdate()" +
           "," + glb_function.glb_strUserId +
            "," + txtPkid.Text +
            "," + lstCurrency.SelectedValue.ToString() +
            "," + strCasherAccId +
            "," + dMainValue +
            "," + dCurrValue +
            "," + nmbExchangeRate.Value.ToString() +
            ",''" +
           ")");
            if (icheck <= 0)
            {
                cnn.glb_RollbackTransaction();
                glb_function.MsgBox("حدث خطأ اثناء تعديل التفاصيل");
                return;
            }



            cnn.glb_commitTransaction();
            GetData(txtPkid.Text);
            if (glb_function.MsgBox("تمت عملية التعديل بنجاح" + "\n" + "هل تريد طباعة سند القبض؟", "", true) == false)
                return;




        }

        private void GetData(string strPk)
        {
            new glb_function().clearItems(this);
            PrepareForm();
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtCashRecData = cnn.GetDataTable("SELECT h.Pkid, h.stat, Branch_id, jour_no, trans_name, trans_id, date_format(jour_date,'%d/%m/%Y') jour_date, jour_note,Person, " +
                          "  d.pkid dpkid, d.stat dstat, curr_id, acc_id, main_value, jour_value, exchange_Rate, jour_details " +
                          "  FROM journal_header h " +
                          "  join journal_details d " +
                          "  on(h.pkid = d.header_id) " +
                          "  where h.pkid =  " +strPk );
            bLoad = true;
            txtAccNo.Text = strCasherAccId;
            txtPkid.Text = strPk;

            txtJourNo.Text = dtCashRecData.Rows[0]["jour_no"].ToString();
            dtpJourDate.SelectedDate = DateTime.ParseExact(dtCashRecData.Rows[0]["jour_date"].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            txtHeaderNote.Text = dtCashRecData.Rows[0]["jour_note"].ToString();
            txtPerson.Text = dtCashRecData.Rows[0]["Person"].ToString();
            
           if (dtCashRecData.Rows[0]["acc_id"].ToString()== strCasherAccId)
            {
                lstCurrency.SelectedValue = dtCashRecData.Rows[1]["curr_id"].ToString();
                lstCurrency_SelectionChanged(null, null);
                lstAccNo.SelectedValue = dtCashRecData.Rows[1]["acc_id"].ToString();
                lstAccName.SelectedValue = dtCashRecData.Rows[1]["acc_id"].ToString();
                nmbExchangeRate.Value = Convert.ToDouble(dtCashRecData.Rows[1]["exchange_Rate"].ToString());
                nmbDept.Value = Convert.ToDouble(dtCashRecData.Rows[1]["main_value"].ToString());
                nmbCurrValue.Value = Convert.ToDouble(dtCashRecData.Rows[1]["jour_value"].ToString());

              
                   
               
            }
           else
            {
                lstCurrency.SelectedValue = dtCashRecData.Rows[0]["curr_id"].ToString();
                lstCurrency_SelectionChanged(null, null);
                lstAccNo.SelectedValue = dtCashRecData.Rows[0]["acc_id"].ToString();
                lstAccName.SelectedValue = dtCashRecData.Rows[1]["acc_id"].ToString();
                nmbExchangeRate.Value = Convert.ToDouble(dtCashRecData.Rows[0]["exchange_Rate"].ToString());
                nmbDept.Value = Convert.ToDouble(dtCashRecData.Rows[0]["main_value"].ToString());
                nmbCurrValue.Value = Convert.ToDouble(dtCashRecData.Rows[0]["jour_value"].ToString());
            }
            if (nmbDept.Value < 0)
            {
                nmbDept.Value *= -1;
                nmbCurrValue.Value *= -1;
            }
           
            if (UserTemplate.HasPrivilege("btnUpdate"))
                btnUpdate.IsEnabled = true;



            btnSave.IsEnabled = false;
            bLoad = false ;
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            frmFindJournal frm = new frmFindJournal();
            frm.strWhere = " and trans_name='سند صرف' and branch_id=" + glb_function.glb_strBranchPkid + " ";
            frm.ShowDialog();
            if (frm.strPKid != "")
                GetData(frm.strPKid);
        }
    }
}
