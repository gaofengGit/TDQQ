using System;
using System.Collections;
using System.IO;
using System.Threading;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using TDQQ.Common;
using TDQQ.MyWindow;

namespace TDQQ.Export
{
    class ExportE : ExportBase
    {
        public ExportE() : base() { }
        public ExportE(string personDatabase, string selectFeatrue, string basicDatabase) : base(personDatabase, selectFeatrue, basicDatabase) { }

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
            var templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\template\农村土地承包经营权公示结果归户表.xls";
            var wait = new Wait();
            wait.SetInfoInvoke("正在导出归户表.....");
            wait.SetProgressInfo(string.Empty);
            var para = new Hashtable();
            para["folderPath"] = folderPath;
            para["templatePath"] = templatePath;
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
            var templatePath = para["templatePath"].ToString();
            var wait = para["wait"] as Wait;
            try
            {
                var sqlstring = string.Format("select FBFMC,FBFFZRXM from {0}", "FBF");
                var accessFactory = new AccessFactory(BasicDatabase);
                var dtfbf = accessFactory.Query(sqlstring);
                if (dtfbf == null || dtfbf.Rows.Count != 1)
                {
                    wait.CloseWait();
                    para["ret"] = false;
                    return;
                }
                var fbfmc = dtfbf.Rows[0][0].ToString();
                var fbffzr = dtfbf.Rows[0][1].ToString();
                sqlstring = string.Format("Select CBFBM,CBFMC,LXDH,CBFZJHM,CBFDZ,YZBM from {0}", "CBF");
                var dtcbf = accessFactory.Query(sqlstring);
                if (dtcbf == null)
                {
                    wait.CloseWait();
                    para["ret"] = false;
                    return;
                }
                var rowCount = dtcbf.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    wait.SetProgressInfo(((double)i / (double)rowCount).ToString("p"));
                    var saveExcel = folderPath + @"\" + dtcbf.Rows[i][0].ToString() + @"_" + dtcbf.Rows[i][1].ToString() + ".xls";
                    File.Copy(templatePath, saveExcel, true);
                    Export(dtcbf.Rows[i], fbfmc, fbffzr, saveExcel);
                    Export(dtcbf.Rows[i][0].ToString(), saveExcel);
                }
                wait.CloseWait();
                para["ret"] = true;
                return;
            }
            catch (Exception)
            {
                wait.CloseWait();
                para["ret"] = false;
                return;
            }

        }
        /// <summary>
        /// 导出承包方信息和家庭成员信息
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fbfmc"></param>
        /// <param name="fbffzr"></param>
        /// <param name="saveFilePath"></param>
        private void Export(System.Data.DataRow row, string fbfmc, string fbffzr, string saveFilePath)
        {
            var sqlString = string.Format("Select CYXM,YHZGX,CYZJHM,CYBZ from {0} where trim(CBFBM)='{1}' order by YHZGX", "CBF_JTCY",
                 row[0].ToString());
            var accessFactory = new AccessFactory(BasicDatabase);
            var dtjtcy = accessFactory.Query(sqlString);
            using (var fileStream = new FileStream(saveFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                IWorkbook workbook = new HSSFWorkbook(fileStream);
                ISheet sheet = workbook.GetSheetAt(0);
                HSSFRow rowSource = (HSSFRow)sheet.GetRow(3);
                rowSource.GetCell(2).SetCellValue(fbfmc);
                rowSource.GetCell(8).SetCellValue(fbffzr);
                //填写承包方信息
                rowSource = (HSSFRow)sheet.GetRow(4);
                rowSource.GetCell(2).SetCellValue(row[0].ToString() + "J");
                rowSource = (HSSFRow)sheet.GetRow(6);
                rowSource.GetCell(3).SetCellValue(row[1].ToString());
                rowSource.GetCell(8).SetCellValue(row[2].ToString());
                rowSource = (HSSFRow)sheet.GetRow(7);
                rowSource.GetCell(8).SetCellValue(row[3].ToString());
                rowSource = (HSSFRow)sheet.GetRow(10);
                rowSource.GetCell(2).SetCellValue(row[4].ToString());
                rowSource.GetCell(9).SetCellValue(row[5].ToString());
                //填写家庭成员信息
                sheet = (HSSFSheet)workbook.GetSheetAt(1);
                var start_row_index = 2;
                rowSource = (HSSFRow)sheet.GetRow(0);
                rowSource.GetCell(9).SetCellValue(dtjtcy.Rows.Count);
                for (int i = 0; i < dtjtcy.Rows.Count; i++)
                {
                    rowSource = (HSSFRow)sheet.GetRow(start_row_index + i);
                    rowSource.GetCell(0).SetCellValue(dtjtcy.Rows[i][0].ToString());
                    rowSource.GetCell(3).SetCellValue(Transcode.CodeToRelationship(dtjtcy.Rows[i][1].ToString()));
                    rowSource.GetCell(5).SetCellValue(dtjtcy.Rows[i][2].ToString());
                    rowSource.GetCell(6).SetCellValue(dtjtcy.Rows[i][3].ToString());
                }
                FileStream fs = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write);
                workbook.Write(fs);
                fs.Close();
                fileStream.Close();
            }
        }

        /// <summary>
        /// 填写地块信息
        /// </summary>
        /// <param name="cbfbm"></param>
        /// <param name="saveFilePath"></param>
        private void Export(string cbfbm, string saveFilePath)
        {
            var sqlString = string.Format("select DKMC,DKBM,DKDZ,DKNZ,DKXZ," +
                                             "DKBZ,HTMJ,SCMJ,TDYT,DLDJ,DKBZXX from {0} where trim(CBFBM)='{1}'",
                   SelectFeatrue, cbfbm);
            var accessFactory = new AccessFactory(PersonDatabase);
            var dtCbdk = accessFactory.Query(sqlString);
            if (dtCbdk == null) return;
            using (FileStream fileStream = new FileStream(saveFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                IWorkbook workbook = new HSSFWorkbook(fileStream);
                ISheet sheet = workbook.GetSheetAt(0);
                IRow rowSource;
                var mjSum = 0.0;
                int start_row_index = 14;
                int row_gap = 4;
                for (int i = 0; i < dtCbdk.Rows.Count; i++)
                {
                    rowSource = (HSSFRow)sheet.GetRow(start_row_index);
                    rowSource.GetCell(0).SetCellValue(dtCbdk.Rows[i][0].ToString());
                    rowSource.GetCell(1).SetCellValue(dtCbdk.Rows[i][1].ToString().Substring(14, 5));
                    var dksz = "东：" + dtCbdk.Rows[i][2].ToString() + "\n" +
                               "南：" + dtCbdk.Rows[i][3].ToString() + "\n" +
                               "西：" + dtCbdk.Rows[i][4].ToString() + "\n" +
                               "北：" + dtCbdk.Rows[i][5].ToString();
                    rowSource.GetCell(2).SetCellValue(dksz);
                    //保留有效位数
                    double htmj;
                    if (string.IsNullOrEmpty(dtCbdk.Rows[i][6].ToString()))
                    {
                        htmj = 0.0;
                    }
                    else
                    {
                        htmj = Convert.ToDouble(double.Parse(dtCbdk.Rows[i][6].ToString()).ToString("f"));
                    }                    
                    mjSum += htmj;
                    rowSource.GetCell(4).SetCellValue(htmj.ToString("f"));
                    if (string.IsNullOrEmpty(dtCbdk.Rows[i][7].ToString()))
                    {
                        rowSource.GetCell(5).SetCellValue(0.0);
                    }
                    else
                    {
                        rowSource.GetCell(5).SetCellValue(double.Parse(dtCbdk.Rows[i][7].ToString()).ToString("f"));
                    }                    
                    rowSource.GetCell(6).SetCellValue(Transcode.CodeToTdyt(dtCbdk.Rows[i][8].ToString()));
                    rowSource.GetCell(7).SetCellValue(Transcode.CodeToDldj(dtCbdk.Rows[i][9].ToString()));
                    rowSource.GetCell(8).SetCellValue(dtCbdk.Rows[i][10].ToString());
                    start_row_index += row_gap;
                }
                //添加承包地块信息
                rowSource = (HSSFRow)sheet.GetRow(11);
                rowSource.GetCell(3).SetCellValue(dtCbdk.Rows.Count + "块");
                rowSource.GetCell(4).SetCellValue(mjSum.ToString("f") + "亩");
                FileStream fs = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write);
                workbook.Write(fs);
                fs.Close();
                fileStream.Close();
            }

        }
    }
}
