<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HTML2.aspx.cs" Inherits="eReimbursement.HTML2" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $('input:text:first').focus();
            var $inp = $('input:text');
            $inp.bind('keydown', function (e) {
                var key = e.which;
                if (key == 13) {
                    e.preventDefault();
                    var nxtIdx = $inp.index(this) + 1;
                    $(":input:text:eq(" + nxtIdx + ")").focus();
                }
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server">
    </ext:ResourceManager>
    <div>
        <asp:TextBox ID="txt1" runat="server" /><br />
        <asp:TextBox ID="txt2" runat="server" /><br />
        <asp:TextBox ID="txt3" runat="server" /><br />
        <asp:TextBox ID="txt4" runat="server" /><br />
    </div>
    </form>
</body>
</html>
