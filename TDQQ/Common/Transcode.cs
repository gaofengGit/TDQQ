
namespace TDQQ.Common
{
    public class Transcode
    {
        /// <summary>
        /// 家庭关系代码转换
        /// </summary>
        /// <param name="code">家庭关系代码</param>
        /// <returns>家庭关系</returns>
        public static string CodeToRelationship(string code)
        {
            string relationShip;
            switch (code)
            {
                case "01":
                    relationShip = "本人";
                    break;
                case "02":
                    relationShip = "户主";
                    break;
                case "10":
                    relationShip = "配偶";
                    break;
                case "11":
                    relationShip = "夫";
                    break;
                case "12":
                    relationShip = "妻";
                    break;
                case "20":
                    relationShip = "子";
                    break;
                case "21":
                    relationShip = "独生子";
                    break;
                case "22":
                    relationShip = "长子";
                    break;
                case "23":
                    relationShip = "次子";
                    break;
                case "24":
                    relationShip = "三子";
                    break;
                case "25":
                    relationShip = "四子";
                    break;
                case "26":
                    relationShip = "五子";
                    break;
                case "27":
                    relationShip = "养子";
                    break;
                case "28":
                    relationShip = "女婿";
                    break;
                case "29":
                    relationShip = "其他儿子";
                    break;
                case "30":
                    relationShip = "女";
                    break;
                case "31":
                    relationShip = "独生女";
                    break;
                case "32":
                    relationShip = "长女";
                    break;
                case "33":
                    relationShip = "次女";
                    break;
                case "34":
                    relationShip = "三女";
                    break;
                case "35":
                    relationShip = "四女";
                    break;
                case "36":
                    relationShip = "五女";
                    break;
                case "37":
                    relationShip = "养女";
                    break;
                case "38":
                    relationShip = "儿媳";
                    break;
                case "39":
                    relationShip = "其他女儿";
                    break;
                case "40":
                    relationShip = "孙子或孙女";
                    break;
                case "41":
                    relationShip = "孙子";
                    break;
                case "42":
                    relationShip = "孙女";
                    break;
                case "43":
                    relationShip = "外孙子";
                    break;
                case "44":
                    relationShip = "外孙女";
                    break;
                case "45":
                    relationShip = "孙媳妇";
                    break;
                case "46":
                    relationShip = "孙女婿";
                    break;
                case "47":
                    relationShip = "曾孙子";
                    break;
                case "48":
                    relationShip = "曾孙女";
                    break;
                case "49":
                    relationShip = "其他孙子";
                    break;
                case "50":
                    relationShip = "父母";
                    break;
                case "51":
                    relationShip = "父亲";
                    break;
                case "52":
                    relationShip = "母亲";
                    break;
                case "53":
                    relationShip = "公公";
                    break;
                case "54":
                    relationShip = "婆婆";
                    break;
                case "55":
                    relationShip = "岳父";
                    break;
                case "56":
                    relationShip = "岳母";
                    break;
                case "57":
                    relationShip = "继父";
                    break;
                case "58":
                    relationShip = "继母";
                    break;
                case "59":
                    relationShip = "其他父母";
                    break;
                case "60":
                    relationShip = "祖父母";
                    break;
                case "61":
                    relationShip = "祖父";
                    break;
                case "62":
                    relationShip = "祖母";
                    break;
                case "63":
                    relationShip = "外祖父";
                    break;
                case "64":
                    relationShip = "外祖母";
                    break;
                case "65":
                    relationShip = "配偶的祖父母";
                    break;
                case "66":
                    relationShip = "曾祖父";
                    break;
                case "67":
                    relationShip = "曾祖母";
                    break;
                case "68":
                    relationShip = "配偶的曾祖父母";
                    break;
                case "69":
                    relationShip = "其他曾祖父母";
                    break;
                case "70":
                    relationShip = "兄弟姐妹";
                    break;
                case "71":
                    relationShip = "兄";
                    break;
                case "72":
                    relationShip = "嫂";
                    break;
                case "73":
                    relationShip = "弟";
                    break;
                case "74":
                    relationShip = "弟媳";
                    break;
                case "75":
                    relationShip = "姐姐";
                    break;
                case "76":
                    relationShip = "姐夫";
                    break;
                case "77":
                    relationShip = "妹妹";
                    break;
                case "78":
                    relationShip = "妹夫";
                    break;
                case "79":
                    relationShip = "其他兄弟姐妹";
                    break;
                case "81":
                    relationShip = "伯父";
                    break;
                case "82":
                    relationShip = "伯母";
                    break;
                case "83":
                    relationShip = "叔父";
                    break;
                case "84":
                    relationShip = "婶母";
                    break;
                case "85":
                    relationShip = "舅父";
                    break;
                case "86":
                    relationShip = "舅母";
                    break;
                case "87":
                    relationShip = "姨夫";
                    break;
                case "88":
                    relationShip = "姨母";
                    break;
                case "89":
                    relationShip = "姑父";
                    break;
                case "90":
                    relationShip = "姑妈";
                    break;
                case "91":
                    relationShip = "堂兄弟姐妹";
                    break;
                case "92":
                    relationShip = "表兄弟姐妹";
                    break;
                case "93":
                    relationShip = "侄子";
                    break;
                case "94":
                    relationShip = "侄女";
                    break;
                case "96":
                    relationShip = "外甥女";
                    break;
                case "95":
                    relationShip = "外甥";
                    break;
                case "97":
                    relationShip = "其他亲属";
                    break;
                default:
                    relationShip = "非亲属";
                    break;
            }
            return relationShip;
        }

        /// <summary>
        /// 地块类别代码
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns>地块类别</returns>
        public static string CodeToDklb(string code)
        {
            string dklb;
            switch (code)
            {
                case "10":
                    dklb = "承包地块";
                    break;
                case "21":
                    dklb = "自留地";
                    break;
                case "22":
                    dklb = "机动地";
                    break;
                case "23":
                    dklb = "开荒地";
                    break;
                case "99":
                    dklb = "其他集体土地";
                    break;
                default:
                    dklb = "其他集体土地";
                    break;
            }
            return dklb;

        }

        /// <summary>
        /// 将性别代码装换成性别
        /// </summary>
        /// <param name="code">性别代码</param>
        /// <returns>性别</returns>
        public static string CodeToSex(string code)
        {
            string sex;
            switch (code)
            {
                case "1":
                    sex = "男";
                    break;
                case "2":
                    sex = "女";
                    break;
                default:
                    sex = "不详";
                    break;
            }
            return sex;
        }

        public static string CodeToSfjbnt(string code)
        {
            var jbnt = "是";
            switch (code)
            {
                case "1":
                    jbnt = "是";
                    break;
                case "2":
                    jbnt = "否";
                    break;
            }
            return jbnt;
        }


        public static string CodeToTdyt(string code)
        {
            string tdyt;
            switch (code)
            {
                case "1":
                    tdyt = "种植业";
                    break;
                case "2":
                    tdyt = "林业";
                    break;
                case "3":
                    tdyt = "畜牧业";
                    break;
                case "4":
                    tdyt = "渔业";
                    break;
                default:
                    tdyt = "非农业用途";
                    break;
            }
            return tdyt;
        }

        public static string CodeToDldj(string code)
        {
            string dldj;
            switch (code)
            {
                case "01":
                    dldj = "一等地";
                    break;
                case "02":
                    dldj = "二等地";
                    break;
                case "03":
                    dldj = "三等地";
                    break;
                case "04":
                    dldj = "四等地";
                    break;
                case "05":
                    dldj = "五等地";
                    break;
                case "06":
                    dldj = "六等地";
                    break;
                case "07":
                    dldj = "七等地";
                    break;
                case "08":
                    dldj = "八等地";
                    break;
                case "09":
                    dldj = "九等地";
                    break;
                default:
                    dldj = "十等地";
                    break;
            }
            return dldj;

        }
        #region 所有权性质下拉框选择
        /// <summary>
        /// 所有权性质下拉选项
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns>下拉选项的值</returns>
        public static int SyqyxzCombox(string code)
        {
            var selectIndex = -1;
            switch (code)
            {
                case "10":
                    selectIndex = 0;
                    break;
                case "30":
                    selectIndex = 1;
                    break;
                case "31":
                    selectIndex = 2;
                    break;
                case "32":
                    selectIndex = 3;
                    break;
                case "33":
                    selectIndex = 4;
                    break;
                case "34":
                    selectIndex = 5;
                    break;
                default:
                    break;
            }
            return selectIndex;
        }

        public static string SyqxzCombox(int index)
        {
            var code = "34";
            switch (index)
            {
                case 0:
                    code = "10";
                    break;
                case 1:
                    code = "30";
                    break;
                case 2:
                    code = "31";
                    break;
                case 3:
                    code = "32";
                    break;
                case 4:
                    code = "33";
                    break;
                case 5:
                    code = "34";
                    break;
                default:
                    break;
            }
            return code;
        }
        #endregion

        #region 地块类别下拉选项

        public static int DklbCombox(string code)
        {
            var index = -1;
            switch (code)
            {
                case "10":
                    index = 0;
                    break;
                case "21":
                    index = 1;
                    break;
                case "22":
                    index = 2;
                    break;
                case "23":
                    index = 3;
                    break;
                case "99":
                    index = 4;
                    break;
                default:
                    break;

            }
            return index;
        }

        public static string DklbCombox(int index)
        {
            string code;
            switch (index)
            {
                case 0:
                    code = "10";
                    break;
                case 1:
                    code = "21";
                    break;
                case 2:
                    code = "22";
                    break;
                case 3:
                    code = "23";
                    break;
                default:
                    code = "99";
                    break;
            }
            return code;
        }
        #endregion

        #region 土地利用类型
        //(011)水田
        //(012)水浇地
        //(013)旱地
        public static int TdlylxCombox(string code)
        {
            var index = -1;
            switch (code)
            {
                case "011":
                    index = 0;
                    break;
                case "012":
                    index = 1;
                    break;
                case "013":
                    index = 2;
                    break;
                default:
                    break;

            }
            return index;
        }

        public static string TdlylxCombox(int index)
        {
            var code = "012";
            switch (index)
            {
                case 0:
                    code = "011";
                    break;
                case 1:
                    code = "012";
                    break;
                case 2:
                    code = "013";
                    break;
                default:
                    break;

            }
            return code;
        }
        #endregion

        #region 地力等级

        public static int DldjCombox(string code)
        {
            var index = -1;
            switch (code)
            {
                case "01":
                    index = 0;
                    break;
                case "02":
                    index = 1;
                    break;
                case "03":
                    index = 2;
                    break;
                case "04":
                    index = 3;
                    break;
                case "05":
                    index = 4;
                    break;
                case "06":
                    index = 5;
                    break;
                case "07":
                    index = 6;
                    break;
                case "08":
                    index = 7;
                    break;
                case "09":
                    index = 8;
                    break;
                case "10":
                    index = 9;
                    break;
                default:
                    break;
            }
            return index;
        }

        public static string DldjCombox(int index)
        {
            var code = "01";
            switch (index)
            {
                case 0:
                    code = "01";
                    break;
                case 1:
                    code = "02";
                    break;
                case 2:
                    code = "03";
                    break;
                case 3:
                    code = "04";
                    break;
                case 4:
                    code = "05";
                    break;
                case 5:
                    code = "06";
                    break;
                case 6:
                    code = "07";
                    break;
                case 7:
                    code = "08";
                    break;
                case 8:
                    code = "09";
                    break;
                case 9:
                    code = "10";
                    break;
                default:
                    break;
            }
            return code;
        }
        #endregion

        #region 土地用途

        public static int TdytCombox(string code)
        {
            var index = -1;
            switch (code)
            {
                case "1":
                    index = 0;
                    break;
                case "2":
                    index = 1;
                    break;
                case "3":
                    index = 2;
                    break;
                case "4":
                    index = 3;
                    break;
                case "5":
                    index = 4;
                    break;
                default:
                    break;
            }
            return index;
        }

        public static string TdytCombox(int index)
        {
            string code = "1";
            switch (index)
            {
                case 0:
                    code = "1";
                    break;
                case 1:
                    code = "2";
                    break;
                case 2:
                    code = "3";
                    break;
                case 3:
                    code = "4";
                    break;
                case 4:
                    code = "5";
                    break;
                default:
                    break;
            }
            return code;
        }
        #endregion

        #region 是否基本农田

        public static int SfwjbntCombox(string code)
        {
            var index = -1;
            switch (code)
            {
                case "1":
                    index = 0;
                    break;
                case "2":
                    index = 1;
                    break;
                default:
                    break;
            }
            return index;
        }

        public static string SfwjbntCombox(int index)
        {
            var code = "1";
            switch (index)
            {
                case 0:
                    code = "1";
                    break;
                case 1:
                    code = "2";
                    break;
                default:
                    break;
            }
            return code;
        }
        #endregion

        #region 承包经营权取得方式

        public static int CbjyqqdfsCombox(string code)
        {
            int index = -1;
            switch (code)
            {
                case "100":
                    index = 0;
                    break;
                case "110":
                    index = 1;
                    break;
                case "120":
                    index = 2;
                    break;
                case "121":
                    index = 3;
                    break;
                case "122":
                    index = 4;
                    break;
                case "123":
                    index = 5;
                    break;
                case "129":
                    index = 6;
                    break;
                case "200":
                    index = 7;
                    break;
                case "300":
                    index = 8;
                    break;
                case "900":
                    index = 9;
                    break;
                default:
                    break;
            }
            return index;
        }

        public static string CbjyqqdfsCombox(int index)
        {
            string code = "110";
            switch (index)
            {
                case 0:
                    code = "100";
                    break;
                case 1:
                    code = "110";
                    break;
                case 2:
                    code = "120";
                    break;
                case 3:
                    code = "121";
                    break;
                case 4:
                    code = "122";
                    break;
                case 5:
                    code = "123";
                    break;
                case 6:
                    code = "129";
                    break;
                case 7:
                    code = "200";
                    break;
                case 8:
                    code = "300";
                    break;
                case 9:
                    code = "900";
                    break;
                default:
                    break;

            }
            return code;
        }
        #endregion

        #region 证件类型转换

        /// <summary>
        /// 发包方证件类型转换
        /// </summary>
        /// <param name="code">证件类型代码</param>
        /// <returns>证件类型</returns>
        public static string Fbfzjlx(string code)
        {
            string zjlx;
            switch (code)
            {
                case "1":
                    zjlx = "☑身份证□军官证□护照□户口簿□其他____";
                    break;
                case "2":
                    zjlx = "□身份证☑军官证□护照□户口簿□其他____";
                    break;
                case "4":
                    zjlx = "□身份证□军官证□护照☑户口簿□其他____";
                    break;
                case "5":
                    zjlx = "□身份证□军官证☑护照□户口簿□其他____";
                    break;
                default:
                    zjlx = "□身份证□军官证□护照□户口簿☑其他____";
                    break;
            }
            return zjlx;
        }

        public static string Cbfzjlx(string code)
        {
            var zjlx = string.Empty;
            switch (code)
            {
                case "1":
                    zjlx = "☑身份证 □军官证 □护照 □行政、企事业单位机构代码证或法人代码证 \n  □户口簿□其他____";
                    break;
                case "2":
                    zjlx = "□身份证 ☑军官证 □护照 □行政、 企事业单位机构代码证或法人代码证 \n  □户口簿□其他____";
                    break;
                case "3":
                    zjlx = "□身份证 □军官证 □护照 ☑行政、企事业单位机构代码证或法人代码证 \n  □户口簿□其他____";
                    break;
                case "4":
                    zjlx = "□身份证 □军官证 □护照 □行政、企事业单位机构代码证或法人代码证 \n  ☑户口簿□其他____";
                    break;
                case "5":
                    zjlx = "□身份证 □军官证 ☑护照 □行政、企事业单位机构代码证或法人代码证 \n  □户口簿□其他____";
                    break;
                default:
                    zjlx = "□身份证 □军官证 □护照 □行政、 企事业单位机构代码证或法人代码证 \n  ☑户口簿□其他____";
                    break;
            }
            return zjlx;
        }

        public static string ComboxFbfzjlx(int index)
        {
            string zjlx = string.Empty;
            switch (index)
            {
                case 0:
                    zjlx = "1";
                    break;
                case 1:
                    zjlx = "2";
                    break;
                case 2:
                    zjlx = "3";
                    break;
                case 3:
                    zjlx = "4";
                    break;
                case 4:
                    zjlx = "5";
                    break;
                default:
                    break;

            }
            return zjlx;
        }
        #endregion
    }
}
