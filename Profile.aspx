﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="eReimbursement.Profile" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>My Profile</title>
    <script src="Scripts/PageJS.js" type="text/javascript"></script>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
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
                        <li class="q-menuitem"><a href="Budget.aspx">预算管理</a></li>
                        <li class="q-menuitem"><a href="Profile.aspx" id="apply">基础数据</a></li>
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
        <div class="W_main W_profile">
            <form id="Form1" runat="server">
            <ext:ResourceManager ID="ResourceManager1" runat="server" />
            <ext:Panel ID="Panel1" runat="server" Height="680" Border="false">
                <Items>
                    <ext:BorderLayout ID="BorderLayout1" runat="server">
                        <West Collapsible="true" Split="true" CollapseMode="Mini">
                            <ext:Panel ID="Panel2" runat="server" Width="175">
                                <Items>
                                    <ext:TreePanel ID="TreePanel1" runat="server" Header="false" Lines="false" UseArrows="true"
                                        RootVisible="false" Width="175" Border="false">
                                        <Root>
                                            <ext:TreeNode Text="Base Setting" Expanded="true">
                                                <Nodes>
                                                    <ext:TreeNode Text="My Profile" Href="Profile.aspx" Icon="FlagCa" NodeID="a1" />
                                                    <ext:TreeNode Text="Substitute Setting" Href="Agent.aspx" Icon="FlagCatalonia" NodeID="a2" />
                                                    <ext:TreeNode Text="Flow Setting" Href="Role.aspx" Icon="FlagCd" NodeID="a4" />
                                                    <ext:TreeNode Text="Role Setting" Href="RoleModule.aspx" Icon="FlagCf" NodeID="a5" />
                                                    <ext:TreeNode Text="Currency Setting" Href="CuySet.aspx" Icon="FlagCf" NodeID="a6" />
                                                    <ext:TreeNode Text="Exchange Rate" Href="ExchangeRate.aspx" Icon="FlagCf" NodeID="a7" />
                                                    <ext:TreeNode Text="Mail Setting" Href="MailSetting.aspx" Icon="FlagCf" NodeID="a8" />
                                                    <ext:TreeNode Text="File Setting" Href="FileSetting.aspx" Icon="FlagCf" NodeID="a9" />
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
                            <ext:Panel ID="Panel3" runat="server" Title="My Profile" Height="600" Padding="10" MinHeight="300"
                                AutoScroll="true" Layout="AbsoluteLayout">
                                <Items>
                                    <ext:Label ID="Label27" runat="server" Text="User ID:" Width="70" X="20" Y="10" />
                                    <ext:Label ID="LBUserID" runat="server" Text="" Width="60" X="100" Y="10" />
                                    <ext:Label ID="Label29" runat="server" Text="User Name:" Width="70" X="20" Y="40" />
                                    <ext:Label ID="LBUserName" runat="server" Text=" " Width="150" X="100" Y="40" />
                                    <ext:Label ID="Label15" runat="server" Text="Unit:" Width="70" X="20" Y="70" />
                                    <ext:Label ID="LBStation" runat="server" Text="" Width="100" X="100" Y="70" />

                                     <ext:Label ID="Label2" runat="server" Text="Cost Unit:" Width="70" X="200" Y="70" />
                                    <ext:Label ID="Coststation" runat="server" Text="" Width="100" X="285" Y="70" />


                                    <ext:Label ID="Label17" runat="server" Text="Department:" Width="70" X="20" Y="220" />
                                    <ext:Label ID="LBDepartmentCode" runat="server" Text="" Width="100" X="100" Y="220" />
                                    <ext:Label ID="Label19" runat="server" Text="Email:" Width="70" X="20" Y="250" />
                                    <ext:Label ID="LBMail" runat="server" Text="H" Width="220"
                                        X="100" Y="250" />
                                    <ext:Label ID="Label21" runat="server" Text="Phone:" Width="70" X="20" Y="100" />
                                    <ext:Label ID="LBTel" runat="server" Text="" Width="120" X="100" Y="100" />
                                      <ext:Label ID="Labesl2s" runat="server" Text="Tel:" Width="70" X="20" Y="130" />
                                   <ext:Label ID="LBMobile" runat="server" Text="" Width="120" X="100" Y="130" />
                                    <ext:Label ID="Label3" runat="server" Text="Station Role:" Width="70" X="20" Y="160" />
                                    <ext:Label ID="LBStations" runat="server" Text="" X="100" Y="160" Anchor="100%" />
                                   <ext:Label ID="Label1" runat="server" Text="Module Role:" Width="70" X="20" Y="190" />
                                    <ext:Label ID="LBModules" runat="server" Text="" X="100"
                                        Y="190" Anchor="100%" />
                                   
                                </Items>
                            </ext:Panel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel><ext:Window ID="loginWindow" runat="server" Title="Login" Icon="User" Width="300px"
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
            </form>
        </div>
    </div>
</body>
</html>
