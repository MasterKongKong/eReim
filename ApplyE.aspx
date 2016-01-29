﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplyE.aspx.cs" Inherits="eReimbursement.ApplyE" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>交际费</title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
    <link href="Styles/StyleSheet1.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/PageJS.js" type="text/javascript"></script>
    <script type="text/javascript">
//        var LOADCOATip = function () {
//            var record = cbxCOA.findRecord(cbxCOA.displayField, this.triggerElement.innerHTML);
//            this.body.update('COACode: ' + record.get(cbxCOA.valueField));
//        };

//        var LOADCOATip1 = function () {
//            var record = cbxCOA.findRecord(cbxCOA.displayField, cbxCOA.getRawValue());
//            if (record != undefined) {
//                this.body.update('COACode: ' + record.get(cbxCOA.valueField));
//            }
//            else {
//                this.hide();
//            }
//        };
        var prepare = function (grid, toolbar, rowIndex, record) {
            var firstButton = toolbar.items.get(1);
            if (record.data.Draft != '0') {
                firstButton.setDisabled(true);
                firstButton.setTooltip("Disabled");
            }
            //you can return false to cancel toolbar for this record
        };
        var PostData = function () {
            //向主窗体传值
            var parentStore = parent.GridPanel1.store;
            var temps = parentStore.query('Type', 'E').items;
            Ext.each(temps, function (temp) {
                parentStore.remove(temp);
            });
            for (var i = 0; i < Store1.getAllRange().length; i++) {
                var newrecord = new parentStore.recordType();
                var record = Store1.getAllRange()[i].data;
                newrecord.data.Draft = '0';
                newrecord.data.Type = 'E';
//                newrecord.data.AccountName = record.AccountName;
                newrecord.data.AccountCode = "62010900";
                newrecord.data.AccountDes = record.AccountDes;
                newrecord.data.Cur = record.Cur;
                newrecord.data.Amount = record.Amount;
                newrecord.data.TSation = record.TSation;
                newrecord.data.Attach = record.Attach;
                newrecord.data.Tdate = record.Tdate;
                newrecord.data.EType = record.EType;
                newrecord.data.ETypeCode = record.ETypeCode;
                newrecord.data.Ecompany = record.Ecompany;
                newrecord.data.Eperson = record.Eperson;
                newrecord.data.Epurpos = record.Epurpos;
                newrecord.data.COAName = "";
                if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
                    newrecord.data.SubType = '交际费';
                }
                else {
                    newrecord.data.SubType = 'Entertainment';
                }
//                newrecord.data.Department1 = record.Department1;

//                newrecord.data.Vendor = record.Vendor;
//                newrecord.data.PaymentType = record.PaymentType;
//                newrecord.data.PaymentDate = record.PaymentDate;
//                newrecord.data.Budget = record.Budget;
//                newrecord.data.BudgetCurrent = record.BudgetCurrent;
                newrecord.data.DetailID = record.DetailID;
                parentStore.add(newrecord);
            }
            parent.Window1.hide();
//            parentStore.load();
            parent.GetSum();
            parent.SaveAll('D');
        };
        var GetSum = function () {
            //重新计算合计
            var Amount = 0; var no = 0;
            var data = Store1.getAllRange();
            Ext.each(data, function (record) {
                Amount += record.data.Amount == '' ? 0 : parseFloat(record.data.Amount);
                no++;
            });
            if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
                GridPanel1.setTitle('张数合计: ' + no.toString() + ', 金额合计: ' + GetNumber(Amount.toString()) + '.');
            }
            else {
                GridPanel1.setTitle('Total sheets:' + no.toString() + ', Sum: ' + GetNumber(Amount.toString()) + '.');
            }
        };
        var RowCommand = function (command, record, rowIndex) {
            if (command == 'Edit') {
                RowSelectionModel1.selectRow(rowIndex, true);
            }
            else if (command == 'Delete') {
                Store1.removeAt(rowIndex);
                EditBudgetCurrentStatus();
                GetSum();
                hdStore1id.setValue('');
            }
        };
        var UpdateStore = function (a) {
            if (dfDate.getValue().toString() == '') {
                if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
                    Ext.Msg.show({ title: '提示', msg: '请选择日期.', buttons: { ok: 'Ok'} });
                }
                else {
                    Ext.Msg.show({ title: 'Message', msg: 'Please select Date.', buttons: { ok: 'Ok'} });
                }
                return false;
            }
            if (nfAmount1.getValue() == '') {
                if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
                    Ext.Msg.show({ title: '提示', msg: '请输入金额.', buttons: { ok: 'Ok'} });
                }
                else {
                    Ext.Msg.show({ title: 'Message', msg: 'Please input Amount.', buttons: { ok: 'Ok'} });
                }

                return false;
            }
//            LabelPersonBG.setText(''); LabelStationBG.setText(''); LabelDepartmentBG.setText(''); LabelCOA.setText(''); LabelBudget.setText('');
            if (a == 'Update') {
                if (hdStore1id.getValue() != undefined && hdStore1id.getValue() != '') {
                    var id = hdStore1id.getValue();
                    Store1.getById(id).set('AccountDes', txtCOAContent.getValue());
                    Store1.getById(id).set('Cur', LabelCurrency.getText());
                    Store1.getById(id).set('Amount', nfAmount1.getValue().toString());
                    Store1.getById(id).set('TSation', cbxTStation.getValue());
                    Store1.getById(id).set('Tdate', dfDate.getRawValue());
                    Store1.getById(id).set('Attach', HLAttachFile.getText());
                    Store1.getById(id).set('EType', cbxEType.getValue());
                    Store1.getById(id).set('ETypeCode', cbxEType.getRawValue());
                    Store1.getById(id).set('Ecompany', cbxCustomer.getRawValue());
                    Store1.getById(id).set('Eperson', txtEperson.getValue());
                    Store1.getById(id).set('Epurpos', txtEpurpos.getValue());
//                    Store1.getById(id).set('Vendor', cbxVendor.getRawValue());
//                    Store1.getById(id).set('PaymentType', cbxPayType.getValue());
//                    Store1.getById(id).set('PaymentDate', dfPayDate.getRawValue());
//                    Store1.getById(id).set('Budget', cbxBudget.getValue()); //是否强制预算外
                    GetSum();
                }
                else {
                    Ext.Msg.show({ title: 'Message', msg: 'Please select one row to edit.', buttons: Ext.Msg.OK, icon: Ext.Msg.WARNING });
                    return false;
                }
            }
            else if (a == 'Insert') {
                var newrecord = new Store1.recordType();
                hdDetailIDSeed.setValue(parseInt(hdDetailIDSeed.getValue()) + 1);
                newrecord.data.DetailID = (parseInt(hdDetailIDSeed.getValue()) + 1).toString();
                newrecord.data.AccountDes = txtCOAContent.getValue();
                newrecord.data.Cur = LabelCurrency.getText();
                newrecord.data.Amount = nfAmount1.getValue().toString();
                newrecord.data.TSation = cbxTStation.getValue();
                newrecord.data.Tdate = dfDate.getRawValue();
                newrecord.data.Attach = HLAttachFile.getText();

                newrecord.data.EType = cbxEType.getValue();
                newrecord.data.ETypeCode = cbxEType.getRawValue();
                newrecord.data.Ecompany = cbxCustomer.getRawValue();
                newrecord.data.Eperson = txtEperson.getValue();
                newrecord.data.Epurpos = txtEpurpos.getValue();
                newrecord.data.Draft = '0';
                newrecord.data.AccountCode = "62010900";
                newrecord.data.Type = "E";
//                newrecord.data.Vendor = cbxVendor.getRawValue();
//                newrecord.data.PaymentType = cbxPayType.getValue();
//                newrecord.data.PaymentDate = dfPayDate.getRawValue();
//                newrecord.data.Budget = cbxBudget.getValue();
                Store1.add(newrecord);

                GetSum();
            }
            EditBudgetCurrentStatus();
        };
        var EditStore = function (json) {
            if (json.length > 0) {
                var obj = eval('(' + json + ')');
                for (var i = 0; i < obj.length; i++) {
                    var temps = Store1.query('DetailID', obj[i].DetailID).items[0].id;
                    Store1.getById(temps).set('BudgetCurrent', obj[i].BudgetCurrent);
                }
            }
        };
        var EditBudgetCurrentStatus = function () {
            var detail = ""; //记录Store数据为json格式
            for (var i = 0; i < Store1.getAllRange().length; i++) {
                var record = Store1.getAllRange()[i].data;
                detail += Ext.encode(record);
                if (i != Store1.getAllRange().length - 1) {
                    detail += ',';
                }
            };
            detail = '[' + detail + ']';
            RM.GetBudget(detail, {
                eventMask: {
                    showMask: true,
                    tartget: "Page"
                }
            });
        };

        var UpdateList = function (a) {
            if (a == 'Update') {
                if (hdStore1id.getValue() != undefined && hdStore1id.getValue() != '') {
                    var id = hdStore1id.getValue();
                    //                    Store1.getById(id).set('AccountName', cbxCOAType.getRawValue());
                    //                    Store1.getById(id).set('AccountCode', cbxCOAType.getValue());
                    Store1.getById(id).set('AccountDes', txtCOAContent.getValue());
                    Store1.getById(id).set('Cur', LabelCurrency.getText());
                    //                    Store1.getById(id).set('Amount', txtAmount1.getValue());
                    Store1.getById(id).set('Amount', nfAmount1.getValue().toString());
                    Store1.getById(id).set('TSation', cbxTStation.getValue());
                    Store1.getById(id).set('Tdate', dfDate.getRawValue());
                    Store1.getById(id).set('Attach', HLAttachFile.getText());
                    Store1.getById(id).set('EType', cbxEType.getValue());
                    Store1.getById(id).set('ETypeCode', cbxEType.getRawValue());
                    Store1.getById(id).set('Ecompany', cbxCustomer.getRawValue());
                    Store1.getById(id).set('Eperson', txtEperson.getValue());
                    Store1.getById(id).set('Epurpos', txtEpurpos.getValue());
//                    Store1.getById(id).set('COAName', cbxCOA.getRawValue());
//                    Store1.getById(id).set('AccountCode', cbxCOA.getValue());
//                    Store1.getById(id).set('Department1', cbxDepartment1.getValue());

//                    Store1.getById(id).set('Vendor', cbxVendor.getRawValue());
//                    Store1.getById(id).set('PaymentType', cbxPayType.getValue());
//                    Store1.getById(id).set('PaymentDate', dfPayDate.getRawValue());

                    GetSum();
                }
                else {
                    Ext.Msg.show({ title: 'Message', msg: 'Please select one row to edit.', buttons: Ext.Msg.OK, icon: Ext.Msg.WARNING });
                    return false;
                }
            }
            else if (a == 'Insert') {
                var newrecord = new Store1.recordType();
                //                newrecord.data.AccountName = cbxCOAType.getRawValue();
                //                newrecord.data.AccountCode = cbxCOAType.getValue();
                newrecord.data.AccountDes = txtCOAContent.getValue();
                newrecord.data.Cur = LabelCurrency.getText();
                //                newrecord.data.Amount = txtAmount1.getValue();
                newrecord.data.Amount = nfAmount1.getValue().toString();
                newrecord.data.TSation = cbxTStation.getValue();
                newrecord.data.Tdate = dfDate.getRawValue();
                newrecord.data.Attach = HLAttachFile.getText();

                newrecord.data.EType = cbxEType.getValue();
                newrecord.data.ETypeCode = cbxEType.getRawValue();
                newrecord.data.Ecompany = cbxCustomer.getRawValue();
                newrecord.data.Eperson = txtEperson.getValue();
                newrecord.data.Epurpos = txtEpurpos.getValue();
                newrecord.data.Draft = '0';
//                newrecord.data.COAName = cbxCOA.getRawValue();
                newrecord.data.AccountCode = "62010900";
                newrecord.data.Type = 'E';
//                newrecord.data.Department1 = cbxDepartment1.getValue();

//                newrecord.data.Vendor = cbxVendor.getRawValue();
//                newrecord.data.PaymentType = cbxPayType.getValue();
//                newrecord.data.PaymentDate = dfPayDate.getRawValue();

                Store1.add(newrecord);

                GetSum();
            }
        };
        var getDetail = function (row) {
            var reg = new RegExp("<br />");
            //            txtPurpose1.setValue(row.data.Purpose.replace(reg, "\n"));
            //            cbxCOAType.setValueAndFireSelect(row.data.AccountCode);
            txtCOAContent.setValue(row.data.AccountDes);
            LabelCurrency.setText(row.data.Cur);
            nfAmount1.setValue(row.data.Amount);
            cbxTStation.setValue(row.data.TSation);
            dfDate.setValue(row.data.Tdate);

            //            txtEcompany.setValue(row.data.Ecompany);
            cbxCustomer.setValue(row.data.Ecompany);
            txtEperson.setValue(row.data.Eperson);
            txtEpurpos.setValue(row.data.Epurpos);
            HLAttachFile.setText(row.data.Attach);
            HLAttachFile.setUrl('./Upload/' + row.data.Attach);
            hdStore1id.setValue(row.id.toString());
            hdDetailID.setValue(row.data.DetailID);
//            cbxCOA.setValueAndFireSelect(row.data.AccountCode);
            cbxEType.setValueAndFireSelect(row.data.EType);
//            cbxDepartment1.setValue(row.data.Department1);

//            cbxVendor.setValue(row.data.Vendor);
//            RM.GetVendor(row.data.Vendor,row.data.Ecompany, {});
//            cbxPayType.setValueAndFireSelect(row.data.PaymentType);
//            dfPayDate.setValue(row.data.PaymentDate);

//            cbxBudget.setValue(row.data.Budget);
//            LabelBudget.setText(row.data.BudgetCurrent);
//            LabelPersonBG.setText(''); LabelStationBG.setText(''); LabelDepartmentBG.setText(''); LabelCOA.setText(''); LabelBudget.setText('');

//            if (dfDate.value != '') {
//                //查看时,不算入本页临时数据
//                RM.LoadBG('', 'GetDetail', 0, {
//                    eventMask: {
//                        showMask: true,
//                        tartget: "Page"
//                    },
//                    timeout: 300000
//                });
//            }
//            if (row.data.AccountCode != '') {
//                cbxCOA.triggers[0].show();
//            }
//            else {
//                cbxCOA.triggers[0].hide();
//            }
//            if (row.data.TSation != '') {
//                cbxTStation.triggers[0].show();
//            }
//            else {
//                cbxTStation.triggers[0].hide();
//            }
//            if (row.data.Vendor != '') {
//                cbxVendor.triggers[0].show();
//            }
//            else {
//                cbxVendor.triggers[0].hide();
//            }
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
//            if (cbxCOA.getRawValue() == '') {
//                if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
//                    Ext.Msg.show({ title: '提示', msg: '请选择费用类型.', buttons: { ok: 'Ok'} });
//                }
//                else {
//                    Ext.Msg.show({ title: 'Message', msg: 'Please select Expense Type.', buttons: { ok: 'Ok'} });
//                }

//                return false;
//            }
//            if (cbxDepartment1.getRawValue() == '') {
//                if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
//                    Ext.Msg.show({ title: '提示', msg: '请选择所属预算部门.', buttons: { ok: 'Ok'} });
//                }
//                else {
//                    Ext.Msg.show({ title: 'Message', msg: 'Please select Budget department.', buttons: { ok: 'Ok'} });
//                }

//                return false;
//            }
//            LabelPersonBG.setText(''); LabelStationBG.setText(''); LabelDepartmentBG.setText(''); LabelCOA.setText(''); LabelBudget.setText('');
            return true;
        };
        var LoadBG = function () {
            if (CheckBG()) {
                var sum = 0;
                for (var i = 0; i < Store1.getAllRange().length; i++) {
                    var record = Store1.getAllRange()[i].data;
                    if (!record.Budget) {
                        sum += parseFloat(record.Amount);
                    }
                }
                RM.LoadBG('', 'GetDetail', sum, {
                    eventMask: {
                        showMask: true,
                        tartget: "Page"
                    },
                    timeout: 300000
                });
            }
        };
        var GetList = function (a, b, c, d) {
            var q = a.query;

            a.query = new RegExp(q);
            a.query.length = q.length;
        };
    </script>
    <style type="text/css">
        #Panel4 .x-panel-header
        {
            border-top-width: 0px;
            border-left-width: 0px;
        }
        #Panel4 .x-panel-body
        {
            border-bottom-width: 0px;
            border-left-width: 0px;
        }
        #Panel12 .x-panel-header
        {
            border-top-width: 0px;
            border-right-width: 0px;
        }
        #Panel12 .x-panel-body
        {
            border-bottom-width: 0px;
            border-right-width: 0px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" DirectMethodNamespace="RM"
        Locale="en-US">
    </ext:ResourceManager>
    <ext:Hidden ID="hdScanFile" runat="server" />
    <ext:Hidden ID="hdStore1id" runat="server" />
    <ext:Hidden ID="hdDetailID" runat="server" />
    <ext:Hidden ID="hdDetailIDSeed" runat="server" />
    <ext:Hidden ID="hdBudget" runat="server" />
    <ext:Hidden ID="hdCurrency" runat="server" />
    <ext:Hidden ID="hdStation" runat="server" />
    <ext:Store ID="Store1" runat="server">
        <Reader>
            <ext:JsonReader>
                <Fields>
                    <ext:RecordField Name="DetailID" />
                    <ext:RecordField Name="MainID" />
                    <ext:RecordField Name="Type" />
                    <ext:RecordField Name="AccountName" />
                    <ext:RecordField Name="AccountCode" />
                    <ext:RecordField Name="AccountDes" />
                    <ext:RecordField Name="Cur" />
                    <ext:RecordField Name="Amount" />
                    <ext:RecordField Name="TSation" />
                    <ext:RecordField Name="Tdate" />
                    <ext:RecordField Name="Fcity" />
                    <ext:RecordField Name="Tcity" />
                    <ext:RecordField Name="Attach" />
                    <ext:RecordField Name="EType" />
                    <ext:RecordField Name="EcompanyCode" />
                    <ext:RecordField Name="Ecompany" />
                    <ext:RecordField Name="Eperson" />
                    <ext:RecordField Name="Epurpos" />
                    <ext:RecordField Name="Creadedby" />
                    <ext:RecordField Name="CreaedeDate" />
                    <ext:RecordField Name="EffectTime" />
                    <ext:RecordField Name="ETypeCode" />
                    <ext:RecordField Name="Draft" />
                    <ext:RecordField Name="COAName" />
                    <ext:RecordField Name="Department1" />

                    <ext:RecordField Name="PaymentType" />
                    <ext:RecordField Name="PaymentDate" />
                    <ext:RecordField Name="Vendor" />
                    <ext:RecordField Name="Budget" Type="Boolean"/>
                    <ext:RecordField Name="BudgetCurrent" />
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
    </ext:Store>
    <ext:Panel ID="Panel3" runat="server" Height="510" Width="880" Border="false" Padding="10"
        MinHeight="300" AutoScroll="true" Layout="AbsoluteLayout">
        <Items>
            <ext:TabPanel ID="Panel1" runat="server" Height="190" X="10" Y="10">
                <Items>
                    <ext:Panel ID="Panel5" runat="server" Layout="FitLayout" Title="<%$ Resources:LocalText,GeneralInfo%>">
                        <Items>
                            <ext:ColumnLayout ID="ColumnLayout1" runat="server">
                                <Columns>
                                    <ext:LayoutColumn ColumnWidth="1">
                                        <ext:Panel ID="Panel4" runat="server" Layout="AbsoluteLayout" Padding="10" AutoScroll="true"
                                            Title="<%$ Resources:LocalText,ExpenseDetail%>" StyleSpec="margin-right:3px;">
                                            <Items>
                                                <ext:Label ID="Label4" runat="server" X="10" Y="13" Text="<%$ Resources:LocalText,Currency%>" />
                                                <ext:Label ID="LabelCurrency" runat="server" X="70" Y="13" />
                                                <%--<ext:TextField ID="txtAmount1" runat="server" FieldLabel="<%$ Resources:LocalText,Amount%>"
                                                    X="110" Y="40" Width="120" LabelWidth="60" />--%>
                                                <ext:NumberField ID="nfAmount1" runat="server" FieldLabel="<%$ Resources:LocalText,Amount%>"
                                                    X="110" Y="10" Width="120" LabelWidth="60" />
                                                <ext:DateField ID="dfDate" runat="server" FieldLabel="<%$ Resources:LocalText,Date%>"
                                                    Format="yyyy/MM/dd" X="235" Y="10" Width="160" LabelWidth="40" EmptyText="yyyy/MM/dd" />
                                                <ext:ComboBox ID="cbxTStation" runat="server" FieldLabel="<%$ Resources:LocalText,CostCenter%>"
                                                    X="415" Y="10" Width="180" LabelWidth="70" EmptyText="Station Code" DisplayField="cityCode"
                                                    ValueField="cityCode" Disabled="true">
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
                                                <ext:Label ID="Label2" runat="server" Width="85" X="10" Y="43" Text="<%$ Resources:LocalText,File1%>"
                                                    Cls="UploadLabel" />
                                                <ext:FileUploadField ID="FileUploadField1" runat="server" Icon="Attach" X="80" Y="30"
                                                    Width="150" ButtonText="" />
                                                <ext:Button ID="btnUploadScanFile" runat="server" X="243" Y="40" Icon="Add">
                                                    <DirectEvents>
                                                        <Click OnEvent="UploadScanFileClick" Before="if (FileUploadField1.getValue()=='') { Ext.Msg.alert('Error','Please select file.'); return false; } 
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
                                                <ext:Button ID="Button1" runat="server" X="268" Y="40" Icon="Delete">
                                                    <Listeners>
                                                        <Click Handler="HLAttachFile.setText('');hdScanFile.setValue('');" />
                                                    </Listeners>
                                                </ext:Button>
                                                <ext:HyperLink ID="HLAttachFile" runat="server" Text="" X="298" Y="43" Target="_blank" />
                                                <ext:TextField ID="txtCOAContent" runat="server" FieldLabel="<%$ Resources:LocalText,COARemark%>"
                                                    X="10" Y="70" Anchor="100%" LabelWidth="70" MaxLength="100">
                                                </ext:TextField>
                                                <ext:ComboBox ID="cbxVendor" runat="server" FieldLabel="<%$ Resources:LocalText,Vendor%>"
                                LabelWidth="60" Width="200" X="10" Y="130" EmptyText="Customer Code" DisplayField="cityCode"
                                ValueField="cityID" ForceSelection="False"  MaxLength="100" Hidden="true">
                                <Store>
                                    <ext:Store ID="StoreVendor" runat="server">
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
                                    <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();LabelVendorName.setText('');StoreVendor.removeAll();}" />
                                    <Select Handler="this.triggers[0].show();LabelVendorName.setText(cbxVendor.getValue());" />
                                    <KeyUp Fn="CheckKey" />
                                    <Render Handler="this.triggers[1].hide();" />
                                </Listeners>
                                <DirectEvents>
                                    <KeyUp OnEvent="GetVendorList" Timeout="300000" Delay="1000" />
                                    <%--<Select OnEvent="GetCustomerSum" Timeout="300000" />--%>
                                </DirectEvents>
                            </ext:ComboBox>
                            <ext:Label ID="LabelVendorName" runat="server" X="220" Y="133" Hidden="true"/>
                            <ext:ComboBox ID="cbxPayType" runat="server" FieldLabel="<%$ Resources:LocalText,PaymentType%>"
                                        LabelWidth="100" Width="205" X="10" Y="160" DisplayField="Text" ValueField="Value"
                                        Editable="False" Hidden="true">
                                        <Store>
                                            <ext:Store ID="StorePayType" runat="server">
                                                <Reader>
                                                    <ext:JsonReader>
                                                        <Fields>
                                                            <ext:RecordField Name="Text" />
                                                            <ext:RecordField Name="Value" />
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
                                    <ext:DateField ID="dfPayDate" runat="server" FieldLabel="<%$ Resources:LocalText,PaymentDate%>"
                                        LabelWidth="100" Width="200" X="225" Y="160" Format="yyyy/MM/dd" EmptyText="yyyy/MM/dd" Hidden="true">
                                    </ext:DateField>
                                    <ext:Label ID="Label1" runat="server" Text="强制预算外" X="435" Y="163" Cls="UploadLabel" Hidden="true"/>
                                        <ext:Checkbox ID="cbxBudget" runat="server" X="510" Y="160" Hidden="true"/>

                                                <ext:Button ID="Button9" runat="server" Text="<%$ Resources:LocalText,Others%>" X="10"
                                                    Y="98" Width="185">
                                                    <Listeners>
                                                        <Click Handler="Panel1.setActiveTab(1);" />
                                                    </Listeners>
                                                </ext:Button>
                                                <ext:Button ID="Button5" runat="server" Text="<%$ Resources:LocalText,Edit%>" X="200"
                                                    Y="98" Width="90" Icon="Accept">
                                                    <Listeners>
                                                        <Click Handler="UpdateStore('Update');" />
                                                    </Listeners>
                                                </ext:Button>
                                                <ext:Button ID="Button6" runat="server" Text="<%$ Resources:LocalText,Add%>" X="295"
                                                    Y="98" Width="90" Icon="Add">
                                                    <Listeners>
                                                        <Click Handler="UpdateStore('Insert');" />
                                                    </Listeners>
                                                </ext:Button>
                                                <ext:Button ID="btnLoadBudget" runat="server" Text="<%$ Resources:LocalText,BudgetReview%>"
                                                    X="295" Y="98" Width="90" Hidden="true">
                                                    <Listeners>
                                                        <Click Fn="LoadBG" />
                                                    </Listeners>
                                                </ext:Button>
                                            </Items>
                                        </ext:Panel>
                                    </ext:LayoutColumn>
                                    <ext:LayoutColumn>
                                        <ext:Panel ID="Panel12" runat="server" Title="<%$ Resources:LocalText,Budget%>" Width="250"
                                            Layout="FitLayout" Hidden="true">
                                            <Items>
                                                <ext:Panel ID="PanelBudget" runat="server" Layout="AbsoluteLayout" Border="false"
                                                    AutoScroll="True">
                                                    <Items>
                                                <ext:Label ID="LabelCOA" runat="server" X="5" Y="5" Cls="budgetfont" />
                                                <ext:Label ID="LabelStationBG" runat="server" X="5" Y="40" Cls="budgetfont" />
                                                <ext:Label ID="LabelDepartmentBG" runat="server" X="5" Y="60" Cls="budgetfont" />
                                                <ext:Label ID="LabelPersonBG" runat="server" X="5" Y="80" Cls="budgetfont" />
                                                <ext:Label ID="LabelBudget" runat="server" X="5" Y="100" Cls="budgetfont" />
                                                    </Items>
                                                </ext:Panel>
                                            </Items>
                                        </ext:Panel>
                                    </ext:LayoutColumn>
                                </Columns>
                            </ext:ColumnLayout>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel2" runat="server" Title="<%$ Resources:LocalText,Others%>" Layout="AbsoluteLayout"
                        Padding="10">
                        <Items>
                            <ext:ComboBox ID="cbxEType" runat="server" FieldLabel="<%$ Resources:LocalText,CustomerType%>"
                                X="10" Y="10" Width="200" LabelWidth="90" ValueField="Code" DisplayField="Name"
                                Editable="False">
                                <Store>
                                    <ext:Store ID="StoreEType" runat="server">
                                        <Reader>
                                            <ext:JsonReader>
                                                <Fields>
                                                    <ext:RecordField Name="Name" />
                                                    <ext:RecordField Name="Code" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <%--<Triggers>
                                    <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                                    <Select Handler="this.triggers[0].show();if(cbxEType.getValue()=='1') {LabelCustomerName.setText('');LabelCustomerSum.setText('');StoreCustomer.removeAll();}" />
                                </Listeners>--%>
                            </ext:ComboBox>
                            <ext:TextField ID="txtEperson" runat="server" FieldLabel="<%$ Resources:LocalText,Guest%>"
                                X="225" Y="10" Width="205" LabelWidth="60"  MaxLength="100"/>
                            <%--<ext:TextField ID="txtEcompany" runat="server" FieldLabel="客户" X="10" Y="40" Width="205"
                                LabelWidth="60" Hidden="true" />--%>
                            <ext:ComboBox ID="cbxCustomer" runat="server" FieldLabel="<%$ Resources:LocalText,Customer%>"
                                LabelWidth="60" Width="200" X="10" Y="40" EmptyText="Customer Code" DisplayField="cityCode"
                                ValueField="cityID" ForceSelection="False"  MaxLength="100">
                                <Store>
                                    <ext:Store ID="StoreCustomer" runat="server">
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
                                    <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();LabelCustomerName.setText('');LabelCustomerSum.setText('');StoreCustomer.removeAll();}" />
                                    <Select Handler="this.triggers[0].show();" />
                                    <KeyUp Fn="CheckKey" />
                                </Listeners>
                                <DirectEvents>
                                    <KeyUp OnEvent="GetCustomer" Timeout="300000" Delay="1000" />
                                    <Select OnEvent="GetCustomerSum" Timeout="300000" />
                                </DirectEvents>
                            </ext:ComboBox>
                            <ext:Label ID="LabelCustomerName" runat="server" X="220" Y="43" />
                            <ext:Label ID="LabelCustomerSum" runat="server" X="580" Y="43" />
                            <ext:TextField ID="txtEpurpos" runat="server" FieldLabel="<%$ Resources:LocalText,Purpose%>"
                                X="10" Y="70" Anchor="100%" LabelWidth="60"  MaxLength="100"/>
                            <ext:Button ID="Button2" runat="server" Text="<%$ Resources:LocalText,Edit%>" X="10"
                                Y="98" Width="90" Icon="Accept">
                                <Listeners>
                                    <Click Handler="UpdateStore('Update');" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="Button3" runat="server" Text="<%$ Resources:LocalText,Add%>" X="105"
                                Y="98" Width="90" Icon="Add">
                                <Listeners>
                                    <Click Handler="UpdateStore('Insert');" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="Button10" runat="server" Text="<%$ Resources:LocalText,GeneralInfo%>"
                                X="200" Y="98" Width="90">
                                <Listeners>
                                    <Click Handler="Panel1.setActiveTab(0);" />
                                </Listeners>
                            </ext:Button>
                        </Items>
                    </ext:Panel>
                </Items>
            </ext:TabPanel>
            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" Title="<%$ Resources:LocalText,ExpenseList%>"
                TrackMouseOver="false" Height="160" X="10" Y="193" AutoScroll="true">
                <ColumnModel ID="ColumnModel1" runat="server">
                    <Columns>
                        <ext:CommandColumn Width="50" ButtonAlign="Center">
                            <Commands>
                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit" ToolTip-Text="<%$ Resources:LocalText,View%>" />
                                <ext:GridCommand Icon="Delete" CommandName="Delete" ToolTip-Text="<%$ Resources:LocalText,Delete%>" />
                            </Commands>
                            <PrepareToolbar Fn="prepare" />
                        </ext:CommandColumn>
                        <%--<ext:Column Header="<%$ Resources:LocalText,ExpenseType%>" Width="200" DataIndex="COAName" />
                        <ext:Column Header="COACode" Width="90" DataIndex="AccountCode" />--%>
                        <ext:Column Header="<%$ Resources:LocalText,Amount%>" Width="60" DataIndex="Amount">
                            <Renderer Fn="GetNumber" />
                        </ext:Column>
                        <%--<ext:Column Header="<%$ Resources:LocalText,CostCenter%>" Width="120" DataIndex="TSation" />--%>
                        <%--<ext:Column Header="<%$ Resources:LocalText,Department1%>" Width="120" DataIndex="Department1" />--%>
                        <ext:Column Header="<%$ Resources:LocalText,COARemark%>" Width="100" DataIndex="AccountDes" />
                        <ext:DateColumn Header="<%$ Resources:LocalText,Date%>" Width="80" DataIndex="Tdate"
                            Format="yyyy/MM/dd" />
                        <ext:Column Header="<%$ Resources:LocalText,File%>" Width="160" DataIndex="Attach">
                            <Renderer Fn="attachlink" />
                        </ext:Column>
                        <%--<ext:Column Header="<%$ Resources:LocalText,BudgetOrNot%>" Width="100" DataIndex="BudgetCurrent" Sortable="False"/>
                        <ext:CheckColumn Header="<%$ Resources:LocalText,UnBudget%>" Width="150" DataIndex="Budget" Sortable="False"/>--%>
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                        <Listeners>
                            <RowSelect Handler="if(record!=undefined){getDetail(record);}else{return false;}" />
                        </Listeners>
                    </ext:RowSelectionModel>
                </SelectionModel>
                <Listeners>
                    <Command Fn="RowCommand" />
                    <Render Handler="if(parent.AddSubStore('E')){EditBudgetCurrentStatus();}" />
                </Listeners>
                
            </ext:GridPanel>

            <ext:GridPanel ID="GridPanelBudget" runat="server" TrackMouseOver="false" Height="110" X="10" Y="353" AutoScroll="true">
                <Store>
                    <ext:Store ID="StoreBudget" runat="server">
                        <Reader>
                            <ext:JsonReader>
                                <Fields>
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <ColumnModel ID="ColumnModel4" runat="server">
                    <Columns>
                    </Columns>
                </ColumnModel>
                <View>
                    <ext:GridView ID="GridView2" runat="server" />
                </View>
            </ext:GridPanel>

            <ext:Button ID="btnSave" runat="server" Text="<%$ Resources:LocalText,SaveAsDraft%>" X="20"
                Y="470" Width="120">
                <Listeners>
                    <Click Fn="PostData" />
                </Listeners>
            </ext:Button>
            <ext:Button ID="btnSaveDraft" runat="server" Text="<%$ Resources:LocalText,Cancel%>"
                X="150" Y="470" Width="120">
                <Listeners>
                    <Click Handler="parent.Window1.hide();" />
                </Listeners>
            </ext:Button>
        </Items>
    </ext:Panel>
    <%--<ext:ToolTip ID="ToolTip14" runat="server" Target="={#{cbxCOA}.list}" TrackMouse="true">
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
    </ext:ToolTip>--%>
    </form>
</body>
</html>
