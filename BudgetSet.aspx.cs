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

namespace eReimbursement
{
   
    public partial class BudgetSet : System.Web.UI.Page
    { public string dad="";
        DBAdapter dbs = new DBAdapter();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                if (Session["UserID"] == null)
                {

                    Response.Redirect("Budget.aspx");
                    return;

                }
                else if (Comm.JdRole(Request.ServerVariables["URL"].ToString()) == 0)
                {
                    Response.Redirect("Budget.aspx");
                    return;
                }
                else
                {
                    string T = Request.QueryString["T"].ToString();
                    if (T == "1")
                    {
                        LBType.Text = "Person";

                        DLDepartment.Hide();
                        DLUser.Show();
                    }
                    else
                    {
                        DLUser.Hide();
                        DLDepartment.Show();
                        LBType.Text = "Department";
                        GetDepartment();
                    }
                    DB();
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "$('div.gn_person ul.q-menubox li:eq(0) a').text('" + Session["UserName"].ToString() + "');", true);
                    X.AddScript("loginWindow.hide();Panel1.enable();");
             //DataSet ds=dbs.GetSqlDataSet("select * from AccoundCode where BAccountCode in(select AccountCode from BudgetMain where id="+Request.QueryString["id"].ToString()+")");
             //if (ds.Tables[0].Rows.Count == 0)
             //{ DataSet dss=dbs.GetSqlDataSet("select '1' as id,'"+LBAccountCode.Text+"' as SAccountCode,'"+LBAccountDes.Text+"' as SAccountName,'' as ADes");
             //    SAccoundcode.DataSource = dss;
             //    SAccoundcode.DataBind();
                
             //}
             //else
             //{
             //    SAccoundcode.DataSource = ds;
             //    SAccoundcode.DataBind();
             //    DLAccoundCode.SelectedIndex = 300;
             //}
                 
                
                }
               
             
             
            }
            //code.Text = Request.QueryString["c"].ToString();
            //des.Text = Request.QueryString["d"].ToString();
            //m.Text = Request.QueryString["mm"].ToString();
            //mt.Text = Request.QueryString["mt"].ToString();
            //ss.Text = Request.QueryString["s"].ToString();
            //yy.Text = Request.QueryString["y"].ToString();
            //if (!X.IsAjaxRequest)
            //{
            //    this.Store1.DataSource = new object[]
            //{
            //    new object[] { "Hughson Huang","50", "MIS","200", "300", "400", "500", "600", "100", "100", "100", "100", "100", "1000", "1200" },
            //    new object[] { "Andy Kang","50", "MIS","200","200","200","200","200","200","200","200","200","200","200", "2400" }
            //};

            //    this.Store1.DataBind();
            //  //  GridPanel1.ColumnModel.SetHidden(2, true);
            //}
        }
        protected void BACK(object sender, DirectEventArgs e)
        {
            Response.Redirect("Budget.aspx");
        }
        protected void copy(object sender, DirectEventArgs e)
        { int nm=int.Parse(LBMonths.Text)+1;
        if (nm > 12)
        {
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Message",
                Message = "已经超过12月，不能拷贝",
                Buttons = MessageBox.Button.OK,
                Width = 320,
                Icon = MessageBox.Icon.INFO
            });
        }
        else
        {
            string nnamount = dbs.ExeSqlScalar("select sum(amount) from BudgetDetail where fid=" + Request.QueryString["id"].ToString() + " and months=" + LBMonths.Text);
            if (nnamount == "")
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Message",
                    Message = "要拷贝的月份数据为空，不能拷贝",
                    Buttons = MessageBox.Button.OK,
                    Width = 320,
                    Icon = MessageBox.Icon.INFO
                });
            }
            else
            {
                double ctt = double.Parse(nnamount);
                string nmonthtt=dbs.ExeSqlScalar("select m"+nm.ToString()+" from BudgetMain where id=" + Request.QueryString["id"].ToString());
                if (nmonthtt == "")
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Message",
                        Message = "下个月预算为空，不能拷贝",
                        Buttons = MessageBox.Button.OK,
                        Width = 320,
                        Icon = MessageBox.Icon.INFO
                    });
                }
                else
                {
                    if (double.Parse(nmonthtt) < double.Parse(nnamount))
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Message",
                            Message = "下个月预算小于要拷贝的数据，不能拷贝",
                            Buttons = MessageBox.Button.OK,
                            Width = 320,
                            Icon = MessageBox.Icon.INFO
                        });
                    }
                    else
                    {
                        int ii = dbs.ExeSql("INSERT INTO [BudgetDetail]([FID],[Months],[Name],[Amount],[Type] ,[SAccoundcode],[SAccoundName],[Deptment],[Station],[UserID],BaccountCode)select [FID],'" + nm.ToString() + "' as [Months],[Name],[Amount],[Type] ,[SAccoundcode],[SAccoundName],[Deptment],[Station],[UserID],BaccountCode from [BudgetDetail] where fid=" + Request.QueryString["id"].ToString() + " and months=" + LBMonths.Text);
                       if (ii > 0)
                       {
                           LBMonths.Value = nm.ToString();
                           DBDetail();
                           X.Msg.Show(new MessageBoxConfig
                           {
                               Title = "Message",
                               Message = "拷贝成功",
                               Buttons = MessageBox.Button.OK,
                               Width = 320,
                               Icon = MessageBox.Icon.INFO
                           });
                       }
                       else
                       {
                           X.Msg.Show(new MessageBoxConfig
                           {
                               Title = "Message",
                               Message = "拷贝失败",
                               Buttons = MessageBox.Button.OK,
                               Width = 320,
                               Icon = MessageBox.Icon.INFO
                           });
                       }
                    }
                }
               
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

        public void DB()
        {
            DataSet ds = dbs.GetSqlDataSet("select * from  BudgetMain where id=" + Request.QueryString["id"].ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                LBStation.Text = ds.Tables[0].Rows[0]["station"].ToString();
                LBYears.Text = ds.Tables[0].Rows[0]["Years"].ToString();
                LBAccountCode.Text = ds.Tables[0].Rows[0]["AccountCode"].ToString();
                LBAccountDes.Text = ds.Tables[0].Rows[0]["AccountDes"].ToString();
                LBAmount.Text = ds.Tables[0].Rows[0]["M1"].ToString();
               // dad=ds.Tables[0].Rows[0]["ADes"].ToString();
                DBDetail();
            }
        
        }
        protected void button1_Search1(object sender, DirectEventArgs e)
        {
            //if (ComboBox2.SelectedItem.Value == "个人")
            //{
            //    //us.Visible = true;
            //    //dp.Visible = false;
            //    dp.Hide();
            //    us.Show();
            //    GridPanel1.ColumnModel.SetHidden(2, false);
            //    this.Store1.DataSource = new object[]
            //{
            //    new object[] { "Hughson Huang","50", "MIS","200", "300", "400", "500", "600", "100", "100", "100", "100", "100", "1000", "1200" },
            //    new object[] { "Andy Kang","50", "MIS","200","200","200","200","200","200","200","200","200","200","200", "2400" }
            //};

            //    this.Store1.DataBind();
            //    GridPanel1.DataBind();
            //}
            //else
            //{
            //    us.Hide();
            //    dp.Show();
            //    GridPanel1.ColumnModel.SetHidden(2, true);
            //    this.Store1.DataSource = new object[]
            //{
            //    new object[] { "空运部","50", "100","200", "300", "400", "500", "600", "100", "100", "100", "100", "100", "1000", "1200" },
            //    new object[] { "海运部","50", "200","200","200","200","200","200","200","200","200","200","200","200", "2400" }
               
            //};

            //    this.Store1.DataBind();
            //    GridPanel1.DataBind();
            //    //us.Visible = false;
            //    //dp.Visible = true;
            //}

          
        }
        public void GetDepartment()
        {
            DataSet ds=DIMERCO.SDK.Utilities.LSDK.getCRPDepartment();
            //SqlConnection cn = new SqlConnection("Data Source=10.130.40.20;Initial Catalog=ReSM;User ID=sa;Password=dim1rc0@");
            //SqlDataAdapter da = new SqlDataAdapter("select distinct departmentname from  SMDepartment", cn);
            //DataSet ds = new DataSet();
            //da.Fill(ds);
            DataTable tb = ds.Tables[0];
            SDepartment.DataSource = tb;
            SDepartment.DataBind();
            ds.Dispose();
        }
        protected void Add_Search(object sender, DirectEventArgs e)
        {
            //if (DLAccoundCode.Text == "")
            //{
            //    X.Msg.Show(new MessageBoxConfig
            //    {
            //        Title = "Message",
            //        Message = "请选择Accountcode",
            //        Buttons = MessageBox.Button.OK,
            //        Width = 320,
            //        Icon = MessageBox.Icon.INFO
            //    });
            //}
            //else
            //{
                if (double.Parse(LBLast.Text) >= double.Parse(TXTAmount.Text))
                {
                    string name = DLDepartment.SelectedItem.Text;
                    string Dept = DLDepartment.SelectedItem.Text;
                    string Userid = "";
                    if (Request.QueryString["T"].ToString() == "1")
                    {
                        Userid = DLUser.Value.ToString();
                        name = DLUser.SelectedItem.Text;
                        if (DLUser.Value.ToString().Contains("Other")==false)
                        {

                            DataSet des = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(DLUser.Value.ToString());
                            Dept = des.Tables[0].Rows[0]["CRPDepartmentName"].ToString();
                        }
                        else {

                            Dept = DLUser.SelectedItem.Text.ToString().Replace("Other-", "");
                        
                        }
                    } 
                    string baccountcode = dbs.ExeSqlScalar("select BAccountCode from AccoundCode where SAccountCode='" + LBAccountCode.Text + "'");

                    dbs.ExeSql("update BudgetMain set type=" + Request.QueryString["T"].ToString() + " where id=" + Request.QueryString["id"].ToString() + "  insert into BudgetDetail(BaccountCode,SAccoundcode,SAccoundName,FID,Months,Name,Amount,Type,Deptment,Station,UserID) values('" + baccountcode + "','" + LBAccountCode.Text + "','" + LBAccountDes.Text + "'," + Request.QueryString["id"].ToString() + ",'" + LBMonths.Text + "','" + name + "','" + TXTAmount.Text + "'," + Request.QueryString["T"].ToString() + ",'" + Dept + "','" + Request.QueryString["S"].ToString() + "','" + Userid + "')");
                    DBDetail();

                }
           // }
        }
        
        protected void LBMonths_Search(object sender, DirectEventArgs e)
        {
          
                LBAmount.Text = dbs.ExeSqlScalar("select M" + LBMonths.Text + " from BudgetMain where id=" + Request.QueryString["id"].ToString());
                DBDetail();
             
            
        }
        public DataTable addtb(DataTable a)
        {
            DataSet ds = DIMERCO.SDK.Utilities.LSDK.getCRPDepartment();
          
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = a.NewRow();
                dr["UserID"] = "Other-" + ds.Tables[0].Rows[i]["DepartmentName"].ToString();
                dr["FullName"] ="Other-"+ds.Tables[0].Rows[i]["DepartmentName"].ToString();
                a.Rows.Add(dr);
            }
            return a;

        }
        protected void GetUser(object sender, DirectEventArgs e)
        {
            if (X.GetValue("DLUser").Length > 2)
            {
               // if (Request.QueryString["S"].ToString() == "GCRSHA")
               // {
               //DataSet ds=dbs.GetSqlDataSet("select Userid as UserID,UserName as FullName from ESUser where station='GCRSHA' and username like '%"+X.GetValue("DLUser")+"%'");

               //DataTable tb = addtb(ds.Tables[0]);
               //SUser.DataSource = tb;
               //SUser.DataBind();
               //ds.Dispose();
               // }
               // else if (Request.QueryString["S"].ToString() == "GCRHKG")
               // {
               //     DataSet ds = dbs.GetSqlDataSet("select Userid as UserID,UserName as FullName from ESUser where station='GCRHKG' and username like '%"+X.GetValue("DLUser")+"%'");

               //     DataTable tb = addtb(ds.Tables[0]);
               //     SUser.DataSource = tb;
               //     SUser.DataBind();
               //     ds.Dispose();
               // }
               // else
               // {
                DataSet ds = DIMERCO.SDK.Utilities.LSDK.getUserDataBYCostCenterNUser(Request.QueryString["S"].ToString(), "", X.GetValue("DLUser"));
                //SqlConnection cn = new SqlConnection("Data Source=10.130.40.20;Initial Catalog=ReSM;User ID=sa;Password=dim1rc0@");
                //string sql = "select distinct userid,fullname from  SMUser inner join SMstation on SMUser.stationid=SMstation.stationid where StationCode='" + Request.QueryString["S"].ToString() + "' and SMUser.fullname like '%" + X.GetValue("DLUser") + "%'";
                //SqlDataAdapter da = new SqlDataAdapter(sql, cn);
                //DataSet ds = new DataSet();
                //da.Fill(ds);
                DataTable tb = addtb(ds.Tables[0]);
                SUser.DataSource = tb;
                SUser.DataBind();
                ds.Dispose();}
           // }

        }
        public void DBDetail()
        {
            string mm = LBMonths.Text;
            if (mm == "")
            {
                mm = "1";
            }
            string sqll = "";
            //if (DLAccoundCode.Value != null)
            //{
            //    if (DLAccoundCode.Value.ToString() != "")
            //    {
            sqll = "SAccoundcode='" + LBAccountCode.Text + "' and ";
            //    }
            //}
            DataSet ds = dbs.GetSqlDataSet("select * from  BudgetDetail where "+sqll+" months='" + mm + "' AND  fid=" + Request.QueryString["id"].ToString());
            SDetail.DataSource = ds.Tables[0];
            SDetail.DataBind();
            CalLast();
        
        }
        public void CalLast()
        {
            string mm = LBMonths.Text;
            if (mm == "")
            {
                mm = "1";
            }
            string nn=LBAmount.Text;
            if (nn == "")
            {
                nn = "0";
            }
            string AM = dbs.ExeSqlScalar("select sum(amount) from BudgetDetail where fid="+Request.QueryString["id"].ToString()+" and  Months='" + mm + "'");
            if (AM == "")
            {
                AM = "0";
            }
            double pp = double.Parse(nn) - double.Parse(AM);
            LBLast.Text = pp.ToString("F2");
        
        }
        [DirectMethod]
        public void DeleUsers(string ID)
        {
            string fid = dbs.ExeSqlScalar("select fid from BudgetDetail where id=" + ID);
            dbs.ExeSql("Delete from BudgetDetail where id=" + ID);
            int ctt = int.Parse(dbs.ExeSqlScalar("select count(*) from BudgetDetail where fid=" + fid));
            if (ctt == 0)
            {
                dbs.ExeSql("update BudgetMain set type=0 where where id=" + fid);
            }
            DBDetail();
        }
    }
}