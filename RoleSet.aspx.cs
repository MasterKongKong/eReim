using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using DBOperatorSql;
using System.Data;
using System.Data.SqlClient;

namespace eReimbursement
{
    public partial class RoleSet : System.Web.UI.Page
    {
        DBAdapter dbs = new DBAdapter();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] == null)
                {

                    Response.Redirect("Role.aspx");
                    return;

                }
                else if (Comm.JdRole(Request.ServerVariables["URL"].ToString()) == 0)
                {
                    Response.Redirect("Role.aspx");
                    return;
                }
                else
                {
                    DB();
                    DLStep.SetValue(MaxStep());
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "$('div.gn_person ul.q-menubox li:eq(0) a').text('" + Session["UserName"].ToString() + "');", true);
                    X.AddScript("loginWindow.hide();Panel1.enable();");
                }
            }
          
        }
        protected void GetUser(object sender, DirectEventArgs e)
        {
            if (X.GetValue("DLUser").Length > 2)
            {
                DataSet ds = DIMERCO.SDK.Utilities.LSDK.getUserDataBYUserName(X.GetValue("DLUser"), 10);
                //SqlConnection cn = new SqlConnection("Data Source=10.130.40.20;Initial Catalog=ReSM;User ID=sa;Password=dim1rc0@");
                //string sql = "select distinct top 10 UserID,fullname from smuser where fullname like '%" + X.GetValue("DLUser") + "%'";
                //SqlDataAdapter da = new SqlDataAdapter(sql, cn);
                //DataSet ds = new DataSet();
                //da.Fill(ds);
                DataTable tb = ds.Tables[0];
                SUser.DataSource = tb;
                SUser.DataBind();
                ds.Dispose();
            }

        }
        protected void btnBack(object sender, DirectEventArgs e)
        {
            Response.Redirect("Role.aspx");
        }
        protected void BTN_AddUser(object sender, DirectEventArgs e)
        {
            int cc = int.Parse(dbs.ExeSqlScalar("select count(*) from GroupFlow where type='"+Request.QueryString["M"].ToString()+"' and  Gid=" + Request.QueryString["id"].ToString() + " and FlowNo=" + DLStep.Value));
            if (cc <= 0)
            {
                if (DLUser.Text.Trim() != "")
                {
                    string uid = DLUser.Value.ToString();
                    if (DLUser.SelectedItem.Text.ToString() == "Paul Lee")
                    {
                        uid = "A0104";
                    }
                    dbs.ExeSql("insert into GroupFlow(Gid,FlowNo,FlowUser,FlowUserID,Type,Fn) values(" + Request.QueryString["id"].ToString() + "," + DLStep.Value + ",'" + DLUser.SelectedItem.Text + "','" + uid + "','" + Request.QueryString["M"].ToString() + "','" + ff.Value + "')");

                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Message",
                        Message = "Please input Approver ID",
                        Buttons = MessageBox.Button.OK,
                        Width = 320,
                        Icon = MessageBox.Icon.INFO
                    });
                }
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Message",
                    Message = "The flow No. is exist",
                    Buttons = MessageBox.Button.OK,
                    Width = 320,
                    Icon = MessageBox.Icon.INFO
                });
            }
            DB();
        }
        public int MaxStep()
        {
            string max = dbs.ExeSqlScalar("select max(FlowNo) from GroupFlow where type='" + Request.QueryString["M"].ToString() + "' and  gid=" + Request.QueryString["id"].ToString());
            if (max == "")
            {
                max = "0";
            }
            return int.Parse(max) + 1;
        }
        public void DB()
        {
            DataSet ds = dbs.GetSqlDataSet("select id,Gid,FlowNo,FlowUser,Fn,FlowUserID,Remark from GroupFlow where type='" + Request.QueryString["M"].ToString() + "' and  gid=" + Request.QueryString["id"].ToString() + " order by flowno");
            SGroupFlow.DataSource = ds.Tables[0];
            SGroupFlow.DataBind();
        
        }
        protected void btnLogin_Click(object sender, DirectEventArgs e)
        {
            DataSet ds = new DataSet();
            bool user = DIMERCO.SDK.Utilities.ReSM.CheckUserInfo(tfUserID.Text.Trim(), tfPW.Text.Trim(), ref ds);
            if (ds.Tables[0].Rows.Count == 1)
            {
                DataTable dtuser = ds.Tables[0];
                Session["UserID"] = dtuser.Rows[0]["UserID"].ToString();
                DataSet ds1 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtuser.Rows[0]["UserID"].ToString());
                if (ds1.Tables[0].Rows.Count == 1)
                {
                    DataTable dt1 = ds1.Tables[0];
                    Session["UserName"] = dt1.Rows[0]["fullName"].ToString();
                    Session["Station"] = dt1.Rows[0]["stationCode"].ToString();
                    Session["Department"] = dt1.Rows[0]["DepartmentName"].ToString();
                    Session["CostCenter"] = dt1.Rows[0]["CostCenter"].ToString();
                    X.AddScript("window.location.reload();");
                }
                else
                {
                    X.Msg.Alert("Message", "Data Error.").Show();
                    return;
                }
            }
            else
            {
                X.Msg.Alert("Message", "Please confirm your UserID and Password.").Show();
                return;
            }
            if (ds != null)
            {
                ds.Dispose();
            }
        }
        [DirectMethod]
        public void DeleUsers(string ID)
        {

            dbs.ExeSql("Delete from GroupFlow where id=" + ID);

            DB();
        }
    }
   
}