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
using System.Windows.Threading;

namespace TDQQ.MyWindow
{
    /// <summary>
    /// Interaction logic for Wait.xaml
    /// </summary>
    public partial class Wait : Window
    {
        public Wait()
        {
            InitializeComponent();
        }
        public void SetInfoInvoke(string info)
        {
            //this.LabelInfo.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            //{
            //    this.LabelInfo.Content = info;
            //}));
            this.LabelInfo.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                this.LabelInfo.Content = info;
            }));
        }

        public void SetProgressInfo(string progress)
        {
            //this.LabelInfo.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            //{
            //    this.LabelProgress.Content = progress;
            //}));
            this.LabelInfo.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                this.LabelProgress.Content = progress;
            }));
        }

        public void CloseWait()
        {
            //this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>Close()));
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => Close()));
        }
    }
}
