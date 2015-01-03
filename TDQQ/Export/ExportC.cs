using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TDQQ.AE;
using TDQQ.Common;
using TDQQ.MyWindow;

namespace TDQQ.Export
{
    class ExportC : ExportBase
    {
        public ExportC() : base() { }
        public ExportC(string personDatabase, string selectFeature, string basicDatabase) : base(personDatabase, selectFeature, basicDatabase) { }

        public override bool Export()
        {
            if (!AeHelper.IsExist(PersonDatabase, SelectFeatrue + "_JZX") || !AeHelper.IsExist(PersonDatabase, SelectFeatrue + "_JZD"))
            {
                MessageBox.MessageWarning.Show("系统提示", "尚未提取界址点或界址线要素类！");
                return false;
            }
            DialogFactory dialogFactory = new DialogFactory();
            var folderPath = dialogFactory.OpenFolderDialog();
            if (string.IsNullOrEmpty(folderPath)) return false;
            Wait wait = new Wait();
            wait.SetProgressInfo(string.Empty);
            wait.SetInfoInvoke("正在导出承包地块调查表");
            var para = new Hashtable();
            para["folderPath"] = folderPath;
            para["wait"] = wait;
            para["ret"] = false;
            Thread t = new Thread(new ParameterizedThreadStart(Export));
            t.Start(para);
            wait.ShowDialog();
            t.Abort();
            return (bool)para["ret"];

        }

        private void Export(object p)
        {
            var para = p as Hashtable;
            var folderPath = para["folderPath"].ToString();
            var wait = para["wait"] as Wait;
            try
            {
                var sqlString = string.Format("Select distinct CBFBM,CBFMC From {0}", SelectFeatrue);
                var accessFactory = new AccessFactory(PersonDatabase);
                var dt = accessFactory.Query(sqlString);
                if (dt == null)
                {
                    wait.CloseWait();
                    para["ret"] = false;
                    return;
                }
                var jzxFeature = SelectFeatrue + "_JZX";
                var jxdFeature = SelectFeatrue + "_JZD";
                //依次选择每个承包方
                var rowCount = dt.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    wait.SetProgressInfo(((double)i / (double)rowCount).ToString("P"));
                    var dir = new DirectoryInfo(folderPath);
                    dir.CreateSubdirectory(dt.Rows[i][0].ToString() + "_" + dt.Rows[i][1].ToString());
                    var singleFolderPath = folderPath + @"\" + dt.Rows[i][0].ToString() + "_" + dt.Rows[i][1].ToString();
                    Export(jzxFeature, jxdFeature, singleFolderPath, dt.Rows[i]);
                }
                wait.CloseWait();
                para["ret"] = true;
            }
            catch (Exception)
            {
                wait.CloseWait();
                para["ret"] = false;
                return;
            }

        }

        private void Export(string jzxFeature, string jzdFeature, string singleFolderPath, System.Data.DataRow row)
        {
            var sqlString = string.Format("Select OBJECTID,CBFMC,DKBM,DKMC From {0} where trim(CBFBM)='{1}' ", SelectFeatrue,
                        row[0].ToString());
            var accessFactory = new AccessFactory(PersonDatabase);
            var dtFields = accessFactory.Query(sqlString);
            for (int j = 0; j < dtFields.Rows.Count; j++)
            {
                var excelUrl = singleFolderPath + @"\" + dtFields.Rows[j][2].ToString() + @"_" + dtFields.Rows[j][3].ToString() + ".xls";
                int fid = (int)dtFields.Rows[j][0];
                Tools4Jz.CreateOneCTable(PersonDatabase, fid, excelUrl, SelectFeatrue, jzxFeature, jzdFeature);
            }
            dtFields.Clear();
        }

    }
}
