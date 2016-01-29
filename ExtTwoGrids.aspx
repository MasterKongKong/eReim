<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExtTwoGrids.aspx.cs" Inherits="eReimbursement.ExtTwoGrids" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Ext.NET Examples</title>
    
    <script type="text/javascript">
        var CountrySelector = {
            add: function (source, destination) {
                source = source || GridPanel1;
                destination = destination || GridPanel2;
                if (source.hasSelection()) {
                    var records = source.selModel.getSelections();
                    source.deleteSelected();
                    Ext.each(records, function (record) {
                        destination.store.addSorted(record);
                    });
                }
            },
            addAll: function (source, destination) {
                source = source || GridPanel1;
                destination = destination || GridPanel2;
                var records = source.store.getRange();
                source.store.removeAll();
                Ext.each(records, function (record) {
                    destination.store.addSorted(record);
                });
            },
            addByName: function (name) {
                if (!Ext.isEmpty(name)) {
                    var result = Store1.query("Name", name);
                    if (!Ext.isEmpty(result.items)) {
                        GridPanel1.store.remove(result.items[0]);
                        GridPanel2.store.add(result.items[0]);
                    }
                }
            },
            addByNames: function (name) {
                for (var i = 0; i < name.length; i++) {
                    this.addByName(name[i]);
                }
            },
            remove: function (source, destination) {
                this.add(destination, source);
            },
            removeAll: function (source, destination) {
                this.addAll(destination, source);
            }
        };
    </script>
    <link href="../../../../resources/css/examples.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="Form1" runat="server">
        <ext:ResourceManager runat="server" />
        
        <ext:Label ID="Label1" runat="server" />
        
         
        <ext:Store runat="server" ID="Store1">
            <SortInfo Field="Name" Direction="ASC" />            
            <Reader>
                <ext:ArrayReader>
                    <Fields>
                        <ext:RecordField Name="Name" />                        
                    </Fields>
                </ext:ArrayReader>
            </Reader>
        </ext:Store>
        
        <ext:Store runat="server" ID="Store2" OnSubmitData="SubmitData">
            <SortInfo Field="Name" Direction="ASC" />
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="Name" />                        
                    </Fields>
                </ext:JsonReader>
            </Reader>         
        </ext:Store>
        
        <ext:Window 
            runat="server" 
            ShowOnLoad="true"
            Closable="false"
            Height="553"
            Width="700"
            Icon="WorldAdd"
            Title="Country Selector"
            Padding="5"
            BodyBorder="false">
            <TopBar>
                <ext:Toolbar runat="server">
                    <Items>
                        <ext:Button runat="server" Text="Options">
                            <Menu>
                                <ext:Menu runat="server">
                                    <Items>
                                        <ext:MenuItem runat="server" Text="Select All">
                                            <Listeners>
                                                <Click Handler="CountrySelector.addAll(GridPanel1, GridPanel2);" />
                                            </Listeners>
                                        </ext:MenuItem>
                                        <ext:MenuItem runat="server" Text="UnSelect All">
                                            <Listeners>
                                                <Click Handler="CountrySelector.removeAll(GridPanel1, GridPanel2);" />
                                            </Listeners>
                                        </ext:MenuItem>
                                        <ext:MenuItem runat="server" Text="Regions">
                                            <Menu>
                                                <ext:Menu runat="server">
                                                    <Items>
                                                        <ext:MenuItem runat="server" Text="Asia">
                                                            <Listeners>
                                                                <Click Handler="CountrySelector.addByNames(['China', 'Japan', 'Taiwan', 'South Korea']);" />
                                                            </Listeners>
                                                        </ext:MenuItem>
                                                        <ext:MenuItem runat="server" Text="Europe">
                                                            <Listeners>
                                                                <Click Handler="CountrySelector.addByNames(['United Kingdom', 'France', 'Germany', 'Spain', 'Switzerland', 'Italy', 'Austria', 'Belgium']);" />
                                                            </Listeners>
                                                        </ext:MenuItem>
                                                        <ext:MenuItem runat="server" Text="North America">
                                                            <Listeners>
                                                                <Click Handler="CountrySelector.addByNames(['Canada', 'United States', 'Mexico']);" />
                                                            </Listeners>
                                                        </ext:MenuItem>
                                                    </Items>
                                                </ext:Menu>
                                            </Menu>
                                            <Listeners>
                                                <Click Handler="CountrySelector.removeAll(GridPanel1, GridPanel2);" />
                                            </Listeners>
                                        </ext:MenuItem>
                                    </Items>
                                </ext:Menu>
                            </Menu>
                        </ext:Button>
                    </Items>
                </ext:Toolbar>
            </TopBar>
            <Items>
                <ext:ColumnLayout runat="server" FitHeight="true">
                    <Columns>
                        <ext:LayoutColumn ColumnWidth="0.5">
                           <ext:GridPanel 
                                ID="GridPanel1" 
                                runat="server" 
                                EnableDragDrop="true"
                                AutoExpandColumn="Country"
                                StoreID="Store1">
                                <ColumnModel runat="server">
	                                <Columns>
                                        <ext:Column ColumnID="Country" Header="Available Countries" DataIndex="Name" />                   
	                                </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel runat="server" />
                                </SelectionModel> 
                                <Plugins>
                                    <ext:GridFilters runat="server" Local="true">
                                        <Filters>
                                            <ext:StringFilter DataIndex="Name" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                            </ext:GridPanel>
                        </ext:LayoutColumn>
                        <ext:LayoutColumn>
                            <ext:Panel 
                                runat="server" 
                                Width="35" 
                                BodyStyle="background-color: transparent;" 
                                Border="false" 
                                Layout="Anchor">
                                <Items>
                                    <ext:Panel runat="server" Border="false" BodyStyle="background-color: transparent;" AnchorVertical="70%" />
                                    <ext:Panel runat="server" Border="false" BodyStyle="background-color: transparent;" Padding="5">
                                        <Items>
                                            <ext:Button runat="server" Icon="ResultsetNext" StyleSpec="margin-bottom:2px;">
                                                <Listeners>
                                                    <Click Handler="CountrySelector.add();" />
                                                </Listeners>
                                                <ToolTips>
                                                    <ext:ToolTip runat="server" Title="Add" Html="Add Selected Rows" />
                                                </ToolTips>
                                            </ext:Button>
                                            <ext:Button runat="server" Icon="ResultsetLast" StyleSpec="margin-bottom:2px;">
                                                <Listeners>
                                                    <Click Handler="CountrySelector.addAll();" />
                                                </Listeners>
                                                <ToolTips>
                                                    <ext:ToolTip runat="server" Title="Add all" Html="Add All Rows" />
                                                </ToolTips>
                                            </ext:Button>
                                            <ext:Button runat="server" Icon="ResultsetPrevious" StyleSpec="margin-bottom:2px;">
                                                <Listeners>
                                                    <Click Handler="CountrySelector.remove(GridPanel1, GridPanel2);" />
                                                </Listeners>
                                                <ToolTips>
                                                    <ext:ToolTip runat="server" Title="Remove" Html="Remove Selected Rows" />
                                                </ToolTips>
                                            </ext:Button>
                                            <ext:Button runat="server" Icon="ResultsetFirst" StyleSpec="margin-bottom:2px;">
                                                <Listeners>
                                                    <Click Handler="CountrySelector.removeAll(GridPanel1, GridPanel2);" />
                                                </Listeners>
                                                <ToolTips>
                                                    <ext:ToolTip runat="server" Title="Remove all" Html="Remove All Rows" />
                                                </ToolTips>
                                            </ext:Button>
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                        </ext:LayoutColumn>
                        <ext:LayoutColumn ColumnWidth="0.5">
                            <ext:GridPanel 
                                ID="GridPanel2" 
                                runat="server" 
                                EnableDragDrop="false"
                                AutoExpandColumn="Country" 
                                StoreID="Store2">
                                <Listeners>
                                </Listeners>
                                <ColumnModel runat="server">
	                                <Columns>
                                        <ext:Column ColumnID="Country" Header="Selected Countries" DataIndex="Name" />                   
	                                </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" />
                                </SelectionModel>  
                                <SaveMask ShowMask="true" />
                            </ext:GridPanel>
                        </ext:LayoutColumn>
                    </Columns>
                </ext:ColumnLayout>              
            </Items>  
            <Buttons>
                <ext:Button runat="server" Text="Save Selected Countries" Icon="Disk">
                    <Listeners>
                        <Click Handler="#{GridPanel2}.submitData();" />
                    </Listeners>
                </ext:Button>
                <ext:Button ID="Button2" runat="server" Text="Cancel" Icon="Cancel">
                    <Listeners>
                        <Click Handler="CountrySelector.removeAll(GridPanel1, GridPanel2);" />
                    </Listeners>
                </ext:Button>
            </Buttons>      
        </ext:Window>
    </form>
</body>
</html>
