<%@ Page Language="C#" %>

<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Xml.Serialization" %>
<%@ Import Namespace="System.ServiceModel" %>
<%@ Import Namespace="System.ServiceModel.Web" %>
<%@ Import Namespace="System.Runtime.Serialization.Json" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Data" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<script runat="server">
    static object Data = new object[]
    {
        new object[] { "1. Air Ticket - Int'l","10", "11", "12" },
        new object[] { "Domestic","20", "21", "22" },
        new object[] { "2. Hotel Bill","30", "31", "32" },
        new object[] { "3. Meals","30", "31", "32" },
        new object[] { "4. Entertainment","30", "31", "32" },
        new object[] { "5. Car Rental/Transportation","30", "31", "32" },
        new object[] { "6. Communication","30", "31", "32" },
        new object[] { "7. Local Trip","30", "31", "32" },
        new object[] { "8. Overseas Trip USD15/day","30", "31", "32" },
        new object[] { "9. Airport Tax/Travel Insurance","30", "31", "32" },
        new object[] { "10. Others","30", "31", "32" },
        new object[] { "Total","1000", "3000", "100" }
    };

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            //hdColCount.Value = "3";
            //Store store = this.GridPanel1.GetStore();
            //store.DataSource = Data;
            //store.DataBind();
            DataTable dt = new DataTable();
            Store1.Reader[0].Fields.Add("Category", RecordFieldType.String);
            dt.Columns.Add("Category", typeof(String));
            string fieldPName = "Station_0_P";
            Store1.Reader[0].Fields.Add(fieldPName, RecordFieldType.String);
            string fieldCName = "Station_0_C";
            Store1.Reader[0].Fields.Add(fieldCName, RecordFieldType.String);
            dt.Columns.Add("Station_0_P", typeof(String));
            dt.Columns.Add("Station_0_C", typeof(String));
            //
            Store1.Reader[0].Fields.Add("Station_1_P", RecordFieldType.String);
            Store1.Reader[0].Fields.Add("Station_1_C", RecordFieldType.String);
            dt.Columns.Add("Station_1_P", typeof(String));
            dt.Columns.Add("Station_1_C", typeof(String));

            //合计列
            Store1.Reader[0].Fields.Add("TotalP", RecordFieldType.String);
            Store1.Reader[0].Fields.Add("TotalC", RecordFieldType.String);
            dt.Columns.Add("TotalP", typeof(String));
            dt.Columns.Add("TotalC", typeof(String));

            DataRow dr = dt.NewRow();
            dr[0] = "1. Air Ticket - Int'l";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "Domestic";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "2. Hotel Bill";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "3. Meals";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "4. Entertainment";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "5. Car Rental/Transportation";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "6. Communication";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "7. Local Trip";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "8. Overseas Trip USD15/day";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "9. Airport Tax/Travel Insurance";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "10. Others";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "Total";
            dt.Rows.Add(dr);

            Store1.DataSource = dt;
            Store1.DataBind();




            var TitleCol = new Column();
            TitleCol.DataIndex = "Category";
            TitleCol.Sortable = false;
            TitleCol.Resizable = false;
            TitleCol.MenuDisabled = true;
            TitleCol.Width = 180;
            this.GridPanel1.ColumnModel.Columns.Add(TitleCol);

            var txtP = new TextField();
            //txtP.Listeners.Blur.Fn = "Cal";
            //txtP.Listeners.Blur.Delay = 50;
            var colP = new Column();
            colP.DataIndex = fieldPName;
            colP.Sortable = false;
            colP.Resizable = false;
            colP.MenuDisabled = true;
            colP.Width = 110;
            colP.Locked = true;
            colP.Editor.Add(txtP);
            this.GridPanel1.ColumnModel.Columns.Add(colP);

            var txtC = new TextField();
            //txtC.Listeners.Blur.Fn = "Cal";
            //txtC.Listeners.Blur.Delay = 50;
            var colC = new Column();
            colC.DataIndex = fieldCName;
            colC.Sortable = false;
            colC.Resizable = false;
            colC.MenuDisabled = true;
            colC.Width = 110;
            colP.Locked = true;
            colC.Editor.Add(txtC);
            this.GridPanel1.ColumnModel.Columns.Add(colC);
            //
            var txtP1 = new TextField();
            //txtP1.Listeners.Blur.Fn = "Cal";
            //txtP1.Listeners.Blur.Delay = 50;
            var colP1 = new Column();
            colP1.DataIndex = "Station_1_P";
            colP1.Sortable = false;
            colP1.Resizable = false;
            colP1.MenuDisabled = true;
            colP1.Width = 110;
            colP1.Locked = true;
            colP1.Editor.Add(txtP1);
            this.GridPanel1.ColumnModel.Columns.Add(colP1);

            var txtC1 = new TextField();
            //txtC1.Listeners.Blur.Fn = "Cal";
            //txtC1.Listeners.Blur.Delay = 50;
            var colC1 = new Column();
            colC1.DataIndex = "Station_1_C";
            colC1.Sortable = false;
            colC1.Resizable = false;
            colC1.MenuDisabled = true;
            colC1.Width = 110;
            colC1.Locked = true;
            colC1.Editor.Add(txtC1);
            this.GridPanel1.ColumnModel.Columns.Add(colC1);
            //
            var TotalP = new TextField();
            TotalP.ReadOnly = true;
            var colTotalP = new Column();
            colTotalP.DataIndex = "TotalP";
            colTotalP.Sortable = false;
            colTotalP.Resizable = false;
            colTotalP.MenuDisabled = true;
            colTotalP.Width = 110;
            colTotalP.Locked = true;
            colTotalP.Editor.Add(TotalP);
            this.GridPanel1.ColumnModel.Columns.Add(colTotalP);

            var TotalC = new TextField();
            TotalC.ReadOnly = true;
            var colTotalC = new Column();
            colTotalC.DataIndex = "TotalC";
            colTotalC.Sortable = false;
            colTotalC.Resizable = false;
            colTotalC.MenuDisabled = true;
            colTotalC.Width = 110;
            colTotalC.Locked = true;
            colTotalC.Editor.Add(TotalC);
            this.GridPanel1.ColumnModel.Columns.Add(colTotalC);

            var Title1 = new Ext.Net.Label();
            Title1.Text = "Station:";
            HeaderColumn hcTitle1 = new HeaderColumn();
            hcTitle1.Component.Add(Title1);
            this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcTitle1);

            var Station = new TextField();
            Station.Text = "CNTSN";
            HeaderColumn hcStation = new HeaderColumn();
            hcStation.Component.Add(Station);
            this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcStation);

            var Button = new Ext.Net.Button();
            Button.Text = "Remove";
            Button.Listeners.Click.Handler = "removecol(this,0);";
            Button.Listeners.Click.Delay = 50;
            HeaderColumn hcButton = new HeaderColumn();
            hcButton.Component.Add(Button);
            this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcButton);
            //
            var Station1 = new TextField();
            Station1.Text = "CNTSN1";
            HeaderColumn hcStation1 = new HeaderColumn();
            hcStation1.Component.Add(Station1);
            this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcStation1);

            var Button1 = new Ext.Net.Button();
            Button1.Text = "Remove";
            Button1.Listeners.Click.Handler = "removecol(this,1);";
            Button1.Listeners.Click.Delay = 50;
            HeaderColumn hcButton1 = new HeaderColumn();
            hcButton1.Component.Add(Button1);
            this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcButton1);
            //
            HeaderColumn hcTotal1 = new HeaderColumn();
            this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcTotal1);

            HeaderColumn hcTotal2 = new HeaderColumn();
            this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcTotal2);


            var Title2 = new Ext.Net.Label();
            Title2.Text = "Cost Center:";
            HeaderColumn hcTitle2 = new HeaderColumn();
            hcTitle2.Component.Add(Title2);
            this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcTitle2);

            var CostCenter = new TextField();
            CostCenter.Text = "CNTSN";
            HeaderColumn hcCostCenter = new HeaderColumn();
            hcCostCenter.Component.Add(CostCenter);
            this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcCostCenter);

            //HeaderColumn hcCostCenter1 = new HeaderColumn();
            //this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcCostCenter1);
            var ButtonGetSum = new Ext.Net.Button();
            ButtonGetSum.Text = "Calculate";
            ButtonGetSum.Listeners.Click.Handler = "GetSum();";
            ButtonGetSum.Listeners.Click.Delay = 50;
            HeaderColumn hcButtonGetSum = new HeaderColumn();
            hcButtonGetSum.Component.Add(ButtonGetSum);
            this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcButtonGetSum);
            //
            var CostCenter0 = new TextField();
            CostCenter0.Text = "CNTSN1";
            HeaderColumn hcCostCenter0 = new HeaderColumn();
            hcCostCenter0.Component.Add(CostCenter0);
            this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcCostCenter0);

            //HeaderColumn hcCostCenter01 = new HeaderColumn();
            //this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcCostCenter01);
            var ButtonGetSum1 = new Ext.Net.Button();
            ButtonGetSum1.Text = "Calculate";
            ButtonGetSum1.Listeners.Click.Handler = "GetSum();";
            ButtonGetSum1.Listeners.Click.Delay = 50;
            HeaderColumn hcButtonGetSum1 = new HeaderColumn();
            hcButtonGetSum1.Component.Add(ButtonGetSum);
            this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcButtonGetSum1);
            //
            HeaderColumn hcTotal3 = new HeaderColumn();
            this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcTotal3);

            HeaderColumn hcTotal4 = new HeaderColumn();
            this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcTotal4);

            var Title3 = new Ext.Net.Label();
            Title3.Text = "Travel Period:";
            HeaderColumn hcTitle3 = new HeaderColumn();
            hcTitle3.Component.Add(Title3);
            this.GridPanel1.GetView().HeaderRows[2].Columns.Add(hcTitle3);

            HeaderColumn hcDate1 = new HeaderColumn();
            var Date1 = new DateField();
            Date1.EmptyText = "yyyy/M/d";
            Date1.SetValue("2013/11/1");
            hcDate1.Component.Add(Date1);
            this.GridPanel1.GetView().HeaderRows[2].Columns.Add(hcDate1);

            HeaderColumn hcDate2 = new HeaderColumn();
            var Date2 = new DateField();
            Date2.EmptyText = "yyyy/M/d";
            Date2.SetValue("2013/11/2");
            hcDate2.Component.Add(Date2);
            this.GridPanel1.GetView().HeaderRows[2].Columns.Add(hcDate2);
            //
            HeaderColumn hcDate3 = new HeaderColumn();
            var Date3 = new DateField();
            Date3.EmptyText = "yyyy/M/d";
            Date3.SetValue("2013/11/1");
            hcDate3.Component.Add(Date3);
            this.GridPanel1.GetView().HeaderRows[2].Columns.Add(hcDate3);

            HeaderColumn hcDate4 = new HeaderColumn();
            var Date4 = new DateField();
            Date4.EmptyText = "yyyy/M/d";
            Date4.SetValue("2013/11/2");
            hcDate4.Component.Add(Date4);
            this.GridPanel1.GetView().HeaderRows[2].Columns.Add(hcDate4);
            //

            //HeaderColumn hcTotal5 = new HeaderColumn();
            //this.GridPanel1.GetView().HeaderRows[2].Columns.Add(hcTotal5);

            //HeaderColumn hcTotal6 = new HeaderColumn();
            //this.GridPanel1.GetView().HeaderRows[2].Columns.Add(hcTotal6);

            var TitleTotalP = new Ext.Net.Label();
            TitleTotalP.Text = "Reimbursement";
            HeaderColumn hcTitleTotalP = new HeaderColumn();
            hcTitleTotalP.Component.Add(TitleTotalP);
            this.GridPanel1.GetView().HeaderRows[2].Columns.Add(hcTitleTotalP);

            var TitleTotalC = new Ext.Net.Label();
            TitleTotalC.Text = "Company Paid";
            HeaderColumn hcTitleTotalC = new HeaderColumn();
            hcTitleTotalC.Component.Add(TitleTotalC);
            this.GridPanel1.GetView().HeaderRows[2].Columns.Add(hcTitleTotalC);

            //GridPanel1.Render();
        }
    }
    [DirectMethod]
    public void AddCol(string StoreData, string header0string, string header1string, string header2string)
    {
        StoreSubmitDataEventArgs eSubmit = new StoreSubmitDataEventArgs(StoreData, null);
        XmlNode xml = eSubmit.Xml;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml.InnerXml);
        int dtcol = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes.Count;
        int colc = (dtcol - 3) / 2;
        DataTable dt = new DataTable();

        Store1.Reader[0].Fields.Add("Category", RecordFieldType.String);
        dt.Columns.Add(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[0].Name, typeof(String));
        for (int i = 0; i < (dtcol - 3) / 2; i++)
        {
            Store1.Reader[0].Fields.Add("Station_" + i.ToString() + "_P", RecordFieldType.String);
            Store1.Reader[0].Fields.Add("Station_" + i.ToString() + "_C", RecordFieldType.String);
            dt.Columns.Add("Station_" + i.ToString() + "_P", typeof(String));
            dt.Columns.Add("Station_" + i.ToString() + "_C", typeof(String));
        }
        string fieldPNameNew = "Station_" + colc.ToString() + "_P";
        Store1.Reader[0].Fields.Add(fieldPNameNew, RecordFieldType.String);
        string fieldCNameNew = "Station_" + colc.ToString() + "_C";
        Store1.Reader[0].Fields.Add(fieldCNameNew, RecordFieldType.String);
        dt.Columns.Add(fieldPNameNew, typeof(String));
        dt.Columns.Add(fieldCNameNew, typeof(String));
        //合计列
        Store1.Reader[0].Fields.Add("TotalP", RecordFieldType.String);
        Store1.Reader[0].Fields.Add("TotalC", RecordFieldType.String);
        dt.Columns.Add("TotalP", typeof(String));
        dt.Columns.Add("TotalC", typeof(String));
        //decimal row0Psum = 0, row0Csum = 0, row1Psum = 0, row1Csum = 0, row2Psum = 0, row2Csum = 0, row3Psum = 0, row3Csum = 0, row4Psum = 0, row4Csum = 0, row5Psum = 0, row5Csum = 0, row6Psum = 0, row6Csum = 0, row7Psum = 0, row7Csum = 0, row8Psum = 0, row8Csum = 0, row9Psum = 0, row9Csum = 0, row10Psum = 0, row10Csum = 0;

        for (int i = 0; i < doc.SelectNodes("records").Item(0).SelectNodes("record").Count - 1; i++)
        {
            DataRow dr = dt.NewRow();
            dt.Rows.Add(dr);
            for (int j = 0; j < dtcol - 3; j++)
            {
                string wr = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).ChildNodes[j].InnerText;
                dt.Rows[i][j] = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).ChildNodes[j].InnerText;
            }
        }
        dt.Rows.Add(dt.NewRow());
        dt.Rows[11][0] = "Total";
        for (int i = 0; i < 11; i++)
        {
            for (int j = 1; j < dtcol - 3; j++)
            {
                if (j % 2 == 1)
                {
                    dt.Rows[i][dtcol - 1] = (Convert.ToDecimal(dt.Rows[i][dtcol - 1].ToString() == "" ? "0" : dt.Rows[i][dtcol - 1].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString() == "0" ? "" : (Convert.ToDecimal(dt.Rows[i][dtcol - 1].ToString() == "" ? "0" : dt.Rows[i][dtcol - 1].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString();
                    dt.Rows[11][dtcol - 1] = (Convert.ToDecimal(dt.Rows[11][dtcol - 1].ToString() == "" ? "0" : dt.Rows[11][dtcol - 1].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString() == "0" ? "" : (Convert.ToDecimal(dt.Rows[11][dtcol - 1].ToString() == "" ? "0" : dt.Rows[11][dtcol - 1].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString();
                }
                else
                {
                    dt.Rows[i][dtcol] = (Convert.ToDecimal(dt.Rows[i][dtcol].ToString() == "" ? "0" : dt.Rows[i][dtcol].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString() == "0" ? "" : (Convert.ToDecimal(dt.Rows[i][dtcol].ToString() == "" ? "0" : dt.Rows[i][dtcol].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString();
                    dt.Rows[11][dtcol] = (Convert.ToDecimal(dt.Rows[11][dtcol].ToString() == "" ? "0" : dt.Rows[11][dtcol].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString() == "0" ? "" : (Convert.ToDecimal(dt.Rows[11][dtcol].ToString() == "" ? "0" : dt.Rows[11][dtcol].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString();
                }
                dt.Rows[11][j] = (Convert.ToDecimal(dt.Rows[11][j].ToString() == "" ? "0" : dt.Rows[11][j].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString() == "0" ? "" : (Convert.ToDecimal(dt.Rows[11][j].ToString() == "" ? "0" : dt.Rows[11][j].ToString()) + Convert.ToDecimal(dt.Rows[i][j].ToString() == "" ? "0" : dt.Rows[i][j].ToString())).ToString();
            }
        }

        Store1.DataSource = dt;
        Store1.DataBind();

        var TitleCol = new Column();
        TitleCol.DataIndex = "Category";
        TitleCol.Sortable = false;
        TitleCol.Resizable = false;
        TitleCol.MenuDisabled = true;
        TitleCol.Width = 180;
        this.GridPanel1.ColumnModel.Columns.Add(TitleCol);

        var Title1 = new Ext.Net.Label();
        Title1.Text = "Station:";
        HeaderColumn hcTitle1 = new HeaderColumn();
        hcTitle1.Component.Add(Title1);
        this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcTitle1);

        var Title2 = new Ext.Net.Label();
        Title2.Text = "Cost Center:";
        HeaderColumn hcTitle2 = new HeaderColumn();
        hcTitle2.Component.Add(Title2);
        this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcTitle2);

        var Title3 = new Ext.Net.Label();
        Title3.Text = "Travel Period:";
        HeaderColumn hcTitle3 = new HeaderColumn();
        hcTitle3.Component.Add(Title3);
        this.GridPanel1.GetView().HeaderRows[2].Columns.Add(hcTitle3);

        for (int i = 0; i < colc; i++)//准备复制已有信息
        {
            string fieldPName = "Station_" + i.ToString() + "_P";
            //RecordField field1 = new RecordField(fieldAName, RecordFieldType.Float);
            //Store1.Reader[0].Fields.Add(fieldPName, RecordFieldType.Float);
            //this.Store1.AddField(field1, columncount);
            string fieldCName = "Station_" + i.ToString() + "_C";
            //RecordField field1 = new RecordField(fieldAName, RecordFieldType.Float);
            //Store1.Reader[0].Fields.Add(fieldCName, RecordFieldType.Float);

            var txtP = new TextField();
            //txtP.Listeners.Blur.Fn = "Cal";
            var colP = new Column();
            colP.DataIndex = fieldPName;
            colP.Sortable = false;
            colP.Resizable = false;
            colP.MenuDisabled = true;
            colP.Width = 110;
            colP.Editor.Add(txtP);
            this.GridPanel1.ColumnModel.Columns.Add(colP);

            var txtC = new TextField();
            //txtC.Listeners.Blur.Fn = "Cal";
            var colC = new Column();
            colC.DataIndex = fieldCName;
            colC.Sortable = false;
            colC.Resizable = false;
            colC.MenuDisabled = true;
            colC.Width = 110;
            colC.Editor.Add(txtC);
            this.GridPanel1.ColumnModel.Columns.Add(colC);

            var Station = new TextField();
            if (header0string.Split(',')[i] != "NA")
            {
                Station.Text = header0string.Split(',')[i];
            }
            HeaderColumn hcStation = new HeaderColumn();
            hcStation.Component.Add(Station);
            this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcStation);

            var Button = new Ext.Net.Button();
            Button.Text = "Remove";
            Button.Listeners.Click.Handler = "removecol(this," + i.ToString() + ");";
            Button.Listeners.Click.Delay = 50;
            HeaderColumn hcButton = new HeaderColumn();
            hcButton.Component.Add(Button);
            this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcButton);

            var CostCenter = new TextField();
            if (header1string.Split(',')[i] != "NA")
            {
                CostCenter.Text = header1string.Split(',')[i];
            }
            HeaderColumn hcCostCenter = new HeaderColumn();
            hcCostCenter.Component.Add(CostCenter);
            this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcCostCenter);

            //HeaderColumn hcCostCenter1 = new HeaderColumn();
            //this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcCostCenter1);
            var ButtonGetSum = new Ext.Net.Button();
            ButtonGetSum.Text = "Calculate";
            ButtonGetSum.Listeners.Click.Handler = "GetSum();";
            ButtonGetSum.Listeners.Click.Delay = 50;
            HeaderColumn hcButtonGetSum = new HeaderColumn();
            hcButtonGetSum.Component.Add(ButtonGetSum);
            this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcButtonGetSum);

            var datefrom = new DateField();
            if (header2string.Split(',')[i * 2] != "NA")
            {
                datefrom.SetValue(header2string.Split(',')[i * 2]);
            }
            HeaderColumn Date1 = new HeaderColumn();
            Date1.Component.Add(datefrom);
            this.GridPanel1.GetView().HeaderRows[2].Columns.Add(Date1);

            var dateto = new DateField();
            if (header2string.Split(',')[i * 2 + 1] != "NA")
            {
                dateto.SetValue(header2string.Split(',')[i * 2 + 1]);
            }
            HeaderColumn Date2 = new HeaderColumn();
            Date2.Component.Add(dateto);
            this.GridPanel1.GetView().HeaderRows[2].Columns.Add(Date2);
        }



        var txtPNew = new TextField();
        //txtPNew.Listeners.Blur.Fn = "Cal";
        var colPNew = new Column();
        colPNew.DataIndex = fieldPNameNew;
        colPNew.Sortable = false;
        colPNew.Resizable = false;
        colPNew.MenuDisabled = true;
        colPNew.Width = 110;
        colPNew.Editor.Add(txtPNew);
        this.GridPanel1.ColumnModel.Columns.Add(colPNew);

        var txtCNew = new TextField();
        //txtCNew.Listeners.Blur.Fn = "Cal";
        var colCNew = new Column();
        colCNew.DataIndex = fieldCNameNew;
        colCNew.Sortable = false;
        colCNew.Resizable = false;
        colCNew.MenuDisabled = true;
        colCNew.Width = 110;
        colCNew.Editor.Add(txtCNew);
        this.GridPanel1.ColumnModel.Columns.Add(colCNew);

        var TotalP = new TextField();
        TotalP.ReadOnly = true;
        var colTotalP = new Column();
        colTotalP.DataIndex = "TotalP";
        colTotalP.Sortable = false;
        colTotalP.Resizable = false;
        colTotalP.MenuDisabled = true;
        colTotalP.Width = 110;
        colTotalP.Locked = true;
        colTotalP.Editor.Add(TotalP);
        this.GridPanel1.ColumnModel.Columns.Add(colTotalP);

        var TotalC = new TextField();
        TotalC.ReadOnly = true;
        var colTotalC = new Column();
        colTotalC.DataIndex = "TotalC";
        colTotalC.Sortable = false;
        colTotalC.Resizable = false;
        colTotalC.MenuDisabled = true;
        colTotalC.Width = 110;
        colTotalC.Locked = true;
        colTotalC.Editor.Add(TotalC);
        this.GridPanel1.ColumnModel.Columns.Add(colTotalC);

        var StationNew = new TextField();
        HeaderColumn hcStationNew = new HeaderColumn();
        hcStationNew.Component.Add(StationNew);
        this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcStationNew);

        var ButtonNew = new Ext.Net.Button();
        ButtonNew.Text = "Remove";
        ButtonNew.Listeners.Click.Handler = "removecol(this," + colc.ToString() + ");";
        ButtonNew.Listeners.Click.Delay = 50;
        HeaderColumn hcButtonNew = new HeaderColumn();
        hcButtonNew.Component.Add(ButtonNew);
        this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcButtonNew);

        HeaderColumn hcTotal1 = new HeaderColumn();
        this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcTotal1);

        HeaderColumn hcTotal2 = new HeaderColumn();
        this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcTotal2);

        var CostCenterNew = new TextField();
        HeaderColumn hcCostCenterNew = new HeaderColumn();
        hcCostCenterNew.Component.Add(CostCenterNew);
        this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcCostCenterNew);

        //HeaderColumn hcCostCenter1New = new HeaderColumn();
        //this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcCostCenter1New);
        var ButtonGetSumNew = new Ext.Net.Button();
        ButtonGetSumNew.Text = "Calculate";
        ButtonGetSumNew.Listeners.Click.Handler = "GetSum();";
        ButtonGetSumNew.Listeners.Click.Delay = 50;
        HeaderColumn hcButtonGetSumNew = new HeaderColumn();
        hcButtonGetSumNew.Component.Add(ButtonGetSumNew);
        this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcButtonGetSumNew);

        HeaderColumn hcTotal3 = new HeaderColumn();
        this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcTotal3);

        HeaderColumn hcTotal4 = new HeaderColumn();
        this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcTotal4);

        HeaderColumn Date1New = new HeaderColumn();
        Date1New.Component.Add(new DateField());
        this.GridPanel1.GetView().HeaderRows[2].Columns.Add(Date1New);

        HeaderColumn Date2New = new HeaderColumn();
        Date2New.Component.Add(new DateField());
        this.GridPanel1.GetView().HeaderRows[2].Columns.Add(Date2New);

        var TitleTotalP = new Ext.Net.Label();
        TitleTotalP.Text = "Reimbursement";
        HeaderColumn hcTitleTotalP = new HeaderColumn();
        hcTitleTotalP.Component.Add(TitleTotalP);
        this.GridPanel1.GetView().HeaderRows[2].Columns.Add(hcTitleTotalP);

        var TitleTotalC = new Ext.Net.Label();
        TitleTotalC.Text = "Company Paid";
        HeaderColumn hcTitleTotalC = new HeaderColumn();
        hcTitleTotalC.Component.Add(TitleTotalC);
        this.GridPanel1.GetView().HeaderRows[2].Columns.Add(hcTitleTotalC);

        //HeaderColumn hcTotal5 = new HeaderColumn();
        //this.GridPanel1.GetView().HeaderRows[2].Columns.Add(hcTotal5);

        //HeaderColumn hcTotal6 = new HeaderColumn();
        //this.GridPanel1.GetView().HeaderRows[2].Columns.Add(hcTotal6);

        this.GridPanel1.Render();
        //Store store2 = this.GridPanel1.GetStore();



        //this.GridPanel1.RefreshView();
        GridPanel1.Reconfigure();
    }
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
    <title>Ext.Net Example</title>
    <script type="text/javascript">
        var Cal = function (a) {
            var af = a;
//            GetSum();
        };
        var GetSum = function () {
            Store1.getAllRange(11, 11)[0].set('TotalP', '');
            Store1.getAllRange(11, 11)[0].set('TotalC', '');
            var jh = Store1.getAllRange(11, 11)[0].data;
            for (var o in jh) {
                if ((o.indexOf('Station_') != -1) && ((o.indexOf('_P') != -1) || (o.indexOf('_C') != -1))) {
                    Store1.getAllRange(11, 11)[0].set(o, '');
                }
            }
            var totalp = 0, totalc = 0;
            for (var i = 0; i < Store1.data.length - 1; i++) {
                Store1.getAllRange(i, i)[0].set('TotalP', '');
                Store1.getAllRange(i, i)[0].set('TotalC', '');
                var item = Store1.getAllRange(i, i)[0].data;
                for (var o in item) {
                    if (o.indexOf('Station_') != -1 && o.indexOf('_P') != -1) {
                        eval('tb=Store1.getAllRange(i,i)[0].data.' + o);
                        tb = tb == '' ? '0' : tb;
                        var tc = parseFloat(tb);
                        var ta = parseFloat(Store1.getAllRange(i, i)[0].data.TotalP == '' ? '0' : Store1.getAllRange(i, i)[0].data.TotalP);
                        var sum = ta + tc == 0 ? "" : (ta + tc).toString();
                        Store1.getAllRange(i, i)[0].set('TotalP', sum);

                        eval('t11=Store1.getAllRange(11,11)[0].data.' + o);
                        t11 = t11 == '' ? '0' : t11;
                        var t11c = parseFloat(t11);
                        var sum11 = t11c + tc == 0 ? "" : (t11c + tc).toString();
                        Store1.getAllRange(11, 11)[0].set(o, sum11);

                        totalp += tc;

                    }
                    if (o.indexOf('Station_') != -1 && o.indexOf('_C') != -1) {
                        eval('tb=Store1.getAllRange(i,i)[0].data.' + o);
                        tb = tb == '' ? '0' : tb;
                        var tc = parseFloat(tb);
                        var ta = parseFloat(Store1.getAllRange(i, i)[0].data.TotalC == '' ? '0' : Store1.getAllRange(i, i)[0].data.TotalC);
                        var sum = ta + tc == 0 ? "" : (ta + tc).toString();
                        Store1.getAllRange(i, i)[0].set('TotalC', sum);

                        eval('t11=Store1.getAllRange(11,11)[0].data.' + o);
                        t11 = t11 == '' ? '0' : t11;
                        var t11c = parseFloat(t11);
                        var sum11 = t11c + tc == 0 ? "" : (t11c + tc).toString();
                        Store1.getAllRange(11, 11)[0].set(o, sum11);

                        totalc += tc;
                    }
                }
            }
            Store1.getAllRange(11, 11)[0].set('TotalP', totalp == 0 ? '' : totalp);
            Store1.getAllRange(11, 11)[0].set('TotalC', totalc == 0 ? '' : totalc);
        };
        var removecol = function (a, b) {
            var ds = b * 2 + 1;
            //            GridPanel1.removeColumn(ds);
            //            GridPanel1.removeColumn(ds);
            GridPanel1.colModel.setHidden(ds, true);
            GridPanel1.colModel.setHidden(ds + 1, true);
            var field1 = 'Station_' + b.toString() + '_P';
            var field2 = 'Station_' + b.toString() + '_C';
            Store1.removeField(field1);
            Store1.removeField(field2);
            //计算合计
            GetSum();
            //            eval('b' + o + '=1235');eval('b=Store1.getAllRange(1,1)[0].data.'+o)
            //Store3.getAllRange(i, i)[0].set('Type', '1');

            //            for (var item in Store1.getAllRange()) {
            //                var da = item.data;
            //                for (var o in da) {
            //                    var e = o;
            //                }
            //            }
            //            Store1.getAllRange(0, 0)[0].set('TotalP', '100')

            //记录隐藏的列
            //            var dc = hdDelCol.getValue().toString() == '' ? b.toString() : (hdDelCol.getValue() + ',' + b.toString());
            //            hdDelCol.setValue(dc);
        };
        var ClearCol = function (a, b, c) {
            //            Store1.removeField('TotalP');
            //            Store1.removeField('TotalC');
            var headercol = GridPanel1.view.headerRows[0].columns.length;
            var col = GridView1.cm.columns.length;
            var colnew = GridView1.cm.columns.length;
            //            GridPanel1.clearContent();
            var header0string = ''; var header1string = ''; var header2string = '';
            for (var i = 1; i < col - 2; i++) {
                if (GridView1.cm.columns[i].hidden) {
                    colnew--;
                }
                else {
                    if (i % 2 == 1) {
                        //                        var h0 = 'NA';
                        //                        var h1 = 'NA';
                        var h0 = GridPanel1.view.headerRows[0].columns[i].component.getValue() == '' ? 'NA' : GridPanel1.view.headerRows[0].columns[i].component.getValue();
                        var h1 = GridPanel1.view.headerRows[1].columns[i].component.getValue() == '' ? 'NA' : GridPanel1.view.headerRows[1].columns[i].component.getValue();
                        header0string += (header0string != '') ? (',' + h0) : h0;
                        header1string += (header1string != '') ? (',' + h1) : h1;
                    }
                    //                    var h2 = 'NA';
                    var h2 = GridPanel1.view.headerRows[2].columns[i].component.getRawValue() == '' ? 'NA' : GridPanel1.view.headerRows[2].columns[i].component.getRawValue();
                    header2string += (header2string != '') ? (',' + h2) : h2;
                }
            }

            var detail = Ext.encode(GridPanel1.getRowsValues()); //记录Store数据为json格式
            //            for (var i = 0; i < Store1.getAllRange().length; i++) {
            //                var record = Store1.getAllRange()[i].data;
            //                detail += Ext.encode(record);
            //                if (i != Store1.getAllRange().length - 1) {
            //                    detail += ',';
            //                }
            //            };
            //            detail = '[' + detail + ']';
            //            hdData.setValue(detail);

            hdColCount.setValue(colnew.toString());
            GridPanel1.setWidth(GridPanel1.getWidth() + 200);
            RM.AddCol(detail, header0string, header1string, header2string, {
                eventMask: {
                    showMask: true,
                    tartget: "Page"
                },
                timeout: 300000
            });
        };
        var SelectCell = function (a, row, column) {
            var sr = a;
            if (row == 11 || column == 0 || column == GridView1.cm.columns.length - 1 || column == GridView1.cm.columns.length - 2) {
                return false;
            }
            if (column % 2 == 1) {
                var station = GridView1.headerRows[0].columns[column].component.getValue();
                var costcenter = GridView1.headerRows[1].columns[column].component.getValue();
                var costtype = a.selection.record.data.Category;
            }
            else {
                var station = GridView1.headerRows[0].columns[column - 1].component.getValue();
                var costcenter = GridView1.headerRows[1].columns[column - 1].component.getValue();
                var costtype = a.selection.record.data.Category;
            }
        };
        var MouseOver = function (a, b, c, d, e) {
            var wr = a;
        };
    </script>
    <style type="text/css">
    </style>
</head>
<body>
    <form id="Form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" DirectMethodNamespace="RM" />
    <%--<input id="Text1" type="text" readonly="readonly" />--%>
    <ext:Hidden ID="hdColCount" runat="server">
    </ext:Hidden>
    <ext:Hidden ID="hdDelCol" runat="server">
    </ext:Hidden>
    <ext:Hidden ID="hdData" runat="server">
    </ext:Hidden>
    <ext:Panel ID="Panel1" runat="server" Title="Grid">
        <Items>
            <ext:GridPanel ID="GridPanel1" runat="server" Height="500">
                <Store>
                    <ext:Store ID="Store1" runat="server">
                        <Reader>
                            <ext:JsonReader>
                                <Fields>
                                    <%--<ext:RecordField Name="Category" />
                                    <ext:RecordField Name="Station_0_P" />
                                    <ext:RecordField Name="Station_0_C" />--%>
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <Plugins>
                    <ext:EditableGrid ID="EditableGrid1" runat="server" />
                </Plugins>
                <SelectionModel>
                    <ext:CellSelectionModel ID="CellSelectionModel1" runat="server">
                        <Listeners>
                            <CellSelect Fn="SelectCell" />
                        </Listeners>
                    </ext:CellSelectionModel>
                </SelectionModel>
                <ColumnModel ID="ColumnModel1" runat="server">
                    <Columns>
                        <%--<ext:Column Header="" DataIndex="Category" MenuDisabled="true" Sortable="false" Resizable="False"
                            Width="180px" />
                        <ext:Column Header="" DataIndex="Station_0_P" MenuDisabled="true" Sortable="false"
                            Resizable="False">
                            <Editor>
                                <ext:TextField ID="TextField3" runat="server" AllowBlank="false">
                                    <Listeners>
                                        <Blur Fn="Cal" />
                                    </Listeners>
                                </ext:TextField>
                            </Editor>
                        </ext:Column>
                        <ext:Column Header="" DataIndex="Station_0_C" MenuDisabled="true" Sortable="false"
                            Resizable="False">
                            <Editor>
                                <ext:TextField ID="TextField4" runat="server" AllowBlank="false">
                                    <Listeners>
                                        <Blur Fn="Cal" />
                                    </Listeners>
                                </ext:TextField>
                            </Editor>
                        </ext:Column>--%>
                    </Columns>
                </ColumnModel>
                <View>
                    <ext:GridView ID="GridView1" runat="server">
                        <HeaderRows>
                            <ext:HeaderRow>
                                <Columns>
                                    <%--<ext:HeaderColumn>
                                        <Component>
                                            <ext:Label ID="Label2" runat="server" Text="Station:" />
                                        </Component>
                                    </ext:HeaderColumn>
                                    <ext:HeaderColumn>
                                        <Component>
                                            <ext:TextField ID="TextField1" runat="server" Text="CNBJS" />
                                        </Component>
                                    </ext:HeaderColumn>
                                    <ext:HeaderColumn>
                                        <Component>
                                            <ext:Button ID="Button2" runat="server" Text="Remove">
                                                <Listeners>
                                                    <Click Fn="removecol" />
                                                </Listeners>
                                            </ext:Button>
                                        </Component>
                                    </ext:HeaderColumn>--%>
                                </Columns>
                            </ext:HeaderRow>
                            <ext:HeaderRow>
                                <Columns>
                                    <%--<ext:HeaderColumn>
                                        <Component>
                                            <ext:Label ID="Label1" runat="server" Text="Cost Center:" />
                                        </Component>
                                    </ext:HeaderColumn>
                                    <ext:HeaderColumn>
                                        <Component>
                                            <ext:TextField ID="TextField2" runat="server" Text="CNTSN" />
                                        </Component>
                                    </ext:HeaderColumn>
                                    <ext:HeaderColumn>
                                    </ext:HeaderColumn>--%>
                                </Columns>
                            </ext:HeaderRow>
                            <ext:HeaderRow>
                                <Columns>
                                    <%--<ext:HeaderColumn>
                                        <Component>
                                            <ext:Label ID="Label3" runat="server" Text="Travel Period:" />
                                        </Component>
                                    </ext:HeaderColumn>
                                    <ext:HeaderColumn>
                                        <Component>
                                            <ext:DateField ID="DateField1" runat="server">
                                            </ext:DateField>
                                        </Component>
                                    </ext:HeaderColumn>
                                    <ext:HeaderColumn>
                                        <Component>
                                            <ext:DateField ID="DateField2" runat="server">
                                            </ext:DateField>
                                        </Component>
                                    </ext:HeaderColumn>--%>
                                </Columns>
                            </ext:HeaderRow>
                        </HeaderRows>
                    </ext:GridView>
                </View>
            </ext:GridPanel>
        </Items>
    </ext:Panel>
    <ext:Button ID="Button1" runat="server" Text="Add New DSTN">
        <Listeners>
            <Click Fn="ClearCol" Delay="50" />
        </Listeners>
        <%--<DirectEvents>
            <Click OnEvent="AddColumn" Timeout="300000">
                <ExtraParams>
                    <ext:Parameter Name="e1" Value="Ext.encode(GridPanel1.getRowsValues())" Mode="Raw" />
                    <ext:Parameter Name="e2" Value="GridPanel1.view.headerRows[0].columns.length.toString()"
                        Mode="Raw" />
                </ExtraParams>
                <EventMask ShowMask="true" Target="Page" />
            </Click>
        </DirectEvents>--%>
    </ext:Button>
    </form>
</body>
</html>
