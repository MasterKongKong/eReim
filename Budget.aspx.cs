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
    public partial class Budget : System.Web.UI.Page
    {
        DBAdapter dbs = new DBAdapter();
        protected void Page_Load(object sender, EventArgs e)
        {
            //string newsql = "select distinct userid as PersonID from BudgetDetail";
            //DataSet  newdt = dbs.GetSqlDataSet(newsql);
            //for (int i = 0; i < newdt.Tables[0].Rows.Count; i++)
            //{
            //    DataSet dstest = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(newdt.Tables[0].Rows[i]["PersonID"].ToString());
            //    if (dstest.Tables[0].Rows.Count == 1)
            //    {
            //        DataTable dtnew = dstest.Tables[0];
            //        string odep = dtnew.Rows[0]["CRPDepartmentName"].ToString();
            //        //string upsqp = "update Ecommon set Department='" + odep + "' where PersonID='" + newdt.Rows[i]["PersonID"].ToString() + "'";
            //        string upsqp = "update BudgetDetail set Deptment='" + odep + "' where userid='" + newdt.Tables[0].Rows[i]["PersonID"].ToString() + "'";
            //        dbs.ExeSql(upsqp);
            //    }
            //}
            //DataTable dt = Comm.RtnEB("A2232", "MIS/IT", "ZJDTSN", "ZJDTSN", "62010900", "2013", "11");
           // DataTable dt= Comm.RtnEB("A0377", "Marketing", "CRP", "CRP", "62010910", "2014", "2");
        //    DataTable dtBG = Comm.RtnEB("A0360", "MIS/IT", "GCRSHA", "GCRSHA", "62011900", "2014", "3");
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
                        StationDB();
                        DBB();
                    }
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "$('div.gn_person ul.q-menubox li:eq(0) a').text('" + Session["UserName"].ToString() + "');", true);
                    X.AddScript("loginWindow.hide();Panel1.enable();");
                }
             
            }
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


        protected void down_Search(object sender, DirectEventArgs e)
        {
            //Response.Clear();
            //Response.AddHeader("content-disposition", "attachment;filename=FileName.xls");
            //Response.Charset = "gb2312";
            //Response.ContentType = "application/vnd.xls";
            //System.IO.StringWriter stringWrite = new System.IO.StringWriter();
            //System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
            //GridView1.RenderControl(htmlWrite);

            //Response.Write(stringWrite.ToString());
            //Response.End();
            //XmlNode xml = e.Xml;

            //this.Response.Clear();
            //this.Response.ContentType = "application/vnd.ms-excel";
            //this.Response.AddHeader("Content-Disposition", "attachment; filename=submittedData.xls");
            //XslCompiledTransform xtExcel = new XslCompiledTransform();
            //xtExcel.Load(Server.MapPath("Excel.xsl"));
            //xtExcel.Transform(xml, null, Response.OutputStream);

            //string json = GridData.Value.ToString();
            //StoreSubmitDataEventArgs eSubmit = new StoreSubmitDataEventArgs(json, null);
            //XmlNode xml = eSubmit.Xml;

            //this.Response.Clear();
            //this.Response.ContentType = "application/vnd.ms-excel";
            //this.Response.AddHeader("Content-Disposition", "attachment; filename=submittedData.xls");
            //XslCompiledTransform xtExcel = new XslCompiledTransform();
            //xtExcel.Load(Server.MapPath("Excel.xsl"));
            //xtExcel.Transform(xml, null, this.Response.OutputStream);
            //this.Response.End();

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
     
        protected void Load_Search(object sender, DirectEventArgs e)
        {
            LoadBudget();
        }
        public void LoadBudget()
        {
           
           int ct = int.Parse(dbs.ExeSqlScalar("select count(*) from BudgetMain where Station='" + DLStation.Text + "' and Years='" + DLYears.Text + "'"));
           if (ct <= 0)
           {
               if (DLStation.Text.ToString().Trim() != "")
               {
                   string WebServiceUrl = System.Configuration.ConfigurationManager.AppSettings["WebserviceURL"].ToString();
                   ServiceReference1.BudgetApplicationSoapClient objService = new ServiceReference1.BudgetApplicationSoapClient("BudgetApplicationSoap", WebServiceUrl);
                   DataSet Budgetds = objService.GetBudgetPackage(DLStation.Text.ToString(), int.Parse(DLYears.Text.ToString()), "BudgetPackageToken");
                   string sql = "set xact_abort on BEGIN TRAN ";
                   for (int i = 0; i < Budgetds.Tables[0].Rows.Count; i++)
                   {
                       DataRow dr = Budgetds.Tables[0].Rows[i];
                       string baccountcode = dbs.ExeSqlScalar("select BAccountCode from AccoundCode where SAccountCode='" + dr["AccountCode"].ToString() + "'");
                       sql = sql + " insert into BudgetMain(BaacountCode,Station,Years,AccountCode,AccountDes,M1,M2,M3,M4,M5,M6,M7,M8,M9,M10,M11,M12,type)";
                       sql = sql + "Values ('"+baccountcode+"','" + DLStation.Text.ToString() + "'," + DLYears.Text.ToString() + ",'" + dr["AccountCode"].ToString() + "','" + dr["AccountDesc"].ToString() + "'," + JDnull(dr["bJAN"].ToString()) + "," + JDnull(dr["bFEB"].ToString()) + "," + JDnull(dr["bMAR"].ToString()) + "," + JDnull(dr["bAPR"].ToString()) + "," + JDnull(dr["bMAY"].ToString()) + "," + JDnull(dr["bJUN"].ToString()) + "," + JDnull(dr["bJUN1"].ToString()) + "," + JDnull(dr["bAUG"].ToString()) + "," + JDnull(dr["bSEP"].ToString()) + "," + JDnull(dr["bOCT"].ToString()) + "," + JDnull(dr["bNOV"].ToString()) + "," + JDnull(dr["bDEC"].ToString()) + ",0)";

                   }
                   sql += " COMMIT TRAN";
                   int r = dbs.ExeSql(sql);
                   if (r < 0)
                   {
                       X.Msg.Show(new MessageBoxConfig
                       {
                           Title = "Message",
                           Message = "载入失败",
                           Buttons = MessageBox.Button.OK,
                           Width = 320,
                           Icon = MessageBox.Icon.INFO
                       });
                   }
                   else
                   {
                       LoadData.Hide();
                       X.Msg.Show(new MessageBoxConfig
                       {
                           Title = "Message",
                           Message = "载入成功",
                           Buttons = MessageBox.Button.OK,
                           Width = 320,
                           Icon = MessageBox.Icon.INFO
                       });
                       DBB();
                   }
               }
               else
               {

                   X.Msg.Show(new MessageBoxConfig
                   {
                       Title = "Message",
                       Message = "请选择站点",
                       Buttons = MessageBox.Button.OK,
                       Width = 320,
                       Icon = MessageBox.Icon.INFO
                   });
               }
           }
           else
           {
               X.Msg.Show(new MessageBoxConfig
               {
                   Title = "Message",
                   Message = "预算已载入",
                   Buttons = MessageBox.Button.OK,
                   Width = 320,
                   Icon = MessageBox.Icon.INFO
               });
           
           }
        
        }
        public string JDnull(string word)
        {
            if (word.Trim() == "")
            {
                return "NULL";
            }
            else
            {
            return word;
            }
        }
        public void DBB()
        {
            string TTstation = "";
            string TTyears = "";
           
           
             if (DLStation.Value == null && Session["TTstation"]!=null)
             {
                 TTstation = Session["TTstation"].ToString();
                 TTyears = Session["TTyears"].ToString();
            
             }
             if (DLStation.Value != null)
             {
                 TTstation = DLStation.Value.ToString();
                 TTyears = DLYears.Value.ToString();
                 Session["TTstation"] = DLStation.Value.ToString();
                 Session["TTyears"] = DLYears.Value.ToString();
             }
             if (DLStation.Value == null && Session["TTstation"] != null)
             {
                 DLStation.Value = TTstation;
                 DLYears.Value = TTyears;
             }
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("sort", typeof(string)));
                dt.Columns.Add(new DataColumn("my", typeof(string)));
                dt.Columns.Add(new DataColumn("main", typeof(string)));
                dt.Columns.Add(new DataColumn("code", typeof(string)));
                dt.Columns.Add(new DataColumn("des", typeof(string)));
                dt.Columns.Add(new DataColumn("m1", typeof(string)));
                dt.Columns.Add(new DataColumn("m2", typeof(string)));
                dt.Columns.Add(new DataColumn("m3", typeof(string)));
                dt.Columns.Add(new DataColumn("m4", typeof(string)));
                dt.Columns.Add(new DataColumn("m5", typeof(string)));
                dt.Columns.Add(new DataColumn("m6", typeof(string)));
                dt.Columns.Add(new DataColumn("m7", typeof(string)));
                dt.Columns.Add(new DataColumn("m8", typeof(string)));
                dt.Columns.Add(new DataColumn("m9", typeof(string)));
                dt.Columns.Add(new DataColumn("m10", typeof(string)));
                dt.Columns.Add(new DataColumn("m11", typeof(string)));
                dt.Columns.Add(new DataColumn("m12", typeof(string)));
                dt.Columns.Add(new DataColumn("tt", typeof(string)));
                dt.Columns.Add(new DataColumn("Type", typeof(string)));
                dt.Columns.Add(new DataColumn("id", typeof(string)));
                DataSet ds = dbs.GetSqlDataSet("select *,(m1+m2+m3+m4+m5+m6+m7+m8+m9+m10+m11+m12) as TT from BudgetMain where station='" + TTstation + "' AND YEARS='" + TTyears + "'");
         
                int k = 0;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string main = ds.Tables[0].Rows[i]["AccountCode"].ToString() + "/" + ds.Tables[0].Rows[i]["AccountDes"].ToString();
                    DataRow mdr = dt.NewRow();
                    mdr["id"] = ds.Tables[0].Rows[i]["id"].ToString();
                    mdr["sort"] = k.ToString();
                    mdr["my"] = "0";
                    mdr["main"] = main;
                    mdr["code"] = ds.Tables[0].Rows[i]["AccountCode"].ToString();
                    mdr["des"] = ds.Tables[0].Rows[i]["AccountDes"].ToString(); ;
                    mdr["m1"] = ds.Tables[0].Rows[i]["m1"].ToString();
                    mdr["m2"] = ds.Tables[0].Rows[i]["m2"].ToString();
                    mdr["m3"] = ds.Tables[0].Rows[i]["m3"].ToString();
                    mdr["m4"] = ds.Tables[0].Rows[i]["m4"].ToString();
                    mdr["m5"] = ds.Tables[0].Rows[i]["m5"].ToString();
                    mdr["m6"] = ds.Tables[0].Rows[i]["m6"].ToString();
                    mdr["m7"] = ds.Tables[0].Rows[i]["m7"].ToString();
                    mdr["m8"] = ds.Tables[0].Rows[i]["m8"].ToString();
                    mdr["m9"] = ds.Tables[0].Rows[i]["m9"].ToString();
                    mdr["m10"] = ds.Tables[0].Rows[i]["m10"].ToString();
                    mdr["m11"] = ds.Tables[0].Rows[i]["m11"].ToString();
                    mdr["m12"] = ds.Tables[0].Rows[i]["m12"].ToString();
                    mdr["tt"] = ds.Tables[0].Rows[i]["TT"].ToString();
                    mdr["Type"] = ds.Tables[0].Rows[i]["Type"].ToString();
                    dt.Rows.Add(mdr);
                    DataSet dss = dbs.GetSqlDataSet("select distinct SAccoundcode,SAccoundName from BudgetDetail where fid='" + ds.Tables[0].Rows[i]["id"].ToString() + "'");
                    if (dss.Tables[0].Rows.Count == 0)
                    {
                        k = k + 1;
                    }
                    for (int j = 0; j < dss.Tables[0].Rows.Count; j++)
                    {
                        k = k + 1;
                        DataRow dr = dt.NewRow();
                        dr["sort"] = k.ToString();
                        dr["my"] = "1";
                        dr["main"] = main;
                        dr["id"] = "0";
                        dr["code"] = dss.Tables[0].Rows[j]["SAccoundcode"].ToString();
                        dr["Type"] = ds.Tables[0].Rows[i]["type"].ToString();
                        dr["des"] = dss.Tables[0].Rows[j]["SAccoundName"].ToString();
                        string saccountcode = dss.Tables[0].Rows[j]["SAccoundcode"].ToString();
                        double Stotal = 0;
                        for (int m = 1; m < 13; m++)
                        {
                            string samount = dbs.ExeSqlScalar("select sum(Amount) from BudgetDetail where Months=" + m + " and  fid='" + ds.Tables[0].Rows[i]["id"].ToString() + "' and SAccoundcode='" + saccountcode + "'");
                            string mid = "m" + m.ToString();
                            if (samount != "")
                            {
                                dr[mid] = samount;
                                Stotal = Stotal + double.Parse(samount);
                            }
                            else
                            { dr[mid] = "0.00"; }
                        }
                        dr["tt"] = Stotal.ToString();
                        dt.Rows.Add(dr);

                    }
                    if (dss.Tables[0].Rows.Count > 0)
                    {
                        k = k + 1;
                        DataRow tdr = dt.NewRow();
                        tdr["sort"] = k.ToString();
                        tdr["my"] = "1";
                        tdr["main"] = main;
                        tdr["id"] = "0";
                        tdr["code"] = "";
                        tdr["Type"] = ds.Tables[0].Rows[i]["type"].ToString();
                        tdr["des"] = "The balance";
                        double ttamount = 0;
                        for (int q = 1; q < 13; q++)
                        {
                            string mq = "m" + q.ToString();
                            double tms = jdouble(lastamout(q.ToString(), ds.Tables[0].Rows[i]["id"].ToString()));
                            ttamount = ttamount + tms;
                            tdr[mq] = Convert.ToString(double.Parse(ds.Tables[0].Rows[i][mq].ToString()) - tms);
                        }
                        tdr["tt"] = Convert.ToString(double.Parse(ds.Tables[0].Rows[i]["tt"].ToString()) - ttamount);
                        dt.Rows.Add(tdr);
                        k = k + 1;
                    }
                }
                //GridView1.DataSource = dt;
                //GridView1.DataBind();
                Store2.DataSource = dt;
                Store2.DataBind();
           
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
        protected void Chanage_Search(object sender, DirectEventArgs e)
        {
            int ct = int.Parse(dbs.ExeSqlScalar("select count(*) from BudgetMain where Station='" + DLStation.Text + "' and Years='" + DLYears.Text + "'"));
            if (ct <= 0)
            { LoadData.Show(); DoadData.Hide(); }
            else
            {
              
                LoadData.Hide(); DoadData.Show();
                
            }
            //DataSet ds = dbs.GetSqlDataSet("select *,(m1+m2+m3+m4+m5+m6+m7+m8+m9+m10+m11+m12) as TT from BudgetMain where years='" + DLYears.Text + "' and  station='" + DLStation.Text + "'");
            //Store1.DataSource = ds.Tables[0];
            //this.Store1.DataBind();
            DBB();
        }
        public string lastamout(string m, string fid)
        {
            return dbs.ExeSqlScalar("select sum(Amount) from BudgetDetail where Months=" + m + " and  fid='" + fid + "' ");
        }
        public double jdouble(string aa)
        {
            if (aa == "")
            { return 0; }
            else { return double.Parse(aa); }
        }
    }
      
}