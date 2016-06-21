<%@ Page Language="C#" AutoEventWireup="true" Inherits="eReimbursement.Budget" CodeBehind="Budget.aspx.cs" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>Budget Setting</title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/PageJS.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
     <link href="../../../../resources/css/examples.css" rel="stylesheet" type="text/css" />
    
    <style type="text/css">
        .x-grid3-cell-inner {
            font-family : "segoe ui",tahoma, arial, sans-serif;
        }
         
        .x-grid-group-hd div {
            font-family : "segoe ui",tahoma, arial, sans-serif;
        }
         
        .x-grid3-hd-inner {
            font-family : "segoe ui",tahoma, arial, sans-serif;
            font-size   : 12px;
        }
         
        .x-grid3-body .x-grid3-td-Cost {
            background-color : #f1f2f4;
        }
         
        .x-grid3-summary-row .x-grid3-td-Cost {
            background-color : #e1e2e4;
        }    
         
        .total-field{
            background-color : #fff;
            font-weight      : bold !important;                       
            color            : #000;
            border           : solid 1px silver;
            padding          : 2px;
            margin-right     : 5px;
        } 
        .my_row_style div{ background:#E0EEEE; border-color:#E0EEEE; font-weight: bold;}
         .my_row_style1 div{ font-weight: bold;}
    </style>
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

        var CelCK = function (grid, command, record, row) {

            if (record > 1 && record < 14) {
                var mm = record - 1;
                var c = Store1.data.items[command].json[0];
                var d = Store1.data.items[command].json[1];
                var y = ComboBox2.getValue();
              
                var s = ComboBox4.getValue();
               
                var mt = Store1.data.items[command].json[record];
                window.location.href = "BudgetSet.aspx?c=" + c + "&d=" + d + "&mt=" + mt + "&mm=" + mm+"&y="+y+"&s="+s;

            }
        };
        var editdetail = function (v, record, rowIndex) {
          
            if (v == "U") {
                window.location.href = "BudgetSet.aspx?T=1&S=" + DLStation.getValue() + "&id=" + record.data.id;

            }
            if (v == "F") {
                window.location.href = "BudgetSet.aspx?T=2&S=" + DLStation.getValue() + "&id=" + record.data.id;
            }
        };
        var prepare = function (grid, toolbar, rowIndex, record) {

            if (record.data.my == "0") {
                if (record.data.Type == "1") {
                    var bttt = toolbar.items.get(1);
                    bttt.setDisabled(true);
                }
                if (record.data.Type == "2") {
                    var bttt = toolbar.items.get(0);
                    bttt.setDisabled(true);
                }
            }
            else {
                var b1 = toolbar.items.get(1);
                b1.hide();
                var b2 = toolbar.items.get(0);
                b2.hide();
            }
            //you can return false to cancel toolbar for this record
        };

        var getRowClass = function (record, index, rowParams, store) {

            if (record.data.my == "0") {
                return "my_row_style";

            }
            if (record.data.code == "") {
                return "my_row_style1";

            }
        };
        var saveData = function () {
            GridData.setValue(Ext.encode(GridPanel2.getRowsValues({ selectedOnly: false })));
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
         
            <ext:Store ID="SStation" runat="server">
                <Reader>
                    <ext:JsonReader>
                        <Fields>
                            <ext:RecordField Name="StationCode" />
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
                                                        NodeID="a1">
                                                    </ext:TreeNode>
                                                    <ext:TreeNode Text="Budget Report" Expanded="true" Href="BudgetReport.aspx" Icon="UserKey" NodeID="a2">
                                                    </ext:TreeNode>
                                                    <ext:TreeNode Text="Budget Remind" Expanded="true" Href="BudgetEmail.aspx" Icon="UserKey" NodeID="a3">
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

                            <ext:Panel ID="Panel3" runat="server" Title="Budget Setting" Padding="10" AutoScroll="true"
                                Layout="AbsoluteLayout">
                                <Items>
                                 <ext:Label ID="DSD" runat="server" X="10" Y="10" Text="" FieldLabel="1. 如果需要同时分配个人和部门预算，请点击Personal 图标，分配个人预算后，部门其它人的共享预算数值请设置到“Other+部门名称”。"  LabelWidth="760" Width="760"/>
                                  <ext:Label ID="Label1" runat="server" X="10" Y="25" Text="" FieldLabel="2. 如果仅仅需要分配部门预算，请点击Department 图标。需要注意，预算按照部门分配后，不能再分配个人预算。"  LabelWidth="760" Width="760"/>
                                    <ext:ComboBox ID="DLYears" runat="server" FieldLabel="Years" LabelWidth="35" Width="115"
                                        X="10" Y="58" SelectedIndex="4">
                                        <Items>
                                            <ext:ListItem Text="2012"  />
                                            <ext:ListItem Text="2013" />
                                            <ext:ListItem Text="2014" />
                                            <ext:ListItem Text="2015" />
                                            <ext:ListItem Text="2016" />
                                        </Items>
                                       <DirectEvents>
                                       <Select OnEvent="Chanage_Search"/>
                                       </DirectEvents>
                                    </ext:ComboBox>
                                       <ext:ComboBox StoreID="SStation" ID="DLStation" runat="server"  FieldLabel="Unit" LabelWidth="35"
                                        Width="115" X="180" Y="58" SelectedIndex="1" DisplayField="StationCode"  ValueField="StationCode">
                                        <Items>
                                        </Items>
                                           <DirectEvents>
                                       <Select OnEvent="Chanage_Search"/>
                                       </DirectEvents>
                                    </ext:ComboBox>
                                       <ext:Button ID="LoadData" runat="server" Hidden="true" Text="Load Budget" Width="75" X="360" Y="55">
                                         <DirectEvents>
                                            <Click OnEvent="Load_Search">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>

                                      <ext:Button ID="DoadData" runat="server" Hidden="true" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel" Width="75" X="360" Y="58">
                                         <Listeners>
                                <Click Fn="saveData" />
                            </Listeners>
                                    </ext:Button>
                                    <ext:GridPanel AutoScroll="true"  Title="Budget Setting."
            ID="GridPanel2"
            runat="server"
            Frame="true"
            StripeRows="true"
          
          
            Collapsible="true"
            AnimCollapse="false"
            Icon="ApplicationViewColumns"
            TrackMouseOver="false"
          
            Height="550"           
            ClicksToEdit="1"
           X="10" Y="100"
            >
            <Store>
                <ext:Store ID="Store2" runat="server" GroupField="main">
                    <Reader>
                        <ext:JsonReader IDProperty="sort">
                            <Fields>
                                <ext:RecordField Name="sort" />
                                <ext:RecordField Name="my" />
                                <ext:RecordField Name="main" />
                                <ext:RecordField Name="code" />
                                <ext:RecordField Name="des"/>
                                <ext:RecordField Name="m1" Type="Float" />
                                   <ext:RecordField Name="m2" Type="Float" />
                                      <ext:RecordField Name="m3" Type="Float" />
                                         <ext:RecordField Name="m4" Type="Float" />
                                            <ext:RecordField Name="m5" Type="Float" />
                                               <ext:RecordField Name="m6" Type="Float" />
                                                  <ext:RecordField Name="m7" Type="Float" />
                                                     <ext:RecordField Name="m8" Type="Float" />
                                                        <ext:RecordField Name="m9" Type="Float" />
                                                           <ext:RecordField Name="m10" Type="Float" />
                                                              <ext:RecordField Name="m11" Type="Float" />
                                                                 <ext:RecordField Name="m12" Type="Float" />
                                <ext:RecordField Name="tt"  />
                                    <ext:RecordField Name="Type"  />
                                      <ext:RecordField Name="id" />
                            </Fields>
                        </ext:JsonReader>
                    </Reader>
                </ext:Store>
            </Store>
            <ColumnModel ID="ColumnModel2" runat="server">
                <Columns>
                    <ext:CommandColumn Width="150" ButtonAlign="Center">
                                                            <Commands>
                                                                <ext:GridCommand Icon="User"  CommandName="U" ToolTip-Text="Personal" />
                                                                <ext:GridCommand Icon="Group" CommandName="F" ToolTip-Text="Department" />
                                                            </Commands>
                                                            <PrepareToolbar Fn="prepare" />
                                                        </ext:CommandColumn>
                       <ext:Column ColumnID="main" Header="main" DataIndex="main" Width="20" Sortable="false" />
                   <ext:Column ColumnID="code" Header="AccountCode" DataIndex="code" Width="200"  Sortable="false"/>
                    <ext:Column ColumnID="des" Header="Descript" DataIndex="des" Width="200"  Sortable="false"/>
                      <ext:Column ColumnID="m1" Header="Jan" DataIndex="m1" Width="200"  Sortable="false"/>
                        <ext:Column ColumnID="m2" Header="Feb" DataIndex="m2" Width="200"  Sortable="false"/>
                          <ext:Column ColumnID="m3" Header="Mar" DataIndex="m3" Width="200"  Sortable="false"/>
                   <ext:Column ColumnID="m4" Header="Apr" DataIndex="m4" Width="200" Sortable="false" />
                     <ext:Column ColumnID="m5" Header="May" DataIndex="m5" Width="200"  Sortable="false" />
                       <ext:Column ColumnID="m6" Header="Jun" DataIndex="m6" Width="200" Sortable="false" />
                         <ext:Column ColumnID="m7" Header="Jul" DataIndex="m7" Width="200"  Sortable="false"/>
                           <ext:Column ColumnID="m8" Header="Aug" DataIndex="m8" Width="200"  Sortable="false"/>
                             <ext:Column ColumnID="m9" Header="Sep" DataIndex="m9" Width="200"  Sortable="false"/>
                               <ext:Column ColumnID="m10" Header="Oct" DataIndex="m10" Width="200"  Sortable="false"/>
                                 <ext:Column ColumnID="m11" Header="Nov" DataIndex="m11" Width="200"  Sortable="false"/>
                                   <ext:Column ColumnID="m12" Header="Dec" DataIndex="m12" Width="200"  Sortable="false"/>
                                     <ext:Column ColumnID="tt" Header="Total" DataIndex="tt" Width="200"  Sortable="false"/>
                </Columns>
             
            </ColumnModel>
          <Listeners>
                                            <Command Fn="editdetail" />
                                           
                                        
                                        </Listeners>
            <View>
                <ext:GroupingView ID="GroupingView1"
                    runat="server"
                    ForceFit="true"
                    MarkDirty="false"
                    ShowGroupName="false"
                    EnableNoGroups="true"
                    HideGroupedColumn="true"
                     ><GetRowClass Fn="getRowClass" /></ext:GroupingView>
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
