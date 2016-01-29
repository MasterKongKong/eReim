<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplyTravel.aspx.cs" Inherits="eReimbursement.ApplyTravel" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>差旅费申请单</title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
    <link href="Styles/StyleSheet1.css" rel="stylesheet" type="text/css" />
    <link href="Styles/StyleSheet2.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/PageJS.js" type="text/javascript"></script>
    <script type="text/javascript">
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
        var LOADCOATip = function () {
            var record = cbxCOA.findRecord(cbxCOA.displayField, this.triggerElement.innerHTML);
            this.body.update('COACode: ' + record.get(cbxCOA.valueField));
        };

        var LOADCOATip1 = function () {
            var record = cbxCOA.findRecord(cbxCOA.displayField, cbxCOA.getRawValue());
            if (record != undefined) {
                this.body.update('COACode: ' + record.get(cbxCOA.valueField));
            }
            else {
                this.hide();
            }
        };
        var Preview = function () {
            if (hdTravelRequestID.getValue() != '') {
                window.open("Preview.aspx?RequestID=" + hdTravelRequestID.getValue() + "&Type=T");
            }
        };
        var SaveAll = function (a) {
            if (a == 'ND') {
                //正式申请
                if (Store3.getAllRange().length < 1) {
                    Ext.Msg.show({ title: 'Message', msg: 'No expense to apply.', buttons: { ok: 'Ok'} });
                    return false;
                }
                else {
                    Ext.Msg.confirm('Message', 'Are you sure?', function (btn, text) {
                        if (btn == 'yes') {
                            var detail = ""; //记录Store数据为json格式
                            for (var i = 0; i < Store3.getAllRange().length; i++) {
                                var record = Store3.getAllRange()[i].data;
                                detail += Ext.encode(record);
                                if (i != Store3.getAllRange().length - 1) {
                                    detail += ',';
                                }
                                Store3.getAllRange(i, i)[0].set('Type', '1'); //用来更新删除按钮状态
                            };
                            detail = '[' + detail + ']';
                            var CCMailListdetail = ""; //记录Store数据为json格式
                            for (var i = 0; i < StoreCCList.getAllRange().length; i++) {
                                var record1 = StoreCCList.getAllRange()[i].data;
                                CCMailListdetail += Ext.encode(record1);
                                if (i != StoreCCList.getAllRange().length - 1) {
                                    CCMailListdetail += ',';
                                }
                            };
                            CCMailListdetail = '[' + CCMailListdetail + ']';
                            RM.SaveAll(a, detail, CCMailListdetail, {
                                eventMask: {
                                    showMask: true,
                                    tartget: "customtarget",
                                    customTarget: Panel3
                                }
                            });
                        }
                    });
                }
                Store3.load();
            }
            else {
                //草稿  
                var detail = ""; //记录Store数据为json格式  
                for (var i = 0; i < Store3.getAllRange().length; i++) {
                    var record = Store3.getAllRange()[i].data;
                    detail += Ext.encode(record);
                    if (i != Store3.getAllRange().length - 1) {
                        detail += ',';
                    }
                };
                detail = '[' + detail + ']';
                var CCMailListdetail = ""; //记录Store数据为json格式
                for (var i = 0; i < StoreCCList.getAllRange().length; i++) {
                    var record1 = StoreCCList.getAllRange()[i].data;
                    CCMailListdetail += Ext.encode(record1);
                    if (i != StoreCCList.getAllRange().length - 1) {
                        CCMailListdetail += ',';
                    }
                };
                CCMailListdetail = '[' + CCMailListdetail + ']';
                RM.SaveAll(a, detail, CCMailListdetail, {
                    eventMask: {
                        showMask: true,
                        tartget: "customtarget",
                        customTarget: Panel3
                    }
                });
            }

        };
        var saveData = function () {
            GridData.setValue(Ext.encode(GridPanel1.getRowsValues({ filterRecord: GridPanel1.filters.getRecordFilter() })));
        };
        var CheckStore = function () {
            if (Store3.getAllRange().length < 1) {
                Ext.Msg.show({ title: 'Message', msg: 'No expense to apply.', buttons: { ok: 'Ok'} });
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
            if (record.data.Type != '0') {
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
            if (cbxCity.getRawValue() == '') {
                if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
                    Ext.Msg.show({ title: '提示', msg: '请填写出差地点.', buttons: { ok: 'Ok'} });
                }
                else {
                    Ext.Msg.show({ title: 'Message', msg: 'Please input Location.', buttons: { ok: 'Ok'} });
                }

                return false;
            }
            if (dfDate.getValue().toString() == '') {
                if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
                    Ext.Msg.show({ title: '提示', msg: '请选择日期.', buttons: { ok: 'Ok'} });
                }
                else {
                    Ext.Msg.show({ title: 'Message', msg: 'Please select Date.', buttons: { ok: 'Ok'} });
                }
                return false;
            }
            if (cbxCOA.getRawValue() == '') {
                if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
                    Ext.Msg.show({ title: '提示', msg: '请选择费用类型.', buttons: { ok: 'Ok'} });
                }
                else {
                    Ext.Msg.show({ title: 'Message', msg: 'Please select Expense Type.', buttons: { ok: 'Ok'} });
                }

                return false;
            }
            if (cbxDepartment.getRawValue() == '') {
                if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
                    Ext.Msg.show({ title: '提示', msg: '请选择预算部门.', buttons: { ok: 'Ok'} });
                }
                else {
                    Ext.Msg.show({ title: 'Message', msg: 'Please select Department.', buttons: { ok: 'Ok'} });
                }

                return false;
            }
            LabelPersonBG.setText(''); LabelStationBG.setText(''); LabelDepartmentBG.setText(''); LabelCOA.setText(''); LabelPersonBGOneYear.setText('');
            if (cbxCOA.getValue() != '' && dfDate.value != '') {
                RM.LoadBG(a,'Update',{
                    eventMask: {
                        showMask: true,
                        tartget: "Page"
                    },
                    timeout: 300000
                });
            }
        };
        var UpdateList = function (a) {
            if (a == 'Update') {
                if (hdDetailID.getValue() != undefined && hdDetailID.getValue() != '') {
                    var id = hdTempDetailID.getValue();
                    Store3.getById(id).set('Tocity', cbxCity.getValue());
                    //                Store3.getById(id).set('AccountName', cbxCOAType.getRawValue());
                    //                Store3.getById(id).set('AccountCode', hdCOAType.getValue());
                    Store3.getById(id).set('AccountDes', txtCOAContent.getValue());
                    Store3.getById(id).set('Cur', LabelCurrency.getText());
                    //                Store3.getById(id).set('Pamount', txtAmount1.getValue());
                    //                Store3.getById(id).set('Camount', txtAmount2.getValue());
                    Store3.getById(id).set('Pamount', nfAmount1.getValue().toString());
                    Store3.getById(id).set('Camount', nfAmount2.getValue().toString());
                    Store3.getById(id).set('TSation', cbxCOACenter.getValue());
                    Store3.getById(id).set('Tdate', dfDate.getRawValue());
                    Store3.getById(id).set('COAName', cbxCOA.getRawValue());
                    Store3.getById(id).set('AccountCode', cbxCOA.getValue());
                    Store3.getById(id).set('Department1', cbxDepartment.getValue());
                    GetSum();
                }
                else {
                    Ext.Msg.show({ title: 'Message', msg: 'Please select one row to edit.', buttons: Ext.Msg.OK, icon: Ext.Msg.WARNING });
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
                //            newrecord.data.AccountName = cbxCOAType.getRawValue();
                //            newrecord.data.AccountCode = hdCOAType.getValue();
                newrecord.data.AccountDes = txtCOAContent.getValue();
                newrecord.data.Cur = LabelCurrency.getText();
                //            newrecord.data.Pamount = txtAmount1.getValue();
                //            newrecord.data.Camount = txtAmount2.getValue();
                newrecord.data.Pamount = nfAmount1.getValue().toString();
                newrecord.data.Camount = nfAmount2.getValue().toString();
                newrecord.data.TSation = cbxCOACenter.getValue();
                newrecord.data.Tdate = dfDate.getRawValue();
                newrecord.data.COAName = cbxCOA.getRawValue();
                newrecord.data.AccountCode = cbxCOA.getValue();
                newrecord.data.Department1 = cbxDepartment.getValue();
                newrecord.data.Type = "0";
                Store3.add(newrecord);
                GetSum();
            }
        };
        var SelectType = function (combox, data, selectindex) {
            this.triggers[0].show();
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

        var GetDetail = function (row) {
            var reg = new RegExp("<br />");
            cbxCity.setValue(row.data.Tocity);
            //        cbxCOAType.setValue(row.data.AccountName);
            //        hdCOAType.setValue(row.data.AccountCode);
            //        if (cbxCOAType.getRawValue() != '') {
            //            cbxCOAType.triggers[0].show();
            //        }
            cbxCOA.setValueAndFireSelect(row.data.AccountCode);
            LabelCurrency.setText(row.data.Cur);
            //        txtAmount1.setValue(row.data.Pamount);
            //        txtAmount2.setValue(row.data.Camount);
            nfAmount1.setValue(row.data.Pamount);
            nfAmount2.setValue(row.data.Camount);
            txtCOAContent.setValue(row.data.AccountDes);
            cbxCOACenter.setValue(row.data.TSation);
            dfDate.setValue(row.data.Tdate);
            hdDetailID.setValue(row.data.ID);
            hdTempDetailID.setValue(row.id.toString());
            cbxDepartment.setValue(row.data.Department1);
            //            cbxOwner.getValue(), labelDepartment.getText(), labelStation.getText(), cbxCOACenter.getRawValue(), hdCOAType.getValue(), dfDate.getValue(), 
            LabelPersonBG.setText(''); LabelStationBG.setText(''); LabelDepartmentBG.setText(''); LabelCOA.setText(''); LabelPersonBGOneYear.setText('');
            if (cbxCOA.getValue() != '' && dfDate.value != '') {
                RM.LoadBG('','GetDetail',{
                    eventMask: {
                        showMask: true,
                        tartget: "customtarget",
                        customTarget: PanelBudget
                    },
                    timeout: 300000
                });
            }
            if (row.data.AccountCode != '') {
                cbxCOA.triggers[0].show();
            }
            else {
                cbxCOA.triggers[0].hide();
            }
            if (row.data.Tocity != '') {
                cbxCity.triggers[0].show();
            }
            else {
                cbxCity.triggers[0].hide();
            }
            if (row.data.TSation != '') {
                cbxCOACenter.triggers[0].show();
            }
            else {
                cbxCOACenter.triggers[0].hide();
            }
        };
        var CheckBG = function () {
            if (dfDate.getValue().toString() == '') {
                if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
                    Ext.Msg.show({ title: '提示', msg: '请选择日期.', buttons: { ok: 'Ok'} });
                }
                else {
                    Ext.Msg.show({ title: 'Message', msg: 'Please select Date.', buttons: { ok: 'Ok'} });
                }
                return false;
            }
            if (cbxCOA.getRawValue() == '') {
                if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
                    Ext.Msg.show({ title: '提示', msg: '请选择费用类型.', buttons: { ok: 'Ok'} });
                }
                else {
                    Ext.Msg.show({ title: 'Message', msg: 'Please select Expense Type.', buttons: { ok: 'Ok'} });
                }

                return false;
            }
            if (cbxDepartment.getRawValue() == '') {
                if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
                    Ext.Msg.show({ title: '提示', msg: '请选择所属预算部门.', buttons: { ok: 'Ok'} });
                }
                else {
                    Ext.Msg.show({ title: 'Message', msg: 'Please select Budget department.', buttons: { ok: 'Ok'} });
                }

                return false;
            }
            LabelPersonBG.setText(''); LabelStationBG.setText(''); LabelDepartmentBG.setText(''); LabelCOA.setText(''); LabelPersonBGOneYear.setText('');
            return true;
        };
        var LoadBG = function () {
            if (CheckBG()) {
                RM.LoadBG('','GetDetail',{
                    eventMask: {
                        showMask: true,
                        tartget: "customtarget",
                        customTarget: PanelBudget
                    },
                    timeout: 300000
                });
            }
        };
        var GetSelectRow = function (v, record, rowIndex) {
            RowSelectionModel1.selectRow(rowIndex, true)
        };
        $(document).ready(function () {
            //中英双语设置
            if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {

                document.title = '差旅费申请单';
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
                document.title = 'Travel Expense Apply';
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
            TreePanel1.getNodeById('a1').select(true);
        };
        var AddMail = function () {
            var temps = StoreCCList.query('Email', cbxCCName.getValue()).items;
            if (temps.length > 0 || cbxCCName.getValue() == '') {
                return false;
            }
            var newrecord = new StoreCCList.recordType();
            newrecord.data.Email = cbxCCName.getValue();
            StoreCCList.add(newrecord);
        };
        var MailRowCommand = function (command, record, rowIndex) {
            if (command == 'Delete') {
                StoreCCList.removeAt(rowIndex);
                StoreCCList.sort();
            }
        };
        var GetCCList = function () {
            ToolTipCC.show(); ToolTipCC.hide(); WindowCCList.show();
        };
        var SetCCListToolTip = function () {
            WindowCCList.hide();
            var newmaillist = '';
            for (var i = 0; i < StoreCCList.getAllRange().length; i++) {
                var record = StoreCCList.getAllRange()[i].data;
                newmaillist += record.Email + '<br />';
            };
            //            Ext.each(StoreCCList.getAllRange(), function (record) {
            //                newmaillist += record.data.Email + '<br />';
            //            });
            newmaillist = newmaillist.length > 0 ? newmaillist.substring(0, newmaillist.length - 6) : '';
            $("#ToolTipCC .x-tip-body").html(newmaillist);
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
            <ext:Store ID="Store3" runat="server">
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
                            <ext:RecordField Name="COAName" />
                            <ext:RecordField Name="Department1" />
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
            <ext:Hidden ID="hdUser" runat="server" />
            <ext:Hidden ID="hdStatus" runat="server" />
            <ext:Hidden ID="hdBudget" runat="server" />
            <ext:Panel ID="Panel1" runat="server" Height="680" Border="false">
                <Items>
                    <ext:BorderLayout ID="BorderLayout1" runat="server">
                        <West Collapsible="true" Split="true" CollapseMode="Mini">
                            <ext:Panel ID="Panel2" runat="server" Width="195" Layout="FitLayout">
                                <Items>
                                    <ext:TreePanel ID="TreePanel1" runat="server" Header="false" Lines="false" UseArrows="true"
                                        RootVisible="false" Border="false" AutoScroll="true">
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
                            <ext:Panel ID="Panel3" runat="server" Title="<%$ Resources:LocalText,TravellingExpenseApply%>"
                                Height="600" Padding="10" MinHeight="300" AutoScroll="true" Layout="AbsoluteLayout">
                                <Items>
                                    <ext:ComboBox ID="cbxOwner" runat="server" FieldLabel="<%$ Resources:LocalText,Owner%>"
                                        LabelWidth="60" Width="180" X="10" Y="10" Editable="False">
                                        <DirectEvents>
                                            <Select OnEvent="ChangePerson" Timeout="300000">
                                                <EventMask ShowMask="true" Target="Page" />
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:Label ID="Label16" runat="server" Text="<%$ Resources:LocalText,StationLabel%>"
                                        X="200" Y="13" />
                                    <ext:Label ID="labelStation" runat="server" X="250" Y="13" />
                                    <ext:Label ID="Label11" runat="server" Text="<%$ Resources:LocalText,Department%>"
                                        X="330" Y="13" />
                                    <ext:Label ID="labelDepartment" runat="server" X="405" Y="13" />
                                    <ext:Label ID="Label1" runat="server" Width="160" X="540" Y="13" Text="My Business Trip @eLeave:" />
                                    <ext:Button ID="btnGeteLeave" runat="server" Text="Load" Width="60" X="705" Y="10"
                                        EnableToggle="true">
                                        <Listeners>
                                            <Toggle Fn="GetStatus" />
                                        </Listeners>
                                    </ext:Button>
                                    
                                    <ext:DateField ID="dfBdate" runat="server" FieldLabel="<%$ Resources:LocalText,Period%>"
                                        LabelWidth="60" Width="160" X="10" Y="40" Format="yyyy/MM/dd" EmptyText="yyyy/MM/dd">
                                    </ext:DateField>
                                    <ext:Label ID="Label2" runat="server" X="175" Y="43" Text="--">
                                    </ext:Label>
                                    <ext:DateField ID="dfEdate" runat="server" Width="100" X="190" Y="40" Format="yyyy/MM/dd"
                                        EmptyText="yyyy/MM/dd">
                                    </ext:DateField>
                                    <ext:Label ID="Label5" runat="server" Width="100" X="300" Y="43" Text="<%$ Resources:LocalText,TravelReport%>" />
                                    <ext:FileUploadField ID="FileUploadField1" runat="server" Icon="Attach" X="390" Y="30"
                                        Width="130" EmptyText="Select file." ButtonText="" />
                                    <ext:Button ID="btnUploadReport" runat="server" X="533" Y="40" Icon="Add">
                                        <DirectEvents>
                                            <Click OnEvent="UploadTravelReportClick" Before="if (FileUploadField1.getValue()=='') { Ext.Msg.alert('Error','Please select file.'); return false; } 
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
                                    <ext:Button ID="Button1" runat="server" X="558" Y="40" Icon="Delete">
                                        <Listeners>
                                            <Click Handler="linkTravelReport.setText('');hdReport.setValue('');" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:HyperLink ID="linkTravelReport" runat="server" Text="" X="588" Y="43" Target="_blank" />

                                    <ext:ComboBox ID="cbxBudget" runat="server" FieldLabel="<%$ Resources:LocalText,BudgetOrNot%>"
                                        LabelWidth="60" Width="120" X="10" Y="70" Editable="False" SelectedIndex="0" ReadOnly="true">
                                        <Items>
                                            <ext:ListItem Text="YES" />
                                            <ext:ListItem Text="NO" />
                                        </Items>
                                        <DirectEvents>
                                            <Select OnEvent="ChangeBudget" Timeout="300000">
                                                <EventMask ShowMask="true" Target="Page" />
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>

                                    <ext:Label ID="Label17" runat="server" Width="70" X="300" Y="73" Text="<%$ Resources:LocalText,ScanFile%>" />
                                    <ext:FileUploadField ID="FileUploadField2" runat="server" Icon="Attach" X="390" Y="38"
                                        Width="130" EmptyText="Select file." ButtonText="" />
                                    <ext:Button ID="btnUploadScanFile" runat="server" X="533" Y="70" Icon="Add">
                                        <DirectEvents>
                                            <Click OnEvent="UploadScanFileClick" Before="if (FileUploadField2.getValue()=='') { Ext.Msg.alert('Error','Please select file.'); return false; } 
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
                                    <ext:Button ID="Button2" runat="server" X="558" Y="70" Icon="Delete">
                                        <Listeners>
                                            <Click Handler="linkScanFile.setText('');hdScanFile.setValue('');" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:HyperLink ID="linkScanFile" runat="server" Text="" X="588" Y="73" Target="_blank" />
                                    <ext:Panel ID="Panel5" runat="server" Height="185" X="10" Y="100" Border="false"
                                        Layout="FitLayout">
                                        <Items>
                                            <ext:ColumnLayout ID="ColumnLayout1" runat="server">
                                                <Columns>
                                                    <ext:LayoutColumn ColumnWidth="1">
                                                        <ext:Panel ID="Panel43" runat="server" Padding="10" Title="<%$ Resources:LocalText,ExpenseDetail%>"
                                                            AutoScroll="true" Layout="AbsoluteLayout" StyleSpec="margin-right:3px;">
                                                            <Items>
                                                                <ext:ComboBox ID="cbxCity" runat="server" FieldLabel="<%$ Resources:LocalText,Location%>"
                                                                    LabelWidth="70" Width="200" X="10" Y="10" EmptyText="City Code" DisplayField="cityCode"
                                                                    ValueField="cityCode" StoreID="StoreCity">
                                                                    <Triggers>
                                                                        <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                                                                    </Triggers>
                                                                    <Listeners>
                                                                        <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                                                                        <Select Handler="this.triggers[0].show();" />
                                                                        <KeyUp Fn="CheckKey" />
                                                                        <Render Handler="this.triggers[1].hide();" />
                                                                    </Listeners>
                                                                    <DirectEvents>
                                                                        <KeyUp OnEvent="GetCity" Timeout="300000" Delay="1000">
                                                                        </KeyUp>
                                                                    </DirectEvents>
                                                                </ext:ComboBox>
                                                                <ext:DateField ID="dfDate" runat="server" FieldLabel="<%$ Resources:LocalText,Date%>"
                                                                    Format="yyyy/MM/dd" X="220" Y="10" Width="205" LabelWidth="60" EmptyText="yyyy/MM/dd" />
                                                                <ext:ComboBox ID="cbxCOA" runat="server" FieldLabel="<%$ Resources:LocalText,COAType%>"
                                                                    X="10" Y="40" Width="305" LabelWidth="70" EmptyText="COA Type" DisplayField="COAName"
                                                                    ValueField="COACode">
                                                                    <Store>
                                                                        <ext:Store ID="StoreCOA" runat="server">
                                                                            <Reader>
                                                                                <ext:JsonReader>
                                                                                    <Fields>
                                                                                        <ext:RecordField Name="COACode" />
                                                                                        <ext:RecordField Name="COAName" />
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
                                                                    </Listeners>
                                                                </ext:ComboBox>
                                                                
                                                                <ext:Label ID="Label4" runat="server" X="10" Y="73" Text="<%$ Resources:LocalText,Currency%>" />
                                                                <ext:Label ID="LabelCurrency" runat="server" X="70" Y="73" />
                                                                <%--<ext:TextField ID="txtAmount1" runat="server" FieldLabel="<%$ Resources:LocalText,Amount%>"
                                                                    X="110" Y="70" Width="160" LabelWidth="60" EmptyText="<%$ Resources:LocalText,PersonalExpense%>" Hidden="true"/>--%>
                                                                <ext:NumberField ID="nfAmount1" runat="server" FieldLabel="<%$ Resources:LocalText,Amount%>"
                                                                    X="110" Y="70" Width="130" LabelWidth="50" EmptyText="<%$ Resources:LocalText,PersonalExpense%>" />
                                                                <%-- <ext:TextField ID="txtAmount2" runat="server" X="275" Y="70" Width="100" EmptyText="<%$ Resources:LocalText,CompanyExpense%>" Hidden="true"/>--%>
                                                                <ext:NumberField ID="nfAmount2" runat="server" X="245" Y="70" Width="80" EmptyText="<%$ Resources:LocalText,CompanyExpense%>" />
                                                                <ext:ComboBox ID="cbxCOACenter" runat="server" FieldLabel="<%$ Resources:LocalText,CostCenter%>"
                                                                    LabelWidth="80" Width="200" X="335" Y="70" EmptyText="Station Code" DisplayField="cityCode"
                                                                    ValueField="cityCode">
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
                                                                        <Render Handler="this.triggers[1].hide();" />
                                                                    </Listeners>
                                                                    <DirectEvents>
                                                                        <KeyUp OnEvent="GetStation" Timeout="300000" Delay="1000">
                                                                        </KeyUp>
                                                                    </DirectEvents>
                                                                </ext:ComboBox>
                                                                <ext:TextField ID="txtCOAContent" runat="server" FieldLabel="<%$ Resources:LocalText,COARemark%>"
                                                                    X="10" Y="100" Width="510" LabelWidth="70">
                                                                </ext:TextField>
                                                                <ext:Button ID="btnEditDetail" runat="server" Text="<%$ Resources:LocalText,Edit%>"
                                                                    X="10" Y="130" Width="75" Icon="Accept">
                                                                    <Listeners>
                                                                        <Click Handler="UpdateStore3('Update');" />
                                                                    </Listeners>
                                                                </ext:Button>
                                                                <ext:Button ID="btnNewDetail" runat="server" Text="<%$ Resources:LocalText,Add%>"
                                                                    X="95" Y="130" Width="75" Icon="Add">
                                                                    <Listeners>
                                                                        <Click Handler="UpdateStore3('Insert');" />
                                                                    </Listeners>
                                                                </ext:Button>
                                                                <ext:Button ID="btnLoadBudget" runat="server" Text="<%$ Resources:LocalText,BudgetReview%>"
                                                                    X="180" Y="130" Width="75">
                                                                    <Listeners>
                                                                        <Click Fn="LoadBG" />
                                                                    </Listeners>
                                                                </ext:Button>
                                                            </Items>
                                                        </ext:Panel>
                                                    </ext:LayoutColumn>
                                                    <ext:LayoutColumn>
                                                        <ext:Panel ID="Panel12" runat="server" Title="<%$ Resources:LocalText,Budget%>" Width="250"
                                                            Layout="FitLayout">
                                                            <Items>
                                                                <ext:Panel ID="PanelBudget" runat="server" Layout="AbsoluteLayout" Border="false"
                                                                    AutoScroll="True">
                                                                    <Items>
                                                                        <ext:ComboBox ID="cbxDepartment" runat="server" FieldLabel="<%$ Resources:LocalText,Department1%>"
                                                                            X="5" Y="5" Width="210" LabelWidth="70" DisplayField="Depart" ValueField="Depart"
                                                                            Editable="False">
                                                                            <Store>
                                                                                <ext:Store ID="StoreDepartment" runat="server">
                                                                                    <Reader>
                                                                                        <ext:JsonReader>
                                                                                            <Fields>
                                                                                                <ext:RecordField Name="Depart" />
                                                                                            </Fields>
                                                                                        </ext:JsonReader>
                                                                                    </Reader>
                                                                                </ext:Store>
                                                                            </Store>
                                                                        </ext:ComboBox>
                                                                        <ext:Label ID="LabelCOA" runat="server" X="5" Y="29" Cls="budgetfont" />
                                                <ext:Label ID="LabelStationBG" runat="server" X="5" Y="64" Cls="budgetfont" />
                                                <ext:Label ID="LabelDepartmentBG" runat="server" X="5" Y="84" Cls="budgetfont" />
                                                <ext:Label ID="LabelPersonBG" runat="server" X="5" Y="104" Cls="budgetfont" />
                                                <ext:Label ID="LabelPersonBGOneYear" runat="server" X="5" Y="124" Cls="budgetfont" />
                                                                    </Items>
                                                                </ext:Panel>
                                                            </Items>
                                                        </ext:Panel>
                                                    </ext:LayoutColumn>
                                                </Columns>
                                            </ext:ColumnLayout>
                                        </Items>
                                    </ext:Panel>
                                    <ext:GridPanel ID="GridPanel2" runat="server" StoreID="Store3" Title="<%$ Resources:LocalText,ExpenseList%>"
                                        TrackMouseOver="false" Height="225" X="10" Y="288" AutoScroll="true">
                                        <ColumnModel ID="ColumnModel2" runat="server">
                                            <Columns>
                                                <ext:RowNumbererColumn Width="30" />
                                                <ext:CommandColumn Width="50" ButtonAlign="Center">
                                                    <Commands>
                                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit" ToolTip-Text="<%$ Resources:LocalText,View%>" />
                                                        <ext:GridCommand Icon="Delete" CommandName="Delete" ToolTip-Text="<%$ Resources:LocalText,Delete%>" />
                                                    </Commands>
                                                    <PrepareToolbar Fn="prepare" />
                                                </ext:CommandColumn>
                                                <ext:Column Header="<%$ Resources:LocalText,Location%>" Width="75" DataIndex="Tocity" />
                                                <ext:DateColumn Header="<%$ Resources:LocalText,Date%>" Width="85" DataIndex="Tdate"
                                                    Format="yyyy/MM/dd" />
                                                <ext:Column Header="<%$ Resources:LocalText,ExpenseType%>" Width="200" DataIndex="COAName" />
                                                <ext:Column Header="COACode" Width="90" DataIndex="AccountCode" />
                                                <ext:Column Header="<%$ Resources:LocalText,CurrencyColumn%>" Width="80" DataIndex="Cur" />
                                                <ext:Column Header="<%$ Resources:LocalText,PersonalExpense%>" Width="80" DataIndex="Pamount">
                                                    <Renderer Fn="GetNumber" />
                                                </ext:Column>
                                                <ext:Column Header="<%$ Resources:LocalText,CompanyExpense%>" Width="80" DataIndex="Camount">
                                                    <Renderer Fn="GetNumber" />
                                                </ext:Column>
                                                <ext:Column Header="<%$ Resources:LocalText,CostCenter%>" Width="160" DataIndex="TSation" />
                                                <ext:Column Header="<%$ Resources:LocalText,Department1%>" Width="160" DataIndex="Department1" />
                                                <ext:Column Header="<%$ Resources:LocalText,COARemark%>" Width="100" DataIndex="AccountDes">
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
                                    </ext:GridPanel>
                                    <ext:TextField ID="txtSum" runat="server" X="10" Y="520" FieldLabel="<%$ Resources:LocalText,AllTotal%>"
                                        LabelWidth="60" Width="180" ReadOnly="true" />
                                    <ext:TextField ID="txtPersonalSum" runat="server" X="200" Y="520" FieldLabel="<%$ Resources:LocalText,PersonalExpenseTotal%>"
                                        LabelWidth="80" Width="180" ReadOnly="true" />
                                    <ext:TextField ID="txtCompanySum" runat="server" X="390" Y="520" FieldLabel="<%$ Resources:LocalText,CompanyExpenseTotal%>"
                                        LabelWidth="100" Width="180" ReadOnly="true" />
                                    <ext:TextField ID="txtRemark" runat="server" FieldLabel="<%$ Resources:LocalText,Remark%>"
                                        LabelWidth="60" X="10" Y="550" Anchor="100%">
                                    </ext:TextField>
                                    <ext:Button ID="btnSaveAndSend" runat="server" Text="<%$ Resources:LocalText,SaveApply%>"
                                        X="10" Y="580" Width="80" Disabled="true">
                                        <Listeners>
                                            <Click Handler="SaveAll('ND');" />
                                        </Listeners>
                                        <%--<DirectEvents>
                                            <Click OnEvent="Save" Success="#{GridPanel2}.submitData();" Timeout="300000">
                                                <ExtraParams>
                                                    <ext:Parameter Name="type" Value="ND" Mode="Value">
                                                    </ext:Parameter>
                                                </ExtraParams>
                                                <EventMask ShowMask="true" CustomTarget="Panel3" Target="CustomTarget" />
                                                <Confirmation ConfirmRequest="true" Title="提示" Message="保存后无法修改,是否保存?" />
                                            </Click>
                                        </DirectEvents>--%>
                                    </ext:Button>
                                    <ext:Button ID="btnSaveDraft" runat="server" Text="<%$ Resources:LocalText,SaveAsDraft%>"
                                        X="100" Y="580" Width="80">
                                        <Listeners>
                                            <Click Handler="SaveAll('D');" />
                                        </Listeners>
                                        <%--<DirectEvents>
                                            <Click OnEvent="Save" Success="#{GridPanel2}.submitData();" Timeout="300000">
                                                <ExtraParams>
                                                    <ext:Parameter Name="type" Value="D" Mode="Value">
                                                    </ext:Parameter>
                                                </ExtraParams>
                                                <EventMask ShowMask="true" CustomTarget="Panel3" Target="CustomTarget" />
                                            </Click>
                                        </DirectEvents>--%>
                                    </ext:Button>
                                    <ext:Button ID="btnExport" runat="server" Text="<%$ Resources:LocalText,Export%>"
                                        X="190" Y="580" Width="80" Icon="Report" Disabled="true">
                                        <Listeners>
                                            <Click Fn="Preview" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:Button ID="btnCC" runat="server" Text="<%$ Resources:LocalText,CC%>" X="280"
                                        Y="580" Width="80" Icon="Mail">
                                        <Listeners>
                                            <Click Fn="GetCCList" />
                                        </Listeners>
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
                        <Listeners>
                            <Command Fn="PasseLeaveData" />
                        </Listeners>
                    </ext:GridPanel>
                </Items>
            </ext:Window>
            <ext:Window ID="WindowCCList" runat="server" Title="<%$ Resources:LocalText,CCListTitle%>"
                Hidden="true" Layout="AbsoluteLayout" Width="530" Height="295" Resizable="False"
                AutoScroll="true" Modal="true" Padding="5" Closable="false">
                <Items>
                    <ext:ComboBox ID="cbxCCName" runat="server" X="10" Y="5" FieldLabel="<%$ Resources:LocalText,CC%>"
                        LabelWidth="50" EmptyText="Input Name" Width="200" DisplayField="Name" ValueField="Email">
                        <Store>
                            <ext:Store ID="StoreMail" runat="server">
                                <Reader>
                                    <ext:JsonReader>
                                        <Fields>
                                            <ext:RecordField Name="Name" />
                                            <ext:RecordField Name="Email" />
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
                            <Render Handler="this.triggers[1].hide();" />
                        </Listeners>
                        <DirectEvents>
                            <KeyUp OnEvent="GetEmail" Timeout="300000" Delay="1000">
                            </KeyUp>
                        </DirectEvents>
                    </ext:ComboBox>
                    <ext:Button ID="btnAddCC" runat="server" Icon="Add" X="215" Y="5">
                        <Listeners>
                            <Click Fn="AddMail" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="Button3" runat="server" X="440" Y="5" Width="70" Text="<%$ Resources:LocalText,Return%>">
                        <Listeners>
                            <Click Fn="SetCCListToolTip" />
                        </Listeners>
                    </ext:Button>
                    <ext:Panel ID="Panel4" runat="server" Height="220" Layout="FitLayout" X="5" Y="35">
                        <Items>
                            <ext:GridPanel ID="GPCCList" runat="server" StripeRows="true" TrackMouseOver="false"
                                Border="false">
                                <Store>
                                    <ext:Store ID="StoreCCList" runat="server">
                                        <Reader>
                                            <ext:JsonReader>
                                                <Fields>
                                                    <ext:RecordField Name="Email" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel3" runat="server">
                                    <Columns>
                                        <ext:RowNumbererColumn Width="30" />
                                        <ext:CommandColumn Width="25" ButtonAlign="Center">
                                            <Commands>
                                                <ext:GridCommand Icon="Delete" CommandName="Delete" ToolTip-Text="<%$ Resources:LocalText,Delete%>" />
                                            </Commands>
                                        </ext:CommandColumn>
                                        <ext:Column Header="Email" Width="200" DataIndex="Email" />
                                    </Columns>
                                </ColumnModel>
                                <Listeners>
                                    <Command Fn="MailRowCommand" />
                                </Listeners>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
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
    <ext:ToolTip ID="ToolTipCC" runat="server" Target="btnCC" Anchor="bottom" Height="80"
        Width="210" Padding="5" Title="<%$ Resources:LocalText,CCListTitle%>">
    </ext:ToolTip>
    <ext:ToolTip ID="ToolTip14" runat="server" Target="={#{cbxCOA}.list}" TrackMouse="true">
        <CustomConfig>
            <ext:ConfigItem Name="delegate" Value=".x-combo-list-item" Mode="Value" />
        </CustomConfig>
        <Listeners>
            <Show Fn="LOADCOATip" />
        </Listeners>
    </ext:ToolTip>
    <ext:ToolTip ID="ToolTip1" runat="server" Target="cbxCOA" TrackMouse="true">
        <Listeners>
            <Show Fn="LOADCOATip1" />
        </Listeners>
    </ext:ToolTip>
    </form>
</body>
</html>
