<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyDrafts.aspx.cs" Inherits="eReimbursement.MyDrafts" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>草稿报销单</title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        var template = '<span style="color:{0};">{1}</span>';

        var change = function (value) {
            return String.format(template, (value > 0) ? "green" : "red", value);
        };

        var pctChange = function (value) {
            return String.format(template, (value > 0) ? "green" : "red", value + "%");
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
            txtFrom1.setValue(row.data.From.replace(reg, "\n"));
            txtTo.setValue(row.data.To.replace(reg, "\n"));
            txtPurpose.setValue(row.data.Purpose.replace(reg, "\n"));
        };
        var rowdbclick = function (control, rowindex, button) {
            if (Store1) {
                if (Store1.data.items[rowindex].data.ApplyType == '通用费用') {
                    window.open("Apply.aspx");
                }
                else if (Store1.data.items[rowindex].data.ApplyType == '差旅费') {
                    window.open("ApplyTravel.aspx");
                }
            }
        };
        var RowCommand = function (v, record, rowIndex) {
            if (record.data.ApplyType == '通用费用') {
                window.open("Apply.aspx");
            }
            else if (record.data.ApplyType == '差旅费') {
                window.open("ApplyTravel.aspx");
            }
        };
    </script>
</head>
<body>
    <div class="W_miniblog">
        <div class="WB_global_nav">
            <div class="gn_header">
                <div class="gn_nav">
                    <div class="gn_title">
                        <a target="_top" tabindex="4" class="gn_tab" href="#">eReimbursement</a>
                    </div>
                </div>
                <div class="gn_center">
                    <ul class="q-menubox">
                        <li class="q-menuitem"><a href="#" target="_top">首页</a></li>
                        <li class="q-menuitem"><a href="MyClaims.aspx" id="apply">报销申请</a></li>
                        <li class="q-menuitem"><a href="Approve.aspx">报销审核</a></li>
                        <li class="q-menuitem"><a href="FileManagement.aspx">影像管理</a></li>
                        <li class="q-menuitem"><a href="Budget.aspx">预算管理</a></li>
                        <li class="q-menuitem"><a href="Profile.aspx">基础数据</a></li>
                    </ul>
                </div>
                <div class="gn_person">
                    <ul class="q-menubox">
                        <li class="q-menuitem"><a href="#">Hughson Huang</a></li><li class="q-navitem"><span>
                        </span></li>
                        <li class="q-menuitem"><a href="#">信息</a></li><li class="q-navitem"><span></span>
                        </li>
                        <li class="q-menuitem"><a href="#">设置</a></li><li class="q-navitem"><span></span>
                        </li>
                        <li class="q-menuitem"><a href="#">登出</a></li>
                    </ul>
                </div>
            </div>
        </div>
        <div class="W_main W_profile">
            <form id="Form1" runat="server">
            <ext:ResourceManager ID="ResourceManager1" runat="server" />
            <ext:Hidden runat="server" ID="hdrowindex">
            </ext:Hidden>
            <ext:Store ID="Store1" runat="server">
                <Reader>
                    <ext:ArrayReader>
                        <Fields>
                            <ext:RecordField Name="TempApplyNo" />
                            <ext:RecordField Name="ApplyType" />
                            <ext:RecordField Name="Amounts" />
                            <ext:RecordField Name="ApplyDate" Type="Date" DateFormat="yyyy-MM-dd" SortType="AsDate" />
                            <ext:RecordField Name="Remark" />
                            <ext:RecordField Name="Status" />
                        </Fields>
                    </ext:ArrayReader>
                </Reader>
            </ext:Store>
            <ext:Panel ID="Panel1" runat="server" Height="680" Border="false">
                <Items>
                    <ext:BorderLayout ID="BorderLayout1" runat="server">
                        <West Collapsible="true" Split="true" CollapseMode="Mini">
                            <ext:Panel ID="Panel2" runat="server" Width="175">
                                <Items>
                                    <ext:TreePanel ID="TreePanel1" runat="server" Header="false" Lines="false" UseArrows="true"
                                        RootVisible="false" Width="175" Border="false">
                                        <Root>
                                            <ext:TreeNode Text="申请单" Expanded="true">
                                                <Nodes>
                                                    <ext:TreeNode Text="费用报销" Expanded="true" Href="#" Icon="UserKey">
                                                        <Nodes>
                                                            <ext:TreeNode Text="差旅费申请" Href="ApplyTravel.aspx" Icon="FlagAe" NodeID="a1" />
                                                            <ext:TreeNode Text="通用费用申请" Href="Apply.aspx" Icon="FlagAf" NodeID="a2" />
                                                        </Nodes>
                                                    </ext:TreeNode>
                                                    <ext:TreeNode Text="草稿报销单" Href="MyDrafts.aspx" Icon="FlagAm" NodeID="b1" />
                                                    <ext:TreeNode Text="我的报销单" Href="MyClaims.aspx" Icon="FlagAn" NodeID="c1" />
                                                </Nodes>
                                            </ext:TreeNode>
                                        </Root>
                                        <Listeners>
                                            <AfterRender Handler="TreePanel1.getNodeById('b1').select(true);" />
                                        </Listeners>
                                    </ext:TreePanel>
                                </Items>
                            </ext:Panel>
                        </West>
                        <Center>
                            <ext:Panel ID="Panel3" runat="server" Title="草稿报销单" Height="600" MinHeight="300"
                                Padding="10" AutoScroll="true" Layout="AbsoluteLayout">
                                <Items>
                                    <ext:TextField ID="TextField1" runat="server" FieldLabel="临时表单号" LabelWidth="80"
                                        Width="220" X="10" Y="10">
                                    </ext:TextField>
                                    <ext:ComboBox ID="ComboBox2" runat="server" FieldLabel="单据类型" LabelWidth="65" Width="180"
                                        X="240" Y="10" SelectedIndex="2">
                                        <Items>
                                            <ext:ListItem Text="差旅费申请" />
                                            <ext:ListItem Text="通用费用申请" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:TextField ID="TextField2" runat="server" FieldLabel="金额" LabelWidth="30" Width="155"
                                        X="440" Y="10">
                                    </ext:TextField>
                                    <ext:Label ID="Label2" runat="server" Text="--" X="603" Y="13" Width="15">
                                    </ext:Label>
                                    <ext:TextField ID="TextField3" runat="server" Width="125" X="620" Y="10">
                                    </ext:TextField>
                                    <ext:TextField ID="TextField4" runat="server" FieldLabel="备注" LabelWidth="80" Width="220"
                                        X="10" Y="40">
                                    </ext:TextField>
                                    <ext:DateField ID="DateField3" runat="server" FieldLabel="提交时间" LabelWidth="65" Width="180"
                                        X="240" Y="40" Format="yyyy-MM-dd" EmptyText="0000-00-00">
                                    </ext:DateField>
                                    <ext:Label ID="Label3" runat="server" Text="--" X="425" Y="43" Width="15">
                                    </ext:Label>
                                    <ext:DateField ID="DateField1" runat="server" Width="115" X="440" Y="40" Format="yyyy-MM-dd"
                                        EmptyText="0000-00-00">
                                    </ext:DateField>
                                    <ext:ComboBox ID="ComboBox1" runat="server" FieldLabel="报销人" LabelWidth="50" Width="180"
                                        X="565" Y="40" SelectedIndex="0">
                                        <Items>
                                            <ext:ListItem Text="Hughson Hugang" />
                                            <ext:ListItem Text="Paul Lee" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:Button ID="Button1" runat="server" Text="查找" Width="75" X="670" Y="70">
                                    </ext:Button>
                                    <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                        Title="双击打开详细信息." TrackMouseOver="false" Height="530" AutoExpandColumn="Remark"
                                        X="10" Y="100">
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                                <ext:Column Header="临时表单号" Width="120" DataIndex="TempApplyNo" />
                                                <ext:Column Header="单据类型" Width="100" DataIndex="ApplyType" />
                                                <ext:Column Header="金额" Width="100" DataIndex="Amounts" />
                                                <ext:DateColumn Header="提交时间" Width="100" DataIndex="ApplyDate" Format="yyyy-MM-dd" />
                                                <ext:Column Header="报销单备注" Width="150" DataIndex="Remark" />
                                                <ext:Column Header="单据状态" Width="100" DataIndex="Status" />
                                                <ext:CommandColumn Width="110">
                                                    <Commands>
                                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="编辑" />
                                                        <ext:GridCommand Icon="Delete" CommandName="Delete" Text="删除" />
                                                    </Commands>
                                                </ext:CommandColumn>
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                            </ext:RowSelectionModel>
                                        </SelectionModel>
                                        <Listeners>
                                            <Command Fn="RowCommand" />
                                            <RowDblClick Fn="rowdbclick" />
                                            <RowContextMenu Handler="e.preventDefault();hdrowindex.setValue(rowIndex); #{RowContextMenu}.dataRecord = this.store.getAt(rowIndex);#{RowContextMenu}.showAt(e.getXY());" />
                                        </Listeners>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="20" DisplayInfo="true"
                                                DisplayMsg="Displaying items {0} - {1} of {2}" EmptyMsg="No items to display"
                                                HideRefresh="true">
                                                <Items>
                                                    <ext:Label ID="Label1" runat="server" Text="Page size:" />
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
                                    <ext:Menu ID="RowContextMenu" runat="server" Width="120">
                                        <Items>
                                            <ext:MenuItem ID="MenuItem1" runat="server" Text="删除" Icon="Delete">
                                                <Listeners>
                                                    <Click Handler="deleteRows(#{GridPanel1},#{RowContextMenu}.dataRecord);" />
                                                </Listeners>
                                            </ext:MenuItem>
                                        </Items>
                                    </ext:Menu>
                                </Items>
                            </ext:Panel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
            </form>
        </div>
    </div>
</body>
</html>
