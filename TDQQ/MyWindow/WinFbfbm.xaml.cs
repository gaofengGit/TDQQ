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

namespace TDQQ.MyWindow
{
    /// <summary>
    /// Interaction logic for WinFbfbm.xaml
    /// </summary>
    public partial class WinFbfbm : Window
    {
        public WinFbfbm()
        {
            InitializeComponent();
            InitControl();
        }
        public string Fbfbm { get; set; }
        private void InitControl()
        {
            this.ImageClose.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => this.Close();
            this.ButtonConfirm.Click += (object sender, RoutedEventArgs e) => this.SaveFbfbm();
        }

        private void SaveFbfbm()
        {
            Fbfbm = this.TextBoxFbfbm.Text.Trim();
            if (string.IsNullOrEmpty(Fbfbm) || Fbfbm.Length != 14)
            {
                MessageBox.MessageWarning.Show("系统提示", "发包方填写错误");
                this.TextBoxFbfbm.SelectAll();
                this.TextBoxFbfbm.Focus();
                return;
            }
            this.DialogResult = true;
        }
    }
}
