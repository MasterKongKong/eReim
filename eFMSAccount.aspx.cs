using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using System.Data;

namespace eReimbursement
{
    public partial class eFMSAccount : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                this.Store1.DataSource = new object[]
            {
                new object[] { "ZJDBJS_2013010001", "交通费","Sales","Kevin Zhang","Kevin Zhang", "10800.00/10张", "2011-12-01", "报销单备注","草稿单据","Paul Lee" },
                new object[] { "ZJDBJS_2013010002", "差旅费","Sales","Hughson Huang","Kevin Zhang", "10800.00/10张", "2011-12-02", "报销单备注","草稿单据","Paul Lee" },
                new object[] { "ZJDBJS_2013010003", "差旅费","Sales","Kevin Zhang","Kevin Zhang", "10800.00/10张", "2011-12-02", "报销单备注","草稿单据","Paul Lee" },
                new object[] { "ZJDBJS_2013010004", "差旅费","MIS","Andy Kang","Andy Kang", "1800.00/12张", "2011-12-02", "报销单备注","草稿单据","Billy Yu" },
                new object[] { "ZJDBJS_2013010005", "差旅费","Sales","Kevin Zhang","Kevin Zhang", "1800.00/12张", "2011-12-02", "报销单备注","草稿单据","Billy Yu" },
                new object[] { "ZJDBJS_2013010006", "差旅费","Sales","Kevin Zhang","Kevin Zhang", "1800.00/12张", "2011-12-02", "报销单备注","草稿单据","Hughson Huang" },
                new object[] { "ZJDBJS_2013010007", "差旅费","Sales","Kevin Zhang","Kevin Zhang", "1800.00/12张", "2011-12-02", "报销单备注","草稿单据","Paul Lee" },
                new object[] { "ZJDBJS_2013010008", "差旅费","Sales","Kevin Zhang","Kevin Zhang", "1800.00/12张", "2011-12-02", "报销单备注","草稿单据","Paul Lee" },
                new object[] { "ZJDBJS_2013010009", "差旅费","Sales","Kevin Zhang","Kevin Zhang", "1800.00/12张", "2011-12-02", "报销单备注","草稿单据","Paul Lee" },
                new object[] { "ZJDBJS_2013010011", "差旅费","Sales","Kevin Zhang","Kevin Zhang", "1800.00/12张", "2011-12-02", "报销单备注","草稿单据","Paul Lee" },
                new object[] { "ZJDBJS_2013010010", "差旅费","Sales","Kevin Zhang","Kevin Zhang", "1800.00/12张", "2011-12-02", "报销单备注","草稿单据","Paul Lee" }
            };

                this.Store1.DataBind();
            //    this.Store2.DataSource = new object[]
            //{
            //    new object[] { "2011-12-01", "Tianjin Nanjing Road", "Beijing Beijing Road", "Conference", 57.1 },
            //    new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 27.21 },
            //    new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 38.65 },
            //    new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 57.2 },
            //    new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 57.2 },
            //    new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 57.2 },
            //    new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 357.2 },
            //    new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 257.2 },
            //    new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 157.2 },
            //    new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 527.2 },
            //    new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 1257.2 },
            //    new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 5327.2 },
            //    new object[] { "2011-12-02", "Tianjin1 Nanjing Road", "Beijing2 Beijing Road", "Conference", 57.2 }
            //};

            }
        }
        protected void ChangeGrid(object sender, DirectEventArgs e)
        {
            string ss = e.ExtraParams[0].Value;
            DataTable dt = new DataTable();
            DataRow dr;
            dt.Columns.Add("ID");
            dt.Columns.Add("DateT");
            dt.Columns.Add("From");
            dt.Columns.Add("To");
            dt.Columns.Add("Purpose");
            dt.Columns.Add("Amounts");
            for (int i = 0; i < 10; i++)
            {
                dr = dt.NewRow();
                dr[0] = i.ToString();
                dr[1] = "2012-08-1" + i.ToString();
                dr[2] = "Beijing" + ss;
                dr[3] = "Tianjin" + i.ToString();
                dr[4] = "吃饭" + i.ToString();
                dr[5] = "50.1" + i.ToString();
                dt.Rows.Add(dr);
            }
            Store2.RemoveAll();
            Store2.DataSource = dt;
            Store2.DataBind();
            //GridPanel2.ColumnModel.Columns[2].Header = "Column2";
            //GridPanel2.ColumnModel.Columns.RemoveAt(3);
            //GridPanel2.ColumnModel.Columns.Add(new Column()
            //{
            //    Header = "New Header",
            //    DataIndex = "To",
            //    Editor = { 
            //        new TextField ()
            //        {
            //            AllowBlank=false
            //        }
            //    }
            //});

            //GridPanel2.Reconfigure();
            //Ext.Net.TemplateColumn col1 = new Ext.Net.TemplateColumn();
            //col1.Header = "Change";
            //col1.Width = 75;
            //col1.Sortable = true;
            //col1.Template.ID = "Template3";
            //col1.Template.Html = @"<input id=""{Purpose}"" type=""text"" value=""{Purpose}"" style=""width: 100%""></input>";
            //GridPanel2.AddColumn(col1);
            //add Column
            //Column col = new Column();
            //col.Header = "Change %";
            //col.Width = 75;
            //col.Sortable = true;
            //col.DataIndex = "From";

            //ComboBox cb = new ComboBox();
            //cb.Items.Add(new Ext.Net.ListItem("1", "1"));
            //cb.Items.Add(new Ext.Net.ListItem("2", "2"));
            //cb.Items.Add(new Ext.Net.ListItem("3", "3"));
            //this.Controls.Add(cb);

            //col.Editor.Add(cb);

            //this.GridPanel2.AddColumn(col);

            //Ext.Net.TemplateColumn col = new Ext.Net.TemplateColumn();
            //col.Header = "Change";
            //col.Width = 75;
            //col.Sortable = true;
            //col.Template.ID = "Template5";
            //col.Template.Html = @"<input id=""{Purpose}"" type=""text"" value=""{Purpose}"" style=""width: 100%""></input>";
            //GridPanel2.AddColumn(col);
        }
    }
}