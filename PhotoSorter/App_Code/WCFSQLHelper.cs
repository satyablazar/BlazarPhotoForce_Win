using System;
using System.Collections.Generic;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.IO;

namespace PhotoForce.App_Code
{
    public class WCFSQLHelper
    {
        //used in clsDashBoard-GetStudentForAdminCdYearbookcd(ArrayList PhotoId, string windowname) Method.
        static public DataSet getDataSetText(string sql)
        {

            DataSet ds = new DataSet();
            string thisConnectionString = clsConnectionString.connectionString;
            SqlConnection oConn = new SqlConnection(thisConnectionString);
            oConn.Open();                                                       // Tempory until DAAB implementation
            SqlCommand oCmd = new SqlCommand(sql, oConn);
            oCmd.CommandType = CommandType.Text;
            oCmd.CommandTimeout = 120;
            SqlDataAdapter adp = new SqlDataAdapter(oCmd);
            adp.Fill(ds);
            ds.Dispose();
            oConn.Close();
            return ds;
        }
        //used in AddEditMasksVM-delete Method
        static public int executeNonQuery_SP(string storedProcName, SqlParameter[] param)
        {
            int retVal = 0;
            string thisConnectionString = clsConnectionString.connectionString;
            SqlConnection oConn = new SqlConnection(thisConnectionString);
            oConn.Open();   // Tempory until DAAB implementation
            SqlCommand oCmd = new SqlCommand(storedProcName, oConn);
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandTimeout = 120;
            for (int i = 0; i < param.Length; i++)
            {
                oCmd.Parameters.Add(param[i]);
            }
            retVal = oCmd.ExecuteNonQuery();
            oConn.Close();
            return retVal;
        }
        static public DataTable getDataTable(string sql)
        {
            DataSet ds = new DataSet();
            string thisConnectionString = clsConnectionString.connectionString;
            SqlConnection oConn = new SqlConnection(thisConnectionString);
            oConn.Open();                                                       // Tempory until DAAB implementation
            SqlCommand oCmd = new SqlCommand(sql, oConn);
            oCmd.CommandType = CommandType.Text;
            oCmd.CommandTimeout = 120;
            SqlDataAdapter adp = new SqlDataAdapter(oCmd);
            adp.Fill(ds);
            ds.Dispose();
            oConn.Close();
            return ds.Tables[0];
        }
        //Commented by mohan ; created new method "getImagesByMaskDetails" in clsGroup
        //export vm
        static public DataSet getDataSetText_SP(string Text, SqlParameter[] param)
        {
            DataSet ds = new DataSet();
            string thisConnectionString = clsConnectionString.connectionString;
            SqlConnection oConn = new SqlConnection(thisConnectionString);

            oConn.Open();                                                       // Tempory until DAAB implementation
            SqlCommand oCmd = new SqlCommand(Text, oConn);
            oCmd.CommandType = CommandType.Text;
            oCmd.CommandTimeout = 120;
            for (int i = 0; i < param.Length; i++)
            {
                oCmd.Parameters.Add(param[i]);
            }
            SqlDataAdapter adp = new SqlDataAdapter(oCmd);
            adp.Fill(ds);
            ds.Dispose();
            oConn.Close();
            return ds;
        }
        //used in AddEditSchool VM
        static public DataTable getDataTable_SP(string storedProcName, SqlParameter[] param)
        {
            DataTable dt = new DataTable();
            string thisConnectionString = clsConnectionString.connectionString;
            SqlConnection oConn = new SqlConnection(thisConnectionString);
            oConn.Open();                                                       // Tempory until DAAB implementation
            SqlCommand oCmd = new SqlCommand(storedProcName, oConn);
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandTimeout = 120;
            for (int i = 0; i < param.Length; i++)
            {
                oCmd.Parameters.Add(param[i]);
            }
            SqlDataAdapter adp = new SqlDataAdapter(oCmd);
            adp.Fill(dt);
            dt.Dispose();
            oConn.Close();
            return dt;
        }
        static public int executeScaler_SP(string storedProcName, SqlParameter[] param)
        {
            int retVal = 0;
            string thisConnectionString = clsConnectionString.connectionString;
            SqlConnection oConn = new SqlConnection(thisConnectionString);
            oConn.Open();                                                   // Tempory until DAAB implementation
            SqlCommand oCmd = new SqlCommand(storedProcName, oConn);
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandTimeout = 120;
            for (int i = 0; i < param.Length; i++)
            {
                oCmd.Parameters.Add(param[i]);
            }
            retVal = Convert.ToInt32(oCmd.ExecuteScalar());
            oConn.Close();
            return retVal;
        }
        //used in masks vm
        static public DataSet getDataSet_SP(string storedProcName)
        {
            DataSet ds = new DataSet();
            string thisConnectionString = clsConnectionString.connectionString;//ConfigurationManager.ConnectionStrings["PhotoSaver.Properties.Settings.freedphotosorterConnectionString"].ConnectionString;
            SqlConnection oConn = new SqlConnection(thisConnectionString);
            oConn.Open();                                                       // Tempory until DAAB implementation
            SqlCommand oCmd = new SqlCommand(storedProcName, oConn);
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandTimeout = 120;
            SqlDataAdapter adp = new SqlDataAdapter(oCmd);
            adp.Fill(ds);
            ds.Dispose();
            oConn.Close();
            return ds;
        }
    }
}
