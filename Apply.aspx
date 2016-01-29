<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Apply.aspx.cs" Inherits="eReimbursement.Apply" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>通用费用申请单</title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
    <link href="Styles/StyleSheet1.css" rel="stylesheet" type="text/css" />
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
    var Preview = function () {
        if (hdTravelRequestID.getValue() != '') {
            window.open("Preview.aspx?RequestID=" + hdTravelRequestID.getValue() + "&Type=G");
        }
    };
    var a1 = ""; //记录申请类型:草稿或者正式
    var detail1 = ""; //记录费用明细
    var CCMailListdetail1 = ""; //记录邮件抄送人列表
    var SaveAllNew = function (budget) {
        RM.SaveAll1(a1, detail1, CCMailListdetail1, budget, {
            eventMask: {
                showMask: true,
                tartget: "Page"
            },
            timeout: 300000
        });
    };
    var SaveAll = function (a) {
        //            if (!Radio1.getValue()&&!Radio2.getValue()) {
        //                Ext.Msg.show({ title: 'Message', msg: 'Please select Budget type.', buttons: { ok: 'Ok'} });
        //                return false;
        //            }
        a1 = a;
        if (a == 'ND') {
            //正式申请
            if (Store1.getAllRange().length < 1) {
                Ext.Msg.show({ title: 'Message', msg: 'No expense to apply.', buttons: { ok: 'Ok'} });
                return false;
            }
            else {
                Ext.Msg.confirm('Message', 'Are you sure?', function (btn, text) {
                    if (btn == 'yes') {
                        var detail = ""; //记录Store数据为json格式
                        for (var i = 0; i < Store1.getAllRange().length; i++) {
                            var record = Store1.getAllRange()[i].data;
                            detail += Ext.encode(record);
                            if (i != Store1.getAllRange().length - 1) {
                                detail += ',';
                            }
                            Store1.getAllRange(i, i)[0].set('Draft', '1'); //用来更新删除按钮状态
                            //                                Store1.getAt(i).set('Draft', '1');//用来更新删除按钮状态
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

                        RM.SaveAll(a, detail, CCMailListdetail, {
                            eventMask: {
                                showMask: true,
                                tartget: "Page"
                            },
                            timeout: 300000
                        });
                    }
                });
            }
            //                Store1.load();
        }
        else {
            //草稿  
            var detail = ""; //记录Store数据为json格式  
            for (var i = 0; i < Store1.getAllRange().length; i++) {
                var record = Store1.getAllRange()[i].data;
                detail += Ext.encode(record);
                if (i != Store1.getAllRange().length - 1) {
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

            RM.SaveAll(a, detail, CCMailListdetail, {
                eventMask: {
                    showMask: true,
                    tartget: "Page"
                },
                timeout: 300000
            });
        }

    };
    var CheckStore = function () {
        if (Store1.getAllRange().length < 1) {
            Ext.Msg.show({ title: 'Message', msg: 'No expense to apply.', buttons: { ok: 'Ok'} });
            return false;
        }
        return true;
    };
    var prepare = function (grid, toolbar, rowIndex, record) {
        //            var firstButton = toolbar.items.get(1);
        //            if (record.data.Draft != '0') {
        //                firstButton.setDisabled(true);
        //                firstButton.setTooltip("Disabled");
        //            }
        //you can return false to cancel toolbar for this record
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
            GridPanel1.setTitle('Total sheets: ' + no.toString() + ', Sum: ' + GetNumber(Amount.toString()) + '.');
        }

        hdSum.setValue(Amount.toString());
    };
    //新增费用明细
    //160113,垫付人识别
    var AddDetail = function (type) {
        var userid = cbxOnBehalfName.getValue() == "" ? cbxPerson.getValue() : cbxOnBehalfName.getValue();
        if (type == 'T') {
            Window1.loadContent({ url: 'ApplyT.aspx?UserID=' + userid, mode: 'iframe' });
            Window1.show();
        }
        else if (type == 'C') {
            Window1.loadContent({ url: 'ApplyC.aspx?UserID=' + userid, mode: 'iframe' });
            Window1.show();
        }
        else if (type == 'O') {
            Window1.loadContent({ url: 'ApplyO.aspx?UserID=' + userid, mode: 'iframe' });
            Window1.show();
        }
        else if (type == 'E') {
            Window1.loadContent({ url: 'ApplyE.aspx?UserID=' + userid, mode: 'iframe' });
            Window1.show();
        }
    };
    //160113,垫付人识别
    var rowdbclick = function (control, rowindex, button) {
        if (Store1) {
            var userid = cbxOnBehalfName.getValue() == "" ? cbxPerson.getValue() : cbxOnBehalfName.getValue();
            if (Store1.data.items[rowindex].data.Type == 'T') {

                //Draft=2,则表示非复制而来,已经完成的申请,传递RequestID到子窗体以便读取Budget_Complete数据
                if (Store1.data.items[rowindex].data.Draft.toString() == '2') {
                    Window1.loadContent({ url: 'ApplyT.aspx?Draft=' + Store1.data.items[rowindex].data.Draft + '&UserID=' + userid + '&RequestID=' + hdTravelRequestID.getValue().toString(), mode: 'iframe' });
                }
                else {
                    Window1.loadContent({ url: 'ApplyT.aspx?Draft=' + Store1.data.items[rowindex].data.Draft + '&UserID=' + userid, mode: 'iframe' });
                }
                Window1.show();
            }
            else if (Store1.data.items[rowindex].data.Type == 'C') {
                //                    Window1.loadContent({ url: 'ApplyC.aspx?Cur=' + Store1.data.items[rowindex].data.Cur + '&Draft=' + Store1.data.items[rowindex].data.Draft + '&UserID=' + cbxPerson.getValue(), mode: 'iframe' });
                //                    Window1.show();
                //Draft=2,则表示非复制而来,已经完成的申请,传递RequestID到子窗体以便读取Budget_Complete数据
                if (Store1.data.items[rowindex].data.Draft.toString() == '2') {
                    Window1.loadContent({ url: 'ApplyC.aspx?Draft=' + Store1.data.items[rowindex].data.Draft + '&UserID=' + userid + '&RequestID=' + hdTravelRequestID.getValue().toString(), mode: 'iframe' });
                }
                else {
                    Window1.loadContent({ url: 'ApplyC.aspx?Draft=' + Store1.data.items[rowindex].data.Draft + '&UserID=' + userid, mode: 'iframe' });
                }
                Window1.show();
            }
            else if (Store1.data.items[rowindex].data.Type == 'O') {
                //                    Window1.loadContent({ url: 'ApplyO.aspx?Cur=' + Store1.data.items[rowindex].data.Cur + '&Draft=' + Store1.data.items[rowindex].data.Draft + '&UserID=' + cbxPerson.getValue(), mode: 'iframe' });
                //                    Window1.show();
                //Draft=2,则表示非复制而来,已经完成的申请,传递RequestID到子窗体以便读取Budget_Complete数据
                if (Store1.data.items[rowindex].data.Draft.toString() == '2') {
                    Window1.loadContent({ url: 'ApplyO.aspx?Draft=' + Store1.data.items[rowindex].data.Draft + '&UserID=' + userid + '&RequestID=' + hdTravelRequestID.getValue().toString(), mode: 'iframe' });
                }
                else {
                    Window1.loadContent({ url: 'ApplyO.aspx?Draft=' + Store1.data.items[rowindex].data.Draft + '&UserID=' + userid, mode: 'iframe' });
                }
                Window1.show();
            }
            else {
                //                    Window1.loadContent({ url: 'ApplyE.aspx?Cur=' + Store1.data.items[rowindex].data.Cur + '&Draft=' + Store1.data.items[rowindex].data.Draft + '&UserID=' + cbxPerson.getValue(), mode: 'iframe' });
                //                    Window1.show();
                //Draft=2,则表示非复制而来,已经完成的申请,传递RequestID到子窗体以便读取Budget_Complete数据
                if (Store1.data.items[rowindex].data.Draft.toString() == '2') {
                    Window1.loadContent({ url: 'ApplyE.aspx?Draft=' + Store1.data.items[rowindex].data.Draft + '&UserID=' + userid + '&RequestID=' + hdTravelRequestID.getValue().toString(), mode: 'iframe' });
                }
                else {
                    Window1.loadContent({ url: 'ApplyE.aspx?Draft=' + Store1.data.items[rowindex].data.Draft + '&UserID=' + userid, mode: 'iframe' });
                }
                Window1.show();
            }
        }
        SetWindowTitle(null, null, null);
    };
    var RowCommand = function (command, record, rowIndex) {
        var userid = cbxOnBehalfName.getValue() == "" ? cbxPerson.getValue() : cbxOnBehalfName.getValue();
        if (command == 'Edit') {
            if (record.data.Type == 'T') {
                Window1.loadContent({ url: 'ApplyT.aspx?Draft=' + record.data.Draft + '&UserID=' + userid, mode: 'iframe' });
                Window1.show();
            }
            else if (record.data.Type == 'C') {
                Window1.loadContent({ url: 'ApplyC.aspx?Draft=' + record.data.Draft + '&UserID=' + userid, mode: 'iframe' });
                Window1.show();
            }
            else if (record.data.Type == 'O') {
                Window1.loadContent({ url: 'ApplyO.aspx?Draft=' + record.data.Draft + '&UserID=' + userid, mode: 'iframe' });
                Window1.show();
            }
            else {
                Window1.loadContent({ url: 'ApplyE.aspx?Draft=' + record.data.Draft + '&UserID=' + userid, mode: 'iframe' });
                Window1.show();
            }
            SetWindowTitle(null, null, null);
        }
        else if (command == 'Delete') {
            Store1.removeAt(rowIndex);
            //                Store1.load();
            GetSum();
        }
    };
    var AddSubStore = function (type) {
        var subStore = Window1_IFrame.Store1;
        var maxdetailid = 0;
        for (var i = 0; i < Store1.getAllRange().length; i++) {
            var newrecord = new subStore.recordType();
            var record = Store1.getAllRange()[i].data;
            if (record.Type == type) {
                if (parseInt(record.DetailID) >= maxdetailid) {
                    maxdetailid = record.DetailID;
                }
                newrecord.data.DetailID = record.DetailID;
                newrecord.data.MainID = record.MainID;
                newrecord.data.Draft = record.Draft;
                newrecord.data.Type = type;
                newrecord.data.AccountCode = record.AccountCode;
                newrecord.data.AccountDes = record.AccountDes;
                newrecord.data.Cur = record.Cur;
                newrecord.data.Amount = record.Amount;
                newrecord.data.TSation = record.TSation;
                newrecord.data.Tdate = record.Tdate;
                newrecord.data.Fcity = record.Fcity;
                newrecord.data.Tcity = record.Tcity;
                newrecord.data.Attach = record.Attach;
                newrecord.data.EType = record.EType;
                newrecord.data.ETypeCode = record.ETypeCode;
                newrecord.data.Ecompany = record.Ecompany;
                newrecord.data.Eperson = record.Eperson;
                newrecord.data.Epurpos = record.Epurpos;
                newrecord.data.COAName = record.COAName;
                newrecord.data.EffectTime = record.EffectTime;
                newrecord.data.PaymentType = record.PaymentType;
                newrecord.data.PaymentDate = record.PaymentDate;
                newrecord.data.Vendor = record.Vendor;
                newrecord.data.Budget = record.Budget;
                newrecord.data.BudgetCurrent = record.BudgetCurrent;
                subStore.add(newrecord);
            }
        }
        Window1_IFrame.hdDetailIDSeed.setValue(maxdetailid);
        return true;
        //根据Budget状态设置弹出窗口中的Budget类型
        //            var wr = Radio1.getValue() ? "YES" : "NO";
        //            Window1_IFrame.hdBudget.setValue(wr);
    };
    var SetWindowTitle = function (a, b, c) {
        var window1url = Window1.autoLoad.url;
        if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
            if (window1url.indexOf('ApplyE.aspx') == 0) {
                Window1.setTitle('交际费');
            }
            else if (window1url.indexOf('ApplyT.aspx') == 0) {
                Window1.setTitle('交通费');
            }
            else if (window1url.indexOf('ApplyC.aspx') == 0) {
                Window1.setTitle('通讯费');
            }
            else if (window1url.indexOf('ApplyO.aspx') == 0) {
                Window1.setTitle('其他费用');
            }
        }
        else {
            if (window1url.indexOf('ApplyE.aspx') == 0) {
                Window1.setTitle('Entertainment');
            }
            else if (window1url.indexOf('ApplyT.aspx') == 0) {
                Window1.setTitle('Transportation');
            }
            else if (window1url.indexOf('ApplyC.aspx') == 0) {
                Window1.setTitle('Communication');
            }
            else if (window1url.indexOf('ApplyO.aspx') == 0) {
                Window1.setTitle('Other Expense');
            }
        }
    };

    var RenderTree = function () {
        if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {
            $('div#TreePanel1 div.x-tree-node-el a span').css('font-size', '14px');
        }
        else {
            $('div#TreePanel1 div.x-tree-node-el a span').css('font-size', '12px');
        }
        TreePanel1.getNodeById('a2').select(true);
        var ipa = "http://219.150.98.243:88/eReimbursement_Old/MyClaims.aspx";
        TreePanel1.getNodeById('d1').setHref(ipa);
    };
    $(document).ready(function () {
        //中英双语设置
        if (getCookie('lang') != undefined && getCookie('lang').toLowerCase() == 'zh-cn') {

            document.title = '通用费用申请单';
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
            document.title = 'General Expense Apply';
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
    var CheckLang = function () {
        alert(navigator.language);
    };

    var ConfirmSave = function () {
        Ext.Msg.confirm("提示", "是否此报销所有明细均已输入完毕，现在提交申请?", function (btn, text) {
            if (btn == 'yes') {
                return true;
            }
        });
    };
    var template = '<span style="color:{0};">{1}</span>';

    var change = function (value) {
        return String.format(template, (value > 200) ? "green" : "red", value);
    };

    var pctChange = function (value) {
        return String.format(template, (value > 200) ? "green" : "red", value + "%");
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
        newmaillist = newmaillist.length > 0 ? newmaillist.substring(0, newmaillist.length - 6) : '';
        $("#ToolTipCC .x-tip-body").html(newmaillist);
    };
    var SetOnBehalfData = function () {
        this.triggers[0].show();
        hdOnBehalf.setValue(cbxOnBehalfName.getValue());
        var temps = Store2.query('UserID', cbxOnBehalfName.getValue()).items[0].data;
        LabelDept.setText(temps.Department);
        LabelUnit.setText(temps.Unit);
        LabelCost.setText(temps.CostCenter);
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
            <ext:Hidden ID="hdCopy" runat="server" />
            <ext:Hidden ID="hdTravelRequestID" runat="server" />
            <ext:Hidden ID="hdTravelRequestNo" runat="server" />
            <ext:Hidden ID="hdDetailID" runat="server" />
            <ext:Hidden ID="hdReport" runat="server" />
            <ext:Hidden ID="hdScanFile" runat="server" />
            <ext:Hidden ID="hdTempDetailID" runat="server" />
            <ext:Hidden ID="hdSum" runat="server" />
            <ext:Hidden ID="hdSubStatus" runat="server" />
            <ext:Hidden ID="hdCur" runat="server" />
            <ext:Hidden ID="hdUser" runat="server" />
            <ext:Hidden ID="hdTravelRequestID2" runat="server" />
            <ext:Hidden ID="hdOnBehalf" runat="server" />
            <ext:Hidden ID="hdCurrency" runat="server" />
            <%--传递给子页面以判断按钮状态 0:允许修改,允许上传;1:不允许修改,允许上传;2:全不允许 --%>
            <ext:Store ID="Store1" runat="server">
                <Reader>
                    <ext:JsonReader>
                        <Fields>
                            <ext:RecordField Name="DetailID" Type="Int" />
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
                            <ext:RecordField Name="SubType" />
                            <ext:RecordField Name="COAName" />
                            <ext:RecordField Name="Department1" />
                            <ext:RecordField Name="PaymentType" />
                            <ext:RecordField Name="PaymentDate" />
                            <ext:RecordField Name="Vendor" />
                            <ext:RecordField Name="Budget" Type="Boolean" />
                            <ext:RecordField Name="BudgetCurrent" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Panel ID="Panel1" runat="server" Height="680" Border="false">
                <Items>
                    <ext:BorderLayout ID="BorderLayout1" runat="server">
                        <West Collapsible="true" Split="true" CollapseMode="Mini">
                            <ext:Panel ID="Panel2" runat="server" Width="195" Layout="FitLayout">
                                <Items>
                                    <ext:TreePanel ID="TreePanel1" runat="server" Lines="false" UseArrows="true" RootVisible="false"
                                        Border="false" AutoScroll="true">
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
                            <ext:Panel ID="Panel3" runat="server" Title='<%$ Resources:LocalText,GeneralExpenseApply%>'
                                Height="600" Padding="10" MinHeight="300" AutoScroll="true" Layout="AbsoluteLayout">
                                <Items>
                                    <ext:ComboBox ID="cbxPerson" runat="server" FieldLabel="<%$ Resources:LocalText,Owner%>"
                                        LabelWidth="60" Width="220" X="10" Y="10" Editable="False" Disabled="true">
                                        <DirectEvents>
                                            <Select OnEvent="ChangePerson" Timeout="300000">
                                                <EventMask ShowMask="true" CustomTarget="Panel3" Target="CustomTarget" />
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:FieldSet ID="FieldSet5" runat="server" Title="On behalf of" Layout="AbsoluteLayout"
                                        Height="63" Width="600" X="440" Y="3">
                                        <Items>
                                            <ext:ComboBox ID="cbxOnBehalfName" runat="server" X="0" Y="0" FieldLabel="Person"
                                                LabelWidth="80" Width="220" DisplayField="Name" ValueField="UserID" Disabled="true">
                                                <Store>
                                                    <ext:Store ID="Store2" runat="server">
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
                                            <ext:Label ID="Label2" runat="server" X="240" Y="0" Text="<%$ Resources:LocalText,Department%>" />
                                            <ext:Label ID="LabelDept" runat="server" X="325" Y="0" Text="" />
                                            <ext:Label ID="Label4" runat="server" Text="<%$ Resources:LocalText,StationLabel%>"
                                                X="0" Y="23" />
                                            <ext:Label ID="LabelUnit" runat="server" X="85" Y="23" Text="" />
                                            <ext:Label ID="Label7" runat="server" Text="<%$ Resources:LocalText,CostCenter1%>"
                                                X="240" Y="23" />
                                            <ext:Label ID="LabelCost" runat="server" X="325" Y="23" Text="" />
                                        </Items>
                                    </ext:FieldSet>
                                    <ext:Label ID="Label5" runat="server" X="240" Y="13" Text="<%$ Resources:LocalText,Department%>" />
                                    <ext:Label ID="LabelDepartment" runat="server" X="325" Y="13" />
                                    <ext:ComboBox ID="cbxBudget" runat="server" FieldLabel="<%$ Resources:LocalText,BudgetOrNot%>"
                                        LabelWidth="60" Width="120" X="445" Y="10" Editable="False" SelectedIndex="0"
                                        ReadOnly="true" Hidden="true">
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
                                    <ext:RadioGroup ID="RadioGroup1" runat="server" X="445" Y="10" Width="180" Hidden="true">
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
                                    <ext:Label ID="Label16" runat="server" Text="<%$ Resources:LocalText,StationLabel%>"
                                        X="10" Y="43" />
                                    <ext:Label ID="LabelStation" runat="server" X="80" Y="43" />
                                    <ext:Label ID="Label1" runat="server" Text="<%$ Resources:LocalText,Monthly%>" X="240"
                                        Y="43" />
                                    <ext:Label ID="LabelMonth" runat="server" X="300" Y="43" />
                                    <ext:TextField ID="txtRemark" runat="server" FieldLabel="<%$ Resources:LocalText,Remark%>"
                                        X="10" Y="70" Width="300" LabelWidth="50" Anchor="100%" />
                                    <ext:Label ID="LabelBudgetLink" runat="server" X="425" Y="13" />
                                    <ext:HyperLink ID="HLUnBudgetRequest" runat="server" Text="" X="575" Y="13" Target="_blank" />
                                    <ext:Label ID="LabelBudgetLink1" runat="server" X="425" Y="43" />
                                    <ext:HyperLink ID="HLUnBudgetRequest1" runat="server" Text="" X="575" Y="43" Target="_blank" />
                                    <ext:Button ID="btnSaveAndSend" runat="server" Text="<%$ Resources:LocalText,SaveApply%>"
                                        X="10" Y="592" Width="120" Disabled="true">
                                        <Listeners>
                                            <Click Handler="SaveAll('ND');" />
                                        </Listeners>
                                        <%--<DirectEvents>
                                            <Click OnEvent="Save" Success="#{GridPanel1}.submitData();" Timeout="300000">
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
                                        X="140" Y="592" Width="120">
                                        <Listeners>
                                            <Click Handler="SaveAll('D');" />
                                        </Listeners>
                                        <%--<DirectEvents>
                                            <Click OnEvent="Save" Success="#{GridPanel1}.submitData();" Timeout="300000">
                                                <ExtraParams>
                                                    <ext:Parameter Name="type" Value="D" Mode="Value">
                                                    </ext:Parameter>
                                                </ExtraParams>
                                                <EventMask ShowMask="true" CustomTarget="Panel3" Target="CustomTarget" />
                                            </Click>
                                        </DirectEvents>--%>
                                    </ext:Button>
                                    <ext:Button ID="btnExport" runat="server" Text="<%$ Resources:LocalText,Export%>"
                                        X="270" Y="592" Width="120" Icon="Report" Disabled="true">
                                        <Listeners>
                                            <Click Fn="Preview" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:Button ID="btnCC" runat="server" Text="<%$ Resources:LocalText,CC%>" X="400"
                                        Y="592" Width="120" Icon="Mail">
                                        <Listeners>
                                            <Click Fn="GetCCList" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:Label ID="labelInfo" runat="server" Text="" X="10" Y="622">
                                    </ext:Label>
                                    <ext:Panel ID="Panel4" runat="server" X="10" Y="100" Height="482" Layout="AbsoluteLayout"
                                        Title="<%$ Resources:LocalText,DetailTitle%>" Padding="10">
                                        <Items>
                                            <ext:Button ID="btnE" runat="server" Text="<%$ Resources:LocalText,Entertainment%>"
                                                X="10" Y="10" Width="150" Icon="Add">
                                                <Listeners>
                                                    <Click Handler="AddDetail('E');" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="btnT" runat="server" Text="<%$ Resources:LocalText,Transportation%>"
                                                X="170" Y="10" Width="150" Icon="Add">
                                                <Listeners>
                                                    <Click Handler="AddDetail('T');" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="btnC" runat="server" Text="<%$ Resources:LocalText,Communication%>"
                                                X="330" Y="10" Width="150" Icon="Add">
                                                <Listeners>
                                                    <Click Handler="AddDetail('C');" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="btnO" runat="server" Text="<%$ Resources:LocalText,OtherExpense%>"
                                                X="490" Y="10" Width="150" Icon="Add">
                                                <Listeners>
                                                    <Click Handler="AddDetail('O');" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" TrackMouseOver="false"
                                                Height="295" X="10" Y="40" AutoScroll="true">
                                                <ColumnModel ID="ColumnModel1" runat="server">
                                                    <Columns>
                                                        <ext:CommandColumn Width="30" ButtonAlign="Center" Sortable="False">
                                                            <Commands>
                                                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit" ToolTip-Text="<%$ Resources:LocalText,View%>" />
                                                            </Commands>
                                                            <PrepareToolbar Fn="prepare" />
                                                        </ext:CommandColumn>
                                                        <ext:Column Header="<%$ Resources:LocalText,ExpenseItem%>" Width="100" DataIndex="SubType"
                                                            Sortable="False" />
                                                        <%--<ext:Column Header="<%$ Resources:LocalText,BudgetOrNot%>" Width="70" DataIndex="BudgetCurrent" Sortable="False"/>--%>
                                                        <ext:Column Header="<%$ Resources:LocalText,Amount%>" Width="90" DataIndex="Amount"
                                                            Sortable="False">
                                                            <Renderer Fn="GetNumber" />
                                                        </ext:Column>
                                                        <%--<ext:Column Header="<%$ Resources:LocalText,CostCenter%>" Width="80" DataIndex="TSation" Sortable="False"/>--%>
                                                        <ext:Column Header="<%$ Resources:LocalText,COARemark%>" Width="100" DataIndex="AccountDes"
                                                            Sortable="False" />
                                                        <ext:DateColumn Header="<%$ Resources:LocalText,Date%>" Width="80" DataIndex="Tdate"
                                                            Format="yyyy/MM/dd" Sortable="False" />
                                                        <ext:Column Header="<%$ Resources:LocalText,EffectPeriod%>" Width="60" DataIndex="EffectTime"
                                                            Sortable="False" />
                                                        <ext:Column Header="<%$ Resources:LocalText,File%>" Width="140" DataIndex="Attach"
                                                            Sortable="False">
                                                            <Renderer Fn="attachlink" />
                                                        </ext:Column>
                                                        <%--<ext:Column Header="<%$ Resources:LocalText,ExpenseType%>" Width="200" DataIndex="COAName" Sortable="False"/>
                                                        <ext:CheckColumn Header="<%$ Resources:LocalText,UnBudget%>" Width="80" DataIndex="Budget" Sortable="False"/>--%>
                                                    </Columns>
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                                    </ext:RowSelectionModel>
                                                </SelectionModel>
                                                <Listeners>
                                                    <Command Fn="RowCommand" />
                                                    <RowDblClick Fn="rowdbclick" />
                                                </Listeners>
                                            </ext:GridPanel>
                                            <ext:GridPanel ID="GridPanelBudget" runat="server" TrackMouseOver="false" Height="110"
                                                X="10" Y="335" AutoScroll="true">
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
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
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
                    <ext:Panel ID="Panel5" runat="server" Height="220" Layout="FitLayout" X="5" Y="35">
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
            <ext:Window ID="Window1" runat="server" Title="交通费" Hidden="true" Width="894" Height="543"
                Modal="True" Resizable="False" AutoScroll="true" Maximizable="True" DefaultRenderTo="Form">
                <AutoLoad ShowMask="True">
                </AutoLoad>
                <Listeners>
                    <Show Fn="SetWindowTitle" />
                </Listeners>
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
            <%--<ext:Toolbar ID="Toolbar1" runat="server" Width="160" Flat="true">
                <Items>
                    <ext:Button ID="Button7" runat="server" Icon="FlagUs" Text="English" Width="70" />
                    <ext:Button ID="Button8" runat="server" Icon="FlagCn" Text="中文" Width="70" />
                </Items>
                <Listeners>
                    <Show Handler=" var left=$('.gn_person').offset().left-340;var top=$('.gn_person').height()-740;
                        this.setPosition(left,top);" />
                </Listeners>
            </ext:Toolbar>
            <ext:Window ID="Window2" runat="server" Hidden="true" Width="160" Height="50" Closable="false"
                Modal="false" Resizable="False" DefaultRenderTo="Form">
                <Listeners>
                    <Show Handler=" var left=$('.gn_person').offset().left;var top=$('.gn_person').height();
                        this.setPosition(left,top);" />
                </Listeners>
            </ext:Window>--%>
            <div id="Div_lang" style="position: absolute; z-index: 9013; visibility: hidden;
                left: -10000px; top: -10000px; width: 200px; display: block;">
                <ext:Toolbar ID="Toolbar2" runat="server" Width="160" Flat="true">
                    <Items>
                        <%--<ext:Button ID="Button9" runat="server" Icon="FlagUs" Text="English" Width="70" PostBackUrl="Apply.aspx?lang=en-US"
                            AutoPostBack="true">
                            <Listeners>
                                <Click Fn="SetCookie" />
                            </Listeners>
                        </ext:Button>
                        <ext:Button ID="Button10" runat="server" Icon="FlagCn" Text="中文" Width="70" PostBackUrl="Apply.aspx?lang=zh-CN"
                            AutoPostBack="true">
                            <Listeners>
                                <Click Fn="SetCookie" />
                            </Listeners>
                        </ext:Button>--%>
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
