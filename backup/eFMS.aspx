<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="eFMS.aspx.cs" Inherits="eReimbursement.eFMS" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" 
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>eFMS记账</title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
    <link href="../../../../resources/css/examples.css" rel="stylesheet" type="text/css" />
    <ext:ResourcePlaceHolder ID="ResourcePlaceHolder1" runat="server" Mode="Script" />
    <script type="text/javascript">
        window.lookup = {};
        var GetCheck = function () {
            idcontainer = [];
            var selectgrid = Hidden1.getValue().substr(0, Hidden1.getValue().length - 1).split(',');
            var selectrow = "";
            for (var i = 0; i < selectgrid.length; i++) {
                selectgrid1 = L1_r0_Grid.lookupComponent(selectgrid[i]).value;
                if (selectgrid1 != undefined) {
                    var selectgrid2 = eval(selectgrid1);
                    for (var j = 0; j < selectgrid2.length; j++) {
                        selectrow += selectgrid2[j].RecordID + ',';
                    }
                }
            }
            txtTo1.setValue(selectrow.substr(0, selectrow.length - 1));
        };


        var clean = function (view, isDestroy) {
            var controls = window.lookup[view.grid.id] || {},
                ids = [];

            for (var c in controls) {
                ids.push(controls[c].id || controls[c].storeId);
            }

            if (ids.length > 0) {
                if (isDestroy !== true) {
                    view.grid.getRowExpander().collapseAll();
                }

                for (var i = 0; i < ids.length; i++) {
                    removeFromCache(ids[i], view.grid.id);
                }
            }
        };

        var addToCache = function (c, parent) {
            window.lookup[parent] = window.lookup[parent] || {};
            window.lookup[parent][c] = window[c];
        }

        var removeFromCache = function (c, parent) {
            window.lookup[parent] = window.lookup[parent] || {};

            var ctrl = window.lookup[parent][c];
            delete window.lookup[parent][c];
            if (ctrl) {
                if (ctrl.view) {
                    clean(ctrl.view, true);
                }
                ctrl.destroy();
            }
        }

        var loadLevel = function (expander, record, body, row) {
            if (body.rendered) {
                return;
            }

            var recId = record.id,
                gridId = expander.grid.id,
                dtype = record.data.Type,
                level = record.data.Level;

            mLevels.BuildLevel(level + 1, recId, gridId, dtype, {
                eventMask: {
                    showMask: true,
                    tartget: "customtarget",
                    customTarget: expander.grid.body
                },

                success: function () {
                    body.rendered = true;
                }
            });
        };
        var template = '<span style="color:{0};">{1}</span>';

        var change = function (value) {
            return String.format(template, (value > 0) ? "green" : "red", value);
        };

        var pctChange = function (value) {
            return String.format(template, (value > 0) ? "green" : "red", value + "%");
        };
        var CheckRows = function () {
            var checkitem = CheckboxSelectionModel1.selections;
            var sumno = 0, sumamount = 0;
            for (var i = 0; i < CheckboxSelectionModel1.selections.length; i++) {
                if (checkitem.items[i].data.Amounts && checkitem.items[i].data.Amounts > 0) {
                    sumno++; sumamount += parseFloat(checkitem.items[i].data.Amounts);
                }
            }
            GridPanel2.setTitle("勾选后保存单据.已选择明细张数合计:" + sumno.toString() + ",金额合计:" + Math.round(sumamount * 100) / 100);
        };
        var Fromformat = function (value) {
            var rem = '<input id="Button1" type="text" value="{0}" style="width:100%"/>';
            return String.format(rem, value);
        };
    </script>
    <style type="text/css">
        #Container1
        {
            border: 1px solid #99BBE8;
        }
        #con1
        {
            font-size: 20px;
            line-height: 400px;
            text-align: center;
        }
    </style>
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
                        <li class="q-menuitem"><a href="eFMSAccount.aspx" id="apply">eFMS记账</a></li>
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
            <ext:ResourceManager ID="ResourceManager1" runat="server" DirectMethodNamespace="mLevels" />
            <ext:Hidden ID="Hidden1" runat="server">
            </ext:Hidden>
            <ext:Panel ID="Panel1" runat="server" Height="680" Border="false">
                <Items>
                    <ext:BorderLayout ID="BorderLayout1" runat="server">
                        <West Collapsible="true" Split="true" CollapseMode="Mini">
                            <ext:Panel ID="Panel2" runat="server" Width="175">
                                <Items>
                                    <ext:TreePanel ID="TreePanel1" runat="server" Header="false" Lines="false" UseArrows="true"
                                        RootVisible="false" Width="175" Border="false">
                                        <Root>
                                            <ext:TreeNode Text="eFMS记账" Expanded="true">
                                                <Nodes>
                                                    <ext:TreeNode Text="付款操作" Href="eFMSAccount.aspx" Icon="FlagRed" NodeID="a1" />
                                                    <ext:TreeNode Text="单据传输" Href="eFMSTrans.aspx" Icon="FlagAd" NodeID="b1" />
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
                            <ext:Panel ID="Panel3" runat="server" Height="600" MinHeight="300" Padding="10" AutoScroll="true"
                                Title="付款操作" Layout="AbsoluteLayout">
                                <Items>
                                    <ext:TextField ID="TextField3" runat="server" FieldLabel="表单号" LabelWidth="80" Width="220"
                                        X="10" Y="10">
                                    </ext:TextField>
                                    <ext:ComboBox ID="ComboBox2" runat="server" FieldLabel="单据类型" LabelWidth="65" Width="180"
                                        X="240" Y="10" SelectedIndex="2">
                                        <Items>
                                            <ext:ListItem Text="差旅费" />
                                            <ext:ListItem Text="交际费" />
                                            <ext:ListItem Text="交通费" />
                                            <ext:ListItem Text="通讯费" />
                                            <ext:ListItem Text="其他费用" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:TextField ID="TextField4" runat="server" FieldLabel="金额" LabelWidth="30" Width="155"
                                        X="440" Y="10">
                                    </ext:TextField>
                                    <ext:Label ID="Label2" runat="server" Text="--" X="603" Y="13" Width="15">
                                    </ext:Label>
                                    <ext:TextField ID="TextField5" runat="server" Width="125" X="620" Y="10">
                                    </ext:TextField>
                                    <ext:TextField ID="TextField6" runat="server" FieldLabel="备注" LabelWidth="80" Width="220"
                                        X="10" Y="40">
                                    </ext:TextField>
                                    <ext:DateField ID="DateField3" runat="server" FieldLabel="提交时间" LabelWidth="65" Width="180"
                                        X="240" Y="40" Format="yyyy-MM-dd" EmptyText="0000-00-00">
                                    </ext:DateField>
                                    <ext:Label ID="Label3" runat="server" Text="--" X="425" Y="43" Width="15">
                                    </ext:Label>
                                    <ext:DateField ID="DateField1" runat="server" Width="115" X="440" Y="40" Format="yyyy-MM-dd"
                                        EmptyText="0000-00-00">
                                    </ext:DateField>
                                    <ext:ComboBox ID="ComboBox3" runat="server" FieldLabel="申请人" LabelWidth="80" Width="220"
                                        X="10" Y="70" SelectedIndex="0">
                                        <Items>
                                            <ext:ListItem Text="Hughson Huang" />
                                            <ext:ListItem Text="Paul Lee" />
                                            <ext:ListItem Text="Andy Kang" />
                                            <ext:ListItem Text="Sunhui Chen" />
                                            <ext:ListItem Text="Robin Li" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="ComboBox4" runat="server" FieldLabel="提单人" LabelWidth="65" Width="220"
                                        X="240" Y="70" SelectedIndex="0">
                                        <Items>
                                            <ext:ListItem Text="Hughson Huang" />
                                            <ext:ListItem Text="Paul Lee" />
                                            <ext:ListItem Text="Andy Kang" />
                                            <ext:ListItem Text="Sunhui Chen" />
                                            <ext:ListItem Text="Robin Li" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="ComboBox1" runat="server" FieldLabel="单据状态" LabelWidth="65" Width="175"
                                        X="470" Y="70" SelectedIndex="3">
                                        <Items>
                                            <ext:ListItem Text="部门审批" />
                                            <ext:ListItem Text="单据审核" />
                                            <ext:ListItem Text="核准人审批" />
                                            <ext:ListItem Text="银行付款" />
                                            <ext:ListItem Text="Value+归档" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:Button ID="Button3" runat="server" Text="查找" Width="75" X="670" Y="70" AutoPostBack="true"
                                        OnClick="loadgrid">
                                    </ext:Button>
                                    <ext:Container ID="Container1" runat="server" X="10" Y="100" Height="400" Html="<div id='con1'>请点击查找.</div>">
                                        <Items>
                                        </Items>
                                    </ext:Container>
                                    <ext:Panel ID="Panel4" runat="server" X="10" Y="500" Height="150" Layout="AbsoluteLayout"
                                        Padding="10">
                                        <Items>
                                            <ext:TextField ID="txtAmounts" runat="server" FieldLabel="凭证号" X="10" Y="10" Width="205"
                                                LabelWidth="70" />
                                            <ext:TextField ID="txtFrom" runat="server" FieldLabel="科目" X="225" Y="10" Width="400"
                                                LabelWidth="70">
                                            </ext:TextField>
                                            <ext:ComboBox ID="ComboBox6" runat="server" FieldLabel="银行" LabelWidth="70" Width="205"
                                                X="10" Y="40" SelectedIndex="0">
                                                <Items>
                                                    <ext:ListItem Text="中国银行" />
                                                    <ext:ListItem Text="招商银行" />
                                                    <ext:ListItem Text="浦东发展银行" />
                                                    <ext:ListItem Text="中菲行" />
                                                </Items>
                                            </ext:ComboBox>
                                            <ext:ComboBox ID="ComboBox7" runat="server" FieldLabel="付款方式" LabelWidth="70" Width="205"
                                                X="225" Y="40" SelectedIndex="0">
                                                <Items>
                                                    <ext:ListItem Text="现金" />
                                                    <ext:ListItem Text="支票" />
                                                    <ext:ListItem Text="信用卡" />
                                                    <ext:ListItem Text="支付宝" />
                                                </Items>
                                            </ext:ComboBox>
                                            <ext:Label ID="Label5" runat="server" Text="Air" Width="30" X="480" Y="43">
                                            </ext:Label>
                                            <ext:Checkbox ID="Checkbox1" runat="server" Width="30" X="510" Y="40">
                                            </ext:Checkbox>
                                            <ext:Label ID="Label6" runat="server" Text="Ocean" Width="30" X="565" Y="43">
                                            </ext:Label>
                                            <ext:Checkbox ID="Checkbox2" runat="server" Width="30" X="615" Y="40">
                                            </ext:Checkbox>
                                            <ext:TextField ID="txtTo1" runat="server" FieldLabel="摘要" X="10" Y="70" LabelWidth="70"
                                                Anchor="100%" EmptyText="默认为报销单号+数字+人名+项目">
                                            </ext:TextField>
                                            <ext:Button ID="Button5" runat="server" Text="保存" X="10" Y="100" Width="75">
                                                <Listeners>
                                                    <Click Fn="GetCheck" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Panel>
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
