using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using TDQQ.Common;
using TDQQ.MessageBox;

namespace TDQQ.Process
{
    class SearchInfo
    {
        public ObservableCollection<FieldInfo> GetFieldInfo(string personDatabase, string selectFeatrue)
        {
            ObservableCollection<FieldInfo> fieldInfos = new ObservableCollection<FieldInfo>();
            var sqlString = string.Format("select DKMC,DKBM,CBFMC from {0} ", selectFeatrue);
            var accessFactory = new AccessFactory(personDatabase);
            var dt = accessFactory.Query(sqlString);
            if (dt == null) return null;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                fieldInfos.Add(new FieldInfo()
                {
                    Dkmc = dt.Rows[i][0].ToString(),
                    Dkbm = dt.Rows[i][1].ToString(),
                    Cbfmc = dt.Rows[i][2].ToString()
                });
            }
            return fieldInfos;
        }
        public ObservableCollection<FarmerInfo> GetFarmerInfo(string personDatabase, string selectFeatrue)
        {
            ObservableCollection<FarmerInfo> farmInfos = new ObservableCollection<FarmerInfo>();
            var sqlString = string.Format("select distinct CBFBM,CBFMC from {0}", selectFeatrue);
            var accessFactory = new AccessFactory(personDatabase);
            var dt = accessFactory.Query(sqlString);
            if (dt == null) return null;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                farmInfos.Add(new FarmerInfo()
                {
                    Cbfbm = dt.Rows[i][0].ToString(),
                    Cbfmc = dt.Rows[i][1].ToString()
                });
            }
            return farmInfos;
        }

        public void AreaInfo(string personDatabase, string selectFeaure)
        {
            try
            {
                if (!AE.AeHelper.CheckField(personDatabase, selectFeaure, "SCMJ"))
                {
                    MessageWarning.Show("系统提示", "不存在SCMJ字段");
                }
                var pAccess = new AccessFactory(personDatabase);
                string sqlString = string.Format("select SUM(SCMJ) from {0}", selectFeaure);
                var sumjSum = (double)pAccess.ExecuteScalar(sqlString);
                MessageInfomation.Show("系统提示", "实测面积为" + sumjSum.ToString("F") + "亩");
            }
            catch (Exception e)
            {
                MessageWarning.Show("系统提示", "计算面积失败");
            }
            
        }
    }
    public class FieldInfo
    {
        public string Dkmc { get; set; }
        public string Dkbm { get; set; }
        public string Cbfmc { get; set; }
    }
    public class FarmerInfo
    {
        public string Cbfbm { get; set; }
        public string Cbfmc { get; set; }
    }
}
