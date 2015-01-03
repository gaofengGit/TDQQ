using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Collections;
using System.Windows;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Display;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using ESRI.ArcGIS.ADF.COMSupport;
using stdole;
using TDQQ.MessageBox;
using TDQQ.MyWindow;
using Application = System.Windows.Forms.Application;
using IFont = NPOI.SS.UserModel.IFont;
using IPicture = NPOI.SS.UserModel.IPicture;

namespace TDQQ.Export
{
    public class Tools4Jz
    {
        public static IFeatureWorkspace OpenWorkspace(string databaseUrl)
        {
            try
            {
                IWorkspaceName pWorkspaceName = new WorkspaceNameClass();
                pWorkspaceName.WorkspaceFactoryProgID = "esriDataSourcesGDB.AccessWorkspaceFactory";
                pWorkspaceName.PathName = databaseUrl;
                ESRI.ArcGIS.esriSystem.IName n = pWorkspaceName as ESRI.ArcGIS.esriSystem.IName;
                IFeatureWorkspace workspace = n.Open() as IFeatureWorkspace;

                return workspace;
            }
            catch (Exception ex)
            {
                // MessageBox.Show("打开工作空间失败!\n" + ex.Message, "空间工作空间", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                MessageWarning.Show("系统提示", "打开工作空间失败!\n");
            }
            return null;
        }
        public static bool CreateJZD(string databaseUrl, string input, string output)
        {
            string tmpDir = AppDomain.CurrentDomain.BaseDirectory + "\\TMP";
            if (!Directory.Exists(tmpDir))
                Directory.CreateDirectory(tmpDir);

            IFeatureWorkspace workspace = OpenWorkspace(databaseUrl);
            if (workspace != null)
            {
                try
                {
                    IFeatureClass inputFC = workspace.OpenFeatureClass(input);

                    // delIfExist(workspace, esriDatasetType.esriDTFeatureClass, output + "_T");

                    FeatureVerticesToPoints fvtp = new FeatureVerticesToPoints();
                    fvtp.in_features = inputFC;
                    fvtp.out_feature_class = tmpDir + "\\" + output + "_T.shp";
                    Geoprocessor GP = new Geoprocessor();
                    GP.OverwriteOutput = true;
                    GP.Execute(fvtp, null);

                    //                    IFeatureClass outFC = workspace.OpenFeatureClass(output + "_T");

                    //                    delIfExist(workspace, esriDatasetType.esriDTFeatureClass, output + "XY");
                    AddXY axy = new AddXY();
                    axy.in_features = tmpDir + "\\" + output + "_T.shp";

                    GP.Execute(axy, null);

                    //                  delIfExist(workspace, esriDatasetType.esriDTFeatureClass, output + "_T");
                    //                  IFeatureClass outFC = workspace.OpenFeatureClass(output + "_XY");

                    Dissolve dlv = new Dissolve();
                    delIfExist(workspace, esriDatasetType.esriDTFeatureClass, output);
                    dlv.dissolve_field = "POINT_X;POINT_Y";
                    dlv.multi_part = "SINGLE_PART";
                    dlv.in_features = tmpDir + "\\" + output + "_T.shp";
                    dlv.out_feature_class = databaseUrl + "\\" + output;
                    GP.Execute(dlv, null);
                    IFeatureClass outFC = workspace.OpenFeatureClass(output);
                    IFields fields = outFC.Fields;
                    int j = 0;
                    while (fields.FieldCount != j)
                    {
                        IField field = fields.get_Field(j);
                        if (field.Type != esriFieldType.esriFieldTypeOID && field.Type != esriFieldType.esriFieldTypeGeometry)
                        {
                            outFC.DeleteField(field);
                        }
                        else
                        {
                            j++;
                        }
                    }
                    var pField = new FieldClass();
                    var pFieldEdit = pField as IFieldEdit;
                    pFieldEdit.Name_2 = "BSM";
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                    pFieldEdit.Length_2 = 10;
                    outFC.AddField(pFieldEdit);

                    pField = new FieldClass();
                    pFieldEdit = pField as IFieldEdit;
                    pFieldEdit.Name_2 = "YSDM";
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                    pFieldEdit.Length_2 = 6;
                    outFC.AddField(pFieldEdit);

                    pField = new FieldClass();
                    pFieldEdit = pField as IFieldEdit;
                    pFieldEdit.Name_2 = "JZDH";
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                    pFieldEdit.Length_2 = 10;
                    outFC.AddField(pFieldEdit);

                    pField = new FieldClass();
                    pFieldEdit = pField as IFieldEdit;
                    pFieldEdit.Name_2 = "JZDLX";
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                    pFieldEdit.Length_2 = 1;
                    outFC.AddField(pFieldEdit);

                    pField = new FieldClass();
                    pFieldEdit = pField as IFieldEdit;
                    pFieldEdit.Name_2 = "JBLX";
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                    pFieldEdit.Length_2 = 1;
                    outFC.AddField(pFieldEdit);

                    IWorkspaceEdit workspaceEdit = workspace as IWorkspaceEdit;
                    workspaceEdit.StartEditing(false);
                    workspaceEdit.StartEditOperation();
                    IFeatureCursor featureCursor = outFC.Update(null, false);
                    IFeature feature = featureCursor.NextFeature();
                    while (feature != null)
                    {
                        feature.set_Value(2, feature.get_Value(0));
                        feature.set_Value(3, "211021");
                        feature.set_Value(4, feature.get_Value(0).ToString());
                        feature.set_Value(5, "3");
                        feature.set_Value(6, "9");

                        featureCursor.UpdateFeature(feature);

                        feature = featureCursor.NextFeature();
                    }
                    featureCursor.Flush();
                    workspaceEdit.StopEditOperation();
                    workspaceEdit.StopEditing(true);
                    return true;
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("生成界址点失败!\n" + ex.Message, "界址点生成", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                    //MessageWarning.Show("系统提示", "生成界址点失败!\n");
                }
            }
            return false;
        }
        public static bool CreateJZDAsyn(string databaseUrl, string input, string output)
        {
            //Hashtable param = new Hashtable();
            //Wait w = new Wait();
            //w.SetInfoInvoke("正在生成界址点......");
            //w.SetProgressInfo(string.Empty);
            //param["wait"] = w;
            //param["databaseUrl"] = databaseUrl;
            //param["input"] = input;
            //param["output"] = output;
            //param["ret"] = false;
            //Thread t = new Thread(new ParameterizedThreadStart(CreateJZDAsynF));
            //t.Start(param);
            //w.ShowDialog();        
            //bool ret = (bool)param["ret"];
            //return ret;
            return true;
        }
        public static bool CreateJZDAsynF(string databaseUrl, string input, string output)
        {
            //   Hashtable param = p as Hashtable;
            try
            {

                //string databaseUrl = param["databaseUrl"].ToString();
                //string input = param["input"].ToString();
                //string output = param["output"].ToString();


                //LicenseInitializer aoInit = new LicenseInitializer();
                //aoInit.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced }, new esriLicenseExtensionCode[]{ esriLicenseExtensionCode.esriLicenseExtensionCodeRuntimeAdvanced});

                return CreateJZD(databaseUrl, input, output);
                // param["ret"] = ret;
            }
            catch (Exception e)
            {
                //MessageHelper.InfoMessage(e.ToString());
                MessageWarning.Show("系统提示", e.ToString());
            }
            //  Wait w = param["wait"] as Wait;
            // w.Dispatcher.Invoke(new EventHandler(delegate
            // {
            //   w.Close();
            //   }));
            return false;
        }

        public static bool CreateJZX(string databaseUrl, string input, string output)
        {
            IFeatureWorkspace workspace = OpenWorkspace(databaseUrl);

            if (workspace != null)
            {
                try
                {

                    IFeatureClass inputFC = workspace.OpenFeatureClass(input);
                    string outputT = databaseUrl + "\\T_" + output;

                    delIfExist(workspace, esriDatasetType.esriDTFeatureClass, "T_" + output);

                    PolygonToLine ptl = new PolygonToLine();
                    ptl.in_features = inputFC;
                    ptl.out_feature_class = outputT;
                    Geoprocessor GP = new Geoprocessor();
                    GP.ResetEnvironments();
                    GP.OverwriteOutput = true;
                    GP.Execute(ptl, null);

                    delIfExist(workspace, esriDatasetType.esriDTFeatureClass, output);
                    SplitLine sl = new SplitLine();
                    sl.in_features = outputT;
                    sl.out_feature_class = databaseUrl + "\\" + output;
                    GP.ResetEnvironments();
                    GP.OverwriteOutput = true;
                    GP.Execute(sl, null);

                    delIfExist(workspace, esriDatasetType.esriDTFeatureClass, "T_" + output);
                    IFeatureClass outFC = workspace.OpenFeatureClass(output);

                    var pField = new FieldClass();
                    var pFieldEdit = pField as IFieldEdit;
                    pFieldEdit.Name_2 = "BSM";
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                    pFieldEdit.Length_2 = 10;
                    outFC.AddField(pFieldEdit);

                    pField = new FieldClass();
                    pFieldEdit = pField as IFieldEdit;
                    pFieldEdit.Name_2 = "YSDM";
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                    pFieldEdit.Length_2 = 6;
                    outFC.AddField(pFieldEdit);

                    pField = new FieldClass();
                    pFieldEdit = pField as IFieldEdit;
                    pFieldEdit.Name_2 = "JXXZ";
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                    pFieldEdit.Length_2 = 6;
                    outFC.AddField(pFieldEdit);

                    pField = new FieldClass();
                    pFieldEdit = pField as IFieldEdit;
                    pFieldEdit.Name_2 = "JZXLB";
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                    pFieldEdit.Length_2 = 6;
                    outFC.AddField(pFieldEdit);

                    pField = new FieldClass();
                    pFieldEdit = pField as IFieldEdit;
                    pFieldEdit.Name_2 = "JZXWZ";
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                    pFieldEdit.Length_2 = 1;
                    outFC.AddField(pFieldEdit);

                    pField = new FieldClass();
                    pFieldEdit = pField as IFieldEdit;
                    pFieldEdit.Name_2 = "JZXSM";
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                    pFieldEdit.Length_2 = 300;
                    outFC.AddField(pFieldEdit);

                    pField = new FieldClass();
                    pFieldEdit = pField as IFieldEdit;
                    pFieldEdit.Name_2 = "PLDWQLR";
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                    pFieldEdit.Length_2 = 100;
                    outFC.AddField(pFieldEdit);

                    pField = new FieldClass();
                    pFieldEdit = pField as IFieldEdit;
                    pFieldEdit.Name_2 = "PLDWZJR";
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                    pFieldEdit.Length_2 = 100;
                    outFC.AddField(pFieldEdit);

                    int leftFidIndex = outFC.FindField("LEFT_FID");
                    int rightFidIndex = outFC.FindField("RIGHT_FID");
                    int cbfmcIndex = inputFC.FindField("CBFMC");
                    int zjrIndex = inputFC.FindField("ZJRXM");
                    if (cbfmcIndex == -1 || zjrIndex == -1)
                    {
                        MessageWarning.Show("系统提示", "不存在CBFMC和ZJRXM字段");
                        return false;
                    }
                    IWorkspaceEdit workspaceEdit = workspace as IWorkspaceEdit;
                    workspaceEdit.StartEditing(false);
                    workspaceEdit.StartEditOperation();

                    IFeatureCursor featureCursor = outFC.Update(null, false);
                    IFeature feature = featureCursor.NextFeature();
                    IFeature featureTmp = null;
                    int left_fid = -1;
                    int right_fid = -1;
                    string left_pldwqlr = "";
                    string right_pldwqlr = "";
                    string left_pldwzjr = "";
                    string right_pldwzjr = "";
                    while (feature != null)
                    {
                        left_fid = (int)feature.get_Value(leftFidIndex);
                        right_fid = (int)feature.get_Value(rightFidIndex);
                        if (left_fid < 0)
                        {
                            left_pldwqlr = "";
                            left_pldwzjr = "";
                        }
                        else
                        {
                            featureTmp = inputFC.GetFeature(left_fid);
                            left_pldwqlr = featureTmp.get_Value(cbfmcIndex) as string;
                            left_pldwzjr = featureTmp.get_Value(zjrIndex) as string;
                        }
                        if (right_fid < 0)
                        {
                            right_pldwqlr = "";
                            right_pldwzjr = "";
                        }
                        else
                        {
                            featureTmp = inputFC.GetFeature(right_fid);
                            right_pldwqlr = featureTmp.get_Value(cbfmcIndex) as string;
                            right_pldwzjr = featureTmp.get_Value(zjrIndex) as string;
                        }


                        feature.set_Value(5, feature.OID);
                        feature.set_Value(6, "211031");
                        feature.set_Value(7, "600001");
                        if (left_fid < 0 || right_fid < 0)
                        {
                            feature.set_Value(8, "03");
                            feature.set_Value(9, "1");
                        }
                        else
                        {
                            feature.set_Value(8, "01");
                            feature.set_Value(9, "2");
                        }
                        feature.set_Value(10, "");

                        feature.set_Value(11, left_pldwqlr + "," + right_pldwqlr);
                        feature.set_Value(12, right_pldwzjr + "," + right_pldwzjr);

                        featureCursor.UpdateFeature(feature);
                        feature = featureCursor.NextFeature();
                    }
                    featureCursor.Flush();
                    workspaceEdit.StopEditOperation();
                    workspaceEdit.StopEditing(true);

                    outFC.DeleteField(outFC.Fields.get_Field(outFC.FindField("LEFT_FID")));
                    outFC.DeleteField(outFC.Fields.get_Field(outFC.FindField("RIGHT_FID")));
                    return true;
                }
                catch (Exception ex)
                {
                    //  MessageBox.Show("生成界址线失败!\n" + ex.Message, "界址线生成", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                    MessageWarning.Show("系统提示", ex.ToString());
                }
            }
            return false;

        }
        public static bool CreateJZXAsyn(string databaseUrl, string input, string output)
        {
            //Hashtable param = new Hashtable();
            //Wait w = new Wait();
            //w.SetInfoInvoke("正在生成界址线......");
            //w.SetProgressInfo(string.Empty);
            //param["wait"] = w;
            //param["databaseUrl"] = databaseUrl;
            //param["input"] = input;
            //param["output"] = output;
            //param["ret"] = false;
            //Thread t = new Thread(new ParameterizedThreadStart(CreateJZXAsynF));
            //t.Start(param);

            //w.ShowDialog();
            //bool ret = (bool)param["ret"];
            //return ret;
            return true;
        }
        public static bool CreateJZXAsynF(string databaseUrl, string input, string output)
        {
            //    Hashtable param = p as Hashtable;
            try
            {

                //string databaseUrl = param["databaseUrl"].ToString();
                //string input = param["input"].ToString();
                //string output = param["output"].ToString();


                //LicenseInitializer aoInit = new LicenseInitializer();
                //aoInit.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced }, new esriLicenseExtensionCode[]{ esriLicenseExtensionCode.esriLicenseExtensionCodeRuntimeAdvanced});

                return CreateJZX(databaseUrl, input, output);
                //   param["ret"] = ret;
                //aoInit.ShutdownApplication();
            }
            catch
            {
            }
            //  Wait w = param["wait"] as Wait;
            //w.Dispatcher.Invoke(new EventHandler(delegate
            //{
            //    w.Close();
            //}));
            return false;
        }

        public static bool CreateOneCTable(string databaseUrl, int fid, string outpath, string zdtname, string jzxname, string jzdname)
        {
            try
            {
                IFeatureWorkspace workspace = OpenWorkspace(databaseUrl);
                IFeatureClass zdt = workspace.OpenFeatureClass(zdtname);
                IFeatureClass jzx = workspace.OpenFeatureClass(jzxname);
                IFeatureClass jzd = workspace.OpenFeatureClass(jzdname);

                return CreateCTable(workspace, outpath, zdt.GetFeature(fid), jzx, jzd);
            }
            catch
            {
            }
            return false;
        }
        public static bool CreateOneCTableAsyn(string databaseUrl, int fid, string outpath, string zdtname, string jzxname, string jzdname)
        {
            Hashtable param = new Hashtable();
            param["databaseUrl"] = databaseUrl;
            param["fid"] = fid;
            param["outpath"] = outpath;
            param["zdtname"] = zdtname;
            param["jzxname"] = jzxname;
            param["jzdname"] = jzdname;
            param["ret"] = false;
            Wait w = new Wait();
            w.SetInfoInvoke("正在生成承包方调查表...");
            w.SetProgressInfo(string.Empty);
            param["wait"] = w;
            Thread t = new Thread(new ParameterizedThreadStart(CreateOneCTableAsynF));
            t.Start(param);

            w.ShowDialog();
            bool ret = (bool)param["ret"];
            return ret;
        }
        public static void CreateOneCTableAsynF(object p)
        {
            Hashtable param = p as Hashtable;
            string databaseUrl = param["databaseUrl"].ToString();
            int fid = (int)param["fid"];
            string outpath = param["outpath"].ToString();
            string zdtname = param["zdtname"].ToString();
            string jzxname = param["jzxname"].ToString();
            string jzdname = param["jzdname"].ToString();

            bool ret = CreateOneCTable(databaseUrl, fid, outpath, zdtname, jzxname, jzdname);
            param["ret"] = ret;

            Wait w = param["wait"] as Wait;
            w.Dispatcher.Invoke(new EventHandler(delegate
            {
                w.Close();
            }));
        }

        public static bool CreateMultCTable(string databaseUrl, int[] fids, string outdir, string zdtname, string jzxname, string jzdname)
        {
            try
            {
                IFeatureWorkspace workspace = OpenWorkspace(databaseUrl);
                IFeatureClass zdt = workspace.OpenFeatureClass(zdtname);
                IFeatureClass jzx = workspace.OpenFeatureClass(jzxname);
                IFeatureClass jzd = workspace.OpenFeatureClass(jzdname);

                if (fids == null)
                {
                    IFeatureCursor cursor = zdt.Search(null, false);
                    IFeature f = null;
                    List<int> fidList = new List<int>();
                    while ((f = cursor.NextFeature()) != null)
                    {
                        fidList.Add(f.OID);
                    }
                    fids = fidList.ToArray();
                }
                int cbfmcIndex = zdt.Fields.FindField("CBFMC");
                int dkbmIndex = zdt.Fields.FindField("DKBM");
                string name = "";
                IFeature feature = null;
                foreach (int i in fids)
                {
                    feature = zdt.GetFeature(i);
                    name = feature.get_Value(dkbmIndex).ToString() + "_" + feature.get_Value(cbfmcIndex) + ".xls";
                    name = System.IO.Path.Combine(outdir, name);
                    if (!CreateCTable(workspace, name, feature, jzx, jzd))
                    {
                        System.Windows.Forms.MessageBox.Show("生成C表错误，fid为" + i);
                        return false;
                    }
                }
                return true;
            }
            catch
            {
            }
            return false;
        }
        public static bool CreateMultCTableAsyn(string databaseUrl, int[] fids, string outdir, string zdtname, string jzxname, string jzdname)
        {
            Hashtable param = new Hashtable();
            param["databaseUrl"] = databaseUrl;
            param["fids"] = fids;
            param["outdir"] = outdir;
            param["zdtname"] = zdtname;
            param["jzxname"] = jzxname;
            param["jzdname"] = jzdname;
            param["ret"] = false;
            Wait w = new Wait();
            w.SetInfoInvoke("正在生成承包方调查表...");
            w.SetProgressInfo(string.Empty);
            param["wait"] = w;
            Thread t = new Thread(new ParameterizedThreadStart(CreateMultCTableAsynF));
            t.Start(param);

            w.ShowDialog();
            bool ret = (bool)param["ret"];
            return ret;
        }
        public static void CreateMultCTableAsynF(object P)
        {
            Hashtable param = P as Hashtable;
            string databaseUrl = param["databaseUrl"].ToString();
            int[] fids = (int[])param["fids"];
            string outdir = param["outdir"].ToString();
            string zdtname = param["zdtname"].ToString();
            string jzxname = param["jzxname"].ToString();
            string jzdname = param["jzdname"].ToString();

            bool ret = CreateMultCTable(databaseUrl, fids, outdir, zdtname, jzxname, jzdname);
            param["ret"] = ret;

            Wait w = param["wait"] as Wait;
            w.Dispatcher.Invoke(new EventHandler(delegate
            {
                w.Close();
            }));
        }

        public static bool CreateCTable(IFeatureWorkspace workspace, string outpath, IFeature zdtF, IFeatureClass jzx, IFeatureClass jzd)
        {

            Hashtable tdytHashTable = new Hashtable();
            tdytHashTable.Add("1", "种植业");
            tdytHashTable.Add("2", "林业");
            tdytHashTable.Add("3", "畜牧业");
            tdytHashTable.Add("4", "渔业");
            tdytHashTable.Add("5", "其他");

            Hashtable tdlylxHashTable = new Hashtable();
            tdlylxHashTable.Add("011", "水田");
            tdlylxHashTable.Add("012", "水浇地");
            tdlylxHashTable.Add("013", "旱地");

            Hashtable dldjHashTable = new Hashtable();
            dldjHashTable.Add("01", "一等地");
            dldjHashTable.Add("02", "二等地");
            dldjHashTable.Add("03", "三等地");
            dldjHashTable.Add("04", "四等地");
            dldjHashTable.Add("05", "五等地");
            dldjHashTable.Add("06", "六等地");
            dldjHashTable.Add("07", "七等地");

            Hashtable jblxHT = new Hashtable();
            jblxHT.Add("1", 1);
            jblxHT.Add("2", 2);
            jblxHT.Add("3", 3);
            jblxHT.Add("4", 4);
            jblxHT.Add("null", 5);

            Hashtable jzxlxHT = new Hashtable();
            jzxlxHT.Add("01", 6);
            jzxlxHT.Add("02", 7);
            jzxlxHT.Add("03", 8);
            jzxlxHT.Add("04", 9);
            jzxlxHT.Add("05", 10);
            jzxlxHT.Add("06", 11);
            jzxlxHT.Add("07", 12);
            jzxlxHT.Add("08", 13);
            jzxlxHT.Add("09", 14);

            Hashtable jzxwzHT = new Hashtable();
            jzxwzHT.Add("1", 15);
            jzxwzHT.Add("2", 16);
            jzxwzHT.Add("3", 17);



            string templateUrl = AppDomain.CurrentDomain.BaseDirectory + @"template\承包地块调查表.xls";


            int fbfbmIndex = zdtF.Fields.FindField("FBFBM");
            int cbfmcIndex = zdtF.Fields.FindField("CBFMC");
            int dkbhIndex = zdtF.Fields.FindField("DKBM");
            int dkmcIndex = zdtF.Fields.FindField("DKMC");
            int htmjIndex = zdtF.Fields.FindField("HTMJ");
            int dzIndex = zdtF.Fields.FindField("DKDZ");
            int xzIndex = zdtF.Fields.FindField("DKXZ");
            int nzIndex = zdtF.Fields.FindField("DKNZ");
            int bzIndex = zdtF.Fields.FindField("DKBZ");
            int tdytIndex = zdtF.Fields.FindField("TDYT");
            int tdlylxIndex = zdtF.Fields.FindField("TDLYLX");
            int dldjIndex = zdtF.Fields.FindField("DLDJ");
            int sfjbntIndex = zdtF.Fields.FindField("SFJBNT");
            int jzdlxIndex = jzd.Fields.FindField("JBLX");
            int jzdhIndex = jzd.Fields.FindField("JZDH");
            int jzxlbIndex = jzx.Fields.FindField("JZXLB");
            int jzxwzIndex = jzx.Fields.FindField("JZXWZ");
            int jzxsmIndex = jzx.Fields.FindField("JZXSM");
            int pldwqlrIndex = jzx.Fields.FindField("PLDWQLR");
            int pldwzjrIndex = jzx.Fields.FindField("PLDWZJR");

            // if (File.Exists(outpath)) File.Delete(outpath);
            File.Copy(templateUrl, outpath, true);
            using (System.IO.FileStream fileStream = new System.IO.FileStream(outpath, FileMode.Open, FileAccess.ReadWrite))
            {
                HSSFWorkbook workbookSource = new HSSFWorkbook(fileStream);
                HSSFSheet sheetSource = (HSSFSheet)workbookSource.GetSheetAt(0);
                //设定合并单元格的样式
                HSSFCellStyle style = (HSSFCellStyle)workbookSource.CreateCellStyle();
                style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
                style.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.CENTER;
                style.BorderBottom = NPOI.SS.UserModel.BorderStyle.THIN;
                style.BorderRight = NPOI.SS.UserModel.BorderStyle.THIN;
                style.BorderLeft = NPOI.SS.UserModel.BorderStyle.THIN;
                style.BorderTop = NPOI.SS.UserModel.BorderStyle.THIN;
                style.WrapText = true;

                string cbfmc = null;

                var tmprow = sheetSource.GetRow(2);
                var tmpcell = tmprow.GetCell(4);
                tmpcell.SetCellValue(zdtF.get_Value(fbfbmIndex).ToString());

                tmpcell = tmprow.GetCell(12);
                cbfmc = zdtF.get_Value(cbfmcIndex).ToString();
                tmpcell.SetCellValue(zdtF.get_Value(cbfmcIndex).ToString());

                tmprow = sheetSource.GetRow(3);
                tmpcell = tmprow.GetCell(4);
                tmpcell.SetCellValue(zdtF.get_Value(dkbhIndex).ToString());

                tmpcell = tmprow.GetCell(12);
                tmpcell.SetCellValue(zdtF.get_Value(dkmcIndex).ToString());

                tmpcell = tmprow.GetCell(20);
                tmpcell.SetCellValue(((double)zdtF.get_Value(htmjIndex)).ToString("f"));

                tmprow = sheetSource.GetRow(4);
                tmpcell = tmprow.GetCell(1);
                tmpcell.SetCellValue(zdtF.get_Value(dzIndex).ToString());

                tmpcell = tmprow.GetCell(9);
                tmpcell.SetCellValue(zdtF.get_Value(nzIndex).ToString());

                tmpcell = tmprow.GetCell(16);
                tmpcell.SetCellValue(zdtF.get_Value(xzIndex).ToString());

                tmpcell = tmprow.GetCell(20);
                tmpcell.SetCellValue(zdtF.get_Value(bzIndex).ToString());

                tmprow = sheetSource.GetRow(5);
                tmpcell = tmprow.GetCell(1);
                string tmpstr = tdytHashTable[zdtF.get_Value(tdytIndex)].ToString();
                IRichTextString rich = tmpcell.RichStringCellValue;
                IFont font = workbookSource.GetFontAt(rich.GetFontAtIndex(rich.String.Length - 1));
                tmpstr = rich.String.Replace("□" + tmpstr, "☑" + tmpstr);
                rich = new HSSFRichTextString(tmpstr);
                rich.ApplyFont(tmpstr.IndexOf('_'), tmpstr.LastIndexOf('_'), font);
                tmpcell.SetCellValue(rich);

                tmpcell = tmprow.GetCell(15);
                tmpstr = tdlylxHashTable[zdtF.get_Value(tdlylxIndex)].ToString();
                rich = tmpcell.RichStringCellValue;
                font = workbookSource.GetFontAt(rich.GetFontAtIndex(rich.String.Length - 1));
                tmpstr = rich.String.Replace("□" + tmpstr, "☑" + tmpstr);
                rich = new HSSFRichTextString(tmpstr);
                rich.ApplyFont(tmpstr.IndexOf('_'), tmpstr.LastIndexOf('_'), font);
                tmpcell.SetCellValue(rich);

                tmpcell = tmprow.GetCell(10);
                tmpstr = dldjHashTable[zdtF.get_Value(dldjIndex)].ToString();
                tmpcell.SetCellValue(tmpstr);

                tmpcell = tmprow.GetCell(21);
                tmpstr = zdtF.get_Value(sfjbntIndex).ToString();
                if ("2".CompareTo(tmpstr) != 0)
                {
                    tmpcell.SetCellValue("☑是");
                }
                else
                {
                    tmprow = sheetSource.GetRow(6);
                    tmpcell = tmprow.GetCell(21);
                    tmpcell.SetCellValue("☑否");
                }

                ISpatialFilter sf = new SpatialFilterClass();
                sf.Geometry = zdtF.ShapeCopy;
                sf.GeometryField = "SHAPE";
                sf.SpatialRel = esriSpatialRelEnum.esriSpatialRelRelation;
                sf.SpatialRelDescription = "F*TT*TF*T";
                var cursor = jzd.Search(sf, false);
                IFeature tmpFeature = null;
                List<IFeature> jzdList = new List<IFeature>();
                while ((tmpFeature = cursor.NextFeature()) != null)
                {
                    jzdList.Add(tmpFeature);
                }
                IGeometryCollection gc = zdtF.ShapeCopy as IGeometryCollection;
                if (gc.GeometryCount > 1)
                {
                    int a = gc.GeometryCount;
                }
                sf = new SpatialFilterClass();
                sf.GeometryField = "SHAPE";
                sf.Geometry = zdtF.ShapeCopy;
                sf.SpatialRel = esriSpatialRelEnum.esriSpatialRelRelation;
                sf.SpatialRelDescription = "FFTTT*FF*";

                cursor = jzx.Search(sf, false);
                tmpFeature = null;
                List<IFeature> jzxList = new List<IFeature>();

                while ((tmpFeature = cursor.NextFeature()) != null)
                {
                    jzxList.Add(tmpFeature);
                }

                List<IFeature> jzdSorted = new List<IFeature>();
                List<IFeature> jzxSorted = new List<IFeature>();

                if (jzdList.Count != jzxList.Count)
                {
                    System.Windows.Forms.MessageBox.Show("筛选出的界址点与界址线数量不同！");
                    return false;
                }

                int j = jzdList.Count;
                IFeature tmppoint = jzdList[0];
                IFeature tmpline = null;
                int gcCount = (zdtF.ShapeCopy as IGeometryCollection).GeometryCount - 1;
                while (jzxList.Count > 0)
                {
                    if (tmppoint == null)
                    {
                        if (gcCount > 0)
                        {
                            jzxSorted.Add(null);
                            jzdSorted.Add(null);
                            tmppoint = jzdList[0];
                            --gcCount;
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("筛选出的界址点与界址线错误！");
                            return false;
                        }
                    }

                    tmpline = getRelationLine(tmppoint.ShapeCopy, jzxList);
                    if (tmpline == null)
                    {

                        System.Windows.Forms.MessageBox.Show("筛选出的界址点与界址线错误！");
                        return false;
                    }

                    jzdSorted.Add(tmppoint);
                    jzdList.Remove(tmppoint);
                    jzxSorted.Add(tmpline);
                    jzxList.Remove(tmpline);

                    tmppoint = getRelationPoint(tmpline.ShapeCopy, jzdList);

                }

                if (jzxSorted.Count > 9)
                    createRows(sheetSource, 27, jzxSorted.Count - 9);
                int i = 10;
                int tmpInt = -1;
                bool first = true;
                foreach (IFeature f in jzdSorted)
                {
                    if (f != null)
                    {
                        tmprow = sheetSource.GetRow(i);
                        tmpcell = tmprow.GetCell(0);
                        tmpcell.SetCellValue(f.get_Value(jzdhIndex).ToString());

                        tmpstr = f.get_Value(jzdlxIndex).ToString();
                        if (jblxHT[tmpstr] == null)
                            tmpInt = (int)jblxHT["null"];
                        else
                            tmpInt = (int)jblxHT[tmpstr];

                        tmpcell = tmprow.GetCell(tmpInt);
                        tmpcell.SetCellValue("√");
                    }
                    if (first)
                    {
                        i += 1;
                        first = false;
                    }
                    else
                        i += 2;
                }

                i = 10;
                foreach (IFeature f in jzxSorted)
                {
                    if (f != null)
                    {
                        tmprow = sheetSource.GetRow(i);

                        tmpstr = f.get_Value(jzxlbIndex).ToString();
                        if (jzxlxHT[tmpstr] == null)
                            tmpInt = (int)jzxlxHT["null"];
                        else
                            tmpInt = (int)jzxlxHT[tmpstr];

                        tmpcell = tmprow.GetCell(tmpInt);
                        tmpcell.SetCellValue("√");

                        tmpstr = f.get_Value(jzxwzIndex).ToString();
                        if (jzxwzHT[tmpstr] == null)
                            tmpInt = (int)jzxwzHT["null"];
                        else
                            tmpInt = (int)jzxwzHT[tmpstr];

                        tmpcell = tmprow.GetCell(tmpInt);
                        tmpcell.SetCellValue("√");

                        tmpcell = tmprow.GetCell(18);
                        tmpcell.SetCellValue(f.get_Value(jzxsmIndex).ToString());

                        tmpcell = tmprow.GetCell(19);
                        tmpstr = f.get_Value(pldwqlrIndex).ToString();
                        string[] qlrArr = tmpstr.Split(',');
                        if (qlrArr[0].CompareTo(cbfmc) == 0)
                        {
                            tmpcell.SetCellValue(qlrArr[1]);
                            tmpcell = tmprow.GetCell(20);
                            tmpstr = f.get_Value(pldwzjrIndex).ToString();
                            qlrArr = tmpstr.Split(',');
                            tmpcell.SetCellValue(qlrArr[1]);
                            tmpcell = tmprow.GetCell(21);
                            tmpcell.SetCellValue(qlrArr[0]);
                        }
                        else
                        {
                            tmpcell.SetCellValue(qlrArr[0]);
                            tmpcell = tmprow.GetCell(20);
                            tmpstr = f.get_Value(pldwzjrIndex).ToString();
                            qlrArr = tmpstr.Split(',');
                            tmpcell.SetCellValue(qlrArr[0]);
                            tmpcell = tmprow.GetCell(21);
                            tmpcell.SetCellValue(qlrArr[1]);
                        }
                    }
                    i += 2;
                }

                System.IO.FileStream fs = new System.IO.FileStream(outpath, FileMode.Open, FileAccess.ReadWrite);
                workbookSource.Write(fs);
                fs.Close();
                return true;
            }
            return false;
        }

        public static IFeature getRelationLine(IGeometry query, List<IFeature> fList)
        {
            IRelationalOperator rel = query as IRelationalOperator;
            IGeometry tmpgeo = null;
            foreach (IFeature f in fList)
            {
                tmpgeo = f.ShapeCopy;
                if (rel.Touches(tmpgeo))
                {
                    return f;
                    break;
                }
            }
            return null;
        }

        public static IFeature getRelationPoint(IGeometry query, List<IFeature> fList)
        {
            IRelationalOperator rel = query as IRelationalOperator;
            foreach (IFeature f in fList)
            {
                if (rel.Touches(f.ShapeCopy))
                {
                    return f;
                    break;
                }
            }
            return null;
        }

        public static void createOneRow(HSSFSheet sourceSheet, int rownum)
        {
            var srcRow = sourceSheet.GetRow(rownum - 1);
            var newRow = sourceSheet.CreateRow(rownum);
            newRow.Height = srcRow.Height;

            for (int m = 0; m < 22; m++)
            {
                ICell cell = newRow.CreateCell(m);
                // cell.SetCellValue("0");
                ICellStyle cellStyle = srcRow.Cells[m].CellStyle;

                cell.CellStyle = cellStyle;
                cell.SetCellType(newRow.Cells[m].CellType);
            }
        }

        public static void createTwoRow(HSSFSheet sourceSheet, int rownum)
        {
            createOneRow(sourceSheet, rownum);
            createOneRow(sourceSheet, rownum + 1);
            for (int i = 0; i < 6; i++)
            {
                sourceSheet.AddMergedRegion(new CellRangeAddress(rownum, rownum + 1, i, i));
                sourceSheet.SetEnclosedBorderOfRegion(new CellRangeAddress(rownum, rownum + 1, i, i), NPOI.SS.UserModel.BorderStyle.THIN, NPOI.HSSF.Util.HSSFColor.BLACK.index);
            }
            for (int i = 6; i < 22; i++)
            {
                sourceSheet.AddMergedRegion(new CellRangeAddress(rownum - 1, rownum, i, i));
                sourceSheet.SetEnclosedBorderOfRegion(new CellRangeAddress(rownum - 1, rownum, i, i), NPOI.SS.UserModel.BorderStyle.THIN, NPOI.HSSF.Util.HSSFColor.BLACK.index);
                sourceSheet.SetEnclosedBorderOfRegion(new CellRangeAddress(rownum + 1, rownum + 1, i, i), NPOI.SS.UserModel.BorderStyle.THIN, NPOI.HSSF.Util.HSSFColor.BLACK.index);
            }


        }

        public static void createRows(HSSFSheet sourceSheet, int rownum, int count)
        {
            sourceSheet.ShiftRows(27, 34, count * 2, true, false, true);
            for (int i = 0; i < count; i++)
            {
                createTwoRow(sourceSheet, rownum + 2 * i);
            }
        }

        public static void delIfExist(IFeatureWorkspace workspace, esriDatasetType type, string name)
        {
            IEnumDataset dataset = (workspace as IWorkspace).get_Datasets(esriDatasetType.esriDTAny);
            IDataset tmp = null;

            while ((tmp = dataset.Next()) != null && tmp.Name != name) ;
            if (tmp != null)
                tmp.Delete();
        }

        /// <summary>
        /// 输出指定承包方的地块示意图
        /// </summary>
        /// <param name="map">输入的地图文档</param>
        /// <param name="cbfbm">承办方编码</param>
        /// <param name="xlsPath">xls文档路径，已将模版文档复制到此路径</param>
        /// <param name="bjxFC">边界线要素类</param>
        /// <param name="dbUrl">输出临时要素类的mdb数据库</param>
        /// <param name="zdtFC">宗地图要素类</param>
        /// <returns></returns>
        public static bool ExportOneSYT(IMap map, string cbfbm, IFeatureClass zdtFC, IFeatureClass bjxFC, string xlsPath, string dbUrl,
            string cbfmc, string familyNumber, int fieldNumber, double scmj, string address)
        {
            if (map == null || cbfbm == null || zdtFC == null || bjxFC == null)
                return false;
            IFeatureClass new_zdtFC = null;
            List<DkInfo> dkInfoList = null;
            IFeatureClass pointFC = null;
            if (!BuildNewFeatureClass(zdtFC, cbfbm, out new_zdtFC, out pointFC, out dkInfoList, dbUrl))
            {
                System.Windows.Forms.MessageBox.Show("创建新要素类失败！");
                return false;
            }
            if (!InsertPointClass(new_zdtFC, dkInfoList, pointFC))
            {
                System.Windows.Forms.MessageBox.Show("插入点要素类信息失败！");
                return false;
            }
            //重新布局要素类
            //if (!ReLayoutFeature(new_zdtFC, cbfbm))
            //{
            //    MessageBox.Show("重新布局要素类失败！");
            //    return false;
            //}
            if (!FixDataSource(map, new_zdtFC, bjxFC, pointFC))
            {
                return false;
            }

            if (!SetQuery(map, cbfbm))
            {
                return false;
            }

            if (!AddRoadTextElement(map, new_zdtFC, dkInfoList, pointFC))
            {
                return false;
            }

            IFeature feature = null;
            IFeatureCursor cursor = new_zdtFC.Search(null, false);
            IEnvelope envelope = null;
            while ((feature = cursor.NextFeature()) != null)
            {
                if (envelope == null)
                    envelope = feature.Shape.Envelope;
                else
                    envelope.Union(feature.Shape.Envelope);
            }

            IFeatureCursor bjxCursor = bjxFC.Search(null, false);
            IFeature tmpF = bjxCursor.NextFeature();

            ILayer zdtLayer = null;
            ILayer bjxLayer = null;
            ILayer pointLayer = null;
            ILayer layer = null;
            for (int i = 0; i < map.LayerCount; ++i)
            {
                layer = map.get_Layer(i);
                if (layer.Name == "zdt")
                {
                    zdtLayer = layer;
                }
                else if (layer.Name == "bjx")
                {
                    bjxLayer = layer;
                }
                else if (layer.Name == "point")
                {
                    pointLayer = layer;
                }
            }

            if (zdtLayer == null || bjxLayer == null || pointLayer == null)
            {
                System.Windows.Forms.MessageBox.Show("地图文档内容错误！");
                return false;
            }
            zdtLayer.Visible = true;
            bjxLayer.Visible = false;
            pointLayer.Visible = false;
            string syt = System.IO.Path.Combine(Application.StartupPath, "tmp\\syt.jpg");       //临时示意图路径
            if (File.Exists(syt))
                File.Delete(syt);
            if (!ExportImageToLocal(map as IActiveView, envelope, 600, 850, syt))
            {
                System.Windows.Forms.MessageBox.Show("导出示意图图片失败！");
                return false;
            }

            IGraphicsContainer gContainer = map as IGraphicsContainer;
            gContainer.DeleteAllElements();

            zdtLayer.Visible = false;
            bjxLayer.Visible = true;
            pointLayer.Visible = true;
            string slt = System.IO.Path.Combine(Application.StartupPath, "tmp\\slt.jpg");       //临时缩略图路径
            if (File.Exists(slt))
                File.Delete(slt);

            envelope.Union(tmpF.Shape.Envelope);
            if (!ExportImageToLocal(map as IActiveView, envelope, 150, 150, slt))
            {
                System.Windows.Forms.MessageBox.Show("导出缩略图图片失败！");
                return false;
            }
            #region 清除产生临时文件
            for (int i = 0; i < map.LayerCount; i++)
            {
                IGeoFeatureLayer tmp = map.get_Layer(i) as IGeoFeatureLayer;
                tmp.FeatureClass = null;
            }

            Marshal.FinalReleaseComObject(new_zdtFC);
            Marshal.FinalReleaseComObject(pointFC);
            new_zdtFC = null;
            pointFC = null;

            GC.WaitForPendingFinalizers();
            GC.Collect();


            #endregion

            #region 将图片存放到Excel中


            //byte[] sytByte = File.ReadAllBytes(syt);

            //byte[] sltByte = File.ReadAllBytes(slt);
            using (var fileStream = new System.IO.FileStream(xlsPath, FileMode.Open, FileAccess.ReadWrite))
            {
                IWorkbook workbook = new HSSFWorkbook(fileStream);
                AddPictureToExcle(syt, workbook, 10, 10, 0, 0, 0, 1, 7, 4, true, 0);
                AddPictureToExcle(slt, workbook, 10, 10, 0, 0, 6, 4, 8, 7, false, 0);
                ISheet sheet = workbook.GetSheetAt(0);
                NPOI.SS.UserModel.IRow row = null;
                row = sheet.GetRow(4);
                row.GetCell(1).SetCellValue(cbfbm.Substring(14, 4));
                row.GetCell(3).SetCellValue(cbfmc);
                row.GetCell(5).SetCellValue(familyNumber + "人");
                row = sheet.GetRow(5);
                row.GetCell(1).SetCellValue(fieldNumber.ToString() + "块");
                row.GetCell(4).SetCellValue(scmj.ToString("f") + "亩");
                row = sheet.GetRow(6);
                row.GetCell(0).SetCellValue("地址：" + address);
                System.IO.FileStream fs = new System.IO.FileStream(xlsPath, FileMode.Create, FileAccess.Write);
                workbook.Write(fs);
                fs.Close();
                fileStream.Close();
            }
            #endregion
            return true;
        }

        public static bool ExportOneSyt(IMap map, string cbfbm, IFeatureClass zdtFC, IFeatureClass bjxFC, string xlsPath, string dbUrl,
            string cbfmc, string familyNumber, int fieldNumber, double scmj, string address)
        {
            if (map == null || cbfbm == null || zdtFC == null || bjxFC == null)
                return false;
            IFeatureClass new_zdtFC = null;
            List<DkInfo> dkInfoList = null;
            IFeatureClass pointFC = null;
            if (!BuildNewFeatureClass(zdtFC, cbfbm, out new_zdtFC, out pointFC, out dkInfoList, dbUrl))
            {
                System.Windows.Forms.MessageBox.Show("创建新要素类失败！");
                return false;
            }
            if (!InsertPointClass(new_zdtFC, dkInfoList, pointFC))
            {
                System.Windows.Forms.MessageBox.Show("插入点要素类信息失败！");
                return false;
            }
            //重新布局要素类
            /*
            if (!ReLayoutFeature(new_zdtFC, cbfbm))
            {
                MessageBox.Show("重新布局要素类失败！");
                return false;
            }
             */

            if (!FixDataSource(map, new_zdtFC, bjxFC, pointFC))
            {
                return false;
            }

            if (!SetQuery(map, cbfbm))
            {
                return false;
            }

            if (!AddRoadTextElement(map, new_zdtFC, dkInfoList, pointFC))
            {
                return false;
            }
            IEnvelope envelope = null;
            IFeatureCursor bjxCursor = bjxFC.Search(null, false);
            IFeature tmpF = bjxCursor.NextFeature();
            ILayer zdtLayer = null;
            ILayer bjxLayer = null;
            ILayer pointLayer = null;
            ILayer layer = null;
            for (int i = 0; i < map.LayerCount; ++i)
            {
                layer = map.get_Layer(i);
                if (layer.Name == "zdt")
                {
                    zdtLayer = layer;
                }
                else if (layer.Name == "bjx")
                {
                    bjxLayer = layer;
                }
                else if (layer.Name == "point")
                {
                    pointLayer = layer;
                }
            }
            if (zdtLayer == null || bjxLayer == null || pointLayer == null)
            {
                System.Windows.Forms.MessageBox.Show("地图文档内容错误！");
                return false;
            }



            /////地块详细图
            zdtLayer.Visible = true;
            bjxLayer.Visible = false;
            pointLayer.Visible = false;
            //查询
            IQueryFilter query = new QueryFilterClass();
            query.WhereClause = "CBFBM = \"" + cbfbm + "\"";
            IFeatureCursor cursor = new_zdtFC.Search(query, false);
            IFeature feature;
            int count = 0;
            string syt;
            while ((feature = cursor.NextFeature()) != null)
            {
                //图片位置

                syt = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tmp\\dk" + count.ToString() + ".jpg");       //临时缩略图路径
                if (File.Exists(syt))
                    File.Delete(syt);
                envelope = feature.Shape.Envelope;
                ISpatialFilter pSpatialFilter = new SpatialFilterClass();
                pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelTouches;
                pSpatialFilter.Geometry = feature.Shape;
                IFeatureCursor mFeatureCursor = new_zdtFC.Search(pSpatialFilter, false);
                IFeature pFeature = mFeatureCursor.NextFeature();
                while (pFeature != null)
                {
                    envelope.Union(pFeature.Shape.Envelope);
                    pFeature = mFeatureCursor.NextFeature();
                }
                Marshal.ReleaseComObject(mFeatureCursor);
                if (!ExportImageToLocal(map as IActiveView, envelope, 320, 420, syt))
                {
                    System.Windows.Forms.MessageBox.Show("导出地块图片失败！");
                    return false;
                }
                count++;
            }
            Marshal.ReleaseComObject(cursor);
            IGraphicsContainer gContainer = map as IGraphicsContainer;
            gContainer.DeleteAllElements();

            //缩略图
            zdtLayer.Visible = false;
            bjxLayer.Visible = true;
            pointLayer.Visible = true;
            string slt = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tmp\\slt.jpg");       //临时缩略图路径
            if (File.Exists(slt))
                File.Delete(slt);
            envelope = tmpF.Shape.Envelope;
            if (!ExportImageToLocal(map as IActiveView, envelope, 150, 150, slt))
            {
                System.Windows.Forms.MessageBox.Show("导出缩略图图片失败！");
                return false;
            }
            #region 清除产生临时文件
            for (int i = 0; i < map.LayerCount; i++)
            {
                IGeoFeatureLayer tmp = map.get_Layer(i) as IGeoFeatureLayer;
                tmp.FeatureClass = null;
            }
            gContainer.DeleteAllElements();
            //清空数据
            Marshal.FinalReleaseComObject(new_zdtFC);
            Marshal.FinalReleaseComObject(pointFC);
            new_zdtFC = null;
            pointFC = null;

            GC.WaitForPendingFinalizers();
            GC.Collect();

            #endregion
            const int perCount = 4;
            //插入Excel其他信息和缩略图
            using (var fileStream = new System.IO.FileStream(xlsPath, FileMode.Open, FileAccess.ReadWrite))
            {
                IWorkbook workbook = new HSSFWorkbook(fileStream);
                ISheet sheet = workbook.GetSheetAt(0);
                AddPictureToExcle(slt, workbook, 20, 20, 0, 0, 7, 6, 9, 9, false, 0);
                //NPOI.SS.UserModel.IRow row = null;
                //row = sheet.GetRow(6);
                //row.GetCell(2).SetCellValue(cbfbm.Substring(14, 4));
                //row.GetCell(4).SetCellValue(cbfmc);
                //row.GetCell(6).SetCellValue(familyNumber + "人");
                //row = sheet.GetRow(7);
                //row.GetCell(2).SetCellValue(fieldNumber.ToString() + "块");
                //row.GetCell(4).SetCellValue(scmj.ToString("f") + "亩");
                //row.GetCell(5).SetCellValue("第1张");
                //row = sheet.GetRow(8);
                //row.GetCell(2).SetCellValue(address);
                //row.GetCell(5).SetCellValue("共" + Math.Ceiling((double)count / (double)perCount)+"张");
                System.IO.FileStream fs = new System.IO.FileStream(xlsPath, FileMode.Create, FileAccess.Write);
                workbook.Write(fs);
                fs.Close();
                fileStream.Close();
            }
            //每张sheet文件
            using (var fileStream = new System.IO.FileStream(xlsPath, FileMode.Open, FileAccess.ReadWrite))
            {
                IWorkbook workbook = new HSSFWorkbook(fileStream);
                

                for (int i = 0; i < count; i++)
                {
                    int sheetIndex = i / perCount;
                    //ISheet sheet = workbook.GetSheetAt(sheetIndex);
                    syt = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tmp\\dk" + i.ToString() + ".jpg");
                    int position = i % perCount;
                    int dx1, dy1, dx2, dy2, col1, row1, col2, row2;
                    dx1 = dy1 = 20;
                    dx2 = dy2 = 10;
                    col1 = 1;
                    row1 = 2;
                    col2 = 5;
                    row2 = 4;

                    if (position == 1)
                    {
                        dx1 = dy1 = 20;
                        dx2 = dy2 = 10;
                        col1 = 5;
                        row1 = 2;
                        col2 = 9;
                        row2 = 4;
                    }
                    if (position == 2)
                    {
                        dx1 = dy1 = 20;
                        dx2 = dy2 = 10;
                        col1 = 1;
                        row1 = 4;
                        col2 = 5;
                        row2 = 6;
                    }
                    if (position == 3)
                    {
                        dx1 = dy1 = 20;
                        dx2 = dy2 = 10;
                        col1 = 5;
                        row1 = 4;
                        col2 = 9;
                        row2 = 6;
                    }
                    AddPictureToExcle(syt, workbook, dx1, dy1, dx2, dy2, col1, row1, col2, row2, true, sheetIndex);
                    if (i%perCount==0)
                    {
                        AddPostScript(workbook, sheetIndex, cbfbm, cbfmc, familyNumber, fieldNumber, scmj, address,
                     sheetIndex + 1, (int)Math.Ceiling((double)count / (double)perCount));
                    }   
                }
                System.IO.FileStream fs = new System.IO.FileStream(xlsPath, FileMode.Create, FileAccess.Write);
                workbook.Write(fs);
                fs.Close();
                fileStream.Close();
            }
            return true;
        }

        private static void AddPostScript(IWorkbook workbook, int sheetIndex, string cbfbm, string cbfmc, string familyCount, int fieldCount, double scmj, string address,int currentPage,int totalPage)
        {
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            NPOI.SS.UserModel.IRow row = null;
            row = sheet.GetRow(6);
            row.GetCell(2).SetCellValue(cbfbm.Substring(14, 4));
            row.GetCell(4).SetCellValue(cbfmc);
            row.GetCell(6).SetCellValue(familyCount + "人");
            row = sheet.GetRow(7);
            row.GetCell(2).SetCellValue(fieldCount.ToString() + "块");
            row.GetCell(4).SetCellValue(scmj.ToString("f") + "亩");
            row.GetCell(5).SetCellValue("第"+currentPage+"张");
            row = sheet.GetRow(8);
            row.GetCell(2).SetCellValue(address);
            row.GetCell(5).SetCellValue("共" + totalPage + "张");
        }
        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="picPath">图片的地址</param>
        /// <param name="workbook">excel的workbook</param>
        /// <param name="dx1">dx1</param>
        /// <param name="dy1">dy1</param>
        /// <param name="dx2">dx2</param>
        /// <param name="dy2">dy2</param>
        /// <param name="col1">col1</param>
        /// <param name="row1">row1</param>
        /// <param name="col2">col2</param>
        /// <param name="row2">row2</param>
        /// <param name="isResize">是否恢复原大小</param>
        private static void AddPictureToExcle(string picPath, IWorkbook workbook,
            int dx1, int dy1, int dx2, int dy2, int col1, int row1, int col2, int row2, bool isResize, int pageIndex)
        {
            byte[] picByte = File.ReadAllBytes(picPath);
            int picInt = workbook.AddPicture(picByte, PictureType.JPEG);
            ISheet sheet = workbook.GetSheetAt(pageIndex);

            HSSFPatriarch patriarch = (HSSFPatriarch)sheet.CreateDrawingPatriarch();
            HSSFClientAnchor anchor = new HSSFClientAnchor(dx1, dy1, dx2, dy2, col1, row1, col2, row2);
            IPicture pic = patriarch.CreatePicture(anchor, picInt);
            if (isResize) pic.Resize();
            return;
        }
        /// <summary>
        /// 添加路标签到地图上
        /// </summary>
        /// <param name="map">地图文档接口</param>
        /// <param name="new_zdtFC">筛选后新建的宗地图类</param>
        /// <param name="dkInfoList">地块信息列表</param>
        /// <param name="pointFC">宗地图中心点要素类</param>
        /// <returns></returns>
        private static bool AddRoadTextElement(IMap map, IFeatureClass new_zdtFC, List<DkInfo> dkInfoList, IFeatureClass pointFC)
        {
            IGraphicsContainer graphContainer = map as IGraphicsContainer;
            ITextElement road = null;
            road = new TextElementClass();
            IFeature feature = null;
            IPoint point = null;
            IPolygon polygon = new PolygonClass();
            IPointCollection pointCollection = polygon as IPointCollection;

            foreach (DkInfo info in dkInfoList)
            {
                feature = new_zdtFC.GetFeature(info.dkid);

                if (info.dz == DkInfo.ROAD_ID)
                {
                    point = GetTextPoint(feature, 0);

                    road = CreateTextElement(point, esriTextHorizontalAlignment.esriTHALeft, esriTextVerticalAlignment.esriTVACenter);
                    graphContainer.AddElement(road as IElement, 0);
                }

                if (info.xz == DkInfo.ROAD_ID)
                {
                    point = GetTextPoint(feature, 1);
                    road = CreateTextElement(point, esriTextHorizontalAlignment.esriTHARight, esriTextVerticalAlignment.esriTVACenter);
                    graphContainer.AddElement(road as IElement, 0);
                }
                if (info.nz == DkInfo.ROAD_ID)
                {
                    point = GetTextPoint(feature, 2);
                    road = CreateTextElement(point, esriTextHorizontalAlignment.esriTHACenter, esriTextVerticalAlignment.esriTVATop);
                    graphContainer.AddElement(road as IElement, 0);
                }
                if (info.bz == DkInfo.ROAD_ID)
                {
                    point = GetTextPoint(feature, 3);
                    road = CreateTextElement(point, esriTextHorizontalAlignment.esriTHACenter, esriTextVerticalAlignment.esriTVABottom);
                    graphContainer.AddElement(road as IElement, 0);
                }
            }
            return true;
        }

        /// <summary>
        /// 根据方向和地块多边形确定标签位置
        /// </summary>
        /// <param name="target">地块多边形</param>
        /// <param name="dir">方向</param>
        /// <returns></returns>
        private static IPoint GetTextPoint(IFeature target, int dir)
        {
            IEnvelope queryEnvelop = target.Shape.Envelope;
            queryEnvelop.Expand(1.1, 1.3, true);
            IEnvelope envelope = target.Shape.Envelope;

            IPoint A = null;
            IPoint B = null;
            IPoint C = new PointClass();
            C.SpatialReference = envelope.SpatialReference;
            C.PutCoords((queryEnvelop.XMax + queryEnvelop.XMin) / 2, (queryEnvelop.YMax + queryEnvelop.YMin) / 2);

            switch (dir)
            {
                case 0:     //东至
                    A = queryEnvelop.UpperRight;
                    B = queryEnvelop.LowerRight;
                    break;
                case 1:     //西至
                    A = queryEnvelop.UpperLeft;
                    B = queryEnvelop.LowerLeft;
                    break;
                case 2:     //南至
                    A = queryEnvelop.LowerLeft;
                    B = queryEnvelop.LowerRight;
                    break;
                case 3:     //北至
                    A = queryEnvelop.UpperRight;
                    B = queryEnvelop.UpperLeft;
                    break;
                default:    //错误
                    return null;
            }
            ILine line = null;
            IRing ring = null;
            ISegmentCollection segColl = null;
            segColl = new RingClass();
            line = new LineClass();
            line.PutCoords(A, B);
            object missing = Type.Missing;
            segColl.AddSegment(line as ISegment, ref missing, ref missing);
            line = new LineClass();
            line.PutCoords(B, C);
            segColl.AddSegment(line as ISegment, ref missing, ref missing);
            ring = segColl as IRing;
            ring.Close();
            IGeometryCollection polygon;
            polygon = new PolygonClass();
            polygon.AddGeometry(ring, ref missing, ref missing);

            IRelationalOperator relate = polygon as IRelationalOperator;

            IPointCollection pointColl = target.ShapeCopy as IPointCollection;

            double avg = 0;
            double cnt = 0;
            for (int i = 0; i < pointColl.PointCount; ++i)
            {
                IPoint tmp = pointColl.get_Point(i);
                if (relate.Contains(pointColl.get_Point(i)))
                {
                    switch (dir)
                    {
                        case 0:
                        case 1:
                            avg += pointColl.get_Point(i).Y;
                            ++cnt;
                            break;
                        case 2:
                        case 3:
                            avg += pointColl.get_Point(i).X;
                            ++cnt;
                            break;
                        default:
                            return null;
                    }
                }
            }
            IPoint ret = new PointClass();
            ret.SpatialReference = envelope.SpatialReference;
            if (cnt == 0)
            {
                if (dir == 0 || dir == 1)
                    avg = C.Y;
                if (dir == 2 || dir == 3)
                    avg = C.X;
            }
            else
                avg /= cnt;
            switch (dir)
            {
                case 0:
                    ret.PutCoords(envelope.XMax, avg);
                    break;
                case 1:
                    ret.PutCoords(envelope.XMin, avg);
                    break;
                case 2:
                    ret.PutCoords(avg, envelope.YMin);
                    break;
                case 3:
                    ret.PutCoords(avg, envelope.YMax);
                    break;
                default:
                    return null;
            }

            return ret;
        }

        /// <summary>
        /// 创建Text元素，用来标识路信息
        /// </summary>
        /// <param name="point">放置Text的位置点</param>
        /// <param name="h">水平方向上对齐方式</param>
        /// <param name="v">垂直方向上对齐方式</param>
        /// <returns></returns>
        private static ITextElement CreateTextElement(IPoint point, esriTextHorizontalAlignment h, esriTextVerticalAlignment v)
        {
            ITextElement element = new TextElementClass();

            System.Drawing.Font font = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold);
            ITextSymbol textSymbol = new TextSymbolClass();
            textSymbol.Size = 2;
            IRgbColor color = new RgbColorClass();
            color.RGB = 0;
            textSymbol.Color = color;
            textSymbol.Font = (stdole.IFontDisp)OLE.GetIFontDispFromFont(font);
            textSymbol.HorizontalAlignment = h;
            textSymbol.VerticalAlignment = v;
            textSymbol.Text = "路";
            element.ScaleText = true;
            element.Symbol = textSymbol;
            element.Text = "路";

            IElement geoElement = element as IElement;
            geoElement.Geometry = point;

            return element;
        }
        /// <summary>  
        /// 根据传入的源要素类OldFeatureClass,新空间范围,要素存储工作空间,新要素类名  
        /// 产生具有相同字段结构和不同空间范围的要素类  
        /// </summary>  
        /// <param name="OldFeatureClass">源要素类</param>  
        /// <param name="SaveFeatWorkspace">存储工作空间</param>  
        /// <param name="FeatClsName">新要素类名</param>  
        /// <param name="pDomainEnv">新空间范围,可为null</param>  
        /// <returns></returns>  
        private static IFeatureClass CloneFeatureClassInWorkspace(IFeatureClass OldFeatureClass, IFeatureWorkspace SaveFeatWorkspace, string FeatClsName, IEnvelope pDomainEnv)
        {
            IFields pFields = CloneFeatureClassFields(OldFeatureClass, null);
            //delIfExist(SaveFeatWorkspace, esriDatasetType.esriDTFeatureClass,FeatClsName);

            var pfeaureclass = SaveFeatWorkspace.CreateFeatureClass(FeatClsName, pFields, null, null, esriFeatureType.esriFTSimple, OldFeatureClass.ShapeFieldName, "");
            // System.Runtime.InteropServices.Marshal.ReleaseComObject(SaveFeatWorkspace);
            return pfeaureclass;
        }

        /// <summary>
        /// 复制要素类字段
        /// </summary>
        /// <param name="pFeatureClass"></param>
        /// <param name="pDomainEnv"></param>
        /// <returns></returns>
        private static IFields CloneFeatureClassFields(IFeatureClass pFeatureClass, IEnvelope pDomainEnv)
        {
            IFields pFields = new FieldsClass();
            IFieldsEdit pFieldsEdit = (IFieldsEdit)pFields;
            //根据传入的要素类,将除了shape字段之外的字段复制  
            long nOldFieldsCount = pFeatureClass.Fields.FieldCount;
            long nOldGeoIndex = pFeatureClass.Fields.FindField(pFeatureClass.ShapeFieldName);
            for (int i = 0; i < nOldFieldsCount; i++)
            {
                if (i != nOldGeoIndex)
                {
                    pFieldsEdit.AddField(pFeatureClass.Fields.get_Field(i));
                }
                else
                {
                    IGeometryDef pGeomDef = new GeometryDefClass();
                    IGeometryDefEdit pGeomDefEdit = (IGeometryDefEdit)pGeomDef;
                    ISpatialReference pSR = null;
                    if (pDomainEnv != null)
                    {
                        pSR = new UnknownCoordinateSystemClass();
                        pSR.SetDomain(pDomainEnv.XMin, pDomainEnv.XMax, pDomainEnv.YMin, pDomainEnv.YMax);
                    }
                    else
                    {
                        IGeoDataset pGeoDataset = pFeatureClass as IGeoDataset;
                        pSR = pGeoDataset.SpatialReference;
                    }
                    //设置新要素类Geometry的参数  
                    pGeomDefEdit.GeometryType_2 = pFeatureClass.ShapeType;
                    pGeomDefEdit.GridCount_2 = 1;
                    pGeomDefEdit.set_GridSize(0, 10);
                    pGeomDefEdit.AvgNumPoints_2 = 2;
                    pGeomDefEdit.SpatialReference_2 = pSR;
                    //产生新的shape字段  
                    IField pField = new FieldClass();
                    IFieldEdit pFieldEdit = (IFieldEdit)pField;
                    pFieldEdit.Name_2 = pFeatureClass.Fields.get_Field(i).Name;
                    pFieldEdit.AliasName_2 = pFeatureClass.Fields.get_Field(i).AliasName;
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
                    pFieldEdit.GeometryDef_2 = pGeomDef;
                    pFieldsEdit.AddField(pField);
                }
            }
            return pFields;
        }

        /// <summary>
        /// 将地块中心点插入到新的要素类里去
        /// </summary>
        /// <param name="zdt">宗地图要素类</param>
        /// <param name="dkInfoList">地块信息列表，用于筛选地块</param>
        /// <param name="pointFC">新的点要素类</param>
        /// <returns></returns>
        private static bool InsertPointClass(IFeatureClass zdt, List<DkInfo> dkInfoList, IFeatureClass pointFC)
        {
            IFeature feature = null;
            IFeatureCursor pointCursor = pointFC.Insert(true);
            IFeatureBuffer pointBuf = null;
            IPoint point = null;
            try
            {
                foreach (DkInfo info in dkInfoList)
                {
                    feature = zdt.GetFeature(info.dkid);
                    point = new PointClass();
                    point.SpatialReference = feature.ShapeCopy.SpatialReference;
                    point.X = (feature.Shape.Envelope.XMax + feature.Shape.Envelope.XMin) / 2;
                    point.Y = (feature.Shape.Envelope.YMax + feature.Shape.Envelope.YMin) / 2;
                    pointBuf = pointFC.CreateFeatureBuffer();
                    pointBuf.Shape = point;

                    pointCursor.InsertFeature(pointBuf);
                }

                pointCursor.Flush();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据承包方编码，来创建只包含承包方地块要素和与承包方地块相邻地块的要素的要素类
        /// </summary>
        /// <param name="zdt">宗地图要素类</param>
        /// <param name="cbfbm">承包方编码</param>
        /// <param name="dkInfoList">(out)地块信息列表</param>
        /// <param name="dkPointFC">(out)输出的地块点要素</param>
        /// <param name="outFC">(out)输出的地块要素</param>
        /// <param name="dbUrl">输出临时要素类的mdb数据库</param>
        /// <returns></returns>
        private static bool BuildNewFeatureClass(IFeatureClass zdt, string cbfbm, out IFeatureClass outFC, out IFeatureClass dkPointFC, out List<DkInfo> dkInfoList, string dbUrl)
        {
            outFC = null;
            dkInfoList = null;
            dkPointFC = null;
            if (zdt == null || cbfbm == null || dbUrl == null)
                return false;

            dkInfoList = new List<DkInfo>();
            IQueryFilter query = new QueryFilterClass();
            query.WhereClause = "CBFBM = \"" + cbfbm + "\"";
            IFeatureCursor cursor = zdt.Search(query, false);
            IFeature feature = null;
            bool ret = true;
            DkInfo dkInfo = null;
            int dzIndex = zdt.Fields.FindField("DKDZ");
            int xzIndex = zdt.Fields.FindField("DKXZ");
            int nzIndex = zdt.Fields.FindField("DKNZ");
            int bzIndex = zdt.Fields.FindField("DKBZ");
            int cbfmcIndex = zdt.Fields.FindField("CBFMC");

            ISpatialFilter spatialQuery = new SpatialFilterClass();
            spatialQuery.GeometryField = zdt.ShapeFieldName;
            spatialQuery.SpatialRel = esriSpatialRelEnum.esriSpatialRelTouches;

            string dkdz = null;
            string dkxz = null;
            string dknz = null;
            string dkbz = null;
            string tmpCbfmc = null;

            while ((feature = cursor.NextFeature()) != null)
            {
                spatialQuery.Geometry = feature.ShapeCopy;
                dkInfo = new DkInfo();
                dkInfo.dkid = feature.OID;
                dkdz = feature.get_Value(dzIndex).ToString().Trim();
                dkxz = feature.get_Value(xzIndex).ToString().Trim();
                dknz = feature.get_Value(nzIndex).ToString().Trim();
                dkbz = feature.get_Value(bzIndex).ToString().Trim();

                IFeatureCursor scursor = zdt.Search(spatialQuery, false);
                IFeature tmp = null;
                int tmpIndex = -1;
                while ((tmp = scursor.NextFeature()) != null)
                {
                    tmpCbfmc = tmp.get_Value(cbfmcIndex).ToString().Trim();
                    tmpIndex = GetDirection(feature.Shape.Envelope, tmp.Shape.Envelope);
                    if (tmpCbfmc == dkdz && tmpIndex == 0)      //东至
                    {
                        dkInfo.dz = tmp.OID;
                    }
                    else if (tmpCbfmc == dkxz && tmpIndex == 1)  //西至
                    {
                        dkInfo.xz = tmp.OID;
                    }
                    else if (tmpCbfmc == dknz && tmpIndex == 2)   //南至
                    {
                        dkInfo.nz = tmp.OID;
                    }
                    else if (tmpCbfmc == dkbz && tmpIndex == 3)   //北至
                    {
                        dkInfo.bz = tmp.OID;
                    }
                }
                bool valid = true;
                if (dkdz != "路" && dkInfo.dz == DkInfo.ROAD_ID)
                {
                    valid = false;
                }
                else if (dkxz != "路" && dkInfo.xz == DkInfo.ROAD_ID)
                {
                    valid = false;
                }
                else if (dknz != "路" && dkInfo.nz == DkInfo.ROAD_ID)
                {
                    valid = false;
                }
                else if (dkbz != "路" && dkInfo.bz == DkInfo.ROAD_ID)
                {
                    valid = false;
                }

                if (!valid)
                {
                    //System.Windows.Forms.MessageBox.Show("重新匹配东西南北至错误！");
                }
                dkInfoList.Add(dkInfo);
            }

            HashSet<int> idSet = new HashSet<int>();
            foreach (DkInfo info in dkInfoList)
            {
                idSet.Add(info.dkid);
                idSet.Add(info.dz);
                idSet.Add(info.nz);
                idSet.Add(info.xz);
                idSet.Add(info.bz);
            }
            idSet.Remove(-1);

            IFeatureCursor tmpCursor = zdt.GetFeatures(idSet.ToArray(), false);
            //IWorkspaceFactory workspaceFactory = new InMemoryWorkspaceFactoryClass();
            //ESRI.ArcGIS.Geodatabase.IWorkspaceName workspaceName = workspaceFactory.Create("", "MyWorkspace", null, 0);
            //ESRI.ArcGIS.esriSystem.IName name = (ESRI.ArcGIS.esriSystem.IName)workspaceName;
            //ESRI.ArcGIS.Geodatabase.IFeatureWorkspace inmemWor = (IFeatureWorkspace)name.Open();
            //string tmpwork = System.IO.Path.Combine(Application.StartupPath,"tmp");
            //IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace inmemWor = OpenWorkspace(dbUrl);
            delIfExist(inmemWor, esriDatasetType.esriDTFeatureClass, "tmp_zdt");
            delIfExist(inmemWor, esriDatasetType.esriDTFeatureClass, "point");

            outFC = CloneFeatureClassInWorkspace(zdt, inmemWor, "tmp_zdt", null);

            IFieldsEdit fieldsEdit = new FieldsClass() as IFieldsEdit;
            IGeometryDef geometryDef = new GeometryDefClass();
            IGeometryDefEdit geometryDefEdit = geometryDef as IGeometryDefEdit;
            geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
            geometryDefEdit.GridCount_2 = 1;
            geometryDefEdit.set_GridSize(0, 10);
            geometryDefEdit.SpatialReference_2 = (zdt as IGeoDataset).SpatialReference;

            IField fieldShape = new FieldClass();
            IFieldEdit fieldShapeEdit = fieldShape as IFieldEdit;
            fieldShapeEdit.Name_2 = "SHAPE";
            fieldShapeEdit.AliasName_2 = "SHAPE";
            fieldShapeEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            fieldShapeEdit.GeometryDef_2 = geometryDef;
            fieldsEdit.AddField(fieldShape);

            IField fieldOID = new FieldClass();
            IFieldEdit fieldOIDEdit = fieldOID as IFieldEdit;
            fieldOIDEdit.Name_2 = "OBJECTID";
            fieldOIDEdit.AliasName_2 = "OBJECTID";
            fieldOIDEdit.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldsEdit.AddField(fieldOID);

            dkPointFC = inmemWor.CreateFeatureClass("point", fieldsEdit, null, null, esriFeatureType.esriFTSimple, "SHAPE", "");


            IFeatureCursor outCursor = outFC.Insert(true);

            IFeature temp = null;
            IFeatureBuffer buffer = null;

            IFields fields = outFC.Fields;
            Hashtable hashmap = new Hashtable();
            hashmap.Add(-1, -1);
            int oIndex = outFC.FindField(outFC.OIDFieldName);
            int sIndex = outFC.FindField(outFC.ShapeFieldName);
            int tmpOid = -1;

            Hashtable indexMap = new Hashtable();
            for (int i = 0; i < outFC.Fields.FieldCount; ++i)
            {
                indexMap.Add(i, zdt.FindField(outFC.Fields.get_Field(i).Name));
            }
            try
            {
                while ((temp = tmpCursor.NextFeature()) != null)
                {
                    buffer = outFC.CreateFeatureBuffer();

                    for (int i = 0; i < fields.FieldCount; ++i)
                    {
                        if (i == oIndex)
                            continue;
                        if (i == sIndex)
                        {
                            buffer.Shape = temp.ShapeCopy;
                            continue;
                        }
                        try
                        {
                            if (fields.get_Field(i).Editable)
                            {
                                buffer.set_Value(i, temp.get_Value((int)indexMap[i]));
                            }
                        }
                        catch
                        {
                        }
                    }
                    tmpOid = (int)outCursor.InsertFeature(buffer);
                    hashmap.Add(temp.OID, tmpOid);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return false;
            }
            outCursor.Flush();

            foreach (DkInfo info in dkInfoList)
            {
                info.dkid = (int)hashmap[info.dkid];
                info.dz = (int)hashmap[info.dz];
                info.xz = (int)hashmap[info.xz];
                info.nz = (int)hashmap[info.nz];
                info.bz = (int)hashmap[info.bz];
            }
            //System.Runtime.InteropServices.Marshal.ReleaseComObject(inmemWor);
            return ret;
        }

        /// <summary>
        /// 修复地图文档中图层的数据源
        /// </summary>
        /// <param name="map">要修复的地图</param>
        /// <param name="zdtFC">宗地图要素类</param>
        /// <param name="bjxFC">边界线要素类</param>
        /// <param name="pointFC">地块点要素类</param>
        /// <returns></returns>
        private static bool FixDataSource(IMap map, IFeatureClass zdtFC, IFeatureClass bjxFC, IFeatureClass pointFC)
        {
            if (map == null || zdtFC == null || bjxFC == null)
                return false;
            ILayer layer = null;
            bool zdt = false, bjx = false, p = false;
            for (int i = 0; i < map.LayerCount; ++i)
            {
                layer = map.get_Layer(i);
                if (layer.Name == "zdt")
                {
                    (layer as IGeoFeatureLayer).FeatureClass = zdtFC;
                    zdt = true;
                }
                else if (layer.Name == "bjx")
                {
                    (layer as IGeoFeatureLayer).FeatureClass = bjxFC;
                    bjx = true;
                }
                else if (layer.Name == "point")
                {
                    (layer as IGeoFeatureLayer).FeatureClass = pointFC;
                    p = true;
                }
                if (bjx && zdt && p)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 设置要素显示的方式和要素注记的显示方式
        /// </summary>
        /// <param name="map">要修复的地图</param>
        /// <param name="cbfbm">承办方编码</param>
        /// <returns></returns>
        private static bool SetQuery(IMap map, String cbfbm)
        {
            ILayer layer = null;
            IGeoFeatureLayer geoLayer = null;
            for (int i = 0; i < map.LayerCount; ++i)
            {
                layer = map.get_Layer(i);
                if (layer.Name == "zdt")
                {
                    geoLayer = layer as IGeoFeatureLayer;
                    break;
                }
            }

            if (geoLayer == null)
            {
                System.Windows.Forms.MessageBox.Show("未找到宗地图图层！");
                return false;
            }

            IUniqueValueRenderer render = geoLayer.Renderer as IUniqueValueRenderer;

            if (render == null || render.ValueCount != 1)
            {
                System.Windows.Forms.MessageBox.Show("地图文档不正确！");
                return false;
            }
            render.set_Value(0, cbfbm);

            IAnnotateLayerPropertiesCollection IPALPColl = geoLayer.AnnotationProperties;
            IAnnotateLayerProperties layerProp = null;
            IElementCollection element = null;

            if (IPALPColl == null || IPALPColl.Count != 2)
            {
                System.Windows.Forms.MessageBox.Show("地图文档不正确！");
                return false;
            }

            bool d = false, cbf = false;
            for (int i = 0; i < IPALPColl.Count; ++i)
            {
                IPALPColl.QueryItem(i, out layerProp, out element, out element);
                if (layerProp.Class == "Default")
                {
                    layerProp.WhereClause = "[CBFBM] <> \"" + cbfbm + "\"";
                    d = true;
                }
                else if (layerProp.Class == "CBF")
                {
                    layerProp.WhereClause = "[CBFBM] = \"" + cbfbm + "\"";
                    cbf = true;
                }
                if (cbf && d)
                    break;
            }

            if (!(cbf && d))
            {
                System.Windows.Forms.MessageBox.Show("地图文档不正确！");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 输出图片到本地文件
        /// </summary>
        /// <param name="pActiveView"></param>
        /// <param name="envelop">输出的地理位置</param>
        /// <param name="imgWidth">图片宽度</param>
        /// <param name="imgHeight">图片高度</param>
        /// <param name="imagepath">图片路径</param>
        /// <returns></returns>
        public static bool ExportImageToLocal(IActiveView pActiveView, IEnvelope envelop, int imgWidth, int imgHeight, string imagepath)
        {
            IEnvelope pEnvelope = new EnvelopeClass();
            ITrackCancel pTrackCancel = new CancelTrackerClass();

            tagRECT ptagRECT = new tagRECT();// pActiveView.ExportFrame;
            ptagRECT.left = 0;
            ptagRECT.top = 0;
            ptagRECT.right = imgWidth;// (int)pActiveView.Extent.Width;
            ptagRECT.bottom = imgHeight;// (int)pActiveView.Extent.Height;

            int pResolution = (int)(pActiveView.ScreenDisplay.DisplayTransformation.Resolution);
            if (pResolution == 0)
            {
                pResolution = 96;
            }
            pEnvelope.PutCoords(ptagRECT.left, ptagRECT.bottom, ptagRECT.right, ptagRECT.top);

            IEnvelope newEnvelope = envelop.Envelope;

            newEnvelope.Expand(newEnvelope.Width * 0.025, newEnvelope.Height * 0.025, false);

            ExportJPEGClass bitmap = new ExportJPEGClass();
            bitmap.Resolution = pResolution;
            bitmap.ExportFileName = imagepath;
            bitmap.PixelBounds = pEnvelope;

            pActiveView.Output(bitmap.StartExporting(), pResolution, ref ptagRECT, newEnvelope, pTrackCancel);
            bitmap.FinishExporting();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(bitmap);

            return true;
        }
        /// <summary>
        /// 重新分布要素类里的要素，要求保存要素的相对位置不变，相接的要素仍然相接，减少要素
        /// 之间的空白区域
        /// </summary>
        /// <param name="fc">输入的要素类</param>
        /// <param name="cbfbm">该地块的承包方编码</param>
        /// <returns>
        /// 重新分布要素位置成功返回true
        /// 重新分布要素位置失败返回false
        /// </returns>
        //private static bool ReLayoutFeature(IFeatureClass fc, string cbfbm)
        //{

        //    /*
        //   为了保证该地块邻接的地块在平移的过程拓扑关系不变，需要将与该地块
        //   相邻的要素作为一个整体进行平移。平移过程是以某个大地块的Envelope作为
        //   一个参考对象，接下来的大地块的Envelope与参考的Envelope的方位，来确定整体的
        //   的平移向量，平移后，参考Envelope将和平移过来的envelope进行合并，作为新的参考Envelope
        //   */


        //    //获取工作空间
        //    IDataset pDataset = fc as IDataset;
        //    IWorkspace pworkSpace = pDataset.Workspace;
        //    IWorkspaceEdit pWorkspaceEdit = (IWorkspaceEdit)pworkSpace;
        //    //对当前农户的地块进行筛选，以承包方编码为关键词
        //    IQueryFilter query = new QueryFilterClass();
        //    query.WhereClause = "CBFBM = \"" + cbfbm + "\"";
        //    IFeatureCursor cursor = fc.Update(query, false);
        //    IEnvelope pEnvelopeReference = null;//参考envelope

        //    /*
        //    * 获取该农户的第一个田块，由此获得第一个参考Envelope；
        //    */
        //    IFeature feature = cursor.NextFeature();
        //    if (feature == null) return false;
        //    //空间查询条件
        //    ISpatialFilter spatial = new SpatialFilterClass();
        //    spatial.Geometry = feature.Shape;
        //    spatial.SpatialRel = esriSpatialRelEnum.esriSpatialRelTouches;
        //    IFeatureCursor pCursor = fc.Update(spatial, false);
        //    IFeature pFeature = pCursor.NextFeature();
        //    pEnvelopeReference = feature.Shape.Envelope;//查询的对象作为参考Envelope
        //    //获取与其接触的，获取一个最大的Envelope
        //    while (pFeature != null)
        //    {
        //        pEnvelopeReference.Union(pFeature.Shape.Envelope);
        //        pFeature = pCursor.NextFeature();
        //    }

        //    Marshal.ReleaseComObject(pCursor);
        //    //开始编辑
        //    pWorkspaceEdit.StartEditing(true);
        //    pWorkspaceEdit.StartEditOperation();
        //    //查找下一个该农户的田块
        //    feature = cursor.NextFeature();
        //    while (feature != null)
        //    {
        //        // MessageBox.Show("原先位置：" +"\n"+ GetPosition(feature.Shape.Envelope));
        //        IEnvelope pEnvelopeMove;
        //        spatial.Geometry = feature.Shape;
        //        spatial.SpatialRel = esriSpatialRelEnum.esriSpatialRelTouches;
        //        pCursor = fc.Update(spatial, false);
        //        pFeature = pCursor.NextFeature();
        //        pEnvelopeMove = pFeature.Shape.Envelope;
        //        //获取要平移的Envelope
        //        while ((pFeature = pCursor.NextFeature()) != null)
        //        {
        //            pEnvelopeMove.Union(pFeature.Shape.Envelope);
        //        }
        //        Marshal.ReleaseComObject(pCursor);

        //        #region 以缓冲区来获取所有元素,就会包含原始的地块
        //        ITopologicalOperator topo;
        //        IGeometry topoGeometry;
        //        topo = feature.Shape as ITopologicalOperator;
        //        topoGeometry = topo.Buffer(0.1);
        //        ISpatialFilter pSpatialFilter = new SpatialFilterClass();
        //        pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
        //        pSpatialFilter.Geometry = topoGeometry;
        //        IFeatureCursor mFeatureCursor = fc.Update(pSpatialFilter, false);

        //        #endregion


        //        //获取要平移的向量
        //        var pLine = GetMoveVector(pEnvelopeReference, pEnvelopeMove);
        //        if (pLine != null)
        //        {
        //            //开始移动所有要素
        //            while ((pFeature = mFeatureCursor.NextFeature()) != null)
        //            {

        //                IGeometry geo = pFeature.Shape as IGeometry;
        //                ((ITransform2D)geo).MoveVector(pLine);
        //                pFeature.Store();
        //                pEnvelopeReference.Union(pFeature.Shape.Envelope);
        //            }
        //            Marshal.ReleaseComObject(mFeatureCursor);
        //        }


        //        //移动该地块
        //        //IGeometry geofeature = feature.Shape as IGeometry;
        //        // ((ITransform2D)geofeature).MoveVector(pLine);
        //        //保存修改后的位置
        //        // feature.Store();
        //        //MessageBox.Show("移动后位置：" + "\n" + GetPosition(feature.Shape.Envelope));

        //        //IGeometry geoEnvelope = pEnvelopeMove.Envelope as IGeometry;
        //        // ((ITransform2D)geoEnvelope).MoveVector(pLine);

        //        // MessageBox.Show("移动后envelope位置：" + "\n" + GetPosition(pEnvelopeMove));
        //        //pEnvelopeReference.Union(pEnvelopeMove);                
        //        feature = cursor.NextFeature();
        //    }
        //    Marshal.ReleaseComObject(cursor);
        //    //停止编辑

        //    pWorkspaceEdit.StopEditOperation();
        //    pWorkspaceEdit.StopEditing(true);
        //    GC.WaitForPendingFinalizers();
        //    GC.Collect();
        //    return true;
        //}
        /// <summary>
        /// 根据两者的外包Ienvelope求解移动向量
        /// </summary>
        /// <param name="envelopeOrigin">中心外包矩形</param>
        /// <param name="envelopeMove">移动外包矩形</param>
        /// <returns>移动向量</returns>
        //private static ILine GetMoveVector(IEnvelope envelopeOrigin, IEnvelope envelopeMove)
        //{


        //    //throw new NotImplementedException();
        //    //如果两个地块相离
        //    IPoint pointFromCenter = new PointClass();
        //    pointFromCenter.PutCoords((envelopeMove.XMin + envelopeMove.XMax) * 0.5,
        //    (envelopeMove.YMin + envelopeMove.YMax) * 0.5);
        //    IPoint pointToCenter = new PointClass();
        //    pointToCenter.PutCoords((envelopeOrigin.XMax + envelopeOrigin.XMin) * 0.5,
        //             (envelopeOrigin.YMax + envelopeOrigin.YMin) * 0.5);
        //    IRelationalOperator topo = envelopeMove as IRelationalOperator;

        //    ILine pInterctLine = new LineClass();
        //    pInterctLine.PutCoords(pointFromCenter, pointToCenter);
        //    IPoint fromPoint = new PointClass();
        //    IPoint toPoint = new PointClass();
        //    ILine pline = new LineClass();
        //    fromPoint = GetLineEnvelopeIntersect(pInterctLine, envelopeMove);
        //    toPoint = GetLineEnvelopeIntersect(pInterctLine, envelopeOrigin);
        //    if (fromPoint != null && toPoint != null)
        //    {

        //        pline.PutCoords(fromPoint, toPoint);
        //        return pline;
        //    }
        //    fromPoint = new PointClass();
        //    toPoint = new PointClass();
        //    //如果两个地块相交
        //    int direction = AETools.AeHelper.GetDirection(envelopeOrigin, envelopeMove);
        //    //east
        //    if (direction == 0)
        //    {
        //        fromPoint.PutCoords(envelopeMove.XMin, pointFromCenter.Y);
        //        toPoint.PutCoords(envelopeOrigin.XMax, pointToCenter.Y);
        //        pline.PutCoords(fromPoint, toPoint);
        //        return pline;
        //    }
        //    //west
        //    if (direction == 1)
        //    {
        //        fromPoint.PutCoords(envelopeMove.XMax, pointFromCenter.Y);
        //        toPoint.PutCoords(envelopeOrigin.XMin, pointToCenter.Y);
        //        pline.PutCoords(fromPoint, toPoint);
        //        return pline;
        //    }
        //    //south
        //    if (direction == 2)
        //    {
        //        fromPoint.PutCoords(pointFromCenter.X, envelopeMove.YMax);
        //        toPoint.PutCoords(pointToCenter.X, envelopeOrigin.YMin);
        //        pline.PutCoords(fromPoint, toPoint);
        //        return pline;
        //    }
        //    //north
        //    if (direction == 3)
        //    {
        //        fromPoint.PutCoords(pointFromCenter.X, envelopeMove.YMin);
        //        toPoint.PutCoords(pointToCenter.X, envelopeOrigin.YMax);
        //        pline.PutCoords(fromPoint, toPoint);
        //        return pline;
        //    }
        //    return null;

        //}

        private static IPoint GetLineEnvelopeIntersect(ILine pline, IEnvelope envelope)
        {
            IPoint point = new PointClass();
            //外包矩形的角点的值
            double xMin = envelope.XMin - 5;
            double yMin = envelope.YMin - 5;
            double xMax = envelope.XMax + 5;
            double yMax = envelope.YMax + 5;
            //直线的参数
            double x1 = pline.FromPoint.X;
            double y1 = pline.FromPoint.Y;
            double x2 = pline.ToPoint.X;
            double y2 = pline.ToPoint.Y;
            //如果是平行于Y轴
            if (Math.Abs(x1 - x2) <= 0.00001)
            {
                if ((y1 - yMax) * (y2 - yMax) < 0)
                {
                    point.PutCoords(x1, yMax);
                    return point;
                }
                if ((y1 - yMin) * (y2 - yMin) < 0)
                {
                    point.PutCoords(x1, yMin);
                    return point;
                }
            }
            //如果平行于X轴
            if (Math.Abs(y1 - y2) <= 0.00001)
            {
                if ((x1 - xMax) * (x2 - xMax) < 0)
                {
                    point.PutCoords(xMax, y1);
                    return point;
                }
                if ((x1 - xMin) * (x2 - xMin) < 0)
                {
                    point.PutCoords(xMin, y1);
                    return point;
                }
            }

            //如果是普通直线
            double k = (y1 - y2) / (x1 - x2);
            double b = y1 - x1 * k;
            //y值
            double y;
            //如果是左边那条线        
            y = k * xMin + b;
            if ((yMin - y) * (yMax - y) < 0 && (x1 - xMin) * (x2 - xMin) < 0)
            {
                point.PutCoords(xMin, y);
                return point;
            }
            //如果是交点
            if (Math.Abs(yMin - y) < 0.00001 && (x1 - xMin) * (x2 - xMin) < 0)
            {
                point.PutCoords(xMin, yMin);
                return point;
            }
            if (Math.Abs(yMax - y) < 0.00001 && (x1 - xMin) * (x2 - xMin) < 0)
            {
                point.PutCoords(xMin, yMax);
                return point;
            }
            //如果是右边那条线
            y = k * xMax + b;
            if ((yMax - y) * (yMin - y) < 0 && (x1 - xMax) * (x2 - xMax) < 0)
            {
                point.PutCoords(xMax, y);
                return point;
            }
            //如果是角点
            if (Math.Abs(yMin - y) < 0.00001 && (x1 - xMax) * (x2 - xMax) < 0)
            {
                point.PutCoords(xMax, yMin);
                return point;
            }
            if (Math.Abs(yMax - y) < 0.00001 && (x1 - xMax) * (x2 - xMax) < 0)
            {
                point.PutCoords(xMax, yMax);
                return point;
            }
            double x;
            //如果是上面那条线
            x = (yMax - b) / k;
            if ((xMax - x) * (xMin - x) < 0 && (y1 - yMax) * (y2 - yMax) < 0)
            {
                point.PutCoords(x, yMax);
                return point;
            }
            //如果是下面那条线
            x = (yMin - b) / k;
            if ((xMax - x) * (xMin - x) < 0 && (y1 - yMin) * (y2 - yMin) < 0)
            {
                point.PutCoords(x, yMin);
                return point;
            }
            return null;
        }

        /// <summary>
        /// 以多边形的外包矩形为判断依据
        /// </summary>
        /// <param name="centerPolygon">中心多边形</param>
        /// <param name="targetPolygon">目标多边形</param>
        /// <returns>索引值</returns>
        private static int GetDirection(IEnvelope centerPolygon, IEnvelope targetPolygon)
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
                return 1;
            }
            //如果方向是南
            if (vectorPoint.Y < 0 && (Math.Abs(vectorPoint.Y)) > (Math.Abs(vectorPoint.X)))
            {
                return 2;
            }
            //如果方向是北
            if (vectorPoint.Y > 0 && (Math.Abs(vectorPoint.Y)) > (Math.Abs(vectorPoint.X)))
            {
                return 3;
            }
            return -1;
        }
        class DkInfo
        {
            public const int ROAD_ID = -1; //路的objectid值
            public int dkid = -1;          //该地块的objectid值
            public int dz = -1;            //东至
            public int xz = -1;            //西至
            public int nz = -1;            //南至
            public int bz = -1;            //北至

        }

        public static string GetPosition(IEnvelope pEnvelope)
        {
            double xx = (pEnvelope.XMin + pEnvelope.XMax) /
             2.0;
            double yy = (pEnvelope.YMax + pEnvelope.YMin) / 2.0;
            return "X:" + xx.ToString("f") + "\n" + "Y:" + yy.ToString("f");
        }
    }
}
