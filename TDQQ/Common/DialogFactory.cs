using System.Windows.Forms;

namespace TDQQ.Common
{
    public sealed class DialogFactory
    {
        public string Extension { get; set; }
        public DialogFactory(string extension)
        {
            Extension = extension;
        }
        public DialogFactory()
        { }
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="title">对话框的标题</param>
        /// <returns>文件的路径</returns>
        public string OpenFile(string title)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = Extension;
            dialog.Title = title;
            dialog.RestoreDirectory = true;
            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : string.Empty;
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="title">对话框的标题</param>
        /// <returns>保存的路径</returns>
        public string SaveFile(string title)
        {
            var dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.Filter = Extension;
            dialog.RestoreDirectory = true;
            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : string.Empty;
        }
        /// <summary>
        /// 打开文件夹对话框
        /// </summary>
        /// <returns>文件路劲</returns>
        public string OpenFolderDialog()
        {
            var folerDialog = new FolderBrowserDialog();
            folerDialog.ShowNewFolderButton = true;
            return folerDialog.ShowDialog() == DialogResult.OK? folerDialog.SelectedPath:string.Empty;
        }
    }
}
