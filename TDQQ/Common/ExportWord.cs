using System;
using Microsoft.Office.Interop.Word;
using System.Runtime.InteropServices;

namespace TDQQ.Common
{
    public class ExportWord
    {
        //私有成员
        private _Application wordApp = null;
        private _Document wordDoc = null;
        //公共属性
        public _Application Application { get; set; }
        public _Document Document { get; set; }
        //方法
        //通过模板创建新文档
        public void CreateNewDocument(string filePath)
        {
            KillWinWordProcess();
            wordApp = new ApplicationClass();
            wordApp.DisplayAlerts = WdAlertLevel.wdAlertsNone;
            wordApp.Visible = false;
            object missing = System.Reflection.Missing.Value;
            object templateName = filePath;
            wordDoc = wordApp.Documents.Open(ref templateName, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing);

        }
        //保存新文件
        public void SaveDocument(string filePath)
        {
            object fileName = filePath;
            object format = WdSaveFormat.wdFormatDocument;//保存格式
            object miss = System.Reflection.Missing.Value;
            wordDoc.SaveAs(ref fileName, ref format, ref miss,
                ref miss, ref miss, ref miss, ref miss,
                ref miss, ref miss, ref miss, ref miss,
                ref miss, ref miss, ref miss, ref miss,
                ref miss);
            //关闭wordDoc，wordApp对象
            object SaveChanges = WdSaveOptions.wdSaveChanges;
            object OriginalFormat = WdOriginalFormat.wdOriginalDocumentFormat;
            object RouteDocument = false;
            wordDoc.Close(ref SaveChanges, ref OriginalFormat, ref RouteDocument);
            wordApp.Quit(ref SaveChanges, ref OriginalFormat, ref RouteDocument);
            Marshal.ReleaseComObject(wordApp);
        }
        //在书签处插入值
        public bool InsertValue(string bookmark, string value)
        {
            object bkObj = bookmark;
            if (wordApp.ActiveDocument.Bookmarks.Exists(bookmark))
            {
                wordApp.ActiveDocument.Bookmarks.get_Item(ref bkObj).Select();
                wordApp.Selection.TypeText(value);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 删除掉表格中多余的行
        /// </summary>
        /// <param name="tableIndex">表格所在的序号</param>
        /// <param name="startRow">开始行</param>
        /// <param name="startCol">开始列</param>
        /// <param name="count">总共需要删除的次数</param>
        public void DeleteTableRow(int tableIndex, int startRow, int startCol, int count)
        {
            Microsoft.Office.Interop.Word.Table appTable = wordDoc.Tables[tableIndex];
            for (int i = 0; i < count; i++)
            {
                appTable.Cell(startRow, startCol).Range.Rows.Delete();
            }
        }
        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="tableIndex">表的序号</param>
        /// <param name="startRow">合并单元格开始的行</param>
        /// <param name="startCol">合并单元格开始的列</param>
        /// <param name="stopRow">合并单元格结束的行</param>
        /// <param name="stopCol">合并单元格结束的列</param>
        public void MergeTableCells(int tableIndex, int startRow, int startCol, int stopRow, int stopCol)
        {
            Microsoft.Office.Interop.Word.Table appTable = wordDoc.Tables[tableIndex];
            appTable.Cell(startRow, startCol).Merge(appTable.Cell(stopRow, stopCol));
        }

        public void FillCellText(int tableIndex, int fillCellRow, int fillCellCol, string text)
        {
            Microsoft.Office.Interop.Word.Table appTable = wordDoc.Tables[tableIndex];
            appTable.Cell(fillCellRow, fillCellCol).Range.Text = text;
        }
        // 杀掉winword.exe进程
        public void KillWinWordProcess()
        {
            try
            {
                System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("WINWORD");
                foreach (System.Diagnostics.Process process in processes)
                {
                    bool b = process.MainWindowTitle == "";
                    if (process.MainWindowTitle == "")
                    {
                        process.Kill();
                    }
                }
            }
            catch (Exception e)
            {
                return;
            }
        }
    }
}
