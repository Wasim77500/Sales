using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Configuration;
using System.Security.Cryptography;
using System.Windows.Media;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace Sales
{
     class glb_function
     {
         internal static DataTable dtReport;

         //when program statr
     internal static string glb_strUserId="" ;
	 internal static string glb_strUserName ="";
     internal static string glb_strBranchPkid = "";

        internal static bool blMsg;
    
   

    

      
         
       internal static void  MoveCursor(Control c)
        {
          c. MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    internal void clearItems(Visual myControl)
    {

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(myControl); i++)
            {
              
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(myControl, i);
                if (childVisual.GetType() == typeof(TextBox))
                {
                    ((TextBox)childVisual).Text = "";

                }
                else if (childVisual.GetType() == typeof(PasswordBox))
                {
                    ((PasswordBox)childVisual).Password = "";
                }
                else if (childVisual.GetType() == typeof(DatePicker))
                {
                    ((DatePicker)childVisual).SelectedDate = DateTime.Now;
                }
                else if (childVisual.GetType() == typeof(ComboBox))
                {
                    ((ComboBox)childVisual).ItemsSource = null;
                    ((ComboBox)childVisual).SelectedIndex = -1;
                }
                else if (childVisual.GetType() == typeof(DataGrid))
                {
                    ((DataGrid)childVisual).Items.Clear();
                }
                else if (childVisual.GetType() == typeof(Xceed.Wpf.Toolkit.IntegerUpDown))
                {
                    ((Xceed.Wpf.Toolkit.IntegerUpDown)childVisual).Value = 0;
                }
                else
             
                clearItems(childVisual);
            }
        }
   


        internal static void MsgBox(string strMsg)
        {
            frmMsg frm = new frmMsg();
            frm.lblMsg.Text = strMsg;
            frm.txtTitel.Text  = "رسالة تنبيه";
            frm.stpOk.Visibility = Visibility.Visible;
            frm.stpYesNO.Visibility = Visibility.Hidden;
            frm.ShowDialog();
          //  frm.Dispose();
          

        }
        internal static void MsgBox(string strMsg, string strTitle)
        {
            frmMsg frm = new frmMsg();
            frm.lblMsg.Text = strMsg;
            frm.txtTitel.Text = strTitle;
            frm.stpOk.Visibility = Visibility.Visible;
            frm.stpYesNO.Visibility = Visibility.Hidden;
            frm.ShowDialog();
            //  frm.Dispose();


        }
        internal static bool MsgBox(string strMsg, string strTitle, bool blDialog)
        {
            frmMsg frm = new frmMsg();
            frm.lblMsg.Text = strMsg;
            frm.txtTitel.Text = strTitle;
            frm.stpYesNO.Visibility = Visibility.Visible;
            frm.stpOk.Visibility = Visibility.Hidden;
            frm.ShowDialog();
            

            return blMsg;

        }

        internal string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            AppSettingsReader settingsReader = new AppSettingsReader();



            string key = "Hashpassword98549642";


            if (useHashing)
            {

                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

            tdes.Key = keyArray;

            tdes.Mode = CipherMode.ECB;


            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();

            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);

            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        internal static string GetCellValue(ref DataGrid Grid1, int iColumn ,int iRow)
        {
            TextBlock x = Grid1.Columns[iColumn].GetCellContent(Grid1.Items[iRow]) as TextBlock;
            string strReturnValue;
            if (x == null)
                strReturnValue = "";
            else
                strReturnValue = x.Text.Trim();


            return strReturnValue;
        }

        internal static string GetCellValueControl(ref DataGrid Grid1, string strControl, int iColumn, int iRow)
        {
            string strReturnValue = "";
            DataGridRow row = Grid1.ItemContainerGenerator.ContainerFromIndex(iRow) as DataGridRow;
            ContentPresenter CP = Grid1.Columns[iColumn].GetCellContent(row) as ContentPresenter;

            if (strControl == "Date")
            {
                DatePicker DP = FindVisualChild<DatePicker>(CP);
                if (DP != null)
                    strReturnValue = DP.SelectedDate.Value.ToString("dd/MM/yyyy");
                else
                    strReturnValue = "";
            }
            if (strControl == "TextBox")
            {
                TextBox DP = FindVisualChild<TextBox>(CP);
                if (DP != null)
                    strReturnValue = DP.Text.Trim();
                else
                    strReturnValue = "";
            }
            if (strControl == "Double")
            {
                DoubleUpDown DP = FindVisualChild<DoubleUpDown>(CP);
                if (DP != null)
                    strReturnValue = DP.Value.ToString().Trim();
                else
                    strReturnValue = "";
            }
            if (strControl == "Int")
            {
                IntegerUpDown DP = FindVisualChild<IntegerUpDown>(CP);
                if (DP != null)
                    strReturnValue = DP.Value.ToString().Trim();
                else
                    strReturnValue = "";
            }


            return strReturnValue;
        }
        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }
        private static T FindParent<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);

            if (parent == null) return null;

            var parentT = parent as T;
            return parentT ?? FindParent<T>(parent);
        }


        public static bool UpdateHistory(string strTableName,string strColName,string strPkid,string strOldValue,string strNewValue,string strColArName)
        {
            ConnectionToMySQL cnn = new ConnectionToMySQL();
            int icheck = 0;
            // insert into sales.USER_TEMPLET values((select ifnull(max(b.pkid),0)+1 from sales.USER_TEMPLET b),
            
                icheck = cnn.TranDataToDB("insert into sales.updatehistory values((select ifnull(max(b.pk),0)+1 from sales.updatehistory b)," + glb_function.glb_strUserId + ",sysdate(),'"+strTableName+"','"+strColName+"'," + strPkid + ",'" + strOldValue + "','" + strNewValue + "','"+strColArName+"')");

            if (icheck <= 0)
                return false;


            cnn.glb_commitTransaction();
            return true;
            
        }

    }

}
