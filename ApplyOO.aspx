<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplyOO.aspx.cs" Inherits="eReimbursement.ApplyOO" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>其他费用</title>
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
                    <ext:RecordField Name="Type" />
                    <ext:RecordField Name="Amounts" />
                    <ext:RecordField Name="Miaoshu" />
                    <ext:RecordField Name="Fujian" />
                </Fields>
            </ext:ArrayReader>
        </Reader>
    </ext:Store>
    <ext:Panel ID="Panel3" runat="server" Height="450" Width="850" Border="false" Padding="10"
        MinHeight="300" AutoScroll="true" Layout="AbsoluteLayout">
        <Items>
            <ext:Panel ID="Panel4" runat="server" X="10" Y="10" Height="160" Layout="AbsoluteLayout"
                Padding="10">
                <Items>
                    <ext:Label ID="Label1" runat="server" Text="日期:" Width="55" X="10" Y="10" />
                    <ext:Label ID="Label2" runat="server" Text="2012-02-31" Width="100" X="65" Y="10" />
                    <ext:Label ID="Label4" runat="server" Text="金额:" Width="55" X="175" Y="10" />
                    <ext:Label ID="Label5" runat="server" Text="57.1" Width="100" X="230" Y="10" />
                    <ext:Label ID="Label6" runat="server" Text="附件:" Width="55" X="340" Y="10" />
                    <ext:HyperLink ID="HyperLink1" runat="server" Text="a.pdf" X="395" Y="10" />
                    <ext:Label ID="Label9" runat="server" Text="费用类型:" Width="55" X="10" Y="40" />
                    <ext:Label ID="Label10" runat="server" Text="摊销" Width="100" X="65" Y="40" />
                    <ext:Label ID="Label12" runat="server" Text="费用描述:" Width="55" X="10" Y="70" />
                    <ext:Label ID="Label13" runat="server" Text="吃饭去" X="65" Y="70" Anchor="100%" />
                    <ext:Button ID="Button2" runat="server" Text="返回" X="10" Y="100" Width="75">
                        <Listeners>
                            <Click Handler="parent.Window1.hide();" />
                        </Listeners>
                    </ext:Button>
                </Items>
            </ext:Panel>
            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" Title="单击每行以在上方编辑.报销张数合计:5 金额报销合计:12070.21."
                TrackMouseOver="false" Height="200" X="10" Y="170">
                <ColumnModel ID="ColumnModel1" runat="server">
                    <Columns>
                        <ext:RowNumbererColumn Width="30" />
                        <ext:DateColumn Header="日期" Width="80" DataIndex="DateT" Format="yyyy-MM-dd" />
                        <ext:Column Header="费用类型" Width="120" DataIndex="Type" />
                        <ext:Column Header="金额" Width="60" DataIndex="Amounts">
                            <Renderer Fn="change" />
                        </ext:Column>
                        <ext:Column Header="费用描述" Width="100" DataIndex="Miaoshu" />
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
                X="20" Y="390">
            </ext:Label>
        </Items>
    </ext:Panel>
    </form>
</body>
</html>
