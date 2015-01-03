using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using Microsoft.Windows.Shell;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using TDQQ.AE;
using TDQQ.Common;
using IRow = NPOI.SS.UserModel.IRow;

namespace TDQQ.Check
{
    public class ValidCheck
    {
        /// <summary>
        /// 家庭成员信息表的列表是否按照标准数据
        /// </summary>
        /// <param name="excelPath">excel文件地址</param>
        /// <returns>是否满足条件</returns>
        public static bool ExcelColumnSorted(string excelPath)
        {
            try
            {
                using (var fileStream = new System.IO.FileStream(excelPath, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbook = new HSSFWorkbook(fileStream);
                    ISheet sheet = workbook.GetSheetAt(0);
                    //获取第一行的列名称
                    IRow row = sheet.GetRow(0);
                    // ICell cell = row.GetCell(0);
                    bool flag = true;
                    if (row.GetCell(0).ToString().Trim() != "CBFBM") flag = false;
                    if (row.GetCell(1).ToString().Trim() != "CYXB") flag = false;
                    if (row.GetCell(2).ToString().Trim() != "CYXM") flag = false;
                    if (row.GetCell(3).ToString().Trim() != "CYZJLX") flag = false;
                    if (row.GetCell(4).ToString().Trim() != "CYZJHM") flag = false;
                    if (row.GetCell(5).ToString().Trim() != "CYBZ") flag = false;
                    if (row.GetCell(6).ToString().Trim() != "YHZGX") flag = false;
                    if (row.GetCell(7).ToString().Trim() != "CYSZC") flag = false;
                    if (row.GetCell(8).ToString().Trim() != "YZBM") flag = false;
                    if (row.GetCell(9).ToString().Trim() != "SFGYR") flag = false;
                    if (row.GetCell(10).ToString().Trim() != "LXDH") flag = false;
                    fileStream.Close();
                    return flag;

                }
            }
            catch (Exception)
            {
                return false;
            }
           
        }


        /// <summary>
        /// 检查字段是否为空
        /// </summary>
        /// <param name="personDatabase"></param>
        /// <param name="selectFeature"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool PersonDatabaseNullField(string personDatabase, string selectFeature, string fieldName)
        {
            AccessFactory accessFactory = new AccessFactory(personDatabase);
            var sqlString = string.Format("Select Count(*) from {0} where {1} is null", selectFeature,fieldName);
            var res = accessFactory.ExecuteScalar(sqlString);
            if ((int)res > 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查某个字段是否满足是否规定的格式
        /// </summary>
        /// <param name="persondDatabase"></param>
        /// <param name="selectFeaure"></param>
        /// <param name="fieldName"></param>
        /// <param name="targetFieldType"></param>
        /// <returns></returns>
        public static bool FieldTypeCheck(string persondDatabase, string selectFeaure,string fieldName,
            esriFieldType targetFieldType)
        {
            IAeFactory pAeFactory=new PersonalGeoDatabase(persondDatabase);
            IFeatureClass pFeatureClass = pAeFactory.OpenFeatureClasss(selectFeaure);
            bool flag;
            if (pFeatureClass.Fields.FindField(fieldName)==-1)
            {
                flag = false;
            }
            else
            {
                var type = pFeatureClass.Fields.Field[pFeatureClass.Fields.FindField(fieldName)].Type;
                flag = type == targetFieldType
                    ? true
                    : false;
            }
          pAeFactory.ReleaseFeautureClass(pFeatureClass);
            return flag;

        }

        //public static bool FieldExistCheck(string personDatabase, string selectFeaure, string fieldName)
        //{
        //    IAeFactory pAeFactory = new PersonalGeoDatabase(personDatabase);
        //    IFeatureClass pFeatureClass = pAeFactory.OpenFeatureClasss(selectFeaure);
        //    bool flag = pFeatureClass.FindField(fieldName) == -1 ? false: true;
        //    pAeFactory.ReleaseFeautureClass(pFeatureClass);
        //    return flag;
        //}
        /// <summary>
        /// 检查字段是否存在
        /// </summary>
        /// <param name="personDatabase">个人地理数据库</param>
        /// <param name="selectFeaure">选择的要素类</param>
        /// <param name="fields">字段集合</param>
        /// <returns>只要有一个不存在则返回flag</returns>
        public static bool FieldExistCheck(string personDatabase, string selectFeaure, params string[] fields)
        {
            IAeFactory pAeFactory = new PersonalGeoDatabase(personDatabase);
            IFeatureClass pFeatureClass = pAeFactory.OpenFeatureClasss(selectFeaure);
            bool flag = true;
            foreach (var field in fields)
            {
                if (pFeatureClass.FindField(field)==-1)
                {
                    flag = false;
                    break;
                }
            }
            pAeFactory.ReleaseFeautureClass(pFeatureClass);
            return flag;
        }
    }
}
