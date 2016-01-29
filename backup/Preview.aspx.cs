using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Microsoft.Reporting.WebForms;
using System.Data;
using System.Reflection;
using Ext.Net;
namespace eReimbursement
{
    public partial class Preview : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cs.DBCommand dbc = new cs.DBCommand();
                //取得基本信息
                //if (Request.QueryString["RequestID"] == null)
                //{
                //    ErrorHandle("Data Error."); return;
                //}

                if (Request.QueryString["RequestID"] != null && Request.QueryString["Type"] != null)
                {
                    string ID = Request.QueryString["RequestID"].ToString();
                    string Type = Request.QueryString["Type"].ToString();
                    System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^\d*$");
                    if (reg1.IsMatch(ID))
                    {
                        string sql1 = "select * from V_Eflow_ETravel where RequestID=" + ID + " and [Type]='" + Type + "' order by Step,FlowID";
                        DataTable dt1 = new DataTable();
                        dt1 = dbc.GetData("eReimbursement", sql1);
                        if (dt1 != null && dt1.Rows.Count > 0)
                        {
                            //取得明细信息
                            if (Type == "T")
                            {
                                string sql = "select * from ETraveleDetail where [No]=" + ID + " order by [Tocity],[AccountCode]";

                                DataTable dtDetail = dbc.GetData("eReimbursement", sql);
                                List<Detail> list = new List<Detail>();
                                List<DetailAll> listall = new List<DetailAll>();
                                DataTable dtall = new DataTable();
                                dtall.Columns.Add("Tocity", typeof(String));
                                for (int i = 0; i < 11; i++)
                                {
                                    dtall.Columns.Add("P" + i.ToString(), typeof(Decimal));
                                    dtall.Columns.Add("C" + i.ToString(), typeof(Decimal));
                                }
                                for (int i = 0; i < dtDetail.Rows.Count; i++)
                                {
                                    if (dtDetail.Rows[i]["AccountCode"].ToString() == "62012023")//Air Ticket - Int'l - Row 1
                                    {
                                        bool exi = false;
                                        bool exicity = false;
                                        foreach (Detail detail in list)
                                        {
                                            if (detail.Tocity == dtDetail.Rows[i]["Tocity"].ToString() && detail.Row == "0")
                                            {
                                                detail.Pamount += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                                detail.Camount += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                                exi = true;
                                                break;
                                            }
                                        }
                                        if (!exi)
                                        {
                                            Detail detailnew = new Detail();
                                            detailnew.Tocity = dtDetail.Rows[i]["Tocity"].ToString();
                                            detailnew.Row = "0";
                                            detailnew.Cur = dtDetail.Rows[i]["Cur"].ToString();
                                            detailnew.Pamount = dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                            detailnew.Camount = dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                            list.Add(detailnew);
                                        }

                                        for (int j = 0; j < dtall.Rows.Count; j++)
                                        {
                                            if (dtDetail.Rows[i]["Tocity"].ToString() == dtall.Rows[j]["Tocity"].ToString())
                                            {
                                                exicity = true;
                                                break;
                                            }
                                        }
                                        if (!exicity)
                                        {
                                            DataRow dr = dtall.NewRow();
                                            dr["Tocity"] = dtDetail.Rows[i]["Tocity"].ToString();
                                            dtall.Rows.Add(dr);
                                        }
                                    }
                                    else if (dtDetail.Rows[i]["AccountCode"].ToString() == "62012011" || dtDetail.Rows[i]["AccountCode"].ToString() == "62012021")//Hotel Bill row 2
                                    {
                                        bool exi = false;
                                        bool exicity = false;
                                        foreach (Detail detail in list)
                                        {

                                            if (detail.Tocity == dtDetail.Rows[i]["Tocity"].ToString() && detail.Row == "2")
                                            {
                                                detail.Pamount += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                                detail.Camount += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                                exi = true;
                                                break;
                                            }
                                        }
                                        if (!exi)
                                        {
                                            Detail detailnew = new Detail();
                                            detailnew.Tocity = dtDetail.Rows[i]["Tocity"].ToString();
                                            detailnew.Row = "2";
                                            detailnew.Cur = dtDetail.Rows[i]["Cur"].ToString();
                                            detailnew.Pamount = dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                            detailnew.Camount = dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                            list.Add(detailnew);
                                        }
                                        for (int j = 0; j < dtall.Rows.Count; j++)
                                        {
                                            if (dtDetail.Rows[i]["Tocity"].ToString() == dtall.Rows[j]["Tocity"].ToString())
                                            {
                                                exicity = true;
                                                break;
                                            }
                                        }
                                        if (!exicity)
                                        {
                                            DataRow dr = dtall.NewRow();
                                            dr["Tocity"] = dtDetail.Rows[i]["Tocity"].ToString();
                                            dtall.Rows.Add(dr);
                                        }
                                    }
                                    else if (dtDetail.Rows[i]["AccountCode"].ToString() == "62010901" || dtDetail.Rows[i]["AccountCode"].ToString() == "62010910" || dtDetail.Rows[i]["AccountCode"].ToString() == "62010920")//Enter row 4
                                    {
                                        bool exi = false;
                                        bool exicity = false;
                                        foreach (Detail detail in list)
                                        {

                                            if (detail.Tocity == dtDetail.Rows[i]["Tocity"].ToString() && detail.Row == "4")
                                            {
                                                detail.Pamount += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                                detail.Camount += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                                exi = true;
                                                break;
                                            }
                                        }
                                        if (!exi)
                                        {
                                            Detail detailnew = new Detail();
                                            detailnew.Tocity = dtDetail.Rows[i]["Tocity"].ToString();
                                            detailnew.Row = "4";
                                            detailnew.Cur = dtDetail.Rows[i]["Cur"].ToString();
                                            detailnew.Pamount = dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                            detailnew.Camount = dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                            list.Add(detailnew);
                                        }
                                        for (int j = 0; j < dtall.Rows.Count; j++)
                                        {
                                            if (dtDetail.Rows[i]["Tocity"].ToString() == dtall.Rows[j]["Tocity"].ToString())
                                            {
                                                exicity = true;
                                                break;
                                            }
                                        }
                                        if (!exicity)
                                        {
                                            DataRow dr = dtall.NewRow();
                                            dr["Tocity"] = dtDetail.Rows[i]["Tocity"].ToString();
                                            dtall.Rows.Add(dr);
                                        }
                                    }
                                    else if (dtDetail.Rows[i]["AccountCode"].ToString() == "62011901" || dtDetail.Rows[i]["AccountCode"].ToString() == "62011910" || dtDetail.Rows[i]["AccountCode"].ToString() == "62011920" || dtDetail.Rows[i]["AccountCode"].ToString() == "62011930" || dtDetail.Rows[i]["AccountCode"].ToString() == "62011940" || dtDetail.Rows[i]["AccountCode"].ToString() == "62012013")//Car row 5
                                    {
                                        bool exi = false;
                                        bool exicity = false;
                                        foreach (Detail detail in list)
                                        {

                                            if (detail.Tocity == dtDetail.Rows[i]["Tocity"].ToString() && detail.Row == "5")
                                            {
                                                detail.Pamount += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                                detail.Camount += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                                exi = true;
                                                break;
                                            }
                                        }
                                        if (!exi)
                                        {
                                            Detail detailnew = new Detail();
                                            detailnew.Tocity = dtDetail.Rows[i]["Tocity"].ToString();
                                            detailnew.Row = "5";
                                            detailnew.Cur = dtDetail.Rows[i]["Cur"].ToString();
                                            detailnew.Pamount = dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                            detailnew.Camount = dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                            list.Add(detailnew);
                                        }
                                        for (int j = 0; j < dtall.Rows.Count; j++)
                                        {
                                            if (dtDetail.Rows[i]["Tocity"].ToString() == dtall.Rows[j]["Tocity"].ToString())
                                            {
                                                exicity = true;
                                                break;
                                            }
                                        }
                                        if (!exicity)
                                        {
                                            DataRow dr = dtall.NewRow();
                                            dr["Tocity"] = dtDetail.Rows[i]["Tocity"].ToString();
                                            dtall.Rows.Add(dr);
                                        }
                                    }
                                    else if (dtDetail.Rows[i]["AccountCode"].ToString() == "62010501" || dtDetail.Rows[i]["AccountCode"].ToString() == "62010510" || dtDetail.Rows[i]["AccountCode"].ToString() == "62010520" || dtDetail.Rows[i]["AccountCode"].ToString() == "62010530" || dtDetail.Rows[i]["AccountCode"].ToString() == "62010540" || dtDetail.Rows[i]["AccountCode"].ToString() == "62010550" || dtDetail.Rows[i]["AccountCode"].ToString() == "62010560")//Commu row 6
                                    {
                                        bool exi = false;
                                        bool exicity = false;
                                        foreach (Detail detail in list)
                                        {

                                            if (detail.Tocity == dtDetail.Rows[i]["Tocity"].ToString() && detail.Row == "6")
                                            {
                                                detail.Pamount += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                                detail.Camount += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                                exi = true;
                                                break;
                                            }
                                        }
                                        if (!exi)
                                        {
                                            Detail detailnew = new Detail();
                                            detailnew.Tocity = dtDetail.Rows[i]["Tocity"].ToString();
                                            detailnew.Row = "6";
                                            detailnew.Cur = dtDetail.Rows[i]["Cur"].ToString();
                                            detailnew.Pamount = dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                            detailnew.Camount = dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                            list.Add(detailnew);
                                        }
                                        for (int j = 0; j < dtall.Rows.Count; j++)
                                        {
                                            if (dtDetail.Rows[i]["Tocity"].ToString() == dtall.Rows[j]["Tocity"].ToString())
                                            {
                                                exicity = true;
                                                break;
                                            }
                                        }
                                        if (!exicity)
                                        {
                                            DataRow dr = dtall.NewRow();
                                            dr["Tocity"] = dtDetail.Rows[i]["Tocity"].ToString();
                                            dtall.Rows.Add(dr);
                                        }
                                    }
                                    else if (dtDetail.Rows[i]["AccountCode"].ToString() == "62012012")//Local row 7
                                    {
                                        bool exi = false;
                                        bool exicity = false;
                                        foreach (Detail detail in list)
                                        {

                                            if (detail.Tocity == dtDetail.Rows[i]["Tocity"].ToString() && detail.Row == "7")
                                            {
                                                detail.Pamount += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                                detail.Camount += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                                exi = true;
                                                break;
                                            }
                                        }
                                        if (!exi)
                                        {
                                            Detail detailnew = new Detail();
                                            detailnew.Tocity = dtDetail.Rows[i]["Tocity"].ToString();
                                            detailnew.Row = "7";
                                            detailnew.Cur = dtDetail.Rows[i]["Cur"].ToString();
                                            detailnew.Pamount = dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                            detailnew.Camount = dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                            list.Add(detailnew);
                                        }
                                        for (int j = 0; j < dtall.Rows.Count; j++)
                                        {
                                            if (dtDetail.Rows[i]["Tocity"].ToString() == dtall.Rows[j]["Tocity"].ToString())
                                            {
                                                exicity = true;
                                                break;
                                            }
                                        }
                                        if (!exicity)
                                        {
                                            DataRow dr = dtall.NewRow();
                                            dr["Tocity"] = dtDetail.Rows[i]["Tocity"].ToString();
                                            dtall.Rows.Add(dr);
                                        }
                                    }
                                    else if (dtDetail.Rows[i]["AccountCode"].ToString() == "62012022")//Oversea row 8
                                    {
                                        bool exi = false;
                                        bool exicity = false;
                                        foreach (Detail detail in list)
                                        {

                                            if (detail.Tocity == dtDetail.Rows[i]["Tocity"].ToString() && detail.Row == "8")
                                            {
                                                detail.Pamount += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                                detail.Camount += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                                exi = true;
                                                break;
                                            }
                                        }
                                        if (!exi)
                                        {
                                            Detail detailnew = new Detail();
                                            detailnew.Tocity = dtDetail.Rows[i]["Tocity"].ToString();
                                            detailnew.Row = "8";
                                            detailnew.Cur = dtDetail.Rows[i]["Cur"].ToString();
                                            detailnew.Pamount = dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                            detailnew.Camount = dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                            list.Add(detailnew);
                                        }
                                        for (int j = 0; j < dtall.Rows.Count; j++)
                                        {
                                            if (dtDetail.Rows[i]["Tocity"].ToString() == dtall.Rows[j]["Tocity"].ToString())
                                            {
                                                exicity = true;
                                                break;
                                            }
                                        }
                                        if (!exicity)
                                        {
                                            DataRow dr = dtall.NewRow();
                                            dr["Tocity"] = dtDetail.Rows[i]["Tocity"].ToString();
                                            dtall.Rows.Add(dr);
                                        }
                                    }
                                    else if (dtDetail.Rows[i]["AccountCode"].ToString() == "62020630")//Airport row 9
                                    {
                                        bool exi = false;
                                        bool exicity = false;
                                        foreach (Detail detail in list)
                                        {

                                            if (detail.Tocity == dtDetail.Rows[i]["Tocity"].ToString() && detail.Row == "9")
                                            {
                                                detail.Pamount += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                                detail.Camount += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                                exi = true;
                                                break;
                                            }
                                        }
                                        if (!exi)
                                        {
                                            Detail detailnew = new Detail();
                                            detailnew.Tocity = dtDetail.Rows[i]["Tocity"].ToString();
                                            detailnew.Row = "9";
                                            detailnew.Cur = dtDetail.Rows[i]["Cur"].ToString();
                                            detailnew.Pamount = dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                            detailnew.Camount = dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                            list.Add(detailnew);
                                        }
                                        for (int j = 0; j < dtall.Rows.Count; j++)
                                        {
                                            if (dtDetail.Rows[i]["Tocity"].ToString() == dtall.Rows[j]["Tocity"].ToString())
                                            {
                                                exicity = true;
                                                break;
                                            }
                                        }
                                        if (!exicity)
                                        {
                                            DataRow dr = dtall.NewRow();
                                            dr["Tocity"] = dtDetail.Rows[i]["Tocity"].ToString();
                                            dtall.Rows.Add(dr);
                                        }
                                    }
                                    else
                                    //else if (dtDetail.Rows[i]["AccountCode"].ToString() == "62012014" || dtDetail.Rows[i]["AccountCode"].ToString() == "62012024")//Others row 10
                                    {
                                        bool exi = false;
                                        bool exicity = false;
                                        foreach (Detail detail in list)
                                        {

                                            if (detail.Tocity == dtDetail.Rows[i]["Tocity"].ToString() && detail.Row == "10")
                                            {
                                                detail.Pamount += dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                                detail.Camount += dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                                exi = true;
                                                break;
                                            }
                                        }
                                        if (!exi)
                                        {
                                            Detail detailnew = new Detail();
                                            detailnew.Tocity = dtDetail.Rows[i]["Tocity"].ToString();
                                            detailnew.Row = "10";
                                            detailnew.Cur = dtDetail.Rows[i]["Cur"].ToString();
                                            detailnew.Pamount = dtDetail.Rows[i]["Pamount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Pamount"].ToString());
                                            detailnew.Camount = dtDetail.Rows[i]["Camount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Camount"].ToString());
                                            list.Add(detailnew);
                                        }
                                        for (int j = 0; j < dtall.Rows.Count; j++)
                                        {
                                            if (dtDetail.Rows[i]["Tocity"].ToString() == dtall.Rows[j]["Tocity"].ToString())
                                            {
                                                exicity = true;
                                                break;
                                            }
                                        }
                                        if (!exicity)
                                        {
                                            DataRow dr = dtall.NewRow();
                                            dr["Tocity"] = dtDetail.Rows[i]["Tocity"].ToString();
                                            dtall.Rows.Add(dr);
                                        }
                                    }
                                }
                                string sq = "";

                                foreach (Detail detail in list)
                                {
                                    for (int i = 0; i < dtall.Rows.Count; i++)
                                    {
                                        if (detail.Tocity == dtall.Rows[i]["Tocity"].ToString())
                                        {
                                            dtall.Rows[i]["P" + detail.Row] = detail.Pamount;
                                            dtall.Rows[i]["C" + detail.Row] = detail.Camount;
                                        }
                                    }
                                }
                                //组成3*i的Datatable
                                DataTable dtnew = new DataTable();
                                dtnew.Columns.Add("Tocity", typeof(String));
                                dtnew.Columns.Add("Pamount", typeof(String));//P0,P1,...,P10
                                dtnew.Columns.Add("Camount", typeof(String));//C0,C1,...,C10
                                for (int i = 0; i < dtall.Rows.Count; i++)
                                {
                                    DataRow dr = dtnew.NewRow();
                                    dr["Tocity"] = dtall.Rows[i]["Tocity"].ToString();
                                    string Pamount = "";
                                    Pamount += (string.IsNullOrEmpty(dtall.Rows[i]["P0"].ToString()) ? "0" : dtall.Rows[i]["P0"].ToString()) + ",";
                                    Pamount += (string.IsNullOrEmpty(dtall.Rows[i]["P1"].ToString()) ? "0" : dtall.Rows[i]["P1"].ToString()) + ",";
                                    Pamount += (string.IsNullOrEmpty(dtall.Rows[i]["P2"].ToString()) ? "0" : dtall.Rows[i]["P2"].ToString()) + ",";
                                    Pamount += (string.IsNullOrEmpty(dtall.Rows[i]["P3"].ToString()) ? "0" : dtall.Rows[i]["P3"].ToString()) + ",";
                                    Pamount += (string.IsNullOrEmpty(dtall.Rows[i]["P4"].ToString()) ? "0" : dtall.Rows[i]["P4"].ToString()) + ",";
                                    Pamount += (string.IsNullOrEmpty(dtall.Rows[i]["P5"].ToString()) ? "0" : dtall.Rows[i]["P5"].ToString()) + ",";
                                    Pamount += (string.IsNullOrEmpty(dtall.Rows[i]["P6"].ToString()) ? "0" : dtall.Rows[i]["P6"].ToString()) + ",";
                                    Pamount += (string.IsNullOrEmpty(dtall.Rows[i]["P7"].ToString()) ? "0" : dtall.Rows[i]["P7"].ToString()) + ",";
                                    Pamount += (string.IsNullOrEmpty(dtall.Rows[i]["P8"].ToString()) ? "0" : dtall.Rows[i]["P8"].ToString()) + ",";
                                    Pamount += (string.IsNullOrEmpty(dtall.Rows[i]["P9"].ToString()) ? "0" : dtall.Rows[i]["P9"].ToString()) + ",";
                                    Pamount += string.IsNullOrEmpty(dtall.Rows[i]["P10"].ToString()) ? "0" : dtall.Rows[i]["P10"].ToString();
                                    dr["Pamount"] = Pamount;

                                    string Camount = "";
                                    Camount += (string.IsNullOrEmpty(dtall.Rows[i]["C0"].ToString()) ? "0" : dtall.Rows[i]["C0"].ToString()) + ",";
                                    Camount += (string.IsNullOrEmpty(dtall.Rows[i]["C1"].ToString()) ? "0" : dtall.Rows[i]["C1"].ToString()) + ",";
                                    Camount += (string.IsNullOrEmpty(dtall.Rows[i]["C2"].ToString()) ? "0" : dtall.Rows[i]["C2"].ToString()) + ",";
                                    Camount += (string.IsNullOrEmpty(dtall.Rows[i]["C3"].ToString()) ? "0" : dtall.Rows[i]["C3"].ToString()) + ",";
                                    Camount += (string.IsNullOrEmpty(dtall.Rows[i]["C4"].ToString()) ? "0" : dtall.Rows[i]["C4"].ToString()) + ",";
                                    Camount += (string.IsNullOrEmpty(dtall.Rows[i]["C5"].ToString()) ? "0" : dtall.Rows[i]["C5"].ToString()) + ",";
                                    Camount += (string.IsNullOrEmpty(dtall.Rows[i]["C6"].ToString()) ? "0" : dtall.Rows[i]["C6"].ToString()) + ",";
                                    Camount += (string.IsNullOrEmpty(dtall.Rows[i]["C7"].ToString()) ? "0" : dtall.Rows[i]["C7"].ToString()) + ",";
                                    Camount += (string.IsNullOrEmpty(dtall.Rows[i]["C8"].ToString()) ? "0" : dtall.Rows[i]["C8"].ToString()) + ",";
                                    Camount += (string.IsNullOrEmpty(dtall.Rows[i]["C9"].ToString()) ? "0" : dtall.Rows[i]["C9"].ToString()) + ",";
                                    Camount += string.IsNullOrEmpty(dtall.Rows[i]["C10"].ToString()) ? "0" : dtall.Rows[i]["C10"].ToString();
                                    dr["Camount"] = Camount;

                                    dtnew.Rows.Add(dr);
                                }
                                //
                                DataTable dt = new DataTable();

                                dt.Columns.Add("Row", typeof(Int32));
                                for (int i = 0; i < 4; i++)
                                {
                                    dt.Columns.Add("Station" + i.ToString(), typeof(String));
                                    dt.Columns.Add("Station" + i.ToString() + "_P", typeof(String));
                                    dt.Columns.Add("Station" + i.ToString() + "_C", typeof(String));
                                }
                                for (int i = 0; i < dtnew.Rows.Count / 4 + (dtnew.Rows.Count % 4 == 0 ? 0 : 1); i++)
                                {
                                    int index = 0;
                                    DataRow dr = dt.NewRow();
                                    dr["Row"] = i;
                                    for (int j = 0 + i * 4; j < (dtnew.Rows.Count <= (i + 1) * 4 ? dtnew.Rows.Count : (i + 1) * 4); j++)
                                    {
                                        dr["Station" + index.ToString()] = dtnew.Rows[j]["Tocity"].ToString();
                                        dr["Station" + index.ToString() + "_P"] = dtnew.Rows[j]["Pamount"].ToString();
                                        dr["Station" + index.ToString() + "_C"] = dtnew.Rows[j]["Camount"].ToString();
                                        index++;
                                    }
                                    dt.Rows.Add(dr);
                                }
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    if (dt.Rows[i]["Station0"].ToString() == "")
                                    {
                                        dt.Rows[i]["Station0_P"] = "0,0,0,0,0,0,0,0,0,0,0";
                                        dt.Rows[i]["Station0_C"] = "0,0,0,0,0,0,0,0,0,0,0";
                                    }
                                    if (dt.Rows[i]["Station1"].ToString() == "")
                                    {
                                        dt.Rows[i]["Station1_P"] = "0,0,0,0,0,0,0,0,0,0,0";
                                        dt.Rows[i]["Station1_C"] = "0,0,0,0,0,0,0,0,0,0,0";
                                    }
                                    if (dt.Rows[i]["Station2"].ToString() == "")
                                    {
                                        dt.Rows[i]["Station2_P"] = "0,0,0,0,0,0,0,0,0,0,0";
                                        dt.Rows[i]["Station2_C"] = "0,0,0,0,0,0,0,0,0,0,0";
                                    }
                                    if (dt.Rows[i]["Station3"].ToString() == "")
                                    {
                                        dt.Rows[i]["Station3_P"] = "0,0,0,0,0,0,0,0,0,0,0";
                                        dt.Rows[i]["Station3_C"] = "0,0,0,0,0,0,0,0,0,0,0";
                                    }
                                }
                                //获取审批流程
                                DataTable dtFlow = new DataTable();
                                dtFlow.Columns.Add("Row",typeof(String));
                                dtFlow.Columns.Add("SubmitPerson", typeof(String));
                                dtFlow.Columns.Add("SubmitDate", typeof(String));
                                for (int i = 0; i < 5; i++)
                                {
                                    dtFlow.Columns.Add("Approver" + i.ToString());
                                    dtFlow.Columns.Add("Approver" + i.ToString() + "_Date");
                                }
                                if (dt1.Rows[0]["Step"].ToString() == "0")//草稿
                                {
                                    DataRow dr = dtFlow.NewRow();
                                    dr[0] = "0";
                                    dr[1] = "Submitted by:";
                                    dtFlow.Rows.Add(dr);
                                }
                                else//已正式申请
                                {
                                    for (int i = 0; i < dt1.Rows.Count; i++)
                                    {
                                        if (dt1.Rows[i]["Active"].ToString() == "2")
                                        {
                                            for (int j = dt1.Rows.Count - 1; j > i; j--)
                                            {
                                                dt1.Rows.RemoveAt(j);
                                            }
                                            break;
                                        }
                                    }
                                    for (int i = 0; i < dt1.Rows.Count / 5 + (dt1.Rows.Count % 5 == 0 ? 0 : 1); i++)
                                    {
                                        int index = 0;
                                        DataRow dr = dtFlow.NewRow();
                                        dr["Row"] = i;
                                        if (i == 0)//只在第一行显示一次提单人Submitted by
                                        {
                                            dr["SubmitPerson"] = "Submitted by:\n" + dt1.Rows[0]["CreadedBy"].ToString();
                                            dr["SubmitDate"] = dt1.Rows[0]["CreadedDate"].ToString() == "" ? "" : Convert.ToDateTime(dt1.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd");
                                        }
                                        for (int j = 0 + i * 5; j < ((i + 1) * 5 > dt1.Rows.Count ? dt1.Rows.Count : (i + 1) * 5); j++)//
                                        {
                                            if (dt1.Rows[j]["Status"].ToString()=="1")
                                            {
                                                string msg1 = "";
                                                if (dt1.Rows[j]["FlowFn"].ToString().ToLower() == "verifier")
                                                {
                                                    msg1 = ". To Be Verify:";
                                                }
                                                else if (dt1.Rows[j]["FlowFn"].ToString().ToLower() == "issuer")
                                                {
                                                    msg1 = ". To Be Issue:";
                                                }
                                                else
                                                {
                                                    msg1 = ". Waiting for Approval:";
                                                }
                                                dr["Approver" + index.ToString()] = (j + 1).ToString() +msg1+ "\n" + dt1.Rows[j]["Approver"].ToString();
                                                //dr["Approver" + index.ToString() + "_Date"] = dt1.Rows[j]["ApproveDate"].ToString() == "" ? "Pending" : Convert.ToDateTime(dt1.Rows[j]["ApproveDate"].ToString()).ToString("yyyy/MM/dd");
                                            }
                                            else if (dt1.Rows[j]["Status"].ToString() == "2")
                                            {
                                                string msg1 = "";
                                                if (dt1.Rows[j]["FlowFn"].ToString().ToLower() == "verifier")
                                                {
                                                    msg1 = ". Verified by:";
                                                }
                                                else if (dt1.Rows[j]["FlowFn"].ToString().ToLower() == "issuer")
                                                {
                                                    msg1 = ". Issued by:";
                                                }
                                                else
                                                {
                                                    msg1 = ". Approved by:";
                                                }
                                                if (dt1.Rows[j]["Active"].ToString() == "2")
                                                {
                                                    dr["Approver" + index.ToString()] = (j + 1).ToString() +msg1+ "\n" + dt1.Rows[j]["Approver"].ToString();
                                                    dr["Approver" + index.ToString() + "_Date"] = dt1.Rows[j]["ApproveDate"].ToString() == "" ? "" : ("(Complete)" + Convert.ToDateTime(dt1.Rows[j]["ApproveDate"].ToString()).ToString("yyyy/MM/dd"));
                                                }
                                                else
                                                {
                                                    dr["Approver" + index.ToString()] = (j + 1).ToString() +msg1+ "\n" + dt1.Rows[j]["Approver"].ToString();
                                                    dr["Approver" + index.ToString() + "_Date"] = dt1.Rows[j]["ApproveDate"].ToString() == "" ? "" : Convert.ToDateTime(dt1.Rows[j]["ApproveDate"].ToString()).ToString("yyyy/MM/dd");
                                                }
                                            }
                                            else
                                            {
                                                dr["Approver" + index.ToString()] = (j + 1).ToString() + ". Rejected by:\n" + dt1.Rows[j]["Approver"].ToString();
                                                dr["Approver" + index.ToString() + "_Date"] = dt1.Rows[j]["ApproveDate"].ToString() == "" ? "" : ("(Complete)" + Convert.ToDateTime(dt1.Rows[j]["ApproveDate"].ToString()).ToString("yyyy/MM/dd"));
                                            }
                                            index++;
                                        }
                                        dtFlow.Rows.Add(dr);
                                    }
                                }
                                

                                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                                //控制导出格式
                                RenderingExtension[] resut = ReportViewer1.LocalReport.ListRenderingExtensions();
                                foreach (RenderingExtension item in ReportViewer1.LocalReport.ListRenderingExtensions())
                                {
                                    // item 具有Visable属性，但是这个属性是只读属性，不能修改，所以通过反射进行了修改
                                    if (item.Name.ToUpper() == "PDF")
                                    {
                                        // 显示excel2003格式导出按钮
                                        FieldInfo fi = item.GetType().GetField("m_isVisible", BindingFlags.Instance | BindingFlags.NonPublic);
                                        fi.SetValue(item, true);
                                    }
                                    else
                                    {
                                        FieldInfo fi = item.GetType().GetField("m_isVisible", BindingFlags.Instance | BindingFlags.NonPublic);
                                        fi.SetValue(item, false);
                                    }
                                }

                                ReportViewer1.LocalReport.ReportPath = MapPath("~/rdlc/ReportT.rdlc");
                                ReportViewer1.LocalReport.DataSources.Clear();

                                ReportDataSource rds = new ReportDataSource("DataSet1", dt);
                                ReportViewer1.LocalReport.DataSources.Add(rds);
                                ReportDataSource rds2 = new ReportDataSource("DataSet2", dtFlow);
                                ReportViewer1.LocalReport.DataSources.Add(rds2);

                                ReportParameter rpCur = new ReportParameter("Currency", dt1.Rows[0]["Cur"].ToString());
                                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rpCur });
                                ReportParameter rp = new ReportParameter("Person", dt1.Rows[0]["Person"].ToString());
                                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp });

                                string budget = dt1.Rows[0]["Budget"].ToString() == "1" ? "Budget" : "UnBudget";
                                ReportParameter rpBudget = new ReportParameter("Budget", budget);
                                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rpBudget });

                                string bb = "";
                                if (dt1.Rows[0]["Bdate"].ToString() != "")
                                {
                                    bb += Convert.ToDateTime(dt1.Rows[0]["Bdate"].ToString()).ToString("yyyy/MM/dd");
                                }
                                if (dt1.Rows[0]["Edate"].ToString() != "")
                                {
                                    bb += " - " + Convert.ToDateTime(dt1.Rows[0]["Edate"].ToString()).ToString("yyyy/MM/dd");
                                }
                                ReportParameter rpTravelPeriod = new ReportParameter("Period", bb);
                                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rpTravelPeriod });

                                ReportViewer1.LocalReport.Refresh();
                            }
                            else if (Type == "G")
                            {
                                string aName = "";
                                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                                {
                                    aName = "ADes";
                                }
                                else
                                {
                                    aName = "SAccountName";
                                }
                                string sql = "select [AName]=t2." + aName + ",CONVERT(varchar, Tdate, 111) as Tdate1,t1.ID,t1.Type,t1.Amount,t1.TSation,t1.Fcity,t1.Tcity,t1.Ecompany,t1.Eperson,CONVERT(varchar, t1.EffectTime, 111) as ET,t1.Months,t1.AccountDes from (select * from EeommonDetail where [No]='" + ID + "') t1 left join AccoundCode t2 on t2.SAccountCode=t1.AccountCode order by [Type],[AccountCode]";
                                DataTable dtDetail = new DataTable();
                                dtDetail = dbc.GetData("eReimbursement", sql);

                                DataTable dt = new DataTable();
                                dt.Columns.Add("AName",typeof(String));
                                dt.Columns.Add("Tdate1", typeof(String));
                                dt.Columns.Add("ID", typeof(String));
                                dt.Columns.Add("Type", typeof(String));
                                dt.Columns.Add("Amount", typeof(String));
                                dt.Columns.Add("TSation", typeof(String));
                                dt.Columns.Add("Fcity", typeof(String));
                                dt.Columns.Add("Tcity", typeof(String));
                                dt.Columns.Add("Ecompany", typeof(String));
                                dt.Columns.Add("Eperson", typeof(String));
                                dt.Columns.Add("Months", typeof(String));
                                dt.Columns.Add("AccountDes", typeof(String));
                                dt.Columns.Add("Mark", typeof(String));
                                //int ECount = 0; int TCount = 0; int CCount = 0; int OCount = 0;
                                decimal total = 0;
                                for (int i = 0; i < dtDetail.Rows.Count; i++)
                                {
                                    total += dtDetail.Rows[i]["Amount"].ToString() == "" ? 0 : Convert.ToDecimal(dtDetail.Rows[i]["Amount"].ToString());
                                    //添加分类标题行
                                    if (dtDetail.Rows[i]["Type"].ToString() == "E")
                                    {
                                        if (i == 0 || (dtDetail.Rows[i - 1]["Type"].ToString() != "E"))
                                        {
                                            DataRow dr = dt.NewRow();
                                            dr["ID"] = "E1";
                                            dr["AName"] = "Entertainment:";
                                            dr["Amount"] = "0";
                                            dr["Mark"] = "-1";
                                            dt.Rows.Add(dr);
                                            dr = dt.NewRow();
                                            dr["ID"] = "E2";
                                            dr["AName"] = "Expense Type";
                                            dr["Tdate1"] = "Date";
                                            dr["Type"] = "";
                                            dr["Amount"] = "0";
                                            dr["TSation"] = "Collection Station";
                                            dr["Fcity"] = "";
                                            dr["Tcity"] = "";
                                            dr["Ecompany"] = "Customer";
                                            dr["Eperson"] = "Guest";
                                            dr["Months"] = "";
                                            dr["AccountDes"] = "Remark";
                                            dr["Mark"] = "-1";
                                            dt.Rows.Add(dr);
                                        }
                                    }
                                    else if (dtDetail.Rows[i]["Type"].ToString() == "T")
                                    {
                                        if (i == 0 || (dtDetail.Rows[i - 1]["Type"].ToString() != "T"))
                                        {
                                            DataRow dr = dt.NewRow();
                                            dr["ID"] = "T1";
                                            dr["AName"] = "Transportation:";
                                            dr["Amount"] = "0";
                                            dr["Mark"] = "-1";
                                            dt.Rows.Add(dr);
                                            dr = dt.NewRow();
                                            dr["ID"] = "T2";
                                            dr["AName"] = "Expense Type";
                                            dr["Tdate1"] = "Date";
                                            dr["Type"] = "";
                                            dr["Amount"] = "0";
                                            dr["TSation"] = "Collection Station";
                                            dr["Fcity"] = "";
                                            dr["Tcity"] = "";
                                            dr["Ecompany"] = "From";
                                            dr["Eperson"] = "To";
                                            dr["Months"] = "";
                                            dr["AccountDes"] = "Remark";
                                            dt.Rows.Add(dr);
                                        }
                                    }
                                    else if (dtDetail.Rows[i]["Type"].ToString() == "C")
                                    {
                                        if (i == 0 || (dtDetail.Rows[i - 1]["Type"].ToString() != "C"))
                                        {
                                            DataRow dr = dt.NewRow();
                                            dr["ID"] = "C1";
                                            dr["AName"] = "Communication:";
                                            dr["Amount"] = "0";
                                            dr["Mark"] = "-1";
                                            dt.Rows.Add(dr);
                                            dr = dt.NewRow();
                                            dr["ID"] = "C2";
                                            dr["AName"] = "Expense Type";
                                            dr["Tdate1"] = "Period";
                                            dr["Type"] = "";
                                            dr["Amount"] = "0";
                                            dr["TSation"] = "Collection Station";
                                            dr["Fcity"] = "";
                                            dr["Tcity"] = "";
                                            dr["Ecompany"] = "";
                                            dr["Eperson"] = "";
                                            dr["Months"] = "";
                                            dr["AccountDes"] = "Remark";
                                            dr["Mark"] = "-1";
                                            dt.Rows.Add(dr);
                                        }
                                    }
                                    else
                                    {
                                        if (i == 0 || (dtDetail.Rows[i - 1]["Type"].ToString() != "O"))
                                        {
                                            DataRow dr = dt.NewRow();
                                            dr["ID"] = "O1";
                                            dr["AName"] = "Others:";
                                            dr["Amount"] = "0";
                                            dr["Mark"] = "-1";
                                            dt.Rows.Add(dr);
                                            dr = dt.NewRow();
                                            dr["ID"] = "O2";
                                            dr["AName"] = "Expense Type";
                                            dr["Tdate1"] = "Date";
                                            dr["Type"] = "";
                                            dr["Amount"] = "0";
                                            dr["TSation"] = "Collection Station";
                                            dr["Fcity"] = "";
                                            dr["Tcity"] = "";
                                            dr["Ecompany"] = "";
                                            dr["Eperson"] = "";
                                            dr["Months"] = "";
                                            dr["AccountDes"] = "Remark";
                                            dt.Rows.Add(dr);
                                        }
                                    }
                                    DataRow drdetail = dt.NewRow();
                                    drdetail["AName"] = dtDetail.Rows[i]["AName"].ToString();
                                    if (dtDetail.Rows[i]["Type"].ToString() == "C")
                                    {
                                        drdetail["Tdate1"] = dtDetail.Rows[i]["ET"].ToString();
                                    }
                                    else
                                    {
                                        drdetail["Tdate1"] = dtDetail.Rows[i]["Tdate1"].ToString();
                                    }
                                    drdetail["ID"] = dtDetail.Rows[i]["ID"].ToString();
                                    drdetail["Type"] = dtDetail.Rows[i]["Type"].ToString();
                                    drdetail["Amount"] = dtDetail.Rows[i]["Amount"].ToString();
                                    drdetail["TSation"] = dtDetail.Rows[i]["TSation"].ToString();
                                    drdetail["Fcity"] = dtDetail.Rows[i]["Fcity"].ToString();
                                    drdetail["Tcity"] = dtDetail.Rows[i]["Tcity"].ToString();
                                    drdetail["Ecompany"] = dtDetail.Rows[i]["Ecompany"].ToString();
                                    drdetail["Eperson"] = dtDetail.Rows[i]["Eperson"].ToString();
                                    drdetail["Months"] = dtDetail.Rows[i]["Months"].ToString();
                                    drdetail["AccountDes"] = dtDetail.Rows[i]["AccountDes"].ToString();
                                    dt.Rows.Add(drdetail);

                                    //if (dtDetail.Rows[i]["Type"].ToString() == "E")
                                    //{
                                    //    ECount++;
                                    //}
                                    //else if (dtDetail.Rows[i]["Type"].ToString() == "T")
                                    //{
                                    //    TCount++;
                                    //}
                                    //else if (dtDetail.Rows[i]["Type"].ToString() == "C")
                                    //{
                                    //    CCount++;
                                    //}
                                    //else
                                    //{
                                    //    OCount++;
                                    //}
                                }
                                //获取审批流程
                                DataTable dtFlow = new DataTable();
                                dtFlow.Columns.Add("Row", typeof(String));
                                dtFlow.Columns.Add("SubmitPerson", typeof(String));
                                dtFlow.Columns.Add("SubmitDate", typeof(String));
                                for (int i = 0; i < 6; i++)
                                {
                                    dtFlow.Columns.Add("Approver" + i.ToString());
                                    dtFlow.Columns.Add("Approver" + i.ToString() + "_Date");
                                }
                                if (dt1.Rows[0]["Step"].ToString() == "0")//草稿
                                {
                                    DataRow dr = dtFlow.NewRow();
                                    dr[0] = "0";
                                    dr[1] = "Submitted by:";
                                    dtFlow.Rows.Add(dr);
                                }
                                else//已正式申请
                                {
                                    for (int i = 0; i < dt1.Rows.Count; i++)
                                    {
                                        if (dt1.Rows[i]["Active"].ToString() == "2")
                                        {
                                            for (int j = dt1.Rows.Count - 1; j > i; j--)
                                            {
                                                dt1.Rows.RemoveAt(j);
                                            }
                                            break;
                                        }
                                    }
                                    for (int i = 0; i < dt1.Rows.Count / 6 + (dt1.Rows.Count % 6 == 0 ? 0 : 1); i++)
                                    {
                                        int index = 0;
                                        DataRow dr = dtFlow.NewRow();
                                        dr["Row"] = i;
                                        if (i == 0)//只在第一行显示一次提单人Submitted by
                                        {
                                            dr["SubmitPerson"] = "Submitted by:\n" + dt1.Rows[0]["CreadedBy"].ToString();
                                            dr["SubmitDate"] = dt1.Rows[0]["CreadedDate"].ToString() == "" ? "" : Convert.ToDateTime(dt1.Rows[0]["CreadedDate"].ToString()).ToString("yyyy/MM/dd");
                                        }
                                        for (int j = 0 + i * 6; j < ((i + 1) * 6 > dt1.Rows.Count ? dt1.Rows.Count : (i + 1) * 6); j++)//
                                        {
                                            if (dt1.Rows[j]["Status"].ToString() == "1")
                                            {
                                                string msg1 = "";
                                                if (dt1.Rows[j]["FlowFn"].ToString().ToLower() == "verifier")
                                                {
                                                    msg1 = ". To Be Verify:";
                                                }
                                                else if (dt1.Rows[j]["FlowFn"].ToString().ToLower() == "issuer")
                                                {
                                                    msg1 = ". To Be Issue:";
                                                }
                                                else
                                                {
                                                    msg1 = ". Waiting for Approval:";
                                                }
                                                dr["Approver" + index.ToString()] = (j + 1).ToString() +msg1+ "\n" + dt1.Rows[j]["Approver"].ToString();
                                                //dr["Approver" + index.ToString() + "_Date"] = dt1.Rows[j]["ApproveDate"].ToString() == "" ? "Pending" : Convert.ToDateTime(dt1.Rows[j]["ApproveDate"].ToString()).ToString("yyyy/MM/dd");
                                            }
                                            else if (dt1.Rows[j]["Status"].ToString() == "2")
                                            {
                                                string msg1 = "";
                                                if (dt1.Rows[j]["FlowFn"].ToString().ToLower() == "verifier")
                                                {
                                                    msg1 = ". Verified by:";
                                                }
                                                else if (dt1.Rows[j]["FlowFn"].ToString().ToLower() == "issuer")
                                                {
                                                    msg1 = ". Issued by:";
                                                }
                                                else
                                                {
                                                    msg1 = ". Approved by:";
                                                }

                                                if (dt1.Rows[j]["Active"].ToString() == "2")
                                                {
                                                    dr["Approver" + index.ToString()] = (j + 1).ToString() +msg1+ "\n" + dt1.Rows[j]["Approver"].ToString();
                                                    dr["Approver" + index.ToString() + "_Date"] = dt1.Rows[j]["ApproveDate"].ToString() == "" ? "" : ("(Complete)" + Convert.ToDateTime(dt1.Rows[j]["ApproveDate"].ToString()).ToString("yyyy/MM/dd"));
                                                }
                                                else
                                                {
                                                    dr["Approver" + index.ToString()] = (j + 1).ToString() +msg1+ "\n" + dt1.Rows[j]["Approver"].ToString();
                                                    dr["Approver" + index.ToString() + "_Date"] = dt1.Rows[j]["ApproveDate"].ToString() == "" ? "" : Convert.ToDateTime(dt1.Rows[j]["ApproveDate"].ToString()).ToString("yyyy/MM/dd");
                                                }
                                            }
                                            else
                                            {
                                                dr["Approver" + index.ToString()] = (j + 1).ToString() + ". Rejected by:\n" + dt1.Rows[j]["Approver"].ToString();
                                                dr["Approver" + index.ToString() + "_Date"] = dt1.Rows[j]["ApproveDate"].ToString() == "" ? "" : ("(Complete)" + Convert.ToDateTime(dt1.Rows[j]["ApproveDate"].ToString()).ToString("yyyy/MM/dd"));
                                            }
                                            index++;
                                        }
                                        dtFlow.Rows.Add(dr);
                                    }
                                }

                                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                                //控制导出格式
                                RenderingExtension[] resut = ReportViewer1.LocalReport.ListRenderingExtensions();
                                foreach (RenderingExtension item in ReportViewer1.LocalReport.ListRenderingExtensions())
                                {
                                    // item 具有Visable属性，但是这个属性是只读属性，不能修改，所以通过反射进行了修改
                                    if (item.Name.ToUpper() == "PDF")
                                    {
                                        // 显示excel2003格式导出按钮
                                        FieldInfo fi = item.GetType().GetField("m_isVisible", BindingFlags.Instance | BindingFlags.NonPublic);
                                        fi.SetValue(item, true);
                                    }
                                    else
                                    {
                                        FieldInfo fi = item.GetType().GetField("m_isVisible", BindingFlags.Instance | BindingFlags.NonPublic);
                                        fi.SetValue(item, false);
                                    }
                                }

                                ReportViewer1.LocalReport.ReportPath = MapPath("~/rdlc/ReportG1.rdlc");
                                ReportViewer1.LocalReport.DataSources.Clear();

                                ReportDataSource rds = new ReportDataSource("DataSet1", dt);
                                ReportViewer1.LocalReport.DataSources.Add(rds);

                                ReportDataSource rds2 = new ReportDataSource("DataSet2", dtFlow);
                                ReportViewer1.LocalReport.DataSources.Add(rds2);

                                ReportParameter rpCur = new ReportParameter("Currency", dt1.Rows[0]["Cur"].ToString());
                                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rpCur });

                                ReportParameter rp = new ReportParameter("Person", dt1.Rows[0]["Person"].ToString());
                                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp });

                                ReportParameter rpMonth = new ReportParameter("Month", dt1.Rows[0]["Month"].ToString());
                                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rpMonth });

                                ReportParameter rpSum = new ReportParameter("Sum", total.ToString());
                                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rpSum });

                                ReportParameter rpRows = new ReportParameter("Rows", dt.Rows.Count.ToString());
                                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rpRows });

                                ReportParameter rpDepartment = new ReportParameter("Department", dt1.Rows[0]["Department"].ToString() + " @ " + dt1.Rows[0]["Station"].ToString());
                                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rpDepartment });

                                string budget = dt1.Rows[0]["Budget"].ToString() == "1" ? "Budget" : "UnBudget";
                                ReportParameter rpBudget = new ReportParameter("Budget", budget);
                                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rpBudget });

                                ReportViewer1.LocalReport.Refresh();
                            }
                        }
                    }
                }
                
            }
            
        }
        //private void ReflectivelySetVisibilityFalse(RenderingExtension extension)
        //{
        //    FieldInfo info = extension.GetType().GetField("m_serverExtension",
        //                                                  BindingFlags.NonPublic
        //                    | BindingFlags.Instance);
        //    if (info != null)
        //    {

        //        Extension rsExtension = info.GetValue(extension) as Extension;
        //        if (rsExtension != null)
        //        {
        //            rsExtension.Visible = false;
        //        }
        //    }
        //}
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
        private class Detail
        {
            public string Tocity { get; set; }
            public string Row { get; set; }
            public string Cur { get; set; }
            public decimal Pamount { get; set; }
            public decimal Camount { get; set; }
        }
        private class DetailAll
        {
            public string Tocity { get; set; }
            public string P0 { get; set; }
            public string C0 { get; set; }
            public string P1 { get; set; }
            public string C1 { get; set; }
            public string P2 { get; set; }
            public string C2 { get; set; }
            public string P3 { get; set; }
            public string C3 { get; set; }
            public string P4 { get; set; }
            public string C4 { get; set; }
            public string P5 { get; set; }
            public string C5 { get; set; }
            public string P6 { get; set; }
            public string C6 { get; set; }
            public string P7 { get; set; }
            public string C7 { get; set; }
            public string P8 { get; set; }
            public string C8 { get; set; }
            public string P9 { get; set; }
            public string C9 { get; set; }
            public string P10 { get; set; }
            public string C10 { get; set; }
        }
    }
}