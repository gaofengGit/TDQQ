using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Microsoft.Windows.Controls;
using TDQQ.AE;
using TDQQ.Common;
using TDQQ.MessageBox;
using TDQQ.Process;

namespace TDQQ.MyWindow
{
    /// <summary>
    /// Interaction logic for WinFieldsInfo.xaml
    /// </summary>
    public partial class WinFieldsInfo : Window
    {
        public WinFieldsInfo()
        {
            InitializeComponent();
        }
        private IMap _pMap;
        private string _personDatabase;
        private string _selectFeature;

        public WinFieldsInfo(IMap pMap, string personDatabase, string selectFeature)
        {
            _pMap = pMap;
            _personDatabase = personDatabase;
            _selectFeature = selectFeature;
            InitializeComponent();
            InitControls();
        }

        private void InitControls()
        {
            this.LabelMove.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => this.DragMove();
            this.ImageClose.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => this.Close();
            //初始化Datagrid控件
            this.Loaded += (object sender, RoutedEventArgs e) =>
            {
                SearchInfo processFactory = new SearchInfo();
                var fieldInfos = processFactory.GetFieldInfo(_personDatabase, _selectFeature);
                if (fieldInfos == null) return;
                this.DataGridFields.ItemsSource = fieldInfos;
            };
            this.ButtonSave.Click += (object sender, RoutedEventArgs e) => SaveEdit();

        }
        //按钮选择
        private void DataGridFields_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var selectItem = this.DataGridFields.SelectedItem;
            var row = selectItem as TDQQ.Process.FieldInfo;
            if (row == null) return;
            FillTextBox(row.Dkbm);
            FreshMap(row.Dkbm);
        }

        private void FillTextBox(string dkbm)
        {
            var sqlString =
                string.Format("select DKMC,CBFMC,DKBM,SCMJ,HTMJ,DKDZ,DKNZ,DKXZ,DKBZ from {0} where trim(DKBM)='{1}'",
                    _selectFeature, dkbm);
            var accessFactory = new AccessFactory(_personDatabase);
            var dt = accessFactory.Query(sqlString);
            if (dt == null || dt.Rows.Count != 1) return;
            //填充信息
            TextBoxFieldName.Text = dt.Rows[0][0].ToString();
            TextBoxCbfmc.Text = dt.Rows[0][1].ToString();
            TextBoxDkbm.Text = dt.Rows[0][2].ToString().Substring(14, 5);
            if (string.IsNullOrEmpty(dt.Rows[0][3].ToString().Trim()))
            {
                TextBoxScmj.Text = "N/A";
            }
            else
            {
                TextBoxScmj.Text = Convert.ToDouble(dt.Rows[0][3].ToString().Trim()).ToString("f");
            }
            if (string.IsNullOrEmpty(dt.Rows[0][4].ToString()))
            {
                TextBoxHtmj.Text = "N/A";
            }
            else
            {
                TextBoxHtmj.Text = Convert.ToDouble(dt.Rows[0][4].ToString()).ToString("f");
            }
            TextBoxDkdz.Text = dt.Rows[0][5].ToString();
            TextBoxDknz.Text = dt.Rows[0][6].ToString();
            TextBoxDkxz.Text = dt.Rows[0][7].ToString();
            TextBoxDkbz.Text = dt.Rows[0][8].ToString();
        }

        private void FreshMap(string dkbm)
        {
            try
            {
                IFeatureLayer pFeatureLayer = _pMap.get_Layer(0) as IFeatureLayer;
                IFeatureSelection pSection = pFeatureLayer as IFeatureSelection;
                IQueryFilter queryFilter = new SpatialFilterClass();
                queryFilter.WhereClause = string.Format("trim(DKBM)={0}", dkbm);
                pSection.SelectFeatures(queryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                IAeFactory aeFactory = new PersonalGeoDatabase(_personDatabase);
                IFeatureClass pFeatureClass = aeFactory.OpenFeatureClasss(_selectFeature);
                IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
                var fieldIndex = pFeatureClass.Fields.FindField("DKBM");
                IFeature pFeature;
                while ((pFeature = pFeatureCursor.NextFeature()) != null)
                {
                    if (pFeature.get_Value(fieldIndex).ToString() == dkbm)
                        break;
                }
                if (pFeature == null) return;
                ITopologicalOperator topo = pFeature.Shape as ITopologicalOperator;
                var topoGeometry = topo.Buffer(400);
                var pEnvelope = topoGeometry.Envelope;
                var pview = _pMap as IActiveView;
                if (pview == null) return;
                pview.Extent = pEnvelope;
                pview.Refresh();
                Marshal.FinalReleaseComObject(pFeatureClass);
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception e)
            {
                //System.Windows.Forms.MessageBox.Show(e.ToString());
            }
           
        }

        private void SaveEdit()
        {
            try
            {
                var row = DataGridFields.SelectedItem as TDQQ.Process.FieldInfo;
                if (row == null) return;
                var dkmc = TextBoxFieldName.Text.Trim();
                var cbfmc = TextBoxCbfmc.Text.Trim();
                double htmj;
                if (TextBoxHtmj.Text.Trim() == "N/A")
                {
                    htmj = 0.0;
                }
                else
                {
                    htmj = Convert.ToDouble(TextBoxHtmj.Text.Trim());
                }
                var dkdz = TextBoxDkdz.Text.Trim();
                var dknz = TextBoxDknz.Text.Trim();
                var dkxz = TextBoxDkxz.Text.Trim();
                var dkbz = TextBoxDkbz.Text.Trim();
                var dkbm = row.Dkbm;
                var sqlString =
                    string.Format(
                        "UPDATE {0} SET DKMC = '{1}',CBFMC ='{2}',HTMJ = {3},DKDZ = '{4}',DKNZ = '{5}',DKXZ = '{6}' " +
                        ",DKBZ='{7}'" +
                        " WHERE trim(DKBM)= {8}", _selectFeature, dkmc, cbfmc, htmj, dkdz, dknz, dkxz, dkbz, dkbm);
                var accessFactory = new AccessFactory(_personDatabase);
                var ret = accessFactory.Execute(sqlString);
                this.Topmost = false;
                if (ret != -1)
                {
                    MessageInfomation.Show("系统提示", "更新成功！");
                    return;
                }
                MessageWarning.Show("系统提示", "更新失败！");
            }
            catch (Exception e)
            {
               // System.Windows.Forms.MessageBox.Show(e.ToString());
            }
           
        }
    }
}
