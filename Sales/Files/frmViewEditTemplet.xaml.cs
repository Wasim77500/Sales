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
    public partial class frmViewEditTemplet : Window
    {
        public frmViewEditTemplet()
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtHeaderTemp = cnn.GetDataTable("select h.pkid,h.templet_name from templet_header h  " +
                    " where form_eng_name = '" + txtFormEnName.Text.Trim() + "' and form_type_en='" + txtForm_type.Text.Trim() + "'");
            clsTempHeader newRow;
            for (int i = 0; i < dtHeaderTemp.Rows.Count; i++)
            {
                newRow = new clsTempHeader();
                newRow.clmPkId = dtHeaderTemp.Rows[i]["pkid"].ToString();
                newRow.clmFormArName = dtHeaderTemp.Rows[i]["templet_name"].ToString();
                dgvTempHeader.Items.Add(newRow);
          
            }
        }

        private void dgvTempHeader_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvTempHeader.Items.Count > 0)
                if (dgvTempHeader.SelectedItems.Count > 0)
                {

                    //To get Selected Column in datagrid
                    DataGridCellInfo cellInfo = dgvTempHeader.CurrentCell;
                    //DataGridColumn column = cellInfo.Column;
                    //************************************


                    GetTempDetailsData(dgvTempHeader.SelectedIndex);
                }
        }

        private void dgvTempDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvTempHeader.Items.Count > 0)
                if (dgvTempHeader.SelectedItems.Count > 0)
                {

                    //To get Selected Column in datagrid
                    DataGridCellInfo cellInfo = dgvTempHeader.CurrentCell;
                    //DataGridColumn column = cellInfo.Column;
                    //************************************


                    GetTempDetailsData(dgvTempHeader.SelectedIndex);
                }
        }

        private void GetTempDetailsData(int iRow)
        {
           

            dgvTempDetails.Items.Clear();
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtTempDetail = cnn.GetDataTable("select d.pkid, control_type, en_name, ar_name, control_value, real_value  from templet_details d " +
                           "  where d.header_id  = " + glb_function.GetCellValue(ref dgvTempHeader, clmPkId.DisplayIndex, iRow));

            clsTempDetails newRow;

            for (int i = 0; i < dtTempDetail.Rows.Count; i++)
            {
                newRow = new clsTempDetails();
                newRow.clmPropName = dtTempDetail.Rows[i]["ar_name"].ToString();
                newRow.clmPropValue = dtTempDetail.Rows[i]["control_value"].ToString();
                dgvTempDetails.Items.Add(newRow);
            }

        }
        class clsTempDetails
        {
            public string clmPropName { get; set; }
            public string clmPropValue { get; set; }
        }

        class clsTempHeader
        {
            public string clmPkId { get; set; }
            public string clmFormArName { get; set; }
        }
    }
}
