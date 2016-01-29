<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplyEE.aspx.cs" Inherits="eReimbursement.ApplyEE" %>

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
    <script type="text/javascript">
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
            //            var reg = new RegExp("<br />");
            //            dfDate.setValue(row.data.DateT);
            //            txtAmounts.setValue(row.data.Amounts);
            //            txtFrom.setValue(row.data.From.replace(reg, "\n"));
            //            txtTo1.setValue(row.data.To.replace(reg, "\n"));
            //            txtPurpose1.setValue(row.data.Purpose.replace(reg, "\n"));
        };
    </script>
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
                </Fields>
            </ext:ArrayReader>
        </Reader>
    </ext:Store>
    <ext:Panel ID="Panel3" runat="server" Height="450" Width="850" Border="false" Padding="10"
        MinHeight="300" AutoScroll="true" Layout="AbsoluteLayout">
        <Items>
            <ext:Panel ID="Panel4" runat="server" X="10" Y="10" Height="190" Layout="AbsoluteLayout"
                Title="已使用/年度预算:2100/40000 已使用/月度预算:200/4000" Padding="10">
                <Items>
                    <ext:Label ID="Label1" runat="server" Text="日期:" Width="55" X="10" Y="10" />
                    <ext:Label ID="Label2" runat="server" Text="2012-11-23" Width="100" X="65" Y="10" />
                    <ext:Label ID="Label4" runat="server" Text="洽谈对象:" Width="55" X="225" Y="10" />
                    <ext:Label ID="Label5" runat="server" Text="ZJDTSN" Width="200" X="280" Y="10" />
                    <ext:Label ID="Label6" runat="server" Text="金额:" Width="55" X="480" Y="10" />
                    <ext:Label ID="Label9" runat="server" Text="1200" Width="100" X="535" Y="10" />
                    <ext:Label ID="Label10" runat="server" Text="类型:" Width="55" X="635" Y="10" />
                    <ext:Label ID="Label12" runat="server" Text="客户" Width="50" X="685" Y="10" />
                    <ext:Label ID="Label13" runat="server" Text="客户:" Width="55" X="10" Y="40" />
                    <ext:Label ID="Label14" runat="server" Text="Google" Width="120" X="65" Y="40" />
                    <ext:Label ID="Label15" runat="server" Text="宾客:" Width="55" X="225" Y="40" />
                    <ext:Label ID="Label16" runat="server" Text="Larry Page" Width="120" X="280" Y="40" />
                    <ext:Label ID="Label17" runat="server" Text="洽谈公司:" Width="55" X="480" Y="40" />
                    <ext:Label ID="Label18" runat="server" Text="Google" Width="120" X="535" Y="40" />
                    <ext:Label ID="Label11" runat="server" X="10" Y="70" Text="客户年度合计:" Width="100" />
                    <ext:Label ID="Label19" runat="server" X="110" Y="70" Text="1000000" Width="100" />
                    <ext:Label ID="Label20" runat="server" Text="目的用途:" Width="55" X="225" Y="70" />
                    <ext:Label ID="Label21" runat="server" Text="拯救世界" Width="120" X="280" Y="70" />
                    <ext:Label ID="Label8" runat="server" Text="附件:" Width="55" X="10" Y="100" />
                    <ext:HyperLink ID="HyperLink1" runat="server" Text="a.png" X="65" Y="100" />
                    <ext:Label ID="Label22" runat="server" Text="费用描述:" Width="55" X="225" Y="100" />
                    <ext:Label ID="Label23" runat="server" Text="拯救世界" Width="120" X="280" Y="100" />
                    <ext:Button ID="Button1" runat="server" Text="返回" X="10" Y="130" Width="75">
                        <Listeners>
                            <Click Handler="parent.Window1.hide();" />
                        </Listeners>
                    </ext:Button>
                </Items>
            </ext:Panel>
            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                Title="单击每行以在上方查看.<a>报销张数合计:13 金额报销合计:12070.21.</a>" TrackMouseOver="false" Height="200"
                AutoExpandColumn="DateT" X="10" Y="200">
                <ColumnModel ID="ColumnModel1" runat="server">
                    <Columns>
                        <ext:RowNumbererColumn Width="30" />
                        <ext:DateColumn Header="日期" Width="80" DataIndex="DateT" Format="yyyy-MM-dd" />
                        <ext:Column Header="洽谈公司" Width="80" DataIndex="Company" />
                        <ext:Column Header="客户年度合计" Width="100" DataIndex="Kehuheji" />
                        <ext:Column Header="目的用途" Width="100" DataIndex="Purpose" />
                        <ext:Column Header="洽谈对象" Width="100" DataIndex="Person" />
                        <ext:Column Header="金额" Width="60" DataIndex="Amounts">
                            <Renderer Fn="change" />
                        </ext:Column>
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
