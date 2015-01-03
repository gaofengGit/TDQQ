using System;
using System.Collections;
using System.IO;
using System.Threading;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using TDQQ.AE;
using TDQQ.Common;
using TDQQ.MyWindow;

namespace TDQQ.Export
{
    class ExportCertification
    {
        public string PersonDatabase { get; set; }
        public string SelectFeature { get; set; }
        public string BasicDatabase { get; set; }

        public ExportCertification(string personDatabase, string selectFeature, string basicDatabase)
        {
            PersonDatabase = personDatabase;
            SelectFeature = selectFeature;
            BasicDatabase = basicDatabase;
        }

        public bool Export()
        {
            if (!AeHelper.CheckVaild(PersonDatabase, SelectFeature))
            {
                MessageBox.MessageWarning.Show("系统提示", "字段尚未全部添加！");
                return false;
            }
            var winJyz = new WinJyz();
            string xian = string.Empty, xiang = string.Empty, cun = string.Empty;
            if (winJyz.ShowDialog() == true)
            {
                xian = winJyz.Xian; xiang = winJyz.Xiang; cun = winJyz.Cun;
            }
            else return false;
            var winSelectFeature = new WinSelectFeature(PersonDatabase);
            winSelectFeature.SetTitle("请选择边界");
            string cunEdge = string.Empty;
            if (winSelectFeature.ShowDialog() == true)
                cunEdge = winSelectFeature.SelectFeature;
            else
                return false;
            var dialogFactory = new DialogFactory();
            var folderPath = dialogFactory.OpenFolderDialog();
            if (string.IsNullOrEmpty(folderPath)) return false;
            Wait wait = new Wait();
            wait.SetInfoInvoke("正在导出经营权证");
            wait.SetProgressInfo(string.Empty);
            var para = new Hashtable();
            para["xian"] = xian;
            para["xiang"] = xiang;
            para["cun"] = cun;
            para["cunEdge"] = cunEdge;
            para["folderPath"] = folderPath;
            para["wait"] = wait;
            para["ret"] = false;
            Thread t = new Thread(new ParameterizedThreadStart(Export));
            t.Start(para);
            wait.ShowDialog();
            t.Abort();
            return (bool)para["ret"];
        }

        private void Export(object p)
        {
            var para = p as Hashtable;
            Wait wait = para["wait"] as Wait;
            var folderPath = para["folderPath"].ToString();
            try
            {
                var sqlString = string.Format("select distinct CBFBM,CBFMC from {0} order by CBFBM", SelectFeature);
                var accessFactory = new AccessFactory(PersonDatabase);
                var dt = accessFactory.Query(sqlString);
                if (dt == null)
                {
                    System.Windows.Forms.MessageBox.Show("1");
                    wait.CloseWait();
                    para["ret"] = false;
                    return;
                }
                sqlString = string.Format("select FBFMC,FBFDZ from {0}", "FBF");
                accessFactory = new AccessFactory(BasicDatabase);
                var dtFbf = accessFactory.Query(sqlString);
                if (dtFbf == null || dtFbf.Rows.Count != 1)
                {

                    wait.CloseWait();
                    para["ret"] = false;
                    return;
                }
                var rowCount = dt.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    wait.SetProgressInfo(((double)i / (double)rowCount).ToString("p"));
                    var dir = new DirectoryInfo(folderPath);
                    dir.CreateSubdirectory(dt.Rows[i][0].ToString() + "_" + dt.Rows[i][1].ToString());
                    var toSaveDocPath = folderPath + @"\" + dt.Rows[i][0].ToString() + "_" + dt.Rows[i][1].ToString() + @"\" + dt.Rows[i][0].ToString() +
                                       dt.Rows[i][1].ToString() + "_经营权证.doc";
                    var toSaveExcelPath = folderPath + @"\" + dt.Rows[i][0].ToString() + "_" + dt.Rows[i][1].ToString() + @"\" + dt.Rows[i][0].ToString() +
                                       dt.Rows[i][1].ToString() + "_地块示意图.xls";
                    ExportInfo(toSaveDocPath, dtFbf.Rows[0], dt.Rows[i][0].ToString(), para["xian"].ToString(), para["xiang"].ToString(), para["cun"].ToString());
                    ExportDksyt(toSaveExcelPath, dt.Rows[i][0].ToString(), para["cunEdge"].ToString(), dt.Rows[i][1].ToString(), dtFbf.Rows[0][1].ToString());
                }
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

        private void ExportInfo(string toSaveFilePath, System.Data.DataRow rowFbf, string cbfbm, string xian, string xiang, string cun)
        {
            var sqlString = string.Format("select CBFMC,CBFZJHM,YZBM,LXDH " +
                                         "from {0} where trim(CBFBM)='{1}'", "CBF", cbfbm);
            var accessFactory = new AccessFactory(BasicDatabase);
            var dtCbf = accessFactory.Query(sqlString);
            if (dtCbf == null || dtCbf.Rows.Count != 1) return;
            sqlString =
                   string.Format("select DKMC,DKBM,SCMJ,SFJBNT,DKDZ,DKXZ,DKNZ,DKBZ from {0} where trim(CBFBM)='{1}'",
                       SelectFeature, cbfbm);
            accessFactory = new AccessFactory(PersonDatabase);
            var dtFields = accessFactory.Query(sqlString);
            if (dtFields == null) return;
            if (dtFields.Rows.Count>17)
            {
                System.Windows.Forms.MessageBox.Show(cbfbm.ToString() + "该户的地块数目超过17块，请处理");
                return;
            }
            var cbfmc = dtCbf.Rows[0][0].ToString();
            var cbfzjhm = dtCbf.Rows[0][1].ToString();
            var yzbm = dtCbf.Rows[0][2].ToString();
            var lxdh = dtCbf.Rows[0][3].ToString();
            var year = DateTime.Now.Year.ToString();
            var month = DateTime.Now.Month.ToString();
            var day = DateTime.Now.Day.ToString();
            var templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\template\承包经营权.doc";
            var exportWord = new ExportWord();
            //先填充封面信息
            exportWord.CreateNewDocument(templatePath);
            exportWord.InsertValue("证书编号1", cbfbm + "J");
            exportWord.InsertValue("年1", year);
            exportWord.InsertValue("月1", month);
            exportWord.InsertValue("日1", day);
            ////插入最后的年月日
            exportWord.InsertValue("年2", year);
            exportWord.InsertValue("月2", month);
            exportWord.InsertValue("日2", day);

            //填充共有信息
            exportWord.InsertValue("发包方", rowFbf[0].ToString());
            exportWord.InsertValue("承包方名称", cbfmc);
            exportWord.InsertValue("身份证号码", cbfzjhm);
            exportWord.InsertValue("县", xian);
            exportWord.InsertValue("镇", xiang);
            exportWord.InsertValue("村", cun);
            exportWord.InsertValue("邮编", yzbm);
            if (lxdh != string.Empty)
            {
                exportWord.InsertValue("联系电话", lxdh);
            }
            else
            {
                exportWord.InsertValue("联系电话", "");
            }
            exportWord.InsertValue("证书编号2", cbfbm + "J");
            //根据每个田块填充
            double sumScmj = 0.0;
            int fieldCount = dtFields.Rows.Count;
            for (int i = 0; i < fieldCount; i++)
            {
                if (string.IsNullOrEmpty(dtFields.Rows[i][2].ToString().Trim()))
                {
                    sumScmj += 0.0;
                }
                else
                {
                    sumScmj += Convert.ToDouble(double.Parse(dtFields.Rows[i][2].ToString().Trim()).ToString("f"));
                }             
            }
            //填充整体信息
            exportWord.InsertValue("实测面积", sumScmj.ToString("f"));
            exportWord.InsertValue("地块数", fieldCount.ToString());
            for (int i = 0; i < fieldCount; i++)
            {
                exportWord.InsertValue("地块名称" + i.ToString(), dtFields.Rows[i][0].ToString());
                exportWord.InsertValue("地块编码" + i.ToString(), dtFields.Rows[i][1].ToString());
                if (string.IsNullOrEmpty(dtFields.Rows[i][2].ToString().Trim()))
                {
                    exportWord.InsertValue("实测面积" + i.ToString(), 0.0.ToString("f"));
                }
                else
                {
                    exportWord.InsertValue("实测面积" + i.ToString(),
                    Convert.ToDouble(dtFields.Rows[i][2].ToString().Trim()).ToString("f"));
                }               
                exportWord.InsertValue("基本农田" + i.ToString(),
                    Transcode.CodeToSfjbnt(dtFields.Rows[i][3].ToString()));
                exportWord.InsertValue("东" + i.ToString(),
                    dtFields.Rows[i][4].ToString());
                exportWord.InsertValue("南" + i.ToString(),
                    dtFields.Rows[i][5].ToString());
                exportWord.InsertValue("西" + i.ToString(),
    dtFields.Rows[i][6].ToString());
                exportWord.InsertValue("北" + i.ToString(),
    dtFields.Rows[i][7].ToString());
            }
            for (int i = fieldCount; i < 17; i++)
            {
                exportWord.InsertValue("地块名称" + i.ToString(), "    ");

                exportWord.InsertValue("地块编码" + i.ToString(), "    ");
                exportWord.InsertValue("实测面积" + i.ToString(), "    ");
                exportWord.InsertValue("基本农田" + i.ToString(), "    ");
                exportWord.InsertValue("东" + i.ToString(), "    ");
                exportWord.InsertValue("西" + i.ToString(), "    ");
                exportWord.InsertValue("南" + i.ToString(), "    ");
                exportWord.InsertValue("北" + i.ToString(), "    ");
            }
            exportWord.SaveDocument(toSaveFilePath);
        }

        private void ExportDksyt(string toSaveFilePath, string cbfbm, string cunEdge, string cbfmc, string fbfdz)
        {
            try
            {
                var sqlString = string.Format("Select SCMJ from {0} where trim(CBFBM)='{1}'", SelectFeature, cbfbm);
                var accessFactory = new AccessFactory(PersonDatabase);
                var dtField = accessFactory.Query(sqlString);
                double scmj = 0.0;
                for (int k = 0; k < dtField.Rows.Count; k++)
                {
                    scmj += Convert.ToDouble(double.Parse(dtField.Rows[k][0].ToString().Trim()).ToString("f"));
                }
                sqlString = string.Format("select CBFCYSL from {0} where trim(CBFBM)='{1}'", "CBF", cbfbm);
                accessFactory = new AccessFactory(BasicDatabase);
                var dt = accessFactory.Query(sqlString);
                IAeFactory aeFactory = new PersonalGeoDatabase(PersonDatabase);
                IFeatureClass zdtFC = aeFactory.OpenFeatureClasss(SelectFeature);
                IFeatureClass bjxFC = aeFactory.OpenFeatureClasss(cunEdge);
                IMapDocument mapDoc = new MapDocumentClass();
                mapDoc.Open(AppDomain.CurrentDomain.BaseDirectory + @"\dkct.mxd", "");
                IMap pMap = mapDoc.get_Map(0);
                var templatePath = AppDomain.CurrentDomain.BaseDirectory + @"\template\梁山土地承包地块示意图.xls";
                File.Copy(templatePath, toSaveFilePath, true);
                Tools4Jz.ExportOneSyt(pMap, cbfbm, zdtFC, bjxFC, toSaveFilePath, PersonDatabase, cbfmc, dt.Rows.Count.ToString(), dtField.Rows.Count, scmj, fbfdz);
            }
            catch (Exception vev)
            {
                System.Windows.Forms.MessageBox.Show(vev.ToString());
                throw;
            }
           
        }

    }
}
