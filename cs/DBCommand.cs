using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Data;
using System.Xml;
using System.Text;
using System.Globalization;
using System.Data.SqlClient;
using System.Configuration;

namespace eReimbursement.cs
{
    public class DBCommand
    {
        public string UpdateData(string sqlcon, string command, string type)
        {
            if (type == "Insert")
            {
                try
                {

                    string sConnection = ConfigurationManager.ConnectionStrings[sqlcon].ConnectionString;
                    DataSet dsSet = new DataSet();
                    SqlDataAdapter sdaAdapter = null;
                    SqlConnection scConnection = new SqlConnection(sConnection);
                    
                    SqlCommand scCommand = scConnection.CreateCommand();
                    scCommand.CommandText = command;
                    scConnection.Open();
                    sdaAdapter = new SqlDataAdapter(scCommand);
                    sdaAdapter.Fill(dsSet, "Request");
                    sdaAdapter.Dispose();
                    scCommand.Dispose();
                    scConnection.Close();
                    scConnection.Dispose();
                    return dsSet.Tables[0].Rows[0][0].ToString();
                }
                catch (Exception ex)
                {
                    DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();

                    mail.FromDispName = "eReimbursement";
                    mail.From = "DIC2@dimerco.com";
                    mail.To = "Andy_Kang@dimerco.com";
                    mail.Title = "eReimbursement Bug" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:dd");
                    mail.Body = "<div>SQL Command: " + command + "<br /><br />" + ex.Message + "</div>";
                    mail.Send();
                    return "-1";
                }

            }
            else if (type == "Update")
            {
                try
                {
                    string sConnection = ConfigurationManager.ConnectionStrings[sqlcon].ConnectionString;
                    DataSet dsSet = new DataSet();
                    SqlConnection scConnection = new SqlConnection(sConnection);
                    scConnection.Open();
                    SqlCommand scCommand = scConnection.CreateCommand();
                    scCommand.CommandText = command;
                    int row = scCommand.ExecuteNonQuery();
                    scCommand.Dispose();
                    scConnection.Close();
                    scConnection.Dispose();
                    return "1";
                }
                catch (Exception ex)
                {
                    DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();

                    mail.FromDispName = "eReimbursement Bug";
                    mail.From = "DIC2@dimerco.com";
                    mail.To = "Andy_Kang@dimerco.com";
                    mail.Title = "eReimbursement Bug";
                    mail.Body = "<div>SQL Command: " + command + "<br /><br />" + ex.Message + "</div>";
                    mail.Send();
                    return "-1";
                }
            }
            return "0";
        }
        public DataTable GetData(string sqlcon, string command)
        {
            try
            {
                string sConnection = ConfigurationManager.ConnectionStrings[sqlcon].ConnectionString;
                DataSet dsSet = new DataSet();
                SqlDataAdapter sdaAdapter = null;
                //SqlCommandBuilder scbBuilder = null;
                //建立Connection
                SqlConnection scConnection = new SqlConnection(sConnection);
                scConnection.Open();
                //建立Command
                SqlCommand scCommand = scConnection.CreateCommand();
                scCommand.CommandText = command;
                //建立Adapter
                sdaAdapter = new SqlDataAdapter(scCommand);

                //该对象负责生成用于更新数据库的SQL语句，不必自己创建这些语句
                //scbBuilder = new SqlCommandBuilder(sdaAdapter);

                //得到数据
                sdaAdapter.Fill(dsSet, "Request");
                sdaAdapter.Dispose();
                scCommand.Dispose();
                scConnection.Close();
                scConnection.Dispose();
                return dsSet.Tables[0];
            }
            catch (Exception ex)
            {

                DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();

                mail.FromDispName = "eReimbursement";
                mail.From = "DIC2@dimerco.com";
                mail.To = "Andy_Kang@dimerco.com";
                mail.Title = "eReimbursement Bug" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:dd");
                mail.Body = ex.Message + "<br/></div>";
                mail.Send();
            }
            return null;
        }
        public string ConvertString(string tempstr)
        {
            string NewStr=tempstr.Replace("'",@"''");
            NewStr = NewStr.Replace("%",@"\%");
            NewStr = NewStr.Replace("_", @"\_");
            NewStr = NewStr.Replace("％", @"\％");
            NewStr = NewStr.Replace("＿", @"\＿");
            NewStr = NewStr.Replace("@", @"\@");
            NewStr = NewStr.Replace(@"\", @"\\");
            return NewStr;
        }
    }
}