<%@ Page Language="C#" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<script runat="server">
    
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Ext.Net Example</title>
</head>
<body>
    <form id="Form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <div>
        <div style='font-size: small;'>
            Dear Andy Kang,</div>
        <br />
        <div style='font-size: small;'>
            The following eReimbursement application: Seek For Your Approval.</div>
        <br />
        <br />
        <div style='font-size: small;'>
            No#:ZJDTSN14030043G(UnBudgeted)</div>
        <div style='font-size: small;'>
            Applicant:Andy Kang</div>
        <div style='font-size: small;'>
            Unit:ZJDTSN</div>
        <div style='font-size: small;'>
            Department:MIS/IT</div>
        <br />
        <div>
            <table>
                <tr>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        Expense Item
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        Currency
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        current
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        Budget Year
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        Personal<br />
                        Used
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        Personal<br />
                        Budget
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        %<br />
                        (Current+Used) /<br />
                        Budget
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        Department<br />
                        Used
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        Department<br />
                        Budget
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        %<br />
                        (Current+Used) /<br />
                        Budget
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        Unit<br />
                        Used
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        Unit<br />
                        Budget
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        %<br />
                        (Current+Used) /<br />
                        Budget
                    </td>
                </tr>
                <tr>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        Communication
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        CNY
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        2.00
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        2014
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        422.00
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        3,000.00
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        14.13 %
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        243,903.00
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        6,000.00
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show; color: red;' width='110px' align='right'>
                        4,065.08 %
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        243,903.00
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        243,480.00
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show; color: red;' width='110px' align='right'>
                        100.17 %
                    </td>
                </tr>
                <tr>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        Sub Total
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        CNY
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        2.00
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        &nbsp;
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        422.00
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        3,000.00
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        243,903.00
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        6,000.00
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        &nbsp;
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        243,903.00
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        243,480.00
                    </td>
                    <td style='border: silver 1px ridge; font-size: small; background-color: #FFFFFF;
                        empty-cells: show;' width='110px' align='right'>
                        123
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <br />
    </form>
</body>
</html>
