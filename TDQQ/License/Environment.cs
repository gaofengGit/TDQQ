using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using TDQQ.MyWindow;

namespace TDQQ.License
{
    class Environment
    {
        /// <summary>
        /// 获取CPU型号
        /// </summary>
        /// <returns></returns>
        private static string UniqueId()
        {
           // return CpuId() + BiosId() + DiskId() + BaseId();
            return CpuId()+BiosId() + DiskId() + BaseId();
        }
        private static string Identifier(string wmiClass, string wmiProperty)
        {
            string result = "";
            System.Management.ManagementClass mc =new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                //Only get the first one
                if (result == "")
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                    }
                }
            }
            return result;
        }        
        private static string CpuId()
        {
            //Uses first CPU identifier available in order of preference
            //Don't get all identifiers, as it is very time consuming
            string retVal = Identifier("Win32_Processor", "UniqueId");
            if (retVal == "") //If no UniqueID, use ProcessorID
            {
                retVal = Identifier("Win32_Processor", "ProcessorId");
                if (retVal == "") //If no ProcessorId, use Name
                {
                    retVal = Identifier("Win32_Processor", "Name");
                    if (retVal == "") //If no Name, use Manufacturer
                    {
                        retVal = Identifier("Win32_Processor", "Manufacturer");
                    }
                }
            }
            return retVal;
        }
        private static string BiosId()
        {
            return Identifier("Win32_BIOS", "Manufacturer")
            + Identifier("Win32_BIOS", "SMBIOSBIOSVersion")
            + Identifier("Win32_BIOS", "IdentificationCode")
            + Identifier("Win32_BIOS", "SerialNumber")
            + Identifier("Win32_BIOS", "ReleaseDate")
            + Identifier("Win32_BIOS", "Version");
        }
        private static string DiskId()
        {
            return Identifier("Win32_DiskDrive", "Model")
            + Identifier("Win32_DiskDrive", "Manufacturer")
            + Identifier("Win32_DiskDrive", "Signature")
            + Identifier("Win32_DiskDrive", "TotalHeads");
        }
        private static string BaseId()
        {
            return Identifier("Win32_BaseBoard", "Model")
            + Identifier("Win32_BaseBoard", "Manufacturer")
            + Identifier("Win32_BaseBoard", "Name")
            + Identifier("Win32_BaseBoard", "SerialNumber");
        }

        private static string MacId()
        {
            /*
            ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection queryCollection = query.Get();
            string mac=string.Empty;
            IEnumerable<string> 
            foreach (ManagementObject mo in queryCollection)
            {
                if (mo["IPEnabled"].ToString() == "True")
                {
                   
                }
                    mac += mo["MacAddress"].ToString();
            }
            return mac;
             */
            var macList = MacList();
            var list= macList.ToList();
            list.Sort();
            string code = string.Empty;
            for (int i = 0; i < list.Count; i++)
            {
                code += list[i];
            }
            return code;

        }

        private static IEnumerable<string> MacList()
        {
            ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection queryCollection = query.Get();
            foreach (ManagementObject mo in queryCollection)
            {
                if (mo["IPEnabled"].ToString() == "True")
                    yield return mo["MacAddress"].ToString();
            }
        } 
       
        /// <summary>
        /// 从注册文件中获取注册值
        /// </summary>
        /// <returns></returns>
        private static string LicFileValue()
        {
            string licPath = AppDomain.CurrentDomain.BaseDirectory + @"\TDQQ.lic";
            //如果文件不存在，则返回Null
            if (!File.Exists(licPath))return null;
            using (FileStream fileStream=new FileStream(licPath,FileMode.Open,FileAccess.Read))
            {
                StreamReader streamReader=new StreamReader(fileStream);
                var zcm = streamReader.ReadLine();
                streamReader.Close();
                fileStream.Close();
                return zcm;
            }
        }


        public static bool CheckLic(ref string cpuCode)
        {
            //System.Windows.Forms.MessageBox.Show(MacId());
            string macid = MacId();
            if (string.IsNullOrEmpty(macid))
            {
                return false;
            }
            cpuCode = Security.Md5Encrypt(MacId());
            string zcm = cpuCode + "gaufung";
            string md5 = Security.Md5Encrypt(zcm);
            string readMd5 = LicFileValue();
            //WinLicense winLicense = new WinLicense(cpuCode);
            if(string.IsNullOrEmpty(readMd5))
            {
               // winLicense.ShowDialog();
                //System.Windows.Forms.MessageBox.Show("注册码为：" + cpuCode);
                return false;
            }
            if (readMd5!=md5)
            {
               // System.Windows.Forms.MessageBox.Show("注册文件不正确 \n" + "注册码为：" + cpuCode);
               // winLicense.ShowDialog();
                return false;
            }
            return true;
        }

        public static bool CutoffTime()
        {
            int year = 2015;
            int month = 2;
            int day = 14;
            DateTime cutofftime=new DateTime(year,month,day);
            DateTime systemTime = System.DateTime.Now;
            return cutofftime > systemTime ? true : false;
        }
    }
}
