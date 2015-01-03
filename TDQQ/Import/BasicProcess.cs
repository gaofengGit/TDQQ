using System;
using System.Collections;
using System.Data;
using System.Threading;
using TDQQ.Common;
using TDQQ.MyWindow;
namespace TDQQ.Import
{
    class BasicProcess : ImportBase
    {
         public BasicProcess(){ }
         public BasicProcess(string basicDatabase) : base(basicDatabase) { }

        public bool SetCbfmc()
        {
            Wait wait=new Wait();
            wait.SetInfoInvoke("正在获取承包方名称");
            wait.SetProgressInfo(string.Empty);
            Hashtable para=new Hashtable();
            para["wait"] = wait;
            para["ret"] = false;
            Thread  t=new Thread(new ParameterizedThreadStart(SetCbfmc));
            t.Start(para);
            wait.ShowDialog();
            t.Abort();
            return (bool) para["ret"];
        }
        
        private void SetCbfmc(object p)
        {
            Hashtable para = p as Hashtable;
            Wait wait = para["wait"] as Wait;
            try
            {
                var sqlString = string.Format("update CBF_JTCY set CBFMC = CYXM where trim(YHZGX)='02'");
                var accessFactory = new AccessFactory(BasicDatabase);
                var ret = accessFactory.Execute(sqlString);
                if (ret == -1)
                {
                    wait.CloseWait();
                    para["ret"] = false;
                    return;
                }
                sqlString = string.Format("select CBFBM,CBFMC from CBF_JTCY where trim(YHZGX)='02'");
                var dt = accessFactory.Query(sqlString);
                if (dt == null)
                {
                    wait.CloseWait();
                    para["ret"] = false;
                    return;
                }
                int rowCount = dt.Rows.Count;
                int currentIndex = 0;
                for (int i = 0; i < rowCount; i++)
                {
                    wait.SetProgressInfo(((double)currentIndex++/(double)rowCount).ToString("P"));
                    var cbfmc = dt.Rows[i][1].ToString().Trim();
                    sqlString = string.Format("update CBF_JTCY set CBFMC='{0}'where trim(CBFBM)='{1}'", cbfmc,
                        dt.Rows[i][0].ToString().Trim());
                    accessFactory.Execute(sqlString);
                }
                wait.CloseWait();
                para["ret"] = true;
                return ;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
                wait.CloseWait();
                para["ret"] = false;
                return;
            }           
        }

        public bool CreateCbf()
        {
            Wait wait = new Wait();
            wait.SetInfoInvoke("正在创建承包方表");
            wait.SetProgressInfo(string.Empty);
            Hashtable para = new Hashtable();
            para["wait"] = wait;
            para["ret"] = false;
            Thread t = new Thread(new ParameterizedThreadStart(CreateCbf));
            t.Start(para);
            wait.ShowDialog();
            t.Abort();
            return (bool)para["ret"];
        }
        private void CreateCbf(object p)
        {
            Hashtable para = p as Hashtable;
            Wait wait = para["wait"] as Wait;
            try
            {
                DeleteTable("CBF");
                var sqlString = string.Format("Select CBFBM,CBFMC,CYZJLX,CYZJHM,CYSZC,YZBM,LXDH,CYXB from {0} " +
                                              "Where trim(YHZGX)='{1}'", "CBF_JTCY", "02");
                AccessFactory accessFactory = new AccessFactory(BasicDatabase);
                var dt = accessFactory.Query(sqlString);
                if (dt == null)
                {
                    wait.CloseWait();
                    para["ret"] = false;
                    return;
                }
                int rowCount = dt.Rows.Count;
                int currentIndex = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    wait.SetProgressInfo(((double)currentIndex++ / (double)rowCount).ToString("P"));
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
                wait.CloseWait();
                para["ret"] = true;
                return;
            }
            catch (Exception)
            {
                wait.CloseWait();
                para["ret"] = false;
                return;
            }         
        }
    }
}
