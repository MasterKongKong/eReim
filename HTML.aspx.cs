using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;

namespace eReimbursement
{
    public partial class HTML : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!X.IsAjaxRequest)
            //{
            //    tooltip1.Html = "<div id='con1'><img src='http://ww3.sinaimg.cn/bmiddle/5e4de534jw1e27yaqpl6nj.jpg' alt='pic1' /></div>";
            //}
        }
        //protected void btnLogin_Click(object sender, DirectEventArgs e)
        //{
        //    cs.DBCommand dbc = new cs.DBCommand();
        //    DataSet ds1 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(tfUserID.Text);
        //    if (ds1.Tables[0].Rows.Count == 1)
        //    {
        //        DataTable dt1 = ds1.Tables[0];
        //        Session["UserID"] = dt1.Rows[0]["UserID"].ToString();
        //        Session["UserName"] = dt1.Rows[0]["fullName"].ToString();
        //        Session["Station"] = dt1.Rows[0]["stationCode"].ToString();
        //        Session["Department"] = dt1.Rows[0]["CRPDepartmentName"].ToString();
        //        Session["CostCenter"] = dt1.Rows[0]["CostCenter"].ToString();
        //        if (Request.Cookies["eReimUserID"] != null)
        //        {
        //            Response.Cookies["eReimUserID"].Value = dt1.Rows[0]["UserID"].ToString();  //将值写入到客户端硬盘Cookie
        //            Response.Cookies["eReimUserID"].Expires = DateTime.Now.AddHours(12);//设置Cookie过期时间
        //        }
        //        else
        //        {
        //            HttpCookie cookie = new HttpCookie("eReimUserID", dt1.Rows[0]["UserID"].ToString());
        //            cookie.Expires = DateTime.Now.AddHours(12);
        //            Response.Cookies.Add(cookie);
        //        }
        //        if (Request.Cookies["eReimUserName"] != null)
        //        {
        //            Response.Cookies["eReimUserName"].Value = dt1.Rows[0]["fullName"].ToString();  //将值写入到客户端硬盘Cookie
        //            Response.Cookies["eReimUserName"].Expires = DateTime.Now.AddHours(12);//设置Cookie过期时间
        //        }
        //        else
        //        {
        //            HttpCookie cookie = new HttpCookie("eReimUserName", dt1.Rows[0]["fullName"].ToString());
        //            cookie.Expires = DateTime.Now.AddHours(12);
        //            Response.Cookies.Add(cookie);
        //        }
        //        if (Request.Cookies["eReimStation"] != null)
        //        {
        //            Response.Cookies["eReimStation"].Value = dt1.Rows[0]["stationCode"].ToString();  //将值写入到客户端硬盘Cookie
        //            Response.Cookies["eReimStation"].Expires = DateTime.Now.AddHours(12);//设置Cookie过期时间
        //        }
        //        else
        //        {
        //            HttpCookie cookie = new HttpCookie("eReimStation", dt1.Rows[0]["stationCode"].ToString());
        //            cookie.Expires = DateTime.Now.AddHours(12);
        //            Response.Cookies.Add(cookie);
        //        }
        //        if (Request.Cookies["eReimDepartment"] != null)
        //        {
        //            Response.Cookies["eReimDepartment"].Value = dt1.Rows[0]["CRPDepartmentName"].ToString();  //将值写入到客户端硬盘Cookie
        //            Response.Cookies["eReimDepartment"].Expires = DateTime.Now.AddHours(12);//设置Cookie过期时间
        //        }
        //        else
        //        {
        //            HttpCookie cookie = new HttpCookie("eReimDepartment", dt1.Rows[0]["CRPDepartmentName"].ToString());
        //            cookie.Expires = DateTime.Now.AddHours(12);
        //            Response.Cookies.Add(cookie);
        //        }
        //        if (Request.Cookies["eReimCostCenter"] != null)
        //        {
        //            Response.Cookies["eReimCostCenter"].Value = dt1.Rows[0]["CostCenter"].ToString();  //将值写入到客户端硬盘Cookie
        //            Response.Cookies["eReimCostCenter"].Expires = DateTime.Now.AddHours(12);//设置Cookie过期时间
        //        }
        //        else
        //        {
        //            HttpCookie cookie = new HttpCookie("eReimCostCenter", dt1.Rows[0]["CostCenter"].ToString());
        //            cookie.Expires = DateTime.Now.AddHours(12);
        //            Response.Cookies.Add(cookie);
        //        }
        //        X.AddScript("window.location.reload();");
        //    }
        //    else
        //    {
        //        X.Msg.Alert("Message", "Data Error.").Show();
        //        return;
        //    }

        //}
    }
}