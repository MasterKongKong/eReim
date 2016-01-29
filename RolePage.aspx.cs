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
    public partial class RolePage : System.Web.UI.Page
    {
        DBAdapter dbs = new DBAdapter();
        protected override void CreateChildControls()
        {
            if (Session["UserID"] == null)
            {

                Response.Redirect("RoleModule.aspx");
                return;

            }
            else if (Comm.JdRole(Request.ServerVariables["URL"].ToString()) == 0)
            {
                Response.Redirect("RoleModule.aspx");
                return;
            }
            else
            {
                base.CreateChildControls();
                DDB();
                ScriptManager.RegisterStartupScript(this, GetType(), "", "$('div.gn_person ul.q-menubox li:eq(0) a').text('" + Session["UserName"].ToString() + "');", true);
                X.AddScript("loginWindow.hide();Panel1.enable();");
            }
            //动态生成的方法;
          
        }
        protected void BACK(object sender, DirectEventArgs e)
        {
            Response.Redirect("RoleStation.aspx?Uid=" + Request.QueryString["Uid"].ToString() + "&Unm=" + Request.QueryString["Unm"].ToString() + "&S=" + Request.QueryString["S"].ToString() + "");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                TXTUserID.Text = Request.QueryString["Uid"].ToString();
                TXTName.Text = Request.QueryString["Unm"].ToString();
            }
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

        public void DDB()
        {
       

           DataSet Mds = dbs.GetSqlDataSet("select id,ModuleName from ModuleManage");

           for (int i = 0; i < Mds.Tables[0].Rows.Count; i++)
           {
               Ext.Net.Checkbox C1 = new Ext.Net.Checkbox();
               C1.ID = Mds.Tables[0].Rows[i]["ID"].ToString();
               C1.BoxLabel = Mds.Tables[0].Rows[i]["ModuleName"].ToString();
               int Tct=int.Parse(dbs.ExeSqlScalar("select count(*) from StationRole where  UserID='" + Request.QueryString["Uid"].ToString() + "' and  charindex('," +  Mds.Tables[0].Rows[i]["id"].ToString() + ",',ModuleID)>0"));
               if (Tct > 0)
               {
                   C1.Checked = true;
               }

               ChkGrp.Items.Add(C1);
           }
                
        }
        protected void SAVE_Search(object sender, DirectEventArgs e)
        {


            string sck = ",";
            ChkGrp.CheckedItems.ForEach(delegate(Checkbox checkbox)
            {

                sck = sck + checkbox.ID.ToString() + ",";


            });
            int ct = int.Parse(dbs.ExeSqlScalar("select count(*) from StationRole where UserID='" + Request.QueryString["Uid"].ToString() + "'"));
            if (ct > 0)
            {
                string sql = "update StationRole set ModuleID='" + sck + "',createdby='" + Session["UserID"].ToString() + "', createdDate='" + DateTime.Now.ToString() + "' where UserID='" + Request.QueryString["Uid"].ToString() + "'";
                int r = dbs.ExeSql(sql);
                if (r > 0)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Message",
                        Message = "Save success",
                        Buttons = MessageBox.Button.OK,
                        Width = 320,
                        Icon = MessageBox.Icon.INFO
                    });
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Message",
                        Message = "Save fail",
                        Buttons = MessageBox.Button.OK,
                        Width = 320,
                        Icon = MessageBox.Icon.INFO
                    });
                }

            }
            else
            {
                string sql = " insert into StationRole(UserID,ModuleID,UserName,Admin,station,createdby,createdDate)  values ('" + Request.QueryString["Uid"].ToString() + "','" + sck + "','" + Request.QueryString["Unm"].ToString() + "','0','" + Request.QueryString["S"].ToString() + "','" + Session["UserID"].ToString() + "','" + DateTime.Now.ToString() + "') ";
                int r = dbs.ExeSql(sql);
                if (r > 0)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Message",
                        Message = "Save success",
                        Buttons = MessageBox.Button.OK,
                        Width = 320,
                        Icon = MessageBox.Icon.INFO
                    });
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Message",
                        Message = "Save fail",
                        Buttons = MessageBox.Button.OK,
                        Width = 320,
                        Icon = MessageBox.Icon.INFO
                    });
                }
            }

        }
    }
}