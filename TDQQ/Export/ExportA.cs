using System;
using System.IO;
using NPOI.HSSF.UserModel;
using TDQQ.Common;

namespace TDQQ.Export
{
    class ExportA : ExportBase
    {
        public ExportA()
            : base()
        {

        }
        public ExportA(string personDatabase, string selectFeatrue, string basicDatabase)
            : base(personDatabase, selectFeatrue, basicDatabase)
        {

        }
        public override bool Export()
        {

           
            var templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\template\发包方调查表.xls";
            var savedPath = CopyTemplateTo(templatePath, "导出发包方调查表");
            if (string.IsNullOrEmpty(savedPath)) return false;
            try
            {
                var sqlString = string.Format("SELECT FBFMC,FBFBM,FBFFZRXM,LXDH," +
                                              "FBFDZ,YZBM,FZRZJLX,FZRZJHM,FBFDCJS,FBFDCY from FBF");
                var accessFactory = new AccessFactory(BasicDatabase);
                var dt = accessFactory.Query(sqlString);
                if (dt == null) return false;
                return ExprotToExcel(savedPath, dt);
            }
            catch (Exception)
            {
                return false;
            }

        }

        private bool ExprotToExcel(string savedPath, System.Data.DataTable dtInfo)
        {
            using (var fileStream = new FileStream(savedPath, FileMode.Open, FileAccess.ReadWrite))
            {
                //设置格式要素
                var workbookSource = new HSSFWorkbook(fileStream);
                var sheetSource = workbookSource.GetSheetAt(0);
                var rowSource = sheetSource.GetRow(3);
                rowSource.GetCell(CommonHelper.Col('C')).SetCellValue(dtInfo.Rows[0][0].ToString());
                rowSource.GetCell(CommonHelper.Col('H')).SetCellValue(dtInfo.Rows[0][1].ToString());
                rowSource = sheetSource.GetRow(4);
                rowSource.GetCell(CommonHelper.Col('C')).SetCellValue(dtInfo.Rows[0][2].ToString());
                rowSource.GetCell(CommonHelper.Col('H')).SetCellValue(dtInfo.Rows[0][3].ToString());
                rowSource = sheetSource.GetRow(5);
                rowSource.GetCell(CommonHelper.Col('C')).SetCellValue(dtInfo.Rows[0][4].ToString());
                rowSource.GetCell(CommonHelper.Col('I')).SetCellValue(dtInfo.Rows[0][5].ToString());
                rowSource = sheetSource.GetRow(6);
                rowSource.GetCell(CommonHelper.Col('D')).SetCellValue(Transcode.Fbfzjlx(dtInfo.Rows[0][6].ToString().Trim()));
                rowSource.GetCell(CommonHelper.Col('I')).SetCellValue(dtInfo.Rows[0][7].ToString());
                rowSource = sheetSource.GetRow(7);
                rowSource.GetCell(CommonHelper.Col('B')).SetCellValue(dtInfo.Rows[0][8].ToString());
                rowSource = sheetSource.GetRow(14);
                rowSource.GetCell(CommonHelper.Col('B')).SetCellValue(dtInfo.Rows[0][9].ToString());
                var fs = new FileStream(savedPath, FileMode.Create, FileAccess.Write);
                workbookSource.Write(fs);
                fs.Close();
                fileStream.Close();
            }
            return true;
        }
    }
}
