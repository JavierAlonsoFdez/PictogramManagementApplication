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
        /// Busca los tableros solicitados en el servidor
        /// </summary>
        /// <param name="businessLayer">Controlador de negocio</param>
        /// <param name="dashboardNames">Array con el nombre de los tableros buscados</param>
        /// <returns>Retorna una lista con las vistas previas del tablero</returns>
        private List<ImageItem> SearchForDashboardInServer(Controller.BusinessLayer businessLayer, string[] dashboardNames)
        {
            List<ImageItem> dashboardImages = new List<ImageItem>();
            List<Dashboard> dashboardsToProcess = new List<Dashboard>();
            List<Dashboard> dashboardsResult = new List<Dashboard>();

            foreach (string dashboardName in dashboardNames)
            {
                dashboardsToProcess.AddRange(businessLayer.GetDashboard(dashboardName));
                foreach (Dashboard dashboard in dashboardsToProcess)
                {
                    SaveImagesFromDashboard(businessLayer, dashboard);

                }
                dashboardsResult.AddRange(dashboardsToProcess);
                dashboardsToProcess = new List<Dashboard>();
            }

            foreach (Dashboard dash in dashboardsResult)
            {
                // TODO: Crear imagen en el archivo temporal de dashboards
                string dashboardPath = dataAccess.ConfigDictionary["DashboardsTemp"] + dash.Title + ".png";
                dashboardImages.Add(new ImageItem(dash.Title, dashboardPath, false));
            }

            return dashboardImages;
        }

        /// <summary>
        /// Guarda todas las imagenes de los dashboard que vienen del servidor como imagenes temporales
        /// </summary>
        /// <param name="businessLayer">Controlador de negocio</param>
        /// <param name="dashboard">Objeto tablero</param>
        private void SaveImagesFromDashboard (Controller.BusinessLayer businessLayer, Dashboard dashboard)
        {
            foreach (PictoManagementVocabulary.Image image in dashboard.Images)
            {
                string[] title = new string[1];
                title[0] = image.Title;
                List<ImageItem> imageItems = new List<ImageItem>(); SearchForImagesLocally(title);
                if (imageItems.Count > 0)
                {
                    continue;
                }
                else
                {
                    imageItems = SearchForImagesInServer(businessLayer, title);
                }
            }
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

        private void SearchImages (ref List<ImageItem> itemsToShow, string[] search)
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

        public string[] PrepareTextForSearching (string searchText)
        {
            // Se busca el texto separado por comas, si no tiene comas el texto, se separa por espacios
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

            return search;
        }

        public void PrepareDashboardForServer(string title, List<ImageItem> imagesToDashboard)
        {
            string dashboardRequest = title + ",";
            foreach (ImageItem imageItem in imagesToDashboard)
            {
                dashboardRequest = dashboardRequest + imageItem.title;
            }

            try
            {
                Controller.BusinessLayer businessLayer = new Controller.BusinessLayer(dataAccess.ConfigDictionary["Address"],
                Int32.Parse(dataAccess.ConfigDictionary["Port"]));
                businessLayer.SendDashboard(dashboardRequest);
            }
            catch
            {
                // Mostrar mensaje de no conexion posible
            }
        }

        public Dashboard GetDashboardFromTemporalDatabase(string title)
        {
            return dataAccess.GetDashboardFromTemporalList(title);
        }

        public List<ImageItem> GetAllDashboards()
        {
            List<ImageItem> imageItems = new List<ImageItem>();
            foreach (Dashboard dashboard in dataAccess.Dashboards)
            {
                string dashboardPath = dataAccess.ConfigDictionary["DashboardsFolder"] + dashboard.Title + ".png";
                string secondaryDashboardPath = dataAccess.ConfigDictionary["DashboardsTemp"] + dashboard.Title + ".png";
                if (File.Exists(dashboardPath))
                {
                    imageItems.Add(new ImageItem(dashboard.Title, dashboardPath, false));
                }
                else if (File.Exists(secondaryDashboardPath))
                {
                    imageItems.Add(new ImageItem(dashboard.Title, secondaryDashboardPath, false));
                }
            }

            return imageItems;
        }

        /* --- 
         * 
         *  FUNCIONES PROPIAS DE LA VISTA
         * 
         * 
         * --- */

        /*  ------------ BUSQUEDA ------------ */

        // Busqueda de la pestaña uno, genera una lista de imagenes o de vistas previas de tableros
        private void MainSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchText = MainSearchbox.Text;
            string[] search = PrepareTextForSearching(searchText);
            
            List<ImageItem> itemsToShow = new List<ImageItem>();

            // Si el checkbox está checkeado, se buscan dashboards
            if (ImagesOrDashboards.IsChecked == true)
            {
                itemsToShow.AddRange(SearchForDashboardLocally(search));
                try
                {
                    Controller.BusinessLayer businessLayer = new Controller.BusinessLayer(dataAccess.ConfigDictionary["Address"],
                            Int32.Parse(dataAccess.ConfigDictionary["Port"]));
                    itemsToShow.AddRange(SearchForDashboardInServer(businessLayer, search));
                }
                // Aqui quizas seria mejor mostrar un mensaje Toast
                catch (Exception exc)
                {
                    throw exc;
                }
            }
            // Si el checkbox no está checkeado, se buscan imagenes
            else
            {
                SearchImages(ref itemsToShow, search);
            }

            itemsToShow = itemsToShow.OrderBy(o => o.title).ToList();
            list_Images.ItemsSource = itemsToShow;

            MainSearchbox.Text = "";
        }

        /*  ------------ CREAR TABLERO DESDE CERO ------------ */

        // Muestra las imagenes buscadas en la lista
        private void NewImagesDashboard_Click(object sender, RoutedEventArgs e)
        {
            string searchText = ImagesSearchbox.Text;
            string[] search = PrepareTextForSearching(searchText);

            List<ImageItem> itemsToShow = new List<ImageItem>();

            SearchImages(ref itemsToShow, search);

            itemsToShow = itemsToShow.OrderBy(o => o.title).ToList();
            images_forNewDashboard.ItemsSource = itemsToShow;
            CreateDashboard.IsEnabled = true;
        }

        // Crea un dashboard y lo guarda en imagen
        private void CreateNewDashboard_Click(object sender, RoutedEventArgs e)
        {
            List<ImageItem> imagesFromList = ((IEnumerable<ImageItem>)this.images_forNewDashboard.ItemsSource).ToList();
            List<ImageItem> imagesToDashboard = new List<ImageItem>();
            string dashboardPath = dataAccess.ConfigDictionary["DashboardsFolder"] + NewDashboardTitle.Text + ".png";

            foreach (ImageItem item in imagesFromList)
            {
                if (item.isIncluded.IsChecked == true)
                {
                    imagesToDashboard.Add(item);
                }
            }

            if (imagesToDashboard.Count > 0)
            {
                // Crear imagen del dashboard y guardarlo en la lista

                if (ShareNewDashboard.IsChecked == true)
                {
                    PrepareDashboardForServer(NewDashboardTitle.Text, imagesToDashboard);
                }
            }

            NewDashboardTitle.Text = "";
            ImagesSearchbox.Text = "";
            images_forNewDashboard.ItemsSource = new List<ImageItem>();
            CreateDashboard.IsEnabled = false;
        }

        /*  ------------ CREAR TABLERO A PARTIR DE UNO EXISTENTE ------------ */

        private void SearchExistingDashboard_Click(object sender, RoutedEventArgs e)
        {
            string searchText = DashboardSearchbox.Text;
            string[] search = PrepareTextForSearching(searchText);

            List<ImageItem> itemsToShow = new List<ImageItem>();

            itemsToShow.AddRange(SearchForDashboardLocally(search));
            try
            {
                Controller.BusinessLayer businessLayer = new Controller.BusinessLayer(dataAccess.ConfigDictionary["Address"],
                        Int32.Parse(dataAccess.ConfigDictionary["Port"]));
                itemsToShow.AddRange(SearchForDashboardInServer(businessLayer, search));
            }
            // Aqui quizas seria mejor mostrar un mensaje Toast
            catch (Exception exc)
            {
                throw exc;
            }

            itemsToShow = itemsToShow.OrderBy(o => o.title).ToList();
            dashboards_fromServer.ItemsSource = itemsToShow;

            DashboardSearchbox.Text = "";
            EditSelectedDashboard.IsEnabled = true;
        }

        private void NewImagesExistingDashboard_Click(object sender, RoutedEventArgs e)
        {
            List<ImageItem> imagesFromList = ((IEnumerable<ImageItem>)this.dashboards_fromServer.ItemsSource).ToList();
            string searchText = SearchImagesForExistingDashboard.Text;
            string[] search = PrepareTextForSearching(searchText);

            SearchImages(ref imagesFromList, search);

            dashboards_fromServer.ItemsSource = imagesFromList;
        }

        // Selecciona un dashboard y saca sus imagenes
        private void EditSelectedDashboard_Click(object sender, RoutedEventArgs e)
        {
            List<ImageItem> dashboardImagesFromList = ((IEnumerable<ImageItem>)this.dashboards_fromServer.ItemsSource).ToList();
            List<ImageItem> imagesFromDashboard = new List<ImageItem>();
            Dashboard selectedDashboard = null;

            foreach (ImageItem dashboardItem in dashboardImagesFromList)
            {
                // Coge el primer dashboard seleccionado
                if (dashboardItem.isIncluded.IsEnabled == true)
                {
                    selectedDashboard = GetDashboardFromTemporalDatabase(dashboardItem.title);
                    break;
                }
            }

            if (selectedDashboard != null)
            {
                DashboardTitle.Text = selectedDashboard.Title;
                List<string> imageTitles = new List<string>();
                foreach (PictoManagementVocabulary.Image image in selectedDashboard.Images)
                {
                    imageTitles.Add(image.Title);
                }
                string[] imageArray = imageTitles.ToArray();
                SearchImages(ref imagesFromDashboard, imageArray);

                foreach (ImageItem imageItem in imagesFromDashboard)
                {
                    imageItem.isIncluded.IsEnabled = true;
                }

                dashboards_fromServer.ItemsSource = imagesFromDashboard;
                SearchingImagesForExistingDashboard.IsEnabled = true;
                SaveSelectedDashboard.IsEnabled = true;
            }

        }

        private void SaveSelectedDashboard_Click(object sender, RoutedEventArgs e)
        {
            List<ImageItem> imagesFromList = ((IEnumerable<ImageItem>)this.dashboards_fromServer.ItemsSource).ToList();
            List<ImageItem> imagesToDashboard = new List<ImageItem>();
            string dashboardPath = dataAccess.ConfigDictionary["DashboardsFolder"] + DashboardTitle.Text + ".png";

            foreach (ImageItem item in imagesFromList)
            {
                if (item.isIncluded.IsChecked == true)
                {
                    imagesToDashboard.Add(item);
                }
            }

            if (imagesToDashboard.Count > 0)
            {
                // Crear imagen del dashboard y guardarlo en la lista

            }

            DashboardTitle.Text = "";
            SearchImagesForExistingDashboard.Text = "";
            DashboardSearchbox.Text = "";
            dashboards_fromServer.ItemsSource = new List<ImageItem>();
            SearchingImagesForExistingDashboard.IsEnabled = false;
            EditSelectedDashboard.IsEnabled = false;
            SaveSelectedDashboard.IsEnabled = false;
        }

        /*  ------------ MODIFICAR TABLERO EXISTENTE ------------ */

        private void MyDashboardsSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchText = ModifyingDashboardSearchbox.Text;
            string[] search = PrepareTextForSearching(searchText);

            List<ImageItem> dashboards = SearchForDashboardLocally(search);

            if (dashboards.Count != 0)
            {
                dashboards = dashboards.OrderBy(o => o.title).ToList();
                dashboard_andImages.ItemsSource = dashboards;
                ModifyExistingDashboard.IsEnabled = true;
            }
            else
            {
                // Mostrar mensaje estilo "No existe un tablero con ese nombre"
            }
        }

        private void SearchImagesModifying_Click(object sender, RoutedEventArgs e)
        {
            string searchText = ImagesSearchbox.Text;
            string[] search = PrepareTextForSearching(searchText);
            List<ImageItem> dashboardImagesFromList = ((IEnumerable<ImageItem>)this.dashboard_andImages.ItemsSource).ToList();

            SearchImages(ref dashboardImagesFromList, search);

            dashboardImagesFromList = dashboardImagesFromList.OrderBy(o => o.title).ToList();
            dashboard_andImages.ItemsSource = dashboardImagesFromList;
        }

        private void ModifyMyDashboard_Click(object sender, RoutedEventArgs e)
        {
            List<ImageItem> dashboardImagesFromList = ((IEnumerable<ImageItem>)this.dashboard_andImages.ItemsSource).ToList();
            List<ImageItem> imagesFromDashboard = new List<ImageItem>();
            Dashboard selectedDashboard = null;

            foreach (ImageItem dashboardItem in dashboardImagesFromList)
            {
                // Coge el primer dashboard seleccionado
                if (dashboardItem.isIncluded.IsEnabled == true)
                {
                    selectedDashboard = GetDashboardFromTemporalDatabase(dashboardItem.title);
                    break;
                }
            }

            if (selectedDashboard != null)
            {
                ModifiedTitle.Text = selectedDashboard.Title;
                List<string> imageTitles = new List<string>();
                foreach (PictoManagementVocabulary.Image image in selectedDashboard.Images)
                {
                    imageTitles.Add(image.Title);
                }
                string[] imageArray = imageTitles.ToArray();
                SearchImages(ref imagesFromDashboard, imageArray);

                foreach (ImageItem imageItem in imagesFromDashboard)
                {
                    imageItem.isIncluded.IsEnabled = true;
                }

                dashboard_andImages.ItemsSource = imagesFromDashboard;
                SearchImagesModifying.IsEnabled = true;
                SaveModifiedDashboard.IsEnabled = true;
            }
        }

        private void SaveModifiedDashboard_Click(object sender, RoutedEventArgs e)
        {
            List<ImageItem> imagesFromList = ((IEnumerable<ImageItem>)this.dashboards_fromServer.ItemsSource).ToList();
            List<ImageItem> imagesToDashboard = new List<ImageItem>();
            string dashboardPath = dataAccess.ConfigDictionary["DashboardsFolder"] + DashboardTitle.Text + ".png";

            foreach (ImageItem item in imagesFromList)
            {
                if (item.isIncluded.IsChecked == true)
                {
                    imagesToDashboard.Add(item);
                }
            }

            if (imagesToDashboard.Count > 0)
            {
                // Crear imagen del dashboard y guardarlo en la lista

            }

            ModifyingDashboardSearchbox.Text = "";
            ModifiedTitle.Text = "";
            NewImageModifySearchbox.Text = "";
            dashboard_andImages.ItemsSource = new List<ImageItem>();
            SearchImagesModifying.IsEnabled = false;
            ModifyExistingDashboard.IsEnabled = false;
            SaveModifiedDashboard.IsEnabled = false;
        }


        /*  ------------ VER MIS TABLEROS ------------ */

        private void SeeMyDashboards_Click(object sender, RoutedEventArgs e)
        {
            List<ImageItem> imagesToShow = GetAllDashboards();
            imagesToShow = imagesToShow.OrderBy(o => o.title).ToList();

            own_Dashboards.ItemsSource = imagesToShow;
        }
    }

    public class ImageItem
    {
        public string title { get; set; }
        public string image { get; set; }
        public CheckBox isIncluded { get; set; }

        public ImageItem(string _title, string _image, bool _include)
        {
            title = _title;
            image = _image;
            this.isIncluded = new CheckBox
            {
                IsChecked = _include,
                Content = "Incluir: " + _title
            };
        }
    }
}
