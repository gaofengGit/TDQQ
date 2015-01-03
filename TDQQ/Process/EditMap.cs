using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using TDQQ.AE;

namespace TDQQ.Process
{
    /// <summary>
    /// 地块点选合框
    /// </summary>
    class EditMap
    {
        public IFeature GetPointFeature(IMap pMap, IPoint pPoint)
        {
            IFeatureLayer pFeatureLayer = pMap.get_Layer(0) as IFeatureLayer;
            IFeatureSelection pSection = pFeatureLayer as IFeatureSelection;
            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.Geometry = pPoint;
            pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelWithin;
            pSection.SelectFeatures(pSpatialFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
            IActiveView pActiveView = pMap as IActiveView;
            pActiveView.Refresh();
            //返回选择的Feature；
            ISelection selection = pMap.FeatureSelection;
            IEnumFeature pEnumFeature = (IEnumFeature)selection;
            return pEnumFeature.Next();
        }
        public bool ShowRectSelection(IMap pMap, IEnvelope pEnvelope)
        {
            IFeatureLayer pFeatureLayer = pMap.get_Layer(0) as IFeatureLayer;
            IFeatureSelection pSection = pFeatureLayer as IFeatureSelection;
            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.Geometry = pEnvelope;
            pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            pSection.SelectFeatures(pSpatialFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
            IActiveView pActiveView = pMap as IActiveView;
            pActiveView.Refresh();
            if (pMap.SelectionCount == 0)
                return false;
            return true;
        }

        public bool UpdateSelectFields(List<string> updateValue, IMap pMap, string personDatabase, string selectedFeature)
        {
            try
            {
                ISelection selection = pMap.FeatureSelection;
                IEnumFeature pEnumFeature = (IEnumFeature)selection;
                IFeature feature = pEnumFeature.Next();
                //var pFeatureLayer = pMap.get_Layer(0) as IFeatureLayer;
                //var pFeatrueClass = pFeatureLayer as IFeatureClass;
                IAeFactory aefactory = new PersonalGeoDatabase(personDatabase);
                var pFeatrueClass = aefactory.OpenFeatureClasss(selectedFeature);
                IDataset dataset = (IDataset)pFeatrueClass;
                IWorkspace myworkspace = dataset.Workspace;
                IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)myworkspace;
                var fieldIndex = GetFieldIndex(pFeatrueClass);
                workspaceEdit.StartEditing(true);
                workspaceEdit.StartEditOperation();
                while (feature != null)
                {
                    int index;
                    //大地块名称
                    if (!string.IsNullOrEmpty(updateValue[0]))
                    {
                        fieldIndex.TryGetValue("DKMC", out index);
                        feature.set_Value(index, updateValue[0]);
                    }
                    //指界人姓名
                    if (!string.IsNullOrEmpty(updateValue[1]))
                    {
                        fieldIndex.TryGetValue("ZJRXM", out index);
                        feature.set_Value(index, updateValue[1]);
                    }
                    //地块备注信息
                    if (!string.IsNullOrEmpty(updateValue[2]))
                    {
                        fieldIndex.TryGetValue("DKBZXX", out index);
                        feature.set_Value(index, updateValue[2]);
                    }
                    //地块东至
                    if (!string.IsNullOrEmpty(updateValue[3]))
                    {
                        fieldIndex.TryGetValue("DKDZ", out index);
                        feature.set_Value(index, updateValue[3]);
                    }
                    //地块南至
                    if (!string.IsNullOrEmpty(updateValue[4]))
                    {
                        fieldIndex.TryGetValue("DKNZ", out index);
                        feature.set_Value(index, updateValue[4]);
                    }
                    //地块西至
                    if (!string.IsNullOrEmpty(updateValue[5]))
                    {
                        fieldIndex.TryGetValue("DKXZ", out index);
                        feature.set_Value(index, updateValue[5]);
                    }
                    //地块北至
                    if (!string.IsNullOrEmpty(updateValue[6]))
                    {
                        fieldIndex.TryGetValue("DKBZ", out index);
                        feature.set_Value(index, updateValue[6]);
                    }
                    //承包经营权取得方式
                    if (!string.IsNullOrEmpty(updateValue[7]))
                    {
                        fieldIndex.TryGetValue("CBJYQQDFS", out index);
                        feature.set_Value(index, updateValue[7]);
                    }
                    //土地利用类型
                    if (!string.IsNullOrEmpty(updateValue[8]))
                    {
                        fieldIndex.TryGetValue("TDLYLX", out index);
                        feature.set_Value(index, updateValue[8]);
                    }
                    //是否基本农田
                    if (!string.IsNullOrEmpty(updateValue[9]))
                    {
                        fieldIndex.TryGetValue("SFJBNT", out index);
                        feature.set_Value(index, updateValue[9]);
                    }
                    //地块类别
                    if (!string.IsNullOrEmpty(updateValue[10]))
                    {
                        fieldIndex.TryGetValue("DKLB", out index);
                        feature.set_Value(index, updateValue[10]);
                    }
                    //地力等级
                    if (!string.IsNullOrEmpty(updateValue[11]))
                    {
                        fieldIndex.TryGetValue("DLDJ", out index);
                        feature.set_Value(index, updateValue[11]);
                    }
                    //所有权性质
                    if (!string.IsNullOrEmpty(updateValue[12]))
                    {
                        fieldIndex.TryGetValue("SYQXZ", out index);
                        feature.set_Value(index, updateValue[12]);
                    }
                    //土地用途
                    if (!string.IsNullOrEmpty(updateValue[13]))
                    {
                        fieldIndex.TryGetValue("TDYT", out index);
                        feature.set_Value(index, updateValue[13]);
                    }
                    feature.Store();
                    feature = pEnumFeature.Next();
                }
                workspaceEdit.StopEditOperation();
                workspaceEdit.StopEditing(true);
                return true;
            }
            catch (Exception ex)
            {
                
                return false;
            }

        }
        private Dictionary<string, int> GetFieldIndex(IFeatureClass pFeatureClass)
        {
            Dictionary<string, int> fieldsIndex = new Dictionary<string, int>();
            fieldsIndex.Add("CBFMC", pFeatureClass.Fields.FindField("CBFMC"));
            fieldsIndex.Add("YHTMJ", pFeatureClass.Fields.FindField("YHTMJ"));
            fieldsIndex.Add("DKMC", pFeatureClass.Fields.FindField("DKMC"));
            fieldsIndex.Add("YSDM", pFeatureClass.Fields.FindField("YSDM"));
            fieldsIndex.Add("DKBM", pFeatureClass.Fields.FindField("DKBM"));
            fieldsIndex.Add("DKBZXX", pFeatureClass.Fields.FindField("ZJRXM"));
            //fieldsIndex.Add("DKMC", pFeatureClass.Fields.FindField("DKMC"));
            fieldsIndex.Add("FBFBM", pFeatureClass.Fields.FindField("FBFBM"));
            //fieldsIndex.Add("FBFBM", pFeatureClass.Fields.FindField("FBFBM"));
            fieldsIndex.Add("SYQXZ", pFeatureClass.Fields.FindField("SYQXZ"));
            fieldsIndex.Add("DKLB", pFeatureClass.Fields.FindField("DKLB"));
            fieldsIndex.Add("DLDJ", pFeatureClass.Fields.FindField("DLDJ"));
            fieldsIndex.Add("TDYT", pFeatureClass.Fields.FindField("TDYT"));
            fieldsIndex.Add("SFJBNT", pFeatureClass.Fields.FindField("SFJBNT"));
            fieldsIndex.Add("TDLYLX", pFeatureClass.Fields.FindField("TDLYLX"));
            fieldsIndex.Add("CBJYQQDFS", pFeatureClass.Fields.FindField("CBJYQQDFS"));
            fieldsIndex.Add("HTMJ", pFeatureClass.Fields.FindField("HTMJ"));
            fieldsIndex.Add("SCMJ", pFeatureClass.Fields.FindField("SCMJ"));
            fieldsIndex.Add("BSM", pFeatureClass.Fields.FindField("BSM"));
            fieldsIndex.Add("DKDZ", pFeatureClass.Fields.FindField("DKDZ"));
            fieldsIndex.Add("DKNZ", pFeatureClass.Fields.FindField("DKNZ"));
            fieldsIndex.Add("DKXZ", pFeatureClass.Fields.FindField("DKXZ"));
            fieldsIndex.Add("DKBZ", pFeatureClass.Fields.FindField("DKBZ"));
            return fieldsIndex;
        }
    }
}
