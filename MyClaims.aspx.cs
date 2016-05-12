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
using org.in2bits.MyXls;
namespace eReimbursement
{
    public partial class MyClaims : App_Code.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                //DataSet dsuser = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList("A0703");
                //if (dsuser.Tables[0].Rows.Count == 1)
                //{
                //    DataTable dt1 = dsuser.Tables[0];
                //    dt1 = null;
                //}

                //DataTable newr = Comm.RtnEB("A0703", "Administration", "DIMYVR", "DIMYVR", "62010910", "2014", "1");
                //DataSet ds3 = DIMERCO.SDK.Utilities.LSDK.getStationHierarchy();
                //for (int i = 0; i < ds3.Tables[0].Rows.Count; i++)
                //{
                //    if (ds3.Tables[0].Rows[i][0].ToString()=="GCRSHA")
                //    {
                //        string sw = ds3.Tables[0].Rows[i][0].ToString();
                //    }
                //}
                //DataTable dttt = ds3.Tables[0];
                //判断登录状态
                //Session["UserID"] = "A5236"; Session["UserName"] = "Angel Chen";
                //if (Request.Cookies["eReimUserID"] != null)
                //{
                //    Response.Cookies["eReimUserID"].Value = "A5236";  //将值写入到客户端硬盘Cookie
                //    Response.Cookies["eReimUserID"].Expires = DateTime.Now.AddHours(12);//设置Cookie过期时间
                //}
                //else
                //{
                //    HttpCookie cookie = new HttpCookie("eReimUserID", "A5236");
                //    cookie.Expires = DateTime.Now.AddHours(12);
                //    Response.Cookies.Add(cookie);
                //}
                //if (Request.Cookies["eReimUserName"] != null)
                //{
                //    Response.Cookies["eReimUserName"].Value = "Angel Chen";  //将值写入到客户端硬盘Cookie
                //    Response.Cookies["eReimUserName"].Expires = DateTime.Now.AddHours(12);//设置Cookie过期时间
                //}
                //else
                //{
                //    HttpCookie cookie = new HttpCookie("eReimUserName", "Angel Chen");
                //    cookie.Expires = DateTime.Now.AddHours(12);
                //    Response.Cookies.Add(cookie);
                //}

                cs.DBCommand dbc = new cs.DBCommand();
                if (Request.Cookies.Get("eReimUserID") == null)
                {
                    X.AddScript("loginWindow.show();Panel1.disable();");
                    return;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "$('div.gn_person ul.q-menubox li:eq(0) a').text('" + Request.Cookies.Get("eReimUserName").Value + "');", true); X.AddScript("loginWindow.hide();Panel1.enable();");
                }
                //准备下拉菜单内容
                Ext.Net.ListItem li = new Ext.Net.ListItem(Request.Cookies.Get("eReimUserName").Value, Request.Cookies.Get("eReimUserID").Value);
                cbxPerson.Items.Add(li);
                string sqlitem = "select * from Eagent where [St]=1 and [PAgentID]='" + Request.Cookies.Get("eReimUserID").Value + "'";

                try
                {
                    DataTable dtitem = new DataTable();
                    dtitem = dbc.GetData("eReimbursement", sqlitem);
                    int itemcount = 0;
                    if (dtitem!=null)
                    {
                        for (int j = 0; j < dtitem.Rows.Count; j++)
                        {
                            string sqlpara = sqlitem;
                            bool d1 = true;
                            bool d2 = false;
                            if (dtitem.Rows[j][5].ToString() != "")
                            {
                                //sqlpara += " and getdate()>='" + dtitem.Rows[j]["Bdate"].ToString() + "' ";
                                if (DateTime.Now >= Convert.ToDateTime(dtitem.Rows[j][5].ToString()))
                                {
                                    d1 = true;
                                }
                                else
                                {
                                    d1 = false;
                                }
                            }
                            if (dtitem.Rows[j][6].ToString() != "")
                            {
                                //sqlpara += " and getdate()<='" + dtitem.Rows[j]["Edate"].ToString() + "' ";
                                if (DateTime.Now <= Convert.ToDateTime(dtitem.Rows[j][6].ToString()))
                                {
                                    d2 = true;
                                }
                                else
                                {
                                    d2 = false;
                                }
                            }
                            if (d1 && d2)
                            {
                                li = new Ext.Net.ListItem(dtitem.Rows[j][1].ToString(), dtitem.Rows[j][2].ToString());
                                cbxPerson.Items.Add(li);
                                itemcount++;
                            }
                        }
                        if (itemcount < 1)
                        {
                            cbxPerson.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();

                        mail.FromDispName = "eReimbursement";
                        mail.From = "DIC2@dimerco.com";
                        mail.To = "Andy_Kang@dimerco.com";
                        mail.Title = "eReimbursement Bug" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:dd");
                        mail.Body = "<div>Error<br/>" + sqlitem + "</div>";
                        mail.Send();
                    }
                    
                }
                catch (Exception ex)
                {
                    DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();

                    mail.FromDispName = "eReimbursement";
                    mail.From = "DIC2@dimerco.com";
                    mail.To = "Andy_Kang@dimerco.com";
                    mail.Title = "eReimbursement Bug" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:dd");
                    mail.Body = ex.Message + "<br/>" + ex.InnerException.ToString() + sqlitem + "</div>";
                    mail.Send();
                }
                
                string sqltype = "";
                string sqldraft = "";
                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                {
                    PagingToolbar1.DisplayMsg = "显示 {0} - {1} of {2}";
                    ResourceManager1.Locale = "zh-CN";
                    sqltype += "select [Text]=CText,[Value]=CValue from Edic where KeyValue='MainType'";
                    sqldraft += "select [Text]=CText,[Value]=CValue from Edic where KeyValue='Type'";
                }
                else
                {
                    PagingToolbar1.DisplayMsg = "Displaying items {0} - {1} of {2}";
                    ResourceManager1.Locale = "en-US";
                    sqltype += "select [Text]=EText,[Value]=CValue from Edic where KeyValue='MainType'";
                    sqldraft += "select [Text]=EText,[Value]=CValue from Edic where KeyValue='Type'";
                }
                //if (Request.Cookies["lang"] != null)
                //{
                //    string lang = Request.Cookies["lang"].Value;
                //    if (lang.ToLower() == "en-us")
                //    {
                //        PagingToolbar1.DisplayMsg = "Displaying items {0} - {1} of {2}";
                //        ResourceManager1.Locale = "en-US";
                //        sqltype += "select [Text]=EText,[Value]=CValue from Edic where KeyValue='MainType'";
                //        sqldraft += "select [Text]=EText,[Value]=CValue from Edic where KeyValue='Type'";
                //    }
                //    else
                //    {
                //        PagingToolbar1.DisplayMsg = "显示 {0} - {1} of {2}";
                //        ResourceManager1.Locale = "zh-CN";
                //        sqltype += "select [Text]=CText,[Value]=CValue from Edic where KeyValue='MainType'";
                //        sqldraft += "select [Text]=CText,[Value]=CValue from Edic where KeyValue='Type'";
                //    }
                //}
                //else
                //{
                //    sqltype += "select [Text]=CText,[Value]=CValue from Edic where KeyValue='MainType'";
                //    sqldraft += "select [Text]=CText,[Value]=CValue from Edic where KeyValue='Type'";
                //}
                DataTable dttype = dbc.GetData("eReimbursement", sqltype);
                StoreType.DataSource = dttype;
                StoreType.DataBind();
                DataTable dtdraft = dbc.GetData("eReimbursement", sqldraft);
                StoreDraft.DataSource = dtdraft;
                StoreDraft.DataBind();

                //载入半年内申请
                string sqldate = " and ApplyDate >='" + DateTime.Now.AddMonths(-6).Date.ToString() + "' and ApplyDate <='" + DateTime.Now.AddDays(1).Date.ToString() + "'";
                BindData(sqldate);
            }
        }
        protected void Search(object sender, DirectEventArgs e)
        {
            string sql = "";
            cs.DBCommand dbc = new cs.DBCommand();

            //string sql = "select * from V_Eflow_ETravel where [Active]=1 or [Active]=2 ";

            if (!string.IsNullOrEmpty(txtNo.Text))
            {
                string rem = dbc.ConvertString(txtNo.Text).IndexOf('\\') == -1 ? (dbc.ConvertString(txtNo.Text) + "%'") : (dbc.ConvertString(txtNo.Text) + @"%' escape '\'");
                sql += "and ([No] like '%" + rem + ") ";
            }
            if (!string.IsNullOrEmpty(cbxType.Text))
            {
                sql += "and [Type]='" + cbxType.Text + "' ";
            }
            if (!string.IsNullOrEmpty(txtAmount1.Text))
            {
                sql += "and [Tamount]>='" + txtAmount1.Text + "' ";
            }
            if (!string.IsNullOrEmpty(txtAmount2.Text))
            {
                sql += "and [Tamount]<='" + txtAmount2.Text + "' ";
            }
            if (!string.IsNullOrEmpty(txtRemark.Text))
            {
                string rem = dbc.ConvertString(txtRemark.Text).IndexOf('\\') == -1 ? (dbc.ConvertString(txtRemark.Text) + "%'") : (dbc.ConvertString(txtRemark.Text) + @"%' escape '\'");
                sql += "and ([Remark] like '%" + rem + ") ";
            }
            if (!string.IsNullOrEmpty(dfTDate1.RawText))
            {
                sql += "and convert(varchar(10),CreadedDate,111)>='" + dfTDate1.RawText + "' ";
            }
            if (!string.IsNullOrEmpty(dfTDate2.RawText))
            {
                sql += "and convert(varchar(10),CreadedDate,111)<='" + dfTDate2.RawText + "' ";
            }
            if (!string.IsNullOrEmpty(cbxSubType.Text))
            {
                sql += cbxSubType.Text == "0" ? "and [Step]!=0 " : "and [Step]=0 ";
            }
            if (!string.IsNullOrEmpty(cbxPerson.Text))
            {
                sql += "and PersonID='" + cbxPerson.Text + "' ";
            }
            BindData(sql);
        }
        protected void BindData(string sql)
        {
            try
            {
                sql += " and (PersonID='" + Request.Cookies.Get("eReimUserID").Value + "' or CreadedByID='" + Request.Cookies.Get("eReimUserID").Value + "' ";
                //判断是否为代理人
                cs.DBCommand dbc = new cs.DBCommand();
                string ApplyIDByAgent = "";
                string sqlagent = "select * from Eagent where [St]=1 and [PAgentID]='" + Request.Cookies.Get("eReimUserID").Value + "' and getdate()<=Edate and getdate()>=Bdate";
                DataTable dtagent = dbc.GetData("eReimbursement", sqlagent);
                if (dtagent!=null && dtagent.Rows.Count > 0)
                {
                    ApplyIDByAgent += " or PersonID in (";
                    for (int g = 0; g < dtagent.Rows.Count; g++)
                    {
                        if (g != dtagent.Rows.Count - 1)
                        {
                            ApplyIDByAgent += "'" + dtagent.Rows[g]["OwnerID"].ToString() + "',";
                        }
                        else
                        {
                            ApplyIDByAgent += "'" + dtagent.Rows[g]["OwnerID"].ToString() + "'";
                        }
                    }
                    ApplyIDByAgent += ")";
                }

                sql += ApplyIDByAgent + ")";

                string sql1 = "select t1.*";
                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                {
                    sql1 += ",[Status1]=TDicStatus.CText,[Type1]=TDicMainType.CText,[Draft1]=TDicType.CText";
                }
                else
                {
                    sql1 += ",[Status1]=TDicStatus.EText,[Type1]=TDicMainType.EText,[Draft1]=TDicType.EText";
                }
                //if (Request.Cookies["lang"] != null)
                //{
                //    string lang = Request.Cookies["lang"].Value;
                //    if (lang.ToLower() == "en-us")
                //    {
                //        PagingToolbar1.DisplayMsg = "Displaying items {0} - {1} of {2}";
                //        ResourceManager1.Locale = "en-US";
                //        sql1 += ",[Status1]=TDicStatus.EText,[Type1]=TDicMainType.EText,[Draft1]=TDicType.EText";
                //    }
                //    else
                //    {
                //        PagingToolbar1.DisplayMsg = "显示 {0} - {1} of {2}";
                //        ResourceManager1.Locale = "zh-CN";
                //        sql1 += ",[Status1]=TDicStatus.CText,[Type1]=TDicMainType.CText,[Draft1]=TDicType.CText";
                //    }
                //}
                //else
                //{
                //    sql1 += ",[Status1]=TDicStatus.CText,[Type1]=TDicMainType.CText,[Draft1]=TDicType.CText";
                //}
                sql1 += " from (select [Draft]=case when [Status]=0 then 1 else 0 end,* from V_Eflow_ETravel where FlowID>16097 and (Active=1 or Active=2) " + sql + ") t1";
                sql1 += " left join (select * from Edic where KeyValue='MainType') TDicMainType on TDicMainType.CValue=t1.Type";
                sql1 += " left join (select * from Edic where KeyValue='Status') TDicStatus on TDicStatus.CValue=t1.Status";
                sql1 += " left join (select * from Edic where KeyValue='Type') TDicType on TDicType.CValue=t1.Draft  order by FlowID desc";

                DataTable dtdetail = new DataTable();
                dtdetail = dbc.GetData("eReimbursement", sql1);
                DataTable dtnew = new DataTable();
                dtnew.Columns.Add("FlowID", System.Type.GetType("System.String"));
                dtnew.Columns.Add("No", System.Type.GetType("System.String"));
                dtnew.Columns.Add("Type", System.Type.GetType("System.String"));
                dtnew.Columns.Add("Station", System.Type.GetType("System.String"));
                dtnew.Columns.Add("Department", System.Type.GetType("System.String"));
                dtnew.Columns.Add("Person", System.Type.GetType("System.String"));
                dtnew.Columns.Add("CreadedBy", System.Type.GetType("System.String"));
                dtnew.Columns.Add("CreadedDate", System.Type.GetType("System.String"));
                dtnew.Columns.Add("Tamount", System.Type.GetType("System.String"));
                dtnew.Columns.Add("Step", System.Type.GetType("System.String"));
                dtnew.Columns.Add("Status", System.Type.GetType("System.String"));
                dtnew.Columns.Add("Approver", System.Type.GetType("System.String"));
                dtnew.Columns.Add("ApproveDate", System.Type.GetType("System.String"));
                dtnew.Columns.Add("Remark", System.Type.GetType("System.String"));
                dtnew.Columns.Add("ApproverID", System.Type.GetType("System.String"));
                dtnew.Columns.Add("Draft", System.Type.GetType("System.String"));
                dtnew.Columns.Add("RequestID", System.Type.GetType("System.String"));
                dtnew.Columns.Add("Status1", System.Type.GetType("System.String"));
                dtnew.Columns.Add("Type1", System.Type.GetType("System.String"));
                dtnew.Columns.Add("Draft1", System.Type.GetType("System.String"));
                dtnew.Columns.Add("ApplyDate", System.Type.GetType("System.String"));
                for (int i = 0; i < dtdetail.Rows.Count; i++)
                {
                    DataRow dr = dtnew.NewRow();
                    dr["FlowID"] = dtdetail.Rows[i]["FlowID"].ToString();
                    dr["No"] = dtdetail.Rows[i]["No"].ToString();
                    dr["Type"] = dtdetail.Rows[i]["Type"].ToString();
                    dr["Station"] = dtdetail.Rows[i]["Station"].ToString();
                    dr["Department"] = dtdetail.Rows[i]["Department"].ToString();
                    dr["Person"] = dtdetail.Rows[i]["Person"].ToString();
                    dr["CreadedBy"] = dtdetail.Rows[i]["CreadedBy"].ToString();
                    dr["CreadedDate"] = dtdetail.Rows[i]["CreadedDate"].ToString() == "" ? "" : Convert.ToDateTime(dtdetail.Rows[i]["CreadedDate"].ToString()).ToString("yyyy/MM/dd");
                    dr["Tamount"] = dtdetail.Rows[i]["Tamount"].ToString();
                    dr["Step"] = dtdetail.Rows[i]["Step"].ToString();
                    dr["Status"] = dtdetail.Rows[i]["Status"].ToString();
                    dr["Approver"] = dtdetail.Rows[i]["Approver"].ToString();
                    dr["ApproveDate"] = dtdetail.Rows[i]["ApproveDate"].ToString() == "" ? "" : Convert.ToDateTime(dtdetail.Rows[i]["ApproveDate"].ToString()).ToString("yyyy/MM/dd");
                    dr["Remark"] = dtdetail.Rows[i]["Remark"].ToString();
                    dr["ApproverID"] = dtdetail.Rows[i]["ApproverID"].ToString();
                    dr["Draft1"] = dtdetail.Rows[i]["Draft1"].ToString();
                    dr["Draft"] = dtdetail.Rows[i]["Draft"].ToString();
                    dr["RequestID"] = dtdetail.Rows[i]["RequestID"].ToString();
                    dr["Status1"] = dtdetail.Rows[i]["Status1"].ToString();
                    dr["Type1"] = dtdetail.Rows[i]["Type1"].ToString();
                    dr["ApplyDate"] = Convert.ToDateTime(dtdetail.Rows[i]["ApplyDate"].ToString()).ToString("yyyy/MM/dd");
                    dtnew.Rows.Add(dr);
                }
                Store1.DataSource = dtnew;
                Store1.DataBind();
                if (dtdetail.Rows.Count > 0)
                {
                    X.AddScript("btnExport.enable();");
                }
            }
            catch (Exception ex)
            {
                DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();

                mail.FromDispName = "eReimbursement";
                mail.From = "DIC2@dimerco.com";
                mail.To = "Andy_Kang@dimerco.com";
                mail.Title = "eReimbursement Bug" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:dd");
                mail.Body = ex.Message + "<br/>" + ex.InnerException.ToString() + "</div>";
                mail.Send();
            }
        }
        protected void btnLogin_Click(object sender, DirectEventArgs e)
        {
            try
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
            
            catch (Exception ex)
            {
                DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();

                mail.FromDispName = "eReimbursement";
                mail.From = "DIC2@dimerco.com";
                mail.To = "Andy_Kang@dimerco.com";
                mail.Title = "eReimbursement Bug" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:dd");
                mail.Body = ex.Message+"<br/>"+ex.InnerException.ToString() + "</div>";
                mail.Send();
            }
        }
        [DirectMethod]
        public void Logout()
        {
            Session["UserID"] = null;
            Session["UserName"] = null;
            Session["Station"] = null;
            Session["Department"] = null;
            Session["CostCenter"] = null;
            //Request.Cookies.Remove("eReimUserID");
            //Request.Cookies.Remove("eReimUserName");
            //Request.Cookies.Remove("eReimStation");
            //Request.Cookies.Remove("eReimDepartment");
            //Request.Cookies.Remove("eReimCostCenter");

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
        protected void GetDetail(object sender, DirectEventArgs e)
        {
            string RequestID = e.ExtraParams[0].Value;
            string Type = e.ExtraParams[1].Value;
            string sql = "select * from V_Eflow_ETravel where RequestID='" + RequestID + "' and [Type]='" + Type + "' order by Step,FlowID";
            cs.DBCommand dbc = new cs.DBCommand();
            DataTable dt = new DataTable();
            dt = dbc.GetData("eReimbursement", sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                string html = "";
                string status1 = ""; string status2 = ""; string status3 = ""; string status4 = ""; string status5 = ""; string status6 = "";
                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                {
                    status1 = "待提交";
                    status2 = "提交申请";
                    status3 = "待批";
                    status4 = "已批准";
                    status5 = "已拒绝";
                    status6 = "完成";
                }
                else
                {
                    status1 = "Pending.";
                    status2 = "Applied by";
                    status3 = "Waiting for app.";
                    status4 = "Approved.";
                    status5 = "Rejected by";
                    status6 = "Complete.";
                }


                bool tijiao = false;
                int countdiv = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["Step"].ToString() == "0")
                    {
                        html += "<div class=\"StatusIcon Pending\">";
                        html += "<span class=\"spanIcon\">" + status1 + "<br />";
                        html += dt.Rows[i]["Person"].ToString();
                        html += "</span><b class=\"bIcon bIcon1\"></b>";
                        html += "</div>";
                        countdiv++;
                        break;
                    }
                    else
                    {
                        if (dt.Rows[i]["Step"].ToString() == "1" && !tijiao)
                        {
                            html += "<div class=\"StatusIcon StatusIcon0\">";
                            html += "<span class=\"spanIcon\">" + status2 + "<br />";
                            html += dt.Rows[i]["CreadedBy"].ToString();
                            html += dt.Rows[i]["CreadedDate"].ToString() == "" ? "" : "<br />" + Convert.ToDateTime(dt.Rows[i]["CreadedDate"].ToString()).ToString("yyyy/MM/dd");
                            html += "</span><b class=\"bIcon bIcon1\"></b>";
                            html += "</div>";
                            countdiv++;
                            tijiao = true;
                        }
                        if (dt.Rows[i]["Status"].ToString() == "1")
                        {
                            string msg = "";
                            if (dt.Rows[i]["FlowFn"].ToString().ToLower() == "verifier")
                            {
                                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                {
                                    msg = "待查.";
                                }
                                else
                                {
                                    msg = "To be Verified by";
                                }
                            }
                            else if (dt.Rows[i]["FlowFn"].ToString().ToLower() == "issuer")
                            {
                                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                {
                                    msg = "待发放.";
                                }
                                else
                                {
                                    msg = "To be Issued by";
                                }
                            }
                            else
                            {
                                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                {
                                    msg = "待批.";
                                }
                                else
                                {
                                    msg = "Waiting for Approval by";
                                }
                            }

                            html += "<div class=\"StatusIcon Pending\">";
                            html += "<span class=\"spanIcon\">" + msg + "<br />";
                            html += dt.Rows[i]["Approver"].ToString();
                            html += "</span><b class=\"bIcon bIcon1\"></b>";
                            html += "</div>";
                            countdiv++;
                        }
                        else if (dt.Rows[i]["Status"].ToString() == "2")
                        {
                            string msg = "";
                            if (dt.Rows[i]["FlowFn"].ToString().ToLower() == "verifier")
                            {
                                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                {
                                    msg = "已查.";
                                }
                                else
                                {
                                    msg = "Verified by";
                                }
                            }
                            else if (dt.Rows[i]["FlowFn"].ToString().ToLower() == "issuer")
                            {
                                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                {
                                    msg = "已发放.";
                                }
                                else
                                {
                                    msg = "Issued by";
                                }
                            }
                            else
                            {
                                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                {
                                    msg = "已批.";
                                }
                                else
                                {
                                    msg = "Approved by";
                                }
                            }

                            html += "<div class=\"StatusIcon Approve\">";
                            html += "<span class=\"spanIcon\">" + msg + "<br />";
                            html += dt.Rows[i]["Approver"].ToString();
                            html += dt.Rows[i]["ApproveDate"].ToString() == "" ? "" : "<br />" + Convert.ToDateTime(dt.Rows[i]["ApproveDate"].ToString()).ToString("yyyy/MM/dd");
                            html += "</span><b class=\"bIcon bIcon1\"></b>";
                            html += "</div>";
                            countdiv++;
                        }
                        else if (dt.Rows[i]["Status"].ToString() == "3")
                        {
                            html += "<div class=\"StatusIcon Reject\">";
                            html += "<span class=\"spanIcon\">" + status5 + "<br />";
                            html += dt.Rows[i]["Approver"].ToString();
                            html += dt.Rows[i]["ApproveDate"].ToString() == "" ? "" : "<br />" + Convert.ToDateTime(dt.Rows[i]["ApproveDate"].ToString()).ToString("yyyy/MM/dd");
                            html += "</span><b class=\"bIcon bIcon1\"></b>";
                            html += "</div>";
                            countdiv++;
                        }
                        if (dt.Rows[i]["Active"].ToString() == "2")
                        {
                            html += "<div class=\"StatusIcon Complete\">";
                            html += "<span class=\"spanIcon\">" + status6;
                            html += "</span><b class=\"bIcon bIcon1\"></b>";
                            html += "</div>";
                            countdiv++;
                            break;
                        }
                    }
                }
                if (countdiv > 7)
                {
                    int width = countdiv * 110;
                    html = "'<div style=\"width:" + width.ToString() + "px\">" + html;
                }
                else
                {
                    html = "'<div>" + html;
                }
                html += "</div>'";
                X.AddScript("setFlow(" + html + ");Container2.show();");
            }
        }
        protected void changelang(object sender, EventArgs e)
        {
            HttpCookie cookie = new HttpCookie("lang");
            Ext.Net.Button btnEN = (Ext.Net.Button)sender;
            if (btnEN.Text == "中文")
            {
                cookie.Value = "zh-cn";
            }
            else
            {
                cookie.Value = "en-us";
            }

            Request.Cookies.Remove("lang");
            Request.Cookies.Add(cookie);
            Response.Cookies.Remove("lang");
            Response.Cookies.Add(cookie);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cookie.Value);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cookie.Value);
        }
        protected void btnExport_Click(object sender, EventArgs e)
        {
            XlsDocument xls = new XlsDocument();//新建一个xls文档
            xls.FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

            Worksheet sheet;
            sheet = xls.Workbook.Worksheets.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));

            XF titleXF = xls.NewXF(); // 为xls生成一个XF实例，XF是单元格格式对象
            titleXF.HorizontalAlignment = HorizontalAlignments.Left; // 设定文字居中
            titleXF.VerticalAlignment = VerticalAlignments.Centered; // 垂直居中
            titleXF.UseBorder = false; // 使用边框
            titleXF.Font.Height = 12 * 20; // 字大小（字体大小是以 1/20 point 为单位的）

            XF titleXF1 = xls.NewXF(); // 为xls生成一个XF实例，XF是单元格格式对象
            titleXF1.HorizontalAlignment = HorizontalAlignments.Left; // 设定文字居中
            titleXF1.VerticalAlignment = VerticalAlignments.Centered; // 垂直居中
            titleXF1.UseBorder = false; // 使用边框
            titleXF1.Font.Bold = true;
            titleXF1.Font.Height = 12 * 20; // 字大小（字体大小是以 1/20 point 为单位的）
            // 开始填充数据到单元格
            org.in2bits.MyXls.Cells cells = sheet.Cells;
            cells.Add(1, 1, "NO#", titleXF1);
            cells.Add(1, 2, "Status", titleXF1);
            cells.Add(1, 3, "Claim Type", titleXF1);
            cells.Add(1, 4, "Amount", titleXF1);
            cells.Add(1, 5, "Process", titleXF1);
            cells.Add(1, 6, "Current Approver", titleXF1);
            cells.Add(1, 7, "Owner", titleXF1);
            cells.Add(1, 8, "Submitted by", titleXF1);
            cells.Add(1, 9, "Submit date", titleXF1);
            cells.Add(1, 10, "Remark", titleXF1);

            //添加数据
            string json = GridData.Value.ToString();
            StoreSubmitDataEventArgs eSubmit = new StoreSubmitDataEventArgs(json, null);
            XmlNode xml = eSubmit.Xml;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.InnerXml);
            for (int i = 0; i < doc.SelectNodes("records").Item(0).SelectNodes("record").Count; i++)
            {
                if (!string.IsNullOrEmpty(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Tamount").Item(0).InnerXml))
                {
                    cells.Add(2 + i, 4, Convert.ToDouble(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Tamount").Item(0).InnerXml), titleXF);
                }
                else
                {
                    cells.Add(2 + i, 4, "", titleXF);
                }
                cells.Add(2 + i, 1, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("No").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 2, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Draft1").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 3, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Type1").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 5, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Status1").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 6, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Approver").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 7, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Person").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 8, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("CreadedBy").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 9, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("CreadedDate").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 10, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Remark").Item(0).InnerXml, titleXF);
            }

            xls.Send();
        }
        [DirectMethod]
        public void Delete(string RequestID, string Type, string rowIndex)
        {
            string sqldel = "";
            if (Type == "T")
            {
                sqldel += "select * from Eflow where RequestID='" + RequestID + "' and [Type]='T';";
                sqldel += "select * from ETravel where ID='" + RequestID + "';";
                sqldel += "select * from ETraveleDetail where [No]='" + RequestID + "';";
                //sqldel += "delete from Eflow where RequestID='" + RequestID + "' and [Type]='T';";
                //sqldel += "delete from ETravel where ID='" + RequestID + "';";
                //sqldel += "delete from ETraveleDetail where [No]='" + RequestID + "';";
                sqldel += "update ETravel set Void=1 where ID='" + RequestID + "';";
            }
            else if (Type == "G")
            {
                sqldel += "select * from Eflow where RequestID='" + RequestID + "' and [Type]='G';";
                sqldel += "select * from Ecommon where ID='" + RequestID + "';";
                sqldel += "select * from EeommonDetail where [No]='" + RequestID + "';";
                //sqldel += "delete from Eflow where RequestID='" + RequestID + "' and [Type]='G';";
                //sqldel += "delete from Ecommon where ID='" + RequestID + "';";
                //sqldel += "delete from EeommonDetail where [No]='" + RequestID + "';";
                sqldel += "update Ecommon set Void=1 where ID='" + RequestID + "';";
            }
            cs.DBCommand dbc = new cs.DBCommand();
            string sqlre = dbc.UpdateData("eReimbursement", sqldel, "Update");
            //X.AddScript("Store1.removeAt(" + rowIndex + ");Store1.load();");
        }
    }
}