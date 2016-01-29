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
    public partial class BudgetDetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                this.Store1.DataSource = new object[]
            {
                new object[] { "62010500","Communication", "100","100", "100", "100", "100", "100", "100", "100", "100", "100", "100", "100", "1200" },
                new object[] { "62010900","Entertainment", "200","200","200","200","200","200","200","200","200","200","200","200", "2400" },
                new object[] { "62012000","Traveling", "150","150","150","150","150","150","150","150","150","150","150","150", "1800" }
            };

                this.Store1.DataBind();
            }
        }
        
        protected void DatePicker1_Select(object sender, DirectEventArgs e)
        {


        }
        protected void button1_Search(object sender, DirectEventArgs e)
        {


        }


    }
}