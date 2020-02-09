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
    public partial class frmCurrency : Window
    {
        public frmCurrency()
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            int icheck = cnn.TranDataToDB("insert into currency values(" +
                "(SELECT ifnull(max(b.pkid),0)+1 FROM currency b)" +
                ",'فعال'" +
                ",sysdate()" +
                "," + glb_function.glb_strUserId +
                ",'" + txtCurr_name.Text.Trim() + "'" +
                ",'" + txtCURR_SMPL.Text.Trim() + "'" +
                ",'" + numCURR_DECIMAL.Value.ToString() + "'" +
                ",'" + numCURR_CHANGE_Rate.Value.ToString() + "'" +
                ",'" + txtCURR_NOTE.Text.Trim() + "'" +
                ")");

            if (icheck <= 0)
            {
                glb_function.MsgBox("حدث خطأ اثناء الإضافة");
                return;
            }

            cnn.glb_commitTransaction();
            glb_function.MsgBox("تمت عملية الإضافة بنجاح");

            txtCurr_name.Text = "";
            txtCURR_SMPL.Text = "";
            txtCURR_NOTE.Text = "";
            numCURR_DECIMAL.Value = 0;
            numCURR_CHANGE_Rate.Value = 0;
            GetCurrency();


        }
      class  clsCurrencyData
        {
            public string clmPKid { get; set; }
            public string clmCurrName { get; set; }
            public string clmCurrSample { get; set; }
            public string clmCurrDecimal { get; set; }
            public string clmExchangeRate { get; set; }
            public string clmCurrNotes { get; set; }

        }
        private void GetCurrency()
        {
            dgvCurrencies.Items.Clear();
          
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtCurrData = cnn.GetDataTable("SELECT pkid,curr_name,curr_smpl,curr_decimal,curr_change_rate,curr_note " +
                " FROM currency order by pkid");
            List<clsCurrencyData> lstSource = new List<clsCurrencyData>();
            clsCurrencyData newRow;
            for (int i = 0; i < dtCurrData.Rows.Count; i++)
            {
                newRow = new clsCurrencyData();
                newRow.clmPKid = dtCurrData.Rows[i]["pkid"].ToString();
                newRow.clmCurrName = dtCurrData.Rows[i]["curr_name"].ToString();
                newRow.clmCurrSample = dtCurrData.Rows[i]["curr_smpl"].ToString();
                newRow.clmCurrDecimal = dtCurrData.Rows[i]["curr_decimal"].ToString();
                newRow.clmExchangeRate = dtCurrData.Rows[i]["curr_change_rate"].ToString();
                newRow.clmCurrNotes = dtCurrData.Rows[i]["curr_note"].ToString();

                dgvCurrencies.Items.Add(newRow);

            }

           

        }

        private void btnUpdateCurrency_Click(object sender, RoutedEventArgs e)
        {
          string strPkid=  glb_function.GetCellValue(ref dgvCurrencies, clmPKid.DisplayIndex, dgvCurrencies.SelectedIndex);
            string strNote = glb_function.GetCellValue(ref dgvCurrencies, clmCurrNotes.DisplayIndex, dgvCurrencies.SelectedIndex);
            string strExhangeRate = glb_function.GetCellValueControl(ref dgvCurrencies, "Double", clmExchangeRate.DisplayIndex, dgvCurrencies.SelectedIndex);

            if(strNote.Trim() == "العملة الرئيسية")
            {
                glb_function.MsgBox("لايمكن تغيير سعر صرف العملة الرئيسية");
                GetCurrency();
                return;
            }

            ConnectionToMySQL cnn = new ConnectionToMySQL();
            int icheck = cnn.TranDataToDB("update currency set curr_change_rate=" + strExhangeRate + " where pkid="+strPkid );

            if(icheck <=0)
            {
                glb_function.MsgBox("حدث خطأ اثناء عملية التعديل");
                return;
            }
            cnn.glb_commitTransaction();
            glb_function.MsgBox("تمت عملية التعديل بنجاح");


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetCurrency();
        }
    }
}
