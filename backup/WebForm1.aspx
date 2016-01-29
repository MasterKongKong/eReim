<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="eReimbursement.backup.WebForm1" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Simple Array Grid - Ext.NET Examples</title>
    <link href="../../../../resources/css/examples.css" rel="stylesheet" type="text/css" />    

    <script type="text/javascript">
        var template = '<span style="color:{0};">{1}</span>';

        var change = function (value) {
            return String.format(template, (value > 0) ? "green" : "red", value);
        };

        var pctChange = function (value) {
            return String.format(template, (value > 0) ? "green" : "red", value + "%");
        };
    </script>
</head>
<body>
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    
    <h1>Simple Array Grid</h1>
    
    <ext:GridPanel 
        ID="GridPanel1"
        runat="server" 
        StripeRows="true"
        Title="Array Grid" 
        TrackMouseOver="true"
        Width="600" 
        Height="350"
        AutoExpandColumn="company">
        <Store>
            <ext:Store ID="Store1" runat="server">
                <Reader>
                    <ext:ArrayReader>
                        <Fields>
                            <ext:RecordField Name="company" />
                            <ext:RecordField Name="price" Type="Float" />
                            <ext:RecordField Name="change" Type="Float" />
                            <ext:RecordField Name="pctChange" Type="Float" />
                            <ext:RecordField Name="lastChange" Type="Date" DateFormat="M/d hh:mmtt" />
                            <ext:RecordField Name="Budget" Type="Boolean" />
                        </Fields>
                    </ext:ArrayReader>
                </Reader>
            </ext:Store>
        </Store>
        <ColumnModel ID="ColumnModel1" runat="server">
            <Columns>
                <ext:Column ColumnID="Company" Header="Company" DataIndex="company" />
                <ext:Column Header="Price" DataIndex="price">                  
                    <Renderer Format="UsMoney" />
                </ext:Column>
                <ext:Column ColumnID="Change" Header="Change" DataIndex="change">
                    <Renderer Fn="change" />
                </ext:Column>
                <ext:Column Header="Change" DataIndex="pctChange">
                    <Renderer Fn="pctChange" />
                </ext:Column>
                <ext:CheckColumn Header="Budget" DataIndex="Budget" Editable="true" />
            </Columns>
        </ColumnModel>
    </ext:GridPanel>
</body>
</html>
