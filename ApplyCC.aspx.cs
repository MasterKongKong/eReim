using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
namespace eReimbursement
{
    public partial class ApplyCC : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                this.Store1.DataSource = new object[]
            {
                new object[] { "1月-6月", "IP电话费", "500","电话费","a.PNG" },
                new object[] { "1月-6月", "IP电话费", "500","电话费","a.PNG" },
                new object[] { "1月-6月", "IP电话费", "500","电话费","a.PNG" },
                new object[] { "1月-6月", "IP电话费", "500","电话费","a.PNG" },
                new object[] { "1月-6月", "IP电话费", "500","电话费","a.PNG" }
            };

                this.Store1.DataBind();
            }
        }
    }
}