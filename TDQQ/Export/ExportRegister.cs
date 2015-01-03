using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using TDQQ.Common;
using TDQQ.MyWindow;

namespace TDQQ.Export
{
    /// <summary>
    /// 登记薄
    /// </summary>
    class ExportRegister : ExportBase
    {
        public ExportRegister() : base() { }
        public ExportRegister(string personDatabase, string selectFeaure, string basicDatabase) : base(personDatabase, selectFeaure, basicDatabase) { }
        public override bool Export()
        {
            if (!CheckValid())
            {
                MessageBox.MessageWarning.Show("系统提示", "字段尚未全部添加！");
                return false;
            }   
            var dialogFactory = new DialogFactory();
            var folderPath = dialogFactory.OpenFolderDialog();
            if (string.IsNullOrEmpty(folderPath)) return false;
            var wait = new Wait();
            wait.SetInfoInvoke("正在导出登记薄");
            wait.SetProgressInfo(string.Empty);
            var para = new Hashtable();
            para["wait"] = wait;
            para["folderPath"] = folderPath;
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
            var wait = para["wait"] as Wait;
            var folderPath = para["folderPath"].ToString();
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
                var templateExcelPath = AppDomain.CurrentDomain.BaseDirectory + @"\template\登记薄.xls";
                var rowCount = dt.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    wait.SetProgressInfo(((double)i / (double)rowCount).ToString("P"));
                    var dir = new DirectoryInfo(folderPath);
                    dir.CreateSubdirectory(dt.Rows[i][0].ToString() + "_" + dt.Rows[i][1].ToString());
                    var savedDocPathContract = folderPath + @"\" + dt.Rows[i][0].ToString() + "_" +
                                               dt.Rows[i][1].ToString() + @"\" + dt.Rows[i][0].ToString() +
                                               dt.Rows[i][1].ToString() + "_登记薄.doc";
                    //var savedExcelPath = folderPath + @"\" + dt.Rows[i][0].ToString() + "_" + dt.Rows[i][1].ToString() + @"\" +
                    //                   dt.Rows[i][1].ToString() + "_插入等级中地块信息和承包方信息.xls";
                    //File.Copy(templateExcelPath, savedExcelPath, true);
                    FillDocFile(savedDocPathContract, dt.Rows[i][0].ToString());
                    //FillExcelFile(savedExcelPath, dt.Rows[i][0].ToString());
                }
                wait.CloseWait();
                para["ret"] = true;
                return;
            }
            catch (Exception ioException)
            {
                System.Windows.Forms.MessageBox.Show(ioException.ToString());
                wait.CloseWait();
                para["ret"] = false;
                return;
            }

        }

        private void FillDocFile(string savedFilePath, string cbfbm)
        {
            var dtfbf = GetFbfInfo();
            var dtCbf = GetCbfInfo(cbfbm);
            var dtCbfJtcy = GetCbfJtcyInfo(cbfbm);
            var dtField = GetFieldInfo(cbfbm);
            //如果获得数据不符合规范
            if (dtfbf == null || dtCbf == null || dtCbfJtcy == null || dtField == null || dtfbf.Rows.Count != 1 ||
                dtCbf.Rows.Count != 1) return;
            //如果家庭成员超过7人
            if (dtCbfJtcy.Rows.Count > 11)
            {
                //throw new IOException(cbfbm + "户的家庭成员数超过模板，情修改模板,并联系开发人员");
                System.Windows.Forms.MessageBox.Show(cbfbm + "该农户的地块数超过17块，请处理");
                return;
            }
            var report = new ExportWord();
            var fileDocSourth = AppDomain.CurrentDomain.BaseDirectory + @"\template\登记薄" + dtField.Rows.Count.ToString() + ".doc";
            //var fileDocSourth = @"C:\Users\feng\Desktop\登记薄17.doc";
            report.CreateNewDocument(fileDocSourth);
            report.InsertValue("经营权证号", cbfbm + "J");
            //填充发包方和承包方信息
            report.InsertValue("fbfmc", dtfbf.Rows[0][0].ToString());
            report.InsertValue("cbfmc", dtCbf.Rows[0][0].ToString());
            report.InsertValue("sfzhm", dtCbf.Rows[0][1].ToString());
            report.InsertValue("cbfdz", dtfbf.Rows[0][1].ToString());
            report.InsertValue("yzbm", dtCbf.Rows[0][2].ToString());
            report.InsertValue("lxdh", dtCbf.Rows[0][3].ToString());
            report.InsertValue("htbm", cbfbm + "J");
            //填充承包方家庭成员信息
            for (int i = 0; i < dtCbfJtcy.Rows.Count; i++)
            {
                report.InsertValue("xm" + (i + 1).ToString(), dtCbfJtcy.Rows[i][0].ToString());
                report.InsertValue("gx" + (i + 1).ToString(),
                    Transcode.CodeToRelationship(dtCbfJtcy.Rows[i][1].ToString()));
                report.InsertValue("hm" + (i + 1).ToString(), dtCbfJtcy.Rows[i][2].ToString());
                report.InsertValue("bz" + (i + 1).ToString(), dtCbfJtcy.Rows[i][3].ToString());
            }
            //填充地块信息
            double htSum = 0.0;
            double scSum = 0.0;
            for (int i = 0; i < dtField.Rows.Count; i++)
            {
                report.InsertValue("mc" + (i + 1).ToString(), dtField.Rows[i][0].ToString());
                report.InsertValue("bm" + (i + 1).ToString(), dtField.Rows[i][1].ToString());
                double htSingle, scSingle;
                if (string.IsNullOrEmpty(dtField.Rows[i][2].ToString().Trim()))
                {
                    htSingle = 0.0;
                }
                else
                {
                    htSingle = Convert.ToDouble((Convert.ToDouble(dtField.Rows[i][2].ToString())).ToString("f"));
                }
                htSum += htSingle;
                if (string.IsNullOrEmpty(dtField.Rows[i][3].ToString()))
                {
                    scSingle = 0.0;
                }
                else
                {
                    scSingle = Convert.ToDouble((Convert.ToDouble(dtField.Rows[i][3].ToString())).ToString("f"));
                }
                scSum += scSingle;
                report.InsertValue("ht" + (i + 1).ToString(), htSingle.ToString("f"));
                report.InsertValue("sc" + (i + 1).ToString(), scSingle.ToString("f"));
                report.InsertValue("sf" + (i + 1).ToString(), Transcode.CodeToSfjbnt(dtField.Rows[i][4].ToString()));
                report.InsertValue("d" + (i + 1).ToString(), dtField.Rows[i][5].ToString());
                report.InsertValue("n" + (i + 1).ToString(), dtField.Rows[i][6].ToString());
                report.InsertValue("x" + (i + 1).ToString(), dtField.Rows[i][7].ToString());
                report.InsertValue("b" + (i + 1).ToString(), dtField.Rows[i][8].ToString());
            }
            report.InsertValue("htsum", htSum.ToString("f"));
            report.InsertValue("scsum", scSum.ToString("f"));
            report.InsertValue("dkzs", dtField.Rows.Count.ToString());
            report.SaveDocument(savedFilePath);
            report.CreateNewDocument(savedFilePath);
            report.DeleteTableRow(1, 10 + dtCbfJtcy.Rows.Count + 1, 1, 11 - dtCbfJtcy.Rows.Count);
            // report.DeleteTableRow(2,dtField.Rows.Count+3,1,17-dtField.Rows.Count);
            //  report.MergeTableCells(2,2,1,dtField.Rows.Count+2,1);
            //   report.FillCellText(2,2,1,"\n 承 \n 包 \n 地 \n 块 \n 情 \n 况 \n");
            report.SaveDocument(savedFilePath);
            report.KillWinWordProcess();
        }

        private void FillExcelFile(string saveFilePath, string cbfbm)
        {
            FillSheetOne(saveFilePath, cbfbm);
            FillSheetTwo(saveFilePath, cbfbm);
        }

        private System.Data.DataTable GetFbfInfo()
        {
            AccessFactory accessFactory = new AccessFactory(BasicDatabase);
            var sqlString = string.Format("select FBFMC,FBFDZ from {0}", "FBF");
            return accessFactory.Query(sqlString);
        }

        private System.Data.DataTable GetCbfInfo(string cbfbm)
        {
            AccessFactory accessFactory = new AccessFactory(BasicDatabase);
            var sqlString = string.Format("select CBFMC,CBFZJHM,YZBM,LXDH from {0} where trim(CBFBM)='{1}'", "CBF", cbfbm);
            return accessFactory.Query(sqlString);
        }

        private System.Data.DataTable GetCbfJtcyInfo(string cbfbm)
        {
            AccessFactory accessFactory = new AccessFactory(BasicDatabase);
            var sqlString = string.Format("select CYXM,YHZGX,CYZJHM,CYBZ from {0} where trim(CBFBM)='{1}' order by YHZGX", "CBF_JTCY",
               cbfbm);
            return accessFactory.Query(sqlString);
        }
        private System.Data.DataTable GetFieldInfo(string cbfbm)
        {
            AccessFactory accessFactory = new AccessFactory(PersonDatabase);
            var sqlString =
                string.Format("select DKMC,DKBM,HTMJ,SCMJ,SFJBNT,DKDZ,DKNZ,DKXZ,DKBZ from {0} where trim(CBFBM)='{1}'",
                    SelectFeatrue, cbfbm);
            return accessFactory.Query(sqlString);
        }
        /// <summary>
        /// 填充登记薄的第一个页面
        /// </summary>
        /// <param name="saveFilePath"></param>
        /// <param name="cbfbm"></param>
        private void FillSheetOne(string saveFilePath, string cbfbm)
        {
            AccessFactory accessFactory = new AccessFactory(BasicDatabase);
            var sqlString = string.Format("select FBFMC,FBFDZ from {0}", "FBF");
            var dtFbf = accessFactory.Query(sqlString);
            if (dtFbf == null) return;
            sqlString = string.Format("select CBFMC,CBFZJHM,YZBM,LXDH from {0} where trim(CBFBM)='{1}'", "CBF", cbfbm);
            var dtCbf = accessFactory.Query(sqlString);
            if (dtCbf == null) return;
            sqlString = string.Format("select CYXM,YHZGX,CYZJHM,CYBZ from {0} where trim(CBFBM)='{1}' order by YHZGX", "CBF_JTCY",
                cbfbm);
            var dtFamily = accessFactory.Query(sqlString);
            if (dtFamily == null) return;
            using (FileStream fileStream = new FileStream(saveFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                IWorkbook workbook = new HSSFWorkbook(fileStream);
                ISheet sheet = workbook.GetSheetAt(0);
                IRow row;
                row = sheet.GetRow(0);
                row.GetCell(1).SetCellValue(dtFbf.Rows[0][0].ToString());
                row = sheet.GetRow(1);
                row.GetCell(1).SetCellValue(dtCbf.Rows[0][0].ToString());
                row = sheet.GetRow(2);
                row.GetCell(1).SetCellValue(dtCbf.Rows[0][1].ToString());
                row = sheet.GetRow(4);
                row.GetCell(1).SetCellValue(dtFbf.Rows[0][1].ToString());
                row = sheet.GetRow(5);
                row.GetCell(1).SetCellValue(dtCbf.Rows[0][2].ToString());
                row.GetCell(3).SetCellValue(dtCbf.Rows[0][3].ToString());
                row = sheet.GetRow(6);
                row.GetCell(1).SetCellValue(cbfbm + "J");
                //填充家庭成员信息
                int startRow = 10;
                for (int i = 0; i < dtFamily.Rows.Count; i++)
                {
                    row = sheet.GetRow(startRow + i);
                    row.GetCell(0).SetCellValue(dtFamily.Rows[i][0].ToString());
                    row.GetCell(1).SetCellValue(Transcode.CodeToRelationship(dtFamily.Rows[i][1].ToString()));
                    row.GetCell(2).SetCellValue(dtFamily.Rows[i][2].ToString());
                    row.GetCell(3).SetCellValue(dtFamily.Rows[i][3].ToString());
                }
                EditExcel(workbook, startRow + dtFamily.Rows.Count, 0);
                FileStream fs = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write);
                workbook.Write(fs);
                //  System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                fs.Close();
                fileStream.Close();
            }

        }

        /// <summary>
        /// 填充登记薄第二个页面
        /// </summary>
        /// <param name="saveFilePath"></param>
        /// <param name="cbfbm"></param>
        private void FillSheetTwo(string saveFilePath, string cbfbm)
        {
            AccessFactory accessFactory = new AccessFactory(PersonDatabase);
            var sqlString =
                string.Format("select DKMC,DKBM,HTMJ,SCMJ,SFJBNT,DKDZ,DKNZ,DKXZ,DKBZ from {0} where trim(CBFBM)='{1}'",
                    SelectFeatrue, cbfbm);
            var dtFields = accessFactory.Query(sqlString);
            if (dtFields == null) return;
            using (var fileStream = new FileStream(saveFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                IWorkbook workbook = new HSSFWorkbook(fileStream);
                ISheet sheet = workbook.GetSheetAt(1);
                double htmjSum = 0.0;
                double scmjSum = 0.0;
                IRow row;
                int startRow = 2;
                for (int i = 0; i < dtFields.Rows.Count; i++)
                {
                    row = sheet.GetRow(startRow + i);
                    row.GetCell(1).SetCellValue(dtFields.Rows[i][0].ToString());
                    row.GetCell(2).SetCellValue(dtFields.Rows[i][1].ToString());
                    var htmj = Convert.ToDouble(Convert.ToDouble(dtFields.Rows[i][2].ToString()).ToString("F"));
                    var scmj = Convert.ToDouble(Convert.ToDouble(dtFields.Rows[i][3].ToString()).ToString("F"));
                    htmjSum += htmj;
                    scmjSum += scmj;
                    row.GetCell(5).SetCellValue(htmj);
                    row.GetCell(6).SetCellValue(scmj);
                    row.GetCell(8).SetCellValue(Transcode.CodeToSfjbnt(dtFields.Rows[i][4].ToString()));
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("东：" + dtFields.Rows[i][5].ToString() + "\n");
                    stringBuilder.Append("南：" + dtFields.Rows[i][6].ToString() + "\n");
                    stringBuilder.Append("西：" + dtFields.Rows[i][7].ToString() + "\n");
                    stringBuilder.Append("北：" + dtFields.Rows[i][8].ToString());
                    row.GetCell(9).SetCellValue(stringBuilder.ToString());
                }
                sheet.AddMergedRegion(new CellRangeAddress(startRow - 1, startRow + dtFields.Rows.Count - 1, 0, 0));
                row = sheet.GetRow(0);
                row.GetCell(3).SetCellValue(htmjSum);
                row.GetCell(6).SetCellValue(scmjSum);
                row.GetCell(10).SetCellValue(dtFields.Rows.Count);
                row = sheet.GetRow(1);
                ICell cell = row.GetCell(0);
                cell.CellStyle = MergetStyle(workbook);
                cell.SetCellValue("承 \n 包 \n 地 \n 块 \n 情 \n 况");

                int endRow = startRow + dtFields.Rows.Count;
                for (int i = sheet.LastRowNum; i >= endRow; i--)
                {
                    row = sheet.GetRow(i);
                    sheet.RemoveRow(row);
                }
                FileStream fs = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write);
                workbook.Write(fs);
                fs.Close();
                fileStream.Close();
            }

        }
        protected override void EditExcel(IWorkbook workbook, int endRow, int sheetIndex)
        {
            //删除多余行
            ISheet sheetSource = workbook.GetSheetAt(sheetIndex);
            for (int i = sheetSource.LastRowNum; i >= endRow + 1; i--)
            {
                sheetSource.ShiftRows(i, i + 1, -1);
            }
        }

    }
}
