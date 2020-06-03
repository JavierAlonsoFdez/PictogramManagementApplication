using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PictoManagementVocabulary;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PictoManagementClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataAccessLayer.DataAccess dataAccess;

        public MainWindow()
        {
            InitializeComponent();
            dataAccess = new DataAccessLayer.DataAccess();
        }

        private void MainSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchingImages = MainSearchbox.Text;
            // En caso de haber conexión
            try
            {
                Controller.BusinessLayer businessLayer = new Controller.BusinessLayer(dataAccess.ConfigDictionary["Address"], Int32.Parse(dataAccess.ConfigDictionary["Port"]));
                

                if (ImagesOrDashboards.IsChecked == true)
                {
                    string[] requestDashboards = searchingImages.Split(' ');
                    foreach (string request in requestDashboards)
                    {
                        List<Dashboard> dashboardsReceived = businessLayer.GetDashboard(request);
                        if (dashboardsReceived.Count > 0)
                        {
                            dataAccess.IncludeMultipleDashboardInList(dashboardsReceived);
                        }
                    }

                }

                else
                {
                    string[] requestImages = searchingImages.Split(' ');
                    PictoManagementVocabulary.Image[] imagesReceived = businessLayer.RequestImages(requestImages);
                    foreach (PictoManagementVocabulary.Image img in imagesReceived)
                    {
                        dataAccess.SaveNewImage(img.Title, img.FileBase64);
                    }
                }
            }
            
            catch
            {
                if (ImagesOrDashboards.IsChecked == true)
                {
                    string[] requestDashboards = searchingImages.Split(' ');
                    List<Dashboard> dashboardsResult = new List<Dashboard>();
                    foreach (string request in requestDashboards)
                    {
                        Dashboard dashboard = dataAccess.GetDashboardByName(request);
                        if (dashboard != null)
                            dashboardsResult.Add(dashboard);
                    }
                }

                else
                {
                    string[] requestImages = searchingImages.Split(' ');
                    List<System.Drawing.Image> imagesList = new List<System.Drawing.Image>();
                    foreach (string img in requestImages)
                    {
                        System.Drawing.Image localImage = dataAccess.GetImageFromFolder(img);
                        if (localImage != null)
                            imagesList.Add(localImage);
                    }
                }

            }
            
        }

        private void NewImagesDashboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CreateNewDashboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SearchExistingDashboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditSelectedDashboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveSelectedDashboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MyDashboardsSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SearchImagesModifying_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ModifyMyDashboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveModifiedDashboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditMyDashboard_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
