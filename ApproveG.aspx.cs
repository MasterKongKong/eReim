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
    public partial class ApproveG : App_Code.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                //检查是否登录超时
                if (Request.Cookies.Get("eReimUserID") == null)
                {
                    if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                    {
                        X.AddScript("Ext.Msg.show({ title: '提示', msg: '登录超时,将刷新页面.', buttons: { ok: 'Ok' }, fn: function (btn) { parent.location.reload(); } });");
                    }
                    else
                    {
                        X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Time out,reloading...', buttons: { ok: 'Ok' }, fn: function (btn) { parent.location.reload(); } });");
                    }
                    return;
                }
                if (Request.QueryString["RequestID"] != null)//判断链接地址是否正确
                {
                    string ID = Request.QueryString["RequestID"].ToString();
                    System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^\d*$");
                    if (reg1.IsMatch(ID))
                    {
                        string sql = "select * from Ecommon where ID='" + ID + "'";
                        cs.DBCommand dbc = new cs.DBCommand();
                        DataTable dt = new DataTable();
                        dt = dbc.GetData("eReimbursement", sql);
                        if (dt != null && dt.Rows.Count == 1)
                        {
                            LabelPerson.Text = dt.Rows[0]["Person"].ToString();
                            LabelStation.Text = dt.Rows[0]["Station"].ToString();
                            LabelDepartment.Text = dt.Rows[0]["Department"].ToString();
                            LabelMonth.Text = Convert.ToDateTime(dt.Rows[0]["ApplyDate"].ToString()).Month.ToString();
                            //LabelSum.Text = dt.Rows[0]["Tamount"].ToString();
                            LabelSum.Text = dt.Rows[0]["Tamount"].ToString() == "" ? "0" : Convert.ToDecimal(dt.Rows[0]["Tamount"].ToString()).ToString("#,##0.00");
                            LabelRemark.Text = dt.Rows[0]["Remark"].ToString();
                            //160119 垫付
                            LabelBehalfPersonName.Text = dt.Rows[0]["OnBehalfPersonName"].ToString();
                            LabelBehalfCost.Text = dt.Rows[0]["OnBehalfPersonCostCenter"].ToString();
                            //根据语言设置
                            string sqlp = "";
                            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                            {
                                sqlp += ",[SubType]=case when t1.Type='O' then t2.ADes else TDicSubType.CText end,[COAName]=case when t1.Type='O' then t2.ADes else '' end,[EType1]=t3.CText,[PaymentType1]=t4.CText";
                            }
                            else
                            {
                                sqlp += ",[SubType]=case when t1.Type='O' then t2.SAccountName else TDicSubType.EText end,[COAName]=case when t1.Type='O' then t2.SAccountName else '' end,[EType1]=t3.EText,[PaymentType1]=t4.EText";
                            }
                            //if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                            //{
                            //    sqlp += ",[SubType]=TDicSubType.CText,[COAName]=case when t1.Type='O' then t2.ADes else '' end,[EType1]=t3.CText,[PaymentType1]=t4.CText";
                            //}
                            //else
                            //{
                            //    sqlp += ",[SubType]=TDicSubType.EText,[COAName]=case when t1.Type='O' then t2.SAccountName else '' end,[EType1]=t3.EText,[PaymentType1]=t4.EText";
                            //}
                            string detailsql = "select t1.*" + sqlp + " from EeommonDetail t1 left join (select * from Edic where KeyValue='EType') t3 on t3.CValue=t1.EType left join (select * from Edic where KeyValue='SubType') TDicSubType on TDicSubType.CValue=t1.Type left join AccoundCode t2 on t2.SAccountCode=t1.AccountCode left join (select * from Edic where KeyValue='PayType') t4 on t4.CValue=t1.PaymentType where t1.No='" + ID + "'";
                            
                            //string detailsql = "select * from EeommonDetail where No='" + ID + "'";
                            DataTable dtdetail = new DataTable();
                            dtdetail = dbc.GetData("eReimbursement", detailsql);
                            LabelCur.Text = dtdetail.Rows[0]["Cur"].ToString();
                            DataTable dtnew = new DataTable();
                            dtnew.Columns.Add("DetailID", System.Type.GetType("System.String"));
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
                            dtnew.Columns.Add("SubType", System.Type.GetType("System.String"));
                            dtnew.Columns.Add("COAName", System.Type.GetType("System.String"));
                            dtnew.Columns.Add("EffectTime", System.Type.GetType("System.String"));
                            dtnew.Columns.Add("StationBudget", System.Type.GetType("System.String"));
                            dtnew.Columns.Add("DepartmentBudget", System.Type.GetType("System.String"));
                            dtnew.Columns.Add("PersonBudget", System.Type.GetType("System.String"));
                            dtnew.Columns.Add("PersonBudgetYTD", System.Type.GetType("System.String"));
                            dtnew.Columns.Add("PersonYTD", System.Type.GetType("System.String"));
                            dtnew.Columns.Add("DepartmentYTD", System.Type.GetType("System.String"));
                            dtnew.Columns.Add("StationYTD", System.Type.GetType("System.String"));
                            dtnew.Columns.Add("Department1", System.Type.GetType("System.String"));
                            dtnew.Columns.Add("Vendor", System.Type.GetType("System.String"));
                            dtnew.Columns.Add("PaymentType1", System.Type.GetType("System.String"));
                            dtnew.Columns.Add("PaymentDate", System.Type.GetType("System.String"));

                            dtnew.Columns.Add("Year", System.Type.GetType("System.String"));//记录日期的年份
                            dtnew.Columns.Add("Month", System.Type.GetType("System.String"));//记录日期的月份

                            dtnew.Columns.Add("StationU", typeof(System.Decimal));//记录当月站点已用预算
                            dtnew.Columns.Add("DepU", typeof(System.Decimal));//记录当月部门已用预算
                            dtnew.Columns.Add("PersonU", typeof(System.Decimal));//记录当月个人已用预算
                            dtnew.Columns.Add("StationB", typeof(System.Decimal));//记录当月站点全部预算
                            dtnew.Columns.Add("DepB", typeof(System.Decimal));//记录当月部门全部预算
                            dtnew.Columns.Add("PersonB", typeof(System.Decimal));//记录当月个人全部预算

                            dtnew.Columns.Add("StationUYTD", typeof(System.Decimal));//记录全年站点已用预算
                            dtnew.Columns.Add("DepUYTD", typeof(System.Decimal));//记录全年部门已用预算
                            dtnew.Columns.Add("PersonUYTD", typeof(System.Decimal));//记录全年个人已用预算
                            dtnew.Columns.Add("StationBYTD", typeof(System.Decimal));//记录全年站点全部预算
                            dtnew.Columns.Add("DepBYTD", typeof(System.Decimal));//记录全年部门全部预算
                            dtnew.Columns.Add("PersonBYTD", typeof(System.Decimal));//记录全年个人全部预算
                            decimal sum = 0;
                            for (int i = 0; i < dtdetail.Rows.Count; i++)
                            {
                                DataRow dr = dtnew.NewRow();
                                dr["DetailID"] = dtdetail.Rows[i]["ID"].ToString();
                                dr["Type"] = dtdetail.Rows[i]["Type"].ToString();
                                dr["AccountName"] = dtdetail.Rows[i]["AccountName"].ToString();
                                dr["AccountCode"] = dtdetail.Rows[i]["AccountCode"].ToString();
                                dr["AccountDes"] = dtdetail.Rows[i]["AccountDes"].ToString();
                                dr["Cur"] = dtdetail.Rows[i]["Cur"].ToString();
                                dr["Amount"] = dtdetail.Rows[i]["Amount"].ToString();
                                dr["TSation"] = dtdetail.Rows[i]["TSation"].ToString();
                                dr["Tdate"] = dtdetail.Rows[i]["Tdate"].ToString() == "" ? "" : Convert.ToDateTime(dtdetail.Rows[i]["Tdate"].ToString()).ToString("yyyy/MM/dd");
                                dr["Year"] = Convert.ToDateTime(dtdetail.Rows[i]["Tdate"].ToString()).ToString("yyyy/MM/dd").Split('/')[0];
                                dr["Month"] = Convert.ToDateTime(dtdetail.Rows[i]["Tdate"].ToString()).ToString("yyyy/MM/dd").Split('/')[1];
                                dr["Fcity"] = dtdetail.Rows[i]["Fcity"].ToString();
                                dr["Tcity"] = dtdetail.Rows[i]["Tcity"].ToString();
                                dr["Attach"] = dtdetail.Rows[i]["Attach"].ToString();
                                dr["EType"] = dtdetail.Rows[i]["Type"].ToString() == "E" ? dtdetail.Rows[i]["EType1"].ToString() : "";
                                dr["EcompanyCode"] = dtdetail.Rows[i]["EcompanyCode"].ToString();
                                dr["Ecompany"] = dtdetail.Rows[i]["Ecompany"].ToString();
                                dr["Eperson"] = dtdetail.Rows[i]["Eperson"].ToString();
                                dr["Epurpos"] = dtdetail.Rows[i]["Epurpos"].ToString();
                                dr["SubType"] = dtdetail.Rows[i]["SubType"].ToString();
                                dr["COAName"] = dtdetail.Rows[i]["COAName"].ToString();
                                dr["EffectTime"] = dtdetail.Rows[i]["EffectTime"].ToString();
                                dr["Department1"] = dtdetail.Rows[i]["Department1"].ToString();

                                //dr["Vendor"] = dtdetail.Rows[i]["Vendor"].ToString();
                                //dr["PaymentType1"] = dtdetail.Rows[i]["PaymentType1"].ToString();
                                //dr["PaymentDate"] = dtdetail.Rows[i]["PaymentDate"].ToString() == "" ? "" : Convert.ToDateTime(dtdetail.Rows[i]["PaymentDate"].ToString()).ToString("yyyy/MM/dd");
                                dtnew.Rows.Add(dr);
                                sum += dtdetail.Rows[i]["Amount"].ToString() == "" ? 0 : Convert.ToDecimal(dtdetail.Rows[i]["Amount"].ToString());
                            }

                            Store1.DataSource = dtnew;
                            Store1.DataBind();

                            //140226 显示预算
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
                            if ((dt.Rows[0]["Status"].ToString() == "2" || dt.Rows[0]["Status"].ToString() == "3") && dtnn.Rows.Count > 0)
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
                                        Header = "%(Current+Used/Budget)",
                                        Renderer = new Renderer { Fn = "GetNumberPercent" },
                                        Sortable = false,
                                        Resizable = false,
                                        MenuDisabled = true,
                                        Width = 160
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
                                        Width = 160
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
                                        Width = 160
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
                                string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dt.Rows[0]["Station2"].ToString());
                                

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
                                //string userid = dt.Rows[0]["PersonID"].ToString();
                                //string ostation = ""; string station = ""; string department = "";
                                //DataSet ds2 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(userid);
                                //if (ds2.Tables[0].Rows.Count == 1)
                                //{
                                //    DataTable dt1 = ds2.Tables[0];
                                //    ostation = dt1.Rows[0]["CostCenter"].ToString();//记录用户预算站点,即CostCenter
                                //    station = dt1.Rows[0]["stationCode"].ToString();//记录用户所在站点
                                //    department = dt1.Rows[0]["CRPDepartmentName"].ToString();
                                //}
                                //160119 垫付
                                string userid = dt.Rows[0]["OnBehalfPersonID"].ToString() == "" ? dt.Rows[0]["PersonID"].ToString() : dt.Rows[0]["OnBehalfPersonID"].ToString();
                                string department = dt.Rows[0]["OnBehalfPersonID"].ToString() == "" ? dt.Rows[0]["Department"].ToString() : dt.Rows[0]["OnBehalfPersonDept"].ToString();
                                string ostation = dt.Rows[0]["Station2"].ToString();//预算站点,与基本信息中的CostCenter一致(Station2)

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
                                        dtC = Comm.ExRtnEB(userid, department, ostation, ostation, accountcode, dtB.Rows[g]["Year"].ToString(), "1","G",ID);
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
                                        decimal PPercent = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[i]["Current"].ToString())+Convert.ToDecimal(dtbudget.Rows[i]["PU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()), 2);
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
                                        decimal DPercent = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtbudget.Rows[i]["DU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()), 2);
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
                                        decimal SPercent = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtbudget.Rows[i]["SU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()), 2);
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
                                //如果个人,部门,站点,全未分配预算,则判断为Un-Budget
                                if (!UnBudget)
                                {
                                    if (!PB && !DB && !SB)
                                    {
                                        UnBudget = true;
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
                                        Header = "%(Current+Used/Budget)",
                                        Renderer = new Renderer { Fn = "GetNumberPercent" },
                                        Sortable = false,
                                        Resizable = false,
                                        MenuDisabled = true,
                                        Width = 160
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
                                        Width = 160
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
                                        Width = 160
                                    });
                                }
                            }
                            string sqlflow = "select * from V_Eflow_ETravel where RequestID='" + ID + "' and [Type]='G' order by Step,FlowID";
                            DataTable dtflow = new DataTable();
                            dtflow = dbc.GetData("eReimbursement", sqlflow);
                            //160119 垫付,如果登录用户是被垫付人审批人之一,则显示预算,否则不显示
                            if (dtflow != null && dtflow.Rows.Count > 0)
                            {
                                if (dtflow.Select("ApproverID='" + Request.Cookies.Get("eReimUserID").Value + "' and FPersonID=OnBehalfPersonID").Count() > 0 || dtflow.Select("ApproverID='" + Request.Cookies.Get("eReimUserID").Value + "' and OnBehalfPersonID is null").Count() > 0)
                                {
                                    StoreBudget.DataSource = dtbudget;
                                    StoreBudget.DataBind();
                                }
                                
                            }

                            if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                            {
                                GridPanel1.Title = "张数合计: " + dtdetail.Rows.Count.ToString() + ",金额合计: " + sum.ToString("#,##0.00");
                            }
                            else
                            {
                                GridPanel1.Title = "Total sheets: " + dtdetail.Rows.Count.ToString() + ",Sum: " + sum.ToString("#,##0.00");
                            }

                            
                            if (dtflow != null && dtflow.Rows.Count > 0)
                            {
                                string html = "";
                                int countdiv = 0;
                                bool tijiao = false;
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
                                for (int i = 0; i < dtflow.Rows.Count; i++)
                                {
                                    if (dtflow.Rows[i]["Step"].ToString() == "0")
                                    {
                                        html += "<div class=\"StatusIcon Pending\">";
                                        html += "<span class=\"spanIcon\">" + status1 + "<br />";
                                        html += dtflow.Rows[i]["Person"].ToString();
                                        html += "</span><b class=\"bIcon bIcon1\"></b>";
                                        html += "</div>";
                                        countdiv++;
                                        break;
                                    }
                                    else
                                    {
                                        if (dtflow.Rows[i]["Step"].ToString() == "1" && !tijiao)
                                        {
                                            html += "<div class=\"StatusIcon StatusIcon0\">";
                                            html += "<span class=\"spanIcon\">" + status2 + "<br />";
                                            html += dtflow.Rows[i]["CreadedBy"].ToString();
                                            html += dtflow.Rows[i]["CreadedDate"].ToString() == "" ? "" : "<br />" + Convert.ToDateTime(dtflow.Rows[i]["CreadedDate"].ToString()).ToString("yyyy/MM/dd");
                                            html += "</span><b class=\"bIcon bIcon1\"></b>";
                                            html += "</div>";
                                            tijiao = true;
                                            countdiv++;
                                        }
                                        //if (dtflow.Rows[i]["Status"].ToString() == "1")
                                        //{
                                        //    html += "<div class=\"StatusIcon Pending\">";
                                        //    html += "<span class=\"spanIcon\">" + status3 + "<br />";
                                        //    html += dtflow.Rows[i]["Approver"].ToString();
                                        //    html += "</span><b class=\"bIcon bIcon1\"></b>";
                                        //    html += "</div>";
                                        //    countdiv++;
                                        //}
                                        //else if (dtflow.Rows[i]["Status"].ToString() == "2")
                                        //{
                                        //    html += "<div class=\"StatusIcon Approve\">";
                                        //    html += "<span class=\"spanIcon\">" + status4 + "<br />";
                                        //    html += dtflow.Rows[i]["Approver"].ToString();
                                        //    html += dtflow.Rows[i]["ApproveDate"].ToString() == "" ? "" : "<br />" + Convert.ToDateTime(dtflow.Rows[i]["ApproveDate"].ToString()).ToString("yyyy/MM/dd");
                                        //    html += "</span><b class=\"bIcon bIcon1\"></b>";
                                        //    html += "</div>";
                                        //    countdiv++;
                                        //}
                                        if (dtflow.Rows[i]["Status"].ToString() == "1")
                                        {
                                            string msg = "";
                                            if (dtflow.Rows[i]["FlowFn"].ToString().ToLower() == "verifier")
                                            {
                                                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                                {
                                                    msg = "待查.";
                                                }
                                                else
                                                {
                                                    msg = "To Be Verified by";
                                                }
                                            }
                                            else if (dtflow.Rows[i]["FlowFn"].ToString().ToLower() == "issuer")
                                            {
                                                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                                {
                                                    msg = "待发放.";
                                                }
                                                else
                                                {
                                                    msg = "To Be Issued by";
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
                                            html += dtflow.Rows[i]["Approver"].ToString();
                                            html += "</span><b class=\"bIcon bIcon1\"></b>";
                                            html += "</div>";
                                            countdiv++;
                                        }
                                        else if (dtflow.Rows[i]["Status"].ToString() == "2")
                                        {
                                            string msg = "";
                                            if (dtflow.Rows[i]["FlowFn"].ToString().ToLower() == "verifier")
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
                                            else if (dtflow.Rows[i]["FlowFn"].ToString().ToLower() == "issuer")
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
                                            html += dtflow.Rows[i]["Approver"].ToString();
                                            html += dtflow.Rows[i]["ApproveDate"].ToString() == "" ? "" : "<br />" + Convert.ToDateTime(dtflow.Rows[i]["ApproveDate"].ToString()).ToString("yyyy/MM/dd");
                                            html += "</span><b class=\"bIcon bIcon1\"></b>";
                                            html += "</div>";
                                            countdiv++;
                                        }
                                        else if (dtflow.Rows[i]["Status"].ToString() == "3")
                                        {
                                            html += "<div class=\"StatusIcon Reject\">";
                                            html += "<span class=\"spanIcon\">" + status5 + "<br />";
                                            html += dtflow.Rows[i]["Approver"].ToString();
                                            html += dtflow.Rows[i]["ApproveDate"].ToString() == "" ? "" : "<br />" + Convert.ToDateTime(dtflow.Rows[i]["ApproveDate"].ToString()).ToString("yyyy/MM/dd");
                                            html += "</span><b class=\"bIcon bIcon1\"></b>";
                                            html += "</div>";
                                            countdiv++;
                                        }
                                        if (dtflow.Rows[i]["Active"].ToString() == "2")
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

                            if (Request.QueryString["Status"].ToString() != "1")
                            {
                                X.AddScript("Button1.disable();Button2.disable();Button3.disable();");
                            }
                            if (Request.QueryString["ID"] != null)
                            {
                                string Status = Request.QueryString["ID"].ToString();
                                System.Text.RegularExpressions.Regex reg2 = new System.Text.RegularExpressions.Regex(@"^\d*$");
                                if (reg2.IsMatch(Status))
                                {
                                    //string lingsql = "select * from V_Eflow_ETravel where FlowID=" + Status;
                                    //DataTable dtlink = new DataTable();
                                    //dtlink = dbc.GetData("eReimbursement", lingsql);
                                    //if (dtlink != null && dtlink.Rows.Count == 1)
                                    //{
                                    //    if (Request.Cookies.Get("eReimUserID").Value != dtlink.Rows[0]["ApproverID"].ToString())
                                    //    {
                                    //        ErrorHandle("No right."); return;
                                    //    }
                                    //    txtRemark.Text = dtlink.Rows[0]["RemarkFlow"].ToString();
                                    //}
                                    //string sqlr = "select * from V_Eflow_ETravel where [Type]='G' and RequestID=(select RequestID from V_Eflow_ETravel where FlowID="+Status+" and Type='G') order by cast(Step as int)";
                                    //DataTable dtr = new DataTable();
                                    //dtr = dbc.GetData("eReimbursement", sqlr);
                                    if (dtflow!=null)
                                    {
                                        //151029,如果登陆人是历史审批人,则有权查看
                                        if (dtflow.Select("ApproverID='" + Request.Cookies.Get("eReimUserID").Value + "'").Count() > 0)
                                        {
                                            string remark = "";
                                            for (int i = 0; i < dtflow.Rows.Count; i++)
                                            {
                                                if (dtflow.Rows[i]["FlowID"].ToString() == Status)
                                                {
                                                    if (dtflow.Rows[i]["Active"].ToString()=="1")
                                                    {
                                                        if (Request.Cookies.Get("eReimUserID").Value == dtflow.Rows[i]["ApproverID"].ToString())
                                                        {
                                                            //txtRemark.Text = dtflow.Rows[i]["RemarkFlow"].ToString();
                                                            //if (dtflow.Rows[i]["RemarkFlow"].ToString().Trim().Length != 0)
                                                            //{
                                                            //    remark += dtflow.Rows[i]["Approver"].ToString() + ":" + dtflow.Rows[i]["RemarkFlow"].ToString() + " ";
                                                            //}
                                                            if (dtflow.Rows[0]["OnBehalfPersonID"].ToString()=="")
                                                            {
                                                                X.AddScript("Button1.enable();Button2.enable();Button3.enable();");
                                                            }
                                                            else
                                                            {
                                                                X.AddScript("Button1.enable();Button2.enable();Button3.disable();");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            X.AddScript("Button1.disable();Button2.disable();Button3.disable();");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        txtRemark.Text = dtflow.Rows[i]["RemarkFlow"].ToString();
                                                        if (dtflow.Rows[i]["RemarkFlow"].ToString().Trim().Length != 0)
                                                        {
                                                            remark += dtflow.Rows[i]["Approver"].ToString() + ":" + dtflow.Rows[i]["RemarkFlow"].ToString() + " ";
                                                        }
                                                        X.AddScript("Button1.disable();Button2.disable();Button3.disable();");
                                                    }
                                                }
                                                else
                                                {
                                                    if (dtflow.Rows[i]["RemarkFlow"].ToString().Trim().Length != 0)
                                                    {
                                                        remark += dtflow.Rows[i]["Approver"].ToString() + ":" + dtflow.Rows[i]["RemarkFlow"].ToString() + " ";
                                                    }
                                                }
                                            }
                                            if (remark.Length == 0)
                                            {
                                                Label11.Text = "";
                                            }
                                            else
                                            {
                                                LabelRemarkFlow.Text = remark;
                                            }
                                        }
                                        else
                                        {
                                            ErrorHandle("No right.");
                                            X.AddScript("Button1.disable();Button2.disable();Button3.disable();");
                                            return;
                                        }
                                        
                                    }
                                }
                            }
                            hdTravelRequestID.Value = Request.QueryString["RequestID"].ToString();
                        }
                    }
                }
            }
        }
        protected void Save(object sender, DirectEventArgs e)
        {
            //检查是否登录超时
            if (Request.Cookies.Get("eReimUserID") == null)
            {
                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                {
                    X.AddScript("Ext.Msg.show({ title: '提示', msg: '登录超时,将刷新页面.', buttons: { ok: 'Ok' }, fn: function (btn) { window.location.reload(); } });");
                }
                else
                {
                    X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Time out,reloading...', buttons: { ok: 'Ok' }, fn: function (btn) { window.location.reload(); } });");
                }
                return;
            }
            cs.DBCommand dbc = new cs.DBCommand();
            string para = e.ExtraParams[0].Value;
            string ID = Request.QueryString["RequestID"].ToString();
            SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eReimbursement"].ConnectionString);
            //string sqlmaxid = "select maxid=max(id) from Eflow where Step=(select maxstep=max(Step) from Eflow where RequestID='" + Request.QueryString["RequestID"].ToString() + "') and RequestID='" + Request.QueryString["RequestID"].ToString() + "'";
            //DataTable dtmax = new DataTable();
            //dtmax = dbc.GetData("eReimbursement", sqlmaxid);
            //string maxid = dtmax.Rows[0][0].ToString();
            string sql = "select * from Eflow where RequestID='" + Request.QueryString["RequestID"].ToString() + "' and [Type]='" + Request.QueryString["Type"].ToString() + "' order by Step,id";
            
            DataTable dt = new DataTable();
            dt = dbc.GetData("eReimbursement", sql);
            string sqlf = "select * from Ecommon where ID=" + ID;
            DataTable dtf = dbc.GetData("eReimbursement", sqlf);
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["Active"].ToString() == "1")
                    {
                        if (para == "3")
                        {
                            string updatesql = "update Eflow set Active=2,Status=3,Remark='" + txtRemark.Text.Replace("'", "''") + "',ApproveDate='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' where id=" + dt.Rows[i]["id"].ToString();
                            if (dt.Rows[i]["Type"].ToString() == "T")
                            {
                                updatesql += ";update ETravel set Status=3 where ID=" + dt.Rows[i]["RequestID"].ToString();
                            }
                            else
                            {
                                updatesql += ";update Ecommon set Status=3 where ID=" + dt.Rows[i]["RequestID"].ToString();
                            }
                            string newid = dbc.UpdateData("eReimbursement", updatesql, "Update");
                            if (newid == "-1")
                            {
                                ErrorHandle("Data Error");
                                return;
                            }
                            else
                            {
                                //保存预算信息
                                try
                                {
                                    //string sqld = "select * from ETraveleDetail where [No]='" + ID + "' order by id";
                                    //DataTable dtall = new DataTable();
                                    //dtall = dbc.GetData("eReimbursement", sqld);
                                    ////预算
                                    ////140226 显示预算
                                    //DataTable dtbudget = new DataTable();
                                    //dtbudget.Columns.Add("RequestID", typeof(System.Int16));
                                    //dtbudget.Columns.Add("Status", typeof(System.Int16));
                                    //dtbudget.Columns.Add("EName", typeof(System.String));
                                    //dtbudget.Columns.Add("COACode", typeof(System.String));
                                    //dtbudget.Columns.Add("LocalCur", typeof(System.String));
                                    //dtbudget.Columns.Add("CenterCur", typeof(System.String));
                                    //dtbudget.Columns.Add("Rate", typeof(System.Decimal));
                                    //dtbudget.Columns.Add("Current", typeof(System.Decimal));
                                    //dtbudget.Columns.Add("PU", typeof(System.Decimal));
                                    //dtbudget.Columns.Add("PB", typeof(System.Decimal));
                                    //dtbudget.Columns.Add("PPercent", typeof(System.Decimal));
                                    //dtbudget.Columns.Add("DU", typeof(System.Decimal));
                                    //dtbudget.Columns.Add("DB", typeof(System.Decimal));
                                    //dtbudget.Columns.Add("DPercent", typeof(System.Decimal));
                                    //dtbudget.Columns.Add("SU", typeof(System.Decimal));
                                    //dtbudget.Columns.Add("SB", typeof(System.Decimal));
                                    //dtbudget.Columns.Add("SPercent", typeof(System.Decimal));

                                    ////取得预算日期
                                    //string sqlA = "select convert(varchar(10),min(Tdate0),111) as BudgetDate from ETraveleDetail where No='" + ID + "'";
                                    //DataTable dtA = dbc.GetData("eReimbursement", sqlA);
                                    ////取得本币与成本中心汇率转换
                                    //decimal rate = 1;
                                    //string CurLocal = dtall.Rows[0]["Cur"].ToString();
                                    //string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dtall.Rows[0]["TSation"].ToString());
                                    //if (CurLocal != CurBudget)
                                    //{
                                    //    rate = DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(CurBudget, CurLocal, Convert.ToDateTime(dtA.Rows[0]["BudgetDate"].ToString()).Year);
                                    //}

                                    ////取得4大类合计
                                    ////string sqlB = "select sum(T1) as T1,sum(T2) as T2,sum(T3) as T3,sum(T4) as T4 from (select case when AccountCode='62012000' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T1],case when AccountCode='62010900' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T2],case when AccountCode='62011900' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T3],case when AccountCode='62010500' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T4] from ETraveleDetail where No=" + ID + ") t";
                                    //string sqlB = "select sum(isnull(Pamount,0)+isnull(Camount,0)) as Amount,'62012000' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62012000' union all select sum(isnull(Pamount,0)+isnull(Camount,0)) as Amount,'62010900' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62010900' union all select sum(isnull(Pamount,0)+isnull(Camount,0)) as Amount,'62011900' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62011900' union all select sum(isnull(Pamount,0)+isnull(Camount,0)) as Amount,'62010500' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62010500'";
                                    //DataTable dtB = dbc.GetData("eReimbursement", sqlB);
                                    ////取得传递预算的参数
                                    //string userid = dtf.Rows[0]["PersonID"].ToString();
                                    //string dpt = dtf.Rows[0]["Department"].ToString();
                                    //string ostation = dtf.Rows[0]["Station2"].ToString();//预算站点,与基本信息中的CostCenter一致(Station2)
                                    //string tstation = ostation;//Etravel表中的Station2,目前与预算站点一致,不允许更改
                                    //string year = Convert.ToDateTime(dtA.Rows[0]["BudgetDate"].ToString()).Year.ToString();
                                    //string month = Convert.ToDateTime(dtA.Rows[0]["BudgetDate"].ToString()).Month.ToString();
                                    //string accountcode = "";
                                    //for (int g = 0; g < dtB.Rows.Count; g++)
                                    //{
                                    //    if (Convert.ToDecimal(dtB.Rows[g]["Amount"].ToString()) != 0)
                                    //    {
                                    //        DataRow dr = dtbudget.NewRow();
                                    //        dr["Current"] = Convert.ToDecimal(dtB.Rows[g]["Amount"].ToString());
                                    //        dr["COACode"] = dtB.Rows[g]["COACode"].ToString();
                                    //        accountcode = dtB.Rows[g]["COACode"].ToString();
                                    //        DataTable dtC = new DataTable();
                                    //        dtC = Comm.ExRtnEB(userid, dpt, ostation, tstation, accountcode, year, month, "T", ID);

                                    //        for (int h = 0; h < dtC.Rows.Count; h++)
                                    //        {
                                    //            if (dtC.Rows[h]["Type"].ToString() == "全年个人")
                                    //            {
                                    //                dr["PU"] = Convert.ToDecimal(dtC.Rows[h]["Used"].ToString());
                                    //                dr["PB"] = Convert.ToDecimal(dtC.Rows[h]["Budget"].ToString());
                                    //            }
                                    //            else if (dtC.Rows[h]["Type"].ToString() == "全年部门")
                                    //            {
                                    //                dr["DU"] = Convert.ToDecimal(dtC.Rows[h]["Used"].ToString());
                                    //                dr["DB"] = Convert.ToDecimal(dtC.Rows[h]["Budget"].ToString());
                                    //            }
                                    //            else if (dtC.Rows[h]["Type"].ToString() == "全年站点")
                                    //            {
                                    //                dr["SU"] = Convert.ToDecimal(dtC.Rows[h]["Used"].ToString());
                                    //                dr["SB"] = Convert.ToDecimal(dtC.Rows[h]["Budget"].ToString());
                                    //            }
                                    //        }
                                    //        dtbudget.Rows.Add(dr);
                                    //    }
                                    //}
                                    ////计算%,取得名称,预算转换为本地汇率
                                    //for (int g = 0; g < dtbudget.Rows.Count; g++)
                                    //{
                                    //    if (Convert.ToDecimal(dtbudget.Rows[g]["PB"].ToString()) != 0)//如果Budget不为0,则计算%
                                    //    {
                                    //        dtbudget.Rows[g]["PPercent"] = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[g]["Current"].ToString()) + Convert.ToDecimal(dtbudget.Rows[g]["PU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[g]["PB"].ToString()), 2);

                                    //    }
                                    //    if (Convert.ToDecimal(dtbudget.Rows[g]["DB"].ToString()) != 0)//如果Budget不为0,则计算%
                                    //    {
                                    //        dtbudget.Rows[g]["DPercent"] = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[g]["Current"].ToString()) + Convert.ToDecimal(dtbudget.Rows[g]["DU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[g]["DB"].ToString()), 2);

                                    //    }
                                    //    if (Convert.ToDecimal(dtbudget.Rows[g]["SB"].ToString()) != 0)//如果Budget不为0,则计算%
                                    //    {
                                    //        dtbudget.Rows[g]["SPercent"] = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[g]["Current"].ToString()) + Convert.ToDecimal(dtbudget.Rows[g]["SU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[g]["SB"].ToString()), 2);

                                    //    }
                                    //    if (dtbudget.Rows[g]["COACode"].ToString() == "62012000")
                                    //    {
                                    //        dtbudget.Rows[g]["EName"] = "Travel expense";
                                    //    }
                                    //    else if (dtbudget.Rows[g]["COACode"].ToString() == "62010900")
                                    //    {
                                    //        dtbudget.Rows[g]["EName"] = "Entertainment";
                                    //    }
                                    //    else if (dtbudget.Rows[g]["COACode"].ToString() == "62011900")
                                    //    {
                                    //        dtbudget.Rows[g]["EName"] = "Transportation";
                                    //    }
                                    //    else if (dtbudget.Rows[g]["COACode"].ToString() == "62010500")
                                    //    {
                                    //        dtbudget.Rows[g]["EName"] = "Communication";
                                    //    }
                                    //    dtbudget.Rows[g]["PU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["PU"].ToString()), 2);
                                    //    dtbudget.Rows[g]["PB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["PB"].ToString()), 2);
                                    //    dtbudget.Rows[g]["DU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["DU"].ToString()), 2);
                                    //    dtbudget.Rows[g]["DB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["DB"].ToString()), 2);
                                    //    dtbudget.Rows[g]["SU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["SU"].ToString()), 2);
                                    //    dtbudget.Rows[g]["SB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[g]["SB"].ToString()), 2);
                                    //    dtbudget.Rows[g]["RequestID"] = ID;
                                    //    dtbudget.Rows[g]["Status"] = 3;
                                    //    dtbudget.Rows[g]["LocalCur"] = CurLocal;
                                    //    dtbudget.Rows[g]["CenterCur"] = CurBudget;
                                    //    dtbudget.Rows[g]["Rate"] = rate;
                                    //}
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
                                    string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dtall.Rows[0]["TSation"].ToString());

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
                                            dtC = Comm.ExRtnEB(userid, department, ostation, ostation, accountcode, dtB.Rows[g]["Year"].ToString(), "1", "G", ID);
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
                                        dtbudget.Rows[g]["Status"] = 3;
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
                                    if (!SendMail("G", Request.QueryString["RequestID"].ToString(), dt.Rows[i]["id"].ToString(), dtbudget))//Budget已经计入Current,%不需要重新计算
                                    {
                                        ErrorHandle("Error mail address."); return;
                                    }
                                }
                                catch (Exception)
                                {

                                    throw;
                                }
                                //if (!SendMail("G", Request.QueryString["RequestID"].ToString(), dt.Rows[i]["id"].ToString()))
                                //{
                                //    ErrorHandle("Error mail address."); return;
                                //}
                                X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Rejected and form complete.', buttons: { ok: 'Ok' }, fn: function (btn) { parent.Window1.hide();parent.Button1.fireEvent('click'); } });");
                            }
                        }
                        else
                        {
                            if (i == dt.Rows.Count-1)
                            {
                                string updatesql = "update Eflow set Active=2,Status=2,Remark='" + txtRemark.Text.Replace("'", "''") + "',ApproveDate='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' where id=" + dt.Rows[i]["id"].ToString();
                                if (dt.Rows[i]["Type"].ToString() == "T")
                                {
                                    updatesql += ";update ETravel set Status=2 where ID=" + dt.Rows[i]["RequestID"].ToString();
                                }
                                else
                                {
                                    updatesql += ";update Ecommon set Status=2 where ID=" + dt.Rows[i]["RequestID"].ToString();
                                }
                                string newid = dbc.UpdateData("eReimbursement", updatesql, "Update");
                                if (newid == "-1")
                                {
                                    ErrorHandle("Data Error");
                                    return;
                                }
                                else
                                {
                                    //保存预算信息
                                    try
                                    {
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
                                        string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dtall.Rows[0]["TSation"].ToString());
                                        

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
                                                dtC = Comm.ExRtnEB(userid, department, ostation, ostation, accountcode, dtB.Rows[g]["Year"].ToString(), "1", "G", ID);
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
                                        if (!SendMail("G", Request.QueryString["RequestID"].ToString(), dt.Rows[i]["id"].ToString(), dtbudget))//Budget已经计入Current,%不需要重新计算
                                        {
                                            ErrorHandle("Error mail address."); return;
                                        }
                                    }
                                    catch (Exception)
                                    {

                                        throw;
                                    }
                                    X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Approved and form complete.', buttons: { ok: 'Ok' }, fn: function (btn) { parent.Window1.hide();parent.Button1.fireEvent('click'); } });");
                                }
                            }
                            else
                            {
                                string updatesql = "update Eflow set Active=-1,Status=2,Remark='" + txtRemark.Text.Replace("'", "''") + "',ApproveDate='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' where id=" + dt.Rows[i]["id"].ToString();
                                updatesql += ";update Eflow set Active=1 where id=" + dt.Rows[i + 1]["id"].ToString();
                                string newid = dbc.UpdateData("eReimbursement", updatesql, "Update");
                                if (newid == "-1")
                                {
                                    ErrorHandle("Data Error");
                                    return;
                                }
                                else
                                {
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
                                    string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dtall.Rows[0]["TSation"].ToString());
                                    

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
                                            dtC = Comm.ExRtnEB(userid, department, ostation, ostation, accountcode, dtB.Rows[g]["Year"].ToString(), "1", "G", ID);
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
                                    if (!SendMail("G", Request.QueryString["RequestID"].ToString(), dt.Rows[i + 1]["id"].ToString(), dtbudget))//Budget已经计入Current,%不需要重新计算
                                    {
                                        ErrorHandle("Error mail address."); return;
                                    }
                                    X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Approved.', buttons: { ok: 'Ok' }, fn: function (btn) { parent.Window1.hide();parent.Button1.fireEvent('click'); } });");
                                }
                            }
                        }
                        break;
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
        protected void AddApp(object sender, DirectEventArgs e)
        {
            cs.DBCommand dbc = new cs.DBCommand();
            string ID = Request.QueryString["RequestID"].ToString();
            string userid1 = e.ExtraParams[0].Value;
            string username1 = e.ExtraParams[1].Value;
            string updatesql = "update Eflow set Active=-1,Status=2,Remark='" + txtRemark.Text.Replace("'", "''") + "',ApproveDate='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' where id=" + Request.QueryString["ID"].ToString();
            updatesql += ";insert into Eflow ([Type],[Step],[Approver],[ApproverID],[ApproveDate],[RequestID],[Active],[Status]) values ";
            updatesql += "('G'," + Request.QueryString["Step"].ToString() + ",'" + username1 + "','" + userid1 + "','" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "','" + Request.QueryString["RequestID"].ToString() + "',1,1);select id from Eflow where id=@@IDENTITY";
            string newid = dbc.UpdateData("eReimbursement", updatesql, "Insert");
            if (newid == "-1")
            {
                ErrorHandle();
                return;
            }
            else
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
                string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dtall.Rows[0]["TSation"].ToString());
                

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
                        //dtC = Comm.RtnEB(userid, department, ostation, ostation, accountcode, dtB.Rows[g]["Year"].ToString(), "1");
                        dtC = Comm.ExRtnEB(userid, department, ostation, ostation, accountcode, dtB.Rows[g]["Year"].ToString(), "1","G",ID);
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
                if (!SendMail("G", Request.QueryString["RequestID"].ToString(), newid, dtbudget))//Budget已经计入Current,%不需要重新计算
                {
                    ErrorHandle("Error mail address."); return;
                }
                X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Approved.', buttons: { ok: 'Ok' }, fn: function (btn) { parent.Window1.hide();parent.Button1.fireEvent('click'); } });");
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
        protected void GetStation(object sender, DirectEventArgs e)
        {
            DataSet getCostCenterBYStationCode = DIMERCO.SDK.Utilities.LSDK.getUserDataBYUserName(X.GetValue("cbxCOACenter"), 10);
            DataTable dtCOACenter = (DataTable)getCostCenterBYStationCode.Tables[0];
            DataTable dtCOACenternew = new DataTable();
            dtCOACenternew.Columns.Add("cityID", System.Type.GetType("System.String"));
            dtCOACenternew.Columns.Add("cityCode", System.Type.GetType("System.String"));
            dtCOACenternew.Columns.Add("Station", System.Type.GetType("System.String"));
            for (int c = 0; c < dtCOACenter.Rows.Count; c++)
            {
                DataRow dr = dtCOACenternew.NewRow();
                dr["cityID"] = dtCOACenter.Rows[c][0].ToString();
                dr["cityCode"] = dtCOACenter.Rows[c][1].ToString();
                dr["Station"] = dtCOACenter.Rows[c][2].ToString();
                dtCOACenternew.Rows.Add(dr);
            }
            StoreCOACenter.DataSource = dtCOACenternew;
            StoreCOACenter.DataBind();
        }
        protected bool SendMail(string type, string RequestID, string FlowID, DataTable dtPar)
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
                //基本信息
                string sqlbase = "select * from V_Eflow_ETravel where FlowID='" + FlowID + "'";
                DataTable dtbase = new DataTable();
                dtbase = dbc.GetData("eReimbursement", sqlbase);
                if (dtbase.Rows[0]["Active"].ToString() == "2")//完成
                {
                    if (dtbase.Rows[0]["Status"].ToString() == "3")//完成:拒绝
                    {
                        mail.Title = "Dimerco eReimbursement "+budget+" " + dtMail.Rows[0]["Person"].ToString() + " - Application Rejected.";
                        sb.Append("<div " + divstyle + ">Dear " + dtbase.Rows[0]["Person"].ToString() + ",</div><br />");
                        //sb.Append("<div " + divstyleReject + ">The following eReimbursement application has been rejected:</div><br />");
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
                            sb.Append("<div " + divstyleReject + ">eReimbursement Rejected Remarks:" + dtbase.Rows[0]["RemarkFlow"].ToString() + "</div><br /><br />");
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
                        mail.Title = "Dimerco eReimbursement "+budget+ " "+ dtMail.Rows[0]["Person"].ToString() + ") - Application Approved.";
                        sb.Append("<div " + divstyle + ">Dear " + dtbase.Rows[0]["Person"].ToString() + ",</div><br />");
                        sb.Append("<div " + divstyle + ">The following eReimbursement application has been approved(Complete):</div><br />");
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
                    //收件人
                    DataSet dsTo = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(dtbase.Rows[0]["PersonID"].ToString());
                    if (dsTo != null && dsTo.Tables.Count >= 1 && dsTo.Tables[0].Rows.Count == 1)
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
                            else
                            {
                                msg = "Seek For Your Approval.";
                            }

                            sb.Append("<div " + divstyle + ">Dear " + dtMail.Rows[i]["Approver"].ToString() + ",</div><br />");
                            //sb.Append("<div " + divstyle + ">The following eReimbursement application: "+msg+"</div><br />");
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
                    mail.Title = "Dimerco eReimbursement " + budget + " " + dtMail.Rows[0]["Person"].ToString() + " - " + msg;
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
                string testmailstr = "";
                testmailstr += "<div " + divstyleReject + ">THIS IS A TEST MAIL." + mailtestword + "</div><br />";
                testmailstr += "<div>";
                
                if (type == "G")//通用费用
                {
                    DataTable dtpar1 = dtPar.DefaultView.ToTable(true, "Year");
                    bool YearOrNot = dtpar1.Rows.Count == 1 ? false : true;

                    //160119 垫付

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
                        else
                        {
                            msg1 = " Approved by: ";
                        }

                        if (dtMail.Rows[i]["Active"].ToString() == "-1")//未完成
                        {
                            sb1.Append("<div " + divstyle + ">" + (i + 1).ToString()+"." + msg1 + dtMail.Rows[i]["Approver"].ToString() + ", Date: " + Convert.ToDateTime(dtMail.Rows[i]["ApproveDate"].ToString()).ToString("yy/MM/dd"));
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
                //160127 shanshan
                mail.Body = testmailstr+sb.ToString();
                mail.Send();
            }
            return true;
        }
    }
}