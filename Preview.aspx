<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Preview.aspx.cs" Inherits="eReimbursement.Preview" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Preview</title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/PageJS.js" type="text/javascript"></script>

    <script type="text/javascript">
        //        $(document).ready(function () {
        //            var clientwidth = document.documentElement.clientHeight.toString();
        //            $("#ReportViewer1").css("height", clientwidth);
        //        });
    </script>
    <style type="text/css">
        html, body, form
        {
            width: 100%;
            height: 100%;
            margin: 0;
            padding: 0;
            overflow: auto;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div style="word-break:break-all;">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt"
            InteractiveDeviceInfos="(集合)" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt"
            Width="100%" Height="100%" PageCountMode="Actual" ShowRefreshButton="False" ZoomMode="PageWidth" ShowBackButton="true" SizeToReportContent="True">
        </rsweb:ReportViewer>
    </div>
    </form>
</body>
</html>
