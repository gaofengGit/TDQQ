using System;
using System.Collections;
using System.Data.Odbc;
using System.IO;
using System.Threading;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using TDQQ.AE;
using TDQQ.Common;
using TDQQ.MyWindow;

namespace TDQQ.Export
{
    class ExportD : ExportBase
    {
        public ExportD()
            : base()
        {

        }
        public ExportD(string personDatabase, string selectFeature, string basicDatabase)
            : base(personDatabase, selectFeature, basicDatabase)
        {

        }
        public override bool Export()
        {
            if (!CheckValid())
            {
                MessageBox.MessageWarning.Show("系统提示", "字段尚未全部添加！");
                return false;
            }
            var templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\template\二榜公示表.xls";
            var savedFilePath = CopyTemplateTo(templatePath, "地块信息公示表");
            if (string.IsNullOrEmpty(savedFilePath)) return false;
            var para = new Hashtable();
            para["savedFilePath"] = savedFilePath;
            var wait = new Wait();
            wait.SetInfoInvoke("正在导出地块信息表");
            wait.SetProgressInfo(string.Empty);
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
            var savedFilePath = para["savedFilePath"].ToString();
            var wait = para["wait"] as Wait;
            try
            {
                var sqlString = string.Format("Select distinct CBFBM from {0} order by CBFBM", SelectFeatrue);
                var accessFactory = new AccessFactory(PersonDatabase);
                var dt = accessFactory.Query(sqlString);
                if (dt == null)
                {
                    wait.CloseWait();
                    para["ret"] = false;
                    return;
                }
                int startRowIndex = 6;
                try
                {
                    using (var fileStream = new FileStream(savedFilePath, FileMode.Open, FileAccess.ReadWrite))
                    {
                        var workbookSource = new HSSFWorkbook(fileStream);
                        //  var sheetSource = (HSSFSheet)workbookSource.GetSheetAt(0);
                        //生成一户的地块承包信息
                        var style = MergetStyle(workbookSource);
                        var rowCount = dt.Rows.Count;
                        for (int i = 0; i < rowCount; i++)
                        {
                            wait.SetProgressInfo(((double)i / (double)rowCount).ToString("p"));
                            Export(workbookSource, dt.Rows[i], ref startRowIndex,style, i);
                        }
                        EditExcel(workbookSource, startRowIndex, 0);
                        var fs = new FileStream(savedFilePath, FileMode.Create, FileAccess.Write);
                        workbookSource.Write(fs);
                        fs.Close();
                        fileStream.Close();
                    }
                    para["ret"] = true;
                }
                catch (Exception)
                {

                    para["ret"] = false;
                }
                wait.CloseWait();
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
        private void Export(HSSFWorkbook workbook, System.Data.DataRow row, ref int startRowIndex, ICellStyle style, int index)
        {
            var cbfbm = row[0].ToString();
            var sqlLins =
                string.Format(
                    "select CBFMC,DKMC,DKBM,DKDZ,DKNZ,DKXZ,DKBZ,HTMJ,SCMJ,DKBZXX from {0} where trim(CBFBM) ={1}",
                    SelectFeatrue, cbfbm);
            var accessFactory = new AccessFactory(PersonDatabase);
            var cbdks = accessFactory.Query(sqlLins);
            double htmjSum = 0.0;
            double scmjSum = 0.0;
            var cbfmc = string.Empty;
            var sheet = workbook.GetSheetAt(0);
            for (int j = 0; j < cbdks.Rows.Count; j++)
            {

                var rowSource = (HSSFRow)sheet.GetRow(startRowIndex);
                if (cbdks.Rows.Count == 1)
                {
                    rowSource.Height = 1200;
                }
                cbfmc = cbdks.Rows[j][0].ToString();
                //DKMC
                rowSource.GetCell(4).SetCellValue(cbdks.Rows[j][1].ToString());
                //DKBM
                var dkbm = cbdks.Rows[j][2].ToString().Trim();
                if (dkbm != string.Empty)
                {
                    dkbm = dkbm.Substring(14, 5);
                }
                rowSource.GetCell(5).SetCellValue(dkbm);
                //四至
                rowSource.GetCell(6).SetCellValue(cbdks.Rows[j][3].ToString());
                rowSource.GetCell(7).SetCellValue(cbdks.Rows[j][4].ToString());
                rowSource.GetCell(8).SetCellValue(cbdks.Rows[j][5].ToString());
                rowSource.GetCell(9).SetCellValue(cbdks.Rows[j][6].ToString());
                //合同面积
                double htmjSingle, scmjSingle;
                if (string.IsNullOrEmpty(cbdks.Rows[j][7].ToString()))
                {
                    htmjSingle = 0.0;
                }
                else
                {
                    htmjSingle = double.Parse(cbdks.Rows[j][7].ToString().Trim());
                }
                htmjSum += htmjSingle;
                if (string.IsNullOrEmpty(cbdks.Rows[j][8].ToString().Trim()))
                {
                    scmjSingle = 0.0;
                }
                else
                {
                    scmjSingle = double.Parse(cbdks.Rows[j][8].ToString().Trim()); 
                }
                scmjSum += scmjSingle;
                rowSource.GetCell(10).SetCellValue(htmjSingle.ToString("f"));
                rowSource.GetCell(11).SetCellValue(scmjSingle.ToString("f"));
                startRowIndex++;
            }

            var rowSet = (HSSFRow)sheet.GetRow(startRowIndex - cbdks.Rows.Count);
            //设置编号和合并单元格
            sheet.AddMergedRegion(new CellRangeAddress(startRowIndex - cbdks.Rows.Count,
                startRowIndex - 1, 0, 0));
            var cellIndex = (HSSFCell)rowSet.GetCell(0);
            cellIndex.SetCellValue(index + 1);
            cellIndex.CellStyle = style;
            //承包方名称
            var cellcbfmc = (HSSFCell)rowSet.GetCell(1);
            cellcbfmc.SetCellValue(cbfmc);
            sheet.AddMergedRegion(new CellRangeAddress(startRowIndex - cbdks.Rows.Count,
startRowIndex - 1, 1, 1));
            //设置地块汇总情况
            var cellHtmj = (HSSFCell)rowSet.GetCell(2);
            var cellScmj = (HSSFCell)rowSet.GetCell(3);
            var htmjhz = string.Format("合计：\n {0}块 \n {1}亩", cbdks.Rows.Count, htmjSum.ToString("f"));
            var scmjhz = string.Format("合计：\n {0}块 \n {1}亩", cbdks.Rows.Count, scmjSum.ToString("f"));
            cellHtmj.SetCellValue(htmjhz);
            cellScmj.SetCellValue(scmjhz);
            cellHtmj.CellStyle = style;
            cellScmj.CellStyle = style;
            sheet.AddMergedRegion(new CellRangeAddress(startRowIndex - cbdks.Rows.Count,
startRowIndex - 1, 2, 2));
            sheet.AddMergedRegion(new CellRangeAddress(startRowIndex - cbdks.Rows.Count,
startRowIndex - 1, 3, 3));
        }
        protected override void EditExcel(IWorkbook workbook, int lastRowIndex, int sheetIndex)
        {
            //删除多余行
            var sheetSource = (HSSFSheet)workbook.GetSheetAt(sheetIndex);
            for (int i = sheetSource.LastRowNum; i >= lastRowIndex + 1; i--)
            {
                sheetSource.ShiftRows(i, i + 1, -1);
            }
            //删除最后一行
            lastRowIndex = sheetSource.LastRowNum;
            sheetSource.ShiftRows(lastRowIndex, lastRowIndex, -1);
            //增加制表信息
            lastRowIndex = sheetSource.LastRowNum;
            var lastRow = (HSSFRow)sheetSource.CreateRow(lastRowIndex);
            lastRow.Height = 500;
            //合并样式
            var style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.CENTER;
            style.VerticalAlignment = VerticalAlignment.CENTER;
            //合并单元格
            sheetSource.AddMergedRegion(new CellRangeAddress(lastRowIndex, lastRowIndex, 0, 2));
            sheetSource.AddMergedRegion(new CellRangeAddress(lastRowIndex, lastRowIndex, 3, 6));
            sheetSource.AddMergedRegion(new CellRangeAddress(lastRowIndex, lastRowIndex, 7, 9));
            sheetSource.AddMergedRegion(new CellRangeAddress(lastRowIndex, lastRowIndex, 10, 12));
            //填写内容
            var cell = lastRow.CreateCell(0);
            cell.SetCellValue("制表人：_______________");
            cell.CellStyle = style;
            cell = lastRow.CreateCell(3);
            cell.SetCellValue("制表日期：__________ 年 ______ 月 ______ 日");
            cell.CellStyle = style;
            cell = lastRow.CreateCell(7);
            cell.SetCellValue("审核人:_____________");
            cell = lastRow.CreateCell(10);
            cell.CellStyle = style;
            cell.SetCellValue("审核日期:__________ 年______ 月 ______ 日");
        }
    }
}
