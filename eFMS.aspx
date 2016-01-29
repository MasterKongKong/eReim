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
    <link href="Styles/StyleSheet2.css" rel="stylesheet" type="text/css" />
    <link href="../../../../resources/css/examples.css" rel="stylesheet" type="text/css" />
    <ext:ResourcePlaceHolder ID="ResourcePlaceHolder1" runat="server" Mode="Script" />
    <script type="text/javascript">
        var expangrid = function (grid) {
            Grid1.lookupComponent(grid).plugins[0].expandAll();
            Grid1.lookupComponent(grid).plugins[0].collapseAll();
        };
        Array.prototype.in_array = function (e) {
            for (i = 0; i < this.length; i++) {
                if (this[i] == e)
                    return true;
            }
            return false;
        };
        var checkarray = function (array, e) {
            for (i = 0; i < array.length; i++) {
                if (array[i] == e)
                    return true;
            }
            return false;
        };
        var addstore2 = function (record) {
            var ex = false;
            for (var i = 0; i < Store2.getRange().length; i++) {
                if (Store2.getRange()[i].data.ID == record.data.ID) {
                    ex = true;
                    break;
                }
            }
            if (!ex) {
                var newrecord = new Store2.recordType();
                newrecord.data.ID = record.data.ID;
                newrecord.data.Type = record.data.Type;
                newrecord.data.Heji = record.data.Heji;
                newrecord.data.Biaodanhao = record.data.Biaodanhao;
                newrecord.data.Tijiao = record.data.Tijiao;
                newrecord.data.Level = record.data.Level;
                Store2.addSorted(newrecord);
            }
        };
        var CountrySelector = {
            add: function (source, destination) {
                var grid1selections = Grid1.selModel.selections.keys;
                var store1all = Store1.getRange();
                Ext.each(store1all, function (record) {
                    var L2gridid = "L2_" + record.data.ID + "_Grid";
                    var L2grid = Grid1.lookupComponent(L2gridid);
                    var L2store = L2grid.store.getRange();
                    var L2selections = L2grid.selModel.getSelections();
                    var L2selectionkeys = L2grid.selModel.selections.keys;
                    var exz = checkarray(grid1selections, record.data.ID);
                    if (exz) {
                        if (record.data.Type == "差旅费") {
                            Ext.each(L2store, function (record2) {
                                addstore2(record2);
                            });
                        }
                        else {
                            Ext.each(L2store, function (record2) {
                                var L3girdid = "L3_" + record2.data.ID + "_Grid";
                                var L3grid = Grid1.lookupComponent(L3girdid);
                                var L3store = L3grid.store.getRange();
                                Ext.each(L3store, function (record3) {
                                    addstore2(record3);
                                });
                            });
                        }
                    }
                    else {
                        if (record.data.Type == "差旅费") {
                            Ext.each(L2selections, function (record2) {
                                addstore2(record2);
                            });
                        }
                        else {
                            Ext.each(L2store, function (record2) {
                                var L3girdid = "L3_" + record2.data.ID + "_Grid";
                                var L3grid = Grid1.lookupComponent(L3girdid);
                                var L3store = L3grid.store.getRange();
                                var L3selections = L3grid.selModel.getSelections();
                                var exz2 = checkarray(L2selectionkeys, record2.data.ID);
                                if (exz2) {
                                    Ext.each(L3store, function (record3) {
                                        addstore2(record3);
                                    });
                                }
                                else {
                                    Ext.each(L3selections, function (record3) {
                                        addstore2(record3);
                                    });
                                }
                            });
                        }
                    }
                });
            },
            addAll: function (source, destination) {
                var grid1sel = Store1.getRange();
                Ext.each(grid1sel, function (record) {
                    var L2gridid = "L2_" + record.data.ID + "_Grid";
                    var L2grid = Grid1.lookupComponent(L2gridid);
                    var L2store = L2grid.store.getRange();
                    if (record.data.Type == "差旅费") {
                        Ext.each(L2store, function (record2) {
                            addstore2(record2);
                        });
                    }
                    else {
                        Ext.each(L2store, function (record2) {
                            var L3girdid = "L3_" + record2.data.ID + "_Grid";
                            var L3grid = Grid1.lookupComponent(L3girdid);
                            var L3store = L3grid.store.getRange();
                            Ext.each(L3store, function (record3) {
                                addstore2(record3);
                            });
                        });
                    }
                });
                Store2.load();
            },
            addByName: function (name) {
                if (!Ext.isEmpty(name)) {
                    var result = Store1.query("Name", name);
                    if (!Ext.isEmpty(result.items)) {
                        GridPanel1.store.remove(result.items[0]);
                        GridPanel2.store.add(result.items[0]);
                    }
                }
            },
            addByNames: function (name) {
                for (var i = 0; i < name.length; i++) {
                    this.addByName(name[i]);
                }
            },
            remove: function (source, destination) {
                GridPanel2.deleteSelected();

            },
            removeAll: function (source, destination) {
                Store2.removeAll();
            }
        };

        window.lookup = {};
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
                    customTarget: Grid1
                },
                before: function () {
                    Button4.disable();
                    Button6.disable();
                },
                success: function () {
                    body.rendered = true;
                    Button4.enable();
                    Button6.enable();
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
        .x-toolbar .my-toolbar
        {
            background-color: #F3F3F3;
            border: 1px solid #F4F4F4;
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
                        <li class="q-menuitem"><a href="#" target="_top">首页</a></li>
                        <li class="q-menuitem"><a href="MyClaims.aspx">报销申请</a></li>
                        <li class="q-menuitem"><a href="Approve.aspx">报销审核</a></li>
                        <li class="q-menuitem"><a href="FileManagement.aspx">影像管理</a></li>
                        <li class="q-menuitem"><a href="Budget.aspx">预算管理</a></li>
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
            <ext:Hidden ID="hdExpand" runat="server">
            </ext:Hidden>
            <ext:Store runat="server" ID="Store2" OnSubmitData="SubmitData">
                <SortInfo Field="ID" Direction="ASC" />
                <Reader>
                    <ext:JsonReader IDProperty="ID">
                        <Fields>
                            <ext:RecordField Name="ID" />
                            <ext:RecordField Name="Type" />
                            <ext:RecordField Name="Heji" />
                            <ext:RecordField Name="Biaodanhao" />
                            <ext:RecordField Name="Tijiao" />
                            <ext:RecordField Name="Level" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Panel ID="Panel1" runat="server" Height="680" Border="false" Margins="10">
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
                                                    <ext:TreeNode Text="付款操作" Href="eFMS.aspx" Icon="FlagRed" NodeID="a1" />
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
                            <ext:Panel ID="Panel3" runat="server" MinHeight="300" Padding="10" AutoScroll="true"
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
                                    <ext:Button ID="Button3" runat="server" Text="查找" Width="75" X="670" Y="70">
                                        <DirectEvents>
                                            <Click OnEvent="loadgrid2" Before="Panel5.clearContent();Button4.disable();Button6.disable();"
                                                Timeout="300000" Success="Button4.enable();Button6.enable();">
                                                <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="Panel5" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Panel ID="Panel6" runat="server" X="10" Y="100" Height="530" Border="false">
                                        <Items>
                                            <ext:ColumnLayout ID="ColumnLayout1" runat="server" FitHeight="true">
                                                <Columns>
                                                    <ext:LayoutColumn ColumnWidth="0.6">
                                                        <ext:Panel ID="Panel5" runat="server" Html="<div id='con1'>请点击查找.</div>">
                                                            <Items>
                                                            </Items>
                                                        </ext:Panel>
                                                    </ext:LayoutColumn>
                                                    <ext:LayoutColumn>
                                                        <ext:Panel ID="Panel9" runat="server" Width="32" BodyStyle="background-color: transparent;"
                                                            Border="false" Layout="Anchor">
                                                            <Items>
                                                                <ext:Panel ID="Panel11" runat="server" Border="false" BodyStyle="background-color: transparent;"
                                                                    Padding="5">
                                                                    <Items>
                                                                        <ext:Button ID="Button4" runat="server" Icon="BulletAdd" StyleSpec="margin-bottom:2px;">
                                                                            <Listeners>
                                                                                <Click Handler="CountrySelector.add();" />
                                                                            </Listeners>
                                                                            <ToolTips>
                                                                                <ext:ToolTip ID="ToolTip1" runat="server" Title="Add" Html="Add Selected Rows" />
                                                                            </ToolTips>
                                                                        </ext:Button>
                                                                        <ext:Button ID="Button6" runat="server" Icon="Add" StyleSpec="margin-bottom:2px;">
                                                                            <Listeners>
                                                                                <Click Handler="CountrySelector.addAll();" />
                                                                            </Listeners>
                                                                            <ToolTips>
                                                                                <ext:ToolTip ID="ToolTip2" runat="server" Title="Add all" Html="Add All Rows" />
                                                                            </ToolTips>
                                                                        </ext:Button>
                                                                        <ext:Button ID="Button7" runat="server" Icon="BulletDelete" StyleSpec="margin-bottom:2px;">
                                                                            <Listeners>
                                                                                <Click Handler="CountrySelector.remove();" />
                                                                            </Listeners>
                                                                            <ToolTips>
                                                                                <ext:ToolTip ID="ToolTip3" runat="server" Title="Remove" Html="Remove Selected Rows" />
                                                                            </ToolTips>
                                                                        </ext:Button>
                                                                        <ext:Button ID="Button8" runat="server" Icon="Delete" StyleSpec="margin-bottom:2px;">
                                                                            <Listeners>
                                                                                <Click Handler="CountrySelector.removeAll();" />
                                                                            </Listeners>
                                                                            <ToolTips>
                                                                                <ext:ToolTip ID="ToolTip4" runat="server" Title="Remove all" Html="Remove All Rows" />
                                                                            </ToolTips>
                                                                        </ext:Button>
                                                                    </Items>
                                                                </ext:Panel>
                                                            </Items>
                                                        </ext:Panel>
                                                    </ext:LayoutColumn>
                                                    <ext:LayoutColumn ColumnWidth="0.4">
                                                        <ext:Panel ID="Panel7" runat="server" Border="false" Layout="AbsoluteLayout">
                                                            <Items>
                                                                <ext:Panel ID="Panel8" runat="server" Layout="AbsoluteLayout" X="0" Y="0" Height="60"
                                                                    Title="点击左侧&quot;-&quot;按钮以删减数据.">
                                                                    <Items>
                                                                        <ext:TextField ID="TextField1" runat="server" FieldLabel="凭证号" X="3" Y="3" Width="205"
                                                                            LabelWidth="55" EmptyText="输入凭证号点击&quot;载入&quot;" />
                                                                        <ext:Button ID="Button2" runat="server" Text="载入" X="218" Y="3" Width="50">
                                                                        </ext:Button>
                                                                    </Items>
                                                                </ext:Panel>
                                                                <ext:GridPanel ID="GridPanel2" runat="server" EnableDragDrop="false" AutoExpandColumn="ID"
                                                                    StoreID="Store2" X="0" Y="60" Height="280">
                                                                    <ColumnModel ID="ColumnModel1" runat="server">
                                                                        <Columns>
                                                                            <ext:RowNumbererColumn Width="30" />
                                                                            <ext:Column Header="费用" DataIndex="ID" />
                                                                            <ext:Column Header="COA" DataIndex="Biaodanhao" />
                                                                        </Columns>
                                                                    </ColumnModel>
                                                                    <SelectionModel>
                                                                        <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" />
                                                                    </SelectionModel>
                                                                </ext:GridPanel>
                                                                <ext:Panel ID="Panel10" runat="server" Layout="AbsoluteLayout" Padding="10" X="0"
                                                                    Y="340" Height="190">
                                                                    <Items>
                                                                        <ext:TextField ID="txtFrom" runat="server" FieldLabel="科目" X="10" Y="10" Anchor="100%"
                                                                            LabelWidth="60">
                                                                        </ext:TextField>
                                                                        <ext:ComboBox ID="ComboBox6" runat="server" FieldLabel="银行" LabelWidth="60" Anchor="100%"
                                                                            X="10" Y="40" SelectedIndex="0">
                                                                            <Items>
                                                                                <ext:ListItem Text="中国银行" />
                                                                                <ext:ListItem Text="招商银行" />
                                                                                <ext:ListItem Text="浦东发展银行" />
                                                                                <ext:ListItem Text="中菲行" />
                                                                            </Items>
                                                                        </ext:ComboBox>
                                                                        <ext:ComboBox ID="ComboBox7" runat="server" FieldLabel="付款方式" LabelWidth="60" X="10"
                                                                            Y="70" SelectedIndex="0" Anchor="100%">
                                                                            <Items>
                                                                                <ext:ListItem Text="现金" />
                                                                                <ext:ListItem Text="支票" />
                                                                                <ext:ListItem Text="信用卡" />
                                                                                <ext:ListItem Text="支付宝" />
                                                                            </Items>
                                                                        </ext:ComboBox>
                                                                        <ext:Label ID="Label5" runat="server" Text="Air" Width="30" X="10" Y="103">
                                                                        </ext:Label>
                                                                        <ext:Checkbox ID="Checkbox1" runat="server" Width="30" X="40" Y="100">
                                                                        </ext:Checkbox>
                                                                        <ext:Label ID="Label6" runat="server" Text="Ocean" Width="40" X="80" Y="103">
                                                                        </ext:Label>
                                                                        <ext:Checkbox ID="Checkbox2" runat="server" Width="30" X="125" Y="100">
                                                                        </ext:Checkbox>
                                                                        <ext:ComboBox ID="ComboBox5" runat="server" FieldLabel="部门" LabelWidth="30" Anchor="100%"
                                                                            X="160" Y="100" SelectedIndex="0">
                                                                            <Items>
                                                                                <ext:ListItem Text="MIS" />
                                                                                <ext:ListItem Text="MIS" />
                                                                                <ext:ListItem Text="MIS" />
                                                                                <ext:ListItem Text="MIS" />
                                                                            </Items>
                                                                        </ext:ComboBox>
                                                                        <ext:TextField ID="txtTo1" runat="server" FieldLabel="Note" X="10" Y="130" LabelWidth="30"
                                                                            Anchor="100%" EmptyText="默认为报销单号+数字+人名+项目">
                                                                        </ext:TextField>
                                                                        <ext:Button ID="Button5" runat="server" Text="保存" X="10" Y="160" Width="75">
                                                                            <%--<Listeners>
                                                    <Click Handler="#{GridPanel2}.submitData();" />
                                                </Listeners>--%>
                                                                            <DirectEvents>
                                                                                <Click OnEvent="SaveGrid2" Before="#{GridPanel2}.submitData();">
                                                                                    <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="Panel7" />
                                                                                </Click>
                                                                            </DirectEvents>
                                                                        </ext:Button>
                                                                    </Items>
                                                                </ext:Panel>
                                                            </Items>
                                                        </ext:Panel>
                                                    </ext:LayoutColumn>
                                                </Columns>
                                            </ext:ColumnLayout>
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
