using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace eReimbursement
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public DataTable GetData(string sqlcon, string command)
        {
            try
            {
                ListBox1.Items.Add(sqlcon);
                string sConnection = ConfigurationManager.ConnectionStrings[sqlcon].ConnectionString;
                DataSet dsSet = new DataSet();
                SqlDataAdapter sdaAdapter = null;
                //SqlCommandBuilder scbBuilder = null;
                //建立Connection
                SqlConnection scConnection = new SqlConnection(sConnection);

                ListBox1.Items.Add("Open Connection:Start");
                scConnection.Open();
                ListBox1.Items.Add("Open Connection:Complete");
                ListBox1.Items.Add("Open Connection:" + sConnection);
                //建立Command
                SqlCommand scCommand = scConnection.CreateCommand();
                scCommand.CommandText = command;
                //建立Adapter
                sdaAdapter = new SqlDataAdapter(scCommand);

                ListBox1.Items.Add("CommandText:" + scCommand.CommandText);
                //该对象负责生成用于更新数据库的SQL语句，不必自己创建这些语句
                //scbBuilder = new SqlCommandBuilder(sdaAdapter);

                //得到数据
                ListBox1.Items.Add("Fill:Start");
                sdaAdapter.Fill(dsSet, "Request");
                ListBox1.Items.Add("Fill:Complete");
                sdaAdapter.Dispose();
                scCommand.Dispose();
                scConnection.Close();
                scConnection.Dispose();
                return dsSet.Tables[0];
            }
            catch (Exception ex)
            {
                ListBox1.Items.Add("Error:" + ex.Message);
            }
            return null;
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            string sqlitem = "select * from ESUser where UserID='A2232'";

            try
            {
                DataTable dtitem = new DataTable();
                ListBox1.Items.Add("GetData:Start");
                dtitem = GetData("eReimbursement", sqlitem);
                ListBox1.Items.Add("GetData:Complete");

                if (dtitem != null)
                {
                    foreach (DataColumn col in dtitem.Columns)
                    {
                        ListBox1.Items.Add("Column:" + col.ColumnName);
                    }

                    if (dtitem.Rows.Count > 0)
                    {
                        string userid = dtitem.Rows[0]["Userid"].ToString();
                    }
                }
                else
                {
                    ListBox1.Items.Add("dtitem is null.");
                }

            }
            catch (Exception ex)
            {
                ListBox1.Items.Add("Button1_Click" + ex.Message);
            }
        }
    }
}