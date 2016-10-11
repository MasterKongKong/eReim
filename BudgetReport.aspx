<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BudgetReport.aspx.cs" Inherits="eReimbursement.BudgetReport" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>Budget Report</title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/PageJS.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
      <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <script type="text/javascript">

        var CelCK = function (grid, command, record, row) {

            //            if (record > 1 && record < 14) {
            //                var mm = record - 1;
            //                var c = Store1.data.items[command].json[0];
            //                var d = Store1.data.items[command].json[1];
            //                var y = ComboBox2.getValue();

            //                var s = ComboBox4.getValue();

            //                var mt = Store1.data.items[command].json[record];
            //                window.location.href = "BudgetSet.aspx?c=" + c + "&d=" + d + "&mt=" + mt + "&mm=" + mm + "&y=" + y + "&s=" + s;

            //            }
        };
        var template = '<span style="color:{0};">{1}</span>';

        var tts = function (value) {
            
            var vv = value.split("/");
            var mm = parseFloat(vv[0]) / parseFloat(vv[1]);
           
            return String.format(template, (mm <1) ? "black" : "red", value);
        };
        var tt = function (value) {
            return String.format(template, (value > 0) ? "black" : "red", value + "%");
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
        var saveData = function () {
            GridData.setValue(Ext.encode(GridPanel1.getRowsValues({ selectedOnly: false })));
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
            </ext:Store>
              <ext:Store ID="SStation" runat="server">
                <Reader>
                    <ext:JsonReader>
                        <Fields>
                            <ext:RecordField Name="StationCode" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
                 <ext:Store ID="Store2" runat="server">
                <Reader>
                    <ext:JsonReader>
                        <Fields>
                            <ext:RecordField Name="Baccountcode" />
                            <ext:RecordField Name="Saccountname" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
                <ext:Hidden ID="GridData" runat="server" />
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
                                                        NodeID="a2">
                                                    </ext:TreeNode>
                                                    <ext:TreeNode Text="Budget Report" Expanded="true" Href="BudgetReport.aspx" Icon="UserKey" NodeID="a1">
                                                    </ext:TreeNode>
                                                    <ext:TreeNode Text="Budget Remind" Expanded="true" Href="BudgetEmail.aspx" Icon="UserKey" NodeID="a2">
                                                    </ext:TreeNode>
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
                            <ext:Panel ID="Panel3" runat="server" Title="Budget Report" Padding="10" AutoScroll="true"
                                Layout="AbsoluteLayout">
                                <Items>

                                
                                     <ext:ComboBox ID="DLUser" runat="server" FieldLabel="Users" LabelWidth="60" Width="285"
                                        X="330" Y="50" DisplayField="FullName" ValueField="UserID"  Hidden="true">
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
                                        X="330" Y="50" Hidden="true" DisplayField="Departmentname" ValueField="Departmentname" >
                                      <Items>
                                        </Items>
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
                                    <ext:ComboBox StoreID="Store2" ID="ComboBox1" runat="server"  FieldLabel="Acccount Name" LabelWidth="115"
                                        Width="300" X="10" Y="50" SelectedIndex="1" DisplayField="Saccountname"  ValueField="Baccountcode">
                                        <Items>
                                        </Items>
                                          
                                    </ext:ComboBox>

                                    <ext:ComboBox ID="DLType" runat="server" FieldLabel="Categories" LabelWidth="65" Width="145"
                                        X="10" Y="10" SelectedIndex="0">
                                        <Items>
                                            <ext:ListItem Text="Unit" />
                                            <ext:ListItem Text="Department" />
                                            <ext:ListItem Text="Personal" />
                                        </Items>
                                          <DirectEvents>  <Select OnEvent="EXchange"/>
                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="DLYears" runat="server" FieldLabel="Years" LabelWidth="35" Width="115"
                                        X="310" Y="10" SelectedIndex="4">
                                        <Items>
                                            <ext:ListItem Text="2012" />
                                            <ext:ListItem Text="2013" />
                                            <ext:ListItem Text="2014" />
                                            <ext:ListItem Text="2015" />
                                            <ext:ListItem Text="2016" />
                                        </Items>
                                    </ext:ComboBox>
                                      <ext:ComboBox StoreID="SStation" ID="DLStation" runat="server"  FieldLabel="Unit" LabelWidth="35"
                                        Width="115" X="175" Y="10" SelectedIndex="1" DisplayField="StationCode"  ValueField="StationCode">
                                        <Items>
                                        </Items>
                                          
                                    </ext:ComboBox>
                                   
                                    <ext:ComboBox ID="DLMonths" runat="server" FieldLabel="Month" LabelWidth="35" Width="115"
                                        X="445" Y="10" SelectedIndex="11">
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
                                    </ext:ComboBox>
                                    <ext:Button ID="Button8" runat="server" Text="Report" Width="75" X="570" Y="10">
                                        <DirectEvents>
                                            <Click OnEvent="button1_Search">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="DoadData" runat="server"  Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Width="75" X="650" Y="10">
                                       <Listeners>
                                <Click Fn="saveData" />
                            </Listeners>
                                    </ext:Button>
                                  <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                        Title="Budget Report." TrackMouseOver="false" Height="380" X="10" Y="80">
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                            </Columns>
                                        </ColumnModel>
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
            </form>
        </div>
    </div>
</body>
</html>
