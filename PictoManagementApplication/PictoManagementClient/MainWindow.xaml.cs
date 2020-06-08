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
            List<ImageItem> imagesToShow = new List<ImageItem>();
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
                        // Faltaría crear las imágenes que forman el dashboard y mostrarlas
                        System.Drawing.Image[] dashboardsPreview = new System.Drawing.Image[0];

                        foreach (System.Drawing.Image img in dashboardsPreview)
                        {
                            imagesToShow.Add(new ImageItem("dashtitle", img, false));
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
                        System.Drawing.Image givenImage = dataAccess.GetImageFromFolder(img.Title);
                        imagesToShow.Add(new ImageItem(img.Title, givenImage, false));
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
                    // Faltaría crear las imágenes que forman el dashboard y mostrarlas
                    // En este caso merecería la pena tener una carpeta con las imagenes ya creadas de los dashboards
                    System.Drawing.Image[] dashboardsPreview = new System.Drawing.Image[0];

                    foreach (System.Drawing.Image img in dashboardsPreview)
                    {
                        imagesToShow.Add(new ImageItem("dashtitle", img, false));
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
                        imagesToShow.Add(new ImageItem(img, localImage, false));
                    }
                    // Faltaría mostrar las imágenes recibidas
                }

            }
            finally
            {
                imagesToShow = imagesToShow.OrderBy(o => o.title).ToList();
                list_Images.ItemsSource = imagesToShow;
            }
            
        }

        private void NewImagesDashboard_Click(object sender, RoutedEventArgs e)
        {
            string requestedImages = ImagesSearchbox.Text;
            string[] requestImages = requestedImages.Split(' ');
            List<System.Drawing.Image> imagesList = new List<System.Drawing.Image>();
            List<ImageItem> imageItems = new List<ImageItem>();

            try
            {
                Controller.BusinessLayer businessLayer = new Controller.BusinessLayer(dataAccess.ConfigDictionary["Address"], Int32.Parse(dataAccess.ConfigDictionary["Port"]));
                PictoManagementVocabulary.Image[] imagesReceived = businessLayer.RequestImages(requestImages);
                foreach (PictoManagementVocabulary.Image img in imagesReceived)
                {
                    dataAccess.SaveNewImage(img.Title, img.FileBase64);
                    System.Drawing.Image givenImage = dataAccess.GetImageFromFolder(img.Title);
                    imageItems.Add(new ImageItem(img.Title, givenImage, false));
                }
                // Faltaría mostrar las imágenes recibidas
            }

            catch
            {
                foreach (string img in requestImages)
                {
                    System.Drawing.Image localImage = dataAccess.GetImageFromFolder(img);
                    if (localImage != null)
                        imagesList.Add(localImage);
                    imageItems.Add(new ImageItem(img, localImage, false));
                }

                // Faltaría mostrar las imágenes recibidas
            }
            finally
            {
                imageItems = imageItems.OrderBy(o => o.title).ToList();
                images_forNewDashboard.ItemsSource = imageItems;
            }
        }

        private void CreateNewDashboard_Click(object sender, RoutedEventArgs e)
        {
            // Pasar la lista de la función anterior (las seleccionadas en el canvas) a un array y formar el dashboard
            // Crear la imagen y guardarlo en el fichero
            // Comprogar si se comparte o no y, en caso afirmativo, enviar al 
        }

        private void SearchExistingDashboard_Click(object sender, RoutedEventArgs e)
        {
            string[] requestedDashboards = DashboardSearchbox.Text.Split(' ');
            List<Dashboard> receivedDashboards = new List<Dashboard>();

            try
            {
                Controller.BusinessLayer businessLayer = new Controller.BusinessLayer(dataAccess.ConfigDictionary["Address"], Int32.Parse(dataAccess.ConfigDictionary["Port"]));
                foreach (string dashboardName in requestedDashboards)
                {
                    List<Dashboard> dashboards = businessLayer.GetDashboard(dashboardName);
                    if (dashboards.Count > 0)
                    {
                        foreach (Dashboard dash in dashboards)
                            receivedDashboards.Add(dash);
                    }
                }
            }

            catch
            {
                foreach (string request in requestedDashboards)
                {
                    Dashboard dashboard = dataAccess.GetDashboardByName(request);
                    if (dashboard != null)
                        receivedDashboards.Add(dashboard);
                }
            }
        }

        private void EditSelectedDashboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveSelectedDashboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MyDashboardsSearch_Click(object sender, RoutedEventArgs e)
        {
            string requestedDashboard = ModifyingDashboardSearchbox.Text;
            Dashboard dashboard = dataAccess.GetDashboardByName(requestedDashboard);
            if (dashboard != null)
            {
                // Mostrar dashboard en pantalla
            }
            else
            {
                // Mostrar mensaje estilo "No existe un tablero con ese nombre"
            }
        }

        private void SearchImagesModifying_Click(object sender, RoutedEventArgs e)
        {
            string requestedImages = ImagesSearchbox.Text;
            string[] requestImages = requestedImages.Split(' ');
            List<System.Drawing.Image> imagesList = new List<System.Drawing.Image>();
            try
            {
                Controller.BusinessLayer businessLayer = new Controller.BusinessLayer(dataAccess.ConfigDictionary["Address"], Int32.Parse(dataAccess.ConfigDictionary["Port"]));
                PictoManagementVocabulary.Image[] imagesReceived = businessLayer.RequestImages(requestImages);
                foreach (PictoManagementVocabulary.Image img in imagesReceived)
                {
                    dataAccess.SaveNewImage(img.Title, img.FileBase64);
                }
                // Faltaría mostrar las imágenes recibidas
            }

            catch
            {
                foreach (string img in requestImages)
                {
                    System.Drawing.Image localImage = dataAccess.GetImageFromFolder(img);
                    if (localImage != null)
                        imagesList.Add(localImage);
                }

                // Faltaría mostrar las imágenes recibidas
            }
        }

        private void ModifyMyDashboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveModifiedDashboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SeeMyDashboards_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class ImageItem
    {
        public string title { get; set; }
        public System.Drawing.Image image { get; set; }
        public bool include { get; set; }

        public ImageItem(string _title, System.Drawing.Image _image, bool _include)
        {
            title = _title;
            image = _image;
            include = _include;
        }
    }
}
