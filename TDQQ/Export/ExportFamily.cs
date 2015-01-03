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
    class ExportFamily : ExportBase
    {
        public ExportFamily() : base() { }
        public ExportFamily(string personDatabase, string seletctFeature, string basicDatabase) : base(personDatabase, seletctFeature, basicDatabase) { }

        public override bool Export()
        {
            if (!CheckValid())
            {
                MessageBox.MessageWarning.Show("系统提示", "字段尚未全部添加！");
                return false;
            }
            //return base.Export();
            var templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\template\家庭成员信息表.xls";
            var savedFilePath = CopyTemplateTo(templatePath, "公示表");
            if (string.IsNullOrEmpty(savedFilePath)) return false;
            Wait wait = new Wait();
            wait.SetInfoInvoke("正在导出家庭成员.....");
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
            var sqlString = string.Format("select  CBFBM,CBFMC from {0}", "CBF");
            var accessFactory = new AccessFactory(BasicDatabase);
            var dt = accessFactory.Query(sqlString);
            if (dt == null)
            {
                wait.CloseWait();
                para["ret"] = false;
                return;
            }
            int startRow = 3, endRow = 3;
            using (var fileStream = new System.IO.FileStream(savedFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                IWorkbook workbookSource = new HSSFWorkbook(fileStream);
                ICellStyle style = MergetStyle(workbookSource);
                var rowCount = dt.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    wait.SetProgressInfo(((double)i / (double)rowCount).ToString("P"));
                    var cbfbm = dt.Rows[i][0].ToString();
                    int familyCount;
                    FillOneFamily(workbookSource, cbfbm, ref endRow, out familyCount);
                    //合并单元格
                    ISheet sheet = workbookSource.GetSheetAt(0);
                    IRow row = sheet.GetRow(startRow);
                    //序号
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, 0, 0));
                    ICell cell = row.GetCell(0);
                    cell.CellStyle = style;
                    cell.SetCellValue((i + 1).ToString());
                    //户主姓名
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, 1, 1));
                    cell = row.GetCell(1);
                    cell.CellStyle = style;
                    cell.SetCellValue(dt.Rows[i][1].ToString());
                    //家庭成员数量
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, 2, 2));
                    cell = row.GetCell(2);
                    cell.CellStyle = style;
                    cell.SetCellValue(familyCount.ToString());
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

        private void FillOneFamily(IWorkbook workbook, string cbfbm, ref int endRow, out int familyCount)
        {
            var sqlString = string.Format("select CYXM,CYZJHM,YHZGX from {0} where CBFBM='{1}' order by YHZGX", "CBF_JTCY", cbfbm);
            AccessFactory accessFactory = new AccessFactory(BasicDatabase);
            var dt = accessFactory.Query(sqlString);
            if (dt == null)
            {
                familyCount = 0;
                return;
            }
            familyCount = dt.Rows.Count;
            ISheet sheet = workbook.GetSheetAt(0);
            for (int i = 0; i < familyCount; i++)
            {
                IRow row = sheet.GetRow(endRow + i);
                row.GetCell(3).SetCellValue(dt.Rows[i][0].ToString());
                row.GetCell(4).SetCellValue(dt.Rows[i][1].ToString());
                row.GetCell(5).SetCellValue(Transcode.CodeToRelationship(dt.Rows[i][2].ToString()));
                //endRow++;
            }
            endRow = endRow + familyCount - 1;
        }

        protected override void EditExcel(IWorkbook workbook, int endRow, int sheetIndex)
        {
            //base.EditExcel(workbook, lastRowIndex);
            HSSFSheet sheetSource = (HSSFSheet)workbook.GetSheetAt(sheetIndex);
            for (int i = sheetSource.LastRowNum; i >= endRow + 1; i--)
            {
                sheetSource.ShiftRows(i, i + 1, -1);
            }
        }
    }
}
