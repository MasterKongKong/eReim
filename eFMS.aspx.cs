using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using System.Data;
using Ext.Net.Utilities;
using System.Xml;
using System.Text;
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
        protected void SaveGrid2(object sender, DirectEventArgs e)
        { 

        }
        public class Country
        {
            public string id { get; set; }
            public string ID { get; set; }
            public string Type { get; set; }
            public string Heji { get; set; }
            public string Biaodanhao { get; set; }
            public string Tijiao { get; set; }
            public string Level { get; set; }
        }
        protected void SubmitData(object sender, StoreSubmitDataEventArgs e)
        {
            string json = e.Json;
            XmlNode xml = e.Xml;
            List<Country> countries = e.Object<Country>();

            StringBuilder sb = new StringBuilder(255);

            sb.Append("Selected:");

            foreach (Country country in countries)
            {
                sb.AppendFormat("{0},", country.ID);
            }

            this.txtTo1.Text = sb.ToString().Substring(0, sb.ToString().Length - 1);
        }
        protected void loadgrid2(object sender, DirectEventArgs e)
        {
            var store = new Store { ID = "Store1" };
            var reader = new JsonReader { IDProperty = "ID" };
            reader.Fields.Add("ID", "Type", "Heji", "Biaodanhao", "Tijiao");
            reader.Fields.Add(new RecordField
            {
                Name = "Level",
                Convert = { Handler = "return ".ConcatWith(1, ";") }
            });
            store.Reader.Add(reader);

            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Type");
            dt.Columns.Add("Heji");
            dt.Columns.Add("Biaodanhao");
            dt.Columns.Add("Tijiao");
            DataRow dr = dt.NewRow();
            dr["ID"] = "10";
            dr["Type"] = "差旅费";
            dr["Heji"] = "100";
            dr["Biaodanhao"] = "BJS1101";
            dr["Tijiao"] = "2012-12-11";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["ID"] = "11";
            dr["Type"] = "通用费用";
            dr["Heji"] = "1100";
            dr["Biaodanhao"] = "BJS1112";
            dr["Tijiao"] = "2012-12-21";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["ID"] = "12";
            dr["Type"] = "差旅费";
            dr["Heji"] = "100";
            dr["Biaodanhao"] = "BJS1123";
            dr["Tijiao"] = "2012-12-31";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["ID"] = "13";
            dr["Type"] = "通用费用";
            dr["Heji"] = "100";
            dr["Biaodanhao"] = "BJS1134";
            dr["Tijiao"] = "2012-11-11";
            dt.Rows.Add(dr);
            
            var grid = new GridPanel
            {
                ID = "Grid1",
                Store = { 
                                    store
                                 },
                AutoScroll = true,
                Border = false,
                Title = "可按住Ctrl以多选,然后点击右侧&quot;+&quot;按钮向右侧添加数据.",
                Height = 530
            };
            //build columns
            grid.ColumnModel.Columns.Add(new RowNumbererColumn { Width = 25 });
            grid.ColumnModel.Columns.Add(new Column { DataIndex = "Biaodanhao", Header = "表单号" });
            grid.ColumnModel.Columns.Add(new Column { DataIndex = "Type", Header = "单据类型" });
            grid.ColumnModel.Columns.Add(new Column { DataIndex = "Heji", Header = "合计" });
            grid.ColumnModel.Columns.Add(new Column { DataIndex = "Tijiao", Header = "提交时间" });
            grid.ColumnModel.ID = "Grid1_CM";
            var view = new Ext.Net.GridView
            {
                ID = "Grid1_View",
                ForceFit = true
            };
            grid.View.Add(view);
            var sm = new RowSelectionModel { ID = "Grid1_SM" };
            //sm.Listeners.BeforeRowSelect.Handler = "return false;";//合计类不允许选择
            grid.SelectionModel.Add(sm);

            // add expander for all levels except last (last level is 5)
            view.Listeners.BeforeRefresh.Fn = "clean";
            var re = new RowExpander
            {
                ID = "Grid1_RE",
                EnableCaching = true,
                Template = { ID = "Grid1_TPL", Html = "<div id=\"row_{ID}\" style=\"background-color:white;\"></div>" }
            };
            re.Listeners.BeforeExpand.Fn = "loadLevel";
            grid.Plugins.Add(re);
            Panel5.Items.Add(grid);
            store.DataSource = dt;
            store.DataBind();
            grid.Listeners.ViewReady.Fn = "expangrid";
            grid.Listeners.ViewReady.Single = true;
            grid.Render();
            //X.AddScript("expandallgrid();");
            //re.ExpandAll();
            //BuildLevel(int level, string recId, string gridId,string dtype)
            //
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    if (dt.Rows[i]["Type"].ToString() == "差旅费")
            //    {
            //        var newgridid = "L2_" + dt.Rows[i]["ID"].ToString() + "_Grid";
            //        var store2 = new Store { ID = "L2_" + dt.Rows[i]["ID"].ToString() + "_Store" };
            //        var reader2 = new JsonReader { IDProperty = "ID" };
            //        reader.Fields.Add("ID", "Type", "Heji", "Biaodanhao", "Tijiao");
            //        reader.Fields.Add(new RecordField
            //        {
            //            Name = "Level",
            //            Convert = { Handler = "return ".ConcatWith(2, ";") }
            //        });
            //        store2.Reader.Add(reader2);

            //        DataTable dt2 = new DataTable();
            //        dt2.Columns.Add("ID");
            //        dt2.Columns.Add("Type");
            //        dt2.Columns.Add("Heji");
            //        dt2.Columns.Add("Biaodanhao");
            //        dt2.Columns.Add("Tijiao");
            //        Random ran = new Random();
            //        DataRow dr2 = dt2.NewRow();
            //        dr2["ID"] = dt.Rows[i]["ID"].ToString() + "0";
            //        dr2["Type"] = "票价合计";
            //        dr2["Heji"] = "120";
            //        dr2["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
            //        dr2["Tijiao"] = "2012-12-11";
            //        dt2.Rows.Add(dr2);
            //        dr2 = dt2.NewRow();
            //        dr2["ID"] = dt.Rows[i]["ID"].ToString() + "1";
            //        dr2["Type"] = "酒店";
            //        dr2["Heji"] = "120";
            //        dr2["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
            //        dr2["Tijiao"] = "2012-12-11";
            //        dt2.Rows.Add(dr2);
            //        dr2 = dt2.NewRow();
            //        dr2["ID"] = dt.Rows[i]["ID"].ToString() + "2";
            //        dr2["Type"] = "交通费";
            //        dr2["Heji"] = "120";
            //        dr2["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
            //        dr2["Tijiao"] = "2012-12-11";
            //        dt2.Rows.Add(dr2);
            //        dr2 = dt2.NewRow();
            //        dr2["ID"] = dt.Rows[i]["ID"].ToString() + "3";
            //        dr2["Type"] = "膳食费";
            //        dr2["Heji"] = "120";
            //        dr2["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
            //        dr2["Tijiao"] = "2012-12-11";
            //        dt2.Rows.Add(dr2);
            //        dr2 = dt2.NewRow();
            //        dr2["ID"] = dt.Rows[i]["ID"].ToString() + "4";
            //        dr2["Type"] = "机场费";
            //        dr2["Heji"] = "120";
            //        dr2["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
            //        dr2["Tijiao"] = "2012-12-11";
            //        dt2.Rows.Add(dr2);
            //        dr2 = dt2.NewRow();
            //        dr2["ID"] = dt.Rows[i]["ID"].ToString() + "5";
            //        dr2["Type"] = "其他";
            //        dr2["Heji"] = "120";
            //        dr2["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
            //        dr2["Tijiao"] = "2012-12-11";
            //        dt2.Rows.Add(dr2);
            //        dr2 = dt2.NewRow();
            //        dr2["ID"] = dt.Rows[i]["ID"].ToString() + "6";
            //        dr2["Type"] = "每日津贴";
            //        dr2["Heji"] = "120";
            //        dr2["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
            //        dr2["Tijiao"] = "2012-12-11";
            //        dt2.Rows.Add(dr2);

            //        var grid2 = new GridPanel
            //        {
            //            ID = newgridid,
            //            Store = { 
            //                        store2
            //                     },
            //            AutoHeight = true,
            //            AutoScroll = true,
            //            EnableColumnMove = false,
            //        };
            //        //build columns
            //        grid2.ColumnModel.Columns.Add(new RowNumbererColumn { Width = 25 });
            //        grid2.ColumnModel.Columns.Add(new Column { DataIndex = "Type", Header = "费用类型", Resizable = false });
            //        grid2.ColumnModel.Columns.Add(new Column { DataIndex = "Heji", Header = "合计", Resizable = false });
            //        grid2.ColumnModel.Columns.Add(new Column { DataIndex = "Biaodanhao", Header = "公司预支", Resizable = false });
            //        grid2.ColumnModel.ID = newgridid + "_CM";
            //        var view2 = new Ext.Net.GridView
            //        {
            //            ID = newgridid+"_View",
            //            ForceFit = true
            //        };
            //        grid2.View.Add(view2);
            //        var sm2 = new RowSelectionModel { ID = newgridid+"_SM" };
            //        sm2.Listeners.BeforeRowSelect.Handler = "return false;";//合计类不允许选择
            //        grid2.SelectionModel.Add(sm2);

            //        // add expander for all levels except last (last level is 5)
            //        view2.Listeners.BeforeRefresh.Fn = "clean";
            //        var re2 = new RowExpander
            //        {
            //            ID = newgridid+"_RE",
            //            EnableCaching = true,
            //            Template = { ID = newgridid+"_TPL", Html = "<div id=\"row_{ID}\" style=\"background-color:white;\"></div>" }
            //        };
            //        //re2.Listeners.BeforeExpand.Fn = "loadLevel";
            //        grid2.Plugins.Add(re2);

            //        var renderEl2 = "row_" + dt.Rows[i]["ID"].ToString();
            //        X.Get(renderEl2).SwallowEvent(new string[] { "click", "mousedown", "mouseup", "dblclick" }, true);
            //        this.RemoveFromCache(newgridid, "Grid1");

            //        store2.DataSource = dt2;
            //        store2.DataBind();

            //        grid.Render(renderEl2, RenderMode.RenderTo);
            //        this.AddToCache(newgridid, "Grid1");
            //    }
            //}
            //if (1 == 1)
            //{
            //    grid.Title = "可按住Ctrl以多选.";
            //    grid.Height = 400;
            //    grid.AutoHeight = false;
            //    grid.Border = false;
            //    //this.Form.Controls.Add(grid);
            //    Panel5.Html = "";
            //    Panel5.Items.Add(grid);
            //    //grid.Plugins.Add(new PanelResizer());
            //}
            //else
            //{
                //var renderEl = "row_" + recId;
                //X.Get(renderEl).SwallowEvent(new string[] { "click", "mousedown", "mouseup", "dblclick" }, true);

                //this.RemoveFromCache(newGridId, gridId);
                //grid.Render(renderEl, RenderMode.RenderTo);
                //this.AddToCache(newGridId, gridId);
            //}
            //
            
            
        }
        protected void loadgrid1(object sender, DirectEventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            for (int i = 0; i < 6; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = "a" + i.ToString();
                dt.Rows.Add(dr);
            }
            Store2.DataSource = dt;
            Store2.DataBind();

            //Ext.Net.Button btn = new Ext.Net.Button();

            //btn.X = 560; btn.Y = 40; btn.Width = 70; btn.ID = "new_btn"; btn.Text = "new";
            //this.Panel3.Items.Add(btn);
            //btn.Render();
            //var button = new Ext.Net.Button
            //{
            //    ID = "buttomdi",
            //    Text = "Button",
            //    X = 560,
            //    Y = 40,
            //    Width = 70
            //};
            //Panel3.Items.Add(button);
            //button.Render();
            //Ext.Net.MenuItem mi1 = new Ext.Net.MenuItem("New item 1");
            //Ext.Net.MenuItem mi2 = new Ext.Net.MenuItem("New item 2");
            //this.Menu1.Items.Add(mi1);
            //mi1.Render();
            //this.Menu1.Items.Add(mi2);
            //mi2.Render();
        }
        //protected void loadgrid(object sender, EventArgs e)
        //{
        //    if (hdCollapse.Value.ToString() == "1")
        //    {
        //        Panel2.Collapse(true);
        //    }
        //    X.AddScript("Hidden1.setValue('');");
        //    this.BuildLevel(1, "r0", "g0", "");
        //}
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
                    dr["ID"] = recId + "20";
                    dr["Type"] = "票价合计";
                    dr["Heji"] = "120";
                    dr["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
                    dr["Tijiao"] = "2012-12-11";
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["ID"] = recId + "21";
                    dr["Type"] = "酒店";
                    dr["Heji"] = "120";
                    dr["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
                    dr["Tijiao"] = "2012-12-11";
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["ID"] = recId + "22";
                    dr["Type"] = "交通费";
                    dr["Heji"] = "120";
                    dr["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
                    dr["Tijiao"] = "2012-12-11";
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["ID"] = recId + "23";
                    dr["Type"] = "膳食费";
                    dr["Heji"] = "120";
                    dr["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
                    dr["Tijiao"] = "2012-12-11";
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["ID"] = recId + "24";
                    dr["Type"] = "机场费";
                    dr["Heji"] = "120";
                    dr["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
                    dr["Tijiao"] = "2012-12-11";
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["ID"] = recId + "25";
                    dr["Type"] = "其他";
                    dr["Heji"] = "120";
                    dr["Biaodanhao"] = (120 * Math.Round((decimal)ran.Next(1, 10), 1)).ToString();
                    dr["Tijiao"] = "2012-12-11";
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["ID"] = recId + "26";
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
                    dr["ID"] = recId + "20";
                    dr["Type"] = "交际费";
                    dr["Heji"] = "100";
                    dr["Biaodanhao"] = "BJS110" + level.ToString();
                    dr["Tijiao"] = "2012-12-11";
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["ID"] = recId + "21";
                    dr["Type"] = "交通费";
                    dr["Heji"] = "1100";
                    dr["Biaodanhao"] = "BJS111" + level.ToString();
                    dr["Tijiao"] = "2012-12-21";
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["ID"] = recId + "22";
                    dr["Type"] = "通讯费";
                    dr["Heji"] = "100";
                    dr["Biaodanhao"] = "BJS112" + level.ToString();
                    dr["Tijiao"] = "2012-12-31";
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dr["ID"] = recId + "23";
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
                    dr["ID"] = recId + "3" + i.ToString();
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
                AutoScroll = true,
                EnableColumnMove = level == 1,
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
            //if (level != 3 && dtype != "差旅费")
            //{
            //    sm.Listeners.BeforeRowSelect.Handler = "return false;";//合计类不允许选择
            //}
            grid.SelectionModel.Add(sm);
            RowExpander rem = new RowExpander();
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
                rem = re;
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
                Panel5.Html = "";
                Panel5.Items.Add(grid);
                //grid.Plugins.Add(new PanelResizer());
            }
            else
            {
                var renderEl = "row_" + recId;
                X.Get(renderEl).SwallowEvent(new string[] { "click", "mousedown", "mouseup", "dblclick" }, true);

                this.RemoveFromCache(newGridId, gridId);
                if (level < 3)
                {
                    grid.Listeners.ViewReady.Fn = "expangrid";
                    grid.Listeners.ViewReady.Single = true;
                }
                grid.Render(renderEl, RenderMode.RenderTo);
                this.AddToCache(newGridId, gridId);
            }
            //string gl = newGridId;
            ////记录选择行的空间名
            //if (level == 3 || (level == 2 && dtype == "差旅费"))
            //{
            //    X.AddScript("var st=Hidden1.getValue();Hidden1.setValue(st+='" + gl + ",');");
            //    //X.AddScript("var st=TextField1.getValue();TextField1.setValue(st+='" + gl + ",');");
            //}
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