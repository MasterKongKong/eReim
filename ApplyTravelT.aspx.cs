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
    public partial class ApplyTravelT : App_Code.BasePage
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
                        string sql = "select * from ETravel where ID='" + ID + "'";
                        cs.DBCommand dbc = new cs.DBCommand();
                        DataTable dt = new DataTable();
                        dt = dbc.GetData("eReimbursement", sql);
                        if (dt != null && dt.Rows.Count == 1)
                        {
                            string sqldetail2 = "select min(convert(varchar(10),Tdate0,111)) as Tdate0,max(convert(varchar(10),Tdate,111)) as Tdate1 from ETraveleDetail where [No]=" + ID + "";
                            DataTable dtder = dbc.GetData("eReimbursement", sqldetail2);

                            LabelPerson.Text = dt.Rows[0]["Person"].ToString();
                            LabelStation.Text = dt.Rows[0]["Station"].ToString();
                            LabelDepartment.Text = dt.Rows[0]["Department"].ToString();
                            LabelBdate.Text = dtder.Rows[0]["Tdate0"].ToString() == "" ? "" : Convert.ToDateTime(dtder.Rows[0]["Tdate0"].ToString()).ToString("yyyy/MM/dd");
                            LabelEdate.Text = dtder.Rows[0]["Tdate1"].ToString() == "" ? "" : Convert.ToDateTime(dtder.Rows[0]["Tdate1"].ToString()).ToString("yyyy/MM/dd");
                            if (dt.Rows[0]["ReportFile"].ToString() != "")
                            {
                                LinkReport.Text = dt.Rows[0]["ReportFile"].ToString();
                                LinkReport.NavigateUrl = "./Upload/" + dt.Rows[0]["ReportFile"].ToString();
                            }
                            if (dt.Rows[0]["Attach"].ToString() != "")
                            {
                                LinkScanFile.Text = dt.Rows[0]["Attach"].ToString();
                                LinkScanFile.NavigateUrl = "./Upload/" + dt.Rows[0]["Attach"].ToString();
                            }
                            LabelSum.Text = dt.Rows[0]["Tamount"].ToString() == "" ? "0" : Convert.ToDecimal(dt.Rows[0]["Tamount"].ToString()).ToString("#,##0.00");
                            LabelPSum.Text = dt.Rows[0]["Pamout"].ToString() == "" ? "0" : Convert.ToDecimal(dt.Rows[0]["Pamout"].ToString()).ToString("#,##0.00");
                            LabelCSum.Text = dt.Rows[0]["Camount"].ToString() == "" ? "0" : Convert.ToDecimal(dt.Rows[0]["Camount"].ToString()).ToString("#,##0.00");
                            LabelRemark.Text = dt.Rows[0]["Remark"].ToString();

                            //160119 垫付
                            LabelBehalfPersonName.Text = dt.Rows[0]["OnBehalfPersonName"].ToString();
                            LabelBehalfCost.Text = dt.Rows[0]["OnBehalfPersonCostCenter"].ToString();

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
                            LabelCur.Text = dtall.Rows[0]["Cur"].ToString();
                            //预算
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
                            //取得Budget_Complete中数据
                            DataTable dtnn = new DataTable();
                            string sqlbu = "select EName,COACode,LocalAmount as [Current],PU,PB,PPercent,DU,DB,DPercent,SU,SB,SPercent from Budget_Complete where FormType='T' and RequestID=" + ID;
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
                                var cmBudget = GridPanelBudget.ColumnModel;
                                cmBudget.Columns.Add(new Column
                                {
                                    DataIndex = "EName",
                                    Header = "Expense Item",
                                    Sortable = false,
                                    Resizable = false,
                                    MenuDisabled = true,
                                    Width = 100
                                });
                                cmBudget.Columns.Add(new Column
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
                                    cmBudget.Columns.Add(new Column
                                    {
                                        DataIndex = "PU",
                                        Header = "Personal Used",
                                        Renderer = new Renderer { Fn = "GetNumber" },
                                        Sortable = false,
                                        Resizable = false,
                                        MenuDisabled = true,
                                        Width = 100
                                    });
                                    cmBudget.Columns.Add(new Column
                                    {
                                        DataIndex = "PB",
                                        Header = "Personal Budget",
                                        Renderer = new Renderer { Fn = "GetNumber" },
                                        Sortable = false,
                                        Resizable = false,
                                        MenuDisabled = true,
                                        Width = 100
                                    });
                                    cmBudget.Columns.Add(new Column
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
                                    cmBudget.Columns.Add(new Column
                                    {
                                        DataIndex = "DU",
                                        Header = "Department Used",
                                        Renderer = new Renderer { Fn = "GetNumber" },
                                        Sortable = false,
                                        Resizable = false,
                                        MenuDisabled = true,
                                        Width = 100
                                    });
                                    cmBudget.Columns.Add(new Column
                                    {
                                        DataIndex = "DB",
                                        Header = "Department Budget",
                                        Renderer = new Renderer { Fn = "GetNumber" },
                                        Sortable = false,
                                        Resizable = false,
                                        MenuDisabled = true,
                                        Width = 100
                                    });
                                    cmBudget.Columns.Add(new Column
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
                                    cmBudget.Columns.Add(new Column
                                    {
                                        DataIndex = "SU",
                                        Header = "Unit Used",
                                        Renderer = new Renderer { Fn = "GetNumber" },
                                        Sortable = false,
                                        Resizable = false,
                                        MenuDisabled = true,
                                        Width = 100
                                    });
                                    cmBudget.Columns.Add(new Column
                                    {
                                        DataIndex = "SB",
                                        Header = "Unit Budget",
                                        Renderer = new Renderer { Fn = "GetNumber" },
                                        Sortable = false,
                                        Resizable = false,
                                        MenuDisabled = true,
                                        Width = 100
                                    });
                                    cmBudget.Columns.Add(new Column
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
                            //140226 显示预算
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
                                string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dt.Rows[0]["Station2"].ToString());
                                if (CurLocal != CurBudget)
                                {
                                    rate = DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(CurLocal, CurBudget, Convert.ToDateTime(dtA.Rows[0]["BudgetDate"].ToString()).Year);
                                }

                                //取得4大类合计
                                //string sqlB = "select sum(T1) as T1,sum(T2) as T2,sum(T3) as T3,sum(T4) as T4 from (select case when AccountCode='62012000' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T1],case when AccountCode='62010900' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T2],case when AccountCode='62011900' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T3],case when AccountCode='62010500' then isnull(Pamount,0)+isnull(Camount,0) else 0 end as [T4] from ETraveleDetail where No=" + ID + ") t";
                                string sqlB = "select isnull(sum(isnull(Pamount,0)+isnull(Camount,0)),0) as Amount,'62012000' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62012000' union all select isnull(sum(isnull(Pamount,0)+isnull(Camount,0)),0) as Amount,'62010900' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62010900' union all select isnull(sum(isnull(Pamount,0)+isnull(Camount,0)),0) as Amount,'62011900' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62011900' union all select isnull(sum(isnull(Pamount,0)+isnull(Camount,0)),0) as Amount,'62010500' as COACode from ETraveleDetail where No=" + ID + " and AccountCode='62010500'";
                                DataTable dtB = dbc.GetData("eReimbursement", sqlB);
                                //取得传递预算的参数
                                //160119 垫付
                                string userid = dt.Rows[0]["OnBehalfPersonID"].ToString() == "" ? dt.Rows[0]["PersonID"].ToString() : dt.Rows[0]["OnBehalfPersonID"].ToString();
                                string dpt = dt.Rows[0]["OnBehalfPersonID"].ToString() == "" ? dt.Rows[0]["Department"].ToString() : dt.Rows[0]["OnBehalfPersonDept"].ToString();
                                string ostation = dt.Rows[0]["Station2"].ToString();//预算站点,与基本信息中的CostCenter一致(Station2)
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
                                        dtC = Comm.ExRtnEB(userid, dpt, ostation, tstation, accountcode, year, month, "T", ID);

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
                                        dtbudget.Rows[i]["PPercent"] = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtbudget.Rows[i]["PU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()), 2);
                                        if (!PB)
                                        {
                                            PB = true;
                                        }
                                    }
                                    if (Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()) != 0)//如果Budget不为0,则计算%
                                    {
                                        dtbudget.Rows[i]["DPercent"] = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtbudget.Rows[i]["DU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()), 2);
                                        if (!DB)
                                        {
                                            DB = true;
                                        }
                                    }
                                    if (Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()) != 0)//如果Budget不为0,则计算%
                                    {
                                        dtbudget.Rows[i]["SPercent"] = System.Math.Round((Convert.ToDecimal(dtbudget.Rows[i]["Current"].ToString()) + Convert.ToDecimal(dtbudget.Rows[i]["SU"].ToString())) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()), 2);
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
                                //添加数据列
                                var cmBudget = GridPanelBudget.ColumnModel;
                                cmBudget.Columns.Add(new Column
                                {
                                    DataIndex = "EName",
                                    Header = "Expense Item",
                                    Sortable = false,
                                    Resizable = false,
                                    MenuDisabled = true,
                                    Width = 100
                                });
                                cmBudget.Columns.Add(new Column
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
                                    cmBudget.Columns.Add(new Column
                                    {
                                        DataIndex = "PU",
                                        Header = "Personal Used",
                                        Renderer = new Renderer { Fn = "GetNumber" },
                                        Sortable = false,
                                        Resizable = false,
                                        MenuDisabled = true,
                                        Width = 100
                                    });
                                    cmBudget.Columns.Add(new Column
                                    {
                                        DataIndex = "PB",
                                        Header = "Personal Budget",
                                        Renderer = new Renderer { Fn = "GetNumber" },
                                        Sortable = false,
                                        Resizable = false,
                                        MenuDisabled = true,
                                        Width = 100
                                    });
                                    cmBudget.Columns.Add(new Column
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
                                    cmBudget.Columns.Add(new Column
                                    {
                                        DataIndex = "DU",
                                        Header = "Department Used",
                                        Renderer = new Renderer { Fn = "GetNumber" },
                                        Sortable = false,
                                        Resizable = false,
                                        MenuDisabled = true,
                                        Width = 100
                                    });
                                    cmBudget.Columns.Add(new Column
                                    {
                                        DataIndex = "DB",
                                        Header = "Department Budget",
                                        Renderer = new Renderer { Fn = "GetNumber" },
                                        Sortable = false,
                                        Resizable = false,
                                        MenuDisabled = true,
                                        Width = 100
                                    });
                                    cmBudget.Columns.Add(new Column
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
                                    cmBudget.Columns.Add(new Column
                                    {
                                        DataIndex = "SU",
                                        Header = "Unit Used",
                                        Renderer = new Renderer { Fn = "GetNumber" },
                                        Sortable = false,
                                        Resizable = false,
                                        MenuDisabled = true,
                                        Width = 100
                                    });
                                    cmBudget.Columns.Add(new Column
                                    {
                                        DataIndex = "SB",
                                        Header = "Unit Budget",
                                        Renderer = new Renderer { Fn = "GetNumber" },
                                        Sortable = false,
                                        Resizable = false,
                                        MenuDisabled = true,
                                        Width = 100
                                    });
                                    cmBudget.Columns.Add(new Column
                                    {
                                        DataIndex = "SPercent",
                                        Header = "%(Curren+Used/Budget)",
                                        Renderer = new Renderer { Fn = "GetNumberPercent" },
                                        Sortable = false,
                                        Resizable = false,
                                        MenuDisabled = true,
                                        Width = 100
                                    });
                                }
                            }


                            string sqlflow = "select * from V_Eflow_ETravel where RequestID='" + ID + "' and [Type]='T' order by Step,FlowID";
                            DataTable dtflow = new DataTable();
                            dtflow = dbc.GetData("eReimbursement", sqlflow);
                            //160119 垫付,如果登录用户是被垫付人审批人之一,则显示预算,否则不显示
                            //if (dtflow != null && dtflow.Rows.Count > 0 && dtflow.Select("ApproverID='" + Request.Cookies.Get("eReimUserID").Value + "' and FPersonID=OnBehalfPersonID").Count() > 0)
                            //{
                            //    StoreBudget.DataSource = dtbudget;
                            //    StoreBudget.DataBind();
                            //}
                            if (dtflow != null && dtflow.Rows.Count > 0)
                            {
                                if (dtflow.Select("ApproverID='" + Request.Cookies.Get("eReimUserID").Value + "' and FPersonID=OnBehalfPersonID").Count() > 0 || dtflow.Select("ApproverID='" + Request.Cookies.Get("eReimUserID").Value + "' and OnBehalfPersonID is null").Count() > 0)
                                {
                                    StoreBudget.DataSource = dtbudget;
                                    StoreBudget.DataBind();
                                }

                            }


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
                            int colc = dtall.Rows.Count / 11;
                            //140226
                            //第一行组列
                            var continentGroupRow = new HeaderGroupRow();
                            continentGroupRow.Columns.Add(new HeaderGroupColumn
                            {
                                Header = ""
                                //Align = Alignment.Center,
                                //ColSpan = cities.Length * products.Length
                            });
                            for (int i = 0; i < colc; i++)//准备复制已有信息
                            {
                                continentGroupRow.Columns.Add(new HeaderGroupColumn
                                {
                                    Header = "Reimbursement",
                                    Align = Alignment.Center
                                });
                                continentGroupRow.Columns.Add(new HeaderGroupColumn
                                {
                                    Header = "Company Paid",
                                    Align = Alignment.Center
                                });
                            }
                            continentGroupRow.Columns.Add(new HeaderGroupColumn
                            {
                                Header = ""
                            });
                            continentGroupRow.Columns.Add(new HeaderGroupColumn
                            {
                                Header = ""
                            });
                            //第二行组列
                            var cityGroupRow = new HeaderGroupRow();
                            cityGroupRow.Columns.Add(new HeaderGroupColumn
                            {
                                Header = "Destination:"
                            });
                            for (int i = 0; i < colc; i++)//准备复制已有信息
                            {
                                cityGroupRow.Columns.Add(new HeaderGroupColumn
                                {
                                    Header = dtall.Rows[i * 11]["Tocity"].ToString(),
                                    Align = Alignment.Center,
                                    ColSpan = 2
                                });
                            }
                            cityGroupRow.Columns.Add(new HeaderGroupColumn
                            {
                                Header = ""
                            });
                            cityGroupRow.Columns.Add(new HeaderGroupColumn
                            {
                                Header = ""
                            });
                            //第三行组列
                            var HeaderGroupRow3 = new HeaderGroupRow();
                            HeaderGroupRow3.Columns.Add(new HeaderGroupColumn
                            {
                                Header = "Cost Center:"
                            });
                            for (int i = 0; i < colc; i++)//准备复制已有信息
                            {
                                HeaderGroupRow3.Columns.Add(new HeaderGroupColumn
                                {
                                    Header = dtall.Rows[i * 11]["TSation"].ToString(),
                                    Align = Alignment.Center,
                                    ColSpan = 2
                                });
                            }
                            HeaderGroupRow3.Columns.Add(new HeaderGroupColumn
                            {
                                Header = ""
                            });
                            HeaderGroupRow3.Columns.Add(new HeaderGroupColumn
                            {
                                Header = ""
                            });
                            //添加数据列
                            var cm = GridPanel2.ColumnModel;
                            cm.Columns.Add(new Column
                            {
                                DataIndex = "Category",
                                Header = "Travel Period:",
                                Sortable = false,
                                Resizable = false,
                                MenuDisabled = true,
                                Width = 180
                            });
                            for (int i = 0; i < colc; i++)//准备复制已有信息
                            {
                                string dtfroms = dtall.Rows[i * 11]["Tdate0"].ToString() == "" ? "" : Convert.ToDateTime(dtall.Rows[i * 11]["Tdate0"].ToString()).ToString("yyyy/MM/dd");
                                string dtfroms1 = dtall.Rows[i * 11]["Tdate"].ToString() == "" ? "" : Convert.ToDateTime(dtall.Rows[i * 11]["Tdate"].ToString()).ToString("yyyy/MM/dd");
                                    
                                string fieldPName = "Station_" + i.ToString() + "_P";
                                string fieldCName = "Station_" + i.ToString() + "_C";
                                cm.Columns.Add(new Column
                                {
                                    DataIndex = fieldPName,
                                    Header = dtfroms,
                                    Sortable = false,
                                    Resizable = false,
                                    MenuDisabled = true,
                                    Width = 100
                                });
                                cm.Columns.Add(new Column
                                {
                                    DataIndex = fieldCName,
                                    Header = dtfroms1,
                                    Sortable = false,
                                    Resizable = false,
                                    MenuDisabled = true,
                                    Width = 100
                                });
                            }
                            cm.Columns.Add(new Column
                            {
                                DataIndex = "TotalP",
                                Header = "Total(Personal paid)",
                                Sortable = false,
                                Resizable = false,
                                MenuDisabled = true,
                                Width = 100
                            });
                            cm.Columns.Add(new Column
                            {
                                DataIndex = "TotalC",
                                Header = "Total(Company)",
                                Sortable = false,
                                Resizable = false,
                                MenuDisabled = true,
                                Width = 100
                            });
                            GridPanel2.View[0].HeaderGroupRows.Add(continentGroupRow);
                            GridPanel2.View[0].HeaderGroupRows.Add(cityGroupRow);
                            GridPanel2.View[0].HeaderGroupRows.Add(HeaderGroupRow3);


                            Store2.DataSource = dtnew;
                            Store2.DataBind();


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

                            if (Request.QueryString["Status"].ToString()!="1")
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
                                    //string sqlr = "select * from V_Eflow_ETravel where [Type]='T' and RequestID=(select RequestID from V_Eflow_ETravel where FlowID=" + Status + " and Type='T') order by cast(Step as int)";
                                    //DataTable dtr = new DataTable();
                                    //dtr = dbc.GetData("eReimbursement", sqlr);
                                    //if (dtr != null)
                                    //{
                                    //    string remark = "";
                                    //    for (int i = 0; i < dtr.Rows.Count; i++)
                                    //    {
                                    //        if (dtr.Rows[i]["FlowID"].ToString() == Status)
                                    //        {
                                    //            if (Request.Cookies.Get("eReimUserID").Value != dtr.Rows[i]["ApproverID"].ToString())
                                    //            {
                                    //                ErrorHandle("No right.");
                                    //                X.AddScript("Button1.disable();Button2.disable();Button3.disable();");
                                    //                return;
                                    //            }
                                    //            txtRemark.Text = dtr.Rows[i]["RemarkFlow"].ToString();
                                    //            break;
                                    //        }
                                    //        else if (dtr.Rows[i]["RemarkFlow"].ToString().Trim().Length!= 0)
                                    //        {
                                    //            remark += dtr.Rows[i]["Approver"].ToString() + ":" + dtr.Rows[i]["RemarkFlow"].ToString() + " ";
                                    //        }
                                    //    }
                                    //    if (remark.Length == 0)
                                    //    {
                                    //        Label8.Text = "";
                                    //    }
                                    //    else
                                    //    {
                                    //        LabelRemarkFlow.Text = remark;
                                    //    }
                                    //}
                                    if (dtflow != null)
                                    {
                                        //151029,如果登陆人是历史审批人,则有权查看
                                        if (dtflow.Select("ApproverID='" + Request.Cookies.Get("eReimUserID").Value + "'").Count() > 0)
                                        {
                                            string remark = "";
                                            for (int i = 0; i < dtflow.Rows.Count; i++)
                                            {
                                                if (dtflow.Rows[i]["FlowID"].ToString() == Status)
                                                {
                                                    if (dtflow.Rows[i]["Active"].ToString() == "1")
                                                    {
                                                        if (Request.Cookies.Get("eReimUserID").Value == dtflow.Rows[i]["ApproverID"].ToString())
                                                        {
                                                            //txtRemark.Text = dtflow.Rows[i]["RemarkFlow"].ToString();
                                                            //if (dtflow.Rows[i]["RemarkFlow"].ToString().Trim().Length != 0)
                                                            //{
                                                            //    remark += dtflow.Rows[i]["Approver"].ToString() + ":" + dtflow.Rows[i]["RemarkFlow"].ToString() + " ";
                                                            //}
                                                            //X.AddScript("Button1.enable();Button2.enable();Button3.enable();");
                                                            if (dtflow.Rows[0]["OnBehalfPersonID"].ToString() == "")
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
                                                Label2.Text = "";
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
            SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eReimbursement"].ConnectionString);
            string ID = Request.QueryString["RequestID"].ToString();
            string sql = "select * from Eflow where RequestID='" + Request.QueryString["RequestID"].ToString() + "' and [Type]='" + Request.QueryString["Type"].ToString() + "' order by Step,id";
            DataTable dt = new DataTable();
            dt = dbc.GetData("eReimbursement", sql);

            string sqlf = "select * from ETravel where ID=" + ID;
            DataTable dtf = dbc.GetData("eReimbursement", sqlf);
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["Active"].ToString() == "1")
                    {
                        if (para=="3")//拒绝申请
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
                            if (newid != "1")
                            {
                                ErrorHandle("Data Error.");
                                return;
                            }
                            else
                            {
                                //保存预算信息
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
                                    string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dtall.Rows[0]["TSation"].ToString());
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

                                        spdetail = new SqlParameter("@COACode", SqlDbType.VarChar,50);
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
                                    if (!SendMail("T", Request.QueryString["RequestID"].ToString(), dt.Rows[i]["id"].ToString(), dtbudget))//Budget已经计入Current,%不需要重新计算
                                    {
                                        ErrorHandle("Error mail address."); return;
                                    }
                                }
                                catch (Exception)
                                {
                                    
                                    throw;
                                }
                                X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'Rejected and form complete.', buttons: { ok: 'Ok' }, fn: function (btn) { parent.Window1.hide();parent.Button1.fireEvent('click'); } });");
                            }
                        }
                        else//同意申请
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
                                if (newid != "1")
                                {
                                    ErrorHandle("Data Error.");
                                    return;
                                }
                                else
                                {
                                    //保存预算信息
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
                                        string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dtall.Rows[0]["TSation"].ToString());
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
                                        if (!SendMail("T", Request.QueryString["RequestID"].ToString(), dt.Rows[i]["id"].ToString(), dtbudget))//Budget已经计入Current,%不需要重新计算
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
                                if (newid != "1")
                                {
                                    ErrorHandle(newid);
                                    return;
                                }
                                else
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
                                    string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dtall.Rows[0]["TSation"].ToString());
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

                                    if (!SendMail("T", Request.QueryString["RequestID"].ToString(), dt.Rows[i + 1]["id"].ToString(), dtbudget))//Budget已经计入Current,%不需要重新计算
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
        protected void AddApp(object sender, DirectEventArgs e)
        {
            cs.DBCommand dbc = new cs.DBCommand();
            string ID = Request.QueryString["RequestID"].ToString();
            string userid1 = e.ExtraParams[0].Value;
            string username1 = e.ExtraParams[1].Value;
            string updatesql = "update Eflow set Active=-1,Status=2,Remark='" + txtRemark.Text.Replace("'", "''") + "',ApproveDate='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' where id=" + Request.QueryString["ID"].ToString();
            updatesql += ";insert into Eflow ([Type],[Step],[Approver],[ApproverID],[ApproveDate],[RequestID],[Active],[Status]) values ";
            updatesql += "('T'," + Request.QueryString["Step"].ToString() + ",'" + username1 + "','" + userid1 + "','" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "','" + Request.QueryString["RequestID"].ToString() + "',1,1);select id from Eflow where id=@@IDENTITY";
            string newid = dbc.UpdateData("eReimbursement", updatesql, "Insert");
            if (newid == "-1")
            {
                ErrorHandle();
                return;
            }
            else
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
                string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(dtall.Rows[0]["TSation"].ToString());
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
                //DataTable dtBudget = new DataTable();
                if (!SendMail("T", Request.QueryString["RequestID"].ToString(), newid, dtbudget))//Budget已经计入Current,%不需要重新计算
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
                        mail.Title = "Dimerco eReimbursement "+budget+" " + dtMail.Rows[0]["Person"].ToString() + " - Application Approved.";
                        sb.Append("<div " + divstyle + ">Dear " + dtbase.Rows[0]["Person"].ToString() + ",</div><br />");
                        //sb.Append("<div " + divstyle + ">The following eReimbursement application has been approved(Complete):</div><br />");
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
                                sb.Append("<div " + divstyle + ">eReimbursement Approval Remarks:" + dtMail.Rows[i - 1]["RemarkFlow"].ToString() + "</div><br /><br />");
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
                    //160119 垫付

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
    }
}