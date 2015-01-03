using System;
using TDQQ.Common;

namespace TDQQ.Export
{
    class ExportPost:ExportBase
    {
        public ExportPost(string personDatabase, string selectFeatrue, string basicDatabase)
            : base(personDatabase, selectFeatrue, basicDatabase) { }
        public override bool Export()
        {
           // return base.Export();
            var fbfdz = GetFbfdz();
            if (string.IsNullOrEmpty(fbfdz))
            {
                MessageBox.MessageWarning.Show("系统提示", "发包方信息错误");
                return false;
            }
            var cbfCount = GetCbfCount();
            var fieldCount = GetFieldCount();
            var area = GetArea();
            if (area==null)
            {
                MessageBox.MessageWarning.Show("系统提示", "宗地面积错误");
                return false;
            }
            var dialogFactory = new DialogFactory();
            var folderPath=dialogFactory.OpenFolderDialog();
            if (string.IsNullOrEmpty(folderPath)) return false;
            if (!ExportCountryPost(fbfdz,cbfCount,fieldCount,(double)area,folderPath))return false;
            if (!ExportDepartmentPost(fbfdz, cbfCount, fieldCount, (double)area, folderPath))return false;
            return true;

        }

        private bool ExportCountryPost(string fbfdz,int cbfCount,int fieldCount,double area,string folderPath)
        {
            var templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\template\村组公示公告.doc";
            var name = SplitNameFromFbfdz(fbfdz);
            var saveDocPath = folderPath + @"\" + name + "村组公示公告.doc";
            ExportWord exportWord=new ExportWord();
            exportWord.CreateNewDocument(templatePath);
            exportWord.InsertValue("户数", cbfCount.ToString());
            exportWord.InsertValue("宗地", fieldCount.ToString());
            exportWord.InsertValue("面积", area.ToString("f1"));
            exportWord.InsertValue("发包方地址1", fbfdz);
            exportWord.InsertValue("发包方地址2", fbfdz);
            exportWord.InsertValue("发包方地址3", fbfdz);
            exportWord.InsertValue("村委会", name + "村民委员会");
            exportWord.InsertValue("发包方名称", fbfdz + "村民委员会");
            exportWord.SaveDocument(saveDocPath);
            exportWord.KillWinWordProcess();
            return true;
        }

        private bool ExportDepartmentPost(string fbfdz, int cbfCount, int fieldCount, double area, string folderPath)
        {
            var templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\template\部门审核公告.doc";
            var name = SplitNameFromFbfdz(fbfdz);
            var saveDocPath = folderPath + @"\" + name + "部门审核公告.doc";
            ExportWord exportWord = new ExportWord();
            exportWord.CreateNewDocument(templatePath);
            exportWord.InsertValue("发包方地址1", fbfdz);
            exportWord.InsertValue("村庄", name);
            exportWord.InsertValue("户数", cbfCount.ToString());
            exportWord.InsertValue("宗地", fieldCount.ToString());
            exportWord.InsertValue("面积", area.ToString("f1"));
            exportWord.InsertValue("发包方地址2", fbfdz);
            exportWord.SaveDocument(saveDocPath);
            exportWord.KillWinWordProcess();
            return true;
        }
        /// <summary>
        /// 获取发包方地址
        /// </summary>
        /// <returns></returns>
        private string GetFbfdz()
        {
            var sqlString = string.Format("select FBFDZ from {0}", "FBF");
            var accessFactory = new AccessFactory(BasicDatabase);
            var dt = accessFactory.Query(sqlString);
            return dt == null ||dt.Rows.Count!=1? null : dt.Rows[0][0].ToString().Trim();
        }

        /// <summary>
        /// 从发包方地址名称中获取村的名称
        /// </summary>
        /// <param name="fbfdz"></param>
        /// <returns></returns>
        private string SplitNameFromFbfdz(string fbfdz)
        {
            var zhenIndex = fbfdz.IndexOf("镇") == -1 ? fbfdz.IndexOf("乡") : fbfdz.IndexOf("镇");
            return fbfdz.Substring(zhenIndex + 1);
        }

        /// <summary>
        ///获取该村的户主的数量
        /// </summary>
        /// <returns></returns>
        private int GetCbfCount()
        {
            var sqlString = string.Format("select Count(*) from {0}", "CBF");
            var accessFactory = new AccessFactory(BasicDatabase);
            return (int) accessFactory.ExecuteScalar(sqlString);
        }
        /// <summary>
        /// 获取宗地数目
        /// </summary>
        /// <returns></returns>
        private int GetFieldCount()
        {
            var sqlString = string.Format("select Count(*) from {0}", SelectFeatrue);
            var accessFactory = new AccessFactory(PersonDatabase);
            return (int)accessFactory.ExecuteScalar(sqlString);
        }
        /// <summary>
        /// 获取总面积
        /// </summary>
        /// <returns></returns>
        private double? GetArea()
        {
            if (!AE.AeHelper.CheckField(PersonDatabase, SelectFeatrue, "SCMJ")) return null;         
            var pAccess = new AccessFactory(PersonDatabase);
            string sqlString = string.Format("select SUM(SCMJ) from {0}", SelectFeatrue);
            return (double)pAccess.ExecuteScalar(sqlString); 
        }
    }
}
