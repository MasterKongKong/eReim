using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using System.Xml;
using System.Text;
namespace eReimbursement
{
    public partial class ExtTwoGrids : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                this.Store1.DataSource = new object[]
            {
                new object[] { "China" },
                new object[] { "USA" },
                new object[] { "Venan" },
                new object[] { "ENG" },
                new object[] { "JPN" }
            };

                this.Store1.DataBind();
            }
        }
        public class Country
        {
            public string Name { get; set; }
        }

        protected void SubmitData(object sender, StoreSubmitDataEventArgs e)
        {
            string json = e.Json;
            XmlNode xml = e.Xml;
            List<Country> countries = e.Object<Country>();

            StringBuilder sb = new StringBuilder(255);

            sb.Append("<h3>Selected Countries</h3>");

            foreach (Country country in countries)
            {
                sb.AppendFormat("{0}<br />", country.Name);
            }

            this.Label1.Html = sb.ToString();
        }
    }
}