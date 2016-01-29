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
    public partial class RoleStation : System.Web.UI.Page
    {
        DBAdapter dbs = new DBAdapter();
        protected override void CreateChildControls()
        {
            //动态生成的方法;
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
                
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] != null)
                {

                    TXTUserID.Text = Request.QueryString["Uid"].ToString();
                    TXTName.Text = Request.QueryString["Unm"].ToString();
                }
                   // ScriptManager.RegisterStartupScript(this, GetType(), "", "$('div.gn_person ul.q-menubox li:eq(0) a').text('" + Session["UserName"].ToString() + "');loginWindow.hide();Panel1.enable();", true);
             
               // DDB();
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
            DataTable TTB;
            string TTstations = dbs.ExeSqlScalar("select stations from StationRole where UserID='" + Request.QueryString["Uid"].ToString() + "'");
            string Nct = dbs.ExeSqlScalar("select top 1 admin from StationRole where UserID='" + Session["UserID"].ToString() + "'").ToString();
        if (Nct == "1")
        {
            DataSet ds = DIMERCO.SDK.Utilities.LSDK.getStationHierarchy();
            //SqlConnection cn = new SqlConnection("Data Source=10.130.40.20;Initial Catalog=ReSM;User ID=sa;Password=dim1rc0@");
            //SqlDataAdapter da = new SqlDataAdapter("select distinct StationCode from SMStation order by StationCode", cn);
            //DataSet ds = new DataSet();
            //da.Fill(ds);
            TTB = ds.Tables[0];
        }
        else
        {
            string Tstations = dbs.ExeSqlScalar("select stations from StationRole where UserID='" + Session["UserID"] + "'");
            DataTable Dt = new DataTable();
            Dt.Columns.Add("StationCode");
            string[] Sp = Tstations.Split(',');
            for (int i = 0; i < Sp.Length; i++)
            {
                if (Sp[i].ToString().Trim() != "")
                {
                    DataRow newDr = Dt.NewRow();
                    newDr["StationCode"] = Sp[i].ToString();
                    Dt.Rows.Add(newDr);
                }
            }
            TTB = Dt;
           
        }
        DataTable drnw = new DataTable();
        TTB.DefaultView.Sort = "StationCode ASC";
        drnw = TTB.DefaultView.ToTable();
        for (int i = 0; i < drnw.Rows.Count; i++)
            {
                Ext.Net.Checkbox C1 = new Ext.Net.Checkbox();
                C1.ID = drnw.Rows[i]["StationCode"].ToString();
                C1.BoxLabel = drnw.Rows[i]["StationCode"].ToString();

                if (TTstations.Contains(drnw.Rows[i]["StationCode"].ToString()+","))
                {
                    C1.Checked = true;
                }
             
                ChkGrp.Items.Add(C1);
            }
        }
        protected void BACK(object sender, DirectEventArgs e)
        {
            Response.Redirect("RolePage.aspx?Uid=" + Request.QueryString["Uid"].ToString() + "&Unm=" + Request.QueryString["Unm"].ToString() + "&S=" + Request.QueryString["S"].ToString() + "");
        }
        protected void SAVE_Search(object sender, DirectEventArgs e)
        {
         
         
            string sck = "";
            ChkGrp.CheckedItems.ForEach(delegate(Checkbox checkbox)
                {

                    sck = sck + checkbox.BoxLabel.ToString() + ",";


                });
            int ct = int.Parse(dbs.ExeSqlScalar("select count(*) from StationRole where UserID='" + Request.QueryString["Uid"].ToString() + "'"));
            if (ct > 0)
            {
                string sql = "update StationRole set Stations='" + sck + "',createdby='" + Session["UserID"].ToString() + "', createdDate='" + DateTime.Now.ToString() + "' where UserID='" + Request.QueryString["Uid"].ToString() + "'";
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
                        Message = "Save Fail",
                        Buttons = MessageBox.Button.OK,
                        Width = 320,
                        Icon = MessageBox.Icon.INFO
                    });
                }
            
            }
            else
            {
                string sql = " insert into StationRole(UserID,Stations,UserName,Admin,station,createdby,createdDate)  values ('" + Request.QueryString["Uid"].ToString() + "','" + sck + "','" + Request.QueryString["Unm"].ToString() + "','0','" + Request.QueryString["S"].ToString() + "','" + Session["UserID"].ToString() + "','" + DateTime.Now.ToString() + "') ";
                int r = dbs.ExeSql(sql);
                if (r > 0)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Message",
                        Message = "保存成功",
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
                        Message = "保存失败",
                        Buttons = MessageBox.Button.OK,
                        Width = 320,
                        Icon = MessageBox.Icon.INFO
                    });
                }
            }
          
        }
    }
}