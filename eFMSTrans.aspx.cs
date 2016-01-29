﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
namespace eReimbursement
{
    public partial class eFMSTrans : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                this.Store1.DataSource = new object[]
            {
                new object[] { "ZJDBJS_2013010001", "11020201","招商银行","转账","Kevin Zhang", 10800.00, "2012-12-01", "Posted" },
                new object[] { "ZJDBJS_2013010002", "11020301","招商银行","转账","Kevin Zhang", 1800.00, "2012-12-01", "NotPosted" },
                new object[] { "ZJDBJS_2013010003", "12020101","招商银行","转账","Kevin Zhang", 1080.00, "2012-12-01", "Posted" },
                new object[] { "ZJDBJS_2013010004", "13021201","招商银行","转账","Kevin Zhang", 800.00, "2011-12-01", "Posted" },
                new object[] { "ZJDBJS_2013010005", "11020201","招商银行","转账","Kevin Zhang", 800.00, "2011-12-01", "Posted" },
                new object[] { "ZJDBJS_2013010006", "11020201","招商银行","转账","Kevin Zhang", 10800.00, "2011-12-01", "Posted" }
            };

                this.Store1.DataBind();
                this.Store2.DataSource = new object[]
            {
                new object[] { "2011-12-01", "Tianjin Nanjing Road", "Beijing Beijing Road", "Conference", 57.1 },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 27.21 },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 38.65 },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 57.2 },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 57.2 },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 57.2 },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 357.2 },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 257.2 },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 157.2 },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 527.2 },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 1257.2 },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 5327.2 },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 57.2 }
            };

                this.Store2.DataBind();
            }
        }
    }
}