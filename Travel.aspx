<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Travel.aspx.cs" Inherits="eReimbursement.Travel" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
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
        var serverip = window.location.host;
        function trim(str) { //删除左右两端的空格
            return str.replace(/(^\s*)|(\s*$)/g, "");
        }
        var a1 = ""; //记录申请类型:草稿或者正式
        var detail1 = ""; //记录费用明细
        var CCMailListdetail1 = ""; //记录邮件抄送人列表
        var header0string1 = ""; //记录第一行列头
        var header1string1 = ""; //记录第二行列头
        var header3string1 = ""; //记录第三行列头
        var SaveAllNew = function (budget) {
            RM.SaveAll1(a1, detail1, CCMailListdetail1, header0string1, header1string1, header2string1, LabelCurrency.getText(), labelDepartment.getText(), budget, {
                eventMask: {
                    showMask: true,
                    tartget: "Page"
                },
                timeout: 300000
            });
        };
        var LoadBudget = function () {
            if ($("#CheckBoxOnBehalfItem").attr("checked")!= "checked"&&hdOwnerID.getValue().toString() == '') {
                if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
                    Ext.Msg.show({ title: 'Message', msg: '请从eLeave选择出差记录.', buttons: { ok: 'Ok'} });
                }
                else {
                    Ext.Msg.show({ title: 'Message', msg: 'Please select eLeave Data.', buttons: { ok: 'Ok'} });
                }
                return false;
            }
            var headercol = GridPanel2.view.headerRows[0].columns.length;
            var col = GridView1.cm.columns.length;
            var havestation = false; var havedate1 = false; var havedate2 = false;
            //出差站点不能为空不能重复
            var dstnlist = [];
            for (var i = 1; i < headercol - 2; i++) {
                if (!GridView1.cm.columns[i].hidden) {
                    if (i % 2 == 1) {
                        if (GridPanel2.view.headerRows[2].columns[i].component.getRawValue() == '' || GridPanel2.view.headerRows[2].columns[i + 1].component.getRawValue() == '') {
                            var sd = 'Please input Travel Period of ' + GridPanel2.view.headerRows[0].columns[i].component.getValue() + '.';
                            Ext.Msg.show({ title: 'Message', msg: sd, buttons: { ok: 'Ok'} });
                            return false;
                        }
                    }
                }
            }

            //            var col = GridView1.cm.columns.length;
            var colnew = GridView1.cm.columns.length;
            var header0string = ''; var header1string = ''; var header2string = '';
            for (var i = 1; i < col - 2; i++) {
                if (GridView1.cm.columns[i].hidden) {
                    colnew--;
                }
                else {
                    if (i % 2 == 1) {
                        //                        var h0 = 'NA';
                        //                        var h1 = 'NA';
                        var h0 = GridPanel2.view.headerRows[0].columns[i].component.getValue() == '' ? 'NA' : GridPanel2.view.headerRows[0].columns[i].component.getValue();
                        var h1 = GridPanel2.view.headerRows[1].columns[i].component.getValue() == '' ? 'NA' : GridPanel2.view.headerRows[1].columns[i].component.getValue();
                        header0string += (header0string != '') ? (',' + h0) : h0;
                        header1string += (header1string != '') ? (',' + h1) : h1;
                    }
                    //                    var h2 = 'NA';
                    var h2 = GridPanel2.view.headerRows[2].columns[i].component.getRawValue() == '' ? 'NA' : GridPanel2.view.headerRows[2].columns[i].component.getRawValue();
                    header2string += (header2string != '') ? (',' + h2) : h2;
                }
            }
            if (colnew < 4) {
                Ext.Msg.show({ title: 'Message', msg: 'Please Add DSTN.', buttons: { ok: 'Ok'} });
                return false;
            }
            var detail = ""; //记录Store数据为json格式
            for (var i = 0; i < Store2.getAllRange().length; i++) {
                var record = Store2.getAllRange()[i].data;
                detail += Ext.encode(record);
                if (i != Store2.getAllRange().length - 1) {
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
            RM.LoadBudget(detail, header1string, header2string, {
                eventMask: {
                    showMask: true,
                    tartget: "Page"
                }
            });
        };
        var SaveAll = function (a) {
            //            if (!Radio1.getValue() && !Radio2.getValue()) {
            //                if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
            //                    Ext.Msg.show({ title: 'Message', msg: '请选择Budget或者Un-Budget.', buttons: { ok: 'Ok'} });
            //                }
            //                else {
            //                    Ext.Msg.show({ title: 'Message', msg: 'Please select Budget or Un-Budget.', buttons: { ok: 'Ok'} });
            //                }
            //                return false;
            //            }
            if ($("#CheckBoxOnBehalfItem").attr("checked") != "checked" && hdOwner.getValue().toString() == '') {
                if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
                    Ext.Msg.show({ title: 'Message', msg: '请从eLeave选择出差记录.', buttons: { ok: 'Ok'} });
                }
                else {
                    Ext.Msg.show({ title: 'Message', msg: 'Please select eLeave Data.', buttons: { ok: 'Ok'} });
                }
                return false;
            }
            if ($("#CheckBoxOnBehalfItem").attr("checked") == "checked" && cbxOnBehalfName.getValue().toString() == '') {
                if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
                    Ext.Msg.show({ title: 'Message', msg: '请选择被垫付人.', buttons: { ok: 'Ok'} });
                }
                else {
                    Ext.Msg.show({ title: 'Message', msg: 'Please select On behalf of Person.', buttons: { ok: 'Ok'} });
                }
                return false;
            }

            GetSum();
            //为全局变量复制
            a1 = a;
            //
            if (a == 'ND') {
                //正式申请
                if ($("#CheckBoxOnBehalfItem").attr("checked") != "checked"&&hdReport.getValue().toString() == '') {
                    if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
                        Ext.Msg.show({ title: 'Message', msg: '请上传出差报告.', buttons: { ok: 'Ok'} });
                    }
                    else {
                        Ext.Msg.show({ title: 'Message', msg: 'Please upload travel report.', buttons: { ok: 'Ok'} });
                    }
                    return false;
                }
                var headercol = GridPanel2.view.headerRows[0].columns.length;
                var col = GridView1.cm.columns.length;
                var havestation = false; var havedate1 = false; var havedate2 = false;
                //出差站点不能为空不能重复
                var dstnlist = [];
                for (var i = 1; i < headercol - 2; i++) {
                    if (!GridView1.cm.columns[i].hidden) {
                        if (i % 2 == 1) {
                            if (GridPanel2.view.headerRows[0].columns[i].component.getValue() != '') {
                                havestation = true;
                                if (GridPanel2.view.headerRows[0].columns[i].component.getValue().indexOf(',') != -1 || GridPanel2.view.headerRows[1].columns[i].component.getValue().indexOf(',') != -1) {
                                    Ext.Msg.show({ title: 'Message', msg: 'Comma(,) is invalid,pls remove it.', buttons: { ok: 'Ok'} });
                                    return false;
                                }
                                if (GridPanel2.view.headerRows[2].columns[i].component.getRawValue() == '' || GridPanel2.view.headerRows[2].columns[i + 1].component.getRawValue() == '') {
                                    var sd = 'Please input Travel Period of ' + GridPanel2.view.headerRows[0].columns[i].component.getValue() + '.';
                                    Ext.Msg.show({ title: 'Message', msg: sd, buttons: { ok: 'Ok'} });
                                    return false;
                                }

                                for (var j = 0; j < dstnlist.length; j++) {
                                    if (dstnlist[j] == GridPanel2.view.headerRows[0].columns[i].component.getValue()) {
                                        var sd = 'Duplicate Location:' + GridPanel2.view.headerRows[0].columns[i].component.getValue() + '.';
                                        Ext.Msg.show({ title: 'Message', msg: sd, buttons: { ok: 'Ok'} });
                                        return false;
                                    }
                                }

                                dstnlist.push(GridPanel2.view.headerRows[0].columns[i].component.getValue());
                            }
                        }
                    }
                }
                if (!havestation) {
                    Ext.Msg.show({ title: 'Message', msg: 'Please input DSTN.', buttons: { ok: 'Ok'} });
                    return false;
                }
                else {
                    Ext.Msg.confirm('Message', 'Are you sure?', function (btn, text) {
                        if (btn == 'yes') {
                            var col = GridView1.cm.columns.length;
                            var colnew = GridView1.cm.columns.length;
                            var header0string = ''; var header1string = ''; var header2string = '';
                            for (var i = 1; i < col - 2; i++) {
                                if (GridView1.cm.columns[i].hidden) {
                                    colnew--;
                                }
                                else {
                                    if (i % 2 == 1) {
                                        //                        var h0 = 'NA';
                                        //                        var h1 = 'NA';
                                        if (GridPanel2.view.headerRows[0].columns[i].component.getValue().indexOf(',') != -1 || GridPanel2.view.headerRows[1].columns[i].component.getValue().indexOf(',') != -1) {
                                            Ext.Msg.show({ title: 'Message', msg: 'Comma(,) is invalid,pls remove it.', buttons: { ok: 'Ok'} });
                                            return false;
                                        }
                                        var h0 = GridPanel2.view.headerRows[0].columns[i].component.getValue() == '' ? 'NA' : GridPanel2.view.headerRows[0].columns[i].component.getValue();
                                        var h1 = GridPanel2.view.headerRows[1].columns[i].component.getValue() == '' ? 'NA' : GridPanel2.view.headerRows[1].columns[i].component.getValue();
                                        header0string += (header0string != '') ? (',' + h0) : h0;
                                        header1string += (header1string != '') ? (',' + h1) : h1;
                                    }
                                    //                    var h2 = 'NA';
                                    var h2 = GridPanel2.view.headerRows[2].columns[i].component.getRawValue() == '' ? 'NA' : GridPanel2.view.headerRows[2].columns[i].component.getRawValue();
                                    header2string += (header2string != '') ? (',' + h2) : h2;
                                }
                            }
                            if (colnew < 4) {
                                Ext.Msg.show({ title: 'Message', msg: 'Please Add DSTN.', buttons: { ok: 'Ok'} });
                                return false;
                            }
                            var detail = ""; //记录Store数据为json格式
                            for (var i = 0; i < Store2.getAllRange().length; i++) {
                                var record = Store2.getAllRange()[i].data;
                                detail += Ext.encode(record);
                                if (i != Store2.getAllRange().length - 1) {
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
                            //全局变量复制
                            detail1 = detail;
                            CCMailListdetail1 = CCMailListdetail;
                            header0string1 = header0string;
                            header1string1 = header1string;
                            header2string1 = header2string;

                            RM.SaveAll(a, detail, CCMailListdetail, header0string, header1string, header2string, LabelCurrency.getText(), labelDepartment.getText(), {
                                eventMask: {
                                    showMask: true,
                                    tartget: "Page"
                                }
                            });
                        }
                    });
                }
                //                Store2.load();
            }
            else {
                //草稿
                var headercol = GridPanel2.view.headerRows[0].columns.length;
                var col = GridView1.cm.columns.length;
                var havestation = false; var havedate1 = false; var havedate2 = false;
                //出差站点不能为空
                var dstnlist = [];
                for (var i = 1; i < headercol - 2; i++) {
                    if (!GridView1.cm.columns[i].hidden) {
                        if (i % 2 == 1) {
                            if (GridPanel2.view.headerRows[0].columns[i].component.getValue() != '') {
                                if (GridPanel2.view.headerRows[0].columns[i].component.getValue().indexOf(',') != -1 || GridPanel2.view.headerRows[1].columns[i].component.getValue().indexOf(',') != -1) {
                                    Ext.Msg.show({ title: 'Message', msg: 'Comma(,) is invalid,pls remove it.', buttons: { ok: 'Ok'} });
                                    return false;
                                }
                                havestation = true;
                                if (GridPanel2.view.headerRows[2].columns[i].component.getRawValue() == '' || GridPanel2.view.headerRows[2].columns[i + 1].component.getRawValue() == '') {
                                    var sd = 'Please input Travel Period of ' + GridPanel2.view.headerRows[0].columns[i].component.getValue() + '.';
                                    Ext.Msg.show({ title: 'Message', msg: sd, buttons: { ok: 'Ok'} });
                                    return false;
                                }

                                for (var j = 0; j < dstnlist.length; j++) {
                                    if (dstnlist[j] == GridPanel2.view.headerRows[0].columns[i].component.getValue()) {
                                        var sd = 'Duplicate Location:' + GridPanel2.view.headerRows[0].columns[i].component.getValue() + '.';
                                        Ext.Msg.show({ title: 'Message', msg: sd, buttons: { ok: 'Ok'} });
                                        return false;
                                    }
                                }

                                dstnlist.push(GridPanel2.view.headerRows[0].columns[i].component.getValue());
                            }
                        }
                    }
                }
                //至少有一个DSTN
                if (!havestation) {
                    Ext.Msg.show({ title: 'Message', msg: 'Please input DSTN.', buttons: { ok: 'Ok'} });
                    return false;
                }

                var colnew = GridView1.cm.columns.length;
                var header0string = ''; var header1string = ''; var header2string = '';
                for (var i = 1; i < col - 2; i++) {
                    if (GridView1.cm.columns[i].hidden) {
                        colnew--;
                    }
                    else {
                        if (i % 2 == 1) {
                            //                        var h0 = 'NA';
                            //                        var h1 = 'NA';
                            if (GridPanel2.view.headerRows[0].columns[i].component.getValue().indexOf(',') != -1 || GridPanel2.view.headerRows[1].columns[i].component.getValue().indexOf(',') != -1) {
                                Ext.Msg.show({ title: 'Message', msg: 'Comma(,) is invalid,pls remove it.', buttons: { ok: 'Ok'} });
                                return false;
                            }
                            var h0 = GridPanel2.view.headerRows[0].columns[i].component.getValue() == '' ? 'NA' : GridPanel2.view.headerRows[0].columns[i].component.getValue();
                            var h1 = GridPanel2.view.headerRows[1].columns[i].component.getValue() == '' ? 'NA' : GridPanel2.view.headerRows[1].columns[i].component.getValue();
                            header0string += (header0string != '') ? (',' + h0) : h0;
                            header1string += (header1string != '') ? (',' + h1) : h1;
                        }
                        //                    var h2 = 'NA';
                        var h2 = GridPanel2.view.headerRows[2].columns[i].component.getRawValue() == '' ? 'NA' : GridPanel2.view.headerRows[2].columns[i].component.getRawValue();
                        header2string += (header2string != '') ? (',' + h2) : h2;
                    }
                }
                if (colnew < 4) {
                    Ext.Msg.show({ title: 'Message', msg: 'Please Add DSTN.', buttons: { ok: 'Ok'} });
                    return false;
                }

                var detail = ""; //记录Store数据为json格式  
                for (var i = 0; i < Store2.getAllRange().length; i++) {
                    var record = Store2.getAllRange()[i].data;
                    detail += Ext.encode(record);
                    if (i != Store2.getAllRange().length - 1) {
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

                //全局变量复制
                detail1 = detail;
                CCMailListdetail1 = CCMailListdetail;
                header0string1 = header0string;
                header1string1 = header1string;
                header2string1 = header2string;

                RM.SaveAll(a, detail, CCMailListdetail, header0string, header1string, header2string, LabelCurrency.getText(), labelDepartment.getText(), {
                    eventMask: {
                        showMask: true,
                        tartget: "Page"
                    }
                });
            }

        };
        var getRowClass = function (record, index, rowParams, store) {
            if (record.data.Category == 'Total') {
                return "custom-row";
            }
        };
        var GetSum = function () {
            Store2.getAllRange(11, 11)[0].set('TotalP', '');
            Store2.getAllRange(11, 11)[0].set('TotalC', '');
            var jh = Store2.getAllRange(11, 11)[0].data;
            for (var o in jh) {
                if ((o.indexOf('Station_') != -1) && ((o.indexOf('_P') != -1) || (o.indexOf('_C') != -1))) {
                    Store2.getAllRange(11, 11)[0].set(o, '');
                }
            }
            var totalp = 0, totalc = 0;
            for (var i = 0; i < Store2.data.length - 1; i++) {
                Store2.getAllRange(i, i)[0].set('TotalP', '');
                Store2.getAllRange(i, i)[0].set('TotalC', '');
                var item = Store2.getAllRange(i, i)[0].data;
                for (var o in item) {
                    if (o.indexOf('Station_') != -1 && o.indexOf('_P') != -1) {
                        eval('tb=Store2.getAllRange(i,i)[0].data.' + o);
                        tb = tb == '' ? '0' : tb;
                        var tc = parseFloat(tb);
                        var ta = parseFloat(Store2.getAllRange(i, i)[0].data.TotalP == '' ? '0' : Store2.getAllRange(i, i)[0].data.TotalP);
                        var sum = ta + tc == 0 ? "" : (ta + tc).toString();
                        Store2.getAllRange(i, i)[0].set('TotalP', sum);

                        eval('t11=Store2.getAllRange(11,11)[0].data.' + o);
                        t11 = t11 == '' ? '0' : t11;
                        var t11c = parseFloat(t11);
                        var sum11 = t11c + tc == 0 ? "" : (t11c + tc).toString();
                        Store2.getAllRange(11, 11)[0].set(o, sum11);

                        totalp += tc;

                    }
                    if (o.indexOf('Station_') != -1 && o.indexOf('_C') != -1) {
                        eval('tb=Store2.getAllRange(i,i)[0].data.' + o);
                        tb = tb == '' ? '0' : tb;
                        var tc = parseFloat(tb);
                        var ta = parseFloat(Store2.getAllRange(i, i)[0].data.TotalC == '' ? '0' : Store2.getAllRange(i, i)[0].data.TotalC);
                        var sum = ta + tc == 0 ? "" : (ta + tc).toString();
                        Store2.getAllRange(i, i)[0].set('TotalC', sum);

                        eval('t11=Store2.getAllRange(11,11)[0].data.' + o);
                        t11 = t11 == '' ? '0' : t11;
                        var t11c = parseFloat(t11);
                        var sum11 = t11c + tc == 0 ? "" : (t11c + tc).toString();
                        Store2.getAllRange(11, 11)[0].set(o, sum11);

                        totalc += tc;
                    }
                }
            }
            Store2.getAllRange(11, 11)[0].set('TotalP', totalp == 0 ? '' : totalp);
            Store2.getAllRange(11, 11)[0].set('TotalC', totalc == 0 ? '' : totalc);
            Color(null, null, null, null);
        };
        var removecol = function (a, b) {
            var ds = b * 2 + 1;
            GridPanel2.colModel.setHidden(ds, true);
            GridPanel2.colModel.setHidden(ds + 1, true);
            //            GridPanel2.setWidth(GridPanel2.getWidth() - 220);
            var field1 = 'Station_' + b.toString() + '_P';
            var field2 = 'Station_' + b.toString() + '_C';
            Store2.removeField(field1);
            Store2.removeField(field2);
            //计算合计
            GetSum();
        };
        var ClearCol = function (a, b, c) {
            if ($("#CheckBoxOnBehalfItem").attr("checked")!= "checked"&&hdOwner.getValue().toString() == '') {
                if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
                    Ext.Msg.show({ title: 'Message', msg: '请从eLeave选择出差记录.', buttons: { ok: 'Ok'} });
                }
                else {
                    Ext.Msg.show({ title: 'Message', msg: 'Please select eLeave Data.', buttons: { ok: 'Ok'} });
                }
                return false;
            }
            var headercol = GridPanel2.view.headerRows[0].columns.length;
            var col = GridView1.cm.columns.length;
            var colnew = GridView1.cm.columns.length;
            //            GridPanel2.clearContent();
            var header0string = ''; var header1string = ''; var header2string = '';
            //隐藏出差站点为空的列
            //            for (var i = 1; i < headercol - 2; i++) {
            //                if (i % 2 == 1) {
            //                    if (GridPanel2.view.headerRows[0].columns[i].component.getValue() == '') {
            //                        GridPanel2.colModel.setHidden(i, true);
            //                        GridPanel2.colModel.setHidden(i + 1, true);
            //                        var field1 = 'Station_' + ((i - 1) / 2).toString() + '_P';
            //                        var field2 = 'Station_' + ((i - 1) / 2).toString() + '_C';
            //                        Store2.removeField(field1);
            //                        Store2.removeField(field2);
            //                    }
            //                }
            //            }
            for (var i = 1; i < col - 2; i++) {
                if (GridView1.cm.columns[i].hidden) {
                    colnew--;
                }
                else {
                    if (i % 2 == 1) {
                        //                        var h0 = 'NA';
                        //                        var h1 = 'NA';
                        var h0 = GridPanel2.view.headerRows[0].columns[i].component.getValue() == '' ? 'NA' : GridPanel2.view.headerRows[0].columns[i].component.getValue();
                        var h1 = GridPanel2.view.headerRows[1].columns[i].component.getValue() == '' ? 'NA' : GridPanel2.view.headerRows[1].columns[i].component.getValue();
                        header0string += (header0string != '') ? (',' + h0) : h0;
                        header1string += (header1string != '') ? (',' + h1) : h1;
                    }
                    //                    var h2 = 'NA';
                    var h2 = GridPanel2.view.headerRows[2].columns[i].component.getRawValue() == '' ? 'NA' : GridPanel2.view.headerRows[2].columns[i].component.getRawValue();
                    header2string += (header2string != '') ? (',' + h2) : h2;
                }
            }

            var detail = Ext.encode(GridPanel2.getRowsValues()); //记录Store数据为json格式
            //            var storedata;
            //            for (var i = 0; i < Store2.data.items.length; i++) {
            //                storedata.push(Store2.data.items[i].data);
            //            }
            //            hdColCount.setValue(colnew.toString());
            var DSTN = hdDSTN.getValue().toString();
            var date1 = hdLeaveDate1.getValue().toString();
            var date2 = hdLeaveDate2.getValue().toString();
            RM.AddCol(detail, header0string, header1string, header2string, DSTN, date1, date2, {
                eventMask: {
                    showMask: true,
                    tartget: "Page"
                },
                timeout: 300000
            });
            //            GridPanel2.setWidth(184 + 110 * (colnew + 1));
        };
        var Color = function (a, b, c, d) {
            //            var wer = GridPanel2.getWidth();
            //            GridPanel2.setWidth(184 + 105 * (GridView1.cm.columns.length - 1));
            //            GridPanel2.setHeight(386);
            //            $('#GridPanel2 .x-grid3-col-0')[11].style.setProperty('color', '#FF00FF');
            //            $('#GridPanel2 .x-grid3-col-0')[11].style.setProperty('font-weight', 'bold');
            $('#GridPanel2 .x-grid3-col-0')[11].style.fontWeight = 'bold';
            $('#GridPanel2 .x-grid3-col-0')[11].style.color = '#FF00FF';
            for (var i = 1; i < GridView1.cm.columns.length; i++) {
                var inoutname = "#" + $('#GridPanel2 .x-grid3-col-' + i.toString() + ' input')[11].id;
                $(inoutname).css('font-weight', 'bold');
                $(inoutname).css('color', '#FF00FF');
                $(inoutname).attr("readonly", "readonly");
            }
            for (var i = 0; i < 11; i++) {
                var colw = GridView1.cm.columns.length;
                var inoutname = "#" + $('#GridPanel2 .x-grid3-col-' + (colw - 1).toString() + ' input')[i].id;
                $(inoutname).css('font-weight', 'bold');
                $(inoutname).css('color', '#FF00FF');
                $(inoutname).attr("readonly", "readonly");

                var inoutname1 = "#" + $('#GridPanel2 .x-grid3-col-' + (colw - 2).toString() + ' input')[i].id;
                $(inoutname1).css('font-weight', 'bold');
                $(inoutname1).css('color', '#FF00FF');
                $(inoutname1).attr("readonly", "readonly");
            }
        }

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
    var Preview = function () {
        if (hdTravelRequestID.getValue() != '') {
            window.open("Preview.aspx?RequestID=" + hdTravelRequestID.getValue() + "&Type=T");
        }
    };
    var PasseLeaveData = function (command, record, rowIndex) {
        //            dfBdate.setValue(record.data.leaveStart1);
        //            dfEdate.setValue(record.data.leaveEnd1);

        //        labelOwner.setText(record.data.Owner);
        hdOwner.setValue(record.data.Owner);
        hdOwnerID.setValue(record.data.OwnerID);
        //160616 Andy Kang
        cbxPerson.setValue(record.data.OwnerID);
        cbxPerson.disable();


        hdLeaveDate1.setValue(record.data.leaveStart1);
        hdLeaveDate2.setValue(record.data.leaveEnd1);
        hdDSTN.setValue(record.data.Destination);


        labelStation.setText(record.data.Station);
        hdStation.setValue(record.data.Station)
        labelDepartment.setText(record.data.Department);
        LabelCurrency.setText(record.data.Currency);

        hdCurrency.setValue(record.data.Currency); //记录币种
        hdCostCenter.setValue(record.data.CostCenter); //记录成本中心


        var headercol = GridPanel2.view.headerRows[0].columns.length;
        //出差站点不能为空
        for (var i = 0; i < (headercol - 3) / 2; i++) {
            GridPanel2.view.headerRows[2].columns[1 + i * 2].component.setValue(record.data.leaveStart1);
            GridPanel2.view.headerRows[2].columns[2 + i * 2].component.setValue(record.data.leaveEnd1);
            GridPanel2.view.headerRows[0].columns[1 + i * 2].component.setValue(record.data.Destination);
            GridPanel2.view.headerRows[1].columns[1 + i * 2].component.setValue(record.data.CostCenter);
        }
        Window1.hide();
        //            RM.CheckEFlow({
        //                eventMask: {
        //                    showMask: true,
        //                    tartget: "Page"
        //                },
        //                timeout: 300000
        //            });
        //            btnGeteLeave.toggle();
    };
    var GetStatus = function (a, b) {
        Window1.show();
        RM.GetDataFromeLeave({
            eventMask: {
                showMask: true,
                tartget: "customtarget",
                customTarget: GridPanel1
            },
            timeout: 300000
        });
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


        var ipa = "";
        if (window.location.hostname == 'localhost') {
            ipa = "http://" + window.location.host + "/MyClaims.aspx";
        }
        else {
            ipa = "http://" + window.location.host + "/eReimbursement/MyClaims.aspx";
        }
        $('div.gn_center ul.q-menubox li:eq(0) a').attr('href', ipa);

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
        var ipa = "http://219.150.98.243:88/eReimbursement_Old/MyClaims.aspx";
        TreePanel1.getNodeById('d1').setHref(ipa);
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
    //160119 垫付
    var SetOnBehalfData = function () {
        this.triggers[0].show();
        hdOnBehalf.setValue(cbxOnBehalfName.getValue());
        var temps = Store4.query('UserID', cbxOnBehalfName.getValue()).items[0].data;
        LabelDept.setText(temps.Department);
        LabelUnit.setText(temps.Unit);
        LabelCost.setText(temps.CostCenter);
    };
    var linkr = function () {
        //            var linktype = '';
        //            var spotindex = -1;
        //            if ($("#linkTravelReport a")[0] != null) {
        //                if ($("#linkTravelReport a")[0].innerHTML != null) {
        //                    spotindex = $("#linkTravelReport a")[0].innerHTML.indexOf('.');
        //                }
        //            }
        alert('1');
        //            $('#linkTravelReport a')[0].click(function () {
        //                
        //                //                if (spotindex == -1) {
        //                //                    Ext.Msg.alert('Error', 'The file could not be open by Browser,please contact local MIS for help.');
        //                //                }
        //            });
    };

    var CheckOnBehalfItemClick = function () {
        if ($("#CheckBoxOnBehalfItem").attr("checked") == "checked") {//選中
            btnGeteLeave.disable(); cbxOnBehalfName.enable(); btnBudgetView.disable();
        } else {//取消選中
            btnGeteLeave.enable(); cbxOnBehalfName.disable(); btnBudgetView.enable();

            //清除 已經上傳的附件以及錄入的被墊付人信息
            $("#linkTravelReport").text("");
            $("#hdReport").text("");

            $("#hdLeaveDate1").text("");
            $("#hdLeaveDate2").text("");

            cbxOnBehalfName.setValue("");
            $("#hdOnBehalf").text("");
            $("#LabelDept").text("");
            $("#LabelUnit").text("");
            $("#LabelCost").text("");
            StoreBudget.removeAll();

        }
        //alert($("#CheckBoxOnBehalfItem").attr("checked"));
        //Ext.Msg.alert("Error","sdsd");

    };
    </script>
    <style type="text/css">
        .custom-row
        {
            color: #FF00FF;
            font-weight: bold !important;
        }
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
                            <ext:RecordField Name="Owner" />
                            <ext:RecordField Name="OwnerID" />
                            <ext:RecordField Name="Station" />
                            <ext:RecordField Name="Department" />
                            <ext:RecordField Name="Currency" />
                            <ext:RecordField Name="CurrencyBudget" />
                            <ext:RecordField Name="leaveStart" />
                            <ext:RecordField Name="leaveStart1" />
                            <ext:RecordField Name="leaveEnd" />
                            <ext:RecordField Name="leaveEnd1" />
                            <ext:RecordField Name="leaveCount" />
                            <ext:RecordField Name="Destination" />
                            <ext:RecordField Name="CostCenter" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Store ID="Store3" runat="server">
                <Reader>
                    <ext:JsonReader>
                        <Fields>
                            <ext:RecordField Name="DetailID" />
                            <ext:RecordField Name="Tocity" />
                            <ext:RecordField Name="AccountCode" />
                            <ext:RecordField Name="Cur" />
                            <ext:RecordField Name="Pamount" />
                            <ext:RecordField Name="Camount" />
                            <ext:RecordField Name="TSation" />
                            <ext:RecordField Name="Tdate" />
                            <ext:RecordField Name="Tdate0" />
                            <ext:RecordField Name="Type" />
                            <ext:RecordField Name="Department1" />
                            <ext:RecordField Name="DetailCode" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Hidden ID="hdTravelRequestID" runat="server" />
            <ext:Hidden ID="hdTravelRequestNo" runat="server" />
            <ext:Hidden ID="hdCurrency" runat="server" />
            <ext:Hidden ID="hdReport" runat="server" />
            <ext:Hidden ID="hdScanFile" runat="server" />
            <ext:Hidden ID="hdCostCenter" runat="server" />
            <%--<ext:Hidden ID="hdSum" runat="server" />
            <ext:Hidden ID="hdPamountSum" runat="server" />
            <ext:Hidden ID="hdCamountSum" runat="server" />--%>
            <ext:Hidden ID="hdUser" runat="server" />
            <ext:Hidden ID="hdStatus" runat="server" />
            <ext:Hidden ID="hdStation" runat="server" />
            <ext:Hidden ID="hdOwner" runat="server" />
            <ext:Hidden ID="hdOwnerID" runat="server" />
            <ext:Hidden ID="hdLeaveDate1" runat="server" />
            <ext:Hidden ID="hdLeaveDate2" runat="server" />
            <ext:Hidden ID="hdDSTN" runat="server" />
            <ext:Hidden ID="hdOnBehalf" runat="server" />

            <ext:Panel ID="Panel1" runat="server" Height="750" Border="false">
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
                                                    <ext:TreeNode Text="<%$ Resources:LocalText,MyExpenseOld%>" NodeID="d1" Icon="Table" />
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
                                    <%--<ext:ComboBox ID="cbxOwner" runat="server" FieldLabel="<%$ Resources:LocalText,Owner%>"
                                        LabelWidth="60" Width="180" X="10" Y="10" Editable="False" Hidden="true">
                                        <DirectEvents>
                                            <Select OnEvent="ChangePerson" Timeout="300000">
                                                <EventMask ShowMask="true" Target="Page" />
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>--%>
                                    <ext:Label ID="LabelOnBehalfOf" runat="server" Width="160" X="10" Y="8" Text="On Behalf Item:" />
                                    <ext:CheckBox ID="CheckBoxOnBehalfItem" runat="server" Width="160" X="175" Y="6" Disabled="true">
                                        <Listeners>
                                            <Check Fn="CheckOnBehalfItemClick" />
                                        </Listeners>
                                    </ext:CheckBox>
                                    <ext:Label ID="Label1" runat="server" Width="160" X="10" Y="33" Text="My Business Trip @ eLeave:" />
                                    <ext:Button ID="btnGeteLeave" runat="server" Text="Load" Width="60" X="175" Y="30"
                                        Disabled="true">
                                        <Listeners>
                                            <Click Fn="GetStatus" />
                                        </Listeners>
                                    </ext:Button>

                                    <ext:ComboBox ID="cbxPerson" runat="server" FieldLabel="<%$ Resources:LocalText,Owner%>"
                                        LabelWidth="60" Width="200" X="240" Y="30" Editable="false" Disabled="true">
                                        <DirectEvents>
                                            <Select OnEvent="ChangePerson" Timeout="300000">
                                                <EventMask ShowMask="true" CustomTarget="Panel3" Target="CustomTarget" />
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>

                                    <%--<ext:Label ID="LabelText" runat="server" Text="<%$ Resources:LocalText,PersonLabel%>"
                                        X="260" Y="33" />
                                    <ext:Label ID="labelOwner" runat="server" X="320" Y="33" />--%>
                                    <ext:Label ID="Label16" runat="server" Text="<%$ Resources:LocalText,StationLabel%>"
                                        X="450" Y="33" />
                                    <ext:Label ID="labelStation" runat="server" X="510" Y="33" />
                                    <ext:Label ID="Label11" runat="server" Text="<%$ Resources:LocalText,Department%>"
                                        X="590" Y="33" />
                                    <ext:Label ID="labelDepartment" runat="server" X="665" Y="33" />
                                    <ext:Label ID="Label4" runat="server" X="10" Y="63" Text="<%$ Resources:LocalText,Currency%>" />
                                    <ext:Label ID="LabelCurrency" runat="server" X="70" Y="63" />
                                    <ext:Label ID="Label5" runat="server" Width="100" X="145" Y="63" Text="<%$ Resources:LocalText,TravelReport%>" />
                                    <ext:FileUploadField ID="FileUploadField1" runat="server" Icon="Attach" X="235" Y="50"
                                        Width="130" EmptyText="Select file." ButtonText="" />
                                    <ext:Button ID="btnUploadReport" runat="server" X="378" Y="60" Icon="Add">
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
                                    <ext:Button ID="Button1" runat="server" X="403" Y="60" Icon="Delete">
                                        <Listeners>
                                            <Click Handler="linkTravelReport.setText('');hdReport.setValue('');" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:HyperLink ID="linkTravelReport" runat="server" Text="" X="433" Y="63" Target="_blank" />
                                    <%--<ext:ComboBox ID="cbxBudget" runat="server" FieldLabel="<%$ Resources:LocalText,BudgetOrNot%>"
                                        LabelWidth="60" Width="120" X="10" Y="70" Editable="False" SelectedIndex="1"
                                        Hidden="true">
                                        <Items>
                                            <ext:ListItem Text="YES" />
                                            <ext:ListItem Text="NO" />
                                        </Items>
                                        <DirectEvents>
                                            <Select OnEvent="ChangeBudget" Timeout="300000">
                                                <EventMask ShowMask="true" Target="Page" />
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>--%>
                                    <ext:RadioGroup ID="RadioGroup1" runat="server" X="10" Y="90" Width="180" Hidden="true">
                                        <Items>
                                            <ext:Radio ID="Radio1" runat="server" BoxLabel="Budget" Width="60" />
                                            <ext:Radio ID="Radio2" runat="server" BoxLabel="Un-Budget" Width="100" />
                                        </Items>
                                        <DirectEvents>
                                            <Change OnEvent="ChangeBudget" Timeout="300000">
                                                <EventMask ShowMask="true" Target="Page" />
                                            </Change>
                                        </DirectEvents>
                                    </ext:RadioGroup>
                                    <ext:Label ID="Label17" runat="server" Width="100" X="145" Y="93" Text="<%$ Resources:LocalText,ScanFile%>" />
                                    <ext:FileUploadField ID="FileUploadField2" runat="server" Icon="Attach" X="235" Y="58"
                                        Width="130" EmptyText="Select file." ButtonText="" />
                                    <ext:Button ID="btnUploadScanFile" runat="server" X="378" Y="90" Icon="Add">
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
                                    <ext:Button ID="Button2" runat="server" X="403" Y="90" Icon="Delete">
                                        <Listeners>
                                            <Click Handler="linkScanFile.setText('');hdScanFile.setValue('');" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:HyperLink ID="linkScanFile" runat="server" Text="" X="433" Y="93" Target="_blank" />
                                    <ext:TextField ID="txtRemark" runat="server" FieldLabel="<%$ Resources:LocalText,Remark%>"
                                        LabelWidth="60" X="10" Y="120" Anchor="100%">
                                    </ext:TextField>
                                    <ext:FieldSet ID="FieldSet5" runat="server" Title="On behalf of" Layout="AbsoluteLayout"
                                        Height="65" Width="498" X="590" Y="49">
                                        <Items>
                                            <ext:ComboBox ID="cbxOnBehalfName" runat="server" X="0" Y="0" FieldLabel="Person"
                                                LabelWidth="80" Width="220" DisplayField="Name" ValueField="UserID" Disabled="true">
                                                <Store>
                                                    <ext:Store ID="Store4" runat="server">
                                                        <Reader>
                                                            <ext:JsonReader>
                                                                <Fields>
                                                                    <ext:RecordField Name="Name" />
                                                                    <ext:RecordField Name="UserID" />
                                                                    <ext:RecordField Name="CostCenter" />
                                                                    <ext:RecordField Name="Department" />
                                                                    <ext:RecordField Name="Unit" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                    </ext:Store>
                                                </Store>
                                                <Triggers>
                                                    <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                                                </Triggers>
                                                <Listeners>
                                                    <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide(); hdOnBehalf.setValue(null);LabelDept.setText('');LabelUnit.setText('');LabelCost.setText('');}" />
                                                    <Select Fn="SetOnBehalfData" />
                                                    <KeyUp Fn="CheckKey" />
                                                    <Render Handler="this.triggers[1].hide();" />
                                                </Listeners>
                                                <DirectEvents>
                                                    <KeyUp OnEvent="GetOnBehalfUserData" Timeout="300000" Delay="1000">
                                                    </KeyUp>
                                                </DirectEvents>
                                            </ext:ComboBox>
                                            <ext:Label ID="Label2" runat="server" X="240" Y="5" Text="<%$ Resources:LocalText,Department%>" />
                                            <ext:Label ID="LabelDept" runat="server" X="325" Y="5" Text="" />
                                            <ext:Label ID="Label21" runat="server" Text="<%$ Resources:LocalText,StationLabel%>"
                                                X="0" Y="23" />
                                            <ext:Label ID="LabelUnit" runat="server" X="85" Y="23" Text="" />
                                            <ext:Label ID="Label7" runat="server" Text="<%$ Resources:LocalText,CostCenter1%>"
                                                X="240" Y="23" />
                                            <ext:Label ID="LabelCost" runat="server" X="325" Y="23" Text="" />
                                        </Items>
                                    </ext:FieldSet>
                                    <ext:Button ID="btnAddDSTN" runat="server" Text="<%$ Resources:LocalText,NewDSTN%>"
                                        X="10" Y="145" Width="120">
                                        <Listeners>
                                            <Click Fn="ClearCol" Delay="50" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:Button ID="btnBudgetView" runat="server" Text="<%$ Resources:LocalText,BudgetReview%>"
                                        X="140" Y="145" Width="120">
                                        <Listeners>
                                            <Click Fn="LoadBudget" Delay="50" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:GridPanel ID="GridPanel2" runat="server" Title="<%$ Resources:LocalText,ExpenseList%>"
                                        TrackMouseOver="false" Height="400" X="10" Y="170" AutoScroll="true">
                                        <Store>
                                            <ext:Store ID="Store2" runat="server">
                                                <Reader>
                                                    <ext:JsonReader>
                                                        <Fields>
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <Plugins>
                                            <ext:EditableGrid ID="EditableGrid1" runat="server" />
                                        </Plugins>
                                        <SelectionModel>
                                            <ext:CellSelectionModel ID="CellSelectionModel1" runat="server" />
                                        </SelectionModel>
                                        <ColumnModel ID="ColumnModel2" runat="server">
                                            <Columns>
                                            </Columns>
                                        </ColumnModel>
                                        <View>
                                            <ext:GridView ID="GridView1" runat="server">
                                                <%--<GetRowClass Fn="getRowClass" />--%>
                                                <HeaderRows>
                                                    <ext:HeaderRow>
                                                        <Columns>
                                                        </Columns>
                                                    </ext:HeaderRow>
                                                    <ext:HeaderRow>
                                                        <Columns>
                                                        </Columns>
                                                    </ext:HeaderRow>
                                                    <ext:HeaderRow>
                                                        <Columns>
                                                        </Columns>
                                                    </ext:HeaderRow>
                                                </HeaderRows>
                                            </ext:GridView>
                                        </View>
                                        <Listeners>
                                            <ViewReady Fn="Color" Delay="2000" />
                                        </Listeners>
                                    </ext:GridPanel>
                                    <%--<ext:Panel ID="Panel5" runat="server" Height="110" Title="<%$ Resources:LocalText,BudgetYTD%>"
                                        X="10" Y="550" Layout="AbsoluteLayout">
                                        <Items>
                                            <ext:Label ID="lbCOA" runat="server" X="10" Y="5" />
                                            <ext:Label ID="lbStationBG" runat="server" X="10" Y="25" />
                                            <ext:Label ID="lbDepartmentBG" runat="server" X="10" Y="45" />
                                            <ext:Label ID="lbStaffBG" runat="server" X="10" Y="65" />
                                        </Items>
                                    </ext:Panel>--%>
                                    <ext:GridPanel ID="GridPanelBudget" runat="server" TrackMouseOver="false" Height="110"
                                        X="10" Y="570" AutoScroll="true">
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
                                    <ext:Button ID="btnSaveAndSend" runat="server" Text="<%$ Resources:LocalText,SaveApply%>"
                                        X="10" Y="690" Width="120" Disabled="true">
                                        <Listeners>
                                            <Click Handler="SaveAll('ND');" Delay="50" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:Button ID="btnSaveDraft" runat="server" Text="<%$ Resources:LocalText,SaveAsDraft%>"
                                        X="140" Y="690" Width="120">
                                        <Listeners>
                                            <Click Handler="SaveAll('D');" Delay="50" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:Button ID="btnExport" runat="server" Text="<%$ Resources:LocalText,Export%>"
                                        X="270" Y="690" Width="120" Icon="Report" Disabled="true">
                                        <Listeners>
                                            <Click Fn="Preview" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:Button ID="btnCC" runat="server" Text="<%$ Resources:LocalText,CC%>" X="400"
                                        Y="690" Width="120" Icon="Mail">
                                        <Listeners>
                                            <Click Fn="GetCCList" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:Button ID="Button4" runat="server" Text="Transfer to eFMS-Invoice Voucher Entry" X="530"
                                        Y="690" Width="120" >
                                        
                                    </ext:Button>
                                    <ext:Label ID="labelInfo" runat="server" Text="" X="20" Y="720">
                                    </ext:Label>
                                </Items>
                            </ext:Panel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
            <ext:Window ID="Window1" runat="server" Title="My Business Trip @ eLeave" Hidden="true"
                Layout="FitLayout" Width="850" Height="500" Resizable="False" AutoScroll="true">
                <%--<Listeners>
                    <Show Handler=" var pos = btnGeteLeave.getPosition();
                        pos[0] += -683;
                        pos[1] += 29;
                        this.setPosition(pos);" />
                </Listeners>--%>
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
                                <ext:Column Header="Owner" Width="150" DataIndex="Owner" />
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
    </form>
</body>
</html>
