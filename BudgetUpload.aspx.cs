using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using System.Data.SqlClient;
using System.Data;

namespace eReimbursement
{
    public partial class BudgetUpload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                //DataTable dt = new DataTable();
                //DataRow dr;
                //dt.Columns.Add(new DataColumn("Name", typeof(string)));
                //dt.Columns.Add(new DataColumn("ja", typeof(string)));
                //dt.Columns.Add(new DataColumn("fe", typeof(string)));
                //dt.Columns.Add(new DataColumn("mo", typeof(string)));
                //dt.Columns.Add(new DataColumn("ap", typeof(string)));
                //dt.Columns.Add(new DataColumn("ma", typeof(string)));
                //dt.Columns.Add(new DataColumn("ju", typeof(string)));
                //dt.Columns.Add(new DataColumn("jl", typeof(string)));
                //dt.Columns.Add(new DataColumn("ag", typeof(string)));
                //dt.Columns.Add(new DataColumn("se", typeof(string)));
                //dt.Columns.Add(new DataColumn("oc", typeof(string)));
                //dt.Columns.Add(new DataColumn("no", typeof(string)));
                //dt.Columns.Add(new DataColumn("de", typeof(string)));
                //dt.Columns.Add(new DataColumn("tt", typeof(string)));
                //dt.Columns.Add(new DataColumn("tp", typeof(string)));
                this.Store1.DataSource = new object[]
                {
                    new object[] { "62010500","Communication", "100","100", "100", "100", "100", "100", "100", "100", "100", "100", "100", "100", "1200" },
                    new object[] { "62010900","Entertainment", "200","200","200","200","200","200","200","200","200","200","200","200", "2400" },
                    new object[] { "62012000","Traveling", "150","150","150","150","150","150","150","150","150","150","150","150", "1800" }
                };

                this.Store1.DataBind();
            }
        }



    }
}