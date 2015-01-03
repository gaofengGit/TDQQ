using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using TDQQ.AE;
using TDQQ.Common;
using TDQQ.MyWindow;

namespace TDQQ.Export
{
    class ExportContract
    {
        #region 属性字段
        public string PersonDatabase { get; set; }
        public string SelectFeature { get; set; }
        public string BasicDatabase { get; set; }
        #endregion

        public ExportContract(string personDatabase, string selectFeature, string basicDatabase)
        {
            PersonDatabase = personDatabase;
            SelectFeature = selectFeature;
            BasicDatabase = basicDatabase;
        }

        public bool Export()
        {
            if (!AeHelper.CheckVaild(PersonDatabase, SelectFeature))
            {
                MessageBox.MessageWarning.Show("系统提示", "字段尚未全部添加！");
                return false;
            }
            var dialogFactory = new DialogFactory();
            var folderPath = dialogFactory.OpenFolderDialog();
            if (string.IsNullOrEmpty(folderPath)) return false;
            //var templateDocPath = AppDomain.CurrentDomain.BaseDirectory + @"\template\承包合同.doc";
            //var templateExcelPath = AppDomain.CurrentDomain.BaseDirectory + @"\template\地块信息表.xls";
            var wait = new Wait();
            wait.SetInfoInvoke("正在导出合同书");
            wait.SetProgressInfo(string.Empty);
            var para = new Hashtable();
            para["folderPath"] = folderPath;
            para["wait"] = wait;
            para["ret"] = false;
            var t = new Thread(new ParameterizedThreadStart(Export));
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
                var sqlString = string.Format("Select distinct CBFBM,CBFMC From {0}", SelectFeature);
                var accessFactory = new AccessFactory(PersonDatabase);
                var dt = accessFactory.Query(sqlString);
                if (dt == null)
                {
                    wait.CloseWait();
                    para["ret"] = false;
                    return;
                }
                //var templateDocPath = AppDomain.CurrentDomain.BaseDirectory + @"\template\承包合同.doc";
                //  var templateExcelPath = AppDomain.CurrentDomain.BaseDirectory + @"\template\地块信息表.xls";
                var rowCount = dt.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    wait.SetProgressInfo(((double)i / (double)rowCount).ToString("P"));
                    var dir = new DirectoryInfo(folderPath);
                    dir.CreateSubdirectory(dt.Rows[i][0].ToString() + "_" + dt.Rows[i][1].ToString());
                    var savedDocPathContract = folderPath + @"\" + dt.Rows[i][0].ToString() + "_" + dt.Rows[i][1].ToString() + @"\" +dt.Rows[i][0].ToString() +
                                       dt.Rows[i][1].ToString() + "_承包经营权合同书.doc";
                    var savedDocPathStatement = folderPath + @"\" + dt.Rows[i][0].ToString() + "_" + dt.Rows[i][1].ToString() + @"\"+dt.Rows[i][0].ToString() +
                                       dt.Rows[i][1].ToString() + "_农户声明书.doc";
                    /*
                    var savedExcelPath = folderPath + @"\" + dt.Rows[i][0].ToString() + "_" + dt.Rows[i][1].ToString() + @"\" +
                                       dt.Rows[i][1].ToString() + "_插入合同书中地块信息表.xls";
                     
                    File.Copy(templateExcelPath, savedExcelPath, true);
                     * */
                    ExportDoc(savedDocPathContract, savedDocPathStatement, dt.Rows[i]);
                    // ExportExcel(savedExcelPath, dt.Rows[i]);
                }
                wait.CloseWait();
                para["ret"] = true;
                return;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
                wait.CloseWait();
                para["ret"] = false;
                return;
            }


        }

        private void ExportDoc(string savedDocPathContract, string savedDocPathStatement, System.Data.DataRow row)
        {
            var sqlString = string.Format("Select FBFMC,FBFFZRXM,FBFDZ from {0}", "FBF");
            var accessFactory = new AccessFactory(BasicDatabase);
            var dtFbf = accessFactory.Query(sqlString);
            if (dtFbf == null || dtFbf.Rows.Count != 1)
            {
                return;
            }
            sqlString = string.Format("select CYXB,CBFZJHM,CBFCYSL,CBFDZ,CBFBM,CBFMC from {0} where trim(CBFBM)='{1}'", "CBF",
                  row[0].ToString());
            accessFactory = new AccessFactory(BasicDatabase);
            var dtCbf = accessFactory.Query(sqlString);
            if (dtCbf == null) return;
            ExportDoc(savedDocPathContract, savedDocPathStatement, dtFbf.Rows[0], dtCbf.Rows[0]);
        }

        private void ExportDoc(string savedDocPathContract, string savedDocPathStatement, System.Data.DataRow rowFbf, System.Data.DataRow rowCbf)
        {
            var scmjSum = 0.0;
            //string sqlString= string.Format("select SCMJ,DKMC,DKBM,DKDZ,DKXZ,DKNZ,DKBZ,SFJBNT from {0} where trim(CBFBM)='{1}'", 
            //    SelectFeature,rowCbf[4].ToString());
            string sqlString = string.Format("select SCMJ from {0} where trim(CBFBM)='{1}'",
    SelectFeature, rowCbf[4].ToString());
            var accessFactory = new AccessFactory(PersonDatabase);
            var dtDk = accessFactory.Query(sqlString);
            if (dtDk == null) return;
            for (int i = 0; i < dtDk.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(dtDk.Rows[i][0].ToString()))
                {
                    scmjSum += 0.0;
                }
                else
                {
                    scmjSum += Convert.ToDouble(double.Parse(dtDk.Rows[i][0].ToString().Trim()).ToString("f"));
                }               
            }
            sqlString = string.Format("Select CYXM from {0} where trim(CBFBM)='{1}' order by YHZGX", "CBF_JTCY",
    rowCbf[4].ToString());
            accessFactory = new AccessFactory(BasicDatabase);
            var dtCyxx = accessFactory.Query(sqlString);
            if (dtCyxx == null) return;
            var cyxx = new StringBuilder(string.Empty);
            cyxx.Append(dtCyxx.Rows[0][0].ToString());
            for (int i = 1; i < dtCyxx.Rows.Count; i++)
            {
                cyxx.Append("、" + dtCyxx.Rows[i][0].ToString());
            }
            cyxx.Append("共计" + dtCyxx.Rows.Count.ToString() + "人");
            var currentTime = System.DateTime.Now.ToLongDateString();
            var dt = GetFieldTable(rowCbf[4].ToString());
            if (dt == null || dt.Rows.Count > 17)
            {
                System.Windows.Forms.MessageBox.Show(rowCbf[4].ToString() + "该农户的地块数超过17块");
                return;
            }
            var report = new ExportWord();
            //  report.CreateNewDocument(AppDomain.CurrentDomain.BaseDirectory + @"\template\合同\"+"承包合同"+dt.Rows.Count+".doc");
            report.CreateNewDocument(AppDomain.CurrentDomain.BaseDirectory + @"\template\承包合同.doc");
            report.InsertValue("合同编号", rowCbf[4].ToString() + "J");
            report.InsertValue("id", rowCbf[1].ToString());
            report.InsertValue("发包方名称", rowFbf[0].ToString());
            // report.InsertValue("承包方名称1", fbffzrxm);
            report.InsertValue("承包方名称1", rowCbf[5].ToString());
            report.InsertValue("承包方住所1", rowFbf[2].ToString());
            //  report.InsertValue("承包方住所2", fbfdz);
            report.InsertValue("实测面积", scmjSum.ToString("f"));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                report.InsertValue("dkmc" + (i + 1).ToString(), dt.Rows[i][1].ToString());
                report.InsertValue("dkbm" + (i + 1).ToString(), dt.Rows[i][2].ToString());
                if (string.IsNullOrEmpty(dt.Rows[i][0].ToString()))
                {
                    report.InsertValue("scmj" + (i + 1).ToString(), 0.0.ToString("f"));
                }
                else
                {
                    report.InsertValue("scmj" + (i + 1).ToString(), Convert.ToDouble(dt.Rows[i][0].ToString()).ToString("f"));
                }              
                report.InsertValue("dz" + (i + 1).ToString(), dt.Rows[i][3].ToString());
                report.InsertValue("nz" + (i + 1).ToString(), dt.Rows[i][4].ToString());
                report.InsertValue("xz" + (i + 1).ToString(), dt.Rows[i][5].ToString());
                report.InsertValue("bz" + (i + 1).ToString(), dt.Rows[i][6].ToString());
                report.InsertValue("sf" + (i + 1).ToString(), Transcode.CodeToSfjbnt(dt.Rows[i][7].ToString()));
            }
            scmjSum = Convert.ToDouble(scmjSum.ToString("f"));
            report.InsertValue("大写", CommonHelper.ConvertSum(scmjSum.ToString()));
            report.InsertValue("小写", scmjSum.ToString("f"));
            report.InsertValue("地块", dt.Rows.Count.ToString());
            report.SaveDocument(savedDocPathContract);
            ///
            report.CreateNewDocument(AppDomain.CurrentDomain.BaseDirectory + @"template\农户声明书.doc");
            report.InsertValue("承包方名称2", rowCbf[5].ToString());
            report.InsertValue("承包方住所2", rowFbf[2].ToString());
            report.InsertValue("性别1", Transcode.CodeToSex(rowCbf[0].ToString()));
            //       sqlString = string.Format("select CYXB,CBFZJHM,CBFCYSL,CBFDZ,CBFBM,CBFMC from {0} where trim(CBFBM)='{1}'", "CBF",
            report.InsertValue("身份证号1", rowCbf[1].ToString());
            report.InsertValue("家庭成员信息", cyxx.ToString());
            // report.InsertValue("成员数量", cysl);
            report.InsertValue("日期1", currentTime);
            report.InsertValue("承包方名称3", rowCbf[5].ToString());
            report.InsertValue("承包方住所3", rowFbf[2].ToString());
            report.InsertValue("性别2", Transcode.CodeToSex(rowCbf[0].ToString()));
            report.InsertValue("身份证号2", rowCbf[1].ToString());
            report.InsertValue("承包方住所4", rowFbf[2].ToString());
            report.InsertValue("日期2", currentTime);
            report.InsertValue("日期3", currentTime);
            report.SaveDocument(savedDocPathStatement);
            report.CreateNewDocument(savedDocPathContract);
            report.DeleteTableRow(1, 2 + dt.Rows.Count + 1, 1, 17 - dt.Rows.Count);
            report.SaveDocument(savedDocPathContract);
            report.KillWinWordProcess();
        }

        private void ExportExcel(string savedExcelPath, System.Data.DataRow row)
        {
            string sqlString = string.Format("select SCMJ,DKMC,DKBM,DKDZ,DKXZ,DKNZ,DKBZ,SFJBNT from {0} where trim(CBFBM)='{1}'",
                SelectFeature, row[0].ToString());
            var accessFactory = new AccessFactory(PersonDatabase);
            var dtDk = accessFactory.Query(sqlString);
            if (dtDk == null) return;
            double scmjSum = 0.0;
            for (int i = 0; i < dtDk.Rows.Count; i++)
            {
                scmjSum += Convert.ToDouble(double.Parse(dtDk.Rows[i][0].ToString().Trim()).ToString("f"));
            }
            using (var fileStream = new FileStream(savedExcelPath, FileMode.Open, FileAccess.ReadWrite))
            {
                IWorkbook workbook = new HSSFWorkbook(fileStream);
                ISheet sheet = workbook.GetSheetAt(0);
                NPOI.SS.UserModel.IRow rows;
                int endRow = 1;
                //  MessageBox.Show(dtFields.Rows.Count.ToString());
                HSSFCellStyle style = (HSSFCellStyle)workbook.CreateCellStyle();
                style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
                style.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
                style.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
                style.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
                style.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
                style.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
                style.WrapText = true;
                for (int i = 0; i < dtDk.Rows.Count; i++)
                {
                    endRow++;
                    rows = sheet.GetRow(endRow);
                    //序号
                    ICell cell = rows.GetCell(0);
                    cell.SetCellValue(i + 1);
                    //地块名称
                    cell = rows.GetCell(1);

                    cell.SetCellValue(dtDk.Rows[i][1].ToString());
                    //地块编码
                    cell = rows.GetCell(2);
                    cell.CellStyle = style;
                    cell.SetCellValue(dtDk.Rows[i][2].ToString());
                    //实测面积
                    cell = rows.GetCell(3);
                    cell.CellStyle = style;
                    cell.SetCellValue(double.Parse(dtDk.Rows[i][0].ToString()).ToString("f"));
                    //地块东至
                    cell = rows.GetCell(4);
                    cell.CellStyle = style;
                    cell.SetCellValue(dtDk.Rows[i][3].ToString());
                    //地块西至
                    cell = rows.GetCell(5);
                    cell.CellStyle = style;
                    cell.SetCellValue(dtDk.Rows[i][4].ToString());
                    //地块南至
                    cell = rows.GetCell(6);
                    cell.CellStyle = style;
                    cell.SetCellValue(dtDk.Rows[i][5].ToString());
                    //地块北至
                    cell = rows.GetCell(7);
                    cell.CellStyle = style;
                    cell.SetCellValue(dtDk.Rows[i][6].ToString());
                    //是否基本农田
                    cell = rows.GetCell(8);
                    cell.CellStyle = style;
                    cell.SetCellValue(Transcode.CodeToSfjbnt(dtDk.Rows[i][7].ToString()));

                }
                endRow++;
                for (int i = sheet.LastRowNum; i >= endRow + 1; i--)
                {
                    sheet.ShiftRows(i, i + 1, -1);
                }
                rows = sheet.GetRow(endRow);
                sheet.AddMergedRegion(new CellRangeAddress(endRow, endRow, 0, 8));
                ICell cells = rows.GetCell(0);
                cells.CellStyle = style;
                string info = "合计大写 ";
                info += "     " + Common.CommonHelper.ConvertSum(scmjSum.ToString()) + "      " + "亩（小写）";
                info += scmjSum + "亩；";
                info += "地块数" + "    " + dtDk.Rows.Count.ToString() + "   " + "块";

                cells.SetCellValue(info);
                var fs = new FileStream(savedExcelPath, FileMode.Create, FileAccess.Write);
                workbook.Write(fs);
                fs.Close();
                fileStream.Close();
            }

        }

        private System.Data.DataTable GetFieldTable(string cbfbm)
        {
            string sqlString = string.Format("select SCMJ,DKMC,DKBM,DKDZ,DKNZ,DKXZ,DKBZ,SFJBNT from {0} where trim(CBFBM)='{1}'",
               SelectFeature, cbfbm);
            var accessFactory = new AccessFactory(PersonDatabase);
            return accessFactory.Query(sqlString);
        }

    }
}
