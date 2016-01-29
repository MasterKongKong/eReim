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
    public partial class BudgetEmail : System.Web.UI.Page
    { DBAdapter dbs = new DBAdapter();
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


        protected void Save(object sender, DirectEventArgs e)
        {
                int cc = int.Parse(dbs.ExeSqlScalar("select count(*) from EmailTo where station='" + DLStation.Value + "'"));
                if (cc == 0)
                {
                    int r = dbs.ExeSql("insert into EmailTo(Station,Email,CreatedDate,CreatedBy) values('" + DLStation.Value + "','" + TXTEmail.Text + "','" + DateTime.Now.ToString() + "','" + Session["UserName"].ToString() + "')");
                    if (r > 0)
                    {
                        DLStation.Value = null;
                        TXTEmail.Text = "";
                        DBB();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Message",
                            Message = "Save success",
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
                            Message = "Save Faile",
                            Buttons = MessageBox.Button.OK,
                            Width = 320,
                            Icon = MessageBox.Icon.INFO
                        });

                    }
                }
                else
                {
                    int r = dbs.ExeSql("update  EmailTo set email='" + TXTEmail.Text + "',createddate='" + DateTime.Now.ToString() + "',createdby='" + Session["UserName"].ToString() + "' where station='" + DLStation.Value + "'");
                    if (r > 0)
                    {
                        DLStation.Value = null;
                        TXTEmail.Text = "";
                        DBB();
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Title = "Message",
                            Message = "Save success",
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
                            Message = "Save Faile",
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
           DataSet ds = dbs.GetSqlDataSet("select * from EmailTo where station='" + DLStation.Value.ToString() + "'");
           Store1.DataSource = ds.Tables[0];
           Store1.DataBind();
           TXTEmail.Text = "";
       
       }
       [DirectMethod]
       public void SetDefaultData(string ID)
       {
         
           DataSet ds = dbs.GetSqlDataSet("select * from EmailTo where Station='" + ID+"'");
           DLStation.Value = ds.Tables[0].Rows[0]["station"].ToString();
           TXTEmail.Text = ds.Tables[0].Rows[0]["email"].ToString();
       }
       [DirectMethod]
       public void DeleUsers(string ID)
       {

          int r=dbs.ExeSql("Delete from EmailTo where station='" + ID+"'");
           if (r > 0)
           {
               
               DBB();
               X.Msg.Show(new MessageBoxConfig
               {
                   Title = "Message",
                   Message = "Delete success",
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
                   Message = "Delete Faile",
                   Buttons = MessageBox.Button.OK,
                   Width = 320,
                   Icon = MessageBox.Icon.INFO
               });
           }
          
       }
       public void DBB()
       {
           if (DLStation.Value != null)
           {
               DataSet ds = dbs.GetSqlDataSet("select * from EmailTo where station='" + DLStation.Value.ToString() + "'");
               Store1.DataSource = ds.Tables[0];
               Store1.DataBind();
           }
           else
           {
               DataSet ds = dbs.GetSqlDataSet("select * from EmailTo where createdby='" + Session["UserName"].ToString() + "'");
               Store1.DataSource = ds.Tables[0];
               Store1.DataBind();
           }

       }
    }
}