var GetNumber = function (value) {
    if (value == '--' || value == '') {
        return '--';
    }
    else {
        return (value == '' || value == null) ? '' : Ext.util.Format.number(value.replace(/[,]/g, ''), '0,0.00');
    }
    
};
var template = '<span style="color:{0};">{1}</span>';
var GetNumberPercent = function (value) {
    if (value == '--' || value == '') {
        return '--';
    }
    else {
        return String.format(template, (value > 100) ? "red" : "black", value + "%");
    }

};
var linktem = '<a href="./Upload/{0}" target="_blank">{1}</a>';
var attachlink = function (value) {
    if (value != undefined && value != '') {
        return String.format(linktem, value, value);
    }
    else {
        return '';
    }
};

var Request = {
    QueryString: function (item) {
        var svalue = location.search.match(new RegExp("[\?\&]" + item + "=([^\&]*)(\&?)", "i"));
        return svalue ? svalue[1] : svalue;
    }
};

var ShowFunction = function (btn) {
    window.location.href = './Approve.aspx';
};

var CheckKey = function (a, b) {
    if (b.button == 36 || b.button == 37 || b.button == 38 || b.button == 39 || b.button == 12 || b.button == 8)
    { return false; }
    if (a.getRawValue() == '') {
        a.triggers[0].hide();
        return false;
    }
    else {
        a.triggers[0].show();
    }
    if (a.getRawValue().length < 3)
    { return false; }
};
var addCookie = function (objName, objValue, objHours) {      //添加cookie

    var str = objName + "=" + escape(objValue);

    if (objHours > 0) {                               //为时不设定过期时间，浏览器关闭时cookie自动消失

        var date = new Date();

        var ms = objHours * 3600 * 1000;

        date.setTime(date.getTime() + ms);

        str += "; expires=" + date.toGMTString();

    }

    document.cookie = str;
}
var SetCookie = function (lang, b)//两个参数，一个是cookie的名子，一个是值
{
    var lang = lang.getText() == 'English' ? 'en-us' : 'zh-cn';
    var Days = 3000; //此 cookie 将被保存 30 天

    var exp = new Date();    //new Date("December 31, 9998");

    exp.setTime(exp.getTime() + Days * 24 * 3600 * 1000);
    delCookie("lang");
    document.cookie = 'lang' + "=" + escape(lang) + ";expires=" + exp.toGMTString();

    window.location.reload();
};
var getCookie = function (objName) {//获取指定名称的cookie的值

    var arrStr = document.cookie.split("; ");

    for (var i = 0; i < arrStr.length; i++) {

        var temp = arrStr[i].split("=");

        if (temp[0] == objName) return unescape(temp[1]);

    }

};
var delCookie=function (name) {//为了删除指定名称的cookie，可以将其过期时间设定为一个过去的时间
    var date = new Date();
    date.setTime(date.getTime() - 10000);
    document.cookie = name + "=a; expires=" + date.toGMTString();
};


