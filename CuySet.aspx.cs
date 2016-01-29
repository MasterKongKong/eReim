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
    public partial class CuySet : System.Web.UI.Page
    {
        DBAdapter dbs = new DBAdapter();
        protected void Page_Load(object sender, EventArgs e)
        {
            //Session["UserID"] = "A2528";
            //Session["UserName"] = "Sunhui Chen";
            if (!X.IsAjaxRequest)
            {
                if (Session["UserID"] == null)
                {
                    X.AddScript("loginWindow.show();Panel1.disable();");
                    return;
                }
                else
                {
                    StationDB();
                    DBB();
                 
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "$('div.gn_person ul.q-menubox li:eq(0) a').text('" + Session["UserName"].ToString() + "');", true);
                    X.AddScript("loginWindow.hide();Panel1.enable();");
                }

            }
        }
        protected void button1_Search(object sender, DirectEventArgs e)
        {
            DataTable tbb; string stationsss = "";
            string Nct = dbs.ExeSqlScalar("select top 1 admin from StationRole where UserID='" + Session["UserID"].ToString() + "'").ToString();
            if (Nct == "1")
            {
                DataSet ds = DIMERCO.SDK.Utilities.LSDK.getStationHierarchy();
                //SqlConnection cn = new SqlConnection("Data Source=10.130.40.20;Initial Catalog=ReSM;User ID=sa;Password=dim1rc0@");
                //SqlDataAdapter da = new SqlDataAdapter("select distinct StationCode from SMStation order by StationCode", cn);
                //DataSet ds = new DataSet();
                //da.Fill(ds);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    stationsss = stationsss + "'" + ds.Tables[0].Rows[i]["StationCode"].ToString()+ "',";
                }
               

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
                        stationsss = stationsss + "'" + Sp[i].ToString() + "',";
                        
                    }
                }
             
            }
            DataSet dss = dbs.GetSqlDataSet("select * from ESUser where station in (" + stationsss + "'0')");
            Store1.DataSource = dss.Tables[0];
            Store1.DataBind();
        }

        protected void Chanage_Search(object sender, DirectEventArgs e)
        {
           // int ct = int.Parse(dbs.ExeSqlScalar("select count(*) from BudgetMain where Station='" + DLStation.Text + "' and Years='" + DLYears.Text + "'"));
          
            //DataSet ds = dbs.GetSqlDataSet("select *,(m1+m2+m3+m4+m5+m6+m7+m8+m9+m10+m11+m12) as TT from BudgetMain where years='" + DLYears.Text + "' and  station='" + DLStation.Text + "'");
            //Store1.DataSource = ds.Tables[0];
            //this.Store1.DataBind();
            DBB();
        }
        protected void BTN_Search(object sender, DirectEventArgs e)
        {
           
            if (HH.Value == "")
            {
                int CT = int.Parse(dbs.ExeSqlScalar("select count(*) from ESUser where Userid='" + DLUser.Value + "'"));
                if (CT <= 0)
                {
                    int r = dbs.ExeSql("insert into ESUser(Userid,UserName,Station,Currency) values('" + DLUser.Value + "','" + DLUser.SelectedItem.Text + "','" + DLStation.SelectedItem.Text + "','" + DLCurrency.SelectedItem.Text + "')");
                    DBB();
                    Window1.Hide();
                    if (r > 0)
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Message",
                            Message = "保存成功",
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
                            Message = "保存失败",
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
                        Message = "该用户币种表已经存在，请点击修改",
                        Buttons = MessageBox.Button.OK,
                        Width = 320,
                        Icon = MessageBox.Icon.INFO
                    });
                }


            }
            else
            {
                int r = dbs.ExeSql("update ESUser set Currency='" + DLCurrency.SelectedItem.Text + "' where id=" + HH.Value);
                HH.Value = "";
                DBB();
                Window1.Hide();
                if (r > 0)
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Message",
                        Message = "保存成功",
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
                        Message = "保存失败",
                        Buttons = MessageBox.Button.OK,
                        Width = 320,
                        Icon = MessageBox.Icon.INFO
                    });
                }

            }
        }
        public void StationDB()
        {
            string Nct = dbs.ExeSqlScalar("select top 1 admin from StationRole where UserID='" + Session["UserID"].ToString() + "'").ToString();
            if (Nct == "1")
            {
                DataSet ds = DIMERCO.SDK.Utilities.LSDK.getStationHierarchy();
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
        public void DBB()
        {
            DataSet ds = dbs.GetSqlDataSet("select * from ESUser where station='" + DLStation.Value + "'");
            Store1.DataSource = ds.Tables[0];
            Store1.DataBind();
            DataSet dss = DIMERCO.SDK.Utilities.ReSM.GetCurrency();
            Store2.DataSource = dss.Tables[0];
            Store2.DataBind();

        }
        protected void GetUser(object sender, DirectEventArgs e)
        {
            if (X.GetValue("DLUser").Length > 2)
            {
                DataSet ds = DIMERCO.SDK.Utilities.LSDK.getUserDataBYStationCodeNUser(DLStation.Value.ToString(), "", X.GetValue("DLUser"));
                //DataSet ds = DIMERCO.SDK.Utilities.LSDK.getUserDataBYUserName(X.GetValue("DLUser"), 10);
                //SqlConnection cn = new SqlConnection("Data Source=10.130.40.20;Initial Catalog=ReSM;User ID=sa;Password=dim1rc0@");
                //string sql = "select distinct userid,fullname from  SMUser inner join SMstation on SMUser.stationid=SMstation.stationid where SMUser.fullname like '%" + X.GetValue("DLUser") + "%'";
                //SqlDataAdapter da = new SqlDataAdapter(sql, cn);
                //DataSet ds = new DataSet();
                //da.Fill(ds);
                DataTable tb = ds.Tables[0];
                SUser.DataSource = tb;
                SUser.DataBind();
                ds.Dispose();
            }

        }

        [DirectMethod]
        public void DeleUsers(string ID)
        {

            dbs.ExeSql("Delete from ESUser where id=" + ID);

            DBB();
        }
        [DirectMethod]
        public void SetDefaultData(string ID)
        {
            DataSet ds = dbs.GetSqlDataSet("select * from ESUser where id=" + ID);
            if (ds.Tables[0].Rows.Count > 0)
            {
                // DLUser.Items.Add(new Ext.Net.ListItem(ds.Tables[0].Rows[0]["PAgent"].ToString(), ds.Tables[0].Rows[0]["PAgentID"].ToString()));
                DataTable TTB = new DataTable();
                TTB.Columns.Add("UserID");
                TTB.Columns.Add("FullName");
                DataRow dr = TTB.NewRow();
                dr["UserID"] = ds.Tables[0].Rows[0]["Userid"].ToString();
                dr["FullName"] = ds.Tables[0].Rows[0]["UserName"].ToString();
                TTB.Rows.Add(dr);
                SUser.DataSource = TTB;
                SUser.DataBind();
                DLUser.SelectedIndex = 0;
                DLCurrency.Value = ds.Tables[0].Rows[0]["Currency"].ToString();
            }
        }
    }
}