<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="mailtest.aspx.cs" Inherits="eReimbursement.backup.mailtest" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server">
    </ext:ResourceManager>
    <div>
        <table style='border-collapse: collapse;' width="2000px">
            <tr>
                <td>
                    12
                </td>
                <td>
                    &nbsp;
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td>
                    &nbsp;
                </td>
                <td nowrap="nowrap">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td>
                    &nbsp;
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
        </table>
    </div>
    <div>
        <ext:Button ID="Button1" runat="server" Text="Submit">
            <DirectEvents>
                <Click OnEvent="SendMail">
                </Click>
            </DirectEvents>
        </ext:Button>
    </div>
    </form>
</body>
</html>
