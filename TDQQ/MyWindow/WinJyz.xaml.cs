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
    /// Interaction logic for WinJyz.xaml
    /// </summary>
    public partial class WinJyz : Window
    {
        public WinJyz()
        {
            InitializeComponent();
            InitControls();
        }
        public string Xian { get; set; }
        public string Xiang { get; set; }
        public string Cun { get; set; }

        private void InitControls()
        {
            this.ImageClose.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => this.Close();
            this.ButtonConfirm.Click += (object sender, RoutedEventArgs e) =>
            {
                Xian = this.TextBoxXian.Text.Trim();
                Xiang = this.TextBoxXiang.Text.Trim();
                Cun = this.TextBoxCun.Text.Trim();
                if (string.IsNullOrEmpty(Xian) || string.IsNullOrEmpty(Xiang) || string.IsNullOrEmpty(Cun))
                {
                    MessageWarning.Show("系统提示", "信息提示不全");
                    return;
                }
                this.DialogResult = true;
            };
        }
    }
}
