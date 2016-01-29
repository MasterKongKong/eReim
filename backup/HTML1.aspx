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

            Store1.Reader[0].Fields.Add("Category", RecordFieldType.String);

            string fieldPName = "Station_0_P";
            Store1.Reader[0].Fields.Add(fieldPName, RecordFieldType.Float);
            string fieldCName = "Station_0_C";
            Store1.Reader[0].Fields.Add(fieldCName, RecordFieldType.Float);
            
            //合计列
            //Store1.Reader[0].Fields.Add("TotalP", RecordFieldType.Float);
            //Store1.Reader[0].Fields.Add("TotalC", RecordFieldType.Float);
            
            DataTable dt = new DataTable();
            dt.Columns.Add("Category", typeof(String));
            dt.Columns.Add("Station_0_P", typeof(String));
            dt.Columns.Add("Station_0_C", typeof(String));
            //dt.Columns.Add("TotalP", typeof(String));
            //dt.Columns.Add("TotalC", typeof(String));
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
            txtP.Listeners.Blur.Fn = "Cal";
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
            txtC.Listeners.Blur.Fn = "Cal";
            var colC = new Column();
            colC.DataIndex = fieldCName;
            colC.Sortable = false;
            colC.Resizable = false;
            colC.MenuDisabled = true;
            colC.Width = 110;
            colP.Locked = true;
            colC.Editor.Add(txtC);
            this.GridPanel1.ColumnModel.Columns.Add(colC);

            //var colTotalP = new Column();
            //colTotalP.DataIndex = "TotalP";
            //colTotalP.Sortable = false;
            //colTotalP.Resizable = false;
            //colTotalP.MenuDisabled = true;
            //colTotalP.Width = 110;
            //colTotalP.Locked = true;
            //this.GridPanel1.ColumnModel.Columns.Add(colTotalP);

            //var colTotalC = new Column();
            //colTotalC.DataIndex = "TotalC";
            //colTotalC.Sortable = false;
            //colTotalC.Resizable = false;
            //colTotalC.MenuDisabled = true;
            //colTotalC.Width = 110;
            //colTotalC.Locked = true;
            //this.GridPanel1.ColumnModel.Columns.Add(colTotalC);

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
            HeaderColumn hcButton = new HeaderColumn();
            hcButton.Component.Add(Button);
            this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcButton);

            //HeaderColumn hcTotal1 = new HeaderColumn();
            //this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcTotal1);

            //HeaderColumn hcTotal2 = new HeaderColumn();
            //this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcTotal2);

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

            HeaderColumn hcCostCenter1 = new HeaderColumn();
            this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcCostCenter1);

            //HeaderColumn hcTotal3 = new HeaderColumn();
            //this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcTotal3);

            //HeaderColumn hcTotal4 = new HeaderColumn();
            //this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcTotal4);

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

            //var TitleTotalP = new Ext.Net.Label();
            //TitleTotalP.Text = "Reimbursement";
            //HeaderColumn hcTitleTotalP = new HeaderColumn();
            //hcTitleTotalP.Component.Add(TitleTotalP);
            //this.GridPanel1.GetView().HeaderRows[2].Columns.Add(hcTitleTotalP);

            //var TitleTotalC = new Ext.Net.Label();
            //TitleTotalC.Text = "Company Paid";
            //HeaderColumn hcTitleTotalC = new HeaderColumn();
            //hcTitleTotalC.Component.Add(TitleTotalC);
            //this.GridPanel1.GetView().HeaderRows[2].Columns.Add(hcTitleTotalC);

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
        int colc = (dtcol - 1) / 2;
        DataTable dt = new DataTable();

        Store1.Reader[0].Fields.Add("Category", RecordFieldType.String);
        dt.Columns.Add(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[0].Name, typeof(String));
        for (int i = 0; i < (dtcol - 1) / 2; i++)
        {
            Store1.Reader[0].Fields.Add("Station_" + i.ToString() + "_P", RecordFieldType.Float);
            Store1.Reader[0].Fields.Add("Station_" + i.ToString() + "_C", RecordFieldType.Float);
            dt.Columns.Add(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[1 + i * 2].Name, typeof(String));
            dt.Columns.Add(doc.SelectNodes("records").Item(0).SelectNodes("record").Item(0).ChildNodes[1 + i * 2 + 1].Name, typeof(String));
        }
        string fieldPNameNew = "Station_" + colc.ToString() + "_P";
        Store1.Reader[0].Fields.Add(fieldPNameNew, RecordFieldType.Float);
        string fieldCNameNew = "Station_" + colc.ToString() + "_C";
        Store1.Reader[0].Fields.Add(fieldCNameNew, RecordFieldType.Float);
        //合计列
        //Store1.Reader[0].Fields.Add("TotalP", RecordFieldType.Float);
        //Store1.Reader[0].Fields.Add("TotalC", RecordFieldType.Float);
        //dt.Columns.Add("TotalP", typeof(String));
        //dt.Columns.Add("TotalC", typeof(String));
        for (int i = 0; i < doc.SelectNodes("records").Item(0).SelectNodes("record").Count; i++)
        {
            DataRow dr = dt.NewRow();
            dt.Rows.Add(dr);
            for (int j = 0; j < dtcol - 1; j++)
            {
                dt.Rows[i][j] = doc.SelectNodes("records").Item(0).SelectNodes("record").Item(i).ChildNodes[j].InnerText;
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
            txtP.Listeners.Blur.Fn = "Cal";
            var colP = new Column();
            colP.DataIndex = fieldPName;
            colP.Sortable = false;
            colP.Resizable = false;
            colP.MenuDisabled = true;
            colP.Width = 110;
            colP.Editor.Add(txtP);
            this.GridPanel1.ColumnModel.Columns.Add(colP);

            var txtC = new TextField();
            txtC.Listeners.Blur.Fn = "Cal";
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

            HeaderColumn hcCostCenter1 = new HeaderColumn();
            this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcCostCenter1);

            var datefrom = new DateField();
            if (header2string.Split(',')[i * 2]!="NA")
            {
                datefrom.SetValue(header2string.Split(',')[i * 2]);
            }
            HeaderColumn Date1 = new HeaderColumn();
            Date1.Component.Add(datefrom);
            this.GridPanel1.GetView().HeaderRows[2].Columns.Add(Date1);

            var dateto = new DateField();
            if (header2string.Split(',')[i * 2 + 1]!="NA")
            {
                dateto.SetValue(header2string.Split(',')[i * 2 + 1]);
            }
            HeaderColumn Date2 = new HeaderColumn();
            Date2.Component.Add(dateto);
            this.GridPanel1.GetView().HeaderRows[2].Columns.Add(Date2);
        }
        


        var txtPNew = new TextField();
        txtPNew.Listeners.Blur.Fn = "Cal";
        var colPNew = new Column();
        colPNew.DataIndex = fieldPNameNew;
        colPNew.Sortable = false;
        colPNew.Resizable = false;
        colPNew.MenuDisabled = true;
        colPNew.Width = 110;
        colPNew.Editor.Add(txtPNew);
        this.GridPanel1.ColumnModel.Columns.Add(colPNew);

        var txtCNew = new TextField();
        txtCNew.Listeners.Blur.Fn = "Cal";
        var colCNew = new Column();
        colCNew.DataIndex = fieldCNameNew;
        colCNew.Sortable = false;
        colCNew.Resizable = false;
        colCNew.MenuDisabled = true;
        colCNew.Width = 110;
        colCNew.Editor.Add(txtCNew);
        this.GridPanel1.ColumnModel.Columns.Add(colCNew);

        //var colTotalP = new Column();
        //colTotalP.DataIndex = "TotalP";
        //colTotalP.Sortable = false;
        //colTotalP.Resizable = false;
        //colTotalP.MenuDisabled = true;
        //colTotalP.Width = 110;
        //colTotalP.Locked = true;
        //this.GridPanel1.ColumnModel.Columns.Add(colTotalP);

        //var colTotalC = new Column();
        //colTotalC.DataIndex = "TotalC";
        //colTotalC.Sortable = false;
        //colTotalC.Resizable = false;
        //colTotalC.MenuDisabled = true;
        //colTotalC.Width = 110;
        //colTotalC.Locked = true;
        //this.GridPanel1.ColumnModel.Columns.Add(colTotalC);

        var StationNew = new TextField();
        HeaderColumn hcStationNew = new HeaderColumn();
        hcStationNew.Component.Add(StationNew);
        this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcStationNew);

        var ButtonNew = new Ext.Net.Button();
        ButtonNew.Text = "Remove";
        ButtonNew.Listeners.Click.Handler = "removecol(this," + colc.ToString() + ");";
        HeaderColumn hcButtonNew = new HeaderColumn();
        hcButtonNew.Component.Add(ButtonNew);
        this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcButtonNew);

        //HeaderColumn hcTotal1 = new HeaderColumn();
        //this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcTotal1);

        //HeaderColumn hcTotal2 = new HeaderColumn();
        //this.GridPanel1.GetView().HeaderRows[0].Columns.Add(hcTotal2);

        var CostCenterNew = new TextField();
        HeaderColumn hcCostCenterNew = new HeaderColumn();
        hcCostCenterNew.Component.Add(CostCenterNew);
        this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcCostCenterNew);

        HeaderColumn hcCostCenter1New = new HeaderColumn();
        this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcCostCenter1New);

        //HeaderColumn hcTotal3 = new HeaderColumn();
        //this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcTotal3);

        //HeaderColumn hcTotal4 = new HeaderColumn();
        //this.GridPanel1.GetView().HeaderRows[1].Columns.Add(hcTotal4);

        HeaderColumn Date1New = new HeaderColumn();
        Date1New.Component.Add(new DateField());
        this.GridPanel1.GetView().HeaderRows[2].Columns.Add(Date1New);

        HeaderColumn Date2New = new HeaderColumn();
        Date2New.Component.Add(new DateField());
        this.GridPanel1.GetView().HeaderRows[2].Columns.Add(Date2New);

        //var TitleTotalP = new Ext.Net.Label();
        //TitleTotalP.Text = "Reimbursement";
        //HeaderColumn hcTitleTotalP = new HeaderColumn();
        //hcTitleTotalP.Component.Add(TitleTotalP);
        //this.GridPanel1.GetView().HeaderRows[2].Columns.Add(hcTitleTotalP);

        //var TitleTotalC = new Ext.Net.Label();
        //TitleTotalC.Text = "Company Paid";
        //HeaderColumn hcTitleTotalC = new HeaderColumn();
        //hcTitleTotalC.Component.Add(TitleTotalC);
        //this.GridPanel1.GetView().HeaderRows[2].Columns.Add(hcTitleTotalC);

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
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
    <title>Ext.Net Example</title>
    <script type="text/javascript">
        var Cal = function (a) {
            var af = a;
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
            //记录隐藏的列
            var dc = hdDelCol.getValue().toString() == '' ? b.toString() : (hdDelCol.getValue() + ',' + b.toString());
            hdDelCol.setValue(dc);
        };
        var ClearCol = function (a, b, c) {
//            Store1.removeField('TotalP');
//            Store1.removeField('TotalC');
            var headercol = GridPanel1.view.headerRows[0].columns.length;
            var col = GridView1.cm.columns.length;
            var colnew = GridView1.cm.columns.length;
            //            GridPanel1.clearContent();
            var header0string = ''; var header1string = ''; var header2string = '';
            for (var i = 1; i < col; i++) {
                if (GridView1.cm.columns[i].hidden) {
                    colnew--;
                }
                else {
                    if (i % 2 == 1) {
                        var h0 = GridPanel1.view.headerRows[0].columns[i].component.getValue() == '' ? 'NA' : GridPanel1.view.headerRows[0].columns[i].component.getValue();
                        var h1 = GridPanel1.view.headerRows[1].columns[i].component.getValue() == '' ? 'NA' : GridPanel1.view.headerRows[1].columns[i].component.getValue();
                        header0string += (header0string != '') ? (',' + h0) : h0;
                        header1string += (header1string != '') ? (',' + h1) : h1;
                    }
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
            RM.AddCol(detail, header0string, header1string, header2string, {
                eventMask: {
                    showMask: true,
                    tartget: "Page"
                },
                timeout: 300000
            });
        };
    </script>
</head>
<body>
    <form id="Form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" DirectMethodNamespace="RM"/>
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
            <Click Fn="ClearCol" />
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
