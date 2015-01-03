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
    /// Interaction logic for WinLicense.xaml
    /// </summary>
    public partial class WinLicense : Window
    {
        public WinLicense()
        {
            InitializeComponent();
        }

        private string _license;

        public WinLicense(string license)
        {
            _license = license;
            InitializeComponent();
            InitControl();
        }

        public void InitControl()
        {
            this.TextBoxLicense.Text = _license;
            this.TextBoxLicense.SelectAll();
            this.ButtonConfirm.Click += (object sender, RoutedEventArgs e) =>
            {
                this.DialogResult = true;
            };
        }

       
    }
}
