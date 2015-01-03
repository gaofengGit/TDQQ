using System;
using System.Windows;
using ESRI.ArcGIS.esriSystem;
using TDQQ.MyWindow;
using Environment = TDQQ.License.Environment;

namespace TDQQ
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
           // KillTDQQProcess();
            string cpucode = string.Empty;
          // if (!Environment.CheckLic(ref cpucode))
           //if (false)
            if(!Environment.CutoffTime())
            {
                /*
                if (string.IsNullOrEmpty(cpucode))
                {
                    this.Shutdown();
                }
                else
                {
                    WinLicense winLicense = new WinLicense(cpucode);
                    this.StartupUri = new Uri("MyWindow/WinLicense.xaml", UriKind.RelativeOrAbsolute);
                    winLicense.ShowDialog();
                    // winLicense.Close();
                    this.Shutdown();
                }
                 */
                this.Shutdown();
            }
            else
            {
                this.StartupUri = new Uri("MainWindow.xaml", UriKind.RelativeOrAbsolute);
                base.OnStartup(e);
                ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
                InitializeEngineLicense();
            }
           
        }
        private void InitializeEngineLicense()
        {
            AoInitialize aoi = new AoInitializeClass();
            const esriLicenseProductCode productCode = esriLicenseProductCode.esriLicenseProductCodeAdvanced;
            if (aoi.IsProductCodeAvailable(productCode) == esriLicenseStatus.esriLicenseAvailable)
            {
                aoi.Initialize(productCode);
            }
        }
        public void KillTDQQProcess()
        {
            try
            {
                System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("TDQQ");
                foreach (System.Diagnostics.Process process in processes)
                {
                    bool b = process.MainWindowTitle == "";
                    if (process.MainWindowTitle == "")
                    {
                        process.Kill();
                    }
                }
            }
            catch (Exception e)
            {
                return;
            }
        }   
    }
}
