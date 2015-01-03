using System;
using System.Collections;
using System.IO;
using System.Threading;
using NPOI.HSSF.UserModel;
using TDQQ.Common;
using TDQQ.MyWindow;

namespace TDQQ.Export
{
    class ExportB : ExportBase
    {
        public ExportB() : base() { }

        public ExportB(string personDatabase, string selectFeatrue, string basicDatabse)
            : base(personDatabase, selectFeatrue, basicDatabse) { }
        public override bool Export()
        {
            
            var dialogFactory = new DialogFactory();
            var folderPath = dialogFactory.OpenFolderDialog();
            if (string.IsNullOrEmpty(folderPath)) return false;
            Wait wait = new Wait();
            wait.SetInfoInvoke("正在导出承包方调查表......");
            Hashtable para = new Hashtable();
            para["folderPath"] = folderPath;
            para["wait"] = wait;
            para["ret"] = false;
            Thread t = new Thread(new ParameterizedThreadStart(ExportF));
            t.Start(para);
            wait.ShowDialog();
            var ret = (bool)para["ret"];
            t.Abort();
            return ret;
        }

        private void ExportF(object p)
        {
            var para = p as Hashtable;
            var folderPath = para["folderPath"].ToString();
            var wait = para["wait"] as Wait;
            try
            {
                var sqlString = string.Format("Select CBFBM," +
                                          "CBFMC,LXDH,CBFDZ,YZBM,CBFZJLX,CBFZJHM From {0}",
                                          "CBF");
                var accessFactory = new AccessFactory(BasicDatabase);
                var dt = accessFactory.Query(sqlString);
                if (dt == null)
                {
                    para["ret"] = false;
                    wait.CloseWait();
                    return;
                }
                var templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\template\承包方调查表.xls";
                var rowCount = dt.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    var savePath = folderPath + @"\" + dt.Rows[i][0].ToString().Trim() + @"_" + dt.Rows[i][1].ToString().Trim() +
                                        @".xls";
                    wait.SetProgressInfo(((double)i / (double)rowCount).ToString("P"));
                    ExportF(templatePath, savePath, dt.Rows[i]);
                }
                para["ret"] = true;
                wait.CloseWait();
            }
            catch (Exception)
            {
                para["ret"] = false;
                wait.CloseWait();
                return;
            }


        }

        private bool ExportF(string templatePath, string savePath, System.Data.DataRow row)
        {
            File.Copy(templatePath, savePath, true);
            var sqlString = string.Format("select CYXM,YHZGX,CYZJHM,CYBZ from {0} where trim(CBFBM)='{1}' order by YHZGX", "CBF_JTCY",
                row[0].ToString());
            var accessFactory = new AccessFactory(BasicDatabase);
            var dt = accessFactory.Query(sqlString);
            if (dt == null) return false;
            using (FileStream fileStream = new FileStream(savePath, FileMode.Open, FileAccess.ReadWrite))
            {
                //设置格式要素
                var workbookSource = new HSSFWorkbook(fileStream);
                var sheetSource = workbookSource.GetSheetAt(0);
                var rowSource = sheetSource.GetRow(2);
                rowSource.GetCell(CommonHelper.Col('C')).SetCellValue(row[0].ToString().Substring(0, 14));
                rowSource.GetCell(CommonHelper.Col('I')).SetCellValue(row[0].ToString().Substring(14, 4));
                rowSource = (HSSFRow)sheetSource.GetRow(3);
                rowSource.GetCell(CommonHelper.Col('C')).SetCellValue(row[1].ToString());
                rowSource.GetCell(CommonHelper.Col('H')).SetCellValue(row[2].ToString());
                rowSource = (HSSFRow)sheetSource.GetRow(4);
                rowSource.GetCell(CommonHelper.Col('C')).SetCellValue(row[3].ToString());
                rowSource.GetCell(CommonHelper.Col('I')).SetCellValue(row[4].ToString());
                rowSource = (HSSFRow)sheetSource.GetRow(5);
                rowSource.GetCell(CommonHelper.Col('C')).SetCellValue(row[5].ToString());
                rowSource.GetCell(CommonHelper.Col('H')).SetCellValue(row[6].ToString());
                rowSource = (HSSFRow)sheetSource.GetRow(11);
                rowSource.GetCell(CommonHelper.Col('J')).SetCellValue("共" + dt.Rows.Count.ToString() + "人");
                int start_row_index = 13;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    rowSource = sheetSource.GetRow(start_row_index + i);
                    rowSource.GetCell(CommonHelper.Col('A')).SetCellValue(dt.Rows[i][0].ToString());
                    rowSource.GetCell(CommonHelper.Col('C')).SetCellValue(Transcode.CodeToRelationship(dt.Rows[i][1].ToString()));
                    rowSource.GetCell(CommonHelper.Col('E')).SetCellValue(dt.Rows[i][2].ToString());
                    rowSource.GetCell(CommonHelper.Col('H')).SetCellValue(dt.Rows[i][3].ToString());
                }
                FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write);
                workbookSource.Write(fs);
                fs.Close();
                fileStream.Close();
            }
            return true;
        }
    }
}
