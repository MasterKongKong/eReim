<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApproveDG.aspx.cs" Inherits="eReimbursement.ApproveDG" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>报销审核</title>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        var rowdbclick = function (control, rowindex, button) {
            if (Store1) {
                if (Store1.data.items[rowindex].data.Type == '交通费') {
                    Window1.setTitle('交通费');
                    Window1.loadContent({ url: 'ApplyTT.aspx', mode: 'iframe' });
                    Window1.show();
                }
                else if (Store1.data.items[rowindex].data.Type == '通讯费') {
                    Window1.setTitle('通讯费');
                    Window1.loadContent({ url: 'ApplyCC.aspx', mode: 'iframe' });
                    Window1.show();
                }
                else if (Store1.data.items[rowindex].data.Type == '其他费用') {
                    Window1.setTitle('其他费用');
                    Window1.loadContent({ url: 'ApplyOO.aspx', mode: 'iframe' });
                    Window1.show();
                }
                else if (Store1.data.items[rowindex].data.Type == '交际费') {
                    Window1.setTitle('交际费');
                    Window1.loadContent({ url: 'ApplyEE.aspx', mode: 'iframe' });
                    Window1.show();
                }
            }
        };
        var RowCommand = function (v, record, rowIndex) {
            if (record.data.Type == '交通费') {
                Window1.setTitle('交通费');
                Window1.loadContent({ url: 'ApplyTT.aspx', mode: 'iframe' });
                Window1.show();
            }
            else if (record.data.Type == '通讯费') {
                Window1.setTitle('通讯费');
                Window1.loadContent({ url: 'ApplyCC.aspx', mode: 'iframe' });
                Window1.show();
            }
            else if (record.data.Type == '其他费用') {
                Window1.setTitle('其他费用');
                Window1.loadContent({ url: 'ApplyOO.aspx', mode: 'iframe' });
                Window1.show();
            }
            else if (record.data.Type == '交际费') {
                Window1.setTitle('交际费');
                Window1.loadContent({ url: 'ApplyEE.aspx', mode: 'iframe' });
                Window1.show();
            }
        };
        var template = '<span style="color:{0};">{1}</span>';

        var change = function (value) {
            return String.format(template, (value > 57.1) ? "green" : "red", value);
        };

        var pctChange = function (value) {
            return String.format(template, (value > 0) ? "green" : "red", value + "%");
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
                        <li class="q-menuitem"><a href="Approve.aspx" id="apply">报销审核</a></li>
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
                            <ext:RecordField Name="Type" />
                            <ext:RecordField Name="DateT" Type="Date" DateFormat="yyyy-MM-dd"/>
                            <ext:RecordField Name="Person" />
                            <ext:RecordField Name="Station" />
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
                                                    <ext:TreeNode Text="待我审批" Href="Approve.aspx" Icon="FlagRed" NodeID="a1" />
                                                    <ext:TreeNode Text="我已审批" Href="Approve.aspx" Icon="FlagAd" NodeID="b1" />
                                                </Nodes>
                                            </ext:TreeNode>
                                        </Root>
                                    </ext:TreePanel>
                                </Items>
                            </ext:Panel>
                        </West>
                        <Center>
                            <ext:Panel ID="Panel3" runat="server" Title="通用费用申请单:ZJDBJS_2013010001" Height="600"
                                MinHeight="300" Padding="10" AutoScroll="true" Layout="AbsoluteLayout">
                                <Items>
                                    <ext:Label ID="Label1" runat="server" Text="申请人:" Width="55" X="10" Y="10" />
                                    <ext:Label ID="Label2" runat="server" Text="Hughson Huang" Width="150" X="65" Y="10" />
                                    <ext:Label ID="Label7" runat="server" Text="站点:" Width="35" X="235" Y="10" />
                                    <ext:Label ID="Label8" runat="server" Text="GCR" Width="150" X="280" Y="10" />
                                    <ext:Label ID="Label9" runat="server" Text="报销月度:" Width="55" X="380" Y="10" />
                                    <ext:Label ID="Label10" runat="server" Text="2012-12" Width="150" X="455" Y="10" />
                                    <ext:Label ID="Label13" runat="server" Text="总金额:" Width="55" X="555" Y="10" />
                                    <ext:Label ID="Label14" runat="server" Text="12070.21" Width="150" X="610" Y="10" />
                                    <ext:Label ID="Label3" runat="server" Text="提单人:" Width="55" X="10" Y="40" />
                                    <ext:Label ID="Label4" runat="server" Text="Hughson Huang" Width="150" X="65" Y="40" />
                                    <ext:Label ID="Label5" runat="server" Text="部门:" Width="35" X="235" Y="40" />
                                    <ext:Label ID="Label6" runat="server" Text="MIS" Width="150" X="280" Y="40" />
                                    <ext:Label ID="Label11" runat="server" Text="提单时间:" Width="55" X="380" Y="40" />
                                    <ext:Label ID="Label12" runat="server" Text="2012-12-13" Width="100" X="455" Y="40" />
                                    <ext:Label ID="Label23" runat="server" Text="附件:" Width="55" X="555" Y="40" />
                                    <ext:HyperLink ID="HyperLink1" runat="server" Text="a.pdf" X="610" Y="40" />
                                    <ext:Label ID="Label36" runat="server" Text="备注:" Width="55" X="10" Y="70" />
                                    <ext:Label ID="Label37" runat="server" Text="Noting" Width="55" X="65" Y="70" Anchor="100%" />
                                    <ext:Label ID="Label26" runat="server" Text="Created By: 2012-12-13 ( Hughson Huang ) Updated By: 2012-12-13 ( Hughson Huang )"
                                        X="10" Y="100" />
                                    <ext:TextField ID="TextField1" runat="server" FieldLabel="审批意见" X="10" Y="130" Width="770"
                                        LabelWidth="65" Anchor="100%">
                                    </ext:TextField>
                                    <ext:Button ID="Button1" runat="server" Text="预算" X="10" Y="160" Width="70">
                                        <Listeners>
                                            <Click Handler="Window2.show();" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:Button ID="Button2" runat="server" Text="同意" X="90" Y="160" Width="70">
                                    </ext:Button>
                                    <ext:Button ID="Button3" runat="server" Text="拒绝" X="170" Y="160" Width="70">
                                    </ext:Button>
                                    <ext:Button ID="Button4" runat="server" Text="加批" X="250" Y="160" Width="70">
                                    </ext:Button>
                                    <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                        Title="报销明细:双击显示详细信息." TrackMouseOver="false" Height="450" AutoExpandColumn="Person"
                                        X="10" Y="190">
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                                <ext:RowNumbererColumn Width="30" />
                                                <ext:CommandColumn Width="60">
                                                    <Commands>
                                                        <ext:GridCommand Icon="NoteGo" CommandName="View" Text="查看" />
                                                    </Commands>
                                                </ext:CommandColumn>
                                                <ext:Column Header="报销类型" Width="120" DataIndex="Type" />
                                                <ext:DateColumn Header="报销月度" Width="80" DataIndex="DateT" Format="yyyy-MM" />
                                                <ext:Column Header="报销人" Width="120" DataIndex="Person" />
                                                <ext:Column Header="站点" Width="100" DataIndex="Station" />
                                                <ext:Column Header="合计" Width="60" DataIndex="Amounts">
                                                    <Renderer Fn="change" />
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
                                                    <ext:Label ID="Label25" runat="server" Text="Page size:" />
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
                                        <Listeners>
                                            <Command Fn="RowCommand" />
                                            <RowDblClick Fn="rowdbclick" />
                                        </Listeners>
                                    </ext:GridPanel>
                                </Items>
                            </ext:Panel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
            <ext:Window ID="Window2" runat="server" Title="预算" Icon="Database" Width="600" Resizable="false"
                Frame="true" BodyStyle="background-color: #fff;" Closable="true" Height="200"
                Hidden="true" Layout="AbsoluteLayout" Collapsible="true">
                <Items>
                    <ext:Label ID="Label24" runat="server" X="10" Y="10" Text="已使用/年度预算（站点）:2000/80000">
                    </ext:Label>
                    <ext:Label ID="Label31" runat="server" X="300" Y="10" Text="已使用/月度预算（站点）:500/20000">
                    </ext:Label>
                    <ext:Label ID="Label32" runat="server" X="10" Y="40" Text="已使用/年度预算（部门）:1000/30000">
                    </ext:Label>
                    <ext:Label ID="Label33" runat="server" X="300" Y="40" Text="已使用/月度预算（部门）:400/10070">
                    </ext:Label>
                    <ext:Label ID="Label34" runat="server" X="10" Y="70" Text="已使用/年度预算（个人）:200/8000">
                    </ext:Label>
                    <ext:Label ID="Label35" runat="server" X="300" Y="70" Text="已使用/月度预算（个人）:50/No Budget">
                    </ext:Label>
                </Items>
                <Buttons>
                    <ext:Button ID="Button5" runat="server" Text="关闭" Width="70">
                        <Listeners>
                            <Click Handler="Window2.hide();" />
                        </Listeners>
                    </ext:Button>
                </Buttons>
            </ext:Window>
            <ext:Window ID="Window1" runat="server" Title="交通费" Hidden="true" Width="864" Height="483"
                Modal="True" Resizable="False" AutoScroll="true" Maximizable="True">
                <AutoLoad ShowMask="True">
                </AutoLoad>
            </ext:Window>
            </form>
        </div>
    </div>
</body>
</html>
