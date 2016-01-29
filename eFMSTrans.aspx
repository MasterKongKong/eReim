<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="eFMSTrans.aspx.cs" Inherits="eReimbursement.eFMSTrans" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>单据传输</title>
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
        var rowdbclick = function (a, b, c) {
            window.open("ApproveTransportation.aspx");
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
                        <li class="q-menuitem"><a href="MyClaims.aspx">报销申请</a></li>
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
                            <ext:RecordField Name="Kemu" />
                            <ext:RecordField Name="Yinhang" />
                            <ext:RecordField Name="Fukuan" />
                            <ext:RecordField Name="ApplyPersonProxy" />
                            <ext:RecordField Name="Amounts" />
                            <ext:RecordField Name="ApplyDate" Type="Date" DateFormat="yyyy-MM-dd" SortType="AsDate" />
                            <ext:RecordField Name="Status" />
                        </Fields>
                    </ext:ArrayReader>
                </Reader>
            </ext:Store>
            <ext:Store ID="Store2" runat="server">
                <Reader>
                    <ext:ArrayReader>
                        <Fields>
                            <ext:RecordField Name="DateT" Type="Date" DateFormat="yyyy-MM-dd" SortType="AsDate" />
                            <ext:RecordField Name="From" />
                            <ext:RecordField Name="To" />
                            <ext:RecordField Name="Purpose" />
                            <ext:RecordField Name="Amounts" />
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
                                                    <ext:TreeNode Text="付款操作" Href="eFMS.aspx" Icon="FlagRed" NodeID="a1" />
                                                    <ext:TreeNode Text="单据传输" Href="eFMSTrans.aspx" Icon="FlagAd" NodeID="b1" />
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
                            <ext:Panel ID="Panel3" runat="server" Title="单据传输" Height="600" MinHeight="300" Padding="10"
                                AutoScroll="true" Layout="AbsoluteLayout">
                                <Items>
                                    <ext:TextField ID="TextField1" runat="server" FieldLabel="凭证号" LabelWidth="65" Width="220"
                                        X="10" Y="10">
                                    </ext:TextField>
                                    <ext:ComboBox ID="ComboBox2" runat="server" FieldLabel="科目" LabelWidth="65" Width="180"
                                        X="240" Y="10" SelectedIndex="0">
                                        <Items>
                                            <ext:ListItem Text="11020201" />
                                            <ext:ListItem Text="12020201" />
                                            <ext:ListItem Text="13020201" />
                                            <ext:ListItem Text="13030201" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:TextField ID="TextField2" runat="server" FieldLabel="金额" LabelWidth="30" Width="155"
                                        X="440" Y="10">
                                    </ext:TextField>
                                    <ext:Label ID="Label2" runat="server" Text="--" X="603" Y="13" Width="15">
                                    </ext:Label>
                                    <ext:TextField ID="TextField3" runat="server" Width="125" X="620" Y="10">
                                    </ext:TextField>
                                    <ext:TextField ID="TextField4" runat="server" FieldLabel="摘要" LabelWidth="65" Width="220"
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
                                    <ext:ComboBox ID="ComboBox3" runat="server" FieldLabel="凭证状态" LabelWidth="65" Width="220"
                                        X="10" Y="70" SelectedIndex="0">
                                        <Items>
                                            <ext:ListItem Text="Posted" />
                                            <ext:ListItem Text="NotPosted" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="ComboBox6" runat="server" FieldLabel="银行" LabelWidth="65" Width="205"
                                        X="240" Y="70" SelectedIndex="0">
                                        <Items>
                                            <ext:ListItem Text="中国银行" />
                                            <ext:ListItem Text="招商银行" />
                                            <ext:ListItem Text="浦东发展银行" />
                                            <ext:ListItem Text="中菲行" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="ComboBox7" runat="server" FieldLabel="付款方式" LabelWidth="65" Width="205"
                                        X="455" Y="70" SelectedIndex="0">
                                        <Items>
                                            <ext:ListItem Text="现金" />
                                            <ext:ListItem Text="支票" />
                                            <ext:ListItem Text="信用卡" />
                                            <ext:ListItem Text="支付宝" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:Button ID="Button1" runat="server" Text="查找" Width="75" X="670" Y="70">
                                    </ext:Button>
                                    <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                        Title="点击按钮直接批准或者双击打开详细信息." TrackMouseOver="false" Height="530" AutoExpandColumn="TempApplyNo"
                                        X="10" Y="100">
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                                <ext:RowNumbererColumn Width="30" />
                                                <ext:CommandColumn Width="60" ColumnID="commandColumn">
                                                    <Commands>
                                                        <ext:GridCommand Icon="PillGo" CommandName="Post" Text="Post" />
                                                    </Commands>
                                                </ext:CommandColumn>
                                                <ext:Column Header="凭证号" Width="120" DataIndex="TempApplyNo" />
                                                <ext:Column Header="科目" Width="70" DataIndex="Kemu" />
                                                <ext:Column Header="银行" Width="70" DataIndex="Yinhang" />
                                                <ext:Column Header="付款方式" Width="70" DataIndex="Fukuan" />
                                                <ext:Column Header="提单人" Width="100" DataIndex="ApplyPersonProxy" />
                                                <ext:Column Header="金额" Width="60" DataIndex="Amounts" />
                                                <ext:DateColumn Header="提交时间" Width="80" DataIndex="ApplyDate" Format="yyyy-MM-dd" />
                                                <ext:Column Header="凭证状态" Width="70" DataIndex="Status" />
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                            </ext:RowSelectionModel>
                                        </SelectionModel>
                                        <Listeners>
                                            <Command Handler="GridPanel2.show();Panel4.show();" />
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
                                    <%--<ext:GridPanel ID="GridPanel2" runat="server" StoreID="Store2" StripeRows="true"
                                        Hidden="true" Title="勾选后保存单据.报销张数合计:13 金额报销合计:12070.21." TrackMouseOver="false"
                                        Height="200" AutoExpandColumn="Purpose" X="10" Y="300">
                                        <ColumnModel ID="ColumnModel2" runat="server">
                                            <Columns>
                                                <ext:RowNumbererColumn Width="30" />
                                                <ext:DateColumn ColumnID="Date" Header="日期" Width="80" DataIndex="DateT" Format="yyyy-MM-dd" />
                                                <ext:Column ColumnID="From" Header="起始地" Width="200" DataIndex="From" />
                                                <ext:Column ColumnID="To" Header="目的地" Width="200" DataIndex="To" />
                                                <ext:Column ColumnID="Purpose" Header="费用描述" Width="100" DataIndex="Purpose" />
                                                <ext:Column ColumnID="Amounts" Header="金额" Width="60" DataIndex="Amounts">
                                                </ext:Column>
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:CheckboxSelectionModel ID="CheckboxSelectionModel1" runat="server" />
                                        </SelectionModel>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolbar2" runat="server" PageSize="20" DisplayInfo="true"
                                                DisplayMsg="Displaying items {0} - {1} of {2}" EmptyMsg="No items to display"
                                                HideRefresh="true">
                                                <Items>
                                                    <ext:Label ID="Label4" runat="server" Text="Page size:" />
                                                    <ext:ToolbarSpacer ID="ToolbarSpacer2" runat="server" Width="10" />
                                                    <ext:ComboBox ID="ComboBox5" runat="server" Width="80">
                                                        <Items>
                                                            <ext:ListItem Text="10" />
                                                            <ext:ListItem Text="20" />
                                                            <ext:ListItem Text="50" />
                                                            <ext:ListItem Text="100" />
                                                            <ext:ListItem Text="200" />
                                                        </Items>
                                                        <SelectedItem Value="20" />
                                                        <Listeners>
                                                            <Select Handler="#{PagingToolbar2}.pageSize = parseInt(this.getValue()); #{PagingToolbar2}.doLoad();" />
                                                        </Listeners>
                                                    </ext:ComboBox>
                                                </Items>
                                            </ext:PagingToolbar>
                                        </BottomBar>
                                    </ext:GridPanel>
                                    <ext:Panel ID="Panel4" runat="server" X="10" Y="500" Height="150" Layout="AbsoluteLayout"
                                        Hidden="true" Padding="10">
                                        <Items>
                                            <ext:TextField ID="txtAmounts" runat="server" FieldLabel="凭证号" X="10" Y="10" Width="205"
                                                LabelWidth="70" />
                                            <ext:TextField ID="txtFrom" runat="server" FieldLabel="科目" X="225" Y="10" Width="400"
                                                LabelWidth="70">
                                            </ext:TextField>
                                            <ext:ComboBox ID="ComboBox1" runat="server" FieldLabel="银行" LabelWidth="70" Width="205"
                                                X="10" Y="40" SelectedIndex="0">
                                                <Items>
                                                    <ext:ListItem Text="中国银行" />
                                                    <ext:ListItem Text="招商银行" />
                                                    <ext:ListItem Text="浦东发展银行" />
                                                    <ext:ListItem Text="中菲行" />
                                                </Items>
                                            </ext:ComboBox>
                                            <ext:ComboBox ID="ComboBox4" runat="server" FieldLabel="付款方式" LabelWidth="70" Width="205"
                                                X="225" Y="40" SelectedIndex="0">
                                                <Items>
                                                    <ext:ListItem Text="现金" />
                                                    <ext:ListItem Text="支票" />
                                                    <ext:ListItem Text="信用卡" />
                                                    <ext:ListItem Text="支付宝" />
                                                </Items>
                                            </ext:ComboBox>
                                            <ext:Label ID="Label5" runat="server" Text="Air" Width="30" X="480" Y="43">
                                            </ext:Label>
                                            <ext:Checkbox ID="Checkbox1" runat="server" Width="30" X="510" Y="40">
                                            </ext:Checkbox>
                                            <ext:Label ID="Label6" runat="server" Text="Ocean" Width="30" X="565" Y="43">
                                            </ext:Label>
                                            <ext:Checkbox ID="Checkbox2" runat="server" Width="30" X="615" Y="40">
                                            </ext:Checkbox>
                                            <ext:TextField ID="txtTo1" runat="server" FieldLabel="摘要" X="10" Y="70" LabelWidth="70"
                                                Anchor="100%" EmptyText="默认为报销单号+数字+人名+项目">
                                            </ext:TextField>
                                            <ext:Button ID="Button5" runat="server" Text="保存" X="10" Y="100" Width="75">
                                            </ext:Button>
                                            <ext:Button ID="Button6" runat="server" Text="放弃" X="95" Y="100" Width="75">
                                            </ext:Button>
                                        </Items>
                                    </ext:Panel>--%>
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
