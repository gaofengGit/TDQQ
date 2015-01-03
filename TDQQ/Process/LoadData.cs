using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using TDQQ.AE;
using TDQQ.Common;
using TDQQ.MessageBox;
using TDQQ.MyWindow;

namespace TDQQ.Process
{
    /// <summary>
    /// 加载数据
    /// </summary>
    class LoadData
    {
        #region 加载地图
        public void LoadMap(ref string personDatabase, ref string selectFeaure,ref string basicDatabase,IMap pMap)
        {
            var tryNewPersonDatabase = OpenPersonDatabase();
            // personDatabase=string.IsNullOrEmpty(tryNewPersonDatabase)?personDatabase:tryNewPersonDatabase;
            if (string.IsNullOrEmpty(tryNewPersonDatabase)) return;
            personDatabase = tryNewPersonDatabase;
            var trynewSelectFeture = SelectFeature(personDatabase);
            if (string.IsNullOrEmpty(trynewSelectFeture)) return;
            selectFeaure = trynewSelectFeture;
            if (!TDQQ.Check.ValidCheck.PersonDatabaseNullField(personDatabase, selectFeaure, "SHAPE_Length"))
            {
                MessageWarning.Show("系统提示", "存在空地块");
                personDatabase = selectFeaure = basicDatabase = string.Empty;
                return;
            }
            //询问是否拓扑检查
            IAeFactory pAeFactory = new PersonalGeoDatabase(personDatabase);
            IFeatureClass pFeatureClass = pAeFactory.OpenFeatureClasss(selectFeaure);
            if (MessageBox.MessageQuestion.Show("系统提示", "是否要进行拓扑检查？") == true)
            {

                int total = GetFeatureCount(personDatabase, selectFeaure);
                if (!TopoCheck(pFeatureClass, total))
                {
                    pAeFactory.ReleaseFeautureClass(pFeatureClass);
                    MessageBox.MessageWarning.Show("系统提示", "拓扑检查失败");
                    return;
                }
            }
            DeleteMapLayer(pMap);
            if (!AddFeaureClassToMap(pFeatureClass, pMap))
            {
                MessageBox.MessageWarning.Show("系统提示", "加载地图失败");
                return;
            }
            basicDatabase = CopyBasicDatabase(personDatabase);

        }
        private string OpenPersonDatabase()
        {
            var dialogFactory = new DialogFactory("个人地理数据库(MDB)|*.mdb");
            return dialogFactory.OpenFile("请选择个人地理数据库");
        }

        private string SelectFeature(string personDatabase)
        {
            var winSelectFeature = new WinSelectFeature(personDatabase);
            return winSelectFeature.ShowDialog() == true ?
                winSelectFeature.SelectFeature : string.Empty;
        }
        private void DeleteMapLayer(IMap pMap)
        {
            while (pMap.LayerCount != 0)
            {
                var pLayer = pMap.get_Layer(0);
                pMap.DeleteLayer(pLayer);
            }
        }

        private string CopyBasicDatabase(string personDatabase)
        {
            //"C:\马营许庄张清良.mdb"
            int floderIndex = personDatabase.LastIndexOf('\\');
            int nameIndex = personDatabase.LastIndexOf('.');
            string floderPath = personDatabase.Substring(0, floderIndex + 1);
            string fileName = personDatabase.Substring(floderIndex + 1, nameIndex-floderIndex-1);
            string templateBasicDatabase = AppDomain.CurrentDomain.BaseDirectory + @"\template\基础数据模板.mdb";
            string currentBasicDatabasePath = floderPath + fileName + "_基础数据库.mdb";
            if (!File.Exists(currentBasicDatabasePath))
            {
                File.Copy(templateBasicDatabase, currentBasicDatabasePath);
            }
            return currentBasicDatabasePath;
        }
        private bool AddFeaureClassToMap(IFeatureClass pFeatureClass, IMap pMap)
        {
            try
            {
                var featureLayer = new FeatureLayerClass();
                featureLayer.FeatureClass = pFeatureClass;
                pMap.AddLayer(featureLayer);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// 要素类的拓扑检查
        /// </summary>
        /// <param name="pFeatureClass"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        private bool TopoCheck(IFeatureClass pFeatureClass, int total)
        {
            var para = new Hashtable();
            var w = new Wait();
            w.SetInfoInvoke("正在检查拓扑关系......");
            w.SetProgressInfo(string.Empty);
            para["wait"] = w;
            para["total"] = total;
            para["ifeatureclass"] = pFeatureClass;
            para["result"] = false;
            Thread t = new Thread(new ParameterizedThreadStart(TopoCheck));
            t.Start(para);
            w.ShowDialog();
            var ret = (bool)para["result"];
            t.Abort();
            return ret;
        }
        private void TopoCheck(object p)
        {
            Hashtable para = p as Hashtable;
            var total = (int)para["total"];
            var w = para["wait"] as Wait;
            IFeatureClass pFeatureClass = para["ifeatureclass"] as IFeatureClass;
            try
            {
                IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
                IFeature pFeature = pFeatureCursor.NextFeature();
                IGeometry topoGeometry;
                int currentIndex = 0;
                while (pFeature != null)
                {
                    w.SetProgressInfo(((double)currentIndex++ / (double)total).ToString("p"));
                    topoGeometry = pFeature.Shape;
                    ISpatialFilter pSpatialFilter = new SpatialFilterClass();
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelOverlaps;
                    pSpatialFilter.Geometry = topoGeometry;
                    IFeatureCursor mFeatureCursor = pFeatureClass.Search(pSpatialFilter, false);
                    IFeature feature = mFeatureCursor.NextFeature();
                    //第一个对象找到一个相交的
                    if (feature != null)
                    {

                        para["result"] = false;
                        w.CloseWait();
                        return;
                    }
                    //释放内存空间          
                    Marshal.ReleaseComObject(mFeatureCursor);
                    pFeature = pFeatureCursor.NextFeature();
                }
                Marshal.ReleaseComObject(pFeatureCursor);
                para["result"] = true;
                w.CloseWait();
                return;
            }
            catch (Exception)
            {
                para["result"] = false;
                w.CloseWait();
                return;
            }
        }


        private int GetFeatureCount(string personDatabase, string selectFeature)
        {
            var pAccess = new AccessFactory(personDatabase);
            var commandText = string.Format("Select Count(*) from {0}", selectFeature);
            return (int)pAccess.ExecuteScalar(commandText);
        }
        #endregion

        #region 加载基础数据库

        public void OpenBasicDatabase(ref string baiscDatabase)
        {
            DialogFactory dialogFactory = new DialogFactory("基础数据库(MDB)|*.mdb");
            var tryOpenBasicDatabase = dialogFactory.OpenFile("请选择基础数据库");
            if (string.IsNullOrEmpty(tryOpenBasicDatabase)) return;
            baiscDatabase = tryOpenBasicDatabase;    
        }
        #endregion

        #region 关闭所有地图

        public void CloseMap(ref string personDatabase, ref string selectedFeature, ref string basicDatabase,IMap pMap)
        {
            personDatabase = selectedFeature = basicDatabase = string.Empty;
            DeleteMapLayer(pMap);
        }
        #endregion
    }
}
