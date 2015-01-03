using System;
using System.Windows;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using TDQQ.Common;
using TDQQ.Export;
using TDQQ.Import;
using TDQQ.MessageBox;
using TDQQ.MyWindow;
using TDQQ.Process;

namespace TDQQ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 初始化工作
        private string _personDatabase;
        private string _selectFeaure;
        private string _basicDatabase;
        /// <summary>
        /// 地图控件
        /// </summary>
        AxMapControl _mainMapControl = null;
        private void CreateEngineControls()
        {
            _mainMapControl = new AxMapControl();
            MainFormsHost.Child = _mainMapControl;
        }
        private void SetMapProperties()
        {
            _mainMapControl.Dock = DockStyle.None;
            _mainMapControl.BackColor = System.Drawing.Color.AliceBlue;
        }
        #endregion
        private bool _isPointEdit = false;

        private bool _isRectEdit = false;

        private WinPointEdit winPointEdit = null;
        public MainWindow()
        {
            InitializeComponent();
            _basicDatabase = _personDatabase = _selectFeaure = string.Empty;
            CreateEngineControls();
            SetMapProperties();
            StateCheck();
            InitTabs();
        }

       
        /// <summary>
        /// 根据字段值，设置按钮是否可用
        /// </summary>
        private void StateCheck()
        {
            if (string.IsNullOrEmpty(_personDatabase) || string.IsNullOrEmpty(_selectFeaure) ||
                string.IsNullOrEmpty(_basicDatabase))
            {
                this.ButtonOpenMap.IsEnabled = true;
                //this.ButtonOpenBasic.IsEnabled = true;
                this.ButtonCloseMap.IsEnabled = false;
                this.ButtonAddField.IsEnabled = false;
                this.ButtonDefault.IsEnabled = false;
                this.ButtonUpdateCbfbm.IsEnabled = false;
                this.ButtonBoundary.IsEnabled = false;
                this.ButtonJzd.IsEnabled = false;
                this.ButtonJzx.IsEnabled = false;
                this.ButtonUpdateCbfmc.IsEnabled = false;
                this.ButtonChangeCbfmc.IsEnabled = false;
                this.ButtonField.IsEnabled = false;
                this.ButtonFarmer.IsEnabled = false;
                this.ButtonArea.IsEnabled = false;
                this.ButtonStartPoint.IsEnabled = false;
                this.ButtonStopPoint.IsEnabled = false;
                this.ButtonStartBox.IsEnabled = false;
                this.ButtonStopBox.IsEnabled = false;
                this.ButtonATable.IsEnabled = false;
                this.ButtonBTable.IsEnabled = false;
                this.ButtonCTable.IsEnabled = false;
                this.ButtonDTable.IsEnabled = false;
                this.ButtonETable.IsEnabled = false;
                this.ButtonFamily.IsEnabled = false;
                this.ButtonOpenTable.IsEnabled = false;
                this.ButtonSignTable.IsEnabled = false;
                this.ButtonCertification.IsEnabled = false;
                this.ButtonContract.IsEnabled = false;
                this.ButtonList.IsEnabled = false;
                this.ButtonRegister.IsEnabled = false;
                this.ButtonImportBasic.IsEnabled = false;
                //this.ButtonCbfmc.IsEnabled = false;
                //this.ButtonCbf.IsEnabled = false;
                this.ButtonImportFbf.IsEnabled = false;
                this.ButtonSetHtmj.IsEnabled = false;
                this.ButtonDkbm.IsEnabled = false;
            //    this.ButtonCounty.IsEnabled = false;
                this.ButtonDepartment.IsEnabled = false;
            }
            else
            {
                this.ButtonOpenMap.IsEnabled = true;
                //this.ButtonOpenBasic.IsEnabled = true;
                this.ButtonCloseMap.IsEnabled = true;
                this.ButtonAddField.IsEnabled = true;
                this.ButtonDefault.IsEnabled = true;
                this.ButtonUpdateCbfbm.IsEnabled = true;
                this.ButtonBoundary.IsEnabled = true;
                this.ButtonJzd.IsEnabled = true;
                this.ButtonJzx.IsEnabled = true;
                this.ButtonUpdateCbfmc.IsEnabled = true;
                this.ButtonChangeCbfmc.IsEnabled = true;
                this.ButtonField.IsEnabled = true;
                this.ButtonFarmer.IsEnabled = true;
                this.ButtonArea.IsEnabled = true;
                this.ButtonStartPoint.IsEnabled = true;
                this.ButtonStopPoint.IsEnabled = true;
                this.ButtonStartBox.IsEnabled = true;
                this.ButtonStopBox.IsEnabled = true;
                this.ButtonATable.IsEnabled = true;
                this.ButtonBTable.IsEnabled = true;
                this.ButtonCTable.IsEnabled = true;
                this.ButtonDTable.IsEnabled = true;
                this.ButtonETable.IsEnabled = true;
                this.ButtonFamily.IsEnabled = true;
                this.ButtonOpenTable.IsEnabled = true;
                this.ButtonSignTable.IsEnabled = true;
                this.ButtonCertification.IsEnabled = true;
                this.ButtonContract.IsEnabled = true;
                this.ButtonList.IsEnabled = true;
                this.ButtonRegister.IsEnabled = true;
                this.ButtonImportBasic.IsEnabled = true;
                //this.ButtonCbfmc.IsEnabled = true;
                //this.ButtonCbf.IsEnabled = true;
                this.ButtonImportFbf.IsEnabled = true;
                this.ButtonSetHtmj.IsEnabled = true;
                this.ButtonDkbm.IsEnabled = true;
            //    this.ButtonCounty.IsEnabled = true;
                this.ButtonDepartment.IsEnabled = true;
            }
        }


        private void InitTabs()
        {
            InitDataTab();
            InitEditTab();
            InitInfoTab();
            InitEditMap();
            InitMap();
            InitExportMap();
            InitImport();
            InitHelp();
        }

        private void InitDataTab()
        {
            LoadData loadData=new LoadData();
            this.ButtonOpenMap.Click += (object sender, RoutedEventArgs e) =>
            {
                
                loadData.LoadMap(ref _personDatabase, ref _selectFeaure,ref _basicDatabase, _mainMapControl.Map);
                StateCheck();
            };
            //this.ButtonOpenBasic.Click += (object sender, RoutedEventArgs e) =>
            //{
                
            //    loadData.OpenBasicDatabase(ref _basicDatabase);
            //    StateCheck();
            //};
            this.ButtonCloseMap.Click +=
                (object sender, RoutedEventArgs e) =>
                {
                   
                    loadData.CloseMap(ref _personDatabase, ref _selectFeaure, ref _basicDatabase, _mainMapControl.Map);
                    StateCheck();
                };
        }

        private void InitEditTab()
        {
            EditData editData = new EditData();
            this.ButtonAddField.Click += (object sender, RoutedEventArgs e) =>
            {
                if (MessageBox.MessageQuestion.Show("系统提示", "是否编辑字段？") == true)
                {
                    string error = "字段编辑成功";
                    var ret = editData.EditFields(_personDatabase, _selectFeaure, ref error);
                    if (ret)
                        MessageBox.MessageInfomation.Show("系统提示", error);
                    else
                        MessageBox.MessageWarning.Show("系统提示", error);
                }         
               
            };
            this.ButtonDefault.Click += (object sender, RoutedEventArgs e) =>
            {
                var ret = editData.SetDefaultValue(_personDatabase, _selectFeaure);
                if (ret)
                    MessageBox.MessageInfomation.Show("系统提示", "字段设值成功");
                else
                    MessageBox.MessageWarning.Show("系统提示", "字段设值失败");
            };
            this.ButtonBoundary.Click += (object sender, RoutedEventArgs e) =>
            {
                //string error = "四至提取成功";
                var ret = editData.GetSurround(_personDatabase, _selectFeaure);
                if (ret)
                    MessageBox.MessageInfomation.Show("系统提示", "四至提取成功");
                else
                    MessageBox.MessageWarning.Show("系统提示", "四至提取失败");
            };
            this.ButtonJzd.Click += (object sender, RoutedEventArgs e) =>
            {
                var ret = Tools4Jz.CreateJZDAsynF(_personDatabase, _selectFeaure, _selectFeaure + "_JZD");
                //ret = Tools4Jz.CreateJZDAsynF(_personDatabase, _selectFeature, _selectFeature + "_JZD");
                var msg = ret ? "生成界址点成功！" : "生成界址点失败！";
                MessageInfomation.Show("系统提示", msg);
            };
            this.ButtonJzx.Click += (object sender, RoutedEventArgs e) =>
            {
                var ret = Tools4Jz.CreateJZXAsynF(_personDatabase, _selectFeaure, _selectFeaure + "_JZX");
                var msg = ret ? "生成界址线成功！" : "生成界址线失败！";
                MessageInfomation.Show("系统提示", msg);
            };
            this.ButtonUpdateCbfbm.Click += (object sender, RoutedEventArgs e) =>
            {
                var ret = editData.UnpdateCbfbm(_personDatabase, _selectFeaure,_basicDatabase);
                if (ret)
                    MessageBox.MessageInfomation.Show("系统提示", "承包方编码提取成功");
                else
                    MessageBox.MessageWarning.Show("系统提示", "承包方编码提取失败");
            };
            this.ButtonSetHtmj.Click += (object sender, RoutedEventArgs e) =>
            {
                var ret = editData.SetHtmj(_personDatabase, _selectFeaure);
                if (ret)
                    MessageBox.MessageInfomation.Show("系统提示", "合同面积设置成功");
                else
                    MessageBox.MessageWarning.Show("系统提示", "合同面积设置失败");
            };
            this.ButtonUpdateCbfmc.Click += (object sender, RoutedEventArgs e) =>
            {
                var ret = editData.ReplaceCbfmc(_personDatabase, _selectFeaure);
                if (ret)
                {
                    MessageBox.MessageInfomation.Show("系统提示", "承包方名称替换成功");
                    System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\Log\logfile.txt");
                }                   
                else
                    MessageBox.MessageWarning.Show("系统提示", "承包方名称替换失败");
                
            };
            this.ButtonChangeCbfmc.Click += (object sender, RoutedEventArgs e) =>
            {
                var ret = editData.ChangeNotCbfmc(_personDatabase, _selectFeaure,_basicDatabase);
                if (ret)
                {
                    MessageBox.MessageInfomation.Show("系统提示", "非承包方修改成功");
                    System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\Log\logfile.txt");
                }
                    
                else
                    MessageBox.MessageWarning.Show("系统提示", "非承包方修改失败");
                
            };
            this.ButtonDkbm.Click += (object sender, RoutedEventArgs e) =>
            {
                var ret = editData.SortDkbm(_personDatabase, _selectFeaure);
                if (ret)
                {
                    MessageBox.MessageInfomation.Show("系统提示", "地块编码成功");
                }

                else
                    MessageBox.MessageWarning.Show("系统提示", "地块编码失败");
            };
        }

        private void InitInfoTab()
        {
            this.ButtonField.Click += (object sender, RoutedEventArgs e) =>
            {

                WinFieldsInfo winFieldsInfo = new WinFieldsInfo(_mainMapControl.Map, _personDatabase, _selectFeaure);
                winFieldsInfo.Owner = this;
                winFieldsInfo.Show();
               // winFieldsInfo.Topmost = true;
            };
            this.ButtonFarmer.Click += (object sender, RoutedEventArgs e) =>
            {
                WinFarmerInfo winFarmerInfo = new WinFarmerInfo(_personDatabase, _selectFeaure, _mainMapControl.Map);
                winFarmerInfo.Owner = this;
                winFarmerInfo.Show();
               // winFarmerInfo.Topmost = true;
            };
            this.ButtonArea.Click += (object sender, RoutedEventArgs e) =>
            {
                SearchInfo searchInfo=new SearchInfo();
                searchInfo.AreaInfo(_personDatabase, _selectFeaure);
            };
        }

        private void InitEditMap()
        {
            this.ButtonStartPoint.Click += (object sender, RoutedEventArgs e) =>
            {

                _isPointEdit = true;
                _isRectEdit = false;
            };
            this.ButtonStopPoint.Click += (object sender, RoutedEventArgs e) =>
            {
                _isPointEdit = false;
                _isRectEdit = false;
                if (winPointEdit != null) winPointEdit.Close();
                this._mainMapControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
            };
            this.ButtonStartBox.Click += (object sender, RoutedEventArgs e) =>
            {
                _isRectEdit = true;
                _isPointEdit = false;
            };
            this.ButtonStopBox.Click += (object sender, RoutedEventArgs e) =>
            {
                _isRectEdit = false;
                _isPointEdit = false;
                this._mainMapControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
            };
        }

        private void InitMap()
        {
            EditMap editMap=new EditMap();
            this._mainMapControl.OnMouseDown += (object sender, IMapControlEvents2_OnMouseDownEvent e) =>
            {

                if (e.button==4)
                {
                    _mainMapControl.MousePointer = esriControlsMousePointer.esriPointerHand;
                    this._mainMapControl.Pan();
                }


                this._mainMapControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
                if (_isPointEdit)
                {
                   
                    this._mainMapControl.MousePointer = esriControlsMousePointer.esriPointerHand;
                    IPoint pPoint = new PointClass();
                    pPoint.PutCoords(e.mapX, e.mapY);
                    var pFeatrue = editMap.GetPointFeature(_mainMapControl.Map, pPoint);
                    if (pFeatrue == null) return;
                    if (winPointEdit == null || CommonHelper.IsDisposed(winPointEdit))
                    {
                        winPointEdit = new WinPointEdit(_personDatabase, _selectFeaure);
                        winPointEdit.Owner = this;
                        winPointEdit.Show();
                        //winPointEdit.Topmost = true;
                    }
                    winPointEdit.ShowInfo(pFeatrue);
                }
                if (_isRectEdit)
                {
                    _mainMapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                    IEnvelope pEnv = _mainMapControl.TrackRectangle();
                    var ret = editMap.ShowRectSelection(_mainMapControl.Map, pEnv);
                    if (!ret) return;
                    WinMultiFieldInfo winMultiFieldInfo = new WinMultiFieldInfo();
                    if (winMultiFieldInfo.ShowDialog() == true)
                    {
                        //MessageWarning.Show("123", "123");
                        var updateInfo = winMultiFieldInfo.InfoList;
                        ret = editMap.UpdateSelectFields(updateInfo, _mainMapControl.Map, _personDatabase, _selectFeaure);
                        if (ret)
                        {
                            MessageInfomation.Show("系统提示", "更新成功！");
                        }
                        else
                        {
                            MessageWarning.Show("系统提示", "更新失败！");
                        }                   
                    }
                }
            };
            
        }

        private void InitExportMap()
        {
            this.ButtonATable.Click += (object sender, RoutedEventArgs e) =>
            {

                var export = new ExportA(_personDatabase, _selectFeaure, _basicDatabase);
                var ret = export.Export();
                if (ret)
                    MessageInfomation.Show("系统提示", "发包方调查表导出成功！");
                else
                    MessageWarning.Show("系统提示", "发包方调查表导出失败！");
            };
            this.ButtonBTable.Click += (object sender, RoutedEventArgs e) =>
            {
                
                var export = new ExportB(_personDatabase, _selectFeaure, _basicDatabase);
                var ret = export.Export();
                if (ret)
                    MessageInfomation.Show("系统提示", "承包方调查表导出成功！");
                else
                    MessageWarning.Show("系统提示", "承包方调查表导出失败！");
            };
            this.ButtonCTable.Click += (object sender, RoutedEventArgs e) =>
            {
                
                var export = new ExportC(_personDatabase, _selectFeaure, _basicDatabase);
                var ret = export.Export();
                if (ret)
                    MessageInfomation.Show("系统提示", "地块调查表导出成功！");
                else
                    MessageWarning.Show("系统提示", "地块调查表导出失败！");
            };
            this.ButtonDTable.Click += (object sender, RoutedEventArgs e) =>
            {
               
                var export = new ExportD(_personDatabase, _selectFeaure, _basicDatabase);
                var ret = export.Export();
                if (ret)
                    MessageInfomation.Show("系统提示", "公示表导出成功！");
                else
                    MessageWarning.Show("系统提示", "公示表导出失败！");
            };
            this.ButtonETable.Click += (object sender, RoutedEventArgs e) =>
            {
                
                var export = new ExportE(_personDatabase, _selectFeaure, _basicDatabase);
                var ret = export.Export();
                if (ret)
                    MessageInfomation.Show("系统提示", "归户表导出成功！");
                else
                    MessageWarning.Show("系统提示", "归户表导出失败！");
            };
            this.ButtonOpenTable.Click += (object sender, RoutedEventArgs e) =>
            {
                
                var export = new ExportOpen(_personDatabase, _selectFeaure, _basicDatabase);
                var ret = export.Export();
                if (ret)
                    MessageInfomation.Show("系统提示", "公示表导出成功！");
                else
                    MessageWarning.Show("系统提示", "公示表导出失败！");
            };
            this.ButtonSignTable.Click += (object sender, RoutedEventArgs e) =>
            {
                
                var export = new ExportSign(_personDatabase, _selectFeaure, _basicDatabase);
                var ret = export.Export();
                if (ret)
                    MessageInfomation.Show("系统提示", "签字表导出成功！");
                else
                    MessageWarning.Show("系统提示", "签字表导出失败！");
            };
            this.ButtonContract.Click += (object sender, RoutedEventArgs e) =>
            {
                
                var exportContract = new ExportContract(_personDatabase, _selectFeaure, _basicDatabase);
                var ret = exportContract.Export();
                if (ret)
                    MessageInfomation.Show("系统提示", "合同书导出成功！");
                else
                    MessageWarning.Show("系统提示", "合同书导出失败！");
            };
            this.ButtonCertification.Click += (object sender, RoutedEventArgs e) =>
            {
                
                var exportCertification = new ExportCertification(_personDatabase, _selectFeaure, _basicDatabase);
                var ret = exportCertification.Export();
                if (ret)
                    MessageInfomation.Show("系统提示", "经营权证导出成功！");
                else
                    MessageWarning.Show("系统提示", "经营权证导出失败！");
            };
            this.ButtonFamily.Click += (object sender, RoutedEventArgs e) =>
            {
                
                var exportFamily = new ExportFamily(_personDatabase, _selectFeaure, _basicDatabase);
                var ret = exportFamily.Export();
                if (ret)
                    MessageInfomation.Show("系统提示", "家庭信息导出成功！");
                else
                    MessageWarning.Show("系统提示", "家庭信息导出失败！");
            };
            this.ButtonList.Click += (object sender, RoutedEventArgs e) =>
            {
                
                var exportList = new ExportList(_personDatabase, _selectFeaure, _basicDatabase);
                var ret = exportList.Export();
                if (ret)
                    MessageInfomation.Show("系统提示", "颁证清单导出成功！");
                else
                    MessageWarning.Show("系统提示", "颁证清单导出失败！");
            };
            this.ButtonRegister.Click += (object sender, RoutedEventArgs e) =>
            {
                
                var exportRegister = new ExportRegister(_personDatabase, _selectFeaure, _basicDatabase);
                var ret = exportRegister.Export();
                if (ret)
                    MessageInfomation.Show("系统提示", "登记薄导出成功！");
                else
                    MessageWarning.Show("系统提示", "登记薄导出失败！");
            };
            this.ButtonDepartment.Click += (object sender, RoutedEventArgs e) =>
            {
                var exportPost = new ExportPost(_personDatabase, _selectFeaure, _basicDatabase);
                var ret = exportPost.Export();
                if (ret)
                    MessageInfomation.Show("系统提示", "公示公告导出成功！");
                else
                    MessageWarning.Show("系统提示", "公示公告导出失败！");
            };

        }

        private void InitImport()
        {
            this.ButtonCbfbm.Click += (object sender, RoutedEventArgs e) =>
            {
                WinCbfbm winCbfbm=new WinCbfbm();
                winCbfbm.ShowDialog();
            };
            this.ButtonImportBasic.Click += (object sender, RoutedEventArgs e) =>
            {
                ImportCbfjtcy importCbfjtcy=new ImportCbfjtcy(_basicDatabase);
                var ret = importCbfjtcy.Import();
                if (ret)
                {
                    MessageBox.MessageInfomation.Show("系统提示", "基础信息信息导入成功");
                }
                else
                {
                    MessageBox.MessageWarning.Show("系统提示", "基础信息信息导入失败");
                    System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\Log\logfile.txt");
                }
            };
            /*
            this.ButtonCbfmc.Click += (object sender, RoutedEventArgs e) =>
            {
                var import = new BasicProcess(_basicDatabase);
                var ret = import.SetCbfmc();
                if (ret)
                    MessageInfomation.Show("系统提示", "承包方名称提取成功！");
                else
                    MessageWarning.Show("系统提示", "承包方名称提取失败！");
            };
            this.ButtonCbf.Click += (object sender, RoutedEventArgs e) =>
            {
                var import = new BasicProcess(_basicDatabase);
                var ret = import.CreateCbf();
                if (ret)
                    MessageInfomation.Show("系统提示", "承包方表提取成功！");
                else
                    MessageWarning.Show("系统提示", "承包方表提取失败！");
            };
             */
            this.ButtonImportFbf.Click += (object sender, RoutedEventArgs e) =>
            {
                var import = new ImportFbf(_basicDatabase);
                var ret = import.Import();
                if (ret)
                    MessageInfomation.Show("系统提示", "发包方设置成功！");
                else
                    MessageWarning.Show("系统提示", "发包方设置失败！");
            };
        }

        private void InitHelp()
        {
            this.ButtonHelp.Click += (object sender, RoutedEventArgs e) => System.Diagnostics.
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\TDQQ.chm");
          
        }

    }
}
