
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using TDQQ.AE;
using TDQQ.Common;

namespace TDQQ.Export
{
    class ExportBase
    {
        public string PersonDatabase { get; set; }
        public string SelectFeatrue { get; set; }
        public string BasicDatabase { get; set; }

        public ExportBase()
        {
        }
        public ExportBase(string personDatabase, string selectFeatrue, string basicDatabase)
        {
            PersonDatabase = personDatabase;
            SelectFeatrue = selectFeatrue;
            BasicDatabase = basicDatabase;
        }
        /// <summary>
        /// 用户在弹出对话框中选择文件存放的位置（为Excel文件）
        /// </summary>
        /// <param name="templatePath">模板文件的位置</param>
        /// <param name="title">对话框标题</param>
        /// <returns>已经Copy的文件地址</returns>
        protected string CopyTemplateTo(string templatePath, string title)
        {
            var dialogFactory = new DialogFactory("Excel文件|*.xls");
            if (string.IsNullOrEmpty(templatePath)) return null;
            var savePath = dialogFactory.SaveFile(title);
            if (string.IsNullOrEmpty(savePath))
            {
                return null;
            }
            File.Copy(templatePath, savePath, true);
            return savePath;
        }

        /// <summary>
        /// 合并单元格样式
        /// </summary>
        /// <param name="workbook">工作簿</param>
        /// <returns>样式</returns>
        protected ICellStyle MergetStyle(IWorkbook workbook)
        {
            ICellStyle style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.CENTER;
            style.VerticalAlignment = VerticalAlignment.CENTER;
            style.BorderBottom = BorderStyle.THIN;
            style.BorderRight = BorderStyle.THIN;
            style.BorderLeft = BorderStyle.THIN;
            style.BorderTop = BorderStyle.THIN;
            style.WrapText = true;
            return style;
        }

        /// <summary>
        /// 处理合并后的Excel文件
        /// </summary>
        /// <param name="workbook">工作簿</param>
        /// <param name="lastRowIndex">最后一行的序号</param>
        protected virtual void EditExcel(IWorkbook workbook, int lastRowIndex, int sheetIndex)
        {
        }

        public virtual bool Export()
        {
            return true;
        }

        protected bool CheckValid()
        {
            return AeHelper.CheckVaild(PersonDatabase, SelectFeatrue);
        }
        private IEnumerable<Farmer> GetFarmer()
        {
            var sqlString = string.Format("select distinct CBFBM,CBFMC from {0} order by CBFBM", SelectFeatrue);
            AccessFactory accessFactory = new AccessFactory(PersonDatabase);
            var dt = accessFactory.Query(sqlString);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                yield return new Farmer() { Index = i + 1, Cbfmc = dt.Rows[i][1].ToString() };
            }
        }
        protected void FillCbfmcIndexSheet(string savedExcelPath)
        {
            try
            {
                //var sqlString = string.Format("select distinct CBFBM,CBFMC from {0} order by CBFMC", SelectFeatrue);
                //AccessFactory accessFactory = new AccessFactory(PersonDatabase);
                //var dt = accessFactory.Query(sqlString);
                var farmers = GetFarmer();
                var sortFarmersByCbfmc = farmers.OrderBy(farmer => farmer.Cbfmc);
                using (FileStream fileStream = new FileStream(savedExcelPath, FileMode.Open, FileAccess.ReadWrite))
                {
                    IWorkbook workbook = new HSSFWorkbook(fileStream);
                    ISheet sheet = workbook.GetSheetAt(1);
                    int startRow = 2;
                    int rowCount = 7;
                    int index = 0;
                    foreach (var farmer in sortFarmersByCbfmc)
                    {
                        int currentRow = startRow + index / rowCount;
                        NPOI.SS.UserModel.IRow row = sheet.GetRow(currentRow);
                        row.GetCell(index % rowCount * 2).SetCellValue((farmer.Index).ToString());
                        row.GetCell(index % rowCount * 2 + 1).SetCellValue(farmer.Cbfmc);
                        index++;
                    }
                    int endRow = startRow + index / rowCount + 1;
                    for (int i = sheet.LastRowNum; i >= endRow + 1; i--)
                    {
                        sheet.ShiftRows(i, i + 1, -1);
                    }
                    FileStream fs = new FileStream(savedExcelPath, FileMode.Create, FileAccess.Write);
                    workbook.Write(fs);
                    fs.Close();
                    fileStream.Close();
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }

        }   
    }

    class Farmer
    {
        public int Index { get; set; }
        public string Cbfmc { get; set; }

       
    }
}
