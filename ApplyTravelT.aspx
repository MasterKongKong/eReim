<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplyTravelT.aspx.cs" Inherits="eReimbursement.ApplyTravelT" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>差旅费</title>
    <script src="Scripts/jquery-1.9.0.min.js" type="text/javascript"></script>
    <script src="Scripts/pagesetting.js" type="text/javascript"></script>
    <link href="Styles/mainpage2.css" rel="stylesheet" type="text/css" />
    <link href="Styles/mainpage.css" rel="stylesheet" type="text/css" />
    <link href="Styles/StyleSheet1.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/PageJS.js" type="text/javascript"></script>
    <link href="Styles/StyleSheetFlow.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        var setFlow = function (a) {
            $('#divFlow').html(a);
        };
        var GetStation = function (a) {
            var temps = StoreCOACenter.query('cityID', a).items;
            Ext.each(temps, function (temp) {
                Label4.setText(temp.data.Station);
            });
        };
        //        var AddApprove = function (a, b) {
        //            Ext.Msg.prompt('Message', 'Please input .');
        //            Ext.Msg.show({ title: '提示', msg: "成功", buttons: { ok: "Ok" }, fn: function (btn) { parent.Window1.hide(); } });
        //        };
        var showResultText = function (btn, text) {
            Ext.Msg.notify("Button Click", "You clicked the " + btn + 'button and entered the text "' + text + '".');
            if (btn == 'cancelbutton') {
                return false;
            }
        };
        var GetNumber = function (value) {
            return Ext.util.Format.number(value.replace(/[,]/g, ''), '0,0.00')
        };
        var Preview = function () {
            if (hdTravelRequestID.getValue() != '') {
                window.open("Preview.aspx?RequestID=" + hdTravelRequestID.getValue() + "&Type=T");
            }
        };
    </script>
    <style type="text/css">
        .behalf
        {
        	color:Blue;
        }
    </style>
</head>
<body>
    <form id="Form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <ext:Hidden ID="hdTravelRequestID" runat="server" />
    <ext:Store ID="Store1" runat="server">
        <Reader>
            <ext:JsonReader>
                <Fields>
                    <ext:RecordField Name="ID" />
                    <ext:RecordField Name="Tocity" />
                    <ext:RecordField Name="AccountName" />
                    <ext:RecordField Name="AccountCode" />
                    <ext:RecordField Name="AccountDes" />
                    <ext:RecordField Name="Cur" />
                    <ext:RecordField Name="Pamount" />
                    <ext:RecordField Name="Camount" />
                    <ext:RecordField Name="TSation" />
                    <ext:RecordField Name="Tdate" />
                    <ext:RecordField Name="StationBudget" />
                    <ext:RecordField Name="DepartmentBudget" />
                    <ext:RecordField Name="PersonBudget" />
                    <ext:RecordField Name="COAName" />
                    <ext:RecordField Name="Department1" />
                    <ext:RecordField Name="PersonYTD" />
                    <ext:RecordField Name="DepartmentYTD" />
                    <ext:RecordField Name="StationYTD" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Panel ID="Panel3" runat="server" Height="785" Width="900" Border="false" Padding="10"
        MinHeight="300" AutoScroll="true" Layout="AbsoluteLayout">
        <Items>
            <ext:Label ID="Label1" runat="server" Text="<%$ Resources:LocalText,PersonLabel%>"
                X="10" Y="10" />
            <ext:Label ID="LabelPerson" runat="server" X="80" Y="10" />
            <ext:Label ID="Label3" runat="server" Text="<%$ Resources:LocalText,StationLabel%>"
                X="190" Y="10" />
            <ext:Label ID="LabelStation" runat="server" X="230" Y="10" />
            <ext:Label ID="Label5" runat="server" Text="<%$ Resources:LocalText,Department%>"
                X="320" Y="10" />
            <ext:Label ID="LabelDepartment" runat="server" X="390" Y="10" />
            <ext:Label ID="Label6" runat="server" Text="<%$ Resources:LocalText,Currency%>"
                X="500" Y="10" />
            <ext:Label ID="LabelCur" runat="server" X="560" Y="10" />
            <ext:Label ID="Label7" runat="server" Text="<%$ Resources:LocalText,PeriodLabel%>"
                X="10" Y="35" />
            <ext:Label ID="LabelBdate" runat="server" X="80" Y="35" />
            <ext:Label ID="Label9" runat="server" Text="<%$ Resources:LocalText,ToLabel%>" X="160"
                Y="35" />
            <ext:Label ID="LabelEdate" runat="server" X="180" Y="35" />
            <ext:Label ID="Label11" runat="server" Text="<%$ Resources:LocalText,TravelReport%>"
                X="270" Y="35" Hidden="true"/>
            <ext:HyperLink ID="LinkReport" runat="server" X="270" Y="35" Target="_blank" />
            <ext:Label ID="Label12" runat="server" Text="<%$ Resources:LocalText,ScanFile%>"
                X="500" Y="35" Hidden="true"/>
            <ext:HyperLink ID="LinkScanFile" runat="server" X="480" Y="35" Target="_blank" />
            <ext:Label ID="Label14" runat="server" Text="<%$ Resources:LocalText,TotalAmountLabel%>"
                X="10" Y="60" />
            <ext:Label ID="LabelSum" runat="server" X="80" Y="60" />
            <ext:Label ID="Label16" runat="server" Text="<%$ Resources:LocalText,PersonalExpenseTotalLabel%>"
                X="170" Y="60" />
            <ext:Label ID="LabelPSum" runat="server" X="330" Y="60" />
            <ext:Label ID="Label18" runat="server" Text="<%$ Resources:LocalText,CompanyExpenseTotalLabel%>"
                X="400" Y="60" />
            <ext:Label ID="LabelCSum" runat="server" X="550" Y="60" />
            <ext:Label ID="Label2" runat="server" Text="<%$ Resources:LocalText,RemarkLabel%>"
                X="10" Y="85" />
            <ext:Label ID="LabelRemark" runat="server" X="80" Y="85" />

            <ext:Label ID="Label10" runat="server" Text="On behalf of:"
                X="620" Y="10" Cls="behalf"/>
            <ext:Label ID="LabelBehalfPersonName" runat="server" X="700" Y="10" Cls="behalf"/>
            <ext:Label ID="Label13" runat="server" Text="Cost Center:"
                X="620" Y="35" Cls="behalf"/>
            <ext:Label ID="LabelBehalfCost" runat="server" X="700" Y="35" Cls="behalf"/>

            <ext:Panel ID="Panel5" runat="server" X="10" Y="110" Layout="FitLayout" Height="85"
                Border="false">
                <Items>
                    <ext:Container ID="Container2" runat="server" AutoScroll="true">
                        <Content>
                            <div id="divFlow">
                            </div>
                        </Content>
                    </ext:Container>
                </Items>
            </ext:Panel>
            <ext:GridPanel ID="GridPanel2" runat="server" Title="<%$ Resources:LocalText,ExpenseDetail%>"
                TrackMouseOver="false" Height="380" X="10" Y="195" AutoScroll="true">
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
                <ColumnModel ID="ColumnModel1" runat="server">
                    <Columns>
                    </Columns>
                </ColumnModel>
                <View>
                    <ext:GridView ID="GridView1" runat="server">
                        
                    </ext:GridView>
                </View>
            </ext:GridPanel>
            <ext:GridPanel ID="GridPanelBudget" runat="server" TrackMouseOver="false" Height="110" X="10" Y="575" AutoScroll="true">
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
            <ext:TextField ID="txtRemark" runat="server" X="10" Y="695" FieldLabel="<%$ Resources:LocalText,ApprovalRemark%>"
                Anchor="100%" LabelWidth="80" />
            <ext:Button ID="Button1" runat="server" Text="<%$ Resources:LocalText,Approve%>"
                X="10" Y="725" Width="80">
                <DirectEvents>
                    <Click OnEvent="Save" Timeout="300000">
                        <ExtraParams>
                            <ext:Parameter Name="type" Value="2" Mode="Value">
                            </ext:Parameter>
                        </ExtraParams>
                        <EventMask ShowMask="true" CustomTarget="Panel3" Target="CustomTarget" />
                        <Confirmation ConfirmRequest="true" Title="Message" Message="Are you sure?" />
                    </Click>
                </DirectEvents>
            </ext:Button>
            <ext:Button ID="Button2" runat="server" Text="<%$ Resources:LocalText,Reject%>" X="100"
                Y="725" Width="80">
                <Listeners>
                    <Click Handler="if(txtRemark.getValue()==''){Ext.Msg.show({ title: 'Message', msg: 'Please input remark.', buttons: Ext.Msg.OK, icon: Ext.Msg.WARNING });
                    return false;}" />
                </Listeners>
                <DirectEvents>
                    <Click OnEvent="Save" Timeout="300000">
                        <ExtraParams>
                            <ext:Parameter Name="type" Value="3" Mode="Value">
                            </ext:Parameter>
                        </ExtraParams>
                        <EventMask ShowMask="true" CustomTarget="Panel3" Target="CustomTarget" />
                        <Confirmation ConfirmRequest="true" Title="Message" Message="Are you sure?" />
                    </Click>
                </DirectEvents>
            </ext:Button>
            <ext:Button ID="Button4" runat="server" Text="<%$ Resources:LocalText,Return%>" X="190"
                Y="725" Width="80">
                <Listeners>
                    <Click Handler="parent.Window1.hide();" />
                </Listeners>
            </ext:Button>
            <ext:Button ID="btnExport" runat="server" Text="<%$ Resources:LocalText,Export%>"
                X="280" Y="725" Width="80" Icon="Report">
                <Listeners>
                    <Click Fn="Preview" />
                </Listeners>
            </ext:Button>
            <ext:Button ID="Button3" runat="server" Text="<%$ Resources:LocalText,Additional%>"
                X="370" Y="725" Width="80">
                <Listeners>
                    <Click Handler="if(cbxCOACenter.getValue()==''){Ext.Msg.show({ title: 'Message', msg: 'Please input Additional approve person.', buttons: Ext.Msg.OK, icon: Ext.Msg.WARNING });return false;}" />
                </Listeners>
                <DirectEvents>
                    <Click OnEvent="AddApp" Timeout="300000">
                        <ExtraParams>
                            <ext:Parameter Name="type1" Value="cbxCOACenter.getValue()" Mode="Raw" />
                            <ext:Parameter Name="type2" Value="cbxCOACenter.getRawValue()" Mode="Raw" />
                        </ExtraParams>
                        <EventMask ShowMask="true" CustomTarget="Panel3" Target="CustomTarget" />
                        <Confirmation ConfirmRequest="true" Title="Message" Message="Are you sure?" />
                    </Click>
                </DirectEvents>
            </ext:Button>
            <ext:ComboBox ID="cbxCOACenter" runat="server" FieldLabel="<%$ Resources:LocalText,Additional%>"
                LabelWidth="60" X="460" Y="725" Width="240" DisplayField="cityCode" ValueField="cityID">
                <Store>
                    <ext:Store ID="StoreCOACenter" runat="server">
                        <Reader>
                            <ext:JsonReader>
                                <Fields>
                                    <ext:RecordField Name="cityID" />
                                    <ext:RecordField Name="cityCode" />
                                    <ext:RecordField Name="Station" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <Triggers>
                    <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                </Triggers>
                <Listeners>
                    <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();StoreCOACenter.removeAll();Label4.setText('');}" />
                    <Select Handler="this.triggers[0].show();GetStation(this.getValue());return false;" />
                    <KeyUp Fn="CheckKey" />
                    <Render Handler="this.triggers[1].hide();" />
                </Listeners>
                <DirectEvents>
                    <KeyUp OnEvent="GetStation" Timeout="300000" Delay="1000">
                    </KeyUp>
                </DirectEvents>
            </ext:ComboBox>
            <ext:Label ID="Label8" runat="server" X="10" Y="758" Text="<%$ Resources:LocalText,RemarkHistory%>"/>
            <ext:Label ID="LabelRemarkFlow" runat="server" X="110" Y="758" />
            <ext:Label ID="Label4" runat="server" X="10" Y="788" />
        </Items>
    </ext:Panel>
    <%--<ext:ToolTip ID="ToolTip1" runat="server" Target="Button3" Anchor="bottom" AutoHide="false"
        Height="80" Closable="true" Width="210" Padding="5" Title="<a style='font-family: 12px tahoma,arial,helvetica,sans-serif; font-size: 12px; color: #000000'>请选择</a>">
        <Items>
            <ext:ComboBox ID="ComboBox3" runat="server" Width="180" FieldLabel="加批人" LabelWidth="40">
                <Items>
                    <ext:ListItem Text="Hughson Huang" Value="A0360" />
                    <ext:ListItem Text="Andy Kang" Value="A2232" />
                </Items>
            </ext:ComboBox>
        </Items>
    </ext:ToolTip>--%>
    </form>
</body>
</html>
