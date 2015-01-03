using System.Collections.Generic;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace TDQQ.AE
{
    interface IAeFactory
    {
        /// <summary>
        /// 打开工作空间
        /// </summary>
        /// <returns>工作空间</returns>
        IFeatureWorkspace OpenWorkspace();

        /// <summary>
        /// 打开要素类
        /// </summary>
        /// <param name="featureClassName">要素类的名称</param>
        /// <returns>要素类</returns>
        IFeatureClass OpenFeatureClasss(string featureClassName);

        /// <summary>
        /// 获取某个字段的index
        /// </summary>
        /// <param name="featureClassName">要素类名称</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns>字段的index</returns>
        int FindField(string featureClassName, string fieldName);

        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="featureClassName">要素类名称</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldLength">字段的长度</param>
        /// <param name="fieldType">字段的类型</param>
        bool AddField(string featureClassName, string fieldName, int fieldLength, esriFieldType fieldType);

        /// <summary>
        /// 如果存在某个要素，删除该要素
        /// </summary>
        /// <param name="feaureClassName">要素名称</param>
        void DeleteIfExist(string feaureClassName);

        /// <summary>
        /// 释放所占要的资源
        /// </summary>
        /// <param name="pFeatureClass">要素类</param>
        void ReleaseFeautureClass(IFeatureClass pFeatureClass);
        /// <summary>
        /// 删除某个字段
        /// </summary>
        /// <param name="pFeatureClass"></param>
        /// <param name="pField"></param>
        /// <returns></returns>
        bool DeleteFields(IFeatureClass pFeatureClass, List<IField> pField);

        bool IsExist(string feaureClassName);
    }
}
