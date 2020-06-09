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

        private List<ImageItem> SearchForImagesInServer(Controller.BusinessLayer businessLayer, string[] images)
        {
            List<ImageItem> imagesToShow = new List<ImageItem>();

            PictoManagementVocabulary.Image[] imagesReceived = businessLayer.RequestImages(requestImages);
            foreach (PictoManagementVocabulary.Image img in images)
            {
                dataAccess.SaveNewImage(img.Title, img.FileBase64);
                System.Drawing.Image givenImage = dataAccess.GetImageFromFolder(img.Title);
                imagesToShow.Add(new ImageItem(img.Title, givenImage, false));
            }

            return imagesToShow;
        }

        private List<ImageItem> SearchForImagesLocally(string[] requestedImages)
        {
            List<ImageItem> imagesToShow = new List<ImageItem>();
            foreach (string img in requestImages)
            {
                System.Drawing.Image localImage = dataAccess.GetImageFromFolder(img);
                if (localImage != null)
                    imagesToShow.Add(new ImageItem(img, localImage, false));
            }

            return imagesToShow;
        }

        private List<ImageItem> SearchForDashboardInServer(Controller.BusinessLayer businessLayer, string dashboardName)
        {
            List<ImageItem> imagesToShow = new List<ImageItem>();


            List<Dashboard> dashboardsReceived = businessLayer.GetDashboard(dashboardName);
            if (dashboardsReceived.Count > 0)
            {
                dataAccess.IncludeMultipleDashboardInList(dashboardsReceived);
            }
            // TODO: Creación de imágenes de previsualización del dashboard
            System.Drawing.Image[] dashboardsPreview = new System.Drawing.Image[0];

            foreach (System.Drawing.Image img in dashboardsPreview)
            {
                imagesToShow.Add(new ImageItem("dashtitle", img, false));
            }

            return imagesToShow;
        }

        private List<ImageItem> SearchForDashboardLocally(string dashboardName)
        {
            List<ImageItem> imagesToShow = new List<ImageItem>();
            List<Dashboard> dashboardsResult = new List<Dashboard>();
            Dashboard dashboard = dataAccess.GetDashboardByName(request);
            // TODO: GetDashboardByName debería devolver por un contains y devolver una lista de dashboards
            if (dashboard != null)
                dashboardsResult.Add(dashboard);
            
            // TODO: Para los dashboards en la lista de dashboardsResult, sacar imagen de previsualización
            System.Drawing.Image[] dashboardsPreview = new System.Drawing.Image[0];

            foreach (System.Drawing.Image img in dashboardsPreview)
            {
                imagesToShow.Add(new ImageItem("dashtitle", img, false));
            }

            return imagesToShow;
        }

        private void MainSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchingImages = MainSearchbox.Text;
            List<ImageItem> imagesToShow = new List<ImageItem>();

            try
            {
                Controller.BusinessLayer businessLayer = new Controller.BusinessLayer(dataAccess.ConfigDictionary["Address"], Int32.Parse(dataAccess.ConfigDictionary["Port"]));
                // En este caso, existe conexión con el servidor y se usarán sus imágenes   

                if (ImagesOrDashboards.IsChecked == true)
                {
                    // Como el checkbox está seteado a true, se buscarán dashboards
                    imagesToShow = SearchForDashboardInServer(businessLayer, searchingImages);
                }

                else
                {
                    // El checkbox no está checkeado, se buscarán imágenes
                    string[] requestImages = searchingImages.Split(' ');
                    imagesToShow = SearchForImagesInServer(businessLayer, requestImages);
                }
            }
            
            catch
            {
                if (ImagesOrDashboards.IsChecked == true)
                {
                    // Como el checkbox está seteado a true, se buscarán dashboards
                    imagesToShow = SearchForDashboardLocally(searchingImages);
                }

                else
                {
                    // El checkbox no está checkeado, se buscarán imágenes
                    string[] requestImages = searchingImages.Split(' ');
                    imagesToShow = SearchForImagesLocally(requestImages);
                }

            }
            finally
            {
                // Finalmente, sea cual sea la búsqueda, se ordena la lista y se muestra
                imagesToShow = imagesToShow.OrderBy(o => o.title).ToList();
                list_Images.ItemsSource = imagesToShow;
            }
            
        }

        private void NewImagesDashboard_Click(object sender, RoutedEventArgs e)
        {
            string requestedImages = ImagesSearchbox.Text;
            string[] requestImages = requestedImages.Split(' ');
            List<ImageItem> imageItems = new List<ImageItem>();

            try
            {
                Controller.BusinessLayer businessLayer = new Controller.BusinessLayer(dataAccess.ConfigDictionary["Address"], Int32.Parse(dataAccess.ConfigDictionary["Port"]));
                imageItems = SearchForImagesInServer(businessLayer, requestImages);
            }

            catch
            {
                imageItems = SearchForImagesLocally(requestImages);
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
            string requestedDashboard = DashboardSearchbox.Text;
            List<ImageItem> receivedDashboards = new List<ImageItem>();

            try
            {
                Controller.BusinessLayer businessLayer = new Controller.BusinessLayer(dataAccess.ConfigDictionary["Address"], Int32.Parse(dataAccess.ConfigDictionary["Port"]));
                receivedDashboards = SearchForDashboardInServer(businessLayer, requestedDashboard);
            }

            catch
            {
                receivedDashboards = SearchForDashboardLocally(requestedDashboard);
            }

            finally
            {
                receivedDashboards = receivedDashboards.OrderBy(o => o.title).ToList();
                dashboards_fromServer.ItemSource = receivedDashboards;
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
