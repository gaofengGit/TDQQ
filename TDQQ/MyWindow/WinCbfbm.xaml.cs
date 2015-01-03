using System.IO;
using System.Windows;
using System.Windows.Input;
using NPOI.HSSF.UserModel;
using TDQQ.Common;
using TDQQ.MessageBox;

namespace TDQQ.MyWindow
{
    /// <summary>
    /// Interaction logic for WinCbfbm.xaml
    /// </summary>
    public partial class WinCbfbm : Window
    {
        public WinCbfbm()
        {
            InitializeComponent();
            InitControl();
        }

        public string ExcelPath { get; set; }
        public int StartOrder { get; set; }
        public string Fbfbm { get; set; }

        private void InitControl()
        {
            ExcelPath = Fbfbm = string.Empty;
            StartOrder = -1;
            this.ImageClose.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => this.Close();
            this.ButtonOpen.Click += (object sender, RoutedEventArgs e) =>
            {
                var dialogFactory = new DialogFactory("家庭成员表（.xls）|*.xls");
                var ret = dialogFactory.OpenFile("打开基础信息表");
                if (!string.IsNullOrEmpty(ret)) ExcelPath = ret;
            };
            this.ButtonConfirm.Click += (object sender, RoutedEventArgs e) =>
            {
                if (!ValidCheck()) return;
                Process();
            };
        }

        private bool ValidCheck()
        {
            //检查是否打开表
            if (string.IsNullOrEmpty(ExcelPath))
            {
                MessageWarning.Show("系统提示", "未打开基础数据表");
                return false;
            }
            Fbfbm = TextBoxFbfbm.Text.Trim();
            //检查是否为14为发包方编码
            if (string.IsNullOrEmpty(Fbfbm) || Fbfbm.Length != 14)
            {
                MessageWarning.Show("系统提示", "发包方信息填写错误");
                return false;
            }
            //检查起始标号
            int startOrder;
            var ret = int.TryParse(this.TextBoxStart.Text.Trim(), out startOrder);
            if (!ret)
            {
                MessageWarning.Show("系统提示", "起始编号输入有误");
                return false;
            }
            StartOrder = startOrder - 1;
            return true;
        }
        private void Process()
        {
            using (FileStream fileStream = new FileStream(ExcelPath, FileMode.Open, FileAccess.ReadWrite))
            {
                //FileStream fileStream = new FileStream(fileSource, FileMode.Open, FileAccess.ReadWrite);
                HSSFWorkbook workbookSource = new HSSFWorkbook(fileStream);
                HSSFSheet sheetSource = (HSSFSheet)workbookSource.GetSheetAt(0);
                string homeName = string.Empty;
                for (int i = 1; i <= sheetSource.LastRowNum; i++)
                {
                    HSSFRow rowSource = (HSSFRow)sheetSource.GetRow(i);//获取一行数据
                    if (rowSource == null) break;
                    if (rowSource.GetCell(0) == null)
                    {
                        break;
                    }
                    if (homeName == rowSource.GetCell(0).ToString().Trim())
                    {
                        rowSource.GetCell(0).SetCellValue(IntToString(StartOrder));
                    }
                    else
                    {
                        StartOrder++;
                        homeName = rowSource.GetCell(0).ToString().Trim();
                        rowSource.GetCell(0).SetCellValue(IntToString(StartOrder));
                    }
                }
                FileStream fs = new FileStream(ExcelPath, FileMode.Create, FileAccess.Write);

                workbookSource.Write(fs);
                fs.Close();
                fileStream.Close();
                MessageInfomation.Show("系统提示", "承包方编码修改成功！");
            }
        }
        private string IntToString(int ordervalue)
        {
            return Fbfbm + ordervalue.ToString("0000");
        }
    }
}
