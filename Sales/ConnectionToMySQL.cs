using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;

class ConnectionToMySQL
{
        internal static string strDbCnnString = "server=localhost;port=3300;charset=utf8;database=Sales;userid=root;password=bigboss;SslMode=none;AllowPublicKeyRetrieval=True";
        internal static MySqlConnection glb_cnn = new MySqlConnection(strDbCnnString);
        internal static MySqlCommand comm;
        internal static MySqlTransaction trns;
        internal static bool isTrns = false;


        internal DataTable GetDataTable(string SqlSt)
        {
            MySqlCommand MyComm = new MySqlCommand(SqlSt, glb_cnn);
            MySqlDataReader MyDatareadre = null;
            DataTable MyTable = new DataTable();
            DataRow MyDataRow = null;
            int ColsCount = 0;

            try
            {
                if (glb_cnn.State == System.Data.ConnectionState.Closed)
                    glb_cnn.Open();


                MyDatareadre = MyComm.ExecuteReader();
                for (ColsCount = 0; ColsCount < MyDatareadre.FieldCount; ColsCount++)
                {
                    MyTable.Columns.Add(MyDatareadre.GetName(ColsCount), MyDatareadre.GetFieldType(ColsCount));
                }
                while (MyDatareadre.Read())
                {
                    MyDataRow = MyTable.NewRow();
                    for (ColsCount = 0; ColsCount < MyDatareadre.FieldCount; ColsCount++)
                    {
                        MyDataRow[ColsCount] = MyDatareadre.GetValue(ColsCount);

                    }
                    MyTable.Rows.Add(MyDataRow);
                }




                if (MyDatareadre != null)
                {
                    if (MyDatareadre.IsClosed)
                        MyDatareadre.Close();
                }

                MyDatareadre.Close();
                MyDatareadre.Dispose();
                return MyTable;
            }
            catch (Exception ErrGetData)
            {
            
                MessageBox.Show(ErrGetData.Source + Convert.ToChar(13) + ErrGetData.Message, "Error",MessageBoxButton.OK,MessageBoxImage.Error);
               
                return null;

            }

        }

        internal int TranDataToDB(string SqlSt)
        {
            try
            {

                if (glb_cnn.State == System.Data.ConnectionState.Closed)
                    glb_cnn.Open();

                comm = new MySqlCommand(SqlSt, glb_cnn);

                if (isTrns == false)
                {
                    trns = glb_cnn.BeginTransaction();
                    comm.Transaction = trns;
                    isTrns = true;

                }

                return (comm.ExecuteNonQuery());
            }
            catch (Exception ErrGetData)
            {

            MessageBox.Show(ErrGetData.Source + Convert.ToChar(13) + ErrGetData.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            
                return -1;
            }
        }
    internal int InsertDataWithClob(string SqlSt, MySqlParameter para1, MySqlParameter para2, MySqlParameter para3)
    {
        try
        {

            if (glb_cnn.State == ConnectionState.Closed)
                glb_cnn.Open();



            if (isTrns == false)
            {
                comm = new MySqlCommand(SqlSt, glb_cnn);
                trns = glb_cnn.BeginTransaction();
                comm.Transaction = trns;

                isTrns = true;
            }
            else
                comm.CommandText = SqlSt;
            if (para1 != null)
                comm.Parameters.Add(para1);
            if (para2 != null)
                comm.Parameters.Add(para2);
            if (para3 != null)
                comm.Parameters.Add(para3);

            return (comm.ExecuteNonQuery());
        }
        catch (Exception ErrGetData)
        {

            MessageBox.Show(ErrGetData.Source + Convert.ToChar(13) + ErrGetData.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);


            return -1;
        }
    }
    internal void glb_commitTransaction()
        {
            trns.Commit();
            isTrns = false;

        }

        internal void glb_RollbackTransaction()
        {
            trns.Rollback();
            isTrns = false;

        }
    }//end class 

