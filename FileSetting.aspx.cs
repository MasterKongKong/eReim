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
    public partial class FileSetting : App_Code.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                //cs.ZipFloClass zip = new cs.ZipFloClass();
                //string filename = "TR20151111110108.xlsx";
                //string filepath = System.Web.HttpContext.Current.Request.MapPath("Upload/") + "TR20151111110108.xlsx";
                //string zipfilepath = System.Web.HttpContext.Current.Request.MapPath("Upload/") + "TR20151111110108.zip";
                //zip.ZipSingleFile(filename, filepath, zipfilepath);
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

                    //获取被授权站点
                    string getright = "select * from StationRole where UserID='" + Request.Cookies.Get("eReimUserID").Value + "'";
                    DataTable dtright = dbc.GetData("eReimbursement", getright);
                    DataTable dtStation = new DataTable();
                    dtStation.Columns.Add("Text", System.Type.GetType("System.String"));
                    dtStation.Columns.Add("Value", System.Type.GetType("System.String"));
                    for (int j = 0; j < dtright.Rows.Count; j++)
                    {
                        string[] dd = dtright.Rows[j]["Stations"].ToString().Split(',');
                        for (int i = 0; i < dd.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(dd[i].Trim()))
                            {
                                bool have = false;
                                for (int g = 0; g < dtStation.Rows.Count; g++)
                                {
                                    if (dtStation.Rows[g]["Text"].ToString() == dd[i].Trim())
                                    {
                                        have = true;
                                        break;
                                    }
                                }
                                if (!have)
                                {
                                    DataRow dr1 = dtStation.NewRow();
                                    dr1["Text"] = dd[i];
                                    dr1["Value"] = dd[i];
                                    dtStation.Rows.Add(dr1);
                                }
                            }
                        }
                    }
                    dtStation.DefaultView.Sort = "Text ASC";
                    StoreStation.DataSource = dtStation.DefaultView.ToTable();
                    StoreStation.DataBind();
                    cbxStation.Text = Request.Cookies.Get("eReimStation").Value.ToString();

                    //获取站点文件后缀列表
                    RefreshList(Request.Cookies.Get("eReimStation").Value.ToString());
                    //string getpostfix = "select * from FileRule where StationCode='" + Request.Cookies.Get("eReimStation").Value.ToString() + "'";
                    //DataTable dtPostfix = dbc.GetData("eReimbursement", getpostfix);
                    //dtPostfix.Columns.Add("Allow1");
                    //for (int i = 0; i < dtPostfix.Rows.Count; i++)
                    //{
                    //    dtPostfix.Rows[i]["Allow1"] = dtPostfix.Rows[i]["Allow"].ToString() == "0" ? "NO" : "YES";
                    //}
                    //StoreCCList.DataSource = dtPostfix;
                    //StoreCCList.DataBind();

                    //string sql = "select MailList as Email from MailSetting where UserID='" + Request.Cookies.Get("eReimUserID").Value + "' and MailList!=''";
                    //DataTable dtdetail = new DataTable();
                    //dtdetail = dbc.GetData("eReimbursement", sql);
                    //if (dtdetail != null && dtdetail.Rows.Count == 1)
                    //{
                    //    string[] mc = dtdetail.Rows[0]["Email"].ToString().Split(',');
                    //    DataTable dt = new DataTable();
                    //    dt.Columns.Add("Email");
                    //    for (int i = 0; i < mc.Length; i++)
                    //    {
                    //        DataRow dr = dt.NewRow();
                    //        dr["Email"] = mc[i];
                    //        dt.Rows.Add(dr);
                    //    }
                    //    StoreCCList.DataSource = dt;
                    //    StoreCCList.DataBind();
                    //}
                    //DataTable dt = new DataTable();
                    //dt.Columns.Add("Postfix");
                    //dt.Columns.Add("Station");
                    //DataRow dr = dt.NewRow();
                    //dr[0] = "xls";
                    //dr[1] = "ZJDTSN";
                    //dt.Rows.Add(dr);

                    //dr = dt.NewRow();
                    //dr[0] = "xlsx";
                    //dr[1] = "ZJDTSN";
                    //dt.Rows.Add(dr);

                    //dr = dt.NewRow();
                    //dr[0] = "doc";
                    //dr[1] = "ZJDTSN";
                    //dt.Rows.Add(dr);

                    //dr = dt.NewRow();
                    //dr[0] = "docx";
                    //dr[1] = "ZJDTSN";
                    //dt.Rows.Add(dr);

                    //dr = dt.NewRow();
                    //dr[0] = "zip";
                    //dr[1] = "ZJDTSN";
                    //dt.Rows.Add(dr);

                    //dr = dt.NewRow();
                    //dr[0] = "rar";
                    //dr[1] = "ZJDTSN";
                    //dt.Rows.Add(dr);

                    //dr = dt.NewRow();
                    //dr[0] = "txt";
                    //dr[1] = "ZJDTSN";
                    //dt.Rows.Add(dr);

                    //dr = dt.NewRow();
                    //dr[0] = "pdf";
                    //dr[1] = "ZJDTSN";
                    //dt.Rows.Add(dr);

                    //dr = dt.NewRow();
                    //dr[0] = "#exe";
                    //dr[1] = "ZJDTSN";
                    //dt.Rows.Add(dr);

                    //dr = dt.NewRow();
                    //dr[0] = "#";
                    //dr[1] = "ZJDTSN";
                    //dt.Rows.Add(dr);

                    //StoreCCList.DataSource = dt;
                    //StoreCCList.DataBind();
                }
            }
        }
        [DirectMethod]
        public void DeleteRule(string TempNO)
        {
            cs.DBCommand dbc = new cs.DBCommand();
            string sql = "delete from FileRule where ID=" + TempNO;
            string sqlre = dbc.UpdateData("eReimbursement", sql, "Update");
        }
        [DirectMethod]
        public void AddPostfix()
        {
            string sConnection = ConfigurationManager.ConnectionStrings["eReimbursement"].ConnectionString;
            SqlConnection DbConnection = new SqlConnection(sConnection);
            try
            {
                SqlCommand command = new SqlCommand("FileRule_Update", DbConnection);

                command.CommandType = CommandType.StoredProcedure;

                SqlParameter sp = new SqlParameter("@StationCode", SqlDbType.VarChar, 50);
                sp.Value = cbxStation.Text;
                command.Parameters.Add(sp);

                string fix = txtFilePostFix.Text.Trim().ToLower();
                int findex = fix.IndexOf('#');
                int allow = 1;
                string postfix = "";
                if (findex != -1)
                {
                    allow = 0;
                    if (fix.Length>1)
                    {
                        postfix = fix.Substring(1, fix.Length - 1);
                    }
                }
                else
                {
                    allow = 1;
                    postfix = fix;
                }

                sp = new SqlParameter("@Postfix", SqlDbType.VarChar, 50);
                sp.Value = fix;
                command.Parameters.Add(sp);

                sp = new SqlParameter("@PostfixRaw", SqlDbType.VarChar, 50);
                sp.Value = postfix;
                command.Parameters.Add(sp);

                sp = new SqlParameter("@Allow", SqlDbType.Int);
                sp.Value = allow;
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

                //刷新列表
                RefreshList(cbxStation.Text);
            }
            catch (Exception ex)
            {
                errormailfuc(ex);
                throw;
            }
        }
        private void RefreshList(string StationCode)
        {
            //获取站点文件后缀列表
            cs.DBCommand dbc = new cs.DBCommand();
            string getpostfix = "select * from FileRule where StationCode='" + StationCode + "'";
            DataTable dtPostfix = dbc.GetData("eReimbursement", getpostfix);
            dtPostfix.Columns.Add("Allow1");
            for (int i = 0; i < dtPostfix.Rows.Count; i++)
            {
                dtPostfix.Rows[i]["Allow1"] = dtPostfix.Rows[i]["Allow"].ToString() == "0" ? "NO" : "YES";
            }
            StoreCCList.DataSource = dtPostfix;
            StoreCCList.DataBind();
        }
        private void errormailfuc(Exception ex)
        {
            DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();

            mail.FromDispName = "eReim";
            mail.From = "DIC2@dimerco.com";
            mail.To = ConfigurationManager.AppSettings["errormailto"];
            mail.Title = "eReim Bug" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:dd");
            mail.Body = ex.Message + "<br/>" + (ex.InnerException == null ? "" : ex.InnerException.ToString()) + "</div>";
            mail.Send();
        }
        protected void ReFresh(object sender, DirectEventArgs e)
        {
            RefreshList(cbxStation.Text);
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
        
    }
}