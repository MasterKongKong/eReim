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
    public partial class Agent : System.Web.UI.Page
    {
        DBAdapter dbs = new DBAdapter();
        protected void Page_Load(object sender, EventArgs e)
        {   
            if (!X.IsAjaxRequest)
            {
                if (Session["UserID"] == null)
                {
                    X.AddScript("loginWindow.show();Panel1.disable();");
                    return;
                }
                else
                {
                    DBB();
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "$('div.gn_person ul.q-menubox li:eq(0) a').text('" + Session["UserName"].ToString() + "');", true);
                    X.AddScript("loginWindow.hide();Panel1.enable();");
                }
             
            }
        }
        protected void BTN_Search(object sender, DirectEventArgs e)
        {
            string tcks = "0";
            if (CK.Checked == true)
            {
                tcks = "1";
            }
            if (HH.Value == "")
            {

                int r = dbs.ExeSql("insert into Eagent(Owner,OwnerID,PAgent,PAgentID,Bdate,Edate,St,CrededDate) values('" + Session["UserName"] + "','" + Session["UserID"] + "','" + DLUser.SelectedItem.Text + "','" + DLUser.Value + "','" + TXTBdate.Text + "','" + TXTEdate.Text + "','" + tcks + "','" + DateTime.Now.ToString() + "')");
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
               int r= dbs.ExeSql("update Eagent set PAgent='" + DLUser.SelectedItem.Text + "',PAgentID='" + DLUser.Value + "',Bdate='" + TXTBdate.Text + "',Edate='" + TXTEdate.Text + "',St='" + tcks + "',CrededDate='" + DateTime.Now.ToString() + "' where id=" + HH.Value);
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
            DataSet ds = dbs.GetSqlDataSet("select * from eagent where ownerid='" + Session["UserID"].ToString() + "'");
            Store1.DataSource = ds.Tables[0];
            Store1.DataBind();
        
        }
        protected void GetUser(object sender, DirectEventArgs e)
        {
            if (X.GetValue("DLUser").Length > 2)
            {
                DataSet ds = DIMERCO.SDK.Utilities.LSDK.getUserDataBYUserName(X.GetValue("DLUser"), 10);
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

            dbs.ExeSql("Delete from eagent where id=" + ID);

            DBB();
        }
        [DirectMethod]
        public void SetDefaultData(string ID)
        {
            DataSet ds = dbs.GetSqlDataSet("select * from eagent where id=" + ID);
            if (ds.Tables[0].Rows.Count > 0)
            {
               // DLUser.Items.Add(new Ext.Net.ListItem(ds.Tables[0].Rows[0]["PAgent"].ToString(), ds.Tables[0].Rows[0]["PAgentID"].ToString()));
                DataTable TTB = new DataTable();
                TTB.Columns.Add("userID");
                TTB.Columns.Add("fullname");
                DataRow dr = TTB.NewRow();
                dr["userid"] = ds.Tables[0].Rows[0]["PAgentID"].ToString();
                dr["fullname"] = ds.Tables[0].Rows[0]["PAgent"].ToString();
                TTB.Rows.Add(dr);
                SUser.DataSource = TTB;
                SUser.DataBind();
                DLUser.SelectedIndex = 0;
                TXTBdate.Text = ds.Tables[0].Rows[0]["Bdate"].ToString();
                TXTEdate.Text = ds.Tables[0].Rows[0]["Edate"].ToString();
                string ckk = ds.Tables[0].Rows[0]["St"].ToString();
                if (ckk == "1")
                {
                    CK.Checked = true;
             
                }
            }
        }
    }
}