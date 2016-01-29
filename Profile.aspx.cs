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
    public partial class Profile : System.Web.UI.Page
    {
        DBAdapter dbs = new DBAdapter();
        protected void Page_Load(object sender, EventArgs e)
        { 
           
            if (!IsPostBack)
            {
                if (Session["UserID"] == null)
                {
                    X.AddScript("loginWindow.show();Panel1.disable();");
                    return;
                }
                else
                {

                    ScriptManager.RegisterStartupScript(this, GetType(), "", "$('div.gn_person ul.q-menubox li:eq(0) a').text('" + Session["UserName"].ToString() + "');", true);
                    X.AddScript("loginWindow.hide();Panel1.enable();");
                
            DataSet ds= DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(Session["UserID"].ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                LBUserID.Text = ds.Tables[0].Rows[0]["userID"].ToString();
                LBUserName.Text = ds.Tables[0].Rows[0]["fullName"].ToString();
                LBStation.Text = ds.Tables[0].Rows[0]["StationCode"].ToString();
                LBMail.Text = ds.Tables[0].Rows[0]["eMail"].ToString();
                LBMail.Text = ds.Tables[0].Rows[0]["email"].ToString();
                LBTel.Text = ds.Tables[0].Rows[0]["Tel"].ToString();
                LBMobile.Text = ds.Tables[0].Rows[0]["Mobilephone"].ToString();
                LBDepartmentCode.Text = ds.Tables[0].Rows[0]["CRPDepartmentName"].ToString();
                Coststation.Text = ds.Tables[0].Rows[0]["CostCenter"].ToString();
                int sct =int.Parse(dbs.ExeSqlScalar("select count(*) from StationRole where UserID='" + Session["UserID"].ToString() + "' and admin='1'"));
                if (sct > 0)
                {
                    LBStations.Text = "Administrator";
                }
                else
                {
                    LBStations.Text = dbs.ExeSqlScalar("select Stations from StationRole where UserID='" + Session["UserID"].ToString() + "'");
                }
                string moduleids = dbs.ExeSqlScalar("select ModuleID from StationRole where UserID='" + Session["UserID"].ToString() + "'");
                if (moduleids == "")
                {
                    LBModules.Text = "报销申请,报销审批,影音管理";
                }
                else
                {
                    string ModuleNames = "报销申请,报销审批,影音管理,";
                DataSet dsss=dbs.GetSqlDataSet("select ModuleName from ModuleManage where id in(0"+moduleids+"0)");
                for (int i = 0; i < dsss.Tables[0].Rows.Count; i++)
                {

                    ModuleNames = ModuleNames + dsss.Tables[0].Rows[i]["ModuleName"].ToString() + ",";
                }
                LBModules.Text = ModuleNames;
                
                }
            }
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

    }
}