using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;

namespace TDQQ.AE
{
    /// <summary>
    /// 有关Ae的操作
    /// </summary>
    class AeHelper
    {
        /// <summary>
        /// 获取整个个人地理数据库的要素类名称
        /// </summary>
        /// <param name="personDatabase"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAllFeautureClass(string personDatabase)
        {                
                IWorkspaceFactory pWsFt = new AccessWorkspaceFactoryClass();
                IWorkspace pWs = pWsFt.OpenFromFile(personDatabase, 0);
                IEnumDataset pEDataset = pWs.get_Datasets(esriDatasetType.esriDTAny);
                IDataset pDataset = pEDataset.Next();
                while (pDataset != null)
                {
                    if (pDataset.Type == esriDatasetType.esriDTFeatureClass)
                    {
                       // SelectFeauture.Items.Add(pDataset.Name);
                        yield return pDataset.Name;
                    }
                    //如果是数据集
                    else if (pDataset.Type == esriDatasetType.esriDTFeatureDataset)
                    {
                        IEnumDataset pESubDataset = pDataset.Subsets;
                        IDataset pSubDataset = pESubDataset.Next();
                        while (pSubDataset != null)
                        {
                            yield return pSubDataset.Name;
                            pSubDataset = pESubDataset.Next();
                        }
                    }
                    pDataset = pEDataset.Next();
                }          
        }

        public static bool CheckField(string personDatabase, string selectFeaure,string toCheckFieldName)
        {
            IAeFactory pAeFactory=new PersonalGeoDatabase(personDatabase);
            IFeatureClass pFeatureClass = pAeFactory.OpenFeatureClasss(selectFeaure);
            bool flag;
            flag= pFeatureClass.Fields.FindField(toCheckFieldName) == -1 ? false : true;
            pAeFactory.ReleaseFeautureClass(pFeatureClass);
            return flag;
        }

        public static bool CheckVaild(string personDatabase, string selectFeaure)
        {
            IAeFactory pAeFactory = new PersonalGeoDatabase(personDatabase);
            IFeatureClass pFeatureClass = pAeFactory.OpenFeatureClasss(selectFeaure);
            var fieldDirctionary = GetFieldIndex(pFeatureClass);
            bool flag=true;
            foreach (var i in fieldDirctionary)
            {
                if (i.Value==-1)
                {
                    flag = false;
                }
            }
            pAeFactory.ReleaseFeautureClass(pFeatureClass);
            return flag;
        }
        private static Dictionary<string, int> GetFieldIndex(IFeatureClass pFeatureClass)
        {
            Dictionary<string, int> fieldsIndex = new Dictionary<string, int>();
            fieldsIndex.Add("CBFMC", pFeatureClass.Fields.FindField("CBFMC"));
            fieldsIndex.Add("YHTMJ", pFeatureClass.Fields.FindField("YHTMJ"));
            fieldsIndex.Add("DKMC", pFeatureClass.Fields.FindField("DKMC"));
            fieldsIndex.Add("YSDM", pFeatureClass.Fields.FindField("YSDM"));
            fieldsIndex.Add("DKBM", pFeatureClass.Fields.FindField("DKBM"));
            fieldsIndex.Add("DKBZXX", pFeatureClass.Fields.FindField("DKBZXX"));
            fieldsIndex.Add("ZJRXM", pFeatureClass.Fields.FindField("ZJRXM"));
            fieldsIndex.Add("FBFBM", pFeatureClass.Fields.FindField("FBFBM"));
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

        public static bool IsExist(string personDatabase, string selectFeaure)
        {
            IAeFactory pAeFactory=new PersonalGeoDatabase(personDatabase);
            return pAeFactory.IsExist(selectFeaure);
        }
    }
}
