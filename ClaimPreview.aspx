<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClaimPreview.aspx.cs" Inherits="eReimbursement.ClaimPreview" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>打印预览</title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
    <link href="Styles/StyleSheet2.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        var Preview = function () {
            bdhtml = window.document.body.innerHTML;
            sprnstr = "<!--startprint-->";
            eprnstr = "<!--endprint-->";
            prnhtml = bdhtml.substring(bdhtml.indexOf(sprnstr) + 17);
            prnhtml = prnhtml.substring(0, prnhtml.indexOf(eprnstr));
            window.document.body.innerHTML = prnhtml;
            window.print();
        }
        var RowCommand = function (v, record, rowIndex) {
            Window1.show();
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
    </script>
</head>
<body>
    <div class="W_miniblog_print">
        <div class="W_main_print">
            <form id="Form1" runat="server">
            <ext:ResourceManager ID="ResourceManager1" runat="server" />
            <ext:Hidden runat="server" ID="hdrowindex">
            </ext:Hidden>
            <ext:Store ID="Store1" runat="server">
                <Reader>
                    <ext:ArrayReader>
                        <Fields>
                            <ext:RecordField Name="Riqi" Type="Date" DateFormat="yyyy-MM-dd" SortType="AsDate" />
                            <ext:RecordField Name="Qiatankehu" />
                            <ext:RecordField Name="Heji" />
                            <ext:RecordField Name="Yongtu" />
                            <ext:RecordField Name="Duixiang" />
                            <ext:RecordField Name="Kehu" />
                            <ext:RecordField Name="Jine" />
                            <ext:RecordField Name="Miaoshu" />
                        </Fields>
                    </ext:ArrayReader>
                </Reader>
            </ext:Store>
            <div style="margin-bottom: 5px;">
                <ext:Button ID="Button1" runat="server" Text="打印" Width="70">
                    <Listeners>
                        <Click Fn="Preview" />
                    </Listeners>
                </ext:Button>
            </div>
            <!--startprint-->
            <ext:Panel ID="Panel1" runat="server" Height="800" Border="false">
                <Items>
                    <ext:BorderLayout ID="BorderLayout1" runat="server">
                        <Center>
                            <ext:Panel ID="Panel3" runat="server" Height="780" Padding="10" MinHeight="300" AutoScroll="true"
                                Layout="AbsoluteLayout">
                                <Items>
                                    <ext:Label ID="Label4" runat="server" X="315" Y="10" Text="费用申请单" Cls="label20">
                                    </ext:Label>
                                    <ext:Label ID="Label5" runat="server" X="515" Y="40" Text="单号：ZJDBJS_201301220001"
                                        Cls="label15">
                                    </ext:Label>
                                    <ext:Label ID="Label6" runat="server" X="10" Y="70" Text="费用类型：交际费" Cls="label15">
                                    </ext:Label>
                                    <ext:Label ID="Label7" runat="server" X="515" Y="70" Text="报销月度：2013-01" Cls="label15">
                                    </ext:Label>
                                    <ext:Label ID="Label10" runat="server" X="10" Y="100" Text="申请人：Hughson Huang" Cls="label15">
                                    </ext:Label>
                                    <ext:Label ID="Label8" runat="server" X="315" Y="100" Text="站点：ZJDBJS" Cls="label15">
                                    </ext:Label>
                                    <ext:Label ID="Label9" runat="server" X="515" Y="100" Text="部门: MIS" Cls="label15">
                                    </ext:Label>
                                    <ext:Label ID="Label11" runat="server" X="10" Y="130" Text="费用报销张数：4" Cls="label15">
                                    </ext:Label>
                                    <ext:Label ID="Label12" runat="server" X="315" Y="130" Text="费用报销总金额：10800" Cls="label15">
                                    </ext:Label>
                                    <ext:Label ID="Label3" runat="server" X="10" Y="160" Text="费用明细:" Cls="label15">
                                    </ext:Label>
                                    <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                        Border="false" AutoScroll="true" Height="250" AutoExpandColumn="Riqi" X="10"
                                        Y="180" Width="726" AutoWidth="false">
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                                <ext:RowNumbererColumn Width="30" />
                                                <ext:DateColumn Header="日期" Width="120" DataIndex="Riqi" Format="yyyy-MM-dd" />
                                                <ext:Column Header="洽谈客户" Width="80" DataIndex="Qiatankehu" />
                                                <ext:Column Header="客户年度合计" Width="80" DataIndex="Heji" />
                                                <ext:Column Header="用途" Width="80" DataIndex="Yongtu" />
                                                <ext:Column Header="洽谈对象" Width="80" DataIndex="Duixiang" />
                                                <ext:Column Header="客户" Width="80" DataIndex="Kehu" />
                                                <ext:Column Header="金额" Width="80" DataIndex="Jine" />
                                                <ext:Column Header="描述" Width="80" DataIndex="Miaoshu" />
                                            </Columns>
                                        </ColumnModel>
                                    </ext:GridPanel>
                                </Items>
                            </ext:Panel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
            <!--endprint-->
            </form>
        </div>
    </div>
</body>
</html>
