<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplyTravel.aspx.cs" Inherits="eReimbursement.ApplyTravel" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>差旅费</title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
    <link href="Styles/StyleSheet1.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/PageJS.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("div.gn_person ul.q-menubox li:eq(4) a").click(function () {
                Ext.Msg.confirm('提示', '确认登出?', function (btn, text) {
                    if (btn == 'yes') {
                        $('div.gn_person ul.q-menubox li:eq(0) a').text('请点击此处登录或切换用户.');
                        RM.Logout({
                        });
                    }
                });
            });
            $("div.gn_person ul.q-menubox li:eq(0) a").click(function () {
                loginWindow.show();
            });
        });
        var saveData = function () {
            GridData.setValue(Ext.encode(GridPanel1.getRowsValues({ filterRecord: GridPanel1.filters.getRecordFilter() })));
        };
        var CheckStore = function () {
            if (Store3.getAllRange().length < 1) {
                Ext.Msg.show({ title: '提示', msg: '无费用需申请.', buttons: { ok: 'Ok'} });
                return false;
            }
            return true;
        };
        var PasseLeaveData = function (command, record, rowIndex) {
            dfBdate.setValue(record.data.leaveStart1);
            dfEdate.setValue(record.data.leaveEnd1);
            btnGeteLeave.toggle();
        };
        var prepare = function (grid, toolbar, rowIndex, record) {
            var firstButton = toolbar.items.get(1);
            if ((Request.QueryString('Copy') != 'T' || Request.QueryString('Copy') == null) && record.data.Type == '0') {
                firstButton.setDisabled(true);
                firstButton.setTooltip("Disabled");
            }
            //you can return false to cancel toolbar for this record
        };
        var ShowFunction = function (btn) {
            window.location.href = './Approve.aspx';
        };
        var GetSum = function () {
            //重新计算合计
            var Pamount = 0; var Camount = 0;
            var data = Store3.getAllRange();
            Ext.each(data, function (record) {
                Pamount += record.data.Pamount == '' ? 0 : parseFloat(record.data.Pamount);
                Camount += record.data.Camount == '' ? 0 : parseFloat(record.data.Camount);
            });
            txtPersonalSum.setValue(GetNumber(Pamount.toString()));
            txtCompanySum.setValue(GetNumber(Camount.toString()));
            txtSum.setValue(GetNumber((Pamount + Camount).toString()));
            hdPamountSum.setValue(Pamount.toString());
            hdCamountSum.setValue(Camount.toString());
            hdSum.setValue((Pamount + Camount).toString());
        };
        var RowCommand = function (command, record, rowIndex) {
            if (command == 'Edit') {
                RowSelectionModel1.selectRow(rowIndex, true);
            }
            else if (command == 'Delete') {
                Store3.removeAt(rowIndex);
                GetSum();
                Store3.load();
                hdDetailID.setValue('');
            }
        };
        var UpdateStore3 = function (a) {
            //            Store3.remove(Store3.query("ID", '-2').items[0])
            if (a == 'Update') {
                if (hdDetailID.getValue() != undefined && hdDetailID.getValue() != '') {
                    var id = hdTempDetailID.getValue();
                    Store3.getById(id).set('Tocity', cbxCity.getValue());
                    Store3.getById(id).set('AccountName', cbxCOAType.getRawValue());
                    Store3.getById(id).set('AccountCode', hdCOAType.getValue());
                    Store3.getById(id).set('AccountDes', txtCOAContent.getValue());
                    Store3.getById(id).set('Cur', LabelCurrency.getText());
                    Store3.getById(id).set('Pamount', txtAmount1.getValue());
                    Store3.getById(id).set('Camount', txtAmount2.getValue());
                    Store3.getById(id).set('TSation', cbxCOACenter.getValue());
                    Store3.getById(id).set('Tdate', dfDate.getRawValue());
                    GetSum();
                }
                else {
                    Ext.Msg.show({ title: '提示', msg: '该行信息不存在.', buttons: Ext.Msg.OK, icon: Ext.Msg.WARNING });
                    return false;
                }
            }
            else if (a == 'Insert') {
                if (hdDetailID.getValue() == '' || parseInt(hdDetailID.getValue()) >= 0) {
                    hdDetailID.setValue('-1');
                }
                else {
                    hdDetailID.setValue((parseInt(hdDetailID.getValue()) - 1).toString());
                }
                var newrecord = new Store3.recordType();
                newrecord.data.ID = hdDetailID.getValue();
                newrecord.data.Tocity = cbxCity.getValue();
                newrecord.data.AccountName = cbxCOAType.getRawValue();
                newrecord.data.AccountCode = hdCOAType.getValue();
                newrecord.data.AccountDes = txtCOAContent.getValue();
                newrecord.data.Cur = LabelCurrency.getText();
                newrecord.data.Pamount = txtAmount1.getValue();
                newrecord.data.Camount = txtAmount2.getValue();
                newrecord.data.TSation = cbxCOACenter.getValue();
                newrecord.data.Tdate = dfDate.getRawValue();
                Store3.add(newrecord);
                GetSum();
                Store3.load();
            }
        };
        var SaveTravelDetail = function (ty) {
            RM.SaveTravelDetail(ty, {
                eventMask: {
                    showMask: true,
                    tartget: "customtarget",
                    customTarget: GridPanel2
                },
                before: function () {
                    GridPanel2.submitData();
                },
                success: function () {

                },
                timeout: 300000
            });
        };
        var SaveTravelRequest = function () {
            Ext.Msg.confirm('提示', '保存后无法修改,是否保存?', function (btn, text) {
                if (btn == 'yes') {
                    RM.SaveTravelRequest({
                        eventMask: {
                            showMask: true,
                            tartget: "customtarget",
                            customTarget: Panel3
                        },
                        before: function () {

                        },
                        success: function () {

                        },
                        timeout: 300000
                    });
                }
            });
        };
        var SelectType = function (combox, data, selectindex) {
            //            PanelBudget.show();
            this.triggers[0].show();
            var a = 1;
            hdCOAType.setValue(cbxCOAType.getValue());
        };
        var GetList = function (a) {
            this.triggers[0][this.getRawValue().toString().length == 0 ? 'hide' : 'show']();
            var q = a.query;

            a.query = new RegExp(q);
            a.query.length = q.length;
        };


        var GetStatus = function (a, pressed) {
            if (pressed) {
                Window1.show();
                RM.GetDataFromeLeave({
                    eventMask: {
                        showMask: true,
                        tartget: "customtarget",
                        customTarget: GridPanel1
                    },
                    timeout: 300000
                });
            }
            else {
                Window1.hide();
            }
        };
        var GetBudget = function (a) {
            if (a.getValue() == "") {
                Label7.setText("本站预算－个人：1000/10000");
                Label9.setText("部门：5000/50000");
                Label10.setText("站点：20000/200000");
            }
            else if (a.getValue() == "ZJDTSN") {
                Label7.setText("该站预算－个人：N/A");
                Label9.setText("部门：N/A");
                Label10.setText("站点：30000/300000");
            }
            else if (a.getValue() == "GCRSHA") {
                Label7.setText("本站预算－个人：N/A");
                Label9.setText("部门：N/A");
                Label10.setText("站点：130000/1300000");
            }
            else if (a.getValue() == "ZJDTAO") {
                Label7.setText("本站预算－个人：N/A");
                Label9.setText("部门：N/A");
                Label10.setText("站点：13000/130000");
            }
            showtrigger(a);
        };
        var showtrigger = function (a) {
            a.triggers[0].show();
        };
        var ConfirmSave = function () {
            Ext.Msg.confirm("提示", "是否此报销所有明细均已输入完毕，现在提交申请?", function (btn, text) {
                if (btn == 'yes') {
                    //                    Ext.net.DirectMethods.Deletetask(cargoid, com, {
                    //                        success: function () {
                    //                            Panel5.ajaxListeners.activate.fire();
                    //                        },
                    //                        failure: function (errorMsg) {
                    //                            Ext.Msg.alert('Failure', errorMsg);
                    //                        },
                    //                        eventMask: { showMask: true }
                    //                    });
                    return true;
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
        var GetDetail = function (row) {
            var reg = new RegExp("<br />");
            cbxCity.setValue(row.data.Tocity);
            cbxCOAType.setValue(row.data.AccountName);
            hdCOAType.setValue(row.data.AccountCode);
            LabelCurrency.setText(row.data.Cur);
            txtAmount1.setValue(row.data.Pamount);
            txtAmount2.setValue(row.data.Camount);
            txtCOAContent.setValue(row.data.AccountDes);
            cbxCOACenter.setValue(row.data.TSation);
            dfDate.setValue(row.data.Tdate);
            hdDetailID.setValue(row.data.ID);
            hdTempDetailID.setValue(row.id.toString());
        };
        var GetSelectRow = function (v, record, rowIndex) {
            RowSelectionModel1.selectRow(rowIndex, true)
        };
        var rowdbclick = function (a, b, c) {
            window.open("ApplyTransportation.aspx");
        };
    </script>
    <style type="text/css">
        
    </style>
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
                        <li class="q-menuitem"><a href="#">首页</a></li>
                        <li class="q-menuitem"><a href="MyClaims.aspx" id="apply">报销申请</a></li>
                        <li class="q-menuitem"><a href="Approve.aspx">报销审核</a></li>
                        <li class="q-menuitem"><a href="FileManagement.aspx">影像管理</a></li>
                        <li class="q-menuitem"><a href="Budget.aspx">预算管理</a></li>
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
        <div class="W_main W_profile">
            <ext:ResourceManager ID="ResourceManager1" runat="server" DirectMethodNamespace="RM"
                Locale="zh-CN" />
            <ext:Store ID="Store1" runat="server">
                <Reader>
                    <ext:JsonReader>
                        <Fields>
                            <ext:RecordField Name="leaveStart" />
                            <ext:RecordField Name="leaveStart1" />
                            <ext:RecordField Name="leaveEnd" />
                            <ext:RecordField Name="leaveEnd1" />
                            <ext:RecordField Name="leaveCount" />
                            <ext:RecordField Name="Destination" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Store ID="Store2" runat="server">
                <Reader>
                    <ext:ArrayReader>
                        <Fields>
                            <ext:RecordField Name="GLName" />
                            <ext:RecordField Name="GLCode" />
                            <ext:RecordField Name="GLEName" />
                        </Fields>
                    </ext:ArrayReader>
                </Reader>
                <AutoLoadParams>
                    <ext:Parameter Name="start" Value="0" Mode="Raw" />
                    <ext:Parameter Name="limit" Value="10" Mode="Raw" />
                </AutoLoadParams>
            </ext:Store>
            <ext:Store ID="Store3" runat="server" OnSubmitData="SubmitData">
                <Reader>
                    <ext:JsonReader>
                        <Fields>
                            <ext:RecordField Name="ID" />
                            <ext:RecordField Name="Tocity" />
                            <ext:RecordField Name="AccountName" />
                            <ext:RecordField Name="AccountCode" />
                            <ext:RecordField Name="AccountDes" />
                            <ext:RecordField Name="Cur" />
                            <ext:RecordField Name="Pamount" />
                            <ext:RecordField Name="Camount" />
                            <ext:RecordField Name="TSation" />
                            <ext:RecordField Name="Tdate" />
                            <ext:RecordField Name="Type" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Store ID="StoreCity" runat="server">
                <Reader>
                    <ext:JsonReader>
                        <Fields>
                            <ext:RecordField Name="cityID" />
                            <ext:RecordField Name="cityCode" />
                            <ext:RecordField Name="cityName" />
                            <ext:RecordField Name="CountryName" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Hidden ID="hdTravelRequestID" runat="server" />
            <ext:Hidden ID="hdTravelRequestNo" runat="server" />
            <ext:Hidden ID="hdDetailID" runat="server" />
            <ext:Hidden ID="hdReport" runat="server" />
            <ext:Hidden ID="hdScanFile" runat="server" />
            <ext:Hidden ID="hdTempDetailID" runat="server" />
            <ext:Hidden ID="hdSum" runat="server" />
            <ext:Hidden ID="hdPamountSum" runat="server" />
            <ext:Hidden ID="hdCamountSum" runat="server" />
            <ext:Hidden ID="hdCOAType" runat="server" />
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
                                                    <ext:TreeNode Text="费用报销" Expanded="true" Href="#" Icon="UserKey">
                                                        <Nodes>
                                                            <ext:TreeNode Text="差旅费申请" Href="ApplyTravel.aspx" Icon="FlagAe" NodeID="a1" />
                                                            <ext:TreeNode Text="通用费用申请" Href="Apply.aspx" Icon="FlagAf" NodeID="a2" />
                                                        </Nodes>
                                                    </ext:TreeNode>
                                                    <ext:TreeNode Text="我的报销单" Href="MyClaims.aspx" Icon="FlagAn" NodeID="c1" />
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
                            <ext:Panel ID="Panel3" runat="server" Title="新增差旅费申请单" Height="600" Padding="10"
                                MinHeight="300" AutoScroll="true" Layout="AbsoluteLayout">
                                <Items>
                                    <ext:ComboBox ID="cbxOwner" runat="server" FieldLabel="申请人" LabelWidth="60" Width="180"
                                        X="10" Y="10" Editable="False">
                                    </ext:ComboBox>
                                    <ext:Label ID="Label16" runat="server" Text="站点:" X="200" Y="13" />
                                    <ext:Label ID="labelStation" runat="server" X="240" Y="13" />
                                    <ext:Label ID="Label11" runat="server" Text="部门:" X="330" Y="13" />
                                    <ext:Label ID="labelDepartment" runat="server" X="370" Y="13" />
                                    <ext:Label ID="Label1" runat="server" Width="160" X="480" Y="13" Text="My Business Trip @eLeave:" />
                                    <ext:Button ID="btnGeteLeave" runat="server" Text="Load" Width="60" X="645" Y="10"
                                        EnableToggle="true">
                                        <Listeners>
                                            <Toggle Fn="GetStatus" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:DateField ID="dfBdate" runat="server" FieldLabel="出差时间" LabelWidth="60" Width="160"
                                        X="10" Y="40" Format="yyyy/MM/dd" EmptyText="yyyy/MM/dd">
                                    </ext:DateField>
                                    <ext:Label ID="Label2" runat="server" X="175" Y="43" Text="--">
                                    </ext:Label>
                                    <ext:DateField ID="dfEdate" runat="server" Width="100" X="190" Y="40" Format="yyyy/MM/dd"
                                        EmptyText="yyyy/MM/dd">
                                    </ext:DateField>
                                    <ext:Label ID="Label5" runat="server" Width="70" X="300" Y="43" Text="出差报告(*):" />
                                    <ext:FileUploadField ID="FileUploadField1" runat="server" Icon="Attach" X="370" Y="30"
                                        Width="200" EmptyText="上传出差报告" ButtonText="" />
                                    <ext:Button ID="btnUploadReport" runat="server" Text="Upload" X="585" Y="40" Width="60">
                                        <DirectEvents>
                                            <Click OnEvent="UploadTravelReportClick" Before="if (FileUploadField1.getValue()=='') { Ext.Msg.alert('Error','请选择出差报告.'); return false; } 
                                                    Ext.Msg.wait('Uploading your report...', 'Uploading');" Failure="Ext.Msg.show({ 
                                                    title   : 'Error', 
                                                    msg     : 'Error during uploading', 
                                                    minWidth: 200, 
                                                    modal   : true, 
                                                    icon    : Ext.Msg.ERROR, 
                                                    buttons : Ext.Msg.OK 
                                                });">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:HyperLink ID="linkTravelReport" runat="server" Text="" X="650" Y="43" Target="_blank" />
                                    <ext:Label ID="Label17" runat="server" Width="70" X="10" Y="73" Text="扫描文件(*):" />
                                    <ext:FileUploadField ID="FileUploadField2" runat="server" Icon="Attach" X="80" Y="38"
                                        Width="200" EmptyText="上传扫描文件" ButtonText="" />
                                    <ext:Button ID="btnUploadScanFile" runat="server" Text="Upload" X="295" Y="70" Width="60">
                                        <DirectEvents>
                                            <Click OnEvent="UploadScanFileClick" Before="if (FileUploadField2.getValue()=='') { Ext.Msg.alert('Error','请选择扫描文件.'); return false; } 
                                                    Ext.Msg.wait('Uploading your file...', 'Uploading');" Failure="Ext.Msg.show({ 
                                                    title   : 'Error', 
                                                    msg     : 'Error during uploading', 
                                                    minWidth: 200, 
                                                    modal   : true, 
                                                    icon    : Ext.Msg.ERROR, 
                                                    buttons : Ext.Msg.OK 
                                                });">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:HyperLink ID="linkScanFile" runat="server" Text="" X="360" Y="73" Target="_blank" />
                                    <ext:Panel ID="Panel5" runat="server" Height="185" X="10" Y="100" Border="false"
                                        Layout="FitLayout">
                                        <Items>
                                            <ext:ColumnLayout ID="ColumnLayout1" runat="server">
                                                <Columns>
                                                    <ext:LayoutColumn ColumnWidth="1">
                                                        <ext:Panel ID="Panel43" runat="server" Padding="10" Title="在下方编辑费用明细" AutoScroll="true"
                                                            Layout="AbsoluteLayout" StyleSpec="margin-right:3px;">
                                                            <Items>
                                                                <ext:ComboBox ID="cbxCity" runat="server" FieldLabel="出差地点" LabelWidth="70" Width="200"
                                                                    X="10" Y="10" EmptyText="City Code" DisplayField="cityCode" ValueField="cityCode"
                                                                    StoreID="StoreCity">
                                                                    <Triggers>
                                                                        <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                                                                    </Triggers>
                                                                    <Listeners>
                                                                        <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                                                                        <Select Handler="this.triggers[0].show();" />
                                                                        <KeyUp Fn="CheckKey" />
                                                                    </Listeners>
                                                                    <DirectEvents>
                                                                        <KeyUp OnEvent="GetCity" Timeout="300000" Delay="1000">
                                                                        </KeyUp>
                                                                    </DirectEvents>
                                                                </ext:ComboBox>
                                                                <ext:DateField ID="dfDate" runat="server" FieldLabel="日期" Format="yyyy/MM/dd" X="220"
                                                                    Y="10" Width="205" LabelWidth="60" EmptyText="yyyy/MM/dd" />
                                                                <ext:ComboBox ID="cbxCOAType" runat="server" FieldLabel="费用类型" X="10" Y="40" Width="305"
                                                                    LabelWidth="70" StoreID="Store2" ItemSelector="tr.list-item" DisplayField="GLName"
                                                                    ValueField="GLCode" ListWidth="500" MinChars="1" PageSize="10" Mode="Local" EnableKeyEvents="true"
                                                                    ForceSelection="true">
                                                                    <Template ID="Template1" runat="server">
                                                                        <Html>
                                                                            <tpl for=".">
						                                                        <tpl if="[xindex] == 1">
							                                                        <table class="cbStates-list">
								                                                        <tr>
									                                                        <th>费用类型</th>
                                                                                            <th>费用代码</th>
                                                                                            <th>Cost Type</th>
								                                                        </tr>
						                                                        </tpl>
						                                                        <tr class="list-item">
							                                                        <td style="padding:3px 0px;">{GLName}</td>
                                                                                    <td>{GLCode}</td>
                                                                                    <td>{GLEName}</td>
						                                                        </tr>
						                                                        <tpl if="[xcount-xindex]==0">
							                                                        </table>
						                                                        </tpl>
					                                                        </tpl>
                                                                        </Html>
                                                                    </Template>
                                                                    <Triggers>
                                                                        <ext:FieldTrigger Icon="Clear" Qtip="Remove selected" HideTrigger="true" />
                                                                    </Triggers>
                                                                    <Listeners>
                                                                        <BeforeQuery Fn="GetList" />
                                                                        <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();PanelBudget.hide();}" />
                                                                        <Select Fn="SelectType" />
                                                                    </Listeners>
                                                                </ext:ComboBox>
                                                                <ext:TextField ID="txtCOAContent" runat="server" FieldLabel="费用描述" X="325" Y="40"
                                                                    Width="270" LabelWidth="70">
                                                                </ext:TextField>
                                                                <ext:Label ID="Label4" runat="server" X="10" Y="73" Text="币种:" />
                                                                <ext:Label ID="LabelCurrency" runat="server" X="50" Y="73" Text="CNY" />
                                                                <ext:TextField ID="txtAmount1" runat="server" FieldLabel="金额" X="90" Y="70" Width="160"
                                                                    LabelWidth="30" EmptyText="个人支付" />
                                                                <ext:TextField ID="txtAmount2" runat="server" X="255" Y="70" Width="130" EmptyText="公司支付" />
                                                                <ext:ComboBox ID="cbxCOACenter" runat="server" FieldLabel="成本中心" LabelWidth="60"
                                                                    Width="200" X="395" Y="70" EmptyText="Station Code" DisplayField="cityCode" ValueField="cityCode">
                                                                    <Store>
                                                                        <ext:Store ID="StoreCOACenter" runat="server">
                                                                            <Reader>
                                                                                <ext:JsonReader>
                                                                                    <Fields>
                                                                                        <ext:RecordField Name="cityID" />
                                                                                        <ext:RecordField Name="cityCode" />
                                                                                    </Fields>
                                                                                </ext:JsonReader>
                                                                            </Reader>
                                                                        </ext:Store>
                                                                    </Store>
                                                                    <Triggers>
                                                                        <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                                                                    </Triggers>
                                                                    <Listeners>
                                                                        <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                                                                        <Select Handler="this.triggers[0].show();" />
                                                                        <KeyUp Fn="CheckKey" />
                                                                    </Listeners>
                                                                    <DirectEvents>
                                                                        <KeyUp OnEvent="GetStation" Timeout="300000" Delay="1000">
                                                                        </KeyUp>
                                                                    </DirectEvents>
                                                                </ext:ComboBox>
                                                                <ext:Button ID="btnEditDetail" runat="server" Text="编辑" X="10" Y="130" Width="75">
                                                                    <Listeners>
                                                                        <Click Handler="UpdateStore3('Update');" />
                                                                    </Listeners>
                                                                </ext:Button>
                                                                <ext:Button ID="btnNewDetail" runat="server" Text="新增" X="95" Y="130" Width="75">
                                                                    <Listeners>
                                                                        <Click Handler="UpdateStore3('Insert');" />
                                                                    </Listeners>
                                                                </ext:Button>
                                                            </Items>
                                                        </ext:Panel>
                                                    </ext:LayoutColumn>
                                                    <ext:LayoutColumn>
                                                        <ext:Panel ID="Panel12" runat="server" Title="预算" Width="170" Layout="FitLayout">
                                                            <Items>
                                                                <ext:Panel ID="PanelBudget" runat="server" Layout="AbsoluteLayout" Hidden="true"
                                                                    Border="false">
                                                                    <Items>
                                                                        <ext:Label ID="Label7" runat="server" X="10" Y="13" Text="本站个人：5000/10000" Cls="budgetfont" />
                                                                        <ext:Label ID="Label9" runat="server" X="10" Y="43" Text="本站部门：6000/50000" Cls="budgetfont" />
                                                                        <ext:Label ID="Label10" runat="server" X="10" Y="73" Text="本站站点：20000/200000" Cls="budgetfont" />
                                                                        <ext:Label ID="LabelAgent" runat="server" X="10" Y="103" Text="成本中心：20000/200000"
                                                                            Cls="budgetfont" Hidden="true" />
                                                                    </Items>
                                                                </ext:Panel>
                                                            </Items>
                                                        </ext:Panel>
                                                    </ext:LayoutColumn>
                                                </Columns>
                                            </ext:ColumnLayout>
                                        </Items>
                                    </ext:Panel>
                                    <ext:GridPanel ID="GridPanel2" runat="server" StoreID="Store3" Title="单击每行以在上方编辑."
                                        TrackMouseOver="false" Height="225" X="10" Y="288" AutoScroll="true">
                                        <ColumnModel ID="ColumnModel2" runat="server">
                                            <Columns>
                                                <ext:RowNumbererColumn Width="30" />
                                                <ext:CommandColumn Width="50" ButtonAlign="Center">
                                                    <Commands>
                                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit" ToolTip-Text="查看" />
                                                        <ext:GridCommand Icon="Delete" CommandName="Delete" ToolTip-Text="删除" />
                                                    </Commands>
                                                    <PrepareToolbar Fn="prepare" />
                                                </ext:CommandColumn>
                                                <ext:Column Header="出差地点" Width="75" DataIndex="Tocity" />
                                                <ext:DateColumn Header="日期" Width="100" DataIndex="Tdate" Format="yyyy/MM/dd" />
                                                <ext:Column Header="费用类型" Width="100" DataIndex="AccountName" />
                                                <ext:Column Header="币种" Width="80" DataIndex="Cur" />
                                                <ext:Column Header="个人支付" Width="80" DataIndex="Pamount">
                                                    <Renderer Fn="GetNumber" />
                                                </ext:Column>
                                                <ext:Column Header="公司支付" Width="80" DataIndex="Camount">
                                                    <Renderer Fn="GetNumber" />
                                                </ext:Column>
                                                <ext:Column Header="成本中心" Width="60" DataIndex="TSation" />
                                                <ext:Column Header="费用描述" Width="150" DataIndex="AccountDes">
                                                </ext:Column>
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                                <Listeners>
                                                    <RowSelect Handler="if(record!=undefined){GetDetail(record);}else{return false;}" />
                                                </Listeners>
                                            </ext:RowSelectionModel>
                                        </SelectionModel>
                                        <Listeners>
                                            <Command Fn="RowCommand" />
                                        </Listeners>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolbar2" runat="server" PageSize="20" DisplayInfo="true"
                                                DisplayMsg="Displaying items {0} - {1} of {2}" EmptyMsg="No items to display"
                                                HideRefresh="true">
                                                <Items>
                                                    <ext:Label ID="Label6" runat="server" Text="Page size:" />
                                                    <ext:ToolbarSpacer ID="ToolbarSpacer2" runat="server" Width="10" />
                                                    <ext:ComboBox ID="ComboBox2" runat="server" Width="80">
                                                        <Items>
                                                            <ext:ListItem Text="10" />
                                                            <ext:ListItem Text="20" />
                                                            <ext:ListItem Text="50" />
                                                            <ext:ListItem Text="100" />
                                                            <ext:ListItem Text="200" />
                                                        </Items>
                                                        <SelectedItem Value="20" />
                                                        <Listeners>
                                                            <Select Handler="#{PagingToolbar2}.pageSize = parseInt(this.getValue()); #{PagingToolbar2}.doLoad();" />
                                                        </Listeners>
                                                    </ext:ComboBox>
                                                </Items>
                                            </ext:PagingToolbar>
                                        </BottomBar>
                                    </ext:GridPanel>
                                    <ext:TextField ID="txtSum" runat="server" X="10" Y="520" FieldLabel="合计" LabelWidth="60"
                                        Width="180" Disabled="true" DisabledClass="sum" />
                                    <ext:TextField ID="txtPersonalSum" runat="server" X="200" Y="520" FieldLabel="个人支付合计"
                                        LabelWidth="80" Width="180" Disabled="true" DisabledClass="sum" />
                                    <ext:TextField ID="txtCompanySum" runat="server" X="390" Y="520" FieldLabel="公司支付合计"
                                        LabelWidth="80" Width="180" Disabled="true" DisabledClass="sum" />
                                    <ext:TextField ID="txtRemark" runat="server" FieldLabel="备注" LabelWidth="60" X="10"
                                        Y="550" Anchor="100%">
                                    </ext:TextField>
                                    <ext:Button ID="btnSaveAndSend" runat="server" Text="保存并申请" X="10" Y="580" Width="80"
                                        Disabled="true">
                                        <Listeners>
                                            <Click Fn="CheckStore" />
                                        </Listeners>
                                        <DirectEvents>
                                            <Click OnEvent="Save" Success="#{GridPanel2}.submitData();" Timeout="300000">
                                                <ExtraParams>
                                                    <ext:Parameter Name="type" Value="ND" Mode="Value">
                                                    </ext:Parameter>
                                                </ExtraParams>
                                                <EventMask ShowMask="true" CustomTarget="Panel3" Target="CustomTarget" />
                                                <Confirmation ConfirmRequest="true" Title="提示" Message="保存后无法修改,是否保存?" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnSaveDraft" runat="server" Text="存为草稿" X="100" Y="580" Width="80">
                                        <DirectEvents>
                                            <Click OnEvent="Save" Success="#{GridPanel2}.submitData();" Timeout="300000">
                                                <ExtraParams>
                                                    <ext:Parameter Name="type" Value="D" Mode="Value">
                                                    </ext:Parameter>
                                                </ExtraParams>
                                                <EventMask ShowMask="true" CustomTarget="Panel3" Target="CustomTarget" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnExport" runat="server" Text="Export" X="190" Y="580" Width="80"
                                        OnClick="btnExport_Click" AutoPostBack="true" Icon="Report" Disabled="true">
                                    </ext:Button>
                                    <ext:Label ID="labelInfo" runat="server" Text="" X="20" Y="610">
                                    </ext:Label>
                                </Items>
                            </ext:Panel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
            <ext:Window ID="Window1" runat="server" Title="My Business Trip @ eLeave" Hidden="true"
                Layout="FitLayout" Width="850" Height="200" Resizable="False" AutoScroll="true"
                Closable="False">
                <Listeners>
                    <Show Handler=" var pos = btnGeteLeave.getPosition();
                        pos[0] += -683;
                        pos[1] += 29;
                        this.setPosition(pos);" />
                </Listeners>
                <Items>
                    <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                        Border="false" TrackMouseOver="false">
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:RowNumbererColumn Width="40" />
                                <ext:CommandColumn Width="70" ButtonAlign="Center">
                                    <Commands>
                                        <ext:GridCommand Icon="LinkGo" CommandName="Edit" Text="Select" />
                                    </Commands>
                                </ext:CommandColumn>
                                <ext:Column Header="Leave Start" Width="150" DataIndex="leaveStart" />
                                <ext:Column Header="Leave End" Width="150" DataIndex="leaveEnd" />
                                <ext:Column Header="Leave Count" Width="100" DataIndex="leaveCount" />
                                <ext:Column Header="DSTN" Width="80" DataIndex="Destination" />
                            </Columns>
                        </ColumnModel>
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
                        <Listeners>
                            <Command Fn="PasseLeaveData" />
                        </Listeners>
                    </ext:GridPanel>
                </Items>
            </ext:Window>
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
        </div>
    </div>
    </form>
</body>
</html>
