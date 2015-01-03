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
using TDQQ.MessageBox;

namespace TDQQ.MyWindow
{
    /// <summary>
    /// Interaction logic for WinReplce.xaml
    /// </summary>
    public partial class WinReplce : Window
    {
        public WinReplce()
        {
            InitializeComponent();
            InitControl();
        }

        public string OriginCbfmc { get; private set; }
        public string NewCbfmc { get; set; }

        private void InitControl()
        {
            this.ImageClose.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => this.Close();
            this.ButtonConfirm.Click += (object sender, RoutedEventArgs e) => InputCbfmc();
        }

        private void InputCbfmc()
        {
            OriginCbfmc = this.TextBoxOrigin.Text.Trim();
            if (string.IsNullOrEmpty(OriginCbfmc))
            {
                MessageWarning.Show("系统提示", "请输入要替换的承包方名称");
                this.TextBoxOrigin.SelectAll();
                this.TextBoxOrigin.Focus();
                return;
            }
            NewCbfmc = this.TextBoxNew.Text.Trim();
            if (string.IsNullOrEmpty(NewCbfmc))
            {
                MessageWarning.Show("系统提示", "请输入替换后承包方名称");
                this.TextBoxNew.SelectAll();
                this.TextBoxNew.Focus();
                return;
            }
            this.DialogResult = true;
        }

        
    }
}
