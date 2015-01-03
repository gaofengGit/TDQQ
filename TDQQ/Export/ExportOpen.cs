using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ESRI.ArcGIS.Geodatabase;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using TDQQ.Common;
using TDQQ.MyWindow;

namespace TDQQ.Export
{
    class ExportOpen : ExportBase
    {
        public ExportOpen() : base() { }

        public ExportOpen(string personDatabase, string selectFeature, string basicDatabase) : base(personDatabase, selectFeature, basicDatabase) { }
        public override bool Export()
        {
            if (!CheckValid())
            {
                MessageBox.MessageWarning.Show("系统提示", "字段尚未全部添加！");
                return false;
            }
            var templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\template\土地承包经营权确权公示表.xls";
            var savedFilePath = CopyTemplateTo(templatePath, "公示表");
            if (string.IsNullOrEmpty(savedFilePath))
            {
                return false;
            }
            Wait wait = new Wait();
            wait.SetInfoInvoke("正在导出公示表.....");
            wait.SetProgressInfo(string.Empty);
            var para = new Hashtable();
            para["savedFilePath"] = savedFilePath;
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
            var savedFilePath = para["savedFilePath"].ToString();
            var wait = para["wait"] as Wait;
            try
            {
                //按照承包方名称
                var sqlString = string.Format("Select distinct CBFBM from {0} order by CBFBM ", SelectFeatrue);
                var accessFactory = new AccessFactory(PersonDatabase);
                var dt = accessFactory.Query(sqlString);
                if (dt == null)
                {
                    wait.CloseWait();
                    para["ret"] = false;
                    return;
                }
                int startRow = 4, endRow = 4;
                using (var fileStream = new FileStream(savedFilePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    IWorkbook workbookSource = new HSSFWorkbook(fileStream);
                    ICellStyle style = MergetStyle(workbookSource);
                    var rowCount = dt.Rows.Count;
                    for (int i = 0; i < rowCount; i++)
                    {
                        wait.SetProgressInfo(((double)i / (double)rowCount).ToString("P"));
                        var cbfbm = dt.Rows[i][0].ToString().Trim();
                        //地块信息
                        var dt1 = new System.Data.DataTable();
                        //家庭信息
                        var dt2 = new System.Data.DataTable();
                        GetFillInfoTable(cbfbm, out dt1, out dt2);
                        //
                        string cbfmc = dt1.Rows[0][0].ToString();
                        double htmj = 0.0;
                        double scmj = 0.0;
                        var endRowField = FillFields(dt1, workbookSource, endRow, ref htmj, ref scmj);
                        var endRowFamily = FillFamily(dt2, workbookSource, endRow);
                        endRow = Math.Max(endRowField, endRowFamily);
                        MergeCells(workbookSource, i + 1, cbfmc, dt2.Rows.Count, scmj, htmj, startRow, endRow, style);
                        endRow++;
                        startRow = endRow;
                    }
                    EditExcel(workbookSource, endRow, 0);
                    FileStream fs = new FileStream(savedFilePath, FileMode.Create, FileAccess.Write);
                    workbookSource.Write(fs);
                    fs.Close();
                    fileStream.Close();
                }
                FillCbfmcIndexSheet(savedFilePath);
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

        private void MergeCells(IWorkbook workbookSource, int p1, string cbfmc, int p2, double scmj, double htmj, int startRow, int endRow, ICellStyle style)
        {
            HSSFSheet sheetSource = (HSSFSheet)workbookSource.GetSheetAt(0);
            HSSFRow rowSet = (HSSFRow)sheetSource.GetRow(startRow);

            //处理编号合并单元格
            sheetSource.AddMergedRegion(new CellRangeAddress(startRow, endRow, 0, 0));
            HSSFCell cellIndex = (HSSFCell)rowSet.GetCell(0);
            cellIndex.SetCellValue(p1);
            cellIndex.CellStyle = style;
            //处理户主信息合并单元格
            sheetSource.AddMergedRegion(new CellRangeAddress(startRow, endRow, 1, 1));
            HSSFCell cellCbfmc = (HSSFCell)rowSet.GetCell(1);
            cellCbfmc.SetCellValue(cbfmc);
            cellCbfmc.CellStyle = style;
            //处理家庭成员个数
            sheetSource.AddMergedRegion(new CellRangeAddress(startRow, endRow, 2, 2));
            HSSFCell cellJtcygs = (HSSFCell)rowSet.GetCell(2);
            cellJtcygs.SetCellValue(p2);
            cellJtcygs.CellStyle = style;
            //处理实测总面积
            sheetSource.AddMergedRegion(new CellRangeAddress(startRow, endRow, 13, 13));
            HSSFCell cellScmj = (HSSFCell)rowSet.GetCell(13);
            cellScmj.SetCellValue(scmj.ToString("f"));
            cellScmj.CellStyle = style;
            //处理合同总面积
            sheetSource.AddMergedRegion(new CellRangeAddress(startRow, endRow, 15, 15));
            HSSFCell cellHtmj = (HSSFCell)rowSet.GetCell(15);
            cellHtmj.SetCellValue(htmj.ToString("f"));
            cellHtmj.CellStyle = style;
            //盖章签字
            sheetSource.AddMergedRegion(new CellRangeAddress(startRow, endRow, 17, 17));
            HSSFCell cellQzgz = (HSSFCell)rowSet.GetCell(17);
            cellQzgz.SetCellValue(string.Empty);
            cellQzgz.CellStyle = style;
        }

        private int FillFamily(System.Data.DataTable dt2, IWorkbook workbookSource, int endRow)
        {
            HSSFSheet sheetSource = (HSSFSheet)workbookSource.GetSheetAt(0);
            for (int k = 0; k < dt2.Rows.Count; k++)
            {
                HSSFRow rowSource = (HSSFRow)sheetSource.GetRow(endRow + k);
                rowSource.GetCell(3).SetCellValue(dt2.Rows[k][0].ToString());
                rowSource.GetCell(4).SetCellValue(dt2.Rows[k][1].ToString());
                rowSource.GetCell(5).SetCellValue(Transcode.CodeToRelationship(dt2.Rows[k][2].ToString().Trim()));
            }
            return endRow + dt2.Rows.Count - 1;
        }

        private int FillFields(System.Data.DataTable dt1, IWorkbook workbookSource, int endRow, ref double htmj, ref double scmj)
        {
            HSSFSheet sheetSource = (HSSFSheet)workbookSource.GetSheetAt(0);
            //填写承包地块信息，承包方名称
            for (int j = 0; j < dt1.Rows.Count; j++)
            {

                HSSFRow rowSource = (HSSFRow)sheetSource.GetRow(endRow + j);
                //地块名称
                rowSource.GetCell(6).SetCellValue(dt1.Rows[j][1].ToString());
                //地块编码
                var dkbm = dt1.Rows[j][2].ToString().Trim();
                if (dkbm != string.Empty)
                {
                    dkbm = dkbm.Substring(14, 5);
                }
                rowSource.GetCell(7).SetCellValue(dkbm);
                //四至
                rowSource.GetCell(8).SetCellValue(dt1.Rows[j][3].ToString());
                rowSource.GetCell(9).SetCellValue(dt1.Rows[j][4].ToString());
                rowSource.GetCell(10).SetCellValue(dt1.Rows[j][5].ToString());
                rowSource.GetCell(11).SetCellValue(dt1.Rows[j][6].ToString());
                //合同面积和实测面积
                double htmjSingle,scmjSingle;
                if (dt1.Rows[j][7]==null||string.IsNullOrEmpty(dt1.Rows[j][7].ToString().Trim()))
                {
                    htmjSingle = 0.0;
                }
                else
                {
                    htmjSingle = double.Parse(dt1.Rows[j][7].ToString().Trim());
                }
                if (dt1.Rows[j][8] == null||string.IsNullOrEmpty(dt1.Rows[j][8].ToString().Trim()))
                {
                    scmjSingle = 0.0;
                }
                else
                {
                    scmjSingle = double.Parse(dt1.Rows[j][8].ToString().Trim());                   
                }
                htmj += htmjSingle;
                scmj += scmjSingle;
                ICell cellscmj = rowSource.GetCell(12);
                ICell cellhtmj = rowSource.GetCell(14);
                if (htmjSingle > scmjSingle)
                {
                    cellhtmj.CellStyle = LessStyle(workbookSource);
                    cellscmj.CellStyle = LessStyle(workbookSource);
                }
                cellscmj.SetCellValue(scmjSingle.ToString("f"));
                cellhtmj.SetCellValue(htmjSingle.ToString("f"));
                //耕地类型
                rowSource.GetCell(16).SetCellValue(Transcode.CodeToDklb(dt1.Rows[j][9].ToString().Trim()));
            }
            return endRow + dt1.Rows.Count - 1;
        }

        private void GetFillInfoTable(string cbfbm, out System.Data.DataTable dt1, out System.Data.DataTable dt2)
        {
            AccessFactory accessFactoryFields = new AccessFactory(PersonDatabase);
            //先获取地块信息表
            var sqlLins1 = string.Format(
          "select CBFMC,DKMC,DKBM,DKDZ,DKNZ,DKXZ," +
          "DKBZ,HTMJ,SCMJ,DKLB from {0} where trim(CBFBM) ='{1}'",
          SelectFeatrue, cbfbm);
            dt1 = accessFactoryFields.Query(sqlLins1);
            //获取户籍信息
            var sqlLins2 = string.Format("select CYXM,CYZJHM,YHZGX from {0}  where trim(CBFBM) ='{1}' Order by YHZGX", "CBF_JTCY",
                    cbfbm);
            AccessFactory accessFactoryFamily = new AccessFactory(BasicDatabase);
            dt2 = accessFactoryFamily.Query(sqlLins2);
        }

        private ICellStyle LessStyle(IWorkbook workbook)
        {
            HSSFFont font = (HSSFFont)workbook.CreateFont();
            HSSFCellStyle style = (HSSFCellStyle)workbook.CreateCellStyle();
            font.Color = HSSFColor.RED.index;
            style.SetFont(font);
            style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            style.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
            style.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
            style.WrapText = true;
            return style;
        }

        protected override void EditExcel(IWorkbook workbook, int endRow, int sheetIndex)
        {
            //删除多余行
            HSSFSheet sheetSource = (HSSFSheet)workbook.GetSheetAt(sheetIndex);
            for (int i = sheetSource.LastRowNum; i >= endRow + 1; i--)
            {
                sheetSource.ShiftRows(i, i + 1, -1);
            }
            //删除最后一行
            endRow = sheetSource.LastRowNum;
            sheetSource.ShiftRows(endRow, endRow, -1);
            //增加制表信息
            endRow = sheetSource.LastRowNum;
            var lastRow = (HSSFRow)sheetSource.CreateRow(endRow);
            lastRow.Height = 500;
            //合并样式
            ICellStyle style = workbook.CreateCellStyle();
            style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            style.VerticalAlignment = VerticalAlignment.CENTER;
            //合并单元格
            sheetSource.AddMergedRegion(new CellRangeAddress(endRow, endRow, 0, 3));
            sheetSource.AddMergedRegion(new CellRangeAddress(endRow, endRow, 4, 7));
            sheetSource.AddMergedRegion(new CellRangeAddress(endRow, endRow, 8, 11));
            sheetSource.AddMergedRegion(new CellRangeAddress(endRow, endRow, 12, 17));
            //填写内容
            var cell = lastRow.CreateCell(0);
            cell.SetCellValue("制表人：_______________");
            cell.CellStyle = style;
            cell = lastRow.CreateCell(4);
            cell.SetCellValue("制表日期：__________ 年 ______ 月 ______ 日");
            cell.CellStyle = style;
            cell = lastRow.CreateCell(8);
            cell.SetCellValue("审核人:_____________");
            cell = lastRow.CreateCell(12);
            cell.CellStyle = style;
            cell.SetCellValue("审核日期:________ 年______ 月 ______ 日");
        }


    }
}
