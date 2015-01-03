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
    /// 导出颁证清册
    /// </summary>
    class ExportList : ExportBase
    {
        public ExportList() : base() { }

        public ExportList(string personDatabase, string selectFeaure, string basicDatbase) : base(personDatabase, selectFeaure, basicDatbase) { }

        public override bool Export()
        {
            if (!CheckValid())
            {
                MessageBox.MessageWarning.Show("系统提示", "字段尚未全部添加！");
                return false;
            }   
            var templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\template\颁证清册.xls";
            var savedFilePath = CopyTemplateTo(templatePath, "颁证清册");
            if (string.IsNullOrEmpty(savedFilePath)) return false;
            Wait wait = new Wait();
            wait.SetInfoInvoke("正在导出颁证清册");
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
                var sqlString = string.Format("Select distinct CBFBM,CBFMC from {0} order by CBFBM ", SelectFeatrue);
                var accessFactory = new AccessFactory(PersonDatabase);
                var dt = accessFactory.Query(sqlString);
                if (dt == null)
                {
                    wait.CloseWait();
                    para["ret"] = false;
                    return;
                }
                int startRow = 5, endRow = 5;
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
                        string cbfmc = dt.Rows[i][1].ToString();
                        double htmj = 0.0;
                        double scmj = 0.0;
                        var endRowField = FillFields(dt1, workbookSource, endRow, ref scmj);
                        var endRowFamily = FillFamily(dt2, workbookSource, endRow);
                        endRow = Math.Max(endRowField, endRowFamily);
                        MergeCells(workbookSource, i + 1, cbfmc, dt2.Rows.Count, cbfbm, scmj, startRow, endRow, style);
                        endRow++;
                        startRow = endRow;
                    }
                    EditExcel(workbookSource, endRow, 0);
                    FileStream fs = new FileStream(savedFilePath, FileMode.Create, FileAccess.Write);
                    workbookSource.Write(fs);
                    fs.Close();
                    fileStream.Close();
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
        protected override void EditExcel(IWorkbook workbook, int endRow, int sheetIndex)
        {
            //删除多余行
            HSSFSheet sheetSource = (HSSFSheet)workbook.GetSheetAt(sheetIndex);
            for (int i = sheetSource.LastRowNum; i >= endRow + 1; i--)
            {
                sheetSource.ShiftRows(i, i + 1, -1);
            }
        }
        private void MergeCells(IWorkbook workbook, int familyIndex, string cbfmc, int familyCount, string cbfbm, double scmj,
            int startRow, int endRow, ICellStyle style)
        {
            ISheet sheet = workbook.GetSheetAt(0);
            IRow row = sheet.GetRow(startRow);
            ICell cell;
            //合并序号单元格
            sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, 0, 0));
            cell = row.GetCell(0);
            cell.CellStyle = style;
            cell.SetCellValue(familyIndex);
            //承包方名称
            sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, 1, 1));
            cell = row.GetCell(1);
            cell.CellStyle = style;
            cell.SetCellValue(cbfmc);
            //家庭成员数量
            sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, 2, 2));
            cell = row.GetCell(2);
            cell.CellStyle = style;
            cell.SetCellValue(familyCount);
            //合同证书
            sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, 4, 4));
            cell = row.GetCell(4);
            cell.CellStyle = style;
            cell.SetCellValue(cbfbm + "J");
            //实测总面积
            sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, 8, 8));
            cell = row.GetCell(8);
            cell.CellStyle = style;
            cell.SetCellValue(scmj);
            //签字
            sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, 9, 9));

        }
        private void GetFillInfoTable(string cbfbm, out System.Data.DataTable dtField, out System.Data.DataTable dtFamily)
        {
            var accessFactoryPerson = new AccessFactory(PersonDatabase);
            var accessFactoryFamily = new AccessFactory(BasicDatabase);
            var sqlString = string.Format("select CYXM,YHZGX from {0}  where trim(CBFBM) ='{1}' Order by YHZGX", "CBF_JTCY",
                    cbfbm);
            dtFamily = accessFactoryFamily.Query(sqlString);
            sqlString = string.Format("select DKMC,DKBM,SCMJ from {0} where trim(CBFBM) ='{1}'", SelectFeatrue, cbfbm);
            dtField = accessFactoryPerson.Query(sqlString);
        }

        private int FillFields(System.Data.DataTable dtFields, IWorkbook workbook, int endRow, ref double scmj)
        {
            ISheet sheet = workbook.GetSheetAt(0);
            for (int i = 0; i < dtFields.Rows.Count; i++)
            {
                IRow row = sheet.GetRow(endRow + i);
                row.GetCell(5).SetCellValue(dtFields.Rows[i][0].ToString());
                row.GetCell(6).SetCellValue(dtFields.Rows[i][1].ToString());
                double singleScmj = Convert.ToDouble(Convert.ToDouble(dtFields.Rows[i][2].ToString()).ToString("f"));
                row.GetCell(7).SetCellValue(singleScmj);
                scmj += singleScmj;
            }
            return endRow + dtFields.Rows.Count - 1;
        }

        private int FillFamily(System.Data.DataTable dtFamily, IWorkbook workbook, int endRow)
        {
            ISheet sheet = workbook.GetSheetAt(0);
            for (int i = 0; i < dtFamily.Rows.Count; i++)
            {
                IRow row = sheet.GetRow(endRow + i);
                row.GetCell(3).SetCellValue(dtFamily.Rows[i][0].ToString());
            }
            return endRow + dtFamily.Rows.Count - 1;
        }

    }
}
