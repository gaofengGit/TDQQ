
using System.Windows;
using TDQQ.Common;

namespace TDQQ.MyWindow
{
    /// <summary>
    /// Interaction logic for WinSetFieldsValue.xaml
    /// </summary>
    public partial class WinSetFieldsValue : Window
    {
        public WinSetFieldsValue()
        {
            InitializeComponent();
            InitControl();
        }

        #region 属性字段
        public string Fbfbm { get; set; }
        public string Zjrxm { get; set; }
        public string Tdlylx { get; set; }
        public string Cbjyqqdfs { get; set; }
        public string Syqxz { get; set; }
        public string Tdyt { get; set; }

        public string Dldj { get; set; }
        public string Sfjbnt { get; set; }
        public string Dklb { get; set; }
        #endregion

        private void InitControl()
        {
            this.ImageClose.MouseLeftButtonDown += (object sender, System.Windows.Input.MouseButtonEventArgs e)=>this.Close();
            this.ButtonConfirm.Click += (object sender, RoutedEventArgs e) => Confirm();
        }

       
        private void Confirm()
        {
            Fbfbm = this.TextBoxFbfbm.Text.Trim();
            if (string.IsNullOrEmpty(Fbfbm) || Fbfbm.Length != 14)
            {
                this.TextBoxFbfbm.Focus();
                this.TextBoxFbfbm.SelectAll();
                MessageBox.MessageWarning.Show("系统提示", "请填写14位发包方编码");
                return;
            }
            Zjrxm = this.TextBoxZjrxm.Text.Trim();
            Tdlylx = Transcode.TdlylxCombox(this.ComboBoxTdlylx.SelectedIndex);
            Cbjyqqdfs = Transcode.CbjyqqdfsCombox(this.ComboBoxCbjyqqdfs.SelectedIndex);
            Syqxz = Transcode.SyqxzCombox(this.ComboBoxSyqxz.SelectedIndex);
            Tdyt = Transcode.TdytCombox(this.ComboBoxTdyt.SelectedIndex);
            Dldj = Transcode.DldjCombox(this.ComboBoxDldj.SelectedIndex);
            Sfjbnt = Transcode.SfwjbntCombox(this.ComboBoxSfjbnt.SelectedIndex);
            Dklb = Transcode.DklbCombox(this.ComboBoxDldj.SelectedIndex);
            this.DialogResult = true;
        }
    }
}
