﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
namespace eReimbursement
{
    public partial class ApplyTT : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                this.Store1.DataSource = new object[]
            {
                new object[] { "2011-12-01", "Tianjin Nanjing Road", "Beijing Beijing Road", "Conference", 57.1,"A.png" },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 27.21,"A.png" },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 38.65,"A.png" },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 57.2,"A.png" },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 57.2,"A.png" },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 57.2,"A.png" },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 357.2,"A.png" },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 257.2,"A.png" },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 157.2,"A.png" },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 527.2,"A.png" },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 1257.2,"A.png" },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 5327.2,"A.png" },
                new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 57.2,"A.png" }
            };

                this.Store1.DataBind();
            }
        }
    }
}