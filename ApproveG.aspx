<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApproveG.aspx.cs" Inherits="eReimbursement.ApproveG" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <title>通用费用审批</title>
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
        var linktem = '<a href="./Upload/{0}" target="_blank">{1}</a>';
        var attachlink = function (value) {
            return String.format(linktem, value, value);
        };
        var GetNumber = function (value) {
            return Ext.util.Format.number(value.replace(/[,]/g, ''), '0,0.00')
        };
        var Preview = function () {
            if (hdTravelRequestID.getValue() != '') {
                window.open("Preview.aspx?RequestID=" + hdTravelRequestID.getValue() + "&Type=G");
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
                    <ext:RecordField Name="DetailID" />
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
                    <ext:RecordField Name="StationBudget" />
                    <ext:RecordField Name="DepartmentBudget" />
                    <ext:RecordField Name="PersonBudget" />
                    <ext:RecordField Name="SubType" />
                    <ext:RecordField Name="COAName" />
                    <ext:RecordField Name="EffectTime" />
                    <ext:RecordField Name="Department1" />
                    <ext:RecordField Name="PersonBudgetOneYear" />

                    <ext:RecordField Name="Vendor" />
                    <ext:RecordField Name="PaymentType1" />
                    <ext:RecordField Name="PaymentDate" />

                    <ext:RecordField Name="StationYTD" />
                    <ext:RecordField Name="DepartmentYTD" />
                    <ext:RecordField Name="PersonYTD" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Panel ID="Panel3" runat="server" Height="640" Width="900" Border="false" Padding="10"
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
            <ext:Label ID="Label7" runat="server" Text="<%$ Resources:LocalText,Currency%>"
                X="320" Y="35" />
            <ext:Label ID="LabelCur" runat="server" X="390" Y="35" />
            <ext:Label ID="Label4" runat="server" Text="<%$ Resources:LocalText,Monthly%>" X="10"
                Y="35" />
            <ext:Label ID="LabelMonth" runat="server" X="80" Y="35" />
            <ext:Label ID="Label14" runat="server" Text="<%$ Resources:LocalText,TotalAmountLabel%>" X="190"
                Y="35" />
            <ext:Label ID="LabelSum" runat="server" X="230" Y="35" />
            <ext:Label ID="Label2" runat="server" Text="<%$ Resources:LocalText,RemarkLabel%>"
                X="10" Y="60" />
            <ext:Label ID="LabelRemark" runat="server" X="80" Y="60" />

            <ext:Label ID="Label8" runat="server" Text="On behalf of:"
                X="570" Y="10" Cls="behalf"/>
            <ext:Label ID="LabelBehalfPersonName" runat="server" X="650" Y="10" Cls="behalf"/>
            <ext:Label ID="Label9" runat="server" Text="Cost Center:"
                X="570" Y="35" Cls="behalf"/>
            <ext:Label ID="LabelBehalfCost" runat="server" X="650" Y="35" Cls="behalf"/>

            <ext:Panel ID="Panel5" runat="server" X="10" Y="85" Layout="FitLayout" Height="90"
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
            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" Title="<%$ Resources:LocalText,ExpenseDetail%>"
                TrackMouseOver="false" Height="260" X="10" Y="170" AutoScroll="true">
                <ColumnModel ID="ColumnModel1" runat="server">
                    <Columns>
                        <ext:RowNumbererColumn Width="30" />
                        <ext:Column Header="<%$ Resources:LocalText,ExpenseItem%>" Width="110" DataIndex="SubType" />
                        
                        <ext:Column Header="<%$ Resources:LocalText,Amount%>" Width="100" DataIndex="Amount">
                            <Renderer Fn="GetNumber" />
                        </ext:Column>
                        <%--<ext:Column Header="<%$ Resources:LocalText,CostCenter%>" Width="90" DataIndex="TSation" />--%>
                        <%--<ext:Column Header="<%$ Resources:LocalText,Department1%>" Width="120" DataIndex="Department1" />--%>
                        <ext:Column Header="<%$ Resources:LocalText,COARemark%>" Width="250" DataIndex="AccountDes" />
                        <ext:DateColumn Header="<%$ Resources:LocalText,Date%>" Width="80" DataIndex="Tdate"
                            Format="yyyy/MM/dd" />
                        <ext:Column Header="<%$ Resources:LocalText,EffectPeriod%>" Width="60" DataIndex="EffectTime" />
                        <ext:Column Header="<%$ Resources:LocalText,File%>" Width="140" DataIndex="Attach">
                            <Renderer Fn="attachlink" />
                        </ext:Column>
                        <%--<ext:Column Header="<%$ Resources:LocalText,ExpenseType%>" Width="160" DataIndex="COAName" />--%>
                        <%--<ext:Column Header="<%$ Resources:LocalText,StationBudget%>" Width="180" DataIndex="StationBudget" />
                        <ext:Column Header="<%$ Resources:LocalText,DepartmentBudget%>" Width="180" DataIndex="DepartmentBudget" />
                        <ext:Column Header="<%$ Resources:LocalText,PersonBudget%>" Width="180" DataIndex="PersonBudget" />
                        <ext:Column Header="<%$ Resources:LocalText,StationBudgetYTD%>" Width="180" DataIndex="StationYTD" />
                        <ext:Column Header="<%$ Resources:LocalText,DepartmentBudgetYTD%>" Width="180" DataIndex="DepartmentYTD" />
                        <ext:Column Header="<%$ Resources:LocalText,PersonBudgetYTD%>" Width="180" DataIndex="PersonYTD" />--%>
                        <ext:Column Header="<%$ Resources:LocalText,CustomerType%>" Width="100" DataIndex="EType" />
                        <ext:Column Header="<%$ Resources:LocalText,Guest%>" Width="160" DataIndex="Eperson" />
                        <ext:Column Header="<%$ Resources:LocalText,Customer%>" Width="160" DataIndex="Ecompany" />
                        <ext:Column Header="<%$ Resources:LocalText,Purpose%>" Width="260" DataIndex="Epurpos" />

                        <%--<ext:Column Header="<%$ Resources:LocalText,Vendor%>" Width="150" DataIndex="Vendor" />
                        <ext:Column Header="<%$ Resources:LocalText,PaymentType%>" Width="150" DataIndex="PaymentType1" />
                        <ext:Column Header="<%$ Resources:LocalText,PaymentDate%>" Width="150" DataIndex="PaymentDate" />--%>
                    </Columns>
                </ColumnModel>
                
            </ext:GridPanel>
            <ext:GridPanel ID="GridPanelBudget" runat="server" TrackMouseOver="false" Height="110" X="10" Y="430" AutoScroll="true">
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
            <ext:TextField ID="txtRemark" runat="server" X="10" Y="550" FieldLabel="<%$ Resources:LocalText,ApprovalRemark%>"
                Anchor="100%" LabelWidth="80" />
            <ext:Button ID="Button1" runat="server" Text="<%$ Resources:LocalText,Approve%>"
                X="10" Y="580" Width="80">
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
                Y="580" Width="80">
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
                Y="580" Width="80">
                <Listeners>
                    <Click Handler="parent.Window1.hide();" />
                </Listeners>
            </ext:Button>
            <ext:Button ID="btnExport" runat="server" Text="<%$ Resources:LocalText,Export%>"
                X="280" Y="580" Width="80" Icon="Report">
                <Listeners>
                    <Click Fn="Preview" />
                </Listeners>
            </ext:Button>
            <ext:Button ID="Button3" runat="server" Text="<%$ Resources:LocalText,Additional%>"
                X="370" Y="580" Width="80">
                <Listeners>
                    <Click Handler="if(cbxCOACenter.getValue()==''){Ext.Msg.show({ title: 'Message', msg: 'Please input Additional approve person.', buttons: Ext.Msg.OK, icon: Ext.Msg.WARNING });ToolTip1.show();return false;}" />
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
                LabelWidth="60" X="460" Y="580" Width="240" DisplayField="cityCode" ValueField="cityID">
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
                    <Select Handler="this.triggers[0].show();GetStation(this.getValue());" />
                    <KeyUp Fn="CheckKey" />
                    <Render Handler="this.triggers[1].hide();" />
                </Listeners>
                <DirectEvents>
                    <KeyUp OnEvent="GetStation" Timeout="300000" Delay="1000">
                    </KeyUp>
                </DirectEvents>
            </ext:ComboBox>
            <ext:Label ID="Label11" runat="server" X="10" Y="613" Text="<%$ Resources:LocalText,RemarkHistory%>"/>
            <ext:Label ID="LabelRemarkFlow" runat="server" X="110" Y="613" />
            <ext:Label ID="Label6" runat="server" X="10" Y="643" />
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
