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
    /// Interaction logic for WinBuffer.xaml
    /// </summary>
    public partial class WinBuffer : Window
    {
        public WinBuffer()
        {
            InitializeComponent();
            InitControl();
        }
        public double Distance { get; set; }

        private void InitControl()
        {
            this.ImageClose.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => this.Close();
            this.ButtonConfirm.Click += (object sender, RoutedEventArgs e) => Save();
        }

        private void Save()
        {
            double inputDistance;
            var ret = double.TryParse(this.TextBoxDistance.Text.Trim(), out inputDistance);
            if (!ret)
            {
                MessageWarning.Show("系统提示", "请输入正确数值");
                this.TextBoxDistance.Focus();
                this.TextBoxDistance.SelectAll();
            }
            Distance = inputDistance;
            this.DialogResult = true;
        }
    }
}
