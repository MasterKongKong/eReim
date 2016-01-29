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
    public partial class RoleUser : System.Web.UI.Page
    {
        DBAdapter dbs = new DBAdapter();
        protected void Page_Load(object sender, EventArgs e)
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
                if (!IsPostBack)
                {
                    UDB();
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "", "$('div.gn_person ul.q-menubox li:eq(0) a').text('" + Session["UserName"].ToString() + "');", true);
                X.AddScript("loginWindow.hide();Panel1.enable();");
            }
           
        }
        public void UDB()
        {
            DataSet ds = dbs.GetSqlDataSet("select id,UserID,UserName from GroupUsers where gid=" + Request.QueryString["id"].ToString());
            TXTStation.Text = dbs.ExeSqlScalar("select station from GroupType where id=" + Request.QueryString["id"].ToString());
            BDUser.DataSource=ds.Tables[0];
            BDUser.DataBind();
        }

        protected void back(object sender, DirectEventArgs e)
        {
            Response.Redirect("Role.aspx");
        }
        protected void BTN_Search(object sender, DirectEventArgs e)
        {
            DataSet ds = DIMERCO.SDK.Utilities.LSDK.getUserDataBYStationCodeNUser(TXTStation.Text.ToString(), TXTUserID.Text.Trim(), TXTUserName.Text.Trim());
            //SqlConnection cn = new SqlConnection("Data Source=10.130.40.20;Initial Catalog=ReSM;User ID=sa;Password=dim1rc0@");

            //string sql = "select UserID,fullname from  SMUser inner join SMstation on SMUser.stationid=SMstation.stationid where StationCode='" + TXTStation.Text + "'";
            //if(TXTUserID.Text.Trim()!="")
            //{
            //sql=sql+" and UserID='"+TXTUserID.Text.Trim()+"'";
            //}
            //if(TXTUserName.Text.Trim()!="")
            //{
            //    sql = sql + " and fullname like '%" + TXTUserName.Text.Trim() + "%'";
            //}
            //SqlDataAdapter da = new SqlDataAdapter(sql, cn);
            //DataSet ds = new DataSet();
            //da.Fill(ds);
            DataTable tb = ds.Tables[0];
            SVUser.DataSource = tb;
            SVUser.DataBind();
            ds.Dispose();

        }
        protected void BTN_Save(object sender, DirectEventArgs e)
        {
            //RowSelectionModel sm = this.GridPanel2.SelectionModel.Primary as RowSelectionModel;

            //foreach (SelectedRow row in sm.SelectedRows)
            //{
            //    object d = row;
            //}

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
           
         dbs.ExeSql("Delete from GroupUsers where id="+ID);
             
            UDB();
        }
        [DirectMethod]
        public void LogCompanyInfo(string fullname,string UserID)
        {
            string[] st = fullname.Split(',');
            string[] stUserID = UserID.Split(',');
            string thename = "";
            for (int i = 0; i < st.Length-1; i++)
            {
                int e = ExitUser(stUserID[i].ToString());
                if (e <= 0)
                {
                    dbs.ExeSql("insert into GroupUsers(GID,UserID,UserName) values('" + Request.QueryString["id"].ToString() + "','" + stUserID[i].ToString() + "','" + st[i].ToString() + "')");
              
                
                }
                if (e > 0)
                { thename = thename + st[i].ToString()+","; } 
            }
            UDB();
             string msss="Save success";
            if(thename!="")
            {msss =thename+" was in group";
            
            }
            X.Msg.Show(new MessageBoxConfig
            {
               
                Title = "Message",
                Message = msss,
                Buttons = MessageBox.Button.OK,
                Width = 320,
                Icon = MessageBox.Icon.INFO
            });
        }
        public int ExitUser(string fullname)
        {
            return int.Parse(dbs.ExeSqlScalar("select count(*) from GroupUsers where UserID='" + fullname + "'"));
        }

    }
}