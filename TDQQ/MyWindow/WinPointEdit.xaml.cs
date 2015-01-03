using System;
using System.Collections.Generic;
using System.Data;
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
using ESRI.ArcGIS.Geodatabase;
using TDQQ.Common;
using TDQQ.MessageBox;

namespace TDQQ.MyWindow
{
    /// <summary>
    /// Interaction logic for WinPointEdit.xaml
    /// </summary>
    public partial class WinPointEdit : Window
    {
        public WinPointEdit()
        {
            InitializeComponent();
        }

        private string _personDatabase;
        private string _selectFeatrue;

        public WinPointEdit(string personDatabase, string selectFeature)
        {
            _personDatabase = personDatabase;
            _selectFeatrue = selectFeature;
            InitializeComponent();
            InitControls();
        }
        private void InitControls()
        {
            this.LabelMove.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => this.DragMove();
            this.ImageClose.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => this.Close();
            this.ButtonSave.Click += (object sender, RoutedEventArgs e) => SaveInfo();
        }
        public void ShowInfo(IFeature pFeaure)
        {
            var cbfmcIndex = pFeaure.Fields.FindField("CBFMC");
            var cbfbmIndex = pFeaure.Fields.FindField("CBFBM");
            var dkmcIndex = pFeaure.Fields.FindField("DKMC");
            var dkbmIndex = pFeaure.Fields.FindField("DKBM");
            var yhtmjIndex = pFeaure.Fields.FindField("YHTMJ");
            var htmjIndex = pFeaure.Fields.FindField("HTMJ");
            var scmjIndex = pFeaure.Fields.FindField("SCMJ");
            var dkdzIndex = pFeaure.Fields.FindField("DKDZ");
            var dknzIndex = pFeaure.Fields.FindField("DKNZ");
            var dkxzIndex = pFeaure.Fields.FindField("DKXZ");
            var dkbzIndex = pFeaure.Fields.FindField("DKBZ");
            if (cbfbmIndex == -1 || cbfmcIndex == -1 || dkmcIndex == -1 || yhtmjIndex == -1 || scmjIndex == -1 ||
                htmjIndex == -1 || dkdzIndex == -1 || dknzIndex == -1 || dkxzIndex == -1 || dkbzIndex == -1 || dkbmIndex == -1)
            {
                MessageWarning.Show("系统提示", "尚不存在部分字段");
                return;
            }
            this.TextBoxCbfmc.Text = pFeaure.get_Value(cbfmcIndex).ToString();
            this.TextBoxCbfbm.Text = pFeaure.get_Value(cbfbmIndex).ToString();
            this.TextBoxDkmc.Text = pFeaure.get_Value(dkmcIndex).ToString();
            this.TextBoxDkbm.Text = pFeaure.get_Value(dkbmIndex).ToString();
            if (string.IsNullOrEmpty(pFeaure.get_Value(yhtmjIndex).ToString()))
            {
                this.TextBoxYhtmj.Text = "N/A";
            }
            else
            {
                this.TextBoxYhtmj.Text = Convert.ToDouble(pFeaure.get_Value(yhtmjIndex).ToString()).ToString("f");
            }
            if (string.IsNullOrEmpty(pFeaure.get_Value(htmjIndex).ToString()))
            {
                this.TextBoxHtmj.Text = "N/A";
            }
            else
            {
                this.TextBoxHtmj.Text = Convert.ToDouble(pFeaure.get_Value(htmjIndex).ToString()).ToString("f");
            }
            if (string.IsNullOrEmpty(pFeaure.get_Value(scmjIndex).ToString()))
            {
                this.TextBoxScmj.Text = "N/A";
            }
            else
            {
                this.TextBoxScmj.Text = Convert.ToDouble(pFeaure.get_Value(scmjIndex).ToString()).ToString("f");
            }
            this.TextBoxDkdz.Text = pFeaure.get_Value(dkdzIndex).ToString();
            this.TextBoxDknz.Text = pFeaure.get_Value(dknzIndex).ToString();
            this.TextBoxDkxz.Text = pFeaure.get_Value(dkxzIndex).ToString();
            this.TextBoxDkbz.Text = pFeaure.get_Value(dkbzIndex).ToString();
        }
        private void SaveInfo()
        {
            var dkbm = this.TextBoxDkbm.Text.Trim();
            var cbfmc = this.TextBoxCbfmc.Text.Trim();
            var dkmc = this.TextBoxDkmc.Text.Trim();
            double yhtmj,htmj;
            if (this.TextBoxYhtmj.Text == "N/A")
            {
                yhtmj = 0.0;
            }
            else
            {
                yhtmj = Convert.ToDouble(this.TextBoxYhtmj.Text.Trim());
            }
            if (this.TextBoxHtmj.Text == "N/A")
            {
                htmj = 0.0;
            }
            else
            {
                htmj = Convert.ToDouble(this.TextBoxHtmj.Text.Trim());
            }          
            var dkdz = this.TextBoxDkdz.Text.Trim();
            var dknz = this.TextBoxDknz.Text.Trim();
            var dkxz = this.TextBoxDkxz.Text.Trim();
            var dkbz = this.TextBoxDkbz.Text.Trim();
            var sqlString =
    string.Format(
        "update {0} set CBFMC='{1}',DKMC='{2}',YHTMJ={3},HTMJ={4},DKDZ='{5}',DKNZ='{6}',DKXZ='{7}',DKBZ='{8}' where trim(DKBM)={9}",
        _selectFeatrue, cbfmc, dkmc, yhtmj, htmj, dkdz, dknz, dkxz, dkbz, dkbm);
            AccessFactory accessFactory = new AccessFactory(_personDatabase);
            var ret = accessFactory.Execute(sqlString);
            var rets = ret == -1 ? MessageWarning.Show("系统提示", "更新失败！") : MessageInfomation.Show("系统提示", "更新成功！");
        }
        
    }
}
