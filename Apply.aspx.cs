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
using System.Globalization;
using org.in2bits.MyXls;
using System.Net.Mail;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using System.Threading;
namespace eReimbursement
{
    public partial class Apply : App_Code.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                cs.DBCommand dbc = new cs.DBCommand();
                //判断登录状态
                //string wr = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode("CRP");
                ////string newsql = "select distinct PersonID from Ecommon";
                //string newsql = "select distinct PersonID from ETravel";
                //DataTable newdt = dbc.GetData("eReimbursement", newsql);
                //for (int i = 0; i < newdt.Rows.Count; i++)
                //{
                //    DataSet dstest = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(newdt.Rows[i]["PersonID"].ToString());
                //    if (dstest.Tables[0].Rows.Count == 1)
                //    {
                //        DataTable dtnew = dstest.Tables[0];
                //        string odep = dtnew.Rows[0]["CRPDepartmentName"].ToString();
                //        //string upsqp = "update Ecommon set Department='" + odep + "' where PersonID='" + newdt.Rows[i]["PersonID"].ToString() + "'";
                //        string upsqp = "update ETravel set Department='" + odep + "' where PersonID='" + newdt.Rows[i]["PersonID"].ToString() + "'";
                //        string newid = dbc.UpdateData("eReimbursement", upsqp, "Update");
                //    }
                //}
                //DataSet dsuserinfo = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList("A0971");


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
                }

                
                ////取得对美元汇率
                //DataSet ds2 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(Request.Cookies.Get("eReimUserID").ToString());
                //string station = "";
                //if (ds2.Tables[0].Rows.Count == 1)
                //{
                //    DataTable dt1 = ds2.Tables[0];
                //    station = dt1.Rows[0]["stationCode"].ToString();
                //    DataTable dttemp = new DataTable();
                //    string sqltemp = "select * from ESUSER where Userid='" + Request.Cookies.Get("eReimUserID").ToString() + "'";
                //    dttemp = dbc.GetData("eReimbursement", sqltemp);
                //    if (dttemp.Rows.Count > 0)
                //    {
                //        station = dttemp.Rows[0]["Station"].ToString();
                //    }
                //}
                //hdCurrency.Value = System.Math.Round(1 / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(station), 3);


                hdSubStatus.Value = "0";

                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                {
                    //PagingToolbar1.DisplayMsg = "显示 {0} - {1} of {2}";
                    ResourceManager1.Locale = "zh-CN";
                }
                else
                {
                    //PagingToolbar1.DisplayMsg = "Displaying items {0} - {1} of {2}";
                    ResourceManager1.Locale = "en-US";
                }

                //if (Request.Cookies["lang"] != null)
                //{
                //    string lang = Request.Cookies["lang"].Value;
                //    if (lang.ToLower() == "en-us")
                //    {
                        
                //        PagingToolbar1.DisplayMsg = "Displaying items {0} - {1} of {2}";
                //        ResourceManager1.Locale = "en-US";
                //    }
                //    else
                //    {
                        
                //        PagingToolbar1.DisplayMsg = "显示 {0} - {1} of {2}";
                //        ResourceManager1.Locale = "zh-CN";
                //    }
                //}
                //Panel3.Title = Resources.LocalText.GeneralExpenseApply + "－" + Resources.LocalText.TableNo + ":BJS1001";
                if (Request.QueryString["ID"] != null)
                {
                    string ID = Request.QueryString["ID"].ToString();
                    System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^\d*$");
                    if (reg1.IsMatch(ID))
                    {
                        string sql = "select * from V_Eflow_ETravel where RequestID='" + ID + "' and [Type]='G' and (Active=1 or Active=2)";
                        DataTable dt = new DataTable();
                        dt = dbc.GetData("eReimbursement", sql);
                        if (dt != null && dt.Rows.Count == 1)
                        {
                            if (Request.Cookies.Get("eReimUserID").Value == dt.Rows[0]["PersonID"].ToString())//本人
                            {
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
                                    else//查看已申请数据
                                    {
                                        string app = "";
                                        if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                        {
                                            Panel3.Title = "通用申请单: " + dt.Rows[0]["No"].ToString();
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
                                                hdSubStatus.Value = "2";
                                                app += ". 完成.";
                                            }
                                            else
                                            {
                                                hdSubStatus.Value = "1";
                                            }
                                        }
                                        else
                                        {
                                            Panel3.Title = "General Expense Form: " + dt.Rows[0]["No"].ToString();
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
                                                hdSubStatus.Value = "2";
                                                app += ". Complete.";
                                            }
                                            else
                                            {
                                                hdSubStatus.Value = "1";
                                            }
                                        }

                                        X.AddScript("btnE.disable();btnT.disable();btnC.disable();btnO.disable();btnSaveDraft.disable();btnCC.disable();");
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
                                            Panel3.Title = "通用申请单草稿: " + dt.Rows[0]["No"].ToString();
                                        }
                                        else
                                        {
                                            Panel3.Title = "General Expense Draft: " + dt.Rows[0]["No"].ToString();
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
                                    Ext.Net.ListItem li = new Ext.Net.ListItem(dt.Rows[0]["Person"].ToString(), dt.Rows[0]["PersonID"].ToString());
                                    cbxPerson.Items.Add(li);
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
                                                Panel3.Title = "通用申请单: " + dt.Rows[0]["No"].ToString();
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
                                                    hdSubStatus.Value = "2";
                                                    app += ". 完成.";
                                                }
                                                else
                                                {
                                                    hdSubStatus.Value = "1";
                                                }
                                            }
                                            else
                                            {
                                                Panel3.Title = "General Expense Form: " + dt.Rows[0]["No"].ToString();
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
                                                    hdSubStatus.Value = "2";
                                                    app += ". Complete.";
                                                }
                                                else
                                                {
                                                    hdSubStatus.Value = "1";
                                                }
                                            }

                                            X.AddScript("btnE.disable();btnT.disable();btnC.disable();btnO.disable();btnSaveDraft.disable();btnCC.disable();");
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
                                                Panel3.Title = "通用申请单草稿: " + dt.Rows[0]["No"].ToString();
                                            }
                                            else
                                            {
                                                Panel3.Title = "General Expense Draft: " + dt.Rows[0]["No"].ToString();
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
                                        Ext.Net.ListItem li = new Ext.Net.ListItem(dt.Rows[0]["Person"].ToString(), dt.Rows[0]["PersonID"].ToString());
                                        cbxPerson.Items.Add(li);
                                        //更改按钮状态
                                        if (dt.Rows[0]["Step"].ToString() != "0")//正式申请单
                                        {
                                            string app = "";
                                            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                            {
                                                Panel3.Title = "通用申请单: " + dt.Rows[0]["No"].ToString();
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
                                                    app += ". 完成.";
                                                }
                                            }
                                            else
                                            {
                                                Panel3.Title = "General Expense Form: " + dt.Rows[0]["No"].ToString();
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
                                                    app += ". Complete.";
                                                }
                                            }
                                            labelInfo.Text = app;
                                        }
                                        else//草稿
                                        {
                                            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                            {
                                                Panel3.Title = "通用申请单草稿: " + dt.Rows[0]["No"].ToString();
                                            }
                                            else
                                            {
                                                Panel3.Title = "General Expense Draft: " + dt.Rows[0]["No"].ToString();
                                            }
                                            
                                            //X.AddScript("btnSaveAndSend.enable();");
                                        }
                                        hdSubStatus.Value = "2";//不允许传递到子页面时修改或者上传
                                        X.AddScript("btnE.disable();btnT.disable();btnC.disable();btnO.disable();btnSaveDraft.disable();btnSaveAndSend.disable();btnCC.disable();");
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
                else//本人新增
                {
                    //准备下拉菜单内容
                    Ext.Net.ListItem li = new Ext.Net.ListItem(Request.Cookies.Get("eReimUserName").Value, Request.Cookies.Get("eReimUserID").Value);
                    cbxPerson.Items.Add(li);
                    string sqlitem = "select * from Eagent where [St]=1 and [PAgentID]='" + Request.Cookies.Get("eReimUserID").Value + "'";
                    DataTable dtitem = dbc.GetData("eReimbursement", sqlitem);
                    int itemcount = 0;
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
                    //新增记录时,默认为登录用户
                    cbxPerson.SelectedItem.Value = Request.Cookies.Get("eReimUserID").Value;
                    cbxPerson.SelectedItem.Text = Request.Cookies.Get("eReimUserName").Value;
                    LabelStation.Text = Request.Cookies.Get("eReimStation").Value;
                    LabelDepartment.Text = Request.Cookies.Get("eReimDepartment").Value;

                    LabelMonth.Text = DateTime.Now.Month.ToString();
                    ////币种
                    //hdCur.Value = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(Request.Cookies.Get("eReimStation").Value);
                    //DataTable dttemp = new DataTable();
                    //string sqltemp = "select * from ESUSER where Userid='" + Request.Cookies.Get("eReimUserID").Value + "'";
                    //dttemp = dbc.GetData("eReimbursement", sqltemp);
                    //if (dttemp.Rows.Count > 0)
                    //{
                    //    hdCur.Value = dttemp.Rows[0]["Currency"].ToString();//如果单独设置了币种
                    //}
                    X.AddScript("btnSaveAndSend.enable();cbxPerson.enable();cbxOnBehalfName.enable();");

                    ////检查是否已经为该申请人设置过审批人
                    //string sqlCheckFlow = "select * from GroupFlow where GID=(select GID from GroupUsers where UserID='" + cbxPerson.Text + "')";
                    //DataTable dtCheckFlow = dbc.GetData("eReimbursement", sqlCheckFlow);
                    //if (dtCheckFlow.Rows.Count < 1)
                    //{
                    //    if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                    //    {
                    //        ErrorHandle("请联系Local MIS设置审批人.");
                    //    }
                    //    else
                    //    {
                    //        ErrorHandle("Not set Approve flow,please contact with Local MIS.");
                    //    }
                    //}
                }
                
            }
        }
        protected void LoadData(DataTable dt, bool CheckCopy)
        {
            string ID = dt.Rows[0]["RequestID"].ToString();
            cs.DBCommand dbc = new cs.DBCommand();
            //是否传输附件信息:复制而来不传输
            bool transAttach = true;
            bool copysuc = false;//记录是否成功复制
            cbxPerson.SelectedItem.Value = dt.Rows[0]["PersonID"].ToString();
            //160113 垫付人信息
            cbxOnBehalfName.SelectedItem.Value = dt.Rows[0]["OnBehalfPersonName"].ToString();
            LabelDept.Text = dt.Rows[0]["OnBehalfPersonDept"].ToString();
            LabelUnit.Text = dt.Rows[0]["OnBehalfPersonUnit"].ToString();
            LabelCost.Text = dt.Rows[0]["OnBehalfPersonCostCenter"].ToString();
            hdOnBehalf.Value = dt.Rows[0]["OnBehalfPersonID"].ToString();
            if (CheckCopy)//根据Copy判断是否需要判断Copy状态
            {
                if (Request.QueryString["Copy"] != null)
                {
                    if (Request.QueryString["Copy"].ToString() == "T")//Copy而已,作为新增
                    {
                        copysuc = true;
                        hdTravelRequestID.Value = "";
                        hdTravelRequestNo.Value = "";
                        transAttach = false;
                        LabelMonth.Text = DateTime.Now.Month.ToString();
                        if (Request.QueryString["CopyType"] != null)
                        {
                            if (Request.QueryString["CopyType"].ToString() == "A")
                            {
                                transAttach = true;
                            }
                        }

                        ////检查是否已经为该申请人设置过审批人
                        //string sqlCheckFlow = "";
                        //if (dt.Rows[0]["Budget"].ToString() == "1")//使用Budget审批流程
                        //{
                        //    sqlCheckFlow = "select * from GroupFlow where [Type]!=2 and GID=(select GID from GroupUsers where UserID='" + cbxPerson.Value.ToString() + "')";
                        //}
                        //else//使用unBudget审批流程
                        //{
                        //    sqlCheckFlow = "select * from GroupFlow where [Type]=2 and GID=(select GID from GroupUsers where UserID='" + cbxPerson.Value.ToString() + "')";
                        //}
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
                    X.AddScript("btnExport.enable();");
                    LabelMonth.Text = dt.Rows[0]["ApplyDate"].ToString() == "" ? "" : (Convert.ToDateTime(dt.Rows[0]["ApplyDate"].ToString()).Month.ToString());
                }
            }
            else
            {
                hdTravelRequestID.Value = ID;
                hdTravelRequestNo.Value = dt.Rows[0]["No"].ToString();
                LabelMonth.Text = dt.Rows[0]["ApplyDate"].ToString() == "" ? "" : (Convert.ToDateTime(dt.Rows[0]["ApplyDate"].ToString()).Month.ToString());
            }
            
            LabelStation.Text = dt.Rows[0]["Station"].ToString();
            LabelDepartment.Text = dt.Rows[0]["Department"].ToString();
            hdCopy.Value = copysuc ? "1" : "0";//用于传递给子窗口是否复制

            //获取该人币种,可能
            //DataSet ds1 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dt.Rows[0]["PersonID"].ToString());
            //if (ds1.Tables[0].Rows.Count == 1)
            //{
            //    DataTable dt1 = ds1.Tables[0];
            //    hdCur.Value = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dt1.Rows[0]["stationCode"].ToString());

            //    DataTable dttemp = new DataTable();
            //    string sqltemp = "select * from ESUSER where Userid='" + dt.Rows[0]["PersonID"].ToString() + "'";
            //    dttemp = dbc.GetData("eReimbursement", sqltemp);
            //    if (dttemp.Rows.Count > 0)
            //    {
            //        hdCur.Value = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dttemp.Rows[0]["Station"].ToString());
            //    }
            //}
            //else
            //{
            //    ErrorHandle("Data Error."); return;
            //}

            txtRemark.Text = dt.Rows[0]["Remark"].ToString();
            //根据语言设置
            string sqlp = "";
            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
            {
                sqlp += ",[SubType]=case when t1.Type='O' then t2.ADes else TDicSubType.CText end,[COAName]=case when t1.Type='O' then t2.ADes else '' end ";
            }
            else
            {
                sqlp += ",[SubType]=case when t1.Type='O' then t2.SAccountName else TDicSubType.EText end,[COAName]=case when t1.Type='O' then t2.SAccountName else '' end ";
            }
            ////取得另一个申请单链接
            //string fw = "select ID,Budget,[No] from Ecommon where AnotherRequestID=" + ID;
            //DataTable dtfw = new DataTable();
            //dtfw = dbc.GetData("eReimbursement", fw);
            //if (dtfw.Rows.Count == 1)
            //{
            //    if (dtfw.Rows[0]["Budget"].ToString()=="1")
            //    {
            //        if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
            //        {
            //            LabelBudgetLink.Text = "双生表单(预算内):";
            //        }
            //        else
            //        {
            //            LabelBudgetLink.Text = "Twin-Request(Budget):";
            //        }
            //    }
            //    else
            //    {
            //        if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
            //        {
            //            LabelBudgetLink.Text = "双生表单(超预算):";
            //        }
            //        else
            //        {
            //            LabelBudgetLink.Text = "Twin-Request(Un-Budget):";
            //        }
            //    }
            //    HLUnBudgetRequest.Text = dtfw.Rows[0]["No"].ToString();
            //    string url = "";
            //    if (Request.Url.Host != "localhost")
            //    {
            //        url = "http://" + Request.Url.Authority + "/eReimbursement/Apply.aspx?ID=" + dtfw.Rows[0]["ID"].ToString();
            //    }
            //    else
            //    {
            //        url = "http://" + Request.Url.Authority + "/Apply.aspx?ID=" + dtfw.Rows[0]["ID"].ToString();
            //    }
            //    HLUnBudgetRequest.NavigateUrl = url;
            //}
            string detailsql = "select t1.*" + sqlp + " from EeommonDetail t1 left join (select * from Edic where KeyValue='SubType') TDicSubType on TDicSubType.CValue=t1.Type left join AccoundCode t2 on t2.SAccountCode=t1.AccountCode where t1.No='" + ID + "'";
            DataTable dtdetail = new DataTable();
            dtdetail = dbc.GetData("eReimbursement", detailsql);
            ////币种,可能用于向子窗体传递
            //hdCur.Value = dtdetail.Rows[0]["Cur"].ToString();
            //DataTable dttemp = new DataTable();
            //string sqltemp = "select * from ESUSER where Userid='" + dt.Rows[0]["PersonID"].ToString() + "'";
            //dttemp = dbc.GetData("eReimbursement", sqltemp);
            //if (dttemp.Rows.Count > 0)
            //{
            //    hdCur.Value = dttemp.Rows[0]["Currency"].ToString();//如果单独设置了币种
            //}
            DataTable dtnew = new DataTable();
            dtnew.Columns.Add(new DataColumn("DetailID", typeof(System.Int32)));
            dtnew.Columns.Add("MainID", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Type", System.Type.GetType("System.String"));
            dtnew.Columns.Add("AccountName", System.Type.GetType("System.String"));
            dtnew.Columns.Add("AccountCode", System.Type.GetType("System.String"));
            dtnew.Columns.Add("AccountDes", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Cur", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Amount", System.Type.GetType("System.String"));
            dtnew.Columns.Add("TSation", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Tdate", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Fcity", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Tcity", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Attach", System.Type.GetType("System.String"));
            dtnew.Columns.Add("EType", System.Type.GetType("System.String"));
            dtnew.Columns.Add("EcompanyCode", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Ecompany", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Eperson", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Epurpos", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Creadedby", System.Type.GetType("System.String"));
            dtnew.Columns.Add("CreaedeDate", System.Type.GetType("System.String"));
            dtnew.Columns.Add("EffectTime", System.Type.GetType("System.String"));
            dtnew.Columns.Add("ETypeCode", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Draft", System.Type.GetType("System.String"));
            dtnew.Columns.Add("SubType", System.Type.GetType("System.String"));
            dtnew.Columns.Add("COAName", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Department1", System.Type.GetType("System.String"));
            dtnew.Columns.Add("PaymentType", System.Type.GetType("System.String"));
            dtnew.Columns.Add("PaymentDate", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Vendor", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Budget", System.Type.GetType("System.Boolean"));
            dtnew.Columns.Add("BudgetCurrent", System.Type.GetType("System.String"));
            dtnew.Columns.Add(new DataColumn("Year", typeof(System.Int16)));
            dtnew.Columns.Add(new DataColumn("StationBudget", typeof(System.Decimal)));
            dtnew.Columns.Add(new DataColumn("StationBudgetUsed", typeof(System.Decimal)));
            decimal sum = 0;
            for (int i = 0; i < dtdetail.Rows.Count; i++)
            {
                DataRow dr = dtnew.NewRow();
                dr["DetailID"] = Convert.ToInt32(dtdetail.Rows[i]["ID"].ToString());
                dr["MainID"] = dtdetail.Rows[i]["No"].ToString();
                dr["Type"] = dtdetail.Rows[i]["Type"].ToString();
                dr["AccountName"] = dtdetail.Rows[i]["AccountName"].ToString();
                dr["AccountCode"] = dtdetail.Rows[i]["AccountCode"].ToString();
                dr["AccountDes"] = dtdetail.Rows[i]["AccountDes"].ToString();
                dr["Cur"] = dtdetail.Rows[i]["Cur"].ToString();
                dr["Amount"] = dtdetail.Rows[i]["Amount"].ToString();
                dr["TSation"] = dtdetail.Rows[i]["TSation"].ToString();
                dr["Tdate"] = dtdetail.Rows[i]["Type"].ToString() == "C" ? "" : Convert.ToDateTime(dtdetail.Rows[i]["Tdate"].ToString()).ToString("yyyy/MM/dd");
                dr["Fcity"] = dtdetail.Rows[i]["Fcity"].ToString();
                dr["Tcity"] = dtdetail.Rows[i]["Tcity"].ToString();
                dr["Attach"] = transAttach ? dtdetail.Rows[i]["Attach"].ToString() : "";
                dr["EType"] = dtdetail.Rows[i]["EType"].ToString();
                dr["EcompanyCode"] = dtdetail.Rows[i]["EcompanyCode"].ToString();
                dr["Ecompany"] = dtdetail.Rows[i]["Ecompany"].ToString();
                dr["Eperson"] = dtdetail.Rows[i]["Eperson"].ToString();
                dr["Epurpos"] = dtdetail.Rows[i]["Epurpos"].ToString();
                dr["Creadedby"] = dtdetail.Rows[i]["Creadedby"].ToString();
                dr["EffectTime"] = dtdetail.Rows[i]["EffectTime"].ToString();
                dr["ETypeCode"] = dtdetail.Rows[i]["ETypeCode"].ToString();
                dr["CreaedeDate"] = dtdetail.Rows[i]["CreaedeDate"].ToString() == "" ? "" : Convert.ToDateTime(dtdetail.Rows[i]["CreaedeDate"].ToString()).ToString("yyyy/MM/dd");
                dr["Draft"] = hdSubStatus.Value.ToString();//传递给子页面以判断按钮状态 0:允许修改,允许上传;1:不允许修改,允许上传;2:全不允许
                dr["SubType"] = dtdetail.Rows[i]["SubType"].ToString();
                dr["COAName"] = dtdetail.Rows[i]["COAName"].ToString();
                
                
                dtnew.Rows.Add(dr);
                sum += dtdetail.Rows[i]["Amount"].ToString() == "" ? 0 : Convert.ToDecimal(dtdetail.Rows[i]["Amount"].ToString());
            }
            dtnew.DefaultView.Sort = "DetailID ASC";
            DataTable dt2 = dtnew.DefaultView.ToTable();
            //decimal sum1 = 0;
            Store1.DataSource = dt2;
            Store1.DataBind();
            hdSum.Value = sum.ToString();
            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
            {
                GridPanel1.Title = "张数合计: " + dtdetail.Rows.Count.ToString() + ", 金额合计: " + sum.ToString("#,##0.00");
            }
            else
            {
                GridPanel1.Title = "Total sheets: " + dtdetail.Rows.Count.ToString() + ", Sum: " + sum.ToString("#,##0.00");
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

            if (CheckCopy && dtdetail.Rows.Count > 0)
            {
                //如果被复制申请的币种与本人币种不一致,则无法复制保存
                string curOri = dtdetail.Rows[0]["Cur"].ToString();
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

            //140226 显示预算
            if (dt.Rows[0]["OnBehalfPersonID"].ToString()!="")
            {
                return;
            }
            DataTable dtbudget = new DataTable();

            //StoreBudget添加Field
            StoreBudget.Reader[0].Fields.Add("Year", RecordFieldType.String);
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
            string sqlbu = "select Year,EName,COACode,LocalAmount as [Current],PU,PB,PPercent,DU,DB,DPercent,SU,SB,SPercent from Budget_Complete where FormType='G' and RequestID=" + ID;
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
                cm.Columns.Add(new Column
                {
                    DataIndex = "Year",
                    Header = "Budget Year",
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
                
                dtbudget.Columns.Add("Year", typeof(System.String));//区分跨年情况
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

                string sqld = "select * from EeommonDetail where [No]='" + ID + "' order by id";
                DataTable dtall = new DataTable();
                dtall = dbc.GetData("eReimbursement", sqld);
                //取得预算日期
                string sqlA = "select AccountCode as COACode,case when t1.Type='O' then t2.SAccountName else TDicSubType.EText end as [EName],year(Tdate) as [Year],Amount from EeommonDetail t1 left join (select * from Edic where KeyValue='SubType') TDicSubType on TDicSubType.CValue=t1.Type left join AccoundCode t2 on t2.SAccountCode=t1.AccountCode where t1.No='" + ID + "'";
                DataTable dtA = dbc.GetData("eReimbursement", sqlA);
                //取得本币与成本中心汇率转换
                decimal rate = 1;
                string CurLocal = dtall.Rows[0]["Cur"].ToString();
                string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dtall.Rows[0]["TSation"].ToString());
                

                //合计数据
                DataTable dtB = new DataTable();
                dtB.Columns.Add("EName", typeof(System.String));
                dtB.Columns.Add("COACode", typeof(System.String));
                dtB.Columns.Add("Amount", typeof(System.Decimal));
                dtB.Columns.Add("Year", typeof(System.String));
                for (int i = 0; i < dtA.Rows.Count; i++)
                {
                    bool er = false;
                    for (int j = 0; j < dtB.Rows.Count; j++)
                    {
                        if (dtB.Rows[j]["COACode"].ToString() == dtA.Rows[i]["COACode"].ToString() && dtB.Rows[j]["Year"].ToString() == dtA.Rows[i]["Year"].ToString())//已有记录
                        {
                            er = true;
                            break;
                        }
                    }
                    if (!er)//不存在重复记录
                    {
                        DataRow dr = dtB.NewRow();
                        dr["EName"] = dtA.Rows[i]["EName"].ToString();
                        dr["COACode"] = dtA.Rows[i]["COACode"].ToString();
                        dr["Amount"] = dtA.Compute("Sum(Amount)", "Year = " + dtA.Rows[i]["Year"].ToString() + " and COACode = " + dtA.Rows[i]["COACode"].ToString());
                        dr["Year"] = dtA.Rows[i]["Year"].ToString();
                        dtB.Rows.Add(dr);
                    }
                }
                string userid = dt.Rows[0]["PersonID"].ToString();
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
                        if (copysuc)
                        {
                            dtC = Comm.RtnEB(userid, department, ostation, ostation, accountcode, dtB.Rows[g]["Year"].ToString(), "1");
                        }
                        else
                        {
                            dtC = Comm.ExRtnEB(userid, department, ostation, ostation, accountcode, dtB.Rows[g]["Year"].ToString(), "1", "G", ID);
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
                bool UnBudget = false;
                bool PB = false, DB = false, SB = false;
                //计算%,取得名称,转为本地币种汇率,增加列记录Currency为邮件准备
                dtbudget.Columns.Add("Currency", typeof(System.String));
                for (int i = 0; i < dtbudget.Rows.Count; i++)
                {
                    dtbudget.Rows[i]["Currency"] = CurLocal;
                    if (Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()) != 0)//如果Budget不为0,则计算%
                    {
                        decimal PPercent = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["PU"].ToString()) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()), 2);
                        dtbudget.Rows[i]["PPercent"] = PPercent;
                        if (!UnBudget)
                        {
                            if (PPercent > 100)
                            {
                                UnBudget = true;
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
                        dtbudget.Rows[i]["DPercent"] = DPercent;
                        if (!UnBudget)
                        {
                            if (DPercent > 100)
                            {
                                UnBudget = true;
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
                        dtbudget.Rows[i]["SPercent"] = SPercent;
                        if (!UnBudget)
                        {
                            if (SPercent > 100)
                            {
                                UnBudget = true;
                            }
                        }
                        if (!SB)
                        {
                            SB = true;
                        }
                    }
                    if (CurLocal != CurBudget)
                    {
                        rate = DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(CurLocal, CurBudget, Convert.ToInt16(dtbudget.Rows[i]["Year"].ToString()));
                    }
                    dtbudget.Rows[i]["PU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["PU"].ToString()), 2);
                    dtbudget.Rows[i]["PB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()), 2);
                    dtbudget.Rows[i]["DU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["DU"].ToString()), 2);
                    dtbudget.Rows[i]["DB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()), 2);
                    dtbudget.Rows[i]["SU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["SU"].ToString()), 2);
                    dtbudget.Rows[i]["SB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()), 2);
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
                    Width = 160
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
                cm.Columns.Add(new Column
                {
                    DataIndex = "Year",
                    Header = "Budget Year",
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
            StoreBudget.DataSource = dtbudget;
            StoreBudget.DataBind();

            
        }
        [Serializable]
        public class CCMailList
        {
            [XmlElement("Email")]
            public string Email { get; set; }
        }
        [Serializable]
        public class DetailExpense
        {
            [XmlElement("DetailID")]
            public string DetailID { get; set; }

            [XmlElement("MainID")]
            public string MainID { get; set; }

            [XmlElement("Type")]
            public string Type { get; set; }

            [XmlElement("AccountName")]
            public string AccountName { get; set; }

            [XmlElement("AccountCode")]
            public string AccountCode { get; set; }

            [XmlElement("AccountDes")]
            public string AccountDes { get; set; }

            [XmlElement("Cur")]
            public string Cur { get; set; }

            [XmlElement("Amount")]
            public string Amount { get; set; }

            [XmlElement("TSation")]
            public string TSation { get; set; }

            [XmlElement("Tdate")]
            public string Tdate { get; set; }

            [XmlElement("Fcity")]
            public string Fcity { get; set; }

            [XmlElement("Tcity")]
            public string Tcity { get; set; }

            [XmlElement("Attach")]
            public string Attach { get; set; }

            [XmlElement("EType")]
            public string EType { get; set; }

            [XmlElement("EcompanyCode")]
            public string EcompanyCode { get; set; }

            [XmlElement("Ecompany")]
            public string Ecompany { get; set; }

            [XmlElement("Eperson")]
            public string Eperson { get; set; }

            [XmlElement("Epurpos")]
            public string Epurpos { get; set; }

            [XmlElement("Creadedby")]
            public string Creadedby { get; set; }

            [XmlElement("CreaedeDate")]
            public string CreaedeDate { get; set; }

            [XmlElement("EffectTime")]
            public string EffectTime { get; set; }

            [XmlElement("ETypeCode")]
            public string ETypeCode { get; set; }

            [XmlElement("Draft")]
            public string Draft { get; set; }

            [XmlElement("SubType")]
            public string SubType { get; set; }

            [XmlElement("COAName")]
            public string COAName { get; set; }

            [XmlElement("Department1")]
            public string Department1 { get; set; }

            [XmlElement("PaymentType")]
            public string PaymentType { get; set; }
            [XmlElement("PaymentDate")]
            public string PaymentDate { get; set; }
            [XmlElement("Vendor")]
            public string Vendor { get; set; }
            [XmlElement("Budget")]
            public Boolean Budget { get; set; }
            [XmlElement("BudgetCurrent")]
            public string BudgetCurrent { get; set; }
        }
        //protected void changelang(object sender, EventArgs e)
        //{
        //    //PagingToolbar1.DisplayMsg = "Display {0} - {1} of {2}";
        //    //ResourceManager1.Locale = "zh-CN";
        //    //GridPanel1.ColumnModel.Columns[2].Header = "Column2";
        //    //HttpCookie cookie = new HttpCookie("lang");
        //    //Ext.Net.Button btn = (Ext.Net.Button)sender;
        //    //string lang = btn.Text == "English" ? "en-US" : "zh-CN";
        //    //if (Request.Cookies["lang"] == null)//如Cookies中无语言信息,则添加语言信息的Cookie
        //    //{
        //    //    cookie.Value = lang;
        //    //    Response.Cookies.Add(cookie);
        //    //}
        //    //else
        //    //{
        //    //    cookie.Value = lang;
        //    //}

        //    HttpCookie cookie = new HttpCookie("lang");
        //    cookie.Value = "en-us";
        //    Request.Cookies.Remove("lang");
        //    Request.Cookies.Add(cookie);
        //    Response.Cookies.Remove("lang");
        //    Response.Cookies.Add(cookie);
        //    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cookie.Value);
        //    Thread.CurrentThread.CurrentUICulture = new CultureInfo(cookie.Value);
        //}
        protected void ErrorHandle(string msg)
        {
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Message",
                Message = msg,
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.WARNING,
                Fn = new JFunction { Fn = "ShowFunction" }
            });
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
        /// <summary>
        /// 区分Budget保存正式Detail
        /// </summary>
        /// <param name="detailjson">json数据</param>
        /// <param name="Type">Budget:1/Un-Budget:0</param>
        /// <returns></returns>
        protected bool SaveDetail2(string detailjson,string Type,string RequestID)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<DetailExpense> Details = ser.Deserialize<List<DetailExpense>>(detailjson);

            DateTime dtnull = new DateTime(1, 1, 1, 0, 0, 0);
            cs.DBCommand dbc = new cs.DBCommand();
            //删除现有草稿的Detail数据
            string deletesql = "delete from EeommonDetail where [No]=" + RequestID;
            string newid1 = dbc.UpdateData("eReimbursement", deletesql, "Update");
            if (newid1 == "-1")
            {
                ErrorHandle("Data Error.");
                return false;
            }
            if (Type == "1")//预算内
            {
                decimal tamount = 0M;
                foreach (DetailExpense detail in Details)
                {
                    //新增
                    if ((detail.BudgetCurrent == "预算内" || detail.BudgetCurrent == "In Budget") && !detail.Budget)
                    {
                        string word = "[No],[Type],[AccountName],[AccountCode],[AccountDes],[Cur],[Amount],[TSation],[Tdate],[Fcity],[Tcity],[Attach],[EType],[EcompanyCode],[Ecompany],[Eperson],[Epurpos],[Creadedby],[CreaedeDate],[EffectTime],[ETypeCode],[Department1],[PaymentType],[PaymentDate],[Vendor],[Budget],[BudgetCurrent],[CostCenterAmount]";
                        string value = "";
                        value += "'" + RequestID + "',";
                        value += "'" + detail.Type + "',";
                        value += "'" + detail.AccountName + "',";
                        value += "'" + detail.AccountCode + "',";
                        value += detail.AccountDes == null ? "null," : ("'" + detail.AccountDes.Replace("'", "''") + "',");
                        value += "'" + detail.Cur + "',";
                        value += detail.Amount == "" ? "null," : (detail.Amount + ",");
                        value += "'" + detail.TSation + "',";
                        //value += detail.Tdate == "" ? "null," : ("'" + detail.Tdate + "',");
                        if (detail.Type == "C")
                        {
                            value += "'" + Convert.ToDateTime(detail.EffectTime).ToString("yyyy/MM/dd") + "',";
                        }
                        else
                        {
                            value += detail.Tdate == "" ? "null," : ("'" + detail.Tdate + "',");
                        }
                        value += detail.Fcity == null ? "null," : ("'" + detail.Fcity.Replace("'", "''") + "',");
                        value += detail.Tcity == null ? "null," : ("'" + detail.Tcity.Replace("'", "''") + "',");
                        value += "'" + detail.Attach + "',";
                        value += detail.EType == null ? "null," : ("'" + detail.EType.Replace("'", "''") + "',");
                        value += detail.EcompanyCode == null ? "null," : ("'" + detail.EcompanyCode.Replace("'", "''") + "',");
                        value += detail.Ecompany == null ? "null," : ("'" + detail.Ecompany.Replace("'", "''") + "',");
                        value += detail.Eperson == null ? "null," : ("'" + detail.Eperson.Replace("'", "''") + "',");
                        value += detail.Epurpos == null ? "null," : ("'" + detail.Epurpos.Replace("'", "''") + "',");
                        value += "'" + Request.Cookies.Get("eReimUserID").Value + "',";//Session
                        value += "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',";
                        value += detail.EffectTime == null ? "null," : ("'" + detail.EffectTime.Replace("'", "''") + "',");
                        value += detail.ETypeCode == null ? "null" : ("'" + detail.ETypeCode.Replace("'", "''") + "'");
                        value += ",'" + detail.Department1 + "'";
                        value += ",'" + detail.PaymentType + "'";
                        value += detail.PaymentDate == "" ? ",null" : (",'" + detail.PaymentDate + "'");
                        value += ",'" + detail.Vendor + "'";
                        value += ",1,1,";
                        //处理成本中心币种金额
                        DataSet ds2 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(cbxPerson.Text);
                        string station = "";
                        if (ds2.Tables[0].Rows.Count == 1)
                        {
                            DataTable dt1 = ds2.Tables[0];
                            station = dt1.Rows[0]["stationCode"].ToString();
                            DataTable dttemp = new DataTable();
                            string sqltemp = "select * from ESUSER where Userid='" + cbxPerson.Text + "'";
                            dttemp = dbc.GetData("eReimbursement", sqltemp);
                            if (dttemp.Rows.Count > 0)
                            {
                                station = dttemp.Rows[0]["Station"].ToString();
                            }
                        }
                        string Tstation = detail.TSation == "" ? station : detail.TSation;
                        string ccamount = Convert.ToDecimal(System.Math.Round(Convert.ToDouble(detail.Amount) * (DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(station) / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(Tstation)), 2)).ToString();
                        value += detail.Amount == "" ? "null" : ccamount;

                        string updatesql = "insert into EeommonDetail (" + word + ") values(" + value + ")";
                        string newid = dbc.UpdateData("eReimbursement", updatesql, "Update");
                        if (newid == "-1")
                        {
                            ErrorHandle("Data Error."); return false;
                        }
                        tamount += Convert.ToDecimal(detail.Amount);
                    }
                }
                string ecommonsql = "update Ecommon set Tamount=" + tamount + " where ID=" + RequestID;
                string ecommonre = dbc.UpdateData("eReimbursement", ecommonsql, "Update");
            }
            else
            {
                decimal tamount = 0M;
                foreach (DetailExpense detail in Details)
                {
                    //新增
                    if ((detail.BudgetCurrent == "超预算" || detail.BudgetCurrent == "Over Budget" || detail.BudgetCurrent == "NA") || detail.Budget)
                    {
                        string word = "[No],[Type],[AccountName],[AccountCode],[AccountDes],[Cur],[Amount],[TSation],[Tdate],[Fcity],[Tcity],[Attach],[EType],[EcompanyCode],[Ecompany],[Eperson],[Epurpos],[Creadedby],[CreaedeDate],[EffectTime],[ETypeCode],[Department1],[PaymentType],[PaymentDate],[Vendor],[Budget],[BudgetCurrent],[CostCenterAmount]";
                        string value = "";
                        value += "'" + RequestID + "',";
                        value += "'" + detail.Type + "',";
                        value += "'" + detail.AccountName + "',";
                        value += "'" + detail.AccountCode + "',";
                        value += detail.AccountDes == null ? "null," : ("'" + detail.AccountDes.Replace("'", "''") + "',");
                        value += "'" + detail.Cur + "',";
                        value += detail.Amount == "" ? "null," : (detail.Amount + ",");
                        value += "'" + detail.TSation + "',";
                        //value += detail.Tdate == "" ? "null," : ("'" + detail.Tdate + "',");
                        if (detail.Type == "C")
                        {
                            value += "'" + Convert.ToDateTime(detail.EffectTime).ToString("yyyy/MM/dd") + "',";
                        }
                        else
                        {
                            value += detail.Tdate == "" ? "null," : ("'" + detail.Tdate + "',");
                        }
                        value += detail.Fcity == null ? "null," : ("'" + detail.Fcity.Replace("'", "''") + "',");
                        value += detail.Tcity == null ? "null," : ("'" + detail.Tcity.Replace("'", "''") + "',");
                        value += "'" + detail.Attach + "',";
                        value += detail.EType == null ? "null," : ("'" + detail.EType.Replace("'", "''") + "',");
                        value += detail.EcompanyCode == null ? "null," : ("'" + detail.EcompanyCode.Replace("'", "''") + "',");
                        value += detail.Ecompany == null ? "null," : ("'" + detail.Ecompany.Replace("'", "''") + "',");
                        value += detail.Eperson == null ? "null," : ("'" + detail.Eperson.Replace("'", "''") + "',");
                        value += detail.Epurpos == null ? "null," : ("'" + detail.Epurpos.Replace("'", "''") + "',");
                        value += "'" + Request.Cookies.Get("eReimUserID").Value + "',";//Session
                        value += "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',";
                        value += detail.EffectTime == null ? "null," : ("'" + detail.EffectTime.Replace("'", "''") + "',");
                        value += detail.ETypeCode == null ? "null" : ("'" + detail.ETypeCode.Replace("'", "''") + "'");
                        value += ",'" + detail.Department1 + "'";
                        value += ",'" + detail.PaymentType + "'";
                        value += detail.PaymentDate == "" ? ",null" : (",'" + detail.PaymentDate + "'");
                        value += ",'" + detail.Vendor + "'";
                        value += detail.Budget ? ",0" : ",1";
                        if (detail.BudgetCurrent == "超预算" || detail.BudgetCurrent == "Over Budget")
                        {
                            value += ",0,";
                        }
                        else
                        {
                            value += ",-1,";
                        }
                        //处理成本中心币种金额
                        DataSet ds2 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(cbxPerson.Text);
                        string station = "";
                        if (ds2.Tables[0].Rows.Count == 1)
                        {
                            DataTable dt1 = ds2.Tables[0];
                            station = dt1.Rows[0]["stationCode"].ToString();
                            DataTable dttemp = new DataTable();
                            string sqltemp = "select * from ESUSER where Userid='" + cbxPerson.Text + "'";
                            dttemp = dbc.GetData("eReimbursement", sqltemp);
                            if (dttemp.Rows.Count > 0)
                            {
                                station = dttemp.Rows[0]["Station"].ToString();
                            }
                        }
                        string Tstation = detail.TSation == "" ? station : detail.TSation;
                        string ccamount = Convert.ToDecimal(System.Math.Round(Convert.ToDouble(detail.Amount) * (DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(station) / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(Tstation)), 2)).ToString();
                        value += detail.Amount == "" ? "null" : ccamount;

                        string updatesql = "insert into EeommonDetail (" + word + ") values(" + value + ")";
                        string newid = dbc.UpdateData("eReimbursement", updatesql, "Update");
                        if (newid == "-1")
                        {
                            ErrorHandle("Data Error."); return false;
                        }
                        tamount += Convert.ToDecimal(detail.Amount);
                    }
                }
                string ecommonsql = "update Ecommon set Tamount=" + tamount + " where ID=" + RequestID;
                string ecommonre = dbc.UpdateData("eReimbursement", ecommonsql, "Update");
            }
            return true;
        }
        /// <summary>
        /// 保存草稿明细,不保存Budget状态
        /// </summary>
        /// <param name="detailjson">json数据</param>
        /// <returns>保存是否成功</returns>
        protected bool SaveDetail(string detailjson,string localcur,string costcenter,string costcentercur)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<DetailExpense> Details = ser.Deserialize<List<DetailExpense>>(detailjson);

            //DateTime dtnull = new DateTime(1, 1, 1, 0, 0, 0);
            cs.DBCommand dbc = new cs.DBCommand();

            //取得币种转换值
            //删除现有数据
            string deletesql = "delete from EeommonDetail where [No]='" + hdTravelRequestID.Value.ToString() + "'";
            string newid1 = dbc.UpdateData("eReimbursement", deletesql, "Update");
            if (newid1 == "-1")
            {
                ErrorHandle("Data Error.");
                return false;
            }
            foreach (DetailExpense detail in Details)
            {
                
                //新增
                string word = "[No],[Type],[AccountName],[AccountCode],[AccountDes],[Cur],[Amount],[TSation],[Tdate],[Fcity],[Tcity],[Attach],[EType],[EcompanyCode],[Ecompany],[Eperson],[Epurpos],[Creadedby],[CreaedeDate],[EffectTime],[ETypeCode],[Department1],[PaymentType],[PaymentDate],[Vendor],[Budget],[CostCenterAmount]";
                string value = "";
                value += "'" + hdTravelRequestID.Value.ToString() + "',";
                value += "'" + detail.Type + "',";
                value += "'" + detail.AccountName + "',";
                value += "'" + detail.AccountCode + "',";
                value += detail.AccountDes == null ? "null," : ("'" + detail.AccountDes.Replace("'", "''") + "',");
                value += "'" + localcur + "',";
                value += detail.Amount == "" ? "null," : (detail.Amount + ",");
                value += "'" + costcenter + "',";

                if (detail.Type == "C")
                {
                    value += "'" + Convert.ToDateTime(detail.EffectTime).ToString("yyyy/MM/dd") + "',";
                }
                else
                {
                    value += detail.Tdate == "" ? "null," : ("'" + detail.Tdate + "',");
                }
                value += detail.Fcity == null ? "null," : ("'" + detail.Fcity.Replace("'", "''") + "',");
                value += detail.Tcity == null ? "null," : ("'" + detail.Tcity.Replace("'", "''") + "',");
                value += "'" + detail.Attach + "',";
                value += detail.EType == null ? "null," : ("'" + detail.EType.Replace("'", "''") + "',");
                value += detail.EcompanyCode == null ? "null," : ("'" + detail.EcompanyCode.Replace("'", "''") + "',");
                value += detail.Ecompany == null ? "null," : ("'" + detail.Ecompany.Replace("'", "''") + "',");
                value += detail.Eperson == null ? "null," : ("'" + detail.Eperson.Replace("'", "''") + "',");
                value += detail.Epurpos == null ? "null," : ("'" + detail.Epurpos.Replace("'", "''") + "',");
                value += "'" + Request.Cookies.Get("eReimUserID").Value + "',";//Session
                value += "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',";
                value += detail.EffectTime == null ? "null," : ("'" + detail.EffectTime.Replace("'", "''") + "',");
                value += detail.ETypeCode == null ? "null" : ("'" + detail.ETypeCode.Replace("'", "''") + "'");
                value += ",'" + detail.Department1 + "'";
                value += ",'" + detail.PaymentType + "'";
                value += detail.PaymentDate == "" ? ",null" : (",'" + detail.PaymentDate + "'");
                value += ",'" + detail.Vendor + "'";
                if (detail.Budget)
                {
                    value += ",0,";
                }
                else
                {
                    value += ",1,";
                }
                //处理成本中心币种金额
                //DataSet ds2 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(cbxPerson.Text);
                //string station = "";
                //if (ds2.Tables[0].Rows.Count == 1)
                //{
                //    DataTable dt1 = ds2.Tables[0];
                //    station = dt1.Rows[0]["stationCode"].ToString();
                //    DataTable dttemp = new DataTable();
                //    string sqltemp = "select * from ESUSER where Userid='" + cbxPerson.Text + "'";
                //    dttemp = dbc.GetData("eReimbursement", sqltemp);
                //    if (dttemp.Rows.Count > 0)
                //    {
                //        station = dttemp.Rows[0]["Station"].ToString();
                //    }
                //}
                //string Tstation = detail.TSation == "" ? station : detail.TSation;
                decimal re = GetRateByUserID(DateTime.Now.Year, true, localcur,costcentercur);
                string ccamount = String.IsNullOrEmpty(detail.Amount) ? "0" : (System.Math.Round(Convert.ToDecimal(detail.Amount) * re, 2)).ToString();
                value += ccamount;


                string updatesql = "insert into EeommonDetail (" + word + ") values(" + value + ")";
                string newid = dbc.UpdateData("eReimbursement", updatesql, "Update");
                if (newid == "-1")
                {
                    ErrorHandle("Data Error."); return false;
                }
            }
            return true;
        }
        protected decimal GetRateByUserID(int year, bool con, string LocalCurrency, string BudgetCurrency)
        {
            //cs.DBCommand dbc = new cs.DBCommand();
            ////取得币种转换值
            //DataSet dsuserinfo = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(UserID);
            //DataTable dtuserinfo = dsuserinfo.Tables[0];
            //string station = dtuserinfo.Rows[0]["stationCode"].ToString();
            ////string costcenter = dtuserinfo.Rows[0]["CostCenter"].ToString();
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
        [DirectMethod]
        public void SaveAll1(string type, string detail, string MailList,string budget)
        {
            if (Request.Cookies.Get("eReimUserID") == null || hdUser.Value.ToString() != Request.Cookies.Get("eReimUserID").Value)
            {
                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                {
                    X.AddScript("Ext.Msg.show({ title: '提示', msg: '登录超时或已切换用户,将刷新页面.', buttons: { ok: 'Ok' }, fn: function (btn) { window.location.reload(); } });");
                }
                else
                {
                    X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Time out or user changed,reloading...', buttons: { ok: 'Ok' }, fn: function (btn) { window.location.reload(); } });");
                }
                return;
            }
            //页面权限
            //DateTime dtnull = new DateTime(1, 1, 1, 0, 0, 0);
            cs.DBCommand dbc = new cs.DBCommand();
            //string userid = cbxPerson.Value.ToString();
            //160113 垫付人
            string userid = (hdOnBehalf.Value == null || hdOnBehalf.Value == "") ? cbxPerson.Value.ToString() : hdOnBehalf.Value.ToString();
            string ostation = "";
            string station_applyperson = ""; string costcenter_applyperson = ""; string dept_applyperson = "";
            DataSet ds_apply = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(cbxPerson.Value.ToString());
            if (ds_apply.Tables[0].Rows.Count == 1)
            {
                DataTable dt_apply = ds_apply.Tables[0];
                dept_applyperson = dt_apply.Rows[0]["DepartmentName"].ToString();
                station_applyperson = dt_apply.Rows[0]["stationCode"].ToString();
                costcenter_applyperson = dt_apply.Rows[0]["CostCenter"].ToString();
            }
            string para = type;
            string station = ""; string department = "";
            DataSet ds2 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(userid);
            if (ds2.Tables[0].Rows.Count == 1)
            {
                DataTable dt1 = ds2.Tables[0];
                //dpt = dt1.Rows[0]["DepartmentName"].ToString();
                station = dt1.Rows[0]["stationCode"].ToString();
                ostation = dt1.Rows[0]["CostCenter"].ToString();
                department = dt1.Rows[0]["CRPDepartmentName"].ToString();
            }

            decimal rate = 1;//记录用户币种与预算站点币种汇率
            string CurLocal = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(costcenter_applyperson);
            //检查是否本地维护过特殊币种
            DataTable dttemp = new DataTable();
            string sqltemp = "select * from ESUSER where Userid='" + cbxPerson.Value.ToString() + "'";
            dttemp = dbc.GetData("eReimbursement", sqltemp);
            if (dttemp.Rows.Count > 0)
            {
                CurLocal = dttemp.Rows[0]["Currency"].ToString();//如果单独设置了币种
            }
            string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(ostation);
            //预算
            DataTable dtbudget = new DataTable();
            dtbudget.Columns.Add("Year", typeof(System.String));//区分跨年情况
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

            //基础数据
            DataTable dtA = new DataTable();
            dtA.Columns.Add("EName", typeof(System.String));
            dtA.Columns.Add("COACode", typeof(System.String));
            dtA.Columns.Add("Amount", typeof(System.Decimal));
            dtA.Columns.Add("Year", typeof(System.String));

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<DetailExpense> Details = ser.Deserialize<List<DetailExpense>>(detail);
            foreach (DetailExpense details in Details)
            {
                DataRow dr = dtA.NewRow();
                string EName = "";
                if (details.Type == "E")
                {
                    EName = "Entertainment";
                }
                else if (details.Type == "T")
                {
                    EName = "Transportation";
                }
                else if (details.Type == "C")
                {
                    EName = "Communication";
                }
                else
                {
                    EName = details.COAName;
                }
                dr["EName"] = EName;
                dr["COACode"] = details.AccountCode;
                dr["Amount"] = Convert.ToDecimal(details.Amount);
                if (details.Type == "C")
                {
                    dr["Year"] = Convert.ToDateTime(details.EffectTime).Year.ToString();
                }
                else
                {
                    dr["Year"] = Convert.ToDateTime(details.Tdate).Year.ToString();
                }
                dtA.Rows.Add(dr);
            }
            //合计数据
            DataTable dtB = new DataTable();
            dtB.Columns.Add("EName", typeof(System.String));
            dtB.Columns.Add("COACode", typeof(System.String));
            dtB.Columns.Add("Amount", typeof(System.Decimal));
            dtB.Columns.Add("Year", typeof(System.String));
            for (int i = 0; i < dtA.Rows.Count; i++)
            {
                bool er = false;
                for (int j = 0; j < dtB.Rows.Count; j++)
                {
                    if (dtB.Rows[j]["COACode"].ToString() == dtA.Rows[i]["COACode"].ToString() && dtB.Rows[j]["Year"].ToString() == dtA.Rows[i]["Year"].ToString())//已有记录
                    {
                        er = true;
                        break;
                    }
                }
                if (!er)//不存在重复记录
                {
                    DataRow dr = dtB.NewRow();
                    dr["EName"] = dtA.Rows[i]["EName"].ToString();
                    dr["COACode"] = dtA.Rows[i]["COACode"].ToString();
                    dr["Amount"] = dtA.Compute("Sum(Amount)", "Year = " + dtA.Rows[i]["Year"].ToString() + " and COACode = " + dtA.Rows[i]["COACode"].ToString());
                    dr["Year"] = dtA.Rows[i]["Year"].ToString();
                    dtB.Rows.Add(dr);
                }
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
                    //if (!UnBudget)
                    //{
                    //    if (PPercent > 100)
                    //    {
                    //        UnBudget = true;
                    //    }
                    //}
                    //if (!PB)
                    //{
                    //    PB = true;
                    //}
                }
                if (Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()) != 0)//如果Budget不为0,则计算%
                {
                    decimal DPercent = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["DU"].ToString()) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()), 2);
                    dtbudget.Rows[i]["DPercent"] = DPercent;
                    //if (!UnBudget)
                    //{
                    //    if (DPercent > 100)
                    //    {
                    //        UnBudget = true;
                    //    }
                    //}
                    //if (!DB)
                    //{
                    //    DB = true;
                    //}
                }
                if (Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()) != 0)//如果Budget不为0,则计算%
                {
                    decimal SPercent = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["SU"].ToString()) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()), 2);
                    dtbudget.Rows[i]["SPercent"] = SPercent;
                    //if (!UnBudget)
                    //{
                    //    if (SPercent > 100)
                    //    {
                    //        UnBudget = true;
                    //    }
                    //}
                    //if (!SB)
                    //{
                    //    SB = true;
                    //}
                }
                if (CurLocal != CurBudget)
                {
                    rate = DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(CurLocal, CurBudget, Convert.ToInt16(dtbudget.Rows[i]["Year"].ToString()));
                }
                dtbudget.Rows[i]["PU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["PU"].ToString()), 2);
                dtbudget.Rows[i]["PB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()), 2);
                dtbudget.Rows[i]["DU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["DU"].ToString()), 2);
                dtbudget.Rows[i]["DB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()), 2);
                dtbudget.Rows[i]["SU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["SU"].ToString()), 2);
                dtbudget.Rows[i]["SB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()), 2);
            }
            //处理抄送人列表
            string CCMailList = "";
            JavaScriptSerializer serd = new JavaScriptSerializer();
            List<CCMailList> CCMailList1 = serd.Deserialize<List<CCMailList>>(MailList);
            foreach (CCMailList mail in CCMailList1)
            {
                CCMailList += mail.Email + ",";
            }
            CCMailList = CCMailList.Length > 0 ? CCMailList.Substring(0, CCMailList.Length - 1) : "";
            //判断是否设置了审批流程
            string sqlCheckFlow = ""; DataTable dtGroupFlowData = new DataTable();

            if (hdOnBehalf.Value == null || hdOnBehalf.Value.ToString() == "")
            {
                sqlCheckFlow = "select * from GroupFlow where [Type]=2 and GID=(select GID from GroupUsers where UserID='" + cbxPerson.Value.ToString() + "') order by FlowNo";
                dtGroupFlowData = dbc.GetData("eReimbursement", sqlCheckFlow);
                if (dtGroupFlowData.Rows.Count < 1)
                {
                    if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                    {
                        ErrorHandleNojump("请先设置审批人.");
                    }
                    else
                    {
                        ErrorHandleNojump("Not set Approve flow,please contact with Local MIS.");
                    }
                    return;
                }
            }
            else//160113 垫付审批流程
            {
                sqlCheckFlow = "select UserID,t1.* from (select * from GroupFlow where [Type]=2 and GID in (select GID from GroupUsers where UserID='" + cbxPerson.Value.ToString() + "' or UserID='" + hdOnBehalf.Value.ToString() + "')) t1 left join (select * from GroupUsers where UserID='" + cbxPerson.Value.ToString() + "' or UserID='" + hdOnBehalf.Value.ToString() + "') t2 on t2.Gid=t1.Gid order by Gid,FlowNo";
                //sqlCheckFlow = "select * from GroupFlow where [Type]=2 and GID in (select GID from GroupUsers where UserID='" + cbxPerson.Value.ToString() + "' or UserID='" + hdOnBehalf.Value.ToString() + "') order by Gid,FlowNo";
                dtGroupFlowData = dbc.GetData("eReimbursement", sqlCheckFlow);
                if (dtGroupFlowData.Rows.Count < 1)
                {
                    if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                    {
                        ErrorHandleNojump("请先设置审批人.");
                    }
                    else
                    {
                        ErrorHandleNojump("Not set Approve flow,please contact with Local MIS.");
                    }
                    return;
                }
            }





            //string sqlCheckFlow = "";
            //sqlCheckFlow = "select * from GroupFlow where [Type]=2 and GID=(select GID from GroupUsers where UserID='" + userid + "') order by FlowNo";
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
            if (type == "ND")//保存并申请
            {
                if (hdTravelRequestID.Value.ToString() != "")//草稿升级为正式申请
                {
                    if (LabelMonth.Text != DateTime.Now.Month.ToString() && DateTime.Now.Day > 5)
                    {
                        if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                        {
                            X.AddScript("Ext.Msg.show({ title: '提示', msg: '草稿已过期,请新增本月草稿.', buttons: { ok: 'Ok' }, fn: function (btn) { return false; } });");
                        }
                        else
                        {
                            X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Draft expired,please add new draft.', buttons: { ok: 'Ok' }, fn: function (btn) { return false; } });");
                        }
                        return;
                    }
                    string oldno = hdTravelRequestNo.Value.ToString();
                    string newno = hdTravelRequestNo.Value.ToString().Substring(0, hdTravelRequestNo.Value.ToString().Length - 1);
                    string updatesql = "update Ecommon set [No]='" + newno + "',";
                    updatesql += "Person='" + X.GetValue("cbxPerson") + "',";
                    updatesql += "PersonID='" + cbxPerson.Text + "',";
                    updatesql += "Department='" + dept_applyperson + "',";
                    updatesql += "Station='" + station_applyperson + "',";
                    updatesql += "Tamount=" + (hdSum.Value.ToString() == "" ? "null," : ("'" + hdSum.Value.ToString() + "',"));
                    updatesql += "[Type]=0,";
                    updatesql += "CreadedBy='" + Request.Cookies.Get("eReimUserName").Value + "',";
                    updatesql += "CreatedByID='" + Request.Cookies.Get("eReimUserID").Value + "',";
                    updatesql += "CreadedDate='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',";
                    updatesql += "Remark='" + txtRemark.Text.Replace("'", "''") + "',";
                    updatesql += "CCMailList='" + CCMailList + "'";
                    updatesql += ",[Budget]=" + budget;
                    updatesql += ",Station2='" + ostation + "'";
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
                    string sqlDeleteEflow = "delete from Eflow where [Type]='G' and [RequestID]='" + hdTravelRequestID.Value.ToString() + "'";
                    string deleterows = dbc.UpdateData("eReimbursement", sqlDeleteEflow, "Update");
                    string rows = "";

                    //160113 垫付
                    if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                    {
                        string personid = cbxPerson.Value.ToString();
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
                            valueflow += "'G',";
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
                            valueflow += "'G',";
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

                    //for (int j = 0; j < dtGroupFlowData.Rows.Count; j++)
                    //{
                    //    string wordflow = "[RequestID],[Type],[Step],[Status],[Approver],[ApproverID],[FlowFn]";
                    //    //if (dtGroupFlowData.Rows[j]["FlowNo"].ToString() == "1")
                    //    //{
                    //    //    wordflow += ",[Active]";
                    //    //}
                    //    if (j==0)
                    //    {
                    //        wordflow += ",[Active]";
                    //    }
                    //    string valueflow = "";
                    //    valueflow += "'" + hdTravelRequestID.Value.ToString() + "',";
                    //    valueflow += "'G',";
                    //    valueflow += dtGroupFlowData.Rows[j]["FlowNo"].ToString() + ",";
                    //    valueflow += "1,";
                    //    valueflow += "'" + dtGroupFlowData.Rows[j]["FlowUser"].ToString() + "',";
                    //    valueflow += "'" + dtGroupFlowData.Rows[j]["FlowUserid"].ToString() + "'";
                    //    valueflow += ",'" + (dtGroupFlowData.Rows[j]["Fn"].ToString() == "" ? "Approver" : dtGroupFlowData.Rows[j]["Fn"].ToString()) + "'";
                    //    //if (dtGroupFlowData.Rows[j]["FlowNo"].ToString() == "1")
                    //    //{
                    //    //    valueflow += ",1";
                    //    //}
                    //    if (j==0)
                    //    {
                    //        valueflow += ",1";
                    //    }

                    //    string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";

                    //    rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
                    //}
                    if (newid == "-1" || rows == "-1" || rows == "")
                    {
                        ErrorHandle("Data Error");
                    }
                    else
                    {
                        hdTravelRequestNo.Value = newno;
                        // || !SendMail(hdTravelRequestID.Value.ToString())
                        if (!SaveDetail(detail, CurLocal, ostation, CurBudget) || !SendMailNew(dtbudget))
                        {
                            ErrorHandle("Data Error.");
                            return;
                        }
                        if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                        {
                            Panel3.Title = "通用申请单:" + newno;
                            UpdateMSG("保存通用申请单:" + hdTravelRequestNo.Value.ToString() + "成功.");
                        }
                        else
                        {
                            Panel3.Title = "General Expense Form: " + newno;
                            UpdateMSG("Saved General Expense Form: " + hdTravelRequestNo.Value.ToString() + " successfully.");
                        }
                    }
                }
                else//直接正式申请
                {
                    string word = "[No],[Person],[PersonID],[Department],[Station],[ApplyDate],[Remark],[Tamount],[CreadedBy],[CreatedByID],[CreadedDate],[CCMailList],[Budget],[Station2]";
                    //160123 垫付
                    if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                    { word += ",OnBehalfPersonID,OnBehalfPersonName,OnBehalfPersonUnit,OnBehalfPersonDept,OnBehalfPersonCostCenter"; }
                    string value = "";
                    value += "'" + station_applyperson + DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString().PadLeft(2, '0') + "',";//edit
                    value += "'" + X.GetValue("cbxPerson") + "',";
                    value += "'" + cbxPerson.Text + "',";
                    value += "'" + dept_applyperson + "',";
                    value += "'" + station_applyperson + "',";
                    value += "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',";
                    value += "'" + txtRemark.Text.Replace("'", "''") + "',";
                    value += hdSum.Value.ToString() == "" ? "null," : hdSum.Value.ToString() + ",";
                    value += "'" + Request.Cookies.Get("eReimUserName").Value + "',";//session
                    value += "'" + Request.Cookies.Get("eReimUserID").Value + "',";//session
                    value += "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'";
                    value += ",'" + CCMailList + "'";
                    value += "," + budget;
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

                    string updatesql = "insert into Ecommon (" + word + ") values(" + value + ");update Ecommon set [No]=[No]+(select [MonthCount]=right('0000'+cast(count(ID) as varchar(10)),4) from Ecommon where (month(ApplyDate) in (select month(ApplyDate) from Ecommon where [ID]=@@IDENTITY) and (year(ApplyDate) in (select year(ApplyDate) from Ecommon where [ID]=@@IDENTITY)) and Station=(select Station from Ecommon where ID=@@IDENTITY)))+'G' where ID=@@IDENTITY;select [msg]=convert(varchar,ID)+','+[No] from Ecommon where ID=@@IDENTITY";

                    string newid = dbc.UpdateData("eReimbursement", updatesql, "Insert");

                    string rows = "";
                    
                    //160113 垫付
                    if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                    {
                        string personid = cbxPerson.Value.ToString();
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
                            valueflow += "'G',";
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
                        //for (int j = 0; j < dtGroupFlowData.Rows.Count; j++)
                        //{
                        //    string wordflow = "[RequestID],[Type],[Step],[Status],[Approver],[ApproverID],[FlowFn]";
                        //    if (j==0)
                        //    {
                        //        wordflow += ",[Active]";
                        //    }
                        //    string valueflow = "";
                        //    valueflow += "'" + newid.Split(',')[0] + "',";
                        //    valueflow += "'G',";
                        //    valueflow += dtGroupFlowData.Rows[j]["FlowNo"].ToString() + ",";
                        //    valueflow += "1,";
                        //    valueflow += "'" + dtGroupFlowData.Rows[j]["FlowUser"].ToString() + "',";
                        //    valueflow += "'" + dtGroupFlowData.Rows[j]["FlowUserid"].ToString() + "'";
                        //    valueflow += ",'" + (dtGroupFlowData.Rows[j]["Fn"].ToString() == "" ? "Approver" : dtGroupFlowData.Rows[j]["Fn"].ToString()) + "'";
                        //    if (j==0)
                        //    {
                        //        valueflow += ",1";
                        //    }

                        //    string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";
                        //    rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
                        //}
                        for (int j = 0; j < dtGroupFlowData.Rows.Count; j++)
                        {
                            string wordflow = "[RequestID],[Type],[Step],[Status],[Approver],[ApproverID],[FlowFn]";
                            if (j == 0)
                            {
                                wordflow += ",[Active]";
                            }
                            string valueflow = "";
                            valueflow += "'" + newid.Split(',')[0] + "',";
                            valueflow += "'G',";
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
                        ErrorHandle("Data Error");
                    }
                    else
                    {
                        hdTravelRequestID.Value = newid.Split(',')[0];//新增后记录ID
                        hdTravelRequestNo.Value = newid.Split(',')[1];//新增后记录No
                        if (!SaveDetail(detail, CurLocal, ostation, CurBudget) || !SendMailNew(dtbudget))
                        {
                            ErrorHandle("Data Error.");
                            return;
                        }
                        if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                        {
                            Panel3.Title = "通用申请单:" + newid.Split(',')[1];
                            UpdateMSG("保存通用申请单:" + hdTravelRequestNo.Value.ToString() + "成功.");
                        }
                        else
                        {
                            Panel3.Title = "General Expense Form: " + newid.Split(',')[1];
                            UpdateMSG("Saved General Expense Form: " + hdTravelRequestNo.Value.ToString() + " successfully.");
                        }
                    }
                }
                X.AddScript("btnSaveDraft.disable();btnSaveAndSend.disable();btnExport.enable();btnCC.disable();btnE.disable();btnT.disable();btnC.disable();btnO.disable();");
            }
            else//保存草稿信息时不保存Budget状态,仅正式申请时保存
            {
                if (hdTravelRequestID.Value.ToString() != "")//由链接进入的草稿更新
                {
                    if (LabelMonth.Text != DateTime.Now.Month.ToString() && DateTime.Now.Day > 5)//检查日期是否超期
                    {
                        if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                        {
                            X.AddScript("Ext.Msg.show({ title: '提示', msg: '草稿已过期,请新增本月草稿.', buttons: { ok: 'Ok' }, fn: function (btn) { return false; } });");
                        }
                        else
                        {
                            X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Draft expired,please add new draft.', buttons: { ok: 'Ok' }, fn: function (btn) { return false; } });");
                        }
                        return;
                    }

                    string updatesql = "update Ecommon set [Person]='" + X.GetValue("cbxPerson") + "',";
                    updatesql += "[Station]='" + station_applyperson + "',";
                    updatesql += "[Department]='" + dept_applyperson + "',";
                    updatesql += "[Tamount]=" + (hdSum.Value.ToString() == "" ? "null," : hdSum.Value.ToString()) + ",";
                    updatesql += "[Remark]='" + txtRemark.Text.Replace("'", "''") + "',";
                    updatesql += "[PersonID]='" + cbxPerson.Text + "',";
                    updatesql += "CCMailList='" + CCMailList + "'";
                    updatesql += ",Budget=" + budget;
                    updatesql += ",Station2='" + ostation + "'";
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
                    }
                    else
                    {
                        if (!SaveDetail(detail, CurLocal, ostation, CurBudget))
                        {
                            ErrorHandle("Data Error.");
                            return;
                        }
                        if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                        {
                            UpdateMSG("保存通用申请单草稿: " + hdTravelRequestNo.Value.ToString() + "成功.<br />请核对后点击申请按钮.");
                        }
                        else
                        {
                            UpdateMSG("Saved General Expense Draft: " + hdTravelRequestNo.Value.ToString() + " successfully.<br />Please check up and click Apply button.");
                        }

                    }
                }
                else//如果ID为空则判断为新增草稿
                {
                    string word = "[No],[Person],[PersonID],[Department],[Station],[ApplyDate],[Remark],[Tamount],[Type],[CCMailList],[Budget],[Station2]";
                    //160123 垫付
                    if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                    { word += ",OnBehalfPersonID,OnBehalfPersonName,OnBehalfPersonUnit,OnBehalfPersonDept,OnBehalfPersonCostCenter"; }
                    string value = "";
                    value += "'" + station_applyperson + DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString().PadLeft(2, '0') + "',";//edit
                    value += "'" + X.GetValue("cbxPerson") + "',";
                    value += "'" + cbxPerson.Text + "',";
                    value += "'" + dept_applyperson + "',";
                    value += "'" + station_applyperson + "',";
                    value += "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',";
                    value += "'" + txtRemark.Text.Replace("'", "''") + "',";
                    value += hdSum.Value.ToString() == "" ? "null," : hdSum.Value.ToString() + ",";
                    value += "1,";
                    value += "'" + CCMailList + "'";
                    value += "," + budget;
                    value += ",'" + ostation + "'";
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

                    string updatesql = "insert into Ecommon (" + word + ") values(" + value + ");update Ecommon set [No]=[No]+(select [MonthCount]=right('0000'+cast(count(ID) as varchar(10)),4) from Ecommon where (month(ApplyDate) in (select month(ApplyDate) from Ecommon where [ID]=@@IDENTITY) and (year(ApplyDate) in (select year(ApplyDate) from Ecommon where [ID]=@@IDENTITY)) and Station=(select Station from Ecommon where ID=@@IDENTITY)))+'GD' where ID=@@IDENTITY;select [msg]=convert(varchar,ID)+','+[No] from Ecommon where ID=@@IDENTITY";

                    string newid = dbc.UpdateData("eReimbursement", updatesql, "Insert");

                    //操作Flow表
                    string wordflow = "[RequestID],[Type],[Active]";
                    string valueflow = "";
                    valueflow += "'" + newid.Split(',')[0] + "',";
                    valueflow += "'G',";
                    valueflow += "1";

                    string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";
                    string rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
                    //
                    if (newid == "-1" || rows == "-1")
                    {
                        ErrorHandle("Data Error.");
                    }
                    else
                    {
                        hdTravelRequestID.Value = newid.Split(',')[0];//新增后记录ID
                        hdTravelRequestNo.Value = newid.Split(',')[1];//新增后记录No

                        if (!SaveDetail(detail, CurLocal, ostation, CurBudget))
                        {
                            ErrorHandle("Data Error.");
                            return;
                        }
                        if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                        {
                            Panel3.Title = "通用申请单草稿: " + newid.Split(',')[1];
                            UpdateMSG("新增通用申请单草稿: " + newid.Split(',')[1] + "成功.<br />请核对后点击申请按钮.");
                        }
                        else
                        {
                            Panel3.Title = "General Expense Draft: " + newid.Split(',')[1];
                            UpdateMSG("Added General Expense Draft: " + newid.Split(',')[1] + " successfully.<br />Please check up and click Apply button.");
                        }

                    }
                }
            }
            X.AddScript("cbxPerson.disable();cbxOnBehalfName.disable();");
        }
        [DirectMethod]
        public void SaveAll(string type, string detail, string MailList)
        {
            if (Request.Cookies.Get("eReimUserID") == null || hdUser.Value.ToString() != Request.Cookies.Get("eReimUserID").Value)
            {
                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                {
                    X.AddScript("Ext.Msg.show({ title: '提示', msg: '登录超时或已切换用户,将刷新页面.', buttons: { ok: 'Ok' }, fn: function (btn) { window.location.reload(); } });");
                }
                else
                {
                    X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Time out or user changed,reloading...', buttons: { ok: 'Ok' }, fn: function (btn) { window.location.reload(); } });");
                }
                return;
            }
            //页面权限
            //DateTime dtnull = new DateTime(1, 1, 1, 0, 0, 0);
            cs.DBCommand dbc = new cs.DBCommand();
            //string userid = cbxPerson.Value.ToString();
            //160113 垫付人
            string userid = (hdOnBehalf.Value == null || hdOnBehalf.Value.ToString() == "") ? cbxPerson.Value.ToString() : hdOnBehalf.Value.ToString();
            string ostation = "";
            string para = type;
            string station = ""; string department = "";

            string station_applyperson = ""; string costcenter_applyperson = ""; string dept_applyperson = "";
            DataSet ds_apply = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(cbxPerson.Value.ToString());
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
                station = dt1.Rows[0]["stationCode"].ToString();
                ostation = dt1.Rows[0]["CostCenter"].ToString();
                department = dt1.Rows[0]["CRPDepartmentName"].ToString();
            }

            decimal rate = 1;//记录用户币种与预算站点币种汇率
            string CurLocal = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(costcenter_applyperson);
            //检查是否本地维护过特殊币种
            DataTable dttemp = new DataTable();
            string sqltemp = "select * from ESUSER where Userid='" + cbxPerson.Value.ToString() + "'";
            dttemp = dbc.GetData("eReimbursement", sqltemp);
            if (dttemp.Rows.Count > 0)
            {
                CurLocal = dttemp.Rows[0]["Currency"].ToString();//如果单独设置了币种
            }
            string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(ostation);
            //预算
            DataTable dtbudget = new DataTable();
            dtbudget.Columns.Add("Year", typeof(System.String));//区分跨年情况
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

            //StoreBudget添加Field
            //StoreBudget.Reader[0].Fields.Clear();
            StoreBudget.Reader[0].Fields.Add("Year", RecordFieldType.String);
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

            //基础数据
            DataTable dtA = new DataTable();
            dtA.Columns.Add("EName", typeof(System.String));
            dtA.Columns.Add("COACode", typeof(System.String));
            dtA.Columns.Add("Amount", typeof(System.Decimal));
            dtA.Columns.Add("Year", typeof(System.String));

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<DetailExpense> Details = ser.Deserialize<List<DetailExpense>>(detail);
            if (Details.Count < 1)
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
            foreach (DetailExpense details in Details)
            {
                DataRow dr = dtA.NewRow();
                string EName = "";
                if (details.Type == "E")
                {
                    EName = "Entertainment";
                }
                else if (details.Type == "T")
                {
                    EName = "Transportation";
                }
                else if (details.Type == "C")
                {
                    EName = "Communication";
                }
                else
                {
                    EName = details.COAName;
                }
                dr["EName"] = EName;
                dr["COACode"] = details.AccountCode;
                dr["Amount"] = Convert.ToDecimal(details.Amount);
                if (details.Type == "C")
                {
                    dr["Year"] = Convert.ToDateTime(details.EffectTime).Year.ToString();
                }
                else
                {
                    dr["Year"] = Convert.ToDateTime(details.Tdate).Year.ToString();
                }
                dtA.Rows.Add(dr);
            }
            //合计数据
            DataTable dtB = new DataTable();
            dtB.Columns.Add("EName", typeof(System.String));
            dtB.Columns.Add("COACode", typeof(System.String));
            dtB.Columns.Add("Amount", typeof(System.Decimal));
            dtB.Columns.Add("Year", typeof(System.String));
            for (int i = 0; i < dtA.Rows.Count; i++)
            {
                bool er = false;
                for (int j = 0; j < dtB.Rows.Count; j++)
                {
                    if (dtB.Rows[j]["COACode"].ToString() == dtA.Rows[i]["COACode"].ToString() && dtB.Rows[j]["Year"].ToString() == dtA.Rows[i]["Year"].ToString())//已有记录
                    {
                        er = true;
                        break;
                    }
                }
                if (!er)//不存在重复记录
                {
                    DataRow dr = dtB.NewRow();
                    dr["EName"] = dtA.Rows[i]["EName"].ToString();
                    dr["COACode"] = dtA.Rows[i]["COACode"].ToString();
                    dr["Amount"] = dtA.Compute("Sum(Amount)", "Year = " + dtA.Rows[i]["Year"].ToString() + " and COACode = " + dtA.Rows[i]["COACode"].ToString());
                    dr["Year"] = dtA.Rows[i]["Year"].ToString();
                    dtB.Rows.Add(dr);
                }
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

                            //if (Convert.ToDecimal(dtC.Rows[i]["Budget"].ToString())==0)
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
                    //14/10/24
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
                if (CurLocal != CurBudget)
                {
                    rate = DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(CurLocal, CurBudget, Convert.ToInt16(dtbudget.Rows[i]["Year"].ToString()));
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
                dtbudget.Rows[i]["PU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["PU"].ToString()), 2);
                dtbudget.Rows[i]["PB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()), 2);
                dtbudget.Rows[i]["DU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["DU"].ToString()), 2);
                dtbudget.Rows[i]["DB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()), 2);
                dtbudget.Rows[i]["SU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["SU"].ToString()), 2);
                dtbudget.Rows[i]["SB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()), 2);
            }
            //160113 垫付人
            if (hdOnBehalf.Value == null || hdOnBehalf.Value.ToString()=="")
            {
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
                    Width = 160
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
                cm.Columns.Add(new Column
                {
                    DataIndex = "Year",
                    Header = "Budget Year",
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
            
            //return;
            //处理抄送人列表
            string CCMailList = "";
            JavaScriptSerializer serd = new JavaScriptSerializer();
            List<CCMailList> CCMailList1 = serd.Deserialize<List<CCMailList>>(MailList);
            foreach (CCMailList mail in CCMailList1)
            {
                CCMailList += mail.Email + ",";
            }
            CCMailList = CCMailList.Length > 0 ? CCMailList.Substring(0, CCMailList.Length - 1) : "";

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
                        //输出预算内容
                        //DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();

                        //mail.FromDispName = "eReimbursement";
                        //mail.From = "DIC2@dimerco.com";
                        //mail.To = "Andy_Kang@dimerco.com";
                        //mail.Title = "eReimbursement Bug" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:dd");
                        //string body = "<div>" + Request.Cookies.Get("eReimUserID").Value + "<br/>Common<br/><table>";
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
                    {
                        //160224 如果为垫付,则直接保存不提示预算情况
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
                        //输出预算内容
                        //DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();

                        //mail.FromDispName = "eReimbursement";
                        //mail.From = "DIC2@dimerco.com";
                        //mail.To = "Andy_Kang@dimerco.com";
                        //mail.Title = "eReimbursement Bug" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:dd");
                        //string body = "<div>" + Request.Cookies.Get("eReimUserID").Value + "<br/>Common<br/><table>";
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
                return;
            }
            else//全在预算内,按预算内申请流程处理
            {
                //判断是否设置了审批流程
                string sqlCheckFlow = ""; DataTable dtGroupFlowData = new DataTable();

                if (hdOnBehalf.Value == null || hdOnBehalf.Value.ToString() == "")
                {
                    //sqlCheckFlow = "select * from GroupFlow where [Type]!=2 and GID=(select GID from GroupUsers where UserID='" + cbxPerson.Value.ToString() + "')";
                    sqlCheckFlow = "select * from GroupFlow where [Type]!=2 and GID=(select GID from GroupUsers where UserID='" + cbxPerson.Value.ToString() + "') order by FlowNo";
                    dtGroupFlowData = dbc.GetData("eReimbursement", sqlCheckFlow);
                    if (dtGroupFlowData.Rows.Count < 1)
                    {
                        if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                        {
                            ErrorHandleNojump("请先设置审批人.");
                        }
                        else
                        {
                            ErrorHandleNojump("Not set Approve flow,please contact with Local MIS.");
                        }
                        return;
                    }
                }
                else//160113 垫付审批流程
                {
                    sqlCheckFlow = "select UserID,t1.* from (select * from GroupFlow where [Type]!=2 and GID in (select GID from GroupUsers where UserID='" + cbxPerson.Value.ToString() + "' or UserID='" + hdOnBehalf.Value.ToString() + "')) t1 left join (select * from GroupUsers where UserID='" + cbxPerson.Value.ToString() + "' or UserID='" + hdOnBehalf.Value.ToString() + "') t2 on t2.Gid=t1.Gid order by Gid,FlowNo";
                    //sqlCheckFlow = "select * from GroupFlow where [Type]!=2 and GID in (select GID from GroupUsers where UserID='" + cbxPerson.Value.ToString() + "' or UserID='"+hdOnBehalf.Value.ToString()+"') order by Gid,FlowNo";
                    dtGroupFlowData = dbc.GetData("eReimbursement", sqlCheckFlow);
                    if (dtGroupFlowData.Rows.Count < 1)
                    {
                        if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                        {
                            ErrorHandleNojump("请先设置审批人.");
                        }
                        else
                        {
                            ErrorHandleNojump("Not set Approve flow,please contact with Local MIS.");
                        }
                        return;
                    }
                }
                if (type == "ND")//保存并申请
                {
                    if (hdTravelRequestID.Value.ToString() != "")
                    {
                        if (LabelMonth.Text != DateTime.Now.Month.ToString() && DateTime.Now.Day > 5)
                        {
                            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                            {
                                X.AddScript("Ext.Msg.show({ title: '提示', msg: '草稿已过期,请新增本月草稿.', buttons: { ok: 'Ok' }, fn: function (btn) { return false; } });");
                            }
                            else
                            {
                                X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Draft expired,please add new draft.', buttons: { ok: 'Ok' }, fn: function (btn) { return false; } });");
                            }
                            return;
                        }
                        string oldno = hdTravelRequestNo.Value.ToString();
                        string newno = hdTravelRequestNo.Value.ToString().Substring(0, hdTravelRequestNo.Value.ToString().Length - 1);
                        string updatesql = "update Ecommon set [No]='" + newno + "',";
                        updatesql += "Person='" + X.GetValue("cbxPerson") + "',";
                        updatesql += "PersonID='" + cbxPerson.Text + "',";
                        updatesql += "Department='" + dept_applyperson + "',";
                        updatesql += "Station='" + station_applyperson + "',";
                        updatesql += "Tamount=" + (hdSum.Value.ToString() == "" ? "null," : ("'" + hdSum.Value.ToString() + "',"));
                        updatesql += "[Type]=0,";
                        updatesql += "CreadedBy='" + Request.Cookies.Get("eReimUserName").Value + "',";
                        updatesql += "CreatedByID='" + Request.Cookies.Get("eReimUserID").Value + "',";
                        updatesql += "CreadedDate='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',";
                        updatesql += "Remark='" + txtRemark.Text.Replace("'", "''") + "',";
                        updatesql += "CCMailList='" + CCMailList + "'";
                        updatesql += ",[Budget]=1";
                        updatesql += ",Station2='" + ostation + "'";
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
                        string sqlDeleteEflow = "delete from Eflow where [Type]='G' and [RequestID]='" + hdTravelRequestID.Value.ToString() + "'";
                        string deleterows = dbc.UpdateData("eReimbursement", sqlDeleteEflow, "Update");
                        string rows = "";

                        //160113 垫付
                        if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                        {
                            string personid = cbxPerson.Value.ToString();
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
                                valueflow += "'G',";
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
                                valueflow += "'G',";
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
                        
                        if (newid == "-1" || rows == "-1" || rows == "")
                        {
                            ErrorHandle("Data Error");
                        }
                        else
                        {
                            hdTravelRequestNo.Value = newno;
                            // || !SendMail(hdTravelRequestID.Value.ToString())
                            if (!SaveDetail(detail, CurLocal, ostation, CurBudget) || !SendMailNew(dtbudget))//Budget未计入Current,%需重新计算
                            {
                                ErrorHandle("Data Error.");
                                return;
                            }
                            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                            {
                                Panel3.Title = "通用申请单:" + newno;
                                UpdateMSG("保存通用申请单:" + hdTravelRequestNo.Value.ToString() + "成功.");
                            }
                            else
                            {
                                Panel3.Title = "General Expense Form: " + newno;
                                UpdateMSG("Saved General Expense Form: " + hdTravelRequestNo.Value.ToString() + " successfully.");
                            }
                        }
                    }
                    else//直接申请
                    {
                        string word = "[No],[Person],[PersonID],[Department],[Station],[ApplyDate],[Remark],[Tamount],[CreadedBy],[CreatedByID],[CreadedDate],[CCMailList],[Budget],[Station2]";
                        //160123 垫付
                        if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                        { word += ",OnBehalfPersonID,OnBehalfPersonName,OnBehalfPersonUnit,OnBehalfPersonDept,OnBehalfPersonCostCenter"; }
                        string value = "";
                        value += "'" + station_applyperson + DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString().PadLeft(2, '0') + "',";//edit
                        value += "'" + X.GetValue("cbxPerson") + "',";
                        value += "'" + cbxPerson.Text + "',";
                        value += "'" + dept_applyperson + "',";
                        value += "'" + station_applyperson + "',";
                        value += "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',";
                        value += "'" + txtRemark.Text.Replace("'", "''") + "',";
                        value += hdSum.Value.ToString() == "" ? "null," : hdSum.Value.ToString() + ",";
                        value += "'" + Request.Cookies.Get("eReimUserName").Value + "',";//session
                        value += "'" + Request.Cookies.Get("eReimUserID").Value + "',";//session
                        value += "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'";
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

                        string updatesql = "insert into Ecommon (" + word + ") values(" + value + ");update Ecommon set [No]=[No]+(select [MonthCount]=right('0000'+cast(count(ID) as varchar(10)),4) from Ecommon where (month(ApplyDate) in (select month(ApplyDate) from Ecommon where [ID]=@@IDENTITY) and (year(ApplyDate) in (select year(ApplyDate) from Ecommon where [ID]=@@IDENTITY)) and Station=(select Station from Ecommon where ID=@@IDENTITY)))+'G' where ID=@@IDENTITY;select [msg]=convert(varchar,ID)+','+[No] from Ecommon where ID=@@IDENTITY";

                        string newid = dbc.UpdateData("eReimbursement", updatesql, "Insert");

                        string rows = "";

                        //160113 垫付
                        if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                        {
                            string personid = cbxPerson.Value.ToString();
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
                                valueflow += "'G',";
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
                                valueflow += "'G',";
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
                            ErrorHandle("Data Error");
                        }
                        else
                        {
                            hdTravelRequestID.Value = newid.Split(',')[0];//新增后记录ID
                            hdTravelRequestNo.Value = newid.Split(',')[1];//新增后记录No
                            if (!SaveDetail(detail, CurLocal, ostation, CurBudget) || !SendMailNew(dtbudget))
                            {
                                ErrorHandle("Data Error.");
                                return;
                            }
                            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                            {
                                Panel3.Title = "通用申请单:" + newid.Split(',')[1];
                                UpdateMSG("保存通用申请单:" + hdTravelRequestNo.Value.ToString() + "成功.");
                            }
                            else
                            {
                                Panel3.Title = "General Expense Form: " + newid.Split(',')[1];
                                UpdateMSG("Saved General Expense Form: " + hdTravelRequestNo.Value.ToString() + " successfully.");
                            }
                        }
                    }
                    X.AddScript("btnSaveDraft.disable();btnSaveAndSend.disable();btnExport.enable();btnCC.disable();btnE.disable();btnT.disable();btnC.disable();btnO.disable();");
                }
                else//保存草稿信息时不保存Budget状态,仅正式申请时保存
                {
                    if (hdTravelRequestID.Value.ToString() != "")//由链接进入的草稿更新
                    {
                        if (LabelMonth.Text != DateTime.Now.Month.ToString() && DateTime.Now.Day > 5)//检查日期是否超期
                        {
                            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                            {
                                X.AddScript("Ext.Msg.show({ title: '提示', msg: '草稿已过期,请新增本月草稿.', buttons: { ok: 'Ok' }, fn: function (btn) { return false; } });");
                            }
                            else
                            {
                                X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Draft expired,please add new draft.', buttons: { ok: 'Ok' }, fn: function (btn) { return false; } });");
                            }
                            return;
                        }

                        string updatesql = "update Ecommon set [Person]='" + X.GetValue("cbxPerson") + "',";
                        updatesql += "[Station]='" + station_applyperson + "',";
                        updatesql += "[Department]='" + dept_applyperson + "',";
                        updatesql += "[Tamount]=" + (hdSum.Value.ToString() == "" ? "null," : hdSum.Value.ToString()) + ",";
                        updatesql += "[Remark]='" + txtRemark.Text.Replace("'", "''") + "',";
                        updatesql += "[PersonID]='" + cbxPerson.Text + "',";
                        updatesql += "CCMailList='" + CCMailList + "'";
                        updatesql += ",Budget=1";
                        updatesql += ",Station2='" + ostation + "'";
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
                        }
                        else
                        {
                            if (!SaveDetail(detail, CurLocal, ostation, CurBudget))
                            {
                                ErrorHandle("Data Error.");
                                return;
                            }
                            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                            {
                                UpdateMSG("保存通用申请单草稿: " + hdTravelRequestNo.Value.ToString() + "成功.<br />请核对后点击申请按钮.");
                            }
                            else
                            {
                                UpdateMSG("Saved General Expense Draft: " + hdTravelRequestNo.Value.ToString() + " successfully.<br />Please check up and click Apply button.");
                            }

                        }
                    }
                    else//如果ID为空则判断为新增草稿
                    {
                        string word = "[No],[Person],[PersonID],[Department],[Station],[ApplyDate],[Remark],[Tamount],[Type],[CCMailList],[Budget],[Station2]";
                        //160115 垫付
                        if (hdOnBehalf.Value != null && hdOnBehalf.Value.ToString() != "")
                        { word += ",OnBehalfPersonID,OnBehalfPersonName,OnBehalfPersonUnit,OnBehalfPersonDept,OnBehalfPersonCostCenter"; }
					
                        string value = "";
                        value += "'" + station_applyperson + DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString().PadLeft(2, '0') + "',";//edit
                        value += "'" + X.GetValue("cbxPerson") + "',";
                        value += "'" + cbxPerson.Text + "',";
                        value += "'" + dept_applyperson + "',";
                        value += "'" + station_applyperson + "',";
                        value += "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',";
                        value += "'" + txtRemark.Text.Replace("'", "''") + "',";
                        value += hdSum.Value.ToString() == "" ? "null," : hdSum.Value.ToString() + ",";
                        value += "1,";
                        value += "'" + CCMailList + "'";
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

                        string updatesql = "insert into Ecommon (" + word + ") values(" + value + ");update Ecommon set [No]=[No]+(select [MonthCount]=right('0000'+cast(count(ID) as varchar(10)),4) from Ecommon where (month(ApplyDate) in (select month(ApplyDate) from Ecommon where [ID]=@@IDENTITY) and (year(ApplyDate) in (select year(ApplyDate) from Ecommon where [ID]=@@IDENTITY)) and Station=(select Station from Ecommon where ID=@@IDENTITY)))+'GD' where ID=@@IDENTITY;select [msg]=convert(varchar,ID)+','+[No] from Ecommon where ID=@@IDENTITY";

                        string newid = dbc.UpdateData("eReimbursement", updatesql, "Insert");

                        //操作Flow表
                        string wordflow = "[RequestID],[Type],[Active]";
                        string valueflow = "";
                        valueflow += "'" + newid.Split(',')[0] + "',";
                        valueflow += "'G',";
                        valueflow += "1";

                        string sqlupdateEFlow = "insert into Eflow (" + wordflow + ") values(" + valueflow + ")";
                        string rows = dbc.UpdateData("eReimbursement", sqlupdateEFlow, "Update");
                        //
                        if (newid == "-1" || rows == "-1")
                        {
                            ErrorHandle("Data Error.");
                        }
                        else
                        {
                            hdTravelRequestID.Value = newid.Split(',')[0];//新增后记录ID
                            hdTravelRequestNo.Value = newid.Split(',')[1];//新增后记录No

                            if (!SaveDetail(detail, CurLocal, ostation, CurBudget))
                            {
                                ErrorHandle("Data Error.");
                                return;
                            }
                            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                            {
                                Panel3.Title = "通用申请单草稿: " + newid.Split(',')[1];
                                UpdateMSG("新增通用申请单草稿: " + newid.Split(',')[1] + "成功.<br />请核对后点击申请按钮.");
                            }
                            else
                            {
                                Panel3.Title = "General Expense Draft: " + newid.Split(',')[1];
                                UpdateMSG("Added General Expense Draft: " + newid.Split(',')[1] + " successfully.<br />Please check up and click Apply button.");
                            }

                        }
                    }
                }
            }
            X.AddScript("cbxPerson.disable();cbxOnBehalfName.disable();");
        }
        protected bool SendMailNew(DataTable dtPar)
        {
            //发送提醒邮件
            cs.DBCommand dbc = new cs.DBCommand();
            string sql = "select * from V_Eflow_ETravel where [Type]='G' and Step!=0 and RequestID=" + hdTravelRequestID.Value.ToString() + " order by Step,FlowID";
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
                if (dsTo != null && dsTo.Tables.Count >= 1 && dsTo.Tables[0].Rows.Count == 1)
                {
                    mailto += dsTo.Tables[0].Rows[0]["eMail"].ToString() + ",";
                }
                //else
                //{
                //    ErrorHandle("Error mail address."); return false;
                //}
                //DataSet dsCC1 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList("A0001");
                //DataTable dt13 = dsCC1.Tables[0];
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
                mail.To = mailto;
                mail.Cc = mailcc;



                string divstyle = "style='font-size:small;'";
                string divstyleCurrent = "style='font-size:small;color:blue;'";
                string divstyleR = "style='font-size:small;color:red;'";
                string divstyleReject = "style='font-size:small;color:red;'";
                string divstylered = "style='border:silver 1px ridge; font-size:small;background-color: #FFFFFF;empty-cells:show;color:red;' width='110px' align='right'";
                string tdstyle = "style='border:silver 1px ridge; font-size:small;background-color: #FFFFFF;empty-cells:show;' width='110px'align='right'";
                StringBuilder sb = new StringBuilder();


                //160127 Shanshan提出邮件测试
                //sb.Append("<div " + divstyleReject + ">THIS IS A TEST MAIL." + mailtestword + "</div><br />");
                //sb.Append("<div>");



                sb.Append("<div " + divstyle + "> Dear " + dtMail.Rows[0]["Approver"].ToString() + ",</div><br />");
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
                //sb.Append("<div style='font-size:small;'><div style='float:left'>No#:" + dtMail.Rows[0]["No"].ToString() + "</div><a style='color:Red;'>" + budget + "</a></div>");
                //sb.Append("<div style='font-size:small;'><a>No#:" + dtMail.Rows[0]["No"].ToString() + "</a><a style='color:Red;'>" + budget + "</a></div>");
                //sb.Append("<div><table><tr style='font-size:small;'><td width='200px'>No#:" + dtMail.Rows[0]["No"].ToString() + "</td><td style='color:Red;'>" + budget + "</td></tr></table></div>");
                //sb.Append("<div><div style='font-size:small;float: left;'>No#:" + dtMail.Rows[0]["No"].ToString() + "</div><div style='font-size:small;color:red;'>" + budget + "</div></div>");
                sb.Append("<div " + divstyle + ">Applicant:" + dtMail.Rows[0]["Person"].ToString() + "</div>");
                sb.Append("<div " + divstyle + ">Unit:" + dtMail.Rows[0]["Station"].ToString() + "</div>");
                sb.Append("<div " + divstyle + ">Department:" + dtMail.Rows[0]["Department"].ToString() + "</div><br />");
                //140312
                //如果只有一个年份,则不显示Year列
                DataTable dtpar1 = dtPar.DefaultView.ToTable(true, "Year");
                bool YearOrNot = dtpar1.Rows.Count == 1 ? false : true;
                //160115 垫付
                if (hdOnBehalf.Value == null || hdOnBehalf.Value.ToString() == "")
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
                                dr["Currency"] = dtPar.Rows[i]["Currency"].ToString();
                                dr["Current"] = dtPar.Rows[i]["Current"];
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
                                dr["Year"] = dtPar.Rows[i]["Year"].ToString();
                                dr["EName"] = dtPar.Rows[i]["EName"].ToString();
                                dr["Currency"] = dtPar.Rows[i]["Currency"].ToString();
                                dr["Current"] = dtPar.Rows[i]["Current"];
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
                                dr["Year"] = dtPar.Rows[i]["Year"].ToString();
                                dr["EName"] = dtPar.Rows[i]["EName"].ToString();
                                dr["Currency"] = dtPar.Rows[i]["Currency"].ToString();
                                dr["Current"] = dtPar.Rows[i]["Current"];
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
                                sb.Append("<td " + tdstyle + ">" + dtStationNew.Rows[i]["Currency"].ToString() + "</td>");
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
                            else
                            {
                                sb.Append("<tr><td " + tdstyle + ">" + dtStationNew.Rows[i]["EName"].ToString() + "</td>");
                                sb.Append("<td " + tdstyle + ">" + dtStationNew.Rows[i]["Currency"].ToString() + "</td>");
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
                                sb.Append("<td " + tdstyle + ">" + dtStationNew.Rows[i]["Currency"].ToString() + "</td>");
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
        protected bool SendMail(string RequestID)
        {
            //发送提醒邮件
            cs.DBCommand dbc = new cs.DBCommand();
            string sql = "select * from V_Eflow_ETravel where [Type]='G' and Step!=0 and RequestID=" + RequestID + " order by Step,FlowID";
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
                mail.Title = "Dimerco eReimbursement "+budget+" " + dtMail.Rows[0]["Person"].ToString() + " - "+msg;
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
                string tdstyle = "style='border:silver 1px ridge; font-size:small;background-color: #FFFFFF;empty-cells:show;'";
                StringBuilder sb = new StringBuilder();
                sb.Append("<div>");
                sb.Append("<div " + divstyle + "> Dear " + dtMail.Rows[0]["Approver"].ToString() + ",</div><br />");
                sb.Append("<div " + divstyle + ">The following eReimbursement application: "+msg+"</div><br /><br />");
                
                sb.Append("<div " + divstyle + ">No#:" + dtMail.Rows[0]["No"].ToString() + budget + "</div>");
                sb.Append("<div " + divstyle + ">Owner:" + dtMail.Rows[0]["Person"].ToString() + "</div>");
                sb.Append("<div " + divstyle + ">Station:" + dtMail.Rows[0]["Station"].ToString() + "</div>");
                sb.Append("<div " + divstyle + ">Department:" + dtMail.Rows[0]["Department"].ToString() + "</div><br />");
                //string period = "";
                //period += dtMail.Rows[0]["Bdate"].ToString() == "" ? "From NA " : ("From " + Convert.ToDateTime(dtMail.Rows[0]["Bdate"].ToString()).ToString("yyyy/MM/dd") + " ");
                //period += dtMail.Rows[0]["Edate"].ToString() == "" ? "To NA" : "To " + Convert.ToDateTime(dtMail.Rows[0]["Edate"].ToString()).ToString("yyyy/MM/dd");
                //sb.Append("<div " + divstyle + ">Period:" + period + "</div><br />");
                sb.Append("<div><table style='border-collapse:collapse'><thead><tr><th colspan=\"14\" " + tdstyle + ">Expense Detail</th></tr><tr>");
                sb.Append("<th " + tdstyle + "></th>");
                sb.Append("<th " + tdstyle + ">Type</th>");
                sb.Append("<th " + tdstyle + ">Date</th>");
                sb.Append("<th " + tdstyle + ">Expense Type</th>");
                sb.Append("<th " + tdstyle + ">Currency</th>");
                sb.Append("<th " + tdstyle + ">Amounts</th>");
                sb.Append("<th " + tdstyle + ">Cost Center</th>");
                sb.Append("<th style='border:silver 1px ridge; font-size:small;background-color: #FFFFFF;empty-cells:show;width:160px;'>Remark</th>");
                sb.Append("<th " + tdstyle + ">Station Budget MTD:(Used/All)</th>");
                sb.Append("<th " + tdstyle + ">Dept. Budget MTD:(Used/All)</th>");
                sb.Append("<th " + tdstyle + ">Person Budget MTD:(Used/All)</th>");
                sb.Append("<th " + tdstyle + ">Station Budget YTD:(Used/All)</th>");
                sb.Append("<th " + tdstyle + ">Dept. Budget YTD:(Used/All)</th>");
                sb.Append("<th " + tdstyle + ">Person Budget YTD:(Used/All)</th></tr></thead>");

                sb.Append("<tbody>");
                decimal ptotal = 0;
                string sqldetail = "select [StationBudget]='',[DepartmentBudget]='',[PersonBudget]='',[StationYTD]='',[DepartmentYTD]='',[PersonYTD]='',t3.EText,case when t1.Type='O' then t2.SAccountName else '' end as SAccountName,t1.* from EeommonDetail t1 left join AccoundCode t2 on t2.SAccountCode=t1.AccountCode left join (select * from Edic where KeyValue='SubType') t3 on t3.CValue=t1.[Type] where t1.[No]='" + RequestID + "'";
                //string sqldetail = "select t2.SAccountName,t1.* from ETraveleDetail t1 left join AccoundCode t2 on t2.SAccountCode=t1.AccountCode where t1.[No]='" + hdTravelRequestID.Value.ToString() + "'";
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
                    //string accountcode = dtdetail.Rows[i]["AccountCode"].ToString();
                    string accountcode = "";
                    if (dtdetail.Rows[i]["Type"].ToString() == "O")
                    {
                        string newr = "select BAccountCode from AccoundCode where SAccountCode='" + dtdetail.Rows[i]["AccountCode"].ToString() + "'";
                        DataTable den = new DataTable();
                        den = dbc.GetData("eReimbursement", newr);
                        accountcode = den.Rows[0]["BAccountCode"].ToString();
                    }
                    else
                    {
                        accountcode = dtdetail.Rows[i]["AccountCode"].ToString();
                    }
                    string Years = "";
                    string month = "";

                    if (dtdetail.Rows[i]["Type"].ToString() == "C")
                    {
                        if (dtdetail.Rows[i]["EffectTime"].ToString() != "")
                        {
                            Years = Convert.ToDateTime(dtdetail.Rows[i]["EffectTime"].ToString()).Year.ToString();
                            month = Convert.ToDateTime(dtdetail.Rows[i]["EffectTime"].ToString()).Month.ToString();
                        }
                    }
                    else
                    {
                        if (dtdetail.Rows[i]["Tdate"].ToString() != "")
                        {

                            Years = Convert.ToDateTime(dtdetail.Rows[i]["Tdate"].ToString()).Year.ToString();
                            month = Convert.ToDateTime(dtdetail.Rows[i]["Tdate"].ToString()).Month.ToString();
                        }
                    }
                    if (Years != "" && month != "")
                    {
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
                    }

                    sb.Append("<tr><th " + tdstyle + ">" + (i + 1).ToString() + "</th>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["EText"].ToString() + "</td>");
                    string tdate = dtdetail.Rows[i]["Tdate"].ToString() == "" ? "" : Convert.ToDateTime(dtdetail.Rows[i]["Tdate"].ToString()).ToString("yyyy/MM/dd");
                    sb.Append("<td " + tdstyle + ">" + tdate + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["SAccountName"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["Cur"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + ">" + (dtdetail.Rows[i]["Amount"].ToString() == "" ? "" : Convert.ToDecimal(dtdetail.Rows[i]["Amount"].ToString()).ToString("#,##0.00")) + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["TSation"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["AccountDes"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["StationBudget"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["DepartmentBudget"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["PersonBudget"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["StationYTD"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["DepartmentYTD"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["PersonYTD"].ToString() + "</td></tr>");
                    ptotal += dtdetail.Rows[i]["Amount"].ToString() == "" ? 0 : Convert.ToDecimal(dtdetail.Rows[i]["Amount"].ToString());
                }
                sb.Append("</tbody>");
                sb.Append("<tfoot><tr>");
                sb.Append("<th " + tdstyle + ">Total:</th>");
                sb.Append("<td colspan=\"3\" " + tdstyle + "></td>");
                sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[0]["Cur"].ToString() + "</td>");
                sb.Append("<td " + tdstyle + ">" + ptotal.ToString("#,##0.00") + "</td>");
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
                        msg1 = ". Waiting for Approval: "+ dtMail.Rows[i]["Approver"].ToString() + "</div>";
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
            //检查是否已经为该申请人设置过审批人
            string sqlCheckFlow = "";
            if(Radio1.Checked)
            //if (cbxBudget.Value.ToString() == "YES")//使用Budget审批流程
            {
                sqlCheckFlow = "select * from GroupFlow where [Type]!=2 and GID=(select GID from GroupUsers where UserID='" + cbxPerson.Value.ToString() + "')";
            }
            else//使用unBudget审批流程
            {
                sqlCheckFlow = "select * from GroupFlow where [Type]=2 and GID=(select GID from GroupUsers where UserID='" + cbxPerson.Value.ToString() + "')";
            }

            //string sqlCheckFlow = "select * from GroupFlow where GID=(select GID from GroupUsers where UserID='" + cbxOwner.Text + "')";
            DataTable dtCheckFlow = dbc.GetData("eReimbursement", sqlCheckFlow);
            if (dtCheckFlow.Rows.Count < 1)
            {
                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                {
                    ErrorHandleNojump("请联系Local MIS设置审批人.");
                }
                else
                {
                    ErrorHandleNojump("Not set Approve flow,please contact with Local MIS.");
                }
            }
        }
        protected void ChangePerson(object sender, DirectEventArgs e)
        {
            cs.DBCommand dbc = new cs.DBCommand();
            DataSet ds1 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(cbxPerson.Value.ToString());
            if (ds1.Tables[0].Rows.Count == 1)
            {
                
                DataTable dt1 = ds1.Tables[0];
                string station = "";
                station = dt1.Rows[0]["stationCode"].ToString();
                LabelStation.Text = dt1.Rows[0]["stationCode"].ToString();
                LabelDepartment.Text = dt1.Rows[0]["CRPDepartmentName"].ToString();
                //hdCur.Value = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dt1.Rows[0]["stationCode"].ToString());
                ////切换币种
                //DataTable dttemp = new DataTable();
                //string sqltemp = "select * from ESUSER where Userid='" + cbxPerson.Value.ToString() + "'";
                //dttemp = dbc.GetData("eReimbursement", sqltemp);
                //if (dttemp.Rows.Count > 0)
                //{
                //    station = dttemp.Rows[0]["Station"].ToString();
                //    hdCur.Value = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dttemp.Rows[0]["Station"].ToString());
                //}

                ////取得对美元汇率
                //hdCurrency.Value = System.Math.Round(1 / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(station), 3);
            }
            GridPanel1.Title = "";
            X.AddScript("Store1.removeAll();StoreBudget.removeAll();");
            //检查是否已经为该申请人设置过审批人
            //string sqlCheckFlow = "";
            //if(Radio1.Checked)
            ////if (cbxBudget.Value.ToString() == "YES")//使用Budget审批流程
            //{
            //    sqlCheckFlow = "select * from GroupFlow where [Type]!=2 and GID=(select GID from GroupUsers where UserID='" + cbxPerson.Value.ToString() + "')";
            //}
            //else//使用unBudget审批流程
            //{
            //    sqlCheckFlow = "select * from GroupFlow where [Type]=2 and GID=(select GID from GroupUsers where UserID='" + cbxPerson.Value.ToString() + "')";
            //}

            //DataTable dtCheckFlow = dbc.GetData("eReimbursement", sqlCheckFlow);
            //if (dtCheckFlow.Rows.Count < 1)
            //{
            //    if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
            //    {
            //        ErrorHandle("请联系Local MIS设置审批人.");
            //    }
            //    else
            //    {
            //        ErrorHandle("Not set Approve flow,please contact with Local MIS.");
            //    }
            //}
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
            Store2.DataSource = dtCOACenternew;
            Store2.DataBind();
        }
        protected void ErrorHandleNojump(string msg)
        {
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Message",
                Message = msg,
                Buttons = MessageBox.Button.OK,
                Width = 250,
                Icon = MessageBox.Icon.WARNING
            });
        }
    }
}