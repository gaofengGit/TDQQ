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
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using Microsoft.Windows.Controls;
using TDQQ.Process;

namespace TDQQ.MyWindow
{
    /// <summary>
    /// Interaction logic for WinFarmerInfo.xaml
    /// </summary>
    public partial class WinFarmerInfo : Window
    {
        public WinFarmerInfo()
        {
            InitializeComponent();
        }

        private string _personDatabase;
        private string _selectFeature;
        private IMap _pMap;

        public WinFarmerInfo(string personDatabase, string selectFeature, IMap pMap)
        {
            _personDatabase = personDatabase;
            _selectFeature = selectFeature;
            _pMap = pMap;
            InitializeComponent();
            InitControls();
        }

        private void InitControls()
        {
            this.LabelMove.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => this.DragMove();
            this.ImageClose.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => this.Close();
            this.Loaded += (object sender, RoutedEventArgs e) =>
            {
                var searchInfo = new SearchInfo();
                var farmInfo = searchInfo.GetFarmerInfo(_personDatabase, _selectFeature);
                if (farmInfo == null) return;
                this.DataGridFarmerInfo.ItemsSource = farmInfo;
            };
        }

        private void DataGridFarmerInfo_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var row = this.DataGridFarmerInfo.SelectedItem as FarmerInfo;
            if (row == null) return;
            FreshMap(row.Cbfbm);
        }

        private void FreshMap(string cbfbm)
        {
            try
            {
                var pFeatureLayer = _pMap.get_Layer(0) as IFeatureLayer;
                var pSection = pFeatureLayer as IFeatureSelection;
                IQueryFilter queryFilter = new SpatialFilterClass();
                queryFilter.WhereClause = string.Format("trim(CBFBM)={0}", cbfbm);
                pSection.SelectFeatures(queryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                IActiveView pview = _pMap as IActiveView;
                pview.Refresh();
            }
            catch (Exception e)
            {
                //System.Windows.Forms.MessageBox.Show(e.ToString());
            }
          
        }
    }
}
