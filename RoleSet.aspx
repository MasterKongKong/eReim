<%@ Page Language="C#" AutoEventWireup="true" Inherits="eReimbursement.RoleSet" Codebehind="RoleSet.aspx.cs" %>



<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>Flow Setting</title>
     <script src="Scripts/PageJS.js" type="text/javascript"></script>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
 <script>
     var DelteUser = function (v, record, rowIndex) {

         Ext.net.DirectMethods.DeleUsers(record.data.id);
     };

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
            <ext:Store ID="SGroupFlow" runat="server">
                <Reader>
                    <ext:JsonReader>
                        <Fields>
                          <ext:RecordField Name="id" />
                          <ext:RecordField Name="FlowUserID" />
                            <ext:RecordField Name="FlowNo" />
                            <ext:RecordField Name="FlowUser" />
                             <ext:RecordField Name="Remark" />
                               <ext:RecordField Name="Fn" />
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
                                            <AfterRender Handler="TreePanel1.getNodeById('a4').select(true);" />
                                        </Listeners>
                                    </ext:TreePanel>
                                </Items>

                            </ext:Panel>
                        </West>
                        <Center>
                            <ext:Panel ID="Panel3" runat="server" Title="Flow Setting" Height="400" MinHeight="300"
                                Padding="10" AutoScroll="true" Layout="AbsoluteLayout">
                                <Items>
                                 <ext:ComboBox ID="DLStep" runat="server" FieldLabel="Flow No." LabelWidth="65" Width="115"
                                        X="10" Y="10" >
                                        <Items>
                                            <ext:ListItem Text="1"  Value="1" />
                                            <ext:ListItem Text="2" Value="2" />
                                            <ext:ListItem Text="3" Value="3" />
                                            <ext:ListItem Text="4" Value="4" />
                                            <ext:ListItem Text="5"  Value="5"/>
                                            <ext:ListItem Text="6" Value="6" />
                                            <ext:ListItem Text="7" Value="7" />
                                            <ext:ListItem Text="8" Value="8" />
                                            <ext:ListItem Text="9" Value="9" />
                                            <ext:ListItem Text="10" Value="10" />
                                        </Items>
                                    </ext:ComboBox>
                                      <ext:ComboBox ID="ff" runat="server" SelectedIndex="0" FieldLabel="Function" LabelWidth="65" Width="165"
                                        X="410" Y="10" >
                                        <Items>
                                            <ext:ListItem Text="Apporver"  Value="Apporver" />
                                            <ext:ListItem Text="Verifier" Value="Verifier" />
                                            <ext:ListItem Text="Issuer" Value="Issuer" />
                                        </Items>
                                    </ext:ComboBox>
                                   <ext:ComboBox ID="DLUser" runat="server" FieldLabel="Approver" LabelWidth="80"  Width="230" X="150" Y="10" DisplayField="fullname" ValueField="userID" >
                        <Store>
                            <ext:Store runat="server" ID="SUser">
                                <Reader>
                                    <ext:JsonReader>
                                        <Fields>
                                            <ext:RecordField Name="fullname" />
<ext:RecordField Name="userID" />
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
                    

                                    <ext:Button ID="BTNAddUser" runat="server" Text="Add" Width="75" X="580" Y="10">
                                    <DirectEvents>
                                            <Click OnEvent="BTN_AddUser">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                     <ext:Button ID="BTNBack" runat="server" Text="Return" Width="75" X="690" Y="10">
                                       <DirectEvents>
                                            <Click OnEvent="btnBack">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:GridPanel ID="GridPanel1" runat="server" StoreID="SGroupFlow" StripeRows="true"
                                        Title="Flow Setting" TrackMouseOver="false" Height="530" 
                                        X="10" Y="50">
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                             <ext:Column Header="Flow No." Width="80" DataIndex="FlowNo" />
                                               <ext:Column Header="Approver ID" Width="150" DataIndex="FlowUserID" />
                                                <ext:Column Header="Approver Name" Width="150" DataIndex="FlowUser" />
                                                 <ext:Column Header="Function" Width="100" DataIndex="Fn" />
                                               <ext:CommandColumn Width="190">
                                                    <Commands>
                                                         <ext:GridCommand Icon="Delete" CommandName="Delete" Text="Delete" />
                                                    </Commands>
                                                </ext:CommandColumn>
                                            </Columns>
                                        </ColumnModel>
                                         <Listeners>
                                            <Command Fn="DelteUser" />
                                        </Listeners>
                                        <SelectionModel>
                                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
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
            </ext:Panel>  <ext:Window ID="loginWindow" runat="server" Title="Login" Icon="User" Width="300px"
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
            </ext:Window>  <ext:KeyNav ID="KeyNav1" runat="server" Target="={document.body}">
                <Enter Handler="btnOK.fireEvent('click');" />
            </ext:KeyNav>
            </form>
        </div>
    </div>
    <input type="hidden" runat="server" id="TT" value="0" />
</body>
</html>
