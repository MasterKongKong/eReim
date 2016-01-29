<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BudgetSet.aspx.cs" Inherits="eReimbursement.BudgetSet" %>


<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>Budget Setting</title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <script src="Scripts/PageJS.js" type="text/javascript"></script>
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(document).ready(function () {
            $("div.gn_person ul.q-menubox li:eq(4) a").click(function () {
                Ext.Msg.confirm('提示', '确认登出?', function (btn, text) {
                    if (btn == 'yes') {
                        if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
                            $('div.gn_person ul.q-menubox li:eq(0) a').text('请点击此处登录或切换用户.');
                        }
                        else {
                            $('div.gn_person ul.q-menubox li:eq(0) a').text('Please click here to login or change user.');
                        }
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
                document.title = '审批管理';
                $('div.gn_center ul.q-menubox li:eq(0) a').text('首页');
                $('div.gn_center ul.q-menubox li:eq(1) a').text('报销申请');
                $('div.gn_center ul.q-menubox li:eq(2) a').text('报销审核');
                $('div.gn_center ul.q-menubox li:eq(3) a').text('影像管理');
                $('div.gn_center ul.q-menubox li:eq(4) a').text('预算管理');
                $('div.gn_center ul.q-menubox li:eq(5) a').text('基础数据');
                $('div.gn_person ul.q-menubox li:eq(2) a').text('语言设置');
                $('div.gn_person ul.q-menubox li:eq(4) a').text('登出');
            }
            else {
                document.title = 'Approval Management';
                $('div.gn_center ul.q-menubox li:eq(0) a').text('Home');
                $('div.gn_center ul.q-menubox li:eq(1) a').text('Apply');
                $('div.gn_center ul.q-menubox li:eq(2) a').text('Approval');
                $('div.gn_center ul.q-menubox li:eq(3) a').text('Scanning');
                $('div.gn_center ul.q-menubox li:eq(4) a').text('Budget');
                $('div.gn_center ul.q-menubox li:eq(5) a').text('Setting');
                $('div.gn_person ul.q-menubox li:eq(2) a').text('Language');
                $('div.gn_person ul.q-menubox li:eq(4) a').text('Log Out');
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


        var editdetail = function (v, record, rowIndex) {
            Ext.net.DirectMethods.DeleUsers(record.data.id);
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
                        <li class="q-menuitem"><a href="#">首页</a></li>
                        <li class="q-menuitem"><a href="MyClaims.aspx">报销申请</a></li>
                        <li class="q-menuitem"><a href="Approve.aspx">报销审核</a></li>
                        <li class="q-menuitem"><a href="FileManagement.aspx">影像管理</a></li>
                        <li class="q-menuitem"><a href="Budget.aspx" id="apply">预算管理</a></li>
                        <li class="q-menuitem"><a href="Profile.aspx">基础数据</a></li>
                    </ul>
                </div>
             <div class="gn_person">
                    <ul class="q-menubox">
                        <li class="q-menuitem"><a href="#">请点击此处登录或切换用户.</a></li><li class="q-navitem"><span>
                        </span></li>
                        <li class="q-menuitem"><a href="#" id="language">语言设置</a></li><li class="q-navitem">
                            <span></span></li>
                        <li class="q-menuitem"><a href="#">登出</a></li>
                    </ul>
                </div>
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
                    <ext:JsonReader>
                        <Fields>
                            <ext:RecordField Name="Accountcode" />
                            <ext:RecordField Name="Description" />
                            <ext:RecordField Name="m1" />
                            <ext:RecordField Name="m2" />
                            <ext:RecordField Name="m3" />
                            <ext:RecordField Name="m4" />
                            <ext:RecordField Name="m5" />
                            <ext:RecordField Name="m6" />
                            <ext:RecordField Name="m7" />
                            <ext:RecordField Name="m8" />
                            <ext:RecordField Name="m10" />
                            <ext:RecordField Name="m9" />
                            <ext:RecordField Name="m11" />
                            <ext:RecordField Name="m12" />
                            <ext:RecordField Name="Total" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
               <ext:Store ID="SDetail" runat="server">
                <Reader>
                    <ext:JsonReader>
                        <Fields>
                            <ext:RecordField Name="id" />
                             <ext:RecordField Name="FID" />
                            <ext:RecordField Name="Months" />
                            <ext:RecordField Name="Name" />
                            <ext:RecordField Name="Amount" />
                            <ext:RecordField Name="SAccoundcode" />
                             <ext:RecordField Name="SAccoundName" />
                        <ext:RecordField Name="Type" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
                 <ext:Store ID="SAccoundcode" runat="server">
                <Reader>
                    <ext:JsonReader>
                        <Fields>
                            <ext:RecordField Name="id" />
                             <ext:RecordField Name="SAccountCode" />
                            <ext:RecordField Name="SAccountName" />
                            <ext:RecordField Name="ADes" />
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
                                            <ext:TreeNode Text="Budget Manage" Expanded="true">
                                                <Nodes>
                                                     <ext:TreeNode Text="Budget Setting" Expanded="true" Href="Budget.aspx" Icon="UserKey"
                                                        NodeID="a1">
                                                    </ext:TreeNode>
                                                    <ext:TreeNode Text="Budget Report" Expanded="true" Href="BudgetReport.aspx" Icon="UserKey" NodeID="a2">
                                                    </ext:TreeNode>
                                                    <ext:TreeNode Text="Budget Remind" Expanded="true" Href="BudgetEmail.aspx" Icon="UserKey" NodeID="a2">
                                                    </ext:TreeNode> <ext:TreeNode Text="Currency Setting" Href="CuySet.aspx" Icon="FlagCf" NodeID="a6" />
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
                            <ext:Panel ID="Panel3" runat="server" Title="Budget Setting" Padding="10" AutoScroll="true"
                                Layout="AbsoluteLayout">
                                <Items>
                                <ext:Label ID="LBAccountCode" runat="server" X="10" Y="13" Text="" FieldLabel="Account Code"  LabelWidth="100" Width="200"/>
                                 <ext:Label ID="LBStation" runat="server" X="10" Y="50" Text="" FieldLabel="Unit"  LabelWidth="100" Width="200"/>
                                  <ext:Label ID="LBYears" runat="server" X="200" Y="50" Text="" FieldLabel="Years"  LabelWidth="80" Width="200"/>
                                    <ext:Label ID="LBType" runat="server" X="400" Y="50" Text="" FieldLabel="Type"  LabelWidth="50" Width="200"/>
                                    <ext:Label ID="LBAccountDes" runat="server" X="200" Y="13" Text="" FieldLabel="Description"  LabelWidth="80" Width="200" />
                                    <ext:ComboBox ID="LBMonths" runat="server" FieldLabel="Month" LabelWidth="35" Width="115"
                                        X="400" Y="10" SelectedIndex="0" >
                                        <Items>
                                            <ext:ListItem Text="1" />
                                            <ext:ListItem Text="2" />
                                            <ext:ListItem Text="3" />
                                            <ext:ListItem Text="4" />
                                            <ext:ListItem Text="5" />
                                            <ext:ListItem Text="6" />
                                            <ext:ListItem Text="7" />
                                            <ext:ListItem Text="8" />
                                            <ext:ListItem Text="9" />
                                            <ext:ListItem Text="10" />
                                            <ext:ListItem Text="11" />
                                            <ext:ListItem Text="12" />
                                        </Items>
                                        <DirectEvents>
                                        <Select  OnEvent="LBMonths_Search"></Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                   <ext:Label ID="LBAmount" runat="server" X="550" Y="13" Text="" FieldLabel="Monthly Budget"  LabelWidth="80" Width="200" />
                                   <ext:TextField  ID="LBLast" runat="server" ReadOnly="true" X="550" Y="50" Text="" FieldLabel="Monthly balance"  LabelWidth="80" Width="200" />
                                           
                                         

                                     <ext:ComboBox ID="DLUser" runat="server" FieldLabel="Users" LabelWidth="60" Width="285"
                                        X="10" Y="130" DisplayField="FullName" ValueField="UserID">
                                         <Store>
                            <ext:Store runat="server" ID="SUser">
                                <Reader>
                                    <ext:JsonReader>
                                        <Fields>
                                            <ext:RecordField Name="FullName" />
<ext:RecordField Name="UserID" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <DirectEvents>
                            <KeyUp OnEvent="GetUser">
                            </KeyUp>
                        </DirectEvents>
                                    </ext:ComboBox>
                                       <ext:ComboBox ID="DLDepartment" runat="server" FieldLabel="部门" LabelWidth="60" Width="185"
                                        X="10" Y="130" SelectedIndex="0" Hidden="true" DisplayField="Departmentname" ValueField="Departmentname" >
                                       <Store>
                            <ext:Store runat="server" ID="SDepartment">
                                <Reader>
                                    <ext:JsonReader>
                                        <Fields>
                                            <ext:RecordField Name="Departmentname" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                                    </ext:ComboBox>
                                    <ext:TextField ID="TXTAmount" runat="server" FieldLabel="Amount" LabelWidth="50" Width="150"
                                        X="340" Y="130">
                                    </ext:TextField>
                                     
                                    <ext:Button ID="Button1" runat="server" Text="Add" Width="75" X="480" Y="130">
                                        <DirectEvents>
                                            <Click OnEvent="Add_Search">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                     <ext:Button ID="Button2" runat="server" Text="Return" Width="75" X="570" Y="130">
                                        <DirectEvents>
                                            <Click OnEvent="BACK">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                     <ext:Button ID="Button4" runat="server" Text="Copy to next month" Width="125" X="650" Y="130">
                                        <DirectEvents>
                                            <Click OnEvent="copy">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:GridPanel ID="GridPanel1" runat="server" StoreID="SDetail" StripeRows="true"
                                        Title="Budget Setting." TrackMouseOver="false" Height="380" X="10" Y="160">
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>                 
                                              <ext:Column Header="Month" Width="30" DataIndex="Months">
                                                </ext:Column>
                                                  <ext:Column Header="Account Code" Width="60" DataIndex="SAccoundcode">
                                                </ext:Column>
                                                  <ext:Column Header="Account Name" Width="180" DataIndex="SAccoundName">
                                                </ext:Column>
                                                <ext:Column Header="Name" Width="60" DataIndex="Name">
                                                </ext:Column>
                                                <ext:Column Header="Amount" Width="60" DataIndex="Amount">
                                                </ext:Column>
                                                 <ext:CommandColumn Width="60">
                                                    <Commands>
                                                        <ext:GridCommand Icon="Delete" CommandName="F" Text="Delete" />
                                                    </Commands>
                                                </ext:CommandColumn>
                                            </Columns>
                                        </ColumnModel>
                                       <Listeners>
                                       <Command Fn="editdetail" />
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
                                        <View>
                                            <ext:GridView ID="GridView1" runat="server" ForceFit="true">
                                            </ext:GridView>
                                        </View>
                                    </ext:GridPanel>
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
                <Enter Handler="btnOK.fireEvent('click');" />
            </ext:KeyNav>

                  <ext:Window ID="Window1" runat="server" Title="My Business Trip @eLeave" Hidden="true"
                Layout="FitLayout" Width="850" Height="200" Resizable="False" AutoScroll="true" Closable="False">
                <Listeners>
                    <Show Handler=" var pos = Button4.getPosition();
                        pos[0] += -683;
                        pos[1] += 29;
                        this.setPosition(pos);" />
                </Listeners>
                <Items>
                   <ext:Button ID="Button3" runat="server" Text="Back" Width="75" X="560" Y="90">
                                       
                                    </ext:Button>
                </Items>
            </ext:Window>
            </form>
        </div>
    </div>
</body>
</html>
