<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Welcome.aspx.cs" Inherits="eReimbursement.Welcome" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>我的报销单</title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
    <link href="Styles/StyleSheet1.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/PageJS.js" type="text/javascript"></script>
    <link href="Styles/StyleSheetFlow.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        var GoToXLS = function () {
            window.open('ereim.htm');
        };
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
        //            TreePanel1.getNodeById('c1').select(true);
        var ipa = "http://219.150.98.243:88/eReimbursement_Old/MyClaims.aspx";
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
            <ext:Panel ID="Panel1" runat="server" Height="680" Border="false">
                <Items>
                    <ext:BorderLayout ID="BorderLayout1" runat="server">
                        <West Collapsible="true" Split="true" CollapseMode="Mini">
                            <ext:Panel ID="Panel2" runat="server" Width="195" Layout="FitLayout" Collapsed="true">
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
                                                    <ext:TreeNode Text="<%$ Resources:LocalText,MyExpense%>" Href="MyClaims.aspx" NodeID="c1"
                                                        Icon="Table" />
                                                    <ext:TreeNode Text="<%$ Resources:LocalText,MyExpenseOld%>" NodeID="d1" Icon="Table" />
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
                                Height="600" MinHeight="300" PaddingSummary="0 0 0 0" AutoScroll="true" Layout="AnchorLayout">
                                <Items>
                                    <ext:Image ID="Image1" runat="server" ImageUrl="~/Images/eReim1.png">
                                        <Listeners>
                                            <Click Fn="GoToXLS" />
                                        </Listeners>
                                    </ext:Image>
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
