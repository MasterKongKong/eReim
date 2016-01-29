using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Configuration;
using System.Data.SqlClient;
/// <summary>
/// DBAdpate 的摘要说明
/// </summary>
namespace DBOperatorSql
{
    //public enum DBType
    //{
    //    ConnectionStrings,
    //    appSettings,
    //}

    public class DBAdapter
    {
        //DBConn db = new DBConn();
        SqlConnection sqlconn = null;
        private enum Excommand
        {
            ExecuteNonQuery,
            ExecuteScalar,
        }
        public DBAdapter()
        {
            //string ConnectionString = RegistryCache.GetValue(DataBase).ToString().Substring(18);

            //sqlconn.ConnectionString = ConnectionString;
            // sqlconn = ORMGDbConnect.GetConnectFromRegistryCache(DataBase);
            string con = System.Configuration.ConfigurationManager.ConnectionStrings["eReimbursement"].ToString();
            sqlconn = new SqlConnection(con);
        }
        //~DBAdapter()
        //{
        //    Dispose(false);
        //}
        public DataTable GetSqlTable(string SqlStr)
        {
            SqlParameter[] sqlp = new SqlParameter[0];
            return GetSqlTable(SqlStr, sqlp);

        }
        public DataTable GetSqlTable(string SqlStr, SqlParameter[] SqlPColl)
        {
            using (SqlCommand sqlcomm = new SqlCommand(SqlStr, sqlconn))
            {
                sqlcomm.Parameters.AddRange(SqlPColl);
                using (SqlDataAdapter sqldata = new SqlDataAdapter(sqlcomm))
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqldata.Fill(dt);
                        return dt;
                    }
                    catch (SqlException sqlex)
                    {
                        return dt;
                    }
                    finally
                    {

                    }
                }
            }
        }
        public DataTable GetSqlTable(string SqlStr, SqlParameterCollection SqlPColl)
        {
            SqlParameter[] sqlpc = new SqlParameter[SqlPColl.Count];
            return GetSqlTable(SqlStr, sqlpc);
        }
        /// <summary>
        /// 返回DataSet的过程
        /// </summary>
        /// <param name="SqlStr">要提交数据库的SQL语句</param>
        /// <returns></returns>
        public DataSet GetSqlDataSet(string SqlStr)
        {
            SqlParameter[] sqlp = new SqlParameter[0];
            return GetSqlDataSet(SqlStr, sqlp);
        }
        /// <summary>
        /// 返回DataSet的过程
        /// </summary>
        /// <param name="SqlStr">要提交数据库的SQL语句，可带参数</param>
        /// <param name="SqlPColl">参数集合</param>
        /// <returns></returns>
        public DataSet GetSqlDataSet(string SqlStr, SqlParameter[] SqlPColl)
        {
            DataSet ds = new DataSet("NewDataSet");
            // ds.Tables.Add(GetSqlTable(SqlStr,SqlPColl));
            using (SqlDataAdapter sqldba = new SqlDataAdapter(SqlStr, sqlconn))
            {
                sqldba.SelectCommand.Parameters.AddRange(SqlPColl);
                try
                {
                    if (sqlconn.State == ConnectionState.Closed)
                    {
                        sqlconn.Open();
                    }
                    sqldba.Fill(ds);
                    return ds;
                }
                catch (SqlException sqlex)
                {
                    Console.Write(sqlex.ToString());
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }

                }
            }
            return ds;
        }
        /// <summary>
        /// 返回DataSet的过程
        /// </summary>
        /// <param name="SqlStr">要提交数据库的SQL语句，可带参数</param>
        /// <param name="SqlColl">参数集合</param>
        /// <returns></returns>
        public DataSet GetSqlDataSet(string SqlStr, SqlParameterCollection SqlColl)
        {
            SqlParameter[] sqlp = new SqlParameter[SqlColl.Count];
            return GetSqlDataSet(SqlStr, sqlp);
        }
        public SqlDataReader GetSqlDataReader(string SqlStr, SqlParameter[] SqlColl)
        {
            using (SqlCommand sqlc = new SqlCommand(SqlStr, sqlconn))
            {
                sqlc.Parameters.AddRange(SqlColl);
                try
                {
                    if (sqlconn.State == ConnectionState.Closed)
                    {
                        sqlconn.Open();
                    }
                    return sqlc.ExecuteReader();
                }
                catch (Exception sqlex)
                {
                    HttpContext.Current.Response.Write(sqlex.Message);
                    return null;
                }
                finally
                {

                }
            }
        }
        /// <summary>
        /// 返回SqlDataAdapter的过程
        public SqlDataAdapter GetSqlDataAdapter(string sqlstr)
        {
            SqlDataAdapter da = new SqlDataAdapter(sqlstr, sqlconn);
            return da;

        }
        /// <summary>
        /// 返回SqlCommand的过程
        public SqlCommand GetSqlCommand(string sqlstr)
        {
            if (sqlconn.State == ConnectionState.Closed)
            {
                sqlconn.Open();
            }
            SqlCommand sqlc = new SqlCommand(sqlstr, sqlconn);
            return sqlc;
        }
        public SqlDataReader GetSqlDataReader(string SqlStr)
        {
            SqlParameter[] sqlp = new SqlParameter[0];
            return GetSqlDataReader(SqlStr, sqlp);
        }
        public SqlDataReader GetSqlDataReader(string SqlStr, SqlParameterCollection sqlcoll)
        {
            SqlParameter[] sqlp = new SqlParameter[sqlcoll.Count];
            return GetSqlDataReader(SqlStr, sqlp);
        }
        public int ExeSql(string SqlStr)
        {
            SqlParameter[] sqlp = new SqlParameter[0];
            return ExeSql(SqlStr, sqlp);
        }
        public int ExeSql(string SqlStr, SqlParameter[] ParColl)
        {
            if (SqlStr.StartsWith("select"))
            {
                DataTable dt = GetSqlTable(SqlStr);
                return dt.Rows.Count;
            }
            else
            {
                using (SqlCommand sqlc = new SqlCommand(SqlStr, sqlconn))
                {

                    sqlc.Parameters.AddRange(ParColl);
                    //Parm.AddRange(ParColl);

                    try
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        return sqlc.ExecuteNonQuery();
                    }
                    catch (Exception sqlex)
                    {
                        return -1;
                    }
                    finally
                    {
                        if (sqlconn.State == ConnectionState.Open)
                        {
                            sqlconn.Close();
                        }

                    }
                }
            }
        }

        public bool HasRow(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return false;
            return true;
        }
        public int ExeSql(String SqlStr, SqlParameterCollection ParColl)
        {
            SqlParameter[] sqlp = new SqlParameter[ParColl.Count];
            ParColl.CopyTo(sqlp, 0);
            return ExeSql(SqlStr, sqlp);

        }
        public string ExeSqlScalar(string SqlStr, SqlParameter[] SqlColl)
        {
            string sConnection = ConfigurationManager.ConnectionStrings["eReimbursement"].ConnectionString;
            SqlConnection scConnection = new SqlConnection(sConnection);

            SqlCommand scCommand = scConnection.CreateCommand();
            scCommand.CommandText = SqlStr;
            try
            {
                if (scConnection.State == ConnectionState.Closed)
                {
                    scConnection.Open();
                }
                return scCommand.ExecuteScalar().ToString();
            }
            catch (Exception sqlex)
            {
                ////输出预算内容
                //DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();

                //mail.FromDispName = "eReimbursement";
                //mail.From = "DIC2@dimerco.com";
                //mail.To = "Andy_Kang@dimerco.com";
                //mail.Title = "eReimbursement Bug" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:dd");
                //string body = "<div>";
                //body += "sql:" + SqlStr + "<br/>";
                //body += "error:" + sqlex.Message + "</br>";
                //body += (sqlex.InnerException == null ? "" : sqlex.InnerException.ToString()) + "</br>";
                //body += "</div>";
                //mail.Body = body;
                //mail.Send();
                return string.Empty;
            }
            finally
            {
                if (scConnection.State == ConnectionState.Open)
                {
                    scConnection.Close();
                }
            }



            //string connew = System.Configuration.ConfigurationManager.ConnectionStrings["eReimbursement"].ToString();
            //SqlConnection sqlconnew = new SqlConnection(connew);
            //using (SqlCommand sqlc = new SqlCommand(SqlStr, sqlconnew))
            //{
            //    sqlc.Parameters.AddRange(SqlColl);
            //    try
            //    {
            //        if (sqlconn.State == ConnectionState.Closed)
            //        {
            //            sqlconn.Open();
            //        }
            //        return sqlc.ExecuteScalar().ToString();
            //    }
            //    catch (Exception sqlex)
            //    {
            //        //输出预算内容
            //        DIMERCO.SDK.MailMsg mail = new DIMERCO.SDK.MailMsg();

            //        mail.FromDispName = "eReimbursement";
            //        mail.From = "DIC2@dimerco.com";
            //        mail.To = "Andy_Kang@dimerco.com";
            //        mail.Title = "eReimbursement Bug" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:dd");
            //        string body = "<div>";
            //        body += "sql:" + SqlStr + "<br/>";
            //        body += "error:" + sqlex.Message + "</br>";
            //        body += (sqlex.InnerException == null ? "" : sqlex.InnerException.ToString())+"</br>";
            //        body += "</div>";
            //        mail.Body = body;
            //        mail.Send();
            //        return string.Empty;
            //    }
            //    finally
            //    {
            //        if (sqlconn.State == ConnectionState.Open)
            //        {
            //            sqlconn.Close();
            //        }
            //    }
            //}
        }
        public string ExeSqlScalar(string SqlStr)
        {
            SqlParameter[] sqlp = new SqlParameter[0];
            return ExeSqlScalar(SqlStr, sqlp);
        }
        public string ExeSqlScalar(string SqlStr, SqlParameterCollection sqlp)
        {
            SqlParameter[] sqla = new SqlParameter[sqlp.Count];
            return ExeSqlScalar(SqlStr, sqla);
        }


        //public string ExeSqlScalarFromStoredProcedure(string StoredProcedure, SqlParameterCollection StoredPar)
        //{ 

        //}
        public void Dispose()
        {
            //db.Dispose();
            //db = null;
            if (sqlconn.State == ConnectionState.Open)
            {
                sqlconn.Close();
            }
            sqlconn.Dispose();
            sqlconn = null;

        }
        /// <summary>
        /// 执行数据库的过程,正确返回存储过程的返回值,错误返回-100,
        /// </summary>
        /// <param name="produceName"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public string ExecuteProduce(string produceName, SqlParameter[] para, string ReturnValue)
        {
            string returnvalue = (ReturnValue == string.Empty) ? "@returnValue" : ReturnValue;
            using (SqlCommand sqlcomm = new SqlCommand(produceName, sqlconn))
            {
                sqlcomm.CommandType = CommandType.StoredProcedure;
                sqlcomm.Parameters.Add(new SqlParameter(returnvalue, SqlDbType.Int, 4, ParameterDirection.ReturnValue, true, 0, 0, string.Empty, DataRowVersion.Default, null));
                sqlcomm.Parameters.AddRange(para);
                try
                {
                    if (sqlconn.State == ConnectionState.Closed)
                    {
                        sqlconn.Open();
                    }
                    sqlcomm.ExecuteNonQuery();
                    return sqlcomm.Parameters[returnvalue].Value.ToString();
                }
                catch (SqlException sqlex)
                {
                    // return Convert.ToInt16(sqlcomm.Parameters["@returnValue"]);
                    return string.Empty;
                }
                finally
                {
                    sqlcomm.Parameters.Clear();
                }
            }
        }
        /// <summary>
        /// 执行数据库的过程,正确返回存储过程的返回值,错误返回-100,
        /// </summary>
        /// <param name="produceName"></param>
        /// <param name="para"></param>
        /// <param name="returnvalue"></param>
        /// <returns></returns>
        public int ExecuteProduce(string produceName, SqlParameter[] para)
        {
            int returnInt;
            if (int.TryParse(ExecuteProduce(produceName, para, "@returnValue"), out returnInt))
            {
                return returnInt;
            }
            else
            {
                return -100;
            }
        }
        /// <summary>
        /// 通过实体名返回实体id，为空返回-1
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public string ExecuteProduce2(string produceName, SqlParameter[] para)
        {
            using (SqlCommand sqlcomm = new SqlCommand(produceName, sqlconn))
            {
                sqlcomm.CommandType = CommandType.StoredProcedure;
                sqlcomm.Parameters.Clear();
                sqlcomm.Parameters.Add(new SqlParameter("@returnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, true, 0, 0, string.Empty, DataRowVersion.Default, null));
                sqlcomm.Parameters.Add(new SqlParameter("@msg", SqlDbType.VarChar, 100, ParameterDirection.Output, true, 0, 0, string.Empty, DataRowVersion.Default, null));
                sqlcomm.Parameters.AddRange(para);
                try
                {
                    if (sqlconn.State == ConnectionState.Closed)
                    {
                        sqlconn.Open();
                    }
                    sqlcomm.ExecuteNonQuery();
                    return sqlcomm.Parameters["@msg"].Value.ToString();
                }
                catch (SqlException sqlex)
                {
                    // return Convert.ToInt16(sqlcomm.Parameters["@returnValue"]);
                    return "";
                }
                finally
                {
                    sqlconn.Close();
                }
            }
        }

        public int GetEntityIdByName(string entityName)
        {
            string rntstr;
            int rnt;
            rntstr = ExeSqlScalar("select ObjectTypeCode from dbo.Entity where name=\'" + entityName + "\'");
            if (rntstr == string.Empty)
            {
                rnt = -1;
            }
            else
            {
                rnt = Convert.ToInt16(rntstr);
            }
            return rnt;
        }

        public string GetEntityNameById(string entityid)
        {
            return ExeSqlScalar("select name from dbo.Entity where ObjectTypeCode= " + entityid);

        }

        public string GetEntityNameById(int entityid)
        {
            return GetEntityNameById(entityid.ToString());


        }
        /**/
        /// <summary>
        /// 执行存储过程返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public DataTable ExecuteSql1(string sql, string ParentID)
        {
            DataTable dt;
            try
            {
                sqlconn.Open();
                dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, sqlconn);

                SqlParameter parm = new SqlParameter("@ParentID", ParentID);
                da.SelectCommand.Parameters.Add(parm);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.Fill(dt);
                sqlconn.Close();
                return dt;
            }
            catch
            {
                sqlconn.Close();
                return dt = null;
            }
        }

    }
}