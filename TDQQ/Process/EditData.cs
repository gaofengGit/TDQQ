using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using TDQQ.AE;
using TDQQ.Common;
using TDQQ.MyWindow;

namespace TDQQ.Process
{
    /// <summary>
    /// 编辑数据
    /// </summary>
    class EditData
    {
        #region 编辑字段
        public bool EditFields(string personDatabase, string selectFeature, ref string error)
        {
            if (!FieldTypeCheck(personDatabase, selectFeature))
            {
                error = "字段类型不符合要求";
                return false;
            }
            if (!DeleteFields(personDatabase, selectFeature))
            {
                error = "删除字段失败";
                return false;
            }
            if (!AddFields(personDatabase, selectFeature))
            {
                error = "增加字段失败";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查用户自己添加的字段是否满足要求
        /// </summary>
        /// <param name="personDatabase"></param>
        /// <param name="selectFeature"></param>
        /// <returns></returns>
        private bool FieldTypeCheck(string personDatabase, string selectFeature)
        {
            bool flag = true;
            //原合同面积
            if (!Check.ValidCheck.FieldTypeCheck(personDatabase, selectFeature, "YHTMJ", esriFieldType.esriFieldTypeDouble) &&
                !Check.ValidCheck.FieldTypeCheck(personDatabase, selectFeature, "YHTMJ", esriFieldType.esriFieldTypeSingle))
            {
                flag = false;
            }
            //地块名称
            if (!Check.ValidCheck.FieldTypeCheck(personDatabase, selectFeature, "DKMC", esriFieldType.esriFieldTypeString))
            {
                flag = false;
            }
            //承包方名称
            if (!Check.ValidCheck.FieldTypeCheck(personDatabase, selectFeature, "CBFMC", esriFieldType.esriFieldTypeString))
            {
                flag = false;
            }
            return flag;
        }
        private bool DeleteFields(string personDatabase, string selectFeature)
        {
            IAeFactory pAeFactory = new PersonalGeoDatabase(personDatabase);
            IFeatureClass pFeatureClass = pAeFactory.OpenFeatureClasss(selectFeature);
            try
            {
                var toDeleteFields = GetToDeleteFields(pFeatureClass);
                pAeFactory.DeleteFields(pFeatureClass, toDeleteFields);
                pAeFactory.ReleaseFeautureClass(pFeatureClass);
                return true;
            }
            catch (Exception)
            {
                pAeFactory.ReleaseFeautureClass(pFeatureClass);
                return false;
            }
        }
        private List<IField> GetToDeleteFields(IFeatureClass pFeatureClass)
        {
            var toDeleteFields = new List<IField>();
            for (int i = 0; i < pFeatureClass.Fields.FieldCount; i++)
            {
                var fieldName = pFeatureClass.Fields.Field[i].Name.Trim().ToLower();
                if (fieldName != "objectid" && fieldName != "shape" && fieldName != "shape_length" &&
                    fieldName != "shape_area" && fieldName != "cbfmc" && fieldName != "dkmc"
                    && fieldName != "yhtmj" && fieldName != "bl" && fieldName != "clipt")
                    toDeleteFields.Add(pFeatureClass.Fields.Field[i]);
            }
            return toDeleteFields;
        }

        private bool AddFields(string personDatabase, string selectFeature)
        {
            try
            {
                IAeFactory pAeFactory = new PersonalGeoDatabase(personDatabase);
                var toAddFields = GetToAddFields();
                foreach (var tdqqField in toAddFields)
                {
                    pAeFactory.AddField(selectFeature, tdqqField.FieldName, tdqqField.Length, tdqqField.FieldType);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 获取到添加的字段集合
        /// </summary>
        /// <returns>字段集合</returns>
        private IEnumerable<TdqqField> GetToAddFields()
        {
            //string 类型的
            List<TdqqField> listFields = new List<TdqqField>();

            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "DKBM", 19));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "DKMC", 50));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "DKDZ", 50));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "DKNZ", 50));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "DKXZ", 50));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "DKBZ", 50));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "DKBZXX", 300));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "ZJRXM", 100));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "FBFBM", 14));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "CBFBM", 18));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "CBJYQZBM", 19));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "LZHTBM", 20));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "CBHTBM", 18));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "SYQXZ", 2));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "DKLB", 2));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "TDLYLX", 3));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "DLDJ", 2));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "TDYT", 1));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "SFJBNT", 1));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "CBJYQQDFS", 3));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeString, "YSDM", 6));
            //double 类型的
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeDouble, "HTMJ", 15));
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeDouble, "SCMJ", 15));
            //int 类型
            listFields.Add(new TdqqField(esriFieldType.esriFieldTypeInteger, "BSM", 10));
            return listFields;
        }
        #endregion

        #region 设置默认值

        public bool SetDefaultValue(string personDatabase, string selectFeature)
        {
            if (!FieldExistCheck(personDatabase, selectFeature))
            {
                MessageBox.MessageWarning.Show("系统提示", "部分字段尚未添加");
                return false;
            }
            WinSetFieldsValue winSetFieldsValue = new WinSetFieldsValue();

            if (winSetFieldsValue.ShowDialog() == true)
            {

                Wait wait = new Wait();
                wait.SetInfoInvoke("正在设置默认值");
                wait.SetProgressInfo(string.Empty);
                Hashtable para = new Hashtable();
                para["wait"] = wait;
                para["winSetFieldValue"] = winSetFieldsValue;
                para["ret"] = false;
                para["personDatabase"] = personDatabase;
                para["selectFeature"] = selectFeature;
                Thread t = new Thread(new ParameterizedThreadStart(SetDefaultValue));
                t.Start(para);
                wait.ShowDialog();
                t.Abort();
                return (bool)para["ret"];
            }
            return false;
        }

        private void SetDefaultValue(object p)
        {
            Hashtable para = p as Hashtable;
            Wait wait = para["wait"] as Wait;
            WinSetFieldsValue winSetFieldsValue = para["winSetFieldValue"] as WinSetFieldsValue;
            IAeFactory pAeFactory = new PersonalGeoDatabase(para["personDatabase"].ToString());
            IFeatureClass pFeatureClass = pAeFactory.OpenFeatureClasss(para["selectFeature"].ToString());
            var pDataset = pFeatureClass as IDataset;
            if (pDataset == null)
            {
                wait.CloseWait();
                para["ret"] = false;
                return;
            }
            var pWorkspaceEdit = pDataset.Workspace as IWorkspaceEdit;
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
            if (pWorkspaceEdit == null)
            {
                wait.CloseWait();
                para["ret"] = false;
                return;
            }
            int total = GetFeatureCount(para["personDatabase"].ToString(), para["selectFeature"].ToString());
            pWorkspaceEdit.StartEditing(true);
            pWorkspaceEdit.StartEditOperation();
            IFeature pFeature = pFeatureCursor.NextFeature();
            int currentIndex = 0;
            while (pFeature != null)
            {
                wait.SetProgressInfo(((double)currentIndex++ / (double)total).ToString("P"));
                pFeature.set_Value(pFeatureClass.FindField("FBFBM"), winSetFieldsValue.Fbfbm);
                pFeature.set_Value(pFeatureClass.FindField("ZJRXM"), winSetFieldsValue.Zjrxm);
                pFeature.set_Value(pFeatureClass.FindField("TDLYLX"), winSetFieldsValue.Tdlylx);
                pFeature.set_Value(pFeatureClass.FindField("CBJYQQDFS"), winSetFieldsValue.Cbjyqqdfs);
                pFeature.set_Value(pFeatureClass.FindField("SYQXZ"), winSetFieldsValue.Syqxz);
                pFeature.set_Value(pFeatureClass.FindField("TDYT"), winSetFieldsValue.Tdyt);
                pFeature.set_Value(pFeatureClass.FindField("DLDJ"), winSetFieldsValue.Dldj);
                pFeature.set_Value(pFeatureClass.FindField("SFJBNT"), winSetFieldsValue.Sfjbnt);
                pFeature.set_Value(pFeatureClass.FindField("DKLB"), winSetFieldsValue.Dklb);
                pFeature.set_Value(pFeatureClass.FindField("DLDJ"), winSetFieldsValue.Dldj);
                // pFeature.set_Value(pFeatureClass.FindField("DKBM"), winSetFieldsValue.Fbfbm+currentIndex.ToString("00000"));
                //var scmj = Convert.ToDouble(pFeature.get_Value(FindShapeAreIndex(pFeatureClass)).ToString())/666.6;
                //pFeature.set_Value(pFeatureClass.FindField("SCMJ"), scmj);
                pFeature.Store();
                pFeature = pFeatureCursor.NextFeature();
            }
            Marshal.ReleaseComObject(pFeatureCursor);
            pWorkspaceEdit.StopEditOperation();
            pWorkspaceEdit.StopEditing(true);
            wait.CloseWait();
            para["ret"] = true;
            return;
        }

        private bool FieldExistCheck(string personDatabase, string selectFeature)
        {
            /*
            IAeFactory pAeFactory=new PersonalGeoDatabase(personDatabase);
            IFeatureClass pFeatureClass = pAeFactory.OpenFeatureClasss(selectFeature);
            if (pFeatureClass==null)return false;
            var fieldIndex = GetFieldIndex(pFeatureClass);
            foreach (var field in fieldIndex)
            {
                if (field.Value == -1)
                {
                    pAeFactory.ReleaseFeautureClass(pFeatureClass);
                    return false;
                }
            }
            pAeFactory.ReleaseFeautureClass(pFeatureClass);
            return true;
             */
            return Check.ValidCheck.FieldExistCheck(personDatabase, selectFeature,
                "CBFMC", "YHTMJ",
                "DKMC", "YSDM",
                "DKBZXX", "ZJRXM",
                "FBFBM", "SYQXZ",
                "DKLB", "DLDJ",
                "TDYT", "SFJBNT",
                "TDLYLX", "CBJYQQDFS",
                "HTMJ", "SCMJ",
                "BSM", "DKDZ",
                "DKNZ", "DKXZ",
                "DKBZ", "DKBM");
        }

        private Dictionary<string, int> GetFieldIndex(IFeatureClass pFeatureClass)
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

        private int GetFeatureCount(string personDatabase, string selectFeature)
        {
            var pAccess = new AccessFactory(personDatabase);
            var commandText = string.Format("Select Count(*) from {0}", selectFeature);
            return (int)pAccess.ExecuteScalar(commandText);
        }

        private int FindShapeAreIndex(IFeatureClass pFeatureClass)
        {
            for (int i = 0; i < pFeatureClass.Fields.FieldCount; i++)
            {
                if (pFeatureClass.Fields.Field[i].Name.ToLower() == "shape_area")
                {
                    return i;
                }
            }
            return -1;
        }
        #endregion

        #region 提取四至
        public bool GetSurround(string personDatabase, string selectFeature)
        {
            if (!Check.ValidCheck.FieldExistCheck(personDatabase, selectFeature, "CBFMC", "DKDZ", "DKNZ", "DKBZ", "DKXZ"))
            {
                MessageBox.MessageWarning.Show("系统提示", "尚未添加部分字段");
                return false;
            }
            WinBuffer winBuffer = new WinBuffer();
            if (winBuffer.ShowDialog() == true)
            {
                Wait wait = new Wait();
                wait.SetInfoInvoke("正在提取四至");
                wait.SetProgressInfo(string.Empty);
                Hashtable para = new Hashtable();
                para["personDatabase"] = personDatabase;
                para["selectFeature"] = selectFeature;
                para["total"] = GetFeatureCount(personDatabase, selectFeature);
                para["wait"] = wait;
                para["bufferDis"] = winBuffer.Distance;
                para["ret"] = false;
                Thread t = new Thread(new ParameterizedThreadStart(GetSurroundF));
                t.Start(para);
                wait.ShowDialog();
                t.Abort();
                return (bool)para["ret"];

            }
            return false;
        }

        /// <summary>
        /// 获取地块四至函数
        /// </summary>
        /// <param name="p"></param>
        private void GetSurroundF(object p)
        {
            Hashtable para = p as Hashtable;
            var personDatabase = para["personDatabase"].ToString();
            var selectFeature = para["selectFeature"].ToString();
            var wait = para["wait"] as Wait;
            var total = (int)para["total"];
            var distance = (double)para["bufferDis"];

            IAeFactory aeFactory = new PersonalGeoDatabase(personDatabase);
            IFeatureClass pFeatureClass = aeFactory.OpenFeatureClasss(selectFeature);
            IFeatureCursor pCursor = pFeatureClass.Search(null, false);
            IFeature pFeature;
            IDataset dataset = (IDataset)pFeatureClass;
            IWorkspace myworkspace = dataset.Workspace;
            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)myworkspace;

            try
            {
                var dxnbIndex = GetDxnbIndex(pFeatureClass);
                //if (dxnbIndex.Count == 0)
                //{
                //    MessageWarning.Show("系统提示", "东南西北字段不存在！");
                //    wait.CloseWait();
                //    para["ret"] = false;
                //    return;
                //}
                int currentIndex = 0;
                workspaceEdit.StartEditing(true);
                workspaceEdit.StartEditOperation();
                while ((pFeature = pCursor.NextFeature()) != null)
                {
                    wait.SetProgressInfo(((double)currentIndex++ / (double)total).ToString("p"));
                    var surrounds = GetSurroundName(pFeature, pFeatureClass, distance);
                    if (surrounds == null)
                    {
                        wait.CloseWait();
                        para["ret"] = false;
                        return;
                    }
                    pFeature.set_Value(dxnbIndex[0], surrounds[0]);
                    pFeature.set_Value(dxnbIndex[1], surrounds[1]);
                    pFeature.set_Value(dxnbIndex[2], surrounds[2]);
                    pFeature.set_Value(dxnbIndex[3], surrounds[3]);
                    pFeature.Store();
                }
                workspaceEdit.StopEditOperation();
                workspaceEdit.StopEditing(true);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(pFeatureClass);
                GC.WaitForPendingFinalizers();
                GC.Collect();
                wait.CloseWait();
                para["ret"] = true;
                return;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
                Marshal.FinalReleaseComObject(pFeatureClass);
                GC.WaitForPendingFinalizers();
                GC.Collect();
                //wait.CloseWait();
                //para["ret"] = false;
                wait.CloseWait();
                para["ret"] = false;
                return;
            }
        }
        /// <summary>
        /// 设置四至的名称
        /// </summary>
        /// <param name="pFeature">要设置四至地块</param>
        /// <param name="pFeatureClass">整个要素类</param>
        /// <returns>四至</returns>
        private List<string> GetSurroundName(IFeature pFeature, IFeatureClass pFeatureClass, double distance)
        {
            List<string> nameList = new List<string>() { "路", "路", "路", "路" };
            var cbfmcIndex = pFeatureClass.Fields.FindField("CBFMC");
            //if (cbfmcIndex == -1) throw new Exception("不存在承包方编码字段");
            ITopologicalOperator topo = pFeature.Shape as ITopologicalOperator;
            IGeometry topoGeometry = topo.Buffer(distance);
            IEnvelope pCenterEnvelope = pFeature.Shape.Envelope;
            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            pSpatialFilter.Geometry = topoGeometry;
            IFeatureCursor pCursor = pFeatureClass.Search(pSpatialFilter, false);
            IFeature pSurFeaure;
            List<double> shapeArea = new List<double>(4) { 0.0, 0.0, 0.0, 0.0 };
            var shapeAreaIndex = GetFieldIndex(pFeature, "shape_area");
            if (shapeAreaIndex == -1) return null;
            while ((pSurFeaure = pCursor.NextFeature()) != null)
            {
                int direction = GetDirection(pCenterEnvelope, pSurFeaure.Shape.Envelope);
                if (direction != -1)
                {
                    if (Convert.ToDouble(pSurFeaure.get_Value(shapeAreaIndex).ToString()) >=
                        shapeArea[direction])
                    {

                        nameList[direction] = pSurFeaure.get_Value(cbfmcIndex).ToString();
                        shapeArea[direction] =
                          Convert.ToDouble(pSurFeaure.get_Value(shapeAreaIndex).ToString());
                    }
                }
            }
            Marshal.ReleaseComObject(pCursor);
            return nameList;
        }

        private int GetFieldIndex(IFeature pFeature, string fieldNameLowCapper)
        {
            for (int i = 0; i < pFeature.Fields.FieldCount; i++)
            {
                if (pFeature.Fields.Field[i].Name.ToLower() == fieldNameLowCapper)
                    return i;
            }
            return -1;
        }
        /// <summary>
        /// 判断两个地块的方位关系
        /// </summary>
        /// <param name="centerPolygon">中间地块</param>
        /// <param name="targetPolygon">目标地块</param>
        /// <returns></returns>
        private int GetDirection(IEnvelope centerPolygon, IEnvelope targetPolygon)
        {
            //获取中心点和目标中心点
            IPoint centerPoint = new PointClass();
            centerPoint.PutCoords((centerPolygon.XMax + centerPolygon.XMin) / 2, (centerPolygon.YMax + centerPolygon.YMin) / 2);
            IPoint targetPoint = new PointClass();
            targetPoint.PutCoords((targetPolygon.XMax + targetPolygon.XMin) / 2, (targetPolygon.YMax + targetPolygon.YMin) / 2);
            //判断方位
            IPoint vectorPoint = new PointClass();
            vectorPoint.PutCoords(targetPoint.X - centerPoint.X, targetPoint.Y - centerPoint.Y);
            //MessageBox.Show(string.Format("x:{0};y:{1}", vectorPoint.X, vectorPoint.Y));
            //如果方向是东
            if (vectorPoint.X > 0 && (vectorPoint.X >= Math.Abs(vectorPoint.Y)))
            {
                return 0;
            }
            //如果方向是西
            if (vectorPoint.X < 0 && (Math.Abs(vectorPoint.X)) >= (Math.Abs(vectorPoint.Y)))
            {
                return 2;
            }
            //如果方向是南
            if (vectorPoint.Y < 0 && (Math.Abs(vectorPoint.Y)) > (Math.Abs(vectorPoint.X)))
            {
                return 1;
            }
            //如果方向是北
            if (vectorPoint.Y > 0 && (Math.Abs(vectorPoint.Y)) > (Math.Abs(vectorPoint.X)))
            {
                return 3;
            }
            return -1;
        }

        /// <summary>
        /// 获取东南西北四至的字段的位置
        /// </summary>
        /// <param name="pFeatureClass">要素类</param>
        /// <returns>东西南北</returns>
        private List<int> GetDxnbIndex(IFeatureClass pFeatureClass)
        {
            List<int> dxnzList = new List<int>();
            int index;
            index = pFeatureClass.Fields.FindField("DKDZ");
            if (index != -1) dxnzList.Add(index);
            index = pFeatureClass.Fields.FindField("DKNZ");
            if (index != -1) dxnzList.Add(index);
            index = pFeatureClass.Fields.FindField("DKXZ");
            if (index != -1) dxnzList.Add(index);
            index = pFeatureClass.Fields.FindField("DKBZ");
            if (index != -1) dxnzList.Add(index);
            return dxnzList;
        }

        #endregion

        #region 提取承包方编码
        public bool UnpdateCbfbm(string personDatabase, string selectFeature, string basicDatabase)
        {
            if (!Check.ValidCheck.FieldExistCheck(personDatabase, selectFeature, "CBFMC", "CBFBM"))
            {
                MessageBox.MessageWarning.Show("系统提示", "尚不存在CBFBM和CBFMC字段");
                return false;
            }
            var wait = new Wait();
            wait.SetInfoInvoke("正在更新承包方编码");
            wait.SetProgressInfo(string.Empty);
            Hashtable para = new Hashtable();
            para["personDatabase"] = personDatabase;
            para["selectFeature"] = selectFeature;
            para["basicDatabase"] = basicDatabase;
            para["total"] = GetFeatureCount(personDatabase, selectFeature);
            para["wait"] = wait;
            para["ret"] = false;
            Thread t = new Thread(new ParameterizedThreadStart(UpdateCbfbm));
            t.Start(para);
            wait.ShowDialog();
            t.Abort();
            return (bool)para["ret"];
        }

        private void UpdateCbfbm(object p)
        {
            Hashtable para = p as Hashtable;
            var personDatabase = para["personDatabase"].ToString();
            var selectFeature = para["selectFeature"].ToString();
            var basicDatabase = para["basicDatabase"].ToString();
            var total = (int)para["total"];
            var wait = para["wait"] as Wait;
            try
            {
                IAeFactory aeFactory = new PersonalGeoDatabase(personDatabase);
                IFeatureClass pFeatureClass = aeFactory.OpenFeatureClasss(selectFeature);
                var cbfmcIndex = pFeatureClass.Fields.FindField("CBFMC");
                var cbfbmIndex = pFeatureClass.Fields.FindField("CBFBM");
                var pDataset = pFeatureClass as IDataset;
                if (pDataset == null)
                {
                    wait.CloseWait();
                    para["ret"] = false;
                    return;
                }
                var pWorkspaceEdit = pDataset.Workspace as IWorkspaceEdit;
                IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
                if (pWorkspaceEdit == null)
                {
                    wait.CloseWait();
                    para["ret"] = false;
                    return;
                }
                pWorkspaceEdit.StartEditing(true);
                pWorkspaceEdit.StartEditOperation();
                IFeature pFeature = pFeatureCursor.NextFeature();
                int errorIndex = 0;
                int currentIndex = 0;
                //错误的承包编码为同一户，使用同样的错误承包方编码
                Dictionary<string, string> errorCbfmcDictionary = new Dictionary<string, string>();
                while (pFeature != null)
                {
                    wait.SetProgressInfo(((double)currentIndex++ / (double)total).ToString("p"));
                    var cbfmc = pFeature.get_Value(cbfmcIndex).ToString().Trim();
                    if (errorCbfmcDictionary.ContainsKey(cbfmc))
                    {
                        string errorCbfbm;
                        errorCbfmcDictionary.TryGetValue(cbfmc, out errorCbfbm);
                        pFeature.set_Value(cbfbmIndex, errorCbfbm);
                    }
                    else
                    {
                        var cbfbm = GetCbfbm(basicDatabase, cbfmc, ref errorIndex);
                        if (cbfbm.StartsWith("9999"))
                        {
                            errorCbfmcDictionary.Add(cbfmc, cbfbm);
                        }
                        pFeature.set_Value(cbfbmIndex, cbfbm);
                    }
                    pFeature.Store();
                    pFeature = pFeatureCursor.NextFeature();
                }
                Marshal.ReleaseComObject(pFeatureCursor);
                pWorkspaceEdit.StopEditOperation();
                pWorkspaceEdit.StopEditing(true);
                aeFactory.ReleaseFeautureClass(pFeatureClass);
                wait.CloseWait();
                para["ret"] = true;
            }
            catch (Exception e)
            {
                wait.CloseWait();
                para["ret"] = false;
            }

        }
        private string GetCbfbm(string basicDatabase, string cbfmc, ref int errorIndex)
        {
            var sqlString = string.Format("SELECT CBFBM FROM {0} WHERE TRIM(CBFMC)='{1}' ", "CBF", cbfmc);
            AccessFactory accessFactory = new AccessFactory(basicDatabase);
            var dt = accessFactory.Query(sqlString);
            if (dt == null || dt.Rows.Count != 1)
            {
                errorIndex++;
                return "99999999999999" + errorIndex.ToString("0000");
            }
            else
            {
                return dt.Rows[0][0].ToString().Trim();
            }
        }

        private bool ValidCheck(string personDatabase, string selectFeaure)
        {
            IAeFactory pAeFactory = new PersonalGeoDatabase(personDatabase);
            IFeatureClass pFeatureClass = pAeFactory.OpenFeatureClasss(selectFeaure);
            if (pFeatureClass.Fields.FindField("CBFMC") == -1 || pFeatureClass.Fields.FindField("CBFBM") == -1)
            {
                pAeFactory.ReleaseFeautureClass(pFeatureClass);
                return false;
            }
            pAeFactory.ReleaseFeautureClass(pFeatureClass);
            return true;
        }
        #endregion

        #region 设置合同面积

        public bool SetHtmj(string personDatabase, string selectFeauture)
        {
            if (!Check.ValidCheck.FieldExistCheck(personDatabase, selectFeauture, "HTMJ"))
            {
                MessageBox.MessageWarning.Show("系统提示", "不存在HTMJ字段");
                return false;
            }
            bool choice = true;
            if (!SetHtmjChoice(personDatabase, selectFeauture, ref choice)) return false;
            Wait wait = new Wait();
            wait.SetInfoInvoke("正在设置合同面积");
            wait.SetProgressInfo(string.Empty);
            Hashtable para = new Hashtable();
            para["wait"] = wait;
            para["personDatabase"] = personDatabase;
            para["selectFeauture"] = selectFeauture;
            para["choice"] = choice;
            para["ret"] = false;
            Thread t = new Thread(new ParameterizedThreadStart(SetHtmj));
            t.Start(para);
            wait.ShowDialog();
            t.Abort();
            return (bool)para["ret"];
        }

        private void SetHtmj(object p)
        {
            Hashtable para = p as Hashtable;
            Wait wait = para["wait"] as Wait;
            try
            {
                bool choice = (bool)para["choice"];
                IAeFactory pAeFactory = new PersonalGeoDatabase(para["personDatabase"].ToString());
                IFeatureClass pFeatureClass = pAeFactory.OpenFeatureClasss(para["selectFeauture"].ToString());
                var pDataset = pFeatureClass as IDataset;
                if (pDataset == null)
                {
                    wait.CloseWait();
                    para["ret"] = false;
                    return;
                }
                var pWorkspaceEdit = pDataset.Workspace as IWorkspaceEdit;
                IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
                if (pWorkspaceEdit == null)
                {
                    wait.CloseWait();
                    para["ret"] = false;
                    return;
                }
                int total = GetFeatureCount(para["personDatabase"].ToString(), para["selectFeauture"].ToString());
                pWorkspaceEdit.StartEditing(true);
                pWorkspaceEdit.StartEditOperation();
                IFeature pFeature = pFeatureCursor.NextFeature();
                int currentIndex = 0;
                while (pFeature != null)
                {
                    wait.SetProgressInfo(((double)currentIndex++ / (double)total).ToString("P"));
                    if (choice)
                    {
                        pFeature.set_Value(pFeatureClass.FindField("HTMJ"), pFeature.get_Value(pFeatureClass.FindField("YHTMJ")));
                    }
                    else
                    {
                        var scmj = Convert.ToDouble(pFeature.get_Value(FindShapeAreIndex(pFeatureClass)).ToString()) / 666.6;
                        pFeature.set_Value(pFeatureClass.FindField("HTMJ"), scmj);
                    }
                    pFeature.Store();
                    pFeature = pFeatureCursor.NextFeature();
                }
                Marshal.ReleaseComObject(pFeatureCursor);
                pWorkspaceEdit.StopEditOperation();
                pWorkspaceEdit.StopEditing(true);
                pAeFactory.ReleaseFeautureClass(pFeatureClass);
                wait.CloseWait();
                para["ret"] = true;
                return;
            }
            catch (Exception e)
            {

                System.Windows.Forms.MessageBox.Show(e.ToString());
                wait.CloseWait();
                para["ret"] = false;
                return;
            }

        }
        private bool SetHtmjChoice(string personDatabase, string selectFeauture, ref bool choice)
        {
            if (MessageBox.MessageQuestion.Show("系统询问", "是否将原合同面积设置为合同面积？") == true)
            {
                if (!CheckYhtmj(personDatabase, selectFeauture))
                {
                    MessageBox.MessageWarning.Show("系统提示", "不存在YHTMJ字段");
                    return false;
                }

                //if (!CheckHtmj(personDatabase, selectFeauture))
                //{
                //    MessageBox.MessageWarning.Show("系统提示", "不存在HTMJ字段");
                //    return false;
                //}

                choice = true;
            }
            else
            {
                //if (!CheckHtmj(personDatabase, selectFeauture))
                //{
                //    MessageBox.MessageWarning.Show("系统提示", "不存在HTMJ字段");
                //    return false;
                //}
                choice = false;
            }
            return true;
        }
        private bool CheckYhtmj(string personDatabase, string selectFeaure)
        {
            /*
            IAeFactory pAeFactory=new PersonalGeoDatabase(personDatabase);
            IFeatureClass pFeatureClass = pAeFactory.OpenFeatureClasss(selectFeaure);
            if (pFeatureClass.Fields.FindField("YHTMJ") == -1)
            {
                pAeFactory.ReleaseFeautureClass(pFeatureClass);
                return false;
            }
            pAeFactory.ReleaseFeautureClass(pFeatureClass);
            return true;  
             */
            return Check.ValidCheck.FieldExistCheck(personDatabase, selectFeaure, "YHTMJ");
        }

        private bool CheckHtmj(string personDatabase, string selectFeaure)
        {
            IAeFactory pAeFactory = new PersonalGeoDatabase(personDatabase);
            IFeatureClass pFeatureClass = pAeFactory.OpenFeatureClasss(selectFeaure);
            if (pFeatureClass.Fields.FindField("HTMJ") == -1)
            {
                pAeFactory.ReleaseFeautureClass(pFeatureClass);
                return false;
            }
            pAeFactory.ReleaseFeautureClass(pFeatureClass);
            return true;
        }
        #endregion

        #region 替换承包名称

        public bool ReplaceCbfmc(string personDatabase, string selectFeaure)
        {
            if (!Check.ValidCheck.FieldExistCheck(personDatabase, selectFeaure, "CBFMC"))
            {
                MessageBox.MessageWarning.Show("系统提示", "不存在CBFMC字段");
                return false;
            }
            WinReplce winReplce = new WinReplce();
            if (winReplce.ShowDialog() == true)
            {
                Wait wait = new Wait();
                wait.SetInfoInvoke("正在替换名称");
                wait.SetProgressInfo(string.Empty);
                Hashtable para = new Hashtable();
                para["personDatabase"] = personDatabase;
                para["selectFeaure"] = selectFeaure;
                para["wait"] = wait;
                para["winReplce"] = winReplce;
                para["ret"] = false;
                Thread t = new Thread(new ParameterizedThreadStart(ReplaceCbfmc));
                t.Start(para);
                wait.ShowDialog();
                t.Abort();
                return (bool)para["ret"];
            }
            return false;
        }

        private void ReplaceCbfmc(object p)
        {

            Hashtable para = p as Hashtable;
            Wait wait = para["wait"] as Wait;

            WinReplce winReplce = para["winReplce"] as WinReplce;
            IAeFactory aeFactory = new PersonalGeoDatabase(para["personDatabase"].ToString());
            IFeatureClass pFeatureClass = aeFactory.OpenFeatureClasss(para["selectFeaure"].ToString());
            int cbfmcIndex = pFeatureClass.FindField("CBFMC");
            //if (cbfmcIndex == -1)
            //{
            //    wait.CloseWait();
            //    para["ret"] = false;
            //    return;
            //}
            try
            {
                IFeatureCursor pCursor = pFeatureClass.Search(null, false);
                int total = GetFeatureCount(para["personDatabase"].ToString(), para["selectFeaure"].ToString());
                IDataset pDataset = pFeatureClass as IDataset;
                IWorkspaceEdit pWorkspaceEdit = pDataset.Workspace as IWorkspaceEdit;
                pWorkspaceEdit.StartEditing(true);
                pWorkspaceEdit.StartEditOperation();
                int currentIndex = 0;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("承包方名称为：" + winReplce.OriginCbfmc + "替换数量:");
                IFeature pFeature;
                int count = 0;
                while ((pFeature = pCursor.NextFeature()) != null)
                {
                    wait.SetProgressInfo(((double)currentIndex++ / (double)total).ToString("P"));
                    string cbfmc = pFeature.get_Value(cbfmcIndex).ToString().Trim();
                    if (cbfmc == winReplce.OriginCbfmc)
                    {
                        pFeature.set_Value(cbfmcIndex, winReplce.NewCbfmc);
                        count++;
                        pFeature.Store();
                    }
                }
                stringBuilder.Append(count.ToString());
                CommonHelper.WriteErrorInfo(AppDomain.CurrentDomain.BaseDirectory + @"\Log\logfile.txt", stringBuilder.ToString());
                Marshal.ReleaseComObject(pCursor);
                pWorkspaceEdit.StopEditOperation();
                pWorkspaceEdit.StopEditing(true);
                aeFactory.ReleaseFeautureClass(pFeatureClass);
                wait.CloseWait();
                para["ret"] = true;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
                wait.CloseWait();
                para["ret"] = false;
            }
        }
        #endregion

        #region 处理非承包方问题

        public bool ChangeNotCbfmc(string personDatabase, string selectFeature, string baiscDatabase)
        {
            if (!AeHelper.CheckField(personDatabase, selectFeature, "CBFMC"))
            {
                MessageBox.MessageWarning.Show("系统提示", "不存在CBFMC字段");
                return false;
            }
            Wait wait = new Wait();
            wait.SetInfoInvoke("正在处理非承包方名称");
            wait.SetProgressInfo(string.Empty);
            Hashtable para = new Hashtable();
            para["wait"] = wait;
            para["personDatabase"] = personDatabase;
            para["selectFeature"] = selectFeature;
            para["baiscDatabase"] = baiscDatabase;
            para["ret"] = false;
            Thread t = new Thread(new ParameterizedThreadStart(ChangeNotCbfmc));
            t.Start(para);
            wait.ShowDialog();
            t.Abort();
            return (bool)para["ret"];

        }

        private void ChangeNotCbfmc(object p)
        {
            Hashtable para = p as Hashtable;
            Wait wait = para["wait"] as Wait;
            var dt = GetNotCbfmcDataTable(para["personDatabase"].ToString(), para["selectFeature"].ToString());
            if (dt == null)
            {
                wait.CloseWait();
                para["ret"] = false;
                return;
            }
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("处理非承包方名称有：\r\n");
                int rowCount = dt.Rows.Count;
                int currentIndex = 0;
                for (int i = 0; i < rowCount; i++)
                {
                    wait.SetProgressInfo(((double)currentIndex++ / (double)rowCount).ToString("P"));
                    var cbfmc = GetHzFromXm(dt.Rows[i][0].ToString().Trim(), para["baiscDatabase"].ToString());
                    if (string.IsNullOrEmpty(cbfmc)) continue;
                    UpDateNotCbfmc(dt.Rows[i][1].ToString().Trim(), cbfmc, para["personDatabase"].ToString(),
                        para["selectFeature"].ToString());
                    stringBuilder.Append(cbfmc + "\r\n");
                }
                CommonHelper.WriteErrorInfo(AppDomain.CurrentDomain.BaseDirectory + @"\Log\logfile.txt", stringBuilder.ToString());
                wait.CloseWait();
                para["ret"] = true;
                return;
            }
            catch (Exception e)
            {
                wait.CloseWait();
                para["ret"] = false;
            }

        }
        private DataTable GetNotCbfmcDataTable(string personDatabase, string selectFeautre)
        {
            //查询条件为前面14为9999999999999
            var sqlString = string.Format("Select CBFMC,DKBM from {0} where CBFBM Like '{1}'", selectFeautre,
                "99999999999999%");
            AccessFactory accessFactory = new AccessFactory(personDatabase);
            return accessFactory.Query(sqlString);
        }
        private string GetHzFromXm(string notCbfmc, string basicDatabase)
        {
            AccessFactory accessFactory = new AccessFactory(basicDatabase);
            var sqlString = string.Format("select CBFMC from {0} where CYXM='{1}'", "CBF_JTCY", notCbfmc);
            var dt = accessFactory.Query(sqlString);
            if (dt == null || dt.Rows.Count != 1) return string.Empty;
            return dt.Rows[0][0].ToString().Trim();
        }
        private void UpDateNotCbfmc(string dkbm, string cbfmc, string personDatbase, string selectFeaure)
        {
            AccessFactory accessFactory = new AccessFactory(personDatbase);
            var sqlString = string.Format("update {0} set CBFMC ='{1}' where DKBM ='{2}'", selectFeaure, cbfmc,
                dkbm);
            accessFactory.Execute(sqlString);
        }
        #endregion

        #region 地块编码

        private bool SortDkbm(string personDatabase, string selectFeaure)
        {
            if (!Check.ValidCheck.FieldExistCheck(personDatabase, selectFeaure, "DKBM"))
            {
                MessageBox.MessageWarning.Show("系统提示", "不存在地块编码字段");
                return false;
            }
            // throw new NotImplementedException();
            IAeFactory pAeFactory = new PersonalGeoDatabase(personDatabase);
            IFeatureClass pFeatureClass = pAeFactory.OpenFeatureClasss(selectFeaure);
            var dks = GetDks(pFeatureClass);
            var sortdks = dks.OrderBy(feaure => feaure.Ycor);
            var first = sortdks.First();
            var last = sortdks.Last();
            WinDkbm winDkbm = new WinDkbm(last.Ycor - first.Ycor);
            pAeFactory.ReleaseFeautureClass(pFeatureClass);
            if (winDkbm.ShowDialog() == true)
            {
                Wait wait = new Wait();
                wait.SetInfoInvoke("正在对地块进行编码");
                wait.SetProgressInfo("预处理中");
                Hashtable para = new Hashtable();
                para["wait"] = wait;
                para["personDatabase"] = personDatabase;
                para["selectFeature"] = selectFeaure;
                para["gap"] = winDkbm.Gap;
                para["fbfbm"] = winDkbm.Fbfbm;
                // para["sortdks"] = sortdks;
                para["yMax"] = last.Ycor;
                para["yMin"] = first.Ycor;
                para["ret"] = false;
                Thread t = new Thread(new ParameterizedThreadStart(SorkDkbm));
                t.Start(para);
                wait.ShowDialog();
                t.Abort();
                return (bool)para["ret"];
            }
            else
            {
                return false;
            }



        }

        private void SorkDkbm(object p)
        {
            Hashtable para = p as Hashtable;
            Wait wait = para["wait"] as Wait;
            try
            {
                // var sortDks = para["sortdks"] as IEnumerable<SortEnity<object>>;
                var gap = (double)para["gap"];
                var fbfbm = para["fbfbm"].ToString();
                var total = GetFeatureCount(para["personDatabase"].ToString(), para["selectFeature"].ToString());
                var yMax = (double)para["yMax"];
                var yMin = (double)para["yMin"];
                var currentTop = yMax + 10;
                var currentButton = currentTop - gap;
                int currentIndex = 0;
                IAeFactory pAeFactory = new PersonalGeoDatabase(para["personDatabase"].ToString());
                IFeatureClass pFeatureClass = pAeFactory.OpenFeatureClasss(para["selectFeature"].ToString());
                var sortDks = GetDks(pFeatureClass);
                while (currentTop > yMin - 10)
                {
                    var currenDks = GetYDks(sortDks, currentTop, currentButton);
                    SortDkbm(currenDks, fbfbm, ref currentIndex, wait, total, para["personDatabase"].ToString(), para["selectFeature"].ToString());
                    currentTop -= gap;
                    currentButton -= gap;
                }
                /*
                 */
                para["ret"] = true;
                wait.CloseWait();
                return;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
                //throw;
            }

            para["ret"] = false;
            wait.CloseWait();

        }

        private IEnumerable<SortEnity<object>> GetYDks(IEnumerable<SortEnity<object>> dks, double top, double butttom)
        {
            foreach (var sortEnity in dks)
            {
                if (sortEnity.Ycor < top && sortEnity.Ycor >= butttom)
                {
                    yield return sortEnity;
                }
            }
        }
        private void SortDkbm(IEnumerable<SortEnity<object>> ySortDks, string fbfbm, ref int current, Wait wait, int total,
            string personDatabase, string selectFeature)
        {
            AccessFactory accessFactory = new AccessFactory(personDatabase);
            var xSortdk = ySortDks.OrderBy(dk => dk.Xcor);
            var sqlString = string.Empty;
            foreach (var sortEnity in xSortdk)
            {
                current++;
                wait.SetProgressInfo(((double)current / (double)total).ToString("p"));
                sqlString = string.Format("update {0} set DKBM ='{1}' where OBJECTID = {2} ", selectFeature,
                    fbfbm + current.ToString("00000"), sortEnity.Id);
                accessFactory.Execute(sqlString);
            }
        }
        //private void sortDkbm

        /// <summary>
        /// 获取整体地块
        /// </summary>
        /// <param name="pFeatureClass"></param>
        /// <returns></returns>
        private IEnumerable<SortEnity<object>> GetDks(IFeatureClass pFeatureClass)
        {
            //获取ObjectID的序号值
            var objectIdIndex = pFeatureClass.Fields.FindField("OBJECTID");
            IFeatureCursor pCursor = pFeatureClass.Search(null, false);
            List<SortEnity<object>> dks = new List<SortEnity<object>>();
            IFeature pFeature = null;
            try
            {
                //循环，获取整体的地块
                while ((pFeature = pCursor.NextFeature()) != null)
                {
                    IEnvelope pEnvelop = pFeature.Shape.Envelope;
                    dks.Add(new SortEnity<object>()
                    {
                        Id = (object)pFeature.get_Value(objectIdIndex),
                        Xcor = pEnvelop.XMax * 0.5 + pEnvelop.XMin * 0.5,
                        Ycor = pEnvelop.YMax * 0.5 + pEnvelop.YMin * 0.5
                    });
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
                return null;
            }
            return dks;
        }

        #endregion

        #region 设置实测面积

        public bool SetScmj(string personDatabase, string selectFeature)
        {
            if (!FieldExistCheck(personDatabase, selectFeature))
            {
                MessageBox.MessageWarning.Show("系统提示", "部分字段尚未添加");
                return false;
            }
            Wait wait = new Wait();
            wait.SetInfoInvoke("设置实测面积");
            wait.SetProgressInfo(string.Empty);
            Hashtable para = new Hashtable();
            para["wait"] = wait;
            para["ret"] = false;
            para["personDatabase"] = personDatabase;
            para["selectFeature"] = selectFeature;
            Thread t = new Thread(new ParameterizedThreadStart(SetScmj));
            t.Start(para);
            wait.ShowDialog();
            t.Abort();
            return (bool)para["ret"];

        }

        private void SetScmj(object p)
        {
            Hashtable para = p as Hashtable;
            var wait = para["wait"] as Wait;
            IAeFactory pAeFactory = new PersonalGeoDatabase(para["personDatabase"].ToString());
            IFeatureClass pFeatureClass = pAeFactory.OpenFeatureClasss(para["selectFeature"].ToString());
            var pDataset = pFeatureClass as IDataset;
            if (pDataset == null)
            {
                wait.CloseWait();
                para["ret"] = false;
                return;
            }
            var pWorkspaceEdit = pDataset.Workspace as IWorkspaceEdit;
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
            if (pWorkspaceEdit == null)
            {
                wait.CloseWait();
                para["ret"] = false;
                return;
            }
            int total = GetFeatureCount(para["personDatabase"].ToString(), para["selectFeature"].ToString());
            pWorkspaceEdit.StartEditing(true);
            pWorkspaceEdit.StartEditOperation();
            IFeature pFeature = pFeatureCursor.NextFeature();
            int currentIndex = 0;
            while (pFeature != null)
            {
                wait.SetProgressInfo(((double)currentIndex++ / (double)total).ToString("P"));
                var scmj = Convert.ToDouble(pFeature.get_Value(FindShapeAreIndex(pFeatureClass)).ToString()) / 666.6;
                pFeature.set_Value(pFeatureClass.FindField("SCMJ"), scmj);
                pFeature.Store();
                pFeature = pFeatureCursor.NextFeature();
            }
            Marshal.ReleaseComObject(pFeatureCursor);
            pWorkspaceEdit.StopEditOperation();
            pWorkspaceEdit.StopEditing(true);
            wait.CloseWait();
            para["ret"] = true;
        }
        #endregion

        #region 设置地块编码

        public bool SetDkbm(string perosnDatabase, string selectFeature)
        {
            if (MessageBox.MessageQuestion.Show("系统提示", "是否采用新的编码方式？") == true)
            {
                return SortDkbm(perosnDatabase, selectFeature);
            }
            else
            {
                return QueueDkbm(perosnDatabase, selectFeature);
            }

        }

        private bool QueueDkbm(string perosnDatabase, string selectFeature)
        {
            if (!Check.ValidCheck.FieldExistCheck(perosnDatabase, selectFeature, "DKBM"))
            {
                MessageBox.MessageWarning.Show("系统提示", "不存在地块编码字段");
                return false;
            }
            WinFbfbm winFbfbm = new WinFbfbm();
            if (winFbfbm.ShowDialog() == true)
            {
                Wait wait = new Wait();
                wait.SetInfoInvoke("正在对地块进行编码");
                wait.SetProgressInfo(string.Empty);
                Hashtable para = new Hashtable();
                para["wait"] = wait;
                para["personDatabase"] = perosnDatabase;
                para["selectFeature"] = selectFeature;
                para["fbfbm"] = winFbfbm.Fbfbm;
                para["ret"] = false;
                Thread t = new Thread(new ParameterizedThreadStart(QueueDkbm));
                t.Start(para);
                wait.ShowDialog();
                t.Abort();
                return (bool)para["ret"];
            }
            return false;
        }

        private void QueueDkbm(object p)
        {
            Hashtable para = p as Hashtable;
            var wait = para["wait"] as Wait;
            IAeFactory pAeFactory = new PersonalGeoDatabase(para["personDatabase"].ToString());
            IFeatureClass pFeatureClass = pAeFactory.OpenFeatureClasss(para["selectFeature"].ToString());
            var pDataset = pFeatureClass as IDataset;
            if (pDataset == null)
            {
                wait.CloseWait();
                para["ret"] = false;
                return;
            }
            var pWorkspaceEdit = pDataset.Workspace as IWorkspaceEdit;
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
            if (pWorkspaceEdit == null)
            {
                wait.CloseWait();
                para["ret"] = false;
                return;
            }
            int total = GetFeatureCount(para["personDatabase"].ToString(), para["selectFeature"].ToString());
            pWorkspaceEdit.StartEditing(true);
            pWorkspaceEdit.StartEditOperation();
            IFeature pFeature = pFeatureCursor.NextFeature();
            int currentIndex = 0;
            var fbfb = para["fbfbm"].ToString();
            while (pFeature != null)
            {
                wait.SetProgressInfo(((double)currentIndex++ / (double)total).ToString("P"));
                pFeature.set_Value(pFeatureClass.FindField("DKBM"), fbfb + currentIndex.ToString("00000"));
                pFeature.Store();
                pFeature = pFeatureCursor.NextFeature();
            }
            Marshal.ReleaseComObject(pFeatureCursor);
            pWorkspaceEdit.StopEditOperation();
            pWorkspaceEdit.StopEditing(true);
            wait.CloseWait();
            para["ret"] = true;
        }
        #endregion

    }
    /// <summary>
    /// 字段类型
    /// </summary>
    public class TdqqField
    {
        public esriFieldType FieldType { get; set; }
        public string FieldName { get; set; }
        public int Length { get; set; }

        public TdqqField(esriFieldType fieldType, string fieldName, int length)
        {
            FieldType = fieldType;
            FieldName = fieldName;
            Length = length;
        }
    }
    /// <summary>
    /// 为了地块编码而排序设计的实体，采用的是泛型设计
    /// </summary>
    /// <typeparam name="T">实体的类型</typeparam>
    public class SortEnity<T>
    {
        public T Id { get; set; }
        public double Xcor { get; set; }
        public double Ycor { get; set; }

        //public SortEnity(T id, double xcor, double ycor)
        //{
        //    Id = id;
        //    Xcor = xcor;
        //    Ycor = ycor;
        //}

    }
}
