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

namespace Sales.Files
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    public partial class frmFindForms : Window
    {
        public string strPkid;
        public frmFindForms()
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
            GetFormData();
        }
        private void GetFormData()
        {
            dgvForms.Items.Clear();
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            System.Data.DataTable dtUsers = cnn.GetDataTable("select pkid,ar_name,en_name from forms_hd  " +
                                " where ar_name like '%" + txtFormAr.Text + "%' and en_name like '%" + txtFormEn.Text + "%' ");

            clsGrid newRow;
            for (int i = 0; i < dtUsers.Rows.Count; i++)
            {


                newRow = new clsGrid();
                newRow.clmPkid = dtUsers.Rows[i]["pkid"].ToString();
                newRow.clmFormAr = dtUsers.Rows[i]["ar_name"].ToString();
                newRow.clmFormEn = dtUsers.Rows[i]["en_name"].ToString();

                dgvForms.Items.Add(newRow);



            }


        }
        class clsGrid
        {
            public string clmPkid { get; set; }
            public string clmFormAr { get; set; }
            public string clmFormEn { get; set; }

        }
        private void dgvForms_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnOk_Click(null, null);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (dgvForms.SelectedItems.Count > 0)
            {
                // TextBlock x = dgvForms.Columns[clmPkid.DisplayIndex].GetCellContent(dgvForms.Items[dgvForms.SelectedIndex]) as TextBlock;
                // strPkid = x.Text.Trim();
                strPkid = glb_function.GetCellValue(ref dgvForms, clmPkid.DisplayIndex, dgvForms.SelectedIndex);
                this.Close();
            }
            else
                strPkid = "";
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            strPkid = "";
            this.Close();
        }
    }
}
