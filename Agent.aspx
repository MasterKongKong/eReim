<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Agent.aspx.cs" Inherits="eReimbursement.Agent" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>Substitute Setting</title>
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
        var RowCommand = function (v, record, rowIndex) {
            if (v == "Edit") {
                Save.setText("Edit");
                document.getElementById("HH").value = record.data.ID;
                Ext.net.DirectMethods.SetDefaultData(record.data.ID);
                Window1.show();
            }
            else
            { Ext.net.DirectMethods.DeleUsers(record.data.ID); }

        };
        var Addnew = function (v, record, rowIndex) {
            document.getElementById("HH").value = "";
            Save.setText("Save");
            Window1.show();
            DLUser.setValue("");
            TXTBdate.setValue("");
            TXTEdate.setValue("");

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
            <ext:Hidden runat="server" ID="hdrowindex">
            </ext:Hidden>
            <ext:Store ID="Store1" runat="server">
                <Reader>
                    <ext:JsonReader>
                        <Fields>
                           <ext:RecordField Name="ID" />
                            <ext:RecordField Name="Owner" />
                            <ext:RecordField Name="OwnerID" />
                             <ext:RecordField Name="PAgent" />
                              <ext:RecordField Name="PAgentID" />
                            <ext:RecordField Name="Bdate" Type="Date" />
                            <ext:RecordField Name="Edate" Type="Date"  />
                            <ext:RecordField Name="CrededDate" Type="Date"  />
                            <ext:RecordField Name="St" />
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
                                            <AfterRender Handler="TreePanel1.getNodeById('a2').select(true);" />
                                        </Listeners>
                                    </ext:TreePanel>
                                </Items>
                            </ext:Panel>
                        </West>
                        <Center>
                            <ext:Panel ID="Panel3" runat="server" Title="Substitute Setting" Height="600" Padding="10" MinHeight="300"
                                AutoScroll="true" Layout="AbsoluteLayout">
                                <Items>
                                    <ext:Button ID="Button1" runat="server" Text="New Duty Replacement" X="10" Y="10" Width="140">
                                        <Listeners>
                                            <Click Fn="Addnew" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                       Height="550" X="10" Y="40">
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                                <ext:RowNumbererColumn Width="30" />
                                                <ext:CommandColumn Width="110">
                                                    <Commands>
                                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="Edit" />
                                                        <ext:GridCommand Icon="Delete" CommandName="Delete" Text="Delete" />
                                                    </Commands>
                                                </ext:CommandColumn>
                                                <ext:Column Header="User ID" Width="80" DataIndex="PAgentID" />
                                                <ext:Column Header="User Name" Width="100" DataIndex="PAgent" />
                                                <ext:DateColumn Header="Effective Date" Width="120" DataIndex="Bdate" Format="yyyy-MM-dd"/>
                                                <ext:DateColumn Header="Expired Date" Width="120" DataIndex="Edate"  Format="yyyy-MM-dd" />
                                                <ext:DateColumn Header="Created Date" Width="120" DataIndex="CrededDate" Format="yyyy-MM-dd" />
                                                <ext:CheckColumn Header="Status" DataIndex="St" Width="60">
                                                </ext:CheckColumn>
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                            </ext:RowSelectionModel>
                                        </SelectionModel>
                                        <Listeners>
                                            <Command Fn="RowCommand" />
                                           <%-- <RowDblClick Fn="RowCommand" />--%>
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
            <ext:Window ID="Window1" runat="server" Title="Detail" Icon="User" Width="450" Resizable="false"
                Draggable="false" Layout="FormLayout" BodyStyle="background-color: #fff;" Modal="true"
                Closable="true" AutoHeight="True" Hidden="true">
                <Items>
                    <ext:Panel ID="Panel6" runat="server" Height="200" Frame="true" Border="false" Layout="AbsoluteLayout">
                        <Items>
                            <ext:ComboBox ID="DLUser" runat="server" FieldLabel="User" LabelWidth="65" Width="185"
                                        X="10" Y="10" DisplayField="fullname" ValueField="userID">
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
                            <ext:Label ID="Csslb" runat="server" Text="Status:" Width="65" X="10" Y="73">
                            </ext:Label>
                            <ext:Checkbox ID="CK" runat="server" X="125" Y="70" Width="30">
                            </ext:Checkbox>
                            <ext:DateField ID="TXTBdate" runat="server" FieldLabel="Effective Date " LabelWidth="65" Width="185"
                                X="10" Y="40" Format="yyyy-MM-dd" >
                            </ext:DateField>
                            <ext:DateField ID="TXTEdate" runat="server" FieldLabel="Expired Date" LabelWidth="65" Width="185"
                                X="210" Y="40" Format="yyyy-MM-dd" EmptyText="0000-00-00">
                            </ext:DateField>
                           
                            <ext:Button ID="Save"   runat="server" Text="Save" Width="75" X="10" Y="130">
                              <DirectEvents>
                                            <Click OnEvent="BTN_Search">
                                            </Click>
                                        </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="Close" runat="server" Text="Close" Width="75" X="95" Y="130">
                                <Listeners>
                                    <Click Handler="Window1.hide();" />
                                </Listeners>
                            </ext:Button>
                        </Items>
                    </ext:Panel>
                </Items>
            </ext:Window>
            <input type="hidden" runat="server" id="HH" />
            </form>
        </div>
    </div>
</body>
</html>
