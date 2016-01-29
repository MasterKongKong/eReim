<%@ Page Language="C#" %>
 
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
 
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" 
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
    
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            var structure = new Dictionary<string, string[]>
            {
                { "Asia", new string[] {"Beijing", "Tokyo"} },
                { "Europe", new string[] {"Berlin", "London", "Paris"} }
            };
            
            var products = new string[] { "ProductX", "ProductY" };
            var grid = GridPanel1;
            var view = grid.View[0];
            var store = grid.Store[0];
            var cm = grid.ColumnModel;
            var continentGroupRow = new HeaderGroupRow();
            var cityGroupRow = new HeaderGroupRow();
            var data = new object[5];
            var i = 0;
            var random = new Random();

            foreach (KeyValuePair<string, string[]> keyValuePair in structure)
            {
                var continent = keyValuePair.Key;
                var cities = keyValuePair.Value;
                
                continentGroupRow.Columns.Add(new HeaderGroupColumn
                {
                    Header = continent,
                    Align = Alignment.Center,
                    ColSpan = cities.Length*products.Length
                });
                
                foreach (string city in cities)
                {
                    cityGroupRow.Columns.Add(new HeaderGroupColumn
                    {
                        Header = city,
                        ColSpan = products.Length,
                        Align = Alignment.Center
                    });

                    foreach (string product in products)
                    {
                        store.Reader[0].Fields.Add(city+product, RecordFieldType.Int);
                        
                        cm.Columns.Add(new Column
                        {
                            DataIndex = city + product,
                            Header = product,
                            Renderer =
                            {
                                Format = RendererFormat.UsMoney
                            }
                        }); 
                    }

                    var arr = new int[20];
                    
                    for (int j = 0; j < 20; j++)
                    {
                        arr[j] = Convert.ToInt32((Math.Floor(random.NextDouble() * 11) + 1) * 100000);
                    }
                    
                    data[i++] = arr;
                }
            }
            
            view.HeaderGroupRows.Add(continentGroupRow);
            view.HeaderGroupRows.Add(cityGroupRow);

            store.DataSource = data;
            store.DataBind();
        }
    }
</script>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" 
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Grid Column Header Grouping - Ext.NET Examples</title>
    <link href="../../../../resources/css/examples.css" rel="stylesheet" type="text/css" />    
</head>
<body>
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    
    <h1>Grid Column Header Grouping Example</h1>
    
    <p>This example shows how to achieve column grouping using a plugin.</p>
    <table style="width: 100%;">
        <tr>
            <td width="100px">
                <a style="color: #FF0000">aw</a>
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
            <td>
                &nbsp;
            </td>
        </tr>
    </table>
    <ext:GridPanel 
        ID="GridPanel1"
        runat="server" 
        StripeRows="true"
        Title="Sales By Region" 
        TrackMouseOver="true"
        Width="1000" 
        Height="400">
        <Store>
            <ext:Store ID="Store1" runat="server">
                <Reader>
                    <ext:ArrayReader />
                </Reader>
            </ext:Store>
        </Store>
        <SelectionModel>
            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" />
        </SelectionModel>
        <View>
            <ext:GridView ID="GridView1" runat="server" ForceFit="true" />
        </View>
    </ext:GridPanel>          
</body>
</html>