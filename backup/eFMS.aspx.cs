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
    public partial class eFMS : System.Web.UI.Page
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
            //if (!X.IsAjaxRequest)
            //{
            //    //We do not need to DataBind on an DirectEvent
            //    this.BuildLevel(1, "r0", "g0", "");
            //}

            //Build first level
            
        }
        protected void loadgrid1(object sender, DirectEventArgs e)
        {
            X.AddScript("Hidden1.setValue('');");
            this.BuildLevel(1, "r0", "g0", "");
        }
        protected void loadgrid(object sender, EventArgs e)
        {
            X.AddScript("Hidden1.setValue('');");
            this.BuildLevel(1, "r0", "g0", "");
        }
        [DirectMethod]
        public void BuildLevel(int level, string recId, string gridId,string dtype)
        {
            
            var storeId = "L".ConcatWith(level, "_", recId, "_Store");
            var newGridId = "L".ConcatWith(level, "_", recId, "_Grid");

            // build store
            var store = new Store { ID = storeId };
            var reader = new JsonReader { IDProperty = "ID" };
            reader.Fields.Add("ID", "Type", "Heji", "Biaodanhao", "Tijiao");
            reader.Fields.Add(new RecordField
            {
                Name = "Level",
                Convert = { Handler = "return ".ConcatWith(level, ";") }
            });
            store.Reader.Add(reader);
            //store.CustomConfig.Add(new ConfigItem("level", level.ToString(), ParameterMode.Raw));

            // bind store
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Type");
            dt.Columns.Add("Heji");
            dt.Columns.Add("Biaodanhao");
            dt.Columns.Add("Tijiao");
            if (level == 1)
            {
                DataRow dr = dt.NewRow();
                dr["ID"] = "0";
                dr["Type"] = "差旅费";
                dr["Heji"] = "100";
                dr["Biaodanhao"] = "BJS110" + level.ToString();
                dr["Tijiao"] = "2012-12-11";
                dt.Rows.Add(dr);
                dr = dt.NewRow();
                dr["ID"] = "1";
                dr["Type"] = "通用费用";
                dr["Heji"] = "1100";
                dr["Biaodanhao"] = "BJS111" + level.ToString();
                dr["Tijiao"] = "2012-12-21";
                dt.Rows.Add(dr);
                dr = dt.NewRow();
                dr["ID"] = "2";
                dr["Type"] = "差旅费";
                dr["Heji"] = "100";
                dr["Biaodanhao"] = "BJS112" + level.ToString();
                dr["Tijiao"] = "2012-12-31";
                dt.Rows.Add(dr);
                dr = dt.NewRow();
                dr["ID"] = "3";
                dr["Type"] = "通用费用";
                dr["Heji"] = "100";
                dr["Biaodanhao"] = "BJS113" + level.ToString();
                dr["Tijiao"] = "2012-11-11";
                dt.Rows.Add(dr);
            }
            else if(level == 2)
            {
                
                if (dtype == "差旅费")
                {


                    Random ran = new Random();
                    DataRow dr = dt.NewRow();
                    dr["ID"] = recId + "0";
                    dr["Type"] = "票价合计";
                    dr["Heji"] = "120";
                    dr["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
                    dr["Tijiao"] = "2012-12-11";
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["ID"] = recId + "1";
                    dr["Type"] = "酒店";
                    dr["Heji"] = "120";
                    dr["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
                    dr["Tijiao"] = "2012-12-11";
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["ID"] = recId + "2";
                    dr["Type"] = "交通费";
                    dr["Heji"] = "120";
                    dr["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
                    dr["Tijiao"] = "2012-12-11";
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["ID"] = recId + "3";
                    dr["Type"] = "膳食费";
                    dr["Heji"] = "120";
                    dr["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
                    dr["Tijiao"] = "2012-12-11";
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["ID"] = recId + "4";
                    dr["Type"] = "机场费";
                    dr["Heji"] = "120";
                    dr["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
                    dr["Tijiao"] = "2012-12-11";
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["ID"] = recId + "5";
                    dr["Type"] = "其他";
                    dr["Heji"] = "120";
                    dr["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
                    dr["Tijiao"] = "2012-12-11";
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["ID"] = recId + "6";
                    dr["Type"] = "每日津贴";
                    dr["Heji"] = "120";
                    dr["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
                    dr["Tijiao"] = "2012-12-11";
                    dt.Rows.Add(dr);
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    dr = dt.NewRow();
                    dr["ID"] = recId + "0";
                    dr["Type"] = "交际费";
                    dr["Heji"] = "100";
                    dr["Biaodanhao"] = "BJS110" + level.ToString();
                    dr["Tijiao"] = "2012-12-11";
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["ID"] = recId + "1";
                    dr["Type"] = "交通费";
                    dr["Heji"] = "1100";
                    dr["Biaodanhao"] = "BJS111" + level.ToString();
                    dr["Tijiao"] = "2012-12-21";
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["ID"] = recId + "2";
                    dr["Type"] = "通讯费";
                    dr["Heji"] = "100";
                    dr["Biaodanhao"] = "BJS112" + level.ToString();
                    dr["Tijiao"] = "2012-12-31";
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["ID"] = recId + "3";
                    dr["Type"] = "其他费用";
                    dr["Heji"] = "100";
                    dr["Biaodanhao"] = "BJS113" + level.ToString();
                    dr["Tijiao"] = "2012-11-11";
                    dt.Rows.Add(dr);
                }
            }
            else if (level == 3 && dtype != "差旅费")
            {
                for (int i = 0; i < 4; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = recId + i.ToString();
                    dr["Type"] = "其他费用";
                    dr["Heji"] = (100 + i).ToString();
                    dr["Biaodanhao"] = "费用描述";
                    dr["Tijiao"] = "2012-11-11";
                    dt.Rows.Add(dr);
                }
            }

            //var data = new List<object>();

            //for (int i = 1; i <= 9; i++)
            //{
            //    data.Add(new { ID = recId.ConcatWith("_R", i), Biaodanhao = "Level".ConcatWith(level, ": Row " + i), Type = "Level".ConcatWith(level, ": Row " + i), Heji = "Level".ConcatWith(level, ": Row " + i), Tijiao = "Level".ConcatWith(level, ": Row " + i) });
            //}

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
                grid.ColumnModel.Columns.Add(new Column { DataIndex = "Biaodanhao", Header = "表单号" });
                grid.ColumnModel.Columns.Add(new Column { DataIndex = "Type", Header = "单据类型" });
                grid.ColumnModel.Columns.Add(new Column { DataIndex = "Heji", Header = "合计" });
                grid.ColumnModel.Columns.Add(new Column { DataIndex = "Tijiao", Header = "提交时间" });
            }
            else if(level == 2)
            {
                if (dtype == "差旅费")
                {
                    grid.ColumnModel.Columns.Add(new Column { DataIndex = "Type", Header = "费用类型", Resizable = false });
                    grid.ColumnModel.Columns.Add(new Column { DataIndex = "Heji", Header = "合计", Resizable = false });
                    grid.ColumnModel.Columns.Add(new Column { DataIndex = "Biaodanhao", Header = "公司预支", Resizable = false });
                }
                else
                {
                    grid.ColumnModel.Columns.Add(new Column { DataIndex = "Type", Header = "单据类型", Resizable = false });
                    grid.ColumnModel.Columns.Add(new Column { DataIndex = "Heji", Header = "合计", Resizable = false });
                }
            }
            else if (level == 3)
            {
                grid.ColumnModel.Columns.Add(new Column { DataIndex = "Heji", Header = "金额", Resizable = false });
                grid.ColumnModel.Columns.Add(new Column { DataIndex = "Heji", Header = "费用描述", Resizable = false });
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
            if (level != 3 && dtype != "差旅费")
            {
                sm.Listeners.BeforeRowSelect.Handler = "return false;";//合计类不允许选择
            }
            grid.SelectionModel.Add(sm);

            // add expander for all levels except last (last level is 5)
            if (level < 3 && dtype != "差旅费")
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

            store.DataSource = dt;
            store.DataBind();

            if (level == 1)
            {
                grid.Title = "可按住Ctrl以多选.";
                grid.Height = 400;
                grid.AutoHeight = false;
                grid.Border = false;
                //this.Form.Controls.Add(grid);
                Container1.Html = "";
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
            string gl = newGridId + "_SM";
            //记录选择行的空间名
            if (level == 3 || (level == 2 && dtype == "差旅费"))
            {
                X.AddScript("var st=Hidden1.getValue();Hidden1.setValue(st+='" + gl + ",');");
                //X.AddScript("var st=TextField1.getValue();TextField1.setValue(st+='" + gl + ",');");
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