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
    public partial class Role : System.Web.UI.Page
    {
        DBAdapter dbs = new DBAdapter();


        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable tb = Comm.RtnEB("A1306", "Finance/Accounting", "CRP", "CRP", "62012021", "2013", "9");
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
                    DBGroup();
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "", "$('div.gn_person ul.q-menubox li:eq(0) a').text('" + Session["UserName"].ToString() + "');", true);
                X.AddScript("loginWindow.hide();Panel1.enable();");
            }
            //if (Comm.JdRole(Request.ServerVariables["URL"].ToString()) == 0)
            //{ 
            
            
            //}
            //if (!IsPostBack)
            //{
                
               
            
            //}
        }
        public void DBGroup()
        {
            string Station = Session["Station"].ToString();
             if(DLStation.Value!=null)
             {
                 Station=DLStation.Value.ToString();
             }
             string sql = "select id,station,GroupName,Remark from GroupType where station='" + Station + "'";
             if (Session["TSQSLM"] != null&&DLStation.Value==null)
             {
                 sql = Session["TSQSLM"].ToString();
             }
             DataSet ds = dbs.GetSqlDataSet(sql);
             Session["TSQSLM"] = sql;
            Store1.DataSource = ds.Tables[0];
            
            this.Store1.DataBind();

        }
        protected void button1_Search(object sender, DirectEventArgs e)
        {

            if (DLStation.Value==null || DLStation.Value.ToString() == "" || TextField1.Text == "")
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Message",
                    Message = "Please select Station and input Group Name.",
                    Buttons = MessageBox.Button.OK,
                    Width = 320,
                    Icon = MessageBox.Icon.INFO
                });
            }
            else
            {
                string sql = " insert into GroupType(station,GroupName,Remark,CreatedBy,CreatedDate)  values ('" + DLStation.Value.ToString() + "','" + TextField1.Text + "','" + TextField4.Text + "','" + Session["UserName"].ToString() + "','" + DateTime.Now.ToString() + "') ";
                int r = dbs.ExeSql(sql);
                if (r > 0)
                {
                    DataSet ds = dbs.GetSqlDataSet("select id,station,GroupName,Remark from GroupType where station='" + DLStation.Value.ToString() + "'");
                    Store1.DataSource = ds.Tables[0];
                    this.Store1.DataBind();
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
            DBGroup();
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
        [DirectMethod]
        public void DeleUsers(string ID)
        {

          int i= dbs.ExeSql("Delete from GroupType where id=" + ID + " delete from GroupUsers where GID=" + ID + " delete from GroupFlow where Gid=" + ID);
          DBGroup();
            if (i > 0)
          {
              X.Msg.Show(new MessageBoxConfig
              {
                  Title = "Message",
                  Message = "Delete Success",
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
                  Message = "Delete Fail",
                  Buttons = MessageBox.Button.OK,
                  Width = 320,
                  Icon = MessageBox.Icon.INFO
              });
          }
         
        }
    }
}