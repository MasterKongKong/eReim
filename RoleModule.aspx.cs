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
    public partial class RoleModule : System.Web.UI.Page
    {
        DBAdapter dbs = new DBAdapter();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {

                X.AddScript("loginWindow.show();Panel1.disable();");
                return;

            }
            else if (Comm.JdRole(Request.ServerVariables["URL"].ToString()) == 0)
            {
                X.AddScript("loginWindow.show();Panel1.disable();");
                return;
            }
                else
                {

                    ScriptManager.RegisterStartupScript(this, GetType(), "", "$('div.gn_person ul.q-menubox li:eq(0) a').text('" + Session["UserName"].ToString() + "');", true);
                    X.AddScript("loginWindow.hide();Panel1.enable();");
                if (!IsPostBack)
                    {
                        StationDB();
                    }
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

     public void StationDB()
        {
            string Nct = dbs.ExeSqlScalar("select top 1 admin from StationRole where UserID='" + Session["UserID"].ToString() + "'").ToString();
            if (Nct == "1")
            {
                DataSet ds = DIMERCO.SDK.Utilities.LSDK.getStationHierarchy();
                //SqlConnection cn = new SqlConnection("Data Source=10.130.40.20;Initial Catalog=ReSM;User ID=sa;Password=dim1rc0@");
                //SqlDataAdapter da = new SqlDataAdapter("select distinct StationCode from SMStation order by StationCode", cn);
                //DataSet ds = new DataSet();
                //da.Fill(ds);
                DataTable tb = ds.Tables[0];
                DataTable drnw = new DataTable();
                tb.DefaultView.Sort = "StationCode ASC";
                drnw = tb.DefaultView.ToTable();


                SStation.DataSource = drnw;
                SStation.DataBind();
                ds.Dispose();
                DLStation.SelectedIndex = 300;
            }
            else
            {
                string Tstations = dbs.ExeSqlScalar("select Stations from StationRole where UserID='" + Session["UserID"].ToString() + "'");
                DataTable Dt = new DataTable();
                Dt.Columns.Add("StationCode");
                string[] Sp = Tstations.Split(',');
                for(int i=0;i<Sp.Length;i++)
                {
                    if (Sp[i].ToString().Trim() != "")
                    {
                        DataRow newDr = Dt.NewRow();
                        newDr["StationCode"] = Sp[i].ToString();
                        Dt.Rows.Add(newDr);
                    }
                }
                DataTable tb = Dt;
                DataTable drnw = new DataTable();
                Dt.DefaultView.Sort = "StationCode ASC";
                drnw = Dt.DefaultView.ToTable();
                SStation.DataSource = drnw;
                SStation.DataBind();
                DLStation.SelectedIndex = 300;
               
            }
        }
     protected void BTN_Search(object sender, DirectEventArgs e)
        {
            if (DLStation.Value==null)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Message",
                    Message = "Please select station",
                    Buttons = MessageBox.Button.OK,
                    Width = 320,
                    Icon = MessageBox.Icon.INFO
                });
            }
            else
            {
                DataSet ds = DIMERCO.SDK.Utilities.LSDK.getUserDataBYStationCodeNUser(DLStation.Value.ToString().ToString(), TXTUserID.Text.Trim(), TXTUserName.Text.Trim());
                // SqlConnection cn = new SqlConnection("Data Source=10.130.40.20;Initial Catalog=ReSM;User ID=sa;Password=dim1rc0@");
                //string sql="select UserID,fullname from  SMUser inner join SMstation on SMUser.stationid=SMstation.stationid where StationCode='"+DLStation.Value+"'";
                // if(TXTUserID.Text.Trim()!="")
                // {
                // sql=sql+" and UserID='"+TXTUserID.Text.Trim()+"'";
                // }
                // if(TXTUserName.Text.Trim()!="")
                // {
                //     sql = sql + " and fullname like '%" + TXTUserName.Text.Trim() + "%'";
                // }
                // SqlDataAdapter da = new SqlDataAdapter(sql, cn);
                // DataSet ds = new DataSet();
                // da.Fill(ds);
                DataTable tb = ds.Tables[0];

                DataSet dss = dbs.GetSqlDataSet("select UserID,UserName,Stations,ModuleID from StationRole where station='" + DLStation.Value + "'");
                DataTable tb1 = dss.Tables[0];
                DataTable ttb = new DataTable();
                ttb.Columns.Add("UserID");
                ttb.Columns.Add("FullName");
                ttb.Columns.Add("station");
                ttb.Columns.Add("ModuleName");
                foreach (DataRow dr in tb.Rows)
                {
                    DataRow[] drn = tb1.Select("UserID='" + dr["UserID"].ToString() + "'");
                    if (drn.Length > 0)
                    {
                        DataRow newDr = ttb.NewRow();
                        newDr["UserID"] = drn[0]["UserID"];
                        newDr["FullName"] = drn[0]["UserName"];
                        newDr["station"] = drn[0]["Stations"];
                        newDr["ModuleName"] = GetModuleName(drn[0]["ModuleID"].ToString());
                        ttb.Rows.Add(newDr);
                    }
                    else
                    {
                        DataRow newDr = ttb.NewRow();
                        newDr["UserID"] = dr["UserID"];
                        newDr["FullName"] = dr["fullname"];
                        newDr["station"] = "";
                        newDr["ModuleName"] = "";
                        ttb.Rows.Add(newDr);

                    }
                }
                if (Session["TSQL"] != null && DLStation.Value == null)
                {
                    ttb = (DataTable)Session["TSQL"];
                }
                SVUser.DataSource = ttb;
                SVUser.DataBind();
                Session["TSQL"] = ttb;
                ds.Dispose();

            }
        }
     public string GetModuleName(string ModuleID)
     {
         string RtnModule = "";
         DataSet ds = dbs.GetSqlDataSet("select ModuleName from ModuleManage where id in(0" + ModuleID + "0)");
         for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
         {
             RtnModule =RtnModule+ ds.Tables[0].Rows[i]["ModuleName"].ToString() + ",";
         
         }
         return RtnModule;
     }
        }
}