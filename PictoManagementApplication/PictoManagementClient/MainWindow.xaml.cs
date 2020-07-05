using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using PictoManagementVocabulary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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

        

        /// <summary>
        /// Busca imagenes en el directorio local de imagenes
        /// </summary>
        /// <param name="requestedImages">Array con el titulo de las imagenes a buscar</param>
        /// <returns>Lista de objetos con las imagenes</returns>
        private List<ImageItem> SearchForImagesLocally(string[] requestedImages)
        {
            List<ImageItem> localImages = new List<ImageItem>();
            foreach (string img in requestedImages)
            {
                string searchPath = dataAccess.ConfigDictionary["Images"] + img + ".png";
                if (File.Exists(searchPath))
                {
                    localImages.Add(new ImageItem(img, searchPath, false));
                }
            }

            return localImages;
        }

        /// <summary>
        /// Busca imagenes en el servidor, las guarda en el directorio temporal y las pasa a la lista de imagenes a mostrar
        /// </summary>
        /// <param name="businessLayer">Controlador de negocio</param>
        /// <param name="images">Array de imagenes a buscar</param>
        /// <returns>Lista de imagenes a mostrar en la lista</returns>
        private List<ImageItem> SearchForImagesInServer(Controller.BusinessLayer businessLayer, string[] images)
        {
            List<ImageItem> serverImages = new List<ImageItem>();
            PictoManagementVocabulary.Image[] imagesFromServer = businessLayer.RequestImages(images);

            foreach (PictoManagementVocabulary.Image img in imagesFromServer)
            {
                dataAccess.SaveNewTemporalImage(img.Title, img.FileBase64);
                string imgPath = dataAccess.ConfigDictionary["Temp"] + img.Title + ".png";
                if (File.Exists(imgPath))
                {
                    serverImages.Add(new ImageItem(img.Title, imgPath, false));
                }
            }

            return serverImages;
        }

        /// <summary>
        /// Busca los tableros de forma local
        /// </summary>
        /// <param name="dashboardNames">Array con el contenido a buscar</param>
        /// <returns>Lista de resultados encontrados</returns>
        private List<ImageItem> SearchForDashboardLocally(string[] dashboardNames)
        {
            List<ImageItem> dashboardImages = new List<ImageItem>();
            List<Dashboard> dashboardsResult = new List<Dashboard>();

            foreach (string dashboardName in dashboardNames)
            {
                Dashboard dashboard = dataAccess.GetDashboardByName(dashboardName);

                if (dashboard != null)
                {
                    dashboardsResult.Add(dashboard);
                }
                else
                {
                    List<Dashboard> dashboardByContent = dataAccess.GetDashboardByContent(dashboardName);
                    if (dashboardByContent != null)
                    {
                        dashboardsResult.AddRange(dashboardByContent);
                    }
                }
            }

            if (dashboardsResult.Count() > 0)
            {
                foreach (Dashboard dash in dashboardsResult)
                {
                    string dashboardPath = dataAccess.ConfigDictionary["DashboardsFolder"] + dash.Title + ".png";
                    dashboardImages.Add(new ImageItem(dash.Title, dashboardPath, false));
                }

                return dashboardImages;
            }

            return null;
        }

        /// <summary>
        /// Elimina todos los elementos ya existentes en una lista del array de busqueda
        /// </summary>
        /// <param name="list">Lista con los elementos ya buscados</param>
        /// <param name="requestedImages">Elementos a buscar</param>
        /// <returns>Array con los elementos que aun no se han encontrado</returns>
        private string[] leftImages(List<ImageItem> list, string[] requestedImages)
        {
            foreach (ImageItem item in list)
            {
                if (requestedImages.Contains(item.title))
                {
                    int aux = Array.IndexOf(requestedImages, item.title);
                    requestedImages = requestedImages.Where(w => w != requestedImages[aux]).ToArray();
                }
            }

            if (requestedImages.Count() > 0)
            {
                return requestedImages;
            }
            return null;
        }

        private void MainSearch_Click(object sender, RoutedEventArgs e)
        {
            // Se busca el texto separado por comas, si no tiene comas el texto, se separa por espacios
            string searchText = MainSearchbox.Text;
            var listOfString = new List<string>();
            string[] search = listOfString.ToArray();
            if (searchText.Contains(","))
            {
                search = searchText.Split(',');
            }
            else
            {
                search = searchText.Split(' ');
            }
            
            List<ImageItem> itemsToShow = new List<ImageItem>();

            // Si el checkbox está checkeado, se buscan dashboards
            if (ImagesOrDashboards.IsChecked == true)
            {
                itemsToShow.AddRange(SearchForDashboardLocally(search));
            }
            // Si el checkbox no está checkeado, se buscan imagenes
            else
            {
                itemsToShow.AddRange(SearchForImagesLocally(search));
                string[] searchServer = leftImages(itemsToShow, search);
                if (searchServer != null)
                {
                    try
                    {
                        Controller.BusinessLayer businessLayer = new Controller.BusinessLayer(dataAccess.ConfigDictionary["Address"], 
                            Int32.Parse(dataAccess.ConfigDictionary["Port"]));
                        itemsToShow.AddRange(SearchForImagesInServer(businessLayer, searchServer));
                    }
                    // Aqui quizas seria mejor mostrar un mensaje Toast
                    catch (Exception exc)
                    {
                        throw exc; 
                    }
                }
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
            List<ImageItem> imagesToInclude = new List<ImageItem>();
            foreach (var image in images_forNewDashboard.ItemsSource)
            {
                var imageItem = image as ImageItem;
                if (imageItem.include == true)
                {
                    imagesToInclude.Add(imageItem);
                }
            }


            PictoManagementVocabulary.Image[] images = new PictoManagementVocabulary.Image[imagesToInclude.Count];
            foreach (ImageItem temp in imagesToInclude)
            {
                dataAccess.SaveImageFile(temp.title, temp.image);
                string imagePath = dataAccess.ConfigDictionary["Images"] + temp.title + ".png";
                images[imagesToInclude.IndexOf(temp)] = new PictoManagementVocabulary.Image(temp.title, imagePath);
            }

            Dashboard newDashboard = new Dashboard(NewDashboardTitle.Text, images);
            // TODO: Crear la imagen que compone el tablero
            
            // Comprogar si se comparte o no y, en caso afirmativo, enviar al 

            if (ShareNewDashboard.IsChecked == true)
            {
                try
                {
                    Controller.BusinessLayer businessLayer = new Controller.BusinessLayer(dataAccess.ConfigDictionary["Address"], Int32.Parse(dataAccess.ConfigDictionary["Port"]));
                    string dashboardContent = newDashboard.Title + ",";
                    foreach (PictoManagementVocabulary.Image img in newDashboard.Images)
                    {
                        dashboardContent += img.Title + ",";
                    }
                    businessLayer.SendDashboard(dashboardContent);
                }
                catch
                {
                    // TODO: Mostrar de alguna manera el error, por ejemplo en una ventana con el texto tipo "No puede conectarse con el servidor"
                }
            }
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
                dashboards_fromServer.ItemsSource = receivedDashboards;
            }
        }

        private void EditSelectedDashboard_Click(object sender, RoutedEventArgs e)
        {
            // Recoger la lista de dashboards elegida, elegir el seleccionado como a editar y solicitar sus imagenes al server
        }

        private void SaveSelectedDashboard_Click(object sender, RoutedEventArgs e)
        {
            // Recoger todas las imagenes seleccionadas como true en la pantalla y crear el dashboard con el título de la barra de arriba
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
                    dataAccess.SaveNewTemporalImage(img.Title, img.FileBase64);
                }
                
            }

            catch
            {
                foreach (string img in requestImages)
                {
                    System.Drawing.Image localImage = dataAccess.GetImageFromFolder(img);
                    if (localImage != null)
                        imagesList.Add(localImage);
                }

                
            }
            
        }

        private void ModifyMyDashboard_Click(object sender, RoutedEventArgs e)
        {
            // Recoger todos los items mostrados en pantalla, si UNO está a true solicitar sus imágenes y realizar la edición
        }

        private void SaveModifiedDashboard_Click(object sender, RoutedEventArgs e)
        {
            // Recoger todas las imagenes a true y formar con ello el tablero nuevo, sustituyendo al antiguo
        }

        private void SeeMyDashboards_Click(object sender, RoutedEventArgs e)
        {
            List<ImageItem> imagesToShow = new List<ImageItem>();
            foreach (Dashboard dash in dataAccess.Dashboards)
            {
                string folderPath = dataAccess.ConfigDictionary["Dashboards"] + dash.Title + ".png";
                if (File.Exists(folderPath))
                {
                    ImageItem imgItem = new ImageItem(dash.Title, folderPath, false);
                    imagesToShow.Add(imgItem);
                }
            }

            imagesToShow = imagesToShow.OrderBy(o => o.title).ToList();
            own_Dashboards.ItemsSource = imagesToShow;
        }
    }

    public class ImageItem
    {
        public string title { get; set; }
        public string image { get; set; }
        public bool include { get; set; }

        public ImageItem(string _title, string _image, bool _include)
        {
            title = _title;
            image = _image;
            include = _include;
        }

        // Cambiar todos los ImageItem para tener string y no Image
    }
}
