<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="eFMSAccount.aspx.cs" Inherits="eReimbursement.eFMSAccount" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>eFMS记账</title>
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
        var CheckRows = function () {
            var checkitem = CheckboxSelectionModel1.selections;
            var sumno = 0, sumamount = 0;
            for (var i = 0; i < CheckboxSelectionModel1.selections.length; i++) {
                if (checkitem.items[i].data.Amounts && checkitem.items[i].data.Amounts > 0) {
                    sumno++; sumamount += parseFloat(checkitem.items[i].data.Amounts);
                }
            }
            GridPanel2.setTitle("勾选后保存单据.已选择明细张数合计:" + sumno.toString() + ",金额合计:" + Math.round(sumamount*100)/100);
        };
        var Fromformat = function (value) {
            var rem = '<input id="Button1" type="text" value="{0}" style="width:100%"/>';
            return String.format(rem, value);
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
                    <ext:ArrayReader IDProperty="TempApplyNo">
                        <Fields>
                            <ext:RecordField Name="TempApplyNo" />
                            <ext:RecordField Name="ApplyType" />
                            <ext:RecordField Name="Dept" />
                            <ext:RecordField Name="ApplyPerson" />
                            <ext:RecordField Name="ApplyPersonProxy" />
                            <ext:RecordField Name="Amounts" />
                            <ext:RecordField Name="ApplyDate" Type="Date" DateFormat="yyyy-MM-dd" SortType="AsDate" />
                            <ext:RecordField Name="Remark" />
                            <ext:RecordField Name="Status" />
                            <ext:RecordField Name="CurrentApprovePerson" />
                        </Fields>
                    </ext:ArrayReader>
                </Reader>
            </ext:Store>
            <ext:Store ID="Store2" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID">
                        <Fields>
                            <ext:RecordField Name="ID" />
                            <ext:RecordField Name="DateT" Type="Date" DateFormat="yyyy-MM-dd" SortType="AsDate" />
                            <ext:RecordField Name="From" />
                            <ext:RecordField Name="To" />
                            <ext:RecordField Name="Purpose" />
                            <ext:RecordField Name="Amounts" />
                        </Fields>
                    </ext:JsonReader>
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
                                            <ext:TreeNode Text="eFMS记账" Expanded="true">
                                                <Nodes>
                                                    <ext:TreeNode Text="付款操作" Href="eFMSAccount.aspx" Icon="FlagRed" NodeID="a1" />
                                                    <ext:TreeNode Text="单据传输" Href="eFMSTrans.aspx" Icon="FlagAd" NodeID="b1" />
                                                </Nodes>
                                            </ext:TreeNode>
                                        </Root>
                                        <Listeners>
                                            <AfterRender Handler="TreePanel1.getNodeById('a1').select(true);" />
                                        </Listeners>
                                    </ext:TreePanel>
                                </Items>
                            </ext:Panel>
                        </West>
                        <Center>
                            <ext:Panel ID="Panel3" runat="server" Height="600" MinHeight="300" Padding="10" AutoScroll="true"
                                Title="付款操作" Layout="AbsoluteLayout">
                                <Items>
                                    <ext:TextField ID="TextField1" runat="server" FieldLabel="表单号" LabelWidth="80" Width="220"
                                        X="10" Y="10">
                                    </ext:TextField>
                                    <ext:ComboBox ID="ComboBox2" runat="server" FieldLabel="单据类型" LabelWidth="65" Width="180"
                                        X="240" Y="10" SelectedIndex="2">
                                        <Items>
                                            <ext:ListItem Text="差旅费" />
                                            <ext:ListItem Text="交际费" />
                                            <ext:ListItem Text="交通费" />
                                            <ext:ListItem Text="通讯费" />
                                            <ext:ListItem Text="其他费用" />
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
                                    <ext:ComboBox ID="ComboBox3" runat="server" FieldLabel="申请人" LabelWidth="80" Width="220"
                                        X="10" Y="70" SelectedIndex="0">
                                        <Items>
                                            <ext:ListItem Text="Hughson Huang" />
                                            <ext:ListItem Text="Paul Lee" />
                                            <ext:ListItem Text="Andy Kang" />
                                            <ext:ListItem Text="Sunhui Chen" />
                                            <ext:ListItem Text="Robin Li" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="ComboBox4" runat="server" FieldLabel="提单人" LabelWidth="65" Width="220"
                                        X="240" Y="70" SelectedIndex="0">
                                        <Items>
                                            <ext:ListItem Text="Hughson Huang" />
                                            <ext:ListItem Text="Paul Lee" />
                                            <ext:ListItem Text="Andy Kang" />
                                            <ext:ListItem Text="Sunhui Chen" />
                                            <ext:ListItem Text="Robin Li" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="ComboBox1" runat="server" FieldLabel="单据状态" LabelWidth="65" Width="175"
                                        X="470" Y="70" SelectedIndex="3">
                                        <Items>
                                            <ext:ListItem Text="部门审批" />
                                            <ext:ListItem Text="单据审核" />
                                            <ext:ListItem Text="核准人审批" />
                                            <ext:ListItem Text="银行付款" />
                                            <ext:ListItem Text="Value+归档" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:Button ID="Button1" runat="server" Text="查找" Width="75" X="670" Y="70">
                                    </ext:Button>
                                    <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                        Title="点击单行在下方查看明细." TrackMouseOver="false" Height="200" AutoExpandColumn="TempApplyNo"
                                        X="10" Y="100">
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                                <ext:RowNumbererColumn Width="30" />
                                                <%--<ext:TemplateColumn Header="表单号" Width="140">
                                                    <Template ID="Template1" runat="server">
                                                        <Html>
                                                            <input id="{TempApplyNo}" type="text" value="{TempApplyNo}" style="width: 100%"></input>
                                                        </Html>
                                                    </Template>
                                                </ext:TemplateColumn>
                                                <ext:TemplateColumn Header="单据类型" Width="80">
                                                    <Template ID="Template2" runat="server">
                                                        <Html>
                                                            <select id="{ApplyType}" style="width: 100%">
                                                                <option selected="selected">交通费</option>
                                                                <option>交际费</option>
                                                            </select>
                                                        </Html>
                                                    </Template>
                                                </ext:TemplateColumn>--%>
                                                <ext:Column Header="表单号" Width="120" DataIndex="TempApplyNo" />
                                                <ext:Column Header="单据类型" Width="70" DataIndex="ApplyType" />
                                                <ext:Column Header="部门" Width="50" DataIndex="Dept" />
                                                <ext:Column Header="申请人" Width="100" DataIndex="ApplyPerson" />
                                                <ext:Column Header="提单人" Width="100" DataIndex="ApplyPersonProxy" />
                                                <ext:Column Header="金额" Width="60" DataIndex="Amounts" />
                                                <ext:DateColumn Header="提交时间" Width="80" DataIndex="ApplyDate" Format="yyyy-MM-dd" />
                                                <ext:Column Header="报销单备注" Width="100" DataIndex="Remark" />
                                                <ext:Column Header="单据状态" Width="70" DataIndex="Status" />
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                                <Listeners>
                                                    <RowSelect Handler="GridPanel2.show();Panel4.show();" />
                                                </Listeners>
                                                <DirectEvents>
                                                    <RowSelect OnEvent="ChangeGrid">
                                                        <ExtraParams>
                                                            <ext:Parameter Name="rowid" Value="GridPanel1.getSelectionModel().selections.items[0].id"
                                                                Mode="Raw">
                                                            </ext:Parameter>
                                                        </ExtraParams>
                                                        <EventMask Target="CustomTarget" CustomTarget="GridPanel2" ShowMask="true" />
                                                    </RowSelect>
                                                </DirectEvents>
                                            </ext:RowSelectionModel>
                                        </SelectionModel>
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
                                    <ext:GridPanel ID="GridPanel2" runat="server" StoreID="Store2" StripeRows="true"
                                        Hidden="true" Title="勾选后保存单据." TrackMouseOver="false" Height="200" AutoExpandColumn="Purpose"
                                        X="10" Y="300">
                                        <ColumnModel ID="ColumnModel2" runat="server">
                                            <Columns>
                                                <ext:RowNumbererColumn Width="30" />
                                                <ext:DateColumn Header="日期" Width="80" DataIndex="DateT" Format="yyyy-MM-dd" />
                                                <ext:Column Header="起始地" Width="200" DataIndex="From">
                                                </ext:Column>
                                                <ext:Column Header="目的地" Width="200" DataIndex="To">
                                                </ext:Column>
                                                <ext:Column Header="费用描述" Width="100" DataIndex="Purpose" />
                                                <ext:Column Header="金额" Width="60" DataIndex="Amounts">
                                                </ext:Column>
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:CheckboxSelectionModel ID="CheckboxSelectionModel1" runat="server">
                                                <Listeners>
                                                    <RowSelect Fn="CheckRows" />
                                                    <RowDeselect Fn="CheckRows" />
                                                </Listeners>
                                            </ext:CheckboxSelectionModel>
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
                                            <ext:ComboBox ID="ComboBox6" runat="server" FieldLabel="银行" LabelWidth="70" Width="205"
                                                X="10" Y="40" SelectedIndex="0">
                                                <Items>
                                                    <ext:ListItem Text="中国银行" />
                                                    <ext:ListItem Text="招商银行" />
                                                    <ext:ListItem Text="浦东发展银行" />
                                                    <ext:ListItem Text="中菲行" />
                                                </Items>
                                            </ext:ComboBox>
                                            <ext:ComboBox ID="ComboBox7" runat="server" FieldLabel="付款方式" LabelWidth="70" Width="205"
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
                                    </ext:Panel>
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
