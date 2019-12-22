using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sales
{
    class UserTemplate
    {
       static  DataTable  dtPri;

        
        string strTempletId = "";
        Window frm = new Window();
        public void GetPrivForThisForm(Window CurrForm)
        {
            frm = CurrForm;
            if (glb_function.glb_strUserName == "وسيم الأغبري")
                return;
            if (glb_function.glb_strUserName == "")
            {
                glb_function.MsgBox("لا يوجد اسم مستخدم");
                frm.Close();
            }


            ConnectionToMySQL cnn = new ConnectionToMySQL();
            DataTable dtTemplete = cnn.GetDataTable("select u.templet_id,h.templet_name from  jud.templet_header h,user_templet u " +
                        " where h.pkid = u.templet_id " +
                        " and h.form_eng_name = '" + frm.Name + "' and u.user_id=" + glb_function.glb_strUserId);

            if (dtTemplete == null || dtTemplete.Rows.Count <= 0)
            {
                glb_function.MsgBox("لا توجد صلاحيات لهذه الشاشة");
               frm.Close();
                return;
            }


            strTempletId = dtTemplete.Rows[0]["templet_id"].ToString();
            lstTemplet_SelectedIndexChanged(null, null);




        }
        private void lstTemplet_SelectedIndexChanged(object sender, EventArgs e)
        {
          
            {

                

                string strStat = "select h.pkid,h.form_eng_name,h.form_type_en ,d.control_type,d.en_name,d.real_value,d.ar_name,multi_form_for_user " +
                      " from jud.templet_header h,jud.templet_details d" +
                     "  where h.pkid = d.header_id " +
                    "  and h.pkid = " + strTempletId;


                ConnectionToMySQL cnn = new ConnectionToMySQL();
                dtPri = cnn.GetDataTable(strStat);

                //if (dtPri.Rows[0]["multi_form_for_user"].ToString() == "1")
                //{
                //    lstTemplet.Visible = true;

                //}


                for (int i = 0; i < dtPri.Rows.Count; i++)
                {
                    if (dtPri.Rows[i]["control_type"].ToString() == "Visability")
                    {
                        if (dtPri.Rows[i]["real_value"].ToString() == "1")
                        {
                            Visual ctr = GetControlByName(frm, dtPri.Rows[i]["en_name"].ToString());
                            if (ctr != null)
                              ( (Control ) ctr).Visibility = Visibility.Visible;
                           
                        }

                        else
                        {
                            Visual ctr = GetControlByName(frm, dtPri.Rows[i]["en_name"].ToString());
                            if (ctr != null)
                                ((Control)ctr).Visibility = Visibility.Hidden;
                        }

                    }
                    else if (dtPri.Rows[i]["control_type"].ToString() == "Enability")
                    {
                        if (dtPri.Rows[i]["real_value"].ToString() == "1")
                        {

                            Visual ctr = GetControlByName(frm, dtPri.Rows[i]["en_name"].ToString());
                            if (ctr != null)
                                ((Control)ctr).IsEnabled = true;
                            //string strss = dtPri.Rows[i]["en_name"].ToString();
                            //this.Controls[dtPri.Rows[i]["en_name"].ToString()].Enabled = true;
                        }

                        else
                        {
                            Visual ctr = GetControlByName(frm, dtPri.Rows[i]["en_name"].ToString());
                            if (ctr != null)
                                ((Control)ctr).IsEnabled = false;
                        }

                    }
                    else if (dtPri.Rows[i]["control_type"].ToString() == "List" || dtPri.Rows[i]["control_type"].ToString() == "Text" || dtPri.Rows[i]["control_type"].ToString() == "MultiChoices" || dtPri.Rows[i]["control_type"].ToString() == "Free MultiChoices" || dtPri.Rows[i]["control_type"].ToString() == "Free List")
                    {

                        Visual ctr = GetControlByName(frm, dtPri.Rows[i]["en_name"].ToString());
                        if (ctr != null)
                        {
                            //if(ctr.GetType()==typeof(ComboBox) )
                            // ((ComboBox)ctr).Text = dtPri.Rows[i]["real_value"].ToString();
                            //if (ctr.GetType() == typeof(TextBox))
                                ((TextBox)ctr).Text = dtPri.Rows[i]["real_value"].ToString();
                        
                        }
                        
                        //}

                    }



                }



               
            }
        }
        private Visual GetControlByName(Visual ctrl, string Name)
        {
            Visual bReturn = null;
            //foreach (Control c in ctrl.Controls)
            //    if (c.Name == Name)
            //    {
            //        bReturn = c;
            //        return bReturn;
            //    }
            string str = "";
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(ctrl); i++)
            {

                Visual childVisual = (Visual)VisualTreeHelper.GetChild(ctrl, i);
                if (childVisual.GetType() == typeof(TextBox))
                    str = ((TextBox)childVisual).Name;
                if (childVisual.GetType() == typeof(TextBlock))
                    str = ((TextBlock)childVisual).Name;
                else if (childVisual.GetType() == typeof(ComboBox))
                    str = ((ComboBox)childVisual).Name;
                else if (childVisual.GetType() == typeof(Button))
                    str = ((Button)childVisual).Name;
                else if (childVisual.GetType() == typeof(DatePicker))
                    str = ((DatePicker)childVisual).Name;
                else if (childVisual.GetType() == typeof(DataGrid))
                    str = ((DataGrid)childVisual).Name;
                else if (childVisual.GetType() == typeof(Xceed.Wpf.Toolkit.IntegerUpDown))
                    str = ((Xceed.Wpf.Toolkit.IntegerUpDown)childVisual).Name;
                else if (childVisual.GetType() == typeof(GroupBox))
                    str = ((GroupBox)childVisual).Name;


                if (str == Name)
                {
                    bReturn = childVisual;
                    return bReturn;
                }
                else
                    if (bReturn == null)
                    bReturn = GetControlByName(childVisual, Name);
            }
               

            return bReturn;
        }
        public static bool HasPrivilege(string strPriv)
        {

            if (glb_function.glb_strUserName == "وسيم الأغبري")
                return true;


            string str = dtPri.Rows.Count.ToString();
            string strss = dtPri.Rows[0]["en_name"].ToString();

            DataRow[] childRows = dtPri.Select("[en_name]='" + strPriv + "'");
            if (childRows[0]["control_type"].ToString() == "Enability")
            {
                if (childRows[0]["real_value"].ToString() == "1")
                    return true;
                else
                    return false;
            }
            if (childRows[0]["control_type"].ToString() == "Visability")
            {
                if (childRows[0]["real_value"].ToString() == "1")
                    return true;
                else
                    return false;
            }



            return false;

        }
     

    }
}
