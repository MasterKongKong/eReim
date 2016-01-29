<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HtmlTest.aspx.cs" Inherits="eReimbursement.HtmlTest" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<script runat="server">
    
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" 
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>MultiLevel GridPanel - Ext.NET Examples</title>
    <link href="../../../../resources/css/examples.css" rel="stylesheet" type="text/css" />
    <ext:ResourcePlaceHolder ID="ResourcePlaceHolder1" runat="server" Mode="Script" />
    <script type="text/javascript">
        var GetCheck = function () {
            
        };
        window.lookup = {};

        var clean = function (view, isDestroy) {
            var controls = window.lookup[view.grid.id] || {},
                ids = [];

            for (var c in controls) {
                ids.push(controls[c].id || controls[c].storeId);
            }

            if (ids.length > 0) {
                if (isDestroy !== true) {
                    view.grid.getRowExpander().collapseAll();
                }

                for (var i = 0; i < ids.length; i++) {
                    removeFromCache(ids[i], view.grid.id);
                }
            }
        };

        var addToCache = function (c, parent) {
            window.lookup[parent] = window.lookup[parent] || {};
            window.lookup[parent][c] = window[c];
        }

        var removeFromCache = function (c, parent) {
            window.lookup[parent] = window.lookup[parent] || {};

            var ctrl = window.lookup[parent][c];
            delete window.lookup[parent][c];
            if (ctrl) {
                if (ctrl.view) {
                    clean(ctrl.view, true);
                }
                ctrl.destroy();
            }
        }

        var loadLevel = function (expander, record, body, row) {
            if (body.rendered) {
                return;
            }

            var recId = record.id,
                gridId = expander.grid.id,
                level = record.data.Level;

            mLevels.BuildLevel(level + 1, recId, gridId, {
                eventMask: {
                    showMask: true,
                    tartget: "customtarget",
                    customTarget: expander.grid.body
                },

                success: function () {
                    body.rendered = true;
                }
            });
        };
    </script>
</head>
<body>
    <form id="Form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" DirectMethodNamespace="mLevels" />
    <ext:Container ID="Container1" runat="server">
        <Items>
        </Items>
    </ext:Container>
    <ext:Hidden ID="Hidden1" runat="server">
    </ext:Hidden>
    <ext:Button ID="Button1" runat="server" Text="Submit">
        <Listeners>
            <Click Fn="GetCheck" />
        </Listeners>
    </ext:Button>
    </form>
</body>
</html>
