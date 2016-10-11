using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using System.IO;
using System.Data;
using System.Xml;
using System.Text;
using DIMERCO.SDK;
using org.in2bits.MyXls;
using System.Net.Mail;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using System.Configuration;
using System.Data.SqlClient;
namespace eReimbursement
{
    public partial class Approve : App_Code.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                //if (Request.Cookies["eReimUserID"] != null)
                //{
                //    Response.Cookies["eReimUserID"].Value = "A0300";  //将值写入到客户端硬盘Cookie
                //    Response.Cookies["eReimUserID"].Expires = DateTime.Now.AddHours(12);//设置Cookie过期时间
                //}
                //else
                //{
                //    HttpCookie cookie = new HttpCookie("eReimUserID", "A0300");
                //    cookie.Expires = DateTime.Now.AddHours(12);
                //    Response.Cookies.Add(cookie);
                //}
                //if (Request.Cookies["eReimUserName"] != null)
                //{
                //    Response.Cookies["eReimUserName"].Value = "Kevin Zhang";  //将值写入到客户端硬盘Cookie
                //    Response.Cookies["eReimUserName"].Expires = DateTime.Now.AddHours(12);//设置Cookie过期时间
                //}
                //else
                //{
                //    HttpCookie cookie = new HttpCookie("eReimUserName", "Kevin Zhang");
                //    cookie.Expires = DateTime.Now.AddHours(12);
                //    Response.Cookies.Add(cookie);
                //}
                //判断登录状态
                cs.DBCommand dbc = new cs.DBCommand();
                if (Request.Cookies.Get("eReimUserID") == null)
                {
                    X.AddScript("loginWindow.show();Panel1.disable();");
                    return;
                }
                else
                {
                    //Session["UserID"] = "A1231";
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "$('div.gn_person ul.q-menubox li:eq(0) a').text('" + Request.Cookies.Get("eReimUserName").Value + "');", true); X.AddScript("loginWindow.hide();Panel1.enable();");
                }
                string sqltype = "";
                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                {
                    PagingToolbar1.DisplayMsg = "显示 {0} - {1} of {2}";
                    ResourceManager1.Locale = "zh-CN";
                    sqltype += "select [Text]=CText,[Value]=CValue from Edic where KeyValue='MainType'";
                }
                else
                {
                    PagingToolbar1.DisplayMsg = "Displaying items {0} - {1} of {2}";
                    ResourceManager1.Locale = "en-US";
                    sqltype += "select [Text]=EText,[Value]=CValue from Edic where KeyValue='MainType'";
                }
                DataTable dttype = dbc.GetData("eReimbursement", sqltype);
                StoreType.DataSource = dttype;
                StoreType.DataBind();

                string sql = "";//报销单状态
                if (Request.QueryString["Status"] != null)
                {
                    string Status = Request.QueryString["Status"].ToString();
                    System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^\d*$");
                    if (reg1.IsMatch(Status))
                    {
                        if (Request.QueryString["Status"].ToString() == "1")
                        {
                            sql += "and Active=1 ";
                        }
                        else
                        {
                            sql += "and Status=" + Request.QueryString["Status"].ToString() + " ";
                        }
                    }
                    else
                    {
                        ErrorHandle();
                    }
                }
                else
                {
                    sql += "and Active=1 ";
                }
                //准备申请人下拉菜单内容
                string sqlitem = "select distinct Person,PersonID from V_Eflow_ETravel where Step!=0 and [ApproverID]='" + Request.Cookies.Get("eReimUserID").Value + "' " + sql;
                DataTable dtitem = dbc.GetData("eReimbursement", sqlitem);
                for (int j = 0; j < dtitem.Rows.Count; j++)
                {
                    Ext.Net.ListItem li = new Ext.Net.ListItem(dtitem.Rows[j]["Person"].ToString(), dtitem.Rows[j]["PersonID"].ToString());
                    cbxPerson.Items.Add(li);
                }
                //准备提单人下拉菜单内容
                string sqlitem1 = "select distinct CreadedBy,CreadedByID from V_Eflow_ETravel where Step!=0 and [ApproverID]='" + Request.Cookies.Get("eReimUserID").Value + "' " + sql;
                DataTable dtitem1 = dbc.GetData("eReimbursement", sqlitem1);
                for (int j = 0; j < dtitem1.Rows.Count; j++)
                {
                    Ext.Net.ListItem li = new Ext.Net.ListItem(dtitem1.Rows[j]["CreadedBy"].ToString(), dtitem1.Rows[j]["CreadedByID"].ToString());
                    cbxCreatedBy.Items.Add(li);
                }
                //准备站点下拉菜单内容
                string sqlitem2 = "select distinct Station from V_Eflow_ETravel where Step!=0 and [ApproverID]='" + Request.Cookies.Get("eReimUserID").Value + "' " + sql;
                DataTable dtitem2 = dbc.GetData("eReimbursement", sqlitem2);
                for (int j = 0; j < dtitem2.Rows.Count; j++)
                {
                    Ext.Net.ListItem li = new Ext.Net.ListItem(dtitem2.Rows[j]["Station"].ToString(), dtitem2.Rows[j]["Station"].ToString());
                    cbxTstation.Items.Add(li);
                }
                BindData(sql);
                //从邮件链接进入打开详细信息
                if (Request.QueryString["FlowID"] != null)
                {
                    string Status = Request.QueryString["FlowID"].ToString();
                    System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^\d*$");
                    if (reg1.IsMatch(Status))
                    {
                        string lingsql = "select * from V_Eflow_ETravel where FlowID=" + Status;
                        DataTable dtlink = new DataTable();
                        dtlink = dbc.GetData("eReimbursement", lingsql);
                        if (dtlink != null && dtlink.Rows.Count == 1)
                        {
                            string budget = dtlink.Rows[0]["Budget"].ToString() == "1" ? "(Budget)" : "(UnBudget)";
                            if (Request.Cookies.Get("eReimUserID").Value!=dtlink.Rows[0]["ApproverID"].ToString())
                            {
                                //ErrorHandle("No right."); return;
                                //151029,Shanshan要求修改,历史审批人可以继续查看.
                                string RequestID = dtlink.Rows[0]["RequestID"].ToString();
                                string Type = dtlink.Rows[0]["Type"].ToString();
                                string mainsql = "select * from V_Eflow_ETravel where RequestID=" + RequestID + " and Type='" + Type + "'";
                                DataTable dtmain = new DataTable();
                                dtmain = dbc.GetData("eReimbursement", mainsql);

                                if (dtmain.Select("ApproverID='"+Request.Cookies.Get("eReimUserID").Value+"'").Count()>0)
                                {
                                    
                                    if (dtlink.Rows[0]["Type"].ToString() == "T")
                                    {
                                        string url = "ApplyTravelT.aspx?RequestID=" + dtlink.Rows[0]["RequestID"].ToString() + "&ID=" + Status + "&Step=" + dtlink.Rows[0]["Step"].ToString() + "&Type=" + dtlink.Rows[0]["Type"].ToString() + "&Status=" + dtlink.Rows[0]["Status"].ToString();
                                        X.AddScript("Window1.setTitle('差旅费申请单:" + dtlink.Rows[0]["No"].ToString() + budget + "');Window1.loadContent({ url: '" + url + "', mode: 'iframe' });Window1.show();");
                                        //Window1.LoadContent(url);
                                        //Window1.Show();
                                    }
                                    else
                                    {
                                        string url = "ApproveG.aspx?RequestID=" + dtlink.Rows[0]["RequestID"].ToString() + "&ID=" + Status + "&Step=" + dtlink.Rows[0]["Step"].ToString() + "&Type=" + dtlink.Rows[0]["Type"].ToString() + "&Status=" + dtlink.Rows[0]["Status"].ToString();
                                        X.AddScript("Window1.setTitle('通用费用申请单:" + dtlink.Rows[0]["No"].ToString() + budget + "');Window1.loadContent({ url: '" + url + "', mode: 'iframe' });Window1.show();");
                                    }

                                    //DataRow dr = dtmain.Select("ApproverID='" + Request.Cookies.Get("eReimUserID").Value + "'")[0];
                                    //string st = dr["Status"] == "2" ? "approved" : "rejected";
                                    //string msg1 = "This applied item " + dtlink.Rows[0]["No"].ToString() + " has been " + st + " by approver " + Request.Cookies.Get("eReimUserID").Value + " on " + Convert.ToDateTime(dr["ApproveDate"]).ToString("yyyy/MM/dd");

                                    //if (dtlink.Rows[0]["Type"].ToString()=="T")
                                    //{
                                    //    string url = "ApplyTravelT.aspx?RequestID=" + RequestID + "&ID=" + Status + "&Step=" + dtlink.Rows[0]["Step"].ToString() + "&Type=" + dtlink.Rows[0]["Type"].ToString() + "&Status=" + dtlink.Rows[0]["Status"].ToString();
                                    //    X.AddScript("Ext.Msg.show({ title: '提示', msg: '" + msg1 + "', buttons: { ok: 'Ok' }, fn: function (btn) { Window1.setTitle('差旅费申请单:" + dtlink.Rows[0]["No"].ToString() + "');Window1.loadContent({ url: '" + url + "', mode: 'iframe' });Window1.show(); } });");

                                    //    //X.AddScript("Window1.setTitle('差旅费申请单:" + dtlink.Rows[0]["No"].ToString() + "');Window1.loadContent({ url: '" + url + "', mode: 'iframe' });Window1.show();");

                                    //}
                                    //else
                                    //{
                                    //    string url = "ApproveG.aspx?RequestID=" + RequestID + "&ID=" + Status + "&Step=" + dtlink.Rows[0]["Step"].ToString() + "&Type=" + dtlink.Rows[0]["Type"].ToString() + "&Status=" + dtlink.Rows[0]["Status"].ToString();
                                    //    X.AddScript("Ext.Msg.show({ title: '提示', msg: '" + msg1 + "', buttons: { ok: 'Ok' }, fn: function (btn) { Window1.setTitle('通用费用申请单:" + dtlink.Rows[0]["No"].ToString() + "');Window1.loadContent({ url: '" + url + "', mode: 'iframe' });Window1.show(); } });");
                                    //    //X.AddScript("Window1.setTitle('通用费用申请单:" + dtlink.Rows[0]["No"].ToString() + "');Window1.loadContent({ url: '" + url + "', mode: 'iframe' });Window1.show();");
                                    //}
                                }
                                else
                                {
                                    ErrorHandle("No right.");
                                    return;
                                }
                            }
                            else
                            {
                                if (dtlink.Rows[0]["Type"].ToString() == "T")
                                {
                                    string url = "ApplyTravelT.aspx?RequestID=" + dtlink.Rows[0]["RequestID"].ToString() + "&ID=" + Status + "&Step=" + dtlink.Rows[0]["Step"].ToString() + "&Type=" + dtlink.Rows[0]["Type"].ToString() + "&Status=" + dtlink.Rows[0]["Status"].ToString();
                                    X.AddScript("Window1.setTitle('差旅费申请单:" + dtlink.Rows[0]["No"].ToString() + budget + "');Window1.loadContent({ url: '" + url + "', mode: 'iframe' });Window1.show();");
                                    //Window1.LoadContent(url);
                                    //Window1.Show();
                                }
                                else
                                {
                                    string url = "ApproveG.aspx?RequestID=" + dtlink.Rows[0]["RequestID"].ToString() + "&ID=" + Status + "&Step=" + dtlink.Rows[0]["Step"].ToString() + "&Type=" + dtlink.Rows[0]["Type"].ToString() + "&Status=" + dtlink.Rows[0]["Status"].ToString();
                                    X.AddScript("Window1.setTitle('通用费用申请单:" + dtlink.Rows[0]["No"].ToString() + budget + "');Window1.loadContent({ url: '" + url + "', mode: 'iframe' });Window1.show();");
                                }
                            }
                            
                        }
                    }
                }
            }
        }
        protected void ErrorHandle(string msg)
        {
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Message",
                Message = msg,
                Buttons = MessageBox.Button.OK,
                Width = 250,
                Icon = MessageBox.Icon.WARNING,
                Fn = new JFunction { Fn = "ShowFunction" }
            });
        }
        protected void button1_Search(object sender, DirectEventArgs e)
        {
            //string sql = "select * from V_Eflow_ETravel where ApproverID='A0360' ";//Session
            string sql = "";
            cs.DBCommand dbc = new cs.DBCommand();
            if (Request.QueryString["Status"] != null)
            {
                string Status = Request.QueryString["Status"].ToString();
                System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^\d*$");
                if (reg1.IsMatch(Status))
                {
                    if (Request.QueryString["Status"].ToString() == "1")
                    {
                        sql += "and Active=1 ";
                    }
                    else
                    {
                        sql += "and Status=" + Request.QueryString["Status"].ToString() + " ";
                    }
                }
                else
                {
                    ErrorHandle();
                }
            }
            else
            {
                sql += "and Active=1 ";
            }
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
            if (!string.IsNullOrEmpty(cbxTstation.Text))
            {
                sql += "and [Station]='" + cbxTstation.Text + "' ";
            }
            if (!string.IsNullOrEmpty(cbxPerson.Text))
            {
                sql += "and [PersonID]='" + cbxPerson.Text + "' ";
            }
            if (!string.IsNullOrEmpty(cbxCreatedBy.Text))
            {
                sql += "and [CreadedByID]='" + cbxCreatedBy.Text + "' ";
            }
            BindData(sql);
        }
        protected void BindData(string sql)
        {
            string sql1 = "select t1.*";
            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
            {
                PagingToolbar1.DisplayMsg = "显示 {0} - {1} of {2}";
                ResourceManager1.Locale = "zh-CN";
                sql1 += ",[Status1]=TDicStatus.CText,[Type1]=TDicMainType.CText,[Draft1]=TDicType.CText";
            }
            else
            {
                PagingToolbar1.DisplayMsg = "Displaying items {0} - {1} of {2}";
                ResourceManager1.Locale = "en-US";
                sql1 += ",[Status1]=TDicStatus.EText,[Type1]=TDicMainType.EText,[Draft1]=TDicType.EText";
            }

            sql1 += " from (select [Draft]=case when [Status]=0 then 1 else 0 end,* from V_Eflow_ETravel where FlowID>16097 and [Step]!=0 and ApproverID='" + Request.Cookies.Get("eReimUserID").Value + "' " + sql + ") t1";//Session
            sql1 += " left join (select * from Edic where KeyValue='MainType') TDicMainType on TDicMainType.CValue=t1.Type";
            sql1 += " left join (select * from Edic where KeyValue='Status') TDicStatus on TDicStatus.CValue=t1.Status";
            sql1 += " left join (select * from Edic where KeyValue='Type') TDicType on TDicType.CValue=t1.Draft";

            cs.DBCommand dbc = new cs.DBCommand();
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

            dtnew.Columns.Add("PersonID", System.Type.GetType("System.String"));
            dtnew.Columns.Add("CreadedByID", System.Type.GetType("System.String"));
            dtnew.Columns.Add("RemarkFlow", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Active", System.Type.GetType("System.String"));

            dtnew.Columns.Add("Status1", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Type1", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Draft1", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Cur", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Budget", System.Type.GetType("System.String"));
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
                dr["RequestID"] = dtdetail.Rows[i]["RequestID"].ToString();
                dr["PersonID"] = dtdetail.Rows[i]["PersonID"].ToString();
                dr["CreadedByID"] = dtdetail.Rows[i]["CreadedByID"].ToString();
                dr["RemarkFlow"] = dtdetail.Rows[i]["RemarkFlow"].ToString();
                dr["Active"] = dtdetail.Rows[i]["Active"].ToString();
                dr["Status1"] = dtdetail.Rows[i]["Status1"].ToString();
                dr["Type1"] = dtdetail.Rows[i]["Type1"].ToString();
                dr["Draft1"] = dtdetail.Rows[i]["Draft1"].ToString();
                dr["Cur"] = dtdetail.Rows[i]["Cur"].ToString();
                dr["Budget"] = dtdetail.Rows[i]["Budget"].ToString();
                dtnew.Rows.Add(dr);
            }
            Store1.DataSource = dtnew;
            Store1.DataBind();
        }
        protected void Command(object sender, DirectEventArgs e)
        {
            cs.DBCommand dbc = new cs.DBCommand();
            string para = e.ExtraParams[0].Value;
            string paraType = e.ExtraParams[1].Value;
            string sql = "select * from Eflow where RequestID='" + para + "' and [Type]='" + paraType + "' order by Step,id";
            DataTable dt = new DataTable();
            dt = dbc.GetData("eReimbursement", sql);
            SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eReimbursement"].ConnectionString);
            string ID = para;
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["Active"].ToString() == "1")
                    {
                        if (i == dt.Rows.Count-1)
                        {
                            string updatesql = "update Eflow set Active=2,Status=2,ApproveDate='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' where id=" + dt.Rows[i]["id"].ToString();
                            if (dt.Rows[i]["Type"].ToString()=="T")
                            {
                                updatesql += ";update ETravel set Status=2 where ID=" + dt.Rows[i]["RequestID"].ToString();
                                string newid = dbc.UpdateData("eReimbursement", updatesql, "Update");
                                if (newid == "-1")
                                {
                                    ErrorHandle("Data Error.");
                                    return;
                                }
                                else
                                {
                                    
                                    string sqlf = "select * from ETravel where ID=" + ID;
                                    DataTable dtf = dbc.GetData("eReimbursement", sqlf);

                                    try
                                    {
                                        string sqld = "select * from ETraveleDetail where [No]='" + ID + "' order by id";
                                        DataTable dtall = new DataTable();
                                        dtall = dbc.GetData("eReimbursement", sqld);
                                        //预算
                                        //140226 显示预算
                                        DataTable dtbudget = new DataTable();
                                        dtbudget.Columns.Add("RequestID", typeof(System.Int32));
                                        dtbudget.Columns.Add("Status", typeof(System.Int16));
                                        dtbudget.Columns.Add("EName", typeof(System.String));
                                        dtbudget.Columns.Add("COACode", typeof(System.String));
                                        dtbudget.Columns.Add("LocalCur", typeof(System.String));
                                        dtbudget.Columns.Add("CenterCur", typeof(System.String));
                                        dtbudget.Columns.Add("Rate", typeof(System.Decimal));
                                        dtbudget.Columns.Add("Current", typeof(System.Decimal));
                                        dtbudget.Columns.Add("PA", typeof(System.Decimal));
                                        dtbudget.Columns.Add("CA", typeof(System.Decimal));
                                        dtbudget.Columns.Add("PU", typeof(System.Decimal));
                                        dtbudget.Columns.Add("PB", typeof(System.Decimal));
                                        dtbudget.Columns.Add("PPercent", typeof(System.Decimal));
                                        dtbudget.Columns.Add("DU", typeof(System.Decimal));
                                        dtbudget.Columns.Add("DB", typeof(System.Decimal));
                                        dtbudget.Columns.Add("DPercent", typeof(System.Decimal));
                                        dtbudget.Columns.Add("SU", typeof(System.Decimal));
                                        dtbudget.Columns.Add("SB", typeof(System.Decimal));
                                        dtbudget.Columns.Add("SPercent", typeof(System.Decimal));

                                        //取得预算日期
                                        string sqlA = "select convert(varchar(10),min(Tdate0),111) as BudgetDate from ETraveleDetail where No='" + ID + "'";
                                        DataTable dtA = dbc.GetData("eReimbursement", sqlA);
                                        //取得本币与成本中心汇率转换
                                        decimal rate = 1;
                                        string CurLocal = dtall.Rows[0]["Cur"].ToString();
                                        string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dtf.Rows[0]["Station2"].ToString());
                                        if (CurLocal != CurBudget)
                                        {
                                            rate = DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(CurLocal, CurBudget, Convert.ToDateTime(dtA.Rows[0]["BudgetDate"].ToString()).Year);
                                        }

                                        //取得4大类合计
                                        //string sqlB = "select sum(T1) as T1,sum(T2) as T2,sum(T3) as T3,sum(T4) as T4 from (select case when AccountCode='62012000' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T1],case when AccountCode='62010900' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T2],case when AccountCode='62011900' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T3],case when AccountCode='62010500' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T4] from ETraveleDetail where No=" + ID + ") t";
                                        string sqlB = "select sum(isnull(Pamount,0)) as PA,sum(isnull(Camount,0)) as CA,sum(isnull(Pamount,0)+isnull(Camount,0)) as Amount,'62012000' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62012000' union all select sum(isnull(Pamount,0)) as PA,sum(isnull(Camount,0)) as CA,sum(isnull(Pamount,0)+isnull(Camount,0)) as Amount,'62010900' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62010900' union all select sum(isnull(Pamount,0)) as PA,sum(isnull(Camount,0)) as CA,sum(isnull(Pamount,0)+isnull(Camount,0)) as Amount,'62011900' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62011900' union all select sum(isnull(Pamount,0)) as PA,sum(isnull(Camount,0)) as CA,sum(isnull(Pamount,0)+isnull(Camount,0)) as Amount,'62010500' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62010500'";
                                        DataTable dtB = dbc.GetData("eReimbursement", sqlB);
                                        //取得传递预算的参数
                                        string userid = dtf.Rows[0]["PersonID"].ToString();
                                        string dpt = dtf.Rows[0]["Department"].ToString();
                                        string ostation = dtf.Rows[0]["Station2"].ToString();//预算站点,与基本信息中的CostCenter一致(Station2)
                                        string tstation = ostation;//Etravel表中的Station2,目前与预算站点一致,不允许更改
                                        string year = Convert.ToDateTime(dtA.Rows[0]["BudgetDate"].ToString()).Year.ToString();
                                        string month = Convert.ToDateTime(dtA.Rows[0]["BudgetDate"].ToString()).Month.ToString();
                                        string accountcode = "";
                                        for (int g = 0; g < dtB.Rows.Count; g++)
                                        {
                                            if (Convert.ToDecimal(dtB.Rows[g]["Amount"].ToString()) != 0)
                                            {
                                                DataRow dr = dtbudget.NewRow();
                                                dr["Current"] = Convert.ToDecimal(dtB.Rows[g]["Amount"].ToString());
                                                dr["PA"] = Convert.ToDecimal(dtB.Rows[g]["PA"].ToString());
                                                dr["CA"] = Convert.ToDecimal(dtB.Rows[g]["CA"].ToString());
                                                dr["COACode"] = dtB.Rows[g]["COACode"].ToString();
                                                accountcode = dtB.Rows[g]["COACode"].ToString();
                                                DataTable dtC = new DataTable();
                                                dtC = Comm.ExRtnEB(userid, dpt, ostation, tstation, accountcode, year, month, "T", ID);

                                                for (int h = 0; h < dtC.Rows.Count; h++)
                                                {
                                                    if (dtC.Rows[h]["Type"].ToString() == "全年个人")
                                                    {
                                                        dr["PU"] = Convert.ToDecimal(dtC.Rows[h]["Used"].ToString());
                                                        dr["PB"] = Convert.ToDecimal(dtC.Rows[h]["Budget"].ToString());
                                                    }
                                                    else if (dtC.Rows[h]["Type"].ToString() == "全年部门")
                                                    {
                                                        dr["DU"] = Convert.ToDecimal(dtC.Rows[h]["Used"].ToString());
                                                        dr["DB"] = Convert.ToDecimal(dtC.Rows[h]["Budget"].ToString());
                                                    }
                                                    else if (dtC.Rows[h]["Type"].ToString() == "全年站点")
                                                    {
                                                        dr["SU"] = Convert.ToDecimal(dtC.Rows[h]["Used"].ToString());
                                                        dr["SB"] = Convert.ToDecimal(dtC.Rows[h]["Budget"].ToString());
                                                    }
                                                }
                                                dtbudget.Rows.Add(dr);
                                            }
                                        }
                                        //计算%,取得名称,预算转换为本地汇率
                                        for (int g = 0; g < dtbudget.Rows.Count; g++)
                                        {
                                            if (Convert.ToDecimal(dtbudget.Rows[g]["PB"].ToString()) != 0)//如果Budget不为0,则计算%
                                            {
                                                dtbudget.Rows[g]["PPercent"] = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[g]["Current"].ToString()) / rate + Convert.ToDecimal(dtbudget.Rows[g]["PU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[g]["PB"].ToString()), 2);

                                            }
                                            if (Convert.ToDecimal(dtbudget.Rows[g]["DB"].ToString()) != 0)//如果Budget不为0,则计算%
                                            {
                                                dtbudget.Rows[g]["DPercent"] = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[g]["Current"].ToString()) / rate + Convert.ToDecimal(dtbudget.Rows[g]["DU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[g]["DB"].ToString()), 2);

                                            }
                                            if (Convert.ToDecimal(dtbudget.Rows[g]["SB"].ToString()) != 0)//如果Budget不为0,则计算%
                                            {
                                                dtbudget.Rows[g]["SPercent"] = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[g]["Current"].ToString()) / rate + Convert.ToDecimal(dtbudget.Rows[g]["SU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[g]["SB"].ToString()), 2);

                                            }
                                            if (dtbudget.Rows[g]["COACode"].ToString() == "62012000")
                                            {
                                                dtbudget.Rows[g]["EName"] = "Travel expense";
                                            }
                                            else if (dtbudget.Rows[g]["COACode"].ToString() == "62010900")
                                            {
                                                dtbudget.Rows[g]["EName"] = "Entertainment";
                                            }
                                            else if (dtbudget.Rows[g]["COACode"].ToString() == "62011900")
                                            {
                                                dtbudget.Rows[g]["EName"] = "Transportation";
                                            }
                                            else if (dtbudget.Rows[g]["COACode"].ToString() == "62010500")
                                            {
                                                dtbudget.Rows[g]["EName"] = "Communication";
                                            }
                                            dtbudget.Rows[g]["PU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["PU"].ToString()), 2);
                                            dtbudget.Rows[g]["PB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["PB"].ToString()), 2);
                                            dtbudget.Rows[g]["DU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["DU"].ToString()), 2);
                                            dtbudget.Rows[g]["DB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["DB"].ToString()), 2);
                                            dtbudget.Rows[g]["SU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["SU"].ToString()), 2);
                                            dtbudget.Rows[g]["SB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["SB"].ToString()), 2);
                                            dtbudget.Rows[g]["RequestID"] = ID;
                                            dtbudget.Rows[g]["Status"] = 2;
                                            dtbudget.Rows[g]["LocalCur"] = CurLocal;
                                            dtbudget.Rows[g]["CenterCur"] = CurBudget;
                                            dtbudget.Rows[g]["Rate"] = rate;
                                        }
                                        for (int g = 0; g < dtbudget.Rows.Count; g++)
                                        {
                                            SqlCommand scdetail = sqlConn.CreateCommand();
                                            scdetail.CommandText = "Insert into Budget_Complete (FormType,RequestID,Status,COACode,EName,LocalCur,CenterCur,Rate,LocalAmount,PA,CA,PU,PB,PPercent,DU,DB,DPercent,SU,SB,SPercent) values (@FormType,@RequestID,@Status,@COACode,@EName,@LocalCur,@CenterCur,@Rate,@LocalAmount,@PA,@CA,@PU,@PB,@PPercent,@DU,@DB,@DPercent,@SU,@SB,@SPercent)";
                                            SqlParameter spdetail = new SqlParameter("@RequestID", SqlDbType.Int);
                                            spdetail.Value = Convert.ToInt32(ID);
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@FormType", SqlDbType.VarChar, 10);
                                            spdetail.Value = "T";
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@Status", SqlDbType.Int);
                                            spdetail.Value = Convert.ToInt16(dtbudget.Rows[g]["Status"].ToString());
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@COACode", SqlDbType.VarChar, 50);
                                            spdetail.Value = dtbudget.Rows[g]["COACode"].ToString();
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@EName", SqlDbType.VarChar, 50);
                                            spdetail.Value = dtbudget.Rows[g]["EName"].ToString();
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@LocalCur", SqlDbType.VarChar, 10);
                                            spdetail.Value = dtbudget.Rows[g]["LocalCur"].ToString();
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@CenterCur", SqlDbType.VarChar, 10);
                                            spdetail.Value = dtbudget.Rows[g]["CenterCur"].ToString();
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@Rate", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["Rate"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@LocalAmount", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["Current"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@PA", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["PA"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@CA", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["CA"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@PU", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["PU"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@PB", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["PB"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@PPercent", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["PPercent"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@DU", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["DU"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@DB", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["DB"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@DPercent", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["DPercent"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@SU", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["SU"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@SB", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["SB"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@SPercent", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["SPercent"];
                                            scdetail.Parameters.Add(spdetail);

                                            sqlConn.Open();
                                            int row = scdetail.ExecuteNonQuery();
                                            sqlConn.Close();
                                        }
                                        if (!SendMail(paraType, para, dt.Rows[i]["id"].ToString(), dtbudget))//Budget已经计入Current,%不需要重新计算
                                        {
                                            ErrorHandle("Error mail address."); return;
                                        }
                                        if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                        {
                                            X.AddScript("Ext.Msg.show({ title: '提示', msg: '批准成功,该申请单已经完成.', buttons: { ok: 'Ok' }, fn: function (btn) { parent.Window1.hide();parent.Button1.fireEvent('click'); } });");
                                        }
                                        else
                                        {
                                            X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Approved,form completed.', buttons: { ok: 'Ok' }, fn: function (btn) { parent.Window1.hide();parent.Button1.fireEvent('click'); } });");
                                        }
                                    }
                                    catch (Exception)
                                    {

                                        throw;
                                    }
                                }
                            }
                            else//通用费用
                            {
                                updatesql += ";update Ecommon set Status=2 where ID=" + dt.Rows[i]["RequestID"].ToString();
                                string newid = dbc.UpdateData("eReimbursement", updatesql, "Update");
                                if (newid == "-1")
                                {
                                    ErrorHandle("Data Error.");
                                    return;
                                }
                                else
                                {
                                    //保存预算信息
                                    try
                                    {
                                        string sqlf = "select * from Ecommon where ID=" + ID;
                                        DataTable dtf = dbc.GetData("eReimbursement", sqlf);
                                        //140226 显示预算
                                        DataTable dtbudget = new DataTable();
                                        dtbudget.Columns.Add("RequestID", typeof(System.Int32));
                                        dtbudget.Columns.Add("Year", typeof(System.String));
                                        dtbudget.Columns.Add("Status", typeof(System.Int16));
                                        dtbudget.Columns.Add("EName", typeof(System.String));
                                        dtbudget.Columns.Add("COACode", typeof(System.String));
                                        dtbudget.Columns.Add("LocalCur", typeof(System.String));
                                        dtbudget.Columns.Add("CenterCur", typeof(System.String));
                                        dtbudget.Columns.Add("Rate", typeof(System.Decimal));
                                        dtbudget.Columns.Add("Current", typeof(System.Decimal));
                                        dtbudget.Columns.Add("PU", typeof(System.Decimal));
                                        dtbudget.Columns.Add("PB", typeof(System.Decimal));
                                        dtbudget.Columns.Add("PPercent", typeof(System.Decimal));
                                        dtbudget.Columns.Add("DU", typeof(System.Decimal));
                                        dtbudget.Columns.Add("DB", typeof(System.Decimal));
                                        dtbudget.Columns.Add("DPercent", typeof(System.Decimal));
                                        dtbudget.Columns.Add("SU", typeof(System.Decimal));
                                        dtbudget.Columns.Add("SB", typeof(System.Decimal));
                                        dtbudget.Columns.Add("SPercent", typeof(System.Decimal));

                                        string sqld = "select * from EeommonDetail where [No]='" + ID + "' order by id";
                                        DataTable dtall = new DataTable();
                                        dtall = dbc.GetData("eReimbursement", sqld);
                                        //取得预算日期
                                        string sqlA = "select AccountCode as COACode,case when t1.Type='O' then t2.SAccountName else TDicSubType.EText end as [EName],year(Tdate) as [Year],Amount from EeommonDetail t1 left join (select * from Edic where KeyValue='SubType') TDicSubType on TDicSubType.CValue=t1.Type left join AccoundCode t2 on t2.SAccountCode=t1.AccountCode where t1.No='" + ID + "'";
                                        DataTable dtA = dbc.GetData("eReimbursement", sqlA);
                                        //取得本币与成本中心汇率转换
                                        decimal rate = 1;
                                        string CurLocal = dtall.Rows[0]["Cur"].ToString();
                                        string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dtf.Rows[0]["Station2"].ToString());
                                        //if (CurLocal != CurBudget)
                                        //{
                                        //    rate = DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(CurLocal, CurBudget, Convert.ToInt16(dtA.Rows[0]["Year"].ToString()));
                                        //}

                                        //合计数据
                                        DataTable dtB = new DataTable();
                                        dtB.Columns.Add("EName", typeof(System.String));
                                        dtB.Columns.Add("COACode", typeof(System.String));
                                        dtB.Columns.Add("Amount", typeof(System.Decimal));
                                        dtB.Columns.Add("Year", typeof(System.String));
                                        for (int g = 0; g < dtA.Rows.Count; g++)
                                        {
                                            bool er = false;
                                            for (int j = 0; j < dtB.Rows.Count; j++)
                                            {
                                                if (dtB.Rows[j]["COACode"].ToString() == dtA.Rows[g]["COACode"].ToString() && dtB.Rows[j]["Year"].ToString() == dtA.Rows[g]["Year"].ToString())//已有记录
                                                {
                                                    er = true;
                                                    break;
                                                }
                                            }
                                            if (!er)//不存在重复记录
                                            {
                                                DataRow dr = dtB.NewRow();
                                                dr["EName"] = dtA.Rows[g]["EName"].ToString();
                                                dr["COACode"] = dtA.Rows[g]["COACode"].ToString();
                                                dr["Amount"] = dtA.Compute("Sum(Amount)", "Year = " + dtA.Rows[g]["Year"].ToString() + " and COACode = " + dtA.Rows[g]["COACode"].ToString());
                                                dr["Year"] = dtA.Rows[g]["Year"].ToString();
                                                dtB.Rows.Add(dr);
                                            }
                                        }
                                        string userid = dtf.Rows[0]["PersonID"].ToString();
                                        string ostation = ""; string station = ""; string department = "";
                                        DataSet ds2 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(userid);
                                        if (ds2.Tables[0].Rows.Count == 1)
                                        {
                                            DataTable dt1 = ds2.Tables[0];
                                            ostation = dt1.Rows[0]["CostCenter"].ToString();//记录用户预算站点,即CostCenter
                                            station = dt1.Rows[0]["stationCode"].ToString();//记录用户所在站点
                                            department = dt1.Rows[0]["CRPDepartmentName"].ToString();
                                        }
                                        string accountcode = "";
                                        for (int g = 0; g < dtB.Rows.Count; g++)
                                        {
                                            if (Convert.ToDecimal(dtB.Rows[g]["Amount"].ToString()) != 0)
                                            {
                                                DataRow dr = dtbudget.NewRow();
                                                dr["EName"] = dtB.Rows[g]["EName"].ToString();
                                                dr["Current"] = Convert.ToDecimal(dtB.Rows[g]["Amount"].ToString());
                                                dr["COACode"] = dtB.Rows[g]["COACode"].ToString();
                                                dr["Year"] = dtB.Rows[g]["Year"].ToString();
                                                accountcode = dtB.Rows[g]["COACode"].ToString();
                                                DataTable dtC = new DataTable();
                                                dtC = Comm.RtnEB(userid, department, ostation, ostation, accountcode, dtB.Rows[g]["Year"].ToString(), "1");
                                                for (int f = 0; f < dtC.Rows.Count; f++)
                                                {
                                                    if (dtC.Rows[f]["Type"].ToString() == "全年个人")
                                                    {
                                                        dr["PU"] = Convert.ToDecimal(dtC.Rows[f]["Used"].ToString());
                                                        dr["PB"] = Convert.ToDecimal(dtC.Rows[f]["Budget"].ToString());
                                                    }
                                                    else if (dtC.Rows[f]["Type"].ToString() == "全年部门")
                                                    {
                                                        dr["DU"] = Convert.ToDecimal(dtC.Rows[f]["Used"].ToString());
                                                        dr["DB"] = Convert.ToDecimal(dtC.Rows[f]["Budget"].ToString());
                                                    }
                                                    else if (dtC.Rows[f]["Type"].ToString() == "全年站点")
                                                    {
                                                        dr["SU"] = Convert.ToDecimal(dtC.Rows[f]["Used"].ToString());
                                                        dr["SB"] = Convert.ToDecimal(dtC.Rows[f]["Budget"].ToString());
                                                    }
                                                }
                                                dtbudget.Rows.Add(dr);
                                            }
                                        }
                                        //计算%,取得名称,转为本地币种汇率,增加列记录Currency为邮件准备
                                        for (int g = 0; g < dtbudget.Rows.Count; g++)
                                        {
                                            if (CurLocal != CurBudget)
                                            {
                                                rate = DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(CurLocal, CurBudget, Convert.ToInt16(dtbudget.Rows[g]["Year"].ToString()));
                                            }

                                            if (Convert.ToDecimal(dtbudget.Rows[g]["PB"].ToString()) != 0)//如果Budget不为0,则计算%
                                            {
                                                dtbudget.Rows[g]["PPercent"] = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[g]["Current"].ToString()) / rate + Convert.ToDecimal(dtbudget.Rows[g]["PU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[g]["PB"].ToString()), 2);

                                            }
                                            if (Convert.ToDecimal(dtbudget.Rows[g]["DB"].ToString()) != 0)//如果Budget不为0,则计算%
                                            {
                                                dtbudget.Rows[g]["DPercent"] = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[g]["Current"].ToString()) / rate + Convert.ToDecimal(dtbudget.Rows[g]["DU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[g]["DB"].ToString()), 2);

                                            }
                                            if (Convert.ToDecimal(dtbudget.Rows[g]["SB"].ToString()) != 0)//如果Budget不为0,则计算%
                                            {
                                                dtbudget.Rows[g]["SPercent"] = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[g]["Current"].ToString()) / rate + Convert.ToDecimal(dtbudget.Rows[g]["SU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[g]["SB"].ToString()), 2);

                                            }
                                            dtbudget.Rows[g]["PU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["PU"].ToString()), 2);
                                            dtbudget.Rows[g]["PB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["PB"].ToString()), 2);
                                            dtbudget.Rows[g]["DU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["DU"].ToString()), 2);
                                            dtbudget.Rows[g]["DB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["DB"].ToString()), 2);
                                            dtbudget.Rows[g]["SU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["SU"].ToString()), 2);
                                            dtbudget.Rows[g]["SB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["SB"].ToString()), 2);

                                            dtbudget.Rows[g]["RequestID"] = ID;
                                            dtbudget.Rows[g]["Status"] = 2;
                                            dtbudget.Rows[g]["LocalCur"] = CurLocal;
                                            dtbudget.Rows[g]["CenterCur"] = CurBudget;
                                            dtbudget.Rows[g]["Rate"] = rate;
                                        }
                                        string srw = "";
                                        for (int g = 0; g < dtbudget.Rows.Count; g++)
                                        {
                                            SqlCommand scdetail = sqlConn.CreateCommand();
                                            scdetail.CommandText = "Insert into Budget_Complete (Year,FormType,RequestID,Status,COACode,EName,LocalCur,CenterCur,Rate,LocalAmount,PU,PB,PPercent,DU,DB,DPercent,SU,SB,SPercent) values (@Year,@FormType,@RequestID,@Status,@COACode,@EName,@LocalCur,@CenterCur,@Rate,@LocalAmount,@PU,@PB,@PPercent,@DU,@DB,@DPercent,@SU,@SB,@SPercent)";
                                            SqlParameter spdetail = new SqlParameter("@RequestID", SqlDbType.Int);
                                            spdetail.Value = Convert.ToInt32(ID);
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@Year", SqlDbType.Int);
                                            spdetail.Value = Convert.ToInt16(dtbudget.Rows[g]["Year"].ToString());
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@FormType", SqlDbType.VarChar, 10);
                                            spdetail.Value = "G";
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@Status", SqlDbType.Int);
                                            spdetail.Value = Convert.ToInt16(dtbudget.Rows[g]["Status"].ToString());
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@COACode", SqlDbType.VarChar, 50);
                                            spdetail.Value = dtbudget.Rows[g]["COACode"].ToString();
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@EName", SqlDbType.VarChar, 50);
                                            spdetail.Value = dtbudget.Rows[g]["EName"].ToString();
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@LocalCur", SqlDbType.VarChar, 10);
                                            spdetail.Value = dtbudget.Rows[g]["LocalCur"].ToString();
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@CenterCur", SqlDbType.VarChar, 10);
                                            spdetail.Value = dtbudget.Rows[g]["CenterCur"].ToString();
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@Rate", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["Rate"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@LocalAmount", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["Current"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@PU", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["PU"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@PB", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["PB"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@PPercent", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["PPercent"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@DU", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["DU"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@DB", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["DB"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@DPercent", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["DPercent"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@SU", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["SU"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@SB", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["SB"];
                                            scdetail.Parameters.Add(spdetail);

                                            spdetail = new SqlParameter("@SPercent", SqlDbType.Decimal);
                                            spdetail.Value = dtbudget.Rows[g]["SPercent"];
                                            scdetail.Parameters.Add(spdetail);

                                            sqlConn.Open();
                                            int row = scdetail.ExecuteNonQuery();
                                            sqlConn.Close();
                                        }
                                        if (!SendMail("G", para, dt.Rows[i]["id"].ToString(), dtbudget))//Budget已经计入Current,%不需要重新计算
                                        {
                                            ErrorHandle("Error mail address."); return;
                                        }
                                    }
                                    catch (Exception)
                                    {

                                        throw;
                                    }


                                    //if (!SendMail(paraType, para, dt.Rows[i]["id"].ToString(), dtbudget))
                                    //{
                                    //    ErrorHandle("Error mail address."); return;
                                    //}
                                    if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                    {
                                        X.AddScript("Ext.Msg.show({ title: '提示', msg: '批准成功,该申请单已经完成.', buttons: { ok: 'Ok' }, fn: function (btn) { parent.Window1.hide();parent.Button1.fireEvent('click'); } });");
                                    }
                                    else
                                    {
                                        X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Approved,form completed.', buttons: { ok: 'Ok' }, fn: function (btn) { parent.Window1.hide();parent.Button1.fireEvent('click'); } });");
                                    }
                                }
                            }
                            
                        }
                        else//审批通过,但未完成
                        {
                            string updatesql = "update Eflow set Active=-1,Status=2,ApproveDate='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' where id=" + dt.Rows[i]["id"].ToString();
                            updatesql += ";update Eflow set Active=1 where id=" + dt.Rows[i + 1]["id"].ToString();
                            string newid = dbc.UpdateData("eReimbursement", updatesql, "Update");
                            if (newid == "-1")
                            {
                                ErrorHandle("Data Error.");
                                return;
                            }
                            else
                            {
                                if (paraType == "T")
                                {
                                    string sqlf = "select * from ETravel where ID=" + ID;
                                    DataTable dtf = dbc.GetData("eReimbursement", sqlf);

                                    string sqld = "select * from ETraveleDetail where [No]='" + ID + "' order by id";
                                    DataTable dtall = new DataTable();
                                    dtall = dbc.GetData("eReimbursement", sqld);
                                    //预算
                                    //140226 显示预算
                                    DataTable dtbudget = new DataTable();
                                    dtbudget.Columns.Add("RequestID", typeof(System.Int32));
                                    dtbudget.Columns.Add("Status", typeof(System.Int16));
                                    dtbudget.Columns.Add("EName", typeof(System.String));
                                    dtbudget.Columns.Add("COACode", typeof(System.String));
                                    dtbudget.Columns.Add("LocalCur", typeof(System.String));
                                    dtbudget.Columns.Add("CenterCur", typeof(System.String));
                                    dtbudget.Columns.Add("Rate", typeof(System.Decimal));
                                    dtbudget.Columns.Add("Current", typeof(System.Decimal));
                                    dtbudget.Columns.Add("PA", typeof(System.Decimal));
                                    dtbudget.Columns.Add("CA", typeof(System.Decimal));
                                    dtbudget.Columns.Add("PU", typeof(System.Decimal));
                                    dtbudget.Columns.Add("PB", typeof(System.Decimal));
                                    dtbudget.Columns.Add("PPercent", typeof(System.Decimal));
                                    dtbudget.Columns.Add("DU", typeof(System.Decimal));
                                    dtbudget.Columns.Add("DB", typeof(System.Decimal));
                                    dtbudget.Columns.Add("DPercent", typeof(System.Decimal));
                                    dtbudget.Columns.Add("SU", typeof(System.Decimal));
                                    dtbudget.Columns.Add("SB", typeof(System.Decimal));
                                    dtbudget.Columns.Add("SPercent", typeof(System.Decimal));

                                    //取得预算日期
                                    string sqlA = "select convert(varchar(10),min(Tdate0),111) as BudgetDate from ETraveleDetail where No='" + ID + "'";
                                    DataTable dtA = dbc.GetData("eReimbursement", sqlA);
                                    //取得本币与成本中心汇率转换
                                    decimal rate = 1;
                                    string CurLocal = dtall.Rows[0]["Cur"].ToString();
                                    string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dtf.Rows[0]["Station2"].ToString());
                                    if (CurLocal != CurBudget)
                                    {
                                        rate = DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(CurLocal, CurBudget, Convert.ToDateTime(dtA.Rows[0]["BudgetDate"].ToString()).Year);
                                    }

                                    //取得4大类合计
                                    //string sqlB = "select sum(T1) as T1,sum(T2) as T2,sum(T3) as T3,sum(T4) as T4 from (select case when AccountCode='62012000' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T1],case when AccountCode='62010900' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T2],case when AccountCode='62011900' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T3],case when AccountCode='62010500' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T4] from ETraveleDetail where No=" + ID + ") t";
                                    string sqlB = "select sum(isnull(Pamount,0)) as PA,sum(isnull(Camount,0)) as CA,sum(isnull(Pamount,0)+isnull(Camount,0)) as Amount,'62012000' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62012000' union all select sum(isnull(Pamount,0)) as PA,sum(isnull(Camount,0)) as CA,sum(isnull(Pamount,0)+isnull(Camount,0)) as Amount,'62010900' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62010900' union all select sum(isnull(Pamount,0)) as PA,sum(isnull(Camount,0)) as CA,sum(isnull(Pamount,0)+isnull(Camount,0)) as Amount,'62011900' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62011900' union all select sum(isnull(Pamount,0)) as PA,sum(isnull(Camount,0)) as CA,sum(isnull(Pamount,0)+isnull(Camount,0)) as Amount,'62010500' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62010500'";
                                    DataTable dtB = dbc.GetData("eReimbursement", sqlB);
                                    //取得传递预算的参数
                                    string userid = dtf.Rows[0]["PersonID"].ToString();
                                    string dpt = dtf.Rows[0]["Department"].ToString();
                                    string ostation = dtf.Rows[0]["Station2"].ToString();//预算站点,与基本信息中的CostCenter一致(Station2)
                                    string tstation = ostation;//Etravel表中的Station2,目前与预算站点一致,不允许更改
                                    string year = Convert.ToDateTime(dtA.Rows[0]["BudgetDate"].ToString()).Year.ToString();
                                    string month = Convert.ToDateTime(dtA.Rows[0]["BudgetDate"].ToString()).Month.ToString();
                                    string accountcode = "";
                                    for (int g = 0; g < dtB.Rows.Count; g++)
                                    {
                                        if (Convert.ToDecimal(dtB.Rows[g]["Amount"].ToString()) != 0)
                                        {
                                            DataRow dr = dtbudget.NewRow();
                                            dr["Current"] = Convert.ToDecimal(dtB.Rows[g]["Amount"].ToString());
                                            dr["PA"] = Convert.ToDecimal(dtB.Rows[g]["PA"].ToString());
                                            dr["CA"] = Convert.ToDecimal(dtB.Rows[g]["CA"].ToString());
                                            dr["COACode"] = dtB.Rows[g]["COACode"].ToString();
                                            accountcode = dtB.Rows[g]["COACode"].ToString();
                                            DataTable dtC = new DataTable();
                                            dtC = Comm.ExRtnEB(userid, dpt, ostation, tstation, accountcode, year, month, "T", ID);

                                            for (int h = 0; h < dtC.Rows.Count; h++)
                                            {
                                                if (dtC.Rows[h]["Type"].ToString() == "全年个人")
                                                {
                                                    dr["PU"] = Convert.ToDecimal(dtC.Rows[h]["Used"].ToString());
                                                    dr["PB"] = Convert.ToDecimal(dtC.Rows[h]["Budget"].ToString());
                                                }
                                                else if (dtC.Rows[h]["Type"].ToString() == "全年部门")
                                                {
                                                    dr["DU"] = Convert.ToDecimal(dtC.Rows[h]["Used"].ToString());
                                                    dr["DB"] = Convert.ToDecimal(dtC.Rows[h]["Budget"].ToString());
                                                }
                                                else if (dtC.Rows[h]["Type"].ToString() == "全年站点")
                                                {
                                                    dr["SU"] = Convert.ToDecimal(dtC.Rows[h]["Used"].ToString());
                                                    dr["SB"] = Convert.ToDecimal(dtC.Rows[h]["Budget"].ToString());
                                                }
                                            }
                                            dtbudget.Rows.Add(dr);
                                        }
                                    }
                                    //计算%,取得名称,预算转换为本地汇率
                                    for (int g = 0; g < dtbudget.Rows.Count; g++)
                                    {
                                        if (Convert.ToDecimal(dtbudget.Rows[g]["PB"].ToString()) != 0)//如果Budget不为0,则计算%
                                        {
                                            dtbudget.Rows[g]["PPercent"] = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[g]["Current"].ToString()) / rate + Convert.ToDecimal(dtbudget.Rows[g]["PU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[g]["PB"].ToString()), 2);

                                        }
                                        if (Convert.ToDecimal(dtbudget.Rows[g]["DB"].ToString()) != 0)//如果Budget不为0,则计算%
                                        {
                                            dtbudget.Rows[g]["DPercent"] = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[g]["Current"].ToString()) / rate + Convert.ToDecimal(dtbudget.Rows[g]["DU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[g]["DB"].ToString()), 2);

                                        }
                                        if (Convert.ToDecimal(dtbudget.Rows[g]["SB"].ToString()) != 0)//如果Budget不为0,则计算%
                                        {
                                            dtbudget.Rows[g]["SPercent"] = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[g]["Current"].ToString()) / rate + Convert.ToDecimal(dtbudget.Rows[g]["SU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[g]["SB"].ToString()), 2);

                                        }
                                        if (dtbudget.Rows[g]["COACode"].ToString() == "62012000")
                                        {
                                            dtbudget.Rows[g]["EName"] = "Travel expense";
                                        }
                                        else if (dtbudget.Rows[g]["COACode"].ToString() == "62010900")
                                        {
                                            dtbudget.Rows[g]["EName"] = "Entertainment";
                                        }
                                        else if (dtbudget.Rows[g]["COACode"].ToString() == "62011900")
                                        {
                                            dtbudget.Rows[g]["EName"] = "Transportation";
                                        }
                                        else if (dtbudget.Rows[g]["COACode"].ToString() == "62010500")
                                        {
                                            dtbudget.Rows[g]["EName"] = "Communication";
                                        }
                                        dtbudget.Rows[g]["PU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["PU"].ToString()), 2);
                                        dtbudget.Rows[g]["PB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["PB"].ToString()), 2);
                                        dtbudget.Rows[g]["DU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["DU"].ToString()), 2);
                                        dtbudget.Rows[g]["DB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["DB"].ToString()), 2);
                                        dtbudget.Rows[g]["SU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["SU"].ToString()), 2);
                                        dtbudget.Rows[g]["SB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["SB"].ToString()), 2);
                                        dtbudget.Rows[g]["RequestID"] = ID;
                                        dtbudget.Rows[g]["Status"] = 3;
                                        dtbudget.Rows[g]["LocalCur"] = CurLocal;
                                        dtbudget.Rows[g]["CenterCur"] = CurBudget;
                                        dtbudget.Rows[g]["Rate"] = rate;
                                    }
                                    if (!SendMail(paraType, para, dt.Rows[i + 1]["id"].ToString(), dtbudget))//Budget已经计入Current,%不需要重新计算
                                    {
                                        ErrorHandle("Error mail address."); return;
                                    }
                                    if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                    {
                                        X.AddScript("Ext.Msg.show({ title: '提示', msg: '批准成功,该申请单已经完成.', buttons: { ok: 'Ok' }, fn: function (btn) { parent.Window1.hide();parent.Button1.fireEvent('click'); } });");
                                    }
                                    else
                                    {
                                        X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Approved.', buttons: { ok: 'Ok' }, fn: function (btn) { parent.Window1.hide();parent.Button1.fireEvent('click'); } });");
                                    }
                                }
                                else//通用费用
                                {
                                    string sqlf = "select * from Ecommon where ID=" + ID;
                                    DataTable dtf = dbc.GetData("eReimbursement", sqlf);
                                    //140226 显示预算
                                    DataTable dtbudget = new DataTable();
                                    dtbudget.Columns.Add("RequestID", typeof(System.Int32));
                                    dtbudget.Columns.Add("Year", typeof(System.String));
                                    dtbudget.Columns.Add("Status", typeof(System.Int16));
                                    dtbudget.Columns.Add("EName", typeof(System.String));
                                    dtbudget.Columns.Add("COACode", typeof(System.String));
                                    dtbudget.Columns.Add("LocalCur", typeof(System.String));
                                    dtbudget.Columns.Add("CenterCur", typeof(System.String));
                                    dtbudget.Columns.Add("Rate", typeof(System.Decimal));
                                    dtbudget.Columns.Add("Current", typeof(System.Decimal));
                                    dtbudget.Columns.Add("PU", typeof(System.Decimal));
                                    dtbudget.Columns.Add("PB", typeof(System.Decimal));
                                    dtbudget.Columns.Add("PPercent", typeof(System.Decimal));
                                    dtbudget.Columns.Add("DU", typeof(System.Decimal));
                                    dtbudget.Columns.Add("DB", typeof(System.Decimal));
                                    dtbudget.Columns.Add("DPercent", typeof(System.Decimal));
                                    dtbudget.Columns.Add("SU", typeof(System.Decimal));
                                    dtbudget.Columns.Add("SB", typeof(System.Decimal));
                                    dtbudget.Columns.Add("SPercent", typeof(System.Decimal));

                                    string sqld = "select * from EeommonDetail where [No]='" + ID + "' order by id";
                                    DataTable dtall = new DataTable();
                                    dtall = dbc.GetData("eReimbursement", sqld);
                                    //取得预算日期
                                    string sqlA = "select AccountCode as COACode,case when t1.Type='O' then t2.SAccountName else TDicSubType.EText end as [EName],year(Tdate) as [Year],Amount from EeommonDetail t1 left join (select * from Edic where KeyValue='SubType') TDicSubType on TDicSubType.CValue=t1.Type left join AccoundCode t2 on t2.SAccountCode=t1.AccountCode where t1.No='" + ID + "'";
                                    DataTable dtA = dbc.GetData("eReimbursement", sqlA);
                                    //取得本币与成本中心汇率转换
                                    decimal rate = 1;
                                    string CurLocal = dtall.Rows[0]["Cur"].ToString();
                                    string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dtf.Rows[0]["Station2"].ToString());
                                    

                                    //合计数据
                                    DataTable dtB = new DataTable();
                                    dtB.Columns.Add("EName", typeof(System.String));
                                    dtB.Columns.Add("COACode", typeof(System.String));
                                    dtB.Columns.Add("Amount", typeof(System.Decimal));
                                    dtB.Columns.Add("Year", typeof(System.String));
                                    for (int g = 0; g < dtA.Rows.Count; g++)
                                    {
                                        bool er = false;
                                        for (int j = 0; j < dtB.Rows.Count; j++)
                                        {
                                            if (dtB.Rows[j]["COACode"].ToString() == dtA.Rows[g]["COACode"].ToString() && dtB.Rows[j]["Year"].ToString() == dtA.Rows[g]["Year"].ToString())//已有记录
                                            {
                                                er = true;
                                                break;
                                            }
                                        }
                                        if (!er)//不存在重复记录
                                        {
                                            DataRow dr = dtB.NewRow();
                                            dr["EName"] = dtA.Rows[g]["EName"].ToString();
                                            dr["COACode"] = dtA.Rows[g]["COACode"].ToString();
                                            dr["Amount"] = dtA.Compute("Sum(Amount)", "Year = " + dtA.Rows[g]["Year"].ToString() + " and COACode = " + dtA.Rows[g]["COACode"].ToString());
                                            dr["Year"] = dtA.Rows[g]["Year"].ToString();
                                            dtB.Rows.Add(dr);
                                        }
                                    }
                                    string userid = dtf.Rows[0]["PersonID"].ToString();
                                    string ostation = ""; string station = ""; string department = "";
                                    DataSet ds2 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(userid);
                                    if (ds2.Tables[0].Rows.Count == 1)
                                    {
                                        DataTable dt1 = ds2.Tables[0];
                                        ostation = dt1.Rows[0]["CostCenter"].ToString();//记录用户预算站点,即CostCenter
                                        station = dt1.Rows[0]["stationCode"].ToString();//记录用户所在站点
                                        department = dt1.Rows[0]["CRPDepartmentName"].ToString();
                                    }
                                    string accountcode = "";
                                    for (int g = 0; g < dtB.Rows.Count; g++)
                                    {
                                        if (Convert.ToDecimal(dtB.Rows[g]["Amount"].ToString()) != 0)
                                        {
                                            DataRow dr = dtbudget.NewRow();
                                            dr["EName"] = dtB.Rows[g]["EName"].ToString();
                                            dr["Current"] = Convert.ToDecimal(dtB.Rows[g]["Amount"].ToString());
                                            dr["COACode"] = dtB.Rows[g]["COACode"].ToString();
                                            dr["Year"] = dtB.Rows[g]["Year"].ToString();
                                            accountcode = dtB.Rows[g]["COACode"].ToString();
                                            DataTable dtC = new DataTable();
                                            dtC = Comm.RtnEB(userid, department, ostation, ostation, accountcode, dtB.Rows[g]["Year"].ToString(), "1");
                                            for (int f = 0; f < dtC.Rows.Count; f++)
                                            {
                                                if (dtC.Rows[f]["Type"].ToString() == "全年个人")
                                                {
                                                    dr["PU"] = Convert.ToDecimal(dtC.Rows[f]["Used"].ToString());
                                                    dr["PB"] = Convert.ToDecimal(dtC.Rows[f]["Budget"].ToString());
                                                }
                                                else if (dtC.Rows[f]["Type"].ToString() == "全年部门")
                                                {
                                                    dr["DU"] = Convert.ToDecimal(dtC.Rows[f]["Used"].ToString());
                                                    dr["DB"] = Convert.ToDecimal(dtC.Rows[f]["Budget"].ToString());
                                                }
                                                else if (dtC.Rows[f]["Type"].ToString() == "全年站点")
                                                {
                                                    dr["SU"] = Convert.ToDecimal(dtC.Rows[f]["Used"].ToString());
                                                    dr["SB"] = Convert.ToDecimal(dtC.Rows[f]["Budget"].ToString());
                                                }
                                            }
                                            dtbudget.Rows.Add(dr);
                                        }
                                    }
                                    //计算%,取得名称,转为本地币种汇率,增加列记录Currency为邮件准备
                                    for (int g = 0; g < dtbudget.Rows.Count; g++)
                                    {
                                        if (CurLocal != CurBudget)
                                        {
                                            rate = DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(CurLocal, CurBudget, Convert.ToInt16(dtbudget.Rows[g]["Year"].ToString()));
                                        }

                                        if (Convert.ToDecimal(dtbudget.Rows[g]["PB"].ToString()) != 0)//如果Budget不为0,则计算%
                                        {
                                            dtbudget.Rows[g]["PPercent"] = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[g]["Current"].ToString())/rate + Convert.ToDecimal(dtbudget.Rows[g]["PU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[g]["PB"].ToString()), 2);

                                        }
                                        if (Convert.ToDecimal(dtbudget.Rows[g]["DB"].ToString()) != 0)//如果Budget不为0,则计算%
                                        {
                                            dtbudget.Rows[g]["DPercent"] = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[g]["Current"].ToString()) / rate + Convert.ToDecimal(dtbudget.Rows[g]["DU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[g]["DB"].ToString()), 2);

                                        }
                                        if (Convert.ToDecimal(dtbudget.Rows[g]["SB"].ToString()) != 0)//如果Budget不为0,则计算%
                                        {
                                            dtbudget.Rows[g]["SPercent"] = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[g]["Current"].ToString()) / rate + Convert.ToDecimal(dtbudget.Rows[g]["SU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[g]["SB"].ToString()), 2);

                                        }
                                        
                                        dtbudget.Rows[g]["PU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["PU"].ToString()), 2);
                                        dtbudget.Rows[g]["PB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["PB"].ToString()), 2);
                                        dtbudget.Rows[g]["DU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["DU"].ToString()), 2);
                                        dtbudget.Rows[g]["DB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["DB"].ToString()), 2);
                                        dtbudget.Rows[g]["SU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["SU"].ToString()), 2);
                                        dtbudget.Rows[g]["SB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["SB"].ToString()), 2);

                                        dtbudget.Rows[g]["RequestID"] = ID;
                                        dtbudget.Rows[g]["Status"] = 2;
                                        dtbudget.Rows[g]["LocalCur"] = CurLocal;
                                        dtbudget.Rows[g]["CenterCur"] = CurBudget;
                                        dtbudget.Rows[g]["Rate"] = rate;
                                        
                                    }
                                    if (!SendMail("G", para, dt.Rows[i + 1]["id"].ToString(), dtbudget))//Budget已经计入Current,%不需要重新计算
                                    {
                                        ErrorHandle("Error mail address."); return;
                                    }
                                    string srw = "";
                                    //DataTable dtbudget = new DataTable();
                                    //if (!SendMail(paraType, para, dt.Rows[i + 1]["id"].ToString(), dtbudget))
                                    //{
                                    //    ErrorHandle("Error mail address."); return;
                                    //}
                                    if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                    {
                                        X.AddScript("Ext.Msg.show({ title: '提示', msg: '批准成功,该申请单已经完成.', buttons: { ok: 'Ok' }, fn: function (btn) { parent.Window1.hide();parent.Button1.fireEvent('click'); } });");
                                    }
                                    else
                                    {
                                        X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Approved.', buttons: { ok: 'Ok' }, fn: function (btn) { parent.Window1.hide();parent.Button1.fireEvent('click'); } });");
                                    }
                                }
                                
                            }
                        }
                        break;
                    }
                }
            }
        }
        protected void ErrorHandle()
        {
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Message",
                Message = "Data Error.",
                Buttons = MessageBox.Button.OK,
                Width = 250,
                Icon = MessageBox.Icon.WARNING
            });
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
        protected bool SendMail(string type,string RequestID,string FlowID,DataTable dtPar)
        {
            //发送提醒邮件
            cs.DBCommand dbc = new cs.DBCommand();
            string sql = "select * from V_Eflow_ETravel where [Type]='" + type + "' and Step!=0 and RequestID=" + RequestID + " order by Step,FlowID";
            DataTable dtMail = new DataTable();
            dtMail = dbc.GetData("eReimbursement", sql);
            //string budget = dtMail.Rows[0]["Budget"].ToString() == "1" ? "(Budgeted)" : "(UnBudgeted)";
            //14/10/24
            string budget = "";
            if (dtMail.Rows[0]["Budget"].ToString() == "1")
            {
                budget = "(Budgeted)";
            }
            else if (dtMail.Rows[0]["Budget"].ToString() == "0")
            {
                budget = "(UnBudgeted)";
            }
            else if (dtMail.Rows[0]["Budget"].ToString() == "-1")
            {
                budget = "(Over-Budgeted)";
            }
            else if (dtMail.Rows[0]["Budget"].ToString() == "-2")
            {
                budget = "(UnBudgeted & Over-Budgeted)";
            }
            if (dtMail != null && dtMail.Rows.Count > 0)
            {
                DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();
                
                mail.FromDispName = "eReimbursement";
                mail.From = "DIC2@dimerco.com";
                string mailto = ""; string mailcc = "";
                string divstyle = "style='font-size:small;'";
                string divstyleR = "style='font-size:small;color:red;'";
                string divstyleCurrent = "style='font-size:small;color:blue;'";
                string divstyleReject = "style='font-size:small;color:red;'";
                string divstylered = "style='border:silver 1px ridge; font-size:small;background-color: #FFFFFF;empty-cells:show;color:red;' width='110px' align='right'";
                string tdstyle = "style='border:silver 1px ridge; font-size:small;background-color: #FFFFFF;empty-cells:show;' width='110px' align='right'";
                StringBuilder sb = new StringBuilder();
                //sb.Append("<div " + divstyleReject + ">THIS IS A TEST MAIL.</div><br />");
                //sb.Append("<div>");
                //基本信息
                string sqlbase = "select * from V_Eflow_ETravel where FlowID='" + FlowID + "'";
                DataTable dtbase = new DataTable();
                dtbase = dbc.GetData("eReimbursement", sqlbase);
                if (dtbase.Rows[0]["Active"].ToString()=="2")//完成
                {
                    if (dtbase.Rows[0]["Status"].ToString()=="3")//完成:拒绝
                    {
                        //mail.Title = "Dimerco eReimbursement "+budget+" " + dtMail.Rows[0]["Person"].ToString() + " - Application Rejected.";
                        //16090007E 
                        if (dtMail.Rows[0]["OnBehalfPersonID"].ToString() != "")
                        {
                            mail.Title = "Dimerco eReimbursement on behalf of " + dtMail.Rows[0]["OnBehalfPersonName"].ToString() + " " + budget + " " + dtMail.Rows[0]["Person"].ToString() + " - Application Rejected.";
                        }
                        else
                        {
                            mail.Title = "Dimerco eReimbursement " + budget + " " + dtMail.Rows[0]["Person"].ToString() + " - Application Rejected.";
                        }

                        sb.Append("<div " + divstyle + ">Dear " + dtbase.Rows[0]["Person"].ToString() + ",</div><br />");
                        //sb.Append("<div " + divstyleReject + ">The following eReimbursement application has been rejected.</div><br /><br />");
                        //160115 垫付
                        if (dtMail.Rows[0]["OnBehalfPersonID"].ToString() == "")
                        {
                            sb.Append("<div " + divstyleReject + ">The following eReimbursement application has been rejected:</div><br />");
                        }
                        else
                        {
                            sb.Append("<div " + divstyleReject + ">The following eReimbursement application on behalf of " + dtMail.Rows[0]["OnBehalfPersonUnit"].ToString() + " " + dtMail.Rows[0]["OnBehalfPersonName"].ToString() + " has been rejected:</div><br />");
                        }


                        if (dtbase.Rows[0]["RemarkFlow"].ToString() != "")
                        {
                            sb.Append("<div " + divstyleReject + ">eReimbursement Approval Remarks:" + dtbase.Rows[0]["RemarkFlow"].ToString() + "</div><br /><br />");
                        }
                        else
                        {
                            sb.Append("<br />");
                        }
                        
                        sb.Append("<div " + divstyle + ">No#:" + dtbase.Rows[0]["No"].ToString() +budget+ "</div>");
                        sb.Append("<div " + divstyle + ">Applicant:" + dtbase.Rows[0]["Person"].ToString() + "</div>");
                        sb.Append("<div " + divstyle + ">Unit:" + dtbase.Rows[0]["Station"].ToString() + "</div>");
                        sb.Append("<div " + divstyle + ">Department:" + dtbase.Rows[0]["Department"].ToString() + "</div>");

                        //抄送人,截至通过的所有审批人
                        for (int i = 0; i < dtMail.Rows.Count; i++)
                        {
                            DataSet dsCC = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtMail.Rows[i]["ApproverID"].ToString());
                            if (dsCC.Tables[0].Rows.Count == 1)
                            {
                                //mailcc += dsCC.Tables[0].Rows[0]["eMail"].ToString() + ",";
                                string mcc = dsCC.Tables[0].Rows[0]["eMail"].ToString().Trim();
                                if (mcc != "" && mailcc.ToLower().IndexOf(mcc) == -1)
                                {
                                    mailcc += mcc + ",";
                                }
                            }
                            //else
                            //{
                            //    ErrorHandle("Error mail address of Owner."); return false;
                            //}
                            if (dtMail.Rows[i]["Active"].ToString() == "2")
                            {
                                break;
                            }
                        }
                    }
                    else//完成:通过
                    {
                        //mail.Title = "Dimerco eReimbursement "+budget+" " + dtMail.Rows[0]["Person"].ToString() + " - Application Approved.";
                        //16090007E 
                        if (dtMail.Rows[0]["OnBehalfPersonID"].ToString() != "")
                        {
                            mail.Title = "Dimerco eReimbursement on behalf of " + dtMail.Rows[0]["OnBehalfPersonName"].ToString() + " " + budget + " " + dtMail.Rows[0]["Person"].ToString() + " - Application Approved.";
                        }
                        else
                        {
                            mail.Title = "Dimerco eReimbursement " + budget + " " + dtMail.Rows[0]["Person"].ToString() + " - Application Approved.";
                        }
                        sb.Append("<div " + divstyle + ">Dear " + dtbase.Rows[0]["Person"].ToString() + ",</div><br />");
                        //sb.Append("<div " + divstyle + ">The following eReimbursement application has been approved(Complete).</div><br /><br />");
                        //160115 垫付
                        if (dtMail.Rows[0]["OnBehalfPersonID"].ToString() == "")
                        {
                            sb.Append("<div " + divstyle + ">The following eReimbursement application has been has been approved(Complete):</div><br />");
                        }
                        else
                        {
                            sb.Append("<div " + divstyle + ">The following eReimbursement application on behalf of " + dtMail.Rows[0]["OnBehalfPersonUnit"].ToString() + " " + dtMail.Rows[0]["OnBehalfPersonName"].ToString() + " has been approved(Complete):</div><br />");
                        }


                        if (dtbase.Rows[0]["RemarkFlow"].ToString() != "")
                        {
                            sb.Append("<div " + divstyleReject + ">eReimbursement Approval Remarks:" + dtbase.Rows[0]["RemarkFlow"].ToString() + "</div><br /><br />");
                        }
                        else
                        {
                            sb.Append("<br />");
                        }
                        sb.Append("<div " + divstyle + ">No#:" + dtbase.Rows[0]["No"].ToString() +budget+ "</div>");
                        sb.Append("<div " + divstyle + ">Applicant:" + dtbase.Rows[0]["Person"].ToString() + "</div>");
                        sb.Append("<div " + divstyle + ">Unit:" + dtbase.Rows[0]["Station"].ToString() + "</div>");
                        sb.Append("<div " + divstyle + ">Department:" + dtbase.Rows[0]["Department"].ToString() + "</div>");
                    }
                    //收件人-申请人
                    DataSet dsTo = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtbase.Rows[0]["PersonID"].ToString());
                    if (dsTo!=null && dsTo.Tables.Count>= 1 && dsTo.Tables[0].Rows.Count == 1)
                    {
                        mailto += dsTo.Tables[0].Rows[0]["eMail"].ToString() + ",";
                    }
                    //else
                    //{
                    //    ErrorHandle("Error mail address of Approver."); return false;
                    //}
                    //抄送人,截至通过的所有审批人
                    //for (int i = 0; i < dtMail.Rows.Count; i++)
                    //{
                    //    DataSet dsCC = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtMail.Rows[i]["ApproverID"].ToString());
                    //    if (dsCC.Tables[0].Rows.Count == 1)
                    //    {
                    //        mailcc += dsCC.Tables[0].Rows[0]["eMail"].ToString() + ",";
                    //    }
                    //    else
                    //    {
                    //        ErrorHandle("Error mail address of Owner."); return false;
                    //    }
                    //    if (dtMail.Rows[i]["Active"].ToString() == "2")
                    //    {
                    //        break;
                    //    }
                    //}
                    if (dtbase.Rows[0]["CreadedByID"].ToString() != "" && dtbase.Rows[0]["CreadedByID"].ToString() != dtbase.Rows[0]["PersonID"].ToString())//代理人
                    {
                        DataSet dsCC = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtbase.Rows[0]["CreadedByID"].ToString());
                        if (dsCC.Tables[0].Rows.Count == 1)
                        {
                            //mailcc += dsCC.Tables[0].Rows[0]["eMail"].ToString() + ",";
                            string mcc = dsCC.Tables[0].Rows[0]["eMail"].ToString().Trim();
                            if (mcc != "" && mailcc.ToLower().IndexOf(mcc) == -1)
                            {
                                mailcc += mcc + ",";
                            }
                        }
                        //else
                        //{
                        //    ErrorHandle("Error mail address of Owner."); return false;
                        //}
                    }
                    //如果完成,抄送所有被设置CC的邮件
                    //if (dtMail.Rows[0]["CCMailList"].ToString() != "")
                    //{
                    //    mailcc += dtMail.Rows[0]["CCMailList"].ToString();
                    //}
                    if (dtMail.Rows[0]["CCMailList"].ToString() != "")
                    {
                        //mailcc += dtMail.Rows[0]["CCMailList"].ToString();
                        string mcc = dtMail.Rows[0]["CCMailList"].ToString().Trim();
                        string[] mcclist = mcc.Split(',');
                        for (int i = 0; i < mcclist.Length; i++)
                        {
                            if (mcclist[i] != "" && mailcc.ToLower().IndexOf(mcclist[i]) == -1)
                            {
                                mailcc += mcclist[i] + ",";
                            }
                        }
                    }
                    //固定抄送人
                    string sqlmc = "select * from MailSetting where UserID='" + dtMail.Rows[0]["PersonID"].ToString() + "' and MailList!=''";
                    DataTable dtmc = new DataTable();
                    dtmc = dbc.GetData("eReimbursement", sqlmc);
                    if (dtmc != null && dtmc.Rows.Count == 1)
                    {
                        string mcc = dtmc.Rows[0]["MailList"].ToString().Trim();
                        string[] mcclist = mcc.Split(',');
                        for (int i = 0; i < mcclist.Length; i++)
                        {
                            if (mcclist[i] != "" && mailcc.ToLower().IndexOf(mcclist[i]) == -1)
                            {
                                mailcc += mcclist[i] + ",";
                            }
                        }
                    }
                }
                else//审批中,未完成
                {
                    string msg = "";
                    //if (dtbase.Rows[0]["FlowFn"].ToString().ToLower() == "verifier")
                    //{
                    //    msg = "Process Checking.";
                    //}
                    //else if (dtbase.Rows[0]["FlowFn"].ToString().ToLower() == "issuer")
                    //{
                    //    msg = "Process Paying.";
                    //}
                    //else
                    //{
                    //    msg = "Seek For Your Approval.";
                    //}

                    
                    for (int i = 0; i < dtMail.Rows.Count; i++)
                    {
                        if (dtMail.Rows[i]["Active"].ToString() == "1")
                        {
                            if (dtMail.Rows[i]["FlowFn"].ToString().ToLower() == "verifier")
                            {
                                msg = "Process Checking.";
                            }
                            else if (dtMail.Rows[i]["FlowFn"].ToString().ToLower() == "issuer")
                            {
                                msg = "Process Paying.";
                            }
                            else if (dtMail.Rows[i]["FlowFn"].ToString().ToLower() == "confirm")
                            {
                                msg = "Seek For Your Confirmation.";
                            }
                            else
                            {
                                msg = "Seek For Your Approval.";
                            }

                            sb.Append("<div " + divstyle + ">Dear " + dtMail.Rows[i]["Approver"].ToString() + ",</div><br />");
                            //sb.Append("<div " + divstyle + ">The following eReimbursement application: "+msg+"</div><br /><br />");
                            //160115 垫付
                            if (dtMail.Rows[0]["OnBehalfPersonID"].ToString() == "")
                            {
                                sb.Append("<div " + divstyle + ">The following eReimbursement application: " + msg + "</div><br />");
                            }
                            else
                            {
                                sb.Append("<div " + divstyle + ">The following eReimbursement application on behalf of " + dtMail.Rows[0]["OnBehalfPersonUnit"].ToString() + " " + dtMail.Rows[0]["OnBehalfPersonName"].ToString() + ": " + msg + "</div><br />");
                            }


                            if (dtMail.Rows[i - 1]["RemarkFlow"].ToString() != "")
                            {
                                sb.Append("<div " + divstyleReject + ">eReimbursement Approval Remarks:" + dtMail.Rows[i - 1]["RemarkFlow"].ToString() + "</div><br /><br />");
                            }
                            else
                            {
                                sb.Append("<br />");
                            }
                            sb.Append("<div " + divstyle + ">No#:" + dtbase.Rows[0]["No"].ToString() +budget+ "</div>");
                            sb.Append("<div " + divstyle + ">Applicant:" + dtbase.Rows[0]["Person"].ToString() + "</div>");
                            sb.Append("<div " + divstyle + ">Unit:" + dtbase.Rows[0]["Station"].ToString() + "</div>");
                            sb.Append("<div " + divstyle + ">Department:" + dtbase.Rows[0]["Department"].ToString() + "</div>");
                            //收件人
                            DataSet dsTo = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtMail.Rows[i]["ApproverID"].ToString());//下一级审批人
                            if (dsTo.Tables[0].Rows.Count == 1)
                            {
                                mailto += dsTo.Tables[0].Rows[0]["eMail"].ToString() + ",";
                            }
                            //else
                            //{
                            //    ErrorHandle("Error mail address."); return false;
                            //}
                            //抄送人
                            //dsTo = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(Request.Cookies.Get("eReimUserID").Value);//本级审批人
                            //if (dsTo.Tables[0].Rows.Count == 1)
                            //{
                            //    mailcc += dsTo.Tables[0].Rows[0]["eMail"].ToString() + ",";
                            //}
                            //else
                            //{
                            //    ErrorHandle("Error mail address."); return false;
                            //}
                            //抄送人
                            dsTo = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtbase.Rows[0]["PersonID"].ToString());//本人
                            if (dsTo != null && dsTo.Tables.Count >= 1 && dsTo.Tables[0].Rows.Count == 1)
                            {
                                mailcc += dsTo.Tables[0].Rows[0]["eMail"].ToString() + ",";
                            }
                            //else
                            //{
                            //    ErrorHandle("Error mail address."); return false;
                            //}
                            if (dtbase.Rows[0]["CreadedByID"].ToString() != "" && dtbase.Rows[0]["CreadedByID"].ToString() != dtbase.Rows[0]["PersonID"].ToString())//代理人
                            {
                                DataSet dsCC = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtbase.Rows[0]["CreadedByID"].ToString());
                                if (dsCC.Tables[0].Rows.Count == 1)
                                {
                                    //mailcc += dsCC.Tables[0].Rows[0]["eMail"].ToString() + ",";
                                    string mcc = dsCC.Tables[0].Rows[0]["eMail"].ToString().Trim();
                                    if (mcc != "" && mailcc.ToLower().IndexOf(mcc) == -1)
                                    {
                                        mailcc += mcc + ",";
                                    }
                                }
                                //else
                                //{
                                //    ErrorHandle("Error mail address of Owner."); return false;
                                //}
                            }
                        }
                    }
                    //mail.Title = "Dimerco eReimbursement " + budget + " " + dtMail.Rows[0]["Person"].ToString() + " - " + msg;
                    //16090007E 
                    if (dtMail.Rows[0]["OnBehalfPersonID"].ToString() != "")
                    {
                        mail.Title = "Dimerco eReimbursement on behalf of " + dtMail.Rows[0]["OnBehalfPersonName"].ToString() + " " + budget + " " + dtMail.Rows[0]["Person"].ToString() + " - " + msg;
                    }
                    else
                    {
                        mail.Title = "Dimerco eReimbursement " + budget + " " + dtMail.Rows[0]["Person"].ToString() + " - " + msg;
                    }
                }


                //160127 Shanshan提出邮件测试
                string mailtestword = "";
                mailtestword += "<br />Mail to: " + mailto + "<br />";
                mailtestword += "Mail CC: " + mailcc + "<br />";
                DataSet dsowner = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtMail.Rows[0]["PersonID"].ToString());
                if (dsowner != null && dsowner.Tables.Count >= 1 && dsowner.Tables[0].Rows.Count == 1)
                {
                    mail.To = dsowner.Tables[0].Rows[0]["eMail"].ToString();
                }
                string testmailstr = "";
                testmailstr += "<div " + divstyleReject + ">THIS IS A TEST MAIL." + mailtestword + "</div><br />";
                testmailstr += "<div>";

                //mail.To = mailto;
                //mail.Cc = mailcc;



                if (type == "G")//通用费用
                {
                    DataTable dtpar1 = dtPar.DefaultView.ToTable(true, "Year");
                    bool YearOrNot = dtpar1.Rows.Count == 1 ? false : true;

                    if (dtMail.Rows[0]["OnBehalfPersonID"].ToString() == "")
                    {
                        if (dtMail.Rows[0]["Budget"].ToString() == "1")//预算内
                        {
                            DataTable dtPerson = new DataTable();
                            dtPerson.Columns.Add("Year", typeof(System.String));
                            dtPerson.Columns.Add("EName", typeof(System.String));
                            dtPerson.Columns.Add("Currency", typeof(System.String));
                            dtPerson.Columns.Add("Current", typeof(System.Decimal));
                            dtPerson.Columns.Add("PU", typeof(System.Decimal));
                            dtPerson.Columns.Add("PB", typeof(System.Decimal));
                            dtPerson.Columns.Add("PPercent", typeof(System.String));//%(Current+Used) / Budget

                            DataTable dtDepartment = new DataTable();
                            dtDepartment.Columns.Add("Year", typeof(System.String));
                            dtDepartment.Columns.Add("EName", typeof(System.String));
                            dtDepartment.Columns.Add("Currency", typeof(System.String));
                            dtDepartment.Columns.Add("Current", typeof(System.Decimal));
                            dtDepartment.Columns.Add("DU", typeof(System.Decimal));
                            dtDepartment.Columns.Add("DB", typeof(System.Decimal));
                            dtDepartment.Columns.Add("DPercent", typeof(System.String));//%(Current+Used) / Budget

                            DataTable dtStation = new DataTable();
                            dtStation.Columns.Add("Year", typeof(System.String));
                            dtStation.Columns.Add("EName", typeof(System.String));
                            dtStation.Columns.Add("Currency", typeof(System.String));
                            dtStation.Columns.Add("Current", typeof(System.Decimal));
                            dtStation.Columns.Add("SU", typeof(System.Decimal));
                            dtStation.Columns.Add("SB", typeof(System.Decimal));
                            dtStation.Columns.Add("SPercent", typeof(System.String));//%(Current+Used) / Budget

                            for (int i = 0; i < dtPar.Rows.Count; i++)
                            {
                                if (Convert.ToDecimal(dtPar.Rows[i]["PB"].ToString()) != 0)//按照个人分配了预算
                                {
                                    DataRow dr = dtPerson.NewRow();
                                    dr["Year"] = dtPar.Rows[i]["Year"].ToString();
                                    dr["EName"] = dtPar.Rows[i]["EName"].ToString();
                                    dr["Currency"] = dtPar.Rows[i]["LocalCur"].ToString();
                                    dr["Current"] = dtPar.Rows[i]["Current"];
                                    dr["PU"] = dtPar.Rows[i]["PU"];
                                    dr["PB"] = dtPar.Rows[i]["PB"];
                                    dr["PPercent"] = dtPar.Rows[i]["PPercent"].ToString() + "%";
                                    //decimal per = System.Math.Round(100 * (Convert.ToDecimal(dtPar.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtPar.Rows[i]["PU"].ToString())) / Convert.ToDecimal(dtPar.Rows[i]["PB"].ToString()), 2);
                                    //dr["PPercent"] = per.ToString() + "%";
                                    dtPerson.Rows.Add(dr);
                                }
                                if (Convert.ToDecimal(dtPar.Rows[i]["PB"].ToString()) == 0 && Convert.ToDecimal(dtPar.Rows[i]["DB"].ToString()) != 0)//按照部门分配了预算
                                {
                                    DataRow dr = dtDepartment.NewRow();
                                    dr["Year"] = dtPar.Rows[i]["Year"].ToString();
                                    dr["EName"] = dtPar.Rows[i]["EName"].ToString();
                                    dr["Currency"] = dtPar.Rows[i]["LocalCur"].ToString();
                                    dr["Current"] = dtPar.Rows[i]["Current"];
                                    dr["DU"] = dtPar.Rows[i]["DU"];
                                    dr["DB"] = dtPar.Rows[i]["DB"];
                                    dr["DPercent"] = dtPar.Rows[i]["DPercent"].ToString() + "%";
                                    //decimal per = System.Math.Round(100 * (Convert.ToDecimal(dtPar.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtPar.Rows[i]["DU"].ToString())) / Convert.ToDecimal(dtPar.Rows[i]["DB"].ToString()), 2);
                                    //dr["DPercent"] = per.ToString() + "%";
                                    dtDepartment.Rows.Add(dr);
                                }
                                if (Convert.ToDecimal(dtPar.Rows[i]["PB"].ToString()) == 0 && Convert.ToDecimal(dtPar.Rows[i]["DB"].ToString()) == 0 && Convert.ToDecimal(dtPar.Rows[i]["SB"].ToString()) != 0)//按照站点分配了预算
                                {
                                    DataRow dr = dtStation.NewRow();
                                    dr["Year"] = dtPar.Rows[i]["Year"].ToString();
                                    dr["EName"] = dtPar.Rows[i]["EName"].ToString();
                                    dr["Currency"] = dtPar.Rows[i]["LocalCur"].ToString();
                                    dr["Current"] = dtPar.Rows[i]["Current"];
                                    dr["SU"] = dtPar.Rows[i]["SU"];
                                    dr["SB"] = dtPar.Rows[i]["SB"];
                                    dr["SPercent"] = dtPar.Rows[i]["SPercent"].ToString() + "%";
                                    //decimal per = System.Math.Round(100 * (Convert.ToDecimal(dtPar.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtPar.Rows[i]["SU"].ToString())) / Convert.ToDecimal(dtPar.Rows[i]["SB"].ToString()), 2);
                                    //dr["SPercent"] = per.ToString() + "%";
                                    dtStation.Rows.Add(dr);
                                }
                            }
                            if (dtPerson.Rows.Count > 0)//如果有个人分配,则显示个人表格
                            {
                                dtPerson.DefaultView.Sort = "Year ASC";
                                DataTable dtPersonNew = dtPerson.DefaultView.ToTable();
                                for (int i = 0; i < dtPersonNew.Rows.Count; i++)
                                {
                                    if (i == 0 || dtPersonNew.Rows[i]["Year"].ToString() != dtPersonNew.Rows[i - 1]["Year"].ToString())//每个单独Year的第一行
                                    {
                                        if (YearOrNot)
                                        {
                                            sb.Append("<div " + divstyleR + ">Budget Year:" + dtPersonNew.Rows[i]["Year"].ToString() + "</div><div><table><tr>");
                                        }
                                        else
                                        {
                                            sb.Append("<div><table><tr>");
                                        }
                                        sb.Append("<td " + tdstyle + ">Expense Item</td>");
                                        sb.Append("<td " + tdstyle + ">Currency</td>");
                                        sb.Append("<td " + tdstyle + ">current</td>");
                                        sb.Append("<td " + tdstyle + ">Personal<br />Used</td>");
                                        sb.Append("<td " + tdstyle + ">Personal<br />Budget</td>");
                                        sb.Append("<td " + tdstyle + ">%<br />(Current+Used) / Budget</td></tr>");

                                        sb.Append("<tr><td " + tdstyle + ">" + dtPersonNew.Rows[i]["EName"].ToString() + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + dtPersonNew.Rows[i]["Currency"].ToString() + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtPersonNew.Rows[i]["Current"].ToString())) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtPersonNew.Rows[i]["PU"].ToString())) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtPersonNew.Rows[i]["PB"].ToString())) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + dtPersonNew.Rows[i]["PPercent"].ToString() + "</td></tr>");
                                    }
                                    else
                                    {
                                        sb.Append("<tr><td " + tdstyle + ">" + dtPersonNew.Rows[i]["EName"].ToString() + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + dtPersonNew.Rows[i]["Currency"].ToString() + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtPersonNew.Rows[i]["Current"].ToString())) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtPersonNew.Rows[i]["PU"].ToString())) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtPersonNew.Rows[i]["PB"].ToString())) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + dtPersonNew.Rows[i]["PPercent"].ToString() + "</td></tr>");
                                    }
                                    if (i == dtPersonNew.Rows.Count - 1 || dtPersonNew.Rows[i]["Year"].ToString() != dtPersonNew.Rows[i + 1]["Year"].ToString())//每个单独Year的最后一行
                                    {
                                        sb.Append("<tr><td " + tdstyle + ">Sub Total</td>");
                                        sb.Append("<td " + tdstyle + ">" + dtPersonNew.Rows[i]["Currency"].ToString() + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtPersonNew.Compute("Sum(Current)", "Year = " + dtPersonNew.Rows[i]["Year"].ToString()))) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtPersonNew.Compute("Sum(PU)", "Year = " + dtPersonNew.Rows[i]["Year"].ToString()))) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtPersonNew.Compute("Sum(PB)", "Year = " + dtPersonNew.Rows[i]["Year"].ToString()))) + "</td>");
                                        sb.Append("<td " + tdstyle + ">&nbsp;</td></tr>");

                                        sb.Append("</table></div><br />");

                                    }
                                }
                                //sb.Append("<div><table><tr>");
                                //sb.Append("<td " + tdstyle + ">Expense Item</td>");
                                //sb.Append("<td " + tdstyle + ">Currency</td>");
                                //sb.Append("<td " + tdstyle + ">current</td>");
                                //if (YearOrNot)
                                //{
                                //    sb.Append("<td " + tdstyle + ">Budget Year</td>");
                                //}

                                //sb.Append("<td " + tdstyle + ">Personal<br />Used</td>");
                                //sb.Append("<td " + tdstyle + ">Personal<br />Budget</td>");
                                //sb.Append("<td " + tdstyle + ">%<br />(Current+Used) / Budget</td></tr>");
                                //DataRow dr = dtPerson.NewRow();
                                //dr["EName"] = "Sub Total";
                                //dr["Currency"] = dtPerson.Rows[0]["Currency"].ToString();
                                //dr["Current"] = dtPerson.Compute("Sum(Current)", "");
                                //dr["PU"] = dtPerson.Compute("Sum(PU)", "");
                                //dr["PB"] = dtPerson.Compute("Sum(PB)", "");
                                //dr["PPercent"] = "&nbsp;";
                                //dr["Year"] = "&nbsp;";
                                //dtPerson.Rows.Add(dr);
                                //for (int i = 0; i < dtPerson.Rows.Count; i++)
                                //{
                                //    sb.Append("<tr><td " + tdstyle + ">" + dtPerson.Rows[i]["EName"].ToString() + "</td>");
                                //    sb.Append("<td " + tdstyle + ">" + dtPerson.Rows[i]["Currency"].ToString() + "</td>");
                                //    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtPerson.Rows[i]["Current"].ToString())) + "</td>");
                                //    if (YearOrNot)
                                //    {
                                //        sb.Append("<td " + tdstyle + ">" + dtPerson.Rows[i]["Year"].ToString() + "</td>");
                                //    }
                                //    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtPerson.Rows[i]["PU"].ToString())) + "</td>");
                                //    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtPerson.Rows[i]["PB"].ToString())) + "</td>");
                                //    sb.Append("<td " + tdstyle + ">" + dtPerson.Rows[i]["PPercent"].ToString() + "</td></tr>");
                                //}
                                //sb.Append("</table></div><br />");
                            }
                            if (dtDepartment.Rows.Count > 0)//如果有部门分配,则显示部门表格
                            {
                                dtDepartment.DefaultView.Sort = "Year ASC";
                                DataTable dtDepartmentNew = dtDepartment.DefaultView.ToTable();
                                for (int i = 0; i < dtDepartmentNew.Rows.Count; i++)
                                {
                                    if (i == 0 || dtDepartmentNew.Rows[i]["Year"].ToString() != dtDepartmentNew.Rows[i - 1]["Year"].ToString())//每个单独Year的第一行
                                    {
                                        if (YearOrNot)
                                        {
                                            sb.Append("<div " + divstyleR + ">Budget Year:" + dtDepartmentNew.Rows[i]["Year"].ToString() + "</div><div><table><tr>");
                                        }
                                        else
                                        {
                                            sb.Append("<div><table><tr>");
                                        }
                                        sb.Append("<td " + tdstyle + ">Expense Item</td>");
                                        sb.Append("<td " + tdstyle + ">Currency</td>");
                                        sb.Append("<td " + tdstyle + ">current</td>");
                                        sb.Append("<td " + tdstyle + ">Department<br />Used</td>");
                                        sb.Append("<td " + tdstyle + ">Department<br />Budget</td>");
                                        sb.Append("<td " + tdstyle + ">%<br />(Current+Used) / Budget</td></tr>");

                                        sb.Append("<tr><td " + tdstyle + ">" + dtDepartmentNew.Rows[i]["EName"].ToString() + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + dtDepartmentNew.Rows[i]["Currency"].ToString() + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtDepartmentNew.Rows[i]["Current"].ToString())) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtDepartmentNew.Rows[i]["DU"].ToString())) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtDepartmentNew.Rows[i]["DB"].ToString())) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + dtDepartmentNew.Rows[i]["DPercent"].ToString() + "</td></tr>");
                                    }
                                    else
                                    {
                                        sb.Append("<tr><td " + tdstyle + ">" + dtDepartmentNew.Rows[i]["EName"].ToString() + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + dtDepartmentNew.Rows[i]["Currency"].ToString() + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtDepartmentNew.Rows[i]["Current"].ToString())) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtDepartmentNew.Rows[i]["DU"].ToString())) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtDepartmentNew.Rows[i]["DB"].ToString())) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + dtDepartmentNew.Rows[i]["DPercent"].ToString() + "</td></tr>");
                                    }
                                    if (i == dtDepartmentNew.Rows.Count - 1 || dtDepartmentNew.Rows[i]["Year"].ToString() != dtDepartmentNew.Rows[i + 1]["Year"].ToString())//每个单独Year的最后一行
                                    {
                                        sb.Append("<tr><td " + tdstyle + ">Sub Total</td>");
                                        sb.Append("<td " + tdstyle + ">" + dtDepartmentNew.Rows[i]["Currency"].ToString() + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtDepartmentNew.Compute("Sum(Current)", "Year = " + dtDepartmentNew.Rows[i]["Year"].ToString()))) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtDepartmentNew.Compute("Sum(DU)", "Year = " + dtDepartmentNew.Rows[i]["Year"].ToString()))) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtDepartmentNew.Compute("Sum(DB)", "Year = " + dtDepartmentNew.Rows[i]["Year"].ToString()))) + "</td>");
                                        sb.Append("<td " + tdstyle + ">&nbsp;</td></tr>");

                                        sb.Append("</table></div><br />");

                                    }
                                }
                                //sb.Append("<div><table><tr>");
                                //sb.Append("<td " + tdstyle + ">Expense Item</td>");
                                //sb.Append("<td " + tdstyle + ">Currency</td>");
                                //sb.Append("<td " + tdstyle + ">current</td>");
                                //if (YearOrNot)
                                //{
                                //    sb.Append("<td " + tdstyle + ">Budget Year</td>");
                                //}
                                //sb.Append("<td " + tdstyle + ">Department<br />Used</td>");
                                //sb.Append("<td " + tdstyle + ">Department<br />Budget</td>");
                                //sb.Append("<td " + tdstyle + ">%<br />(Current+Used) / Budget</td></tr>");
                                //DataRow dr = dtDepartment.NewRow();
                                //dr["EName"] = "Sub Total";
                                //dr["Currency"] = dtDepartment.Rows[0]["Currency"].ToString();
                                //dr["Current"] = dtDepartment.Compute("Sum(Current)", "");
                                //dr["DU"] = dtDepartment.Compute("Sum(DU)", "");
                                //dr["DB"] = dtDepartment.Compute("Sum(DB)", "");
                                //dr["DPercent"] = "&nbsp;";
                                //dr["Year"] = "&nbsp;";
                                //dtDepartment.Rows.Add(dr);
                                //for (int i = 0; i < dtDepartment.Rows.Count; i++)
                                //{
                                //    sb.Append("<tr><td " + tdstyle + ">" + dtDepartment.Rows[i]["EName"].ToString() + "</td>");
                                //    sb.Append("<td " + tdstyle + ">" + dtDepartment.Rows[i]["Currency"].ToString() + "</td>");
                                //    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtDepartment.Rows[i]["Current"].ToString())) + "</td>");
                                //    if (YearOrNot)
                                //    {
                                //        sb.Append("<td " + tdstyle + ">" + dtDepartment.Rows[i]["Year"].ToString() + "</td>");
                                //    }
                                //    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtDepartment.Rows[i]["DU"].ToString())) + "</td>");
                                //    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtDepartment.Rows[i]["DB"].ToString())) + "</td>");
                                //    sb.Append("<td " + tdstyle + ">" + dtDepartment.Rows[i]["DPercent"].ToString() + "</td></tr>");
                                //}
                                //sb.Append("</table></div><br />");
                            }
                            if (dtStation.Rows.Count > 0)//如果有部门分配,则显示个人表格
                            {
                                dtStation.DefaultView.Sort = "Year ASC";
                                DataTable dtStationNew = dtStation.DefaultView.ToTable();
                                for (int i = 0; i < dtStationNew.Rows.Count; i++)
                                {
                                    if (i == 0 || dtStationNew.Rows[i]["Year"].ToString() != dtStationNew.Rows[i - 1]["Year"].ToString())//每个单独Year的第一行
                                    {
                                        if (YearOrNot)
                                        {
                                            sb.Append("<div " + divstyleR + ">Budget Year:" + dtStationNew.Rows[i]["Year"].ToString() + "</div><div><table><tr>");
                                        }
                                        else
                                        {
                                            sb.Append("<div><table><tr>");
                                        }

                                        sb.Append("<td " + tdstyle + ">Expense Item</td>");
                                        sb.Append("<td " + tdstyle + ">Currency</td>");
                                        sb.Append("<td " + tdstyle + ">current</td>");
                                        sb.Append("<td " + tdstyle + ">Unit<br />Used</td>");
                                        sb.Append("<td " + tdstyle + ">Unit<br />Budget</td>");
                                        sb.Append("<td " + tdstyle + ">%<br />(Current+Used) / Budget</td></tr>");

                                        sb.Append("<tr><td " + tdstyle + ">" + dtStationNew.Rows[i]["EName"].ToString() + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + dtStationNew.Rows[i]["Currency"].ToString() + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Rows[i]["Current"].ToString())) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Rows[i]["SU"].ToString())) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Rows[i]["SB"].ToString())) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + dtStationNew.Rows[i]["SPercent"].ToString() + "</td></tr>");
                                    }
                                    else
                                    {
                                        sb.Append("<tr><td " + tdstyle + ">" + dtStationNew.Rows[i]["EName"].ToString() + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + dtStationNew.Rows[i]["Currency"].ToString() + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Rows[i]["Current"].ToString())) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Rows[i]["SU"].ToString())) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Rows[i]["SB"].ToString())) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + dtStationNew.Rows[i]["SPercent"].ToString() + "</td></tr>");
                                    }
                                    if (i == dtStationNew.Rows.Count - 1 || dtStationNew.Rows[i]["Year"].ToString() != dtStationNew.Rows[i + 1]["Year"].ToString())//每个单独Year的最后一行
                                    {
                                        sb.Append("<tr><td " + tdstyle + ">Sub Total</td>");
                                        sb.Append("<td " + tdstyle + ">" + dtStationNew.Rows[i]["Currency"].ToString() + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Compute("Sum(Current)", "Year = " + dtStationNew.Rows[i]["Year"].ToString()))) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Compute("Sum(SU)", "Year = " + dtStationNew.Rows[i]["Year"].ToString()))) + "</td>");
                                        sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Compute("Sum(SB)", "Year = " + dtStationNew.Rows[i]["Year"].ToString()))) + "</td>");
                                        sb.Append("<td " + tdstyle + ">&nbsp;</td></tr>");

                                        sb.Append("</table></div><br />");

                                    }
                                }
                                //sb.Append("<div><table><tr>");
                                //sb.Append("<td " + tdstyle + ">Expense Item</td>");
                                //sb.Append("<td " + tdstyle + ">Currency</td>");
                                //sb.Append("<td " + tdstyle + ">current</td>");
                                //if (YearOrNot)
                                //{
                                //    sb.Append("<td " + tdstyle + ">Budget Year</td>");
                                //}
                                //sb.Append("<td " + tdstyle + ">Unit<br />Used</td>");
                                //sb.Append("<td " + tdstyle + ">Unit<br />Budget</td>");
                                //sb.Append("<td " + tdstyle + ">%<br />(Current+Used) / Budget</td></tr>");
                                //DataRow dr = dtStation.NewRow();
                                //dr["EName"] = "Sub Total";
                                //dr["Currency"] = dtStation.Rows[0]["Currency"].ToString();
                                //dr["Current"] = dtStation.Compute("Sum(Current)", "");
                                //dr["SU"] = dtStation.Compute("Sum(SU)", "");
                                //dr["SB"] = dtStation.Compute("Sum(SB)", "");
                                //dr["SPercent"] = "&nbsp;";
                                //dr["Year"] = "&nbsp;";
                                //dtStation.Rows.Add(dr);
                                //for (int i = 0; i < dtStation.Rows.Count; i++)
                                //{
                                //    sb.Append("<tr><td " + tdstyle + ">" + dtStation.Rows[i]["EName"].ToString() + "</td>");
                                //    sb.Append("<td " + tdstyle + ">" + dtStation.Rows[i]["Currency"].ToString() + "</td>");
                                //    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStation.Rows[i]["Current"].ToString())) + "</td>");
                                //    if (YearOrNot)
                                //    {
                                //        sb.Append("<td " + tdstyle + ">" + dtStation.Rows[i]["Year"].ToString() + "</td>");
                                //    }
                                //    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStation.Rows[i]["SU"].ToString())) + "</td>");
                                //    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStation.Rows[i]["SB"].ToString())) + "</td>");
                                //    sb.Append("<td " + tdstyle + ">" + dtStation.Rows[i]["SPercent"].ToString() + "</td></tr>");
                                //}
                                //sb.Append("</table></div><br />");
                            }
                        }
                        else
                        {
                            dtPar.DefaultView.Sort = "Year ASC";
                            DataTable dtStationNew = dtPar.DefaultView.ToTable();
                            for (int i = 0; i < dtStationNew.Rows.Count; i++)
                            {
                                if (i == 0 || dtStationNew.Rows[i]["Year"].ToString() != dtStationNew.Rows[i - 1]["Year"].ToString())//每个单独Year的第一行
                                {
                                    if (YearOrNot)
                                    {
                                        sb.Append("<div " + divstyleR + ">Budget Year:" + dtStationNew.Rows[i]["Year"].ToString() + "</div><div><table><tr>");
                                    }
                                    else
                                    {
                                        sb.Append("<div><table><tr>");
                                    }
                                    sb.Append("<td " + tdstyle + ">Expense Item</td>");
                                    sb.Append("<td " + tdstyle + ">Currency</td>");
                                    sb.Append("<td " + tdstyle + ">current</td>");
                                    sb.Append("<td " + tdstyle + ">Personal<br />Used</td>");
                                    sb.Append("<td " + tdstyle + ">Personal<br />Budget</td>");
                                    sb.Append("<td " + tdstyle + ">%<br />(Current+Used) /<br />Budget</td>");
                                    sb.Append("<td " + tdstyle + ">Department<br />Used</td>");
                                    sb.Append("<td " + tdstyle + ">Department<br />Budget</td>");
                                    sb.Append("<td " + tdstyle + ">%<br />(Current+Used) /<br />Budget</td>");
                                    sb.Append("<td " + tdstyle + ">Unit<br />Used</td>");
                                    sb.Append("<td " + tdstyle + ">Unit<br />Budget</td>");
                                    sb.Append("<td " + tdstyle + ">%<br />(Current+Used) /<br />Budget</td></tr>");

                                    sb.Append("<tr><td " + tdstyle + ">" + dtStationNew.Rows[i]["EName"].ToString() + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + dtStationNew.Rows[i]["LocalCur"].ToString() + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Rows[i]["Current"].ToString())) + "</td>");

                                    string PB = ""; string PP = ""; string DB = ""; string DP = ""; string SB = ""; string SP = "";
                                    if (Convert.ToDecimal(dtStationNew.Rows[i]["PB"].ToString()) != 0)
                                    {
                                        PB = string.Format("{0:N2}", dtStationNew.Rows[i]["PB"]);
                                        PP = dtPar.Rows[i]["PPercent"].ToString() + " %";
                                    }
                                    else
                                    {
                                        PB = "--";
                                        PP = "--";
                                    }
                                    if (Convert.ToDecimal(dtStationNew.Rows[i]["DB"].ToString()) != 0)
                                    {
                                        DB = string.Format("{0:N2}", dtStationNew.Rows[i]["DB"]);
                                        DP = dtPar.Rows[i]["DPercent"].ToString() + " %";
                                    }
                                    else
                                    {
                                        DB = "--";
                                        DP = "--";
                                    }
                                    if (Convert.ToDecimal(dtStationNew.Rows[i]["SB"].ToString()) != 0)
                                    {
                                        SB = string.Format("{0:N2}", dtStationNew.Rows[i]["SB"]);
                                        SP = dtPar.Rows[i]["SPercent"].ToString() + " %";
                                    }
                                    else
                                    {
                                        SB = "--";
                                        SP = "--";
                                    }

                                    if (Convert.ToDecimal(dtStationNew.Rows[i]["PB"].ToString()) == 0 && Convert.ToDecimal(dtStationNew.Rows[i]["DB"].ToString()) == 0 && Convert.ToDecimal(dtStationNew.Rows[i]["SB"].ToString()) == 0)
                                    {
                                        PP = "Unbudget Item";
                                        DP = "Unbudget Item";
                                        SP = "Unbudget Item";
                                    }
                                    decimal PPercent, DPercent, SPercent;
                                    //Person
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Rows[i]["PU"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + PB + "</td>");
                                    if (PP == "Unbudget Item")
                                    {
                                        sb.Append("<td " + divstylered + ">" + PP + "</td>");
                                    }
                                    else if (PP == "--" || PP == "&nbsp;")
                                    {
                                        sb.Append("<td " + tdstyle + ">" + PP + "</td>");
                                    }
                                    else if (decimal.TryParse(PP.Substring(0, PP.Length - 1), out PPercent))
                                    {
                                        if (PPercent > 100)
                                        {
                                            sb.Append("<td " + divstylered + ">" + PP + "</td>");
                                        }
                                        else
                                        {
                                            sb.Append("<td " + tdstyle + ">" + PP + "</td>");
                                        }
                                    }
                                    //Department
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Rows[i]["DU"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + DB + "</td>");
                                    if (DP == "Unbudget Item")
                                    {
                                        sb.Append("<td " + divstylered + ">" + DP + "</td>");
                                    }
                                    else if (DP == "--" || DP == "&nbsp;")
                                    {
                                        sb.Append("<td " + tdstyle + ">" + DP + "</td>");
                                    }
                                    else if (decimal.TryParse(DP.Substring(0, DP.Length - 1), out DPercent))
                                    {
                                        if (DPercent > 100)
                                        {
                                            sb.Append("<td " + divstylered + ">" + DP + "</td>");
                                        }
                                        else
                                        {
                                            sb.Append("<td " + tdstyle + ">" + DP + "</td>");
                                        }
                                    }
                                    //Station
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Rows[i]["SU"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + SB + "</td>");
                                    if (SP == "Unbudget Item")
                                    {
                                        sb.Append("<td " + divstylered + ">" + SP + "</td>");
                                    }
                                    else if (SP == "--" || SP == "&nbsp;")
                                    {
                                        sb.Append("<td " + tdstyle + ">" + SP + "</td>");
                                    }
                                    else if (decimal.TryParse(SP.Substring(0, SP.Length - 1), out SPercent))
                                    {
                                        if (SPercent > 100)
                                        {
                                            sb.Append("<td " + divstylered + ">" + SP + "</td>");
                                        }
                                        else
                                        {
                                            sb.Append("<td " + tdstyle + ">" + SP + "</td>");
                                        }
                                    }
                                }
                                else
                                {
                                    sb.Append("<tr><td " + tdstyle + ">" + dtStationNew.Rows[i]["EName"].ToString() + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + dtStationNew.Rows[i]["LocalCur"].ToString() + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Rows[i]["Current"].ToString())) + "</td>");

                                    string PB = ""; string PP = ""; string DB = ""; string DP = ""; string SB = ""; string SP = "";
                                    if (Convert.ToDecimal(dtStationNew.Rows[i]["PB"].ToString()) != 0)
                                    {
                                        PB = string.Format("{0:N2}", dtStationNew.Rows[i]["PB"]);
                                        PP = string.Format("{0:P2}", (Convert.ToDecimal(dtStationNew.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtStationNew.Rows[i]["PU"].ToString())) / Convert.ToDecimal(dtStationNew.Rows[i]["PB"].ToString()));
                                    }
                                    else
                                    {
                                        PB = "--";
                                        PP = "--";
                                    }
                                    if (Convert.ToDecimal(dtStationNew.Rows[i]["DB"].ToString()) != 0)
                                    {
                                        DB = string.Format("{0:N2}", dtStationNew.Rows[i]["DB"]);
                                        DP = string.Format("{0:P2}", (Convert.ToDecimal(dtStationNew.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtStationNew.Rows[i]["DU"].ToString())) / Convert.ToDecimal(dtStationNew.Rows[i]["DB"].ToString()));
                                    }
                                    else
                                    {
                                        DB = "--";
                                        DP = "--";
                                    }
                                    if (Convert.ToDecimal(dtStationNew.Rows[i]["SB"].ToString()) != 0)
                                    {
                                        SB = string.Format("{0:N2}", dtStationNew.Rows[i]["SB"]);
                                        SP = string.Format("{0:P2}", (Convert.ToDecimal(dtStationNew.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtStationNew.Rows[i]["SU"].ToString())) / Convert.ToDecimal(dtStationNew.Rows[i]["SB"].ToString()));
                                    }
                                    else
                                    {
                                        SB = "--";
                                        SP = "--";
                                    }

                                    if (Convert.ToDecimal(dtStationNew.Rows[i]["PB"].ToString()) == 0 && Convert.ToDecimal(dtStationNew.Rows[i]["DB"].ToString()) == 0 && Convert.ToDecimal(dtStationNew.Rows[i]["SB"].ToString()) == 0)
                                    {
                                        PP = "Unbudget Item";
                                        DP = "Unbudget Item";
                                        SP = "Unbudget Item";
                                    }
                                    decimal PPercent, DPercent, SPercent;
                                    //Person
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Rows[i]["PU"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + PB + "</td>");
                                    if (PP == "Unbudget Item")
                                    {
                                        sb.Append("<td " + divstylered + ">" + PP + "</td>");
                                    }
                                    else if (PP == "--" || PP == "&nbsp;")
                                    {
                                        sb.Append("<td " + tdstyle + ">" + PP + "</td>");
                                    }
                                    else if (decimal.TryParse(PP.Substring(0, PP.Length - 1), out PPercent))
                                    {
                                        if (PPercent > 100)
                                        {
                                            sb.Append("<td " + divstylered + ">" + PP + "</td>");
                                        }
                                        else
                                        {
                                            sb.Append("<td " + tdstyle + ">" + PP + "</td>");
                                        }
                                    }
                                    //Department
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Rows[i]["DU"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + DB + "</td>");
                                    if (DP == "Unbudget Item")
                                    {
                                        sb.Append("<td " + divstylered + ">" + DP + "</td>");
                                    }
                                    else if (DP == "--" || DP == "&nbsp;")
                                    {
                                        sb.Append("<td " + tdstyle + ">" + DP + "</td>");
                                    }
                                    else if (decimal.TryParse(DP.Substring(0, DP.Length - 1), out DPercent))
                                    {
                                        if (DPercent > 100)
                                        {
                                            sb.Append("<td " + divstylered + ">" + DP + "</td>");
                                        }
                                        else
                                        {
                                            sb.Append("<td " + tdstyle + ">" + DP + "</td>");
                                        }
                                    }
                                    //Station
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Rows[i]["SU"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + SB + "</td>");
                                    if (SP == "Unbudget Item")
                                    {
                                        sb.Append("<td " + divstylered + ">" + SP + "</td>");
                                    }
                                    else if (SP == "--" || SP == "&nbsp;")
                                    {
                                        sb.Append("<td " + tdstyle + ">" + SP + "</td>");
                                    }
                                    else if (decimal.TryParse(SP.Substring(0, SP.Length - 1), out SPercent))
                                    {
                                        if (SPercent > 100)
                                        {
                                            sb.Append("<td " + divstylered + ">" + SP + "</td>");
                                        }
                                        else
                                        {
                                            sb.Append("<td " + tdstyle + ">" + SP + "</td>");
                                        }
                                    }
                                }
                                if (i == dtStationNew.Rows.Count - 1 || dtStationNew.Rows[i]["Year"].ToString() != dtStationNew.Rows[i + 1]["Year"].ToString())//每个单独Year的最后一行
                                {
                                    sb.Append("<tr><td " + tdstyle + ">Sub Total</td>");
                                    sb.Append("<td " + tdstyle + ">" + dtStationNew.Rows[i]["LocalCur"].ToString() + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Compute("Sum(Current)", "Year = " + dtStationNew.Rows[i]["Year"].ToString()))) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Compute("Sum(PU)", "Year = " + dtStationNew.Rows[i]["Year"].ToString()))) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Compute("Sum(PB)", "Year = " + dtStationNew.Rows[i]["Year"].ToString()))) + "</td>");
                                    sb.Append("<td " + tdstyle + ">&nbsp;</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Compute("Sum(DU)", "Year = " + dtStationNew.Rows[i]["Year"].ToString()))) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Compute("Sum(DB)", "Year = " + dtStationNew.Rows[i]["Year"].ToString()))) + "</td>");
                                    sb.Append("<td " + tdstyle + ">&nbsp;</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Compute("Sum(SU)", "Year = " + dtStationNew.Rows[i]["Year"].ToString()))) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStationNew.Compute("Sum(SB)", "Year = " + dtStationNew.Rows[i]["Year"].ToString()))) + "</td>");
                                    sb.Append("<td " + tdstyle + ">&nbsp;</td></tr>");
                                    sb.Append("</table></div><br />");

                                }
                            }

                        }
                    }
                }
                else//差旅费
                {
                    string sqldetail2 = "select min(convert(varchar(10),Tdate0,111)) as Tdate0,max(convert(varchar(10),Tdate,111)) as Tdate1 from ETraveleDetail where [No]=" + RequestID + "";
                    DataTable dtder = dbc.GetData("eReimbursement", sqldetail2);
                    if (dtder.Rows.Count == 1)
                    {
                        sb.Append("<div " + divstyle + ">Period:From " + Convert.ToDateTime(dtder.Rows[0]["Tdate0"].ToString()).ToString("yyyy/MM/dd") + " To " + Convert.ToDateTime(dtder.Rows[0]["Tdate1"].ToString()).ToString("yyyy/MM/dd") + "</div><br />");
                    }
                    string sqlcity = "select Tocity+'/' from (select distinct Tocity from ETraveleDetail where No=" + RequestID + ") as t for xml path('')";
                    DataTable dtcity = dbc.GetData("eReimbursement", sqlcity);
                    string city = dtcity.Rows[0][0].ToString();
                    sb.Append("<div " + divstyle + ">Destination:" + city.Substring(0, city.Length - 1) + "</div>");
                    sb.Append("<br />");
                    if (dtMail.Rows[0]["OnBehalfPersonID"].ToString() == "")
                    {
                        if (dtMail.Rows[0]["Budget"].ToString() == "1")//预算内
                        {
                            DataTable dtPerson = new DataTable();
                            dtPerson.Columns.Add("EName", typeof(System.String));
                            dtPerson.Columns.Add("Currency", typeof(System.String));
                            dtPerson.Columns.Add("Current", typeof(System.Decimal));
                            dtPerson.Columns.Add("PA", typeof(System.Decimal));
                            dtPerson.Columns.Add("CA", typeof(System.Decimal));
                            dtPerson.Columns.Add("PU", typeof(System.Decimal));
                            dtPerson.Columns.Add("PB", typeof(System.Decimal));
                            dtPerson.Columns.Add("PPercent", typeof(System.String));//%(Current+Used) / Budget

                            DataTable dtDepartment = new DataTable();
                            dtDepartment.Columns.Add("EName", typeof(System.String));
                            dtDepartment.Columns.Add("Currency", typeof(System.String));
                            dtDepartment.Columns.Add("Current", typeof(System.Decimal));
                            dtDepartment.Columns.Add("PA", typeof(System.Decimal));
                            dtDepartment.Columns.Add("CA", typeof(System.Decimal));
                            dtDepartment.Columns.Add("DU", typeof(System.Decimal));
                            dtDepartment.Columns.Add("DB", typeof(System.Decimal));
                            dtDepartment.Columns.Add("DPercent", typeof(System.String));//%(Current+Used) / Budget

                            DataTable dtStation = new DataTable();
                            dtStation.Columns.Add("EName", typeof(System.String));
                            dtStation.Columns.Add("Currency", typeof(System.String));
                            dtStation.Columns.Add("Current", typeof(System.Decimal));
                            dtStation.Columns.Add("PA", typeof(System.Decimal));
                            dtStation.Columns.Add("CA", typeof(System.Decimal));
                            dtStation.Columns.Add("SU", typeof(System.Decimal));
                            dtStation.Columns.Add("SB", typeof(System.Decimal));
                            dtStation.Columns.Add("SPercent", typeof(System.String));//%(Current+Used) / Budget

                            for (int i = 0; i < dtPar.Rows.Count; i++)
                            {
                                if (Convert.ToDecimal(dtPar.Rows[i]["PB"].ToString()) != 0)//按照个人分配了预算
                                {
                                    DataRow dr = dtPerson.NewRow();
                                    dr["EName"] = dtPar.Rows[i]["EName"].ToString();
                                    dr["Currency"] = dtPar.Rows[i]["LocalCur"].ToString();
                                    dr["Current"] = dtPar.Rows[i]["Current"];
                                    dr["PA"] = dtPar.Rows[i]["PA"];
                                    dr["CA"] = dtPar.Rows[i]["CA"];
                                    dr["PU"] = dtPar.Rows[i]["PU"];
                                    dr["PB"] = dtPar.Rows[i]["PB"];
                                    dr["PPercent"] = dtPar.Rows[i]["PPercent"].ToString() + "%";
                                    //decimal per = System.Math.Round(100 * (Convert.ToDecimal(dtPar.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtPar.Rows[i]["PU"].ToString())) / Convert.ToDecimal(dtPar.Rows[i]["PB"].ToString()), 2);
                                    //dr["PPercent"] = per.ToString() + "%";
                                    dtPerson.Rows.Add(dr);
                                }
                                if (Convert.ToDecimal(dtPar.Rows[i]["PB"].ToString()) == 0 && Convert.ToDecimal(dtPar.Rows[i]["DB"].ToString()) != 0)//按照部门分配了预算
                                {
                                    DataRow dr = dtDepartment.NewRow();
                                    dr["EName"] = dtPar.Rows[i]["EName"].ToString();
                                    dr["Currency"] = dtPar.Rows[i]["LocalCur"].ToString();
                                    dr["Current"] = dtPar.Rows[i]["Current"];
                                    dr["PA"] = dtPar.Rows[i]["PA"];
                                    dr["CA"] = dtPar.Rows[i]["CA"];
                                    dr["DU"] = dtPar.Rows[i]["DU"];
                                    dr["DB"] = dtPar.Rows[i]["DB"];
                                    dr["DPercent"] = dtPar.Rows[i]["DPercent"].ToString() + "%";
                                    //decimal per = System.Math.Round(100 * (Convert.ToDecimal(dtPar.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtPar.Rows[i]["DU"].ToString())) / Convert.ToDecimal(dtPar.Rows[i]["DB"].ToString()), 2);
                                    //dr["DPercent"] = per.ToString() + "%";
                                    dtDepartment.Rows.Add(dr);
                                }
                                if (Convert.ToDecimal(dtPar.Rows[i]["PB"].ToString()) == 0 && Convert.ToDecimal(dtPar.Rows[i]["DB"].ToString()) == 0 && Convert.ToDecimal(dtPar.Rows[i]["SB"].ToString()) != 0)//按照站点分配了预算
                                {
                                    DataRow dr = dtStation.NewRow();
                                    dr["EName"] = dtPar.Rows[i]["EName"].ToString();
                                    dr["Currency"] = dtPar.Rows[i]["LocalCur"].ToString();
                                    dr["Current"] = dtPar.Rows[i]["Current"];
                                    dr["PA"] = dtPar.Rows[i]["PA"];
                                    dr["CA"] = dtPar.Rows[i]["CA"];
                                    dr["SU"] = dtPar.Rows[i]["SU"];
                                    dr["SB"] = dtPar.Rows[i]["SB"];
                                    dr["SPercent"] = dtPar.Rows[i]["SPercent"].ToString() + "%";
                                    //decimal per = System.Math.Round(100 * (Convert.ToDecimal(dtPar.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtPar.Rows[i]["SU"].ToString())) / Convert.ToDecimal(dtPar.Rows[i]["SB"].ToString()), 2);
                                    //dr["SPercent"] = per.ToString() + "%";
                                    dtStation.Rows.Add(dr);
                                }
                            }
                            if (dtPerson.Rows.Count > 0)//如果有个人分配,则显示个人表格
                            {
                                sb.Append("<div><table><tr>");
                                sb.Append("<td " + tdstyle + ">Expense Item</td>");
                                sb.Append("<td " + tdstyle + ">Currency</td>");
                                //sb.Append("<td " + tdstyle + ">current</td>");
                                sb.Append("<td " + tdstyle + ">Personal paid</td>");
                                sb.Append("<td " + tdstyle + ">Company</td>");
                                sb.Append("<td " + tdstyle + ">Personal<br />Used</td>");
                                sb.Append("<td " + tdstyle + ">Personal<br />Budget</td>");
                                sb.Append("<td " + tdstyle + ">%<br />(Current+Used) / Budget</td></tr>");
                                DataRow dr = dtPerson.NewRow();
                                dr["EName"] = "Sub Total";
                                dr["Currency"] = dtPerson.Rows[0]["Currency"].ToString();
                                dr["Current"] = dtPerson.Compute("Sum(Current)", "");
                                dr["PA"] = dtPerson.Compute("Sum(PA)", "");
                                dr["CA"] = dtPerson.Compute("Sum(CA)", "");
                                dr["PU"] = dtPerson.Compute("Sum(PU)", "");
                                dr["PB"] = dtPerson.Compute("Sum(PB)", "");
                                dr["PPercent"] = "&nbsp;";
                                dtPerson.Rows.Add(dr);
                                for (int i = 0; i < dtPerson.Rows.Count; i++)
                                {
                                    sb.Append("<tr><td " + tdstyle + ">" + dtPerson.Rows[i]["EName"].ToString() + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + dtPerson.Rows[i]["Currency"].ToString() + "</td>");
                                    //sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtPerson.Rows[i]["Current"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtPerson.Rows[i]["PA"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtPerson.Rows[i]["CA"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtPerson.Rows[i]["PU"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtPerson.Rows[i]["PB"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + dtPerson.Rows[i]["PPercent"].ToString() + "</td></tr>");
                                }
                                sb.Append("</table></div><br />");
                            }
                            if (dtDepartment.Rows.Count > 0)//如果有部门分配,则显示部门表格
                            {
                                sb.Append("<div><table><tr>");
                                sb.Append("<td " + tdstyle + ">Expense Item</td>");
                                sb.Append("<td " + tdstyle + ">Currency</td>");
                                //sb.Append("<td " + tdstyle + ">current</td>");
                                sb.Append("<td " + tdstyle + ">Personal paid</td>");
                                sb.Append("<td " + tdstyle + ">Company</td>");
                                sb.Append("<td " + tdstyle + ">Department<br />Used</td>");
                                sb.Append("<td " + tdstyle + ">Department<br />Budget</td>");
                                sb.Append("<td " + tdstyle + ">%<br />(Current+Used) / Budget</td></tr>");
                                DataRow dr = dtDepartment.NewRow();
                                dr["EName"] = "Sub Total";
                                dr["Currency"] = dtDepartment.Rows[0]["Currency"].ToString();
                                dr["Current"] = dtDepartment.Compute("Sum(Current)", "");
                                dr["PA"] = dtDepartment.Compute("Sum(PA)", "");
                                dr["CA"] = dtDepartment.Compute("Sum(CA)", "");
                                dr["DU"] = dtDepartment.Compute("Sum(DU)", "");
                                dr["DB"] = dtDepartment.Compute("Sum(DB)", "");
                                dr["DPercent"] = "&nbsp;";
                                dtDepartment.Rows.Add(dr);
                                for (int i = 0; i < dtDepartment.Rows.Count; i++)
                                {
                                    sb.Append("<tr><td " + tdstyle + ">" + dtDepartment.Rows[i]["EName"].ToString() + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + dtDepartment.Rows[i]["Currency"].ToString() + "</td>");
                                    //sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtDepartment.Rows[i]["Current"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtDepartment.Rows[i]["PA"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtDepartment.Rows[i]["CA"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtDepartment.Rows[i]["DU"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtDepartment.Rows[i]["DB"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + dtDepartment.Rows[i]["DPercent"].ToString() + "</td></tr>");
                                }
                                sb.Append("</table></div><br />");
                            }
                            if (dtStation.Rows.Count > 0)//如果有部门分配,则显示个人表格
                            {
                                sb.Append("<div><table><tr>");
                                sb.Append("<td " + tdstyle + ">Expense Item</td>");
                                sb.Append("<td " + tdstyle + ">Currency</td>");
                                //sb.Append("<td " + tdstyle + ">current</td>");
                                sb.Append("<td " + tdstyle + ">Personal paid</td>");
                                sb.Append("<td " + tdstyle + ">Company</td>");
                                sb.Append("<td " + tdstyle + ">Unit<br />Used</td>");
                                sb.Append("<td " + tdstyle + ">Unit<br />Budget</td>");
                                sb.Append("<td " + tdstyle + ">%<br />(Current+Used) / Budget</td></tr>");
                                DataRow dr = dtStation.NewRow();
                                dr["EName"] = "Sub Total";
                                dr["Currency"] = dtStation.Rows[0]["Currency"].ToString();
                                dr["Current"] = dtStation.Compute("Sum(Current)", "");
                                dr["PA"] = dtStation.Compute("Sum(PA)", "");
                                dr["CA"] = dtStation.Compute("Sum(CA)", "");
                                dr["SU"] = dtStation.Compute("Sum(SU)", "");
                                dr["SB"] = dtStation.Compute("Sum(SB)", "");
                                dr["SPercent"] = "&nbsp;";
                                dtStation.Rows.Add(dr);
                                for (int i = 0; i < dtStation.Rows.Count; i++)
                                {
                                    sb.Append("<tr><td " + tdstyle + ">" + dtStation.Rows[i]["EName"].ToString() + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + dtStation.Rows[i]["Currency"].ToString() + "</td>");
                                    //sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStation.Rows[i]["Current"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStation.Rows[i]["PA"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStation.Rows[i]["CA"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStation.Rows[i]["SU"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + string.Format("{0:N2}", Convert.ToDecimal(dtStation.Rows[i]["SB"].ToString())) + "</td>");
                                    sb.Append("<td " + tdstyle + ">" + dtStation.Rows[i]["SPercent"].ToString() + "</td></tr>");
                                }
                                sb.Append("</table></div><br />");
                            }
                        }
                        else
                        {
                            //为dtPar增加一行合计
                            DataRow drBudget = dtPar.NewRow();
                            drBudget["EName"] = "Sub Total";
                            drBudget["LocalCur"] = dtPar.Rows[0]["LocalCur"].ToString();
                            drBudget["Current"] = dtPar.Compute("Sum(Current)", "");
                            drBudget["PA"] = dtPar.Compute("Sum(PA)", "");
                            drBudget["CA"] = dtPar.Compute("Sum(CA)", "");
                            drBudget["PU"] = dtPar.Compute("Sum(PU)", "");
                            drBudget["PB"] = dtPar.Compute("Sum(PB)", "");
                            drBudget["DU"] = dtPar.Compute("Sum(DU)", "");
                            drBudget["DB"] = dtPar.Compute("Sum(DB)", "");
                            drBudget["SU"] = dtPar.Compute("Sum(SU)", "");
                            drBudget["SB"] = dtPar.Compute("Sum(SB)", "");
                            dtPar.Rows.Add(drBudget);

                            DataTable dtbudgetNew = new DataTable();//记录预算最终数据,用于传递给EMail显示
                            dtbudgetNew.Columns.Add("EName", typeof(System.String));
                            //dtbudgetNew.Columns.Add("COACode", typeof(System.String));
                            dtbudgetNew.Columns.Add("Current", typeof(System.String));
                            dtbudgetNew.Columns.Add("PA", typeof(System.String));
                            dtbudgetNew.Columns.Add("CA", typeof(System.String));
                            dtbudgetNew.Columns.Add("PU", typeof(System.String));
                            dtbudgetNew.Columns.Add("PB", typeof(System.String));
                            dtbudgetNew.Columns.Add("PPercent", typeof(System.String));
                            dtbudgetNew.Columns.Add("DU", typeof(System.String));
                            dtbudgetNew.Columns.Add("DB", typeof(System.String));
                            dtbudgetNew.Columns.Add("DPercent", typeof(System.String));
                            dtbudgetNew.Columns.Add("SU", typeof(System.String));
                            dtbudgetNew.Columns.Add("SB", typeof(System.String));
                            dtbudgetNew.Columns.Add("SPercent", typeof(System.String));
                            dtbudgetNew.Columns.Add("Currency", typeof(System.String));
                            for (int i = 0; i < dtPar.Rows.Count; i++)
                            {
                                DataRow dr = dtbudgetNew.NewRow();
                                dr["EName"] = dtPar.Rows[i]["EName"].ToString();
                                dr["Currency"] = dtPar.Rows[i]["LocalCur"].ToString();
                                dr["Current"] = string.Format("{0:N2}", dtPar.Rows[i]["Current"]);
                                dr["PA"] = string.Format("{0:N2}", dtPar.Rows[i]["PA"]);
                                dr["CA"] = string.Format("{0:N2}", dtPar.Rows[i]["CA"]);
                                dr["PU"] = string.Format("{0:N2}", dtPar.Rows[i]["PU"]);
                                if (Convert.ToDecimal(dtPar.Rows[i]["PB"].ToString()) != 0)
                                {
                                    dr["PB"] = string.Format("{0:N2}", dtPar.Rows[i]["PB"]);
                                    if (i != dtPar.Rows.Count - 1)
                                    {
                                        //dr["PPercent"] = string.Format("{0:P2}", (Convert.ToDecimal(dtPar.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtPar.Rows[i]["PU"].ToString())) / Convert.ToDecimal(dtPar.Rows[i]["PB"].ToString()));
                                        dr["PPercent"] = dtPar.Rows[i]["PPercent"].ToString() + "%";
                                    }
                                }
                                else
                                {
                                    dr["PB"] = "--";
                                    if (i != dtPar.Rows.Count - 1)
                                    {
                                        dr["PPercent"] = "--";
                                    }
                                }
                                dr["DU"] = string.Format("{0:N2}", dtPar.Rows[i]["DU"]);
                                if (Convert.ToDecimal(dtPar.Rows[i]["DB"].ToString()) != 0)
                                {
                                    dr["DB"] = string.Format("{0:N2}", dtPar.Rows[i]["DB"]);
                                    if (i != dtPar.Rows.Count - 1)
                                    {
                                        //dr["DPercent"] = string.Format("{0:P2}", (Convert.ToDecimal(dtPar.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtPar.Rows[i]["DU"].ToString())) / Convert.ToDecimal(dtPar.Rows[i]["DB"].ToString()));
                                        dr["DPercent"] = dtPar.Rows[i]["DPercent"].ToString() + "%";
                                    }
                                }
                                else
                                {
                                    dr["DB"] = "--";
                                    if (i != dtPar.Rows.Count - 1)
                                    {
                                        dr["DPercent"] = "--";
                                    }
                                }
                                dr["SU"] = string.Format("{0:N2}", dtPar.Rows[i]["SU"]);
                                if (Convert.ToDecimal(dtPar.Rows[i]["SB"].ToString()) != 0)
                                {
                                    dr["SB"] = string.Format("{0:N2}", dtPar.Rows[i]["SB"]);
                                    if (i != dtPar.Rows.Count - 1)
                                    {
                                        //dr["SPercent"] = string.Format("{0:P2}", (Convert.ToDecimal(dtPar.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtPar.Rows[i]["SU"].ToString())) / Convert.ToDecimal(dtPar.Rows[i]["SB"].ToString()));
                                        dr["SPercent"] = dtPar.Rows[i]["SPercent"].ToString() + "%";
                                    }
                                }
                                else
                                {
                                    dr["SB"] = "--";
                                    if (i != dtPar.Rows.Count - 1)
                                    {
                                        dr["SPercent"] = "--";
                                    }
                                }
                                if (Convert.ToDecimal(dtPar.Rows[i]["PB"].ToString()) == 0 && Convert.ToDecimal(dtPar.Rows[i]["DB"].ToString()) == 0 && Convert.ToDecimal(dtPar.Rows[i]["SB"].ToString()) == 0)
                                {
                                    if (i != dtPar.Rows.Count - 1)
                                    {
                                        dr["PPercent"] = "Unbudget Item";
                                        dr["DPercent"] = "Unbudget Item";
                                        dr["SPercent"] = "Unbudget Item";
                                    }
                                }
                                if (i == dtPar.Rows.Count - 1)
                                {
                                    dr["PPercent"] = "&nbsp;";
                                    dr["DPercent"] = "&nbsp;";
                                    dr["SPercent"] = "&nbsp;";
                                }
                                dtbudgetNew.Rows.Add(dr);
                            }
                            sb.Append("<div><table><tr>");
                            sb.Append("<td " + tdstyle + ">Expense Item</td>");
                            sb.Append("<td " + tdstyle + ">Currency</td>");
                            //sb.Append("<td " + tdstyle + ">current</td>");
                            sb.Append("<td " + tdstyle + ">Personal paid</td>");
                            sb.Append("<td " + tdstyle + ">Company</td>");
                            sb.Append("<td " + tdstyle + ">Personal<br />Used</td>");
                            sb.Append("<td " + tdstyle + ">Personal<br />Budget</td>");
                            sb.Append("<td " + tdstyle + ">%<br />(Current+Used) /<br />Budget</td>");
                            sb.Append("<td " + tdstyle + ">Department<br />Used</td>");
                            sb.Append("<td " + tdstyle + ">Department<br />Budget</td>");
                            sb.Append("<td " + tdstyle + ">%<br />(Current+Used) /<br />Budget</td>");
                            sb.Append("<td " + tdstyle + ">Unit<br />Used</td>");
                            sb.Append("<td " + tdstyle + ">Unit<br />Budget</td>");
                            sb.Append("<td " + tdstyle + ">%<br />(Current+Used) /<br />Budget</td></tr>");
                            for (int i = 0; i < dtbudgetNew.Rows.Count; i++)
                            {
                                decimal PPercent, DPercent, SPercent;

                                sb.Append("<tr><td " + tdstyle + ">" + dtbudgetNew.Rows[i]["EName"].ToString() + "</td>");
                                sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["Currency"].ToString() + "</td>");
                                //sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["Current"].ToString() + "</td>");
                                sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["PA"].ToString() + "</td>");
                                sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["CA"].ToString() + "</td>");
                                sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["PU"].ToString() + "</td>");
                                sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["PB"].ToString() + "</td>");
                                //sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["PPercent"].ToString() + "</td>");
                                if (dtbudgetNew.Rows[i]["PPercent"].ToString() == "Unbudget Item")
                                {
                                    sb.Append("<td " + divstylered + ">" + dtbudgetNew.Rows[i]["PPercent"].ToString() + "</td>");
                                }
                                else if (dtbudgetNew.Rows[i]["PPercent"].ToString() == "--" || dtbudgetNew.Rows[i]["PPercent"].ToString() == "&nbsp;")
                                {
                                    sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["PPercent"].ToString() + "</td>");
                                }
                                else if (decimal.TryParse(dtbudgetNew.Rows[i]["PPercent"].ToString().Substring(0, dtbudgetNew.Rows[i]["PPercent"].ToString().Length - 1), out PPercent))
                                {
                                    if (PPercent > 100)
                                    {
                                        sb.Append("<td " + divstylered + ">" + dtbudgetNew.Rows[i]["PPercent"].ToString() + "</td>");
                                    }
                                    else
                                    {
                                        sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["PPercent"].ToString() + "</td>");
                                    }
                                }

                                sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["DU"].ToString() + "</td>");
                                sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["DB"].ToString() + "</td>");
                                //sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["DPercent"].ToString() + "</td>");
                                if (dtbudgetNew.Rows[i]["DPercent"].ToString() == "Unbudget Item")
                                {
                                    sb.Append("<td " + divstylered + ">" + dtbudgetNew.Rows[i]["DPercent"].ToString() + "</td>");
                                }
                                else if (dtbudgetNew.Rows[i]["DPercent"].ToString() == "--" || dtbudgetNew.Rows[i]["DPercent"].ToString() == "&nbsp;")
                                {
                                    sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["DPercent"].ToString() + "</td>");
                                }
                                else if (decimal.TryParse(dtbudgetNew.Rows[i]["DPercent"].ToString().Substring(0, dtbudgetNew.Rows[i]["DPercent"].ToString().Length - 1), out DPercent))
                                {
                                    if (DPercent > 100)
                                    {
                                        sb.Append("<td " + divstylered + ">" + dtbudgetNew.Rows[i]["DPercent"].ToString() + "</td>");
                                    }
                                    else
                                    {
                                        sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["DPercent"].ToString() + "</td>");
                                    }
                                }
                                else
                                {
                                    sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["DPercent"].ToString() + "</td>");
                                }
                                sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["SU"].ToString() + "</td>");
                                sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["SB"].ToString() + "</td>");
                                //sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["SPercent"].ToString() + "</td></tr>");
                                if (dtbudgetNew.Rows[i]["SPercent"].ToString() == "Unbudget Item")
                                {
                                    sb.Append("<td " + divstylered + ">" + dtbudgetNew.Rows[i]["SPercent"].ToString() + "</td></tr>");
                                }
                                else if (dtbudgetNew.Rows[i]["SPercent"].ToString() == "--" || dtbudgetNew.Rows[i]["SPercent"].ToString() == "&nbsp;")
                                {
                                    sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["SPercent"].ToString() + "</td>");
                                }
                                else if (decimal.TryParse(dtbudgetNew.Rows[i]["SPercent"].ToString().Substring(0, dtbudgetNew.Rows[i]["SPercent"].ToString().Length - 1), out SPercent))
                                {
                                    if (SPercent > 100)
                                    {
                                        sb.Append("<td " + divstylered + ">" + dtbudgetNew.Rows[i]["SPercent"].ToString() + "</td></tr>");
                                    }
                                    else
                                    {
                                        sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["SPercent"].ToString() + "</td></tr>");
                                    }
                                }
                                else
                                {
                                    sb.Append("<td " + tdstyle + ">" + dtbudgetNew.Rows[i]["SPercent"].ToString() + "</td></tr>");
                                }
                            }
                            sb.Append("</table></div><br />");
                        }
                        
                    }
                }
                sb.Append("<div " + divstyle + ">Apply Remark:" + dtMail.Rows[0]["Remark"].ToString() + "</div><br />");
                StringBuilder sb1 = new StringBuilder();
                sb1.Append("<div><span " + divstyle + ">Approval Flow:</span>");
                for (int i = 0; i < dtMail.Rows.Count; i++)
                {
                    if (dtMail.Rows[i]["Status"].ToString() == "1")//待批
                    {
                        string msg1 = "";
                        if (dtMail.Rows[i]["FlowFn"].ToString().ToLower() == "verifier")
                        {
                            msg1 = ". To Be Verified: ";
                        }
                        else if (dtMail.Rows[i]["FlowFn"].ToString().ToLower() == "issuer")
                        {
                            msg1 = ". To Be Issued: ";
                        }
                        else if (dtMail.Rows[i]["FlowFn"].ToString().ToLower() == "confirm")
                        {
                            msg1 = ". Waiting For Confirmation.";
                        }
                        else
                        {
                            msg1 = ". Waiting for Approval: ";
                        }

                        if (dtMail.Rows[i]["Active"].ToString() == "1")
                        {
                            sb1.Append("<div " + divstyleCurrent + ">" + (i + 1).ToString() + msg1 + dtMail.Rows[i]["Approver"].ToString());
                        }
                        else
                        {
                            sb1.Append("<div " + divstyle + ">" + (i + 1).ToString() + msg1 + dtMail.Rows[i]["Approver"].ToString());
                        }
                    }
                    else if (dtMail.Rows[i]["Status"].ToString() == "2")//批准
                    {
                        string msg1 = "";
                        if (dtMail.Rows[i]["FlowFn"].ToString().ToLower() == "verifier")
                        {
                            msg1 = " Verified by: ";
                        }
                        else if (dtMail.Rows[i]["FlowFn"].ToString().ToLower() == "issuer")
                        {
                            msg1 = " Issued by: ";
                        }
                        else if (dtMail.Rows[i]["FlowFn"].ToString().ToLower() == "confirm")
                        {
                            msg1 = " Confirmed by: ";
                        }
                        else
                        {
                            msg1 = " Approved by: ";
                        }

                        if (dtMail.Rows[i]["Active"].ToString() == "-1")//未完成
                        {
                            sb1.Append("<div " + divstyle + ">" + (i + 1).ToString() + "." + msg1 + dtMail.Rows[i]["Approver"].ToString() + ", Date: " + Convert.ToDateTime(dtMail.Rows[i]["ApproveDate"].ToString()).ToString("yy/MM/dd"));
                        }
                        else if (dtMail.Rows[i]["Active"].ToString() == "2")//已完成
                        {
                            sb1.Append("<div " + divstyleCurrent + ">" + (i + 1).ToString() + ". Complete - " + msg1 + dtMail.Rows[i]["Approver"].ToString() + ", Date: " + Convert.ToDateTime(dtMail.Rows[i]["ApproveDate"].ToString()).ToString("yy/MM/dd"));
                        }
                    }
                    else if (dtMail.Rows[i]["Status"].ToString() == "3")
                    {
                        sb1.Append("<div " + divstyleCurrent + ">" + (i + 1).ToString() + ". Complete - Rejected by: " + dtMail.Rows[i]["Approver"].ToString() + ", Date: " + Convert.ToDateTime(dtMail.Rows[i]["ApproveDate"].ToString()).ToString("yy/MM/dd"));
                    }
                    if (dtMail.Rows[i]["RemarkFlow"].ToString() != "")
                    {
                        sb1.Append(", Remark: " + dtMail.Rows[i]["RemarkFlow"].ToString() + "");
                    }
                    sb1.Append("</div>");
                    if (dtMail.Rows[i]["Active"].ToString() == "2")
                    {
                        break;
                    }
                }
                sb1.Append("</div><br />");
                sb.Append(sb1.ToString());
                string url = "";
                if (Request.Url.Host != "localhost")
                {
                    //url = "http://61.218.73.79:88/eReimbursement/Approve.aspx";
                    url = "http://" + Request.Url.Authority + "/eReimbursement/Approve.aspx";
                }
                else
                {
                    url = "http://" + Request.Url.Authority + "/Approve.aspx";
                }
                sb.Append("<div><a href=\"" + url + "?FlowID=" + FlowID + "\" style=\"color: #0000FF\">Click here to visit Dimerco eReimbursement.</a></div>");
                sb.Append("</div>");
                mail.Body = testmailstr+sb.ToString();
                mail.Send();
            }
            return true;
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
            cells.Add(1, 2, "Claim Type", titleXF1);
            cells.Add(1, 3, "Station", titleXF1);
            cells.Add(1, 4, "Department", titleXF1);
            cells.Add(1, 5, "Owner", titleXF1);
            cells.Add(1, 6, "Amount", titleXF1);
            cells.Add(1, 7, "Currency", titleXF1);
            cells.Add(1, 8, "Submit Date", titleXF1);
            cells.Add(1, 9, "Status", titleXF1);
            cells.Add(1, 10, "Current Approver", titleXF1);
            cells.Add(1, 11, "Submitted By", titleXF1);
            cells.Add(1, 12, "Remark", titleXF1);

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
                    cells.Add(2 + i, 6, Convert.ToDouble(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Tamount").Item(0).InnerXml), titleXF);
                }
                else
                {
                    cells.Add(2 + i, 6, "", titleXF);
                }
                cells.Add(2 + i, 1, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("No").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 2, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Type1").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 3, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Station").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 4, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Department").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 5, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Person").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 7, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Cur").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 8, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("CreadedDate").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 9, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Status1").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 10, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Approver").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 11, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("CreadedBy").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 12, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Remark").Item(0).InnerXml, titleXF);
            }

            xls.Send();
        }
    }
}