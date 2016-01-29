using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Data;
using System.Xml;
using System.Text;
using DIMERCO.SDK;
using org.in2bits.MyXls;
using System.Net.Mail;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using System.Configuration;
using System.Data.SqlClient;

namespace eReimbursement.backup
{
    public partial class mailtest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        protected void SendMail(object sender, DirectEventArgs e)
        {
            try
            {
                DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();
                mail.Title = "Dimerco eReimbursement";
                mail.FromDispName = "eReimbursement";
                mail.From = "DIC2@dimerco.com";
                mail.To = "andy_kang@dimerco.com,307517870@qq.com";
                mail.Body = "<div><table style='border-collapse: collapse;'><tr><td nowrap='nowrap'>所有跟Shipment相关的文件不能从Value+中导出的全部扫描后传入Document Cloud中,以备任何时间查看或者下载打印;所有跟Shipment相关的文件不能从Value+中导出的全部扫描后传入Document Cloud中,以备任何时间查看或者下载打印;所有跟Shipment相关的文件不能从Value+中导出的全部扫描后传入Document Cloud中,以备任何时间查看或者下载打印;所有跟Shipment相关的文件不能从Value+中导出的全部扫描后传入Document Cloud中,以备任何时间查看或者下载打印;</td><td>13</td></tr><tr><td>22</td><td>23</td></tr></table></div>";
                mail.Send();
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Message",
                    Message = "发送成功",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.WARNING
                });
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}