using System.Data;
using TDQQ.Common;

namespace TDQQ.Import
{
    /// <summary>
    /// 导入数据基础类
    /// </summary>
    class ImportBase
    {
        /// <summary>
        /// 基础数据库的路径
        /// </summary>
        public string BasicDatabase { get; set; } 
        public ImportBase(string basicDatabase)
        {
            BasicDatabase = basicDatabase;
        }
        public ImportBase()
        {
            BasicDatabase = string.Empty;
        }
        protected bool DeleteTable(string tableName)
        {
            var sqlString = string.Format("delete from {0}", tableName);
            var accessFactory = new TDQQ.Common.AccessFactory(BasicDatabase);
            var ret = accessFactory.Execute(sqlString);
            return ret == -1 ? false : true;
        }
        protected bool InsertRow(string insertExpression)
        {
          
            var accessfactory= new Common.AccessFactory(BasicDatabase);
            var ret = accessfactory.Execute(insertExpression);
            return ret == -1 ? false : true;
        }
        /// <summary>
        /// 导入信息
        /// </summary>
        /// <returns></returns>
        public virtual bool Import()
        {
            return true;
        }

        /// <summary>
        /// 数据合法性检查
        /// </summary>
        /// <returns>是否通过检查</returns>
        public virtual bool ValidCheck(string filePath)
        {
            return true;
        }
    }
}
