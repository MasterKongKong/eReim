using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.IO;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.GZip;

namespace eReimbursement.cs
{
    public class ZipFloClass
    {
        public void ZipFile(string strFile, string strZip)
        {
            if (strFile[strFile.Length - 1] != Path.DirectorySeparatorChar)
                strFile += Path.DirectorySeparatorChar;
            ZipOutputStream s = new ZipOutputStream(File.Create(strZip));
            s.SetLevel(6); // 0 - store only to 9 - means best compression
            zip(strFile, s, strFile);
            s.Finish();
            s.Close();
        }

        private void zip(string strFile, ZipOutputStream s, string staticFile)
        {
            if (strFile[strFile.Length - 1] != Path.DirectorySeparatorChar) strFile += Path.DirectorySeparatorChar;
            Crc32 crc = new Crc32();
            string[] filenames = Directory.GetFileSystemEntries(strFile);
            foreach (string file in filenames)
            {
                if (Directory.Exists(file))
                {
                    zip(file, s, staticFile);
                }
                else // 否则直接压缩文件
                {
                    //打开压缩文件
                    FileStream fs = File.OpenRead(file);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    string tempfile = file.Substring(staticFile.LastIndexOf("\\") + 1);
                    ZipEntry entry = new ZipEntry(tempfile);
                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                }
            }
        }
        public void ZipSingleFile(string strOldFileName,string strOldFilePath, string strZipFile)
        {
            ZipOutputStream s = new ZipOutputStream(File.Create(strZipFile));
            s.SetLevel(6); // 0 - store only to 9 - means best compression

            byte[] buffer = new byte[4096];
            ZipEntry entry = new ZipEntry(strOldFileName);
            entry.DateTime = DateTime.Now;
            s.PutNextEntry(entry);
            using (FileStream fs = File.OpenRead(strOldFilePath))
            {
                int sourceBytes;
                do
                {
                    sourceBytes = fs.Read(buffer, 0, buffer.Length);
                    s.Write(buffer, 0, sourceBytes);
                } while (sourceBytes > 0);
            }
            s.Finish();
            s.Close(); 


            //Crc32 crc = new Crc32();
            ////打开压缩文件
            //FileStream fs = File.OpenRead(strOldFilePath);
            //byte[] buffer = new byte[fs.Length];
            //fs.Read(buffer, 0, buffer.Length);
            //ZipEntry entry = new ZipEntry(strOldFileName);
            //entry.DateTime = DateTime.Now;
            //entry.Size = fs.Length;
            //fs.Close();
            //crc.Reset();
            //crc.Update(buffer);
            //entry.Crc = crc.Value;
            //s.PutNextEntry(entry);
            //s.Write(buffer, 0, buffer.Length);
        }
    }
}