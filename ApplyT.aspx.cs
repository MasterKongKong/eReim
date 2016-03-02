using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using System.IO;
using System.Data;
using System.Xml;
using System.Text;
using DIMERCO.SDK;
using System.Net.Mail;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using System.Configuration;
using System.Data.SqlClient;
namespace eReimbursement
{
    public partial class ApplyT : App_Code.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                cs.DBCommand dbc = new cs.DBCommand();
                DataSet ds2 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(Request.QueryString["apply_userid"].ToString());
                string station = "";
                if (ds2.Tables[0].Rows.Count == 1)
                {
                    DataTable dt1 = ds2.Tables[0];
                    station = dt1.Rows[0]["stationCode"].ToString();
                    hdStation.Value = dt1.Rows[0]["stationCode"].ToString();
                    cbxTStation.Text = dt1.Rows[0]["CostCenter"].ToString();
                    LabelCurrency.Text = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(station);
                    DataTable dttemp = new DataTable();
                    string sqltemp = "select * from ESUSER where Userid='" + Request.QueryString["apply_userid"].ToString() + "'";
                    dttemp = dbc.GetData("eReimbursement", sqltemp);
                    if (dttemp.Rows.Count > 0)
                    {
                        LabelCurrency.Text = dttemp.Rows[0]["Currency"].ToString();
                    }
                }

                if (Request.QueryString["Draft"] != null)
                {
                    if (Request.QueryString["Draft"].ToString() == "1")
                    {
                        X.AddScript("Button51.disable();Button6.disable();btnSave.disable();");
                    }
                    else if (Request.QueryString["Draft"].ToString() == "2")
                    {
                        X.AddScript("Button51.disable();Button6.disable();btnSave.disable();btnUploadScanFile.disable();Button1.disable();");
                    }
                }
                ////设置币种
                //if (Request.QueryString["Cur"] != null)
                //{
                //    LabelCurrency.Text = Request.QueryString["Cur"].ToString();
                //}

                //DataSet dsdep = DIMERCO.SDK.Utilities.LSDK.getCRPDepartment();
                //DataTable dtdep = new DataTable();
                //dtdep.Columns.Add(new DataColumn("Depart", typeof(String)));
                //for (int i = 0; i < dsdep.Tables[0].Rows.Count; i++)
                //{
                //    DataRow dr = dtdep.NewRow();
                //    dr[0] = dsdep.Tables[0].Rows[i][2].ToString();
                //    dtdep.Rows.Add(dr);
                //}
                //StoreDepartment.DataSource = dtdep;
                //StoreDepartment.DataBind();

                //string sqltype = "";
                //if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                //{
                //    sqltype += "select [Text]=CText,[Value]=CValue from Edic where KeyValue='PayType'";
                //}
                //else
                //{
                //    sqltype += "select [Text]=EText,[Value]=CValue from Edic where KeyValue='PayType'";
                //}
                //DataTable dttype = dbc.GetData("eReimbursement", sqltype);
                //StorePayType.DataSource = dttype;
                //StorePayType.DataBind();
            }
        }
        protected void UploadScanFileClick(object sender, DirectEventArgs e)
        {
            if (this.FileUploadField1.HasFile)
            {
                string lstrFileName = null;     //上传文件路径
                string lstrFileFolder = null;   //存放文件路径
                string lstrFileNamePath = null; //上传目录及文件名
                cs.DBCommand dbc = new cs.DBCommand();
                //获得上传到服务器的目录名称,如果上传目录为空，就使用"D:\"作为缺省上传目录 
                //if (dir.Value != "") lstrFileFolder = dir.Value; else lstrFileFolder = "D:\\";
                lstrFileFolder = System.Web.HttpContext.Current.Request.MapPath("Upload/");

                //lstrFileName = FileUploadField1.PostedFile.FileName;  //获得文件名称
                //注:FileUploadField1.PostedFile.FileName 返回的是通过文件对话框选择的文件名，这之中包含了文件的目录信息

                lstrFileName = System.IO.Path.GetFileName(FileUploadField1.PostedFile.FileName);  //去掉目录信息，返回文件名称

                //判断上传目录是否存在，不存在就建立 
                if (!System.IO.Directory.Exists(lstrFileFolder)) System.IO.Directory.CreateDirectory(lstrFileFolder);
                string dtime = DateTime.Now.ToString("yyyyMMddHHmmss");
                lstrFileNamePath = lstrFileFolder + "SF" + dtime + System.IO.Path.GetExtension(lstrFileName); //得到上传目录及文件名称 
 


                //FileUploadField1.PostedFile.SaveAs(lstrFileNamePath);       //上传文件到服务器
                //string filename = "SF" + dtime + System.IO.Path.GetExtension(lstrFileName);

                //HLAttachFile.Text = filename; hdScanFile.Value = filename;
                //HLAttachFile.NavigateUrl = "./Upload/" + filename;
                //string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";
                ////更新数据
                //if (hdDetailID.Value.ToString() != "")
                //{
                //    string updatesql = "update EeommonDetail set Attach='" + filename + "' where ID=" + hdDetailID.Value.ToString();
                    
                //    string update = dbc.UpdateData("eReimbursement", updatesql, "Update");
                //}
                //X.Msg.Show(new MessageBoxConfig
                //{
                //    Buttons = MessageBox.Button.OK,
                //    Icon = MessageBox.Icon.INFO,
                //    Title = "Success",
                //    Message = string.Format(tpl, this.FileUploadField1.PostedFile.FileName, this.FileUploadField1.PostedFile.ContentLength)
                //});

                //151124.增加上传附件判断.
                string postfix = Path.GetExtension(lstrFileName);
                string postfixraw = Path.GetExtension(lstrFileName);
                int indexf = postfix.IndexOf('.');
                if (indexf != -1)
                {
                    postfixraw = postfixraw.Substring(1);
                }
                string station = hdStation.Value.ToString();
                string sql = "select * from FileRule where StationCode='" + station + "'";
                DataTable dtrule = dbc.GetData("eReimbursement", sql);
                if (dtrule.Rows.Count<1)
                {
                    string sqlnew = "select * from FileRule where StationCode='DEFAULT'";
                    DataTable dtnew = dbc.GetData("eReimbursement", sqlnew);

                    if (dtnew.Select("Allow=0 and PostfixRaw='" + postfixraw + "'").Count() == 1)
                    {
                        //不允许上传该文件类型
                        X.AddScript("Ext.Msg.show({ title: 'Warning', msg: 'Not allowed file type.Please contact local MIS.', buttons: { ok: 'Ok' }, fn: function (btn) { return false; } });");
                        return;
                    }
                    else if (dtnew.Select("Allow=1 and PostfixRaw='" + postfixraw + "'").Count() == 1)
                    {
                        //允许上传的文件类型
                        FileUploadField1.PostedFile.SaveAs(lstrFileNamePath);       //上传文件到服务器
                        string filename = "SF" + dtime + System.IO.Path.GetExtension(lstrFileName);

                        HLAttachFile.Text = filename; hdScanFile.Value = filename;
                        HLAttachFile.NavigateUrl = "./Upload/" + filename;
                        string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";
                        //更新数据
                        if (hdDetailID.Value.ToString() != "")
                        {
                            string updatesql = "update EeommonDetail set Attach='" + filename + "' where ID=" + hdDetailID.Value.ToString();

                            string update = dbc.UpdateData("eReimbursement", updatesql, "Update");
                        }
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Title = "Success",
                            Message = string.Format(tpl, this.FileUploadField1.PostedFile.FileName, this.FileUploadField1.PostedFile.ContentLength)
                        });
                    }
                    else
                    {
                        //未知文件类型
                        FileUploadField1.PostedFile.SaveAs(lstrFileNamePath);       //上传文件到服务器
                        //压缩文件
                        cs.ZipFloClass zip = new cs.ZipFloClass();
                        string filename = "SF" + dtime + postfix;
                        string filepath = lstrFileNamePath;
                        string zipfilepath = lstrFileFolder + "SF" + dtime +".zip";
                        zip.ZipSingleFile(lstrFileName, filepath, zipfilepath);
                        string newfilename = "SF" + dtime + ".zip";

                        HLAttachFile.Text = newfilename; hdScanFile.Value = newfilename;
                        HLAttachFile.NavigateUrl = "./Upload/" + newfilename;
                        string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";
                        //更新数据
                        if (hdDetailID.Value.ToString() != "")
                        {
                            string updatesql = "update EeommonDetail set Attach='" + newfilename + "' where ID=" + hdDetailID.Value.ToString();

                            string update = dbc.UpdateData("eReimbursement", updatesql, "Update");
                        }
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Title = "Success",
                            Message = string.Format(tpl, this.FileUploadField1.PostedFile.FileName, this.FileUploadField1.PostedFile.ContentLength)
                        });
                    }
                }
                else
                {
                    if (dtrule.Select("Allow=0 and PostfixRaw='" + postfixraw + "'").Count() == 1)
                    {
                        //不允许上传该文件类型
                        X.AddScript("Ext.Msg.show({ title: 'Warning', msg: 'Not allowed file type.Please contact local MIS.', buttons: { ok: 'Ok' }, fn: function (btn) { return false; } });");
                        return;
                    }
                    else if (dtrule.Select("Allow=1 and PostfixRaw='" + postfixraw + "'").Count() == 1)
                    {
                        //允许上传的文件类型
                        FileUploadField1.PostedFile.SaveAs(lstrFileNamePath);       //上传文件到服务器
                        string filename = "SF" + dtime + System.IO.Path.GetExtension(lstrFileName);

                        HLAttachFile.Text = filename; hdScanFile.Value = filename;
                        HLAttachFile.NavigateUrl = "./Upload/" + filename;
                        string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";
                        //更新数据
                        if (hdDetailID.Value.ToString() != "")
                        {
                            string updatesql = "update EeommonDetail set Attach='" + filename + "' where ID=" + hdDetailID.Value.ToString();

                            string update = dbc.UpdateData("eReimbursement", updatesql, "Update");
                        }
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Title = "Success",
                            Message = string.Format(tpl, this.FileUploadField1.PostedFile.FileName, this.FileUploadField1.PostedFile.ContentLength)
                        });
                    }
                    else
                    {
                        //未知文件类型
                        FileUploadField1.PostedFile.SaveAs(lstrFileNamePath);       //上传文件到服务器
                        //压缩文件
                        cs.ZipFloClass zip = new cs.ZipFloClass();
                        string filename = "SF" + dtime + postfix;
                        string filepath = lstrFileNamePath;
                        string zipfilepath = lstrFileFolder + "SF" + dtime + ".zip";
                        zip.ZipSingleFile(lstrFileName, filepath, zipfilepath);
                        string newfilename = "SF" + dtime + ".zip";

                        HLAttachFile.Text = newfilename; hdScanFile.Value = newfilename;
                        HLAttachFile.NavigateUrl = "./Upload/" + newfilename;
                        string tpl = "Uploaded file: {0}<br/>Size: {1} bytes";
                        //更新数据
                        if (hdDetailID.Value.ToString() != "")
                        {
                            string updatesql = "update EeommonDetail set Attach='" + newfilename + "' where ID=" + hdDetailID.Value.ToString();

                            string update = dbc.UpdateData("eReimbursement", updatesql, "Update");
                        }
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.INFO,
                            Title = "Success",
                            Message = string.Format(tpl, this.FileUploadField1.PostedFile.FileName, this.FileUploadField1.PostedFile.ContentLength)
                        });
                    }
                }
            }
        }
        protected void GetStation(object sender, DirectEventArgs e)
        {
            DataSet getCostCenterBYStationCode = DIMERCO.SDK.Utilities.LSDK.getCostCenterBYStationCode(X.GetValue("cbxTStation"), 10);
            //DataSet getCostCenterBYStationCode = DIMERCO.SDK.Utilities.LSDK.getStationHierarchy();
            DataTable dtCOACenter = (DataTable)getCostCenterBYStationCode.Tables[0];
            DataTable dtCOACenternew = new DataTable();
            dtCOACenternew.Columns.Add("cityID", System.Type.GetType("System.String"));
            dtCOACenternew.Columns.Add("cityCode", System.Type.GetType("System.String"));
            for (int c = 0; c < dtCOACenter.Rows.Count; c++)
            {
                DataRow dr = dtCOACenternew.NewRow();
                dr["cityID"] = dtCOACenter.Rows[c][0].ToString();
                dr["cityCode"] = dtCOACenter.Rows[c][1].ToString();
                dtCOACenternew.Rows.Add(dr);
            }
            StoreCOACenter.DataSource = dtCOACenternew;
            StoreCOACenter.DataBind();
        }
        [Serializable]
        public class DetailExpense
        {
            [XmlElement("DetailID")]
            public string DetailID { get; set; }

            [XmlElement("MainID")]
            public string MainID { get; set; }

            [XmlElement("Type")]
            public string Type { get; set; }

            [XmlElement("AccountName")]
            public string AccountName { get; set; }

            [XmlElement("AccountCode")]
            public string AccountCode { get; set; }

            [XmlElement("AccountDes")]
            public string AccountDes { get; set; }

            [XmlElement("Cur")]
            public string Cur { get; set; }

            [XmlElement("Amount")]
            public string Amount { get; set; }

            [XmlElement("TSation")]
            public string TSation { get; set; }

            [XmlElement("Tdate")]
            public string Tdate { get; set; }

            [XmlElement("Fcity")]
            public string Fcity { get; set; }

            [XmlElement("Tcity")]
            public string Tcity { get; set; }

            [XmlElement("Attach")]
            public string Attach { get; set; }

            [XmlElement("EType")]
            public string EType { get; set; }

            [XmlElement("EcompanyCode")]
            public string EcompanyCode { get; set; }

            [XmlElement("Ecompany")]
            public string Ecompany { get; set; }

            [XmlElement("Eperson")]
            public string Eperson { get; set; }

            [XmlElement("Epurpos")]
            public string Epurpos { get; set; }

            [XmlElement("Creadedby")]
            public string Creadedby { get; set; }

            [XmlElement("CreaedeDate")]
            public string CreaedeDate { get; set; }

            [XmlElement("EffectTime")]
            public string EffectTime { get; set; }

            [XmlElement("ETypeCode")]
            public string ETypeCode { get; set; }

            [XmlElement("Draft")]
            public string Draft { get; set; }

            [XmlElement("COAName")]
            public string COAName { get; set; }

            [XmlElement("Department1")]
            public string Department1 { get; set; }

            [XmlElement("PaymentType")]
            public string PaymentType { get; set; }
            [XmlElement("PaymentDate")]
            public string PaymentDate { get; set; }
            [XmlElement("Vendor")]
            public string Vendor { get; set; }
            [XmlElement("Budget")]
            public Boolean Budget { get; set; }
            [XmlElement("BudgetCurrent")]
            public string BudgetCurrent { get; set; }
        }
        [DirectMethod]
        public void GetBudget(string detailjson)
        {
            //160115
            string useridonbehalf = Request.QueryString["UserID"].ToString();
            string loginuserid = Request.Cookies.Get("eReimUserID").Value.ToString();
            //if (useridonbehalf != loginuserid)
            //{
            //    return;
            //}

            cs.DBCommand dbc = new cs.DBCommand();
            DataTable dtbudget = new DataTable();
            //StoreBudget添加Field
            //StoreBudget.Reader[0].Fields.Clear();
            StoreBudget.Reader[0].Fields.Add("Year", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("EName", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("COACode", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("Current", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("PU", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("PB", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("PPercent", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("DU", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("DB", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("DPercent", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("SU", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("SB", RecordFieldType.String);
            StoreBudget.Reader[0].Fields.Add("SPercent", RecordFieldType.String);

            //如果不是复制而来,Status=2或者3
            string ID = "";
            if (!String.IsNullOrEmpty(Request.QueryString["RequestID"]))
            {
                ID = Request.QueryString["RequestID"].ToString();
                DataTable dtnn = new DataTable();
                string sqlbu = "select Year,EName,COACode,LocalAmount as [Current],PU,PB,PPercent,DU,DB,DPercent,SU,SB,SPercent from Budget_Complete where FormType='G' and RequestID=" + ID;
                dtnn = dbc.GetData("eReimbursement", sqlbu);
                if (dtnn.Rows.Count > 0)
                {
                    dtbudget = dtnn;
                    bool PB = false, DB = false, SB = false;
                    //计算%,取得名称,预算转换为本地汇率
                    for (int i = 0; i < dtbudget.Rows.Count; i++)
                    {
                        if (Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()) != 0)//如果Budget不为0,则计算%
                        {
                            if (!PB)
                            {
                                PB = true;
                            }
                        }
                        if (Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()) != 0)//如果Budget不为0,则计算%
                        {
                            if (!DB)
                            {
                                DB = true;
                            }
                        }
                        if (Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()) != 0)//如果Budget不为0,则计算%
                        {
                            if (!SB)
                            {
                                SB = true;
                            }
                        }
                    }
                    //添加数据列
                    var cm = GridPanelBudget.ColumnModel;
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "EName",
                        Header = "Expense Item",
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "Current",
                        Header = "Current",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "Year",
                        Header = "Budget Year",
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    //显示个人预算部分
                    if (PB)
                    {
                        cm.Columns.Add(new Column
                        {
                            DataIndex = "PU",
                            Header = "Personal Used",
                            Renderer = new Renderer { Fn = "GetNumber" },
                            Sortable = false,
                            Resizable = false,
                            MenuDisabled = true,
                            Width = 100
                        });
                        cm.Columns.Add(new Column
                        {
                            DataIndex = "PB",
                            Header = "Personal Budget",
                            Renderer = new Renderer { Fn = "GetNumber" },
                            Sortable = false,
                            Resizable = false,
                            MenuDisabled = true,
                            Width = 100
                        });
                        cm.Columns.Add(new Column
                        {
                            DataIndex = "PPercent",
                            Header = "%(Current+Used/Budget)",
                            Renderer = new Renderer { Fn = "GetNumberPercent" },
                            Sortable = false,
                            Resizable = false,
                            MenuDisabled = true,
                            Width = 100
                        });
                    }
                    if (DB)
                    {
                        cm.Columns.Add(new Column
                        {
                            DataIndex = "DU",
                            Header = "Department Used",
                            Renderer = new Renderer { Fn = "GetNumber" },
                            Sortable = false,
                            Resizable = false,
                            MenuDisabled = true,
                            Width = 100
                        });
                        cm.Columns.Add(new Column
                        {
                            DataIndex = "DB",
                            Header = "Department Budget",
                            Renderer = new Renderer { Fn = "GetNumber" },
                            Sortable = false,
                            Resizable = false,
                            MenuDisabled = true,
                            Width = 100
                        });
                        cm.Columns.Add(new Column
                        {
                            DataIndex = "DPercent",
                            Header = "%(Current+Used/Budget)",
                            Renderer = new Renderer { Fn = "GetNumberPercent" },
                            Sortable = false,
                            Resizable = false,
                            MenuDisabled = true,
                            Width = 100
                        });
                    }
                    if (SB)
                    {
                        cm.Columns.Add(new Column
                        {
                            DataIndex = "SU",
                            Header = "Unit Used",
                            Renderer = new Renderer { Fn = "GetNumber" },
                            Sortable = false,
                            Resizable = false,
                            MenuDisabled = true,
                            Width = 100
                        });
                        cm.Columns.Add(new Column
                        {
                            DataIndex = "SB",
                            Header = "Unit Budget",
                            Renderer = new Renderer { Fn = "GetNumber" },
                            Sortable = false,
                            Resizable = false,
                            MenuDisabled = true,
                            Width = 100
                        });
                        cm.Columns.Add(new Column
                        {
                            DataIndex = "SPercent",
                            Header = "%(Current+Used/Budget)",
                            Renderer = new Renderer { Fn = "GetNumberPercent" },
                            Sortable = false,
                            Resizable = false,
                            MenuDisabled = true,
                            Width = 100
                        });
                    }
                }
            }
            else
            {
                string station_applyperson = ""; string costcenter_applyperson = ""; string dept_applyperson = "";
                DataSet ds_apply = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(Request.QueryString["apply_userid"].ToString());
                if (ds_apply.Tables[0].Rows.Count == 1)
                {
                    DataTable dt_apply = ds_apply.Tables[0];
                    dept_applyperson = dt_apply.Rows[0]["DepartmentName"].ToString();
                    station_applyperson = dt_apply.Rows[0]["stationCode"].ToString();
                    costcenter_applyperson = dt_apply.Rows[0]["CostCenter"].ToString();
                }

                string userid = Request.QueryString["UserID"].ToString();
                string ostation = ""; string station = ""; string department = "";
                DataSet ds2 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(userid);
                if (ds2.Tables[0].Rows.Count == 1)
                {
                    DataTable dt1 = ds2.Tables[0];
                    ostation = dt1.Rows[0]["CostCenter"].ToString();//记录用户预算站点,即CostCenter
                    station = dt1.Rows[0]["stationCode"].ToString();//记录用户所在站点
                    department = dt1.Rows[0]["CRPDepartmentName"].ToString();
                }

                decimal rate = 1;//记录用户币种与预算站点币种汇率
                string CurLocal = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(costcenter_applyperson);
                //检查是否本地维护过特殊币种
                DataTable dttemp = new DataTable();
                string sqltemp = "select * from ESUSER where Userid='" + Request.QueryString["apply_userid"].ToString() + "'";
                dttemp = dbc.GetData("eReimbursement", sqltemp);
                if (dttemp.Rows.Count > 0)
                {
                    CurLocal = dttemp.Rows[0]["Currency"].ToString();//如果单独设置了币种
                }
                string CurBudget = DIMERCO.SDK.Utilities.LSDK.GetStationCurrencyByCode(ostation);
                //if (CurLocal != CurBudget)
                //{
                //    rate = DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(CurBudget, CurLocal, Convert.ToInt16(dtA.Rows[0]["Year"].ToString()));
                //}


                dtbudget.Columns.Add("Year", typeof(System.String));//区分跨年情况
                dtbudget.Columns.Add("EName", typeof(System.String));
                dtbudget.Columns.Add("COACode", typeof(System.String));
                dtbudget.Columns.Add("Current", typeof(System.Decimal));
                dtbudget.Columns.Add("PU", typeof(System.Decimal));
                dtbudget.Columns.Add("PB", typeof(System.Decimal));
                dtbudget.Columns.Add("PPercent", typeof(System.Decimal));
                dtbudget.Columns.Add("DU", typeof(System.Decimal));
                dtbudget.Columns.Add("DB", typeof(System.Decimal));
                dtbudget.Columns.Add("DPercent", typeof(System.Decimal));
                dtbudget.Columns.Add("SU", typeof(System.Decimal));
                dtbudget.Columns.Add("SB", typeof(System.Decimal));
                dtbudget.Columns.Add("SPercent", typeof(System.Decimal));



                //基础数据
                DataTable dtA = new DataTable();
                dtA.Columns.Add("EName", typeof(System.String));
                dtA.Columns.Add("COACode", typeof(System.String));
                dtA.Columns.Add("Amount", typeof(System.Decimal));
                dtA.Columns.Add("Year", typeof(System.String));

                JavaScriptSerializer ser = new JavaScriptSerializer();
                List<DetailExpense> Details = ser.Deserialize<List<DetailExpense>>(detailjson);
                foreach (DetailExpense detail in Details)
                {
                    DataRow dr = dtA.NewRow();
                    string EName = "";
                    if (detail.Type == "E")
                    {
                        EName = "Entertainment";
                    }
                    else if (detail.Type == "T")
                    {
                        EName = "Transportation";
                    }
                    else if (detail.Type == "C")
                    {
                        EName = "Communication";
                    }
                    else
                    {
                        EName = detail.COAName;
                    }
                    dr["EName"] = EName;
                    dr["COACode"] = detail.AccountCode;
                    dr["Amount"] = Convert.ToDecimal(detail.Amount);
                    if (detail.Type == "C")
                    {
                        dr["Year"] = Convert.ToDateTime(detail.EffectTime).Year.ToString();
                    }
                    else
                    {
                        dr["Year"] = Convert.ToDateTime(detail.Tdate).Year.ToString();
                    }
                    dtA.Rows.Add(dr);
                }
                //合计数据
                DataTable dtB = new DataTable();
                dtB.Columns.Add("EName", typeof(System.String));
                dtB.Columns.Add("COACode", typeof(System.String));
                dtB.Columns.Add("Amount", typeof(System.Decimal));
                dtB.Columns.Add("Year", typeof(System.String));
                for (int i = 0; i < dtA.Rows.Count; i++)
                {
                    bool er = false;
                    for (int j = 0; j < dtB.Rows.Count; j++)
                    {
                        if (dtB.Rows[j]["COACode"].ToString() == dtA.Rows[i]["COACode"].ToString() && dtB.Rows[j]["Year"].ToString() == dtA.Rows[i]["Year"].ToString())//已有记录
                        {
                            er = true;
                            break;
                        }
                    }
                    if (!er)//不存在重复记录
                    {
                        DataRow dr = dtB.NewRow();
                        dr["EName"] = dtA.Rows[i]["EName"].ToString();
                        dr["COACode"] = dtA.Rows[i]["COACode"].ToString();
                        dr["Amount"] = dtA.Compute("Sum(Amount)", "Year = " + dtA.Rows[i]["Year"].ToString() + " and COACode = " + dtA.Rows[i]["COACode"].ToString());
                        dr["Year"] = dtA.Rows[i]["Year"].ToString();
                        dtB.Rows.Add(dr);
                    }
                }
                string accountcode = "";
                for (int g = 0; g < dtB.Rows.Count; g++)
                {
                    if (Convert.ToDecimal(dtB.Rows[g]["Amount"].ToString()) != 0)
                    {
                        DataRow dr = dtbudget.NewRow();
                        dr["EName"] = dtB.Rows[g]["EName"].ToString();
                        dr["Current"] = Convert.ToDecimal(dtB.Rows[g]["Amount"].ToString());
                        dr["COACode"] = dtB.Rows[g]["COACode"].ToString();
                        dr["Year"] = dtB.Rows[g]["Year"].ToString();
                        accountcode = dtB.Rows[g]["COACode"].ToString();
                        DataTable dtC = new DataTable();
                        dtC = Comm.RtnEB(userid, department, ostation, ostation, accountcode, dtB.Rows[g]["Year"].ToString(), "1");
                        for (int i = 0; i < dtC.Rows.Count; i++)
                        {
                            if (dtC.Rows[i]["Type"].ToString() == "全年个人")
                            {
                                dr["PU"] = Convert.ToDecimal(dtC.Rows[i]["Used"].ToString());
                                dr["PB"] = Convert.ToDecimal(dtC.Rows[i]["Budget"].ToString());
                            }
                            else if (dtC.Rows[i]["Type"].ToString() == "全年部门")
                            {
                                dr["DU"] = Convert.ToDecimal(dtC.Rows[i]["Used"].ToString());
                                dr["DB"] = Convert.ToDecimal(dtC.Rows[i]["Budget"].ToString());
                            }
                            else if (dtC.Rows[i]["Type"].ToString() == "全年站点")
                            {
                                dr["SU"] = Convert.ToDecimal(dtC.Rows[i]["Used"].ToString());
                                dr["SB"] = Convert.ToDecimal(dtC.Rows[i]["Budget"].ToString());
                            }
                        }
                        dtbudget.Rows.Add(dr);
                    }
                }
                bool UnBudget = false;
                bool PB = false, DB = false, SB = false;
                //计算%,取得名称,转为本地币种汇率,增加列记录Currency为邮件准备
                dtbudget.Columns.Add("Currency", typeof(System.String));
                for (int i = 0; i < dtbudget.Rows.Count; i++)
                {
                    dtbudget.Rows[i]["Currency"] = CurLocal;
                    if (CurLocal != CurBudget)
                    {
                        rate = DIMERCO.SDK.Utilities.LSDK.GetBudgetConverRate(CurLocal, CurBudget, Convert.ToInt16(dtbudget.Rows[i]["Year"].ToString()));
                    }
                    if (Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()) != 0)//如果Budget不为0,则计算%
                    {
                        decimal PPercent = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["PU"].ToString()) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()), 2);
                        dtbudget.Rows[i]["PPercent"] = PPercent;
                        if (!UnBudget)
                        {
                            if (PPercent > 100)
                            {
                                UnBudget = true;
                            }
                        }
                        if (!PB)
                        {
                            PB = true;
                        }
                    }
                    if (Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()) != 0)//如果Budget不为0,则计算%
                    {
                        decimal DPercent = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["DU"].ToString()) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()), 2);
                        dtbudget.Rows[i]["DPercent"] = DPercent;
                        if (!UnBudget)
                        {
                            if (DPercent > 100)
                            {
                                UnBudget = true;
                            }
                        }
                        if (!DB)
                        {
                            DB = true;
                        }
                    }
                    if (Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()) != 0)//如果Budget不为0,则计算%
                    {
                        decimal SPercent = System.Math.Round(Convert.ToDecimal(dtbudget.Rows[i]["SU"].ToString()) * 100 / Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()), 2);
                        dtbudget.Rows[i]["SPercent"] = SPercent;
                        if (!UnBudget)
                        {
                            if (SPercent > 100)
                            {
                                UnBudget = true;
                            }
                        }
                        if (!SB)
                        {
                            SB = true;
                        }
                    }
                    if (Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()) == 0 && Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()) == 0 && Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()) == 0)
                    {
                        //如果个人,部门,站点,全未分配预算,则判断为Un-Budget
                        if (!UnBudget)
                        {
                            UnBudget = true;
                        }
                    }
                    dtbudget.Rows[i]["PU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["PU"].ToString()), 2);
                    dtbudget.Rows[i]["PB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["PB"].ToString()), 2);
                    dtbudget.Rows[i]["DU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["DU"].ToString()), 2);
                    dtbudget.Rows[i]["DB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["DB"].ToString()), 2);
                    dtbudget.Rows[i]["SU"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["SU"].ToString()), 2);
                    dtbudget.Rows[i]["SB"] = System.Math.Round(rate * Convert.ToDecimal(dtbudget.Rows[i]["SB"].ToString()), 2);
                }

                //添加数据列
                var cm = GridPanelBudget.ColumnModel;
                cm.Columns.Clear();
                cm.Columns.Add(new Column
                {
                    DataIndex = "EName",
                    Header = "Expense Item",
                    Sortable = false,
                    Resizable = false,
                    MenuDisabled = true,
                    Width = 160
                });
                cm.Columns.Add(new Column
                {
                    DataIndex = "Current",
                    Header = "Current",
                    Renderer = new Renderer { Fn = "GetNumber" },
                    Sortable = false,
                    Resizable = false,
                    MenuDisabled = true,
                    Width = 100
                });
                cm.Columns.Add(new Column
                {
                    DataIndex = "Year",
                    Header = "Budget Year",
                    Sortable = false,
                    Resizable = false,
                    MenuDisabled = true,
                    Width = 100
                });
                //显示个人预算部分
                if (PB)
                {
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "PU",
                        Header = "Personal Used",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "PB",
                        Header = "Personal Budget",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "PPercent",
                        Header = "%(Used/Budget)",
                        Renderer = new Renderer { Fn = "GetNumberPercent" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                }
                if (DB)
                {
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "DU",
                        Header = "Department Used",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "DB",
                        Header = "Department Budget",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "DPercent",
                        Header = "%(Used/Budget)",
                        Renderer = new Renderer { Fn = "GetNumberPercent" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                }
                if (SB)
                {
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "SU",
                        Header = "Unit Used",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "SB",
                        Header = "Unit Budget",
                        Renderer = new Renderer { Fn = "GetNumber" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                    cm.Columns.Add(new Column
                    {
                        DataIndex = "SPercent",
                        Header = "%(Used/Budget)",
                        Renderer = new Renderer { Fn = "GetNumberPercent" },
                        Sortable = false,
                        Resizable = false,
                        MenuDisabled = true,
                        Width = 100
                    });
                }
            }
            
            StoreBudget.DataSource = dtbudget;
            StoreBudget.DataBind();
            GridPanelBudget.Render();
            
        }
        /// <summary>
        /// 获取预算
        /// </summary>
        /// <param name="command"></param>
        /// <param name="GetDetailOrUpdate"></param>
        /// <param name="sum"></param>
        [DirectMethod]
        public void LoadBG(string command, string GetDetailOrUpdate,decimal sum)
        {
            string userid = Request.QueryString["UserID"].ToString();
            string dpt = "";
            string ostation = "";
            string username = "";
            DataSet ds1 = DIMERCO.SDK.Utilities.LSDK.getUserProfilebyUserList(userid);
            if (ds1.Tables[0].Rows.Count == 1)
            {
                DataTable dt1 = ds1.Tables[0];
                //dpt = dt1.Rows[0]["DepartmentName"].ToString();
                ostation = dt1.Rows[0]["stationCode"].ToString();
                username = dt1.Rows[0]["fullname"].ToString();
                dpt = dt1.Rows[0]["CRPDepartmentName"].ToString();
                cs.DBCommand dbc = new cs.DBCommand();
                DataTable dttemp = new DataTable();
                string sqltemp = "select * from ESUSER where Userid='" + userid + "'";
                dttemp = dbc.GetData("eReimbursement", sqltemp);
                if (dttemp.Rows.Count > 0)
                {
                    ostation = dttemp.Rows[0]["Station"].ToString();
                }
            }
            string tstation = cbxTStation.Value == null ? ostation : cbxTStation.Value.ToString();
            string accountcode = "62011900";
            string Years = Convert.ToDateTime(Convert.ToDateTime(dfDate.Value.ToString())).Year.ToString();
            string month = Convert.ToDateTime(Convert.ToDateTime(dfDate.Value.ToString())).Month.ToString();

            DataTable dt = new DataTable();
            dt = Comm.RtnEB(userid, dpt, ostation, tstation, accountcode, Years, month);
            if (dt != null && dt.Rows.Count > 0)
            {
                LabelStationBG.Text = "Station(YTD): NA";
                LabelDepartmentBG.Text = "Department(YTD): NA";
                LabelPersonBG.Text = "Person(YTD): NA";
                decimal stationbudget = 0, departmentbudget = 0, personbudget = 0, StationUsed = 0;
                LabelCOA.Text = "Transportation:";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    decimal budget = 0, used = 0;
                    budget = Convert.ToDecimal(System.Math.Round(Convert.ToDouble(dt.Rows[i]["Budget"].ToString()) * (DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(tstation) / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(ostation)), 2));
                    used = Convert.ToDecimal(System.Math.Round(Convert.ToDouble(dt.Rows[i]["Used"].ToString()) * (DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(tstation) / DIMERCO.SDK.Utilities.LSDK.GetLatestStationUSDConvertRate(ostation)), 2));
                    if (dt.Rows[i]["Type"].ToString() == "全年站点")
                    {
                        //stationbudget = Convert.ToDecimal(dt.Rows[i]["Budget"].ToString());
                        //StationUsed = Convert.ToDecimal(dt.Rows[i]["Used"].ToString());
                        //string stationbg = dt.Rows[i]["Station"].ToString() + "(YTD): " + Convert.ToDecimal(dt.Rows[i]["Used"].ToString()).ToString("#,##0.00") + "/" + Convert.ToDecimal(dt.Rows[i]["Budget"].ToString()).ToString("#,##0.00");
                        //LabelStationBG.Text = stationbg;
                        stationbudget = budget;
                        StationUsed = used;
                        string stationbg = dt.Rows[i]["Station"].ToString() + "(YTD): " + used.ToString("#,##0.00") + "/" + budget.ToString("#,##0.00");
                        LabelStationBG.Text = stationbg;

                    }
                    else if (dt.Rows[i]["Type"].ToString() == "全年部门")
                    {
                        //departmentbudget = Convert.ToDecimal(dt.Rows[i]["Budget"].ToString());
                        //string stationbg = dpt + "(YTD): " + Convert.ToDecimal(dt.Rows[i]["Used"].ToString()).ToString("#,##0.00") + "/" + Convert.ToDecimal(dt.Rows[i]["Budget"].ToString()).ToString("#,##0.00");
                        //LabelDepartmentBG.Text = stationbg;
                        departmentbudget = budget;
                        string stationbg = dpt + "(YTD): " + used.ToString("#,##0.00") + "/" + budget.ToString("#,##0.00");
                        LabelDepartmentBG.Text = stationbg;
                    }
                    else if (dt.Rows[i]["Type"].ToString() == "全年个人")
                    {
                        //personbudget = Convert.ToDecimal(dt.Rows[i]["Budget"].ToString());
                        //string stationbg = username + "(YTD): " + Convert.ToDecimal(dt.Rows[i]["Used"].ToString()).ToString("#,##0.00") + "/" + Convert.ToDecimal(dt.Rows[i]["Budget"].ToString()).ToString("#,##0.00");
                        //LabelPersonBG.Text = stationbg;
                        personbudget = budget;
                        string stationbg = username + "(YTD): " + used.ToString("#,##0.00") + "/" + budget.ToString("#,##0.00");
                        LabelPersonBG.Text = stationbg;
                    }
                }
                if (stationbudget == 0 && departmentbudget == 0 && personbudget == 0)
                {
                    X.AddScript("Ext.Msg.show({ title: 'Message', msg: 'No budget,please check with Account.', buttons: { ok: 'Ok' }, fn: function (btn) { return false; } });");
                    return;
                }
                //else
                //{
                //    if (GetDetailOrUpdate == "Update")//修改明细,则允许更新Budget状态
                //    {
                //        //获取当前的预算状况,保存到BudgetCurrent
                //        decimal fw = 6.1M;//用户币种对美元汇率
                //        if (cbxBudget.Checked)//强制预算外
                //        {
                //            if (sum + StationUsed - stationbudget > fw * 500)
                //            {
                //                hdBudgetCurrent.Value = "1";

                //                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                //                {
                //                    LabelBudget.Text = "超过预算500美元以上.";
                //                }
                //                else
                //                {
                //                    LabelBudget.Text = "Over Budget up to $500.";
                //                }
                //            }
                //            else
                //            {
                //                hdBudgetCurrent.Value = "0";
                //                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                //                {
                //                    LabelBudget.Text = "预算内";
                //                }
                //                else
                //                {
                //                    LabelBudget.Text = "In Budget";
                //                }
                //            }
                //        }
                //        else
                //        {
                //            if (Convert.ToDecimal(nfAmount1.Text) + sum + StationUsed - stationbudget > fw * 500)
                //            {
                //                hdBudgetCurrent.Value = "1";

                //                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                //                {
                //                    LabelBudget.Text = "超预算";
                //                }
                //                else
                //                {
                //                    LabelBudget.Text = "Over Budget";
                //                }
                //            }
                //            else
                //            {
                //                hdBudgetCurrent.Value = "0";
                //                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                //                {
                //                    LabelBudget.Text = "预算内";
                //                }
                //                else
                //                {
                //                    LabelBudget.Text = "In Budget";
                //                }
                //            }
                //        }
                        
                //        X.AddScript("UpdateList('" + command + "');");
                //    }
                //    else//查看GetDetail
                //    {
                //        if (Request.QueryString["Draft"] != null)
                //        {
                //            if (Request.QueryString["Draft"].ToString() == "0")
                //            {
                //                decimal fw = 6.1M;//用户币种对美元汇率
                //                if (sum + StationUsed - stationbudget > fw * 500)
                //                {
                //                    //hdBudgetCurrent.Value = "1";
                //                    if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                //                    {
                //                        LabelBudget.Text = "超预算";
                //                    }
                //                    else
                //                    {
                //                        LabelBudget.Text = "Over Budget";
                //                    }
                //                }
                //                else
                //                {
                //                    //hdBudgetCurrent.Value = "0";
                //                    if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                //                    {
                //                        LabelBudget.Text = "预算内";
                //                    }
                //                    else
                //                    {
                //                        LabelBudget.Text = "In Budget";
                //                    }
                //                }
                //            }
                //        }
                //        else
                //        {
                //            decimal fw = 6.1M;//用户币种对美元汇率
                //            if (sum + StationUsed - stationbudget > fw * 500)
                //            {
                //                //hdBudgetCurrent.Value = "1";
                //                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                //                {
                //                    LabelBudget.Text = "超预算";
                //                }
                //                else
                //                {
                //                    LabelBudget.Text = "Over Budget";
                //                }
                //            }
                //            else
                //            {
                //                //hdBudgetCurrent.Value = "0";
                //                if (Request.Cookies["lang"] != null && Request.Cookies["lang"].Value.ToLower() == "zh-cn")
                //                {
                //                    LabelBudget.Text = "预算内";
                //                }
                //                else
                //                {
                //                    LabelBudget.Text = "In Budget";
                //                }
                //            }
                //        }
                //    }
                //}
            }
            else
            {
                //hdBudget.Value = "0";
                ErrorHandleNojump("Data error.");
                return;
            }
        }
        protected void ErrorHandleNojump(string msg)
        {
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Message",
                Message = msg,
                Buttons = MessageBox.Button.OK,
                Width = 250,
                Icon = MessageBox.Icon.WARNING
            });
        }
        protected void GetCustomer(object sender, DirectEventArgs e)
        {
            DataSet getCostCenterBYStationCode = DIMERCO.SDK.Utilities.LSDK.GetCustomerInfo(X.GetValue("cbxCustomer"), 10);
            DataTable dtCOACenter = (DataTable)getCostCenterBYStationCode.Tables[0];
            DataTable dtCOACenternew = new DataTable();
            dtCOACenternew.Columns.Add("cityID", System.Type.GetType("System.String"));
            dtCOACenternew.Columns.Add("cityCode", System.Type.GetType("System.String"));
            for (int c = 0; c < dtCOACenter.Rows.Count; c++)
            {
                DataRow dr = dtCOACenternew.NewRow();
                dr["cityID"] = dtCOACenter.Rows[c][2].ToString();
                dr["cityCode"] = dtCOACenter.Rows[c][1].ToString();
                dtCOACenternew.Rows.Add(dr);
            }
            StoreCustomer.DataSource = dtCOACenternew;
            StoreCustomer.DataBind();
        }
        [DirectMethod]
        public void GetVendor(string VendorCode)
        {
            LabelCustomerName.Text = "";
            if (!string.IsNullOrEmpty(VendorCode))
            {
                DataSet getCostCenterBYStationCode = DIMERCO.SDK.Utilities.LSDK.GetCustomerInfo(VendorCode, 1);
                DataTable dtCOACenter = (DataTable)getCostCenterBYStationCode.Tables[0];
                if (dtCOACenter.Rows.Count > 0)
                {
                    LabelCustomerName.Text = dtCOACenter.Rows[0][2].ToString();
                }
            }
        }
    }
}