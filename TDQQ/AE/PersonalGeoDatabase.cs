using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;

namespace TDQQ.AE
{
    /// <summary>
    /// 个人地理数据库的相关操作
    /// </summary>
    class PersonalGeoDatabase:IAeFactory
    {
        private readonly string _personalDatabsePath;
        public PersonalGeoDatabase(string personalDatabase)
        {
            _personalDatabsePath = personalDatabase;
        }
        /// <summary>
        /// 打开工作空间
        /// </summary>
        /// <returns></returns>
        public ESRI.ArcGIS.Geodatabase.IFeatureWorkspace OpenWorkspace()
        {
            try
            {
                IWorkspaceName pWorkspaceName = new WorkspaceNameClass();
                pWorkspaceName.WorkspaceFactoryProgID = "esriDataSourcesGDB.AccessWorkspaceFactory";
                pWorkspaceName.PathName = _personalDatabsePath;
                var n = pWorkspaceName as ESRI.ArcGIS.esriSystem.IName;
                var workspace = n.Open() as IFeatureWorkspace;
                return workspace;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 打开要素类
        /// </summary>
        /// <param name="featureClassName">要素类名</param>
        /// <returns>要素类</returns>
        public ESRI.ArcGIS.Geodatabase.IFeatureClass OpenFeatureClasss(string featureClassName)
        {
            var pFeatureWorkspace = OpenWorkspace();
            //如果工作空间没有打开，返回null,否则返回打开的要素类
            return pFeatureWorkspace == null ? null : pFeatureWorkspace.OpenFeatureClass(featureClassName);          
        }

        /// <summary>
        /// 需找某个字段
        /// </summary>
        /// <param name="featureClassName">要素类名</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns>字段的Index值</returns>
        public int FindField(string featureClassName, string fieldName)
        {
            var pFeatrueClass = OpenFeatureClasss(featureClassName);
            return pFeatrueClass == null ? -1 : pFeatrueClass.FindField(fieldName);
        }
        /// <summary>
        /// 增加某个字段
        /// </summary>
        /// <param name="featureClassName">要素类名</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldLength">字段长度</param>
        /// <param name="fieldType">字段类型</param>
        /// <returns>是否添加成功</returns>
        public bool AddField(string featureClassName, string fieldName, int fieldLength, ESRI.ArcGIS.Geodatabase.esriFieldType fieldType)
        {
            var pFeatureClass = OpenFeatureClasss(featureClassName);
            bool flag;
            try
            {
                if (pFeatureClass == null)
                {
                    flag = false;
                }
                else
                {
                    //如果该要素类已经存在
                    if (pFeatureClass.Fields.FindField(fieldName) != -1)
                    {
                        flag = false;
                    }
                    else
                    {
                        var pField = new FieldClass();
                        var pFieldEdit = pField as IFieldEdit;
                        pFieldEdit.Name_2 = fieldName;
                        pFieldEdit.Type_2 = fieldType;
                        pFieldEdit.Length_2 = fieldLength;
                        pFeatureClass.AddField(pFieldEdit);                       
                        flag = true;
                    }
                    //释放要素类COM
                    ReleaseFeautureClass(pFeatureClass);
                }                
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 删除某个要素类，如果该要素类存在
        /// </summary>
        /// <param name="feaureClassName">要素类名</param>
        public void DeleteIfExist(string feaureClassName)
        {
            //throw new NotImplementedException();
            IFeatureWorkspace workspace = OpenWorkspace();
            IEnumDataset dataset = (workspace as IWorkspace).get_Datasets(esriDatasetType.esriDTAny);
            IDataset tmp = null;
            while ((tmp = dataset.Next()) != null && tmp.Name != feaureClassName);
            if (tmp != null)
                tmp.Delete();
        }

        public bool IsExist(string feaureClassName)
        {
            IFeatureWorkspace workspace = OpenWorkspace();
            IEnumDataset dataset = (workspace as IWorkspace).get_Datasets(esriDatasetType.esriDTAny);
            IDataset tmp = null;
            while ((tmp = dataset.Next()) != null)
            {
                if (tmp.Name == feaureClassName)
                {
                    break;
                }
            }
            if (tmp!=null)
            {
                return true;
            }
            return false;
        }
        //bool IAeFactory.AddField(string featureClassName, string fieldName, int fieldLength, esriFieldType fieldType)
        //{
        //    return AddField(featureClassName, fieldName, fieldLength, fieldType);
        //}

        /// <summary>
        /// 释放占用的资源
        /// </summary>
        /// <param name="pFeatureClass">要素类</param>
        public void ReleaseFeautureClass(IFeatureClass pFeatureClass)
        {
            Marshal.FinalReleaseComObject(pFeatureClass);
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }


        /// <summary>
        /// 删除多个字段
        /// </summary>
        /// <param name="pFeatureClass">要素类</param>
        /// <param name="pFields">字段集合</param>
        /// <returns>是否删除成功</returns>
        public bool DeleteFields(IFeatureClass pFeatureClass, List<IField> pFields)
        {
            try
            {
                foreach (var deleteField in pFields)
                {
                    pFeatureClass.DeleteField(deleteField);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
      
    }
}
