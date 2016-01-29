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
                                ////准备下拉菜单内容
                                //Ext.Net.ListItem li = new Ext.Net.ListItem(Request.Cookies.Get("eReimUserName").Value, Request.Cookies.Get("eReimUserID").Value);
                                //cbxOwner.Items.Add(li);
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
                                //        cbxOwner.Items.Add(li);
                                //        itemcount++;
                                //    }
                                //}
                                //更改按钮状态
                                if (dt.Rows[0]["Step"].ToString() != "0")//正式申请单
                                {
                                    if (Request.QueryString["Copy"] != null)
                                    {
                                        if (Request.QueryString["Copy"].ToString() == "T")//Copy,作为新增
                                        {
                                            X.AddScript("btnSaveAndSend.enable();Radio1.enable();Radio2.enable();");
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
                                                X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();Radio1.disable();Radio2.disable();btnSaveDraft.disable();btnUploadReport.disable();btnUploadScanFile.disable();Button1.disable();Button2.disable();btnCC.disable();");
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
                                                X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();Radio1.disable();Radio2.disable();btnSaveDraft.disable();btnUploadReport.disable();btnUploadScanFile.disable();Button1.disable();Button2.disable();btnCC.disable();");
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
                                            X.AddScript("btnSaveAndSend.enable();Radio1.enable();Radio2.enable();");
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

                                        X.AddScript("btnSaveAndSend.enable();Radio1.enable();Radio2.enable();");
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
                                    //Ext.Net.ListItem li = new Ext.Net.ListItem(dt.Rows[0]["Person"].ToString(), dt.Rows[0]["PersonID"].ToString());
                                    //cbxOwner.Items.Add(li);
                                    //更改按钮状态
                                    if (dt.Rows[0]["Step"].ToString() != "0")//正式申请单
                                    {
                                        if (Request.QueryString["Copy"] != null)
                                        {
                                            if (Request.QueryString["Copy"].ToString() == "T")//Copy,作为新增
                                            {
                                                X.AddScript("btnSaveAndSend.enable();Radio1.enable();Radio2.enable();");
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
                                                    X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();Radio1.disable();Radio2.disable();btnSaveDraft.disable();btnUploadReport.disable();btnUploadScanFile.disable();Button1.disable();Button2.disable();btnCC.disable();");
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
                                                    X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();Radio1.disable();Radio2.disable();btnSaveDraft.disable();btnUploadReport.disable();btnUploadScanFile.disable();Button1.disable();Button2.disable();btnCC.disable();");
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
                                                X.AddScript("btnSaveAndSend.enable();Radio1.enable();Radio2.enable();");
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
                                            X.AddScript("btnSaveAndSend.enable();Radio1.enable();Radio2.enable();");
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
                                        //Ext.Net.ListItem li = new Ext.Net.ListItem(dt.Rows[0]["Person"].ToString(), dt.Rows[0]["PersonID"].ToString());
                                        //cbxOwner.Items.Add(li);
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
                                                    X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();Radio1.disable();Radio2.disable();btnSaveDraft.disable();btnUploadReport.disable();btnUploadScanFile.disable();Button1.disable();Button2.disable();btnCC.disable();");
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
                                                    X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();Radio1.disable();Radio2.disable();btnSaveDraft.disable();btnUploadReport.disable();btnUploadScanFile.disable();Button1.disable();Button2.disable();btnCC.disable();");
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
                    ////准备下拉菜单内容
                    //Ext.Net.ListItem li = new Ext.Net.ListItem(Request.Cookies.Get("eReimUserName").Value, Request.Cookies.Get("eReimUserID").Value);
                    //cbxOwner.Items.Add(li);
                    //string sqlitem = "select * from Eagent where [St]=1 and [PAgentID]='" + Request.Cookies.Get("eReimUserID").Value + "'";
                    //DataTable dtitem = dbc.GetData("eReimbursement", sqlitem);
                    //for (int j = 0; j < dtitem.Rows.Count; j++)
                    //{
                    //    string sqlpara = sqlitem;
                    //    bool d1 = true;
                    //    bool d2 = false;
                    //    if (dtitem.Rows[j][5].ToString() != "")
                    //    {
                    //        //sqlpara += " and getdate()>='" + dtitem.Rows[j]["Bdate"].ToString() + "' ";
                    //        if (DateTime.Now >= Convert.ToDateTime(dtitem.Rows[j][5].ToString()))
                    //        {
                    //            d1 = true;
                    //        }
                    //        else
                    //        {
                    //            d1 = false;
                    //        }
                    //    }
                    //    if (dtitem.Rows[j][6].ToString() != "")
                    //    {
                    //        //sqlpara += " and getdate()<='" + dtitem.Rows[j]["Edate"].ToString() + "' ";
                    //        if (DateTime.Now <= Convert.ToDateTime(dtitem.Rows[j][6].ToString()))
                    //        {
                    //            d2 = true;
                    //        }
                    //        else
                    //        {
                    //            d2 = false;
                    //        }
                    //    }
                    //    if (d1 && d2)
                    //    {
                    //        li = new Ext.Net.ListItem(dtitem.Rows[j]["Owner"].ToString(), dtitem.Rows[j]["OwnerID"].ToString());
                    //        cbxOwner.Items.Add(li);
                    //    }
                    //}
                    ////新增记录时,默认为登录用户
                    //cbxOwner.SelectedItem.Value = Request.Cookies.Get("eReimUserID").Value;
                    //cbxOwner.SelectedItem.Text = Request.Cookies.Get("eReimUserName").Value;
                    //hdOwner.Value = Request.Cookies.Get("eReimUserName").Value;
                    //hdOwnerID.Value = Request.Cookies.Get("eReimUserID").Value;
                    //labelOwner.Text = Request.Cookies.Get("eReimUserName").Value;

                    //labelStation.Text = Request.Cookies.Get("eReimStation").Value;
                    //labelDepartment.Text = Request.Cookies.Get("eReimDepartment").Value;
                    //LabelCurrency.Text = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(Request.Cookies.Get("eReimCostCenter").Value);
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
                    X.AddScript("btnSaveAndSend.enable();Radio1.enable();Radio2.enable();");
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
                    colP.Header = "Person Pay";
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
                    colC.Header = "Company Pay";
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
                    colP1.Header = "Person Pay";
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
                    colC1.Header = "Company Pay";
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
                    var TotalP = new Ext.Net.TextField();
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

                    var TotalC = new Ext.Net.TextField();
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
                    TitleTotalP.Text = "Total(Person)";
                    TitleTotalP.Cls = "custom-row";
                    HeaderColumn hcTitleTotalP = new HeaderColumn();
                    hcTitleTotalP.Component.Add(TitleTotalP);
                    this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTitleTotalP);

                    var TitleTotalC = new Ext.Net.Label();
                    TitleTotalC.Text = "Total(Comp)";
                    TitleTotalC.Cls = "custom-row";
                    HeaderColumn hcTitleTotalC = new HeaderColumn();
                    hcTitleTotalC.Component.Add(TitleTotalC);
                    this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTitleTotalC);

                    X.AddScript("GridPanel2.colModel.setHidden(3, true);GridPanel2.colModel.setHidden(4, true);Store2.removeField('Station_1_P');Store2.removeField('Station_1_C');");
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
            
            if (CheckCopy)//根据Copy判断是否需要判断Copy状态
            {
                if (Request.QueryString["Copy"] != null)
                {
                    if (Request.QueryString["Copy"].ToString() == "T")//Copy而已,作为新增
                    {
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
                    //labelOwner.Text = dt.Rows[0]["Person"].ToString();
                    //hdOwner.Value = dt.Rows[0]["Person"].ToString();
                    //hdOwnerID.Value = dt.Rows[0]["PersonID"].ToString();
                    //labelStation.Text = dt.Rows[0]["Station"].ToString();
                    //labelDepartment.Text = dt.Rows[0]["Department"].ToString();
                    //获取该人币种
                    //LabelCurrency.Text = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dt.Rows[0]["Station"].ToString());
                    //hdCurrency.Value = System.Math.Round(1 / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(dt.Rows[0]["Station"].ToString()), 3);

                    //DataSet ds1 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dt.Rows[0]["PersonID"].ToString());
                    //if (ds1.Tables[0].Rows.Count == 1)
                    //{
                    //    DataTable dt1 = ds1.Tables[0];
                    //    LabelCurrency.Text = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dt1.Rows[0]["stationCode"].ToString());
                    //    hdCurrency.Value = System.Math.Round(1 / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(dt1.Rows[0]["stationCode"].ToString()), 3);
                    //    DataTable dttemp = new DataTable();
                    //    string sqltemp = "select * from ESUSER where Userid='" + dt.Rows[0]["PersonID"].ToString() + "'";
                    //    dttemp = dbc.GetData("eReimbursement", sqltemp);
                    //    if (dttemp.Rows.Count > 0)
                    //    {
                    //        LabelCurrency.Text = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dttemp.Rows[0]["Station"].ToString());
                    //        hdCurrency.Value = System.Math.Round(1 / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(dttemp.Rows[0]["Station"].ToString()), 3);
                    //    }
                    //}
                    //else
                    //{
                    //    ErrorHandle("Data Error."); return;
                    //}

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
                //labelOwner.Text = dt.Rows[0]["Person"].ToString();
                //hdOwner.Value = dt.Rows[0]["Person"].ToString();
                //hdOwnerID.Value = dt.Rows[0]["PersonID"].ToString();
                //labelStation.Text = dt.Rows[0]["Station"].ToString();
                //labelDepartment.Text = dt.Rows[0]["Department"].ToString();
                //获取该人币种
                //LabelCurrency.Text = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dt.Rows[0]["Station"].ToString());
                //hdCurrency.Value = System.Math.Round(1 / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(dt.Rows[0]["Station"].ToString()), 3);

                //DataSet ds1 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dt.Rows[0]["PersonID"].ToString());
                //if (ds1.Tables[0].Rows.Count == 1)
                //{
                //    DataTable dt1 = ds1.Tables[0];
                //    LabelCurrency.Text = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dt1.Rows[0]["stationCode"].ToString());
                //    hdCurrency.Value = System.Math.Round(1 / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(dt1.Rows[0]["stationCode"].ToString()), 3);
                //    DataTable dttemp = new DataTable();
                //    string sqltemp = "select * from ESUSER where Userid='" + dt.Rows[0]["PersonID"].ToString() + "'";
                //    dttemp = dbc.GetData("eReimbursement", sqltemp);
                //    if (dttemp.Rows.Count > 0)
                //    {
                //        LabelCurrency.Text = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dttemp.Rows[0]["Station"].ToString());
                //        hdCurrency.Value = System.Math.Round(1 / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(dttemp.Rows[0]["Station"].ToString()), 3);
                //    }
                //}
                //else
                //{
                //    ErrorHandle("Data Error."); return;
                //}

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
            //Radio1.Checked = dt.Rows[0]["Budget"].ToString() == "1" ? true : false;
            //Radio2.Checked = dt.Rows[0]["Budget"].ToString() != "1" ? true : false;
            //cbxBudget.SelectedItem.Value = dt.Rows[0]["Budget"].ToString() == "1" ? "YES" : "NO";

            
            ////获取该人币种
            //DataSet ds1 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dt.Rows[0]["PersonID"].ToString());
            //if (ds1.Tables[0].Rows.Count == 1)
            //{
            //    DataTable dt1 = ds1.Tables[0];
            //    LabelCurrency.Text = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dt1.Rows[0]["CostCenter"].ToString());

            //    DataTable dttemp = new DataTable();
            //    string sqltemp = "select * from ESUSER where Userid='" + dt.Rows[0]["PersonID"].ToString() + "'";
            //    dttemp = dbc.GetData("eReimbursement", sqltemp);
            //    if (dttemp.Rows.Count > 0)
            //    {
            //        LabelCurrency.Text = dttemp.Rows[0]["Currency"].ToString();
            //    }
            //}
            //else
            //{
            //    ErrorHandle("Data Error."); return;
            //}

            //载入已经保存的基本信息
            labelOwner.Text = dt.Rows[0]["Person"].ToString();
            hdOwner.Value = dt.Rows[0]["Person"].ToString();
            hdOwnerID.Value = dt.Rows[0]["PersonID"].ToString();
            labelStation.Text = dt.Rows[0]["Station"].ToString();
            labelDepartment.Text = dt.Rows[0]["Department"].ToString();

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
            dtbudget.Columns.Add("EName", typeof(System.String));
            dtbudget.Columns.Add("Type", typeof(System.String));
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
            //StoreBudget添加Field
            StoreBudget.Reader[0].Fields.Add("EName", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("Type", RecordFieldType.String);
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
            //取得预算日期
            string sqlA = "select convert(varchar(10),min(Tdate0),111) as BudgetDate from ETraveleDetail where No='" + ID + "'";
            DataTable dtA = dbc.GetData("eReimbursement", sqlA);
            //取得4大类合计
            //string sqlB = "select sum(T1) as T1,sum(T2) as T2,sum(T3) as T3,sum(T4) as T4 from (select case when AccountCode='62012000' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T1],case when AccountCode='62010900' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T2],case when AccountCode='62011900' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T3],case when AccountCode='62010500' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T4] from ETraveleDetail where No=" + ID + ") t";
            string sqlB = "select sum(isnull(Pamount,0)+isnull(Camount,0)) as Amount,'62010900' as Type from ETraveleDetail where No=" + ID + " and AccountCode='62012000' union all select sum(isnull(Pamount,0)+isnull(Camount,0)) as Amount,'62011900' as Type from ETraveleDetail where No=" + ID + " and AccountCode='62010900' union all select sum(isnull(Pamount,0)+isnull(Camount,0)) as Amount,'62010500' as Type from ETraveleDetail where No=" + ID + " and AccountCode='62011900' union all select sum(isnull(Pamount,0)+isnull(Camount,0)) as Amount,'62012000' as Type from ETraveleDetail where No=" + ID + " and AccountCode='62010500'";
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
                    dr["Type"] = dtB.Rows[g]["Type"].ToString();
                    accountcode = dtB.Rows[g]["Type"].ToString();
                    DataTable dtC = new DataTable();
                    dtC = Comm.RtnEB(userid, dpt, ostation, tstation, accountcode, year, month);
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
            //计算%,取得名称
            for (int i = 0; i < dtbudget.Rows.Count; i++)
            {
                if (Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()) != 0)//如果Budget不为0,则计算%
                {
                    dtbudget.Rows[i]["PPercent"] = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["PU"].ToString()) / Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()), 4) * 100;
                }
                if (Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()) != 0)//如果Budget不为0,则计算%
                {
                    dtbudget.Rows[i]["DPercent"] = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["DU"].ToString()) / Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()), 4) * 100;
                }
                if (Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()) != 0)//如果Budget不为0,则计算%
                {
                    dtbudget.Rows[i]["SPercent"] = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["SU"].ToString()) / Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()), 4) * 100;
                }
                if (dtbudget.Rows[i]["Type"].ToString()=="62010900")
                {
                    dtbudget.Rows[i]["EName"] = "Travel expense";
                }
                else if (dtbudget.Rows[i]["Type"].ToString() == "62011900")
                {
                    dtbudget.Rows[i]["EName"] = "Entertainment";
                }
                else if (dtbudget.Rows[i]["Type"].ToString() == "62010500")
                {
                    dtbudget.Rows[i]["EName"] = "Transportation";
                }
                else if (dtbudget.Rows[i]["Type"].ToString() == "62012000")
                {
                    dtbudget.Rows[i]["EName"] = "Communication";
                }
            }
            
            bool PB = false, DB = false, SB = false;
            for (int i = 0; i < dtbudget.Rows.Count; i++)
            {
                if (Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()) != 0)//是否显示个人预算列
                {
                    PB = true;
                }
                if (Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()) != 0)//是否显示部门预算列
                {
                    DB = true;
                }
                if (Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()) != 0)//是否显示站点预算列
                {
                    SB = true;
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
                    colP.Header = "Person Pay";
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
                    colC.Header = "Company Pay";
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
                colP1.Header = "Person Pay";
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
                colC1.Header = "Company Pay";
                colC1.DataIndex = "Station_1_C";
                colC1.Sortable = false;
                colC1.Resizable = false;
                colC1.MenuDisabled = true;
                colC1.Width = 110;
                colC1.Locked = true;
                colC1.Editor.Add(txtC1);
                colC1.Hidden = true;
                this.GridPanel2.ColumnModel.Columns.Add(colC1);

                var TotalP = new Ext.Net.TextField();
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

                var TotalC = new Ext.Net.TextField();
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
                TitleTotalP.Text = "Total(Person)";
                TitleTotalP.Cls = "custom-row";
                HeaderColumn hcTitleTotalP = new HeaderColumn();
                hcTitleTotalP.Component.Add(TitleTotalP);
                this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTitleTotalP);

                HeaderColumn hcTotal2 = new HeaderColumn();
                this.GridPanel2.GetView().HeaderRows[0].Columns.Add(hcTotal2);

                HeaderColumn hcTotal4 = new HeaderColumn();
                this.GridPanel2.GetView().HeaderRows[1].Columns.Add(hcTotal4);

                var TitleTotalC = new Ext.Net.Label();
                TitleTotalC.Text = "Total(Comp)";
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
                    colP.Header = "Person Pay";
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
                    colC.Header = "Company Pay";
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

                var TotalP = new Ext.Net.TextField();
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

                var TotalC = new Ext.Net.TextField();
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
                TitleTotalP.Text = "Total(Person)";
                TitleTotalP.Cls = "custom-row";
                HeaderColumn hcTitleTotalP = new HeaderColumn();
                hcTitleTotalP.Component.Add(TitleTotalP);
                this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTitleTotalP);

                var TitleTotalC = new Ext.Net.Label();
                TitleTotalC.Text = "Total(Comp)";
                TitleTotalC.Cls = "custom-row";
                HeaderColumn hcTitleTotalC = new HeaderColumn();
                hcTitleTotalC.Component.Add(TitleTotalC);
                this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTitleTotalC);
            }
            //根据语言设置
            //string sqlp = "";
            //if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
            //{
            //    sqlp += ",[COAName]=t2.ADes";
            //}
            //else
            //{
            //    sqlp += ",[COAName]=t2.SAccountName";
            //}

            //string detailsql = "select t1.* from ETraveleDetail t1 where t1.No='" + ID + "' order by DetailCode";
            //DataTable dtdetail = new DataTable();
            //dtdetail = dbc.GetData("eReimbursement", detailsql);
            //DataTable dtnew = new DataTable();
            //dtnew.Columns.Add("DetailID", System.Type.GetType("System.String"));
            //dtnew.Columns.Add("Tocity", System.Type.GetType("System.String"));
            //dtnew.Columns.Add("AccountCode", System.Type.GetType("System.String"));
            //dtnew.Columns.Add("Cur", System.Type.GetType("System.String"));
            //dtnew.Columns.Add("Pamount", System.Type.GetType("System.String"));
            //dtnew.Columns.Add("Camount", System.Type.GetType("System.String"));
            //dtnew.Columns.Add("TSation", System.Type.GetType("System.String"));
            //dtnew.Columns.Add("Tdate", System.Type.GetType("System.String"));
            //dtnew.Columns.Add("Tdate0", System.Type.GetType("System.String"));
            //dtnew.Columns.Add("Type", System.Type.GetType("System.String"));
            //dtnew.Columns.Add("Department1", System.Type.GetType("System.String"));
            //for (int i = 0; i < dtdetail.Rows.Count; i++)
            //{
            //    DataRow dr = dtnew.NewRow();
            //    dr["DetailID"] = dtdetail.Rows[i]["ID"].ToString();
            //    dr["Tocity"] = dtdetail.Rows[i]["Tocity"].ToString();
            //    dr["AccountCode"] = dtdetail.Rows[i]["AccountCode"].ToString();
            //    dr["Cur"] = dtdetail.Rows[i]["Cur"].ToString();
            //    dr["Pamount"] = dtdetail.Rows[i]["Pamount"].ToString();
            //    dr["Camount"] = dtdetail.Rows[i]["Camount"].ToString();
            //    dr["TSation"] = dtdetail.Rows[i]["TSation"].ToString();
            //    dr["Tdate"] = dtdetail.Rows[i]["Tdate"].ToString() == "" ? "" : Convert.ToDateTime(dtdetail.Rows[i]["Tdate"].ToString()).ToString("yyyy/MM/dd");
            //    dr["Tdate0"] = dtdetail.Rows[i]["Tdate0"].ToString() == "" ? "" : Convert.ToDateTime(dtdetail.Rows[i]["Tdate0"].ToString()).ToString("yyyy/MM/dd");
            //    dr["Type"] = hdStatus.Value.ToString();//传递给子页面以判断按钮状态 0:允许修改,允许上传;1:不允许修改,允许上传;2:全不允许
            //    dr["Department1"] = dtdetail.Rows[i]["Department1"].ToString();
            //    dtnew.Rows.Add(dr);
            //}
            //Store3.DataSource = dtnew;
            //Store3.DataBind();
            ////txtSum.Text = (psum + csum).ToString("#,##0.00"); hdSum.Value = (psum + csum).ToString();
            ////txtPersonalSum.Text = psum.ToString("#,##0.00"); hdPamountSum.Value = psum.ToString();
            ////txtCompanySum.Text = csum.ToString("#,##0.00"); hdCamountSum.Value = csum.ToString();
            //txtRemark.Text = dt.Rows[0]["Remark"].ToString();
            //if (dt.Rows[0]["CCMailList"].ToString() != "")
            //{
            //    ToolTipCC.Html = dt.Rows[0]["CCMailList"].ToString().Replace(",", "<br />");
            //    DataTable dtCCMailList = new DataTable();
            //    dtCCMailList.Columns.Add("Email", typeof(String));
            //    for (int i = 0; i < dt.Rows[0]["CCMailList"].ToString().Split(',').Length; i++)
            //    {
            //        DataRow dr = dtCCMailList.NewRow();
            //        dr["Email"] = dt.Rows[0]["CCMailList"].ToString().Split(',')[i];
            //        dtCCMailList.Rows.Add(dr);
            //    }
            //    StoreCCList.DataSource = dtCCMailList;
            //    StoreCCList.DataBind();
            //}
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
            if (this.FileUploadField1.HasFile)
            {
                string lstrFileName = null;     //上传文件路径
                string lstrFileFolder = null;   //存放文件路径
                string lstrFileNamePath = null; //上传目录及文件名

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
                FileUploadField1.PostedFile.SaveAs(lstrFileNamePath);       //上传文件到服务器
                string filename = "TR" + dtime + System.IO.Path.GetExtension(lstrFileName);
                linkTravelReport.Text = filename; hdReport.Value = filename;
                linkTravelReport.NavigateUrl = "./Upload/" + filename;
                string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";
                //更新数据
                if (hdTravelRequestID.Value.ToString() != "")
                {
                    string updatesql = "update ETravel set ReportFile='" + filename + "' where ID=" + hdTravelRequestID.Value.ToString();
                    cs.DBCommand dbc = new cs.DBCommand();
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
        protected void UploadScanFileClick(object sender, DirectEventArgs e)
        {
            if (this.FileUploadField2.HasFile)
            {
                string lstrFileName = null;     //上传文件路径
                string lstrFileFolder = null;   //存放文件路径
                string lstrFileNamePath = null; //上传目录及文件名

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
                FileUploadField2.PostedFile.SaveAs(lstrFileNamePath);       //上传文件到服务器
                string filename = "SF" + dtime + System.IO.Path.GetExtension(lstrFileName);
                linkScanFile.Text = filename; hdScanFile.Value = filename;
                linkScanFile.NavigateUrl = "./Upload/" + filename;
                string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";
                //更新数据
                if (hdTravelRequestID.Value.ToString() != "")
                {
                    string updatesql = "update ETravel set Attach='" + filename + "' where ID=" + hdTravelRequestID.Value.ToString();
                    cs.DBCommand dbc = new cs.DBCommand();
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
                    Response.Cookies["eReimUserID"].Expires = DateTime.Now.AddHours(1);//设置Cookie过期时间
                }
                else
                {
                    HttpCookie cookie = new HttpCookie("eReimUserID", dtuser.Rows[0]["UserID"].ToString());
                    cookie.Expires = DateTime.Now.AddHours(1);
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
                        Response.Cookies["eReimUserName"].Expires = DateTime.Now.AddHours(1);//设置Cookie过期时间
                    }
                    else
                    {
                        HttpCookie cookie = new HttpCookie("eReimUserName", dt1.Rows[0]["fullName"].ToString());
                        cookie.Expires = DateTime.Now.AddHours(1);
                        Response.Cookies.Add(cookie);
                    }
                    if (Request.Cookies["eReimStation"] != null)
                    {
                        Response.Cookies["eReimStation"].Value = dt1.Rows[0]["stationCode"].ToString();  //将值写入到客户端硬盘Cookie
                        Response.Cookies["eReimStation"].Expires = DateTime.Now.AddHours(1);//设置Cookie过期时间
                    }
                    else
                    {
                        HttpCookie cookie = new HttpCookie("eReimStation", dt1.Rows[0]["stationCode"].ToString());
                        cookie.Expires = DateTime.Now.AddHours(1);
                        Response.Cookies.Add(cookie);
                    }
                    if (Request.Cookies["eReimDepartment"] != null)
                    {
                        Response.Cookies["eReimDepartment"].Value = dt1.Rows[0]["CRPDepartmentName"].ToString();  //将值写入到客户端硬盘Cookie
                        Response.Cookies["eReimDepartment"].Expires = DateTime.Now.AddHours(1);//设置Cookie过期时间
                    }
                    else
                    {
                        HttpCookie cookie = new HttpCookie("eReimDepartment", dt1.Rows[0]["CRPDepartmentName"].ToString());
                        cookie.Expires = DateTime.Now.AddHours(1);
                        Response.Cookies.Add(cookie);
                    }
                    if (Request.Cookies["eReimCostCenter"] != null)
                    {
                        Response.Cookies["eReimCostCenter"].Value = dt1.Rows[0]["CostCenter"].ToString();  //将值写入到客户端硬盘Cookie
                        Response.Cookies["eReimCostCenter"].Expires = DateTime.Now.AddHours(1);//设置Cookie过期时间
                    }
                    else
                    {
                        HttpCookie cookie = new HttpCookie("eReimCostCenter", dt1.Rows[0]["CostCenter"].ToString());
                        cookie.Expires = DateTime.Now.AddHours(1);
                        Response.Cookies.Add(cookie);
                    }

                    DataTable dttemp = new DataTable();
                    string sqltemp = "select * from ESUSER where Userid='" + dtuser.Rows[0]["UserID"].ToString() + "'";
                    dttemp = dbc.GetData("eReimbursement", sqltemp);
                    if (dttemp.Rows.Count > 0)
                    {
                        //Session["CostCenter"] = dttemp.Rows[0]["Station"].ToString();
                        if (Request.Cookies["eReimCostCenter"] != null)
                        {
                            Response.Cookies["eReimCostCenter"].Value = dttemp.Rows[0][3].ToString();  //将值写入到客户端硬盘Cookie
                            Response.Cookies["eReimCostCenter"].Expires = DateTime.Now.AddHours(1);//设置Cookie过期时间
                        }
                        else
                        {
                            HttpCookie cookie = new HttpCookie("eReimCostCenter", dttemp.Rows[0]["Station"].ToString());
                            cookie.Expires = DateTime.Now.AddHours(1);
                            Response.Cookies.Add(cookie);
                        }
                    }
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
                        dr["leaveCount"] = ts.Days.ToString() + " Days " + ts.Hours.ToString() + " Hours";
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

            string sqlitem = "select * from Eagent where [St]=1 and [PAgentID]='" + Request.Cookies.Get("eReimUserID").Value + "'";
            DataTable dtitem = dbc.GetData("eReimbursement", sqlitem);
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
                    //li = new Ext.Net.ListItem(dtitem.Rows[m][1].ToString(), dtitem.Rows[m][2].ToString());
                    //cbxOwner.Items.Add(li);
                    DataSet dsAgent = DIMERCO.SDK.Utilities.LSDK.GetBizTripByUserID(dtitem.Rows[m][2].ToString());
                    if (dsAgent != null && dsAgent.Tables.Count == 1)
                    {
                        DataTable dteLeaveAgent = dsAgent.Tables[0];
                        for (int i = 0; i < dteLeaveAgent.Rows.Count; i++)
                        {
                            DataRow dr = dtnew.NewRow();
                            dr["Owner"] = dtitem.Rows[m][1].ToString();
                            dr["OwnerID"] = dtitem.Rows[m][2].ToString();
                            DataSet dsuserinfo = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtitem.Rows[m][2].ToString());
                            DataTable dtuserinfo = dsuserinfo.Tables[0];
                            dr["Station"] = dtuserinfo.Rows[0]["stationCode"].ToString();
                            dr["Department"] = dtuserinfo.Rows[0]["CRPDepartmentName"].ToString();
                            //获取该人币种
                            dr["Currency"] = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dtuserinfo.Rows[0]["stationCode"].ToString());

                            DataTable dttemp = new DataTable();
                            string sqltemp = "select * from ESUSER where Userid='" + dtitem.Rows[m][2].ToString() + "'";
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
            }

            Store1.DataSource = dtnew;
            Store1.DataBind();
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
                sb.Append("<th " + tdstyle + ">Employee Pay</th>");
                sb.Append("<th " + tdstyle + ">Company Pay</th>");
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
                colP.Header = "Person Pay";
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
                colC.Header = "Company Pay";
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
            colPNew.Header = "Person Pay";
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
            colCNew.Header = "Company Pay";
            colCNew.DataIndex = fieldCNameNew;
            colCNew.Sortable = false;
            colCNew.Resizable = false;
            colCNew.MenuDisabled = true;
            colCNew.Width = 110;
            colCNew.Editor.Add(txtCNew);
            this.GridPanel2.ColumnModel.Columns.Add(colCNew);

            var TotalP = new Ext.Net.TextField();
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

            var TotalC = new Ext.Net.TextField();
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
            TitleTotalP.Text = "Total(Person)";
            TitleTotalP.Cls = "custom-row";
            HeaderColumn hcTitleTotalP = new HeaderColumn();
            hcTitleTotalP.Component.Add(TitleTotalP);
            this.GridPanel2.GetView().HeaderRows[2].Columns.Add(hcTitleTotalP);

            var TitleTotalC = new Ext.Net.Label();
            TitleTotalC.Text = "Total(Comp)";
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
        public void SaveAll1(string type, string detail, string MailList, string header0string, string header1string, string header2string, string Cur, string dept,string warningmsg)
        {
            DateTime dtnull = new DateTime(1, 1, 1, 0, 0, 0);
            cs.DBCommand dbc = new cs.DBCommand();
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

            //处理抄送人列表
            string CCMailList = "";
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<CCMailList> CCMailList1 = ser.Deserialize<List<CCMailList>>(MailList);
            foreach (CCMailList mail in CCMailList1)
            {
                CCMailList += mail.Email + ",";
            }
            CCMailList = CCMailList.Length > 0 ? CCMailList.Substring(0, CCMailList.Length - 1) : "";

            string userid = hdOwnerID.Value.ToString();
            string ostation = ""; string station = ""; string department = "";
            DataSet ds2 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(userid);
            if (ds2.Tables[0].Rows.Count == 1)
            {
                DataTable dt1 = ds2.Tables[0];
                //dpt = dt1.Rows[0]["DepartmentName"].ToString();
                ostation = dt1.Rows[0]["stationCode"].ToString();
                station = dt1.Rows[0]["stationCode"].ToString();
                department = dt1.Rows[0]["CRPDepartmentName"].ToString();
                DataTable dttemp = new DataTable();
                string sqltemp = "select * from ESUSER where Userid='" + userid + "'";
                dttemp = dbc.GetData("eReimbursement", sqltemp);
                if (dttemp.Rows.Count > 0)
                {
                    ostation = dttemp.Rows[0]["Station"].ToString();
                }
            }
            string para = type;
            if (para == "ND")//保存并申请
            {
                if (hdTravelRequestID.Value.ToString() == "")//直接新增申请,不通过草稿
                {
                    string word = "[No],[Person],[Station],[Department],[ReportFile],[Tamount],[Pamout],[Camount],[CreadedBy],[CreadedDate],[Attach],[Remark],[Bdate],[Edate],[PersonID],[CreadedByID],[ApplyDate],[CCMailList],[Budget],[Station2]";
                    string value = "";
                    value += "'" + station + DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString().PadLeft(2, '0') + "',";//edit
                    value += "'" + hdOwner.Value.ToString() + "',"; value += "'" + station + "',"; value += "'" + department + "',";//edit
                    value += "'" + hdReport.Value.ToString() + "',";
                    value += hdSum.Value.ToString() == "" ? "null" : hdSum.Value.ToString();
                    value += "," + (hdPamountSum.Value.ToString() == "" ? "null" : hdPamountSum.Value.ToString());
                    value += "," + (hdCamountSum.Value.ToString() == "" ? "null" : hdCamountSum.Value.ToString());
                    value += ",'" + Request.Cookies.Get("eReimUserName").Value + "'";//edit
                    value += ",'" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "',";
                    value += "'" + hdScanFile.Value.ToString() + "',";
                    value += "'" + txtRemark.Text.Replace("'", "''") + "',";
                    value += "null,null";
                    value += ",'" + hdOwnerID.Value.ToString() + "'";
                    value += ",'" + Request.Cookies.Get("eReimUserID").Value + "'";
                    value += ",'" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "'";
                    value += ",'" + CCMailList + "'";
                    value += "," + (Radio1.Checked ? "1" : "0");
                    //value += "," + (cbxBudget.Value.ToString() == "YES" ? "1" : "0");
                    value += ",'" + ostation + "'";
                    string updatesql = "insert into ETravel (" + word + ") values(" + value + ");update ETravel set [No]=[No]+(select [MonthCount]=right('0000'+cast(count(ID) as varchar(10)),4) from ETravel where (month(ApplyDate) in (select month(ApplyDate) from ETravel where [ID]=@@IDENTITY) and (year(ApplyDate) in (select year(ApplyDate) from ETravel where [ID]=@@IDENTITY)) and Station=(select Station from ETravel where ID=@@IDENTITY)))+'T' where ID=@@IDENTITY;select [msg]=convert(varchar,ID)+','+[No] from ETravel where ID=@@IDENTITY";

                    string newid = dbc.UpdateData("eReimbursement", updatesql, "Insert");

                    string rows = "";
                    for (int i = 0; i < dtGroupFlowData.Rows.Count; i++)
                    {
                        string wordflow = "[No],[Type],[Station],[Department],[Person],[CreadedBy],[CreatedDate],[Tamount],[Step],[Status],[Approver],[ApproverID],[RequestID],[FlowFn]";
                        if (dtGroupFlowData.Rows[i]["FlowNo"].ToString() == "1")
                        {
                            wordflow += ",[Active]";
                        }
                        string valueflow = "";
                        valueflow += "'" + newid.Split(',')[1] + "',";
                        valueflow += "'T',";
                        valueflow += "'" + station + "',";
                        valueflow += "'" + department + "',";
                        valueflow += "'" + hdOwner.Value.ToString() + "',";
                        valueflow += "'" + Request.Cookies.Get("eReimUserName").Value + "',";
                        valueflow += "'" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "',";
                        valueflow += hdSum.Value.ToString() == "" ? "null," : hdSum.Value.ToString() + ",";
                        valueflow += dtGroupFlowData.Rows[i]["FlowNo"].ToString() + ",";
                        valueflow += "1,";
                        valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUser"].ToString() + "',";
                        valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUserid"].ToString() + "',";
                        valueflow += "'" + newid.Split(',')[0] + "'";
                        valueflow += ",'" + (dtGroupFlowData.Rows[i]["Fn"].ToString() == "" ? "Approver" : dtGroupFlowData.Rows[i]["Fn"].ToString()) + "'";
                        if (dtGroupFlowData.Rows[i]["FlowNo"].ToString() == "1")
                        {
                            valueflow += ",1";
                        }
                        string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";
                        rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
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

                        if (!SaveDetail(detail, header0string, header1string, header2string, Cur, dept) || !SendMail(warningmsg))
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
                    updatesql += "',[Station]='" + station;
                    updatesql += "',[Department]='" + department;
                    updatesql += "',[ReportFile]='" + hdReport.Value.ToString();
                    updatesql += "',[Tamount]=" + (hdSum.Value.ToString() == "" ? "null" : hdSum.Value.ToString());
                    updatesql += ",[Pamout]=" + (hdPamountSum.Value.ToString() == "" ? "null" : hdPamountSum.Value.ToString());
                    updatesql += ",[Camount]=" + (hdCamountSum.Value.ToString() == "" ? "null" : hdCamountSum.Value.ToString());
                    updatesql += ",[Remark]='" + txtRemark.Text.Replace("'", "''") + "'";
                    updatesql += ",[Attach]='" + hdScanFile.Value.ToString() + "'";
                    updatesql += ",[Type]=0";
                    updatesql += ",[PersonID]='" + hdOwnerID.Value.ToString() + "'";
                    string oldno = hdTravelRequestNo.Value.ToString();
                    string newno = hdTravelRequestNo.Value.ToString().Substring(0, hdTravelRequestNo.Value.ToString().Length - 1);
                    updatesql += ",[No]='" + newno + "',";
                    updatesql += "[CreadedDate]='" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "',";
                    updatesql += "[CreadedBy]='" + Request.Cookies.Get("eReimUserName").Value + "',";
                    updatesql += "[CreadedByID]='" + Request.Cookies.Get("eReimUserID").Value + "'";
                    updatesql += ",[CCMailList]='" + CCMailList + "'";
                    updatesql += ",[Budget]=" + (Radio1.Checked ? "1" : "0");
                    //updatesql += ",[Budget]=" + (cbxBudget.Value.ToString() == "YES" ? "1" : "0");
                    updatesql += ",[Station2]='" + ostation + "'";
                    updatesql += " where ID=" + hdTravelRequestID.Value.ToString();

                    string newid = dbc.UpdateData("eReimbursement", updatesql, "Update");

                    //操作Flow表
                    string sqlDeleteEflow = "delete from Eflow where [Type]='T' and [RequestID]='" + hdTravelRequestID.Value.ToString() + "'";
                    string deleterows = dbc.UpdateData("eReimbursement", sqlDeleteEflow, "Update");
                    string rows = "";
                    for (int i = 0; i < dtGroupFlowData.Rows.Count; i++)
                    {
                        string wordflow = "[No],[Type],[Station],[Department],[Person],[CreadedBy],[CreatedDate],[Tamount],[Step],[Status],[Approver],[ApproverID],[RequestID],[FlowFn]";
                        if (dtGroupFlowData.Rows[i]["FlowNo"].ToString() == "1")
                        {
                            wordflow += ",[Active]";
                        }
                        string valueflow = "";
                        valueflow += "'" + newno + "',";
                        valueflow += "'T',";
                        valueflow += "'" + station + "',";
                        valueflow += "'" + department + "',";
                        valueflow += "'" + hdOwner.Value.ToString() + "',";
                        valueflow += "'" + Request.Cookies.Get("eReimUserName").Value + "',";
                        valueflow += "'" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "',";
                        valueflow += hdSum.Value.ToString() == "" ? "null," : hdSum.Value.ToString() + ",";
                        valueflow += dtGroupFlowData.Rows[i]["FlowNo"].ToString() + ",";
                        valueflow += "1,";
                        valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUser"].ToString() + "',";
                        valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUserid"].ToString() + "',";
                        valueflow += hdTravelRequestID.Value.ToString();
                        valueflow += ",'" + (dtGroupFlowData.Rows[i]["Fn"].ToString() == "" ? "Approver" : dtGroupFlowData.Rows[i]["Fn"].ToString()) + "'";
                        if (dtGroupFlowData.Rows[i]["FlowNo"].ToString() == "1")
                        {
                            valueflow += ",1";
                        }
                        string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";
                        rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
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

                        if (!SaveDetail(detail, header0string, header1string, header2string, Cur, dept) || !SendMail(warningmsg))
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
                X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();btnSaveDraft.disable();btnSaveAndSend.disable();Radio1.disable();Radio2.disable();btnExport.enable();btnCC.disable();");
                //X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();btnSaveDraft.disable();btnSaveAndSend.disable();cbxBudget.setReadOnly(true);btnExport.enable();btnCC.disable();");
            }
            else//保存草稿
            {
                if (hdTravelRequestID.Value.ToString() != "")//由链接进入的草稿更新
                {
                    string updatesql = "update ETravel set [Person]='" + hdOwner.Value.ToString();
                    updatesql += "',[Station]='" + station;
                    updatesql += "',[Department]='" + department;
                    updatesql += "',[ReportFile]='" + hdReport.Value.ToString();
                    updatesql += "',[Tamount]=" + (hdSum.Value.ToString() == "" ? "null" : hdSum.Value.ToString());
                    updatesql += ",[Pamout]=" + (hdPamountSum.Value.ToString() == "" ? "null" : hdPamountSum.Value.ToString());
                    updatesql += ",[Camount]=" + (hdCamountSum.Value.ToString() == "" ? "null" : hdCamountSum.Value.ToString());
                    updatesql += ",[Remark]='" + txtRemark.Text.Replace("'", "''") + "'";
                    updatesql += ",[Attach]='" + hdScanFile.Value.ToString() + "'";
                    updatesql += ",[PersonID]='" + hdOwnerID.Value.ToString() + "'";
                    updatesql += ",[CCMailList]='" + CCMailList + "'";
                    updatesql += ",[Budget]=" + (Radio1.Checked ? "1" : "0");
                    //updatesql += ",[Budget]=" + (cbxBudget.Value.ToString() == "YES" ? "1" : "0");
                    updatesql += ",[Station2]='" + ostation + "'";
                    updatesql += " where ID=" + hdTravelRequestID.Value.ToString();

                    string newid = dbc.UpdateData("eReimbursement", updatesql, "Update");

                    if (newid == "-1")
                    {
                        ErrorHandle("Data Error.");
                        return;
                    }
                    else
                    {
                        if (!SaveDetail(detail, header0string, header1string, header2string, Cur, dept))
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
                    string word = "[No],[Person],[Station],[Department],[ReportFile],[Tamount],[Pamout],[Camount],[Attach],[Remark],[Bdate],[Edate],[Type],[PersonID],[ApplyDate],[CCMailList],[Station2],[Budget]";
                    string value = "";
                    value += "'" + station + DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString().PadLeft(2, '0') + "',";//edit
                    value += "'" + hdOwner.Value.ToString() + "',"; value += "'" + station + "',"; value += "'" + department + "',";//edit
                    value += "'" + hdReport.Value.ToString() + "',";
                    value += (hdSum.Value.ToString() == "" ? "null" : hdSum.Value.ToString());
                    value += "," + (hdPamountSum.Value.ToString() == "" ? "null" : hdPamountSum.Value.ToString());
                    value += "," + (hdCamountSum.Value.ToString() == "" ? "null" : hdCamountSum.Value.ToString());
                    value += ",'" + hdScanFile.Value.ToString() + "',";
                    value += "'" + txtRemark.Text.Replace("'", "''") + "',";
                    value += "null,null,";
                    value += "1";//标识为草稿
                    value += ",'" + hdOwnerID.Value.ToString() + "'";
                    value += ",'" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "'";
                    value += ",'" + CCMailList + "'";
                    value += ",'" + ostation + "'";
                    value += "," + (Radio1.Checked ? "1" : "0");
                    //value += "," + (cbxBudget.Value.ToString() == "YES" ? "1" : "0");
                    string updatesql = "insert into ETravel (" + word + ") values(" + value + ");update ETravel set [No]=[No]+(select [MonthCount]=right('0000'+cast(count(ID) as varchar(10)),4) from ETravel where (month(ApplyDate) in (select month(ApplyDate) from ETravel where [ID]=@@IDENTITY) and (year(ApplyDate) in (select year(ApplyDate) from ETravel where [ID]=@@IDENTITY)) and Station=(select Station from ETravel where ID=@@IDENTITY)))+'TD' where ID=@@IDENTITY;select [msg]=convert(varchar,ID)+','+[No] from ETravel where ID=@@IDENTITY";

                    string newid = dbc.UpdateData("eReimbursement", updatesql, "Insert");

                    //操作Flow表
                    string wordflow = "[No],[Type],[Station],[Department],[Person],[CreadedBy],[CreatedDate],[Tamount],[RequestID],[Active]";
                    string valueflow = "";
                    valueflow += "'" + newid.Split(',')[1] + "',";
                    valueflow += "'T',";
                    valueflow += "'" + station + "',";
                    valueflow += "'" + department + "',";
                    valueflow += "'" + hdOwner.Value.ToString() + "',";
                    valueflow += "'" + Request.Cookies.Get("eReimUserName").Value + "',";
                    valueflow += "'" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "',";
                    valueflow += hdSum.Value.ToString() == "" ? "null," : hdSum.Value.ToString() + ",";
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
                        if (!SaveDetail(detail, header0string, header1string, header2string, Cur, dept))
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
        [DirectMethod]
        public void SaveAll(string type, string detail, string MailList, string header0string, string header1string, string header2string, string Cur, string dept)
        {
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
            
            DateTime dtnull = new DateTime(1, 1, 1, 0, 0, 0);
            cs.DBCommand dbc = new cs.DBCommand();
            ////检查是否已经为该申请人设置过审批人
            string sqlCheckFlow = "";
            //if (Radio1.Checked)
            ////if (cbxBudget.Value.ToString() == "YES")//使用Budget审批流程
            //{
            sqlCheckFlow = "select * from GroupFlow where [Type]!=2 and GID=(select GID from GroupUsers where UserID='" + hdOwnerID.Value.ToString() + "')";
            //}
            //else//使用unBudget审批流程
            //{
            //    sqlCheckFlow = "select * from GroupFlow where [Type]=2 and GID=(select GID from GroupUsers where UserID='" + hdOwnerID.Value.ToString() + "')";
            //}
            DataTable dtGroupFlowData = dbc.GetData("eReimbursement", sqlCheckFlow);
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

            //处理抄送人列表
            string CCMailList = "";
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<CCMailList> CCMailList1 = ser.Deserialize<List<CCMailList>>(MailList);
            foreach (CCMailList mail in CCMailList1)
            {
                CCMailList += mail.Email + ",";
            }
            CCMailList = CCMailList.Length > 0 ? CCMailList.Substring(0, CCMailList.Length - 1) : "";

            string userid = hdOwnerID.Value.ToString();
            string ostation = ""; string station = ""; string department = "";
            DataSet ds2 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(userid);
            if (ds2.Tables[0].Rows.Count == 1)
            {
                DataTable dt1 = ds2.Tables[0];
                //dpt = dt1.Rows[0]["DepartmentName"].ToString();
                ostation = dt1.Rows[0]["CostCenter"].ToString();
                station = dt1.Rows[0]["stationCode"].ToString();
                department = dt1.Rows[0]["CRPDepartmentName"].ToString();
                //DataTable dttemp = new DataTable();
                //string sqltemp = "select * from ESUSER where Userid='" + userid + "'";
                //dttemp = dbc.GetData("eReimbursement", sqltemp);
                //if (dttemp.Rows.Count > 0)
                //{
                //    ostation = dttemp.Rows[0]["Station"].ToString();
                //}
            }
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
                DataTable dtbudget = new DataTable();
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
                //string tstation = "", year = "", month = "", coacode = "";
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
                    }
                }
                
                //合计
                DataTable dtB = new DataTable();
                dtB.Columns.Add("COACode", typeof(System.String));
                dtB.Columns.Add("Amount", typeof(System.Decimal));
                if (dtA.Compute("Sum(Amount)", "COACode = 62010900").ToString() != "" && Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62010900").ToString()) != 0)
                {
                    DataRow dr = dtB.NewRow();
                    dr["COACode"] = "62010900";
                    dr["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62010900").ToString());
                    dtB.Rows.Add(dr);
                }
                if (dtA.Compute("Sum(Amount)", "COACode = 62011900").ToString() != "" && Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62011900").ToString()) != 0)
                {
                    DataRow dr = dtB.NewRow();
                    dr["COACode"] = "62011900";
                    dr["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62011900").ToString());
                    dtB.Rows.Add(dr);
                }
                if (dtA.Compute("Sum(Amount)", "COACode = 62010500").ToString() != "" && Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62010500").ToString()) != 0)
                {
                    DataRow dr = dtB.NewRow();
                    dr["COACode"] = "62010500";
                    dr["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62010500").ToString());
                    dtB.Rows.Add(dr);
                }
                if (dtA.Compute("Sum(Amount)", "COACode = 62012000").ToString() != "" && Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62012000").ToString()) != 0)
                {
                    DataRow dr = dtB.NewRow();
                    dr["COACode"] = "62012000";
                    dr["Amount"] = Convert.ToDecimal(dtA.Compute("Sum(Amount)", "COACode = 62012000").ToString());
                    dtB.Rows.Add(dr);
                }

                ////取得传递预算的参数
                //string userid = dt.Rows[0]["PersonID"].ToString();
                //string dpt = dt.Rows[0]["Department"].ToString();
                //string ostation = dt.Rows[0]["CostCenter"].ToString();//预算站点,与基本信息中的CostCenter一致(Station2)
                string tstation = ostation;//Etravel表中的Station2,目前与预算站点一致,不允许更改
                string year = Convert.ToDateTime(dtA.Compute("Min(Date)", "").ToString()).Year.ToString();
                string month = Convert.ToDateTime(dtA.Compute("Min(Date)", "").ToString()).Month.ToString();
                //string accountcode = "";
                //for (int g = 0; g < dtB.Rows.Count; g++)
                //{
                //    if (Convert.ToDecimal(dtB.Rows[g]["Amount"].ToString()) != 0)
                //    {
                //        DataRow dr = dtbudget.NewRow();
                //        dr["Current"] = Convert.ToDecimal(dtB.Rows[g]["Amount"].ToString());
                //        dr["Type"] = dtB.Rows[g]["Type"].ToString();
                //        accountcode = dtB.Rows[g]["Type"].ToString();
                //        DataTable dtC = new DataTable();
                //        dtC = Comm.RtnEB(userid, dpt, ostation, tstation, accountcode, year, month);
                //        for (int i = 0; i < dtC.Rows.Count; i++)
                //        {
                //            if (dtC.Rows[i]["Type"].ToString() == "全年个人")
                //            {
                //                dr["PU"] = Convert.ToDecimal(dtC.Rows[i]["Used"].ToString());
                //                dr["PB"] = Convert.ToDecimal(dtC.Rows[i]["Budget"].ToString());
                //            }
                //            else if (dtC.Rows[i]["Type"].ToString() == "全年部门")
                //            {
                //                dr["DU"] = Convert.ToDecimal(dtC.Rows[i]["Used"].ToString());
                //                dr["DB"] = Convert.ToDecimal(dtC.Rows[i]["Budget"].ToString());
                //            }
                //            else if (dtC.Rows[i]["Type"].ToString() == "全年站点")
                //            {
                //                dr["SU"] = Convert.ToDecimal(dtC.Rows[i]["Used"].ToString());
                //                dr["SB"] = Convert.ToDecimal(dtC.Rows[i]["Budget"].ToString());
                //            }
                //        }
                //        dtbudget.Rows.Add(dr);
                //    }
                //}
                ////计算%,取得名称
                //for (int i = 0; i < dtbudget.Rows.Count; i++)
                //{
                //    if (Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()) != 0)//如果Budget不为0,则计算%
                //    {
                //        dtbudget.Rows[i]["PPercent"] = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["PU"].ToString()) / Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()), 4) * 100;
                //    }
                //    if (Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()) != 0)//如果Budget不为0,则计算%
                //    {
                //        dtbudget.Rows[i]["DPercent"] = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["DU"].ToString()) / Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()), 4) * 100;
                //    }
                //    if (Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()) != 0)//如果Budget不为0,则计算%
                //    {
                //        dtbudget.Rows[i]["SPercent"] = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["SU"].ToString()) / Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()), 4) * 100;
                //    }
                //    if (dtbudget.Rows[i]["Type"].ToString() == "62010900")
                //    {
                //        dtbudget.Rows[i]["EName"] = "Travel expense";
                //    }
                //    else if (dtbudget.Rows[i]["Type"].ToString() == "62011900")
                //    {
                //        dtbudget.Rows[i]["EName"] = "Entertainment";
                //    }
                //    else if (dtbudget.Rows[i]["Type"].ToString() == "62010500")
                //    {
                //        dtbudget.Rows[i]["EName"] = "Transportation";
                //    }
                //    else if (dtbudget.Rows[i]["Type"].ToString() == "62012000")
                //    {
                //        dtbudget.Rows[i]["EName"] = "Communication";
                //    }
                //}

                //bool PB = false, DB = false, SB = false;
                //for (int i = 0; i < dtbudget.Rows.Count; i++)
                //{
                //    if (Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()) != 0)//是否显示个人预算列
                //    {
                //        PB = true;
                //    }
                //    if (Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()) != 0)//是否显示部门预算列
                //    {
                //        DB = true;
                //    }
                //    if (Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()) != 0)//是否显示站点预算列
                //    {
                //        SB = true;
                //    }
                //}
                ////添加数据列
                //var cm = GridPanelBudget.ColumnModel;
                //cm.Columns.Add(new Column
                //{
                //    DataIndex = "EName",
                //    Header = "Expense Item",
                //    Sortable = false,
                //    Resizable = false,
                //    MenuDisabled = true,
                //    Width = 100
                //});
                //cm.Columns.Add(new Column
                //{
                //    DataIndex = "Current",
                //    Header = "Current",
                //    Renderer = new Renderer { Fn = "GetNumber" },
                //    Sortable = false,
                //    Resizable = false,
                //    MenuDisabled = true,
                //    Width = 100
                //});
                ////显示个人预算部分
                //if (PB)
                //{
                //    cm.Columns.Add(new Column
                //    {
                //        DataIndex = "PU",
                //        Header = "Personal Used",
                //        Renderer = new Renderer { Fn = "GetNumber" },
                //        Sortable = false,
                //        Resizable = false,
                //        MenuDisabled = true,
                //        Width = 100
                //    });
                //    cm.Columns.Add(new Column
                //    {
                //        DataIndex = "PB",
                //        Header = "Personal Budget",
                //        Renderer = new Renderer { Fn = "GetNumber" },
                //        Sortable = false,
                //        Resizable = false,
                //        MenuDisabled = true,
                //        Width = 100
                //    });
                //    cm.Columns.Add(new Column
                //    {
                //        DataIndex = "PPercent",
                //        Header = "%(Used/Budget)",
                //        Renderer = new Renderer { Fn = "GetNumberPercent" },
                //        Sortable = false,
                //        Resizable = false,
                //        MenuDisabled = true,
                //        Width = 100
                //    });
                //}
                //if (DB)
                //{
                //    cm.Columns.Add(new Column
                //    {
                //        DataIndex = "DU",
                //        Header = "Department Used",
                //        Renderer = new Renderer { Fn = "GetNumber" },
                //        Sortable = false,
                //        Resizable = false,
                //        MenuDisabled = true,
                //        Width = 100
                //    });
                //    cm.Columns.Add(new Column
                //    {
                //        DataIndex = "DB",
                //        Header = "Department Budget",
                //        Renderer = new Renderer { Fn = "GetNumber" },
                //        Sortable = false,
                //        Resizable = false,
                //        MenuDisabled = true,
                //        Width = 100
                //    });
                //    cm.Columns.Add(new Column
                //    {
                //        DataIndex = "DPercent",
                //        Header = "%(Used/Budget)",
                //        Renderer = new Renderer { Fn = "GetNumberPercent" },
                //        Sortable = false,
                //        Resizable = false,
                //        MenuDisabled = true,
                //        Width = 100
                //    });
                //}
                //if (SB)
                //{
                //    cm.Columns.Add(new Column
                //    {
                //        DataIndex = "SU",
                //        Header = "Unit Used",
                //        Renderer = new Renderer { Fn = "GetNumber" },
                //        Sortable = false,
                //        Resizable = false,
                //        MenuDisabled = true,
                //        Width = 100
                //    });
                //    cm.Columns.Add(new Column
                //    {
                //        DataIndex = "SB",
                //        Header = "Unit Budget",
                //        Renderer = new Renderer { Fn = "GetNumber" },
                //        Sortable = false,
                //        Resizable = false,
                //        MenuDisabled = true,
                //        Width = 100
                //    });
                //    cm.Columns.Add(new Column
                //    {
                //        DataIndex = "SPercent",
                //        Header = "%(Used/Budget)",
                //        Renderer = new Renderer { Fn = "GetNumberPercent" },
                //        Sortable = false,
                //        Resizable = false,
                //        MenuDisabled = true,
                //        Width = 100
                //    });
                //}
                //StoreBudget.DataSource = dtbudget;
                //StoreBudget.DataBind();
            }
            string srr = "";
            return;
            //判断Budget下是否符合预算要求
            DataTable dtlocal = new DataTable();
            dtlocal.Columns.Add(new DataColumn("Station", typeof(System.String)));
            dtlocal.Columns.Add(new DataColumn("COACode", typeof(System.String)));
            dtlocal.Columns.Add(new DataColumn("Year", typeof(System.Int16)));
            dtlocal.Columns.Add(new DataColumn("Month", typeof(System.Int16)));
            dtlocal.Columns.Add(new DataColumn("Amount", typeof(System.Decimal)));
            if (Radio1.Checked)
            //if (cbxBudget.Value.ToString() == "YES")
            {
                StoreSubmitDataEventArgs eSubmit = new StoreSubmitDataEventArgs(detail, null);
                XmlNode xml = eSubmit.Xml;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml.InnerXml);
                int dtcol = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes.Count;
                //doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).ChildNodes[j].InnerText;
                int dtrow = doc.SelectNodes("records").Item(0).SelectNodes("record").Count;
                int groupcount = (dtcol - 3) / 2;//多少个站点会被更新,如果站点为空则不更新
                //string tstation = "", year = "", month = "", coacode = "";
                for (int i = 0; i < groupcount; i++)
                {
                    if (header0string.Split(',')[i] != "NA")//站点为空则不更新
                    {
                        //处理数据,准备合并后比较预算
                        string costcenter = header1string.Split(',')[i] == "NA" ? ostation : header1string.Split(',')[i];
                        //1. Air Ticket - Int'l
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtlocal.NewRow();
                            drnew["Station"] = costcenter;
                            drnew["Year"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Year.ToString();
                            drnew["Month"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Month.ToString();
                            drnew["COACode"] = "62012000";
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            dtlocal.Rows.Add(drnew);
                        }
                        //Domestic
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtlocal.NewRow();
                            drnew["Station"] = costcenter;
                            drnew["Year"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Year.ToString();
                            drnew["Month"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Month.ToString();
                            drnew["COACode"] = "62012000";
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(1).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            dtlocal.Rows.Add(drnew);
                        }
                        //2. Hotel Bill
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtlocal.NewRow();
                            drnew["Station"] = costcenter;
                            drnew["Year"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Year.ToString();
                            drnew["Month"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Month.ToString();
                            drnew["COACode"] = "62012000";
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(2).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            dtlocal.Rows.Add(drnew);
                        }
                        //3. Meals
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtlocal.NewRow();
                            drnew["Station"] = costcenter;
                            drnew["Year"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Year.ToString();
                            drnew["Month"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Month.ToString();
                            drnew["COACode"] = "62012000";
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(3).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            dtlocal.Rows.Add(drnew);
                        }
                        //4. Entertainment
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtlocal.NewRow();
                            drnew["Station"] = costcenter;
                            drnew["Year"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Year.ToString();
                            drnew["Month"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Month.ToString();
                            drnew["COACode"] = "62010900";
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(4).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            dtlocal.Rows.Add(drnew);
                        }
                        //5. Car Rental/Transportation
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtlocal.NewRow();
                            drnew["Station"] = costcenter;
                            drnew["Year"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Year.ToString();
                            drnew["Month"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Month.ToString();
                            drnew["COACode"] = "62011900";
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(5).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            dtlocal.Rows.Add(drnew);
                        }
                        //6. Communication
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtlocal.NewRow();
                            drnew["Station"] = costcenter;
                            drnew["Year"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Year.ToString();
                            drnew["Month"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Month.ToString();
                            drnew["COACode"] = "62010500";
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(6).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            dtlocal.Rows.Add(drnew);
                        }
                        //7. Local Trip
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtlocal.NewRow();
                            drnew["Station"] = costcenter;
                            drnew["Year"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Year.ToString();
                            drnew["Month"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Month.ToString();
                            drnew["COACode"] = "62012000";
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(7).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            dtlocal.Rows.Add(drnew);
                        }
                        //8. Overseas Trip USD15/day
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtlocal.NewRow();
                            drnew["Station"] = costcenter;
                            drnew["Year"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Year.ToString();
                            drnew["Month"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Month.ToString();
                            drnew["COACode"] = "62012000";
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(8).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            dtlocal.Rows.Add(drnew);
                        }
                        //9. Airport Tax/Travel Insurance
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtlocal.NewRow();
                            drnew["Station"] = costcenter;
                            drnew["Year"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Year.ToString();
                            drnew["Month"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Month.ToString();
                            drnew["COACode"] = "62012000";
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(9).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            dtlocal.Rows.Add(drnew);
                        }
                        //10. Others
                        if ((doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[1 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[1 + i * 2].InnerText) != 0) || (doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[2 + i * 2].InnerText != "" && Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[2 + i * 2].InnerText) != 0))
                        {
                            DataRow drnew = dtlocal.NewRow();
                            drnew["Station"] = costcenter;
                            drnew["Year"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Year.ToString();
                            drnew["Month"] = Convert.ToDateTime(header2string.Split(',')[i * 2 + 1]).Month.ToString();
                            drnew["COACode"] = "62012000";
                            decimal r1 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[1 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[1 + i * 2].InnerText);
                            decimal r2 = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[2 + i * 2].InnerText == "" ? 0 : Convert.ToDecimal(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(10).ChildNodes[2 + i * 2].InnerText);
                            drnew["Amount"] = r1 + r2;
                            dtlocal.Rows.Add(drnew);
                        }
                    }
                }
            }
            decimal sum1 = 0M;
            DataTable dt3 = new DataTable();
            dt3.Columns.Add(new DataColumn("Station",typeof(System.String)));
            dt3.Columns.Add(new DataColumn("Year", typeof(System.Int16)));
            dt3.Columns.Add(new DataColumn("COACode", typeof(System.String)));
            dt3.Columns.Add(new DataColumn("Amount", typeof(System.Decimal)));
            DataTable drnw = new DataTable();
            dtlocal.DefaultView.Sort = "Station ASC,Year ASC,COACode ASC";
            drnw = dtlocal.DefaultView.ToTable();

            for (int i = 0; i < drnw.Rows.Count; i++)
            {
                sum1 += Convert.ToDecimal(drnw.Rows[i]["Amount"].ToString());
                if (i == drnw.Rows.Count - 1)//第一行,也是最后一行
                {
                    DataRow dr3 = dt3.NewRow();
                    dr3["Station"] = drnw.Rows[i]["Station"].ToString();
                    dr3["Year"] = Convert.ToInt16(drnw.Rows[i]["Year"].ToString());
                    dr3["COACode"] = drnw.Rows[i]["COACode"].ToString();
                    dr3["Amount"] = sum1;
                    dt3.Rows.Add(dr3);
                }
                else
                {
                    if ((drnw.Rows[i]["Station"].ToString() != drnw.Rows[i + 1]["Station"].ToString()) || (drnw.Rows[i]["Year"].ToString() != drnw.Rows[i + 1]["Year"].ToString()) || (drnw.Rows[i]["COACode"].ToString() != drnw.Rows[i + 1]["COACode"].ToString()))
                    {
                        //与下一行内容不同,新增一行合计后重置sum1
                        DataRow dr3 = dt3.NewRow();
                        dr3["Station"] = drnw.Rows[i]["Station"].ToString();
                        dr3["Year"] = Convert.ToInt16(drnw.Rows[i]["Year"].ToString());
                        dr3["COACode"] = drnw.Rows[i]["COACode"].ToString();
                        dr3["Amount"] = sum1;
                        dt3.Rows.Add(dr3);
                        sum1 = 0M;
                    }
                }
            }
            string warningmsg = "";
            for (int i = 0; i < dt3.Rows.Count; i++)
            {
                decimal budgetstation = 0M, budgetusedstation = 0M;
                DataTable dt4 = new DataTable();
                dt4 = Comm.RtnEB(userid, department, ostation, dt3.Rows[i]["Station"].ToString(), dt3.Rows[i]["COACode"].ToString(), dt3.Rows[i]["Year"].ToString(), "1");
                for (int j = 0; j < dt4.Rows.Count; j++)
                {
                    if (dt4.Rows[j]["Type"].ToString() == "全年站点")
                    {
                        budgetstation = Convert.ToDecimal(dt4.Rows[j]["Budget"].ToString());
                        budgetusedstation = Convert.ToDecimal(dt4.Rows[j]["Used"].ToString());
                        if (ostation!=dt3.Rows[i]["Station"].ToString())
                        {
                            budgetstation = Convert.ToDecimal(System.Math.Round(Convert.ToDouble(dt4.Rows[j]["Budget"].ToString()) * (DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(dt3.Rows[i]["Station"].ToString()) / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(ostation)), 2));
                            budgetusedstation = Convert.ToDecimal(System.Math.Round(Convert.ToDouble(dt4.Rows[j]["Used"].ToString()) * (DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(dt3.Rows[i]["Station"].ToString()) / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(ostation)), 2));
                        }
                        break;
                    }
                }
                if (dt3.Rows[i]["COACode"].ToString() == "62012000")
                {
                    if (Convert.ToDecimal(dt3.Rows[i]["Amount"].ToString()) + budgetusedstation - budgetstation > Convert.ToDecimal(6.01)*500)
                    {
                        X.AddScript("Ext.Msg.show({ title: 'Message', msg: '差旅费部分超出 " + dt3.Rows[i]["Station"].ToString() + " 全年预算总额500美元以上,仅可按<a style=\"color:Red\">Un-Budget</a>流程申请,是否接受?', buttons: { ok: 'Ok',cancel:'No' }, fn: function (btn) { if(btn=='ok'){Radio2.setValue(true);}else{return false;} } });");
                        //X.AddScript("Ext.Msg.show({ title: 'Message', msg: '差旅费部分超出 " + dt3.Rows[i]["Station"].ToString() + " 全年预算总额500美元以上,仅可按<a style=\"color:Red\">Un-Budget</a>流程申请.', buttons: { ok: 'Ok' }, fn: function (btn) { return false; } });");
                        return;
                    }
                }
                else
                {
                    if (Convert.ToDecimal(dt3.Rows[i]["Amount"].ToString()) + budgetusedstation - budgetstation > Convert.ToDecimal(6.01) * 500)
                    {
                        string rw = "";
                        switch (dt3.Rows[i]["COACode"].ToString())
                        {
                            case "62010900":
                                rw = "交际费";
                                break;
                            case "62011900":
                                rw = "交通费";
                                break;
                            case "62010500":
                                rw = "通讯费";
                                break;
                            default:
                                break;
                        }
                        warningmsg += rw + "部分超出 " + dt3.Rows[i]["Station"].ToString() + " 全年预算总额500美元以上.";
                    }
                }
            }
            
            if (warningmsg.Length>0)
            {
                //X.AddScript("Ext.Msg.show({ title: 'Message', msg: '" + warningmsg + "', buttons: { ok: 'Ok' }, fn: function (btn) { RM.SaveAll1('" + type + "','" + detail + "','" + MailList + "','" + header0string + "','" + header1string + "','" + header2string + "','" + Cur + "','" + dept + "',{eventMask:{showMask:true,target:'Page'}}); } });");
                X.AddScript("Ext.Msg.show({ title: 'Message', msg: '" + warningmsg + "', buttons: { ok: 'Ok' }, fn: function (btn) { SaveAll1('" + type + "','" + warningmsg + "')} });");
            }
            else
            {
                if (para == "ND")//保存并申请
                {
                    if (hdTravelRequestID.Value.ToString() == "")//直接新增申请,不通过草稿
                    {
                        string word = "[No],[Person],[Station],[Department],[ReportFile],[Tamount],[Pamout],[Camount],[CreadedBy],[CreadedDate],[Attach],[Remark],[Bdate],[Edate],[PersonID],[CreadedByID],[ApplyDate],[CCMailList],[Budget],[Station2]";
                        string value = "";
                        value += "'" + station + DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString().PadLeft(2, '0') + "',";//edit
                        value += "'" + hdOwner.Value.ToString() + "',"; value += "'" + station + "',"; value += "'" + department + "',";//edit
                        value += "'" + hdReport.Value.ToString() + "',";
                        value += hdSum.Value.ToString() == "" ? "null" : hdSum.Value.ToString();
                        value += "," + (hdPamountSum.Value.ToString() == "" ? "null" : hdPamountSum.Value.ToString());
                        value += "," + (hdCamountSum.Value.ToString() == "" ? "null" : hdCamountSum.Value.ToString());
                        value += ",'" + Request.Cookies.Get("eReimUserName").Value + "'";//edit
                        value += ",'" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "',";
                        value += "'" + hdScanFile.Value.ToString() + "',";
                        value += "'" + txtRemark.Text.Replace("'", "''") + "',";
                        value += "null,null";
                        value += ",'" + hdOwnerID.Value.ToString() + "'";
                        value += ",'" + Request.Cookies.Get("eReimUserID").Value + "'";
                        value += ",'" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "'";
                        value += ",'" + CCMailList + "'";
                        value += "," + (Radio1.Checked ? "1" : "0");
                        //value += "," + (cbxBudget.Value.ToString() == "YES" ? "1" : "0");
                        value += ",'" + ostation + "'";
                        string updatesql = "insert into ETravel (" + word + ") values(" + value + ");update ETravel set [No]=[No]+(select [MonthCount]=right('0000'+cast(count(ID) as varchar(10)),4) from ETravel where (month(ApplyDate) in (select month(ApplyDate) from ETravel where [ID]=@@IDENTITY) and (year(ApplyDate) in (select year(ApplyDate) from ETravel where [ID]=@@IDENTITY)) and Station=(select Station from ETravel where ID=@@IDENTITY)))+'T' where ID=@@IDENTITY;select [msg]=convert(varchar,ID)+','+[No] from ETravel where ID=@@IDENTITY";

                        string newid = dbc.UpdateData("eReimbursement", updatesql, "Insert");

                        string rows = "";
                        for (int i = 0; i < dtGroupFlowData.Rows.Count; i++)
                        {
                            string wordflow = "[No],[Type],[Station],[Department],[Person],[CreadedBy],[CreatedDate],[Tamount],[Step],[Status],[Approver],[ApproverID],[RequestID],[FlowFn]";
                            if (dtGroupFlowData.Rows[i]["FlowNo"].ToString() == "1")
                            {
                                wordflow += ",[Active]";
                            }
                            string valueflow = "";
                            valueflow += "'" + newid.Split(',')[1] + "',";
                            valueflow += "'T',";
                            valueflow += "'" + station + "',";
                            valueflow += "'" + department + "',";
                            valueflow += "'" + hdOwner.Value.ToString() + "',";
                            valueflow += "'" + Request.Cookies.Get("eReimUserName").Value + "',";
                            valueflow += "'" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "',";
                            valueflow += hdSum.Value.ToString() == "" ? "null," : hdSum.Value.ToString() + ",";
                            valueflow += dtGroupFlowData.Rows[i]["FlowNo"].ToString() + ",";
                            valueflow += "1,";
                            valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUser"].ToString() + "',";
                            valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUserid"].ToString() + "',";
                            valueflow += "'" + newid.Split(',')[0] + "'";
                            valueflow += ",'" + (dtGroupFlowData.Rows[i]["Fn"].ToString() == "" ? "Approver" : dtGroupFlowData.Rows[i]["Fn"].ToString()) + "'";
                            if (dtGroupFlowData.Rows[i]["FlowNo"].ToString() == "1")
                            {
                                valueflow += ",1";
                            }
                            string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";
                            rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
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

                            if (!SaveDetail(detail, header0string, header1string, header2string, Cur, dept) || !SendMail(warningmsg))
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
                        updatesql += "',[Station]='" + station;
                        updatesql += "',[Department]='" + department;
                        updatesql += "',[ReportFile]='" + hdReport.Value.ToString();
                        updatesql += "',[Tamount]=" + (hdSum.Value.ToString() == "" ? "null" : hdSum.Value.ToString());
                        updatesql += ",[Pamout]=" + (hdPamountSum.Value.ToString() == "" ? "null" : hdPamountSum.Value.ToString());
                        updatesql += ",[Camount]=" + (hdCamountSum.Value.ToString() == "" ? "null" : hdCamountSum.Value.ToString());
                        updatesql += ",[Remark]='" + txtRemark.Text.Replace("'", "''") + "'";
                        updatesql += ",[Attach]='" + hdScanFile.Value.ToString() + "'";
                        updatesql += ",[Type]=0";
                        updatesql += ",[PersonID]='" + hdOwnerID.Value.ToString() + "'";
                        string oldno = hdTravelRequestNo.Value.ToString();
                        string newno = hdTravelRequestNo.Value.ToString().Substring(0, hdTravelRequestNo.Value.ToString().Length - 1);
                        updatesql += ",[No]='" + newno + "',";
                        updatesql += "[CreadedDate]='" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "',";
                        updatesql += "[CreadedBy]='" + Request.Cookies.Get("eReimUserName").Value + "',";
                        updatesql += "[CreadedByID]='" + Request.Cookies.Get("eReimUserID").Value + "'";
                        updatesql += ",[CCMailList]='" + CCMailList + "'";
                        updatesql += ",[Budget]=" + (Radio1.Checked ? "1" : "0");
                        //updatesql += ",[Budget]=" + (cbxBudget.Value.ToString() == "YES" ? "1" : "0");
                        updatesql += ",[Station2]='" + ostation + "'";
                        updatesql += " where ID=" + hdTravelRequestID.Value.ToString();

                        string newid = dbc.UpdateData("eReimbursement", updatesql, "Update");

                        //操作Flow表
                        string sqlDeleteEflow = "delete from Eflow where [Type]='T' and [RequestID]='" + hdTravelRequestID.Value.ToString() + "'";
                        string deleterows = dbc.UpdateData("eReimbursement", sqlDeleteEflow, "Update");
                        string rows = "";
                        for (int i = 0; i < dtGroupFlowData.Rows.Count; i++)
                        {
                            string wordflow = "[No],[Type],[Station],[Department],[Person],[CreadedBy],[CreatedDate],[Tamount],[Step],[Status],[Approver],[ApproverID],[RequestID],[FlowFn]";
                            if (dtGroupFlowData.Rows[i]["FlowNo"].ToString() == "1")
                            {
                                wordflow += ",[Active]";
                            }
                            string valueflow = "";
                            valueflow += "'" + newno + "',";
                            valueflow += "'T',";
                            valueflow += "'" + station + "',";
                            valueflow += "'" + department + "',";
                            valueflow += "'" + hdOwner.Value.ToString() + "',";
                            valueflow += "'" + Request.Cookies.Get("eReimUserName").Value + "',";
                            valueflow += "'" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "',";
                            valueflow += hdSum.Value.ToString() == "" ? "null," : hdSum.Value.ToString() + ",";
                            valueflow += dtGroupFlowData.Rows[i]["FlowNo"].ToString() + ",";
                            valueflow += "1,";
                            valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUser"].ToString() + "',";
                            valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUserid"].ToString() + "',";
                            valueflow += hdTravelRequestID.Value.ToString();
                            valueflow += ",'" + (dtGroupFlowData.Rows[i]["Fn"].ToString() == "" ? "Approver" : dtGroupFlowData.Rows[i]["Fn"].ToString()) + "'";
                            if (dtGroupFlowData.Rows[i]["FlowNo"].ToString() == "1")
                            {
                                valueflow += ",1";
                            }
                            string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";
                            rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
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

                            if (!SaveDetail(detail, header0string, header1string, header2string, Cur, dept) || !SendMail(warningmsg))
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
                    X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();btnSaveDraft.disable();btnSaveAndSend.disable();Radio1.disable();Radio2.disable();btnExport.enable();btnCC.disable();");
                    //X.AddScript("btnAddDSTN.disable();btnGeteLeave.disable();btnSaveDraft.disable();btnSaveAndSend.disable();cbxBudget.setReadOnly(true);btnExport.enable();btnCC.disable();");
                }
                else//保存草稿
                {
                    if (hdTravelRequestID.Value.ToString() != "")//由链接进入的草稿更新
                    {
                        string updatesql = "update ETravel set [Person]='" + hdOwner.Value.ToString();
                        updatesql += "',[Station]='" + station;
                        updatesql += "',[Department]='" + department;
                        updatesql += "',[ReportFile]='" + hdReport.Value.ToString();
                        updatesql += "',[Tamount]=" + (hdSum.Value.ToString() == "" ? "null" : hdSum.Value.ToString());
                        updatesql += ",[Pamout]=" + (hdPamountSum.Value.ToString() == "" ? "null" : hdPamountSum.Value.ToString());
                        updatesql += ",[Camount]=" + (hdCamountSum.Value.ToString() == "" ? "null" : hdCamountSum.Value.ToString());
                        updatesql += ",[Remark]='" + txtRemark.Text.Replace("'", "''") + "'";
                        updatesql += ",[Attach]='" + hdScanFile.Value.ToString() + "'";
                        updatesql += ",[PersonID]='" + hdOwnerID.Value.ToString() + "'";
                        updatesql += ",[CCMailList]='" + CCMailList + "'";
                        updatesql += ",[Budget]=" + (Radio1.Checked ? "1" : "0");
                        //updatesql += ",[Budget]=" + (cbxBudget.Value.ToString() == "YES" ? "1" : "0");
                        updatesql += ",[Station2]='" + ostation + "'";
                        updatesql += " where ID=" + hdTravelRequestID.Value.ToString();

                        string newid = dbc.UpdateData("eReimbursement", updatesql, "Update");

                        if (newid == "-1")
                        {
                            ErrorHandle("Data Error.");
                            return;
                        }
                        else
                        {
                            if (!SaveDetail(detail, header0string, header1string, header2string, Cur, dept))
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
                        string word = "[No],[Person],[Station],[Department],[ReportFile],[Tamount],[Pamout],[Camount],[Attach],[Remark],[Bdate],[Edate],[Type],[PersonID],[ApplyDate],[CCMailList],[Station2],[Budget]";
                        string value = "";
                        value += "'" + station + DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString().PadLeft(2, '0') + "',";//edit
                        value += "'" + hdOwner.Value.ToString() + "',"; value += "'" + station + "',"; value += "'" + department + "',";//edit
                        value += "'" + hdReport.Value.ToString() + "',";
                        value += (hdSum.Value.ToString() == "" ? "null" : hdSum.Value.ToString());
                        value += "," + (hdPamountSum.Value.ToString() == "" ? "null" : hdPamountSum.Value.ToString());
                        value += "," + (hdCamountSum.Value.ToString() == "" ? "null" : hdCamountSum.Value.ToString());
                        value += ",'" + hdScanFile.Value.ToString() + "',";
                        value += "'" + txtRemark.Text.Replace("'", "''") + "',";
                        value += "null,null,";
                        value += "1";//标识为草稿
                        value += ",'" + hdOwnerID.Value.ToString() + "'";
                        value += ",'" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "'";
                        value += ",'" + CCMailList + "'";
                        value += ",'" + ostation + "'";
                        value += "," + (Radio1.Checked ? "1" : "0");
                        //value += "," + (cbxBudget.Value.ToString() == "YES" ? "1" : "0");
                        string updatesql = "insert into ETravel (" + word + ") values(" + value + ");update ETravel set [No]=[No]+(select [MonthCount]=right('0000'+cast(count(ID) as varchar(10)),4) from ETravel where (month(ApplyDate) in (select month(ApplyDate) from ETravel where [ID]=@@IDENTITY) and (year(ApplyDate) in (select year(ApplyDate) from ETravel where [ID]=@@IDENTITY)) and Station=(select Station from ETravel where ID=@@IDENTITY)))+'TD' where ID=@@IDENTITY;select [msg]=convert(varchar,ID)+','+[No] from ETravel where ID=@@IDENTITY";

                        string newid = dbc.UpdateData("eReimbursement", updatesql, "Insert");

                        //操作Flow表
                        string wordflow = "[No],[Type],[Station],[Department],[Person],[CreadedBy],[CreatedDate],[Tamount],[RequestID],[Active]";
                        string valueflow = "";
                        valueflow += "'" + newid.Split(',')[1] + "',";
                        valueflow += "'T',";
                        valueflow += "'" + station + "',";
                        valueflow += "'" + department + "',";
                        valueflow += "'" + hdOwner.Value.ToString() + "',";
                        valueflow += "'" + Request.Cookies.Get("eReimUserName").Value + "',";
                        valueflow += "'" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "',";
                        valueflow += hdSum.Value.ToString() == "" ? "null," : hdSum.Value.ToString() + ",";
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
                            if (!SaveDetail(detail, header0string, header1string, header2string, Cur, dept))
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
        }
        protected bool SaveDetail(string detail, string header0string, string header1string, string header2string, string Cur, string dept)
        {
            cs.DBCommand dbc = new cs.DBCommand();
            //删除现有数据
            string deletesql = "delete from ETraveleDetail where [No]='" + hdTravelRequestID.Value.ToString() + "'";
            string newid1 = dbc.UpdateData("eReimbursement", deletesql, "Update");

            SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eReimbursement"].ConnectionString);
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
                    DataSet ds2 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(hdOwnerID.Value.ToString());
                    string station = "",Tstation="";
                    if (ds2.Tables[0].Rows.Count == 1)
                    {
                        DataTable dt1 = ds2.Tables[0];
                        station = dt1.Rows[0]["stationCode"].ToString();
                        DataTable dttemp = new DataTable();
                        string sqltemp = "select * from ESUSER where Userid='" + hdOwnerID.Value.ToString() + "'";
                        dttemp = dbc.GetData("eReimbursement", sqltemp);
                        if (dttemp.Rows.Count > 0)
                        {
                            station = dttemp.Rows[0]["Station"].ToString();
                        }
                    }
                    Tstation = header1string.Split(',')[i] == "NA" ? station : header1string.Split(',')[i];
                    double re = System.Math.Round(DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(station) / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(Tstation), 4);
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

                            spdetail = new SqlParameter("@Tocity", SqlDbType.VarChar, 10);
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
                                spdetail.Value = Convert.ToDecimal(System.Math.Round(Convert.ToDouble(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(j).ChildNodes[1 + i * 2].InnerText) * re, 2));
                                
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
                                spdetail.Value = Convert.ToDecimal(System.Math.Round(Convert.ToDouble(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(j).ChildNodes[2 + i * 2].InnerText) * re, 2));

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
        public void LoadBudget(string TStation, string row, string date, string Category)
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