using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TDQQ.Common;

namespace TDQQ.MyWindow
{
    /// <summary>
    /// Interaction logic for WinMultiFieldInfo.xaml
    /// </summary>
    public partial class WinMultiFieldInfo : Window
    {
        public WinMultiFieldInfo()
        {
            InitializeComponent();
            InfoList = new List<string>();
            InitControls();
        }
        public List<string> InfoList;
        private void InitControls()
        {
            this.LabelMove.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => this.DragMove();
            this.ImageClose.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => this.Close();
            this.ButtonSave.Click += (object sender, RoutedEventArgs e) => SaveInfo();
        }
        private void SaveInfo()
        {
            InfoList.Add(this.TextBoxFieldName.Text.Trim());
            InfoList.Add(this.TextBoxZjrxm.Text.Trim());
            InfoList.Add(this.TextBoxDkbzxx.Text.Trim());
            InfoList.Add(this.TextBoxDkdz.Text.Trim());
            InfoList.Add(this.TextBoxDknz.Text.Trim());
            InfoList.Add(this.TextBoxDkxz.Text.Trim());
            InfoList.Add(this.TextBoxDkbz.Text.Trim());
            InfoList.Add(Transcode.CbjyqqdfsCombox(this.ComboBoxCbjyqqdfs.SelectedIndex));
            InfoList.Add(Transcode.TdlylxCombox(this.ComboBoxTdlylx.SelectedIndex));
            InfoList.Add(Transcode.SfwjbntCombox(this.ComboBoxSfjbnt.SelectedIndex));
            InfoList.Add(Transcode.DklbCombox(this.ComboBoxTklb.SelectedIndex));
            InfoList.Add(Transcode.DldjCombox(this.ComboBoxDldj.SelectedIndex));
            InfoList.Add(Transcode.SyqxzCombox(this.ComboBoxSyqxz.SelectedIndex));
            InfoList.Add(Transcode.TdytCombox(this.ComboBoxTdyt.SelectedIndex));
            this.DialogResult = true;
        }
    }
}
