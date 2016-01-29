<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Charge.aspx.cs" Inherits="eReimbursement.Charge" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>费用科目</title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
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
                        <li class="q-menuitem"><a href="Profile.aspx" id="apply">基础数据</a></li>
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
                            <ext:RecordField Name="Mingcheng" />
                            <ext:RecordField Name="ValueMingcheng" />
                            <ext:RecordField Name="ValueCode" />
                            <ext:RecordField Name="Status" Type="Boolean" />
                            <ext:RecordField Name="Chuangjianri" Type="Date" DateFormat="yyyy-MM-dd" SortType="AsDate" />
                            <ext:RecordField Name="Chuangjianren" />
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
                                                    <ext:TreeNode Text="个人信息" Href="Profile.aspx" Icon="FlagCa" NodeID="a1" />
                                                    <ext:TreeNode Text="代理设置" Href="Agent.aspx" Icon="FlagCatalonia" NodeID="a2" />
                                                    <ext:TreeNode Text="费用科目" Href="Charge.aspx" Icon="FlagCc" NodeID="a3" />
                                                    <ext:TreeNode Text="报销流程" Href="Role.aspx" Icon="FlagCd" NodeID="a4" />
                                                    <ext:TreeNode Text="用户角色" Href="RoleModule.aspx" Icon="FlagCf" NodeID="a5" />
                                                </Nodes>
                                            </ext:TreeNode>
                                        </Root>
                                        <Listeners>
                                            <AfterRender Handler="TreePanel1.getNodeById('a3').select(true);" />
                                        </Listeners>
                                    </ext:TreePanel>
                                </Items>
                            </ext:Panel>
                        </West>
                        <Center>
                            <ext:Panel ID="Panel3" runat="server" Title="费用科目设置" Height="600" Padding="10" MinHeight="300"
                                AutoScroll="true" Layout="AbsoluteLayout">
                                <Items>
                                    <ext:Button ID="Button1" runat="server" Text="新增科目" X="10" Y="10" Width="70">
                                        <Listeners>
                                            <Click Handler="Window1.show();" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                        Title="双击打开详细信息." Height="550" AutoExpandColumn="ValueMingcheng" X="10" Y="40">
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                                <ext:RowNumbererColumn Width="30" />
                                                <ext:CommandColumn Width="110">
                                                    <Commands>
                                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="编辑" />
                                                        <ext:GridCommand Icon="Delete" CommandName="Delete" Text="删除" />
                                                    </Commands>
                                                </ext:CommandColumn>
                                                <ext:Column Header="科目名称" Width="90" DataIndex="Mingcheng" />
                                                <ext:Column Header="Value+ 名称" Width="140" DataIndex="ValueMingcheng" />
                                                <ext:Column Header="Value+ GL Code" Width="120" DataIndex="ValueCode" />
                                                <ext:CheckColumn Header="科目状态" DataIndex="Status" Width="60">
                                                </ext:CheckColumn>
                                                <ext:DateColumn Header="科目创建时间" Width="100" DataIndex="Chuangjianri" Format="yyyy-MM-dd" />
                                                <ext:Column Header="创建人" Width="120" DataIndex="Chuangjianren" />
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                            </ext:RowSelectionModel>
                                        </SelectionModel>
                                        <Listeners>
                                            <Command Fn="RowCommand" />
                                            <RowDblClick Fn="RowCommand" />
                                        </Listeners>
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
                                </Items>
                            </ext:Panel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
            <ext:Window ID="Window1" runat="server" Title="Detail" Icon="User" Width="480" Resizable="false"
                Draggable="false" Layout="FormLayout" BodyStyle="background-color: #fff;" Modal="true"
                Closable="true" AutoHeight="True" Hidden="true">
                <Items>
                    <ext:Panel ID="Panel6" runat="server" Height="200" Frame="true" Border="false" Layout="AbsoluteLayout">
                        <Items>
                            <ext:ComboBox ID="ComboBox2" runat="server" FieldLabel="科目名称" LabelWidth="70" Width="200"
                                X="10" Y="10" SelectedIndex="2">
                                <Items>
                                    <ext:ListItem Text="差旅费" />
                                    <ext:ListItem Text="交际费" />
                                    <ext:ListItem Text="交通费" />
                                    <ext:ListItem Text="通讯费" />
                                    <ext:ListItem Text="其他费用" />
                                </Items>
                            </ext:ComboBox>
                            <ext:TextField ID="TextField1" runat="server" FieldLabel="Value+名称" LabelWidth="70"
                                X="10" Y="40" Width="200">
                            </ext:TextField>
                            <ext:TextField ID="TextField2" runat="server" FieldLabel="Value+ GL Code" LabelWidth="100"
                                X="230" Y="10" Width="200">
                            </ext:TextField>
                            <ext:Label ID="Label1" runat="server" Text="启用状态:" Width="65" X="230" Y="43">
                            </ext:Label>
                            <ext:Checkbox ID="Checkbox1" runat="server" X="300" Y="40" Width="30">
                            </ext:Checkbox>
                            <ext:Label ID="Label2" runat="server" Text="授权创建时间:2013-01-16,创建人:Hughson Huang"
                                X="10" Y="73">
                            </ext:Label>
                            <ext:Button ID="Button2" runat="server" Text="保存" Width="75" X="10" Y="100">
                            </ext:Button>
                            <ext:Button ID="Button3" runat="server" Text="关闭" Width="75" X="95" Y="100">
                                <Listeners>
                                    <Click Handler="Window1.hide();" />
                                </Listeners>
                            </ext:Button>
                        </Items>
                    </ext:Panel>
                </Items>
            </ext:Window>
            </form>
        </div>
    </div>
</body>
</html>
