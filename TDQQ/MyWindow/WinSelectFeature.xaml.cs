using System;
using System.Windows;
using System.Windows.Input;
using TDQQ.AE;

namespace TDQQ.MyWindow
{
    /// <summary>
    /// Interaction logic for WinSelectFeature.xaml
    /// </summary>
    public partial class WinSelectFeature : Window
    {
        public WinSelectFeature()
        {
            InitializeComponent();
        }
        public string PersonDatabase { get; set; }
        public string SelectFeature { get; set; } 
        public WinSelectFeature(string personDatabase)
        {
            PersonDatabase = personDatabase;
            SelectFeature = string.Empty;
            InitializeComponent();
            InitControl();
        }
        private void InitControl()
        {
            this.ImageClose.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => this.Close();
            FillCombox();
            this.ButtonConfirm.Click += delegate(object sender, RoutedEventArgs e)
            {
                try
                {
                    SelectFeature = SelectFeauture.SelectedItem.ToString();
                    if (string.IsNullOrEmpty(SelectFeature)) return;
                    this.DialogResult = true;
                }
                catch (Exception)
                {
                    return;
                }

            };
        }
        private void FillCombox()
        {
            //try
            //{
            //    IWorkspaceFactory pWsFt = new AccessWorkspaceFactoryClass();
            //    IWorkspace pWs = pWsFt.OpenFromFile(PersonDatabase, 0);
            //    IEnumDataset pEDataset = pWs.get_Datasets(esriDatasetType.esriDTAny);
            //    IDataset pDataset = pEDataset.Next();
            //    while (pDataset != null)
            //    {
            //        if (pDataset.Type == esriDatasetType.esriDTFeatureClass)
            //        {
            //            SelectFeauture.Items.Add(pDataset.Name);
            //        }
            //        //如果是数据集
            //        else if (pDataset.Type == esriDatasetType.esriDTFeatureDataset)
            //        {
            //            IEnumDataset pESubDataset = pDataset.Subsets;
            //            IDataset pSubDataset = pESubDataset.Next();
            //            while (pSubDataset != null)
            //            {
            //                SelectFeauture.Items.Add(pSubDataset.Name);
            //                pSubDataset = pESubDataset.Next();
            //            }
            //        }
            //        pDataset = pEDataset.Next();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    System.Windows.Forms.MessageBox.Show(ex.ToString());
            //    MessageWarning.Show("系统提示", "要素类加载失败");
            //}
            var featureclassNames = AeHelper.GetAllFeautureClass(PersonDatabase);
            foreach (var featureclassName in featureclassNames)
            {
                this.SelectFeauture.Items.Add(featureclassName);
            }
        }
        public void SetTitle(string newTile)
        {
            this.LabelTitle.Content = newTile;
        }
    }
}
