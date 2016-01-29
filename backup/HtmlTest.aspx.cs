using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using System.Data;
using Ext.Net.Utilities;
namespace eReimbursement
{
    public partial class HtmlTest : System.Web.UI.Page
    {
        //protected void Page_Load(object sender, EventArgs e)
        //{
            //var win = new Window
            //{
            //    ID = "Window1",
            //    Title = "Ajaxian",
            //    Width = Unit.Pixel(880),
            //    Height = Unit.Pixel(710),
            //    Modal = true,
            //    Collapsible = true,
            //    Maximizable = true,
            //    Hidden = true
            //};
            ////LoadConfig lcg = new LoadConfig
            ////{
            ////    Url = "http://www.sina.com.cn/",
            ////    Mode = LoadMode.IFrame,
            ////    ShowMask = true
            ////};
            //win.AutoLoad.Url = "";
            //win.AutoLoad.Mode = LoadMode.IFrame;

            //win.Render(this.Form);
        //}
        protected void Page_Load(object sender, EventArgs e)
        {
            if (X.IsAjaxRequest)
            {
                //We do not need to DataBind on an DirectEvent
                return;
            }

            //Build first level
            this.BuildLevel(1, "r0", "g0");
        }

        [DirectMethod]
        public void BuildLevel(int level, string recId, string gridId)
        {
            var storeId = "L".ConcatWith(level, "_", recId, "_Store");
            var newGridId = "L".ConcatWith(level, "_", recId, "_Grid");

            // build store
            var store = new Store { ID = storeId };
            var reader = new JsonReader { IDProperty = "ID" };
            reader.Fields.Add("ID", "Name");
            reader.Fields.Add(new RecordField
            {
                Name = "Level",
                Convert = { Handler = "return ".ConcatWith(level, ";") }
            });
            store.Reader.Add(reader);
            store.CustomConfig.Add(new ConfigItem("level", level.ToString(), ParameterMode.Raw));

            // bind store
            var data = new List<object>();

            for (int i = 1; i <= 9; i++)
            {
                data.Add(new { ID = recId.ConcatWith("_R", i), Name = "Level".ConcatWith(level, ": Row " + i) });
            }

            //build grid
            var grid = new GridPanel
            {
                ID = newGridId,
                Store = { 
                                    store
                                 },
                AutoHeight = true,
                EnableColumnMove = level == 1
            };

            //build columns
            grid.ColumnModel.Columns.Add(new RowNumbererColumn { Width = 25 });
            if (level==1)
            {
                grid.ColumnModel.Columns.Add(new Column { DataIndex = "Name", Header = "Name" });
                grid.ColumnModel.Columns.Add(new Column { DataIndex = "Name", Header = "Class" });
                grid.ColumnModel.Columns.Add(new Column { DataIndex = "Name", Header = "Class1" });
            }
            else
            {
                grid.ColumnModel.Columns.Add(new Column { DataIndex = "Name", Header = "Name", Resizable = false });
                grid.ColumnModel.Columns.Add(new Column { DataIndex = "Name", Header = "Class", Resizable = false });
                grid.ColumnModel.Columns.Add(new Column { DataIndex = "Name", Header = "Class1", Resizable = false });
            }
            
            grid.ColumnModel.ID = newGridId + "_CM";

            // build view
            var view = new Ext.Net.GridView
            {
                ID = newGridId + "_View",
                ForceFit = true
            };
            grid.View.Add(view);

            // build selection model
            var sm = new RowSelectionModel { ID = newGridId + "_SM" };
            if (level != 3)
            {
                sm.Listeners.BeforeRowSelect.Handler = "return false;";//合计类不允许选择
            }
            grid.SelectionModel.Add(sm);

            // add expander for all levels except last (last level is 5)
            if (level < 3)
            {
                view.Listeners.BeforeRefresh.Fn = "clean";
                var re = new RowExpander
                {
                    ID = newGridId + "_RE",
                    EnableCaching = true,
                    Template = { ID = newGridId + "_TPL", Html = "<div id=\"row_{ID}\" style=\"background-color:white;\"></div>" }
                };

                re.Listeners.BeforeExpand.Fn = "loadLevel";

                grid.Plugins.Add(re);
            }

            store.DataSource = data;
            store.DataBind();

            if (level == 1)
            {
                grid.Title = "MultiLevel grid";
                grid.Width = 600;
                grid.Height = 600;
                grid.AutoHeight = false;
                //this.Form.Controls.Add(grid);
                Container1.Items.Add(grid);
                grid.Plugins.Add(new PanelResizer());
            }
            else
            {
                var renderEl = "row_" + recId;
                X.Get(renderEl).SwallowEvent(new string[] { "click", "mousedown", "mouseup", "dblclick" }, true);

                this.RemoveFromCache(newGridId, gridId);
                grid.Render(renderEl, RenderMode.RenderTo);
                this.AddToCache(newGridId, gridId);
            }
        }

        private void RemoveFromCache(string id, string parentId)
        {
            this.ResourceManager1.AddScript("removeFromCache({0}, {1});", JSON.Serialize(id), JSON.Serialize(parentId));
        }

        private void AddToCache(string id, string parentId)
        {
            this.ResourceManager1.AddScript("addToCache({0}, {1});", JSON.Serialize(id), JSON.Serialize(parentId));
        }
    }
}