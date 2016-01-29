using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using DBOperatorSql;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Xsl;
using System.Xml;
namespace eReimbursement
{
    public partial class BudgetReport : System.Web.UI.Page
    {
        DBAdapter dbs = new DBAdapter();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] == null)
                {

                    X.AddScript("loginWindow.show();Panel1.disable();");
                    return;

                }
                else if (Comm.JdRole(Request.ServerVariables["URL"].ToString()) == 0)
                {
                    X.AddScript("loginWindow.show();Panel1.disable();");
                    return;
                }
                else
                {
                    if (!IsPostBack)
                    {
                      
                    }
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "$('div.gn_person ul.q-menubox li:eq(0) a').text('" + Session["UserName"].ToString() + "');", true);
                    X.AddScript("loginWindow.hide();Panel1.enable();");
                }

            }
            if (!X.IsAjaxRequest)
            {
                
                dbsa();
                StationDB();
                GetDepartment();
                DataSet ds = dbs.GetSqlDataSet("select distinct Baccountcode,Saccountname from AccoundCode where baccountcode=saccountcode");
                Store2.DataSource = ds;
                Store2.DataBind();
                ComboBox1.SelectedIndex = 300;
            //    this.Store1.DataSource = new object[]
            //{
            //    new object[] { "62010500","Communication", "100/200","200/200", "300/500", "400/500", "500/500", "600/500", "100/500", "100/500", "100/500", "100/500", "100/500", "1000/500", "1200" },
            //    new object[] { "62010900","Entertainment", "200","200","200","200","200","200","200","200","200","200","200","200", "2400" },
            //    new object[] { "62012000","Traveling", "150","150","150","150","150","150","150","150","150","150","150","150", "1800" }
            //};

            //    this.Store1.DataBind();
              
            }

        }
        protected void ToExcel(object sender, EventArgs e)
        {
            string json = GridData.Value.ToString();
            StoreSubmitDataEventArgs eSubmit = new StoreSubmitDataEventArgs(json, null);
            XmlNode xml = eSubmit.Xml;

            this.Response.Clear();
            this.Response.ContentType = "application/vnd.ms-excel";
            this.Response.AddHeader("Content-Disposition", "attachment; filename=submittedData.xls");
            XslCompiledTransform xtExcel = new XslCompiledTransform();
            xtExcel.Load(Server.MapPath("Excel.xsl"));
            xtExcel.Transform(xml, null, this.Response.OutputStream);
            this.Response.End();
        }
        protected void EXchange(object sender, DirectEventArgs e)
        {
            if (DLType.Value.ToString() == "Personal")
            {
                DLDepartment.Hide();
                DLUser.Show();
            }
            else if (DLType.Value.ToString() == "Department")
            {
                DLDepartment.Show();
                DLUser.Hide();
            }
            else {
                DLDepartment.Hide();
                DLUser.Hide();
            }
        
        }
        protected void GetUser(object sender, DirectEventArgs e)
        {
            if (DLStation.Value == null)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Message",
                    Message = "请先选择站点",
                    Buttons = MessageBox.Button.OK,
                    Width = 320,
                    Icon = MessageBox.Icon.INFO
                });
            }
            else
            {
                if (X.GetValue("DLUser").Length > 2)
                {

                    DataSet ds = DIMERCO.SDK.Utilities.LSDK.getUserDataBYStationCodeNUser(DLStation.Value.ToString(), "", X.GetValue("DLUser"));
                    //SqlConnection cn = new SqlConnection("Data Source=10.130.40.20;Initial Catalog=ReSM;User ID=sa;Password=dim1rc0@");
                    //string sql = "select distinct userid,fullname from  SMUser inner join SMstation on SMUser.stationid=SMstation.stationid where StationCode='" + Request.QueryString["S"].ToString() + "' and SMUser.fullname like '%" + X.GetValue("DLUser") + "%'";
                    //SqlDataAdapter da = new SqlDataAdapter(sql, cn);
                    //DataSet ds = new DataSet();
                    //da.Fill(ds);
                    DataTable tb = ds.Tables[0];
                    SUser.DataSource = tb;
                    SUser.DataBind();
                    ds.Dispose();

                }
            }

        }
        protected void DatePicker1_Select(object sender, DirectEventArgs e)
        {


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
                    Session["CostCenter"] = dt1.Rows[0]["CostCenter"].ToString();
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
        public void StationDB()
        {
         
            string Nct = dbs.ExeSqlScalar("select top 1 admin from StationRole where UserID='" + Session["UserID"].ToString() + "'").ToString();
            if (Nct == "1")
            {
                DataSet ds = DIMERCO.SDK.Utilities.LSDK.getCostCenterBYStationCode("", 8000);
                //SqlConnection cn = new SqlConnection("Data Source=10.130.40.20;Initial Catalog=ReSM;User ID=sa;Password=dim1rc0@");
                //SqlDataAdapter da = new SqlDataAdapter("select distinct StationCode from SMStation order by StationCode", cn);
                //DataSet ds = new DataSet();
                //da.Fill(ds);
                DataTable tb = ds.Tables[0];
                DataTable drnw = new DataTable();
                tb.DefaultView.Sort = "StationCode ASC";
                drnw = tb.DefaultView.ToTable();

                SStation.DataSource = drnw;
                SStation.DataBind();
                ds.Dispose();
                DLStation.SelectedIndex = 300;

            }
            else
            {
                string Tstations = dbs.ExeSqlScalar("select Stations from StationRole where UserID='" + Session["UserID"].ToString() + "'");
                DataTable Dt = new DataTable();
                Dt.Columns.Add("StationCode");
                string[] Sp = Tstations.Split(',');
                for (int i = 0; i < Sp.Length; i++)
                {
                    if (Sp[i].ToString().Trim() != "")
                    {
                        DataRow newDr = Dt.NewRow();
                        newDr["StationCode"] = Sp[i].ToString();
                        Dt.Rows.Add(newDr);
                    }
                }
                DataTable drnw = new DataTable();
                Dt.DefaultView.Sort = "StationCode ASC";
                drnw = Dt.DefaultView.ToTable();

                SStation.DataSource = drnw;
                SStation.DataBind();
                DLStation.SelectedIndex = 300;
            }
        }
        protected void button1_Search(object sender, DirectEventArgs e)
        {

            dbsa();
        }
        public void GetDepartment()
        {
            DataSet ds = DIMERCO.SDK.Utilities.LSDK.getCRPDepartment();
            //SqlConnection cn = new SqlConnection("Data Source=10.130.40.20;Initial Catalog=ReSM;User ID=sa;Password=dim1rc0@");
            //SqlDataAdapter da = new SqlDataAdapter("select distinct departmentname from  SMDepartment", cn);
            //DataSet ds = new DataSet();
            //da.Fill(ds);
            DataTable tb = ds.Tables[0];
            SDepartment.DataSource = tb;
            SDepartment.DataBind();
            DLStation.SelectedIndex = 300;
            ds.Dispose();
        }
    
        private void dbsa()
        {
            if (DLStation.Value != null)
            {

                if (DLType.Value.ToString() == "Personal")
                {
                    if (DLStation.SelectedItem.Text.ToString() == "")
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Message",
                            Message = "请先选择站点",
                            Buttons = MessageBox.Button.OK,
                            Width = 320,
                            Icon = MessageBox.Icon.INFO
                        });
                    }
                    else{
                        string namme = DLUser.SelectedItem.Text;
                        string sss = "";
                        if (namme != "")
                        {
                            sss = "and name='" + namme + "'";
                        }
                        string sss1 = "";
                        if (namme != "")
                        {
                            sss1 = "and person='" + namme + "'";
                        }
                        string xxx = "";
                        if (ComboBox1.SelectedItem.Value != null)
                        {
                            string code = ComboBox1.SelectedItem.Value.ToString();
                            if (code != "")
                            {
                                xxx = "and accountcode='" + code + "'";
                            }
                        }
                        //string xxx1 = "";
                        //if (code != "")
                        //{
                        //    xxx1 = "and accountcode='" + code + "'";
                        //}

                        string sql = "SELECT isnull(nn.Station,mm.station) as Station,isnull(nn.department,mm.Deptment) as Deptment,isnull(nn.NAME,mm.NAME) as NAME,isnull(nn.SACCOUNDCODE,mm.SACCOUNDCODE) as SACCOUNDCODE ,SACCOUNDNAME=(select SAccountName from AccoundCode where SAccountCode=isnull(nn.SACCOUNDCODE,mm.SACCOUNDCODE)),isnull(nn.amount,0) as used,isnull(mm.amount,0) as budget,last=isnull(mm.amount,0)-isnull(nn.amount,0) FROM " +

                " (select isnull(aa.person,bb.person) as name,isnull(aa.accountcode,bb.accountcode) as SACCOUNDCODE,isnull(aa.station,bb.station) as station,isnull(aa.department,bb.department) as department,(isnull(aa.amount,0)+isnull(bb.amount,0)) as   amount from    " +
                " (select person,accountcode,station,[Department], (isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0)) as amount from ETraveleDetail as b inner join ETravel as a on a.id=b.no where year(tdate)='" + DLYears.Value.ToString() + "' and  month(tdate)<='" + DLMonths.Value.ToString() + "' AND  STATION='" + DLStation.Value.ToString() + "' and  a.type='0' and a.status='2' "+sss1+" "+xxx+" group by  person,accountcode,station,[Department]) as aa " +

                " full outer join  " +

                " (select person,accountcode,station,[Department], isnull(sum(b.CostCenterAmount),0) as amount from EeommonDetail as b inner join Ecommon as a on a.id=b.no where year(tdate)='" + DLYears.Value.ToString() + "' and  month(tdate)<='" + DLMonths.Value.ToString() + "' AND  STATION='" + DLStation.Value.ToString() + "' and  a.type='0'  and a.status='2'  " + sss1 + " " + xxx + "  group by  person,accountcode,station,[Department] ) as  bb " +

                " on aa.person=bb.person and aa.accountcode=bb.accountcode and aa.station=bb.station and aa.department=bb.department ) AS NN"+
                          " full outer join" +
               " (select aaa.NAME,SUM(AMOUNT) AS AMOUNT,bbb.AccountCode as SACCOUNDCODE,bbb.AccountDes as SACCOUNDNAME,aaa.Station,aaa.Deptment from BudgetDetail AS aaa inner join BudgetMain  as bbb on aaa.fid=bbb.id  WHERE bbb.Years='" + DLYears.Value.ToString() + "' and  aaa.Months<='" + DLMonths.Value.ToString() + "' AND  aaa.STATION='" + DLStation.Value.ToString() + "' " + sss + " " + xxx + "  GROUP BY NAME,AccountCode,AccountDes,aaa.Station,Deptment ) AS MM " +

                      "  ON  MM.NAME=NN.name AND MM.SACCOUNDCODE=NN.SACCOUNDCODE AND MM.Station=NN.Station AND MM.Deptment=NN.department " +

                " ORDER BY NAME,SACCOUNDCODE";
                        DataSet ds = dbs.GetSqlDataSet(sql);

                        JsonReader reader = new JsonReader();
                        reader.Fields.Add("Station");
                        reader.Fields.Add("Deptment");
                        reader.Fields.Add("NAME");
                        reader.Fields.Add("SACCOUNDCODE");
                        reader.Fields.Add("SACCOUNDNAME");
                        reader.Fields.Add("used");
                        reader.Fields.Add("budget");
                        reader.Fields.Add("last");
                        Store1.Reader.Add(reader);
                        Store1.DataSource = ds.Tables[0];
                        Store1.DataBind();
                        GridPanel1.ColumnModel.Columns.Add(new Column()
                        {
                            Header = "Unit",
                            DataIndex = "Station"
                        });
                        GridPanel1.ColumnModel.Columns.Add(new Column()
                        {
                            Header = "Department",
                            DataIndex = "Deptment"
                        });

                        GridPanel1.ColumnModel.Columns.Add(new Column()
                        {
                            Header = "User",
                            DataIndex = "NAME"
                        });
                        GridPanel1.ColumnModel.Columns.Add(new Column()
                        {
                            Header = "Account Code",
                            DataIndex = "SACCOUNDCODE"
                        });
                        GridPanel1.ColumnModel.Columns.Add(new Column()
                        {
                            Header = "Account Name",
                            DataIndex = "SACCOUNDNAME"
                        });
                        GridPanel1.ColumnModel.Columns.Add(new Column()
                        {
                            Header = "Used",
                            DataIndex = "used"
                        });
                        GridPanel1.ColumnModel.Columns.Add(new Column()
                        {
                            Header = "Budget",
                            DataIndex = "budget"
                        });
                        GridPanel1.ColumnModel.Columns.Add(new Column()
                        {
                            Header = "Balance",
                            DataIndex = "last"
                        });
                   }
                }
                else if (DLType.Value.ToString() == "Department")
                {
                    if (DLStation.SelectedItem.Text.ToString() == "")
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Message",
                            Message = "请先选择站点",
                            Buttons = MessageBox.Button.OK,
                            Width = 320,
                            Icon = MessageBox.Icon.INFO
                        });
                    }
                    else
                    {
                        string namme = DLDepartment.SelectedItem.Text;
                        string sss = "";
                      
                        if (namme != "")
                        {
                            sss = "and Deptment='" + namme + "'";
                        }
                        string sss1 = "";

                        if (namme != "")
                        {
                            sss1 = "and department='" + namme + "'";
                        }
                        string sql = "";
                        if (DLDepartment.SelectedItem.Text == "")
                        {
                            string xxxd = "";
                            if (ComboBox1.SelectedItem.Value != null)
                            {
                                string coded = ComboBox1.SelectedItem.Value.ToString();
                                if (coded != "")
                                {
                                    xxxd = "and accountcode='" + coded + "'";
                                }
                            }
                            sql = "SELECT isnull(nn.Station,mm.station) as Station,isnull(nn.department,mm.Deptment) as Deptment,isnull(nn.SACCOUNDCODE,mm.SACCOUNDCODE) as SACCOUNDCODE ,SACCOUNDNAME=(select SAccountName from AccoundCode where SAccountCode=isnull(nn.SACCOUNDCODE,mm.SACCOUNDCODE)),isnull(nn.amount,0) as used,isnull(mm.amount,0) as budget,last=isnull(mm.amount,0)-isnull(nn.amount,0) FROM " +

                    " (select isnull(aa.accountcode,bb.accountcode) as SACCOUNDCODE,isnull(aa.station,bb.station) as station,isnull(aa.department,bb.department) as department,(isnull(aa.amount,0)+isnull(bb.amount,0)) as   amount from    " +
                    " (select accountcode,station,[Department], (isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0)) as amount from ETraveleDetail as b inner join ETravel as a on a.id=b.no where year(tdate)='" + DLYears.Value.ToString() + "' and  month(tdate)<='" + DLMonths.Value.ToString() + "' AND  STATION='" + DLStation.Value.ToString() + "' and  a.type='0' and a.status='2' " + sss1 + " " + xxxd + " group by  accountcode,station,[Department]) as aa " +

                    " full outer join  " +

                    " (select accountcode,station,[Department], isnull(sum(b.CostCenterAmount),0) as amount from EeommonDetail as b inner join Ecommon as a on a.id=b.no where year(tdate)='" + DLYears.Value.ToString() + "' and  month(tdate)<='" + DLMonths.Value.ToString() + "' AND  STATION='" + DLStation.Value.ToString() + "' and  a.type='0'  and a.status='2'  " + sss1 + " " + xxxd + " group by  accountcode,station,[Department] ) as  bb " +

                    " on aa.accountcode=bb.accountcode and aa.station=bb.station and aa.department=bb.department ) AS NN" +
  " full outer join" +
                                " (select SUM(AMOUNT) AS AMOUNT,bbb.AccountCode as SACCOUNDCODE,bbb.AccountDes as SACCOUNDNAME,aaa.Station,aaa.Deptment from BudgetDetail AS aaa inner join BudgetMain  as bbb on aaa.fid=bbb.id  WHERE bbb.Years='" + DLYears.Value.ToString() + "' and  aaa.Months<='" + DLMonths.Value.ToString() + "' AND  aaa.STATION='" + DLStation.Value.ToString() + "' " + sss + " " + xxxd + "  GROUP BY AccountCode,AccountDes,aaa.Station,Deptment ) AS MM " +

                                "  ON  MM.SACCOUNDCODE=NN.SACCOUNDCODE AND MM.Station=NN.Station AND MM.Deptment=NN.department " +

                    " ORDER BY Deptment,SACCOUNDCODE";
                             DataSet ds = dbs.GetSqlDataSet(sql);

                             JsonReader reader = new JsonReader();
                             reader.Fields.Add("Station");
                             reader.Fields.Add("Deptment");
                             reader.Fields.Add("SACCOUNDCODE");
                             reader.Fields.Add("SACCOUNDNAME");
                             reader.Fields.Add("used");
                             reader.Fields.Add("budget");
                             reader.Fields.Add("last");
                             Store1.Reader.Add(reader);
                             Store1.DataSource = ds.Tables[0];
                             Store1.DataBind();
                             GridPanel1.ColumnModel.Columns.Add(new Column()
                             {
                                 Header = "Unit",
                                 DataIndex = "Station"
                             });
                             GridPanel1.ColumnModel.Columns.Add(new Column()
                             {
                                 Header = "Department",
                                 DataIndex = "Deptment"
                             });
                           
                             GridPanel1.ColumnModel.Columns.Add(new Column()
                             {
                                 Header = "Account Code",
                                 DataIndex = "SACCOUNDCODE"
                             });
                             GridPanel1.ColumnModel.Columns.Add(new Column()
                             {
                                 Header = "Account Name",
                                 DataIndex = "SACCOUNDNAME"
                             });
                             GridPanel1.ColumnModel.Columns.Add(new Column()
                             {
                                 Header = "Used",
                                 DataIndex = "used"
                             });
                             GridPanel1.ColumnModel.Columns.Add(new Column()
                             {
                                 Header = "Budget",
                                 DataIndex = "budget"
                             });
                             GridPanel1.ColumnModel.Columns.Add(new Column()
                             {
                                 Header = "Balance",
                                 DataIndex = "last"
                             });
                        }
                        else
                        {
                            string xxxd = "";
                            if (ComboBox1.SelectedItem.Value != null)
                            {
                                string coded = ComboBox1.SelectedItem.Value.ToString();
                                if (coded != "")
                                {
                                    xxxd = "and accountcode='" + coded + "'";
                                }
                            }
                            sql = "SELECT isnull(nn.Station,mm.station) as Station,isnull(nn.department,mm.Deptment) as Deptment,isnull(nn.NAME,mm.NAME) as NAME,isnull(nn.SACCOUNDCODE,mm.SACCOUNDCODE) as SACCOUNDCODE ,SACCOUNDNAME=(select SAccountName from AccoundCode where SAccountCode=isnull(nn.SACCOUNDCODE,mm.SACCOUNDCODE)),isnull(nn.amount,0) as used,isnull(mm.amount,0) as budget,last=isnull(mm.amount,0)-isnull(nn.amount,0) FROM " +

                                " (select isnull(aa.person,bb.person) as name,isnull(aa.accountcode,bb.accountcode) as SACCOUNDCODE,isnull(aa.station,bb.station) as station,isnull(aa.department,bb.department) as department,(isnull(aa.amount,0)+isnull(bb.amount,0)) as   amount from    " +
                                " (select person,accountcode,station,[Department], (isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0)) as amount from ETraveleDetail as b inner join ETravel as a on a.id=b.no where year(tdate)='" + DLYears.Value.ToString() + "' and  month(tdate)<='" + DLMonths.Value.ToString() + "' AND  STATION='" + DLStation.Value.ToString() + "' and  a.type='0' and a.status='2' " + sss1 + " " + xxxd + " group by  person,accountcode,station,[Department]) as aa " +

                                " full outer join  " +

                                " (select person,accountcode,station,[Department], isnull(sum(b.CostCenterAmount),0) as amount from EeommonDetail as b inner join Ecommon as a on a.id=b.no where year(tdate)='" + DLYears.Value.ToString() + "' and  month(tdate)<='" + DLMonths.Value.ToString() + "' AND  STATION='" + DLStation.Value.ToString() + "' and  a.type='0'  and a.status='2'  " + sss1 + " " + xxxd + "  group by  person,accountcode,station,[Department] ) as  bb " +

                                " on aa.person=bb.person and aa.accountcode=bb.accountcode and aa.station=bb.station and aa.department=bb.department ) AS NN" +
                            " full outer join" +
                                " (select aaa.NAME,SUM(AMOUNT) AS AMOUNT,bbb.AccountCode as SACCOUNDCODE,bbb.AccountDes as SACCOUNDNAME,aaa.Station,aaa.Deptment from BudgetDetail AS aaa inner join BudgetMain  as bbb on aaa.fid=bbb.id  WHERE bbb.Years='" + DLYears.Value.ToString() + "' and  aaa.Months<='" + DLMonths.Value.ToString() + "' AND  aaa.STATION='" + DLStation.Value.ToString() + "' " + sss + " " + xxxd + "  GROUP BY NAME,AccountCode,AccountDes,aaa.Station,Deptment ) AS MM " +

                                "  ON  MM.NAME=NN.name AND MM.SACCOUNDCODE=NN.SACCOUNDCODE AND MM.Station=NN.Station AND MM.Deptment=NN.department " +

                                " ORDER BY Deptment,SACCOUNDCODE,NAME";
                        
                      
                        DataSet ds = dbs.GetSqlDataSet(sql);

                        JsonReader reader = new JsonReader();
                        reader.Fields.Add("Station");
                        reader.Fields.Add("Deptment");
                        reader.Fields.Add("NAME");
                        reader.Fields.Add("SACCOUNDCODE");
                        reader.Fields.Add("SACCOUNDNAME");
                        reader.Fields.Add("used");
                        reader.Fields.Add("budget");
                        reader.Fields.Add("last");
                        Store1.Reader.Add(reader);
                        Store1.DataSource = ds.Tables[0];
                        Store1.DataBind();
                        GridPanel1.ColumnModel.Columns.Add(new Column()
                        {
                            Header = "Unit",
                            DataIndex = "Station"
                        });
                        GridPanel1.ColumnModel.Columns.Add(new Column()
                        {
                            Header = "Department",
                            DataIndex = "Deptment"
                        });

                        GridPanel1.ColumnModel.Columns.Add(new Column()
                        {
                            Header = "NAME",
                            DataIndex = "NAME"
                        });
                        GridPanel1.ColumnModel.Columns.Add(new Column()
                        {
                            Header = "Account Code",
                            DataIndex = "SACCOUNDCODE"
                        });
                        GridPanel1.ColumnModel.Columns.Add(new Column()
                        {
                            Header = "Account Name",
                            DataIndex = "SACCOUNDNAME"
                        });
                        GridPanel1.ColumnModel.Columns.Add(new Column()
                        {
                            Header = "Used",
                            DataIndex = "used"
                        });
                        GridPanel1.ColumnModel.Columns.Add(new Column()
                        {
                            Header = "Budget",
                            DataIndex = "budget"
                        });
                        GridPanel1.ColumnModel.Columns.Add(new Column()
                        {
                            Header = "Balance",
                            DataIndex = "last"
                        });
                        }
                    }
                }
                else
                {
                    int m = int.Parse(DLMonths.SelectedItem.Text);
                    string xxff = "";
                    for (int x = 0; x < m; x++)
                    { 
                        int xxgg=x+1;
                        if (x == m - 1)
                        {
                            xxff = xxff + "m" + xxgg;
                        }
                        else
                        { xxff = xxff + "m" + xxgg+"+"; }
                    }
                    string xxxd = "";
                    string xxxdd = "";
                    if (ComboBox1.SelectedItem.Value != null)
                    {
                        string coded = ComboBox1.SelectedItem.Value.ToString();
                        if (coded != "")
                        {
                            xxxd = "and accountcode='" + coded + "'";
                            xxxdd = "and BaacountCode='" + coded + "'";
                        }
                    }
                    string sql = "SELECT isnull(nn.Station,mm.station) as Station,isnull(nn.SACCOUNDCODE,mm.SACCOUNDCODE) as SACCOUNDCODE ,SACCOUNDNAME=(select SAccountName from AccoundCode where SAccountCode=isnull(nn.SACCOUNDCODE,mm.SACCOUNDCODE)),isnull(nn.amount,0) as used,isnull(mm.amount,0) as budget,last=isnull(mm.amount,0)-isnull(nn.amount,0) FROM " +

                  " (select isnull(aa.accountcode,bb.accountcode) as SACCOUNDCODE,isnull(aa.station,bb.station) as station,(isnull(aa.amount,0)+isnull(bb.amount,0)) as   amount from    " +
                  " (select accountcode,station, (isnull(sum(b.CenterAmountP),0)+isnull(sum(b.CenterAmountC),0)) as amount from ETraveleDetail as b inner join ETravel as a on a.id=b.no where year(tdate)='" + DLYears.Value.ToString() + "' and  month(tdate)<='" + DLMonths.Value.ToString() + "' AND  STATION='" + DLStation.Value.ToString() + "' and  a.type='0' and a.status='2'" + xxxd + "  group by  accountcode,station) as aa " +

                  " full outer join  " +

                  " (select accountcode,station, isnull(sum(b.CostCenterAmount),0) as amount from EeommonDetail as b inner join Ecommon as a on a.id=b.no where year(tdate)='" + DLYears.Value.ToString() + "' and  month(tdate)<='" + DLMonths.Value.ToString() + "' AND  STATION='" + DLStation.Value.ToString() + "' and  a.type='0'  and a.status='2'  " + xxxd + " group by  accountcode,station ) as  bb " +

                  " on  aa.accountcode=bb.accountcode and aa.station=bb.station ) AS NN" +
                              " full outer join  " +
                 " (select SUM("+xxff+") AS AMOUNT,BaacountCode as SACCOUNDCODE,'' as SACCOUNDNAME,Station from BudgetMain   WHERE Years='" + DLYears.Value.ToString() + "' AND  STATION='" + DLStation.Value.ToString() + "'"+xxxdd+"  GROUP BY BaacountCode,Station) AS MM " +

                        "  ON  MM.SACCOUNDCODE=NN.SACCOUNDCODE AND MM.Station=NN.Station " +

                  " ORDER BY SACCOUNDCODE";
                    DataSet ds = dbs.GetSqlDataSet(sql);

                    JsonReader reader = new JsonReader();
                    reader.Fields.Add("Station");
                    reader.Fields.Add("SACCOUNDCODE");
                    reader.Fields.Add("SACCOUNDNAME");
                    reader.Fields.Add("used");
                    reader.Fields.Add("budget");
                    reader.Fields.Add("last");
                    Store1.Reader.Add(reader);
                    Store1.DataSource = ds.Tables[0];
                    Store1.DataBind();
                    GridPanel1.ColumnModel.Columns.Add(new Column()
                    {
                        Header = "Unit",
                        DataIndex = "Station"
                    });


                    GridPanel1.ColumnModel.Columns.Add(new Column()
                    {
                        Header = "Account Code",
                        DataIndex = "SACCOUNDCODE"
                    });
                    GridPanel1.ColumnModel.Columns.Add(new Column()
                    {
                        Header = "Account Name",
                        DataIndex = "SACCOUNDNAME"
                    });
                    GridPanel1.ColumnModel.Columns.Add(new Column()
                    {
                        Header = "Used",
                        DataIndex = "used"
                    });
                    GridPanel1.ColumnModel.Columns.Add(new Column()
                    {
                        Header = "Budget",
                        DataIndex = "budget"
                    });
                    GridPanel1.ColumnModel.Columns.Add(new Column()
                    {
                        Header = "Balance",
                        DataIndex = "last"
                    });
                }
                GridPanel1.DataBind();

                GridPanel1.Reconfigure();
            }
            else
            {
                string sql = "SELECT mm.Station,mm.Deptment,mm.MONTHS,mm.NAME,mm.SACCOUNDCODE,mm.SACCOUNDNAME,isnull(nn.amount,0) as used,isnull(mm.amount,0) as budget,last=isnull(mm.amount,0)-isnull(nn.amount,0) FROM (select aaa.MONTHS,aaa.NAME,SUM(AMOUNT) AS AMOUNT,aaa.SACCOUNDCODE,aaa.SACCOUNDNAME,aaa.Station,aaa.Deptment from BudgetDetail AS aaa inner join BudgetMain  as bbb on aaa.fid=bbb.id  WHERE 1=2 GROUP BY aaa.MONTHS,NAME,SACCOUNDCODE,SACCOUNDNAME,aaa.Station,Deptment ) AS MM " +
   " left JOIN" +
   " (select aa.months,aa.person,aa.accountcode,aa.station,aa.department,(isnull(aa.amount,0)+isnull(bb.amount,0)) as   amount from  " +
   " (select month(tdate) as months,person,accountcode,station,[Department], (sum(b.pamount)+sum(b.camount)) as amount from ETraveleDetail as b inner join ETravel as a on a.id=b.no where 1=2 and  a.type='0' and a.status<>'3' group by  month(tdate),person,accountcode,station,[Department]) as aa " +

   " full outer join  " +

   " (select month(tdate) as months,person,accountcode,station,[Department], sum(b.Amount) as amount from EeommonDetail as b inner join Ecommon as a on a.id=b.no where 1=2 and  a.type='0'  and a.status<>'3' group by  month(tdate),person,accountcode,station,[Department] ) as  bb " +

   " on aa.months=bb.months and aa.person=bb.person and aa.accountcode=bb.accountcode and aa.station=bb.station and aa.department=bb.department ) AS NN ON MM.MONTHS=NN.MONTHS AND MM.NAME=NN.PERSON AND MM.SACCOUNDCODE=NN.accountcode AND MM.Station=NN.Station AND MM.Deptment=NN.department " +

   " ORDER BY NAME,SACCOUNDCODE,MM.MONTHS";
                DataSet ds = dbs.GetSqlDataSet(sql);

                JsonReader reader = new JsonReader();
                reader.Fields.Add("Station");
                reader.Fields.Add("MONTHS");
                reader.Fields.Add("Deptment");
                reader.Fields.Add("NAME");
                reader.Fields.Add("SACCOUNDCODE");
                reader.Fields.Add("SACCOUNDNAME");
                reader.Fields.Add("used");
                reader.Fields.Add("budget");
                reader.Fields.Add("last");
                Store1.Reader.Add(reader);
                Store1.DataSource = ds.Tables[0];
                Store1.DataBind();
                GridPanel1.ColumnModel.Columns.Add(new Column()
                {
                    Header = "Unit",
                    DataIndex = "Station"
                });
                GridPanel1.ColumnModel.Columns.Add(new Column()
                {
                    Header = "Department",
                    DataIndex = "Deptment"
                });

                GridPanel1.ColumnModel.Columns.Add(new Column()
                {
                    Header = "User",
                    DataIndex = "NAME"
                });
                GridPanel1.ColumnModel.Columns.Add(new Column()
                {
                    Header = "Month",
                    DataIndex = "MONTHS"
                });
                GridPanel1.ColumnModel.Columns.Add(new Column()
                {
                    Header = "Account Code",
                    DataIndex = "SACCOUNDCODE"
                });
                GridPanel1.ColumnModel.Columns.Add(new Column()
                {
                    Header = "Account Name",
                    DataIndex = "SACCOUNDNAME"
                });
                GridPanel1.ColumnModel.Columns.Add(new Column()
                {
                    Header = "Used",
                    DataIndex = "used"
                });
                GridPanel1.ColumnModel.Columns.Add(new Column()
                {
                    Header = "Budget",
                    DataIndex = "budget"
                });
                GridPanel1.ColumnModel.Columns.Add(new Column()
                {
                    Header = "Balance",
                    DataIndex = "last"
                });
                GridPanel1.DataBind();

                GridPanel1.Reconfigure();
            }

        }
        //private void BindData()
        //{

        //    if (Type.Text == "站点")
        //    {
        //        JsonReader reader = new JsonReader();
        //        reader.Fields.Add("Name");
        //        reader.Fields.Add("code");
        //        reader.Fields.Add("des");
        //        reader.Fields.Add("m1");
        //        reader.Fields.Add("m2");
        //        reader.Fields.Add("m3");
        //        reader.Fields.Add("m4");
        //        reader.Fields.Add("m5");
        //        reader.Fields.Add("m6");
        //        reader.Fields.Add("m7");
        //        reader.Fields.Add("m8");
        //        reader.Fields.Add("m9");
        //        reader.Fields.Add("m10");
        //        reader.Fields.Add("m11");
        //        reader.Fields.Add("m12");
        //        reader.Fields.Add("tt");
        //        reader.Fields.Add("tp");
        //        Store1.Reader.Add(reader);
        //        //reader.Fields[1].ServerMapping = "ID";

        //        DataTable dt = new DataTable();
        //        DataRow dr;
        //        dt.Columns.Add(new DataColumn("Name", typeof(string)));
        //        dt.Columns.Add(new DataColumn("code", typeof(string)));
        //        dt.Columns.Add(new DataColumn("des", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m1", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m2", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m3", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m4", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m5", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m6", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m7", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m8", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m9", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m10", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m11", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m12", typeof(string)));
        //        dt.Columns.Add(new DataColumn("tt", typeof(string)));
        //        dt.Columns.Add(new DataColumn("tp", typeof(string)));

        //        dr = dt.NewRow();
        //        dr[0] = DLStation.Text;
        //        dr[1] = "62010500";
        //        dr[2] = "Communication";
        //        dr[3] = "100/200";
        //        dr[4] = "500/200";
        //        dr[5] = "300/500";
        //        dr[6] = "322/600";
        //        dr[7] = "100/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1000/1200";
        //        dr[16] = "-10.55";
        //        dt.Rows.Add(dr);
        //        dr = dt.NewRow();
        //        dr[0] = DLStation.Text;
        //        dr[1] = "62010900";
        //        dr[2] = "Entertainment";
        //        dr[3] = "400/200";
        //        dr[4] = "200/400";
        //        dr[5] = "300/500";
        //        dr[6] = "100/600";
        //        dr[7] = "1/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1000/1300";
        //        dr[16] = "15.55";
        //        dt.Rows.Add(dr);
        //        dr = dt.NewRow();
        //        dr[0] = DLStation.Text;
        //        dr[1] = "62012000";
        //        dr[2] = "Traveling";
        //        dr[3] = "100/200";
        //        dr[4] = "100/200";
        //        dr[5] = "600/500";
        //        dr[6] = "0/600";
        //        dr[7] = "0/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1000/2000";
        //        dr[16] = "50.00";
        //        dt.Rows.Add(dr);
        //        Store1.DataSource = dt;
        //        Store1.DataBind();

        //        GridPanel1.ColumnModel.Columns.Add(new Column()
        //        {
        //            Header = "名称",
        //            DataIndex = "Name"
        //        });
        //        GridPanel1.ColumnModel.Columns.Add(new Column()
        //        {
        //            Header = "代码",
        //            DataIndex = "code"
        //        });
        //        GridPanel1.ColumnModel.Columns.Add(new Column()
        //        {
        //            Header = "描述",
        //            DataIndex = "des"
        //        });

        //        int mm = int.Parse(Months.Text) ;
        //        for (int i = 1; i <= mm; i++)
        //        {
        //            GridPanel1.ColumnModel.Columns.Add(new Column()
        //            {
        //                Header = i.ToString() + "月",
        //                DataIndex = "m" + i.ToString()
        //            });
        //            int mmss = i+2 ;
        //            GridPanel1.ColumnModel.Columns[mmss].Renderer.Fn = "tts";
        //        }
        //        GridPanel1.ColumnModel.Columns.Add(new Column()
        //        {
        //            Header = "合计",
        //            DataIndex = "tt"

        //        });
        //        GridPanel1.ColumnModel.Columns.Add(new Column()
        //        {
        //            Header = "剩余比例",
        //            DataIndex = "tp"

        //        });
        //        int ss = mm + 4;
        //        GridPanel1.ColumnModel.Columns[ss].Renderer.Fn = "tt";
        //        //  GridPanel1.ColumnModel.Columns[0].Width = 180;
        //        //TextField tf = new TextField();
        //        //tf.ID = this.ClientID.ToString() + "abc";
        //        //GridPanel1.ColumnModel.Columns[1].Editor.Add(tf);
        //        GridPanel1.DataBind();

        //        GridPanel1.Reconfigure();
        //    }
        //    else if (Type.Text == "部门")
        //    {
        //        JsonReader reader = new JsonReader();
        //        reader.Fields.Add("Name");
        //        reader.Fields.Add("code");
        //        reader.Fields.Add("des");
        //        reader.Fields.Add("m1");
        //        reader.Fields.Add("m2");
        //        reader.Fields.Add("m3");
        //        reader.Fields.Add("m4");
        //        reader.Fields.Add("m5");
        //        reader.Fields.Add("m6");
        //        reader.Fields.Add("m7");
        //        reader.Fields.Add("m8");
        //        reader.Fields.Add("m9");
        //        reader.Fields.Add("m10");
        //        reader.Fields.Add("m11");
        //        reader.Fields.Add("m12");
        //        reader.Fields.Add("tt");
        //        reader.Fields.Add("tp");
        //        Store1.Reader.Add(reader);
        //        //reader.Fields[1].ServerMapping = "ID";

        //        DataTable dt = new DataTable();
        //        DataRow dr;
        //        dt.Columns.Add(new DataColumn("Name", typeof(string)));
        //        dt.Columns.Add(new DataColumn("code", typeof(string)));
        //        dt.Columns.Add(new DataColumn("des", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m1", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m2", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m3", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m4", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m5", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m6", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m7", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m8", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m9", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m10", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m11", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m12", typeof(string)));
        //        dt.Columns.Add(new DataColumn("tt", typeof(string)));
        //        dt.Columns.Add(new DataColumn("tp", typeof(string)));

        //        dr = dt.NewRow();
        //        dr[0] = "空运部";
        //        dr[1] = "62010500";
        //        dr[2] = "Communication";
        //        dr[3] = "100/200";
        //        dr[4] = "500/200";
        //        dr[5] = "300/500";
        //        dr[6] = "0/600";
        //        dr[7] = "0/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1000/1200";
        //        dr[16] = "-10.55";
        //        dt.Rows.Add(dr);
        //        dr = dt.NewRow();
        //        dr[0] = "空运部";
        //        dr[1] = "62010900";
        //        dr[2] = "Entertainment";
        //        dr[3] = "100/200";
        //        dr[4] = "500/200";
        //        dr[5] = "300/500";
        //        dr[6] = "0/600";
        //        dr[7] = "0/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1200/1400";
        //        dr[16] = "10.55";
        //        dt.Rows.Add(dr);
        //        dr = dt.NewRow();
        //        dr[0] = "空运部";
        //        dr[1] = "62012000";
        //        dr[2] = "Traveling";
        //        dr[3] = "100/200";
        //        dr[4] = "100/200";
        //        dr[5] = "600/500";
        //        dr[6] = "0/600";
        //        dr[7] = "0/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1000/2000";
        //        dr[16] = "50.00";
        //        dt.Rows.Add(dr);
        //                    dr = dt.NewRow();
        //        dr[0] = "海运部";
        //        dr[1] = "62010500";
        //        dr[2] = "Communication";
        //        dr[3] = "100/200";
        //        dr[4] = "500/200";
        //        dr[5] = "300/500";
        //        dr[6] = "0/600";
        //        dr[7] = "0/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1000/1200";
        //        dr[16] = "-10.55";
        //        dt.Rows.Add(dr);
        //        dr = dt.NewRow();
        //        dr[0] = "海运部";
        //        dr[1] = "62010900";
        //        dr[2] = "Entertainment";
        //        dr[3] = "600/200";
        //        dr[4] = "100/200";
        //        dr[5] = "300/500";
        //        dr[6] = "0/600";
        //        dr[7] = "0/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1200/1400";
        //        dr[16] = "10.55";
        //        dt.Rows.Add(dr);
        //        dr = dt.NewRow();
        //        dr[0] = "海运部";
        //        dr[1] = "62012000";
        //        dr[2] = "Traveling";
        //        dr[3] = "130/200";
        //        dr[4] = "100/200";
        //        dr[5] = "400/500";
        //        dr[6] = "0/600";
        //        dr[7] = "0/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1000/2000";
        //        dr[16] = "50.00";
        //        dt.Rows.Add(dr);
        //        Store1.DataSource = dt;
        //        Store1.DataBind();

        //        GridPanel1.ColumnModel.Columns.Add(new Column()
        //        {
        //            Header = "名称",
        //            DataIndex = "Name"
        //        });
        //        GridPanel1.ColumnModel.Columns.Add(new Column()
        //        {
        //            Header = "代码",
        //            DataIndex = "code"
        //        });
        //        GridPanel1.ColumnModel.Columns.Add(new Column()
        //        {
        //            Header = "描述",
        //            DataIndex = "des"
        //        });

        //        int mm = int.Parse(Months.Text);
        //        for (int i = 1; i <= mm; i++)
        //        {
        //            GridPanel1.ColumnModel.Columns.Add(new Column()
        //            {
        //                Header = i.ToString() + "月",
        //                DataIndex = "m" + i.ToString()
        //            });
        //            int mmss = i + 2;
        //            GridPanel1.ColumnModel.Columns[mmss].Renderer.Fn = "tts";
        //        }
        //        GridPanel1.ColumnModel.Columns.Add(new Column()
        //        {
        //            Header = "合计",
        //            DataIndex = "tt"

        //        });
        //        GridPanel1.ColumnModel.Columns.Add(new Column()
        //        {
        //            Header = "剩余比例",
        //            DataIndex = "tp"

        //        });
        //        int ss = mm + 4;
        //        GridPanel1.ColumnModel.Columns[ss].Renderer.Fn = "tt";
        //        //  GridPanel1.ColumnModel.Columns[0].Width = 180;
        //        //TextField tf = new TextField();
        //        //tf.ID = this.ClientID.ToString() + "abc";
        //        //GridPanel1.ColumnModel.Columns[1].Editor.Add(tf);
        //        GridPanel1.DataBind();

        //        GridPanel1.Reconfigure();
            
        //    }
        //    else if (Type.Text == "个人")
        //    {
        //        JsonReader reader = new JsonReader();
        //        reader.Fields.Add("Name");
        //        reader.Fields.Add("code");
        //        reader.Fields.Add("des");
        //        reader.Fields.Add("m1");
        //        reader.Fields.Add("m2");
        //        reader.Fields.Add("m3");
        //        reader.Fields.Add("m4");
        //        reader.Fields.Add("m5");
        //        reader.Fields.Add("m6");
        //        reader.Fields.Add("m7");
        //        reader.Fields.Add("m8");
        //        reader.Fields.Add("m9");
        //        reader.Fields.Add("m10");
        //        reader.Fields.Add("m11");
        //        reader.Fields.Add("m12");
        //        reader.Fields.Add("tt");
        //        reader.Fields.Add("tp");
        //        Store1.Reader.Add(reader);
        //        //reader.Fields[1].ServerMapping = "ID";

        //        DataTable dt = new DataTable();
        //        DataRow dr;
        //        dt.Columns.Add(new DataColumn("Name", typeof(string)));
        //        dt.Columns.Add(new DataColumn("code", typeof(string)));
        //        dt.Columns.Add(new DataColumn("des", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m1", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m2", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m3", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m4", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m5", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m6", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m7", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m8", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m9", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m10", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m11", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m12", typeof(string)));
        //        dt.Columns.Add(new DataColumn("tt", typeof(string)));
        //        dt.Columns.Add(new DataColumn("tp", typeof(string)));

        //        dr = dt.NewRow();
        //        dr[0] = "Hughson Huang";
        //        dr[1] = "62010500";
        //        dr[2] = "Communication";
        //        dr[3] = "100/200";
        //        dr[4] = "500/200";
        //        dr[5] = "300/500";
        //        dr[6] = "0/600";
        //        dr[7] = "0/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1000/1200";
        //        dr[16] = "-10.55";
        //        dt.Rows.Add(dr);
        //        dr = dt.NewRow();
        //        dr[0] = "Hughson Huang";
        //        dr[1] = "62010900";
        //        dr[2] = "Entertainment";
        //        dr[3] = "300/200";
        //        dr[4] = "500/200";
        //        dr[5] = "300/500";
        //        dr[6] = "0/600";
        //        dr[7] = "0/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1200/1400";
        //        dr[16] = "10.55";
        //        dt.Rows.Add(dr);
        //        dr = dt.NewRow();
        //        dr[0] = "Hughson Huang";
        //        dr[1] = "62012000";
        //        dr[2] = "Traveling";
        //        dr[3] = "100/200";
        //        dr[4] = "100/200";
        //        dr[5] = "300/500";
        //        dr[6] = "0/600";
        //        dr[7] = "0/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1000/2000";
        //        dr[16] = "50.00";
        //        dt.Rows.Add(dr);
        //        dr = dt.NewRow();
        //        dr[0] = "Andy Kang";
        //        dr[1] = "62010500";
        //        dr[2] = "Communication";
        //        dr[3] = "100/200";
        //        dr[4] = "500/200";
        //        dr[5] = "300/500";
        //        dr[6] = "0/600";
        //        dr[7] = "0/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1000/1200";
        //        dr[16] = "-10.55";
        //        dt.Rows.Add(dr);
        //        dr = dt.NewRow();
        //        dr[0] = "Andy Kang";
        //        dr[1] = "62010900";
        //        dr[2] = "Entertainment";
        //        dr[3] = "100/200";
        //        dr[4] = "500/200";
        //        dr[5] = "300/500";
        //        dr[6] = "0/600";
        //        dr[7] = "0/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1200/1400";
        //        dr[16] = "10.55";
        //        dt.Rows.Add(dr);
        //        dr = dt.NewRow();
        //        dr[0] = "Andy Kang";
        //        dr[1] = "62012000";
        //        dr[2] = "Traveling";
        //        dr[3] = "100/200";
        //        dr[4] = "100/200";
        //        dr[5] = "600/500";
        //        dr[6] = "0/600";
        //        dr[7] = "0/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1000/2000";
        //        dr[16] = "50.00";
        //        dt.Rows.Add(dr);
        //        dr = dt.NewRow();
        //        dr[0] = "Sunhui Chen";
        //        dr[1] = "62010500";
        //        dr[2] = "Communication";
        //        dr[3] = "100/200";
        //        dr[4] = "500/200";
        //        dr[5] = "300/500";
        //        dr[6] = "0/600";
        //        dr[7] = "0/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1000/1200";
        //        dr[16] = "-10.55";
        //        dt.Rows.Add(dr);
        //        dr = dt.NewRow();
        //        dr[0] = "Sunhui Chen";
        //        dr[1] = "62010900";
        //        dr[2] = "Entertainment";
        //        dr[3] = "140/200";
        //        dr[4] = "500/440";
        //        dr[5] = "300/500";
        //        dr[6] = "333/600";
        //        dr[7] = "555/800";
        //        dr[8] = "777/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1200/1400";
        //        dr[16] = "10.55";
        //        dt.Rows.Add(dr);
        //        dr = dt.NewRow();
        //        dr[0] = "Sunhui Chen";
        //        dr[1] = "62012000";
        //        dr[2] = "Traveling";
        //        dr[3] = "100/200";
        //        dr[4] = "200/200";
        //        dr[5] = "300/500";
        //        dr[6] = "0/600";
        //        dr[7] = "0/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1000/2000";
        //        dr[16] = "30.00";
        //        dt.Rows.Add(dr);
        //        Store1.DataSource = dt;
        //        Store1.DataBind();

        //        GridPanel1.ColumnModel.Columns.Add(new Column()
        //        {
        //            Header = "名称",
        //            DataIndex = "Name"
        //        });
        //        GridPanel1.ColumnModel.Columns.Add(new Column()
        //        {
        //            Header = "代码",
        //            DataIndex = "code"
        //        });
              
        //        GridPanel1.ColumnModel.Columns.Add(new Column()
        //        {
        //            Header = "描述",
        //            DataIndex = "des"
        //        });

        //        int mm = int.Parse(Months.Text);
        //        for (int i = 1; i <= mm; i++)
        //        {
        //            GridPanel1.ColumnModel.Columns.Add(new Column()
        //            {
        //                Header = i.ToString() + "月",
        //                DataIndex = "m" + i.ToString()
        //            });
        //            int mmss = i + 2;
        //            GridPanel1.ColumnModel.Columns[mmss].Renderer.Fn = "tts";
        //        }
        //        GridPanel1.ColumnModel.Columns.Add(new Column()
        //        {
        //            Header = "合计",
        //            DataIndex = "tt"

        //        });
        //        GridPanel1.ColumnModel.Columns.Add(new Column()
        //        {
        //            Header = "剩余比例",
        //            DataIndex = "tp"

        //        });
        //        int ss = mm + 4;
        //        GridPanel1.ColumnModel.Columns[ss].Renderer.Fn = "tt";
        //        //  GridPanel1.ColumnModel.Columns[0].Width = 180;
        //        //TextField tf = new TextField();
        //        //tf.ID = this.ClientID.ToString() + "abc";
        //        //GridPanel1.ColumnModel.Columns[1].Editor.Add(tf);
        //        GridPanel1.DataBind();

        //        GridPanel1.Reconfigure();
            
            
        //    }
        //    else
        //    {

        //        JsonReader reader = new JsonReader();
        //        reader.Fields.Add("Name");
        //        reader.Fields.Add("code");
        //        reader.Fields.Add("des");
        //        reader.Fields.Add("m1");
        //        reader.Fields.Add("m2");
        //        reader.Fields.Add("m3");
        //        reader.Fields.Add("m4");
        //        reader.Fields.Add("m5");
        //        reader.Fields.Add("m6");
        //        reader.Fields.Add("m7");
        //        reader.Fields.Add("m8");
        //        reader.Fields.Add("m9");
        //        reader.Fields.Add("m10");
        //        reader.Fields.Add("m11");
        //        reader.Fields.Add("m12");
        //        reader.Fields.Add("tt");
        //        reader.Fields.Add("tp");
        //        Store1.Reader.Add(reader);
        //        //reader.Fields[1].ServerMapping = "ID";

        //        DataTable dt = new DataTable();
        //        DataRow dr;
        //        dt.Columns.Add(new DataColumn("Name", typeof(string)));
        //        dt.Columns.Add(new DataColumn("code", typeof(string)));
        //        dt.Columns.Add(new DataColumn("des", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m1", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m2", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m3", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m4", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m5", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m6", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m7", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m8", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m9", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m10", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m11", typeof(string)));
        //        dt.Columns.Add(new DataColumn("m12", typeof(string)));
        //        dt.Columns.Add(new DataColumn("tt", typeof(string)));
        //        dt.Columns.Add(new DataColumn("tp", typeof(string)));

        //        dr = dt.NewRow();
        //        dr[0] = DLStation.Text;
        //        dr[1] = "62010500";
        //        dr[2] = "Communication";
        //        dr[3] = "100/200";
        //        dr[4] = "500/200";
        //        dr[5] = "300/500";
        //        dr[6] = "0/600";
        //        dr[7] = "0/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1000/1200";
        //        dr[16] = "10.55";
        //        dt.Rows.Add(dr);
        //        dr = dt.NewRow();
        //        dr[0] = DLStation.Text;
        //        dr[1] = "62010900";
        //        dr[2] = "Entertainment";
        //        dr[3] = "100/200";
        //        dr[4] = "500/200";
        //        dr[5] = "300/500";
        //        dr[6] = "0/600";
        //        dr[7] = "0/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1200/1400";
        //        dr[16] = "10.55";
        //        dt.Rows.Add(dr);
        //        dr = dt.NewRow();
        //        dr[0] = DLStation.Text;
        //        dr[1] = "62012000";
        //        dr[2] = "Traveling";
        //        dr[3] = "100/200";
        //        dr[4] = "500/200";
        //        dr[5] = "300/500";
        //        dr[6] = "0/600";
        //        dr[7] = "0/800";
        //        dr[8] = "0/500";
        //        dr[9] = "0/500";
        //        dr[10] = "0/500";
        //        dr[11] = "0/500";
        //        dr[12] = "100/500";
        //        dr[13] = "100/200";
        //        dr[14] = "100/500";
        //        dr[15] = "1000/2000";
        //        dr[16] = "50.00";
        //        dt.Rows.Add(dr);
        //        Store1.DataSource = dt;
        //        Store1.DataBind();

        //        //GridPanel1.ColumnModel.Columns.Add(new Column()
        //        //{
        //        //    Header = "名称",
        //        //    DataIndex = "Name"
        //        //});
        //        //GridPanel1.ColumnModel.Columns.Add(new Column()
        //        //{
        //        //    Header = "代码",
        //        //    DataIndex = "code"
        //        //});
        //        //GridPanel1.ColumnModel.Columns.Add(new Column()
        //        //{
        //        //    Header = "描述",
        //        //    DataIndex = "des"
        //        //});

        //        //int mm = int.Parse(Months.Text) + 1;
        //        //for (int i = 1; i <= mm; i++)
        //        //{
        //        //    GridPanel1.ColumnModel.Columns.Add(new Column()
        //        //    {
        //        //        Header = i.ToString() + "月",
        //        //        DataIndex = "m" + i.ToString()
        //        //    });
        //        //}
        //        //GridPanel1.ColumnModel.Columns.Add(new Column()
        //        //{
        //        //    Header = "合计",
        //        //    DataIndex = "tt"

        //        //});
        //        //GridPanel1.ColumnModel.Columns.Add(new Column()
        //        //{
        //        //    Header = "比例",
        //        //    DataIndex = "tp"

        //        //});

        //        //  GridPanel1.ColumnModel.Columns[0].Width = 180;
        //        //TextField tf = new TextField();
        //        //tf.ID = this.ClientID.ToString() + "abc";
        //        //GridPanel1.ColumnModel.Columns[1].Editor.Add(tf);
        //        GridPanel1.DataBind();

        //        GridPanel1.Reconfigure();
        //        //for (int j = 0; j < dt.Rows.Count; j++)
        //        //{ 
        //        //GridPa

        //        //}
        //    }
        //}

    }
     
}