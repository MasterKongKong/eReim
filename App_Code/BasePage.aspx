<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BasePage.aspx.cs" Inherits="eReimbursement.App_Code.BasePage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        //        var SetCookie = function (name, value)//两个参数，一个是cookie的名子，一个是值
        //        {
        //            var Days = 30; //此 cookie 将被保存 30 天

        //            var exp = new Date();    //new Date("December 31, 9998");

        //            exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);

        //            document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();

        //        };
        //        var addCookie = function (objName, objValue, objHours) {      //添加cookie

        //            var str = objName + "=" + escape(objValue);

        //            if (objHours > 0) {                               //为时不设定过期时间，浏览器关闭时cookie自动消失

        //                var date = new Date();

        //                var ms = objHours * 3600 * 1000;

        //                date.setTime(date.getTime() + ms);

        //                str += "; expires=" + date.toGMTString();

        //            }

        //            document.cookie = str;

        //        }
        //        var getCookie = function (objName) {//获取指定名称的cookie的值

        //            var arrStr = document.cookie.split("; ");

        //            for (var i = 0; i < arrStr.length; i++) {

        //                var temp = arrStr[i].split("=");

        //                if (temp[0] == objName) return unescape(temp[1]);

        //            }

        //        };
        //        var browserlang = navigator.language;
        //        $(document).ready(function () {
        //            if (getCookie('lang1') == null) {
        //                addCookie('lang1', browserlang, 0);
        //            }
        //        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    </div>
    </form>
</body>
</html>
