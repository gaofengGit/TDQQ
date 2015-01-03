using System;
using System.Data;
using TDQQ.Common;
using TDQQ.MyWindow;

namespace TDQQ.Import
{
    class ImportFbf:ImportBase
    {
        public ImportFbf()
        { }
        public ImportFbf(string basicDatabase) : base(basicDatabase) { }
        public override bool Import()
        {
            try
            {
                DeleteTable("FBF");
                var fbfInfo = new WinFbfInfo();
                if (fbfInfo.ShowDialog() == true)
                {

                    var sqlString = string.Format("insert into {0} (FBFBM,FBFMC,FBFFZRXM,FZRZJLX,FZRZJHM,LXDH,FBFDZ,YZBM)" +
                                     "Values('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", "FBF",
                                     fbfInfo.Fbfbm, fbfInfo.Fbfmc, fbfInfo.Fzrxm, fbfInfo.Fzrzjlx, fbfInfo.Zjhm, fbfInfo.Lxdh, fbfInfo.Fbfdz, fbfInfo.Yzbm);
                    AccessFactory accessFactory = new AccessFactory(BasicDatabase);
                    var ret = accessFactory.Execute(sqlString);
                    return ret == -1 ? false : true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
