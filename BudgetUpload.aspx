<%@ Page Language="C#" AutoEventWireup="true" Inherits="eReimbursement.BudgetUpload" Codebehind="BudgetUpload.aspx.cs" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>My Claims</title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">

        var rowdbclick = function (a, b, c) {
            window.open("ApplyTransportation.aspx");
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
                <Reader>
                    <ext:ArrayReader>
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
                                                    <ext:TreeNode Text="预算录入" Expanded="true" Href="BudgetDetail.aspx" Icon="UserKey"
                                                        NodeID="a1">
                                                    </ext:TreeNode>
                                                    <ext:TreeNode Text="预算管理" Expanded="true" Href="Budget.aspx" Icon="UserKey" NodeID="a2">
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
                            <ext:Panel ID="Panel3" runat="server" Title="预算管理" 
                                Padding="10" AutoScroll="true" Layout="AbsoluteLayout">
                                <Items>
                                  
                                    <ext:ComboBox ID="ComboBox2" runat="server" FieldLabel="年度" LabelWidth="35" Width="115"
                                        X="10" Y="10" SelectedIndex="1">
                                        <Items>
                                            <ext:ListItem Text="2012" />
                                            <ext:ListItem Text="2013" />
                                            <ext:ListItem Text="2014" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="ComboBox4" runat="server" FieldLabel="站点" LabelWidth="35" Width="115"
                                        X="180" Y="10" SelectedIndex="1">
                                        <Items>
                                            <ext:ListItem Text="ZJDXMN" />
                                            <ext:ListItem Text="ZJDQZH" />
                                            <ext:ListItem Text="ZJDXAN" />
                                        </Items>
                                    </ext:ComboBox>
                                   
                                    <ext:Button ID="Button1" runat="server" Text="保存预算" Width="75" X="665" Y="50">
                                        
                                    </ext:Button>
                                    <ext:Button ID="Button8" runat="server"  Text="上传预算" Width="75" X="570" Y="50">
                                   <DirectEvents>
                                            <Click OnEvent="button1_Search">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                        Title="预算管理." TrackMouseOver="false" Height="380" 
                                        X="10" Y="100">
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                                <ext:Column Header="Account code" Width="60" DataIndex="Accountcode"></ext:Column>
                                                <ext:Column Header="Description" Width="60" DataIndex="Description"></ext:Column>
                                                <ext:Column Header="Jan" Width="60" DataIndex="m1"></ext:Column>
                                                <ext:Column Header="Feb" Width="60" DataIndex="m2"></ext:Column>
                                                <ext:Column Header="Mar" Width="60" DataIndex="m3"></ext:Column>
                                                <ext:Column Header="Apr" Width="60" DataIndex="m4"></ext:Column>
                                                <ext:Column Header="May" Width="60" DataIndex="m5"></ext:Column>
                                                <ext:Column Header="Jun" Width="60" DataIndex="m6"></ext:Column>
                                                <ext:Column Header="Jul" Width="60" DataIndex="m7"></ext:Column>
                                                <ext:Column Header="Aug" Width="60" DataIndex="m8"></ext:Column>
                                                <ext:Column Header="Sep" Width="60" DataIndex="m9"></ext:Column>
                                                <ext:Column Header="Oct" Width="60" DataIndex="m10"></ext:Column>
                                                <ext:Column Header="Nov" Width="60" DataIndex="m11"></ext:Column>
                                                <ext:Column Header="Dec" Width="60" DataIndex="m12"></ext:Column>
                                                <ext:Column Header="Total" Width="60" DataIndex="Total"></ext:Column>

                                            </Columns>
                                        </ColumnModel>
                                        <Listeners>
                                            <RowDblClick Fn="rowdbclick" />
                                            <RowContextMenu Handler="e.preventDefault();hdrowindex.setValue(rowIndex); #{RowContextMenu}.dataRecord = this.store.getAt(rowIndex);#{RowContextMenu}.showAt(e.getXY());" />
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
                                        <View><ext:GridView ID="GridView1" runat="server" ForceFit="true">                      
                                         </ext:GridView>                    
                                          </View>
                                    </ext:GridPanel>
                                  
                                </Items>
                            </ext:Panel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
            </form>
        </div>
    </div>
</body>
</html>