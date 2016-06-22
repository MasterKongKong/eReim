using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
    public partial class Travel : App_Code.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //string station = ""; string department = "";
            //DataSet ds1 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList("A0182");
            //if (ds1.Tables[0].Rows.Count == 1)
            //{
            //    DataTable dt1 = ds1.Tables[0];
            //    station = dt1.Rows[0]["stationCode"].ToString();
            //    department = dt1.Rows[0]["CRPDepartmentName"].ToString();
            //}
            if (!X.IsAjaxRequest)
            {
                
                //判断登录状态
                cs.DBCommand dbc = new cs.DBCommand();
                if (Request.Cookies.Get("eReimUserID") == null)
                {
                    X.AddScript("loginWindow.show();Panel1.disable();");
                    return;
                }
                else
                {
                    hdUser.Value = Request.Cookies.Get("eReimUserID").Value;
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "$('div.gn_person ul.q-menubox li:eq(0) a').text('" + Request.Cookies.Get("eReimUserName").Value + "');", true);
                    X.AddScript("loginWindow.hide();Panel1.enable();");


                    if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                    {
                        ResourceManager1.Locale = "zh-CN";
                    }
                    else
                    {
                        ResourceManager1.Locale = "en-US";
                    }
                }

                //hdStatus.Value = "0";

                if (Request.QueryString["ID"] != null)//判断链接地址是否正确
                {
                    string ID = Request.QueryString["ID"].ToString();
                    System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^\d*$");
                    if (reg1.IsMatch(ID))
                    {
                        string sql = "select * from V_Eflow_ETravel where RequestID='" + ID + "' and [Type]='T' and (Active=1 or Active=2)";

                        DataTable dt = new DataTable();
                        dt = dbc.GetData("eReimbursement", sql);

                        if (dt != null && dt.Rows.Count == 1)
                        {

                            if (Request.Cookies.Get("eReimUserID").Value == dt.Rows[0]["PersonID"].ToString())//本人
                            {
                                //note off by Brian 16.04.20 
                                //准备下拉菜单内容
                                Ext.Net.ListItem li = new Ext.Net.ListItem(Request.Cookies.Get("eReimUserName").Value, Request.Cookies.Get("eReimUserID").Value);
                                cbxPerson.Items.Add(li);
                                string sqlitem = "select * from Eagent where [St]=1 and [PAgentID]='" + Request.Cookies.Get("eReimUserID").Value + "'";
                                DataTable dtitem = dbc.GetData("eReimbursement", sqlitem);
                                int itemcount = 0;
                                for (int j = 0; j < dtitem.Rows.Count; j++)
                                {
                                    string sqlpara = sqlitem;
                                    if (dtitem.Rows[j][5].ToString() != "")
                                    {
                                        sqlpara += " and getdate()>='" + dtitem.Rows[j][5].ToString() + "' ";
                                    }
                                    if (dtitem.Rows[j][6].ToString() != "")
                                    {
                                        sqlpara += " and getdate()<='" + dtitem.Rows[j][6].ToString() + "' ";
                                    }
                                    DataTable dtitem1 = dbc.GetData("eReimbursement", sqlpara);
                                    for (int m = 0; m < dtitem1.Rows.Count; m++)
                                    {
                                        li = new Ext.Net.ListItem(dtitem.Rows[m][1].ToString(), dtitem.Rows[m][2].ToString());
                                        cbxPerson.Items.Add(li);
                                        itemcount++;
                                    }
                                }
                                //unlock

                                //更改按钮状态
                                if (dt.Rows[0]["Step"].ToString() != "0")//正式申请单
                                {
                                    if (Request.QueryString["Copy"] != null)
                                    {
                                        if (Request.QueryString["Copy"].ToString() == "T")//Copy,作为新增
                                        {
                                            X.AddScript("btnSaveAndSend.enable();");
                                        }
                                        else
                                        {
                                            ErrorHandle("Data Error.");
                                        }
                                    }
                                    else//已申请数据
                                    {
                                        string app = "";
                                        if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                        {
                                            Panel3.Title = "差旅费申请单: " + dt.Rows[0]["No"].ToString();
                                            //读取当前状态,显示在下方文本框内

                                            if (dt.Rows[0]["Status"].ToString() == "1")
                                            {
                                                app += "提单人: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                app += ", 等待审批: " + dt.Rows[0]["Approver"].ToString();
                                            }
                                            else if (dt.Rows[0]["Status"].ToString() == "2")
                                            {
                                                app += "提单人: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                app += ", 完成审批: " + dt.Rows[0]["Approver"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["ApproveDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                            }
                                            else if (dt.Rows[0]["Status"].ToString() == "3")
                                            {
                                                app += "提单人: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                app += ", 拒绝审批: " + dt.Rows[0]["Approver"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["ApproveDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                            }

                                            if (dt.Rows[0]["Active"].ToString() == "2")//已完成
                                            {
                                                //hdStatus.Value = "2";
                                                app += ". 完成.";
                                                X.AddScript("btnBudgetView.disable();btnAddDSTN.disable();btnGeteLeave.disable();Radio1.disable();Radio2.disable();btnSaveDraft.disable();btnUploadReport.disable();btnUploadScanFile.disable();Button1.disable();Button2.disable();btnCC.disable();");
                                            }
                                            else//待审批
                                            {
                                                //hdStatus.Value = "1";
                                                X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();Radio1.disable();Radio2.disable();btnSaveDraft.disable();btnCC.disable();");
                                            }

                                        }
                                        else
                                        {
                                            Panel3.Title = "Travel Expense Form: " + dt.Rows[0]["No"].ToString();
                                            //读取当前状态,显示在下方文本框内

                                            if (dt.Rows[0]["Status"].ToString() == "1")
                                            {
                                                app += "Applied by: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                app += ", Waiting for approval By: " + dt.Rows[0]["Approver"].ToString();
                                            }
                                            else if (dt.Rows[0]["Status"].ToString() == "2")
                                            {
                                                app += "Applied by: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                app += ", Approved by: " + dt.Rows[0]["Approver"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["ApproveDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                            }
                                            else if (dt.Rows[0]["Status"].ToString() == "3")
                                            {
                                                app += "Applied by: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                app += ", Rejected by: " + dt.Rows[0]["Approver"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["ApproveDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                            }

                                            if (dt.Rows[0]["Active"].ToString() == "2")//已完成
                                            {
                                                //hdStatus.Value = "2";
                                                app += ". Complete.";
                                                X.AddScript("btnBudgetView.disable();btnAddDSTN.disable();btnGeteLeave.disable();Radio1.disable();Radio2.disable();btnSaveDraft.disable();btnUploadReport.disable();btnUploadScanFile.disable();Button1.disable();Button2.disable();btnCC.disable();");
                                            }
                                            else//待审批
                                            {
                                                //hdStatus.Value = "1";
                                                X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();Radio1.disable();Radio2.disable();btnSaveDraft.disable();btnCC.disable();");
                                            }
                                        }
                                        labelInfo.Text = app;
                                    }
                                }
                                else//草稿
                                {
                                    if (Request.QueryString["Copy"] != null)
                                    {
                                        if (Request.QueryString["Copy"].ToString() == "T")//Copy,作为新增
                                        {
                                            X.AddScript("btnSaveAndSend.enable();");
                                        }
                                        else
                                        {
                                            ErrorHandle("Data Error.");
                                        }
                                    }
                                    else
                                    {
                                        if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                        {
                                            Panel3.Title = "差旅费申请单草稿: " + dt.Rows[0]["No"].ToString();
                                        }
                                        else
                                        {
                                            Panel3.Title = "Travel Expense Draft: " + dt.Rows[0]["No"].ToString();
                                        }

                                        X.AddScript("btnSaveAndSend.enable();");
                                    }
                                }
                                //载入通用数据
                                LoadData(dt, true);
                            }
                            else
                            {
                                //判断是否为代理人
                                bool isagent = false;//记录Session["UserID"]是否为代理人
                                string sqlagent = "select * from Eagent where [St]=1 and [OwnerID]='" + dt.Rows[0]["PersonID"].ToString() + "' and getdate()<=Edate and getdate()>=Bdate";
                                DataTable dtagent = dbc.GetData("eReimbursement", sqlagent);
                                for (int g = 0; g < dtagent.Rows.Count; g++)
                                {
                                    if (Request.Cookies.Get("eReimUserID").Value == dtagent.Rows[g]["PAgentID"].ToString())
                                    {
                                        isagent = true;
                                        break;
                                    }
                                }
                                if (isagent)//代理人访问
                                {
                                    //note off by Brian 16.04.20
                                    Ext.Net.ListItem li = new Ext.Net.ListItem(dt.Rows[0]["Person"].ToString(), dt.Rows[0]["PersonID"].ToString());
                                    cbxPerson.Items.Add(li);
                                    //end

                                    //更改按钮状态
                                    if (dt.Rows[0]["Step"].ToString() != "0")//正式申请单
                                    {
                                        if (Request.QueryString["Copy"] != null)
                                        {
                                            if (Request.QueryString["Copy"].ToString() == "T")//Copy,作为新增
                                            {
                                                X.AddScript("btnSaveAndSend.enable();");
                                            }
                                            else
                                            {
                                                ErrorHandle("Data Error.");
                                            }
                                        }
                                        else
                                        {
                                            string app = "";
                                            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                            {
                                                Panel3.Title = "差旅费申请单: " + dt.Rows[0]["No"].ToString();
                                                //读取当前状态,显示在下方文本框内

                                                if (dt.Rows[0]["Status"].ToString() == "1")
                                                {
                                                    app += "提单人: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                    app += ", 等待审批: " + dt.Rows[0]["Approver"].ToString();
                                                }
                                                else if (dt.Rows[0]["Status"].ToString() == "2")
                                                {
                                                    app += "提单人: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                    app += ", 完成审批: " + dt.Rows[0]["Approver"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["ApproveDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                }
                                                else if (dt.Rows[0]["Status"].ToString() == "3")
                                                {
                                                    app += "提单人: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                    app += ", 拒绝审批: " + dt.Rows[0]["Approver"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["ApproveDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                }

                                                if (dt.Rows[0]["Active"].ToString() == "2")//已完成
                                                {
                                                    //hdStatus.Value = "2";
                                                    app += ". 完成.";
                                                    X.AddScript("btnBudgetView.disable();btnAddDSTN.disable();btnGeteLeave.disable();Radio1.disable();Radio2.disable();btnSaveDraft.disable();btnUploadReport.disable();btnUploadScanFile.disable();Button1.disable();Button2.disable();btnCC.disable();");
                                                }
                                                else//待审批
                                                {
                                                    //hdStatus.Value = "1";
                                                    X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();Radio1.disable();Radio2.disable();btnSaveDraft.disable();btnCC.disable();");
                                                }

                                            }
                                            else
                                            {
                                                Panel3.Title = "Travel Expense Form: " + dt.Rows[0]["No"].ToString();
                                                //读取当前状态,显示在下方文本框内

                                                if (dt.Rows[0]["Status"].ToString() == "1")
                                                {
                                                    app += "Applied by: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                    app += ", Waiting for approval By: " + dt.Rows[0]["Approver"].ToString();
                                                }
                                                else if (dt.Rows[0]["Status"].ToString() == "2")
                                                {
                                                    app += "Applied by: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                    app += ", Approved by: " + dt.Rows[0]["Approver"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["ApproveDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                }
                                                else if (dt.Rows[0]["Status"].ToString() == "3")
                                                {
                                                    app += "Applied by: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                    app += ", Rejected by: " + dt.Rows[0]["Approver"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["ApproveDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                }

                                                if (dt.Rows[0]["Active"].ToString() == "2")//已完成
                                                {
                                                    //hdStatus.Value = "2";
                                                    app += ". Complete.";
                                                    X.AddScript("btnBudgetView.disable();btnAddDSTN.disable();btnGeteLeave.disable();Radio1.disable();Radio2.disable();btnSaveDraft.disable();btnUploadReport.disable();btnUploadScanFile.disable();Button1.disable();Button2.disable();btnCC.disable();");
                                                }
                                                else//待审批
                                                {
                                                    //hdStatus.Value = "1";
                                                    X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();Radio1.disable();Radio2.disable();btnSaveDraft.disable();btnCC.disable();");
                                                }
                                            }
                                            labelInfo.Text = app;
                                        }
                                    }
                                    else//草稿
                                    {
                                        if (Request.QueryString["Copy"] != null)
                                        {
                                            if (Request.QueryString["Copy"].ToString() == "T")//Copy,作为新增
                                            {
                                                X.AddScript("btnSaveAndSend.enable();");
                                            }
                                            else
                                            {
                                                ErrorHandle("Data Error.");
                                            }
                                        }
                                        else
                                        {
                                            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                            {
                                                Panel3.Title = "差旅费申请单草稿: " + dt.Rows[0]["No"].ToString();
                                            }
                                            else
                                            {
                                                Panel3.Title = "Travel Expense Draft: " + dt.Rows[0]["No"].ToString();
                                            }
                                            X.AddScript("btnSaveAndSend.enable();");
                                        }
                                    }
                                    //载入通用数据
                                    LoadData(dt, true);
                                }
                                else//判断是否有跨站权限
                                {

                                    bool hasright = false;
                                    string getright = "select * from StationRole where UserID='" + Request.Cookies.Get("eReimUserID").Value + "'";
                                    DataTable dtright = dbc.GetData("eReimbursement", getright);
                                    for (int j = 0; j < dtright.Rows.Count; j++)
                                    {
                                        string[] dd = dtright.Rows[j]["Stations"].ToString().Split(',');
                                        for (int i = 0; i < dd.Length; i++)
                                        {
                                            if (dd[i] == dt.Rows[0]["Station"].ToString())//有权限
                                            {
                                                hasright = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (hasright)
                                    {
                                        //note off by brian 16.04.20
                                        Ext.Net.ListItem li = new Ext.Net.ListItem(dt.Rows[0]["Person"].ToString(), dt.Rows[0]["PersonID"].ToString());
                                        cbxPerson.Items.Add(li);
                                        //end


                                       
                                        //更改按钮状态
                                        if (dt.Rows[0]["Step"].ToString() != "0")//正式申请单
                                        {
                                            string app = "";
                                            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                            {
                                                Panel3.Title = "差旅费申请单: " + dt.Rows[0]["No"].ToString();
                                                //读取当前状态,显示在下方文本框内

                                                if (dt.Rows[0]["Status"].ToString() == "1")
                                                {
                                                    app += "提单人: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                    app += ", 等待审批: " + dt.Rows[0]["Approver"].ToString();
                                                }
                                                else if (dt.Rows[0]["Status"].ToString() == "2")
                                                {
                                                    app += "提单人: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                    app += ", 完成审批: " + dt.Rows[0]["Approver"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["ApproveDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                }
                                                else if (dt.Rows[0]["Status"].ToString() == "3")
                                                {
                                                    app += "提单人: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                    app += ", 拒绝审批: " + dt.Rows[0]["Approver"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["ApproveDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                }

                                                if (dt.Rows[0]["Active"].ToString() == "2")//已完成
                                                {
                                                    //hdStatus.Value = "2";
                                                    app += ". 完成.";
                                                    X.AddScript("btnBudgetView.disable();btnAddDSTN.disable();btnGeteLeave.disable();Radio1.disable();Radio2.disable();btnSaveDraft.disable();btnUploadReport.disable();btnUploadScanFile.disable();Button1.disable();Button2.disable();btnCC.disable();");
                                                }
                                                else//待审批
                                                {
                                                    //hdStatus.Value = "1";
                                                    X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();Radio1.disable();Radio2.disable();btnSaveDraft.disable();btnCC.disable();");
                                                }

                                            }
                                            else
                                            {
                                                Panel3.Title = "Travel Expense Form: " + dt.Rows[0]["No"].ToString();
                                                //读取当前状态,显示在下方文本框内

                                                if (dt.Rows[0]["Status"].ToString() == "1")
                                                {
                                                    app += "Applied by: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                    app += ", Waiting for approval By: " + dt.Rows[0]["Approver"].ToString();
                                                }
                                                else if (dt.Rows[0]["Status"].ToString() == "2")
                                                {
                                                    app += "Applied by: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                    app += ", Approved by: " + dt.Rows[0]["Approver"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["ApproveDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                }
                                                else if (dt.Rows[0]["Status"].ToString() == "3")
                                                {
                                                    app += "Applied by: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                    app += ", Rejected by: " + dt.Rows[0]["Approver"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["ApproveDate"].ToString()).ToString("yyyy/MM/dd hh:mm");
                                                }

                                                if (dt.Rows[0]["Active"].ToString() == "2")//已完成
                                                {
                                                    //hdStatus.Value = "2";
                                                    app += ". Complete.";
                                                    X.AddScript("btnBudgetView.disable();btnAddDSTN.disable();btnGeteLeave.disable();Radio1.disable();Radio2.disable();btnSaveDraft.disable();btnUploadReport.disable();btnUploadScanFile.disable();Button1.disable();Button2.disable();btnCC.disable();");
                                                }
                                                else//待审批
                                                {
                                                    //hdStatus.Value = "1";
                                                    X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();Radio1.disable();Radio2.disable();btnSaveDraft.disable();btnCC.disable();");
                                                }
                                            }
                                            labelInfo.Text = app;
                                        }
                                        else//草稿
                                        {
                                            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                            {
                                                Panel3.Title = "差旅费申请单草稿: " + dt.Rows[0]["No"].ToString();
                                            }
                                            else
                                            {
                                                Panel3.Title = "Travel Expense Draft: " + dt.Rows[0]["No"].ToString();
                                            }

                                            //X.AddScript("btnSaveAndSend.enable();");
                                        }
                                        //hdStatus.Value = "2";//不允许传递到子页面时修改或者上传
                                        X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();Radio1.disable();Radio2.disable();btnSaveDraft.disable();btnCC.disable();");
                                        //无需判断Copy
                                        //载入通用数据
                                        LoadData(dt, false);
                                    }
                                    else
                                    {
                                        ErrorHandle("No right.");
                                    }
                                }
                            }
                        }
                        else
                        {
                            ErrorHandle("Data Error.");
                        }
                    }
                    else
                    {
                        ErrorHandle("Data Error.");
                    }
                }
                else//新增申请
                {

                    //Edit By Brian 16.04.20
                    ////准备下拉菜单内容
                    //Ext.Net.ListItem li = new Ext.Net.ListItem(Request.Cookies.Get("eReimUserName").Value, Request.Cookies.Get("eReimUserID").Value);
                    //cbxPerson.Items.Add(li);
                    //string sqlitem = "select * from Eagent where [St]=1 and [PAgentID]='" + Request.Cookies.Get("eReimUserID").Value + "'";
                    //DataTable dtitem = dbc.GetData("eReimbursement", sqlitem);
                    //int itemcount = 0;
                    //for (int j = 0; j < dtitem.Rows.Count; j++)
                    //{
                    //    string sqlpara = sqlitem;
                    //    if (dtitem.Rows[j][5].ToString() != "")
                    //    {
                    //        sqlpara += " and getdate()>='" + dtitem.Rows[j][5].ToString() + "' ";
                    //    }
                    //    if (dtitem.Rows[j][6].ToString() != "")
                    //    {
                    //        sqlpara += " and getdate()<='" + dtitem.Rows[j][6].ToString() + "' ";
                    //    }
                    //    DataTable dtitem1 = dbc.GetData("eReimbursement", sqlpara);
                    //    for (int m = 0; m < dtitem1.Rows.Count; m++)
                    //    {
                    //        li = new Ext.Net.ListItem(dtitem.Rows[m][1].ToString(), dtitem.Rows[m][2].ToString());
                    //        cbxPerson.Items.Add(li);
                    //        itemcount++;
                    //    }
                    //}
                    // Edit End
                    //160622 Andy Kang
                    //准备下拉菜单内容
                    Ext.Net.ListItem li = new Ext.Net.ListItem(Request.Cookies.Get("eReimUserName").Value, Request.Cookies.Get("eReimUserID").Value);
                    cbxPerson.Items.Add(li);
                    string sqlitem = "select * from Eagent where [St]=1 and [PAgentID]='" + Request.Cookies.Get("eReimUserID").Value + "'";
                    DataTable dtitem = dbc.GetData("eReimbursement", sqlitem);
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
                        }
                    }


                    ////新增记录时,默认为登录用户
                    //cbxOwner.SelectedItem.Value = Request.Cookies.Get("eReimUserID").Value;
                    //cbxOwner.SelectedItem.Text = Request.Cookies.Get("eReimUserName").Value;
                    //160622 Andy Kang
                    hdOwner.Value = Request.Cookies.Get("eReimUserName").Value;
                    hdOwnerID.Value = Request.Cookies.Get("eReimUserID").Value;
                    //labelOwner.Text = Request.Cookies.Get("eReimUserName").Value;

                    //新增记录时,默认为登录用户 add by Brian 16.05.20
                    cbxPerson.SelectedItem.Value = Request.Cookies.Get("eReimUserID").Value;
                    cbxPerson.SelectedItem.Text = Request.Cookies.Get("eReimUserName").Value;

                    labelStation.Text = Request.Cookies.Get("eReimStation").Value;
                    labelDepartment.Text = Request.Cookies.Get("eReimDepartment").Value;
                    LabelCurrency.Text = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(Request.Cookies.Get("eReimCostCenter").Value);
                    //LabelText.Text = "Please load eLeave Data.";
                    

                    ////检查是否已经为该申请人设置过审批人
                    //string sqlCheckFlow = "";
                    //sqlCheckFlow = "select * from GroupFlow where [Type]!=2 and GID=(select GID from GroupUsers where UserID='" + Request.Cookies.Get("eReimUserID").Value + "')";
                    //DataTable dtGroupFlowData = dbc.GetData("eReimbursement", sqlCheckFlow);
                    //if (dtGroupFlowData.Rows.Count < 1)
                    //{
                    //    if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                    //    {
                    //        ErrorHandle("请先设置审批人.");
                    //    }
                    //    else
                    //    {
                    //        ErrorHandle("Not set Approve flow,please contact with Local MIS.");
                    //    }
                    //    return;
                    //}

                    //160119 垫付
                    X.AddScript("btnSaveAndSend.enable();btnGeteLeave.enable();cbxOnBehalfName.enable();");


                    DataTable dttest = new DataTable();
                    Store2.Reader[0].Fields.Add("Category", RecordFieldType.String);
                    dttest.Columns.Add("Category", typeof(String));
                    string fieldPName = "Station_0_P";
                    Store2.Reader[0].Fields.Add(fieldPName, RecordFieldType.String);
                    string fieldCName = "Station_0_C";
                    Store2.Reader[0].Fields.Add(fieldCName, RecordFieldType.String);
                    dttest.Columns.Add("Station_0_P", typeof(String));
                    dttest.Columns.Add("Station_0_C", typeof(String));
                    //
                    Store2.Reader[0].Fields.Add("Station_1_P", RecordFieldType.String);
                    Store2.Reader[0].Fields.Add("Station_1_C", RecordFieldType.String);
                    dttest.Columns.Add("Station_1_P", typeof(String));
                    dttest.Columns.Add("Station_1_C", typeof(String));

                    //合计列
                    Store2.Reader[0].Fields.Add("TotalP", RecordFieldType.String);
                    Store2.Reader[0].Fields.Add("TotalC", RecordFieldType.String);
                    dttest.Columns.Add("TotalP", typeof(String));
                    dttest.Columns.Add("TotalC", typeof(String));

                    DataRow dr = dttest.NewRow();
                    dr[0] = "1. Air Ticket - Int'l";
                    dttest.Rows.Add(dr);

                    dr = dttest.NewRow();
                    dr[0] = "Air Ticket - Domestic";
                    dttest.Rows.Add(dr);

                    dr = dttest.NewRow();
                    dr[0] = "2. Hotel Bill";
                    dttest.Rows.Add(dr);

                    dr = dttest.NewRow();
                    dr[0] = "3. Meals";
                    dttest.Rows.Add(dr);

                    dr = dttest.NewRow();
                    dr[0] = "4. Entertainment";
                    dttest.Rows.Add(dr);

                    dr = dttest.NewRow();
                    dr[0] = "5. Car Rental/Transportation";
                    dttest.Rows.Add(dr);

                    dr = dttest.NewRow();
                    dr[0] = "6. Communication";
                    dttest.Rows.Add(dr);

                    dr = dttest.NewRow();
                    dr[0] = "7. Local Trip Allowance";
                    dttest.Rows.Add(dr);

                    dr = dttest.NewRow();
                    dr[0] = "8. Overseas Trip Allowance";
                    dttest.Rows.Add(dr);

                    dr = dttest.NewRow();
                    dr[0] = "9. Airport Tax/Travel Insurance";
                    dttest.Rows.Add(dr);

                    dr = dttest.NewRow();
                    dr[0] = "10. Others";
                    dttest.Rows.Add(dr);

                    dr = dttest.NewRow();
                    dr[0] = "Total";
                    dttest.Rows.Add(dr);

                    Store2.DataSource = dttest;
                    Store2.DataBind();


                    var TitleCol = new Column();
                    TitleCol.DataIndex = "Category";
                    TitleCol.Sortable = false;
                    TitleCol.Resizable = false;
                    TitleCol.MenuDisabled = true;
                    TitleCol.Width = 180;
                    this.GridPanel2.ColumnModel.Columns.Add(TitleCol);

                    var txtP = new Ext.Net.NumberField();
                    //txtP.Listeners.Blur.Fn = "Cal";
                    //txtP.Listeners.Blur.Delay = 50;
                    var colP = new Column();
                    colP.Header = "Reimbursement";
                    colP.DataIndex = fieldPName;
                    colP.Sortable = false;
                    colP.Resizable = false;
                    colP.MenuDisabled = true;
                    colP.Width = 110;
                    colP.Locked = true;
                    colP.Editor.Add(txtP);
                    this.GridPanel2.ColumnModel.Columns.Add(colP);

                    var txtC = new Ext.Net.NumberField();
                    //txtC.Listeners.Blur.Fn = "Cal";
                    //txtC.Listeners.Blur.Delay = 50;
                    var colC = new Column();
                    colC.Header = "Company Paid";
                    colC.DataIndex = fieldCName;
                    colC.Sortable = false;
                    colC.Resizable = false;
                    colC.MenuDisabled = true;
                    colC.Width = 110;
                    colP.Locked = true;
                    colC.Editor.Add(txtC);
                    this.GridPanel2.ColumnModel.Columns.Add(colC);
                    //
                    var txtP1 = new Ext.Net.NumberField();
                    //txtP1.Listeners.Blur.Fn = "Cal";
                    //txtP1.Listeners.Blur.Delay = 50;
                    var colP1 = new Column();
                    colP1.Header = "Reimbursement";
                    colP1.DataIndex = "Station_1_P";
                    colP1.Sortable = false;
                    colP1.Resizable = false;
                    colP1.MenuDisabled = true;
                    colP1.Width = 110;
                    colP1.Locked = true;
                    colP1.Editor.Add(txtP1);
                    colP1.Hidden = true;
                    this.GridPanel2.ColumnModel.Columns.Add(colP1);

                    var txtC1 = new Ext.Net.NumberField();
                    //txtC1.Listeners.Blur.Fn = "Cal";
                    //txtC1.Listeners.Blur.Delay = 50;
                    var colC1 = new Column();
                    colC1.Header = "Company Paid";
                    colC1.DataIndex = "Station_1_C";
                    colC1.Sortable = false;
                    colC1.Resizable = false;
                    colC1.MenuDisabled = true;
                    colC1.Width = 110;
                    colC1.Locked = true;
                    colC1.Editor.Add(txtC1);
                    colC1.Hidden = true;
                    this.GridPanel2.ColumnModel.Columns.Add(colC1);
                    //
                    var TotalP = new Ext.Net.NumberField();
                    TotalP.ReadOnly = true;
                    var colTotalP = new Column();
                    colTotalP.DataIndex = "TotalP";
                    colTotalP.Sortable = false;
                    colTotalP.Resizable = false;
                    colTotalP.MenuDisabled = true;
                    colTotalP.Width = 110;
                    colTotalP.Locked = true;
                    colTotalP.Editor.Add(TotalP);
                    this.GridPanel2.ColumnModel.Columns.Add(colTotalP);

                    var TotalC = new Ext.Net.NumberField();
                    TotalC.ReadOnly = true;
                    var colTotalC = new Column();
                    colTotalC.DataIndex = "TotalC";
                    colTotalC.Sortable = false;
                    colTotalC.Resizable = false;
                    colTotalC.MenuDisabled = true;
                    colTotalC.Width = 110;
                    colTotalC.Locked = true;
                    colTotalC.Editor.Add(TotalC);
                    this.GridPanel2.ColumnModel.Columns.Add(colTotalC);

                    var Title1 = new Ext.Net.Label();
                    Title1.Text = "Destination:";
                    HeaderColumn hcTitle1 = new HeaderColumn();
                    hcTitle1.Component.Add(Title1);
                    this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcTitle1);

                    var Station = new Ext.Net.TextField();
                    HeaderColumn hcStation = new HeaderColumn();
                    hcStation.Component.Add(Station);
                    this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcStation);
                    //var Station = new Ext.Net.ComboBox();
                    //Station.DisplayField = "CityCode";
                    //Station.ValueField = "CityCode";
                    //Station.TypeAhead = true;
                    //Station.ForceSelection = true;
                    //Station.MinChars = 2;
                    //Station.Mode = DataLoadMode.Local;
                    //var storeStation = new Ext.Net.Store();
                    //storeStation.Reader.Add(new Ext.Net.JsonReader());
                    //DataSet GetCityInfo = DIMERCO.SDK.Utilities.LSDK.GetCityInfo("", 8000);
                    //DataTable dtstation = GetCityInfo.Tables[0];
                    //storeStation.Reader[0].Fields.Add("CityCode", RecordFieldType.String);
                    //Station.Store.Add(storeStation);
                    //storeStation.DataSource = dtstation;
                    //storeStation.DataBind();
                    //HeaderColumn hcStation = new HeaderColumn();
                    //hcStation.Component.Add(Station);
                    //this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcStation);

                    var Button = new Ext.Net.Button();
                    Button.Text = "Remove";
                    Button.Listeners.Click.Handler = "removecol(this,0);";
                    Button.Listeners.Click.Delay = 50;
                    HeaderColumn hcButton = new HeaderColumn();
                    hcButton.Component.Add(Button);
                    this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcButton);
                    //
                    var Station1 = new Ext.Net.TextField();
                    HeaderColumn hcStation1 = new HeaderColumn();
                    hcStation1.Component.Add(Station1);
                    this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcStation1);
                    //var Station1 = new Ext.Net.ComboBox();
                    //Station1.DisplayField = "CityCode";
                    //Station1.ValueField = "CityCode";
                    //Station1.TypeAhead = true;
                    //Station1.MinChars = 2;
                    //Station1.ForceSelection = true;
                    //Station1.Mode = DataLoadMode.Local;
                    //var storeStation1 = new Ext.Net.Store();
                    //storeStation1.Reader.Add(new Ext.Net.JsonReader());
                    //DataTable dtStation1 = new DataTable();
                    //dtStation1.Columns.Add("CityCode", typeof(String));
                    //for (int i = 0; i < 20; i++)
                    //{
                    //    DataRow dr2 = dtStation1.NewRow();
                    //    if (i < 5)
                    //    {
                    //        dr2[0] = "CN" + i.ToString();
                    //    }
                    //    else if (i >= 5 && i < 10)
                    //    {
                    //        dr2[0] = "GCR" + i.ToString();
                    //    }
                    //    else
                    //    {
                    //        dr2[0] = "CRP" + i.ToString();
                    //    }
                    //    dtStation1.Rows.Add(dr2);
                    //}
                    //storeStation1.Reader[0].Fields.Add("CityCode", RecordFieldType.String);
                    //Station1.Store.Add(storeStation1);
                    //storeStation1.DataSource = dtstation;
                    //storeStation1.DataBind();
                    //HeaderColumn hcStation1 = new HeaderColumn();
                    //hcStation1.Component.Add(Station1);
                    //this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcStation1);

                    var Button1 = new Ext.Net.Button();
                    Button1.Text = "Remove";
                    Button1.Listeners.Click.Handler = "removecol(this,1);";
                    Button1.Listeners.Click.Delay = 50;
                    HeaderColumn hcButton1 = new HeaderColumn();
                    hcButton1.Component.Add(Button1);
                    this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcButton1);
                    //
                    HeaderColumn hcTotal1 = new HeaderColumn();
                    this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcTotal1);

                    HeaderColumn hcTotal2 = new HeaderColumn();
                    this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcTotal2);


                    var Title2 = new Ext.Net.Label();
                    Title2.Text = "Cost Center:";
                    HeaderColumn hcTitle2 = new HeaderColumn();
                    hcTitle2.Component.Add(Title2);
                    this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcTitle2);

                    var CostCenter = new Ext.Net.TextField();
                    CostCenter.Disabled = true;
                    CostCenter.EmptyText = "Station Code";
                    HeaderColumn hcCostCenter = new HeaderColumn();
                    hcCostCenter.Component.Add(CostCenter);
                    this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcCostCenter);
                    //var CostCenter = new Ext.Net.ComboBox();
                    //CostCenter.DisplayField = "StationCode";
                    //CostCenter.ValueField = "StationCode";
                    //CostCenter.TypeAhead = true;
                    //CostCenter.ForceSelection = true;
                    //CostCenter.MinChars = 2;
                    //CostCenter.Mode = DataLoadMode.Local;
                    //var storeCostCenter = new Ext.Net.Store();
                    //storeCostCenter.Reader.Add(new Ext.Net.JsonReader());
                    //DataSet GetCostCenterInfo = DIMERCO.SDK.Utilities.LSDK.getCostCenterBYStationCode("", 8000);
                    //DataTable dtCostCenter = GetCostCenterInfo.Tables[0];
                    //storeCostCenter.Reader[0].Fields.Add("StationCode", RecordFieldType.String);
                    //CostCenter.Store.Add(storeCostCenter);
                    //storeCostCenter.DataSource = dtCostCenter;
                    //storeCostCenter.DataBind();
                    //HeaderColumn hcCostCenter = new HeaderColumn();
                    //hcCostCenter.Component.Add(CostCenter);
                    //this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcCostCenter);

                    //HeaderColumn hcCostCenter1 = new HeaderColumn();
                    //this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcCostCenter1);
                    var ButtonGetSum = new Ext.Net.Button();
                    ButtonGetSum.Text = "Calculate";
                    ButtonGetSum.Listeners.Click.Handler = "GetSum();";
                    ButtonGetSum.Listeners.Click.Delay = 50;
                    HeaderColumn hcButtonGetSum = new HeaderColumn();
                    hcButtonGetSum.Component.Add(ButtonGetSum);
                    this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcButtonGetSum);
                    //
                    var CostCenter0 = new Ext.Net.TextField();
                    CostCenter0.Disabled = true;
                    CostCenter0.EmptyText = "Station Code";
                    HeaderColumn hcCostCenter0 = new HeaderColumn();
                    hcCostCenter0.Component.Add(CostCenter0);
                    this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcCostCenter0);
                    //var CostCenter1 = new Ext.Net.ComboBox();
                    //CostCenter1.DisplayField = "StationCode";
                    //CostCenter1.ValueField = "StationCode";
                    //CostCenter1.TypeAhead = true;
                    //CostCenter1.ForceSelection = true;
                    //CostCenter1.MinChars = 2;
                    //CostCenter1.Mode = DataLoadMode.Local;
                    //var storeCostCenter1 = new Ext.Net.Store();
                    //storeCostCenter1.Reader.Add(new Ext.Net.JsonReader());
                    //storeCostCenter1.Reader[0].Fields.Add("StationCode", RecordFieldType.String);
                    //CostCenter1.Store.Add(storeCostCenter1);
                    //storeCostCenter1.DataSource = dtCostCenter;
                    //storeCostCenter1.DataBind();
                    //HeaderColumn hcCostCenter1 = new HeaderColumn();
                    //hcCostCenter1.Component.Add(CostCenter1);
                    //this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcCostCenter1);

                    //HeaderColumn hcCostCenter01 = new HeaderColumn();
                    //this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcCostCenter01);
                    var ButtonGetSum1 = new Ext.Net.Button();
                    ButtonGetSum1.Text = "Calculate";
                    ButtonGetSum1.Listeners.Click.Handler = "GetSum();";
                    ButtonGetSum1.Listeners.Click.Delay = 50;
                    HeaderColumn hcButtonGetSum1 = new HeaderColumn();
                    hcButtonGetSum1.Component.Add(ButtonGetSum1);
                    this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcButtonGetSum1);
                    //
                    HeaderColumn hcTotal3 = new HeaderColumn();
                    this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcTotal3);

                    HeaderColumn hcTotal4 = new HeaderColumn();
                    this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcTotal4);

                    var Title3 = new Ext.Net.Label();
                    Title3.Text = "Travel Period:";
                    HeaderColumn hcTitle3 = new HeaderColumn();
                    hcTitle3.Component.Add(Title3);
                    this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTitle3);

                    HeaderColumn hcDate1 = new HeaderColumn();
                    var Date1 = new DateField();
                    Date1.EmptyText = "yyyy/MM/dd";
                    Date1.Format = "yyyy/MM/dd";
                    //Date1.SetValue("2013/11/1");
                    hcDate1.Component.Add(Date1);
                    this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcDate1);

                    HeaderColumn hcDate2 = new HeaderColumn();
                    var Date2 = new DateField();
                    Date2.EmptyText = "yyyy/MM/dd";
                    Date2.Format = "yyyy/MM/dd";
                    //Date2.SetValue("2013/11/2");
                    hcDate2.Component.Add(Date2);
                    this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcDate2);
                    //
                    HeaderColumn hcDate3 = new HeaderColumn();
                    var Date3 = new DateField();
                    Date3.EmptyText = "yyyy/MM/dd";
                    Date3.Format = "yyyy/MM/dd";
                    //Date3.SetValue("2013/11/1");
                    hcDate3.Component.Add(Date3);
                    this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcDate3);

                    HeaderColumn hcDate4 = new HeaderColumn();
                    var Date4 = new DateField();
                    Date4.EmptyText = "yyyy/MM/dd";
                    Date4.Format = "yyyy/MM/dd";
                    //Date4.SetValue("2013/11/2");
                    hcDate4.Component.Add(Date4);
                    this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcDate4);
                    //

                    //HeaderColumn hcTotal5 = new HeaderColumn();
                    //this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTotal5);

                    //HeaderColumn hcTotal6 = new HeaderColumn();
                    //this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTotal6);

                    var TitleTotalP = new Ext.Net.Label();
                    TitleTotalP.Text = "Total(Personal paid)";
                    TitleTotalP.Cls = "custom-row";
                    HeaderColumn hcTitleTotalP = new HeaderColumn();
                    hcTitleTotalP.Component.Add(TitleTotalP);
                    this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTitleTotalP);

                    var TitleTotalC = new Ext.Net.Label();
                    TitleTotalC.Text = "Total(Company)";
                    TitleTotalC.Cls = "custom-row";
                    HeaderColumn hcTitleTotalC = new HeaderColumn();
                    hcTitleTotalC.Component.Add(TitleTotalC);
                    this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTitleTotalC);


                    //add "CheckBoxOnBehalfItem.enable();cbxPerson.enable();cbxOnBehalfName.disable();" by brian 16.04.21
                    X.AddScript("GridPanel2.colModel.setHidden(3, true);GridPanel2.colModel.setHidden(4, true);Store2.removeField('Station_1_P');Store2.removeField('Station_1_C');CheckBoxOnBehalfItem.enable();cbxPerson.enable();cbxOnBehalfName.disable();");
                }
                //DataSet dsdep = DIMERCO.SDK.Utilities.LSDK.getCRPDepartment();
                //DataTable dtdep = new DataTable();
                //dtdep.Columns.Add(new DataColumn("Depart", typeof(String)));
                //for (int i = 0; i < dsdep.Tables[0].Rows.Count; i++)
                //{
                //    DataRow dr = dtdep.NewRow();
                //    dr[0] = dsdep.Tables[0].Rows[i][2].ToString();
                //    dtdep.Rows.Add(dr);
                //}
                //StoreDepartment.DataSource = dtdep;
                //StoreDepartment.DataBind();

            }
        }
        protected void LoadData(DataTable dt, bool CheckCopy)
        {
            string ID = dt.Rows[0]["RequestID"].ToString();
            cs.DBCommand dbc = new cs.DBCommand();
            //cbxOwner.SelectedItem.Value = dt.Rows[0]["PersonID"].ToString();
            bool copysuc = false;//记录是否成功复制

            //160113 垫付人信息
            cbxOnBehalfName.SelectedItem.Value = dt.Rows[0]["OnBehalfPersonName"].ToString();
            LabelDept.Text = dt.Rows[0]["OnBehalfPersonDept"].ToString();
            LabelUnit.Text = dt.Rows[0]["OnBehalfPersonUnit"].ToString();
            LabelCost.Text = dt.Rows[0]["OnBehalfPersonCostCenter"].ToString();
            hdOnBehalf.Value = dt.Rows[0]["OnBehalfPersonID"].ToString();

            if (dt.Rows[0]["OnBehalfPersonID"].ToString() != "" && dt.Rows[0]["OnBehalfPersonID"] != null)
            {
                CheckBoxOnBehalfItem.Checked = true;
            }


            if (CheckCopy)//根据Copy判断是否需要判断Copy状态
            {
                if (Request.QueryString["Copy"] != null)
                {
                    if (Request.QueryString["Copy"].ToString() == "T")//Copy而已,作为新增
                    {
                        copysuc = true;
                        hdTravelRequestID.Value = "";
                        hdTravelRequestNo.Value = "";
                        if (Request.QueryString["CopyType"] != null)
                        {
                            if (Request.QueryString["CopyType"].ToString() == "A")
                            {
                                if (dt.Rows[0]["ReportFile"].ToString() != "")
                                {
                                    linkTravelReport.Text = dt.Rows[0]["ReportFile"].ToString();
                                    linkTravelReport.NavigateUrl = "./Upload/" + dt.Rows[0]["ReportFile"].ToString();
                                    hdReport.Value = dt.Rows[0]["ReportFile"].ToString();
                                }
                                if (dt.Rows[0]["Attach"].ToString() != "")
                                {
                                    linkScanFile.Text = dt.Rows[0]["Attach"].ToString();
                                    linkScanFile.NavigateUrl = "./Upload/" + dt.Rows[0]["Attach"].ToString();
                                    hdScanFile.Value = dt.Rows[0]["Attach"].ToString();
                                }
                            }
                        }
                    }
                    else
                    {
                        ErrorHandle("Data Error.");
                    }
                }
                else
                {

                    hdTravelRequestID.Value = ID;
                    hdTravelRequestNo.Value = dt.Rows[0]["No"].ToString();
                    if (dt.Rows[0]["ReportFile"].ToString() != "")
                    {
                        linkTravelReport.Text = dt.Rows[0]["ReportFile"].ToString();
                        linkTravelReport.NavigateUrl = "./Upload/" + dt.Rows[0]["ReportFile"].ToString();
                        hdReport.Value = dt.Rows[0]["ReportFile"].ToString();
                    }
                    if (dt.Rows[0]["Attach"].ToString() != "")
                    {
                        linkScanFile.Text = dt.Rows[0]["Attach"].ToString();
                        linkScanFile.NavigateUrl = "./Upload/" + dt.Rows[0]["Attach"].ToString();
                        hdScanFile.Value = dt.Rows[0]["Attach"].ToString();
                    }
                    X.AddScript("btnExport.enable();");
                }
            }
            else
            {
                hdTravelRequestID.Value = ID;
                hdTravelRequestNo.Value = dt.Rows[0]["No"].ToString();
                if (dt.Rows[0]["ReportFile"].ToString() != "")
                {
                    linkTravelReport.Text = dt.Rows[0]["ReportFile"].ToString();
                    linkTravelReport.NavigateUrl = "./Upload/" + dt.Rows[0]["ReportFile"].ToString();
                    hdReport.Value = dt.Rows[0]["ReportFile"].ToString();
                }
                if (dt.Rows[0]["Attach"].ToString() != "")
                {
                    linkScanFile.Text = dt.Rows[0]["Attach"].ToString();
                    linkScanFile.NavigateUrl = "./Upload/" + dt.Rows[0]["Attach"].ToString();
                    hdScanFile.Value = dt.Rows[0]["Attach"].ToString();
                }
            }
            
            if (dt.Rows[0]["CCMailList"].ToString() != "")
            {
                ToolTipCC.Html = dt.Rows[0]["CCMailList"].ToString().Replace(",", "<br />");
                DataTable dtCCMailList = new DataTable();
                dtCCMailList.Columns.Add("Email", typeof(String));
                for (int i = 0; i < dt.Rows[0]["CCMailList"].ToString().Split(',').Length; i++)
                {
                    DataRow dr = dtCCMailList.NewRow();
                    dr["Email"] = dt.Rows[0]["CCMailList"].ToString().Split(',')[i];
                    dtCCMailList.Rows.Add(dr);
                }
                StoreCCList.DataSource = dtCCMailList;
                StoreCCList.DataBind();
            }
            //载入已经保存的基本信息

            //delete by Brian 16.04.20
            //labelOwner.Text = dt.Rows[0]["Person"].ToString();
            hdOwner.Value = dt.Rows[0]["Person"].ToString();
            hdOwnerID.Value = dt.Rows[0]["PersonID"].ToString();

            //add by brian 16.04.20,change to cbx from hidden input
            cbxPerson.SelectedItem.Value = dt.Rows[0]["PersonID"].ToString();
           
            
            labelStation.Text = dt.Rows[0]["Station"].ToString();
            hdStation.Value = dt.Rows[0]["Station"].ToString();
            labelDepartment.Text = dt.Rows[0]["Department"].ToString();
            txtRemark.Text = dt.Rows[0]["Remark"].ToString();
            //组合数据
            DataTable dtnew = new DataTable();
            Store2.Reader[0].Fields.Add("Category", RecordFieldType.String);
            dtnew.Columns.Add("Category", typeof(String));
            dtnew.Columns.Add("TotalP", typeof(String));
            dtnew.Columns.Add("TotalC", typeof(String));
            //默认12行
            for (int i = 0; i < 12; i++)
            {
                DataRow dr = dtnew.NewRow();
                dtnew.Rows.Add(dr);
            }
            //默认第一列是标题
            dtnew.Rows[0][0] = "1. Air Ticket - Int'l";
            dtnew.Rows[1][0] = "Air Ticket - Domestic";
            dtnew.Rows[2][0] = "2. Hotel Bill";
            dtnew.Rows[3][0] = "3. Meals";
            dtnew.Rows[4][0] = "4. Entertainment";
            dtnew.Rows[5][0] = "5. Car Rental/Transportation";
            dtnew.Rows[6][0] = "6. Communication";
            dtnew.Rows[7][0] = "7. Local Trip Allowance";
            dtnew.Rows[8][0] = "8. Overseas Trip Allowance";
            dtnew.Rows[9][0] = "9. Airport Tax/Travel Insurance";
            dtnew.Rows[10][0] = "10. Others";
            dtnew.Rows[11][0] = "Total";

            string sqlf = "select distinct Tocity from ETraveleDetail where [No]='" + ID + "'";
            DataTable dtf = dbc.GetData("eReimbursement", sqlf);
            string sqld = "select * from ETraveleDetail where [No]='" + ID + "' order by id";
            DataTable dtall = new DataTable();
            dtall = dbc.GetData("eReimbursement", sqld);
            LabelCurrency.Text = dtall.Rows[0]["Cur"].ToString();//读取费用明细中的本地币种显示

            //140226 显示预算
            DataTable dtbudget = new DataTable();
            
            //StoreBudget添加Field
            StoreBudget.Reader[0].Fields.Add("EName", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("COACode", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("Current", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("PU", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("PB", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("PPercent", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("DU", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("DB", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("DPercent", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("SU", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("SB", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("SPercent", RecordFieldType.String);
            //如果不是复制而来,Status=2或者3
            DataTable dtnn = new DataTable();
            string sqlbu = "select EName,COACode,LocalAmount as [Current],PU,PB,PPercent,DU,DB,DPercent,SU,SB,SPercent from Budget_Complete where FormType='T' and RequestID=" + ID;
            dtnn = dbc.GetData("eReimbursement", sqlbu);
            if (!copysuc && (dt.Rows[0]["Status"].ToString() == "2" || dt.Rows[0]["Status"].ToString() == "3") && dtnn.Rows.Count > 0)
            {
                dtbudget = dtnn;
                bool PB = false, DB = false, SB = false;
                //计算%,取得名称,预算转换为本地汇率
                for (int i = 0; i < dtbudget.Rows.Count; i++)
                {
                    if (Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()) != 0)//如果Budget不为0,则计算%
                    {
                        if (!PB)
                        {
                            PB = true;
                        }
                    }
                    if (Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()) != 0)//如果Budget不为0,则计算%
                    {
                        if (!DB)
                        {
                            DB = true;
                        }
                    }
                    if (Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()) != 0)//如果Budget不为0,则计算%
                    {
                        if (!SB)
                        {
                            SB = true;
                        }
                    }
                }
                //添加数据列
                var cm = GridPanelBudget.ColumnModel;
                cm.Columns.Add(new Column
                {
                    DataIndex = "EName",
                    Header = "Expense Item",
                    Sortable = false,
                    Resizable = false,
                    MenuDisabled = true,
                    Width = 100
                });
                cm.Columns.Add(new Column
                {
                    DataIndex = "Current",
                    Header = "Current",
                    Renderer = new Renderer { Fn = "GetNumber" },
                    Sortable = false,
                    Resizable = false,
                    MenuDisabled = true,
                    Width = 100
                });
                //显示个人预算部分
                if (PB)
                {
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "PU",
                        Header = "Personal Used",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "PB",
                        Header = "Personal Budget",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "PPercent",
                        Header = "%(Current+Used/Budget)",
                        Renderer = new Renderer { Fn = "GetNumberPercent" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                }
                if (DB)
                {
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "DU",
                        Header = "Department Used",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "DB",
                        Header = "Department Budget",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "DPercent",
                        Header = "%(Current+Used/Budget)",
                        Renderer = new Renderer { Fn = "GetNumberPercent" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                }
                if (SB)
                {
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "SU",
                        Header = "Unit Used",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "SB",
                        Header = "Unit Budget",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "SPercent",
                        Header = "%(Current+Used/Budget)",
                        Renderer = new Renderer { Fn = "GetNumberPercent" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                }
            }
            else
            {
                dtbudget.Columns.Add("EName", typeof(System.String));
                dtbudget.Columns.Add("COACode", typeof(System.String));
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
                
                //取得预算日期
                string sqlA = "select convert(varchar(10),min(Tdate0),111) as BudgetDate from ETraveleDetail where No='" + ID + "'";
                DataTable dtA = dbc.GetData("eReimbursement", sqlA);
                //取得本币与成本中心汇率转换
                decimal rate = 1;
                string CurLocal = dtall.Rows[0]["Cur"].ToString();
                //160616 Andy Kang
                string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dt.Rows[0]["CostCenter"].ToString());
                if (CurLocal != CurBudget)
                {
                    rate = DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(CurLocal, CurBudget, Convert.ToDateTime(dtA.Rows[0]["BudgetDate"].ToString()).Year);
                }

                //取得4大类合计
                //string sqlB = "select sum(T1) as T1,sum(T2) as T2,sum(T3) as T3,sum(T4) as T4 from (select case when AccountCode='62012000' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T1],case when AccountCode='62010900' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T2],case when AccountCode='62011900' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T3],case when AccountCode='62010500' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T4] from ETraveleDetail where No=" + ID + ") t";
                string sqlB = "select isnull(sum(isnull(Pamount,0)+isnull(Camount,0)),0) as Amount,'62012000' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62012000' union all select isnull(sum(isnull(Pamount,0)+isnull(Camount,0)),0) as Amount,'62010900' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62010900' union all select isnull(sum(isnull(Pamount,0)+isnull(Camount,0)),0) as Amount,'62011900' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62011900' union all select isnull(sum(isnull(Pamount,0)+isnull(Camount,0)),0) as Amount,'62010500' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62010500'";
                DataTable dtB = dbc.GetData("eReimbursement", sqlB);
                //取得传递预算的参数
                string userid = dt.Rows[0]["PersonID"].ToString();
                string dpt = dt.Rows[0]["Department"].ToString();
                string ostation = dt.Rows[0]["CostCenter"].ToString();//预算站点,与基本信息中的CostCenter一致(Station2)
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
                        dr["COACode"] = dtB.Rows[g]["COACode"].ToString();
                        accountcode = dtB.Rows[g]["COACode"].ToString();
                        DataTable dtC = new DataTable();
                        DataTable dfr = Comm.RtnEB(userid, dpt, ostation, tstation, accountcode, year, month);
                        //复制而来的话不排除本申请计入Used
                        if (copysuc)
                        {
                            dtC = Comm.RtnEB(userid, dpt, ostation, tstation, accountcode, year, month);
                        }
                        else
                        {
                            dtC = Comm.ExRtnEB(userid, dpt, ostation, tstation, accountcode, year, month, "T", ID);
                        }

                        for (int i = 0; i < dtC.Rows.Count; i++)
                        {
                            if (dtC.Rows[i]["Type"].ToString() == "全年个人")
                            {
                                dr["PU"] = Convert.ToDecimal(dtC.Rows[i]["Used"].ToString());
                                dr["PB"] = Convert.ToDecimal(dtC.Rows[i]["Budget"].ToString());
                            }
                            else if (dtC.Rows[i]["Type"].ToString() == "全年部门")
                            {
                                dr["DU"] = Convert.ToDecimal(dtC.Rows[i]["Used"].ToString());
                                dr["DB"] = Convert.ToDecimal(dtC.Rows[i]["Budget"].ToString());
                            }
                            else if (dtC.Rows[i]["Type"].ToString() == "全年站点")
                            {
                                dr["SU"] = Convert.ToDecimal(dtC.Rows[i]["Used"].ToString());
                                dr["SB"] = Convert.ToDecimal(dtC.Rows[i]["Budget"].ToString());
                            }
                        }
                        dtbudget.Rows.Add(dr);
                    }
                }
                bool PB = false, DB = false, SB = false;
                //计算%,取得名称,预算转换为本地汇率
                for (int i = 0; i < dtbudget.Rows.Count; i++)
                {
                    if (Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()) != 0)//如果Budget不为0,则计算%
                    {
                        dtbudget.Rows[i]["PPercent"] = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["PU"].ToString()) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()), 2);
                        if (!PB)
                        {
                            PB = true;
                        }
                    }
                    if (Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()) != 0)//如果Budget不为0,则计算%
                    {
                        dtbudget.Rows[i]["DPercent"] = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["DU"].ToString()) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()), 2);
                        if (!DB)
                        {
                            DB = true;
                        }
                    }
                    if (Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()) != 0)//如果Budget不为0,则计算%
                    {
                        dtbudget.Rows[i]["SPercent"] = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["SU"].ToString()) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()), 2);
                        if (!SB)
                        {
                            SB = true;
                        }
                    }
                    if (dtbudget.Rows[i]["COACode"].ToString() == "62012000")
                    {
                        dtbudget.Rows[i]["EName"] = "Travel expense";
                    }
                    else if (dtbudget.Rows[i]["COACode"].ToString() == "62010900")
                    {
                        dtbudget.Rows[i]["EName"] = "Entertainment";
                    }
                    else if (dtbudget.Rows[i]["COACode"].ToString() == "62011900")
                    {
                        dtbudget.Rows[i]["EName"] = "Transportation";
                    }
                    else if (dtbudget.Rows[i]["COACode"].ToString() == "62010500")
                    {
                        dtbudget.Rows[i]["EName"] = "Communication";
                    }
                    dtbudget.Rows[i]["PU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["PU"].ToString()), 2);
                    dtbudget.Rows[i]["PB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()), 2);
                    dtbudget.Rows[i]["DU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["DU"].ToString()), 2);
                    dtbudget.Rows[i]["DB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()), 2);
                    dtbudget.Rows[i]["SU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["SU"].ToString()), 2);
                    dtbudget.Rows[i]["SB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()), 2);
                }


                //for (int i = 0; i < dtbudget.Rows.Count; i++)
                //{
                //    if (!PB)
                //    {
                //        if (Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()) != 0)//是否显示个人预算列
                //        {
                //            PB = true;
                //        }
                //    }
                //    if (!DB)
                //    {
                //        if (Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()) != 0)//是否显示部门预算列
                //        {
                //            DB = true;
                //        }
                //    }
                //    if (!SB)
                //    {
                //        if (Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()) != 0)//是否显示站点预算列
                //        {
                //            SB = true;
                //        }
                //    }
                //}
                //添加数据列
                var cm = GridPanelBudget.ColumnModel;
                cm.Columns.Add(new Column
                {
                    DataIndex = "EName",
                    Header = "Expense Item",
                    Sortable = false,
                    Resizable = false,
                    MenuDisabled = true,
                    Width = 100
                });
                cm.Columns.Add(new Column
                {
                    DataIndex = "Current",
                    Header = "Current",
                    Renderer = new Renderer { Fn = "GetNumber" },
                    Sortable = false,
                    Resizable = false,
                    MenuDisabled = true,
                    Width = 100
                });
                //显示个人预算部分
                if (PB)
                {
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "PU",
                        Header = "Personal Used",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "PB",
                        Header = "Personal Budget",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "PPercent",
                        Header = "%(Used/Budget)",
                        Renderer = new Renderer { Fn = "GetNumberPercent" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                }
                if (DB)
                {
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "DU",
                        Header = "Department Used",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "DB",
                        Header = "Department Budget",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "DPercent",
                        Header = "%(Used/Budget)",
                        Renderer = new Renderer { Fn = "GetNumberPercent" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                }
                if (SB)
                {
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "SU",
                        Header = "Unit Used",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "SB",
                        Header = "Unit Budget",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "SPercent",
                        Header = "%(Used/Budget)",
                        Renderer = new Renderer { Fn = "GetNumberPercent" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                }
            }

            //140226 显示预算
            if (dt.Rows[0]["OnBehalfPersonID"].ToString() == "")
            {
                StoreBudget.DataSource = dtbudget;
                StoreBudget.DataBind();
            }
            
            //
            if (dtf.Rows.Count == 1)
            {
                
                for (int i = 0; i < dtall.Rows.Count; i++)
                {
                    if (i % 11 == 0)
                    {
                        string colname = "Station_" + (i / 11).ToString() + "_P";
                        Store2.Reader[0].Fields.Add(colname, RecordFieldType.String);
                        dtnew.Columns.Add(colname, typeof(String));
                        colname = "Station_" + (i / 11).ToString() + "_C";
                        Store2.Reader[0].Fields.Add(colname, RecordFieldType.String);
                        dtnew.Columns.Add(colname, typeof(String));
                    }
                    dtnew.Rows[i % 11][3 + i / 11 * 2] = dtall.Rows[i]["Pamount"].ToString();
                    dtnew.Rows[i % 11][3 + i / 11 * 2 + 1] = dtall.Rows[i]["Camount"].ToString();
                    //列合计
                    string df = dtnew.Rows[11][3 + i / 11 * 2].ToString() == "" ? "0" : dtnew.Rows[11][3 + i / 11 * 2].ToString();
                    string cf = dtall.Rows[i]["Pamount"].ToString() == "" ? "0" : dtall.Rows[i]["Pamount"].ToString();
                    dtnew.Rows[11][3 + i / 11 * 2] = Convert.ToDecimal(df) + Convert.ToDecimal(cf) == 0 ? "" : (Convert.ToDecimal(df) + Convert.ToDecimal(cf)).ToString();

                    string bf = dtnew.Rows[11][3 + i / 11 * 2 + 1].ToString() == "" ? "0" : dtnew.Rows[11][3 + i / 11 * 2 + 1].ToString();
                    string af = dtall.Rows[i]["Camount"].ToString() == "" ? "0" : dtall.Rows[i]["Camount"].ToString();
                    dtnew.Rows[11][3 + i / 11 * 2 + 1] = Convert.ToDecimal(bf) + Convert.ToDecimal(af) == 0 ? "" : (Convert.ToDecimal(bf) + Convert.ToDecimal(af)).ToString();
                    //行合计
                    string df1 = dtnew.Rows[i % 11][1].ToString() == "" ? "0" : dtnew.Rows[i % 11][1].ToString();
                    dtnew.Rows[i % 11][1] = Convert.ToDecimal(df1) + Convert.ToDecimal(cf) == 0 ? "" : (Convert.ToDecimal(df1) + Convert.ToDecimal(cf)).ToString();
                    string bf1 = dtnew.Rows[i % 11][2].ToString() == "" ? "0" : dtnew.Rows[i % 11][2].ToString();
                    dtnew.Rows[i % 11][2] = Convert.ToDecimal(bf1) + Convert.ToDecimal(af) == 0 ? "" : (Convert.ToDecimal(bf1) + Convert.ToDecimal(af)).ToString();
                    //总合计
                    string tf = dtnew.Rows[11][1].ToString() == "" ? "0" : dtnew.Rows[11][1].ToString();
                    dtnew.Rows[11][1] = Convert.ToDecimal(tf) + Convert.ToDecimal(cf) == 0 ? "" : (Convert.ToDecimal(tf) + Convert.ToDecimal(cf)).ToString();
                    string ff = dtnew.Rows[11][2].ToString() == "" ? "0" : dtnew.Rows[11][2].ToString();
                    dtnew.Rows[11][2] = Convert.ToDecimal(ff) + Convert.ToDecimal(af) == 0 ? "" : (Convert.ToDecimal(ff) + Convert.ToDecimal(af)).ToString();
                }
                dtnew.Columns.Add("Station_1_P", typeof(String));
                dtnew.Columns.Add("Station_1_C", typeof(String));
                Store2.Reader[0].Fields.Add("Station_1_P", RecordFieldType.String);
                Store2.Reader[0].Fields.Add("Station_1_C", RecordFieldType.String);
                Store2.Reader[0].Fields.Add("TotalP", RecordFieldType.String);
                Store2.Reader[0].Fields.Add("TotalC", RecordFieldType.String);

                Store2.DataSource = dtnew;
                Store2.DataBind();

                var TitleCol = new Column();
                TitleCol.DataIndex = "Category";
                TitleCol.Sortable = false;
                TitleCol.Resizable = false;
                TitleCol.MenuDisabled = true;
                TitleCol.Width = 180;
                this.GridPanel2.ColumnModel.Columns.Add(TitleCol);

                var Title1 = new Ext.Net.Label();
                Title1.Text = "Destination:";
                HeaderColumn hcTitle1 = new HeaderColumn();
                hcTitle1.Component.Add(Title1);
                this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcTitle1);

                var Title2 = new Ext.Net.Label();
                Title2.Text = "Cost Center:";
                HeaderColumn hcTitle2 = new HeaderColumn();
                hcTitle2.Component.Add(Title2);
                this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcTitle2);

                var Title3 = new Ext.Net.Label();
                Title3.Text = "Travel Period:";
                HeaderColumn hcTitle3 = new HeaderColumn();
                hcTitle3.Component.Add(Title3);
                this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTitle3);

                int colc = dtall.Rows.Count / 11;
                for (int i = 0; i < colc; i++)//准备复制已有信息
                {
                    string fieldPName = "Station_" + i.ToString() + "_P";
                    string fieldCName = "Station_" + i.ToString() + "_C";

                    var txtP = new Ext.Net.NumberField();
                    //txtP.Listeners.Blur.Fn = "Cal";
                    var colP = new Column();
                    colP.Header = "Reimbursement";
                    colP.DataIndex = fieldPName;
                    colP.Sortable = false;
                    colP.Resizable = false;
                    colP.MenuDisabled = true;
                    colP.Width = 110;
                    colP.Editor.Add(txtP);
                    this.GridPanel2.ColumnModel.Columns.Add(colP);

                    var txtC = new Ext.Net.NumberField();
                    //txtC.Listeners.Blur.Fn = "Cal";
                    var colC = new Column();
                    colC.Header = "Company Paid";
                    colC.DataIndex = fieldCName;
                    colC.Sortable = false;
                    colC.Resizable = false;
                    colC.MenuDisabled = true;
                    colC.Width = 110;
                    colC.Editor.Add(txtC);
                    this.GridPanel2.ColumnModel.Columns.Add(colC);

                    var Station = new Ext.Net.TextField();
                    Station.Text = dtall.Rows[i * 11]["Tocity"].ToString();
                    HeaderColumn hcStation = new HeaderColumn();
                    hcStation.Component.Add(Station);
                    this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcStation);

                    var Button = new Ext.Net.Button();
                    Button.Text = "Remove";
                    Button.Listeners.Click.Handler = "removecol(this," + i.ToString() + ");";
                    Button.Listeners.Click.Delay = 50;
                    HeaderColumn hcButton = new HeaderColumn();
                    hcButton.Component.Add(Button);
                    this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcButton);

                    var CostCenter = new Ext.Net.TextField();
                    CostCenter.Disabled = true;
                    CostCenter.EmptyText = "Station Code";
                    CostCenter.Text = dtall.Rows[i * 11]["TSation"].ToString();
                    HeaderColumn hcCostCenter = new HeaderColumn();
                    hcCostCenter.Component.Add(CostCenter);
                    this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcCostCenter);

                    //HeaderColumn hcCostCenter1 = new HeaderColumn();
                    //this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcCostCenter1);
                    var ButtonGetSum = new Ext.Net.Button();
                    ButtonGetSum.Text = "Calculate";
                    ButtonGetSum.Listeners.Click.Handler = "GetSum();";
                    ButtonGetSum.Listeners.Click.Delay = 50;
                    HeaderColumn hcButtonGetSum = new HeaderColumn();
                    hcButtonGetSum.Component.Add(ButtonGetSum);
                    this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcButtonGetSum);

                    var datefrom = new DateField();
                    string dtfroms = dtall.Rows[i * 11]["Tdate0"].ToString() == "" ? "" : Convert.ToDateTime(dtall.Rows[i * 11]["Tdate0"].ToString()).ToString("yyyy/MM/dd");
                    datefrom.SetValue(dtfroms);
                    datefrom.EmptyText = "yyyy/MM/dd";
                    datefrom.Format = "yyyy/MM/dd";
                    HeaderColumn Date1 = new HeaderColumn();
                    Date1.Component.Add(datefrom);
                    this.GridPanel2.GetView().HeaderRows[2].Columns.Add(Date1);

                    var dateto = new DateField();
                    string datetos = dtall.Rows[i * 11]["Tdate"].ToString() == "" ? "" : Convert.ToDateTime(dtall.Rows[i * 11]["Tdate"].ToString()).ToString("yyyy/MM/dd");
                    dateto.SetValue(datetos);
                    dateto.EmptyText = "yyyy/MM/dd";
                    dateto.Format = "yyyy/MM/dd";
                    HeaderColumn Date2 = new HeaderColumn();
                    Date2.Component.Add(dateto);
                    this.GridPanel2.GetView().HeaderRows[2].Columns.Add(Date2);
                }

                var txtP1 = new Ext.Net.NumberField();
                //txtP1.Listeners.Blur.Fn = "Cal";
                //txtP1.Listeners.Blur.Delay = 50;
                var colP1 = new Column();
                colP1.Header = "Reimbursement";
                colP1.DataIndex = "Station_1_P";
                colP1.Sortable = false;
                colP1.Resizable = false;
                colP1.MenuDisabled = true;
                colP1.Width = 110;
                colP1.Locked = true;
                colP1.Editor.Add(txtP1);
                colP1.Hidden = true;
                this.GridPanel2.ColumnModel.Columns.Add(colP1);

                var txtC1 = new Ext.Net.NumberField();
                //txtC1.Listeners.Blur.Fn = "Cal";
                //txtC1.Listeners.Blur.Delay = 50;
                var colC1 = new Column();
                colC1.Header = "Company Paid";
                colC1.DataIndex = "Station_1_C";
                colC1.Sortable = false;
                colC1.Resizable = false;
                colC1.MenuDisabled = true;
                colC1.Width = 110;
                colC1.Locked = true;
                colC1.Editor.Add(txtC1);
                colC1.Hidden = true;
                this.GridPanel2.ColumnModel.Columns.Add(colC1);

                var TotalP = new Ext.Net.NumberField();
                TotalP.ReadOnly = true;
                TotalP.Cls = "custom-row";
                var colTotalP = new Column();
                colTotalP.DataIndex = "TotalP";
                colTotalP.Sortable = false;
                colTotalP.Resizable = false;
                colTotalP.MenuDisabled = true;
                colTotalP.Width = 110;
                colTotalP.Locked = true;
                colTotalP.Editor.Add(TotalP);
                this.GridPanel2.ColumnModel.Columns.Add(colTotalP);

                var TotalC = new Ext.Net.NumberField();
                TotalC.ReadOnly = true;
                TotalC.Cls = "custom-row";
                var colTotalC = new Column();
                colTotalC.DataIndex = "TotalC";
                colTotalC.Sortable = false;
                colTotalC.Resizable = false;
                colTotalC.MenuDisabled = true;
                colTotalC.Width = 110;
                colTotalC.Locked = true;
                colTotalC.Editor.Add(TotalC);
                this.GridPanel2.ColumnModel.Columns.Add(colTotalC);

                var Station1 = new Ext.Net.TextField();
                HeaderColumn hcStation1 = new HeaderColumn();
                hcStation1.Component.Add(Station1);
                this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcStation1);

                var CostCenter0 = new Ext.Net.TextField();
                CostCenter0.Disabled = true;
                CostCenter0.EmptyText = "Station Code";
                HeaderColumn hcCostCenter0 = new HeaderColumn();
                hcCostCenter0.Component.Add(CostCenter0);
                this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcCostCenter0);

                HeaderColumn hcDate3 = new HeaderColumn();
                var Date3 = new DateField();
                Date3.EmptyText = "yyyy/MM/dd";
                Date3.Format = "yyyy/MM/dd";
                //Date3.SetValue("2013/11/1");
                hcDate3.Component.Add(Date3);
                this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcDate3);


                var Button1 = new Ext.Net.Button();
                Button1.Text = "Remove";
                Button1.Listeners.Click.Handler = "removecol(this,1);";
                Button1.Listeners.Click.Delay = 50;
                HeaderColumn hcButton1 = new HeaderColumn();
                hcButton1.Component.Add(Button1);
                this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcButton1);

                var ButtonGetSum1 = new Ext.Net.Button();
                ButtonGetSum1.Text = "Calculate";
                ButtonGetSum1.Listeners.Click.Handler = "GetSum();";
                ButtonGetSum1.Listeners.Click.Delay = 50;
                HeaderColumn hcButtonGetSum1 = new HeaderColumn();
                hcButtonGetSum1.Component.Add(ButtonGetSum1);
                this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcButtonGetSum1);

                HeaderColumn hcDate4 = new HeaderColumn();
                var Date4 = new DateField();
                Date4.EmptyText = "yyyy/MM/dd";
                Date4.Format = "yyyy/MM/dd";
                //Date4.SetValue("2013/11/2");
                hcDate4.Component.Add(Date4);
                this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcDate4);

                HeaderColumn hcTotal1 = new HeaderColumn();
                this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcTotal1);

                HeaderColumn hcTotal3 = new HeaderColumn();
                this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcTotal3);

                var TitleTotalP = new Ext.Net.Label();
                TitleTotalP.Text = "Total(Personal paid)";
                TitleTotalP.Cls = "custom-row";
                HeaderColumn hcTitleTotalP = new HeaderColumn();
                hcTitleTotalP.Component.Add(TitleTotalP);
                this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTitleTotalP);

                HeaderColumn hcTotal2 = new HeaderColumn();
                this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcTotal2);

                HeaderColumn hcTotal4 = new HeaderColumn();
                this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcTotal4);

                var TitleTotalC = new Ext.Net.Label();
                TitleTotalC.Text = "Total(Company)";
                TitleTotalC.Cls = "custom-row";
                HeaderColumn hcTitleTotalC = new HeaderColumn();
                hcTitleTotalC.Component.Add(TitleTotalC);
                this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTitleTotalC);

                X.AddScript("GridPanel2.colModel.setHidden(3, true);GridPanel2.colModel.setHidden(4, true);Store2.removeField('Station_1_P');Store2.removeField('Station_1_C');");
            }
            else
            {
                //string sqld = "select * from ETraveleDetail where [No]='" + ID + "' order by id";
                //DataTable dtall = new DataTable();
                //dtall = dbc.GetData("eReimbursement", sqld);
                for (int i = 0; i < dtall.Rows.Count; i++)
                {
                    if (i % 11 == 0)
                    {
                        string colname = "Station_" + (i / 11).ToString() + "_P";
                        Store2.Reader[0].Fields.Add(colname, RecordFieldType.String);
                        dtnew.Columns.Add(colname, typeof(String));
                        colname = "Station_" + (i / 11).ToString() + "_C";
                        Store2.Reader[0].Fields.Add(colname, RecordFieldType.String);
                        dtnew.Columns.Add(colname, typeof(String));
                    }
                    dtnew.Rows[i % 11][3 + i / 11 * 2] = dtall.Rows[i]["Pamount"].ToString();
                    dtnew.Rows[i % 11][3 + i / 11 * 2 + 1] = dtall.Rows[i]["Camount"].ToString();
                    //列合计
                    string df = dtnew.Rows[11][3 + i / 11 * 2].ToString() == "" ? "0" : dtnew.Rows[11][3 + i / 11 * 2].ToString();
                    string cf = dtall.Rows[i]["Pamount"].ToString() == "" ? "0" : dtall.Rows[i]["Pamount"].ToString();
                    dtnew.Rows[11][3 + i / 11 * 2] = Convert.ToDecimal(df) + Convert.ToDecimal(cf) == 0 ? "" : (Convert.ToDecimal(df) + Convert.ToDecimal(cf)).ToString();

                    string bf = dtnew.Rows[11][3 + i / 11 * 2 + 1].ToString() == "" ? "0" : dtnew.Rows[11][3 + i / 11 * 2 + 1].ToString();
                    string af = dtall.Rows[i]["Camount"].ToString() == "" ? "0" : dtall.Rows[i]["Camount"].ToString();
                    dtnew.Rows[11][3 + i / 11 * 2 + 1] = Convert.ToDecimal(bf) + Convert.ToDecimal(af) == 0 ? "" : (Convert.ToDecimal(bf) + Convert.ToDecimal(af)).ToString();
                    //行合计
                    string df1 = dtnew.Rows[i % 11][1].ToString() == "" ? "0" : dtnew.Rows[i % 11][1].ToString();
                    dtnew.Rows[i % 11][1] = Convert.ToDecimal(df1) + Convert.ToDecimal(cf) == 0 ? "" : (Convert.ToDecimal(df1) + Convert.ToDecimal(cf)).ToString();
                    string bf1 = dtnew.Rows[i % 11][2].ToString() == "" ? "0" : dtnew.Rows[i % 11][2].ToString();
                    dtnew.Rows[i % 11][2] = Convert.ToDecimal(bf1) + Convert.ToDecimal(af) == 0 ? "" : (Convert.ToDecimal(bf1) + Convert.ToDecimal(af)).ToString();
                    //总合计
                    string tf = dtnew.Rows[11][1].ToString() == "" ? "0" : dtnew.Rows[11][1].ToString();
                    dtnew.Rows[11][1] = Convert.ToDecimal(tf) + Convert.ToDecimal(cf) == 0 ? "" : (Convert.ToDecimal(tf) + Convert.ToDecimal(cf)).ToString();
                    string ff = dtnew.Rows[11][2].ToString() == "" ? "0" : dtnew.Rows[11][2].ToString();
                    dtnew.Rows[11][2] = Convert.ToDecimal(ff) + Convert.ToDecimal(af) == 0 ? "" : (Convert.ToDecimal(ff) + Convert.ToDecimal(af)).ToString();
                }
                Store2.Reader[0].Fields.Add("TotalP", RecordFieldType.String);
                Store2.Reader[0].Fields.Add("TotalC", RecordFieldType.String);

                Store2.DataSource = dtnew;
                Store2.DataBind();

                var TitleCol = new Column();
                TitleCol.DataIndex = "Category";
                TitleCol.Sortable = false;
                TitleCol.Resizable = false;
                TitleCol.MenuDisabled = true;
                TitleCol.Width = 180;
                this.GridPanel2.ColumnModel.Columns.Add(TitleCol);

                var Title1 = new Ext.Net.Label();
                Title1.Text = "Destination:";
                HeaderColumn hcTitle1 = new HeaderColumn();
                hcTitle1.Component.Add(Title1);
                this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcTitle1);

                var Title2 = new Ext.Net.Label();
                Title2.Text = "Cost Center:";
                HeaderColumn hcTitle2 = new HeaderColumn();
                hcTitle2.Component.Add(Title2);
                this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcTitle2);

                var Title3 = new Ext.Net.Label();
                Title3.Text = "Travel Period:";
                HeaderColumn hcTitle3 = new HeaderColumn();
                hcTitle3.Component.Add(Title3);
                this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTitle3);

                int colc = dtall.Rows.Count / 11;
                for (int i = 0; i < colc; i++)//准备复制已有信息
                {
                    string fieldPName = "Station_" + i.ToString() + "_P";
                    //RecordField field1 = new RecordField(fieldAName, RecordFieldType.Float);
                    //Store2.Reader[0].Fields.Add(fieldPName, RecordFieldType.Float);
                    //this.Store2.AddField(field1, columncount);
                    string fieldCName = "Station_" + i.ToString() + "_C";
                    //RecordField field1 = new RecordField(fieldAName, RecordFieldType.Float);
                    //Store2.Reader[0].Fields.Add(fieldCName, RecordFieldType.Float);

                    var txtP = new Ext.Net.NumberField();
                    //txtP.Listeners.Blur.Fn = "Cal";
                    var colP = new Column();
                    colP.Header = "Reimbursement";
                    colP.DataIndex = fieldPName;
                    colP.Sortable = false;
                    colP.Resizable = false;
                    colP.MenuDisabled = true;
                    colP.Width = 110;
                    colP.Editor.Add(txtP);
                    this.GridPanel2.ColumnModel.Columns.Add(colP);

                    var txtC = new Ext.Net.NumberField();
                    //txtC.Listeners.Blur.Fn = "Cal";
                    var colC = new Column();
                    colC.Header = "Company Paid";
                    colC.DataIndex = fieldCName;
                    colC.Sortable = false;
                    colC.Resizable = false;
                    colC.MenuDisabled = true;
                    colC.Width = 110;
                    colC.Editor.Add(txtC);
                    this.GridPanel2.ColumnModel.Columns.Add(colC);

                    var Station = new Ext.Net.TextField();
                    Station.Text = dtall.Rows[i * 11]["Tocity"].ToString();
                    HeaderColumn hcStation = new HeaderColumn();
                    hcStation.Component.Add(Station);
                    this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcStation);

                    var Button = new Ext.Net.Button();
                    Button.Text = "Remove";
                    Button.Listeners.Click.Handler = "removecol(this," + i.ToString() + ");";
                    Button.Listeners.Click.Delay = 50;
                    HeaderColumn hcButton = new HeaderColumn();
                    hcButton.Component.Add(Button);
                    this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcButton);

                    var CostCenter = new Ext.Net.TextField();
                    CostCenter.Disabled = true;
                    CostCenter.EmptyText = "Station Code";
                    CostCenter.Text = dtall.Rows[i * 11]["TSation"].ToString();
                    HeaderColumn hcCostCenter = new HeaderColumn();
                    hcCostCenter.Component.Add(CostCenter);
                    this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcCostCenter);

                    //HeaderColumn hcCostCenter1 = new HeaderColumn();
                    //this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcCostCenter1);
                    var ButtonGetSum = new Ext.Net.Button();
                    ButtonGetSum.Text = "Calculate";
                    ButtonGetSum.Listeners.Click.Handler = "GetSum();";
                    ButtonGetSum.Listeners.Click.Delay = 50;
                    HeaderColumn hcButtonGetSum = new HeaderColumn();
                    hcButtonGetSum.Component.Add(ButtonGetSum);
                    this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcButtonGetSum);

                    var datefrom = new DateField();
                    string dtfroms = dtall.Rows[i * 11]["Tdate0"].ToString() == "" ? "" : Convert.ToDateTime(dtall.Rows[i * 11]["Tdate0"].ToString()).ToString("yyyy/MM/dd");
                    datefrom.SetValue(dtfroms);
                    datefrom.EmptyText = "yyyy/MM/dd";
                    datefrom.Format = "yyyy/MM/dd";
                    HeaderColumn Date1 = new HeaderColumn();
                    Date1.Component.Add(datefrom);
                    this.GridPanel2.GetView().HeaderRows[2].Columns.Add(Date1);

                    var dateto = new DateField();
                    string datetos = dtall.Rows[i * 11]["Tdate"].ToString() == "" ? "" : Convert.ToDateTime(dtall.Rows[i * 11]["Tdate"].ToString()).ToString("yyyy/MM/dd");
                    dateto.SetValue(datetos);
                    dateto.EmptyText = "yyyy/MM/dd";
                    dateto.Format = "yyyy/MM/dd";
                    HeaderColumn Date2 = new HeaderColumn();
                    Date2.Component.Add(dateto);
                    this.GridPanel2.GetView().HeaderRows[2].Columns.Add(Date2);
                }

                var TotalP = new Ext.Net.NumberField();
                TotalP.ReadOnly = true;
                TotalP.Cls = "custom-row";
                var colTotalP = new Column();
                colTotalP.DataIndex = "TotalP";
                colTotalP.Sortable = false;
                colTotalP.Resizable = false;
                colTotalP.MenuDisabled = true;
                colTotalP.Width = 110;
                colTotalP.Locked = true;
                colTotalP.Editor.Add(TotalP);
                this.GridPanel2.ColumnModel.Columns.Add(colTotalP);

                var TotalC = new Ext.Net.NumberField();
                TotalC.ReadOnly = true;
                TotalC.Cls = "custom-row";
                var colTotalC = new Column();
                colTotalC.DataIndex = "TotalC";
                colTotalC.Sortable = false;
                colTotalC.Resizable = false;
                colTotalC.MenuDisabled = true;
                colTotalC.Width = 110;
                colTotalC.Locked = true;
                colTotalC.Editor.Add(TotalC);
                this.GridPanel2.ColumnModel.Columns.Add(colTotalC);

                HeaderColumn hcTotal1 = new HeaderColumn();
                this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcTotal1);

                HeaderColumn hcTotal2 = new HeaderColumn();
                this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcTotal2);

                HeaderColumn hcTotal3 = new HeaderColumn();
                this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcTotal3);

                HeaderColumn hcTotal4 = new HeaderColumn();
                this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcTotal4);

                var TitleTotalP = new Ext.Net.Label();
                TitleTotalP.Text = "Total(Personal paid)";
                TitleTotalP.Cls = "custom-row";
                HeaderColumn hcTitleTotalP = new HeaderColumn();
                hcTitleTotalP.Component.Add(TitleTotalP);
                this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTitleTotalP);

                var TitleTotalC = new Ext.Net.Label();
                TitleTotalC.Text = "Total(Company)";
                TitleTotalC.Cls = "custom-row";
                HeaderColumn hcTitleTotalC = new HeaderColumn();
                hcTitleTotalC.Component.Add(TitleTotalC);
                this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTitleTotalC);
            }
            if (CheckCopy && dtall.Rows.Count > 0)
            {
                //如果被复制申请的币种与本人币种不一致,则无法复制保存
                string curOri = dtall.Rows[0]["Cur"].ToString();
                string curSetting = curOri;
                DataTable dttemp = new DataTable();
                string sqltemp = "select * from ESUSER where Userid='" + dt.Rows[0]["PersonID"].ToString() + "'";
                dttemp = dbc.GetData("eReimbursement", sqltemp);
                if (dttemp.Rows.Count > 0)
                {
                    curSetting = dttemp.Rows[0]["Currency"].ToString();//如果单独设置了币种
                    if (curSetting != curOri)
                    {
                        X.AddScript("Ext.Msg.show({ title: 'Warning', msg: 'Currency not match,forbidden to save.', buttons: { ok: 'Ok' }, fn: function (btn) {btnSaveAndSend.disable();btnSaveDraft.disable();} });");
                        //return;
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
                Icon = MessageBox.Icon.WARNING
            });
        }
        protected void UploadTravelReportClick(object sender, DirectEventArgs e)
        {
            //if (this.FileUploadField1.HasFile)
            //{
            //    string lstrFileName = null;     //上传文件路径
            //    string lstrFileFolder = null;   //存放文件路径
            //    string lstrFileNamePath = null; //上传目录及文件名

            //    //获得上传到服务器的目录名称,如果上传目录为空，就使用"D:\"作为缺省上传目录 
            //    //if (dir.Value != "") lstrFileFolder = dir.Value; else lstrFileFolder = "D:\\";
            //    lstrFileFolder = System.Web.HttpContext.Current.Request.MapPath("Upload/");

            //    //lstrFileName = FileUploadField1.PostedFile.FileName;  //获得文件名称
            //    //注:FileUploadField1.PostedFile.FileName 返回的是通过文件对话框选择的文件名，这之中包含了文件的目录信息

            //    lstrFileName = System.IO.Path.GetFileName(FileUploadField1.PostedFile.FileName);  //去掉目录信息，返回文件名称

            //    //判断上传目录是否存在，不存在就建立 
            //    if (!System.IO.Directory.Exists(lstrFileFolder)) System.IO.Directory.CreateDirectory(lstrFileFolder);
            //    string dtime = DateTime.Now.ToString("yyyyMMddHHmmss");
            //    lstrFileNamePath = lstrFileFolder + "TR" + dtime + System.IO.Path.GetExtension(lstrFileName); //得到上传目录及文件名称  
            //    FileUploadField1.PostedFile.SaveAs(lstrFileNamePath);       //上传文件到服务器
            //    string filename = "TR" + dtime + System.IO.Path.GetExtension(lstrFileName);
            //    linkTravelReport.Text = filename; hdReport.Value = filename;
            //    linkTravelReport.NavigateUrl = "./Upload/" + filename;
            //    string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";
            //    //更新数据
            //    if (hdTravelRequestID.Value.ToString() != "")
            //    {
            //        string updatesql = "update ETravel set ReportFile='" + filename + "' where ID=" + hdTravelRequestID.Value.ToString();
            //        cs.DBCommand dbc = new cs.DBCommand();
            //        string update = dbc.UpdateData("eReimbursement", updatesql, "Update");
            //    }
            //    X.Msg.Show(new MessageBoxConfig
            //    {
            //        Buttons = MessageBox.Button.OK,
            //        Icon = MessageBox.Icon.INFO,
            //        Title = "Success",
            //        Message = string.Format(tpl, this.FileUploadField1.PostedFile.FileName, this.FileUploadField1.PostedFile.ContentLength)
            //    });
            //}
            if (this.FileUploadField1.HasFile)
            {
                if (CheckBoxOnBehalfItem.Checked==false&&hdStation.Value.ToString()=="")
                {
                    //不允许上传该文件类型
                    X.AddScript("Ext.Msg.show({ title: 'Warning', msg: 'Please load data from eLeave first.', buttons: { ok: 'Ok' }, fn: function (btn) { return false; } });");
                    return;
                }

                string lstrFileName = null;     //上传文件路径
                string lstrFileFolder = null;   //存放文件路径
                string lstrFileNamePath = null; //上传目录及文件名
                cs.DBCommand dbc = new cs.DBCommand();
                //获得上传到服务器的目录名称,如果上传目录为空，就使用"D:\"作为缺省上传目录 
                //if (dir.Value != "") lstrFileFolder = dir.Value; else lstrFileFolder = "D:\\";
                lstrFileFolder = System.Web.HttpContext.Current.Request.MapPath("Upload/");

                //lstrFileName = FileUploadField1.PostedFile.FileName;  //获得文件名称
                //注:FileUploadField1.PostedFile.FileName 返回的是通过文件对话框选择的文件名，这之中包含了文件的目录信息

                lstrFileName = System.IO.Path.GetFileName(FileUploadField1.PostedFile.FileName);  //去掉目录信息，返回文件名称

                //判断上传目录是否存在，不存在就建立 
                if (!System.IO.Directory.Exists(lstrFileFolder)) System.IO.Directory.CreateDirectory(lstrFileFolder);
                string dtime = DateTime.Now.ToString("yyyyMMddHHmmss");
                lstrFileNamePath = lstrFileFolder + "TR" + dtime + System.IO.Path.GetExtension(lstrFileName); //得到上传目录及文件名称 

                //151124.增加上传附件判断.
                string postfix = Path.GetExtension(lstrFileName);
                string postfixraw = Path.GetExtension(lstrFileName);
                int indexf = postfix.IndexOf('.');
                if (indexf != -1)
                {
                    postfixraw = postfixraw.Substring(1);
                }
                string station = hdStation.Value.ToString();
                string sql = "select * from FileRule where StationCode='" + station + "'";
                DataTable dtrule = dbc.GetData("eReimbursement", sql);
                if (dtrule.Rows.Count < 1)
                {
                    string sqlnew = "select * from FileRule where StationCode='DEFAULT'";
                    DataTable dtnew = dbc.GetData("eReimbursement", sqlnew);

                    if (dtnew.Select("Allow=0 and PostfixRaw='" + postfixraw + "'").Count() == 1)
                    {
                        //不允许上传该文件类型
                        X.AddScript("Ext.Msg.show({ title: 'Warning', msg: 'Not allowed file type.Please contact local MIS.', buttons: { ok: 'Ok' }, fn: function (btn) { return false; } });");
                        return;
                    }
                    else if (dtnew.Select("Allow=1 and PostfixRaw='" + postfixraw + "'").Count() == 1)
                    {
                        //允许上传的文件类型
                        FileUploadField1.PostedFile.SaveAs(lstrFileNamePath);       //上传文件到服务器
                        string filename = "TR" + dtime + System.IO.Path.GetExtension(lstrFileName);

                        linkTravelReport.Text = filename; hdReport.Value = filename;
                        linkTravelReport.NavigateUrl = "./Upload/" + filename;
                        string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";
                        //更新数据
                        if (hdTravelRequestID.Value.ToString() != "")
                        {
                            string updatesql = "update ETravel set Attach='" + filename + "' where ID=" + hdTravelRequestID.Value.ToString();

                            string update = dbc.UpdateData("eReimbursement", updatesql, "Update");
                        }
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Title = "Success",
                            Message = string.Format(tpl, this.FileUploadField1.PostedFile.FileName, this.FileUploadField1.PostedFile.ContentLength)
                        });
                    }
                    else
                    {
                        //未知文件类型
                        FileUploadField1.PostedFile.SaveAs(lstrFileNamePath);       //上传文件到服务器
                        //压缩文件
                        cs.ZipFloClass zip = new cs.ZipFloClass();
                        string filename = "TR" + dtime + postfix;
                        string filepath = lstrFileNamePath;
                        string zipfilepath = lstrFileFolder + "TR" + dtime + ".zip";
                        zip.ZipSingleFile(lstrFileName, filepath, zipfilepath);
                        string newfilename = "TR" + dtime + ".zip";

                        linkTravelReport.Text = newfilename; hdReport.Value = newfilename;
                        linkTravelReport.NavigateUrl = "./Upload/" + newfilename;
                        string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";
                        //更新数据
                        if (hdTravelRequestID.Value.ToString() != "")
                        {
                            string updatesql = "update ETravel set Attach='" + newfilename + "' where ID=" + hdTravelRequestID.Value.ToString();

                            string update = dbc.UpdateData("eReimbursement", updatesql, "Update");
                        }
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Title = "Success",
                            Message = string.Format(tpl, this.FileUploadField1.PostedFile.FileName, this.FileUploadField1.PostedFile.ContentLength)
                        });
                    }
                }
                else
                {
                    if (dtrule.Select("Allow=0 and PostfixRaw='" + postfixraw + "'").Count() == 1)
                    {
                        //不允许上传该文件类型
                        X.AddScript("Ext.Msg.show({ title: 'Warning', msg: 'Not allowed file type.Please contact local MIS.', buttons: { ok: 'Ok' }, fn: function (btn) { return false; } });");
                        return;
                    }
                    else if (dtrule.Select("Allow=1 and PostfixRaw='" + postfixraw + "'").Count() == 1)
                    {
                        //允许上传的文件类型
                        FileUploadField1.PostedFile.SaveAs(lstrFileNamePath);       //上传文件到服务器
                        string filename = "TR" + dtime + System.IO.Path.GetExtension(lstrFileName);

                        linkTravelReport.Text = filename; hdReport.Value = filename;
                        linkTravelReport.NavigateUrl = "./Upload/" + filename;
                        string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";
                        //更新数据
                        if (hdTravelRequestID.Value.ToString() != "")
                        {
                            string updatesql = "update ETravel set Attach='" + filename + "' where ID=" + hdTravelRequestID.Value.ToString();

                            string update = dbc.UpdateData("eReimbursement", updatesql, "Update");
                        }
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Title = "Success",
                            Message = string.Format(tpl, this.FileUploadField1.PostedFile.FileName, this.FileUploadField1.PostedFile.ContentLength)
                        });
                    }
                    else
                    {
                        //未知文件类型
                        FileUploadField1.PostedFile.SaveAs(lstrFileNamePath);       //上传文件到服务器
                        //压缩文件
                        cs.ZipFloClass zip = new cs.ZipFloClass();
                        string filename = "TR" + dtime + postfix;
                        string filepath = lstrFileNamePath;
                        string zipfilepath = lstrFileFolder + "TR" + dtime + ".zip";
                        zip.ZipSingleFile(lstrFileName, filepath, zipfilepath);
                        string newfilename = "TR" + dtime + ".zip";

                        linkTravelReport.Text = newfilename; hdReport.Value = newfilename;
                        linkTravelReport.NavigateUrl = "./Upload/" + newfilename;
                        string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";
                        //更新数据
                        if (hdTravelRequestID.Value.ToString() != "")
                        {
                            string updatesql = "update ETravel set Attach='" + newfilename + "' where ID=" + hdTravelRequestID.Value.ToString();

                            string update = dbc.UpdateData("eReimbursement", updatesql, "Update");
                        }
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Title = "Success",
                            Message = string.Format(tpl, this.FileUploadField1.PostedFile.FileName, this.FileUploadField1.PostedFile.ContentLength)
                        });
                    }
                }
            }
        }
        protected void UploadScanFileClick(object sender, DirectEventArgs e)
        {
            //if (this.FileUploadField2.HasFile)
            //{
            //    string lstrFileName = null;     //上传文件路径
            //    string lstrFileFolder = null;   //存放文件路径
            //    string lstrFileNamePath = null; //上传目录及文件名

            //    //获得上传到服务器的目录名称,如果上传目录为空，就使用"D:\"作为缺省上传目录 
            //    //if (dir.Value != "") lstrFileFolder = dir.Value; else lstrFileFolder = "D:\\";
            //    lstrFileFolder = System.Web.HttpContext.Current.Request.MapPath("Upload/");

            //    //lstrFileName = FileUploadField1.PostedFile.FileName;  //获得文件名称
            //    //注:FileUploadField1.PostedFile.FileName 返回的是通过文件对话框选择的文件名，这之中包含了文件的目录信息

            //    lstrFileName = System.IO.Path.GetFileName(FileUploadField2.PostedFile.FileName);  //去掉目录信息，返回文件名称

            //    //判断上传目录是否存在，不存在就建立 
            //    if (!System.IO.Directory.Exists(lstrFileFolder)) System.IO.Directory.CreateDirectory(lstrFileFolder);
            //    string dtime = DateTime.Now.ToString("yyyyMMddHHmmss");
            //    lstrFileNamePath = lstrFileFolder + "SF" + dtime + System.IO.Path.GetExtension(lstrFileName); //得到上传目录及文件名称  
            //    FileUploadField2.PostedFile.SaveAs(lstrFileNamePath);       //上传文件到服务器
            //    string filename = "SF" + dtime + System.IO.Path.GetExtension(lstrFileName);
            //    linkScanFile.Text = filename; hdScanFile.Value = filename;
            //    linkScanFile.NavigateUrl = "./Upload/" + filename;
            //    string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";
            //    //更新数据
            //    if (hdTravelRequestID.Value.ToString() != "")
            //    {
            //        string updatesql = "update ETravel set Attach='" + filename + "' where ID=" + hdTravelRequestID.Value.ToString();
            //        cs.DBCommand dbc = new cs.DBCommand();
            //        string update = dbc.UpdateData("eReimbursement", updatesql, "Update");
            //    }
            //    X.Msg.Show(new MessageBoxConfig
            //    {
            //        Buttons = MessageBox.Button.OK,
            //        Icon = MessageBox.Icon.INFO,
            //        Title = "Success",
            //        Message = string.Format(tpl, this.FileUploadField2.PostedFile.FileName, this.FileUploadField2.PostedFile.ContentLength)
            //    });
            //}
            if (this.FileUploadField2.HasFile)
            {
                if (CheckBoxOnBehalfItem.Checked == false && hdStation.Value.ToString() == "")
                {
                    //不允许上传该文件类型
                    X.AddScript("Ext.Msg.show({ title: 'Warning', msg: 'Please load data from eLeave first.', buttons: { ok: 'Ok' }, fn: function (btn) { return false; } });");
                    return;
                }
                string lstrFileName = null;     //上传文件路径
                string lstrFileFolder = null;   //存放文件路径
                string lstrFileNamePath = null; //上传目录及文件名
                cs.DBCommand dbc = new cs.DBCommand();
                //获得上传到服务器的目录名称,如果上传目录为空，就使用"D:\"作为缺省上传目录 
                //if (dir.Value != "") lstrFileFolder = dir.Value; else lstrFileFolder = "D:\\";
                lstrFileFolder = System.Web.HttpContext.Current.Request.MapPath("Upload/");

                //lstrFileName = FileUploadField1.PostedFile.FileName;  //获得文件名称
                //注:FileUploadField1.PostedFile.FileName 返回的是通过文件对话框选择的文件名，这之中包含了文件的目录信息

                lstrFileName = System.IO.Path.GetFileName(FileUploadField2.PostedFile.FileName);  //去掉目录信息，返回文件名称

                //判断上传目录是否存在，不存在就建立 
                if (!System.IO.Directory.Exists(lstrFileFolder)) System.IO.Directory.CreateDirectory(lstrFileFolder);
                string dtime = DateTime.Now.ToString("yyyyMMddHHmmss");
                lstrFileNamePath = lstrFileFolder + "SF" + dtime + System.IO.Path.GetExtension(lstrFileName); //得到上传目录及文件名称 

                //151124.增加上传附件判断.
                string postfix = Path.GetExtension(lstrFileName);
                string postfixraw = Path.GetExtension(lstrFileName);
                int indexf = postfix.IndexOf('.');
                if (indexf != -1)
                {
                    postfixraw = postfixraw.Substring(1);
                }
                string station = hdStation.Value.ToString();
                string sql = "select * from FileRule where StationCode='" + station + "'";
                DataTable dtrule = dbc.GetData("eReimbursement", sql);
                if (dtrule.Rows.Count < 1)
                {
                    string sqlnew = "select * from FileRule where StationCode='DEFAULT'";
                    DataTable dtnew = dbc.GetData("eReimbursement", sqlnew);

                    if (dtnew.Select("Allow=0 and PostfixRaw='" + postfixraw + "'").Count() == 1)
                    {
                        //不允许上传该文件类型
                        X.AddScript("Ext.Msg.show({ title: 'Warning', msg: 'Not allowed file type.Please contact local MIS.', buttons: { ok: 'Ok' }, fn: function (btn) { return false; } });");
                        return;
                    }
                    else if (dtnew.Select("Allow=1 and PostfixRaw='" + postfixraw + "'").Count() == 1)
                    {
                        //允许上传的文件类型
                        FileUploadField2.PostedFile.SaveAs(lstrFileNamePath);       //上传文件到服务器
                        string filename = "SF" + dtime + System.IO.Path.GetExtension(lstrFileName);

                        linkScanFile.Text = filename; hdScanFile.Value = filename;
                        linkScanFile.NavigateUrl = "./Upload/" + filename;
                        string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";
                        //更新数据
                        if (hdTravelRequestID.Value.ToString() != "")
                        {
                            string updatesql = "update ETravel set Attach='" + filename + "' where ID=" + hdTravelRequestID.Value.ToString();

                            string update = dbc.UpdateData("eReimbursement", updatesql, "Update");
                        }
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Title = "Success",
                            Message = string.Format(tpl, this.FileUploadField2.PostedFile.FileName, this.FileUploadField2.PostedFile.ContentLength)
                        });
                    }
                    else
                    {
                        //未知文件类型
                        FileUploadField2.PostedFile.SaveAs(lstrFileNamePath);       //上传文件到服务器
                        //压缩文件
                        cs.ZipFloClass zip = new cs.ZipFloClass();
                        string filename = "SF" + dtime + postfix;
                        string filepath = lstrFileNamePath;
                        string zipfilepath = lstrFileFolder + "SF" + dtime + ".zip";
                        zip.ZipSingleFile(lstrFileName, filepath, zipfilepath);
                        string newfilename = "SF" + dtime + ".zip";

                        linkScanFile.Text = newfilename; hdScanFile.Value = newfilename;
                        linkScanFile.NavigateUrl = "./Upload/" + newfilename;
                        string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";
                        //更新数据
                        if (hdTravelRequestID.Value.ToString() != "")
                        {
                            string updatesql = "update ETravel set Attach='" + newfilename + "' where ID=" + hdTravelRequestID.Value.ToString();

                            string update = dbc.UpdateData("eReimbursement", updatesql, "Update");
                        }
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Title = "Success",
                            Message = string.Format(tpl, this.FileUploadField2.PostedFile.FileName, this.FileUploadField2.PostedFile.ContentLength)
                        });
                    }
                }
                else
                {
                    if (dtrule.Select("Allow=0 and PostfixRaw='" + postfixraw + "'").Count() == 1)
                    {
                        //不允许上传该文件类型
                        X.AddScript("Ext.Msg.show({ title: 'Warning', msg: 'Not allowed file type.Please contact local MIS.', buttons: { ok: 'Ok' }, fn: function (btn) { return false; } });");
                        return;
                    }
                    else if (dtrule.Select("Allow=1 and PostfixRaw='" + postfixraw + "'").Count() == 1)
                    {
                        //允许上传的文件类型
                        FileUploadField2.PostedFile.SaveAs(lstrFileNamePath);       //上传文件到服务器
                        string filename = "SF" + dtime + System.IO.Path.GetExtension(lstrFileName);

                        linkScanFile.Text = filename; hdScanFile.Value = filename;
                        linkScanFile.NavigateUrl = "./Upload/" + filename;
                        string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";
                        //更新数据
                        if (hdTravelRequestID.Value.ToString() != "")
                        {
                            string updatesql = "update ETravel set Attach='" + filename + "' where ID=" + hdTravelRequestID.Value.ToString();

                            string update = dbc.UpdateData("eReimbursement", updatesql, "Update");
                        }
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Title = "Success",
                            Message = string.Format(tpl, this.FileUploadField2.PostedFile.FileName, this.FileUploadField2.PostedFile.ContentLength)
                        });
                    }
                    else
                    {
                        //未知文件类型
                        FileUploadField2.PostedFile.SaveAs(lstrFileNamePath);       //上传文件到服务器
                        //压缩文件
                        cs.ZipFloClass zip = new cs.ZipFloClass();
                        string filename = "SF" + dtime + postfix;
                        string filepath = lstrFileNamePath;
                        string zipfilepath = lstrFileFolder + "SF" + dtime + ".zip";
                        zip.ZipSingleFile(lstrFileName, filepath, zipfilepath);
                        string newfilename = "SF" + dtime + ".zip";

                        linkScanFile.Text = newfilename; hdScanFile.Value = newfilename;
                        linkScanFile.NavigateUrl = "./Upload/" + newfilename;
                        string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";
                        //更新数据
                        if (hdTravelRequestID.Value.ToString() != "")
                        {
                            string updatesql = "update ETravel set Attach='" + newfilename + "' where ID=" + hdTravelRequestID.Value.ToString();

                            string update = dbc.UpdateData("eReimbursement", updatesql, "Update");
                        }
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Title = "Success",
                            Message = string.Format(tpl, this.FileUploadField2.PostedFile.FileName, this.FileUploadField2.PostedFile.ContentLength)
                        });
                    }
                }
            }
        }
        [Serializable]
        public class CCMailList
        {
            [XmlElement("Email")]
            public string Email { get; set; }
        }
        protected void UpdateMSG(string msg)
        {
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Message",
                Message = msg,
                Buttons = MessageBox.Button.OK,
                Width = 350,
                Icon = MessageBox.Icon.INFO
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
        [DirectMethod]
        public void GetDataFromeLeave()
        {
            DataTable dtnew = new DataTable();
            dtnew.Columns.Add("Owner", System.Type.GetType("System.String"));
            dtnew.Columns.Add("OwnerID", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Station", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Department", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Currency", System.Type.GetType("System.String"));//记录本人币种
            dtnew.Columns.Add("CurrencyBudget", System.Type.GetType("System.String"));//记录预算站点币种
            dtnew.Columns.Add("leaveStart", System.Type.GetType("System.String"));
            dtnew.Columns.Add("leaveStart1", System.Type.GetType("System.String"));
            dtnew.Columns.Add("leaveEnd", System.Type.GetType("System.String"));
            dtnew.Columns.Add("leaveEnd1", System.Type.GetType("System.String"));
            dtnew.Columns.Add("leaveCount", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Destination", System.Type.GetType("System.String"));
            dtnew.Columns.Add("CostCenter", System.Type.GetType("System.String"));
            //dtnew.Columns.Add("RateBudget", System.Type.GetType("System.String"));

            cs.DBCommand dbc = new cs.DBCommand();
            //添加本人eLeave信息
            DataSet ds = DIMERCO.SDK.Utilities.LSDK.GetBizTripByUserID(Request.Cookies.Get("eReimUserID").Value);
            if (ds != null && ds.Tables.Count == 1)
            {
                DataTable dteLeave = ds.Tables[0];
                for (int i = 0; i < dteLeave.Rows.Count; i++)
                {
                    DataRow dr = dtnew.NewRow();
                    dr["Owner"] = Request.Cookies.Get("eReimUserName").Value;
                    dr["OwnerID"] = Request.Cookies.Get("eReimUserID").Value;
                    DataSet dsuserinfo = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(Request.Cookies.Get("eReimUserID").Value);
                    DataTable dtuserinfo = dsuserinfo.Tables[0];
                    dr["Station"] = Request.Cookies.Get("eReimStation").Value;
                    dr["Department"] = Request.Cookies.Get("eReimDepartment").Value;
                    //获取该人币种
                    //string LocalCurrency = "";
                    dr["Currency"] = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(Request.Cookies.Get("eReimStation").Value);
                    //LocalCurrency = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(Request.Cookies.Get("eReimStation").Value);
                    //检查是否本地维护过特殊币种
                    DataTable dttemp = new DataTable();
                    string sqltemp = "select * from ESUSER where Userid='" + Request.Cookies.Get("eReimUserID").Value + "'";
                    dttemp = dbc.GetData("eReimbursement", sqltemp);
                    if (dttemp.Rows.Count > 0)
                    {
                        dr["Currency"] = dttemp.Rows[0]["Currency"].ToString();//如果单独设置了币种
                        //LocalCurrency = dttemp.Rows[0]["Currency"].ToString();//如果单独设置了币种
                    }
                    //获取预算站点币种
                    //string BudgetCurrency = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(Request.Cookies.Get("eReimCostCenter").Value);
                    dr["CurrencyBudget"] = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(Request.Cookies.Get("eReimCostCenter").Value);
                    dr["CostCenter"] = Request.Cookies.Get("eReimCostCenter").Value;

                    if (dteLeave.Rows[i]["leaveEnd"].ToString() != "" && dteLeave.Rows[i]["leaveStart"].ToString() != "")
                    {
                        TimeSpan ts = Convert.ToDateTime(dteLeave.Rows[i]["leaveEnd"].ToString()) - Convert.ToDateTime(dteLeave.Rows[i]["leaveStart"].ToString());
                        dr["leaveCount"] = (ts.Days + 1).ToString() + " Days " + ts.Hours.ToString() + " Hours";
                    }
                    dr["leaveStart"] = dteLeave.Rows[i]["leaveStart"].ToString() == "" ? "" : (Convert.ToDateTime(dteLeave.Rows[i]["leaveStart"].ToString()).ToString("yyyy/MM/dd") + " " + Convert.ToDateTime(dteLeave.Rows[i]["leaveStart"].ToString()).DayOfWeek.ToString());
                    dr["leaveEnd"] = dteLeave.Rows[i]["leaveEnd"].ToString() == "" ? "" : (Convert.ToDateTime(dteLeave.Rows[i]["leaveEnd"].ToString()).ToString("yyyy/MM/dd") + " " + Convert.ToDateTime(dteLeave.Rows[i]["leaveEnd"].ToString()).DayOfWeek.ToString());
                    dr["Destination"] = dteLeave.Rows[i]["Destination"].ToString();
                    dr["leaveStart1"] = dteLeave.Rows[i]["leaveStart"].ToString() == "" ? "" : (Convert.ToDateTime(dteLeave.Rows[i]["leaveStart"].ToString()).ToString("yyyy/MM/dd"));
                    dr["leaveEnd1"] = dteLeave.Rows[i]["leaveEnd"].ToString() == "" ? "" : (Convert.ToDateTime(dteLeave.Rows[i]["leaveEnd"].ToString()).ToString("yyyy/MM/dd"));
                    //如果2个币种不同,取得转换汇率,Budget:Local
                    //if (LocalCurrency != BudgetCurrency)
                    //{
                    //    dr["Rate"] = DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(BudgetCurrency, LocalCurrency, 2014);
                    //}
                    //dr["Rate"] = System.Math.Round(1 / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(dtuserinfo.Rows[0]["CostCenter"].ToString()), 3).ToString();
                    
                    dtnew.Rows.Add(dr);
                }
            }

            string sqlitem = "select * from Eagent where [St]=1 and [PAgentID]='" + Request.Cookies.Get("eReimUserID").Value + "' and Edate>=getdate() and Bdate<=getdate()";
            DataTable dtitem = dbc.GetData("eReimbursement", sqlitem);
            for (int j = 0; j < dtitem.Rows.Count; j++)
            {
                DataSet dsAgent = DIMERCO.SDK.Utilities.LSDK.GetBizTripByUserID(dtitem.Rows[j][2].ToString());
                if (dsAgent != null && dsAgent.Tables.Count == 1)
                {
                    DataTable dteLeaveAgent = dsAgent.Tables[0];
                    for (int i = 0; i < dteLeaveAgent.Rows.Count; i++)
                    {
                        DataRow dr = dtnew.NewRow();
                        dr["Owner"] = dtitem.Rows[j][1].ToString();
                        dr["OwnerID"] = dtitem.Rows[j][2].ToString();
                        DataSet dsuserinfo = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtitem.Rows[j][2].ToString());
                        DataTable dtuserinfo = dsuserinfo.Tables[0];
                        dr["Station"] = dtuserinfo.Rows[0]["stationCode"].ToString();
                        dr["Department"] = dtuserinfo.Rows[0]["CRPDepartmentName"].ToString();
                        //获取该人币种
                        dr["Currency"] = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dtuserinfo.Rows[0]["stationCode"].ToString());

                        DataTable dttemp = new DataTable();
                        string sqltemp = "select * from ESUSER where Userid='" + dtitem.Rows[j][2].ToString() + "'";
                        dttemp = dbc.GetData("eReimbursement", sqltemp);
                        if (dttemp.Rows.Count > 0)
                        {
                            dr["Currency"] = dttemp.Rows[0]["Currency"].ToString();//如果单独设置了币种
                        }
                        dr["CurrencyBudget"] = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dtuserinfo.Rows[0]["CostCenter"].ToString());
                        dr["CostCenter"] = dtuserinfo.Rows[0]["CostCenter"].ToString();

                        if (dteLeaveAgent.Rows[i]["leaveEnd"].ToString() != "" && dteLeaveAgent.Rows[i]["leaveStart"].ToString() != "")
                        {
                            TimeSpan ts = Convert.ToDateTime(dteLeaveAgent.Rows[i]["leaveEnd"].ToString()) - Convert.ToDateTime(dteLeaveAgent.Rows[i]["leaveStart"].ToString());
                            dr["leaveCount"] = (ts.Days + 1).ToString() + " Days " + ts.Hours.ToString() + " Hours";
                        }
                        dr["leaveStart"] = dteLeaveAgent.Rows[i]["leaveStart"].ToString() == "" ? "" : (Convert.ToDateTime(dteLeaveAgent.Rows[i]["leaveStart"].ToString()).ToString("yyyy/MM/dd") + " " + Convert.ToDateTime(dteLeaveAgent.Rows[i]["leaveStart"].ToString()).DayOfWeek.ToString());
                        dr["leaveEnd"] = dteLeaveAgent.Rows[i]["leaveEnd"].ToString() == "" ? "" : (Convert.ToDateTime(dteLeaveAgent.Rows[i]["leaveEnd"].ToString()).ToString("yyyy/MM/dd") + " " + Convert.ToDateTime(dteLeaveAgent.Rows[i]["leaveEnd"].ToString()).DayOfWeek.ToString());
                        dr["Destination"] = dteLeaveAgent.Rows[i]["Destination"].ToString();
                        dr["leaveStart1"] = dteLeaveAgent.Rows[i]["leaveStart"].ToString() == "" ? "" : (Convert.ToDateTime(dteLeaveAgent.Rows[i]["leaveStart"].ToString()).ToString("yyyy/MM/dd"));
                        dr["leaveEnd1"] = dteLeaveAgent.Rows[i]["leaveEnd"].ToString() == "" ? "" : (Convert.ToDateTime(dteLeaveAgent.Rows[i]["leaveEnd"].ToString()).ToString("yyyy/MM/dd"));

                        //dr["Rate"] = System.Math.Round(1 / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(dtuserinfo.Rows[0]["CostCenter"].ToString()), 3).ToString();
                        dtnew.Rows.Add(dr);
                    }
                }
            }
            System.Data.DataView dv = dtnew.DefaultView;
            dv.Sort = "leaveStart desc";
            Store1.DataSource = dv.ToTable();
            Store1.DataBind();
        }
        protected bool SendMailNew(DataTable dtPar)
        {
            cs.DBCommand dbc = new cs.DBCommand();
            string sql = "select * from V_Eflow_ETravel where [Type]='T' and Step!=0 and RequestID=" + hdTravelRequestID.Value.ToString() + " order by Step,FlowID";
            DataTable dtMail = new DataTable();
            dtMail = dbc.GetData("eReimbursement", sql);

            if (dtMail != null && dtMail.Rows.Count > 0)
            {
                string msg = "";
                if (dtMail.Rows[0]["FlowFn"].ToString().ToLower() == "verifier")
                {
                    msg = "Process Checking.";
                }
                else if (dtMail.Rows[0]["FlowFn"].ToString().ToLower() == "issuer")
                {
                    msg = "Process Paying.";
                }
                else
                {
                    msg = "Seek For Your Approval.";
                }
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
                DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();
                mail.Title = "Dimerco eReimbursement " + budget + " " + dtMail.Rows[0]["Person"].ToString() + " - " + msg;
                mail.FromDispName = "eReimbursement";
                mail.From = "DIC2@dimerco.com";

                string mailto = "";
                DataSet dsTo = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtMail.Rows[0]["ApproverID"].ToString());
                //if (dsTo.Tables[0].Rows.Count == 1)
                //{
                //    mailto += dsTo.Tables[0].Rows[0]["eMail"].ToString() + ",";
                //}
                if (dsTo != null && dsTo.Tables.Count >= 1 && dsTo.Tables[0].Rows.Count == 1)
                {
                    mailto += dsTo.Tables[0].Rows[0]["eMail"].ToString() + ",";
                }

                string mailcc = "";
                DataSet dsCC = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtMail.Rows[0]["PersonID"].ToString());
                if (dsCC != null && dsCC.Tables.Count >= 1 && dsCC.Tables[0].Rows.Count == 1)
                {
                    mailcc += dsCC.Tables[0].Rows[0]["eMail"].ToString() + ",";
                }
                //else
                //{
                //    ErrorHandle("Error mail address."); return false;
                //}
                //if (dtMail.Rows[0]["CreadedByID"].ToString() != "" && dtMail.Rows[0]["CreadedByID"].ToString() != dtMail.Rows[0]["PersonID"].ToString())//代理人
                //{
                //    dsCC = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtMail.Rows[0]["CreadedByID"].ToString());
                //    mailcc += dsCC.Tables[0].Rows[0]["eMail"].ToString() + ",";
                //}
                //if (dtMail.Rows[0]["CCMailList"].ToString() != "")
                //{
                //    mailcc += dtMail.Rows[0]["CCMailList"].ToString();
                //}
                if (dtMail.Rows[0]["CreadedByID"].ToString() != "" && dtMail.Rows[0]["CreadedByID"].ToString() != dtMail.Rows[0]["PersonID"].ToString())//代理人
                {
                    dsCC = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtMail.Rows[0]["CreadedByID"].ToString());
                    //mailcc += dsCC.Tables[0].Rows[0]["eMail"].ToString() + ",";
                    string mcc = dsCC.Tables[0].Rows[0]["eMail"].ToString().Trim();
                    if (mcc != "" && mailcc.ToLower().IndexOf(mcc) == -1)
                    {
                        mailcc += mcc + ",";
                    }
                }
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



                //160127 Shanshan提出邮件测试
                string mailtestword = "";
                mailtestword += "<br />Mail to: " + mailto + "<br />";
                mailtestword += "Mail CC: " + mailcc + "<br />";
                DataSet dsowner = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtMail.Rows[0]["PersonID"].ToString());
                if (dsowner != null && dsowner.Tables.Count >= 1 && dsowner.Tables[0].Rows.Count == 1)
                {
                    mail.To = dsowner.Tables[0].Rows[0]["eMail"].ToString();
                }
                //mail.To = mailto;
                //mail.Cc = mailcc;



                string divstyle = "style='font-size:small;'";
                string divstyleCurrent = "style='font-size:small;color:blue;'";
                string divstyleReject = "style='font-size:small;color:red;'";
                string divstylered = "style='border:silver 1px ridge; font-size:small;background-color: #FFFFFF;empty-cells:show;color:red;' width='110px' align='right'";
                string tdstyle = "style='border:silver 1px ridge; font-size:small;background-color: #FFFFFF;empty-cells:show;' width='110px' align='right'";
                StringBuilder sb = new StringBuilder();

                //160127 Shanshan提出邮件测试
                sb.Append("<div " + divstyleReject + ">THIS IS A TEST MAIL." + mailtestword + "</div><br />");
                sb.Append("<div>");



                sb.Append("<div " + divstyle + "> Dear " + dtMail.Rows[0]["Approver"].ToString() + ",</div><br />");
                //sb.Append("<div " + divstyle + ">The following eReimbursement application:" + msg + "</div><br /><br />");
                //160115 垫付
                if (hdOnBehalf.Value == null || hdOnBehalf.Value.ToString() == "")
                {
                    sb.Append("<div " + divstyle + ">The following eReimbursement application: " + msg + "</div><br /><br />");
                }
                else
                {
                    sb.Append("<div " + divstyle + ">The following eReimbursement application on behalf of " + dtMail.Rows[0]["OnBehalfPersonUnit"].ToString() + " " + dtMail.Rows[0]["OnBehalfPersonName"].ToString() + ": " + msg + "</div><br /><br />");
                }


                sb.Append("<div " + divstyle + ">No#:" + dtMail.Rows[0]["No"].ToString() + budget + "</div>");
                //if (dtMail.Rows[0]["Budget"].ToString() == "1")
                //{
                //    sb.Append("<div " + divstyle + ">No#:" + dtMail.Rows[0]["No"].ToString() + budget + "</div>");
                //}
                //else
                //{
                //    sb.Append("<div " + divstyle + "><div style='float:left'>No#:" + dtMail.Rows[0]["No"].ToString() + "</div><div style='color: Red;float:left'>" + budget + "</div></div>");
                //}
                sb.Append("<div " + divstyle + ">Applicant:" + dtMail.Rows[0]["Person"].ToString() + "</div>");
                sb.Append("<div " + divstyle + ">Unit:" + dtMail.Rows[0]["Station"].ToString() + "</div>");
                sb.Append("<div " + divstyle + ">Department:" + dtMail.Rows[0]["Department"].ToString() + "</div>");
                //string period = "";
                string sqldetail2 = "select min(convert(varchar(10),Tdate0,111)) as Tdate0,max(convert(varchar(10),Tdate,111)) as Tdate1 from ETraveleDetail where [No]=" + hdTravelRequestID.Value.ToString() + "";
                DataTable dtder = dbc.GetData("eReimbursement", sqldetail2);
                if (dtder.Rows.Count == 1)
                {
                    sb.Append("<div " + divstyle + ">Period:From " + Convert.ToDateTime(dtder.Rows[0]["Tdate0"].ToString()).ToString("yyyy/MM/dd") + " To " + Convert.ToDateTime(dtder.Rows[0]["Tdate1"].ToString()).ToString("yyyy/MM/dd") + "</div>");
                }
                string sqlcity = "select Tocity+'/' from (select distinct Tocity from ETraveleDetail where No='" + hdTravelRequestID.Value.ToString() + "') as t for xml path('')";
                DataTable dtcity = dbc.GetData("eReimbursement", sqlcity);
                string city = dtcity.Rows[0][0].ToString();
                sb.Append("<div " + divstyle + ">Destination:" + city.Substring(0, city.Length - 1) + "</div>");
                sb.Append("<br />");
                //return true;
                //160115 垫付
                if (hdOnBehalf.Value == null || hdOnBehalf.Value.ToString() == "")
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
                                dr["Currency"] = dtPar.Rows[i]["Currency"].ToString();
                                dr["Current"] = dtPar.Rows[i]["Current"];
                                dr["PA"] = dtPar.Rows[i]["PA"];
                                dr["CA"] = dtPar.Rows[i]["CA"];
                                dr["PU"] = dtPar.Rows[i]["PU"];
                                dr["PB"] = dtPar.Rows[i]["PB"];
                                //dr["PPercent"] = dtPar.Rows[i]["PPercent"].ToString() + "%";
                                decimal per = System.Math.Round(100 * (Convert.ToDecimal(dtPar.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtPar.Rows[i]["PU"].ToString())) / Convert.ToDecimal(dtPar.Rows[i]["PB"].ToString()), 2);
                                dr["PPercent"] = per.ToString() + "%";
                                dtPerson.Rows.Add(dr);
                            }
                            if (Convert.ToDecimal(dtPar.Rows[i]["PB"].ToString()) == 0 && Convert.ToDecimal(dtPar.Rows[i]["DB"].ToString()) != 0)//按照部门分配了预算
                            {
                                DataRow dr = dtDepartment.NewRow();
                                dr["EName"] = dtPar.Rows[i]["EName"].ToString();
                                dr["Currency"] = dtPar.Rows[i]["Currency"].ToString();
                                dr["Current"] = dtPar.Rows[i]["Current"];
                                dr["PA"] = dtPar.Rows[i]["PA"];
                                dr["CA"] = dtPar.Rows[i]["CA"];
                                dr["DU"] = dtPar.Rows[i]["DU"];
                                dr["DB"] = dtPar.Rows[i]["DB"];
                                //dr["DPercent"] = dtPar.Rows[i]["DPercent"].ToString() + "%";
                                decimal per = System.Math.Round(100 * (Convert.ToDecimal(dtPar.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtPar.Rows[i]["DU"].ToString())) / Convert.ToDecimal(dtPar.Rows[i]["DB"].ToString()), 2);
                                dr["DPercent"] = per.ToString() + "%";
                                dtDepartment.Rows.Add(dr);
                            }
                            if (Convert.ToDecimal(dtPar.Rows[i]["PB"].ToString()) == 0 && Convert.ToDecimal(dtPar.Rows[i]["DB"].ToString()) == 0 && Convert.ToDecimal(dtPar.Rows[i]["SB"].ToString()) != 0)//按照站点分配了预算
                            {
                                DataRow dr = dtStation.NewRow();
                                dr["EName"] = dtPar.Rows[i]["EName"].ToString();
                                dr["Currency"] = dtPar.Rows[i]["Currency"].ToString();
                                dr["Current"] = dtPar.Rows[i]["Current"];
                                dr["PA"] = dtPar.Rows[i]["PA"];
                                dr["CA"] = dtPar.Rows[i]["CA"];
                                dr["SU"] = dtPar.Rows[i]["SU"];
                                dr["SB"] = dtPar.Rows[i]["SB"];
                                //dr["SPercent"] = dtPar.Rows[i]["SPercent"].ToString() + "%";
                                decimal per = System.Math.Round(100 * (Convert.ToDecimal(dtPar.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtPar.Rows[i]["SU"].ToString())) / Convert.ToDecimal(dtPar.Rows[i]["SB"].ToString()), 2);
                                dr["SPercent"] = per.ToString() + "%";
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
                        drBudget["Currency"] = dtPar.Rows[0]["Currency"].ToString();
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
                            dr["Currency"] = dtPar.Rows[i]["Currency"].ToString();
                            dr["Current"] = string.Format("{0:N2}", dtPar.Rows[i]["Current"]);
                            dr["PA"] = string.Format("{0:N2}", dtPar.Rows[i]["PA"]);
                            dr["CA"] = string.Format("{0:N2}", dtPar.Rows[i]["CA"]);
                            dr["PU"] = string.Format("{0:N2}", dtPar.Rows[i]["PU"]);
                            if (Convert.ToDecimal(dtPar.Rows[i]["PB"].ToString()) != 0)
                            {
                                dr["PB"] = string.Format("{0:N2}", dtPar.Rows[i]["PB"]);
                                if (i != dtPar.Rows.Count - 1)
                                {
                                    dr["PPercent"] = string.Format("{0:P2}", (Convert.ToDecimal(dtPar.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtPar.Rows[i]["PU"].ToString())) / Convert.ToDecimal(dtPar.Rows[i]["PB"].ToString()));
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
                                    dr["DPercent"] = string.Format("{0:P2}", (Convert.ToDecimal(dtPar.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtPar.Rows[i]["DU"].ToString())) / Convert.ToDecimal(dtPar.Rows[i]["DB"].ToString()));
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
                                    dr["SPercent"] = string.Format("{0:P2}", (Convert.ToDecimal(dtPar.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtPar.Rows[i]["SU"].ToString())) / Convert.ToDecimal(dtPar.Rows[i]["SB"].ToString()));
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
                
                sb.Append("<div " + divstyle + ">Apply Remark:" + dtMail.Rows[0]["Remark"].ToString() + "</div><br />");

                StringBuilder sb1 = new StringBuilder();
                sb1.Append("<div><span " + divstyle + ">Approval Flow:</span>");
                for (int i = 0; i < dtMail.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        sb1.Append("<div " + divstyleCurrent + ">" + (i + 1).ToString());
                    }
                    else
                    {
                        sb1.Append("<div " + divstyle + ">" + (i + 1).ToString());
                    }

                    string msg1 = "";
                    if (dtMail.Rows[i]["FlowFn"].ToString().ToLower() == "verifier")
                    {
                        msg1 = ". To Be Verified: " + dtMail.Rows[i]["Approver"].ToString() + "</div>";
                    }
                    else if (dtMail.Rows[i]["FlowFn"].ToString().ToLower() == "issuer")
                    {
                        msg1 = ". To Be Issued: " + dtMail.Rows[i]["Approver"].ToString() + "</div>";
                    }
                    else
                    {
                        msg1 = ". Waiting for Approval: " + dtMail.Rows[i]["Approver"].ToString() + "</div>";
                    }
                    sb1.Append(msg1);
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
                sb.Append("<div><a href=\"" + url + "?FlowID=" + dtMail.Rows[0]["FlowID"].ToString() + "\" style=\"color: #0000FF\">Click here to visit Dimerco eReimbursement.</a></div>");
                sb.Append("</div>");
                mail.Body = sb.ToString();
                mail.Send();
            }
            else
            {
                return false;
            }
            return true;
        }
        protected void GetOnBehalfUserData(object sender, DirectEventArgs e)
        {
            DataSet getCostCenterBYStationCode = DIMERCO.SDK.Utilities.LSDK.getUserDataBYUserName(X.GetValue("cbxOnBehalfName"), 10);
            DataTable dtCOACenter = (DataTable)getCostCenterBYStationCode.Tables[0];
            DataTable dtCOACenternew = new DataTable();
            dtCOACenternew.Columns.Add("Name", System.Type.GetType("System.String"));
            dtCOACenternew.Columns.Add("UserID", System.Type.GetType("System.String"));
            dtCOACenternew.Columns.Add("CostCenter", System.Type.GetType("System.String"));
            dtCOACenternew.Columns.Add("Department", System.Type.GetType("System.String"));
            dtCOACenternew.Columns.Add("Unit", System.Type.GetType("System.String"));
            for (int c = 0; c < dtCOACenter.Rows.Count; c++)
            {
                DataRow dr = dtCOACenternew.NewRow();
                dr["Name"] = dtCOACenter.Rows[c][1].ToString();
                dr["UserID"] = dtCOACenter.Rows[c][0].ToString();
                dr["Unit"] = dtCOACenter.Rows[c][2].ToString();
                DataSet ds1 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtCOACenter.Rows[c][0].ToString());
                if (ds1.Tables[0].Rows.Count == 1)
                {
                    DataTable dt1 = ds1.Tables[0];
                    dr["Department"] = dt1.Rows[0]["CRPDepartmentName"].ToString();
                    dr["CostCenter"] = dt1.Rows[0]["CostCenter"].ToString();
                }
                dtCOACenternew.Rows.Add(dr);
            }
            Store4.DataSource = dtCOACenternew;
            Store4.DataBind();
        }
        protected bool SendMail(string warningmsg)
        {
            //发送提醒邮件
            cs.DBCommand dbc = new cs.DBCommand();
            string sql = "select * from V_Eflow_ETravel where [Type]='T' and Step!=0 and RequestID=" + hdTravelRequestID.Value.ToString() + " order by Step,FlowID";
            DataTable dtMail = new DataTable();
            dtMail = dbc.GetData("eReimbursement", sql);

            if (dtMail != null && dtMail.Rows.Count > 0)
            {
                string msg = "";
                if (dtMail.Rows[0]["FlowFn"].ToString().ToLower() == "verifier")
                {
                    msg = "Process Checking.";
                }
                else if (dtMail.Rows[0]["FlowFn"].ToString().ToLower() == "issuer")
                {
                    msg = "Process Paying.";
                }
                else
                {
                    msg = "Seek For Your Approval.";
                }
                string budget = dtMail.Rows[0]["Budget"].ToString() == "1" ? "(Budget)" : "(UnBudget)";

                DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();
                mail.Title = "Dimerco eReimbursement " + budget + " " + dtMail.Rows[0]["Person"].ToString() + " - " + msg;
                mail.FromDispName = "eReimbursement";
                mail.From = "DIC2@dimerco.com";

                string mailto = "";
                DataSet dsTo = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtMail.Rows[0]["ApproverID"].ToString());
                if (dsTo.Tables[0].Rows.Count == 1)
                {
                    mailto += dsTo.Tables[0].Rows[0]["eMail"].ToString() + ",";
                }
                else
                {
                    ErrorHandle("Error mail address."); return false;
                }

                string mailcc = "";
                DataSet dsCC = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtMail.Rows[0]["PersonID"].ToString());
                if (dsCC.Tables[0].Rows.Count == 1)
                {
                    mailcc += dsCC.Tables[0].Rows[0]["eMail"].ToString() + ",";
                }
                else
                {
                    ErrorHandle("Error mail address."); return false;
                }
                if (dtMail.Rows[0]["CreadedByID"].ToString() != "" && dtMail.Rows[0]["CreadedByID"].ToString() != dtMail.Rows[0]["PersonID"].ToString())//代理人
                {
                    dsCC = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtMail.Rows[0]["CreadedByID"].ToString());
                    mailcc += dsCC.Tables[0].Rows[0]["eMail"].ToString() + ",";
                }
                if (dtMail.Rows[0]["CCMailList"].ToString() != "")
                {
                    mailcc += dtMail.Rows[0]["CCMailList"].ToString();
                }
                mail.To = mailto;
                mail.Cc = mailcc;
                string divstyle = "style='font-size:small;'";
                string divstyleCurrent = "style='font-size:small;color:blue;'";
                string divstylered = "style='font-size:small;color:Red;'";
                string tdstyle = "style='border:silver 1px ridge; font-size:small;background-color: #FFFFFF;empty-cells:show;' nowrap='nowrap'";
                StringBuilder sb = new StringBuilder();
                sb.Append("<div>");
                sb.Append("<div " + divstyle + "> Dear " + dtMail.Rows[0]["Approver"].ToString() + ",</div><br />");
                
                if (warningmsg!="")
                {
                    sb.Append("<div " + divstyle + ">The following eReimbursement application:" + msg + "</div><br />");
                    sb.Append("<div " + divstylered + ">" + warningmsg + "</div><br /><br />");
                }
                else
                {
                    sb.Append("<div " + divstyle + ">The following eReimbursement application:" + msg + "</div><br /><br />");
                }
                sb.Append("<div " + divstyle + ">No#:" + dtMail.Rows[0]["No"].ToString() + budget + "</div>");
                sb.Append("<div " + divstyle + ">Owner:" + dtMail.Rows[0]["Person"].ToString() + "</div>");
                sb.Append("<div " + divstyle + ">Station:" + dtMail.Rows[0]["Station"].ToString() + "</div>");
                sb.Append("<div " + divstyle + ">Department:" + dtMail.Rows[0]["Department"].ToString() + "</div>");
                //string period = "";
                string sqldetail2 = "select min(convert(varchar(10),Tdate0,111)) as Tdate0,max(convert(varchar(10),Tdate,111)) as Tdate1 from ETraveleDetail where [No]=" + hdTravelRequestID.Value.ToString() + "";
                DataTable dtder = dbc.GetData("eReimbursement", sqldetail2);
                if (dtder.Rows.Count == 1)
                {
                    sb.Append("<div " + divstyle + ">Period:From " + Convert.ToDateTime(dtder.Rows[0]["Tdate0"].ToString()).ToString("yyyy/MM/dd") + " To " + Convert.ToDateTime(dtder.Rows[0]["Tdate1"].ToString()).ToString("yyyy/MM/dd") + "</div>");
                }
                
                //period += dtMail.Rows[0]["Bdate"].ToString() == "" ? "From NA " : ("From " + Convert.ToDateTime(dtMail.Rows[0]["Bdate"].ToString()).ToString("yyyy/MM/dd") + " ");
                //period += dtMail.Rows[0]["Edate"].ToString() == "" ? "To NA" : "To " + Convert.ToDateTime(dtMail.Rows[0]["Edate"].ToString()).ToString("yyyy/MM/dd");
                sb.Append("<br />");
                sb.Append("<div><table style='border-collapse:collapse;'><thead><tr><th colspan=\"15\" " + tdstyle + ">Expense Detail</th></tr><tr>");
                sb.Append("<th " + tdstyle + "></th>");
                sb.Append("<th " + tdstyle + ">Destination</th>");
                sb.Append("<th " + tdstyle + ">Date</th>");
                sb.Append("<th " + tdstyle + ">Expense Type</th>");
                sb.Append("<th " + tdstyle + ">Currency</th>");
                sb.Append("<th " + tdstyle + ">Reimbursement</th>");
                sb.Append("<th " + tdstyle + ">Company Paid</th>");
                sb.Append("<th " + tdstyle + ">Cost Center</th>");
                sb.Append("<th style='border:silver 1px ridge; font-size:small;background-color: #FFFFFF;empty-cells:show;width:160px;' nowrap='nowrap'>Remark</th>");
                sb.Append("<th " + tdstyle + ">Station Budget MTD:(Used/All)</th>");
                sb.Append("<th " + tdstyle + ">Dept. Budget MTD:(Used/All)</th>");
                sb.Append("<th " + tdstyle + ">Person Budget MTD:(Used/All)</th>");
                sb.Append("<th " + tdstyle + ">Station Budget YTD:(Used/All)</th>");
                sb.Append("<th " + tdstyle + ">Dept. Budget YTD:(Used/All)</th>");
                sb.Append("<th " + tdstyle + ">Person Budget YTD:(Used/All)</th></tr></thead>");

                sb.Append("<tbody>");
                decimal ptotal = 0; decimal ctotal = 0;
                string sqlp = "";
                sqlp += "case when t1.DetailCode=0 then '1. Air Ticket - Intl' when t1.DetailCode=1 then 'Air Ticket - Domestic' ";
                sqlp += "when t1.DetailCode=2 then '2. Hotel Bill' when t1.DetailCode=3 then '3. Meals' ";
                sqlp += "when t1.DetailCode=4 then '4. Entertainment' when t1.DetailCode=5 then '5. Car Rental/Transportation' ";
                sqlp += "when t1.DetailCode=6 then '6. Communication' when t1.DetailCode=7 then '7. Local Trip Allowance' ";
                sqlp += "when t1.DetailCode=8 then '8. Overseas Trip Allowance' when t1.DetailCode=9 then '9. Airport Tax/Travel Insurance' ";
                sqlp += "when t1.DetailCode=10 then '10. Others' else t2.SAccountName end as [SAccountName]";
                string sqldetail = "select [StationBudget]='',[DepartmentBudget]='',[PersonBudget]='',[StationYTD]='',[DepartmentYTD]='',[PersonYTD]=''," + sqlp + ",t1.* from ETraveleDetail t1 left join AccoundCode t2 on t2.SAccountCode=t1.AccountCode where t1.[No]='" + hdTravelRequestID.Value.ToString() + "'";
                DataTable dtdetail = new DataTable();
                dtdetail = dbc.GetData("eReimbursement", sqldetail);
                for (int i = 0; i < dtdetail.Rows.Count; i++)
                {
                    //载入预算内容
                    string userid = dtMail.Rows[0]["PersonID"].ToString();
                    string dpt = "";
                    string ostation = "";
                    DataSet ds1 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(userid);
                    if (ds1.Tables[0].Rows.Count == 1)
                    {
                        DataTable dt1 = ds1.Tables[0];
                        //dpt = dt1.Rows[0]["DepartmentName"].ToString();
                        ostation = dt1.Rows[0]["stationCode"].ToString();
                        dpt = dt1.Rows[0]["CRPDepartmentName"].ToString();
                        DataTable dttemp = new DataTable();
                        string sqltemp = "select * from ESUSER where Userid='" + userid + "'";
                        dttemp = dbc.GetData("eReimbursement", sqltemp);
                        if (dttemp.Rows.Count > 0)
                        {
                            ostation = dttemp.Rows[0]["Station"].ToString();
                        }
                    }
                    string tstation = dtdetail.Rows[i]["TSation"].ToString() == "" ? ostation : dtdetail.Rows[i]["TSation"].ToString();
                    string accountcode = dtdetail.Rows[i]["AccountCode"].ToString();
                    string Years = Convert.ToDateTime(dtdetail.Rows[i]["Tdate"].ToString()).Year.ToString();
                    string month = Convert.ToDateTime(dtdetail.Rows[i]["Tdate"].ToString()).Month.ToString();

                    DataTable dtbudget = new DataTable();
                    dtbudget = Comm.RtnEB(userid, dpt, ostation, tstation, accountcode, Years, month);
                    double re = System.Math.Round(DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(tstation) / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(ostation), 4);
                    if (dtbudget != null && dtbudget.Rows.Count > 0)
                    {
                        string stationbudget = "", departmentbudget = "", personbudget = "", stationYear = "", departmentYear = "", personYear = "";
                        for (int j = 0; j < dtbudget.Rows.Count; j++)
                        {
                            decimal budget1 = 0, used = 0;
                            budget1 = Convert.ToDecimal(System.Math.Round(Convert.ToDouble(dtbudget.Rows[j]["Budget"].ToString()) * re, 2));
                            used = Convert.ToDecimal(System.Math.Round(Convert.ToDouble(dtbudget.Rows[j]["Used"].ToString()) * re, 2));

                            if (dtbudget.Rows[j]["Type"].ToString() == "站点")
                            {
                                stationbudget = used.ToString("#,##0.00") + "/" + budget1.ToString("#,##0.00");
                                dtdetail.Rows[i]["StationBudget"] = stationbudget;
                            }
                            else if (dtbudget.Rows[j]["Type"].ToString() == "部门")
                            {
                                departmentbudget = used.ToString("#,##0.00") + "/" + budget1.ToString("#,##0.00");
                                dtdetail.Rows[i]["DepartmentBudget"] = departmentbudget;
                            }
                            else if (dtbudget.Rows[j]["Type"].ToString() == "个人")
                            {
                                personbudget = used.ToString("#,##0.00") + "/" + budget1.ToString("#,##0.00");
                                dtdetail.Rows[i]["PersonBudget"] = personbudget;
                            }
                            else if (dtbudget.Rows[j]["Type"].ToString() == "全年个人")
                            {
                                personYear = used.ToString("#,##0.00") + "/" + budget1.ToString("#,##0.00");
                                dtdetail.Rows[i]["PersonYTD"] = personYear;
                            }
                            else if (dtbudget.Rows[j]["Type"].ToString() == "全年部门")
                            {
                                departmentYear = used.ToString("#,##0.00") + "/" + budget1.ToString("#,##0.00");
                                dtdetail.Rows[i]["DepartmentYTD"] = departmentYear;
                            }
                            else if (dtbudget.Rows[j]["Type"].ToString() == "全年站点")
                            {
                                stationYear = used.ToString("#,##0.00") + "/" + budget1.ToString("#,##0.00");
                                dtdetail.Rows[i]["StationYTD"] = stationYear;
                            }
                        }
                    }

                    sb.Append("<tr><th " + tdstyle + ">" + (i + 1).ToString() + "</th>");
                    sb.Append("<td " + tdstyle + " nowrap='nowrap'>" + dtdetail.Rows[i]["Tocity"].ToString() + "</td>");
                    string tdate = dtdetail.Rows[i]["Tdate"].ToString() == "" ? "" : Convert.ToDateTime(dtdetail.Rows[i]["Tdate"].ToString()).ToString("yyyy/MM/dd");
                    sb.Append("<td " + tdstyle + " nowrap='nowrap'>" + tdate + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["SAccountName"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["Cur"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + ">" + (dtdetail.Rows[i]["Pamount"].ToString() == "" ? "" : Convert.ToDecimal(dtdetail.Rows[i]["Pamount"].ToString()).ToString("#,##0.00")) + "</td>");
                    sb.Append("<td " + tdstyle + ">" + (dtdetail.Rows[i]["Camount"].ToString() == "" ? "" : Convert.ToDecimal(dtdetail.Rows[i]["Camount"].ToString()).ToString("#,##0.00")) + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["TSation"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["AccountDes"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + " nowrap='nowrap'>" + dtdetail.Rows[i]["StationBudget"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + " nowrap='nowrap'>" + dtdetail.Rows[i]["DepartmentBudget"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + " nowrap='nowrap'>" + dtdetail.Rows[i]["PersonBudget"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + " nowrap='nowrap'>" + dtdetail.Rows[i]["StationYTD"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + " nowrap='nowrap'>" + dtdetail.Rows[i]["DepartmentYTD"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + " nowrap='nowrap'>" + dtdetail.Rows[i]["PersonYTD"].ToString() + "</td></tr>");
                    ptotal += dtdetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtdetail.Rows[i]["Pamount"].ToString());
                    ctotal += dtdetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtdetail.Rows[i]["Camount"].ToString());
                }
                sb.Append("</tbody>");
                sb.Append("<tfoot><tr>");
                sb.Append("<th " + tdstyle + ">Total:</th>");
                sb.Append("<td colspan=\"3\" " + tdstyle + "></td>");
                sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[0]["Cur"].ToString() + "</td>");
                sb.Append("<td " + tdstyle + ">" + ptotal.ToString("#,##0.00") + "</td>");
                sb.Append("<td " + tdstyle + ">" + ctotal.ToString("#,##0.00") + "</td>");
                sb.Append("<td colspan=\"8\" " + tdstyle + "></td>");
                sb.Append("</tr></tfoot></table></div><br />");
                sb.Append("<div " + divstyle + ">Apply Remark:" + dtMail.Rows[0]["Remark"].ToString() + "</div><br />");

                StringBuilder sb1 = new StringBuilder();
                sb1.Append("<div><span " + divstyle + ">Approval Flow:</span>");
                for (int i = 0; i < dtMail.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        sb1.Append("<div " + divstyleCurrent + ">" + (i + 1).ToString());
                    }
                    else
                    {
                        sb1.Append("<div " + divstyle + ">" + (i + 1).ToString());
                    }

                    string msg1 = "";
                    if (dtMail.Rows[i]["FlowFn"].ToString().ToLower() == "verifier")
                    {
                        msg1 = ". To Be Verified: " + dtMail.Rows[i]["Approver"].ToString() + "</div>";
                    }
                    else if (dtMail.Rows[i]["FlowFn"].ToString().ToLower() == "issuer")
                    {
                        msg1 = ". To Be Issued: " + dtMail.Rows[i]["Approver"].ToString() + "</div>";
                    }
                    else
                    {
                        msg1 = ". Waiting for Approval: " + dtMail.Rows[i]["Approver"].ToString() + "</div>";
                    }
                    sb1.Append(msg1);
                }
                sb1.Append("</div><br />");
                sb.Append(sb1.ToString());
                string url = "";
                if (Request.Url.Host != "localhost")
                {
                    url = "http://61.218.73.79:88/eReimbursement/Approve.aspx";
                }
                else
                {
                    url = "http://" + Request.Url.Authority + "/Approve.aspx";
                }
                sb.Append("<div><a href=\"" + url + "?FlowID=" + dtMail.Rows[0]["FlowID"].ToString() + "\" style=\"color: #0000FF\">Click here to visit Dimerco eReimbursement.</a></div>");
                sb.Append("</div>");
                mail.Body = sb.ToString();
                mail.Send();
            }
            else
            {
                return false;
            }
            return true;
        }
        protected void ChangeBudget(object sender, DirectEventArgs e)
        {
            cs.DBCommand dbc = new cs.DBCommand();
            //检查是否选择了出差记录
            string sqlCheckFlow = "";
            if (hdOwnerID.Value == null || hdOwnerID.Value.ToString() == "")
            {
                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                {
                    ErrorHandle("请从eLeave选择出差记录.");
                }
                else
                {
                    ErrorHandle("Please select eLeave Data.");
                }
                return;
            }
            if (Radio1.Checked)
            //if (cbxBudget.Value.ToString() == "YES")//使用Budget审批流程
            {
                sqlCheckFlow = "select * from GroupFlow where [Type]!=2 and GID=(select GID from GroupUsers where UserID='" + hdOwnerID.Value.ToString() + "')";
            }
            else//使用unBudget审批流程
            {
                sqlCheckFlow = "select * from GroupFlow where [Type]=2 and GID=(select GID from GroupUsers where UserID='" + hdOwnerID.Value.ToString() + "')";
            }

            //string sqlCheckFlow = "select * from GroupFlow where GID=(select GID from GroupUsers where UserID='" + cbxOwner.Text + "')";
            DataTable dtCheckFlow = dbc.GetData("eReimbursement", sqlCheckFlow);
            if (dtCheckFlow.Rows.Count < 1)
            {
                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                {
                    ErrorHandle("请联系Local MIS设置审批人.");
                }
                else
                {
                    ErrorHandle("Not set Approve flow,please contact with Local MIS.");
                }
            }
        }
        protected void ChangePerson(object sender, DirectEventArgs e)
        {
            cs.DBCommand dbc = new cs.DBCommand();
            DataSet ds1 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(hdOwnerID.Value.ToString());
            if (ds1.Tables[0].Rows.Count == 1)
            {
                DataTable dt1 = ds1.Tables[0];
                labelStation.Text = dt1.Rows[0]["stationCode"].ToString();
                labelDepartment.Text = dt1.Rows[0]["CRPDepartmentName"].ToString();
                LabelCurrency.Text = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dt1.Rows[0]["stationCode"].ToString());

                hdOwner.Text = dt1.Rows[0]["FullName"].ToString();
                hdOwnerID.Text = dt1.Rows[0]["UserID"].ToString();

                //切换币种
                DataTable dttemp = new DataTable();
                string sqltemp = "select * from ESUSER where Userid='" + hdOwnerID.Value.ToString() + "'";
                dttemp = dbc.GetData("eReimbursement", sqltemp);
                if (dttemp.Rows.Count > 0)
                {
                    LabelCurrency.Text = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dttemp.Rows[0]["Station"].ToString());
                }
            }



            //检查是否已经为该申请人设置过审批人
            string sqlCheckFlow = "";
            if (Radio1.Checked)
            //if (cbxBudget.Value.ToString() == "YES")//使用Budget审批流程
            {
                sqlCheckFlow = "select * from GroupFlow where [Type]!=2 and GID=(select GID from GroupUsers where UserID='" + hdOwnerID.Value.ToString() + "')";
            }
            else//使用unBudget审批流程
            {
                sqlCheckFlow = "select * from GroupFlow where [Type]=2 and GID=(select GID from GroupUsers where UserID='" + hdOwnerID.Value.ToString() + "')";
            }

            //string sqlCheckFlow = "select * from GroupFlow where GID=(select GID from GroupUsers where UserID='" + cbxOwner.Text + "')";
            DataTable dtCheckFlow = dbc.GetData("eReimbursement", sqlCheckFlow);
            if (dtCheckFlow.Rows.Count < 1)
            {
                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                {
                    ErrorHandle("请联系Local MIS设置审批人.");
                }
                else
                {
                    ErrorHandle("Not set Approve flow,please contact with Local MIS.");
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
        public void AddCol(string StoreData, string header0string, string header1string, string header2string, string DSTN, string LeaveDate1, string LeaveDate2)
        {
            StoreSubmitDataEventArgs eSubmit = new StoreSubmitDataEventArgs(StoreData, null);
            XmlNode xml = eSubmit.Xml;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.InnerXml);
            int dtcol = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes.Count;
            int colc = (dtcol - 3) / 2;
            DataTable dt = new DataTable();

            Store2.Reader[0].Fields.Add("Category", RecordFieldType.String);
            dt.Columns.Add(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[0].Name, typeof(String));
            for (int i = 0; i < (dtcol - 3) / 2; i++)
            {
                Store2.Reader[0].Fields.Add("Station_" + i.ToString() + "_P", RecordFieldType.String);
                Store2.Reader[0].Fields.Add("Station_" + i.ToString() + "_C", RecordFieldType.String);
                dt.Columns.Add("Station_" + i.ToString() + "_P", typeof(String));
                dt.Columns.Add("Station_" + i.ToString() + "_C", typeof(String));
            }
            string fieldPNameNew = "Station_" + colc.ToString() + "_P";
            Store2.Reader[0].Fields.Add(fieldPNameNew, RecordFieldType.String);
            string fieldCNameNew = "Station_" + colc.ToString() + "_C";
            Store2.Reader[0].Fields.Add(fieldCNameNew, RecordFieldType.String);
            dt.Columns.Add(fieldPNameNew, typeof(String));
            dt.Columns.Add(fieldCNameNew, typeof(String));
            //合计列
            Store2.Reader[0].Fields.Add("TotalP", RecordFieldType.String);
            Store2.Reader[0].Fields.Add("TotalC", RecordFieldType.String);
            dt.Columns.Add("TotalP", typeof(String));
            dt.Columns.Add("TotalC", typeof(String));
            //decimal row0Psum = 0, row0Csum = 0, row1Psum = 0, row1Csum = 0, row2Psum = 0, row2Csum = 0, row3Psum = 0, row3Csum = 0, row4Psum = 0, row4Csum = 0, row5Psum = 0, row5Csum = 0, row6Psum = 0, row6Csum = 0, row7Psum = 0, row7Csum = 0, row8Psum = 0, row8Csum = 0, row9Psum = 0, row9Csum = 0, row10Psum = 0, row10Csum = 0;

            for (int i = 0; i < doc.SelectNodes("records").Item(0).SelectNodes("record").Count - 1; i++)
            {
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
                for (int j = 0; j < dtcol - 3; j++)
                {
                    string wr = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).ChildNodes[j].InnerText;
                    dt.Rows[i][j] = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).ChildNodes[j].InnerText;
                }
            }
            dt.Rows.Add(dt.NewRow());
            dt.Rows[11][0] = "Total";
            for (int i = 0; i < 11; i++)
            {
                for (int j = 1; j < dtcol - 3; j++)
                {
                    if (j % 2 == 1)
                    {
                        dt.Rows[i][dtcol - 1] = (Convert.ToDecimal(dt.Rows[i][dtcol - 1].ToString() == "" ? "0" : dt.Rows[i][dtcol - 1].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString() == "0" ? "" : (Convert.ToDecimal(dt.Rows[i][dtcol - 1].ToString() == "" ? "0" : dt.Rows[i][dtcol - 1].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString();
                        dt.Rows[11][dtcol - 1] = (Convert.ToDecimal(dt.Rows[11][dtcol - 1].ToString() == "" ? "0" : dt.Rows[11][dtcol - 1].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString() == "0" ? "" : (Convert.ToDecimal(dt.Rows[11][dtcol - 1].ToString() == "" ? "0" : dt.Rows[11][dtcol - 1].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString();
                    }
                    else
                    {
                        dt.Rows[i][dtcol] = (Convert.ToDecimal(dt.Rows[i][dtcol].ToString() == "" ? "0" : dt.Rows[i][dtcol].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString() == "0" ? "" : (Convert.ToDecimal(dt.Rows[i][dtcol].ToString() == "" ? "0" : dt.Rows[i][dtcol].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString();
                        dt.Rows[11][dtcol] = (Convert.ToDecimal(dt.Rows[11][dtcol].ToString() == "" ? "0" : dt.Rows[11][dtcol].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString() == "0" ? "" : (Convert.ToDecimal(dt.Rows[11][dtcol].ToString() == "" ? "0" : dt.Rows[11][dtcol].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString();
                    }
                    dt.Rows[11][j] = (Convert.ToDecimal(dt.Rows[11][j].ToString() == "" ? "0" : dt.Rows[11][j].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString() == "0" ? "" : (Convert.ToDecimal(dt.Rows[11][j].ToString() == "" ? "0" : dt.Rows[11][j].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString();
                }
            }

            Store2.DataSource = dt;
            Store2.DataBind();

            var TitleCol = new Column();
            TitleCol.DataIndex = "Category";
            TitleCol.Sortable = false;
            TitleCol.Resizable = false;
            TitleCol.MenuDisabled = true;
            TitleCol.Width = 180;
            this.GridPanel2.ColumnModel.Columns.Add(TitleCol);

            var Title1 = new Ext.Net.Label();
            Title1.Text = "Destination:";
            HeaderColumn hcTitle1 = new HeaderColumn();
            hcTitle1.Component.Add(Title1);
            this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcTitle1);

            var Title2 = new Ext.Net.Label();
            Title2.Text = "Cost Center:";
            HeaderColumn hcTitle2 = new HeaderColumn();
            hcTitle2.Component.Add(Title2);
            this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcTitle2);

            var Title3 = new Ext.Net.Label();
            Title3.Text = "Travel Period:";
            HeaderColumn hcTitle3 = new HeaderColumn();
            hcTitle3.Component.Add(Title3);
            this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTitle3);
            //取得出差站点列表
            DataSet GetCityInfo = DIMERCO.SDK.Utilities.LSDK.GetCityInfo("", 8000);
            DataTable dtstation = GetCityInfo.Tables[0];
            //取得成本中心列表
            DataSet GetCCInfo = DIMERCO.SDK.Utilities.LSDK.getCostCenterBYStationCode("", 8000);
            DataTable dtCC = GetCCInfo.Tables[0];
            for (int i = 0; i < colc; i++)//准备复制已有信息
            {
                string fieldPName = "Station_" + i.ToString() + "_P";
                //RecordField field1 = new RecordField(fieldAName, RecordFieldType.Float);
                //Store2.Reader[0].Fields.Add(fieldPName, RecordFieldType.Float);
                //this.Store2.AddField(field1, columncount);
                string fieldCName = "Station_" + i.ToString() + "_C";
                //RecordField field1 = new RecordField(fieldAName, RecordFieldType.Float);
                //Store2.Reader[0].Fields.Add(fieldCName, RecordFieldType.Float);

                var txtP = new Ext.Net.NumberField();
                //txtP.Listeners.Blur.Fn = "Cal";
                var colP = new Column();
                colP.Header = "Reimbursement";
                colP.DataIndex = fieldPName;
                colP.Sortable = false;
                colP.Resizable = false;
                colP.MenuDisabled = true;
                colP.Width = 110;
                colP.Editor.Add(txtP);
                this.GridPanel2.ColumnModel.Columns.Add(colP);

                var txtC = new Ext.Net.NumberField();
                //txtC.Listeners.Blur.Fn = "Cal";
                var colC = new Column();
                colC.Header = "Company Paid";
                colC.DataIndex = fieldCName;
                colC.Sortable = false;
                colC.Resizable = false;
                colC.MenuDisabled = true;
                colC.Width = 110;
                colC.Editor.Add(txtC);
                this.GridPanel2.ColumnModel.Columns.Add(colC);

                var Station = new Ext.Net.TextField();
                if (header0string.Split(',')[i] != "NA")
                {
                    Station.Text = header0string.Split(',')[i];
                }
                HeaderColumn hcStation = new HeaderColumn();
                hcStation.Component.Add(Station);
                this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcStation);


                var Button = new Ext.Net.Button();
                Button.Text = "Remove";
                Button.Listeners.Click.Handler = "removecol(this," + i.ToString() + ");";
                Button.Listeners.Click.Delay = 50;
                HeaderColumn hcButton = new HeaderColumn();
                hcButton.Component.Add(Button);
                this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcButton);

                var CostCenter = new Ext.Net.TextField();
                CostCenter.Disabled = true;
                CostCenter.EmptyText = "Station Code";
                if (header1string.Split(',')[i] != "NA")
                {
                    CostCenter.Text = header1string.Split(',')[i];
                }
                HeaderColumn hcCostCenter = new HeaderColumn();
                hcCostCenter.Component.Add(CostCenter);
                this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcCostCenter);

                var ButtonGetSum = new Ext.Net.Button();
                ButtonGetSum.Text = "Calculate";
                ButtonGetSum.Listeners.Click.Handler = "GetSum();";
                ButtonGetSum.Listeners.Click.Delay = 50;
                HeaderColumn hcButtonGetSum = new HeaderColumn();
                hcButtonGetSum.Component.Add(ButtonGetSum);
                this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcButtonGetSum);

                var datefrom = new DateField();
                if (header2string.Split(',')[i * 2] != "NA")
                {
                    datefrom.SetValue(header2string.Split(',')[i * 2]);
                }
                datefrom.EmptyText = "yyyy/MM/dd";
                datefrom.Format = "yyyy/MM/dd";
                HeaderColumn Date1 = new HeaderColumn();
                Date1.Component.Add(datefrom);
                this.GridPanel2.GetView().HeaderRows[2].Columns.Add(Date1);

                var dateto = new DateField();
                if (header2string.Split(',')[i * 2 + 1] != "NA")
                {
                    dateto.SetValue(header2string.Split(',')[i * 2 + 1]);
                }
                dateto.EmptyText = "yyyy/MM/dd";
                dateto.Format = "yyyy/MM/dd";
                HeaderColumn Date2 = new HeaderColumn();
                Date2.Component.Add(dateto);
                this.GridPanel2.GetView().HeaderRows[2].Columns.Add(Date2);
            }



            var txtPNew = new Ext.Net.NumberField();
            //txtPNew.Listeners.Blur.Fn = "Cal";
            var colPNew = new Column();
            colPNew.Header = "Reimbursement";
            colPNew.DataIndex = fieldPNameNew;
            colPNew.Sortable = false;
            colPNew.Resizable = false;
            colPNew.MenuDisabled = true;
            colPNew.Width = 110;
            colPNew.Editor.Add(txtPNew);
            this.GridPanel2.ColumnModel.Columns.Add(colPNew);

            var txtCNew = new Ext.Net.NumberField();
            //txtCNew.Listeners.Blur.Fn = "Cal";
            var colCNew = new Column();
            colCNew.Header = "Company Paid";
            colCNew.DataIndex = fieldCNameNew;
            colCNew.Sortable = false;
            colCNew.Resizable = false;
            colCNew.MenuDisabled = true;
            colCNew.Width = 110;
            colCNew.Editor.Add(txtCNew);
            this.GridPanel2.ColumnModel.Columns.Add(colCNew);

            var TotalP = new Ext.Net.NumberField();
            TotalP.ReadOnly = true;
            TotalP.Cls = "custom-row";
            var colTotalP = new Column();
            colTotalP.DataIndex = "TotalP";
            colTotalP.Sortable = false;
            colTotalP.Resizable = false;
            colTotalP.MenuDisabled = true;
            colTotalP.Width = 110;
            colTotalP.Locked = true;
            colTotalP.Editor.Add(TotalP);
            this.GridPanel2.ColumnModel.Columns.Add(colTotalP);

            var TotalC = new Ext.Net.NumberField();
            TotalC.ReadOnly = true;
            TotalC.Cls = "custom-row";
            var colTotalC = new Column();
            colTotalC.DataIndex = "TotalC";
            colTotalC.Sortable = false;
            colTotalC.Resizable = false;
            colTotalC.MenuDisabled = true;
            colTotalC.Width = 110;
            colTotalC.Locked = true;
            colTotalC.Editor.Add(TotalC);
            this.GridPanel2.ColumnModel.Columns.Add(colTotalC);

            var StationNew = new Ext.Net.TextField();
            StationNew.Text = DSTN;
            HeaderColumn hcStationNew = new HeaderColumn();
            hcStationNew.Component.Add(StationNew);
            this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcStationNew);


            var ButtonNew = new Ext.Net.Button();
            ButtonNew.Text = "Remove";
            ButtonNew.Listeners.Click.Handler = "removecol(this," + colc.ToString() + ");";
            ButtonNew.Listeners.Click.Delay = 50;
            HeaderColumn hcButtonNew = new HeaderColumn();
            hcButtonNew.Component.Add(ButtonNew);
            this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcButtonNew);

            HeaderColumn hcTotal1 = new HeaderColumn();
            this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcTotal1);

            HeaderColumn hcTotal2 = new HeaderColumn();
            this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcTotal2);

            var CostCenterNew = new Ext.Net.TextField();
            CostCenterNew.Disabled = true;
            //160520 代垫费用不复制成本中心
            if (!CheckBoxOnBehalfItem.Checked)
            {
                CostCenterNew.Text = GetUserInfo(hdOwnerID.Value.ToString()).Rows[0]["CostCenter"].ToString();
            }
            CostCenterNew.EmptyText = "Station Code";
            HeaderColumn hcCostCenterNew = new HeaderColumn();
            hcCostCenterNew.Component.Add(CostCenterNew);
            this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcCostCenterNew);


            //HeaderColumn hcCostCenter1New = new HeaderColumn();
            //this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcCostCenter1New);
            var ButtonGetSumNew = new Ext.Net.Button();
            ButtonGetSumNew.Text = "Calculate";
            ButtonGetSumNew.Listeners.Click.Handler = "GetSum();";
            ButtonGetSumNew.Listeners.Click.Delay = 50;
            HeaderColumn hcButtonGetSumNew = new HeaderColumn();
            hcButtonGetSumNew.Component.Add(ButtonGetSumNew);
            this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcButtonGetSumNew);

            HeaderColumn hcTotal3 = new HeaderColumn();
            this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcTotal3);

            HeaderColumn hcTotal4 = new HeaderColumn();
            this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcTotal4);

            var dateFromNew = new DateField();
            dateFromNew.EmptyText = "yyyy/MM/dd";
            dateFromNew.Format = "yyyy/MM/dd";
            dateFromNew.SetValue(LeaveDate1);
            HeaderColumn Date1New = new HeaderColumn();
            Date1New.Component.Add(dateFromNew);
            this.GridPanel2.GetView().HeaderRows[2].Columns.Add(Date1New);

            var datetoNew = new DateField();
            datetoNew.EmptyText = "yyyy/MM/dd";
            datetoNew.Format = "yyyy/MM/dd";
            datetoNew.SetValue(LeaveDate2);
            HeaderColumn Date2New = new HeaderColumn();
            Date2New.Component.Add(datetoNew);
            this.GridPanel2.GetView().HeaderRows[2].Columns.Add(Date2New);

            var TitleTotalP = new Ext.Net.Label();
            TitleTotalP.Text = "Total(Personal paid)";
            TitleTotalP.Cls = "custom-row";
            HeaderColumn hcTitleTotalP = new HeaderColumn();
            hcTitleTotalP.Component.Add(TitleTotalP);
            this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTitleTotalP);

            var TitleTotalC = new Ext.Net.Label();
            TitleTotalC.Text = "Total(Company)";
            TitleTotalC.Cls = "custom-row";
            HeaderColumn hcTitleTotalC = new HeaderColumn();
            hcTitleTotalC.Component.Add(TitleTotalC);
            this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTitleTotalC);

            //HeaderColumn hcTotal5 = new HeaderColumn();
            //this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTotal5);

            //HeaderColumn hcTotal6 = new HeaderColumn();
            //this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTotal6);
            this.GridPanel2.Render();
            //Store store2 = this.GridPanel2.GetStore();



            //this.GridPanel2.RefreshView();
            GridPanel2.Reconfigure();
            //string last = (dtcol - 1).ToString();
            //string sf = "$(\"#\"+$('#GridPanel2 .x-grid3-col-" + last + " input')[0].id).css('font-weight','bold')";
            //X.AddScript(sf);
        }
        [DirectMethod]
        public void SaveAll2(string type, string detail)
        {
            string sf = "";
        }
        [DirectMethod]
        public void SaveAll1(string type, string detail, string MailList, string header0string, string header1string, string header2string, string Cur, string dept,string budget)
        {
            //检查是否登录超时
            if (Request.Cookies.Get("eReimUserID") == null || hdUser.Value.ToString() != Request.Cookies.Get("eReimUserID").Value)
            {
                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                {
                    X.AddScript("Ext.Msg.show({ title: '提示', msg: '已切换用户,将刷新页面.', buttons: { ok: 'Ok' }, fn: function (btn) { window.location.reload(); } });");
                }
                else
                {
                    X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Current user changed,reloading...', buttons: { ok: 'Ok' }, fn: function (btn) { window.location.reload(); } });");
                }
                return;
            }
            cs.DBCommand dbc = new cs.DBCommand();

            //处理抄送人列表
            string CCMailList = "";
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<CCMailList> CCMailList1 = ser.Deserialize<List<CCMailList>>(MailList);
            foreach (CCMailList mail in CCMailList1)
            {
                CCMailList += mail.Email + ",";
            }
            CCMailList = CCMailList.Length > 0 ? CCMailList.Substring(0, CCMailList.Length - 1) : "";

            //string userid = hdOwnerID.Value.ToString();
            //string ostation = ""; string station = ""; string department = "";

            //160113 垫付人
            string userid = (hdOnBehalf.Value == null || hdOnBehalf.Value.ToString() == "") ? hdOwnerID.Value.ToString() : hdOnBehalf.Value.ToString();
            string ostation = "";
            string station = ""; string department = "";

            string station_applyperson = ""; string costcenter_applyperson = ""; string dept_applyperson = "";
            DataSet ds_apply = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(hdOwnerID.Value.ToString());
            if (ds_apply.Tables[0].Rows.Count == 1)
            {
                DataTable dt_apply = ds_apply.Tables[0];
                dept_applyperson = dt_apply.Rows[0]["DepartmentName"].ToString();
                station_applyperson = dt_apply.Rows[0]["stationCode"].ToString();
                costcenter_applyperson = dt_apply.Rows[0]["CostCenter"].ToString();
            }


            DataSet ds2 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(userid);
            if (ds2.Tables[0].Rows.Count == 1)
            {
                DataTable dt1 = ds2.Tables[0];
                ostation = dt1.Rows[0]["CostCenter"].ToString();//记录用户预算站点,即CostCenter
                station = dt1.Rows[0]["stationCode"].ToString();//记录用户所在站点
                department = dt1.Rows[0]["CRPDepartmentName"].ToString();
            }
            //140306
            #region 预算
            decimal rate = 1;//记录用户币种与预算站点币种汇率
            string CurLocal = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(costcenter_applyperson);
            //检查是否本地维护过特殊币种
            DataTable dttemp = new DataTable();
            string sqltemp = "select * from ESUSER where Userid='" + userid + "'";
            dttemp = dbc.GetData("eReimbursement", sqltemp);
            if (dttemp.Rows.Count > 0)
            {
                CurLocal = dttemp.Rows[0]["Currency"].ToString();//如果单独设置了币种
                //LocalCurrency = dttemp.Rows[0]["Currency"].ToString();//如果单独设置了币种
            }
            string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(ostation);

            DataTable dtbudget = new DataTable();//记录预算数据
            dtbudget.Columns.Add("EName", typeof(System.String));
            dtbudget.Columns.Add("COACode", typeof(System.String));
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

            DataTable dtA = new DataTable();//记录原始数据
            dtA.Columns.Add("COACode", typeof(System.String));
            dtA.Columns.Add("Amount", typeof(System.Decimal));
            dtA.Columns.Add("PA", typeof(System.Decimal));
            dtA.Columns.Add("CA", typeof(System.Decimal));
            dtA.Columns.Add("Date", typeof(System.DateTime));

            StoreSubmitDataEventArgs eSubmit = new StoreSubmitDataEventArgs(detail, null);
            XmlNode xml = eSubmit.Xml;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.InnerXml);
            int dtcol = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes.Count;
            int dtrow = doc.SelectNodes("records").Item(0).SelectNodes("record").Count;
            int groupcount = (dtcol - 3) / 2;//多少个站点会被更新,如果站点为空则不更新

            for (int i = 0; i < groupcount; i++)
            {
                if (header0string.Split(',')[i] != "NA")//站点为空则不更新
                {
                    //1. Air Ticket - Int'l
                    if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[2 + i * 2].InnerText) != 0))
                    {
                        DataRow drnew = dtA.NewRow();
                        drnew["COACode"] = "62012000";
                        drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                        decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[1 + i * 2].InnerText);
                        decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[2 + i * 2].InnerText);
                        drnew["Amount"] = r1 + r2;
                        drnew["PA"] = r1;
                        drnew["CA"] = r2;
                        dtA.Rows.Add(drnew);
                    }
                    //Domestic
                    if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[2 + i * 2].InnerText) != 0))
                    {
                        DataRow drnew = dtA.NewRow();
                        drnew["COACode"] = "62012000";
                        drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                        decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[1 + i * 2].InnerText);
                        decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[2 + i * 2].InnerText);
                        drnew["Amount"] = r1 + r2;
                        drnew["PA"] = r1;
                        drnew["CA"] = r2;
                        dtA.Rows.Add(drnew);
                    }
                    //2. Hotel Bill
                    if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[2 + i * 2].InnerText) != 0))
                    {
                        DataRow drnew = dtA.NewRow();
                        drnew["COACode"] = "62012000";
                        drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                        decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[1 + i * 2].InnerText);
                        decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[2 + i * 2].InnerText);
                        drnew["Amount"] = r1 + r2;
                        drnew["PA"] = r1;
                        drnew["CA"] = r2;
                        dtA.Rows.Add(drnew);
                    }
                    //3. Meals
                    if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[2 + i * 2].InnerText) != 0))
                    {
                        DataRow drnew = dtA.NewRow();
                        drnew["COACode"] = "62012000";
                        drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                        decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[1 + i * 2].InnerText);
                        decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[2 + i * 2].InnerText);
                        drnew["Amount"] = r1 + r2;
                        drnew["PA"] = r1;
                        drnew["CA"] = r2;
                        dtA.Rows.Add(drnew);
                    }
                    //4. Entertainment
                    if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[2 + i * 2].InnerText) != 0))
                    {
                        DataRow drnew = dtA.NewRow();
                        drnew["COACode"] = "62010900";
                        drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                        decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[1 + i * 2].InnerText);
                        decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[2 + i * 2].InnerText);
                        drnew["Amount"] = r1 + r2;
                        drnew["PA"] = r1;
                        drnew["CA"] = r2;
                        dtA.Rows.Add(drnew);
                    }
                    //5. Car Rental/Transportation
                    if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[2 + i * 2].InnerText) != 0))
                    {
                        DataRow drnew = dtA.NewRow();
                        drnew["COACode"] = "62011900";
                        drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                        decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[1 + i * 2].InnerText);
                        decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[2 + i * 2].InnerText);
                        drnew["Amount"] = r1 + r2;
                        drnew["PA"] = r1;
                        drnew["CA"] = r2;
                        dtA.Rows.Add(drnew);
                    }
                    //6. Communication
                    if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[2 + i * 2].InnerText) != 0))
                    {
                        DataRow drnew = dtA.NewRow();
                        drnew["COACode"] = "62010500";
                        drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                        decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[1 + i * 2].InnerText);
                        decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[2 + i * 2].InnerText);
                        drnew["Amount"] = r1 + r2;
                        drnew["PA"] = r1;
                        drnew["CA"] = r2;
                        dtA.Rows.Add(drnew);
                    }
                    //7. Local Trip
                    if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[2 + i * 2].InnerText) != 0))
                    {
                        DataRow drnew = dtA.NewRow();
                        drnew["COACode"] = "62012000";
                        drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                        decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[1 + i * 2].InnerText);
                        decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[2 + i * 2].InnerText);
                        drnew["Amount"] = r1 + r2;
                        drnew["PA"] = r1;
                        drnew["CA"] = r2;
                        dtA.Rows.Add(drnew);
                    }
                    //8. Overseas Trip USD15/day
                    if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[2 + i * 2].InnerText) != 0))
                    {
                        DataRow drnew = dtA.NewRow();
                        drnew["COACode"] = "62012000";
                        drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                        decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[1 + i * 2].InnerText);
                        decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[2 + i * 2].InnerText);
                        drnew["Amount"] = r1 + r2;
                        drnew["PA"] = r1;
                        drnew["CA"] = r2;
                        dtA.Rows.Add(drnew);
                    }
                    //9. Airport Tax/Travel Insurance
                    if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[2 + i * 2].InnerText) != 0))
                    {
                        DataRow drnew = dtA.NewRow();
                        drnew["COACode"] = "62012000";
                        drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                        decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[1 + i * 2].InnerText);
                        decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[2 + i * 2].InnerText);
                        drnew["Amount"] = r1 + r2;
                        drnew["PA"] = r1;
                        drnew["CA"] = r2;
                        dtA.Rows.Add(drnew);
                    }
                    //10. Others
                    if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[2 + i * 2].InnerText) != 0))
                    {
                        DataRow drnew = dtA.NewRow();
                        drnew["COACode"] = "62012000";
                        drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                        decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[1 + i * 2].InnerText);
                        decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[2 + i * 2].InnerText);
                        drnew["Amount"] = r1 + r2;
                        drnew["PA"] = r1;
                        drnew["CA"] = r2;
                        dtA.Rows.Add(drnew);
                    }
                }
            }
            //合计
            DataTable dtB = new DataTable();//记录合并后数据
            dtB.Columns.Add("COACode", typeof(System.String));
            dtB.Columns.Add("Amount", typeof(System.Decimal));
            dtB.Columns.Add("PA", typeof(System.Decimal));
            dtB.Columns.Add("CA", typeof(System.Decimal));
            if (dtA.Compute("Sum(Amount)", "COACode = 62012000").ToString() != "" && Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62012000").ToString()) != 0)
            {
                DataRow dr = dtB.NewRow();
                dr["COACode"] = "62012000";
                dr["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62012000").ToString());
                dr["PA"] = Convert.ToDecimal(dtA.Compute("Sum(PA)", "COACode = 62012000").ToString());
                dr["CA"] = Convert.ToDecimal(dtA.Compute("Sum(CA)", "COACode = 62012000").ToString());
                dtB.Rows.Add(dr);
            }
            if (dtA.Compute("Sum(Amount)", "COACode = 62010900").ToString() != "" && Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62010900").ToString()) != 0)
            {
                DataRow dr = dtB.NewRow();
                dr["COACode"] = "62010900";
                dr["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62010900").ToString());
                dr["PA"] = Convert.ToDecimal(dtA.Compute("Sum(PA)", "COACode = 62010900").ToString());
                dr["CA"] = Convert.ToDecimal(dtA.Compute("Sum(CA)", "COACode = 62010900").ToString());
                dtB.Rows.Add(dr);
            }
            if (dtA.Compute("Sum(Amount)", "COACode = 62011900").ToString() != "" && Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62011900").ToString()) != 0)
            {
                DataRow dr = dtB.NewRow();
                dr["COACode"] = "62011900";
                dr["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62011900").ToString());
                dr["PA"] = Convert.ToDecimal(dtA.Compute("Sum(PA)", "COACode = 62011900").ToString());
                dr["CA"] = Convert.ToDecimal(dtA.Compute("Sum(CA)", "COACode = 62011900").ToString());
                dtB.Rows.Add(dr);
            }
            if (dtA.Compute("Sum(Amount)", "COACode = 62010500").ToString() != "" && Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62010500").ToString()) != 0)
            {
                DataRow dr = dtB.NewRow();
                dr["COACode"] = "62010500";
                dr["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62010500").ToString());
                dr["PA"] = Convert.ToDecimal(dtA.Compute("Sum(PA)", "COACode = 62010500").ToString());
                dr["CA"] = Convert.ToDecimal(dtA.Compute("Sum(CA)", "COACode = 62010500").ToString());
                dtB.Rows.Add(dr);
            }

            //计算本地币种与预算站点币种汇率
            if (CurLocal != CurBudget)
            {
                rate = DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(CurLocal, CurBudget, Convert.ToDateTime(dtA.Compute("Min(Date)", "").ToString()).Year);
            }
            ////取得传递预算的参数
            //string userid = dt.Rows[0]["PersonID"].ToString();
            //string dpt = dt.Rows[0]["Department"].ToString();
            //string ostation = dt.Rows[0]["CostCenter"].ToString();//预算站点,与基本信息中的CostCenter一致(Station2)
            //string CDate = dtA.Compute("Min(Date)", "").ToString();//记录预算日期
            string tstation = ostation;//Etravel表中的Station2,目前与预算站点一致,不允许更改
            string year = Convert.ToDateTime(dtA.Compute("Min(Date)", "").ToString()).Year.ToString();
            string month = Convert.ToDateTime(dtA.Compute("Min(Date)", "").ToString()).Month.ToString();
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
                    dtC = Comm.RtnEB(userid, department, ostation, tstation, accountcode, year, month);
                    for (int i = 0; i < dtC.Rows.Count; i++)
                    {
                        if (dtC.Rows[i]["Type"].ToString() == "全年个人")
                        {
                            dr["PU"] = Convert.ToDecimal(dtC.Rows[i]["Used"].ToString());
                            dr["PB"] = Convert.ToDecimal(dtC.Rows[i]["Budget"].ToString());
                        }
                        else if (dtC.Rows[i]["Type"].ToString() == "全年部门")
                        {
                            dr["DU"] = Convert.ToDecimal(dtC.Rows[i]["Used"].ToString());
                            dr["DB"] = Convert.ToDecimal(dtC.Rows[i]["Budget"].ToString());
                        }
                        else if (dtC.Rows[i]["Type"].ToString() == "全年站点")
                        {
                            dr["SU"] = Convert.ToDecimal(dtC.Rows[i]["Used"].ToString());
                            dr["SB"] = Convert.ToDecimal(dtC.Rows[i]["Budget"].ToString());
                        }
                    }
                    dtbudget.Rows.Add(dr);
                }
            }
            //bool UnBudget = false;
            //bool PB = false, DB = false, SB = false;
            //计算%,取得名称,转为本地币种汇率,增加列记录Currency为邮件准备
            dtbudget.Columns.Add("Currency", typeof(System.String));
            for (int i = 0; i < dtbudget.Rows.Count; i++)
            {
                dtbudget.Rows[i]["Currency"] = CurLocal;
                if (Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()) != 0)//如果Budget不为0,则计算%
                {
                    decimal PPercent = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["PU"].ToString()) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()), 2);
                    dtbudget.Rows[i]["PPercent"] = PPercent;
                }
                if (Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()) != 0)//如果Budget不为0,则计算%
                {
                    decimal DPercent = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["DU"].ToString()) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()), 2);
                    dtbudget.Rows[i]["DPercent"] = DPercent;
                }
                if (Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()) != 0)//如果Budget不为0,则计算%
                {
                    decimal SPercent = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["SU"].ToString()) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()), 2);
                    dtbudget.Rows[i]["SPercent"] = SPercent;
                }
                if (dtbudget.Rows[i]["COACode"].ToString() == "62012000")
                {
                    dtbudget.Rows[i]["EName"] = "Travel expense";
                }
                else if (dtbudget.Rows[i]["COACode"].ToString() == "62010900")
                {
                    dtbudget.Rows[i]["EName"] = "Entertainment";
                }
                else if (dtbudget.Rows[i]["COACode"].ToString() == "62011900")
                {
                    dtbudget.Rows[i]["EName"] = "Transportation";
                }
                else if (dtbudget.Rows[i]["COACode"].ToString() == "62010500")
                {
                    dtbudget.Rows[i]["EName"] = "Communication";
                }
                dtbudget.Rows[i]["PU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["PU"].ToString()), 2);
                dtbudget.Rows[i]["PB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()), 2);
                dtbudget.Rows[i]["DU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["DU"].ToString()), 2);
                dtbudget.Rows[i]["DB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()), 2);
                dtbudget.Rows[i]["SU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["SU"].ToString()), 2);
                dtbudget.Rows[i]["SB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()), 2);
            }
            
            string srw = "";
            #endregion
            ////判断是否设置了审批流程
            //string sqlCheckFlow = "";
            //sqlCheckFlow = "select * from GroupFlow where [Type]=2 and GID=(select GID from GroupUsers where UserID='" + hdOwnerID.Value.ToString() + "') order by FlowNo";
            //DataTable dtGroupFlowData = dbc.GetData("eReimbursement", sqlCheckFlow);
            //if (dtGroupFlowData.Rows.Count < 1)
            //{
            //    if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
            //    {
            //        ErrorHandle("请先设置审批人.");
            //    }
            //    else
            //    {
            //        ErrorHandle("Not set Approve flow,please contact with Local MIS.");
            //    }
            //    return;
            //}
            string sqlCheckFlow = ""; DataTable dtGroupFlowData = new DataTable();
            if (hdOnBehalf.Value == null || hdOnBehalf.Value.ToString() == "")
            {
                sqlCheckFlow = "select * from GroupFlow where [Type]=2 and GID=(select GID from GroupUsers where UserID='" + hdOwnerID.Value.ToString() + "') order by FlowNo";
                dtGroupFlowData = dbc.GetData("eReimbursement", sqlCheckFlow);
                if (dtGroupFlowData.Rows.Count < 1)
                {
                    if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                    {
                        ErrorHandle("请先设置审批人.");
                    }
                    else
                    {
                        ErrorHandle("Not set Approve flow,please contact with Local MIS.");
                    }
                    return;
                }
            }
            else//160113 垫付审批流程
            {
                sqlCheckFlow = "select UserID,t1.* from (select * from GroupFlow where [Type]=2 and GID in (select GID from GroupUsers where UserID='" + hdOwnerID.Value.ToString() + "' or UserID='" + hdOnBehalf.Value.ToString() + "')) t1 left join (select * from GroupUsers where UserID='" + hdOwnerID.Value.ToString() + "' or UserID='" + hdOnBehalf.Value.ToString() + "') t2 on t2.Gid=t1.Gid order by Gid,FlowNo";
                //sqlCheckFlow = "select * from GroupFlow where [Type]=2 and GID in (select GID from GroupUsers where UserID='" + hdOwnerID.Value.ToString() + "' or UserID='" + hdOnBehalf.Value.ToString() + "') order by Gid,FlowNo";
                dtGroupFlowData = dbc.GetData("eReimbursement", sqlCheckFlow);
                if (dtGroupFlowData.Rows.Count < 1)
                {
                    if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                    {
                        ErrorHandle("请先设置审批人.");
                    }
                    else
                    {
                        ErrorHandle("Not set Approve flow,please contact with Local MIS.");
                    }
                    return;
                }
            }
            if (type == "ND")//保存并申请
            {
                if (hdTravelRequestID.Value.ToString() == "")//直接新增申请,不通过草稿
                {
                    //Station2保存用户的预算站点,与用户信息中的CostCenter相同
                    string word = "[No],[Person],[Station],[Department],[ReportFile],[CreadedBy],[CreadedDate],[Attach],[Remark],[Bdate],[Edate],[PersonID],[CreadedByID],[ApplyDate],[CCMailList],[Budget],[Station2]";
                    //160123 垫付
                    if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                    { word += ",OnBehalfPersonID,OnBehalfPersonName,OnBehalfPersonUnit,OnBehalfPersonDept,OnBehalfPersonCostCenter"; }
                    string value = "";
                    value += "'" + station_applyperson + DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString().PadLeft(2, '0') + "',";//edit
                    value += "'" + hdOwner.Value.ToString() + "',"; value += "'" + station_applyperson + "',"; value += "'" + dept_applyperson + "',";//edit
                    value += "'" + hdReport.Value.ToString() + "',";
                    value += "'" + Request.Cookies.Get("eReimUserName").Value + "'";//edit
                    value += ",'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',";
                    value += "'" + hdScanFile.Value.ToString() + "',";
                    value += "'" + txtRemark.Text.Replace("'", "''") + "',";
                    value += "null,null";
                    value += ",'" + hdOwnerID.Value.ToString() + "'";
                    value += ",'" + Request.Cookies.Get("eReimUserID").Value + "'";
                    value += ",'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'";
                    value += ",'" + CCMailList + "'";
                    value += ","+budget+"";
                    value += ",'" + ostation + "'";

                    //160123 垫付
                    if (hdOnBehalf.Value != null && hdOnBehalf.Value != "")
                    {
                        value += ",'" + hdOnBehalf.Value.ToString() + "'";
                        DataSet dsE = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(hdOnBehalf.Value.ToString());
                        if (dsE.Tables[0].Rows.Count == 1)
                        {
                            DataTable dtE = dsE.Tables[0];
                            value += ",'" + dtE.Rows[0]["fullName"].ToString() + "'";
                            value += ",'" + dtE.Rows[0]["stationCode"].ToString() + "'";
                            value += ",'" + dtE.Rows[0]["CRPDepartmentName"].ToString() + "'";
                            value += ",'" + dtE.Rows[0]["CostCenter"].ToString() + "'";
                        }
                    }


                    string updatesql = "insert into ETravel (" + word + ") values(" + value + ");update ETravel set [No]=[No]+(select [MonthCount]=right('0000'+cast(count(ID) as varchar(10)),4) from ETravel where (month(ApplyDate) in (select month(ApplyDate) from ETravel where [ID]=@@IDENTITY) and (year(ApplyDate) in (select year(ApplyDate) from ETravel where [ID]=@@IDENTITY)) and Station=(select Station from ETravel where ID=@@IDENTITY)))+'T' where ID=@@IDENTITY;select [msg]=convert(varchar,ID)+','+[No] from ETravel where ID=@@IDENTITY";

                    string newid = dbc.UpdateData("eReimbursement", updatesql, "Insert");

                    string rows = "";
                    //160113 垫付
                    if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                    {
                        string personid = hdOwnerID.Value.ToString();
                        string onbehalfid = hdOnBehalf.Value.ToString();
                        DataTable dtOnbehalf = new DataTable();
                        dtOnbehalf.Columns.Add("FlowNo");
                        dtOnbehalf.Columns.Add("FlowUser");
                        dtOnbehalf.Columns.Add("FlowUserid");
                        dtOnbehalf.Columns.Add("Fn");
                        dtOnbehalf.Columns.Add("PersonID");
                        //被垫付人的Verifier
                        if (dtGroupFlowData.Select("UserID='" + onbehalfid + "' and Fn='Verifier'").Count() >= 1)
                        {
                            DataRow dr = dtOnbehalf.NewRow();
                            DataRow drold = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and Fn='Verifier'")[(dtGroupFlowData.Select("UserID='" + onbehalfid + "' and Fn='Verifier'").Count()) - 1];
                            dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                            dr["FlowUser"] = drold["FlowUser"].ToString();
                            dr["FlowUserid"] = drold["FlowUserid"].ToString();
                            dr["Fn"] = "Verifier";
                            dr["PersonID"] = drold["UserID"].ToString();
                            dtOnbehalf.Rows.Add(dr);
                        }
                        //被垫付人的Apporver
                        if (dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count() >= 1)
                        {
                            int flowcount = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count();
                            if (flowcount >= 2)
                            {
                                DataRow dr = dtOnbehalf.NewRow();
                                DataRow drold = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count()) - 2];
                                dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                                dr["FlowUser"] = drold["FlowUser"].ToString();
                                dr["FlowUserid"] = drold["FlowUserid"].ToString();
                                dr["Fn"] = "Apporver";
                                dr["PersonID"] = drold["UserID"].ToString();
                                dtOnbehalf.Rows.Add(dr);

                                dr = dtOnbehalf.NewRow();
                                drold = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count()) - 1];
                                dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                                dr["FlowUser"] = drold["FlowUser"].ToString();
                                dr["FlowUserid"] = drold["FlowUserid"].ToString();
                                dr["Fn"] = "Apporver";
                                dr["PersonID"] = drold["UserID"].ToString();
                                dtOnbehalf.Rows.Add(dr);
                            }
                            else
                            {
                                DataRow dr = dtOnbehalf.NewRow();
                                DataRow drold = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count()) - 1];
                                dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                                dr["FlowUser"] = drold["FlowUser"].ToString();
                                dr["FlowUserid"] = drold["FlowUserid"].ToString();
                                dr["Fn"] = "Apporver";
                                dr["PersonID"] = drold["UserID"].ToString();
                                dtOnbehalf.Rows.Add(dr);
                            }

                        }
                        //垫付人的Apporver
                        if (dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Apporver' or Fn is null)").Count() >= 1)
                        {
                            DataRow dr = dtOnbehalf.NewRow();
                            DataRow drold = dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Apporver' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Apporver' or Fn is null)").Count()) - 1];
                            dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                            dr["FlowUser"] = drold["FlowUser"].ToString();
                            dr["FlowUserid"] = drold["FlowUserid"].ToString();
                            dr["Fn"] = "Apporver";
                            dr["PersonID"] = drold["UserID"].ToString();
                            dtOnbehalf.Rows.Add(dr);
                        }
                        //垫付人的Issuer
                        if (dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Issuer' or Fn is null)").Count() >= 1)
                        {
                            DataRow dr = dtOnbehalf.NewRow();
                            DataRow drold = dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Issuer' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Issuer' or Fn is null)").Count()) - 1];
                            dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                            dr["FlowUser"] = drold["FlowUser"].ToString();
                            dr["FlowUserid"] = drold["FlowUserid"].ToString();
                            dr["Fn"] = "Issuer";
                            dr["PersonID"] = drold["UserID"].ToString();
                            dtOnbehalf.Rows.Add(dr);
                        }

                        for (int j = 0; j < dtOnbehalf.Rows.Count; j++)
                        {
                            string wordflow = "[RequestID],[Type],[Step],[Status],[Approver],[ApproverID],[FlowFn],PersonID";
                            //if (dtGroupFlowData.Rows[j]["FlowNo"].ToString() == "1")
                            //{
                            //    wordflow += ",[Active]";
                            //}
                            if (j == 0)
                            {
                                wordflow += ",[Active]";
                            }
                            string valueflow = "";
                            valueflow += "'" + newid.Split(',')[0] + "',";
                            valueflow += "'T',";
                            valueflow += dtOnbehalf.Rows[j]["FlowNo"].ToString() + ",";
                            valueflow += "1,";
                            valueflow += "'" + dtOnbehalf.Rows[j]["FlowUser"].ToString() + "',";
                            valueflow += "'" + dtOnbehalf.Rows[j]["FlowUserid"].ToString() + "'";
                            valueflow += ",'" + (dtOnbehalf.Rows[j]["Fn"].ToString() == "" ? "Apporver" : dtOnbehalf.Rows[j]["Fn"].ToString()) + "'";
                            valueflow += ",'" + dtOnbehalf.Rows[j]["PersonID"].ToString() + "'";
                            if (j == 0)
                            {
                                valueflow += ",1";
                            }
                            string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";

                            rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
                        }

                    }
                    else
                    {
                        for (int j = 0; j < dtGroupFlowData.Rows.Count; j++)
                        {
                            string wordflow = "[RequestID],[Type],[Step],[Status],[Approver],[ApproverID],[FlowFn]";
                            if (j == 0)
                            {
                                wordflow += ",[Active]";
                            }
                            string valueflow = "";
                            valueflow += "'" + newid.Split(',')[0] + "',";
                            valueflow += "'T',";
                            valueflow += dtGroupFlowData.Rows[j]["FlowNo"].ToString() + ",";
                            valueflow += "1,";
                            valueflow += "'" + dtGroupFlowData.Rows[j]["FlowUser"].ToString() + "',";
                            valueflow += "'" + dtGroupFlowData.Rows[j]["FlowUserid"].ToString() + "'";
                            valueflow += ",'" + (dtGroupFlowData.Rows[j]["Fn"].ToString() == "" ? "Apporver" : dtGroupFlowData.Rows[j]["Fn"].ToString()) + "'";
                            if (j == 0)
                            {
                                valueflow += ",1";
                            }
                            string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";

                            rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
                        }
                    }

                    //for (int i = 0; i < dtGroupFlowData.Rows.Count; i++)
                    //{
                    //    string wordflow = "[No],[Type],[Station],[Department],[Person],[CreadedBy],[CreatedDate],[Step],[Status],[Approver],[ApproverID],[RequestID],[FlowFn]";
                    //    if (i==0)
                    //    {
                    //        wordflow += ",[Active]";
                    //    }
                    //    string valueflow = "";
                    //    valueflow += "'" + newid.Split(',')[1] + "',";
                    //    valueflow += "'T',";
                    //    valueflow += "'" + station + "',";//
                    //    valueflow += "'" + department + "',";
                    //    valueflow += "'" + hdOwner.Value.ToString() + "',";//申请人
                    //    valueflow += "'" + Request.Cookies.Get("eReimUserName").Value + "',";//填写人
                    //    valueflow += "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',";
                    //    valueflow += dtGroupFlowData.Rows[i]["FlowNo"].ToString() + ",";
                    //    valueflow += "1,";
                    //    valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUser"].ToString() + "',";
                    //    valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUserid"].ToString() + "',";
                    //    valueflow += "'" + newid.Split(',')[0] + "'";
                    //    valueflow += ",'" + (dtGroupFlowData.Rows[i]["Fn"].ToString() == "" ? "Approver" : dtGroupFlowData.Rows[i]["Fn"].ToString()) + "'";
                    //    if (i==0)
                    //    {
                    //        valueflow += ",1";
                    //    }
                    //    string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";
                    //    rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
                    //}
                    if (newid == "-1" || rows == "-1" || rows == "")
                    {
                        ErrorHandle("Data Error.");
                        return;
                    }
                    else
                    {
                        hdTravelRequestID.Value = newid.Split(',')[0];//新增后记录ID
                        hdTravelRequestNo.Value = newid.Split(',')[1];//新增后记录No
                        if (!SaveDetail(detail, header0string, header1string, header2string, CurLocal, dept,CurBudget) || !SendMailNew(dtbudget))//Budget未计入Current,%需重新计算
                        //if (!SaveDetail(detail, header0string, header1string, header2string, Cur, dept) || !SendMail(""))
                        {
                            ErrorHandle("Data Error.");
                            return;
                        }
                        if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                        {
                            Panel3.Title = "差旅费申请单: " + newid.Split(',')[1];
                            UpdateMSG("保存申请单:" + newid.Split(',')[1] + "成功.");
                        }
                        else
                        {
                            Panel3.Title = "Travel Expense Form: " + newid.Split(',')[1];
                            UpdateMSG("Saved Travel Expense Form: " + newid.Split(',')[1] + " successfully.");
                        }

                    }
                    //
                }
                else//由草稿升级为正式申请
                {
                    string updatesql = "update ETravel set [Person]='" + hdOwner.Value.ToString();
                    updatesql += "',[Station]='" + station_applyperson;
                    updatesql += "',[Department]='" + dept_applyperson;
                    updatesql += "',[ReportFile]='" + hdReport.Value.ToString();
                    updatesql += "',[Remark]='" + txtRemark.Text.Replace("'", "''") + "'";
                    updatesql += ",[Attach]='" + hdScanFile.Value.ToString() + "'";
                    updatesql += ",[Type]=0";
                    updatesql += ",[PersonID]='" + hdOwnerID.Value.ToString() + "'";
                    string oldno = hdTravelRequestNo.Value.ToString();
                    string newno = hdTravelRequestNo.Value.ToString().Substring(0, hdTravelRequestNo.Value.ToString().Length - 1);
                    updatesql += ",[No]='" + newno + "',";
                    updatesql += "[CreadedDate]='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',";
                    updatesql += "[CreadedBy]='" + Request.Cookies.Get("eReimUserName").Value + "',";
                    updatesql += "[CreadedByID]='" + Request.Cookies.Get("eReimUserID").Value + "'";
                    updatesql += ",[CCMailList]='" + CCMailList + "'";
                    updatesql += ",[Budget]="+budget+"";
                    updatesql += ",[Station2]='" + ostation + "'";

                    //160123 垫付
                    if (hdOnBehalf.Value != null && hdOnBehalf.Value != "")
                    {
                        updatesql += ",OnBehalfPersonID='" + hdOnBehalf.Value.ToString() + "'";
                        DataSet dsE = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(hdOnBehalf.Value.ToString());
                        if (dsE.Tables[0].Rows.Count == 1)
                        {
                            DataTable dtE = dsE.Tables[0];
                            updatesql += ",OnBehalfPersonName='" + dtE.Rows[0]["fullName"].ToString() + "'";
                            updatesql += ",OnBehalfPersonUnit='" + dtE.Rows[0]["stationCode"].ToString() + "'";
                            updatesql += ",OnBehalfPersonDept='" + dtE.Rows[0]["CRPDepartmentName"].ToString() + "'";
                            updatesql += ",OnBehalfPersonCostCenter='" + dtE.Rows[0]["CostCenter"].ToString() + "'";
                        }
                    }

                    updatesql += " where ID=" + hdTravelRequestID.Value.ToString();

                    string newid = dbc.UpdateData("eReimbursement", updatesql, "Update");

                    //操作Flow表
                    string sqlDeleteEflow = "delete from Eflow where [Type]='T' and [RequestID]='" + hdTravelRequestID.Value.ToString() + "'";
                    string deleterows = dbc.UpdateData("eReimbursement", sqlDeleteEflow, "Update");
                    string rows = "";
                    //for (int i = 0; i < dtGroupFlowData.Rows.Count; i++)
                    //{
                    //    string wordflow = "[No],[Type],[Station],[Department],[Person],[CreadedBy],[CreatedDate],[Step],[Status],[Approver],[ApproverID],[RequestID],[FlowFn]";
                    //    if (i==0)
                    //    {
                    //        wordflow += ",[Active]";
                    //    }
                    //    string valueflow = "";
                    //    valueflow += "'" + newno + "',";
                    //    valueflow += "'T',";
                    //    valueflow += "'" + station + "',";
                    //    valueflow += "'" + department + "',";
                    //    valueflow += "'" + hdOwner.Value.ToString() + "',";
                    //    valueflow += "'" + Request.Cookies.Get("eReimUserName").Value + "',";
                    //    valueflow += "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',";
                    //    valueflow += dtGroupFlowData.Rows[i]["FlowNo"].ToString() + ",";
                    //    valueflow += "1,";
                    //    valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUser"].ToString() + "',";
                    //    valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUserid"].ToString() + "',";
                    //    valueflow += hdTravelRequestID.Value.ToString();
                    //    valueflow += ",'" + (dtGroupFlowData.Rows[i]["Fn"].ToString() == "" ? "Approver" : dtGroupFlowData.Rows[i]["Fn"].ToString()) + "'";
                    //    if (i==0)
                    //    {
                    //        valueflow += ",1";
                    //    }
                    //    string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";
                    //    rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
                    //}
                    //160113 垫付
                    if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                    {
                        string personid = hdOwnerID.Value.ToString();
                        string onbehalfid = hdOnBehalf.Value.ToString();
                        DataTable dtOnbehalf = new DataTable();
                        dtOnbehalf.Columns.Add("FlowNo");
                        dtOnbehalf.Columns.Add("FlowUser");
                        dtOnbehalf.Columns.Add("FlowUserid");
                        dtOnbehalf.Columns.Add("Fn");
                        dtOnbehalf.Columns.Add("PersonID");
                        //被垫付人的Verifier
                        if (dtGroupFlowData.Select("UserID='" + onbehalfid + "' and Fn='Verifier'").Count() >= 1)
                        {
                            DataRow dr = dtOnbehalf.NewRow();
                            DataRow drold = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and Fn='Verifier'")[(dtGroupFlowData.Select("UserID='" + onbehalfid + "' and Fn='Verifier'").Count()) - 1];
                            dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                            dr["FlowUser"] = drold["FlowUser"].ToString();
                            dr["FlowUserid"] = drold["FlowUserID"].ToString();
                            dr["Fn"] = "Verifier";
                            dr["PersonID"] = drold["UserID"].ToString();
                            dtOnbehalf.Rows.Add(dr);
                        }
                        //被垫付人的Apporver
                        if (dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count() >= 1)
                        {
                            int flowcount = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count();
                            if (flowcount >= 2)
                            {
                                DataRow dr = dtOnbehalf.NewRow();
                                DataRow drold = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count()) - 2];
                                dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                                dr["FlowUser"] = drold["FlowUser"].ToString();
                                dr["FlowUserid"] = drold["FlowUserID"].ToString();
                                dr["Fn"] = "Apporver";
                                dr["PersonID"] = drold["UserID"].ToString();
                                dtOnbehalf.Rows.Add(dr);

                                dr = dtOnbehalf.NewRow();
                                drold = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count()) - 1];
                                dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                                dr["FlowUser"] = drold["FlowUser"].ToString();
                                dr["FlowUserid"] = drold["FlowUserID"].ToString();
                                dr["Fn"] = "Apporver";
                                dr["PersonID"] = drold["UserID"].ToString();
                                dtOnbehalf.Rows.Add(dr);
                            }
                            else
                            {
                                DataRow dr = dtOnbehalf.NewRow();
                                DataRow drold = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count()) - 1];
                                dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                                dr["FlowUser"] = drold["FlowUser"].ToString();
                                dr["FlowUserid"] = drold["FlowUserID"].ToString();
                                dr["Fn"] = "Apporver";
                                dr["PersonID"] = drold["UserID"].ToString();
                                dtOnbehalf.Rows.Add(dr);
                            }

                        }
                        //垫付人的Apporver
                        if (dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Apporver' or Fn is null)").Count() >= 1)
                        {
                            DataRow dr = dtOnbehalf.NewRow();
                            DataRow drold = dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Apporver' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Apporver' or Fn is null)").Count()) - 1];
                            dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                            dr["FlowUser"] = drold["FlowUser"].ToString();
                            dr["FlowUserid"] = drold["FlowUserID"].ToString();
                            dr["Fn"] = "Apporver";
                            dr["PersonID"] = drold["UserID"].ToString();
                            dtOnbehalf.Rows.Add(dr);
                        }
                        //垫付人的Issuer
                        if (dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Issuer' or Fn is null)").Count() >= 1)
                        {
                            DataRow dr = dtOnbehalf.NewRow();
                            DataRow drold = dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Issuer' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Issuer' or Fn is null)").Count()) - 1];
                            dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                            dr["FlowUser"] = drold["FlowUser"].ToString();
                            dr["FlowUserid"] = drold["FlowUserID"].ToString();
                            dr["Fn"] = "Issuer";
                            dr["PersonID"] = drold["UserID"].ToString();
                            dtOnbehalf.Rows.Add(dr);
                        }
                        for (int j = 0; j < dtOnbehalf.Rows.Count; j++)
                        {
                            string wordflow = "[RequestID],[Type],[Step],[Status],[Approver],[ApproverID],[FlowFn],PersonID";
                            //if (dtGroupFlowData.Rows[j]["FlowNo"].ToString() == "1")
                            //{
                            //    wordflow += ",[Active]";
                            //}
                            if (j == 0)
                            {
                                wordflow += ",[Active]";
                            }
                            string valueflow = "";
                            valueflow += "'" + hdTravelRequestID.Value.ToString() + "',";
                            valueflow += "'T',";
                            valueflow += dtOnbehalf.Rows[j]["FlowNo"].ToString() + ",";
                            valueflow += "1,";
                            valueflow += "'" + dtOnbehalf.Rows[j]["FlowUser"].ToString() + "',";
                            valueflow += "'" + dtOnbehalf.Rows[j]["FlowUserid"].ToString() + "'";
                            valueflow += ",'" + (dtOnbehalf.Rows[j]["Fn"].ToString() == "" ? "Apporver" : dtOnbehalf.Rows[j]["Fn"].ToString()) + "'";
                            valueflow += ",'" + dtOnbehalf.Rows[j]["PersonID"].ToString() + "'";
                            //if (dtGroupFlowData.Rows[j]["FlowNo"].ToString() == "1")
                            //{
                            //    valueflow += ",1";
                            //}
                            if (j == 0)
                            {
                                valueflow += ",1";
                            }
                            string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";

                            rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
                        }

                    }
                    else
                    {
                        for (int j = 0; j < dtGroupFlowData.Rows.Count; j++)
                        {
                            string wordflow = "[RequestID],[Type],[Step],[Status],[Approver],[ApproverID],[FlowFn]";
                            //if (dtGroupFlowData.Rows[j]["FlowNo"].ToString() == "1")
                            //{
                            //    wordflow += ",[Active]";
                            //}
                            if (j == 0)
                            {
                                wordflow += ",[Active]";
                            }
                            string valueflow = "";
                            valueflow += "'" + hdTravelRequestID.Value.ToString() + "',";
                            valueflow += "'T',";
                            valueflow += dtGroupFlowData.Rows[j]["FlowNo"].ToString() + ",";
                            valueflow += "1,";
                            valueflow += "'" + dtGroupFlowData.Rows[j]["FlowUser"].ToString() + "',";
                            valueflow += "'" + dtGroupFlowData.Rows[j]["FlowUserid"].ToString() + "'";
                            valueflow += ",'" + (dtGroupFlowData.Rows[j]["Fn"].ToString() == "" ? "Apporver" : dtGroupFlowData.Rows[j]["Fn"].ToString()) + "'";
                            //if (dtGroupFlowData.Rows[j]["FlowNo"].ToString() == "1")
                            //{
                            //    valueflow += ",1";
                            //}
                            if (j == 0)
                            {
                                valueflow += ",1";
                            }
                            string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";

                            rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
                        }
                    }

                    //
                    if (newid == "-1" || rows == "-1" || rows == "")
                    {
                        ErrorHandle("Data Error.");
                        return;
                    }
                    else
                    {
                        hdTravelRequestNo.Value = newno;
                        if (!SaveDetail(detail, header0string, header1string, header2string, CurLocal, dept, CurBudget) || !SendMailNew(dtbudget))
                        //if (!SaveDetail(detail, header0string, header1string, header2string, Cur, dept) || !SendMail(warningmsg))
                        {
                            ErrorHandle("Data Error.");
                            return;
                        }


                        if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                        {
                            Panel3.Title = "差旅费申请单:" + newno;
                            UpdateMSG("保存差旅费申请单:" + hdTravelRequestNo.Value.ToString() + "成功.");
                        }
                        else
                        {
                            Panel3.Title = "Travel Expense Form: " + newno;
                            UpdateMSG("Saved Travel Expense Form: " + hdTravelRequestNo.Value.ToString() + " successfully.");
                        }

                    }
                }
                X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();btnSaveDraft.disable();btnSaveAndSend.disable();btnExport.enable();btnCC.disable();");
                //X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();btnSaveDraft.disable();btnSaveAndSend.disable();cbxBudget.setReadOnly(true);btnExport.enable();btnCC.disable();");
            }
            else//保存草稿
            {
                if (hdTravelRequestID.Value.ToString() != "")//由链接进入的草稿更新
                {
                    string updatesql = "update ETravel set [Person]='" + hdOwner.Value.ToString();
                    updatesql += "',[Station]='" + station_applyperson;
                    updatesql += "',[Department]='" + dept_applyperson;
                    updatesql += "',[ReportFile]='" + hdReport.Value.ToString();
                    updatesql += "',[Remark]='" + txtRemark.Text.Replace("'", "''") + "'";
                    updatesql += ",[Attach]='" + hdScanFile.Value.ToString() + "'";
                    updatesql += ",[PersonID]='" + hdOwnerID.Value.ToString() + "'";
                    updatesql += ",[CCMailList]='" + CCMailList + "'";
                    updatesql += ",[Budget]="+budget+"";
                    updatesql += ",[Station2]='" + ostation + "'";
                    //160123 垫付
                    if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                    {
                        updatesql += ",OnBehalfPersonID='" + hdOnBehalf.Value.ToString() + "'";
                        DataSet dsE = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(hdOnBehalf.Value.ToString());
                        if (dsE.Tables[0].Rows.Count == 1)
                        {
                            DataTable dtE = dsE.Tables[0];
                            updatesql += ",OnBehalfPersonName='" + dtE.Rows[0]["fullName"].ToString() + "'";
                            updatesql += ",OnBehalfPersonUnit='" + dtE.Rows[0]["stationCode"].ToString() + "'";
                            updatesql += ",OnBehalfPersonDept='" + dtE.Rows[0]["CRPDepartmentName"].ToString() + "'";
                            updatesql += ",OnBehalfPersonCostCenter='" + dtE.Rows[0]["CostCenter"].ToString() + "'";
                        }
                    }
                    updatesql += " where ID=" + hdTravelRequestID.Value.ToString();

                    string newid = dbc.UpdateData("eReimbursement", updatesql, "Update");

                    if (newid == "-1")
                    {
                        ErrorHandle("Data Error.");
                        return;
                    }
                    else
                    {
                        if (!SaveDetail(detail, header0string, header1string, header2string, CurLocal, dept, CurBudget))
                        {
                            ErrorHandle("Data Error.");
                            return;
                        }
                        //hdTravelRequestID.Value = newid.Split(',')[0];//新增后记录ID
                        //Panel3.Title = "差旅费申请单:" + newid.Split(',')[1];
                        //X.AddScript("GridPanel2.submitData();");
                        if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                        {
                            UpdateMSG("保存差旅费申请单草稿: " + hdTravelRequestNo.Value.ToString() + "成功.");
                        }
                        else
                        {
                            UpdateMSG("Saved Travel Expense Draft: " + hdTravelRequestNo.Value.ToString() + " successfully.");
                        }
                    }

                }
                else//如果ID为空则判断为新增草稿
                {
                    string word = "[No],[Person],[Station],[Department],[ReportFile],[Attach],[Remark],[Bdate],[Edate],[Type],[PersonID],[ApplyDate],[CCMailList],[Station2],[Budget]";
                    //160123 垫付
                    if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                    { word += ",OnBehalfPersonID,OnBehalfPersonName,OnBehalfPersonUnit,OnBehalfPersonDept,OnBehalfPersonCostCenter"; }
                    string value = "";
                    value += "'" + station_applyperson + DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString().PadLeft(2, '0') + "',";//edit
                    value += "'" + hdOwner.Value.ToString() + "',"; value += "'" + station_applyperson + "',"; value += "'" + dept_applyperson + "',";//edit
                    value += "'" + hdReport.Value.ToString() + "',";
                    value += "'" + hdScanFile.Value.ToString() + "',";
                    value += "'" + txtRemark.Text.Replace("'", "''") + "',";
                    value += "null,null,";
                    value += "1";//标识为草稿
                    value += ",'" + hdOwnerID.Value.ToString() + "'";
                    value += ",'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'";
                    value += ",'" + CCMailList + "'";
                    value += ",'" + ostation + "'";
                    value += ","+budget+"";
                    if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                    {
                        value += ",'" + hdOnBehalf.Value.ToString() + "'";
                        DataSet dsE = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(hdOnBehalf.Value.ToString());
                        if (dsE.Tables[0].Rows.Count == 1)
                        {
                            DataTable dtE = dsE.Tables[0];
                            value += ",'" + dtE.Rows[0]["fullName"].ToString() + "'";
                            value += ",'" + dtE.Rows[0]["stationCode"].ToString() + "'";
                            value += ",'" + dtE.Rows[0]["CRPDepartmentName"].ToString() + "'";
                            value += ",'" + dtE.Rows[0]["CostCenter"].ToString() + "'";
                        }
                    }
                    string updatesql = "insert into ETravel (" + word + ") values(" + value + ");update ETravel set [No]=[No]+(select [MonthCount]=right('0000'+cast(count(ID) as varchar(10)),4) from ETravel where (month(ApplyDate) in (select month(ApplyDate) from ETravel where [ID]=@@IDENTITY) and (year(ApplyDate) in (select year(ApplyDate) from ETravel where [ID]=@@IDENTITY)) and Station=(select Station from ETravel where ID=@@IDENTITY)))+'TD' where ID=@@IDENTITY;select [msg]=convert(varchar,ID)+','+[No] from ETravel where ID=@@IDENTITY";

                    string newid = dbc.UpdateData("eReimbursement", updatesql, "Insert");

                    //操作Flow表
                    string wordflow = "[No],[Type],[Station],[Department],[Person],[CreadedBy],[CreatedDate],[RequestID],[Active]";
                    string valueflow = "";
                    valueflow += "'" + newid.Split(',')[1] + "',";
                    valueflow += "'T',";
                    valueflow += "'" + station_applyperson + "',";
                    valueflow += "'" + dept_applyperson + "',";
                    valueflow += "'" + hdOwner.Value.ToString() + "',";
                    valueflow += "'" + Request.Cookies.Get("eReimUserName").Value + "',";
                    valueflow += "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',";
                    valueflow += "'" + newid.Split(',')[0] + "'";
                    valueflow += ",1";
                    string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";
                    string rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
                    //
                    if (newid == "-1" || rows == "-1")
                    {
                        ErrorHandle("Data Error."); return;
                    }
                    else
                    {
                        hdTravelRequestID.Value = newid.Split(',')[0];//新增后记录ID
                        hdTravelRequestNo.Value = newid.Split(',')[1];//新增后记录No
                        if (!SaveDetail(detail, header0string, header1string, header2string, CurLocal, dept, CurBudget))
                        {
                            ErrorHandle("Data Error.");
                            return;
                        }
                        if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                        {
                            Panel3.Title = "差旅费申请单草稿: " + newid.Split(',')[1];
                            UpdateMSG("新增差旅费申请单草稿: " + newid.Split(',')[1] + "成功.");
                        }
                        else
                        {
                            Panel3.Title = "Travel Expense Draft: " + newid.Split(',')[1];
                            UpdateMSG("Added Travel Expense Draft: " + newid.Split(',')[1] + " successfully.");
                        }
                    }
                }
            }
            X.AddScript("btnGeteLeave.disable();cbxOnBehalfName.disable();cbxPerson.disable();");
        }
        [DirectMethod]
        public void LoadBudget(string detail, string header1string, string header2string)
        {
            if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
            {
                X.Msg.Alert("Message", "Forbidden to load other user's budget.").Show();
                return;
            }
            bool copysuc=false;
            if (Request.QueryString["Copy"] != null)
            {
                if (Request.QueryString["Copy"].ToString() == "T")//Copy而已,作为新增
                {
                    copysuc = true;
                }
            }
            DataTable dtbudget = new DataTable();
            dtbudget.Columns.Add("EName", typeof(System.String));
            dtbudget.Columns.Add("COACode", typeof(System.String));
            dtbudget.Columns.Add("Current", typeof(System.String));
            dtbudget.Columns.Add("PU", typeof(System.String));
            dtbudget.Columns.Add("PB", typeof(System.String));
            dtbudget.Columns.Add("PPercent", typeof(System.String));
            dtbudget.Columns.Add("DU", typeof(System.String));
            dtbudget.Columns.Add("DB", typeof(System.String));
            dtbudget.Columns.Add("DPercent", typeof(System.String));
            dtbudget.Columns.Add("SU", typeof(System.String));
            dtbudget.Columns.Add("SB", typeof(System.String));
            dtbudget.Columns.Add("SPercent", typeof(System.String));

            //StoreBudget添加Field
            //StoreBudget.Reader[0].Fields.Clear();
            StoreBudget.Reader[0].Fields.Add("EName", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("COACode", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("Current", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("PU", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("PB", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("PPercent", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("DU", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("DB", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("DPercent", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("SU", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("SB", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("SPercent", RecordFieldType.String);

            DataTable dtA = new DataTable();
            dtA.Columns.Add("COACode", typeof(System.String));
            dtA.Columns.Add("Amount", typeof(System.Decimal));
            dtA.Columns.Add("Date", typeof(System.DateTime));

            StoreSubmitDataEventArgs eSubmit = new StoreSubmitDataEventArgs(detail, null);
            XmlNode xml = eSubmit.Xml;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.InnerXml);
            int dtcol = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes.Count;
            int dtrow = doc.SelectNodes("records").Item(0).SelectNodes("record").Count;
            int groupcount = (dtcol - 3) / 2;//多少个站点会被更新,如果站点为空则不更新

            for (int i = 0; i < groupcount; i++)
            {
                //1. Air Ticket - Int'l
                if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[2 + i * 2].InnerText) != 0))
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62012000";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[1 + i * 2].InnerText);
                    decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[2 + i * 2].InnerText);
                    drnew["Amount"] = r1 + r2;
                    dtA.Rows.Add(drnew);
                }
                else
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62012000";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    drnew["Amount"] = 0;
                    dtA.Rows.Add(drnew);
                }
                //Domestic
                if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[2 + i * 2].InnerText) != 0))
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62012000";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[1 + i * 2].InnerText);
                    decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[2 + i * 2].InnerText);
                    drnew["Amount"] = r1 + r2;
                    dtA.Rows.Add(drnew);
                }
                else
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62012000";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    drnew["Amount"] = 0;
                    dtA.Rows.Add(drnew);
                }
                //2. Hotel Bill
                if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[2 + i * 2].InnerText) != 0))
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62012000";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[1 + i * 2].InnerText);
                    decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[2 + i * 2].InnerText);
                    drnew["Amount"] = r1 + r2;
                    dtA.Rows.Add(drnew);
                }
                else
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62012000";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    drnew["Amount"] = 0;
                    dtA.Rows.Add(drnew);
                }
                //3. Meals
                if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[2 + i * 2].InnerText) != 0))
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62012000";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[1 + i * 2].InnerText);
                    decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[2 + i * 2].InnerText);
                    drnew["Amount"] = r1 + r2;
                    dtA.Rows.Add(drnew);
                }
                else
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62012000";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    drnew["Amount"] = 0;
                    dtA.Rows.Add(drnew);
                }
                //4. Entertainment
                if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[2 + i * 2].InnerText) != 0))
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62010900";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[1 + i * 2].InnerText);
                    decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[2 + i * 2].InnerText);
                    drnew["Amount"] = r1 + r2;
                    dtA.Rows.Add(drnew);
                }
                else
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62010900";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    drnew["Amount"] = 0;
                    dtA.Rows.Add(drnew);
                }
                //5. Car Rental/Transportation
                if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[2 + i * 2].InnerText) != 0))
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62011900";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[1 + i * 2].InnerText);
                    decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[2 + i * 2].InnerText);
                    drnew["Amount"] = r1 + r2;
                    dtA.Rows.Add(drnew);
                }
                else
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62011900";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    drnew["Amount"] = 0;
                    dtA.Rows.Add(drnew);
                }
                //6. Communication
                if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[2 + i * 2].InnerText) != 0))
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62010500";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[1 + i * 2].InnerText);
                    decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[2 + i * 2].InnerText);
                    drnew["Amount"] = r1 + r2;
                    dtA.Rows.Add(drnew);
                }
                else
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62010500";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    drnew["Amount"] = 0;
                    dtA.Rows.Add(drnew);
                }
                //7. Local Trip
                if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[2 + i * 2].InnerText) != 0))
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62012000";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[1 + i * 2].InnerText);
                    decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[2 + i * 2].InnerText);
                    drnew["Amount"] = r1 + r2;
                    dtA.Rows.Add(drnew);
                }
                else
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62012000";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    drnew["Amount"] = 0;
                    dtA.Rows.Add(drnew);
                }
                //8. Overseas Trip USD15/day
                if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[2 + i * 2].InnerText) != 0))
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62012000";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[1 + i * 2].InnerText);
                    decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[2 + i * 2].InnerText);
                    drnew["Amount"] = r1 + r2;
                    dtA.Rows.Add(drnew);
                }
                else
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62012000";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    drnew["Amount"] = 0;
                    dtA.Rows.Add(drnew);
                }
                //9. Airport Tax/Travel Insurance
                if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[2 + i * 2].InnerText) != 0))
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62012000";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[1 + i * 2].InnerText);
                    decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[2 + i * 2].InnerText);
                    drnew["Amount"] = r1 + r2;
                    dtA.Rows.Add(drnew);
                }
                else
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62012000";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    drnew["Amount"] = 0;
                    dtA.Rows.Add(drnew);
                }
                //10. Others
                if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[2 + i * 2].InnerText) != 0))
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62012000";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[1 + i * 2].InnerText);
                    decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[2 + i * 2].InnerText);
                    drnew["Amount"] = r1 + r2;
                    dtA.Rows.Add(drnew);
                }
                else
                {
                    DataRow drnew = dtA.NewRow();
                    drnew["COACode"] = "62012000";
                    drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                    drnew["Amount"] = 0;
                    dtA.Rows.Add(drnew);
                }
            }
            //合计
            DataTable dtB = new DataTable();
            dtB.Columns.Add("COACode", typeof(System.String));
            dtB.Columns.Add("Amount", typeof(System.Decimal));
            DataRow dr1 = dtB.NewRow();
            dr1["COACode"] = "62012000";
            dr1["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62012000").ToString());
            dtB.Rows.Add(dr1);
            DataRow dr2 = dtB.NewRow();
            dr2["COACode"] = "62010900";
            dr2["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62010900").ToString());
            dtB.Rows.Add(dr2);
            DataRow dr3 = dtB.NewRow();
            dr3["COACode"] = "62011900";
            dr3["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62011900").ToString());
            dtB.Rows.Add(dr3);
            DataRow dr4 = dtB.NewRow();
            dr4["COACode"] = "62010500";
            dr4["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62010500").ToString());
            dtB.Rows.Add(dr4);

            //if (dtA.Compute("Sum(Amount)", "COACode = 62012000").ToString() != "" && Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62012000").ToString()) != 0)
            //{
            //    DataRow dr = dtB.NewRow();
            //    dr["COACode"] = "62012000";
            //    dr["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62012000").ToString());
            //    dtB.Rows.Add(dr);
            //}
            //if (dtA.Compute("Sum(Amount)", "COACode = 62010900").ToString() != "" && Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62010900").ToString()) != 0)
            //{
            //    DataRow dr = dtB.NewRow();
            //    dr["COACode"] = "62010900";
            //    dr["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62010900").ToString());
            //    dtB.Rows.Add(dr);
            //}
            //if (dtA.Compute("Sum(Amount)", "COACode = 62011900").ToString() != "" && Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62011900").ToString()) != 0)
            //{
            //    DataRow dr = dtB.NewRow();
            //    dr["COACode"] = "62011900";
            //    dr["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62011900").ToString());
            //    dtB.Rows.Add(dr);
            //}
            //if (dtA.Compute("Sum(Amount)", "COACode = 62010500").ToString() != "" && Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62010500").ToString()) != 0)
            //{
            //    DataRow dr = dtB.NewRow();
            //    dr["COACode"] = "62010500";
            //    dr["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62010500").ToString());
            //    dtB.Rows.Add(dr);
            //}
            //160113 垫付人
            string userid1 = (hdOnBehalf.Value == null || hdOnBehalf.Value.ToString() == "") ? hdOwnerID.Value.ToString() : hdOnBehalf.Value.ToString();
            string ostation = "";
            string station = "";

            string station_applyperson = ""; string costcenter_applyperson = ""; string dept_applyperson = "";
            DataSet ds_apply = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(hdOwnerID.Value.ToString());
            if (ds_apply.Tables[0].Rows.Count == 1)
            {
                DataTable dt_apply = ds_apply.Tables[0];
                dept_applyperson = dt_apply.Rows[0]["DepartmentName"].ToString();
                station_applyperson = dt_apply.Rows[0]["stationCode"].ToString();
                costcenter_applyperson = dt_apply.Rows[0]["CostCenter"].ToString();
            }

            DataSet ds2 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(userid1);
            if (ds2.Tables[0].Rows.Count == 1)
            {
                DataTable dt1 = ds2.Tables[0];
                //dpt = dt1.Rows[0]["DepartmentName"].ToString();
                station = dt1.Rows[0]["stationCode"].ToString();
                ostation = dt1.Rows[0]["CostCenter"].ToString();
            }

            decimal rate = 1;//记录用户币种与预算站点币种汇率
            string CurLocal = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(costcenter_applyperson);
            //检查是否本地维护过特殊币种
            cs.DBCommand dbc = new cs.DBCommand();
            DataTable dttemp = new DataTable();
            string sqltemp = "select * from ESUSER where Userid='" + hdOwnerID.Value.ToString() + "'";
            dttemp = dbc.GetData("eReimbursement", sqltemp);
            if (dttemp.Rows.Count > 0)
            {
                CurLocal = dttemp.Rows[0]["Currency"].ToString();//如果单独设置了币种
            }
            string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(ostation);

            rate = GetRateByUserID(Convert.ToDateTime(dtA.Compute("Min(Date)", "").ToString()).Year, false, CurLocal,CurBudget);
            //计算本地币种与预算站点币种汇率
            //if (CurLocal != CurBudget)
            //{
            //    rate = DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(CurBudget, CurLocal, Convert.ToDateTime(dtA.Compute("Min(Date)", "").ToString()).Year);
            //}
            ////取得传递预算的参数
            //string userid = dt.Rows[0]["PersonID"].ToString();
            //string dpt = dt.Rows[0]["Department"].ToString();
            //string ostation = dt.Rows[0]["CostCenter"].ToString();//预算站点,与基本信息中的CostCenter一致(Station2)
            //string CDate = dtA.Compute("Min(Date)", "").ToString();//记录预算日期
            DataTable dtuser = GetUserInfo(hdOwnerID.Value.ToString());
            string userid = hdOwnerID.Value.ToString();
            string department = dtuser.Rows[0]["Department"].ToString();
            string tstation = dtuser.Rows[0]["CostCenter"].ToString();//Etravel表中的Station2,目前与预算站点一致,不允许更改
            string year = Convert.ToDateTime(dtA.Compute("Min(Date)", "").ToString()).Year.ToString();
            string month = Convert.ToDateTime(dtA.Compute("Min(Date)", "").ToString()).Month.ToString();
            string accountcode = "";
            for (int g = 0; g < dtB.Rows.Count; g++)
            {
                DataRow dr = dtbudget.NewRow();
                dr["Current"] = dtB.Rows[g]["Amount"].ToString();
                dr["COACode"] = dtB.Rows[g]["COACode"].ToString();
                accountcode = dtB.Rows[g]["COACode"].ToString();
                DataTable dtC = new DataTable();
                dtC = Comm.RtnEB(userid, department, ostation, ostation, accountcode, year, month);
                for (int i = 0; i < dtC.Rows.Count; i++)
                {
                    if (dtC.Rows[i]["Type"].ToString() == "全年个人")
                    {
                        dr["PU"] = dtC.Rows[i]["Used"].ToString();
                        dr["PB"] = dtC.Rows[i]["Budget"].ToString();
                    }
                    else if (dtC.Rows[i]["Type"].ToString() == "全年部门")
                    {
                        dr["DU"] = dtC.Rows[i]["Used"].ToString();
                        dr["DB"] = dtC.Rows[i]["Budget"].ToString();
                    }
                    else if (dtC.Rows[i]["Type"].ToString() == "全年站点")
                    {
                        dr["SU"] = dtC.Rows[i]["Used"].ToString();
                        dr["SB"] = dtC.Rows[i]["Budget"].ToString();
                    }
                }
                dtbudget.Rows.Add(dr);
            }
            //bool PB = false, DB = false, SB = false;
            //计算%,取得名称,转为本地币种汇率
            for (int i = 0; i < dtbudget.Rows.Count; i++)
            {
                if (Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()) != 0)//如果Budget不为0,则计算%
                {
                    decimal PPercent = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["PU"].ToString()) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()), 2);
                    dtbudget.Rows[i]["PPercent"] = System.Math.Round(PPercent,2).ToString();
                }
                else
                {
                    dtbudget.Rows[i]["PB"] = "--";
                    dtbudget.Rows[i]["PPercent"] = "--";
                }
                if (Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()) != 0)//如果Budget不为0,则计算%
                {
                    decimal DPercent = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["DU"].ToString()) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()), 2);
                    dtbudget.Rows[i]["DPercent"] = System.Math.Round(DPercent,2).ToString();
                }
                else
                {
                    dtbudget.Rows[i]["DB"] = "--";
                    dtbudget.Rows[i]["DPercent"] = "--";
                }
                if (Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()) != 0)//如果Budget不为0,则计算%
                {
                    decimal SPercent = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["SU"].ToString()) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()), 2);
                    dtbudget.Rows[i]["SPercent"] = System.Math.Round(SPercent, 2).ToString();
                }
                else
                {
                    dtbudget.Rows[i]["SB"] = "--";
                    dtbudget.Rows[i]["SPercent"] = "--";
                }
                if (dtbudget.Rows[i]["COACode"].ToString() == "62012000")
                {
                    dtbudget.Rows[i]["EName"] = "Travel expense";
                }
                else if (dtbudget.Rows[i]["COACode"].ToString() == "62010900")
                {
                    dtbudget.Rows[i]["EName"] = "Entertainment";
                }
                else if (dtbudget.Rows[i]["COACode"].ToString() == "62011900")
                {
                    dtbudget.Rows[i]["EName"] = "Transportation";
                }
                else if (dtbudget.Rows[i]["COACode"].ToString() == "62010500")
                {
                    dtbudget.Rows[i]["EName"] = "Communication";
                }
                dtbudget.Rows[i]["PU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["PU"].ToString()), 2).ToString();
                if (dtbudget.Rows[i]["PB"].ToString()!="--")
                {
                    dtbudget.Rows[i]["PB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()), 2).ToString();
                }

                dtbudget.Rows[i]["DU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["DU"].ToString()), 2).ToString();
                if (dtbudget.Rows[i]["DB"].ToString()!="--")
                {
                    dtbudget.Rows[i]["DB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()), 2).ToString();
                }

                dtbudget.Rows[i]["SU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["SU"].ToString()), 2).ToString();
                if (dtbudget.Rows[i]["SB"].ToString()!="--")
                {
                    dtbudget.Rows[i]["SB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()), 2).ToString();
                }
                
            }
            //添加数据列
            var cm = GridPanelBudget.ColumnModel;
            cm.Columns.Clear();
            cm.Columns.Add(new Column
            {
                DataIndex = "EName",
                Header = "Expense Item",
                Sortable = false,
                Resizable = false,
                MenuDisabled = true,
                Width = 100
            });
            cm.Columns.Add(new Column
            {
                DataIndex = "Current",
                Header = "Current",
                Renderer = new Renderer { Fn = "GetNumber" },
                Sortable = false,
                Resizable = false,
                MenuDisabled = true,
                Width = 100
            });
            //显示个人预算部分
            cm.Columns.Add(new Column
            {
                DataIndex = "PU",
                Header = "Personal Used",
                Renderer = new Renderer { Fn = "GetNumber" },
                Sortable = false,
                Resizable = false,
                MenuDisabled = true,
                Width = 100
            });
            cm.Columns.Add(new Column
            {
                DataIndex = "PB",
                Header = "Personal Budget",
                Renderer = new Renderer { Fn = "GetNumber" },
                Sortable = false,
                Resizable = false,
                MenuDisabled = true,
                Width = 100
            });
            cm.Columns.Add(new Column
            {
                DataIndex = "PPercent",
                Header = "%(Used/Budget)",
                Renderer = new Renderer { Fn = "GetNumberPercent" },
                Sortable = false,
                Resizable = false,
                MenuDisabled = true,
                Width = 100
            });
            cm.Columns.Add(new Column
            {
                DataIndex = "DU",
                Header = "Department Used",
                Renderer = new Renderer { Fn = "GetNumber" },
                Sortable = false,
                Resizable = false,
                MenuDisabled = true,
                Width = 100
            });
            cm.Columns.Add(new Column
            {
                DataIndex = "DB",
                Header = "Department Budget",
                Renderer = new Renderer { Fn = "GetNumber" },
                Sortable = false,
                Resizable = false,
                MenuDisabled = true,
                Width = 100
            });
            cm.Columns.Add(new Column
            {
                DataIndex = "DPercent",
                Header = "%(Used/Budget)",
                Renderer = new Renderer { Fn = "GetNumberPercent" },
                Sortable = false,
                Resizable = false,
                MenuDisabled = true,
                Width = 100
            });
            cm.Columns.Add(new Column
            {
                DataIndex = "SU",
                Header = "Unit Used",
                Renderer = new Renderer { Fn = "GetNumber" },
                Sortable = false,
                Resizable = false,
                MenuDisabled = true,
                Width = 100
            });
            cm.Columns.Add(new Column
            {
                DataIndex = "SB",
                Header = "Unit Budget",
                Renderer = new Renderer { Fn = "GetNumber" },
                Sortable = false,
                Resizable = false,
                MenuDisabled = true,
                Width = 100
            });
            cm.Columns.Add(new Column
            {
                DataIndex = "SPercent",
                Header = "%(Used/Budget)",
                Renderer = new Renderer { Fn = "GetNumberPercent" },
                Sortable = false,
                Resizable = false,
                MenuDisabled = true,
                Width = 100
            });
            StoreBudget.DataSource = dtbudget;
            StoreBudget.DataBind();
            GridPanelBudget.Render();
            //GridPanelBudget.Reconfigure();

        }
        [DirectMethod]
        public void SaveAll(string type, string detail, string MailList, string header0string, string header1string, string header2string, string Cur, string dept)
        {
            //检查是否登录超时
            if (Request.Cookies.Get("eReimUserID") == null || hdUser.Value.ToString() != Request.Cookies.Get("eReimUserID").Value)
            {
                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                {
                    X.AddScript("Ext.Msg.show({ title: '提示', msg: '已切换用户,将刷新页面.', buttons: { ok: 'Ok' }, fn: function (btn) { window.location.reload(); } });");
                }
                else
                {
                    X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Current user changed,reloading...', buttons: { ok: 'Ok' }, fn: function (btn) { window.location.reload(); } });");
                }
                return;
            }
            cs.DBCommand dbc = new cs.DBCommand();

            //处理抄送人列表
            string CCMailList = "";
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<CCMailList> CCMailList1 = ser.Deserialize<List<CCMailList>>(MailList);
            foreach (CCMailList mail in CCMailList1)
            {
                CCMailList += mail.Email + ",";
            }
            CCMailList = CCMailList.Length > 0 ? CCMailList.Substring(0, CCMailList.Length - 1) : "";

            //string userid = hdOwnerID.Value.ToString();
            //string ostation = ""; string station = ""; string department = "";

            //160113 垫付人
            string userid = (hdOnBehalf.Value == null || hdOnBehalf.Value.ToString() == "") ? hdOwnerID.Value.ToString() : hdOnBehalf.Value.ToString();
            string ostation = "";
            string station = ""; string department = "";

            //申请人 station、costcenter、dept
            string station_applyperson = ""; string costcenter_applyperson = ""; string dept_applyperson = "";
            DataSet ds_apply = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(hdOwnerID.Value.ToString());
            if (ds_apply.Tables[0].Rows.Count == 1)
            {
                DataTable dt_apply = ds_apply.Tables[0];
                dept_applyperson = dt_apply.Rows[0]["DepartmentName"].ToString();
                station_applyperson = dt_apply.Rows[0]["stationCode"].ToString();
                costcenter_applyperson = dt_apply.Rows[0]["CostCenter"].ToString();
            }


            DataSet ds2 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(userid);
            if (ds2.Tables[0].Rows.Count == 1)
            {
                DataTable dt1 = ds2.Tables[0];
                //dpt = dt1.Rows[0]["DepartmentName"].ToString();
                ostation = dt1.Rows[0]["CostCenter"].ToString();//记录用户预算站点,即CostCenter
                station = dt1.Rows[0]["stationCode"].ToString();//记录用户所在站点
                department = dt1.Rows[0]["CRPDepartmentName"].ToString();
                //DataTable dttemp = new DataTable();
                //string sqltemp = "select * from ESUSER where Userid='" + userid + "'";
                //dttemp = dbc.GetData("eReimbursement", sqltemp);
                //if (dttemp.Rows.Count > 0)
                //{
                //    ostation = dttemp.Rows[0]["Station"].ToString();
                //}
            }

            decimal rate = 1;//记录用户币种与预算站点币种汇率
            string CurLocal = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(costcenter_applyperson);
            //检查是否本地维护过特殊币种
            DataTable dttemp = new DataTable();
            string sqltemp = "select * from ESUSER where Userid='" + hdOwnerID.Value.ToString() + "'";
            dttemp = dbc.GetData("eReimbursement", sqltemp);
            if (dttemp.Rows.Count > 0)
            {
                CurLocal = dttemp.Rows[0]["Currency"].ToString();//如果单独设置了币种
                //LocalCurrency = dttemp.Rows[0]["Currency"].ToString();//如果单独设置了币种
            }
            string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(ostation);

            string para = type;
            for (int i = 0; i < header0string.Split(',').Length; i++)
            {
                //判断出差站点名称是否合法
                //if (header0string.Split(',')[i] != "NA")//站点为空则不更新
                //{
                //    bool isstation = DIMERCO.SDK.Utilities.LSDK.isStationExist(header0string.Split(',')[i]);
                //    if (!isstation)//如果不是合法站点,提示可选站点
                //    {
                //        DataSet GetCityInfo = DIMERCO.SDK.Utilities.LSDK.GetCityInfo(header0string.Split(',')[i], 5);
                //        DataTable dtGetCityInfo = (DataTable)GetCityInfo.Tables[0];
                //        string suggestStation = "";
                //        for (int j = 0; j < dtGetCityInfo.Rows.Count; j++)
                //        {
                //            suggestStation += dtGetCityInfo.Rows[j]["CityCode"].ToString() + ",";
                //        }
                //        if (suggestStation.Trim()!="")
                //        {
                //            suggestStation = suggestStation.Substring(0, suggestStation.Length - 1);
                //            X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Invalid DSTN(" + header0string.Split(',')[i] + ").Suggestion:" + suggestStation + ".', buttons: { ok: 'Ok' }, fn: function (btn) {  } });");
                //            return;
                //        }
                //        else
                //        {
                //            X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Please input valid DSTN.', buttons: { ok: 'Ok' }, fn: function (btn) {  } });");
                //            return;
                //        }
                //    }
                //}
                //判断成本中心名称是否合法
                if (header0string.Split(',')[i] != "NA" && header1string.Split(',')[i]!="NA")//站点为空则不更新
                {
                    bool isstation = DIMERCO.SDK.Utilities.LSDK.isCostCenterExist(header1string.Split(',')[i]);
                    if (!isstation)//如果不是合法站点,提示可选站点
                    {
                        DataSet GetCityInfo = DIMERCO.SDK.Utilities.LSDK.getCostCenterBYStationCode(header1string.Split(',')[i], 5);
                        DataTable dtGetCityInfo = (DataTable)GetCityInfo.Tables[0];
                        string suggestStation = "";
                        for (int j = 0; j < dtGetCityInfo.Rows.Count; j++)
                        {
                            suggestStation += dtGetCityInfo.Rows[j]["StationCode"].ToString() + ",";
                        }
                        if (suggestStation.Trim() != "")
                        {
                            suggestStation = suggestStation.Substring(0, suggestStation.Length - 1);
                            X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Invalid Cost Center(" + header1string.Split(',')[i] + ").Suggestion:" + suggestStation + ".', buttons: { ok: 'Ok' }, fn: function (btn) {  } });");
                            return;
                        }
                        else
                        {
                            X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Please input valid Cost Center.', buttons: { ok: 'Ok' }, fn: function (btn) {  } });");
                            return;
                        }
                    }
                }
            }

            //140306
            if (true)
            {

                #region 预算
                DataTable dtbudget = new DataTable();
                dtbudget.Columns.Add("EName", typeof(System.String));
                dtbudget.Columns.Add("COACode", typeof(System.String));
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

                //StoreBudget添加Field
                //StoreBudget.Reader[0].Fields.Clear();
                StoreBudget.Reader[0].Fields.Add("EName", RecordFieldType.String);
                StoreBudget.Reader[0].Fields.Add("COACode", RecordFieldType.String);
                StoreBudget.Reader[0].Fields.Add("Current", RecordFieldType.String);
                StoreBudget.Reader[0].Fields.Add("PU", RecordFieldType.String);
                StoreBudget.Reader[0].Fields.Add("PB", RecordFieldType.String);
                StoreBudget.Reader[0].Fields.Add("PPercent", RecordFieldType.String);
                StoreBudget.Reader[0].Fields.Add("DU", RecordFieldType.String);
                StoreBudget.Reader[0].Fields.Add("DB", RecordFieldType.String);
                StoreBudget.Reader[0].Fields.Add("DPercent", RecordFieldType.String);
                StoreBudget.Reader[0].Fields.Add("SU", RecordFieldType.String);
                StoreBudget.Reader[0].Fields.Add("SB", RecordFieldType.String);
                StoreBudget.Reader[0].Fields.Add("SPercent", RecordFieldType.String);

                DataTable dtA = new DataTable();
                dtA.Columns.Add("COACode", typeof(System.String));
                dtA.Columns.Add("Amount", typeof(System.Decimal));
                dtA.Columns.Add("PA", typeof(System.Decimal));
                dtA.Columns.Add("CA", typeof(System.Decimal));
                dtA.Columns.Add("Date", typeof(System.DateTime));

                StoreSubmitDataEventArgs eSubmit = new StoreSubmitDataEventArgs(detail, null);
                XmlNode xml = eSubmit.Xml;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml.InnerXml);
                int dtcol = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes.Count;
                int dtrow = doc.SelectNodes("records").Item(0).SelectNodes("record").Count;
                int groupcount = (dtcol - 3) / 2;//多少个站点会被更新,如果站点为空则不更新
                
                for (int i = 0; i < groupcount; i++)
                {
                    if (header0string.Split(',')[i] != "NA")//站点为空则不更新
                    {
                        //1. Air Ticket - Int'l
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtA.NewRow();
                            drnew["COACode"] = "62012000";
                            drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            drnew["PA"] = r1;
                            drnew["CA"] = r2;
                            dtA.Rows.Add(drnew);
                        }
                        //Domestic
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtA.NewRow();
                            drnew["COACode"] = "62012000";
                            drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            drnew["PA"] = r1;
                            drnew["CA"] = r2;
                            dtA.Rows.Add(drnew);
                        }
                        //2. Hotel Bill
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtA.NewRow();
                            drnew["COACode"] = "62012000";
                            drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            drnew["PA"] = r1;
                            drnew["CA"] = r2;
                            dtA.Rows.Add(drnew);
                        }
                        //3. Meals
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtA.NewRow();
                            drnew["COACode"] = "62012000";
                            drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            drnew["PA"] = r1;
                            drnew["CA"] = r2;
                            dtA.Rows.Add(drnew);
                        }
                        //4. Entertainment
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtA.NewRow();
                            drnew["COACode"] = "62010900";
                            drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            drnew["PA"] = r1;
                            drnew["CA"] = r2;
                            dtA.Rows.Add(drnew);
                        }
                        //5. Car Rental/Transportation
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtA.NewRow();
                            drnew["COACode"] = "62011900";
                            drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            drnew["PA"] = r1;
                            drnew["CA"] = r2;
                            dtA.Rows.Add(drnew);
                        }
                        //6. Communication
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtA.NewRow();
                            drnew["COACode"] = "62010500";
                            drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            drnew["PA"] = r1;
                            drnew["CA"] = r2;
                            dtA.Rows.Add(drnew);
                        }
                        //7. Local Trip
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtA.NewRow();
                            drnew["COACode"] = "62012000";
                            drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            drnew["PA"] = r1;
                            drnew["CA"] = r2;
                            dtA.Rows.Add(drnew);
                        }
                        //8. Overseas Trip USD15/day
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtA.NewRow();
                            drnew["COACode"] = "62012000";
                            drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            drnew["PA"] = r1;
                            drnew["CA"] = r2;
                            dtA.Rows.Add(drnew);
                        }
                        //9. Airport Tax/Travel Insurance
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtA.NewRow();
                            drnew["COACode"] = "62012000";
                            drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            drnew["PA"] = r1;
                            drnew["CA"] = r2;
                            dtA.Rows.Add(drnew);
                        }
                        //10. Others
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtA.NewRow();
                            drnew["COACode"] = "62012000";
                            drnew["Date"] = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            drnew["PA"] = r1;
                            drnew["CA"] = r2;
                            dtA.Rows.Add(drnew);
                        }
                    }
                }
                //检查是否空数据,false空,true有数据
                if (dtA.Rows.Count < 1)
                {
                    if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                    {
                        X.AddScript("Ext.Msg.show({ title: '提示', msg: '请至少填写一项费用.', buttons: { ok: 'Ok' }, fn: function (btn) {  } });");
                    }
                    else
                    {
                        X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Please input at least one expense item.', buttons: { ok: 'Ok' }, fn: function (btn) {  } });");
                    }
                    return;
                }
                //合计
                DataTable dtB = new DataTable();
                dtB.Columns.Add("COACode", typeof(System.String));
                dtB.Columns.Add("Amount", typeof(System.Decimal));
                dtB.Columns.Add("PA", typeof(System.Decimal));
                dtB.Columns.Add("CA", typeof(System.Decimal));
                if (dtA.Compute("Sum(Amount)", "COACode = 62012000").ToString() != "" && Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62012000").ToString()) != 0)
                {
                    DataRow dr = dtB.NewRow();
                    dr["COACode"] = "62012000";
                    dr["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62012000").ToString());
                    dr["PA"] = Convert.ToDecimal(dtA.Compute("Sum(PA)", "COACode = 62012000").ToString());
                    dr["CA"] = Convert.ToDecimal(dtA.Compute("Sum(CA)", "COACode = 62012000").ToString());
                    dtB.Rows.Add(dr);
                }
                if (dtA.Compute("Sum(Amount)", "COACode = 62010900").ToString() != "" && Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62010900").ToString()) != 0)
                {
                    DataRow dr = dtB.NewRow();
                    dr["COACode"] = "62010900";
                    dr["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62010900").ToString());
                    dr["PA"] = Convert.ToDecimal(dtA.Compute("Sum(PA)", "COACode = 62010900").ToString());
                    dr["CA"] = Convert.ToDecimal(dtA.Compute("Sum(CA)", "COACode = 62010900").ToString());
                    dtB.Rows.Add(dr);
                }
                if (dtA.Compute("Sum(Amount)", "COACode = 62011900").ToString() != "" && Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62011900").ToString()) != 0)
                {
                    DataRow dr = dtB.NewRow();
                    dr["COACode"] = "62011900";
                    dr["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62011900").ToString());
                    dr["PA"] = Convert.ToDecimal(dtA.Compute("Sum(PA)", "COACode = 62011900").ToString());
                    dr["CA"] = Convert.ToDecimal(dtA.Compute("Sum(CA)", "COACode = 62011900").ToString());
                    dtB.Rows.Add(dr);
                }
                if (dtA.Compute("Sum(Amount)", "COACode = 62010500").ToString() != "" && Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62010500").ToString()) != 0)
                {
                    DataRow dr = dtB.NewRow();
                    dr["COACode"] = "62010500";
                    dr["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62010500").ToString());
                    dr["PA"] = Convert.ToDecimal(dtA.Compute("Sum(PA)", "COACode = 62010500").ToString());
                    dr["CA"] = Convert.ToDecimal(dtA.Compute("Sum(CA)", "COACode = 62010500").ToString());
                    dtB.Rows.Add(dr);
                }
                
                //计算本地币种与预算站点币种汇率
                if (CurLocal != CurBudget)
                {
                    rate = DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(CurLocal, CurBudget, Convert.ToDateTime(dtA.Compute("Min(Date)", "").ToString()).Year);
                    if (rate == -1)
                    {
                        if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                        {
                            X.AddScript("Ext.Msg.show({ title: '提示', msg: '汇率异常,请联系Local MIS.', buttons: { ok: 'Ok' }, fn: function (btn) { window.location.reload(); } });");
                        }
                        else
                        {
                            X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Currency rate error,please contact Local MIS.', buttons: { ok: 'Ok' }, fn: function (btn) { window.location.reload(); } });");
                        }
                        return;
                    }
                }
                ////取得传递预算的参数
                //string userid = dt.Rows[0]["PersonID"].ToString();
                //string dpt = dt.Rows[0]["Department"].ToString();
                //string ostation = dt.Rows[0]["CostCenter"].ToString();//预算站点,与基本信息中的CostCenter一致(Station2)
                //string CDate = dtA.Compute("Min(Date)", "").ToString();//记录预算日期
                string tstation = ostation;//Etravel表中的Station2,目前与预算站点一致,不允许更改
                string year = Convert.ToDateTime(dtA.Compute("Min(Date)", "").ToString()).Year.ToString();
                string month = Convert.ToDateTime(dtA.Compute("Min(Date)", "").ToString()).Month.ToString();
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
                        dtC = Comm.RtnEB(userid, department, ostation, tstation, accountcode, year, month);
                        for (int i = 0; i < dtC.Rows.Count; i++)
                        {
                            if (dtC.Rows[i]["Type"].ToString() == "全年个人")
                            {
                                dr["PU"] = Convert.ToDecimal(dtC.Rows[i]["Used"].ToString());
                                dr["PB"] = Convert.ToDecimal(dtC.Rows[i]["Budget"].ToString());
                            }
                            else if (dtC.Rows[i]["Type"].ToString() == "全年部门")
                            {
                                dr["DU"] = Convert.ToDecimal(dtC.Rows[i]["Used"].ToString());
                                dr["DB"] = Convert.ToDecimal(dtC.Rows[i]["Budget"].ToString());
                            }
                            else if (dtC.Rows[i]["Type"].ToString() == "全年站点")
                            {
                                dr["SU"] = Convert.ToDecimal(dtC.Rows[i]["Used"].ToString());
                                dr["SB"] = Convert.ToDecimal(dtC.Rows[i]["Budget"].ToString());

                                //if (Convert.ToDecimal(dtC.Rows[i]["Budget"].ToString()) == 0)
                                //{
                                //    //输出预算内容
                                //    DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();

                                //    mail.FromDispName = "eReimbursement";
                                //    mail.From = "DIC2@dimerco.com";
                                //    mail.To = "Andy_Kang@dimerco.com";
                                //    mail.Title = "eReimbursement Bug" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:dd");
                                //    string body = "<div>" + userid + "<br/>";
                                //    body += "department:" + department + "<br/>";
                                //    body += "ostation:" + ostation + "<br/>";
                                //    body += "accountcode:" + accountcode + "<br/>";
                                //    body += "year:" + dtB.Rows[g]["Year"].ToString() + "<br/>";
                                //    body += "budget EB:" + dtC.Rows[i]["Budget"].ToString() + "<br/></div>";
                                //    mail.Body = body;
                                //    mail.Send();
                                //}

                            }
                        }
                        dtbudget.Rows.Add(dr);
                    }
                }
                //bool UnBudget = false;
                //14/10/24
                int budgetnew = 1;//默认预算内,0 - UnBudgeted,-1 Over-Budgeted,-2,UnBudgeted & Over-Budgeted
                bool PB = false, DB = false, SB = false;
                //计算%,取得名称,转为本地币种汇率,增加列记录Currency为邮件准备
                dtbudget.Columns.Add("Currency", typeof(System.String));
                for (int i = 0; i < dtbudget.Rows.Count; i++)
                {
                    dtbudget.Rows[i]["Currency"] = CurLocal;
                    if (Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()) != 0)//如果Budget不为0,则计算%
                    {
                        decimal PPercent = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["PU"].ToString()) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()), 2);
                        decimal PPercent1 = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[i]["Current"].ToString()) / rate + Convert.ToDecimal(dtbudget.Rows[i]["PU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()), 2);
                        dtbudget.Rows[i]["PPercent"] = PPercent;
                        //if (!UnBudget)
                        //{
                        //    if (PPercent1 > 100)
                        //    {
                        //        UnBudget = true;
                        //    }
                        //}
                        if (PPercent1 > 100)
                        {
                            if (budgetnew == 0)
                            {
                                budgetnew = -2;
                            }
                            else if (budgetnew == 1)
                            {
                                budgetnew = -1;
                            }
                        }
                        if (!PB)
                        {
                            PB = true;
                        }
                    }
                    if (Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()) != 0)//如果Budget不为0,则计算%
                    {
                        decimal DPercent = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["DU"].ToString()) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()), 2);
                        decimal DPercent1 = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[i]["Current"].ToString()) / rate + Convert.ToDecimal(dtbudget.Rows[i]["DU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()), 2);
                        dtbudget.Rows[i]["DPercent"] = DPercent;
                        //if (!UnBudget)
                        //{
                        //    if (DPercent1 > 100)
                        //    {
                        //        UnBudget = true;
                        //    }
                        //}
                        //14/10/24
                        if (DPercent1 > 100)
                        {
                            if (budgetnew == 0)
                            {
                                budgetnew = -2;
                            }
                            else if (budgetnew == 1)
                            {
                                budgetnew = -1;
                            }
                        }
                        if (!DB)
                        {
                            DB = true;
                        }
                    }
                    if (Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()) != 0)//如果Budget不为0,则计算%
                    {
                        decimal SPercent = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["SU"].ToString()) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()), 2);
                        decimal SPercent1 = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[i]["Current"].ToString()) / rate + Convert.ToDecimal(dtbudget.Rows[i]["SU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()), 2);
                        dtbudget.Rows[i]["SPercent"] = SPercent;
                        //if (!UnBudget)
                        //{
                        //    if (SPercent1 > 100)
                        //    {
                        //        UnBudget = true;
                        //    }
                        //}
                        //14/10/24
                        if (SPercent1 > 100)
                        {
                            if (budgetnew == 0)
                            {
                                budgetnew = -2;
                            }
                            else if (budgetnew == 1)
                            {
                                budgetnew = -1;
                            }
                        }
                        if (!SB)
                        {
                            SB = true;
                        }
                    }
                    if (Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()) == 0 && Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()) == 0 && Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()) == 0)
                    {
                        //如果个人,部门,站点,全未分配预算,则判断为Un-Budget
                        //if (!UnBudget)
                        //{
                        //    UnBudget = true;
                        //}
                        //14/10/24
                        if (budgetnew == -1)
                        {
                            budgetnew = -2;
                        }
                        else if (budgetnew == 1)
                        {
                            budgetnew = 0;
                        }
                    }
                    if (dtbudget.Rows[i]["COACode"].ToString() == "62012000")
                    {
                        dtbudget.Rows[i]["EName"] = "Travel expense";
                    }
                    else if (dtbudget.Rows[i]["COACode"].ToString() == "62010900")
                    {
                        dtbudget.Rows[i]["EName"] = "Entertainment";
                    }
                    else if (dtbudget.Rows[i]["COACode"].ToString() == "62011900")
                    {
                        dtbudget.Rows[i]["EName"] = "Transportation";
                    }
                    else if (dtbudget.Rows[i]["COACode"].ToString() == "62010500")
                    {
                        dtbudget.Rows[i]["EName"] = "Communication";
                    }
                    dtbudget.Rows[i]["PU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["PU"].ToString()), 2);
                    dtbudget.Rows[i]["PB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()), 2);
                    dtbudget.Rows[i]["DU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["DU"].ToString()), 2);
                    dtbudget.Rows[i]["DB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()), 2);
                    dtbudget.Rows[i]["SU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["SU"].ToString()), 2);
                    dtbudget.Rows[i]["SB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()), 2);
                }
                //如果个人,部门,站点,全未分配预算,则判断为Un-Budget
                //if (!UnBudget)
                //{
                //    if (!PB && !DB && !SB)
                //    {
                //        UnBudget = true;
                //    }
                //}
                //添加数据列
                //160113 垫付人
                if (hdOnBehalf.Value == null || hdOnBehalf.Value.ToString() == "")
                {
                    var cm = GridPanelBudget.ColumnModel;
                    cm.Columns.Clear();
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "EName",
                        Header = "Expense Item",
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "Current",
                        Header = "Current",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    //显示个人预算部分
                    if (PB)
                    {
                        cm.Columns.Add(new Column
                        {
                            DataIndex = "PU",
                            Header = "Personal Used",
                            Renderer = new Renderer { Fn = "GetNumber" },
                            Sortable = false,
                            Resizable = false,
                            MenuDisabled = true,
                            Width = 100
                        });
                        cm.Columns.Add(new Column
                        {
                            DataIndex = "PB",
                            Header = "Personal Budget",
                            Renderer = new Renderer { Fn = "GetNumber" },
                            Sortable = false,
                            Resizable = false,
                            MenuDisabled = true,
                            Width = 100
                        });
                        cm.Columns.Add(new Column
                        {
                            DataIndex = "PPercent",
                            Header = "%(Used/Budget)",
                            Renderer = new Renderer { Fn = "GetNumberPercent" },
                            Sortable = false,
                            Resizable = false,
                            MenuDisabled = true,
                            Width = 100
                        });
                    }
                    if (DB)
                    {
                        cm.Columns.Add(new Column
                        {
                            DataIndex = "DU",
                            Header = "Department Used",
                            Renderer = new Renderer { Fn = "GetNumber" },
                            Sortable = false,
                            Resizable = false,
                            MenuDisabled = true,
                            Width = 100
                        });
                        cm.Columns.Add(new Column
                        {
                            DataIndex = "DB",
                            Header = "Department Budget",
                            Renderer = new Renderer { Fn = "GetNumber" },
                            Sortable = false,
                            Resizable = false,
                            MenuDisabled = true,
                            Width = 100
                        });
                        cm.Columns.Add(new Column
                        {
                            DataIndex = "DPercent",
                            Header = "%(Used/Budget)",
                            Renderer = new Renderer { Fn = "GetNumberPercent" },
                            Sortable = false,
                            Resizable = false,
                            MenuDisabled = true,
                            Width = 100
                        });
                    }
                    if (SB)
                    {
                        cm.Columns.Add(new Column
                        {
                            DataIndex = "SU",
                            Header = "Unit Used",
                            Renderer = new Renderer { Fn = "GetNumber" },
                            Sortable = false,
                            Resizable = false,
                            MenuDisabled = true,
                            Width = 100
                        });
                        cm.Columns.Add(new Column
                        {
                            DataIndex = "SB",
                            Header = "Unit Budget",
                            Renderer = new Renderer { Fn = "GetNumber" },
                            Sortable = false,
                            Resizable = false,
                            MenuDisabled = true,
                            Width = 100
                        });
                        cm.Columns.Add(new Column
                        {
                            DataIndex = "SPercent",
                            Header = "%(Used/Budget)",
                            Renderer = new Renderer { Fn = "GetNumberPercent" },
                            Sortable = false,
                            Resizable = false,
                            MenuDisabled = true,
                            Width = 100
                        });
                    }
                    StoreBudget.DataSource = dtbudget;
                    StoreBudget.DataBind();
                    GridPanelBudget.Render();
                }
                //GridPanelBudget.Reconfigure();
                #endregion
                //处理申请单为Budget或Un-Budget
                //如果有费用大类超过预算,则提示询问
                //if (UnBudget)//
                //{
                //    X.AddScript("Ext.Msg.show({ title: 'Message', msg: '部分费用超过预算,仅可按<a style=\"color:Red\">Un-Budget</a>流程申请,是否接受?', buttons: { ok: 'Ok',cancel:'No' }, fn: function (btn) { if(btn=='ok'){SaveAllNew();}else{return false;} } });");
                //}
                //14/10/24
                if (budgetnew != 1)
                {
                    if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                    {
                        if (budgetnew == 0)
                        {
                            ////输出预算内容
                            //DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();

                            //mail.FromDispName = "eReimbursement";
                            //mail.From = "DIC2@dimerco.com";
                            //mail.To = "Andy_Kang@dimerco.com";
                            //mail.Title = "eReimbursement Bug" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:dd");
                            //string body = "<div>" + Request.Cookies.Get("eReimUserID").Value + "<br/>Travel<br/><table>";
                            //body += "<tr>";
                            //for (int j = 0; j < dtbudget.Columns.Count; j++)
                            //{
                            //    body += "<th>" + dtbudget.Columns[j].ColumnName + "</th>";
                            //}
                            //body += "</tr>";
                            //for (int i = 0; i < dtbudget.Rows.Count; i++)
                            //{
                            //    body += "<tr>";
                            //    for (int j = 0; j < dtbudget.Columns.Count; j++)
                            //    {
                            //        body += "<td>" + dtbudget.Rows[i][j].ToString() + "</td>";
                            //    }
                            //    body += "</tr>";
                            //}
                            //body += "</table></div>";
                            //mail.Body = body;
                            //mail.Send();
                            //160224 如果为垫付,则直接保存不提示预算情况
                            if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                            {
                                X.AddScript("SaveAllNew('" + budgetnew.ToString() + "');");
                            }
                            else
                            {
                                X.AddScript("Ext.Msg.show({ title: 'Message', msg: '未设置预算,仅可按<a style=\"color:Red\">UnBudgeted</a>流程申请,是否接受?', buttons: { ok: 'Ok',cancel:'No' }, fn: function (btn) { if(btn=='ok'){SaveAllNew('" + budgetnew.ToString() + "');}else{return false;} } });");
                            }
                            
                        }
                        else if (budgetnew == -1)
                        {//160224 如果为垫付,则直接保存不提示预算情况
                            if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                            {
                                X.AddScript("SaveAllNew('" + budgetnew.ToString() + "');");
                            }
                            else
                            {
                                X.AddScript("Ext.Msg.show({ title: 'Message', msg: '超出预算,仅可按<a style=\"color:Red\">Over-Budgeted</a>流程申请,是否接受?', buttons: { ok: 'Ok',cancel:'No' }, fn: function (btn) { if(btn=='ok'){SaveAllNew('" + budgetnew.ToString() + "');}else{return false;} } });");
                            }
                        }
                        else if (budgetnew == -2)
                        {
                            //160224 如果为垫付,则直接保存不提示预算情况
                            if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                            {
                                X.AddScript("SaveAllNew('" + budgetnew.ToString() + "');");
                            }
                            else
                            {
                                X.AddScript("Ext.Msg.show({ title: 'Message', msg: '未设置预算或超出预算,仅可按<a style=\"color:Red\">UnBudgeted</a>流程申请,是否接受?', buttons: { ok: 'Ok',cancel:'No' }, fn: function (btn) { if(btn=='ok'){SaveAllNew('" + budgetnew.ToString() + "');}else{return false;} } });");
                            }
                        }
                    }
                    else
                    {
                        if (budgetnew == 0)
                        {
                            ////输出预算内容
                            //DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();

                            //mail.FromDispName = "eReimbursement";
                            //mail.From = "DIC2@dimerco.com";
                            //mail.To = "Andy_Kang@dimerco.com";
                            //mail.Title = "eReimbursement Bug" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:dd");
                            //string body = "<div>" + Request.Cookies.Get("eReimUserID").Value + "<br/>Travel<br/><table>";
                            //body += "<tr>";
                            //for (int j = 0; j < dtbudget.Columns.Count; j++)
                            //{
                            //    body += "<th>" + dtbudget.Columns[j].ColumnName + "</th>";
                            //}
                            //body += "</tr>";
                            //for (int i = 0; i < dtbudget.Rows.Count; i++)
                            //{
                            //    body += "<tr>";
                            //    for (int j = 0; j < dtbudget.Columns.Count; j++)
                            //    {
                            //        body += "<td>" + dtbudget.Rows[i][j].ToString() + "</td>";
                            //    }
                            //    body += "</tr>";
                            //}
                            //body += "</table></div>";
                            //mail.Body = body;
                            //mail.Send();
                            //160224 如果为垫付,则直接保存不提示预算情况
                            if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                            {
                                X.AddScript("SaveAllNew('" + budgetnew.ToString() + "');");
                            }
                            else
                            {
                                X.AddScript("Ext.Msg.show({ title: 'Message', msg: '<a style=\"color:Red\">UnBudgeted</a>,do you accept?', buttons: { ok: 'Ok',cancel:'No' }, fn: function (btn) { if(btn=='ok'){SaveAllNew('" + budgetnew.ToString() + "');}else{return false;} } });");
                            }
                        }
                        else if (budgetnew == -1)
                        {
                            //160224 如果为垫付,则直接保存不提示预算情况
                            if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                            {
                                X.AddScript("SaveAllNew('" + budgetnew.ToString() + "');");
                            }
                            else
                            {
                                X.AddScript("Ext.Msg.show({ title: 'Message', msg: '<a style=\"color:Red\">Over-Budgeted</a>,do you accept?', buttons: { ok: 'Ok',cancel:'No' }, fn: function (btn) { if(btn=='ok'){SaveAllNew('" + budgetnew.ToString() + "');}else{return false;} } });");
                            }
                        }
                        else if (budgetnew == -2)
                        {
                            //160224 如果为垫付,则直接保存不提示预算情况
                            if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                            {
                                X.AddScript("SaveAllNew('" + budgetnew.ToString() + "');");
                            }
                            else
                            {
                                X.AddScript("Ext.Msg.show({ title: 'Message', msg: '<a style=\"color:Red\">UnBudgeted</a>,do you accept?', buttons: { ok: 'Ok',cancel:'No' }, fn: function (btn) { if(btn=='ok'){SaveAllNew('" + budgetnew.ToString() + "');}else{return false;} } });");
                            }
                        }
                    }
                }
                else//全在预算内,按预算内申请流程处理
                {
                    //输出预算内容
                    //DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();

                    //mail.FromDispName = "eReimbursement";
                    //mail.From = "DIC2@dimerco.com";
                    //mail.To = "Andy_Kang@dimerco.com";
                    //mail.Title = "eReimbursement Bug" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:dd");
                    //string body = "<div><table>";
                    //body += "<tr>";
                    //for (int j = 0; j < dtbudget.Columns.Count; j++)
                    //{
                    //    body += "<th>" + dtbudget.Columns[j].ColumnName + "</th>";
                    //}
                    //body += "</tr>";
                    //for (int i = 0; i < dtbudget.Rows.Count; i++)
                    //{
                    //    body += "<tr>";
                    //    for (int j = 0; j < dtbudget.Columns.Count; j++)
                    //    {
                    //        body += "<td>" + dtbudget.Rows[i][j].ToString() + "</td>";
                    //    }
                    //    body += "</tr>";
                    //}
                    //body += "</table></div>";
                    //mail.Body = body;
                    //mail.Send();

                    //判断是否设置了审批流程
                    //string sqlCheckFlow = "";
                    //sqlCheckFlow = "select * from GroupFlow where [Type]!=2 and GID=(select GID from GroupUsers where UserID='" + hdOwnerID.Value.ToString() + "') order by FlowNo";
                    //DataTable dtGroupFlowData = dbc.GetData("eReimbursement", sqlCheckFlow);
                    //if (dtGroupFlowData.Rows.Count < 1)
                    //{
                    //    if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                    //    {
                    //        ErrorHandle("请先设置审批人.");
                    //    }
                    //    else
                    //    {
                    //        ErrorHandle("Not set Approve flow,please contact with Local MIS.");
                    //    }
                    //    return;
                    //}
                    //160119 垫付
                    string sqlCheckFlow = ""; DataTable dtGroupFlowData = new DataTable();

                    if (hdOnBehalf.Value == null || hdOnBehalf.Value.ToString() == "")
                    {
                        sqlCheckFlow = "select * from GroupFlow where [Type]!=2 and GID=(select GID from GroupUsers where UserID='" + hdOwnerID.Value.ToString() + "') order by FlowNo";
                        dtGroupFlowData = dbc.GetData("eReimbursement", sqlCheckFlow);
                        if (dtGroupFlowData.Rows.Count < 1)
                        {
                            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                            {
                                ErrorHandle("请先设置审批人.");
                            }
                            else
                            {
                                ErrorHandle("Not set Approve flow,please contact with Local MIS.");
                            }
                            return;
                        }
                    }
                    else//160113 垫付审批流程
                    {
                        sqlCheckFlow = "select UserID,t1.* from (select * from GroupFlow where [Type]!=2 and GID in (select GID from GroupUsers where UserID='" + hdOwnerID.Value.ToString() + "' or UserID='" + hdOnBehalf.Value.ToString() + "')) t1 left join (select * from GroupUsers where UserID='" + hdOwnerID.Value.ToString() + "' or UserID='" + hdOnBehalf.Value.ToString() + "') t2 on t2.Gid=t1.Gid order by Gid,FlowNo";
                        dtGroupFlowData = dbc.GetData("eReimbursement", sqlCheckFlow);
                        if (dtGroupFlowData.Rows.Count < 1)
                        {
                            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                            {
                                ErrorHandle("请先设置审批人.");
                            }
                            else
                            {
                                ErrorHandle("Not set Approve flow,please contact with Local MIS.");
                            }
                            return;
                        }
                    }


                    if (type == "ND")//保存并申请
                    {
                        if (hdTravelRequestID.Value.ToString() == "")//直接新增申请,不通过草稿
                        {
                            //Station2保存用户的预算站点,与用户信息中的CostCenter相同
                            string word = "[No],[Person],[Station],[Department],[ReportFile],[CreadedBy],[CreadedDate],[Attach],[Remark],[Bdate],[Edate],[PersonID],[CreadedByID],[ApplyDate],[CCMailList],[Budget],[Station2]";
                            //160123 垫付
                            if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                            { word += ",OnBehalfPersonID,OnBehalfPersonName,OnBehalfPersonUnit,OnBehalfPersonDept,OnBehalfPersonCostCenter"; }
                            string value = "";
                            value += "'" + station_applyperson + DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString().PadLeft(2, '0') + "',";//edit
                            value += "'" + hdOwner.Value.ToString() + "',"; value += "'" + station_applyperson + "',"; value += "'" + dept_applyperson + "',";//edit
                            value += "'" + hdReport.Value.ToString() + "',";
                            value += "'" + Request.Cookies.Get("eReimUserName").Value + "'";//edit
                            value += ",'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',";
                            value += "'" + hdScanFile.Value.ToString() + "',";
                            value += "'" + txtRemark.Text.Replace("'", "''") + "',";
                            value += "null,null";
                            value += ",'" + hdOwnerID.Value.ToString() + "'";
                            value += ",'" + Request.Cookies.Get("eReimUserID").Value + "'";
                            value += ",'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'";
                            value += ",'" + CCMailList + "'";
                            value += ",1";
                            value += ",'" + ostation + "'";
                            //160123 垫付
                            if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                            {
                                value += ",'" + hdOnBehalf.Value.ToString() + "'";
                                DataSet dsE = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(hdOnBehalf.Value.ToString());
                                if (dsE.Tables[0].Rows.Count == 1)
                                {
                                    DataTable dtE = dsE.Tables[0];
                                    value += ",'" + dtE.Rows[0]["fullName"].ToString() + "'";
                                    value += ",'" + dtE.Rows[0]["stationCode"].ToString() + "'";
                                    value += ",'" + dtE.Rows[0]["CRPDepartmentName"].ToString() + "'";
                                    value += ",'" + dtE.Rows[0]["CostCenter"].ToString() + "'";
                                }
                            }


                            string updatesql = "insert into ETravel (" + word + ") values(" + value + ");update ETravel set [No]=[No]+(select [MonthCount]=right('0000'+cast(count(ID) as varchar(10)),4) from ETravel where (month(ApplyDate) in (select month(ApplyDate) from ETravel where [ID]=@@IDENTITY) and (year(ApplyDate) in (select year(ApplyDate) from ETravel where [ID]=@@IDENTITY)) and Station=(select Station from ETravel where ID=@@IDENTITY)))+'T' where ID=@@IDENTITY;select [msg]=convert(varchar,ID)+','+[No] from ETravel where ID=@@IDENTITY";

                            string newid = dbc.UpdateData("eReimbursement", updatesql, "Insert");

                            string rows = "";

                            //for (int i = 0; i < dtGroupFlowData.Rows.Count; i++)
                            //{
                            //    string wordflow = "[No],[Type],[Station],[Department],[Person],[CreadedBy],[CreatedDate],[Step],[Status],[Approver],[ApproverID],[RequestID],[FlowFn]";
                            //    if (i==0)
                            //    {
                            //        wordflow += ",[Active]";
                            //    }
                            //    string valueflow = "";
                            //    valueflow += "'" + newid.Split(',')[1] + "',";
                            //    valueflow += "'T',";
                            //    valueflow += "'" + station + "',";//
                            //    valueflow += "'" + department + "',";
                            //    valueflow += "'" + hdOwner.Value.ToString() + "',";//申请人
                            //    valueflow += "'" + Request.Cookies.Get("eReimUserName").Value + "',";//填写人
                            //    valueflow += "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',";
                            //    valueflow += dtGroupFlowData.Rows[i]["FlowNo"].ToString() + ",";
                            //    valueflow += "1,";
                            //    valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUser"].ToString() + "',";
                            //    valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUserid"].ToString() + "',";
                            //    valueflow += "'" + newid.Split(',')[0] + "'";
                            //    valueflow += ",'" + (dtGroupFlowData.Rows[i]["Fn"].ToString() == "" ? "Approver" : dtGroupFlowData.Rows[i]["Fn"].ToString()) + "'";
                            //    if (i == 0)
                            //    {
                            //        valueflow += ",1";
                            //    }
                            //    string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";
                            //    rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
                            //}
                            //160113 垫付
                            if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                            {
                                string personid = hdOwnerID.Value.ToString();
                                string onbehalfid = hdOnBehalf.Value.ToString();
                                DataTable dtOnbehalf = new DataTable();
                                dtOnbehalf.Columns.Add("FlowNo");
                                dtOnbehalf.Columns.Add("FlowUser");
                                dtOnbehalf.Columns.Add("FlowUserid");
                                dtOnbehalf.Columns.Add("Fn");
                                dtOnbehalf.Columns.Add("PersonID");
                                //被垫付人的Verifier
                                if (dtGroupFlowData.Select("UserID='" + onbehalfid + "' and Fn='Verifier'").Count() >= 1)
                                {
                                    DataRow dr = dtOnbehalf.NewRow();
                                    DataRow drold = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and Fn='Verifier'")[(dtGroupFlowData.Select("UserID='" + onbehalfid + "' and Fn='Verifier'").Count()) - 1];
                                    dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                                    dr["FlowUser"] = drold["FlowUser"].ToString();
                                    dr["FlowUserid"] = drold["FlowUserid"].ToString();
                                    dr["Fn"] = "Verifier";
                                    dr["PersonID"] = drold["UserID"].ToString();
                                    dtOnbehalf.Rows.Add(dr);
                                }
                                //被垫付人的Apporver
                                if (dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count() >= 1)
                                {
                                    int flowcount = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count();
                                    if (flowcount >= 2)
                                    {
                                        DataRow dr = dtOnbehalf.NewRow();
                                        DataRow drold = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count()) - 2];
                                        dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                                        dr["FlowUser"] = drold["FlowUser"].ToString();
                                        dr["FlowUserid"] = drold["FlowUserid"].ToString();
                                        dr["Fn"] = "Apporver";
                                        dr["PersonID"] = drold["UserID"].ToString();
                                        dtOnbehalf.Rows.Add(dr);

                                        dr = dtOnbehalf.NewRow();
                                        drold = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count()) - 1];
                                        dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                                        dr["FlowUser"] = drold["FlowUser"].ToString();
                                        dr["FlowUserid"] = drold["FlowUserid"].ToString();
                                        dr["Fn"] = "Apporver";
                                        dr["PersonID"] = drold["UserID"].ToString();
                                        dtOnbehalf.Rows.Add(dr);
                                    }
                                    else
                                    {
                                        DataRow dr = dtOnbehalf.NewRow();
                                        DataRow drold = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count()) - 1];
                                        dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                                        dr["FlowUser"] = drold["FlowUser"].ToString();
                                        dr["FlowUserid"] = drold["FlowUserid"].ToString();
                                        dr["Fn"] = "Apporver";
                                        dr["PersonID"] = drold["UserID"].ToString();
                                        dtOnbehalf.Rows.Add(dr);
                                    }

                                }
                                //垫付人的Apporver
                                if (dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Apporver' or Fn is null)").Count() >= 1)
                                {
                                    DataRow dr = dtOnbehalf.NewRow();
                                    DataRow drold = dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Apporver' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Apporver' or Fn is null)").Count()) - 1];
                                    dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                                    dr["FlowUser"] = drold["FlowUser"].ToString();
                                    dr["FlowUserid"] = drold["FlowUserid"].ToString();
                                    dr["Fn"] = "Apporver";
                                    dr["PersonID"] = drold["UserID"].ToString();
                                    dtOnbehalf.Rows.Add(dr);
                                }
                                //垫付人的Issuer
                                if (dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Issuer' or Fn is null)").Count() >= 1)
                                {
                                    DataRow dr = dtOnbehalf.NewRow();
                                    DataRow drold = dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Issuer' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Issuer' or Fn is null)").Count()) - 1];
                                    dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                                    dr["FlowUser"] = drold["FlowUser"].ToString();
                                    dr["FlowUserid"] = drold["FlowUserid"].ToString();
                                    dr["Fn"] = "Issuer";
                                    dr["PersonID"] = drold["UserID"].ToString();
                                    dtOnbehalf.Rows.Add(dr);
                                }

                                for (int j = 0; j < dtOnbehalf.Rows.Count; j++)
                                {
                                    string wordflow = "[RequestID],[Type],[Step],[Status],[Approver],[ApproverID],[FlowFn],PersonID";
                                    //if (dtGroupFlowData.Rows[j]["FlowNo"].ToString() == "1")
                                    //{
                                    //    wordflow += ",[Active]";
                                    //}
                                    if (j == 0)
                                    {
                                        wordflow += ",[Active]";
                                    }
                                    string valueflow = "";
                                    valueflow += "'" + newid.Split(',')[0] + "',";
                                    valueflow += "'T',";
                                    valueflow += dtOnbehalf.Rows[j]["FlowNo"].ToString() + ",";
                                    valueflow += "1,";
                                    valueflow += "'" + dtOnbehalf.Rows[j]["FlowUser"].ToString() + "',";
                                    valueflow += "'" + dtOnbehalf.Rows[j]["FlowUserid"].ToString() + "'";
                                    valueflow += ",'" + (dtOnbehalf.Rows[j]["Fn"].ToString() == "" ? "Apporver" : dtOnbehalf.Rows[j]["Fn"].ToString()) + "'";
                                    valueflow += ",'" + dtOnbehalf.Rows[j]["PersonID"].ToString() + "'";
                                    if (j == 0)
                                    {
                                        valueflow += ",1";
                                    }
                                    string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";

                                    rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
                                }

                            }
                            else
                            {
                                for (int j = 0; j < dtGroupFlowData.Rows.Count; j++)
                                {
                                    string wordflow = "[RequestID],[Type],[Step],[Status],[Approver],[ApproverID],[FlowFn]";
                                    if (j == 0)
                                    {
                                        wordflow += ",[Active]";
                                    }
                                    string valueflow = "";
                                    valueflow += "'" + newid.Split(',')[0] + "',";
                                    valueflow += "'T',";
                                    valueflow += dtGroupFlowData.Rows[j]["FlowNo"].ToString() + ",";
                                    valueflow += "1,";
                                    valueflow += "'" + dtGroupFlowData.Rows[j]["FlowUser"].ToString() + "',";
                                    valueflow += "'" + dtGroupFlowData.Rows[j]["FlowUserid"].ToString() + "'";
                                    valueflow += ",'" + (dtGroupFlowData.Rows[j]["Fn"].ToString() == "" ? "Apporver" : dtGroupFlowData.Rows[j]["Fn"].ToString()) + "'";
                                    if (j == 0)
                                    {
                                        valueflow += ",1";
                                    }

                                    string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";
                                    rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
                                }
                            }
                            if (newid == "-1" || rows == "-1" || rows == "")
                            {
                                ErrorHandle("Data Error.");
                                return;
                            }
                            else
                            {
                                hdTravelRequestID.Value = newid.Split(',')[0];//新增后记录ID
                                hdTravelRequestNo.Value = newid.Split(',')[1];//新增后记录No
                                if (!SaveDetail(detail, header0string, header1string, header2string, CurLocal, dept, CurBudget) || !SendMailNew(dtbudget))
                                //if (!SaveDetail(detail, header0string, header1string, header2string, Cur, dept) || !SendMail(""))
                                {
                                    ErrorHandle("Data Error.");
                                    return;
                                }
                                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                {
                                    Panel3.Title = "差旅费申请单: " + newid.Split(',')[1];
                                    UpdateMSG("保存申请单:" + newid.Split(',')[1] + "成功.");
                                }
                                else
                                {
                                    Panel3.Title = "Travel Expense Form: " + newid.Split(',')[1];
                                    UpdateMSG("Saved Travel Expense Form: " + newid.Split(',')[1] + " successfully.");
                                }

                            }
                            //
                        }
                        else//由草稿升级为正式申请
                        {
                            string updatesql = "update ETravel set [Person]='" + hdOwner.Value.ToString();
                            updatesql += "',[Station]='" + station_applyperson;
                            updatesql += "',[Department]='" + dept_applyperson;
                            updatesql += "',[ReportFile]='" + hdReport.Value.ToString();
                            updatesql += "',[Remark]='" + txtRemark.Text.Replace("'", "''") + "'";
                            updatesql += ",[Attach]='" + hdScanFile.Value.ToString() + "'";
                            updatesql += ",[Type]=0";
                            updatesql += ",[PersonID]='" + hdOwnerID.Value.ToString() + "'";
                            string oldno = hdTravelRequestNo.Value.ToString();
                            string newno = hdTravelRequestNo.Value.ToString().Substring(0, hdTravelRequestNo.Value.ToString().Length - 1);
                            updatesql += ",[No]='" + newno + "',";
                            updatesql += "[CreadedDate]='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',";
                            updatesql += "[CreadedBy]='" + Request.Cookies.Get("eReimUserName").Value + "',";
                            updatesql += "[CreadedByID]='" + Request.Cookies.Get("eReimUserID").Value + "'";
                            updatesql += ",[CCMailList]='" + CCMailList + "'";
                            updatesql += ",[Budget]=1";
                            updatesql += ",[Station2]='" + ostation + "'";
                            //160123 垫付
                            if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                            {
                                updatesql += ",OnBehalfPersonID='" + hdOnBehalf.Value.ToString() + "'";
                                DataSet dsE = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(hdOnBehalf.Value.ToString());
                                if (dsE.Tables[0].Rows.Count == 1)
                                {
                                    DataTable dtE = dsE.Tables[0];
                                    updatesql += ",OnBehalfPersonName='" + dtE.Rows[0]["fullName"].ToString() + "'";
                                    updatesql += ",OnBehalfPersonUnit='" + dtE.Rows[0]["stationCode"].ToString() + "'";
                                    updatesql += ",OnBehalfPersonDept='" + dtE.Rows[0]["CRPDepartmentName"].ToString() + "'";
                                    updatesql += ",OnBehalfPersonCostCenter='" + dtE.Rows[0]["CostCenter"].ToString() + "'";
                                }
                            }

                            updatesql += " where ID=" + hdTravelRequestID.Value.ToString();

                            string newid = dbc.UpdateData("eReimbursement", updatesql, "Update");

                            //操作Flow表
                            string sqlDeleteEflow = "delete from Eflow where [Type]='T' and [RequestID]='" + hdTravelRequestID.Value.ToString() + "'";
                            string deleterows = dbc.UpdateData("eReimbursement", sqlDeleteEflow, "Update");
                            string rows = "";

                            //160113 垫付
                            if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                            {
                                string personid = hdOwnerID.Value.ToString();
                                string onbehalfid = hdOnBehalf.Value.ToString();
                                DataTable dtOnbehalf = new DataTable();
                                dtOnbehalf.Columns.Add("FlowNo");
                                dtOnbehalf.Columns.Add("FlowUser");
                                dtOnbehalf.Columns.Add("FlowUserid");
                                dtOnbehalf.Columns.Add("Fn");
                                dtOnbehalf.Columns.Add("PersonID");
                                //被垫付人的Verifier
                                if (dtGroupFlowData.Select("UserID='" + onbehalfid + "' and Fn='Verifier'").Count() >= 1)
                                {
                                    DataRow dr = dtOnbehalf.NewRow();
                                    DataRow drold = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and Fn='Verifier'")[(dtGroupFlowData.Select("UserID='" + onbehalfid + "' and Fn='Verifier'").Count()) - 1];
                                    dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                                    dr["FlowUser"] = drold["FlowUser"].ToString();
                                    dr["FlowUserid"] = drold["FlowUserID"].ToString();
                                    dr["Fn"] = "Verifier";
                                    dr["PersonID"] = drold["UserID"].ToString();
                                    dtOnbehalf.Rows.Add(dr);
                                }
                                //被垫付人的Apporver
                                if (dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count() >= 1)
                                {
                                    int flowcount = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count();
                                    if (flowcount >= 2)
                                    {
                                        DataRow dr = dtOnbehalf.NewRow();
                                        DataRow drold = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count()) - 2];
                                        dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                                        dr["FlowUser"] = drold["FlowUser"].ToString();
                                        dr["FlowUserid"] = drold["FlowUserID"].ToString();
                                        dr["Fn"] = "Apporver";
                                        dr["PersonID"] = drold["UserID"].ToString();
                                        dtOnbehalf.Rows.Add(dr);

                                        dr = dtOnbehalf.NewRow();
                                        drold = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count()) - 1];
                                        dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                                        dr["FlowUser"] = drold["FlowUser"].ToString();
                                        dr["FlowUserid"] = drold["FlowUserID"].ToString();
                                        dr["Fn"] = "Apporver";
                                        dr["PersonID"] = drold["UserID"].ToString();
                                        dtOnbehalf.Rows.Add(dr);
                                    }
                                    else
                                    {
                                        DataRow dr = dtOnbehalf.NewRow();
                                        DataRow drold = dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + onbehalfid + "' and (Fn='Apporver' or Fn is null)").Count()) - 1];
                                        dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                                        dr["FlowUser"] = drold["FlowUser"].ToString();
                                        dr["FlowUserid"] = drold["FlowUserID"].ToString();
                                        dr["Fn"] = "Apporver";
                                        dr["PersonID"] = drold["UserID"].ToString();
                                        dtOnbehalf.Rows.Add(dr);
                                    }

                                }
                                //垫付人的Apporver
                                if (dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Apporver' or Fn is null)").Count() >= 1)
                                {
                                    DataRow dr = dtOnbehalf.NewRow();
                                    DataRow drold = dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Apporver' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Apporver' or Fn is null)").Count()) - 1];
                                    dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                                    dr["FlowUser"] = drold["FlowUser"].ToString();
                                    dr["FlowUserid"] = drold["FlowUserID"].ToString();
                                    dr["Fn"] = "Apporver";
                                    dr["PersonID"] = drold["UserID"].ToString();
                                    dtOnbehalf.Rows.Add(dr);
                                }
                                //垫付人的Issuer
                                if (dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Issuer' or Fn is null)").Count() >= 1)
                                {
                                    DataRow dr = dtOnbehalf.NewRow();
                                    DataRow drold = dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Issuer' or Fn is null)")[(dtGroupFlowData.Select("UserID='" + personid + "' and (Fn='Issuer' or Fn is null)").Count()) - 1];
                                    dr["FlowNo"] = (dtOnbehalf.Rows.Count + 1).ToString();
                                    dr["FlowUser"] = drold["FlowUser"].ToString();
                                    dr["FlowUserid"] = drold["FlowUserID"].ToString();
                                    dr["Fn"] = "Issuer";
                                    dr["PersonID"] = drold["UserID"].ToString();
                                    dtOnbehalf.Rows.Add(dr);
                                }
                                for (int j = 0; j < dtOnbehalf.Rows.Count; j++)
                                {
                                    string wordflow = "[RequestID],[Type],[Step],[Status],[Approver],[ApproverID],[FlowFn],PersonID";
                                    //if (dtGroupFlowData.Rows[j]["FlowNo"].ToString() == "1")
                                    //{
                                    //    wordflow += ",[Active]";
                                    //}
                                    if (j == 0)
                                    {
                                        wordflow += ",[Active]";
                                    }
                                    string valueflow = "";
                                    valueflow += "'" + hdTravelRequestID.Value.ToString() + "',";
                                    valueflow += "'T',";
                                    valueflow += dtOnbehalf.Rows[j]["FlowNo"].ToString() + ",";
                                    valueflow += "1,";
                                    valueflow += "'" + dtOnbehalf.Rows[j]["FlowUser"].ToString() + "',";
                                    valueflow += "'" + dtOnbehalf.Rows[j]["FlowUserid"].ToString() + "'";
                                    valueflow += ",'" + (dtOnbehalf.Rows[j]["Fn"].ToString() == "" ? "Apporver" : dtOnbehalf.Rows[j]["Fn"].ToString()) + "'";
                                    valueflow += ",'" + dtOnbehalf.Rows[j]["PersonID"].ToString() + "'";
                                    //if (dtGroupFlowData.Rows[j]["FlowNo"].ToString() == "1")
                                    //{
                                    //    valueflow += ",1";
                                    //}
                                    if (j == 0)
                                    {
                                        valueflow += ",1";
                                    }
                                    string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";

                                    rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
                                }

                            }
                            else
                            {
                                for (int j = 0; j < dtGroupFlowData.Rows.Count; j++)
                                {
                                    string wordflow = "[RequestID],[Type],[Step],[Status],[Approver],[ApproverID],[FlowFn]";
                                    //if (dtGroupFlowData.Rows[j]["FlowNo"].ToString() == "1")
                                    //{
                                    //    wordflow += ",[Active]";
                                    //}
                                    if (j == 0)
                                    {
                                        wordflow += ",[Active]";
                                    }
                                    string valueflow = "";
                                    valueflow += "'" + hdTravelRequestID.Value.ToString() + "',";
                                    valueflow += "'T',";
                                    valueflow += dtGroupFlowData.Rows[j]["FlowNo"].ToString() + ",";
                                    valueflow += "1,";
                                    valueflow += "'" + dtGroupFlowData.Rows[j]["FlowUser"].ToString() + "',";
                                    valueflow += "'" + dtGroupFlowData.Rows[j]["FlowUserid"].ToString() + "'";
                                    valueflow += ",'" + (dtGroupFlowData.Rows[j]["Fn"].ToString() == "" ? "Apporver" : dtGroupFlowData.Rows[j]["Fn"].ToString()) + "'";
                                    //if (dtGroupFlowData.Rows[j]["FlowNo"].ToString() == "1")
                                    //{
                                    //    valueflow += ",1";
                                    //}
                                    if (j == 0)
                                    {
                                        valueflow += ",1";
                                    }
                                    string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";

                                    rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
                                }
                            }

                            //for (int i = 0; i < dtGroupFlowData.Rows.Count; i++)
                            //{
                            //    string wordflow = "[No],[Type],[Station],[Department],[Person],[CreadedBy],[CreatedDate],[Step],[Status],[Approver],[ApproverID],[RequestID],[FlowFn]";
                            //    if (i == 0)
                            //    {
                            //        wordflow += ",[Active]";
                            //    }
                            //    string valueflow = "";
                            //    valueflow += "'" + newno + "',";
                            //    valueflow += "'T',";
                            //    valueflow += "'" + station + "',";
                            //    valueflow += "'" + department + "',";
                            //    valueflow += "'" + hdOwner.Value.ToString() + "',";
                            //    valueflow += "'" + Request.Cookies.Get("eReimUserName").Value + "',";
                            //    valueflow += "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',";
                            //    valueflow += dtGroupFlowData.Rows[i]["FlowNo"].ToString() + ",";
                            //    valueflow += "1,";
                            //    valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUser"].ToString() + "',";
                            //    valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUserid"].ToString() + "',";
                            //    valueflow += hdTravelRequestID.Value.ToString();
                            //    valueflow += ",'" + (dtGroupFlowData.Rows[i]["Fn"].ToString() == "" ? "Approver" : dtGroupFlowData.Rows[i]["Fn"].ToString()) + "'";
                            //    if (i == 0)
                            //    {
                            //        valueflow += ",1";
                            //    }
                            //    string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";
                            //    rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
                            //}
                            //
                            if (newid == "-1" || rows == "-1" || rows == "")
                            {
                                ErrorHandle("Data Error.");
                                return;
                            }
                            else
                            {
                                hdTravelRequestNo.Value = newno;
                                if (!SaveDetail(detail, header0string, header1string, header2string, CurLocal, dept, CurBudget) || !SendMailNew(dtbudget))
                                //if (!SaveDetail(detail, header0string, header1string, header2string, Cur, dept) || !SendMail(warningmsg))
                                {
                                    ErrorHandle("Data Error.");
                                    return;
                                }

                                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                {
                                    Panel3.Title = "差旅费申请单:" + newno;
                                    UpdateMSG("保存差旅费申请单:" + hdTravelRequestNo.Value.ToString() + "成功.");
                                }
                                else
                                {
                                    Panel3.Title = "Travel Expense Form: " + newno;
                                    UpdateMSG("Saved Travel Expense Form: " + hdTravelRequestNo.Value.ToString() + " successfully.");
                                }

                            }
                        }
                        X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();btnSaveDraft.disable();btnSaveAndSend.disable();btnExport.enable();btnCC.disable();");
                        //X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();btnSaveDraft.disable();btnSaveAndSend.disable();cbxBudget.setReadOnly(true);btnExport.enable();btnCC.disable();");
                    }
                    else//保存草稿
                    {
                        if (hdTravelRequestID.Value.ToString() != "")//由链接进入的草稿更新
                        {
                            string updatesql = "update ETravel set [Person]='" + hdOwner.Value.ToString();
                            updatesql += "',[Station]='" + station_applyperson;
                            updatesql += "',[Department]='" + dept_applyperson;
                            updatesql += "',[ReportFile]='" + hdReport.Value.ToString();
                            updatesql += "',[Remark]='" + txtRemark.Text.Replace("'", "''") + "'";
                            updatesql += ",[Attach]='" + hdScanFile.Value.ToString() + "'";
                            updatesql += ",[PersonID]='" + hdOwnerID.Value.ToString() + "'";
                            updatesql += ",[CCMailList]='" + CCMailList + "'";
                            updatesql += ",[Budget]=1";
                            updatesql += ",[Station2]='" + ostation + "'";

                            //160123 垫付
                            if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                            {
                                updatesql += ",OnBehalfPersonID='" + hdOnBehalf.Value.ToString() + "'";
                                DataSet dsE = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(hdOnBehalf.Value.ToString());
                                if (dsE.Tables[0].Rows.Count == 1)
                                {
                                    DataTable dtE = dsE.Tables[0];
                                    updatesql += ",OnBehalfPersonName='" + dtE.Rows[0]["fullName"].ToString() + "'";
                                    updatesql += ",OnBehalfPersonUnit='" + dtE.Rows[0]["stationCode"].ToString() + "'";
                                    updatesql += ",OnBehalfPersonDept='" + dtE.Rows[0]["CRPDepartmentName"].ToString() + "'";
                                    updatesql += ",OnBehalfPersonCostCenter='" + dtE.Rows[0]["CostCenter"].ToString() + "'";
                                }
                            }


                            updatesql += " where ID=" + hdTravelRequestID.Value.ToString();

                            string newid = dbc.UpdateData("eReimbursement", updatesql, "Update");

                            if (newid == "-1")
                            {
                                ErrorHandle("Data Error.");
                                return;
                            }
                            else
                            {
                                if (!SaveDetail(detail, header0string, header1string, header2string, CurLocal, dept, CurBudget))
                                {
                                    ErrorHandle("Data Error.");
                                    return;
                                }
                                //hdTravelRequestID.Value = newid.Split(',')[0];//新增后记录ID
                                //Panel3.Title = "差旅费申请单:" + newid.Split(',')[1];
                                //X.AddScript("GridPanel2.submitData();");
                                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                {
                                    UpdateMSG("保存差旅费申请单草稿: " + hdTravelRequestNo.Value.ToString() + "成功.");
                                }
                                else
                                {
                                    UpdateMSG("Saved Travel Expense Draft: " + hdTravelRequestNo.Value.ToString() + " successfully.");
                                }
                            }

                        }
                        else//如果ID为空则判断为新增草稿
                        {
                            string word = "[No],[Person],[Station],[Department],[ReportFile],[Attach],[Remark],[Bdate],[Edate],[Type],[PersonID],[ApplyDate],[CCMailList],[Station2],[Budget]";
                            //160115 垫付
                            if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                            { word += ",OnBehalfPersonID,OnBehalfPersonName,OnBehalfPersonUnit,OnBehalfPersonDept,OnBehalfPersonCostCenter"; }
					
                            string value = "";
                            value += "'" + station_applyperson + DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString().PadLeft(2, '0') + "',";//edit
                            value += "'" + hdOwner.Value.ToString() + "',"; value += "'" + station_applyperson + "',"; value += "'" + dept_applyperson + "',";//edit
                            value += "'" + hdReport.Value.ToString() + "',";
                            value += "'" + hdScanFile.Value.ToString() + "',";
                            value += "'" + txtRemark.Text.Replace("'", "''") + "',";
                            value += "null,null,";
                            value += "1";//标识为草稿
                            value += ",'" + hdOwnerID.Value.ToString() + "'";
                            value += ",'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'";
                            value += ",'" + CCMailList + "'";
                            value += ",'" + ostation + "'";
                            value += ",1";
                            //160123 垫付
                            if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                            {
                                value += ",'" + hdOnBehalf.Value.ToString() + "'";
                                DataSet dsE = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(hdOnBehalf.Value.ToString());
                                if (dsE.Tables[0].Rows.Count == 1)
                                {
                                    DataTable dtE = dsE.Tables[0];
                                    value += ",'" + dtE.Rows[0]["fullName"].ToString() + "'";
                                    value += ",'" + dtE.Rows[0]["stationCode"].ToString() + "'";
                                    value += ",'" + dtE.Rows[0]["CRPDepartmentName"].ToString() + "'";
                                    value += ",'" + dtE.Rows[0]["CostCenter"].ToString() + "'";
                                }
                            }
                            string updatesql = "insert into ETravel (" + word + ") values(" + value + ");update ETravel set [No]=[No]+(select [MonthCount]=right('0000'+cast(count(ID) as varchar(10)),4) from ETravel where (month(ApplyDate) in (select month(ApplyDate) from ETravel where [ID]=@@IDENTITY) and (year(ApplyDate) in (select year(ApplyDate) from ETravel where [ID]=@@IDENTITY)) and Station=(select Station from ETravel where ID=@@IDENTITY)))+'TD' where ID=@@IDENTITY;select [msg]=convert(varchar,ID)+','+[No] from ETravel where ID=@@IDENTITY";

                            string newid = dbc.UpdateData("eReimbursement", updatesql, "Insert");

                            //操作Flow表
                            string wordflow = "[No],[Type],[Station],[Department],[Person],[CreadedBy],[CreatedDate],[RequestID],[Active]";
                            string valueflow = "";
                            valueflow += "'" + newid.Split(',')[1] + "',";
                            valueflow += "'T',";
                            valueflow += "'" + station_applyperson + "',";
                            valueflow += "'" + dept_applyperson + "',";
                            valueflow += "'" + hdOwner.Value.ToString() + "',";
                            valueflow += "'" + Request.Cookies.Get("eReimUserName").Value + "',";
                            valueflow += "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',";
                            valueflow += "'" + newid.Split(',')[0] + "'";
                            valueflow += ",1";
                            string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";
                            string rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
                            //
                            if (newid == "-1" || rows == "-1")
                            {
                                ErrorHandle("Data Error."); return;
                            }
                            else
                            {
                                hdTravelRequestID.Value = newid.Split(',')[0];//新增后记录ID
                                hdTravelRequestNo.Value = newid.Split(',')[1];//新增后记录No
                                if (!SaveDetail(detail, header0string, header1string, header2string, CurLocal, dept, CurBudget))
                                {
                                    ErrorHandle("Data Error.");
                                    return;
                                }
                                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                {
                                    Panel3.Title = "差旅费申请单草稿: " + newid.Split(',')[1];
                                    UpdateMSG("新增差旅费申请单草稿: " + newid.Split(',')[1] + "成功.");
                                }
                                else
                                {
                                    Panel3.Title = "Travel Expense Draft: " + newid.Split(',')[1];
                                    UpdateMSG("Added Travel Expense Draft: " + newid.Split(',')[1] + " successfully.");
                                }
                            }
                        }
                    }
                }
                //UpdateMSG("Un-Budget:" + UnBudget.ToString());
            }

            X.AddScript("btnGeteLeave.disable();cbxOnBehalfName.disable();cbxPerson.disable();");
            
            
        }
        protected DataTable GetUserInfo(string UserID)
        {
            DataTable dtuser = new DataTable();
            dtuser.Columns.Add("UserID", typeof(System.String));
            dtuser.Columns.Add("Station", typeof(System.String));
            dtuser.Columns.Add("Department", typeof(System.String));
            dtuser.Columns.Add("CostCenter", typeof(System.String));
            dtuser.Columns.Add("LocalCurrency", typeof(System.String));
            dtuser.Columns.Add("BudgetCurrency", typeof(System.String));
            DataRow dr = dtuser.NewRow();
            cs.DBCommand dbc = new cs.DBCommand();
            //取得币种转换值
            DataSet dsuserinfo = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(UserID);
            DataTable dtuserinfo = dsuserinfo.Tables[0];
            string station = dtuserinfo.Rows[0]["stationCode"].ToString();
            string costcenter = dtuserinfo.Rows[0]["CostCenter"].ToString();
            dr["Station"] = dtuserinfo.Rows[0]["stationCode"].ToString();
            dr["CostCenter"] = dtuserinfo.Rows[0]["CostCenter"].ToString();
            dr["Department"] = dtuserinfo.Rows[0]["CRPDepartmentName"].ToString();

            //获取该人币种
            string LocalCurrency = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(station);
            //检查是否本地维护过特殊币种
            DataTable dttemp = new DataTable();
            string sqltemp = "select * from ESUSER where Userid='" + UserID + "'";
            dttemp = dbc.GetData("eReimbursement", sqltemp);
            if (dttemp.Rows.Count > 0)
            {
                LocalCurrency = dttemp.Rows[0]["Currency"].ToString();//如果单独设置了币种
            }
            //获取预算站点币种
            string BudgetCurrency = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(costcenter);
            dr["LocalCurrency"] = LocalCurrency;
            dr["BudgetCurrency"] = BudgetCurrency;
            dtuser.Rows.Add(dr);
            return dtuser;
        }
        protected decimal GetRateByUserID(int year, bool con, string LocalCurrency, string BudgetCurrency)
        {
            //cs.DBCommand dbc = new cs.DBCommand();
            ////取得币种转换值
            //DataSet dsuserinfo = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(UserID);
            //DataTable dtuserinfo = dsuserinfo.Tables[0];
            //string station = dtuserinfo.Rows[0]["stationCode"].ToString();
            //string costcenter = dtuserinfo.Rows[0]["CostCenter"].ToString();
            ////获取该人币种
            //string LocalCurrency = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(station);
            ////检查是否本地维护过特殊币种
            //DataTable dttemp = new DataTable();
            //string sqltemp = "select * from ESUSER where Userid='" + UserID + "'";
            //dttemp = dbc.GetData("eReimbursement", sqltemp);
            //if (dttemp.Rows.Count > 0)
            //{
            //    LocalCurrency = dttemp.Rows[0]["Currency"].ToString();//如果单独设置了币种
            //}
            ////获取预算站点币种
            //string BudgetCurrency = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(costcenter);
            decimal rate = 1;
            if (con)
            {
                rate = DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(BudgetCurrency, LocalCurrency, year);
            }
            else
            {
                rate = DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(LocalCurrency, BudgetCurrency, year);
            }
            return rate;
        }
        protected bool SaveDetail(string detail, string header0string, string header1string, string header2string, string Cur, string dept,string costcenterCur)
        {
            SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eReimbursement"].ConnectionString);
            cs.DBCommand dbc = new cs.DBCommand();
            //删除现有数据
            string deletesql = "delete from ETraveleDetail where [No]='" + hdTravelRequestID.Value.ToString() + "'";
            string newid1 = dbc.UpdateData("eReimbursement", deletesql, "Update");

            //取得币种转换值
            decimal re = GetRateByUserID(DateTime.Now.Year, true,Cur,costcenterCur);

            
            //如果出差站点为空,则不保存信息
            StoreSubmitDataEventArgs eSubmit = new StoreSubmitDataEventArgs(detail, null);
            XmlNode xml = eSubmit.Xml;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.InnerXml);
            int dtcol = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes.Count;
            //doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).ChildNodes[j].InnerText;
            int dtrow = doc.SelectNodes("records").Item(0).SelectNodes("record").Count;
            int groupcount = (dtcol - 3) / 2;//多少个站点会被更新,如果站点为空则不更新
            decimal psum = 0, csum = 0;//记录该申请单的费用合计
            for (int i = 0; i < groupcount; i++)
            {
                if (header0string.Split(',')[i] != "NA")//站点为空则不更新
                {
                    //DataSet ds2 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(hdOwnerID.Value.ToString());
                    //string station = "",Tstation="";
                    //if (ds2.Tables[0].Rows.Count == 1)
                    //{
                    //    DataTable dt1 = ds2.Tables[0];
                    //    station = dt1.Rows[0]["stationCode"].ToString();
                    //    DataTable dttemp = new DataTable();
                    //    string sqltemp = "select * from ESUSER where Userid='" + hdOwnerID.Value.ToString() + "'";
                    //    dttemp = dbc.GetData("eReimbursement", sqltemp);
                    //    if (dttemp.Rows.Count > 0)
                    //    {
                    //        station = dttemp.Rows[0]["Station"].ToString();
                    //    }
                    //}
                    //Tstation = header1string.Split(',')[i] == "NA" ? station : header1string.Split(',')[i];
                    //double re = System.Math.Round(DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(station) / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(Tstation), 4);
                    //共新增11行固定数据
                    for (int j = 0; j < 11; j++)
                    {
                        try
                        {
                            SqlCommand scdetail = sqlConn.CreateCommand();
                            scdetail.CommandText = "insert into ETraveleDetail ([No],[Tocity],[AccountCode],[Cur],[Pamount],[Camount],[TSation],[Createdby],[CreadedDate],[Tdate],[Department1],[DetailCode],[Tdate0],[CenterAmountP],[CenterAmountC]) values (@No,@Tocity,@AccountCode,@Cur,@Pamount,@Camount,@TSation,@Createdby,@CreadedDate,@Tdate,@Department1,@DetailCode,@Tdate0,@CenterAmountP,@CenterAmountC)";
                            SqlParameter spdetail = new SqlParameter("@No", SqlDbType.VarChar, 50);
                            spdetail.Value = hdTravelRequestID.Value.ToString();
                            scdetail.Parameters.Add(spdetail);

                            spdetail = new SqlParameter("@Tocity", SqlDbType.VarChar, 100);
                            spdetail.Value = header0string.Split(',')[i];
                            scdetail.Parameters.Add(spdetail);

                            spdetail = new SqlParameter("@AccountCode", SqlDbType.VarChar, 50);
                            switch (j)
                            {
                                case 4:
                                    spdetail.Value = "62010900";
                                    break;
                                case 5:
                                    spdetail.Value = "62011900";
                                    break;
                                case 6:
                                    spdetail.Value = "62010500";
                                    break;
                                default:
                                    spdetail.Value = "62012000";
                                    break;
                            }
                            scdetail.Parameters.Add(spdetail);

                            spdetail = new SqlParameter("@Cur", SqlDbType.VarChar, 50);
                            spdetail.Value = Cur;
                            scdetail.Parameters.Add(spdetail);

                            spdetail = new SqlParameter("@Pamount", SqlDbType.Decimal);
                            if (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(j).ChildNodes[1 + i * 2].InnerText != "")
                            {
                                spdetail.Value = System.Math.Round(Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(j).ChildNodes[1 + i * 2].InnerText), 2);
                                psum += System.Math.Round(Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(j).ChildNodes[1 + i * 2].InnerText), 2);
                            }
                            else
                            {
                                spdetail.Value = DBNull.Value;
                            }
                            scdetail.Parameters.Add(spdetail);

                            spdetail = new SqlParameter("@CenterAmountP", SqlDbType.Decimal);
                            if (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(j).ChildNodes[1 + i * 2].InnerText != "")
                            {
                                spdetail.Value = Convert.ToDecimal(System.Math.Round(Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(j).ChildNodes[1 + i * 2].InnerText) * re, 2));
                                
                            }
                            else
                            {
                                spdetail.Value = DBNull.Value;
                            }
                            scdetail.Parameters.Add(spdetail);

                            spdetail = new SqlParameter("@Camount", SqlDbType.Decimal);
                            if (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(j).ChildNodes[2 + i * 2].InnerText != "")
                            {
                                spdetail.Value = System.Math.Round(Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(j).ChildNodes[2 + i * 2].InnerText), 2);
                                csum += System.Math.Round(Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(j).ChildNodes[2 + i * 2].InnerText), 2);
                            }
                            else
                            {
                                spdetail.Value = DBNull.Value;
                            }
                            scdetail.Parameters.Add(spdetail);

                            spdetail = new SqlParameter("@CenterAmountC", SqlDbType.Decimal);
                            if (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(j).ChildNodes[2 + i * 2].InnerText != "")
                            {
                                spdetail.Value = Convert.ToDecimal(System.Math.Round(Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(j).ChildNodes[2 + i * 2].InnerText) * re, 2));

                            }
                            else
                            {
                                spdetail.Value = DBNull.Value;
                            }
                            scdetail.Parameters.Add(spdetail);

                            spdetail = new SqlParameter("@TSation", SqlDbType.VarChar, 50);
                            spdetail.Value = header1string.Split(',')[i] == "NA" ? "" : header1string.Split(',')[i];
                            scdetail.Parameters.Add(spdetail);

                            spdetail = new SqlParameter("@Createdby", SqlDbType.VarChar, 50);
                            spdetail.Value = hdOwnerID.Value.ToString();
                            scdetail.Parameters.Add(spdetail);

                            spdetail = new SqlParameter("@CreadedDate", SqlDbType.DateTime);
                            spdetail.Value = DateTime.Now;
                            scdetail.Parameters.Add(spdetail);

                            spdetail = new SqlParameter("@Tdate", SqlDbType.DateTime);
                            if (header2string.Split(',')[i * 2 + 1] == "NA")
                            {
                                spdetail.Value = DBNull.Value;
                            }
                            else
                            {
                                spdetail.Value = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]);
                            }
                            scdetail.Parameters.Add(spdetail);

                            spdetail = new SqlParameter("@Tdate0", SqlDbType.DateTime);
                            if (header2string.Split(',')[i * 2] == "NA")
                            {
                                spdetail.Value = DBNull.Value;
                            }
                            else
                            {
                                spdetail.Value = Convert.ToDateTime(header2string.Split(',')[i * 2]);
                            }
                            scdetail.Parameters.Add(spdetail);

                            spdetail = new SqlParameter("@Department1", SqlDbType.VarChar, 50);
                            spdetail.Value = dept;
                            scdetail.Parameters.Add(spdetail);

                            spdetail = new SqlParameter("@DetailCode", SqlDbType.Int);
                            spdetail.Value = j;
                            scdetail.Parameters.Add(spdetail);

                            sqlConn.Open();
                            int row = scdetail.ExecuteNonQuery();
                            sqlConn.Close();
                        }
                        catch (Exception er)
                        {
                            throw er;
                        }
                    }
                }
            }
            try
            {
                SqlCommand scdetail = sqlConn.CreateCommand();
                scdetail.CommandText = "update ETravel set Pamout=@Pamout,Camount=@Camount,Tamount=@Tamount where [ID]=@ID";
                SqlParameter spdetail = new SqlParameter("@ID", SqlDbType.VarChar, 50);
                spdetail.Value = hdTravelRequestID.Value.ToString();
                scdetail.Parameters.Add(spdetail);

                spdetail = new SqlParameter("@Pamout", SqlDbType.Decimal);
                spdetail.Value = psum;
                scdetail.Parameters.Add(spdetail);

                spdetail = new SqlParameter("@Camount", SqlDbType.Decimal);
                spdetail.Value = csum;
                scdetail.Parameters.Add(spdetail);

                spdetail = new SqlParameter("@Tamount", SqlDbType.Decimal);
                spdetail.Value = psum + csum;
                scdetail.Parameters.Add(spdetail);

                sqlConn.Open();
                int row = scdetail.ExecuteNonQuery();
                sqlConn.Close();
            }
            catch (Exception)
            {

                throw;
            }

            return true;
        }
        protected bool CheckBudget(string userid, string department, string ostation, string tstation, string coacode, string year, string month)
        {
            DataTable dtbudget = new DataTable();
            dtbudget = Comm.RtnEB(userid, department, ostation, tstation, coacode, year, month);
            if (dtbudget != null && dtbudget.Rows.Count > 0)
            {
                decimal stationbudget = 0, departmentbudget = 0, personbudget = 0, stationYear = 0, departmentYear = 0, personYear = 0;
                for (int j = 0; j < dtbudget.Rows.Count; j++)
                {
                    if (dtbudget.Rows[j]["Type"].ToString() == "站点")
                    {
                        stationbudget = Convert.ToDecimal(dtbudget.Rows[j]["Budget"].ToString());
                    }
                    else if (dtbudget.Rows[j]["Type"].ToString() == "部门")
                    {
                        departmentbudget = Convert.ToDecimal(dtbudget.Rows[j]["Budget"].ToString());
                    }
                    else if (dtbudget.Rows[j]["Type"].ToString() == "个人")
                    {
                        personbudget = Convert.ToDecimal(dtbudget.Rows[j]["Budget"].ToString());
                    }
                    else if (dtbudget.Rows[j]["Type"].ToString() == "全年个人")
                    {
                        personYear = Convert.ToDecimal(dtbudget.Rows[j]["Budget"].ToString());
                    }
                    else if (dtbudget.Rows[j]["Type"].ToString() == "全年部门")
                    {
                        departmentYear = Convert.ToDecimal(dtbudget.Rows[j]["Budget"].ToString());
                    }
                    else if (dtbudget.Rows[j]["Type"].ToString() == "全年站点")
                    {
                        stationYear = Convert.ToDecimal(dtbudget.Rows[j]["Budget"].ToString());
                    }
                }
                if (stationbudget == 0 && departmentbudget == 0 && personbudget == 0)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }
        protected void GetCity(object sender, DirectEventArgs e)
        {
            var con = sender;
            DataSet GetCityInfo = DIMERCO.SDK.Utilities.LSDK.GetCityInfo(X.GetValue("cbxCity"), 10);
            DataTable dtcity = GetCityInfo.Tables[0];
            DataTable dtcitynew = new DataTable();
            dtcitynew.Columns.Add("cityID", System.Type.GetType("System.String"));
            dtcitynew.Columns.Add("cityCode", System.Type.GetType("System.String"));
            dtcitynew.Columns.Add("cityName", System.Type.GetType("System.String"));
            dtcitynew.Columns.Add("CountryName", System.Type.GetType("System.String"));
            for (int c = 0; c < dtcity.Rows.Count; c++)
            {
                DataRow dr = dtcitynew.NewRow();
                dr["cityID"] = dtcity.Rows[c]["cityID"].ToString();
                dr["cityCode"] = dtcity.Rows[c]["cityCode"].ToString();
                dr["cityName"] = dtcity.Rows[c]["cityName"].ToString();
                dr["CountryName"] = dtcity.Rows[c]["CountryName"].ToString();
                dtcitynew.Rows.Add(dr);
            }
            //StoreCity.DataSource = dtcitynew;
            //StoreCity.DataBind();
        }
        [DirectMethod]
        public void LoadBudget1(string TStation, string row, string date, string Category)
        {
            //判断成本中心名称是否合法
            //if (TStation != "")
            //{
            //    bool isstation = DIMERCO.SDK.Utilities.LSDK.isCostCenterExist(TStation);
            //    if (!isstation)//如果不是合法站点,提示可选站点
            //    {
            //        DataSet GetCityInfo = DIMERCO.SDK.Utilities.LSDK.getCostCenterBYStationCode(TStation, 5);
            //        DataTable dtGetCityInfo = (DataTable)GetCityInfo.Tables[0];
            //        string suggestStation = "";
            //        for (int j = 0; j < dtGetCityInfo.Rows.Count; j++)
            //        {
            //            suggestStation += dtGetCityInfo.Rows[j]["StationCode"].ToString() + ",";
            //        }
            //        if (suggestStation.Trim() != "")
            //        {
            //            suggestStation = suggestStation.Substring(0, suggestStation.Length - 1);
            //            lbCOA.Text = "Invalid Cost Center(" + TStation + ").Suggestion:" + suggestStation + ".";
            //            //X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Invalid Cost Center(" + TStation + ").Suggestion:" + suggestStation + ".', buttons: { ok: 'Ok' }, fn: function (btn) {  } });");
            //            return;
            //        }
            //        else
            //        {
            //            lbCOA.Text = "Please input valid Cost Center.";
            //            //X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Please input valid Cost Center.', buttons: { ok: 'Ok' }, fn: function (btn) {  } });");
            //            return;
            //        }
            //    }

            //}
            //cs.DBCommand dbc = new cs.DBCommand();
            //string userid = hdOwnerID.Value.ToString();
            //string ostation = "";
            //DataSet ds2 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(userid);
            //if (ds2.Tables[0].Rows.Count == 1)
            //{
            //    DataTable dt1 = ds2.Tables[0];
            //    //dpt = dt1.Rows[0]["DepartmentName"].ToString();
            //    ostation = dt1.Rows[0]["stationCode"].ToString();

            //    DataTable dttemp = new DataTable();
            //    string sqltemp = "select * from ESUSER where Userid='" + userid + "'";
            //    dttemp = dbc.GetData("eReimbursement", sqltemp);
            //    if (dttemp.Rows.Count > 0)
            //    {
            //        ostation = dttemp.Rows[0]["Station"].ToString();
            //    }
            //}
            //string station = ""; string department = "";
            //DataSet ds1 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(hdOwnerID.Value.ToString());
            //if (ds1.Tables[0].Rows.Count == 1)
            //{
            //    DataTable dt1 = ds1.Tables[0];
            //    station = dt1.Rows[0]["stationCode"].ToString();
            //    department = dt1.Rows[0]["CRPDepartmentName"].ToString();
            //}
            //string tstation = "", year = "", month = "", coacode = "";
            //tstation = TStation == "" ? ostation : TStation;
            //year = Convert.ToDateTime(date).Year.ToString();
            //month = Convert.ToDateTime(date).Month.ToString();
            //DataTable dtbudget = new DataTable();
            //switch (row)
            //{
            //    case "4":
            //        coacode = "62010900";
            //        break;
            //    case "5":
            //        coacode = "62011900";
            //        break;
            //    case "6":
            //        coacode = "62010500";
            //        break;
            //    default:
            //        coacode = "62012000";
            //        break;
            //}
            //dtbudget = Comm.RtnEB(userid, department, ostation, tstation, coacode, year, month);
            //double re = System.Math.Round(DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(tstation) / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(ostation), 4);
            //if (dtbudget != null && dtbudget.Rows.Count > 0)
            //{
            //    lbStationBG.Text = "Station(YTD):NA";
            //    lbDepartmentBG.Text = "Department(YTD):NA";
            //    lbStaffBG.Text = "Person(YTD):NA";
            //    decimal stationbudget = 0, departmentbudget = 0, personbudget = 0;
            //    for (int j = 0; j < dtbudget.Rows.Count; j++)
            //    {
            //        decimal budget = 0, used = 0;
            //        budget = Convert.ToDecimal(System.Math.Round(Convert.ToDouble(dtbudget.Rows[j]["Budget"].ToString()) * re, 2));
            //        used = Convert.ToDecimal(System.Math.Round(Convert.ToDouble(dtbudget.Rows[j]["Used"].ToString()) * re, 2));
                    
            //        if (dtbudget.Rows[j]["Type"].ToString() == "全年个人")
            //        {
            //            personbudget = budget;
            //            string stationbg = hdOwner.Value.ToString() + "(YTD): " + used.ToString("#,##0.00") + "/" + budget.ToString("#,##0.00");
            //            lbStaffBG.Text = stationbg;
            //        }
            //        else if (dtbudget.Rows[j]["Type"].ToString() == "全年部门")
            //        {
            //            departmentbudget = budget;
            //            string stationbg = department + "(YTD): " + used.ToString("#,##0.00") + "/" + budget.ToString("#,##0.00");
            //            lbDepartmentBG.Text = stationbg;
            //        }
            //        else if (dtbudget.Rows[j]["Type"].ToString() == "全年站点")
            //        {
            //            stationbudget = budget;
            //            string stationbg = tstation + "(YTD): " + used.ToString("#,##0.00") + "/" + budget.ToString("#,##0.00");
            //            lbStationBG.Text = stationbg;
            //        }
            //    }
            //    if (stationbudget == 0 && departmentbudget == 0 && personbudget == 0)
            //    {
            //        lbCOA.Text = "No budget(" + tstation + ":" + Category + "),please check with Account.";
            //        return;
            //    }
            //    else
            //    {
            //        lbCOA.Text = "";
            //        switch (row)
            //        {
            //            case "0":
            //                lbCOA.Text = "1. Air Ticket - Intl";
            //                break;
            //            case "1":
            //                lbCOA.Text = "Air Ticket - Domestic";
            //                break;
            //            case "2":
            //                lbCOA.Text = "2. Hotel Bill";
            //                break;
            //            case "3":
            //                lbCOA.Text = "3. Meals";
            //                break;
            //            case "4":
            //                lbCOA.Text = "4. Entertainment";
            //                break;
            //            case "5":
            //                lbCOA.Text = "5. Car Rental/Transportation";
            //                break;
            //            case "6":
            //                lbCOA.Text = "6. Communication";
            //                break;
            //            case "7":
            //                lbCOA.Text = "7. Local Trip Allowance";
            //                break;
            //            case "8":
            //                lbCOA.Text = "8. Overseas Trip Allowance";
            //                break;
            //            case "9":
            //                lbCOA.Text = "9. Airport Tax/Travel Insurance";
            //                break;
            //            case "10":
            //                lbCOA.Text = "10. Others";
            //                break;
            //            default:
            //                lbCOA.Text = "Data error.";
            //                break;
            //        }
            //    }
            //}
        }
        [DirectMethod]
        public void CheckEFlow()
        {
            cs.DBCommand dbc = new cs.DBCommand();
            //检查是否已经为该申请人设置过审批人
            string sqlCheckFlow = "";
            if (Radio1.Checked)//使用Budget审批流程
            {
                sqlCheckFlow = "select * from GroupFlow where [Type]!=2 and GID=(select GID from GroupUsers where UserID='" + hdOwnerID.Value.ToString() + "')";
            }
            else//使用unBudget审批流程
            {
                sqlCheckFlow = "select * from GroupFlow where [Type]=2 and GID=(select GID from GroupUsers where UserID='" + hdOwnerID.Value.ToString() + "')";
            }
            DataTable dtGroupFlowData = dbc.GetData("eReimbursement", sqlCheckFlow);
            if (dtGroupFlowData.Rows.Count < 1)
            {
                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                {
                    ErrorHandle("请先设置审批人.");
                }
                else
                {
                    ErrorHandle("Not set Approve flow,please contact with Local MIS.");
                }
                return;
            }
        }
    }
}