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
    /// Interaction logic for WinDkbm.xaml
    /// </summary>
    public partial class WinDkbm : Window
    {
        public WinDkbm()
        {
            InitializeComponent();
        }

        public string Fbfbm { get; set; }
        public double Length { get; set; }
        public double Gap { get; set; }


        public WinDkbm( double length)
        {
           
            //Fbfbm = fbfbm;
            Length = length;
            //Gap = gap;
            InitializeComponent();
            InitControl();
        }

        private void InitControl()
        {
            this.ImageClose.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => this.Close();
            this.TextBoxVerticalLength.Text = Length.ToString("f");
            this.ButtonConfirm.Click += (object sender, RoutedEventArgs e) => Save();
        }

        private void Save()
        {
            Fbfbm = this.TextBoxFbf.Text.Trim();
            if (string.IsNullOrEmpty(Fbfbm)||Fbfbm.Length!=14)
            {
                MessageBox.MessageWarning.Show("系统提示", "发包方编码填写错误");
                return;
            }
            double tryGap;
            var ret = double.TryParse(this.TextBoxGap.Text.Trim(), out tryGap);
            if (!ret)
            {
                MessageBox.MessageWarning.Show("系统提示", "南北间隔填写错误");
                return;
            }
            Gap = tryGap;
            if (Gap > Length || Gap <= 0)
            {
                MessageBox.MessageWarning.Show("系统提示", "南北间隔填写正确的区间");
                return;
            }
            this.DialogResult = true;
        }
    }
}
