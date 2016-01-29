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
using org.in2bits.MyXls;
namespace eReimbursement
{
    public partial class FileManagement : App_Code.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "$('div.gn_person ul.q-menubox li:eq(0) a').text('" + Request.Cookies.Get("eReimUserName").Value + "');", true); X.AddScript("loginWindow.hide();Panel1.enable();");
                }
                string sqltype = "";
                string sqldraft = "";
                string sqlProcess = "";
                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                {
                    PagingToolbar1.DisplayMsg = "显示 {0} - {1} of {2}";
                    ResourceManager1.Locale = "zh-CN";
                    sqltype += "select [Text]=CText,[Value]=CValue from Edic where KeyValue='MainType'";
                    sqldraft += "select [Text]=CText,[Value]=CValue from Edic where KeyValue='Type'";
                    sqlProcess += "select [Text]=CText,[Value]=CValue from Edic where KeyValue='Status'";
                }
                else
                {
                    PagingToolbar1.DisplayMsg = "Displaying items {0} - {1} of {2}";
                    ResourceManager1.Locale = "en-US";
                    sqltype += "select [Text]=EText,[Value]=CValue from Edic where KeyValue='MainType'";
                    sqldraft += "select [Text]=EText,[Value]=CValue from Edic where KeyValue='Type'";
                    sqlProcess += "select [Text]=EText,[Value]=CValue from Edic where KeyValue='Status'";
                }
                
                DataTable dttype = dbc.GetData("eReimbursement", sqltype);
                StoreType.DataSource = dttype;
                StoreType.DataBind();
                DataTable dtdraft = dbc.GetData("eReimbursement", sqldraft);
                StoreDraft.DataSource = dtdraft;
                StoreDraft.DataBind();
                DataTable dtProcess = dbc.GetData("eReimbursement", sqlProcess);
                StoreProcess.DataSource = dtProcess;
                StoreProcess.DataBind();
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
                                if (dtStation.Rows[g]["Text"].ToString()==dd[i].Trim())
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
                //获取被授权站点下的所有申请人
                string station = "";
                for (int i = 0; i < dtStation.Rows.Count; i++)
                {
                    station += "'" + dtStation.Rows[i]["Text"].ToString() + "',";
                }
                if (!string.IsNullOrEmpty(station))
                {
                    station = " and Station in (" + station.Substring(0, station.Length - 1) + ") ";
                }
                string sqlgetPerson = "";
                if (station.Length>0)
                {
                    sqlgetPerson = "select distinct [Text]=Person,[Value]=PersonID from V_Eflow_ETravel where (Active=1 or Active=2) and Person!='' " + station;
                }
                else
                {
                    sqlgetPerson = "select distinct [Text]=Person,[Value]=PersonID from V_Eflow_ETravel where (Active=1 or Active=2) and Person!='' and FlowID is null";
                }
                DataTable dtperson = dbc.GetData("eReimbursement", sqlgetPerson);
                dtperson.DefaultView.Sort = "Text ASC";
                StorePerson.DataSource = dtperson.DefaultView.ToTable();
                StorePerson.DataBind();
                //获取被授权站点下的所有提单人
                string sqlgetCreatedBy = "";
                if (station.Length > 0)
                {
                    sqlgetCreatedBy = "select distinct [Text]=CreadedBy,[Value]=CreadedByID from V_Eflow_ETravel where (Active=1 or Active=2) and CreadedBy!='' " + station;
                }
                else
                {
                    sqlgetCreatedBy = "select distinct [Text]=CreadedBy,[Value]=CreadedByID from V_Eflow_ETravel where (Active=1 or Active=2) and CreadedBy!='' and FlowID is null";
                }
                DataTable dtcreatedby = dbc.GetData("eReimbursement", sqlgetCreatedBy);
                dtcreatedby.DefaultView.Sort = "Text ASC";
                StoreCreatedBy.DataSource = dtcreatedby.DefaultView.ToTable();
                StoreCreatedBy.DataBind();
            }
        }
        protected void Search(object sender, DirectEventArgs e)
        {
            cs.DBCommand dbc = new cs.DBCommand();
            string sql = "";

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
            if (!string.IsNullOrEmpty(cbxCreatedBy.Text))
            {
                sql += "and CreadedByID='" + cbxCreatedBy.Text + "' ";
            }
            if (!string.IsNullOrEmpty(cbxProcess.Text))
            {
                sql += "and Status='" + cbxProcess.Text + "' ";
                //switch (cbxProcess.Text)
                //{
                //    case "待批":
                //        sql += "and Status=1 ";
                //        break;
                //    case "Pending":
                //        sql += "and Status=1 ";
                //        break;
                //    case "已批":
                //        sql += "and Status=2 ";
                //        break;
                //    case "Approved":
                //        sql += "and Status=2 ";
                //        break;
                //    case "已拒绝":
                //        sql += "and Status=3 ";
                //        break;
                //    case "Rejected":
                //        sql += "and Status=3 ";
                //        break;
                //    case "待申请":
                //        sql += "and Status=0 ";
                //        break;
                //    case "Not Apply":
                //        sql += "and Status=0 ";
                //        break;
                //    default:
                //        break;
                //}
            }
            if (!string.IsNullOrEmpty(cbxStation.Text))
            {
                sql += "and Station='" + cbxStation.Text + "' ";
            }
            else
            {
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
                //获取被授权站点下的所有申请人
                string station = "";
                for (int i = 0; i < dtStation.Rows.Count; i++)
                {
                    station += "'" + dtStation.Rows[i]["Text"].ToString() + "',";
                }
                if (!string.IsNullOrEmpty(station))
                {
                    station = " and Station in (" + station.Substring(0, station.Length - 1) + ") ";
                    sql += station;
                }
                else
                {
                    sql += " and FlowID is null ";
                }
            }
            string sql1 = "select t1.*";
            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
            {
                sql1 += ",[Status1]=TDicStatus.CText,[Type1]=TDicMainType.CText,[Draft1]=TDicType.CText";
            }
            else
            {
                sql1 += ",[Status1]=TDicStatus.EText,[Type1]=TDicMainType.EText,[Draft1]=TDicType.EText";
            }
            sql1 += " from (select [Draft]=case when [Status]=0 then 1 else 0 end,* from V_Eflow_ETravel where FlowID>16097 and (Active=1 or Active=2) " + sql + ") t1";
            sql1 += " left join (select * from Edic where KeyValue='MainType') TDicMainType on TDicMainType.CValue=t1.Type";
            sql1 += " left join (select * from Edic where KeyValue='Status') TDicStatus on TDicStatus.CValue=t1.Status";
            sql1 += " left join (select * from Edic where KeyValue='Type') TDicType on TDicType.CValue=t1.Draft";

            
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
            //151010,解析申请人Cost Center by Andy Kang
            dtnew.Columns.Add("PersonID", System.Type.GetType("System.String"));
            dtnew.Columns.Add("CostCenter", System.Type.GetType("System.String"));
            dtnew.Columns.Add("CostCenterNew", System.Type.GetType("System.String"));
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
                dr["Draft"] = dtdetail.Rows[i]["Draft"].ToString();
                dr["RequestID"] = dtdetail.Rows[i]["RequestID"].ToString();
                dr["Status1"] = dtdetail.Rows[i]["Status1"].ToString();
                dr["Type1"] = dtdetail.Rows[i]["Type1"].ToString();
                dr["Draft1"] = dtdetail.Rows[i]["Draft1"].ToString();
                //151010,解析申请人Cost Center by Andy Kang
                dr["PersonID"] = dtdetail.Rows[i]["PersonID"].ToString();
                dr["CostCenter"] = dtdetail.Rows[i]["CostCenter"].ToString();
                DataSet ds1 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtdetail.Rows[i]["PersonID"].ToString());
                if (ds1.Tables[0].Rows.Count == 1)
                {
                    DataTable dt1 = ds1.Tables[0];
                    dr["CostCenterNew"] = dt1.Rows[0]["CostCenter"].ToString();
                }
                else
                {
                    dr["CostCenterNew"] = "N/A";
                }

                dtnew.Rows.Add(dr);
            }
            Store1.DataSource = dtnew;
            Store1.DataBind();
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
                    //        Response.Cookies["eReimCostCenter"].Value = dttemp.Rows[0]["Station"].ToString();  //将值写入到客户端硬盘Cookie
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
            cells.Add(1, 3, "Amount", titleXF1);
            cells.Add(1, 4, "Owner", titleXF1);
            cells.Add(1, 5, "Process", titleXF1);
            cells.Add(1, 6, "Current Approver", titleXF1);
            cells.Add(1, 7, "Submit Date", titleXF1);
            cells.Add(1, 8, "Remark", titleXF1);

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
                    cells.Add(2 + i, 3, Convert.ToDouble(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Tamount").Item(0).InnerXml), titleXF);
                }
                else
                {
                    cells.Add(2 + i, 3, "", titleXF);
                }
                cells.Add(2 + i, 1, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("No").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 2, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Type1").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 4, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Person").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 5, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Status1").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 6, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Approver").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 7, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("CreadedDate").Item(0).InnerXml, titleXF);
                cells.Add(2 + i, 8, doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).SelectNodes("Remark").Item(0).InnerXml, titleXF);
            }

            xls.Send();
        }
    }
}