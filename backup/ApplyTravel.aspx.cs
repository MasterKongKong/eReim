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
namespace eReimbursement
{
    public partial class ApplyTravel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //DataSet GetCityInfo = DIMERCO.SDK.Utilities.LSDK.GetCityInfo("BJS", 20);
            //int sd = GetCityInfo.Tables[0].Rows.Count;
            //bool isValidCity = DIMERCO.SDK.Utilities.LSDK.isValidCity("BJS");
            //DataSet GetCustomerInfo = DIMERCO.SDK.Utilities.LSDK.GetCustomerInfo("TSOE");
            //DataSet getUserDataBYStationCode = DIMERCO.SDK.Utilities.LSDK.getUserDataBYStationCode("TSOE");
            //DataSet getCostCenterBYStationCode = DIMERCO.SDK.Utilities.LSDK.getCostCenterBYStationCode("BJS",20);
            //DataSet getCRPDepartment = DIMERCO.SDK.Utilities.LSDK.getCRPDepartment();
            //BudgetApplicationSoapClient ds = new BudgetApplicationSoapClient();
            //ServiceReference1.BudgetApplicationSoapClient objService = new ServiceReference1.BudgetApplicationSoapClient("BudgetApplicationSoap", "http://www.baidu.com");
            //DataSet dsss = objService.GetBudgetPackage("ZJDTSN", 2013, "BudgetPackageToken");
            if (!X.IsAjaxRequest)
            {
                //判断登录状态
                cs.DBCommand dbc = new cs.DBCommand();
                if (Session["UserID"] == null)
                {
                    X.AddScript("loginWindow.show();Panel1.disable();");
                    return;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "$('div.gn_person ul.q-menubox li:eq(0) a').text('" + Session["UserName"].ToString() + "');", true); X.AddScript("loginWindow.hide();Panel1.enable();");
                    
                }


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
                            
                            if (Session["UserID"].ToString() == dt.Rows[0]["PersonID"].ToString())//本人
                            {
                                //准备下拉菜单内容
                                Ext.Net.ListItem li = new Ext.Net.ListItem(Session["UserName"].ToString(), Session["UserID"].ToString());
                                cbxOwner.Items.Add(li);
                                string sqlitem = "select * from Eagent where [St]=1 and [PAgentID]='" + Session["UserID"].ToString() + "'";
                                DataTable dtitem = dbc.GetData("eReimbursement", sqlitem);
                                int itemcount = 0;
                                for (int j = 0; j < dtitem.Rows.Count; j++)
                                {
                                    string sqlpara = sqlitem;
                                    if (dtitem.Rows[j]["Bdate"].ToString() != "")
                                    {
                                        sqlpara += " and getdate()>='" + dtitem.Rows[j]["Bdate"].ToString() + "' ";
                                    }
                                    if (dtitem.Rows[j]["Edate"].ToString() != "")
                                    {
                                        sqlpara += " and getdate()<='" + dtitem.Rows[j]["Edate"].ToString() + "' ";
                                    }
                                    DataTable dtitem1 = dbc.GetData("eReimbursement", sqlpara);
                                    for (int m = 0; m < dtitem1.Rows.Count; m++)
                                    {
                                        li = new Ext.Net.ListItem(dtitem.Rows[m]["Owner"].ToString(), dtitem.Rows[m]["OwnerID"].ToString());
                                        cbxOwner.Items.Add(li);
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
                                    else//已申请数据
                                    {
                                        Panel3.Title = "差旅费申请单:" + dt.Rows[0]["No"].ToString();
                                        //读取当前状态,显示在下方文本框内
                                        string app = "";
                                        if (dt.Rows[0]["Status"].ToString() == "1")
                                        {
                                            app += "Applied by: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd h:m");
                                            app += ", Waiting for approval By: " + dt.Rows[0]["Approver"].ToString();
                                        }
                                        else if (dt.Rows[0]["Status"].ToString() == "2")
                                        {
                                            app += "Applied by: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd h:m");
                                            app += ", Approved by: " + dt.Rows[0]["Approver"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["ApproveDate"].ToString()).ToString("yyyy/MM/dd h:m");
                                        }
                                        else if (dt.Rows[0]["Status"].ToString() == "3")
                                        {
                                            app += "Applied by: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd h:m");
                                            app += ", Rejected by: " + dt.Rows[0]["Approver"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["ApproveDate"].ToString()).ToString("yyyy/MM/dd h:m");
                                        }

                                        if (dt.Rows[0]["Active"].ToString() == "2")//已完成
                                        {
                                            app += ". Complete.";
                                            X.AddScript("btnGeteLeave.disable();btnEditDetail.disable();btnNewDetail.disable();btnSaveDraft.disable();btnUploadReport.disable();btnUploadScanFile.disable();");
                                        }
                                        else//待审批
                                        {
                                            X.AddScript("btnGeteLeave.disable();btnEditDetail.disable();btnNewDetail.disable();btnSaveDraft.disable();");
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
                                        Panel3.Title = "差旅费申请单草稿:" + dt.Rows[0]["No"].ToString();
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
                                    if (Session["UserID"].ToString() == dtagent.Rows[g]["PAgentID"].ToString())
                                    {
                                        isagent = true;
                                        break;
                                    }
                                }
                                if (isagent)//代理人访问
                                {
                                    Ext.Net.ListItem li = new Ext.Net.ListItem(dt.Rows[0]["Person"].ToString(), dt.Rows[0]["PersonID"].ToString());
                                    cbxOwner.Items.Add(li);
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
                                            Panel3.Title = "差旅费申请单:" + dt.Rows[0]["No"].ToString();
                                            //读取当前状态,显示在下方文本框内
                                            string app = "";
                                            if (dt.Rows[0]["Status"].ToString() == "1")
                                            {
                                                app += "Applied by: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd h:m");
                                                app += ", Waiting for approval By: " + dt.Rows[0]["Approver"].ToString();
                                            }
                                            else if (dt.Rows[0]["Status"].ToString() == "2")
                                            {
                                                app += "Applied by: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd h:m");
                                                app += ", Approved by: " + dt.Rows[0]["Approver"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["ApproveDate"].ToString()).ToString("yyyy/MM/dd h:m");
                                            }
                                            else if (dt.Rows[0]["Status"].ToString() == "3")
                                            {
                                                app += "Applied by: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd h:m");
                                                app += ", Rejected by: " + dt.Rows[0]["Approver"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["ApproveDate"].ToString()).ToString("yyyy/MM/dd h:m");
                                            }

                                            if (dt.Rows[0]["Active"].ToString() == "2")//已完成
                                            {
                                                app += ". Complete.";
                                                X.AddScript("btnGeteLeave.disable();btnEditDetail.disable();btnNewDetail.disable();btnSaveDraft.disable();btnUploadReport.disable();btnUploadScanFile.disable();");
                                            }
                                            else//待审批
                                            {
                                                X.AddScript("btnGeteLeave.disable();btnEditDetail.disable();btnNewDetail.disable();btnSaveDraft.disable();");
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
                                            Panel3.Title = "差旅费申请单草稿:" + dt.Rows[0]["No"].ToString();
                                            X.AddScript("btnSaveAndSend.enable();");
                                        }
                                    }
                                    //载入通用数据
                                    LoadData(dt, true);
                                }
                                else//判断是否有跨站权限
                                {

                                    bool hasright = false;
                                    string getright = "select * from StationRole where UserID='" + Session["UserID"].ToString() + "'";
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
                                        cbxOwner.Items.Add(li);
                                        //更改按钮状态
                                        if (dt.Rows[0]["Step"].ToString() != "0")//正式申请单
                                        {
                                            Panel3.Title = "差旅费申请单:" + dt.Rows[0]["No"].ToString();
                                            //读取当前状态,显示在下方文本框内
                                            string app = "";
                                            if (dt.Rows[0]["Status"].ToString() == "1")
                                            {
                                                app += "Applied by: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd h:m");
                                                app += ", Waiting for approval By: " + dt.Rows[0]["Approver"].ToString();
                                            }
                                            else if (dt.Rows[0]["Status"].ToString() == "2")
                                            {
                                                app += "Applied by: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd h:m");
                                                app += ", Approved by: " + dt.Rows[0]["Approver"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["ApproveDate"].ToString()).ToString("yyyy/MM/dd h:m");
                                            }
                                            else if (dt.Rows[0]["Status"].ToString() == "3")
                                            {
                                                app += "Applied by: " + dt.Rows[0]["CreadedBy"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd h:m");
                                                app += ", Rejected by: " + dt.Rows[0]["Approver"].ToString() + " " + Convert.ToDateTime(dt.Rows[0]["ApproveDate"].ToString()).ToString("yyyy/MM/dd h:m");
                                            }

                                            if (dt.Rows[0]["Active"].ToString() == "2")//已完成
                                            {
                                                app += ". Complete.";
                                                X.AddScript("btnGeteLeave.disable();btnEditDetail.disable();btnNewDetail.disable();btnSaveDraft.disable();btnUploadReport.disable();btnUploadScanFile.disable();");
                                            }
                                            else//待审批
                                            {
                                                X.AddScript("btnGeteLeave.disable();btnEditDetail.disable();btnNewDetail.disable();btnSaveDraft.disable();");
                                            }
                                            labelInfo.Text = app;
                                        }
                                        else//草稿
                                        {
                                            Panel3.Title = "差旅费申请单草稿:" + dt.Rows[0]["No"].ToString();
                                            X.AddScript("btnSaveAndSend.enable();");
                                        }
                                        //无需判断Copy
                                        //载入通用数据
                                        LoadData(dt, false);
                                    }
                                    else
                                    {
                                        ErrorHandle("无权查看.");
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
                else
                {
                    //准备下拉菜单内容
                    Ext.Net.ListItem li = new Ext.Net.ListItem(Session["UserName"].ToString(), Session["UserID"].ToString());
                    cbxOwner.Items.Add(li);
                    string sqlitem = "select * from Eagent where [St]=1 and [PAgentID]='" + Session["UserID"].ToString() + "'";
                    DataTable dtitem = dbc.GetData("eReimbursement", sqlitem);
                    int itemcount = 0;
                    for (int j = 0; j < dtitem.Rows.Count; j++)
                    {
                        string sqlpara = sqlitem;
                        if (dtitem.Rows[j]["Bdate"].ToString() != "")
                        {
                            sqlpara += " and getdate()>='" + dtitem.Rows[j]["Bdate"].ToString() + "' ";
                        }
                        if (dtitem.Rows[j]["Edate"].ToString() != "")
                        {
                            sqlpara += " and getdate()<='" + dtitem.Rows[j]["Edate"].ToString() + "' ";
                        }
                        DataTable dtitem1 = dbc.GetData("eReimbursement", sqlpara);
                        for (int m = 0; m < dtitem1.Rows.Count; m++)
                        {
                            li = new Ext.Net.ListItem(dtitem.Rows[m]["Owner"].ToString(), dtitem.Rows[m]["OwnerID"].ToString());
                            cbxOwner.Items.Add(li);
                            itemcount++;
                        }
                    }
                    //新增记录时,默认为登录用户
                    cbxOwner.SelectedItem.Value = Session["UserID"].ToString();
                    cbxOwner.SelectedItem.Text = Session["UserName"].ToString();
                    labelStation.Text = Session["Station"].ToString();
                    labelDepartment.Text = Session["Department"].ToString();
                    LabelCurrency.Text = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(labelStation.Text);
                    X.AddScript("btnSaveAndSend.enable();");
                }
                this.Store2.DataSource = new object[]
                {
                    new object[] { "差旅費-駐廠人員", "62012001", "Traveling - In House" },
                    new object[] { "差旅費-國內差旅費", "62012010", "Traveling - Domestic" },
                    new object[] { "差旅費-國外差旅費", "62012020", "Traveling - Overseas" },
                    new object[] { "差旅費-機票", "62012023", "Traveling - Overseas - Air ticket" },
                    new object[] { "差旅費-國內住宿費", "62012011", "Traveling - Domestic - Hotel" },
                    new object[] { "差旅費-國外住宿费", "62012021", "Traveling - Overseas - Hotel" },
                    new object[] { "差旅費-他國外差旅費", "62012024", "Traveling - Overseas - others" },
                    new object[] { "差旅費-其他差旅費", "62012014", "Traveling - Domestic - Others" },
                    new object[] { "交際費-駐廠人員", "62010901", "Entertainment - In House" },
                    new object[] { "交際費-國內交際費", "62010910", "Entertainment - Domestic" },
                    new object[] { "交際費-國外交際費", "62010920", "Entertainment - Overseas" },
                    new object[] { "交通費-駐場人員", "62011901", "Transportation - In House" },
                    new object[] { "交通費-汽油費", "62011910", "Transportation - Gasoline" },
                    new object[] { "交通費-車票", "62011920", "Transportation - Ticket" },
                    new object[] { "交通費-計程車資", "62011930", "Transportation - Taxi" },
                    new object[] { "交通費-停車費", "62011940", "Transportation - Parking" },
                    new object[] { "國內出差車票", "62012013", "Domestic - Ticket" },
                    new object[] { "通訊費-駐廠人員電話費", "62010501", "Communication - In House" },
                    new object[] { "通訊費-電話費", "62010510", "Communication - Telephone" },
                    new object[] { "通訊費-傳真費", "62010520", "Communication - Fax" },
                    new object[] { "交際費-行動電話費", "62010530", "Communication - Mobile" },
                    new object[] { "通訊費-網路費", "62010540", "Communication - Internet" },
                    new object[] { "郵資", "62010550", "Postage" },
                    new object[] { "快遞費", "62010560", "Courier" },
                    new object[] { "差旅費-國內日支費", "62012012", "Traveling - Domestic - Daily allowance" },
                    new object[] { "差旅費-國外日支額", "62012022", "Traveling - Overseas - Daily allowance" },
                    new object[] { "旅行保險費", "62020630", "Travel Insurance" },
                };

                this.Store2.DataBind();

            }
        }
        protected void LoadData(DataTable dt,bool CheckCopy)
        {
            string ID = dt.Rows[0]["RequestID"].ToString();
            cs.DBCommand dbc = new cs.DBCommand();
            if (CheckCopy)//根据Copy判断是否需要判断Copy状态
            {
                if (Request.QueryString["Copy"] != null)
                {
                    if (Request.QueryString["Copy"].ToString() == "T")//Copy而已,作为新增
                    {
                        hdTravelRequestID.Value = "";
                        hdTravelRequestNo.Value = "";
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
                }
            }
            else
            {
                hdTravelRequestID.Value = ID;
                hdTravelRequestNo.Value = dt.Rows[0]["No"].ToString();
            }
            cbxOwner.SelectedItem.Value = dt.Rows[0]["PersonID"].ToString();
            labelStation.Text = dt.Rows[0]["Station"].ToString();
            labelDepartment.Text = dt.Rows[0]["Department"].ToString();
            LabelCurrency.Text = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(labelStation.Text);
            dfBdate.Text = dt.Rows[0]["Bdate"].ToString() == "" ? "" : Convert.ToDateTime(dt.Rows[0]["Bdate"].ToString()).ToString("yyyy/MM/dd");
            dfEdate.Text = dt.Rows[0]["Edate"].ToString() == "" ? "" : Convert.ToDateTime(dt.Rows[0]["Edate"].ToString()).ToString("yyyy/MM/dd");
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
            string detailsql = "select ID,Tocity,AccountName,AccountCode,AccountDes,Cur,Pamount,Camount,TSation,Tdate from ETraveleDetail where No='" + ID + "'";
            DataTable dtdetail = new DataTable();
            dtdetail = dbc.GetData("eReimbursement", detailsql);
            DataTable dtnew = new DataTable();
            dtnew.Columns.Add("ID", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Tocity", System.Type.GetType("System.String"));
            dtnew.Columns.Add("AccountName", System.Type.GetType("System.String"));
            dtnew.Columns.Add("AccountCode", System.Type.GetType("System.String"));
            dtnew.Columns.Add("AccountDes", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Cur", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Pamount", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Camount", System.Type.GetType("System.String"));
            dtnew.Columns.Add("TSation", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Tdate", System.Type.GetType("System.String"));
            dtnew.Columns.Add("Type", System.Type.GetType("System.String"));
            decimal psum = 0, csum = 0;
            for (int i = 0; i < dtdetail.Rows.Count; i++)
            {
                DataRow dr = dtnew.NewRow();
                dr["ID"] = dtdetail.Rows[i]["ID"].ToString();
                dr["Tocity"] = dtdetail.Rows[i]["Tocity"].ToString();
                dr["AccountName"] = dtdetail.Rows[i]["AccountName"].ToString();
                dr["AccountCode"] = dtdetail.Rows[i]["AccountCode"].ToString();
                dr["AccountDes"] = dtdetail.Rows[i]["AccountDes"].ToString();
                dr["Cur"] = dtdetail.Rows[i]["Cur"].ToString();
                dr["Pamount"] = dtdetail.Rows[i]["Pamount"].ToString();
                dr["Camount"] = dtdetail.Rows[i]["Camount"].ToString();
                dr["TSation"] = dtdetail.Rows[i]["TSation"].ToString();
                dr["Tdate"] = dtdetail.Rows[i]["Tdate"].ToString() == "" ? "" : Convert.ToDateTime(dtdetail.Rows[i]["Tdate"].ToString()).ToString("yyyy/MM/dd");
                dr["Type"] = dt.Rows[0]["Type"].ToString();
                dtnew.Rows.Add(dr);
                psum += dtdetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtdetail.Rows[i]["Pamount"].ToString());
                csum += dtdetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtdetail.Rows[i]["Camount"].ToString());
            }
            Store3.DataSource = dtnew;
            Store3.DataBind();
            txtSum.Text = (psum + csum).ToString(); hdSum.Value = (psum + csum).ToString();
            txtPersonalSum.Text = psum.ToString(); hdPamountSum.Value = psum.ToString();
            txtCompanySum.Text = csum.ToString(); hdCamountSum.Value = csum.ToString();
            X.AddScript("btnExport.enable();");
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
                lstrFileNamePath = lstrFileFolder + "TR" + DateTime.Now.ToString("yyyyMMddHHmmss") + System.IO.Path.GetExtension(lstrFileName); //得到上传目录及文件名称  
                FileUploadField1.PostedFile.SaveAs(lstrFileNamePath);       //上传文件到服务器
                string filename = "TR" + DateTime.Now.ToString("yyyyMMddHHmmss") + System.IO.Path.GetExtension(lstrFileName);
                linkTravelReport.Text = filename; hdReport.Value = filename;
                linkTravelReport.NavigateUrl = "./Upload/" + filename;
                string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";
                //更新数据
                if (hdTravelRequestID.Value.ToString()!="")
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
                lstrFileNamePath = lstrFileFolder + "SF" + DateTime.Now.ToString("yyyyMMddHHmmss") + System.IO.Path.GetExtension(lstrFileName); //得到上传目录及文件名称  
                FileUploadField2.PostedFile.SaveAs(lstrFileNamePath);       //上传文件到服务器
                string filename = "SF" + DateTime.Now.ToString("yyyyMMddHHmmss") + System.IO.Path.GetExtension(lstrFileName);
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
        protected void Save(object sender, DirectEventArgs e)
        {
            DateTime dtnull = new DateTime(1, 1, 1, 0, 0, 0);
            cs.DBCommand dbc = new cs.DBCommand();
            //检查是否已经为该申请人设置过审批人
            string sqlCheckFlow = "select * from GroupFlow where GID=(select GID from GroupUsers where UserID='" + cbxOwner.Text + "')";
            DataTable dtCheckFlow = dbc.GetData("eReimbursement", sqlCheckFlow);
            if (dtCheckFlow.Rows.Count < 1)
            {
                ErrorHandle("请先设置审批人.");
                return;
            }
            string para = e.ExtraParams[0].Value;
            if (para == "ND")//保存并申请
            {
                if (hdTravelRequestID.Value.ToString() == "")//直接新增申请,不通过草稿
                {
                    string word = "[No],[Person],[Station],[Department],[ReportFile],[Tamount],[Pamout],[Camount],[CreadedBy],[CreadedDate],[Attach],[Remark],[Bdate],[Edate],[PersonID],[CreadedByID]";
                    string value = "";
                    value += "'" + labelStation.Text + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + "',";//edit
                    value += "'" + X.GetValue("cbxOwner") + "',"; value += "'" + labelStation.Text + "',"; value += "'" + labelDepartment.Text + "',";//edit
                    value += "'" + hdReport.Value.ToString() + "',";
                    value += hdSum.Value.ToString() == "" ? "null" : hdSum.Value.ToString();
                    value += "," + (hdPamountSum.Value.ToString() == "" ? "null" : hdPamountSum.Value.ToString());
                    value += "," + (hdCamountSum.Value.ToString() == "" ? "null" : hdCamountSum.Value.ToString());
                    value += ",'" + Session["UserName"].ToString() + "'";//edit
                    value += ",'" + DateTime.Now.ToString() + "',";
                    value += "'" + hdScanFile.Value.ToString() + "',";
                    value += "'" + txtRemark.Text + "',";
                    string Bdate = "";
                    if (dfBdate.SelectedDate != dtnull)
                    {
                        Bdate = dfBdate.SelectedDate.ToString();
                    }
                    value += Bdate == "" ? "null," : "'" + Bdate + "',";
                    string Edate = "";
                    if (dfEdate.SelectedDate != dtnull)
                    {
                        Edate = dfEdate.SelectedDate.ToString();
                    }
                    value += Edate == "" ? "null" : "'" + Edate + "'";
                    value += ",'" + cbxOwner.Text + "'";
                    value += ",'" + Session["UserID"].ToString() + "'";
                    
                    string updatesql = "insert into ETravel (" + word + ") values(" + value + ");update ETravel set [No]=[No]+convert(varchar,ID)+'T' where ID=@@IDENTITY;select [msg]=convert(varchar,ID)+','+[No] from ETravel where ID=@@IDENTITY";

                    string newid = dbc.UpdateData("eReimbursement", updatesql, "Insert");
                    
                    //操作Flow表
                    string sqlGetGroupFlowData = "select * from GroupFlow where GID=(select GID from GroupUsers where UserID='" + cbxOwner.Text + "')";
                    DataTable dtGroupFlowData = new DataTable();
                    dtGroupFlowData = dbc.GetData("eReimbursement", sqlGetGroupFlowData);
                    string rows = "";
                    for (int i = 0; i < dtGroupFlowData.Rows.Count; i++)
                    {
                        string wordflow = "[No],[Type],[Station],[Department],[Person],[CreadedBy],[CreatedDate],[Tamount],[Step],[Status],[Approver],[ApproverID],[Remark],[RequestID]";
                        if (dtGroupFlowData.Rows[i]["FlowNo"].ToString() == "1")
                        {
                            wordflow += ",[Active]";
                        }
                        string valueflow = "";
                        valueflow += "'" + newid.Split(',')[1] + "',";
                        valueflow += "'T',";
                        valueflow += "'" + labelStation.Text + "',";
                        valueflow += "'" + labelDepartment.Text + "',";
                        valueflow += "'" + X.GetValue("cbxOwner") + "',";
                        valueflow += "'" + Session["UserName"].ToString() + "',";
                        valueflow += "'" + DateTime.Now.ToString() + "',";
                        valueflow += hdSum.Value.ToString() == "" ? "null," : hdSum.Value.ToString() + ",";
                        valueflow += dtGroupFlowData.Rows[i]["FlowNo"].ToString() + ",";
                        valueflow += "1,";
                        valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUser"].ToString() + "',";
                        valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUserid"].ToString() + "',";
                        valueflow += "'" + txtRemark.Text + "',";
                        valueflow += "'" + newid.Split(',')[0] + "'";
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
                        Panel3.Title = "差旅费申请单:" + newid.Split(',')[1];
                        UpdateMSG("保存申请单:" + newid.Split(',')[1] + "成功.");

                    }
                    //
                }
                else//由草稿升级为正式申请
                {
                    string updatesql = "update ETravel set [Person]='" + X.GetValue("cbxOwner");
                    updatesql += "',[Station]='" + labelStation.Text;
                    updatesql += "',[Department]='" + labelDepartment.Text;
                    updatesql += "',[ReportFile]='" + hdReport.Value.ToString();
                    updatesql += "',[Tamount]=" + (hdSum.Value.ToString() == "" ? "null" : hdSum.Value.ToString());
                    updatesql += ",[Pamout]=" + (hdPamountSum.Value.ToString() == "" ? "null" : hdPamountSum.Value.ToString());
                    updatesql += ",[Camount]=" + (hdCamountSum.Value.ToString() == "" ? "null" : hdCamountSum.Value.ToString());
                    updatesql += ",[Remark]='" + txtRemark.Text + "'";
                    updatesql += ",[Attach]='" + hdScanFile.Value.ToString() + "'";
                    updatesql += ",[Type]=0";
                    updatesql += ",[PersonID]='" + cbxOwner.Text + "'";
                    string oldno = hdTravelRequestNo.Value.ToString();
                    string newno = hdTravelRequestNo.Value.ToString().Substring(0, hdTravelRequestNo.Value.ToString().Length - 1);
                    updatesql += ",[No]='" + newno + "',";
                    updatesql += "[CreadedDate]='" + DateTime.Now.ToString() + "',";
                    updatesql += "[CreadedBy]='" + Session["UserName"].ToString() + "',";
                    updatesql += "[CreadedByID]='" + Session["UserID"].ToString() + "'";
                    string Bdate = "";
                    if (dfBdate.SelectedDate != dtnull)
                    {
                        Bdate = dfBdate.SelectedDate.ToString();
                    }
                    updatesql += Bdate == "" ? ",Bdate=null" : ",Bdate='" + Bdate + "'";
                    string Edate = "";
                    if (dfEdate.SelectedDate != dtnull)
                    {
                        Edate = dfEdate.SelectedDate.ToString();
                    }
                    updatesql += Edate == "" ? ",Edate=null" : ",Edate='" + Edate + "'";
                    updatesql += " where ID=" + hdTravelRequestID.Value.ToString();

                    string newid = dbc.UpdateData("eReimbursement", updatesql, "Update");
                    
                    //操作Flow表
                    string sqlDeleteEflow = "delete from Eflow where [No]='" + oldno + "'";
                    string deleterows = dbc.UpdateData("eReimbursement", sqlDeleteEflow, "Update");
                    string sqlGetGroupFlowData = "select * from GroupFlow where GID=(select GID from GroupUsers where UserID='" + cbxOwner.Text + "')";
                    DataTable dtGroupFlowData = new DataTable();
                    dtGroupFlowData = dbc.GetData("eReimbursement", sqlGetGroupFlowData);
                    string rows = "";
                    for (int i = 0; i < dtGroupFlowData.Rows.Count; i++)
                    {
                        string wordflow = "[No],[Type],[Station],[Department],[Person],[CreadedBy],[CreatedDate],[Tamount],[Step],[Status],[Approver],[ApproverID],[Remark],[RequestID]";
                        if (dtGroupFlowData.Rows[i]["FlowNo"].ToString() == "1")
                        {
                            wordflow += ",[Active]";
                        }
                        string valueflow = "";
                        valueflow += "'" + newno + "',";
                        valueflow += "'T',";
                        valueflow += "'" + labelStation.Text + "',";
                        valueflow += "'" + labelDepartment.Text + "',";
                        valueflow += "'" + X.GetValue("cbxOwner") + "',";
                        valueflow += "'" + Session["UserName"].ToString() + "',";
                        valueflow += "'" + DateTime.Now.ToString() + "',";
                        valueflow += hdSum.Value.ToString() == "" ? "null," : hdSum.Value.ToString() + ",";
                        valueflow += dtGroupFlowData.Rows[i]["FlowNo"].ToString() + ",";
                        valueflow += "1,";
                        valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUser"].ToString() + "',";
                        valueflow += "'" + dtGroupFlowData.Rows[i]["FlowUserid"].ToString() + "',";
                        valueflow += "'" + txtRemark.Text + "'";
                        valueflow += "," + hdTravelRequestID.Value.ToString();
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
                        Panel3.Title = "差旅费申请单:" + newno;
                        UpdateMSG("保存申请单:" + hdTravelRequestNo.Value.ToString() + "成功.");
                    }
                }
                X.AddScript("btnGeteLeave.disable();btnEditDetail.disable();btnNewDetail.disable();btnSaveDraft.disable();btnSaveAndSend.disable();");
            }
            else//保存草稿
            {
                if (hdTravelRequestID.Value.ToString() != "")//由链接进入的草稿更新
                {
                    string updatesql = "update ETravel set [Person]='" + X.GetValue("cbxOwner");
                    updatesql += "',[Station]='" + labelStation.Text;
                    updatesql += "',[Department]='" + labelDepartment.Text;
                    updatesql += "',[ReportFile]='" + hdReport.Value.ToString();
                    updatesql += "',[Tamount]=" + (hdSum.Value.ToString() == "" ? "null" : hdSum.Value.ToString());
                    updatesql += ",[Pamout]=" + (hdPamountSum.Value.ToString() == "" ? "null" : hdPamountSum.Value.ToString());
                    updatesql += ",[Camount]=" + (hdCamountSum.Value.ToString() == "" ? "null" : hdCamountSum.Value.ToString());
                    updatesql += ",[Remark]='" + txtRemark.Text + "'";
                    updatesql += ",[Attach]='" + hdScanFile.Value.ToString() + "'";
                    updatesql += ",[PersonID]='" + cbxOwner.Text + "'";
                    string Bdate = "";
                    if (dfBdate.SelectedDate != dtnull)
                    {
                        Bdate = dfBdate.SelectedDate.ToString();
                    }
                    updatesql += Bdate == "" ? ",Bdate=null" : ",Bdate='" + Bdate + "'";
                    string Edate = "";
                    if (dfEdate.SelectedDate != dtnull)
                    {
                        Edate = dfEdate.SelectedDate.ToString();
                    }
                    updatesql += Edate == "" ? ",Edate=null" : ",Edate='" + Edate + "'";
                    updatesql += " where ID=" + hdTravelRequestID.Value.ToString();

                    string newid = dbc.UpdateData("eReimbursement", updatesql, "Update");
                    
                    //更新Eflow表
                    string sqlupdateflow = "update Eflow set [Station]='" + labelStation.Text + "',";
                    sqlupdateflow += "[Department]='" + labelDepartment.Text + "',";
                    sqlupdateflow += "[Person]='" + X.GetValue("cbxOwner") + "',";
                    sqlupdateflow += "[Tamount]=" + (hdSum.Value.ToString() == "" ? "null," : hdSum.Value.ToString() + ",");
                    sqlupdateflow += "[Remark]='" + txtRemark.Text + "'";
                    sqlupdateflow += " where [No]='" + hdTravelRequestNo.Value.ToString() + "'";

                    string rows = dbc.UpdateData("eReimbursement", sqlupdateflow, "Update");
                    if (newid == "-1" || rows != "1")
                    {
                        ErrorHandle("Data Error.");
                        return;
                    }
                    else
                    {
                        //hdTravelRequestID.Value = newid.Split(',')[0];//新增后记录ID
                        //Panel3.Title = "差旅费申请单:" + newid.Split(',')[1];
                        //X.AddScript("GridPanel2.submitData();");
                        UpdateMSG("保存草稿:" + hdTravelRequestNo.Value.ToString() + "成功.");
                    }

                }
                else//如果ID为空则判断为新增草稿
                {
                    string word = "[No],[Person],[Station],[Department],[ReportFile],[Tamount],[Pamout],[Camount],[Attach],[Remark],[Bdate],[Edate],[Type],[PersonID]";
                    string value = "";
                    value += "'" + labelStation.Text + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + "',";//edit
                    value += "'" + X.GetValue("cbxOwner") + "',"; value += "'" + "GCR" + "',"; value += "'" + "MIS" + "',";//edit
                    value += "'" + hdReport.Value.ToString() + "',";
                    value += (hdSum.Value.ToString() == "" ? "null" : hdSum.Value.ToString());
                    value += "," + (hdPamountSum.Value.ToString() == "" ? "null" : hdPamountSum.Value.ToString());
                    value += "," + (hdCamountSum.Value.ToString() == "" ? "null" : hdCamountSum.Value.ToString());
                    value += ",'" + hdScanFile.Value.ToString() + "',";
                    value += "'" + txtRemark.Text + "',";
                    string Bdate = "";
                    if (dfBdate.SelectedDate != dtnull)
                    {
                        Bdate = dfBdate.SelectedDate.ToString();
                    }
                    value += Bdate == "" ? "null," : "'" + Bdate + "',";
                    string Edate = "";
                    if (dfEdate.SelectedDate != dtnull)
                    {
                        Edate = dfEdate.SelectedDate.ToString();
                    }
                    value += Edate == "" ? "null," : "'" + Edate + "',";
                    value += "1";//标识为草稿
                    value += ",'" + cbxOwner.Text + "'";
                    string updatesql = "insert into ETravel (" + word + ") values(" + value + ");update ETravel set [No]=[No]+convert(varchar,ID)+'TD' where ID=@@IDENTITY;select [msg]=convert(varchar,ID)+','+[No] from ETravel where ID=@@IDENTITY";

                    string newid = dbc.UpdateData("eReimbursement", updatesql, "Insert");
                    
                    //操作Flow表
                    string wordflow = "[No],[Type],[Station],[Department],[Person],[CreadedBy],[CreatedDate],[Tamount],[Remark],[RequestID],[Active]";
                    string valueflow = "";
                    valueflow += "'" + newid.Split(',')[1] + "',";
                    valueflow += "'T',";
                    valueflow += "'" + labelStation.Text + "',";
                    valueflow += "'" + labelDepartment.Text + "',";
                    valueflow += "'" + X.GetValue("cbxOwner") + "',";
                    valueflow += "'" + Session["UserName"].ToString() + "',";
                    valueflow += "'" + DateTime.Now.ToString() + "',";
                    valueflow += hdSum.Value.ToString() == "" ? "null," : hdSum.Value.ToString() + ",";
                    valueflow += "'" + txtRemark.Text + "',";
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
                        Panel3.Title = "差旅费申请单草稿:" + newid.Split(',')[1];
                        UpdateMSG("新增草稿:" + newid.Split(',')[1] + "成功.");
                    }
                }
            }
        }
        //暂存差旅费明细
        public class Detail
        {
            public string id { get; set; }
            public string ID { get; set; }
            public string Tocity { get; set; }
            public string AccountName { get; set; }
            public string AccountCode { get; set; }
            public string AccountDes { get; set; }
            public string Cur { get; set; }
            public string Pamount { get; set; }
            public string Camount { get; set; }
            public string TSation { get; set; }
            public string Tdate { get; set; }
        }
        //用于保存差旅费明细
        protected void SubmitData(object sender, StoreSubmitDataEventArgs e)
        {
            DateTime dtnull = new DateTime(1, 1, 1, 0, 0, 0);
            cs.DBCommand dbc = new cs.DBCommand();
            //检查是否已经被该人设置过审批人
            string sqlCheckFlow = "select * from GroupFlow where GID=(select GID from GroupUsers where UserID='" + cbxOwner.Text + "')";
            DataTable dtCheckFlow = dbc.GetData("eReimbursement", sqlCheckFlow);
            if (dtCheckFlow.Rows.Count < 1)
            {
                return;
            }
            string json = e.Json;
            XmlNode xml = e.Xml;
            List<Detail> Details = e.Object<Detail>();
            //删除现有数据
            string deletesql = "delete from ETraveleDetail where [No]='" + hdTravelRequestID.Value.ToString() + "'";
            string newid1 = dbc.UpdateData("eReimbursement", deletesql, "Update");
            foreach (Detail detail in Details)
            {
                if (newid1 == "-1")
                {
                    ErrorHandle("Data Error.");
                    return;
                }
                //新增
                string word = "[No],[Tocity],[AccountName],[AccountCode],[AccountDes],[Cur],[Pamount],[Camount],[TSation],[Createdby],[CreadedDate],[Tdate]";
                string value = "";
                value += "'" + hdTravelRequestID.Value.ToString() + "',";
                value += "'" + detail.Tocity + "',";
                value += "'" + detail.AccountName + "',";
                value += "'" + detail.AccountCode + "',";
                value += "'" + detail.AccountDes + "',";
                value += "'" + detail.Cur + "',";
                value += detail.Pamount == "" ? "null," : detail.Pamount + ",";
                value += detail.Camount == "" ? "null," : detail.Camount + ",";
                value += "'" + detail.TSation + "',";
                value += "'" + cbxOwner.Text + "',";//edit
                value += "'" + DateTime.Now.ToString() + "',";
                value += detail.Tdate == "" ? "null" : "'" + detail.Tdate + "'";

                string updatesql = "insert into ETraveleDetail (" + word + ") values(" + value + ");select [ID]=@@IDENTITY from ETraveleDetail";

                string newid = dbc.UpdateData("eReimbursement", updatesql, "Insert");
                if (newid == "-1")
                {
                    ErrorHandle("Data Error."); return;
                }
            }
            //发送提醒邮件
            string sql = "select * from V_Eflow_ETravel where [Type]='T' and Step!=0 and RequestID=" + hdTravelRequestID.Value.ToString() + " order by Step,FlowID";
            DataTable dtMail = new DataTable();
            dtMail = dbc.GetData("eReimbursement", sql);
            if (dtMail != null && dtMail.Rows.Count > 0)
            {
                DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();
                mail.Title = "Dimerco eReimbursement (" + dtMail.Rows[0]["Person"].ToString() + ") - Seek For Your Approval";
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
                    ErrorHandle("Error mail address of Approver."); return;
                }

                string mailcc = "";
                DataSet dsCC = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtMail.Rows[0]["PersonID"].ToString());
                if (dsCC.Tables[0].Rows.Count == 1)
                {
                    mailcc += dsCC.Tables[0].Rows[0]["eMail"].ToString() + ",";
                }
                else
                {
                    ErrorHandle("Error mail address of Owner."); return;
                }
                if (dtMail.Rows[0]["CreadedByID"].ToString() != "" && dtMail.Rows[0]["CreadedByID"].ToString() != dtMail.Rows[0]["PersonID"].ToString())//代理人
                {
                    dsCC = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtMail.Rows[0]["CreadedByID"].ToString());
                    mailcc += dsCC.Tables[0].Rows[0]["eMail"].ToString() + ",";
                }

                mail.To = mailto;
                mail.Cc = mailcc;
                string divstyle = "style='font-size:small;'";
                string divstyleCurrent = "style='font-size:small;color:blue;'";
                string tdstyle = "style='border:silver 1px ridge; font-size:small;background-color: #FFFFFF'";
                StringBuilder sb = new StringBuilder();
                sb.Append("<div>");
                sb.Append("<div " + divstyle + "> Dear " + dtMail.Rows[0]["Approver"].ToString() + ",</div><br />");
                sb.Append("<div " + divstyle + ">The following eReimbursement application for your approval:</div><br /><br />");
                sb.Append("<div " + divstyle + ">eReimbursement Approval Remarks :</div><br />");
                sb.Append("<div " + divstyle + ">Owner:" + dtMail.Rows[0]["Person"].ToString() + "</div>");
                sb.Append("<div " + divstyle + ">Station:" + dtMail.Rows[0]["Station"].ToString() + "</div>");
                sb.Append("<div " + divstyle + ">Department:" + dtMail.Rows[0]["Department"].ToString() + "</div>");
                string period = "";
                period += dtMail.Rows[0]["Bdate"].ToString() == "" ? "From NA " : ("From " + Convert.ToDateTime(dtMail.Rows[0]["Bdate"].ToString()).ToString("yyyy/MM/dd") + " ");
                period += dtMail.Rows[0]["Edate"].ToString() == "" ? "To NA" : "To " + Convert.ToDateTime(dtMail.Rows[0]["Edate"].ToString()).ToString("yyyy/MM/dd");
                sb.Append("<div " + divstyle + ">Period:" + period + "</div><br />");
                sb.Append("<div><table><thead><tr><th colspan=\"9\" " + tdstyle + ">Expense Detail</th></tr><tr>");
                sb.Append("<th " + tdstyle + "></th>");
                sb.Append("<th " + tdstyle + ">Location</th>");
                sb.Append("<th " + tdstyle + ">Date</th>");
                sb.Append("<th " + tdstyle + ">Expense Type</th>");
                sb.Append("<th " + tdstyle + ">Currency</th>");
                sb.Append("<th " + tdstyle + ">Employee Pay</th>");
                sb.Append("<th " + tdstyle + ">Company Pay</th>");
                sb.Append("<th " + tdstyle + ">Collection Station</th>");
                sb.Append("<th " + tdstyle + ">Remark</th></tr></thead>");

                sb.Append("<tbody>");
                decimal ptotal = 0; decimal ctotal = 0;
                string sqldetail = "select t2.SAccountName,t1.* from ETraveleDetail t1 left join AccoundCode t2 on t2.SAccountCode=t1.AccountCode where t1.[No]='" + hdTravelRequestID.Value.ToString() + "'";
                DataTable dtdetail = new DataTable();
                dtdetail = dbc.GetData("eReimbursement", sqldetail);
                for (int i = 0; i < dtdetail.Rows.Count; i++)
                {
                    sb.Append("<tr><th " + tdstyle + ">" + (i + 1).ToString() + "</th>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["Tocity"].ToString() + "</td>");
                    string tdate = dtdetail.Rows[i]["Tdate"].ToString() == "" ? "" : Convert.ToDateTime(dtdetail.Rows[i]["Tdate"].ToString()).ToString("yyyy/MM/dd");
                    sb.Append("<td " + tdstyle + ">" + tdate + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["SAccountName"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["Cur"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["Pamount"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["Camount"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["TSation"].ToString() + "</td>");
                    sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[i]["AccountDes"].ToString() + "</td></tr>");
                    ptotal += dtdetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtdetail.Rows[i]["Pamount"].ToString());
                    ctotal += dtdetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtdetail.Rows[i]["Camount"].ToString());
                }
                sb.Append("</tbody>");
                sb.Append("<tfoot><tr>");
                sb.Append("<th " + tdstyle + ">Total:</th>");
                sb.Append("<td " + tdstyle + "></td>");
                sb.Append("<td " + tdstyle + "></td>");
                sb.Append("<td " + tdstyle + "></td>");
                sb.Append("<td " + tdstyle + ">" + dtdetail.Rows[0]["Cur"].ToString() + "</td>");
                sb.Append("<td " + tdstyle + ">" + ptotal.ToString() + "</td>");
                sb.Append("<td " + tdstyle + ">" + ctotal.ToString() + "</td>");
                sb.Append("<td " + tdstyle + "></td>");
                sb.Append("<td " + tdstyle + "></td>");
                sb.Append("</tr></tfoot></table></div><br />");
                
                
                StringBuilder sb1 = new StringBuilder();
                sb1.Append("<div><span " + divstyle + ">Approval Flow:</span>");
                for (int i = 0; i < dtMail.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        sb1.Append("<div " + divstyleCurrent + ">" + (i + 1).ToString() + ".Current Approver:" + dtMail.Rows[i]["Approver"].ToString() + "</div>");
                    }
                    else
                    {
                        sb1.Append("<div " + divstyle + ">" + (i + 1).ToString() + ".Approver:" + dtMail.Rows[i]["Approver"].ToString() + "</div>");
                    }
                }
                sb1.Append("</div><br />");
                sb.Append(sb1.ToString());
                string url = "http://" + Request.Url.Authority + "/Approve.aspx";
                sb.Append("<div><a href=\"" + url + "?FlowID=" + dtMail.Rows[0]["FlowID"].ToString() + "\" style=\"color: #0000FF\">Click here to visit Dimerco eReimbursement.</a></div>");
                sb.Append("</div>");
                mail.Body = sb.ToString();
                mail.Send();
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
            DataSet ds = new DataSet();
            bool user = DIMERCO.SDK.Utilities.ReSM.CheckUserInfo(tfUserID.Text.Trim(), tfPW.Text.Trim(), ref ds);
            if (ds.Tables[0].Rows.Count == 1)
            {
                DataTable dtuser = ds.Tables[0];
                Session["UserID"] = dtuser.Rows[0]["UserID"].ToString();
                DataSet ds1 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtuser.Rows[0]["UserID"].ToString());
                if (ds1.Tables[0].Rows.Count == 1)
                {
                    DataTable dt1 = ds1.Tables[0];
                    Session["UserName"] = dt1.Rows[0]["fullName"].ToString();
                    Session["Station"] = dt1.Rows[0]["stationCode"].ToString();
                    Session["Department"] = dt1.Rows[0]["DepartmentName"].ToString();
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
            X.AddScript("window.location.reload();");
        }
        [DirectMethod]
        public void GetDataFromeLeave()
        {
            //获取eLeave信息
            DataSet ds = DIMERCO.SDK.Utilities.LSDK.GetBizTripByUserID(Session["UserID"].ToString());
            if (ds != null && ds.Tables.Count == 1)
            {
                DataTable dteLeave = ds.Tables[0];
                DataTable dtnew = new DataTable();
                dtnew.Columns.Add("leaveStart", System.Type.GetType("System.String"));
                dtnew.Columns.Add("leaveStart1", System.Type.GetType("System.String"));
                dtnew.Columns.Add("leaveEnd", System.Type.GetType("System.String"));
                dtnew.Columns.Add("leaveEnd1", System.Type.GetType("System.String"));
                dtnew.Columns.Add("leaveCount", System.Type.GetType("System.String"));
                dtnew.Columns.Add("Destination", System.Type.GetType("System.String"));
                
                for (int i = 0; i < dteLeave.Rows.Count; i++)
                {
                    DataRow dr = dtnew.NewRow();
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
                    
                    dtnew.Rows.Add(dr);
                }
                Store1.DataSource = dtnew;
                Store1.DataBind();
            }
        }
        protected void GetCity(object sender, DirectEventArgs e)
        {
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
            StoreCity.DataSource = dtcitynew;
            StoreCity.DataBind();
        }
        protected void GetStation(object sender, DirectEventArgs e)
        {
            DataSet getCostCenterBYStationCode = DIMERCO.SDK.Utilities.LSDK.getCostCenterBYStationCode(X.GetValue("cbxCOACenter"), 10);
            DataTable dtCOACenter = (DataTable)getCostCenterBYStationCode.Tables[0];
            DataTable dtCOACenternew = new DataTable();
            dtCOACenternew.Columns.Add("cityID", System.Type.GetType("System.String"));
            dtCOACenternew.Columns.Add("cityCode", System.Type.GetType("System.String"));
            for (int c = 0; c < dtCOACenter.Rows.Count; c++)
            {
                DataRow dr = dtCOACenternew.NewRow();
                dr["cityID"] = dtCOACenter.Rows[c][0].ToString();
                dr["cityCode"] = dtCOACenter.Rows[c][1].ToString();
                dtCOACenternew.Rows.Add(dr);
            }
            StoreCOACenter.DataSource = dtCOACenternew;
            StoreCOACenter.DataBind();
        }
        protected void btnExport_Click(object sender, EventArgs e)
        {
            XlsDocument xls = new XlsDocument();//新建一个xls文档
            xls.FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

            string MainID = Request.QueryString["ID"].ToString();
            cs.DBCommand dbc = new cs.DBCommand();
            string sqlmain = "select * from ETravel where ID=" + MainID;
            DataTable dtMain = dbc.GetData("eReimbursement", sqlmain);
            if (dtMain.Rows.Count != 1)
            {
                ErrorHandle("Data Error.");
                return;
            }
            string sqlTocity = "select (ROW_NUMBER() OVER (ORDER BY Tocity)-1) % 4 AS SubRow,(ROW_NUMBER() OVER (ORDER BY Tocity) - 1) / 4 AS Row,Tocity from (select distinct Tocity from ETraveleDetail where [No]='" + MainID + "') t1";
            DataTable dtTocity = dbc.GetData("eReimbursement", sqlTocity);
            int pagecount = 0;
            for (int i = 0; i < dtTocity.Rows.Count; i++)
            {
                if (Convert.ToInt32(dtTocity.Rows[i]["Row"].ToString())>pagecount)
                {
                    pagecount = Convert.ToInt32(dtTocity.Rows[i]["Row"].ToString());
                }
            }
            for (int j = 0; j < pagecount + 1; j++)
            {
                Worksheet sheet;
                sheet = xls.Workbook.Worksheets.Add(DateTime.Now.ToString("yyyyMMddHHmmss" + j.ToString()));
                //首行空白行
                XF titleXF = xls.NewXF(); // 为xls生成一个XF实例，XF是单元格格式对象
                titleXF.HorizontalAlignment = HorizontalAlignments.Left; // 设定文字居中
                titleXF.VerticalAlignment = VerticalAlignments.Centered; // 垂直居中
                titleXF.UseBorder = false; // 使用边框
                titleXF.Font.Height = 12 * 20; // 字大小（字体大小是以 1/20 point 为单位的）
                //第二行
                XF titleXF1 = xls.NewXF(); // 为xls生成一个XF实例，XF是单元格格式对象
                titleXF1.HorizontalAlignment = HorizontalAlignments.Centered; // 设定文字居中
                titleXF1.VerticalAlignment = VerticalAlignments.Centered; // 垂直居中
                titleXF1.UseBorder = false; // 使用边框
                titleXF1.Font.Underline = UnderlineTypes.Single;
                titleXF1.Font.Height = 18 * 20;

                XF columnTitleXF41 = xls.NewXF();

                XF columnTitleXF42 = xls.NewXF();
                columnTitleXF42.HorizontalAlignment = HorizontalAlignments.Centered;
                columnTitleXF42.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF42.Font.Height = 12 * 20;

                XF columnTitleXF43 = xls.NewXF();
                columnTitleXF43.HorizontalAlignment = HorizontalAlignments.Centered;
                columnTitleXF43.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF43.Font.Height = 12 * 20;
                columnTitleXF43.UseBorder = true;
                columnTitleXF43.LeftLineStyle = 2;
                columnTitleXF43.TopLineStyle = 2;
                columnTitleXF43.RightLineStyle = 2;
                columnTitleXF43.BottomLineStyle = 2;

                XF columnTitleXF44 = xls.NewXF();
                columnTitleXF44.HorizontalAlignment = HorizontalAlignments.Centered;
                columnTitleXF44.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF44.Font.Height = 12 * 20;
                columnTitleXF44.UseBorder = true;
                columnTitleXF44.TopLineStyle = 2;
                columnTitleXF44.BottomLineStyle = 2;

                XF columnTitleXF46 = xls.NewXF();
                columnTitleXF46.HorizontalAlignment = HorizontalAlignments.Centered;
                columnTitleXF46.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF46.Font.Height = 12 * 20;
                columnTitleXF46.UseBorder = true;
                columnTitleXF46.TopLineStyle = 2;
                columnTitleXF46.BottomLineStyle = 2;
                columnTitleXF46.RightLineStyle = 2;

                XF columnTitleXF412 = xls.NewXF();
                columnTitleXF412.HorizontalAlignment = HorizontalAlignments.Centered;
                columnTitleXF412.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF412.Font.Height = 12 * 20;
                columnTitleXF412.UseBorder = true;
                columnTitleXF412.TopLineStyle = 2;
                columnTitleXF412.BottomLineStyle = 2;
                columnTitleXF412.RightLineStyle = 2;

                XF columnTitleXF62 = xls.NewXF();
                columnTitleXF62.HorizontalAlignment = HorizontalAlignments.Centered;
                columnTitleXF62.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF62.Font.Height = 10 * 20;
                columnTitleXF62.Font.Bold = true;
                columnTitleXF62.UseBorder = true;
                columnTitleXF62.LeftLineStyle = 2;
                columnTitleXF62.TopLineStyle = 2;
                columnTitleXF62.RightLineStyle = 2;
                columnTitleXF62.BottomLineStyle = 2;

                XF columnTitleXF63 = xls.NewXF();
                columnTitleXF63.HorizontalAlignment = HorizontalAlignments.Centered;
                columnTitleXF63.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF63.Font.Height = 10 * 20;
                columnTitleXF63.UseBorder = true;
                columnTitleXF63.TopLineStyle = 2;
                columnTitleXF63.LeftLineStyle = 1;

                XF columnTitleXF64 = xls.NewXF();
                columnTitleXF64.HorizontalAlignment = HorizontalAlignments.Centered;
                columnTitleXF64.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF64.Font.Height = 10 * 20;
                columnTitleXF64.UseBorder = true;
                columnTitleXF64.TopLineStyle = 2;
                columnTitleXF64.RightLineStyle = 1;

                XF columnTitleXF66 = xls.NewXF();
                columnTitleXF66.HorizontalAlignment = HorizontalAlignments.Centered;
                columnTitleXF66.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF66.Font.Height = 10 * 20;
                columnTitleXF66.UseBorder = true;
                columnTitleXF66.TopLineStyle = 2;

                XF columnTitleXF67 = xls.NewXF();
                columnTitleXF67.HorizontalAlignment = HorizontalAlignments.Centered;
                columnTitleXF67.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF67.Font.Height = 10 * 20;
                columnTitleXF67.Font.Bold = true;
                columnTitleXF67.UseBorder = true;
                columnTitleXF67.LeftLineStyle = 2;
                columnTitleXF67.TopLineStyle = 2;
                columnTitleXF67.BottomLineStyle = 1;

                XF columnTitleXF68 = xls.NewXF();
                columnTitleXF68.HorizontalAlignment = HorizontalAlignments.Centered;
                columnTitleXF68.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF68.Font.Height = 10 * 20;
                columnTitleXF68.UseBorder = true;
                columnTitleXF68.TopLineStyle = 2;
                columnTitleXF68.RightLineStyle = 2;

                XF columnTitleXF72 = xls.NewXF();
                columnTitleXF72.HorizontalAlignment = HorizontalAlignments.Centered;
                columnTitleXF72.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF72.Font.Height = 10 * 20;
                columnTitleXF72.Font.Bold = true;
                columnTitleXF72.UseBorder = true;
                columnTitleXF72.LeftLineStyle = 2;
                columnTitleXF72.RightLineStyle = 2;
                columnTitleXF72.BottomLineStyle = 2;

                XF columnTitleXF73 = xls.NewXF();
                columnTitleXF73.HorizontalAlignment = HorizontalAlignments.Centered;
                columnTitleXF73.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF73.Font.Height = 10 * 20;
                columnTitleXF73.UseBorder = true;
                columnTitleXF73.LeftLineStyle = 1;
                columnTitleXF73.TopLineStyle = 1;
                columnTitleXF73.RightLineStyle = 1;
                columnTitleXF73.BottomLineStyle = 2;

                XF columnTitleXF77 = xls.NewXF();
                columnTitleXF77.HorizontalAlignment = HorizontalAlignments.Centered;
                columnTitleXF77.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF77.Font.Height = 10 * 20;
                columnTitleXF77.Font.Bold = true;
                columnTitleXF77.UseBorder = true;
                columnTitleXF77.LeftLineStyle = 2;
                columnTitleXF77.TopLineStyle = 1;
                columnTitleXF77.RightLineStyle = 1;
                columnTitleXF77.BottomLineStyle = 2;

                XF columnTitleXF78 = xls.NewXF();
                columnTitleXF78.HorizontalAlignment = HorizontalAlignments.Centered;
                columnTitleXF78.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF78.Font.Height = 10 * 20;
                columnTitleXF78.Font.Bold = true;
                columnTitleXF78.UseBorder = true;
                columnTitleXF78.TopLineStyle = 1;
                columnTitleXF78.RightLineStyle = 2;
                columnTitleXF78.BottomLineStyle = 2;

                XF columnTitleXF82 = xls.NewXF();
                columnTitleXF82.HorizontalAlignment = HorizontalAlignments.Left;
                columnTitleXF82.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF82.Font.Height = 10 * 20;
                columnTitleXF82.UseBorder = true;
                columnTitleXF82.LeftLineStyle = 2;
                columnTitleXF82.TopLineStyle = 2;
                columnTitleXF82.RightLineStyle = 2;
                columnTitleXF82.BottomLineStyle = 1;

                XF columnTitleXF83 = xls.NewXF();
                columnTitleXF83.HorizontalAlignment = HorizontalAlignments.Right;
                columnTitleXF83.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF83.Font.Height = 10 * 20;
                columnTitleXF83.UseBorder = true;
                columnTitleXF83.LeftLineStyle = 1;
                columnTitleXF83.TopLineStyle = 1;
                columnTitleXF83.RightLineStyle = 1;
                columnTitleXF83.BottomLineStyle = 1;

                XF columnTitleXF812 = xls.NewXF();
                columnTitleXF812.HorizontalAlignment = HorizontalAlignments.Right;
                columnTitleXF812.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF812.Font.Height = 10 * 20;
                columnTitleXF812.UseBorder = true;
                columnTitleXF812.LeftLineStyle = 1;
                columnTitleXF812.TopLineStyle = 1;
                columnTitleXF812.RightLineStyle = 2;
                columnTitleXF812.BottomLineStyle = 1;

                XF columnTitleXF92 = xls.NewXF();
                columnTitleXF92.HorizontalAlignment = HorizontalAlignments.Left;
                columnTitleXF92.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF92.Font.Height = 10 * 20;
                columnTitleXF92.UseBorder = true;
                columnTitleXF92.LeftLineStyle = 2;
                columnTitleXF92.TopLineStyle = 1;
                columnTitleXF92.RightLineStyle = 2;
                columnTitleXF92.BottomLineStyle = 1;

                XF columnTitleXF93 = xls.NewXF();
                columnTitleXF93.HorizontalAlignment = HorizontalAlignments.Left;
                columnTitleXF93.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF93.Font.Height = 10 * 20;
                columnTitleXF93.UseBorder = true;
                columnTitleXF93.LeftLineStyle = 1;
                columnTitleXF93.TopLineStyle = 1;

                XF columnTitleXF192 = xls.NewXF();
                columnTitleXF192.HorizontalAlignment = HorizontalAlignments.Left;
                columnTitleXF192.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF192.Font.Height = 10 * 20;
                columnTitleXF192.UseBorder = true;
                columnTitleXF192.LeftLineStyle = 2;
                columnTitleXF192.TopLineStyle = 1;
                columnTitleXF192.RightLineStyle = 2;
                columnTitleXF192.BottomLineStyle = 2;

                XF columnTitleXF202 = xls.NewXF();
                columnTitleXF202.HorizontalAlignment = HorizontalAlignments.Left;
                columnTitleXF202.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF202.Font.Height = 10 * 20;
                columnTitleXF202.UseBorder = true;
                columnTitleXF202.LeftLineStyle = 2;
                columnTitleXF202.TopLineStyle = 2;

                XF columnTitleXF208 = xls.NewXF();
                columnTitleXF208.HorizontalAlignment = HorizontalAlignments.Centered;
                columnTitleXF208.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF208.Font.Height = 10 * 20;
                columnTitleXF208.UseBorder = true;
                columnTitleXF208.LeftLineStyle = 1;

                XF columnTitleXF215 = xls.NewXF();
                columnTitleXF215.HorizontalAlignment = HorizontalAlignments.Left;
                columnTitleXF215.VerticalAlignment = VerticalAlignments.Centered;
                columnTitleXF215.Font.Height = 10 * 20;
                columnTitleXF215.UseBorder = true;
                columnTitleXF215.LeftLineStyle = 1;

                // 列标题行
                ColumnInfo col1 = new ColumnInfo(xls, sheet); // 列对象
                col1.ColumnIndexStart = 0; // 起始列为第1列，索引从0开始
                col1.ColumnIndexEnd = 0; // 终止列为第1列，索引从0开始
                col1.Width = 256; // 列的宽度计量单位为 1/256 字符宽
                sheet.AddColumnInfo(col1); // 把格式附加到sheet页上

                ColumnInfo col2 = new ColumnInfo(xls, sheet);
                col2 = new ColumnInfo(xls, sheet); // 列对象
                col2.ColumnIndexStart = 1;
                col2.ColumnIndexEnd = 1;
                col2.Width = 7680;
                sheet.AddColumnInfo(col2);

                ColumnInfo col3 = new ColumnInfo(xls, sheet);
                col3 = new ColumnInfo(xls, sheet); // 列对象
                col3.ColumnIndexStart = 2;
                col3.ColumnIndexEnd = 9;
                col3.Width = 1800;
                sheet.AddColumnInfo(col3);

                ColumnInfo col4 = new ColumnInfo(xls, sheet);
                col4 = new ColumnInfo(xls, sheet); // 列对象
                col4.ColumnIndexStart = 10;
                col4.ColumnIndexEnd = 11;
                col4.Width = 3900;
                sheet.AddColumnInfo(col4);

                //行
                RowInfo rol1 = new RowInfo();
                rol1.RowHeight = 10 * 20;
                rol1.RowIndexStart = 1;
                rol1.RowIndexEnd = 1;
                sheet.AddRowInfo(rol1);

                rol1 = new RowInfo();
                rol1.RowHeight = 20 * 20;
                rol1.RowIndexStart = 2;
                rol1.RowIndexEnd = 2;
                sheet.AddRowInfo(rol1);

                rol1 = new RowInfo();
                rol1.RowHeight = 5 * 20;
                rol1.RowIndexStart = 3;
                rol1.RowIndexEnd = 3;
                sheet.AddRowInfo(rol1);

                rol1 = new RowInfo();
                rol1.RowHeight = 5 * 20;
                rol1.RowIndexStart = 5;
                rol1.RowIndexEnd = 5;
                sheet.AddRowInfo(rol1);

                rol1 = new RowInfo();
                rol1.RowHeight = 18 * 20;
                rol1.RowIndexStart = 6;
                rol1.RowIndexEnd = 23;
                sheet.AddRowInfo(rol1);

                // 数据单元格样式
                XF dataXF = xls.NewXF(); // 为xls生成一个XF实例，XF是单元格格式对象
                dataXF.HorizontalAlignment = HorizontalAlignments.Centered; // 设定文字居中
                dataXF.VerticalAlignment = VerticalAlignments.Centered; // 垂直居中
                dataXF.UseBorder = true; // 使用边框
                dataXF.LeftLineStyle = 1; // 左边框样式
                dataXF.LeftLineColor = Colors.Black; // 左边框颜色
                dataXF.BottomLineStyle = 1;  // 下边框样式
                dataXF.BottomLineColor = Colors.Black;  // 下边框颜色
                dataXF.Font.FontName = "宋体";
                dataXF.Font.Height = 9 * 20; // 设定字大小（字体大小是以 1/20 point 为单位的）
                dataXF.UseProtection = false; // 默认的就是受保护的，导出后需要启用编辑才可修改
                dataXF.TextWrapRight = true; // 自动换行

                // 合并单元格
                MergeArea titleArea = new MergeArea(1, 1, 1, 12);
                sheet.AddMergeArea(titleArea);
                titleArea = new MergeArea(2, 2, 1, 12);
                sheet.AddMergeArea(titleArea);
                titleArea = new MergeArea(3, 3, 1, 12);
                sheet.AddMergeArea(titleArea);
                titleArea = new MergeArea(4, 4, 3, 6);
                sheet.AddMergeArea(titleArea);
                titleArea = new MergeArea(4, 4, 7, 10);
                sheet.AddMergeArea(titleArea);
                titleArea = new MergeArea(4, 4, 11, 12);
                sheet.AddMergeArea(titleArea);
                titleArea = new MergeArea(6, 6, 11, 12);
                sheet.AddMergeArea(titleArea);
                titleArea = new MergeArea(6, 6, 3, 4);
                sheet.AddMergeArea(titleArea);
                titleArea = new MergeArea(6, 6, 5, 6);
                sheet.AddMergeArea(titleArea);
                titleArea = new MergeArea(6, 6, 7, 8);
                sheet.AddMergeArea(titleArea);
                titleArea = new MergeArea(6, 6, 9, 10);
                sheet.AddMergeArea(titleArea);
                titleArea = new MergeArea(6, 7, 2, 2);
                sheet.AddMergeArea(titleArea);
                // 开始填充数据到单元格
                org.in2bits.MyXls.Cells cells = sheet.Cells;
                cells.Add(1, 1, "", titleXF);
                cells.Add(2, 1, "Travel Expense Report", titleXF1);
                cells.Add(3, 1, "", titleXF);
                cells.Add(4, 1, "", columnTitleXF41);
                cells.Add(4, 2, "Applicant:", columnTitleXF42);
                cells.Add(4, 3, dtMain.Rows[0]["Person"].ToString(), columnTitleXF43);
                cells.Add(4, 4, "", columnTitleXF44);
                cells.Add(4, 5, "", columnTitleXF44);
                cells.Add(4, 6, "", columnTitleXF46);

                cells.Add(4, 7, "Travel Period:", columnTitleXF42);
                string bb = "";
                if (dtMain.Rows[0]["Bdate"].ToString()!="")
                {
                    bb += Convert.ToDateTime(dtMain.Rows[0]["Bdate"].ToString()).ToString("yyyy/MM/dd");
                }
                if (dtMain.Rows[0]["Edate"].ToString() != "")
                {
                    bb += " - " + Convert.ToDateTime(dtMain.Rows[0]["Edate"].ToString()).ToString("yyyy/MM/dd");
                }
                cells.Add(4, 11, bb, columnTitleXF43);
                cells.Add(4, 12, "", columnTitleXF412);
                cells.Add(5, 1, "", columnTitleXF41);
                cells.Add(6, 1, "", columnTitleXF41);
                cells.Add(6, 2, "Travel Destination", columnTitleXF62);
                cells.Add(6, 3, "", columnTitleXF63);
                cells.Add(6, 4, "", columnTitleXF64);
                cells.Add(6, 5, "", columnTitleXF63);
                cells.Add(6, 6, "", columnTitleXF64);
                cells.Add(6, 7, "", columnTitleXF63);
                cells.Add(6, 8, "", columnTitleXF64);
                cells.Add(6, 9, "", columnTitleXF63);
                cells.Add(6, 10, "", columnTitleXF64);
                cells.Add(6, 11, "Total Expenses", columnTitleXF67);
                cells.Add(6, 12, "", columnTitleXF68);
                cells.Add(7, 1, "", columnTitleXF41);
                cells.Add(7, 2, "", columnTitleXF72);
                cells.Add(7, 3, "", columnTitleXF73);
                cells.Add(7, 4, "", columnTitleXF73);
                cells.Add(7, 5, "", columnTitleXF73);
                cells.Add(7, 6, "", columnTitleXF73);
                cells.Add(7, 7, "", columnTitleXF73);
                cells.Add(7, 8, "", columnTitleXF73);
                cells.Add(7, 9, "", columnTitleXF73);
                cells.Add(7, 10, "", columnTitleXF73);
                cells.Add(7, 11, "Reimbursement", columnTitleXF77);
                cells.Add(7, 12, "Company Paid", columnTitleXF78);
                cells.Add(8, 1, "", columnTitleXF41);
                cells.Add(8, 2, "1. Air Ticket - Int'l", columnTitleXF82);
                cells.Add(9, 1, "", columnTitleXF41);
                cells.Add(9, 2, "Domestic", columnTitleXF92);
                cells.Add(10, 1, "", columnTitleXF41);
                cells.Add(10, 2, "2. Hotel Bill", columnTitleXF92);
                cells.Add(11, 1, "", columnTitleXF41);
                cells.Add(11, 2, "3. Meals", columnTitleXF92);
                cells.Add(12, 1, "", columnTitleXF41);
                cells.Add(12, 2, "4. Entertainment", columnTitleXF92);
                cells.Add(13, 1, "", columnTitleXF41);
                cells.Add(13, 2, "5. Car Rental/Transportation", columnTitleXF92);
                cells.Add(14, 1, "", columnTitleXF41);
                cells.Add(14, 2, "6. Communication", columnTitleXF92);
                cells.Add(15, 1, "", columnTitleXF41);
                cells.Add(15, 2, "7. Local Trip NTD 800/day", columnTitleXF92);
                cells.Add(16, 1, "", columnTitleXF41);
                cells.Add(16, 2, "8. Overseas Trip USD15/day", columnTitleXF92);
                cells.Add(17, 1, "", columnTitleXF41);
                cells.Add(17, 2, "9. Airport Tax/Travel Insurance", columnTitleXF92);
                cells.Add(18, 1, "", columnTitleXF41);
                cells.Add(18, 2, "10. Others", columnTitleXF92);
                cells.Add(19, 1, "", columnTitleXF41);
                cells.Add(19, 2, "Total", columnTitleXF192);
                cells.Add(20, 1, "", columnTitleXF41);
                cells.Add(20, 2, "Remarks:          USD: NTD =", columnTitleXF202);
                cells.Add(20, 12, "Total Trip Expense", columnTitleXF208);
                cells.Add(21, 7, "Less: Advance", columnTitleXF215);
                cells.Add(22, 7, "Bal Due to Company", columnTitleXF215);
                cells.Add(23, 7, "Bal. Due to Employee", columnTitleXF215);
                for (int i = 0; i < 12; i++)
                {
                    for (int ii = 0; ii < 9; ii++)
                    {
                        cells.Add(8 + i, 3 + ii, "", columnTitleXF83);
                    }
                }
                for (int i = 0; i < 12; i++)
                {
                    cells.Add(8 + i, 12, "", columnTitleXF812);
                }
                string sqlDetail = "select * from ETraveleDetail where [No]='" + MainID + "'";
                DataTable dtDetail = dbc.GetData("eReimbursement", sqlDetail);
                decimal row1TC = 0M;//第二行数据合计
                decimal row1TP = 0M;//第二行数据合计
                decimal row2TC = 0M;//第二行数据合计
                decimal row2TP = 0M;//第二行数据合计
                decimal row3TC = 0M;//第三行数据合计
                decimal row3TP = 0M;//第三行数据合计
                decimal row4TC = 0M;//第三行数据合计
                decimal row4TP = 0M;//第三行数据合计

                decimal row5TC = 0M;//第三行数据合计
                decimal row5TP = 0M;//第三行数据合计
                decimal row6TC = 0M;//第三行数据合计
                decimal row6TP = 0M;//第三行数据合计
                decimal row7TC = 0M;//第三行数据合计
                decimal row7TP = 0M;//第三行数据合计
                decimal row8TC = 0M;//第三行数据合计
                decimal row8TP = 0M;//第三行数据合计
                decimal row9TC = 0M;//第三行数据合计
                decimal row9TP = 0M;//第三行数据合计
                decimal row10TC = 0M;//第三行数据合计
                decimal row10TP = 0M;//第三行数据合计
                decimal row11TC = 0M;//第三行数据合计
                decimal row11TP = 0M;//第三行数据合计
                for (int p = 0; p < dtTocity.Rows.Count; p++)
                {
                    if (Convert.ToInt32(dtTocity.Rows[p]["Row"].ToString()) == j)
                    {
                        decimal row1Pamount = 0M;//第二行数据
                        decimal row1Camount = 0M;//第二行数据
                        decimal row2Pamount = 0M;//第二行数据
                        decimal row2Camount = 0M;//第二行数据
                        decimal row3Pamount = 0M;//第二行数据
                        decimal row3Camount = 0M;//第二行数据
                        decimal row4Pamount = 0M;//第二行数据
                        decimal row4Camount = 0M;//第二行数据

                        decimal row5Pamount = 0M;//第二行数据
                        decimal row5Camount = 0M;//第二行数据
                        decimal row6Pamount = 0M;//第二行数据
                        decimal row6Camount = 0M;//第二行数据
                        decimal row7Pamount = 0M;//第二行数据
                        decimal row7Camount = 0M;//第二行数据
                        decimal row8Pamount = 0M;//第二行数据
                        decimal row8Camount = 0M;//第二行数据
                        decimal row9Pamount = 0M;//第二行数据
                        decimal row9Camount = 0M;//第二行数据
                        decimal row10Pamount = 0M;//第二行数据
                        decimal row10Camount = 0M;//第二行数据
                        decimal row11Pamount = 0M;//第二行数据
                        decimal row11Camount = 0M;//第二行数据


                        decimal column0TC = 0M;//第二列数据合计
                        decimal column1TP = 0M;//第二列数据合计
                        
                        for (int i = 0; i < dtDetail.Rows.Count; i++)
                        {
                            if (dtDetail.Rows[i]["Tocity"].ToString() == dtTocity.Rows[p]["Tocity"].ToString())
                            {
                                if (dtDetail.Rows[i]["AccountCode"].ToString() == "62012013")
                                {
                                    row2Pamount += 0;
                                    row2Camount += 0;

                                    row2TC += 0;
                                    row2TP += 0;
                                }
                                else if (dtDetail.Rows[i]["AccountCode"].ToString() == "62012023")
                                {
                                    row1Pamount += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row1Camount += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());

                                    row1TC += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row1TP += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                }
                                else if (dtDetail.Rows[i]["AccountCode"].ToString() == "62012011" || dtDetail.Rows[i]["AccountCode"].ToString() == "62012021")
                                {
                                    row3Pamount += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row3Camount += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());

                                    row3TC += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row3TP += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                }
                                else if (dtDetail.Rows[i]["AccountCode"].ToString() == "62010901" || dtDetail.Rows[i]["AccountCode"].ToString() == "62010910" || dtDetail.Rows[i]["AccountCode"].ToString() == "62010920")
                                {
                                    row4Pamount += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row4Camount += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());

                                    row4TC += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row4TP += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                }
                                else if (dtDetail.Rows[i]["AccountCode"].ToString() == "62011901" || dtDetail.Rows[i]["AccountCode"].ToString() == "62011910" || dtDetail.Rows[i]["AccountCode"].ToString() == "62011920" || dtDetail.Rows[i]["AccountCode"].ToString() == "62011930" || dtDetail.Rows[i]["AccountCode"].ToString() == "62011940" || dtDetail.Rows[i]["AccountCode"].ToString() == "62012013")
                                {
                                    row5Pamount += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row5Camount += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());

                                    row5TC += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row5TP += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                }
                                else if (dtDetail.Rows[i]["AccountCode"].ToString() == "62010501" || dtDetail.Rows[i]["AccountCode"].ToString() == "62010510" || dtDetail.Rows[i]["AccountCode"].ToString() == "62010520" || dtDetail.Rows[i]["AccountCode"].ToString() == "62010530" || dtDetail.Rows[i]["AccountCode"].ToString() == "62010540" || dtDetail.Rows[i]["AccountCode"].ToString() == "62010550" || dtDetail.Rows[i]["AccountCode"].ToString() == "62010560")
                                {
                                    row6Pamount += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row6Camount += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());

                                    row6TC += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row6TP += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                }
                                else if (dtDetail.Rows[i]["AccountCode"].ToString() == "62012012")
                                {
                                    row7Pamount += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row7Camount += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());

                                    row7TC += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row7TP += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                }
                                else if (dtDetail.Rows[i]["AccountCode"].ToString() == "62012022")
                                {
                                    row8Pamount += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row8Camount += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());

                                    row8TC += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row8TP += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                }
                                else if (dtDetail.Rows[i]["AccountCode"].ToString() == "62020630")
                                {
                                    row9Pamount += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row9Camount += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());

                                    row9TC += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row9TP += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                }
                                else if (dtDetail.Rows[i]["AccountCode"].ToString() == "62012014" || dtDetail.Rows[i]["AccountCode"].ToString() == "62012024")
                                {
                                    row10Pamount += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row10Camount += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());

                                    row10TC += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row10TP += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                }
                                else
                                {
                                    row11Pamount += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row11Camount += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());

                                    row11TC += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                    row11TP += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                }
                                column0TC += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                column1TP += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                            }
                        }

                        cells.Add(6, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 3, dtTocity.Rows[p]["Tocity"].ToString(), columnTitleXF63);
                        cells.Add(7, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 3, "Reim", columnTitleXF73);
                        cells.Add(7, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 4, "Comp", columnTitleXF73);
                        if (row1Pamount != 0M)
                        {
                            cells.Add(8, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 3, row1Pamount, columnTitleXF83);
                        }
                        if (row1Camount != 0M)
                        {
                            cells.Add(8, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 4, row1Camount, columnTitleXF83);
                        }
                        if (row2Pamount != 0M)
                        {
                            cells.Add(9, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 3, row2Pamount, columnTitleXF83);
                        }
                        if (row2Camount != 0M)
                        {
                            cells.Add(9, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 4, row2Camount, columnTitleXF83);
                        }
                        if (row3Pamount != 0M)
                        {
                            cells.Add(10, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 3, row3Pamount, columnTitleXF83);
                        }
                        if (row3Camount != 0M)
                        {
                            cells.Add(10, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 4, row3Camount, columnTitleXF83);
                        }
                        if (row4Pamount != 0M)
                        {
                            cells.Add(11, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 3, row4Pamount, columnTitleXF83);
                        }
                        if (row4Camount != 0M)
                        {
                            cells.Add(11, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 4, row4Camount, columnTitleXF83);
                        }
                        if (row5Pamount != 0M)
                        {
                            cells.Add(12, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 3, row5Pamount, columnTitleXF83);
                        }
                        if (row5Camount != 0M)
                        {
                            cells.Add(12, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 4, row5Camount, columnTitleXF83);
                        }
                        if (row6Pamount != 0M)
                        {
                            cells.Add(13, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 3, row6Pamount, columnTitleXF83);
                        }
                        if (row6Camount != 0M)
                        {
                            cells.Add(13, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 4, row6Camount, columnTitleXF83);
                        }
                        if (row7Pamount != 0M)
                        {
                            cells.Add(14, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 3, row7Pamount, columnTitleXF83);
                        }
                        if (row7Camount != 0M)
                        {
                            cells.Add(14, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 4, row7Camount, columnTitleXF83);
                        }
                        if (row8Pamount != 0M)
                        {
                            cells.Add(15, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 3, row8Pamount, columnTitleXF83);
                        }
                        if (row8Camount != 0M)
                        {
                            cells.Add(15, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 4, row8Camount, columnTitleXF83);
                        }
                        if (row9Pamount != 0M)
                        {
                            cells.Add(16, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 3, row9Pamount, columnTitleXF83);
                        }
                        if (row9Camount != 0M)
                        {
                            cells.Add(16, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 4, row9Camount, columnTitleXF83);
                        }
                        if (row10Pamount != 0M)
                        {
                            cells.Add(17, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 3, row10Pamount, columnTitleXF83);
                        }
                        if (row10Camount != 0M)
                        {
                            cells.Add(17, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 4, row10Camount, columnTitleXF83);
                        }
                        if (row11Pamount != 0M)
                        {
                            cells.Add(18, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 3, row11Pamount, columnTitleXF83);
                        }
                        if (row11Camount != 0M)
                        {
                            cells.Add(18, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 4, row11Camount, columnTitleXF83);
                        }

                        if (column0TC != 0M)
                        {
                            cells.Add(19, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 3, column0TC, columnTitleXF83);
                        }
                        if (column1TP != 0M)
                        {
                            cells.Add(19, Convert.ToInt32(dtTocity.Rows[p]["SubRow"].ToString()) * 2 + 4, column1TP, columnTitleXF83);
                        }
                    }
                }
                if (row1TC != 0M)
                {
                    cells.Add(8, 11, row1TC, columnTitleXF83);
                }
                if (row1TP != 0M)
                {
                    cells.Add(8, 12, row1TP, columnTitleXF812);
                }
                if (row2TC != 0M)
                {
                    cells.Add(9, 11, row2TC, columnTitleXF83);
                }
                if (row2TP != 0M)
                {
                    cells.Add(9, 12, row2TP, columnTitleXF812);
                }
                if (row3TC != 0M)
                {
                    cells.Add(10, 11, row3TC, columnTitleXF83);
                }
                if (row3TP != 0M)
                {
                    cells.Add(10, 12, row3TP, columnTitleXF812);
                }
                if (row4TC != 0M)
                {
                    cells.Add(11, 11, row4TC, columnTitleXF83);
                }
                if (row4TP != 0M)
                {
                    cells.Add(11, 12, row4TP, columnTitleXF812);
                }
                if (row5TC != 0M)
                {
                    cells.Add(12, 11, row5TC, columnTitleXF83);
                }
                if (row5TP != 0M)
                {
                    cells.Add(12, 12, row5TP, columnTitleXF812);
                }
                if (row6TC != 0M)
                {
                    cells.Add(13, 11, row6TC, columnTitleXF83);
                }
                if (row6TP != 0M)
                {
                    cells.Add(13, 12, row6TP, columnTitleXF812);
                }
                if (row7TC != 0M)
                {
                    cells.Add(14, 11, row7TC, columnTitleXF83);
                }
                if (row7TP != 0M)
                {
                    cells.Add(14, 12, row7TP, columnTitleXF812);
                }
                if (row8TC != 0M)
                {
                    cells.Add(15, 11, row8TC, columnTitleXF83);
                }
                if (row8TP != 0M)
                {
                    cells.Add(15, 12, row8TP, columnTitleXF812);
                }
                if (row9TC != 0M)
                {
                    cells.Add(16, 11, row9TC, columnTitleXF83);
                }
                if (row9TP != 0M)
                {
                    cells.Add(16, 12, row9TP, columnTitleXF812);
                }
                if (row10TC != 0M)
                {
                    cells.Add(17, 11, row10TC, columnTitleXF83);
                }
                if (row10TP != 0M)
                {
                    cells.Add(17, 12, row10TP, columnTitleXF812);
                }
                if (row11TC != 0M)
                {
                    cells.Add(18, 11, row11TC, columnTitleXF83);
                }
                if (row11TP != 0M)
                {
                    cells.Add(18, 12, row11TP, columnTitleXF812);
                }
                decimal tc = row1TC + row2TC + row3TC + row4TC + row5TC + row6TC + row7TC + row8TC + row9TC + row10TC + row11TC;
                if (tc!=0M)
                {
                    cells.Add(19, 11, tc, columnTitleXF83);
                }
                decimal tp = row1TP + row2TP + row3TP + row4TP + row5TP + row6TP + row7TP + row8TP + row9TP + row11TP;
                if (tp != 0M)
                {
                    cells.Add(19, 12, tp, columnTitleXF812);
                }
            }
            xls.Send();
        }
    }
}