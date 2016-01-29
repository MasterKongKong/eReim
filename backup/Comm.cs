using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBOperatorSql;
using System.Data;
public class Comm
{
  public static DBAdapter dbs;
    public Comm()
    {
    }
    public static int JdRole(string page)
    {

          
            string pname = page.Substring(page.LastIndexOf("/") + 1);

            if (HttpContext.Current.Session["UserID"] == null)
            {

                return 0;
            }
            else
            {
                if (dbs == null)
                {
                    dbs = new DBAdapter();
                }
                string querylist = dbs.ExeSqlScalar("select ModuleID from StationRole  where UserID='" + HttpContext.Current.Session["UserID"] + "'").ToString();
                if (querylist == "")
                {
                    return 0;
                }
                else
                {
                    return int.Parse(dbs.ExeSqlScalar("select count(*) from  ModuleManage where charindex('" + pname + "',page)>0 and  id in (0" + querylist + "0)"));
                }
            }
           
    }
    public static DataTable ExRtnEB(string userid, string dpt, string ostation, string tstation, string accountcode, string Years, string month,string T,string id)
    {
        if (dbs == null)
        {
            dbs = new DBAdapter();
        }
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("Type", typeof(string)));
        dt.Columns.Add(new DataColumn("Used", typeof(string)));
        dt.Columns.Add(new DataColumn("Budget", typeof(string)));
        dt.Columns.Add(new DataColumn("Station", typeof(string)));
        string thestation = ostation;
        string sss = "station2";
        if (ostation != tstation)
        {
            thestation = tstation;
            sss = "tsation";
        }
        string ExT = "";
        string ExC = "";
        if (T == "T")
        {
            ExT = "a.id<>'" + id + "' and ";
        }
        else
        {
            ExC = "a.id<>'" + id + "' and ";
        }
        string travelUsed = dbs.ExeSqlScalar("select isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0) from ETraveleDetail as b inner join ETravel as a on a.id=b.no where  " + ExT + " a.type='0' and a.status<>'3' and  " + sss + "='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "' AND MONTH(tdate)='" + month + "' and year(tdate)='" + Years + "'").ToString();
        string commUsed = dbs.ExeSqlScalar("select sum(b.CostCenterAmount) from EeommonDetail as b inner join Ecommon as a on a.id=b.no where " + ExC + " a.type='0'  and a.status<>'3' and   " + sss + "='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "'  AND MONTH(tdate)='" + month + "' and year(tdate)='" + Years + "'").ToString();
        string Budget = dbs.ExeSqlScalar("SELECT sum(b.Amount) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.BaccountCode='" + accountcode + "' AND b.MONTHS='" + month + "' and a.Years='" + Years + "'");
        double tt = jds(travelUsed) + jds(commUsed);

        DataRow dr = dt.NewRow();
        dr["Type"] = "站点";
        dr["Used"] = tt.ToString("F2");
        dr["Budget"] = jds(Budget).ToString("F2");
        dr["Station"] = thestation;
        dt.Rows.Add(dr);
        if (ostation != tstation)
        {
            string qtravelUsed3 = dbs.ExeSqlScalar("select isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0) from ETraveleDetail as b inner join ETravel as a on a.id=b.no where " + ExT + " a.type='0' and a.status<>'3' and  station2='" + tstation + "' AND ACCOUNTCODE='" + accountcode + "' AND year(tdate)='" + Years + "'").ToString();
            string qcommUsed3 = dbs.ExeSqlScalar("select sum(b.CostCenterAmount) from EeommonDetail as b inner join Ecommon as a on a.id=b.no where "+ExC+" a.type='0' and a.status<>'3' and  station2='" + tstation + "' AND ACCOUNTCODE='" + accountcode + "'  and year(tdate)='" + Years + "' ").ToString();
            string qBudget3 = dbs.ExeSqlScalar("SELECT sum(b.Amount) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + tstation + "' AND b.BaccountCode='" + accountcode + "' and a.Years='" + Years + "' ");
            double qtt3 = jds(qtravelUsed3) + jds(qcommUsed3);
            DataRow qdr3 = dt.NewRow();
            qdr3["Type"] = "全年站点";
            qdr3["Used"] = qtt3.ToString("F2");
            qdr3["Budget"] = jds(qBudget3).ToString("F2");
            qdr3["Station"] = thestation;
            dt.Rows.Add(qdr3);
        }
        else
        {
            //string mmm = dbs.ExeSqlScalar("SELECT count(*) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.SACCOUNDCODE='" + accountcode + "' AND b.MONTHS='" + month + "' and a.Years='" + Years + "'");
            //if (int.Parse(mmm) > 0)
            //{
            string type = dbs.ExeSqlScalar("SELECT count(*) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.BaccountCode='" + accountcode + "' AND b.MONTHS='" + month + "' and a.Years='" + Years + "' and userid='" + userid + "'");
            string depp = "Deptment";
            //if (int.Parse(type) > 0)
            //{
            //    depp = "Deptment";
            //}
            string travelUsed2 = dbs.ExeSqlScalar("select isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0) from ETraveleDetail as b inner join ETravel as a on a.id=b.no where " + ExT + " a.type='0'  and a.status<>'3' and station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "' AND MONTH(tdate)='" + month + "' and year(tdate)='" + Years + "' and a.Department='" + dpt + "'").ToString();
            string commUsed2 = dbs.ExeSqlScalar("select sum(b.CostCenterAmount) from EeommonDetail as b inner join Ecommon as a on a.id=b.no where " + ExC + " a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "'  AND MONTH(tdate)='" + month + "' and year(tdate)='" + Years + "' and a.Department='" + dpt + "'").ToString();
            string Budget2 = dbs.ExeSqlScalar("SELECT sum(b.Amount) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.SACCOUNDCODE='" + accountcode + "' AND b.MONTHS='" + month + "' and a.Years='" + Years + "' and " + depp + "='" + dpt + "'");
            double tt2 = jds(travelUsed2) + jds(commUsed2);
            DataRow dr2 = dt.NewRow();
            dr2["Type"] = "部门";
            dr2["Used"] = tt2.ToString("F2");
            dr2["Budget"] = jds(Budget2).ToString("F2");
            dr2["Station"] = thestation;
            dt.Rows.Add(dr2);

            //if (int.Parse(type) > 0)
            //{
            string travelUsed1 = dbs.ExeSqlScalar("select isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0) from ETraveleDetail as b inner join ETravel as a on a.id=b.no where " + ExT + "  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "' AND MONTH(tdate)='" + month + "' and year(tdate)='" + Years + "' and PersonID='" + userid + "'").ToString();
            string commUsed1 = dbs.ExeSqlScalar("select sum(b.CostCenterAmount) from EeommonDetail as b inner join Ecommon as a on a.id=b.no where " + ExC + " a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "'  AND MONTH(tdate)='" + month + "' and year(tdate)='" + Years + "' and PersonID='" + userid + "'").ToString();
            string Budget1 = dbs.ExeSqlScalar("SELECT sum(b.Amount) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.BaccountCode='" + accountcode + "' AND b.MONTHS='" + month + "' and a.Years='" + Years + "' and userid='" + userid + "'");
            double tt1 = jds(travelUsed1) + jds(commUsed1);
            DataRow dr1 = dt.NewRow();
            dr1["Type"] = "个人";
            dr1["Used"] = tt1.ToString("F2");
            dr1["Budget"] = jds(Budget1).ToString("F2");
            dr1["Station"] = thestation;
            dt.Rows.Add(dr1);

            string qtravelUsed1 = dbs.ExeSqlScalar("select isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0) from ETraveleDetail as b inner join ETravel as a on a.id=b.no where " + ExT + " a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "' AND year(tdate)='" + Years + "' and PersonID='" + userid + "'").ToString();
            string qcommUsed1 = dbs.ExeSqlScalar("select sum(b.CostCenterAmount) from EeommonDetail as b inner join Ecommon as a on a.id=b.no where " + ExC + " a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "'  and year(tdate)='" + Years + "' and PersonID='" + userid + "'").ToString();
            string qBudget1 = dbs.ExeSqlScalar("SELECT sum(b.Amount) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.BaccountCode='" + accountcode + "' and a.Years='" + Years + "' and userid='" + userid + "'");
            double qtt1 = jds(qtravelUsed1) + jds(qcommUsed1);
            DataRow qdr1 = dt.NewRow();
            qdr1["Type"] = "全年个人";
            qdr1["Used"] = qtt1.ToString("F2");
            qdr1["Budget"] = jds(qBudget1).ToString("F2");
            qdr1["Station"] = thestation;
            dt.Rows.Add(qdr1);

            string qtravelUsed2 = dbs.ExeSqlScalar("select isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0) from ETraveleDetail as b inner join ETravel as a on a.id=b.no where " + ExT + "  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "' AND year(tdate)='" + Years + "' and a.Department='" + dpt + "'").ToString();
            string qcommUsed2 = dbs.ExeSqlScalar("select sum(b.CostCenterAmount) from EeommonDetail as b inner join Ecommon as a on a.id=b.no where " + ExC + " a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "'  and year(tdate)='" + Years + "' and a.Department='" + dpt + "'").ToString();
            string qBudget2 = dbs.ExeSqlScalar("SELECT sum(b.Amount) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.BaccountCode='" + accountcode + "' and a.Years='" + Years + "' and Deptment='" + dpt + "'");
            double qtt2 = jds(qtravelUsed2) + jds(qcommUsed2);
            DataRow qdr2 = dt.NewRow();
            qdr2["Type"] = "全年部门";
            qdr2["Used"] = qtt2.ToString("F2");
            qdr2["Budget"] = jds(qBudget2).ToString("F2");
            qdr2["Station"] = thestation;
            dt.Rows.Add(qdr2);
            string qtravelUsed3 = dbs.ExeSqlScalar("select isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0) from ETraveleDetail as b inner join ETravel as a on a.id=b.no where " + ExT + "  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "' AND year(tdate)='" + Years + "'").ToString();
            string qcommUsed3 = dbs.ExeSqlScalar("select sum(b.CostCenterAmount) from EeommonDetail as b inner join Ecommon as a on a.id=b.no where " + ExC + " a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "'  and year(tdate)='" + Years + "' ").ToString();
            string qBudget3 = dbs.ExeSqlScalar("SELECT sum(b.Amount) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.BaccountCode='" + accountcode + "' and a.Years='" + Years + "' ");
            double qtt3 = jds(qtravelUsed3) + jds(qcommUsed3);
            DataRow qdr3 = dt.NewRow();
            qdr3["Type"] = "全年站点";
            qdr3["Used"] = qtt3.ToString("F2");
            qdr3["Budget"] = jds(qBudget3).ToString("F2");
            qdr3["Station"] = thestation;
            dt.Rows.Add(qdr3);

            //}
            //else
            //{
            //    string travelUsed1 = dbs.ExeSqlScalar("select isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0) from ETraveleDetail as b inner join ETravel as a on a.id=b.no where  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "' AND MONTH(tdate)='" + month + "' and year(tdate)='" + Years + "' and PersonID='" + userid + "'").ToString();
            //    string commUsed1 = dbs.ExeSqlScalar("select sum(b.CostCenterAmount) from EeommonDetail as b inner join Ecommon as a on a.id=b.no where  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "'  AND MONTH(tdate)='" + month + "' and year(tdate)='" + Years + "' and PersonID='" + userid + "'").ToString();
            //    string Budget1 = dbs.ExeSqlScalar("SELECT sum(b.Amount) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.SACCOUNDCODE='" + accountcode + "' AND b.MONTHS='" + month + "' and a.Years='" + Years + "' and userid='" + userid + "'");
            //    double tt1 = jds(travelUsed1) + jds(commUsed1);
            //    DataRow dr1 = dt.NewRow();
            //    dr1["Type"] = "个人";
            //    dr1["Used"] = tt1.ToString("F2");
            //    dr1["Budget"] = jds(Budget1).ToString("F2");
            //    dr1["Station"] = thestation;
            //    dt.Rows.Add(dr1);

            //    string qtravelUsed1 = dbs.ExeSqlScalar("select isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0) from ETraveleDetail as b inner join ETravel as a on a.id=b.no where  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "' AND year(tdate)='" + Years + "' and PersonID='" + userid + "'").ToString();
            //    string qcommUsed1 = dbs.ExeSqlScalar("select sum(b.CostCenterAmount) from EeommonDetail as b inner join Ecommon as a on a.id=b.no where  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "'  and year(tdate)='" + Years + "' and PersonID='" + userid + "'").ToString();
            //    string qBudget1 = dbs.ExeSqlScalar("SELECT sum(b.Amount) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.SACCOUNDCODE='" + accountcode + "' and a.Years='" + Years + "' and name='" + userid + "'");
            //    double qtt1 = jds(qtravelUsed1) + jds(qcommUsed1);
            //    DataRow qdr1 = dt.NewRow();
            //    qdr1["Type"] = "全年";
            //    qdr1["Used"] = qtt1.ToString("F2");
            //    qdr1["Budget"] = jds(qBudget1).ToString("F2");
            //    qdr1["Station"] = thestation;
            //    dt.Rows.Add(qdr1);
            //}



        }
        return dt;
    }
    public static DataTable RtnEB(string userid, string dpt, string ostation, string tstation, string accountcode, string Years, string month)
    {
        if (dbs == null)
        {
            dbs = new DBAdapter();
        }
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("Type", typeof(string)));
        dt.Columns.Add(new DataColumn("Used", typeof(string)));
        dt.Columns.Add(new DataColumn("Budget", typeof(string)));
        dt.Columns.Add(new DataColumn("Station", typeof(string)));
        string thestation = ostation;
        string sss = "station2";
        if (ostation != tstation)
        {
            thestation = tstation;
            sss = "tsation";
        }
        string travelUsed = dbs.ExeSqlScalar("select isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0) from ETraveleDetail as b inner join ETravel as a on a.id=b.no where  a.type='0' and a.status<>'3' and  " + sss + "='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "' AND MONTH(tdate)='" + month + "' and year(tdate)='" + Years + "'").ToString();
        string commUsed = dbs.ExeSqlScalar("select sum(b.CostCenterAmount) from EeommonDetail as b inner join Ecommon as a on a.id=b.no where  a.type='0'  and a.status<>'3' and   " + sss + "='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "'  AND MONTH(tdate)='" + month + "' and year(tdate)='" + Years + "'").ToString();
        string Budget = dbs.ExeSqlScalar("SELECT sum(b.Amount) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.BaccountCode='" + accountcode + "' AND b.MONTHS='" + month + "' and a.Years='" + Years + "'");
        double tt = jds(travelUsed) + jds(commUsed);

        DataRow dr = dt.NewRow();
        dr["Type"] = "站点";
        dr["Used"] = tt.ToString("F2");
        dr["Budget"] = jds(Budget).ToString("F2");
        dr["Station"] = thestation;
        dt.Rows.Add(dr);
        if (ostation != tstation)
        {
            string qtravelUsed3 = dbs.ExeSqlScalar("select isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0) from ETraveleDetail as b inner join ETravel as a on a.id=b.no where  a.type='0' and a.status<>'3' and  station2='" + tstation + "' AND ACCOUNTCODE='" + accountcode + "' AND year(tdate)='" + Years + "'").ToString();
            string qcommUsed3 = dbs.ExeSqlScalar("select sum(b.CostCenterAmount) from EeommonDetail as b inner join Ecommon as a on a.id=b.no where  a.type='0' and a.status<>'3' and  station2='" + tstation + "' AND ACCOUNTCODE='" + accountcode + "'  and year(tdate)='" + Years + "' ").ToString();
            string qBudget3 = dbs.ExeSqlScalar("SELECT sum(b.Amount) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + tstation + "' AND b.BaccountCode='" + accountcode + "' and a.Years='" + Years + "' ");
            double qtt3 = jds(qtravelUsed3) + jds(qcommUsed3);
            DataRow qdr3 = dt.NewRow();
            qdr3["Type"] = "全年站点";
            qdr3["Used"] = qtt3.ToString("F2");
            qdr3["Budget"] = jds(qBudget3).ToString("F2");
            qdr3["Station"] = thestation;
            dt.Rows.Add(qdr3);
        }
        else
        {
            //string mmm = dbs.ExeSqlScalar("SELECT count(*) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.SACCOUNDCODE='" + accountcode + "' AND b.MONTHS='" + month + "' and a.Years='" + Years + "'");
            //if (int.Parse(mmm) > 0)
            //{
            string type = dbs.ExeSqlScalar("SELECT count(*) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.BaccountCode='" + accountcode + "' AND b.MONTHS='" + month + "' and a.Years='" + Years + "' and userid='" + userid + "'");
            string depp = "Deptment";
            //if (int.Parse(type) > 0)
            //{
            //    depp = "Deptment";
            //}
            string travelUsed2 = dbs.ExeSqlScalar("select isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0) from ETraveleDetail as b inner join ETravel as a on a.id=b.no where a.type='0'  and a.status<>'3' and station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "' AND MONTH(tdate)='" + month + "' and year(tdate)='" + Years + "' and a.Department='" + dpt + "'").ToString();
            string commUsed2 = dbs.ExeSqlScalar("select sum(b.CostCenterAmount) from EeommonDetail as b inner join Ecommon as a on a.id=b.no where  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "'  AND MONTH(tdate)='" + month + "' and year(tdate)='" + Years + "' and a.Department='" + dpt + "'").ToString();
            string Budget2 = dbs.ExeSqlScalar("SELECT sum(b.Amount) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.SACCOUNDCODE='" + accountcode + "' AND b.MONTHS='" + month + "' and a.Years='" + Years + "' and " + depp + "='" + dpt + "'");
            double tt2 = jds(travelUsed2) + jds(commUsed2);
            DataRow dr2 = dt.NewRow();
            dr2["Type"] = "部门";
            dr2["Used"] = tt2.ToString("F2");
            dr2["Budget"] = jds(Budget2).ToString("F2");
            dr2["Station"] = thestation;
            dt.Rows.Add(dr2);

            //if (int.Parse(type) > 0)
            //{
            string travelUsed1 = dbs.ExeSqlScalar("select isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0) from ETraveleDetail as b inner join ETravel as a on a.id=b.no where  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "' AND MONTH(tdate)='" + month + "' and year(tdate)='" + Years + "' and PersonID='" + userid + "'").ToString();
            string commUsed1 = dbs.ExeSqlScalar("select sum(b.CostCenterAmount) from EeommonDetail as b inner join Ecommon as a on a.id=b.no where  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "'  AND MONTH(tdate)='" + month + "' and year(tdate)='" + Years + "' and PersonID='" + userid + "'").ToString();
            string Budget1 = dbs.ExeSqlScalar("SELECT sum(b.Amount) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.BaccountCode='" + accountcode + "' AND b.MONTHS='" + month + "' and a.Years='" + Years + "' and userid='" + userid + "'");
            double tt1 = jds(travelUsed1) + jds(commUsed1);
            DataRow dr1 = dt.NewRow();
            dr1["Type"] = "个人";
            dr1["Used"] = tt1.ToString("F2");
            dr1["Budget"] = jds(Budget1).ToString("F2");
            dr1["Station"] = thestation;
            dt.Rows.Add(dr1);

            string qtravelUsed1 = dbs.ExeSqlScalar("select isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0) from ETraveleDetail as b inner join ETravel as a on a.id=b.no where  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "' AND year(tdate)='" + Years + "' and PersonID='" + userid + "'").ToString();
            string qcommUsed1 = dbs.ExeSqlScalar("select sum(b.CostCenterAmount) from EeommonDetail as b inner join Ecommon as a on a.id=b.no where  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "'  and year(tdate)='" + Years + "' and PersonID='" + userid + "'").ToString();
            string qBudget1 = dbs.ExeSqlScalar("SELECT sum(b.Amount) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.BaccountCode='" + accountcode + "' and a.Years='" + Years + "' and userid='" + userid + "'");
            double qtt1 = jds(qtravelUsed1) + jds(qcommUsed1);
            DataRow qdr1 = dt.NewRow();
            qdr1["Type"] = "全年个人";
            qdr1["Used"] = qtt1.ToString("F2");
            qdr1["Budget"] = jds(qBudget1).ToString("F2");
            qdr1["Station"] = thestation;
            dt.Rows.Add(qdr1);

            string qtravelUsed2 = dbs.ExeSqlScalar("select isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0) from ETraveleDetail as b inner join ETravel as a on a.id=b.no where  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "' AND year(tdate)='" + Years + "' and a.Department='" + dpt + "'").ToString();
            string qcommUsed2 = dbs.ExeSqlScalar("select sum(b.CostCenterAmount) from EeommonDetail as b inner join Ecommon as a on a.id=b.no where  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "'  and year(tdate)='" + Years + "' and a.Department='" + dpt + "'").ToString();
            string qBudget2 = dbs.ExeSqlScalar("SELECT sum(b.Amount) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.BaccountCode='" + accountcode + "' and a.Years='" + Years + "' and Deptment='" + dpt + "'");
            double qtt2 = jds(qtravelUsed2) + jds(qcommUsed2);
            DataRow qdr2 = dt.NewRow();
            qdr2["Type"] = "全年部门";
            qdr2["Used"] = qtt2.ToString("F2");
            qdr2["Budget"] = jds(qBudget2).ToString("F2");
            qdr2["Station"] = thestation;
            dt.Rows.Add(qdr2);
            string qtravelUsed3 = dbs.ExeSqlScalar("select isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0) from ETraveleDetail as b inner join ETravel as a on a.id=b.no where  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "' AND year(tdate)='" + Years + "'").ToString();
            string qcommUsed3 = dbs.ExeSqlScalar("select sum(b.CostCenterAmount) from EeommonDetail as b inner join Ecommon as a on a.id=b.no where  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "'  and year(tdate)='" + Years + "' ").ToString();
            string qBudget3 = dbs.ExeSqlScalar("SELECT sum(b.Amount) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.BaccountCode='" + accountcode + "' and a.Years='" + Years + "' ");
            double qtt3 = jds(qtravelUsed3) + jds(qcommUsed3);
            DataRow qdr3 = dt.NewRow();
            qdr3["Type"] = "全年站点";
            qdr3["Used"] = qtt3.ToString("F2");
            qdr3["Budget"] = jds(qBudget3).ToString("F2");
            qdr3["Station"] = thestation;
            dt.Rows.Add(qdr3);

            //}
            //else
            //{
            //    string travelUsed1 = dbs.ExeSqlScalar("select isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0) from ETraveleDetail as b inner join ETravel as a on a.id=b.no where  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "' AND MONTH(tdate)='" + month + "' and year(tdate)='" + Years + "' and PersonID='" + userid + "'").ToString();
            //    string commUsed1 = dbs.ExeSqlScalar("select sum(b.CostCenterAmount) from EeommonDetail as b inner join Ecommon as a on a.id=b.no where  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "'  AND MONTH(tdate)='" + month + "' and year(tdate)='" + Years + "' and PersonID='" + userid + "'").ToString();
            //    string Budget1 = dbs.ExeSqlScalar("SELECT sum(b.Amount) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.SACCOUNDCODE='" + accountcode + "' AND b.MONTHS='" + month + "' and a.Years='" + Years + "' and userid='" + userid + "'");
            //    double tt1 = jds(travelUsed1) + jds(commUsed1);
            //    DataRow dr1 = dt.NewRow();
            //    dr1["Type"] = "个人";
            //    dr1["Used"] = tt1.ToString("F2");
            //    dr1["Budget"] = jds(Budget1).ToString("F2");
            //    dr1["Station"] = thestation;
            //    dt.Rows.Add(dr1);

            //    string qtravelUsed1 = dbs.ExeSqlScalar("select isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0) from ETraveleDetail as b inner join ETravel as a on a.id=b.no where  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "' AND year(tdate)='" + Years + "' and PersonID='" + userid + "'").ToString();
            //    string qcommUsed1 = dbs.ExeSqlScalar("select sum(b.CostCenterAmount) from EeommonDetail as b inner join Ecommon as a on a.id=b.no where  a.type='0' and a.status<>'3' and  station2='" + thestation + "' AND ACCOUNTCODE='" + accountcode + "'  and year(tdate)='" + Years + "' and PersonID='" + userid + "'").ToString();
            //    string qBudget1 = dbs.ExeSqlScalar("SELECT sum(b.Amount) FROM BudgetDetail as b inner join BudgetMain  as a on a.id=b.fid where b.station='" + thestation + "' AND b.SACCOUNDCODE='" + accountcode + "' and a.Years='" + Years + "' and name='" + userid + "'");
            //    double qtt1 = jds(qtravelUsed1) + jds(qcommUsed1);
            //    DataRow qdr1 = dt.NewRow();
            //    qdr1["Type"] = "全年";
            //    qdr1["Used"] = qtt1.ToString("F2");
            //    qdr1["Budget"] = jds(qBudget1).ToString("F2");
            //    qdr1["Station"] = thestation;
            //    dt.Rows.Add(qdr1);
            //}



        }
        return dt;
    }
    private static double jds(string pa)
    {
        if (pa == "")
        { return 0; }
        else
        { return double.Parse(pa); }
    }
}
