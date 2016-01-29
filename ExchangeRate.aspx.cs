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

namespace eReimbursement
{
    public partial class ExchangeRate : System.Web.UI.Page
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
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "$('div.gn_person ul.q-menubox li:eq(0) a').text('" + Request.Cookies.Get("eReimUserName").Value + "');", true); X.AddScript("loginWindow.hide();Panel1.enable();");
                }
                LabelUser.Text = Request.Cookies.Get("eReimUserName").Value.ToString();
                LabelCostCenter.Text = Request.Cookies.Get("eReimCostCenter").Value.ToString() + "(" + DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(Request.Cookies.Get("eReimCostCenter").Value.ToString()) + ")";

                cs.DBCommand dbc = new cs.DBCommand();

                DataTable dtCur = DIMERCO.SDK.Utilities.ReSM.GetCurrency().Tables[0];
                dtCur.DefaultView.Sort = "CurrencyCode ASC";
                DataTable dtCurNew = dtCur.DefaultView.ToTable();
                StoreCurOri.DataSource = dtCurNew;
                StoreCurOri.DataBind();

                StoreCurTar.DataSource = dtCurNew;
                StoreCurTar.DataBind();

                DataSet ds2 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(Request.Cookies.Get("eReimUserID").Value.ToString());
                string station = ""; string CostCenterCur = ""; string UserCur = "";
                if (ds2.Tables[0].Rows.Count == 1)
                {
                    DataTable dt1 = ds2.Tables[0];
                    station = dt1.Rows[0]["stationCode"].ToString();
                    CostCenterCur = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(station);
                    LabelCur.Text = CostCenterCur;
                    DataTable dttemp = new DataTable();
                    string sqltemp = "select * from ESUSER where Userid='" + Request.Cookies.Get("eReimUserID").Value.ToString() + "'";
                    dttemp = dbc.GetData("eReimbursement", sqltemp);
                    if (dttemp.Rows.Count > 0)
                    {
                        UserCur = dttemp.Rows[0]["Currency"].ToString();
                        LabelCur.Text = UserCur;
                    }
                    cbxOriCur.Text = UserCur;
                    cbxTarCur.Text = CostCenterCur;
                    decimal rate = System.Math.Round(DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(cbxTarCur.Text, cbxOriCur.Text, DateTime.Now.Year), 4);
                    LabelRate.Text = rate.ToString();
                    LabelEx.Text = "1000(" + cbxOriCur.Text + ") = " + System.Math.Round(rate * 1000, 2).ToString() + "(" + cbxTarCur.Text + ")";
                }

                
            }
        }
        protected void Exchange(object sender, DirectEventArgs e)
        {
            decimal rate = System.Math.Round(DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(cbxTarCur.Text, cbxOriCur.Text, Convert.ToInt16(cbxYear.Text)), 4);
            LabelRate.Text = rate.ToString();
            LabelEx.Text = "1000(" + cbxOriCur.Text + ") = " + System.Math.Round(rate * 1000, 2).ToString() + "(" + cbxTarCur.Text + ")";
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
            //DataTable dtCur = DIMERCO.SDK.Utilities.ReSM.GetCurrency().Tables[0];
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
                    //        HttpCookie cookie = new HttpCookie("eReimCostCenter", dttemp.Rows[0][3].ToString());
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