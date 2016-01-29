using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using System.Data;
using System.Xml;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace eReimbursement
{
    public partial class MailSetting : App_Code.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                if (Request.Cookies.Get("eReimUserID") == null)
                {
                    X.AddScript("loginWindow.show();Panel1.disable();");
                    return;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "$('div.gn_person ul.q-menubox li:eq(0) a').text('" + Request.Cookies.Get("eReimUserName").Value + "');", true);
                    X.AddScript("loginWindow.hide();Panel1.enable();");

                    cs.DBCommand dbc = new cs.DBCommand();
                    string sql = "select MailList as Email from MailSetting where UserID='" + Request.Cookies.Get("eReimUserID").Value + "' and MailList!=''";
                    DataTable dtdetail = new DataTable();
                    dtdetail = dbc.GetData("eReimbursement", sql);
                    if (dtdetail!=null && dtdetail.Rows.Count==1)
                    {
                        string[] mc = dtdetail.Rows[0]["Email"].ToString().Split(',');
                        DataTable dt = new DataTable();
                        dt.Columns.Add("Email");
                        for (int i = 0; i < mc.Length; i++)
                        {
                            DataRow dr = dt.NewRow();
                            dr["Email"] = mc[i];
                            dt.Rows.Add(dr);
                        }
                        StoreCCList.DataSource = dt;
                        StoreCCList.DataBind();
                    }
                    
                }
            }
        }
        protected void GetEmail(object sender, DirectEventArgs e)
        {
            DataSet getCostCenterBYStationCode = DIMERCO.SDK.Utilities.LSDK.getUserDataBYUserName(X.GetValue("cbxCCName"), 10);
            DataTable dtCOACenter = (DataTable)getCostCenterBYStationCode.Tables[0];
            DataTable dtCOACenternew = new DataTable();
            dtCOACenternew.Columns.Add("Name", System.Type.GetType("System.String"));
            dtCOACenternew.Columns.Add("Email", System.Type.GetType("System.String"));
            for (int c = 0; c < dtCOACenter.Rows.Count; c++)
            {
                if (dtCOACenter.Rows[c][4].ToString().IndexOf('@') == -1)
                {
                    continue;
                }
                DataRow dr = dtCOACenternew.NewRow();
                dr["Name"] = dtCOACenter.Rows[c][1].ToString();
                dr["Email"] = dtCOACenter.Rows[c][4].ToString();
                dtCOACenternew.Rows.Add(dr);
            }
            StoreMail.DataSource = dtCOACenternew;
            StoreMail.DataBind();
        }
        [DirectMethod]
        public void Logout()
        {
            Session["UserID"] = null;
            Session["UserName"] = null;
            Session["Station"] = null;
            Session["Department"] = null;
            Session["CostCenter"] = null;

            HttpCookie cookie = new HttpCookie("eReimUserID", "");
            cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie);

            HttpCookie cookie1 = new HttpCookie("eReimUserName", "");
            cookie1.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie1);

            HttpCookie cookie2 = new HttpCookie("eReimStation", "");
            cookie2.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie2);

            HttpCookie cookie3 = new HttpCookie("eReimDepartment", "");
            cookie3.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie3);

            HttpCookie cookie4 = new HttpCookie("eReimCostCenter", "");
            cookie4.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie4);

            X.AddScript("window.location.reload();");
        }
        protected void btnLogin_Click(object sender, DirectEventArgs e)
        {
            cs.DBCommand dbc = new cs.DBCommand();
            DataSet ds = new DataSet();
            bool user = DIMERCO.SDK.Utilities.ReSM.CheckUserInfo(tfUserID.Text.Trim(), tfPW.Text.Trim(), ref ds);
            if (ds.Tables[0].Rows.Count == 1)
            {
                DataTable dtuser = ds.Tables[0];
                Session["UserID"] = dtuser.Rows[0]["UserID"].ToString();
                if (Request.Cookies["eReimUserID"] != null)
                {
                    Response.Cookies["eReimUserID"].Value = dtuser.Rows[0]["UserID"].ToString();  //将值写入到客户端硬盘Cookie
                    Response.Cookies["eReimUserID"].Expires = DateTime.Now.AddHours(12);//设置Cookie过期时间
                }
                else
                {
                    HttpCookie cookie = new HttpCookie("eReimUserID", dtuser.Rows[0]["UserID"].ToString());
                    cookie.Expires = DateTime.Now.AddHours(12);
                    Response.Cookies.Add(cookie);
                }

                DataSet ds1 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtuser.Rows[0]["UserID"].ToString());
                if (ds1.Tables[0].Rows.Count == 1)
                {
                    DataTable dt1 = ds1.Tables[0];
                    Session["UserName"] = dt1.Rows[0]["fullName"].ToString();
                    Session["Station"] = dt1.Rows[0]["stationCode"].ToString();
                    Session["Department"] = dt1.Rows[0]["CRPDepartmentName"].ToString();
                    Session["CostCenter"] = dt1.Rows[0]["CostCenter"].ToString();
                    if (Request.Cookies["eReimUserName"] != null)
                    {
                        Response.Cookies["eReimUserName"].Value = dt1.Rows[0]["fullName"].ToString();  //将值写入到客户端硬盘Cookie
                        Response.Cookies["eReimUserName"].Expires = DateTime.Now.AddHours(12);//设置Cookie过期时间
                    }
                    else
                    {
                        HttpCookie cookie = new HttpCookie("eReimUserName", dt1.Rows[0]["fullName"].ToString());
                        cookie.Expires = DateTime.Now.AddHours(12);
                        Response.Cookies.Add(cookie);
                    }
                    if (Request.Cookies["eReimStation"] != null)
                    {
                        Response.Cookies["eReimStation"].Value = dt1.Rows[0]["stationCode"].ToString();  //将值写入到客户端硬盘Cookie
                        Response.Cookies["eReimStation"].Expires = DateTime.Now.AddHours(12);//设置Cookie过期时间
                    }
                    else
                    {
                        HttpCookie cookie = new HttpCookie("eReimStation", dt1.Rows[0]["stationCode"].ToString());
                        cookie.Expires = DateTime.Now.AddHours(12);
                        Response.Cookies.Add(cookie);
                    }
                    if (Request.Cookies["eReimDepartment"] != null)
                    {
                        Response.Cookies["eReimDepartment"].Value = dt1.Rows[0]["CRPDepartmentName"].ToString();  //将值写入到客户端硬盘Cookie
                        Response.Cookies["eReimDepartment"].Expires = DateTime.Now.AddHours(12);//设置Cookie过期时间
                    }
                    else
                    {
                        HttpCookie cookie = new HttpCookie("eReimDepartment", dt1.Rows[0]["CRPDepartmentName"].ToString());
                        cookie.Expires = DateTime.Now.AddHours(12);
                        Response.Cookies.Add(cookie);
                    }
                    if (Request.Cookies["eReimCostCenter"] != null)
                    {
                        Response.Cookies["eReimCostCenter"].Value = dt1.Rows[0]["CostCenter"].ToString();  //将值写入到客户端硬盘Cookie
                        Response.Cookies["eReimCostCenter"].Expires = DateTime.Now.AddHours(12);//设置Cookie过期时间
                    }
                    else
                    {
                        HttpCookie cookie = new HttpCookie("eReimCostCenter", dt1.Rows[0]["CostCenter"].ToString());
                        cookie.Expires = DateTime.Now.AddHours(12);
                        Response.Cookies.Add(cookie);
                    }

                    //DataTable dttemp = new DataTable();
                    //string sqltemp = "select * from ESUSER where Userid='" + dtuser.Rows[0]["UserID"].ToString() + "'";
                    //dttemp = dbc.GetData("eReimbursement", sqltemp);
                    //if (dttemp.Rows.Count > 0)
                    //{
                    //    //Session["CostCenter"] = dttemp.Rows[0]["Station"].ToString();
                    //    if (Request.Cookies["eReimCostCenter"] != null)
                    //    {
                    //        Response.Cookies["eReimCostCenter"].Value = dttemp.Rows[0][3].ToString();  //将值写入到客户端硬盘Cookie
                    //        Response.Cookies["eReimCostCenter"].Expires = DateTime.Now.AddHours(12);//设置Cookie过期时间
                    //    }
                    //    else
                    //    {
                    //        HttpCookie cookie = new HttpCookie("eReimCostCenter", dttemp.Rows[0]["Station"].ToString());
                    //        cookie.Expires = DateTime.Now.AddHours(12);
                    //        Response.Cookies.Add(cookie);
                    //    }
                    //}
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
        [Serializable]
        public class CCMailList
        {
            [XmlElement("Email")]
            public string Email { get; set; }
        }
        [DirectMethod]
        public void Add(string address)
        {
            cs.DBCommand dbc = new cs.DBCommand();
            string sConnection = ConfigurationManager.ConnectionStrings["eReimbursement"].ConnectionString;
            SqlConnection DbConnection = new SqlConnection(sConnection);
            SqlCommand command = new SqlCommand("SP_MailSetting", DbConnection);

            command.CommandType = CommandType.StoredProcedure;

            SqlParameter sp = new SqlParameter("@UserID", SqlDbType.VarChar, 50);
            sp.Value = Request.Cookies.Get("eReimUserID").Value;
            command.Parameters.Add(sp);

            //处理抄送人列表
            string CCMailList = "";
            JavaScriptSerializer serd = new JavaScriptSerializer();
            List<CCMailList> CCMailList1 = serd.Deserialize<List<CCMailList>>(address);
            foreach (CCMailList mail in CCMailList1)
            {
                CCMailList += mail.Email + ",";
            }
            CCMailList = CCMailList.Length > 0 ? CCMailList.Substring(0, CCMailList.Length - 1) : "";

            sp = new SqlParameter("@MailList", SqlDbType.VarChar, 2000);
            sp.Value = CCMailList;
            command.Parameters.Add(sp);

            sp = new SqlParameter("@Return", SqlDbType.Int);
            sp.Direction = ParameterDirection.Output;
            command.Parameters.Add(sp);

            DbConnection.Open();
            int rowsAffected = command.ExecuteNonQuery();
            //int result = (int)command.Parameters["Returnvalue"].Value;
            //string rw = command.Parameters["@Return"].Value.ToString();
            int newID = (int)command.Parameters["@Return"].Value;
            DbConnection.Close();
        }
    }
}