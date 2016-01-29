using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
namespace eReimbursement
{
    public partial class ClaimPreview : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                this.Store1.DataSource = new object[]
            {
                new object[] { "2012-01-13", "SAMSUNG", "100000", "吃饭","King","King", "1000","吃饭" },
                new object[] { "2012-01-13", "SAMSUNG", "100000", "吃饭","King","King", "1000","吃饭" },
                new object[] { "2012-01-13", "SAMSUNG", "100000", "吃饭","King","King", "1000","吃饭" },
                new object[] { "2012-01-13", "SAMSUNG", "100000", "吃饭","King","King", "1000","吃饭" }
            };

                this.Store1.DataBind();
            }
        }
    }
}