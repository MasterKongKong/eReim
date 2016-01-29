using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
namespace eReimbursement
{
    public partial class Charge : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                this.Store1.DataSource = new object[]
            {
                new object[] { "通讯费", "Communication-In House", "62010501", true,"2013-01-16","Hughson Huang" },
                new object[] { "通讯费", "Communication-In House", "62010501", true,"2013-01-16","Hughson Huang" },
                new object[] { "通讯费", "Communication-In House", "62010501", false,"2013-01-16","Hughson Huang" },
                new object[] { "通讯费", "Communication-In House", "62010501", true,"2013-01-16","Hughson Huang" }
            };

                this.Store1.DataBind();
            }
        }
    }
}