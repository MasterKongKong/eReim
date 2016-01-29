using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
namespace eReimbursement
{
    public partial class ApplyOthers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                this.Store1.DataSource = new object[]
                {
                    new object[] { "2011-12-01", "办公用品", "100.54", "买东西" },
                    new object[] { "2011-12-01", "办公用品", "100.54", "买东西" },
                    new object[] { "2011-12-01", "办公用品", "100.54", "买东西" },
                    new object[] { "2011-12-01", "办公用品", "100.54", "买东西" }
                };

                this.Store1.DataBind();
            }
        }
    }
}