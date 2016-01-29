using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
namespace eReimbursement
{
    public partial class ApplyEE : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                this.Store1.DataSource = new object[]
            {
                new object[] { "2011-12-01", "微软", "999999", "吃饭","Bill Gates", 57.1,"总吃饭","Weiruan","Weiruan1","a.png" },
                new object[] { "2011-12-01", "微软", "999999", "吃饭","Bill Gates", 57.1,"总吃饭","Weiruan","Weiruan1","ac.png" },
                new object[] { "2011-12-01", "微软", "999999", "吃饭","Bill Gates", 57.1,"总吃饭","Weiruan","Weiruan1","a.png" },
                new object[] { "2011-12-01", "微软", "999999", "吃饭","Bill Gates", 57.1,"总吃饭","Weiruan","Weiruan1","a.png" },
                new object[] { "2011-12-01", "微软", "999999", "吃饭","Bill Gates", 57.1,"总吃饭","Weiruan","Weiruan1","ab.png" },
                new object[] { "2011-12-01", "微软", "999999", "吃饭","Bill Gates", 57.1,"总吃饭","Weiruan","Weiruan1","a.png" }
            };

                this.Store1.DataBind();
            }
        }
    }
}