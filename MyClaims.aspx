<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyClaims.aspx.cs" Inherits="eReimbursement.MyClaims" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>我的报销单</title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
    <link href="Styles/StyleSheet1.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/PageJS.js" type="text/javascript"></script>
    <link href="Styles/StyleSheetFlow.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(document).ready(function () {
            $("div.gn_person ul.q-menubox li:eq(4) a").click(function () {
                Ext.Msg.confirm('Message', 'Confirm to logout?', function (btn, text) {
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
        var prepare = function (grid, toolbar, rowIndex, record) {
            var firstButton = toolbar.items.get(0).menu.items.get(1);
            if (record.data.Draft != '1' && record.data.Status != '3') {
                firstButton.setDisabled(true);
            }
        };

        var saveData = function () {
            GridData.setValue(Ext.encode(GridPanel1.getRowsValues()));
        };
        var SetGridPanel1 = function (a, b) {
            var pos = Panel4.getPosition();
            if (pos[1] != 200) {
                Panel4.setPosition(10, 200);
                Panel4.setHeight(440);
                Panel5.show();
            }
        };
        var editdetail = function (command, record, rowIndex) {
            if (command == 'View') {
                if (record.data.Type == 'T') {
                    //                    if (parseInt(record.data.RequestID) > 170) {
                    //                        window.open("Travel.aspx?ID=" + record.data.RequestID.toString());
                    //                    }
                    //                    else {
                    //                        window.open("ApplyTravel.aspx?ID=" + record.data.RequestID.toString());
                    //                    }
                    window.open("Travel.aspx?ID=" + record.data.RequestID.toString());
                }
                else {
                    window.open("Apply.aspx?ID=" + record.data.RequestID.toString());
                }
            }
            else if (command == 'Copy') {
                if (record.data.Type == 'T') {
                    //                    if (parseInt(record.data.RequestID) > 170) {
                    //                        window.open("Travel.aspx?ID=" + record.data.RequestID.toString() + "&Copy=T");
                    //                    }
                    //                    else {
                    //                        window.open("ApplyTravel.aspx?ID=" + record.data.RequestID.toString() + "&Copy=T");
                    //                    }
                    window.open("Travel.aspx?ID=" + record.data.RequestID.toString() + "&Copy=T");
                }
                else {
                    window.open("Apply.aspx?ID=" + record.data.RequestID.toString() + "&Copy=T");
                }
            }
            else if (command == 'CopyAll') {
                if (record.data.Type == 'T') {
                    //                    if (parseInt(record.data.RequestID) > 170) {
                    //                        window.open("Travel.aspx?ID=" + record.data.RequestID.toString() + "&Copy=T&CopyType=A");
                    //                    }
                    //                    else {
                    //                        window.open("ApplyTravel.aspx?ID=" + record.data.RequestID.toString() + "&Copy=T&CopyType=A");
                    //                    }
                    window.open("Travel.aspx?ID=" + record.data.RequestID.toString() + "&Copy=T&CopyType=A");
                }
                else {
                    window.open("Apply.aspx?ID=" + record.data.RequestID.toString() + "&Copy=T&CopyType=A");
                }
            }
            else if (command == 'Delete') {
                Ext.Msg.confirm('Message', 'Are you sure?', function (btn, text) {
                    if (btn == 'yes') {
                        Store1.removeAt(rowIndex.toString());
                        RM.Delete(record.data.RequestID.toString(), record.data.Type, rowIndex.toString(), {
                            timeout: 300000
                        });
                    }
                });
            }
        };
        var rowdbclick = function (control, rowindex, button) {
            if (Store1) {
                if (Store1.data.items[rowindex].data.Type == 'T') {
                    if (parseInt(Store1.data.items[rowindex].data.RequestID) > 170) {
                        window.open("Travel.aspx?ID=" + Store1.data.items[rowindex].data.RequestID.toString());
                    }
                    else {
                        window.open("ApplyTravel.aspx?ID=" + Store1.data.items[rowindex].data.RequestID.toString());
                    }
                }
                else {
                    window.open("Apply.aspx?ID=" + Store1.data.items[rowindex].data.RequestID.toString());
                }
            }
        };


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
                    Store1.remove(record);
                }

                //return focus
                grid.view.focusEl.focus();
            })
        };
        var setFlow = function (a) {
            $('#divFlow').html(a);
        };
        $(document).ready(function () {
            //中英双语设置
            if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {

                document.title = '我的报销单';
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
                document.title = 'My Reimbursement';
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
            TreePanel1.getNodeById('c1').select(true);
            var ipa = "http://" + window.location.host + "/eReimbursement_Old/MyClaims.aspx";
            TreePanel1.getNodeById('d1').setHref(ipa);
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
                        <li class="q-menuitem"><a href="MyClaims.aspx" id="apply">报销申请</a></li>
                        <li class="q-menuitem"><a href="Approve.aspx">报销审核</a></li>
                        <li class="q-menuitem"><a href="FileManagement.aspx">影像管理</a></li>
                        <li class="q-menuitem"><a href="Budget.aspx">预算管理</a></li>
                        <li class="q-menuitem"><a href="Profile.aspx">基础数据</a></li>
                    </ul>
                </div>
                <div class="gn_person">
                    <ul class="q-menubox">
                        <li class="q-menuitem"><a href="#">由此登录.</a></li><li class="q-navitem"><span></span>
                        </li>
                        <li class="q-menuitem"><a href="#" id="language">语言设置</a></li><li class="q-navitem">
                            <span></span></li>
                        <li class="q-menuitem"><a href="#">登出</a></li><li class="q-navitem"><span></span>
                        </li>
                        <li class="q-menuitem"><a href="Upload/Manual.rar" target="_blank">操作手册</a></li>
                    </ul>
                </div>
            </div>
        </div>
        <div class="W_main W_profile">
            <ext:ResourceManager ID="ResourceManager1" runat="server" DirectMethodNamespace="RM"
                Locale="en-US" />
            <ext:Hidden ID="GridData" runat="server" />
            <ext:Hidden runat="server" ID="hdrowindex">
            </ext:Hidden>
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
                            <ext:RecordField Name="ApplyDate" />
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
                                                    <ext:TreeNode Text="<%$ Resources:LocalText,ExpenseClaim%>" Expanded="true" Href="#"
                                                        Icon="Money">
                                                        <Nodes>
                                                            <ext:TreeNode Text="<%$ Resources:LocalText,TravellingExpenseApply%>" Href="Travel.aspx"
                                                                NodeID="a1" Icon="PageWhiteAdd" />
                                                            <ext:TreeNode Text="<%$ Resources:LocalText,GeneralExpenseApply%>" Href="Apply.aspx"
                                                                NodeID="a2" Icon="PageWhiteAdd" />
                                                        </Nodes>
                                                    </ext:TreeNode>
                                                    <ext:TreeNode Text="<%$ Resources:LocalText,MyExpense%>" Href="MyClaims.aspx"
                                                        NodeID="c1" Icon="Table" />
                                                    <ext:TreeNode Text="<%$ Resources:LocalText,MyExpenseOld%>" 
                                                    NodeID="d1" Icon="Table" />
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
                            <ext:Panel ID="Panel3" runat="server" Title="<%$ Resources:LocalText,Reimbursement%>"
                                Height="600" MinHeight="300" Padding="10" AutoScroll="true" Layout="AbsoluteLayout">
                                <Items>
                                    <ext:TextField ID="txtNo" runat="server" FieldLabel="<%$ Resources:LocalText,NO%>"
                                        LabelWidth="80" Width="220" X="10" Y="10">
                                    </ext:TextField>
                                    <ext:ComboBox ID="cbxType" runat="server" FieldLabel="<%$ Resources:LocalText,ClaimType%>"
                                        LabelWidth="65" Width="205" X="240" Y="10" DisplayField="Text" ValueField="Value"
                                        Editable="False">
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
                                    <ext:TextField ID="txtAmount1" runat="server" FieldLabel="<%$ Resources:LocalText,Amount%>"
                                        LabelWidth="50" Width="150" X="465" Y="10">
                                    </ext:TextField>
                                    <ext:Label ID="Label2" runat="server" Text="--" X="625" Y="13" Width="15">
                                    </ext:Label>
                                    <ext:TextField ID="txtAmount2" runat="server" Width="100" X="640" Y="10">
                                    </ext:TextField>
                                    <ext:TextField ID="txtRemark" runat="server" FieldLabel="<%$ Resources:LocalText,Remark%>"
                                        LabelWidth="80" Width="220" X="10" Y="40">
                                    </ext:TextField>
                                    <ext:DateField ID="dfTDate1" runat="server" FieldLabel="<%$ Resources:LocalText,SubmitDate%>"
                                        LabelWidth="75" Width="175" X="235" Y="40" Format="yyyy/MM/dd" EmptyText="yyyy/MM/dd">
                                    </ext:DateField>
                                    <ext:Label ID="Label3" runat="server" Text="-" X="415" Y="43" Width="15">
                                    </ext:Label>
                                    <ext:DateField ID="dfTDate2" runat="server" Width="100" X="425" Y="40" Format="yyyy/MM/dd"
                                        EmptyText="yyyy/MM/dd">
                                    </ext:DateField>
                                    <ext:ComboBox ID="cbxSubType" runat="server" FieldLabel="<%$ Resources:LocalText,Status%>"
                                        LabelWidth="65" Width="175" X="535" Y="40" DisplayField="Text" ValueField="Value"
                                        Editable="False">
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
                                    <ext:ComboBox ID="cbxPerson" runat="server" FieldLabel="<%$ Resources:LocalText,Owner%>"
                                        LabelWidth="80" Width="220" X="10" Y="70" Editable="false">
                                        <Triggers>
                                            <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                                        </Triggers>
                                        <Listeners>
                                            <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                                            <Select Handler="if(this.getStore().getRange().length>1){this.triggers[0].show();}" />
                                        </Listeners>
                                    </ext:ComboBox>
                                    <ext:Button ID="btnSearch" runat="server" Text="<%$ Resources:LocalText,Search%>"
                                        Width="100" X="500" Y="70" Icon="Magnifier">
                                        <DirectEvents>
                                            <Click OnEvent="Search" Before="btnSearch.disable();Panel5.hide();btnExport.disable();"
                                                Success="btnSearch.enable();btnExport.enable();" Timeout="300000">
                                                <EventMask ShowMask="true" CustomTarget="GridPanel1" Target="CustomTarget" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnExport" runat="server" Text="<%$ Resources:LocalText,Export%>"
                                        OnClick="btnExport_Click" AutoPostBack="true" Width="100" X="610" Y="70" Enabled="false">
                                        <Listeners>
                                            <Click Fn="saveData" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:Panel ID="Panel5" runat="server" X="10" Y="110" Height="90" Border="false">
                                        <Items>
                                            <ext:Container ID="Container2" runat="server" AutoScroll="true">
                                                <Content>
                                                    <div id="divFlow">
                                                    </div>
                                                </Content>
                                            </ext:Container>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel4" runat="server" Height="540" X="10" Y="100" Layout="FitLayout"
                                        Border="false">
                                        <Items>
                                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                                Title="<%$ Resources:LocalText,MyClaimsTitle%>" TrackMouseOver="false" AutoScroll="true">
                                                <ColumnModel ID="ColumnModel1" runat="server">
                                                    <Columns>
                                                        <ext:RowNumbererColumn Width="30" />
                                                        <ext:CommandColumn Width="70" ButtonAlign="Center">
                                                            <Commands>
                                                                <%--<ext:GridCommand Icon="NoteEdit" CommandName="View" ToolTip-Text="<%$ Resources:LocalText,View%>" />--%>
                                                                <%--<ext:GridCommand Icon="Add" CommandName="Copy" ToolTip-Text="<%$ Resources:LocalText,Copy%>" />--%>
                                                                <ext:GridCommand Icon="NoteEdit">
                                                                    <Menu EnableScrolling="false">
                                                                        <Items>
                                                                            <ext:MenuCommand Icon="NoteEdit" CommandName="View" Text="<%$ Resources:LocalText,View%>"/>
                                                                            <ext:MenuCommand Icon="Delete" CommandName="Delete" Text="<%$ Resources:LocalText,Delete%>"/>
                                                                        </Items>
                                                                    </Menu>
                                                                    <ToolTip Text="<%$ Resources:LocalText,Edit%>" />
                                                                </ext:GridCommand>
                                                                <ext:GridCommand Icon="Add">
                                                                    <Menu EnableScrolling="false">
                                                                        <Items>
                                                                            <ext:MenuCommand Icon="Add" CommandName="Copy" Text="<%$ Resources:LocalText,Copy%>"/>
                                                                            <ext:MenuCommand Icon="CogAdd" CommandName="CopyAll" Text="<%$ Resources:LocalText,CopyAll%>"/>
                                                                        </Items>
                                                                    </Menu>
                                                                    <ToolTip Text="<%$ Resources:LocalText,Copy1%>" />
                                                                </ext:GridCommand>
                                                            </Commands>
                                                            <PrepareToolbar Fn="prepare" />
                                                        </ext:CommandColumn>
                                                        <ext:Column Header="<%$ Resources:LocalText,NO%>" Width="140" DataIndex="No" />
                                                        <ext:Column Header="<%$ Resources:LocalText,Status%>" Width="60" DataIndex="Draft1" />
                                                        <ext:Column Header="<%$ Resources:LocalText,ClaimType%>" Width="90" DataIndex="Type1" />
                                                        <ext:Column Header="<%$ Resources:LocalText,Amount%>" Width="80" DataIndex="Tamount">
                                                            <Renderer Fn="GetNumber" />
                                                        </ext:Column>
                                                        <ext:Column Header="<%$ Resources:LocalText,ApproveStatus%>" Width="80" DataIndex="Status1" />
                                                        <ext:Column Header="<%$ Resources:LocalText,CurrentApprover%>" Width="120" DataIndex="Approver" />
                                                        <ext:Column Header="<%$ Resources:LocalText,Owner%>" Width="120" DataIndex="Person" />
                                                        <ext:Column Header="<%$ Resources:LocalText,SubmitPerson%>" Width="120" DataIndex="CreadedBy" />
                                                        <ext:DateColumn Header="<%$ Resources:LocalText,SubmitDate%>" Width="80" DataIndex="CreadedDate"
                                                            Format="yyyy/MM/dd" />
                                                        <ext:Column Header="<%$ Resources:LocalText,Remark%>" Width="150" DataIndex="Remark" />
                                                    </Columns>
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                                        <Listeners>
                                                            <RowSelect Fn="SetGridPanel1" />
                                                        </Listeners>
                                                        <DirectEvents>
                                                            <RowSelect OnEvent="GetDetail">
                                                                <ExtraParams>
                                                                    <ext:Parameter Name="RequestID" Value="record.data.RequestID" Mode="Raw" />
                                                                    <ext:Parameter Name="Type" Value="record.data.Type" Mode="Raw" />
                                                                </ExtraParams>
                                                            </RowSelect>
                                                        </DirectEvents>
                                                    </ext:RowSelectionModel>
                                                </SelectionModel>
                                                <Listeners>
                                                    <Command Fn="editdetail" />
                                                    <RowDblClick Fn="rowdbclick" />
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
                        MsgTarget="Side" BlankText="Your Password is required." Text="" />
                </Items>
                <Buttons>
                    <ext:Button ID="btnOK" runat="server" Text="OK">
                        <Listeners>
                            <Click Handler="if (!#{tfUserID}.validate()) {
                                Ext.Msg.alert('Error','The UserID and Password fields are both required');
                                return false; 
                            }" />
                        </Listeners>
                        <DirectEvents>
                            <Click OnEvent="btnLogin_Click" />
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
