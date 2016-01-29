using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;

namespace eReimbursement
{
    public partial class ApproveDG : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                this.Store1.DataSource = new object[]
            {
                new object[] { "交通费","2011-12-01", "Hughson Huang", "ZJDBJS", 57.1 },
                new object[] { "交际费","2012-12-01", "Hughson Huang", "ZJDBJS", 257.1 },
                new object[] { "其他费用","2013-02-01", "Hughson Huang", "ZJDBJS", 157.1 },
                new object[] { "通讯费","2011-12-01", "Hughson Huang", "ZJDBJS", 57.1 }
            };

                this.Store1.DataBind();
            }
        }
    }
}