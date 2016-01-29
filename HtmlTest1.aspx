<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HtmlTest1.aspx.cs" Inherits="eReimbursement.HtmlTest1" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" 
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>GridPanel with EditableGrid Plugin - Ext.NET Examples</title>
    <link href="../../../../resources/css/examples.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        var EnterNext = function (field, e) {
            var keycode = e.getKey();

            //            if ((48 <= keycode && keycode <= 57) || keycode == 0 || keycode == 45 || keycode == 8 || keycode == 9 || (96 <= keycode && keycode <= 105)) {
            //                return;
            //            } else {
            //                alert("Inpt only Number.");
            //                window.event.returnValue = false;
            //            }
            if (keycode == 13) {
                TextField2.focus();
            }
        }
        //        $(function () {
        //            $('input:text:first').focus();
        //            var $inp = $('input:text');
        //            $inp.bind('keydown', function (e) {
        //                var key = e.which;
        //                if (key == 13) {
        //                    e.preventDefault();
        //                    var nxtIdx = $inp.index(this) + 1;
        //                    $(":input:text:eq(" + nxtIdx + ")").focus();
        //                }
        //            });
        //        });
    </script>
</head>
<body>
    <form id="Form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <ext:TextField ID="TextField1" runat="server" EnableKeyEvents="true" SelectOnFocus="true">
        <Listeners>
            <KeyDown Fn="EnterNext" />
        </Listeners>
    </ext:TextField>
    <ext:TextField ID="TextField2" runat="server" SelectOnFocus="true">
    </ext:TextField>
    <ext:TextField ID="TextField3" runat="server">
    </ext:TextField>
    </form>
</body>
</html>
