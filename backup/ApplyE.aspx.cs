using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
namespace eReimbursement
{
    public partial class ApplyE : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                this.Store1.DataSource = new object[]
            {
                new object[] { "2011-12-01", "微软", "100", "吃饭","Bill Gates", 57.1,"总吃饭","Weiruan","Weiruan1","a.png","駐廠人員" },
                new object[] { "2011-12-01", "微软", "200", "吃饭","Bill Gates", 57.1,"总吃饭","Weiruan","Weiruan1","ac.png","國內交際費" },
                new object[] { "2011-12-01", "微软", "300", "吃饭","Bill Gates", 57.1,"总吃饭","Weiruan","Weiruan1","a.png","國外交際費" }
            };

                this.Store1.DataBind();
                this.Store2.DataSource = new object[]
            {
                new object[] { "駐廠人員", "62010901", "Entertainment - In House" },
                new object[] { "國內交際費", "62010910", "Entertainment - Domestic" },
                new object[] { "國外交際費", "62010920", "Entertainment - Overseas" }
            };

                this.Store2.DataBind();
            }
        }
    }
}