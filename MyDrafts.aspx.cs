using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;

namespace eReimbursement
{
    public partial class MyDrafts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                this.Store1.DataSource = new object[]
            {
                new object[] { "temp_2013010001", "通用费用", 10800.00, "2011-12-01", "报销单备注","草稿单据" },
                new object[] { "temp_2013010002", "差旅费", 10800.00, "2011-12-02", "报销单备注","草稿单据" },
                new object[] { "temp_2013010003", "通用费用", 10800.00, "2011-12-03", "报销单备注","草稿单据" },
                new object[] { "temp_2013010004", "差旅费", 10800.00, "2011-12-04", "报销单备注","草稿单据" },
                new object[] { "temp_2013010005", "差旅费", 10800.00, "2011-12-05", "报销单备注","草稿单据" },
                new object[] { "temp_2013010006", "通用费用", 10800.00, "2011-12-06", "报销单备注","草稿单据" },
                new object[] { "temp_2013010007", "通用费用", 10800.00, "2011-12-07", "报销单备注","草稿单据" },
                new object[] { "temp_2013010010", "通用费用", 10800.00, "2011-12-08", "报销单备注","草稿单据" },
                new object[] { "temp_2013010020", "通用费用", 10800.00, "2011-12-09", "报销单备注","草稿单据" },
                new object[] { "temp_2013010006", "通用费用", 10800.00, "2011-12-06", "报销单备注","草稿单据" },
                new object[] { "temp_2013010007", "通用费用", 10800.00, "2011-12-07", "报销单备注","草稿单据" },
                new object[] { "temp_2013010010", "通用费用", 10800.00, "2011-12-08", "报销单备注","草稿单据" },
                new object[] { "temp_2013010020", "通用费用", 10800.00, "2011-12-09", "报销单备注","草稿单据" }
            };

                this.Store1.DataBind();
            }
        }
    }
}