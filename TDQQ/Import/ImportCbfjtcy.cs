using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using TDQQ.Common;
using TDQQ.MyWindow;

namespace TDQQ.Import
{
    class ImportCbfjtcy : ImportBase
    {
        private string _txtFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\Log\logfile.txt";
        public ImportCbfjtcy(string basicDatabase) : base(basicDatabase) { }


        private string OpenImportFile()
        {
            var dialogFactory = new DialogFactory("家庭成员表（.xls）|*.xls");
            return dialogFactory.OpenFile("打开基础信息表");

        }
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <returns></returns>
        public override bool Import()
        {
            var excelPath = OpenImportFile();
            if (string.IsNullOrEmpty(excelPath)) return false;
            if (!TDQQ.Check.ValidCheck.ExcelColumnSorted(excelPath))
            {
                MessageBox.MessageWarning.Show("系统提示", "基础信息表列顺序不满足要求！");
                return false;
            }
            Wait wait = new Wait();
            wait.SetInfoInvoke("正在导入承包方表");
            wait.SetProgressInfo(string.Empty);
            Hashtable para = new Hashtable();
            para["excelPath"] = excelPath;
            para["wait"] = wait;
            para["ret"] = false;
            Thread t = new Thread(new ParameterizedThreadStart(Import));
            t.Start(para);
            wait.ShowDialog();
            t.Abort();
            return (bool)para["ret"];
        }
      
        private void Import(object p)
        {
            Hashtable para = p as Hashtable;
            var wait = para["wait"] as Wait;
            if (!Import(para["excelPath"].ToString(),wait))
            {
                wait.CloseWait();
                para["ret"] = false;
                return;
            }
            wait.SetInfoInvoke("正在提取承包方名称");
            wait.SetProgressInfo(string.Empty);
            if (!SetCbfmc(wait))
            {
                wait.CloseWait();
                para["ret"] = false;
                return;
            }
            wait.SetInfoInvoke("正在创建承包方表");
            wait.SetProgressInfo(string.Empty);
            if (!CreateCbf(wait))
            {
                wait.CloseWait();
                para["ret"] = false;
                return;
            }
            wait.CloseWait();

            para["ret"] = true;
            return;

        }
        private bool Import(string excelPath,Wait wait)
        {
            //Hashtable para = p as Hashtable;
            //var wait = para["wait"] as Wait;
            //var excelPath = para["excelPath"].ToString();
            try
            {
                if (!DeleteTable("CBF_JTCY")) return false;
                //{
                //   // wait.CloseWait();
                //    //para["ret"] = false;
                //    //return;
                //    return false;
                //}
                using (FileStream fileStream = new FileStream(excelPath, FileMode.Open, FileAccess.Read))
                {
                    HSSFWorkbook workbookSource = new HSSFWorkbook(fileStream);
                    //先填写第一个sheet内容
                    HSSFSheet sheetSource = (HSSFSheet)workbookSource.GetSheetAt(0);
                    int sheetRowCount = sheetSource.LastRowNum;
                    int start_row_index = 1;
                    HSSFRow rowSource = (HSSFRow)sheetSource.GetRow(start_row_index);
                    ICell cell = null;
                    int currentIndex = 0;
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("导入家庭成员信息错误信息：\r\n");
                    while (rowSource != null)
                    {
                        wait.SetProgressInfo("step 1 of 3   "+((double)currentIndex++ / (double)sheetRowCount).ToString("P"));
                        cell = rowSource.GetCell(0);
                        if (cell == null)
                        {
                            stringBuilder.Append(string.Format("第{0}行承包方编码为空 \r\n", currentIndex + 1));
                            Common.CommonHelper.WriteErrorInfo(_txtFilePath, stringBuilder.ToString());
                            start_row_index++;
                            rowSource = (HSSFRow)sheetSource.GetRow(start_row_index);
                            continue;
                        }
                        var cbfbm = rowSource.GetCell(0).ToString().Trim();
                        if (cbfbm.Length!=18)
                        {
                            stringBuilder.Append(string.Format("第{0}行承包方编码不是18位 \r\n", currentIndex + 1));
                            Common.CommonHelper.WriteErrorInfo(_txtFilePath, stringBuilder.ToString());
                            return false;
                        }
                        cell = rowSource.GetCell(1);
                        if (cell == null)
                        {
                            stringBuilder.Append(string.Format("第{0}行性别为空 \r\n", currentIndex + 1));
                            Common.CommonHelper.WriteErrorInfo(_txtFilePath, stringBuilder.ToString());
                            start_row_index++;
                            rowSource = (HSSFRow)sheetSource.GetRow(start_row_index);
                            continue;
                        }
                        var cyxb = cell.ToString().Trim();
                        cell = rowSource.GetCell(2);
                        if (cell == null)
                        {
                           
                            stringBuilder.Append(string.Format("第{0}行承包方名称为空  \r\n", currentIndex + 1));
                            Common.CommonHelper.WriteErrorInfo(_txtFilePath, stringBuilder.ToString());
                            start_row_index++;
                            rowSource = (HSSFRow)sheetSource.GetRow(start_row_index);
                            continue;
                        }
                        var cyxm = rowSource.GetCell(2).ToString().Trim();
                        var cyzjlx = string.Empty;
                        if (rowSource.GetCell(3) != null)
                        {
                            cyzjlx = rowSource.GetCell(3).ToString();
                        }
                        string cyzjhm = string.Empty;
                        if (rowSource.GetCell(4) != null)
                        {
                            cyzjhm = rowSource.GetCell(4).ToString().Trim();
                        }
                        var cybz = string.Empty;
                        if (rowSource.GetCell(5) != null)
                        {
                            cybz = rowSource.GetCell(5).ToString().Trim();
                        }
                        var yhzgx = string.Empty;
                        if (rowSource.GetCell(6) != null)
                        {
                            yhzgx = rowSource.GetCell(6).ToString().Trim();
                        }
                        var cyszc = string.Empty;
                        if (rowSource.GetCell(7) != null)
                        {
                            cyszc = rowSource.GetCell(7).ToString().Trim();
                        }
                        var yzbm = string.Empty;
                        if (rowSource.GetCell(8) != null)
                        {
                            yzbm = rowSource.GetCell(8).ToString().Trim();
                        }
                        var sfgyr = string.Empty;
                        if (rowSource.GetCell(9) != null)
                        {
                            sfgyr = rowSource.GetCell(9).ToString().Trim();
                        }
                        var lxdh = string.Empty;
                        if (rowSource.GetCell(10) != null)
                        {
                            lxdh = rowSource.GetCell(10).ToString().Trim();
                        }
                        //往数据库中插入记录
                        var sqlString =
                            string.Format(
                                "insert into {0} (CBFBM,CYXB,CYXM,CYZJHM,CYZJLX,CYBZ,YHZGX,CYSZC,SFGYR,LXDH,YZBM) " +
                                "VALUES ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')",
                                "CBF_JTCY", cbfbm, cyxb, cyxm, cyzjhm, cyzjlx, cybz, yhzgx, cyszc, sfgyr, lxdh, yzbm);
                        if (!InsertRow(sqlString)) return false;
                        //{
                        //    wait.CloseWait();
                        //    //para["ret"] = false;
                        //    //return;
                        //    return false;
                        //}
                        start_row_index++;
                        rowSource = (HSSFRow)sheetSource.GetRow(start_row_index);
                    }
                    fileStream.Close();
                }
                //wait.CloseWait();
               // para["ret"] = true;
                return true;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
                //wait.CloseWait();
                //para["ret"] = false;
                //return;
                return false;
            }
        }

        private bool SetCbfmc(Wait wait)
        {
            //Hashtable para = p as Hashtable;
            //Wait wait = para["wait"] as Wait;
            try
            {
                var sqlString = string.Format("update CBF_JTCY set CBFMC = CYXM where trim(YHZGX)='02'");
                var accessFactory = new AccessFactory(BasicDatabase);
                var ret = accessFactory.Execute(sqlString);
                if (ret == -1) return false;
                //{
                //    wait.CloseWait();
                //    para["ret"] = false;
                //    return;
                //}
                sqlString = string.Format("select CBFBM,CBFMC from CBF_JTCY where trim(YHZGX)='02'");
                var dt = accessFactory.Query(sqlString);
                if (dt == null) return false;
                //{
                //    wait.CloseWait();
                //    para["ret"] = false;
                //    return;
                //}
                int rowCount = dt.Rows.Count;
                int currentIndex = 0;
                for (int i = 0; i < rowCount; i++)
                {
                    wait.SetProgressInfo("step 2 of 3   " + ((double)currentIndex++ / (double)rowCount).ToString("P"));
                    var cbfmc = dt.Rows[i][1].ToString().Trim();
                    sqlString = string.Format("update CBF_JTCY set CBFMC='{0}'where trim(CBFBM)='{1}'", cbfmc,
                        dt.Rows[i][0].ToString().Trim());
                    accessFactory.Execute(sqlString);
                }
                //wait.CloseWait();
                //para["ret"] = true;
                return true;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
                //wait.CloseWait();
                //para["ret"] = false;
                return false;
            }
        }

        private bool CreateCbf(Wait wait)
        {
            //Hashtable para = p as Hashtable;
            //Wait wait = para["wait"] as Wait;
            try
            {
                DeleteTable("CBF");
                var sqlString = string.Format("Select CBFBM,CBFMC,CYZJLX,CYZJHM,CYSZC,YZBM,LXDH,CYXB from {0} " +
                                              "Where trim(YHZGX)='{1}'", "CBF_JTCY", "02");
                AccessFactory accessFactory = new AccessFactory(BasicDatabase);
                var dt = accessFactory.Query(sqlString);
                if (dt == null) return false;
                //{
                //    //wait.CloseWait();
                //    //para["ret"] = false;
                //    return;
                //}
                int rowCount = dt.Rows.Count;
                int currentIndex = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    wait.SetProgressInfo("step 3 of 3   "+((double)currentIndex++ / (double)rowCount).ToString("P"));
                    var cbfbm = dt.Rows[i][0].ToString();
                    sqlString = string.Format("Select CBFBM from {0} where trim(CBFBM)='{1}'", "CBF_JTCY", cbfbm);
                    var dt1 = accessFactory.Query(sqlString);
                    var cbfcysl = dt1.Rows.Count;
                    var cbflx = "1";
                    var cbfmc = dt.Rows[i][1].ToString().Trim();
                    var cbfzjlx = dt.Rows[i][2].ToString().Trim();
                    var cbfzjhm = dt.Rows[i][3].ToString().Trim();
                    var cbfdz = dt.Rows[i][4].ToString().Trim();
                    var yzbm = dt.Rows[i][5].ToString().Trim();
                    var lxdh = dt.Rows[i][6].ToString().Trim();
                    var cyxb = dt.Rows[i][7].ToString().Trim();
                    sqlString = string.Format("Insert into {0} (CBFBM,CBFLX,CBFMC,CBFZJLX," +
                                              "CBFZJHM,CBFDZ,YZBM,LXDH,CBFCYSL,CYXB) values('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',{9},'{10}')",
                                              "CBF", cbfbm, cbflx, cbfmc, cbfzjlx, cbfzjhm, cbfdz, yzbm, lxdh, cbfcysl, cyxb);
                    accessFactory.Execute(sqlString);
                    dt1.Clear();
                }
                //wait.CloseWait();
                //para["ret"] = true;
                //return;
                return true;
            }
            catch (Exception)
            {
                //wait.CloseWait();
                //para["ret"] = false;
                return false;
            }
        }
        public override bool ValidCheck(string filePath)
        {
           // return base.ValidCheck(filePath);
            StringBuilder stringBuilder=new StringBuilder();
            stringBuilder.Append("家庭基础信息表的格式错误如下：\r\n");
            bool flag=true;
            using (FileStream fileStream=new FileStream(filePath,FileMode.Open,FileAccess.Read))
            {
                IWorkbook workbook=new HSSFWorkbook(fileStream);
                ISheet sheet = workbook.GetSheetAt(0);
                
                if (sheet == null)
                {
                    stringBuilder.Append("承包方信息表没有放在第一张sheet");
                    Common.CommonHelper.WriteErrorInfo(_txtFilePath, stringBuilder.ToString());
                    return false;
                }
                IRow row = sheet.GetRow(0);
                if (row == null)
                {
                    stringBuilder.Append("承包方信息表第一行没有数据");
                    Common.CommonHelper.WriteErrorInfo(_txtFilePath, stringBuilder.ToString());
                    return false;
                }
                if (row.GetCell(0) == null || row.GetCell(0).ToString().Trim() != "CBFBM")
                {
                    stringBuilder.Append("承包方信息表第一列不符合规范 \r\n");
                    flag = false;
                }
                if (row.GetCell(1) == null || row.GetCell(0).ToString().Trim() != "CYXB")
                {
                    stringBuilder.Append("承包方信息表第二列不符合规范 \r\n");
                    flag = false;
                }
                if (row.GetCell(2) == null || row.GetCell(0).ToString().Trim() != "CYXM")
                {
                    stringBuilder.Append("承包方信息表第三列不符合规范 \r\n");
                    flag = false;
                }
                if (row.GetCell(3) == null || row.GetCell(0).ToString().Trim() != "CYZJLX")
                {
                    stringBuilder.Append("承包方信息表第四列不符合规范 \r\n");
                    flag = false;
                }
                if (row.GetCell(4) == null || row.GetCell(0).ToString().Trim() != "CYZJHM")
                {
                    stringBuilder.Append("承包方信息表第五列不符合规范 \r\n");
                    flag = false;
                }
                if (row.GetCell(5) == null || row.GetCell(0).ToString().Trim() != "CYBZ")
                {
                    stringBuilder.Append("承包方信息表第六列不符合规范 \r\n");
                    flag = false;
                }
                if (row.GetCell(6) == null || row.GetCell(0).ToString().Trim() != "YHZGX")
                {
                    stringBuilder.Append("承包方信息表第七列不符合规范 \r\n");
                    flag = false;
                }
                if (row.GetCell(7) == null || row.GetCell(0).ToString().Trim() != "CYSZC")
                {
                    stringBuilder.Append("承包方信息表第八列不符合规范 \r\n");
                    flag = false;
                }
                if (row.GetCell(8) == null || row.GetCell(0).ToString().Trim() != "YZBM")
                {
                    stringBuilder.Append("承包方信息表第九列不符合规范 \r\n");
                    flag = false;
                }
                if (row.GetCell(9) == null || row.GetCell(0).ToString().Trim() != "SFGYR")
                {
                    stringBuilder.Append("承包方信息表第10列不符合规范 \r\n");
                    flag = false;
                }
                if (row.GetCell(10) == null || row.GetCell(0).ToString().Trim() != "LXDH")
                {
                    stringBuilder.Append("承包方信息表第十一列不符合规范 \r\n");
                    flag = false;
                }
                Common.CommonHelper.WriteErrorInfo(_txtFilePath, stringBuilder.ToString());
            }
            return flag;
        }
    }
}
