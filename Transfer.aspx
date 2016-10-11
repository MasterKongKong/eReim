﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Transfer.aspx.cs" Inherits="eReimbursement.Transfer" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>Transfer Report</title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/PageJS.js" type="text/javascript"></script>
    <link href="Styles/StyleSheet1.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        var saveData = function () {
            GridData.setValue(Ext.encode(GridPanel1.getRowsValues()));
        };
        $(document).ready(function () {
            $("div.gn_person ul.q-menubox li:eq(4) a").click(function () {
                Ext.Msg.confirm('提示', '确认登出?', function (btn, text) {
                    if (btn == 'yes') {
                        if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
                            $('div.gn_person ul.q-menubox li:eq(0) a').text('由此登录.');
                        }
                        else {
                            $('div.gn_person ul.q-menubox li:eq(0) a').text('Login here.');
                        }
                        RM.Logout({
                    });
                }
            });
        });
        $("div.gn_person ul.q-menubox li:eq(0) a").click(function () {
            loginWindow.show();
        });
    });
    $(document).ready(function () {
        //中英双语设置
        if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {

            document.title = '影像管理';
            $('div.gn_center ul.q-menubox li:eq(0) a').text('首页');
            $('div.gn_center ul.q-menubox li:eq(1) a').text('报销申请');
            $('div.gn_center ul.q-menubox li:eq(2) a').text('报销审核');
            $('div.gn_center ul.q-menubox li:eq(3) a').text('影像管理');
            $('div.gn_center ul.q-menubox li:eq(4) a').text('预算管理');
            $('div.gn_center ul.q-menubox li:eq(5) a').text('基础数据');
            var username = $('div.gn_person ul.q-menubox li:eq(0) a').text().toString();
            $('div.gn_person ul.q-menubox li:eq(0) a').text('由此登录.');
            if (username != '由此登录.' && username != 'Login here.') {
                $('div.gn_person ul.q-menubox li:eq(0) a').text(username);
            }
            $('div.gn_person ul.q-menubox li:eq(2) a').text('语言设置');
            $('div.gn_person ul.q-menubox li:eq(4) a').text('登出');
            $('div.gn_person ul.q-menubox li:eq(6) a').text('操作手册');


        }
        else {
            document.title = 'Scanning';
            $('div.gn_center ul.q-menubox li:eq(0) a').text('Home');
            $('div.gn_center ul.q-menubox li:eq(1) a').text('Apply');
            $('div.gn_center ul.q-menubox li:eq(2) a').text('Approval');
            $('div.gn_center ul.q-menubox li:eq(3) a').text('Scanning');
            $('div.gn_center ul.q-menubox li:eq(4) a').text('Budget');
            $('div.gn_center ul.q-menubox li:eq(5) a').text('Setting');
            var username = $('div.gn_person ul.q-menubox li:eq(0) a').text().toString();
            $('div.gn_person ul.q-menubox li:eq(0) a').text('Login here.');
            if (username != '由此登录.' && username != 'Login here.') {
                $('div.gn_person ul.q-menubox li:eq(0) a').text(username);
            }
            $('div.gn_person ul.q-menubox li:eq(2) a').text('Language');
            $('div.gn_person ul.q-menubox li:eq(4) a').text('Log Out');
            $('div.gn_person ul.q-menubox li:eq(6) a').text('Manual');

        }
        var ipa = "";
        if (window.location.hostname == 'localhost') {
            ipa = "http://" + window.location.host + "/MyClaims.aspx";
        }
        else {
            ipa = "http://" + window.location.host + "/eReimbursement/MyClaims.aspx";
        }
        $('div.gn_center ul.q-menubox li:eq(0) a').attr('href', ipa);
        // 单击spanAGo，调用超链接的单击事件  
        $('#language').click(function () {
            var left = $('.gn_person').offset().left + 40; var top = $('.gn_person').height();
            if ($("#Div_lang").css("visibility") != 'hidden') {
                $('#Div_lang').css({ 'top': top, 'left': left, 'visibility': 'hidden' });
            }
            else {
                $('#Div_lang').css({ 'top': top, 'left': left, 'visibility': 'visible' });
            }
        });
    });
    var RenderTree = function () {
        if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
            $('div#TreePanel1 div.x-tree-node-el a span').css('font-size', '14px');
        }
        else {
            $('div#TreePanel1 div.x-tree-node-el a span').css('font-size', '12px');
        }
        TreePanel1.getNodeById('a2').select(true);
    };

    </script>
</head>
<body>
    <form id="Form1" runat="server">
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
                        <li class="q-menuitem"><a href="FileManagement.aspx" id="apply">影像管理</a></li>
                        <li class="q-menuitem"><a href="Budget.aspx">预算管理</a></li>
                        <li class="q-menuitem"><a href="Profile.aspx">基础数据</a></li>
                    </ul>
                </div>
                <div class="gn_person">
                    <ul class="q-menubox">
                        <li class="q-menuitem"><a href="#">由此登录.</a></li><li class="q-navitem"><span>
                        </span></li>
                        <li class="q-menuitem"><a href="#" id="language">语言设置</a></li><li class="q-navitem">
                            <span></span></li>
                        <li class="q-menuitem"><a href="#">登出</a></li><li class="q-navitem">
                            <span></span></li>
                        <li class="q-menuitem"><a href="Upload/Manual.rar" target="_blank">操作手册</a></li>
                    </ul>
                </div>
            </div>
        </div>
        <div class="W_main W_profile">
            <ext:ResourceManager ID="ResourceManager1" runat="server" DirectMethodNamespace="RM" Locale="en-US"/>
            <ext:Hidden runat="server" ID="hdrowindex">
            </ext:Hidden>
            <ext:Hidden ID="GridData" runat="server" />
            <ext:Store ID="Store1" runat="server">
                <Reader>
                    <ext:JsonReader>
                        <Fields>
                            <ext:RecordField Name="FlowID" />
                            <ext:RecordField Name="No" />
                            <ext:RecordField Name="Type" />
                            <ext:RecordField Name="Station" />
                            <ext:RecordField Name="Department" />
                            <ext:RecordField Name="Person" />
                            <ext:RecordField Name="CreadedBy" />
                            <ext:RecordField Name="CreadedDate" />
                            <ext:RecordField Name="Tamount" />
                            <ext:RecordField Name="Step" />
                            <ext:RecordField Name="Status" />
                            <ext:RecordField Name="Approver" />
                            <ext:RecordField Name="ApproveDate" />
                            <ext:RecordField Name="Remark" />
                            <ext:RecordField Name="ApproverID" />
                            <ext:RecordField Name="Draft" />
                            <ext:RecordField Name="RequestID" />
                            <ext:RecordField Name="Status1" />
                            <ext:RecordField Name="Type1" />
                            <ext:RecordField Name="Draft1" />
                            <ext:RecordField Name="CostCenter" />
                            <ext:RecordField Name="CostCenterNew" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Panel ID="Panel1" runat="server" Height="680" Border="false">
                <Items>
                    <ext:BorderLayout ID="BorderLayout1" runat="server">
                        <West Collapsible="true" Split="true" CollapseMode="Mini">
                            <ext:Panel ID="Panel2" runat="server" Width="195" Layout="FitLayout">
                                <Items>
                                    <ext:TreePanel ID="TreePanel1" runat="server" Header="false" Lines="false" UseArrows="true"
                                        RootVisible="false" AutoScroll="true" Border="false">
                                        <Root>
                                            <ext:TreeNode Text="申请单" Expanded="true">
                                                <Nodes>
                                                    <ext:TreeNode Text="Scanning" Href="FileManagement.aspx"
                                                        NodeID="a1" Icon="FolderEdit"/>
                                                        <ext:TreeNode Text="Transfer Report" Href="Transfer.aspx"
                                                        NodeID="a2" Icon="FolderEdit"/>
                                                </Nodes>
                                            </ext:TreeNode>
                                        </Root>
                                        <Listeners>
                                            <AfterRender Fn="RenderTree" />
                                        </Listeners>
                                    </ext:TreePanel>
                                </Items>
                            </ext:Panel>
                        </West>
                        <Center>
                            <ext:Panel ID="Panel3" runat="server" Title="Transfer Report"
                                Height="600" MinHeight="300" Padding="10" AutoScroll="true" Layout="AbsoluteLayout">
                                <Items>
                                    <ext:TextField ID="txtNo" runat="server" FieldLabel="NO#"
                                        LabelWidth="65" Width="220" X="10" Y="10">
                                    </ext:TextField>
                                    <ext:ComboBox ID="cbxType" runat="server" FieldLabel="Claim Type"
                                        LabelWidth="65" Width="180" X="240" Y="10" DisplayField="Text" ValueField="Value" Editable="False">
                                        <Store>
                                            <ext:Store ID="StoreType" runat="server">
                                                <Reader>
                                                    <ext:JsonReader>
                                                        <Fields>
                                                            <ext:RecordField Name="Text" />
                                                            <ext:RecordField Name="Value" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <Triggers>
                                            <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                                        </Triggers>
                                        <Listeners>
                                            <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                                            <Select Handler="this.triggers[0].show();" />
                                        </Listeners>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="cbxPerson" runat="server" FieldLabel="Owner"
                                        LabelWidth="65" Width="200" X="430" Y="10" DisplayField="Text" ValueField="Value" Editable="False">
                                        <Store>
                                            <ext:Store ID="StorePerson" runat="server">
                                                <Reader>
                                                    <ext:JsonReader>
                                                        <Fields>
                                                            <ext:RecordField Name="Text" />
                                                            <ext:RecordField Name="Value" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <Triggers>
                                            <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                                        </Triggers>
                                        <Listeners>
                                            <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                                            <Select Handler="this.triggers[0].show();" />
                                        </Listeners>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="cbxStation" runat="server" FieldLabel="Unit"
                                        LabelWidth="45" Width="180" X="640" Y="10" DisplayField="Text" ValueField="Value" Editable="False">
                                        <Store>
                                            <ext:Store ID="StoreStation" runat="server">
                                                <Reader>
                                                    <ext:JsonReader>
                                                        <Fields>
                                                            <ext:RecordField Name="Text" />
                                                            <ext:RecordField Name="Value" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <Triggers>
                                            <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                                        </Triggers>
                                        <Listeners>
                                            <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                                            <Select Handler="this.triggers[0].show();" />
                                        </Listeners>
                                    </ext:ComboBox>
                                    <ext:DateField ID="dfTDate1" runat="server" FieldLabel="Approve Date"
                                        LabelWidth="90" Width="210" X="10" Y="40" Format="yyyy/MM/dd" EmptyText="yyyy/MM/dd">
                                    </ext:DateField>
                                    <ext:Label ID="Label3" runat="server" Text="--" X="225" Y="43" Width="15">
                                    </ext:Label>
                                    <ext:DateField ID="dfTDate2" runat="server" Width="105" X="240" Y="40" Format="yyyy/MM/dd"
                                        EmptyText="yyyy/MM/dd">
                                    </ext:DateField>
                                    <ext:DateField ID="DateField1" runat="server" FieldLabel="GL Date"
                                        LabelWidth="65" Width="170" X="375" Y="40" Format="yyyy/MM/dd" EmptyText="yyyy/MM/dd">
                                    </ext:DateField>
                                    <ext:Label ID="Label2" runat="server" Text="--" X="570" Y="43" Width="15">
                                    </ext:Label>
                                    <ext:DateField ID="DateField2" runat="server" Width="105" X="585" Y="40" Format="yyyy/MM/dd"
                                        EmptyText="yyyy/MM/dd">
                                    </ext:DateField>
                                    
                                    
                                    <ext:ComboBox ID="cbxSubType" runat="server" FieldLabel="Transfer Status"
                                        LabelWidth="90" Width="255" X="10" Y="70" DisplayField="Text" ValueField="Value" Editable="False">
                                        <Store>
                                            <ext:Store ID="StoreDraft" runat="server">
                                                <Reader>
                                                    <ext:JsonReader>
                                                        <Fields>
                                                            <ext:RecordField Name="Text" />
                                                            <ext:RecordField Name="Value" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <Triggers>
                                            <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                                        </Triggers>
                                        <Listeners>
                                            <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                                            <Select Handler="this.triggers[0].show();" />
                                        </Listeners>
                                    </ext:ComboBox>


                                    <ext:Button ID="btnSearch" runat="server" Text="Search"
                                        Width="75" X="275" Y="70" Icon="Magnifier">
                                        <DirectEvents>
                                            <%--<Click OnEvent="Search" Before="btnSearch.disable();" Success="btnSearch.enable();"
                                                Timeout="300000">
                                                <EventMask ShowMask="true" Target="Page" />
                                            </Click>--%>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnExport" runat="server" Text="Export"
                                        AutoPostBack="true" Width="75" X="360" Y="70" Enabled="false">
                                        <Listeners>
                                            
                                        </Listeners>
                                    </ext:Button>
                                    <ext:Panel ID="Panel4" runat="server" Height="540" X="10" Y="100" Layout="FitLayout"
                                        Border="false">
                                        <Items>
                                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                                Title="<%$ Resources:LocalText,ScanningGridTitle%>" TrackMouseOver="false" AutoScroll="true">
                                                <ColumnModel ID="ColumnModel1" runat="server">
                                                    <Columns>
                                                        <ext:RowNumbererColumn Width="30" />
                                                        <ext:Column Header="NO#" Width="140" DataIndex="No" />
                                                        <ext:Column Header="Claim Type" Width="80" DataIndex="Type1" />
                                                        <ext:Column Header="Owner" Width="120" DataIndex="Person" />
                                                        <ext:Column Header="Currency" Width="120" DataIndex="CostCenter" />
                                                        <ext:Column Header="Amount" Width="120" DataIndex="CostCenterNew" />
                                                        <ext:Column Header="Approve Date" Width="80" DataIndex="Status1" />
                                                        <ext:Column Header="Status" Width="120" DataIndex="Approver" />
                                                        <ext:DateColumn Header="GL Date" Width="80" DataIndex="CreadedDate"
                                                            Format="yyyy/MM/dd" />
                                                        <ext:Column Header="<%$ Resources:LocalText,Remark%>" Width="150" DataIndex="Remark" />
                                                    </Columns>
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true" />
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
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
            <ext:Window ID="loginWindow" runat="server" Title="Login" Icon="User" Width="300px"
                Resizable="false" Draggable="false" Layout="Form" BodyStyle="background-color: #fff;"
                Padding="5" Modal="true" Closable="False" AutoHeight="True" Hidden="true">
                <Items>
                    <ext:TextField ID="tfUserID" runat="server" FieldLabel="UserID" AutoFocus="True"
                        AllowBlank="false" MsgTarget="Side" BlankText="Your UserID is required." Text="" />
                    <ext:TextField ID="tfPW" runat="server" FieldLabel="Password" InputType="Password"
                        AllowBlank="false" MsgTarget="Side" BlankText="Your Password is required." Text="" />
                </Items>
                <Buttons>
                    <ext:Button ID="btnOK" runat="server" Text="OK">
                        <Listeners>
                            <Click Handler="if (!#{tfUserID}.validate() || !#{tfPW}.validate()) {
                                Ext.Msg.alert('Error','The UserID and Password fields are both required');
                                return false; 
                            }" />
                        </Listeners>
                        <DirectEvents>
                            <%--<Click OnEvent="btnLogin_Click" />--%>
                        </DirectEvents>
                    </ext:Button>
                    <ext:Button ID="btnCancel" runat="server" Text="Cancel">
                        <Listeners>
                            <Click Handler="#{loginWindow}.hide();#{tfUserID}.reset();#{tfPW}.reset();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnReset" runat="server" Text="Reset">
                        <Listeners>
                            <Click Handler="#{tfUserID}.reset();#{tfPW}.reset();" />
                        </Listeners>
                    </ext:Button>
                </Buttons>
                <Listeners>
                    <Hide Handler="KeyNav1.disable();" />
                    <Show Handler="KeyNav1.enable();" />
                </Listeners>
            </ext:Window>
            <ext:KeyNav ID="KeyNav1" runat="server" Target="={document.body}">
                <Enter Handler="if(!loginWindow.hidden){btnOK.fireEvent('click');}" />
            </ext:KeyNav>
            <div id="Div_lang" style="position: absolute; z-index: 9013; visibility: hidden;
                left: -10000px; top: -10000px; width: 200px; display: block;">
                <ext:Toolbar ID="Toolbar2" runat="server" Width="160" Flat="true">
                    <Items>
                        <ext:Button ID="Button9" runat="server" Icon="FlagUs" Text="English" Width="70">
                            <Listeners>
                                <Click Fn="SetCookie" />
                            </Listeners>
                        </ext:Button>
                        <ext:Button ID="Button10" runat="server" Icon="FlagCn" Text="中文" Width="70">
                            <Listeners>
                                <Click Fn="SetCookie" />
                            </Listeners>
                        </ext:Button>
                    </Items>
                </ext:Toolbar>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
