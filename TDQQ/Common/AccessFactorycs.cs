using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using TDQQ.MessageBox;

namespace TDQQ.Common
{
    public class AccessFactory
    {

        private string _basicDatabase;
        public AccessFactory(string basicDatabase)
        {
            _basicDatabase = basicDatabase;
        }
        private OleDbConnection GetConnection()
        {
            try
            {
                string connnectString = "Provider=Microsoft.Jet.OLEDB.4.0;" + "data source=" + _basicDatabase;
                var con = new OleDbConnection(connnectString);
               
                return con;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sqlString">执行语句</param>
        /// <returns>查询结果</returns>
        public DataTable Query(string sqlString)
        {
            var con = GetConnection();
            //如果数据连接对象为空
            if (con == null)
            {
                return null;
            }
            con.Open();
            if (con.State != ConnectionState.Open)
            {
                MessageWarning.Show("系统提示", "数据库无法打开");
                return null;
            }
            try
            {
                var adapter = new OleDbDataAdapter(sqlString, con);
                var dt = new DataTable();
                adapter.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception e)
            {
                //MessageHelper.InfoMessage(e.ToString());
                con.Close();
                return null;
            }
        }
        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="sqlString">执行语句</param>
        /// <returns>语句影响的行记录,-1：操作失败</returns>
        public int Execute(string sqlString)
        {
            //数据库执行失败的提示
            const int errorState = -1;
            var con = GetConnection();
            //数据库连接对象为空
            if (con == null)
            {
                return errorState;
            }
            con.Open();
            if (con.State != ConnectionState.Open)
            {
                MessageWarning.Show("系统提示", "数据库无法打开");
                return errorState;
            }
            try
            {
                //执行操作语句
                var cmd = new OleDbCommand(sqlString, con);
                var ret = cmd.ExecuteNonQuery();
                con.Close();
                return ret;
            }
            catch (Exception e)
            {
                //System.Windows.Forms.MessageBox.Show(e.ToString());
                con.Close();
                return errorState;
            }
        }

        public object ExecuteScalar(string sqlString)
        {
            var con = GetConnection();
            //数据库连接对象为空
            if (con == null)
            {
                return null;
            }
            con.Open();
            if (con.State != ConnectionState.Open)
            {
                MessageWarning.Show("系统提示", "数据库无法打开");
                return null;
            }
            try
            {
                //执行操作语句
                var cmd = new OleDbCommand(sqlString, con);
                var ret = cmd.ExecuteScalar();
                con.Close();
                return ret;
            }
            catch (Exception e)
            {
                //System.Windows.Forms.MessageBox.Show(e.ToString());
                con.Close();
                return null;
            }
        }

      
    }
}
