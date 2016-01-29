<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplyE.aspx.cs" Inherits="eReimbursement.ApplyE" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>交际费</title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
    <link href="Styles/StyleSheet1.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        var GetList = function (a, b, c, d) {
            var q = a.query;

            a.query = new RegExp(q);
            a.query.length = q.length;
        };
//        var GetBudget = function (a) {
//            if (a.getValue() == "") {
//                Label2.setText("本站预算－个人：1000/10000");
//                Label4.setText("部门：5000/50000");
//                Label5.setText("站点：20000/200000");
//            }
//            else if (a.getValue() == "ZJDTSN") {
//                Label2.setText("该站预算－个人：N/A");
//                Label4.setText("部门：N/A");
//                Label5.setText("站点：30000/300000");
//            }
//            else if (a.getValue() == "GCR") {
//                Label2.setText("本站预算－个人：N/A");
//                Label4.setText("部门：N/A");
//                Label5.setText("站点：130000/1300000");
//            }
//            else if (a.getValue() == "ZJDTAO") {
//                Label2.setText("本站预算－个人：N/A");
//                Label4.setText("部门：N/A");
//                Label5.setText("站点：13000/130000");
//            }
//            showtrigger(a);
//        };
//        var showtrigger = function (a) {
//            a.triggers[0].show();
//        };
        var linktem = '<a href="http://www.dimerco.com/dimerco/en/images/dim_logo.jpg" target="_blank">{0}</a>';
        var attachlink = function (value) {
            return String.format(linktem, value);
        };
        var ConfirmSave = function () {
            Ext.Msg.confirm("提示", "是否此报销所有明细均已输入完毕，现在提交申请?", function (btn, text) {
                if (btn == 'yes') {
                    return true;
                }
            });
        };
        var template = '<span style="color:{0};">{1}</span>';

        var change = function (value) {
            return String.format(template, (value > 57.1) ? "green" : "red", value);
        };

        var pctChange = function (value) {
            return String.format(template, (value > 0) ? "green" : "red", value + "%");
        };
        var prepare = function (grid, toolbar, rowIndex, record) {
            var firstButton = toolbar.items.get(0);

            if (record.data.Amounts < 57.2) {
                firstButton.setDisabled(true);
                firstButton.setTooltip("Disabled");
            }

            //you can return false to cancel toolbar for this record
        };
        var deleteRows = function (grid, record) {
            Ext.Msg.confirm('Delete Rows', 'Are you sure?', function (btn) {
                if (btn == 'yes') {
                    //                    grid.deleteSelected();
                    Store1.remove(record);
                    //                    if (GridPanel1.getSelectionModel().getCount() == 1 && GridPanel1.getSelectionModel().getSelections()[0] == record) {
                    //                        dfDate.reset();
                    //                        txtAmounts.reset();
                    //                        txtFrom1.reset();
                    //                        txtTo.reset();
                    //                        txtPurpose.reset();
                    //                    }
                }

                //return focus
                grid.view.focusEl.focus();
            })
        };
        var getDetail = function (row) {
            var reg = new RegExp("<br />");
            dfDate.setValue(row.data.DateT);
            txtAmounts.setValue(row.data.Amounts);
            txtFrom.setValue(row.data.From.replace(reg, "\n"));
            txtTo1.setValue(row.data.To.replace(reg, "\n"));
            txtPurpose1.setValue(row.data.Purpose.replace(reg, "\n"));
        };
    </script>
    <style type="text/css">
        
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <ext:Hidden runat="server" ID="hdrowindex">
    </ext:Hidden>
    <ext:Store ID="Store1" runat="server">
        <Reader>
            <ext:ArrayReader>
                <Fields>
                    <ext:RecordField Name="DateT" Type="Date" DateFormat="yyyy-MM-dd" SortType="AsDate" />
                    <ext:RecordField Name="Company" />
                    <ext:RecordField Name="Kehuheji" />
                    <ext:RecordField Name="Purpose" />
                    <ext:RecordField Name="Person" />
                    <ext:RecordField Name="Amounts" />
                    <ext:RecordField Name="Remark" />
                    <ext:RecordField Name="CustomerCode" />
                    <ext:RecordField Name="CustomerCode1" />
                    <ext:RecordField Name="Fujian" />
                    <ext:RecordField Name="CostType" />
                </Fields>
            </ext:ArrayReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="Store2" runat="server">
        <Reader>
            <ext:ArrayReader>
                <Fields>
                    <ext:RecordField Name="GLName" />
                    <ext:RecordField Name="GLCode" />
                    <ext:RecordField Name="GLEName" />
                </Fields>
            </ext:ArrayReader>
        </Reader>
    </ext:Store>
    <ext:Panel ID="Panel3" runat="server" Height="450" Width="850" Border="false" Padding="10"
        MinHeight="300" AutoScroll="true" Layout="AbsoluteLayout">
        <Items>
            <ext:TabPanel ID="Panel1" runat="server" Height="190" X="10" Y="10">
                <Items>
                    <ext:Panel ID="Panel5" runat="server" Border="false" Layout="FitLayout" Title="基本信息">
                        <Items>
                            <ext:ColumnLayout runat="server">
                                <Columns>
                                    <ext:LayoutColumn ColumnWidth="1">
                                        <ext:Panel ID="Panel4" runat="server" Layout="AbsoluteLayout" Padding="10" AutoScroll="true"
                                            Title="在下方编辑费用基本信息,如须补充请进入其他信息标签.">
                                            <Items>
                                                <ext:ComboBox ID="cbxCOAType" runat="server" FieldLabel="费用类型" X="10" Y="10" Width="305"
                                                    LabelWidth="70" StoreID="Store2" ItemSelector="tr.list-item" DisplayField="GLName"
                                                    ValueField="GLName" ListWidth="500" MinChars="1" PageSize="10" Mode="Local" EnableKeyEvents="true"
                                                    ForceSelection="true">
                                                    <Template ID="Template2" runat="server">
                                                        <Html>
                                                            <tpl for=".">
						                                            <tpl if="[xindex] == 1">
							                                            <table class="cbStates-list">
								                                            <tr>
									                                            <th>费用类型</th>
                                                                                <th>费用代码</th>
                                                                                <th>Cost Type</th>
								                                            </tr>
						                                            </tpl>
						                                            <tr class="list-item">
							                                            <td style="padding:3px 0px;">{GLName}</td>
                                                                        <td>{GLCode}</td>
                                                                        <td>{GLEName}</td>
						                                            </tr>
						                                            <tpl if="[xcount-xindex]==0">
							                                            </table>
						                                            </tpl>
					                                            </tpl>
                                                        </Html>
                                                    </Template>
                                                    <Triggers>
                                                        <ext:FieldTrigger Icon="Clear" Qtip="Remove selected" HideTrigger="true" />
                                                    </Triggers>
                                                    <Listeners>
                                                        <BeforeQuery Fn="GetList" />
                                                        <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                                                        <Select Handler="this.triggers[0].show();" />
                                                    </Listeners>
                                                </ext:ComboBox>
                                                <ext:TextField ID="txtCOAContent" runat="server" FieldLabel="费用描述" X="325" Y="10"
                                                    Width="270" LabelWidth="70">
                                                </ext:TextField>
                                                <ext:Label ID="Label4" runat="server" X="10" Y="43" Text="币种:" />
                                                <ext:Label ID="LabelCurrency" runat="server" X="50" Y="43" Text="RMB" />
                                                <ext:TextField ID="txtAmount1" runat="server" FieldLabel="金额" X="90" Y="40" Width="160"
                                                    LabelWidth="30" EmptyText="" />
                                                <ext:ComboBox ID="cbxAgent" runat="server" FieldLabel="成本中心" X="265" Y="40" Width="200"
                                                    LabelWidth="60">
                                                    <Items>
                                                        <ext:ListItem Text="ZJDTSN" />
                                                        <ext:ListItem Text="ZJDBJS" />
                                                        <ext:ListItem Text="ZJDTAO" />
                                                    </Items>
                                                    <Triggers>
                                                        <ext:FieldTrigger Icon="Clear" Qtip="Remove selected" HideTrigger="true" />
                                                    </Triggers>
                                                    <Listeners>
                                                        <Select Handler="this.triggers[0].show();LabelAgent.show();" />
                                                        <TriggerClick Handler="if (index == 0) { this.clearValue(); this.triggers[0].hide(); LabelAgent.hide();}" />
                                                    </Listeners>
                                                </ext:ComboBox>
                                                <ext:Label ID="Label2" runat="server" Width="70" X="10" Y="73" Text="上传文件(*):" Cls="UploadLabel" />
                                                <ext:FileUploadField ID="FileUploadField1" runat="server" Icon="Attach" X="70" Y="60"
                                                    Width="300" />
                                                <ext:HyperLink ID="HLAttachFile" runat="server" Text="" X="390" Y="73" Target="_blank" />
                                                <ext:Button ID="Button9" runat="server" Text="完善其他信息" X="10" Y="100" Width="90">
                                                    <Listeners>
                                                        <Click Handler="Panel1.setActiveTab(1);" />
                                                    </Listeners>
                                                </ext:Button>
                                                <ext:Button ID="Button5" runat="server" Text="编辑" X="105" Y="100" Width="90">
                                                </ext:Button>
                                                <ext:Button ID="Button6" runat="server" Text="新增" X="200" Y="100" Width="90">
                                                </ext:Button>
                                                <ext:Button ID="Button7" runat="server" Text="清空" X="295" Y="100" Width="90">
                                                </ext:Button>
                                                <ext:Button ID="Button1" runat="server" Text="返回" X="390" Y="100" Width="90">
                                                    <Listeners>
                                                        <Click Handler="parent.Window1.hide();" />
                                                    </Listeners>
                                                </ext:Button>
                                                <%--<ext:ComboBox ID="ComboBox1" runat="server" FieldLabel="成本中心" X="600" Y="10" Width="200"
                                                    LabelWidth="40">
                                                    <Items>
                                                        <ext:ListItem Text="ZJDTSN" />
                                                        <ext:ListItem Text="GCR" />
                                                        <ext:ListItem Text="ZJDTAO" />
                                                    </Items>
                                                    <Triggers>
                                                        <ext:FieldTrigger Icon="Clear" Qtip="Remove selected" HideTrigger="true" />
                                                    </Triggers>
                                                    <Listeners>
                                                        <Select Fn="GetBudget" />
                                                        <TriggerClick Handler="if (index == 0) { this.clearValue(); this.triggers[0].hide(); Label2.setText('本站预算－个人：1000/10000');Label4.setText('部门：5000/50000');Label5.setText('站点：20000/200000');}" />
                                                    </Listeners>
                                                </ext:ComboBox>
                                                <ext:ComboBox ID="ComboBox2" runat="server" FieldLabel="费用类型" X="10" Y="40" Width="305"
                                                    LabelWidth="60" StoreID="Store2" ItemSelector="tr.list-item" DisplayField="GLName"
                                                    ValueField="GLName" ListWidth="450">
                                                    <Template ID="Template1" runat="server">
                                                        <Html>
                                                            <tpl for=".">
						                                        <tpl if="[xindex] == 1">
							                                        <table class="cbStates-list">
								                                        <tr>
									                                        <th>费用类型</th>
									                                        <th>费用代码</th>
                                                                            <th></th>
								                                        </tr>
						                                        </tpl>
						                                        <tr class="list-item">
							                                        <td style="padding:3px 0px;">{GLName}</td>
							                                        <td>{GLCode}</td>
                                                                    <td>{GLEName}</td>
						                                        </tr>
						                                        <tpl if="[xcount-xindex]==0">
							                                        </table>
						                                        </tpl>
					                                        </tpl>
                                                        </Html>
                                                    </Template>
                                                    <Triggers>
                                                        <ext:FieldTrigger Icon="Clear" Qtip="Remove selected" HideTrigger="true" />
                                                    </Triggers>
                                                    <Listeners>
                                                        <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                                                        <Select Handler="this.triggers[0].show();" />
                                                    </Listeners>
                                                </ext:ComboBox>
                                                <ext:TextField ID="txtPurpose1" runat="server" FieldLabel="费用描述" X="325" Y="40" Width="400"
                                                    LabelWidth="60" Anchor="100%">
                                                </ext:TextField>
                                                <ext:DateField ID="dfDate" runat="server" FieldLabel="日期" Format="yyyy-MM-dd" X="10"
                                                    Y="70" Width="205" LabelWidth="60" EmptyText="0000-00-00" />
                                                <ext:TextField ID="TextField1" runat="server" FieldLabel="洽谈对象" X="225" Y="70" Width="300"
                                                    LabelWidth="60" />
                                                <ext:TextField ID="txtAmounts" runat="server" FieldLabel="金额" X="535" Y="70" Width="120"
                                                    LabelWidth="30" />
                                                <ext:ComboBox ID="ComboBox3" runat="server" FieldLabel="站点" LabelWidth="30" Width="100"
                                                    X="665" Y="70" SelectedIndex="0">
                                                    <Items>
                                                        <ext:ListItem Text="客户" />
                                                        <ext:ListItem Text="其他" />
                                                    </Items>
                                                </ext:ComboBox>
                                                <ext:TextField ID="TextField2" runat="server" FieldLabel="客户" X="10" Y="100" Width="205"
                                                    LabelWidth="60" />
                                                <ext:TextField ID="TextField3" runat="server" FieldLabel="宾客" X="225" Y="100" Width="205"
                                                    LabelWidth="60" />
                                                <ext:TextField ID="TextField4" runat="server" FieldLabel="洽谈公司" X="440" Y="100" Anchor="100%"
                                                    LabelWidth="60" />
                                                <ext:Label ID="Label11" runat="server" X="10" Y="133" Text="客户年度合计：999999" />
                                                <ext:TextField ID="TextField5" runat="server" FieldLabel="目的用途" X="145" Y="130" Width="350"
                                                    LabelWidth="60" />
                                                <ext:Label ID="Label8" runat="server" Width="60" X="505" Y="133" Text="上传文件:" />
                                                <ext:FileUploadField ID="BasicField" runat="server" Width="245" Icon="Attach" X="565"
                                                    Y="120" />--%>
                                            </Items>
                                        </ext:Panel>
                                    </ext:LayoutColumn>
                                    <ext:LayoutColumn>
                                        <ext:Panel ID="Panel6" runat="server" Title="预算" Width="170" Layout="AbsoluteLayout">
                                            <Items>
                                                <ext:Label ID="Label1" runat="server" X="10" Y="5" Text="本站个人：5000/10000" Cls="budgetfont" />
                                                <ext:Label ID="Label9" runat="server" X="10" Y="30" Text="本站部门：6000/50000" Cls="budgetfont" />
                                                <ext:Label ID="Label10" runat="server" X="10" Y="55" Text="本站站点：20000/200000" Cls="budgetfont" />
                                                <ext:Label ID="LabelAgent" runat="server" X="10" Y="80" Text="成本中心：20000/200000"
                                                    Cls="budgetfont" Hidden="true" />
                                                <ext:Label ID="LabelCustomerTotal" runat="server" X="10" Y="105" Text="客户年度合计：999000" />
                                            </Items>
                                        </ext:Panel>
                                    </ext:LayoutColumn>
                                </Columns>
                            </ext:ColumnLayout>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel2" runat="server" Title="其他信息" Layout="AbsoluteLayout" Padding="10">
                        <Items>
                            <ext:DateField ID="dfDate" runat="server" FieldLabel="日期" Format="yyyy-MM-dd" X="10"
                                Y="10" Width="205" LabelWidth="60" EmptyText="0000-00-00" />
                            <ext:TextField ID="TextField1" runat="server" FieldLabel="洽谈对象" X="225" Y="10" Width="200"
                                LabelWidth="60" />
                            <ext:TextField ID="TextField2" runat="server" FieldLabel="客户" X="10" Y="40" Width="205"
                                LabelWidth="60" />
                            <ext:TextField ID="TextField3" runat="server" FieldLabel="宾客" X="225" Y="40" Width="205"
                                LabelWidth="60" />
                            <ext:TextField ID="TextField4" runat="server" FieldLabel="洽谈公司" X="440" Y="40" Width="205"
                                LabelWidth="60" />
                            <ext:TextField ID="TextField5" runat="server" FieldLabel="目的用途" X="10" Y="70" Anchor="100%"
                                LabelWidth="60" />
                            <ext:Button ID="Button2" runat="server" Text="编辑" X="10" Y="130" Width="90">
                            </ext:Button>
                            <ext:Button ID="Button3" runat="server" Text="新增" X="105" Y="130" Width="90">
                            </ext:Button>
                            <ext:Button ID="Button4" runat="server" Text="清空" X="200" Y="130" Width="90">
                            </ext:Button>
                            <ext:Button ID="Button8" runat="server" Text="返回" X="295" Y="130" Width="90">
                                <Listeners>
                                    <Click Handler="parent.Window1.hide();" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="Button10" runat="server" Text="完善基本信息" X="390" Y="130" Width="90">
                                <Listeners>
                                    <Click Handler="Panel1.setActiveTab(0);" />
                                </Listeners>
                            </ext:Button>
                        </Items>
                    </ext:Panel>
                </Items>
            </ext:TabPanel>
            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                Title="单击每行以在下方编辑.报销张数合计:13 金额报销合计:12070.21." TrackMouseOver="false" Height="200"
                AutoExpandColumn="DateT" X="10" Y="200">
                <ColumnModel ID="ColumnModel1" runat="server">
                    <Columns>
                        <ext:RowNumbererColumn Width="30" />
                        <ext:CommandColumn Width="110">
                            <Commands>
                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="编辑" />
                                <ext:GridCommand Icon="Delete" CommandName="Delete" Text="删除" />
                            </Commands>
                            <PrepareToolbar Fn="prepare" />
                        </ext:CommandColumn>
                        <ext:DateColumn Header="日期" Width="80" DataIndex="DateT" Format="yyyy-MM-dd" />
                        <ext:Column Header="洽谈公司" Width="80" DataIndex="Company" />
                        <ext:Column Header="客户年度合计" Width="100" DataIndex="Kehuheji" />
                        <ext:Column Header="目的用途" Width="100" DataIndex="Purpose" />
                        <ext:Column Header="洽谈对象" Width="100" DataIndex="Person" />
                        <ext:Column Header="金额" Width="60" DataIndex="Amounts">
                            <Renderer Fn="change" />
                        </ext:Column>
                        <ext:Column Header="费用类型" Width="100" DataIndex="CostType" />
                        <ext:Column Header="费用描述" Width="100" DataIndex="Remark" />
                        <ext:Column Header="客户" Width="60" DataIndex="CustomerCode" Hidden="true" />
                        <ext:Column Header="宾客" Width="60" DataIndex="CustomerCode1" Hidden="true" />
                        <ext:Column Header="附件" Width="60" DataIndex="Fujian">
                            <Renderer Fn="attachlink" />
                        </ext:Column>
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                    </ext:RowSelectionModel>
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="20" DisplayInfo="true"
                        DisplayMsg="Displaying items {0} - {1} of {2}" EmptyMsg="No items to display"
                        HideRefresh="true">
                        <Items>
                            <ext:Label ID="Label3" runat="server" Text="Page size:" />
                            <ext:ToolbarSpacer ID="ToolbarSpacer1" runat="server" Width="10" />
                            <ext:ComboBox ID="cbxPageSize" runat="server" Width="80">
                                <Items>
                                    <ext:ListItem Text="10" />
                                    <ext:ListItem Text="20" />
                                    <ext:ListItem Text="50" />
                                    <ext:ListItem Text="100" />
                                    <ext:ListItem Text="200" />
                                </Items>
                                <SelectedItem Value="20" />
                                <Listeners>
                                    <Select Handler="#{PagingToolbar1}.pageSize = parseInt(this.getValue()); #{PagingToolbar1}.doLoad();" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:PagingToolbar>
                </BottomBar>
            </ext:GridPanel>
            <ext:Label ID="Label7" runat="server" Text="Created By: 2012-12-13 ( Hughson Huang ) Updated By: 2012-12-13 ( Hughson Huang )"
                X="20" Y="420">
            </ext:Label>
        </Items>
    </ext:Panel>
    </form>
</body>
</html>
