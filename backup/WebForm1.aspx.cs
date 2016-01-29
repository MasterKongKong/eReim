using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;

namespace eReimbursement.backup
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                this.Store1.DataSource = this.Data;
                this.Store1.DataBind();
            }
        }

        private object[] Data
        {
            get
            {
                return new object[]
            {
                new object[] { "3m Co", 71.72, 0.02, 0.03, "9/1 12:00am",true },
                new object[] { "Alcoa Inc", 29.01, 0.42, 1.47, "9/1 12:00am",true },
                new object[] { "Altria Group Inc", 83.81, 0.28, 0.34, "9/1 12:00am",true },
                new object[] { "American Express Company", 52.55, 0.01, 0.02, "9/1 12:00am",false },
                new object[] { "American International Group, Inc.", 64.13, 0.31, 0.49, "9/1 12:00am",true },
                new object[] { "AT&T Inc.", 31.61, -0.48, -1.54, "9/1 12:00am",false }
            };
            }
        }
    }
}