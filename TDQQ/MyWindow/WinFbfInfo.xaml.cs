using System.Windows;
using System.Windows.Input;
using TDQQ.Common;
using TDQQ.MessageBox;

namespace TDQQ.MyWindow
{
    /// <summary>
    /// Interaction logic for WinFbfInfo.xaml
    /// </summary>
    public partial class WinFbfInfo : Window
    {
        public WinFbfInfo()
        {
            InitializeComponent();
            InitControl();
        }
        #region 属性字段

        public string Fbfmc { get; set; }
        public string Fbfbm { get; set; }
        public string Fzrxm { get; set; }
        public string Lxdh { get; set; }
        public string Fbfdz { get; set; }
        public string Yzbm { get; set; }
        public string Fzrzjlx { get; set; }
        public string Zjhm { get; set; }
        #endregion

        private void InitControl()
        {
            this.ImageClose.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => this.Close();
            this.ButtonConfirm.Click += (object sender, RoutedEventArgs e) =>
            {
                if (!ValidCheck()) return;
                this.DialogResult = true;
            };
        }

        private bool ValidCheck()
        {
            Fbfmc = this.TextBoxFbfmc.Text.Trim();
            Fbfbm = this.TextBoxFbfbm.Text.Trim();
            Fzrxm = this.TextBoxFbffzrxm.Text.Trim();
            Lxdh = this.TextBoxLxdh.Text.Trim();
            Fbfdz = this.TextBoxFbfdz.Text.Trim();
            Yzbm = this.TextBoxYzbm.Text.Trim();
            Fzrzjlx = Transcode.ComboxFbfzjlx(this.ComboBoxZjlx.SelectedIndex);
            Zjhm = this.TextBoxZjhm.Text.Trim();
            if (string.IsNullOrEmpty(Fbfmc)||!Fbfmc.Contains("村民委员会"))
            {
                MessageWarning.Show("系统提示", "承包方名称错误");
                return false;
            }
            if (string.IsNullOrEmpty(Fbfbm) || Fbfbm.Length != 14||Fbfbm.Substring(12)!="00")
            {
                MessageWarning.Show("系统提示", "承包方编码错误");
                return false;
            }
            if (string.IsNullOrEmpty(Fbfdz))
            {
                MessageWarning.Show("系统提示", "承包方地址错误");
                return false;
            }
            return true;
        }
    }
}
