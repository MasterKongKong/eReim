<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplyOthers.aspx.cs" Inherits="eReimbursement.ApplyOthers" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>其他费用申请</title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        var ConfirmSave = function () {
            Ext.Msg.confirm("提示", "是否此报销所有明细均已输入完毕，现在提交申请?", function (btn, text) {
                if (btn == 'yes') {
                    return true;
                }
            });
        };
        var template = '<span style="color:{0};">{1}</span>';

        var change = function (value) {
            return String.format(template, (value > 57.1) ? "green" : "red", value);
        };

        var pctChange = function (value) {
            return String.format(template, (value > 0) ? "green" : "red", value + "%");
        };
        var prepare = function (grid, toolbar, rowIndex, record) {
            var firstButton = toolbar.items.get(0);

            if (record.data.Amounts < 57.2) {
                firstButton.setDisabled(true);
                firstButton.setTooltip("Disabled");
            }

            //you can return false to cancel toolbar for this record
        };
        var deleteRows = function (grid, record) {
            Ext.Msg.confirm('Delete Rows', 'Are you sure?', function (btn) {
                if (btn == 'yes') {
                    //                    grid.deleteSelected();
                    Store1.remove(record);
                    //                    if (GridPanel1.getSelectionModel().getCount() == 1 && GridPanel1.getSelectionModel().getSelections()[0] == record) {
                    //                        dfDate.reset();
                    //                        txtAmounts.reset();
                    //                        txtFrom1.reset();
                    //                        txtTo.reset();
                    //                        txtPurpose.reset();
                    //                    }
                }

                //return focus
                grid.view.focusEl.focus();
            })
        };
        var getDetail = function (row) {
            var reg = new RegExp("<br />");
            dfDate.setValue(row.data.DateT);
            txtAmounts.setValue(row.data.Amounts);
            txtFrom.setValue(row.data.From.replace(reg, "\n"));
            txtTo1.setValue(row.data.To.replace(reg, "\n"));
            txtPurpose1.setValue(row.data.Purpose.replace(reg, "\n"));
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
                        <li class="q-menuitem"><a href="MyClaims.aspx" id="apply">报销申请</a></li>
                        <li class="q-menuitem"><a href="Approve.aspx">报销审核</a></li>
                        <li class="q-menuitem"><a href="FileManagement.aspx">影像管理</a></li>
                        <li class="q-menuitem"><a href="Budget.aspx">预算管理</a></li>
                        <li class="q-menuitem"><a href="eFMSAccount.aspx">eFMS记账</a></li>
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
                            <ext:RecordField Name="DateT" Type="Date" DateFormat="yyyy-MM-dd" SortType="AsDate" />
                            <ext:RecordField Name="Type" />
                            <ext:RecordField Name="Amounts" />
                            <ext:RecordField Name="Miaoshu" />
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
                                                    <ext:TreeNode Text="新建申请单" Expanded="true" Href="#" Icon="UserKey">
                                                        <Nodes>
                                                            <ext:TreeNode Text="差旅费申请" Href="ApplyTravel.aspx" Icon="FlagAe" NodeID="a1" />
                                                            <ext:TreeNode Text="交际费申请" Href="ApplyEntertainment.aspx" Icon="FlagAf" NodeID="a2" />
                                                            <ext:TreeNode Text="交通费申请" Href="ApplyTransportation.aspx" Icon="FlagAg" NodeID="a3" />
                                                            <ext:TreeNode Text="通讯费申请" Href="ApplyCommu.aspx" Icon="FlagAi" NodeID="a4" />
                                                            <ext:TreeNode Text="其他费用申请" Href="ApplyOthers.aspx" Icon="FlagAl" NodeID="a5" />
                                                        </Nodes>
                                                    </ext:TreeNode>
                                                    <ext:TreeNode Text="草稿报销单" Href="MyDrafts.aspx" Icon="FlagAm" NodeID="b1" />
                                                    <ext:TreeNode Text="我的报销单" Href="MyClaims.aspx" Icon="FlagAn" NodeID="c1" />
                                                </Nodes>
                                            </ext:TreeNode>
                                        </Root>
                                        <Listeners>
                                            <AfterRender Handler="TreePanel1.getNodeById('a5').select(true);" />
                                        </Listeners>
                                    </ext:TreePanel>
                                </Items>
                            </ext:Panel>
                        </West>
                        <Center>
                            <ext:Panel ID="Panel3" runat="server" Title="其他费用申请单 - 表单:ZJDBJS2013010001" Height="600"
                                Padding="10" MinHeight="300" AutoScroll="true" Layout="AbsoluteLayout">
                                <Items>
                                    <ext:ComboBox ID="ComboBox2" runat="server" FieldLabel="报销人" LabelWidth="60" Width="220"
                                        X="10" Y="10" SelectedIndex="0">
                                        <Items>
                                            <ext:ListItem Text="Hughson Huang" />
                                            <ext:ListItem Text="Paul Lee" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:Label ID="Label5" runat="server" Width="60" X="240" Y="13" Text="部门:" />
                                    <ext:Label ID="Label6" runat="server" Width="160" X="305" Y="13" Text="Manager" />
                                    <ext:ComboBox ID="ComboBox1" runat="server" FieldLabel="站点" LabelWidth="60" Width="220"
                                        X="10" Y="40" SelectedIndex="0">
                                        <Items>
                                            <ext:ListItem Text="GCR" />
                                            <ext:ListItem Text="ZJDBJS" />
                                            <ext:ListItem Text="ZJDTSN" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:DateField ID="DateField3" runat="server" FieldLabel="报销月度" LabelWidth="60" Width="220"
                                        X="240" Y="40" Format="yyyy-MM" EmptyText="0000-00">
                                    </ext:DateField>
                                    <ext:TextArea ID="TextArea1" runat="server" FieldLabel="备注" X="470" Y="10" Width="300"
                                        LabelWidth="50" Height="52" />
                                    <ext:Label ID="Label8" runat="server" Width="60" X="10" Y="73" Text="上传文件:" />
                                    <ext:FileUploadField ID="BasicField" runat="server" Width="385" Icon="Attach" X="65"
                                        Y="60" />
                                    <ext:Button ID="Button8" runat="server" Text="新增费用" X="10" Y="100" Width="80">
                                        <Listeners>
                                            <Click Handler="Panel4.show();" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:Button ID="Button3" runat="server" Text="保存并申请" X="100" Y="100" Width="80">
                                        <Listeners>
                                            <Click Fn="ConfirmSave" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:Button ID="Button1" runat="server" Text="存为草稿" X="190" Y="100" Width="80">
                                    </ext:Button>
                                    <ext:Button ID="Button11" runat="server" Text="放弃" X="280" Y="100" Width="80">
                                    </ext:Button>
                                    <ext:Button ID="Button2" runat="server" Text="打印" X="370" Y="100" Width="80">
                                    </ext:Button>
                                    <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                        Title="单击每行以在下方编辑.报销张数合计:5 金额报销合计:12070.21." TrackMouseOver="false" Height="200"
                                        X="10" Y="130">
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                                <ext:RowNumbererColumn Width="30" />
                                                <ext:CommandColumn Width="110">
                                                    <Commands>
                                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="编辑" />
                                                        <ext:GridCommand Icon="Delete" CommandName="Delete" Text="删除" />
                                                    </Commands>
                                                </ext:CommandColumn>
                                                <ext:DateColumn Header="日期" Width="80" DataIndex="DateT" Format="yyyy-MM-dd" />
                                                <ext:Column Header="费用类型" Width="120" DataIndex="Type" />
                                                <ext:Column Header="金额" Width="60" DataIndex="Amounts">
                                                    <Renderer Fn="change" />
                                                </ext:Column>
                                                <ext:Column Header="费用描述" Width="100" DataIndex="Miaoshu" />
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                                <Listeners>
                                                    <RowSelect Handler="Panel4.show();" />
                                                </Listeners>
                                            </ext:RowSelectionModel>
                                        </SelectionModel>
                                        <Listeners>
                                            <RowContextMenu Handler="e.preventDefault();hdrowindex.setValue(rowIndex); #{RowContextMenu}.dataRecord = this.store.getAt(rowIndex);#{RowContextMenu}.showAt(e.getXY());" />
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
                                    <ext:Panel ID="Panel4" runat="server" X="10" Y="330" Height="160" Layout="AbsoluteLayout"
                                        Padding="10" Hidden="true">
                                        <Items>
                                            <ext:DateField ID="dfDate" runat="server" FieldLabel="日期" Format="yyyy-MM-dd" X="10"
                                                Y="10" Width="205" LabelWidth="70" EmptyText="0000-00-00" />
                                            <ext:TextField ID="txtAmounts" runat="server" FieldLabel="金额" X="225" Y="10" Width="205"
                                                LabelWidth="70" />
                                            <ext:Label ID="Label1" runat="server" X="440" Y="13" Text="/12070.21 合计" />
                                            <ext:DropDownField ID="Field3" runat="server" Editable="false" Width="300" TriggerIcon="SimpleArrowDown"
                                                FieldLabel="费用类型" LabelWidth="70" X="10" Y="40">
                                                <Component>
                                                    <ext:TreeGrid ID="TreeGrid1" runat="server" Title="费用类型" Width="700" Height="300"
                                                        NoLeafIcon="true" EnableDD="true">
                                                        <Columns>
                                                            <ext:TreeGridColumn Header="中文名称" Width="230" DataIndex="CName" />
                                                            <ext:TreeGridColumn Header="GLCode" Width="100" DataIndex="GLCode" />
                                                            <ext:TreeGridColumn Header="GLName" Width="300" DataIndex="GLName" />
                                                        </Columns>
                                                        <Root>
                                                            <ext:TreeNode Text="Tasks">
                                                                <Nodes>
                                                                    <ext:TreeNode Icon="Folder" Expanded="false">
                                                                        <CustomAttributes>
                                                                            <ext:ConfigItem Name="CName" Value="其他可控管的營業費用" Mode="Value" />
                                                                            <ext:ConfigItem Name="GLCode" Value="62010000" />
                                                                            <ext:ConfigItem Name="GLName" Value="Other expenses-controllable" Mode="Value" />
                                                                        </CustomAttributes>
                                                                        <Nodes>
                                                                            <ext:TreeNode Icon="Folder">
                                                                                <CustomAttributes>
                                                                                    <ext:ConfigItem Name="CName" Value="廣告費" Mode="Value" />
                                                                                    <ext:ConfigItem Name="GLCode" Value="62010100" />
                                                                                    <ext:ConfigItem Name="GLName" Value="Advertising" Mode="Value" />
                                                                                </CustomAttributes>
                                                                                <Listeners>
                                                                                    <Click Handler="Field3.setValue(this.attributes.CName);" />
                                                                                </Listeners>
                                                                            </ext:TreeNode>
                                                                            <ext:TreeNode Icon="Folder" Expanded="true">
                                                                                <CustomAttributes>
                                                                                    <ext:ConfigItem Name="CName" Value="會議費用" Mode="Value" />
                                                                                    <ext:ConfigItem Name="GLCode" Value="62010200" />
                                                                                    <ext:ConfigItem Name="GLName" Value="Annual/Quarterly meeting expense" Mode="Value" />
                                                                                </CustomAttributes>
                                                                                <Nodes>
                                                                                    <ext:TreeNode Leaf="true">
                                                                                        <CustomAttributes>
                                                                                            <ext:ConfigItem Name="CName" Value="會議費用-CRP Office 辦的" Mode="Value" />
                                                                                            <ext:ConfigItem Name="GLCode" Value="62010201" />
                                                                                            <ext:ConfigItem Name="GLName" Value="Annual Meeting expenses - CRP" Mode="Value" />
                                                                                        </CustomAttributes>
                                                                                        <Listeners>
                                                                                            <Click Handler="Field3.setValue(this.attributes.CName);" />
                                                                                        </Listeners>
                                                                                    </ext:TreeNode>
                                                                                    <ext:TreeNode Icon="Folder">
                                                                                        <CustomAttributes>
                                                                                            <ext:ConfigItem Name="CName" Value="會議費用-Region 辦的" Mode="Value" />
                                                                                            <ext:ConfigItem Name="GLCode" Value="62010202" />
                                                                                            <ext:ConfigItem Name="GLName" Value="Annual meeting expense - region" Mode="Value" />
                                                                                        </CustomAttributes>
                                                                                        <Listeners>
                                                                                            <Click Handler="Field3.setValue(this.attributes.CName);" />
                                                                                        </Listeners>
                                                                                    </ext:TreeNode>
                                                                                </Nodes>
                                                                            </ext:TreeNode>
                                                                        </Nodes>
                                                                    </ext:TreeNode>
                                                                    <ext:TreeNode Icon="Folder" Expanded="true">
                                                                        <CustomAttributes>
                                                                            <ext:ConfigItem Name="CName" Value="其他不能控管的營業費用" Mode="Value" />
                                                                            <ext:ConfigItem Name="GLCode" Value="62020000" />
                                                                            <ext:ConfigItem Name="GLName" Value="Other expense-non-controllable" Mode="Value" />
                                                                        </CustomAttributes>
                                                                        <Nodes>
                                                                            <ext:TreeNode Icon="Folder">
                                                                                <CustomAttributes>
                                                                                    <ext:ConfigItem Name="CName" Value="攤銷" Mode="Value" />
                                                                                    <ext:ConfigItem Name="GLCode" Value="62020100" />
                                                                                    <ext:ConfigItem Name="GLName" Value="Amortization" Mode="Value" />
                                                                                </CustomAttributes>
                                                                                <Listeners>
                                                                                    <Click Handler="Field3.setValue(this.attributes.CName);" />
                                                                                </Listeners>
                                                                            </ext:TreeNode>
                                                                            <ext:TreeNode Icon="Folder">
                                                                                <CustomAttributes>
                                                                                    <ext:ConfigItem Name="CName" Value="查帳費" Mode="Value" />
                                                                                    <ext:ConfigItem Name="GLCode" Value="62020200" />
                                                                                    <ext:ConfigItem Name="GLName" Value="Audit fee" Mode="Value" />
                                                                                </CustomAttributes>
                                                                                <Nodes>
                                                                                    <ext:TreeNode Leaf="true">
                                                                                        <CustomAttributes>
                                                                                            <ext:ConfigItem Name="CName" Value="查帳費-內部查帳" Mode="Value" />
                                                                                            <ext:ConfigItem Name="GLCode" Value="62020201" />
                                                                                            <ext:ConfigItem Name="GLName" Value="Audit fee - Internal audit" Mode="Value" />
                                                                                        </CustomAttributes>
                                                                                        <Listeners>
                                                                                            <Click Handler="Field3.setValue(this.attributes.CName);" />
                                                                                        </Listeners>
                                                                                    </ext:TreeNode>
                                                                                    <ext:TreeNode Leaf="true" NodeID="a2">
                                                                                        <CustomAttributes>
                                                                                            <ext:ConfigItem Name="CName" Value="查帳費-外部查帳-(會計師)" Mode="Value" />
                                                                                            <ext:ConfigItem Name="GLCode" Value="62020202" />
                                                                                            <ext:ConfigItem Name="GLName" Value="Audit fee - External audit" Mode="Value" />
                                                                                        </CustomAttributes>
                                                                                        <Listeners>
                                                                                            <Click Handler="Field3.setValue(this.attributes.CName);" />
                                                                                        </Listeners>
                                                                                    </ext:TreeNode>
                                                                                </Nodes>
                                                                            </ext:TreeNode>
                                                                        </Nodes>
                                                                    </ext:TreeNode>
                                                                </Nodes>
                                                            </ext:TreeNode>
                                                        </Root>
                                                    </ext:TreeGrid>
                                                </Component>
                                            </ext:DropDownField>
                                            <ext:TextField ID="txtPurpose1" runat="server" FieldLabel="费用描述" X="10" Y="70" Width="750"
                                                LabelWidth="70" Anchor="100%">
                                            </ext:TextField>
                                            <ext:Button ID="Button5" runat="server" Text="编辑" X="10" Y="130" Width="75">
                                            </ext:Button>
                                            <ext:Button ID="Button6" runat="server" Text="新增" X="95" Y="130" Width="75">
                                            </ext:Button>
                                            <ext:Button ID="Button7" runat="server" Text="清空" X="180" Y="130" Width="75">
                                            </ext:Button>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Container ID="Container1" runat="server" X="10" Y="500">
                                        <Content>
                                            <div style="white-space: nowrap; float: left;">
                                                <div class="StatusIconS" id="Div6">
                                                    <span class="spanIcon">开始</span>
                                                </div>
                                                <div class="StatusIcon StatusIcon0" id="customers">
                                                    <span class="spanIcon">填写单据<br />
                                                        Hughson Huang<br />
                                                        2012-12-13</span><b class="bIcon bIcon1"></b>
                                                </div>
                                                <div class="StatusIcon StatusIcon0" id="Div1">
                                                    <span class="spanIcon">部门审批<br />
                                                        Hughson Huang<br />
                                                        2012-12-13</span><b class="bIcon bIcon1"></b>
                                                </div>
                                                <div class="StatusIcon StatusIcon1" id="Div2">
                                                    <span class="spanIcon">单据审核<br />
                                                        Billy Yu<br />
                                                        2012-12-14</span><b class="bIcon bIcon1"></b>
                                                </div>
                                                <div class="StatusIcon StatusIcon2" id="Div3">
                                                    <span class="spanIcon">核准人审批<br />
                                                        Paul Lee</span><b class="bIcon bIcon1"></b>
                                                </div>
                                                <div class="StatusIcon StatusIcon2" id="Div4">
                                                    <span class="spanIcon">银行付款<br />
                                                        Anna Shi</span><b class="bIcon bIcon1"></b>
                                                </div>
                                                <div class="StatusIcon StatusIcon2" id="Div5">
                                                    <span class="spanIcon">Value+归档<br />
                                                        Anna Shi</span><b class="bIcon bIcon1"></b>
                                                </div>
                                                <div class="StatusIconE" id="Div7">
                                                    <span class="spanIcon">结束</span>
                                                </div>
                                            </div>
                                            <div style="white-space: nowrap; float: left;">
                                                <div class="Icon0">
                                                </div>
                                                <span class="Icon0text">完成或者通过</span>
                                                <div class="Icon1">
                                                </div>
                                                <span class="Icon0text">正在进行</span>
                                                <div class="Icon2">
                                                </div>
                                                <span class="Icon0text">拒绝</span>
                                                <div class="Icon3">
                                                </div>
                                                <span class="Icon0text">未开始</span>
                                            </div>
                                        </Content>
                                    </ext:Container>
                                    <ext:Label ID="Label7" runat="server" Text="Created By: 2012-12-13 ( Hughson Huang ) Updated By: 2012-12-13 ( Hughson Huang )"
                                        X="20" Y="620">
                                    </ext:Label>
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
