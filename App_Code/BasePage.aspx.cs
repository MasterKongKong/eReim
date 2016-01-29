using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using System.Globalization;

namespace eReimbursement.App_Code
{
    public partial class BasePage : System.Web.UI.Page
    {
        protected override void InitializeCulture()
        {
            HttpCookie cookie = new HttpCookie("lang");
            //if (Request.QueryString["lang"] != null && Request.Cookies["lang"] == null)
            //{
            //    String lang = Request.QueryString["lang"];
            //    cookie.Value = Request.QueryString["lang"];
            //    Request.Cookies.Clear();
            //    Request.Cookies.Add(cookie);
            //    Response.Cookies.Clear();
            //    Response.Cookies.Add(cookie);
            //    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
            //    Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            //}
            //else if (Request.QueryString["lang"] == null && (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value != ""))
            //{
            //    //cookie = Request.Cookies["lang"];
            //    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Request.Cookies["lang"].Value);
            //    Thread.CurrentThread.CurrentUICulture = new CultureInfo(Request.Cookies["lang"].Value);
            //}
            //else if (Request.QueryString["lang"] == null && Request.Cookies["lang"] == null)
            //{
            //    cookie.Value = Request.UserLanguages[0];
            //    Request.Cookies.Clear();
            //    Request.Cookies.Add(cookie);
            //    Response.Cookies.Clear();
            //    Response.Cookies.Add(cookie);
            //    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cookie.Value);
            //    Thread.CurrentThread.CurrentUICulture = new CultureInfo(cookie.Value);
            //}
            //else
            //{
            //    cookie.Value = Request.QueryString["lang"];
            //    Request.Cookies.Clear();
            //    Request.Cookies.Add(cookie);
            //    Response.Cookies.Clear();
            //    Response.Cookies.Add(cookie);
            //    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cookie.Value);
            //    Thread.CurrentThread.CurrentUICulture = new CultureInfo(cookie.Value);
            //}
            

            //if (Request.QueryString["lang"] != null && (Request.QueryString["lang"].ToLower() == "en-us" || Request.QueryString["lang"].ToLower() == "zh-cn"))
            //{
            //    cookie.Value = Request.QueryString["lang"];
            //    Request.Cookies.Remove("lang");
            //    Request.Cookies.Add(cookie);
            //    Response.Cookies.Remove("lang");
            //    Response.Cookies.Add(cookie);
            //    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Request.QueryString["lang"]);
            //    Thread.CurrentThread.CurrentUICulture = new CultureInfo(Request.QueryString["lang"]);
            //}
            //else
            //{
            //    if (Request.Cookies["lang"] != null)
            //    {
            //        if (Request.Cookies["lang"].Value != null)
            //        {
            //            if (Request.Cookies["lang"].Value != "")
            //            {
            //                //Request.Cookies.Remove("lang");
            //                //Request.Cookies.Add(cookie);
            //                cookie.Value = Request.Cookies["lang"].Value;
            //                cookie.Expires = DateTime.Now.AddDays(1);
            //                Response.Cookies.Remove("lang");
            //                Response.Cookies.Add(cookie);
            //                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Request.Cookies["lang"].Value);
            //                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Request.Cookies["lang"].Value);
            //            }
            //            else
            //            {
            //                cookie.Value = "en-us";
            //                cookie.Expires = DateTime.Now.AddDays(1);
            //                Request.Cookies.Remove("lang");
            //                Request.Cookies.Add(cookie);
            //                Response.Cookies.Remove("lang");
            //                Response.Cookies.Add(cookie);
            //                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-us");
            //                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            //            }
            //        }
            //        else
            //        {
            //            cookie.Value = "en-us";
            //            cookie.Expires = DateTime.Now.AddDays(1);
            //            Request.Cookies.Remove("lang");
            //            Request.Cookies.Add(cookie);
            //            Response.Cookies.Remove("lang");
            //            Response.Cookies.Add(cookie);
            //            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-us");
            //            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            //        }
            //    }
            //    else
            //    {
            //        cookie.Value = "en-us";
            //        cookie.Expires = DateTime.Now.AddDays(1);
            //        Request.Cookies.Remove("lang");
            //        Request.Cookies.Add(cookie);
            //        Response.Cookies.Remove("lang");
            //        Response.Cookies.Add(cookie);
            //        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-us");
            //        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            //    }
            //}
            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value != null && Request.Cookies["lang"].Value != "" && (Request.Cookies["lang"].Value.ToLower() == "en-us" || Request.Cookies["lang"].Value.ToLower() == "zh-cn"))
            {
                //cookie.Value = Request.Cookies["lang"].Value;
                //cookie.Expires = DateTime.Now.AddDays(1);
                //Response.Cookies.Remove("lang");
                //Response.Cookies.Add(cookie);
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Request.Cookies["lang"].Value);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Request.Cookies["lang"].Value);
            }
            else
            {
                cookie.Value = "en-us";
                cookie.Expires = DateTime.MaxValue;
                Request.Cookies.Remove("lang");
                Request.Cookies.Add(cookie);
                Response.Cookies.Remove("lang");
                Response.Cookies.Add(cookie);
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-us");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            }
            base.InitializeCulture();
        }
    }
}