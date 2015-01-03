using System;
using System.Collections;
using System.IO;
using System.Threading;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using TDQQ.Common;
using TDQQ.MyWindow;

namespace TDQQ.Export
{
    class ExportSign : ExportBase
    {
        public ExportSign() : base() { }
        public ExportSign(string personDatabase, string selectFeature, string basicDatabse) : base(personDatabase, selectFeature, basicDatabse) { }
        public override bool Export()
        {
            if (!CheckValid())
            {
                MessageBox.MessageWarning.Show("系统提示", "字段尚未全部添加！");
                return false;
            }
            var templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\template\土地承包经营权确权签字表.xls";
            var savedFilePath = CopyTemplateTo(templatePath, "签字表");
            if (string.IsNullOrEmpty(savedFilePath)) return false;
            Wait wait = new Wait();
            wait.SetInfoInvoke("正在导出签字表.....");
            wait.SetProgressInfo(string.Empty);
            var para = new Hashtable();
            para["savedFilePath"] = savedFilePath;
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
                var sqlString = string.Format("Select distinct CBFBM from {0} order by CBFBM ", SelectFeatrue);
                AccessFactory accessFactory = new AccessFactory(PersonDatabase);
                var dtDk = accessFactory.Query(sqlString);
                if (dtDk == null)
                {
                    wait.CloseWait();
                    para["ret"] = false;
                    return;
                }
                var startRow = 4;
                var endRow = 4;

                using (var fileStream = new FileStream(savedFilePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    //设置格式要素
                    HSSFWorkbook workbookSource = new HSSFWorkbook(fileStream);
                    int rowCount = dtDk.Rows.Count;
                    var style = MergetStyle(workbookSource);
                    for (int i = 0; i < rowCount; i++)
                    {
                        wait.SetProgressInfo(((double)i / (double)rowCount).ToString("P"));
                        var cbfbm = dtDk.Rows[i][0].ToString().Trim();
                        var dtDkSingle = new System.Data.DataTable();
                        var cbfSingle = new System.Data.DataTable();
                        GetFieldFamily(cbfbm, out dtDkSingle, out cbfSingle);
                        var htmj = 0.0;
                        var scmj = 0.0;
                        string cbfmc = dtDkSingle.Rows[0][0].ToString();
                        var endRowField = FillFieldSign(dtDkSingle, workbookSource, endRow, ref htmj, ref scmj);
                        var endRowFamily = FillFamily(cbfSingle, workbookSource, endRow);
                        endRow = Math.Max(endRowField, endRowFamily);
                        MergeCellsSign(workbookSource, i + 1, cbfmc, cbfSingle.Rows.Count, scmj, htmj, startRow, endRow, style);
                        endRow++;
                        startRow = endRow;
                    }
                    EditExcelSign(workbookSource, endRow);
                    FileStream fs = new FileStream(savedFilePath, FileMode.Create, FileAccess.Write);
                    workbookSource.Write(fs);
                    fs.Close();
                    fileStream.Close();
                }
                FillCbfmcIndexSheet(savedFilePath);
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

        private void EditExcelSign(HSSFWorkbook workbookSource, int endRow)
        {
            //throw new NotImplementedException();
            //删除多余行
            HSSFSheet sheetSource = (HSSFSheet)workbookSource.GetSheetAt(0);
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
            ICellStyle style = workbookSource.CreateCellStyle();
            style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            style.VerticalAlignment = VerticalAlignment.CENTER;
            //合并单元格
            sheetSource.AddMergedRegion(new CellRangeAddress(endRow, endRow, 0, 4));
            sheetSource.AddMergedRegion(new CellRangeAddress(endRow, endRow, 5, 9));
            sheetSource.AddMergedRegion(new CellRangeAddress(endRow, endRow, 10, 13));
            sheetSource.AddMergedRegion(new CellRangeAddress(endRow, endRow, 14, 19));
            //填写内容
            var cell = lastRow.CreateCell(0);
            cell.CellStyle = style;
            cell.SetCellValue("制表人：_______________");

            cell = lastRow.CreateCell(5);
            cell.CellStyle = style;
            cell.SetCellValue("制表日期：__________ 年 ______ 月 ______ 日");
            cell = lastRow.CreateCell(10);
            cell.CellStyle = style;
            cell.SetCellValue("审核人:_____________");
            cell = lastRow.CreateCell(14);
            cell.CellStyle = style;
            cell.SetCellValue("审核日期:________ 年______ 月 ______ 日");
        }

        private void MergeCellsSign(HSSFWorkbook workbookSource, int p1, string cbfmc, int p2, double scmj, double htmj, int startRow, int endRow, ICellStyle style)
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
            //备注
            sheetSource.AddMergedRegion(new CellRangeAddress(startRow, endRow, 18, 18));
            HSSFCell cellbz = (HSSFCell)rowSet.GetCell(18);
            cellbz.SetCellValue(string.Empty);
            cellbz.CellStyle = style;
            //盖章签字
            sheetSource.AddMergedRegion(new Region(startRow, 19, endRow, 19));
            HSSFCell cellQzgz = (HSSFCell)rowSet.GetCell(19);
            cellQzgz.SetCellValue(string.Empty);
            cellQzgz.CellStyle = style;
        }

        private int FillFamily(System.Data.DataTable cbfSingle, HSSFWorkbook workbookSource, int endRow)
        {
            HSSFSheet sheetSource = (HSSFSheet)workbookSource.GetSheetAt(0);
            for (int k = 0; k < cbfSingle.Rows.Count; k++)
            {
                HSSFRow rowSource = (HSSFRow)sheetSource.GetRow(endRow + k);
                rowSource.GetCell(3).SetCellValue(cbfSingle.Rows[k][0].ToString());
                rowSource.GetCell(4).SetCellValue(cbfSingle.Rows[k][1].ToString());
                rowSource.GetCell(5).SetCellValue(Transcode.CodeToRelationship(cbfSingle.Rows[k][2].ToString().Trim()));
            }
            return endRow + cbfSingle.Rows.Count - 1;
        }

        private int FillFieldSign(System.Data.DataTable dtDkSingle, HSSFWorkbook workbookSource, int endRow, ref double htmj, ref double scmj)
        {
            //throw new NotImplementedException();
            HSSFSheet sheetSource = (HSSFSheet)workbookSource.GetSheetAt(0);
            //select CBFMC0,DKMC1,DKBM2,DKDZ3,DKNZ4,DKXZ5,DKBZ6,HTMJ7,SCMJ8,DKLB9,SFJBNT10,DKBZXX11
            //填写承包地块信息，承包方名称
            for (int j = 0; j < dtDkSingle.Rows.Count; j++)
            {
                HSSFRow rowSource = (HSSFRow)sheetSource.GetRow(endRow + j);
                //地块名称
                rowSource.GetCell(6).SetCellValue(dtDkSingle.Rows[j][1].ToString());
                //地块编码
                var dkbm = dtDkSingle.Rows[j][2].ToString().Trim();
                if (dkbm != string.Empty)
                {
                    dkbm = dkbm.Substring(14, 5);
                }
                rowSource.GetCell(7).SetCellValue(dkbm);
                //四至
                rowSource.GetCell(8).SetCellValue(dtDkSingle.Rows[j][3].ToString());
                rowSource.GetCell(9).SetCellValue(dtDkSingle.Rows[j][4].ToString());
                rowSource.GetCell(10).SetCellValue(dtDkSingle.Rows[j][5].ToString());
                rowSource.GetCell(11).SetCellValue(dtDkSingle.Rows[j][6].ToString());
                //合同面积和实测面积
                double htmjSingle, scmjSingle;
                if (string.IsNullOrEmpty(dtDkSingle.Rows[j][7].ToString().Trim()))
                {
                    htmjSingle = 0.0;
                }
                else
                {
                    htmjSingle = double.Parse(dtDkSingle.Rows[j][7].ToString().Trim());
                }
                if (string.IsNullOrEmpty(dtDkSingle.Rows[j][8].ToString().Trim()))
                {
                    scmjSingle = 0.0;
                }
                else
                {
                    scmjSingle = double.Parse(dtDkSingle.Rows[j][8].ToString().Trim());
                }
                htmj += htmjSingle;
                scmj += scmjSingle;
                rowSource.GetCell(12).SetCellType(CellType.NUMERIC);
                rowSource.GetCell(12).SetCellValue(scmjSingle.ToString("f"));
                rowSource.GetCell(14).SetCellType(CellType.NUMERIC);
                rowSource.GetCell(14).SetCellValue(htmjSingle.ToString("f"));
                //耕地类型
                rowSource.GetCell(16).SetCellValue(Transcode.CodeToDklb(dtDkSingle.Rows[j][9].ToString().Trim()));
                //是否基本农田
                rowSource.GetCell(17).SetCellValue(Transcode.CodeToSfjbnt(dtDkSingle.Rows[j][10].ToString().Trim()));
                //地块备注
                rowSource.GetCell(18).SetCellValue(dtDkSingle.Rows[j][11].ToString());
            }
            return endRow + dtDkSingle.Rows.Count - 1;
        }
        private void GetFieldFamily(string cbfbm, out System.Data.DataTable dt1, out System.Data.DataTable dt2)
        {
            var sqlString =
    string.Format(
 "select CBFMC,DKMC,DKBM,DKDZ,DKNZ,DKXZ,DKBZ,HTMJ,SCMJ,DKLB,SFJBNT,DKBZXX from {0} where trim(CBFBM) ={1}",
 SelectFeatrue, cbfbm);
            AccessFactory accessFactoryFields = new AccessFactory(PersonDatabase);
            dt1 = accessFactoryFields.Query(sqlString);
            sqlString = string.Format("select CYXM,CYZJHM,YHZGX from {0} where trim(CBFBM) ={1} order by YHZGX", "CBF_JTCY",
                            cbfbm);
            AccessFactory accessFactoryFamily = new AccessFactory(BasicDatabase);
            dt2 = accessFactoryFamily.Query(sqlString);
        }
        //private void FillCbfmcIndexSheet(string savedExcelPath)
        //{
        //    try
        //    {
        //        var sqlString = string.Format("select distinct CBFBM,CBFMC from {0} order by CBFBM", SelectFeatrue);
        //        AccessFactory accessFactory = new AccessFactory(PersonDatabase);
        //        var dt = accessFactory.Query(sqlString);
        //        using (FileStream fileStream = new FileStream(savedExcelPath, FileMode.Open, FileAccess.ReadWrite))
        //        {
        //            IWorkbook workbook = new HSSFWorkbook(fileStream);
        //            ISheet sheet = workbook.GetSheetAt(1);
        //            int startRow = 2;
        //            int rowCount = 7;
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                int currentRow = startRow + i / rowCount;
        //                NPOI.SS.UserModel.IRow row = sheet.GetRow(currentRow);
        //                row.GetCell(i % rowCount * 2).SetCellValue((i + 1).ToString());
        //                row.GetCell(i % rowCount * 2 + 1).SetCellValue(dt.Rows[i][1].ToString());
        //            }
        //            int endRow = startRow + dt.Rows.Count / rowCount + 1;
        //            for (int i = sheet.LastRowNum; i >= endRow; i--)
        //            {
        //                sheet.ShiftRows(i, i + 1, -1);
        //            }
        //            FileStream fs = new FileStream(savedExcelPath, FileMode.Create, FileAccess.Write);
        //            workbook.Write(fs);
        //            fs.Close();
        //            fileStream.Close();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        System.Windows.Forms.MessageBox.Show(e.ToString());
        //    }

        //}
    }
}
