using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Win32;
namespace eReimbursement
{
    public partial class ApplyEntertainment : System.Web.UI.Page
    {
        private static StringBuilder netOutput = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!X.IsAjaxRequest)
            {
                this.Store1.DataSource = new object[]
            {
                new object[] { "2011-12-01", "微软", "999999", "吃饭","Bill Gates", 57.1,"总吃饭","Weiruan","Weiruan1" },
                new object[] { "2011-12-01", "微软", "999999", "吃饭","Bill Gates", 57.1,"总吃饭","Weiruan","Weiruan1" },
                new object[] { "2011-12-01", "微软", "999999", "吃饭","Bill Gates", 57.1,"总吃饭","Weiruan","Weiruan1" },
                new object[] { "2011-12-01", "微软", "999999", "吃饭","Bill Gates", 57.1,"总吃饭","Weiruan","Weiruan1" },
                new object[] { "2011-12-01", "微软", "999999", "吃饭","Bill Gates", 57.1,"总吃饭","Weiruan","Weiruan1" },
                new object[] { "2011-12-01", "微软", "999999", "吃饭","Bill Gates", 57.1,"总吃饭","Weiruan","Weiruan1" }
            };

                this.Store1.DataBind();
            }
            //string cmd;
            //ProcessStartInfo startinfo;
            //Process process;
            //string rarPath = Server.MapPath("/") + "obj\\Debug";
            //string path = rarPath + "\\100KZ712";
            //try
            //{
            //    //regkey = Registry.ClassesRoot.OpenSubKey(@"Applications\WinRAR.exe\shell\open\command");
            //    //regvalue = regkey.GetValue("");
            //    //rarexe = regvalue.ToString();
            //    //regkey.Close();
            //    //rarexe = rarexe.Substring(1, rarexe.Length - 7);
            //    Directory.CreateDirectory(path);
            //    //解压缩命令，相当于在要压缩文件(rarName)上点右键 ->WinRAR->解压到当前文件夹  
            //    cmd = string.Format("lb {0}", "100KZ712.rar");
            //    startinfo = new ProcessStartInfo();
            //    startinfo.FileName = Server.MapPath("/") + "obj\\Debug\\Rar.exe";
            //    startinfo.Arguments = cmd;
            //    startinfo.WindowStyle = ProcessWindowStyle.Hidden;
            //    startinfo.WorkingDirectory = rarPath;
            //    startinfo.RedirectStandardOutput = true;
            //    startinfo.UseShellExecute = false;
            //    process = new Process();
            //    process.StartInfo = startinfo;
            //    process.Start();
            //    StreamReader reader = process.StandardOutput;//截取输出流
            //    string str = reader.ReadToEnd();
            //    process.WaitForExit();
            //    process.Close();
            //}
            //catch (Exception)
            //{
            //    throw;
            //} 
        }
    }
}