using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using PictoManagementVocabulary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PictoManagementClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Controller.DataAccess dataAccess;

        public MainWindow()
        {
            InitializeComponent();
            dataAccess = new Controller.DataAccess();
        }

        /// <summary>
        /// Busca imagenes en el directorio local de imagenes
        /// </summary>
        /// <param name="requestedImages">Array con el titulo de las imagenes a buscar</param>
        /// <returns>Lista de objetos con las imagenes</returns>
        private List<Model.ImageItem> SearchForImagesLocally(string[] requestedImages)
        {
            List<Model.ImageItem> localImages = new List<Model.ImageItem>();
            foreach (string img in requestedImages)
            {
                string searchPath = dataAccess.ConfigDictionary["Images"] + img + ".png";
                if (File.Exists(searchPath))
                {
                    this.Dispatcher.Invoke(()=>localImages.Add(new Model.ImageItem(img, searchPath, false)));
                }
                else
                {
                    string searchTemporalPath = dataAccess.ConfigDictionary["Temp"] + img + ".png";
                    if (File.Exists(searchTemporalPath))
                    {
                        this.Dispatcher.Invoke(() => localImages.Add(new Model.ImageItem(img, searchTemporalPath, false)));
                    }
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
        private List<Model.ImageItem> SearchForImagesInServer(Controller.BusinessLayer businessLayer, string[] images)
        {
            List<Model.ImageItem> serverImages = new List<Model.ImageItem>();
            PictoManagementVocabulary.Image[] imagesFromServer = businessLayer.RequestImages(images);

            foreach (PictoManagementVocabulary.Image img in imagesFromServer)
            {
                dataAccess.SaveNewTemporalImage(img.Title, img.FileBase64);
                string imgPath = dataAccess.ConfigDictionary["Temp"] + img.Title + ".png";
                if (File.Exists(imgPath))
                {
                    this.Dispatcher.Invoke(() => serverImages.Add(new Model.ImageItem(img.Title, imgPath, false)));
                }
            }

            return serverImages;
        }

        /// <summary>
        /// Busca los tableros de forma local
        /// </summary>
        /// <param name="dashboardNames">Array con el contenido a buscar</param>
        /// <returns>Lista de resultados encontrados</returns>
        private void SearchForDashboardLocally(List<Model.ImageItem> dashboardList,string[] dashboardNames)
        {
            List<Model.ImageItem> dashboardImages = new List<Model.ImageItem>();
            List<Dashboard> dashboardsResult = new List<Dashboard>();

            foreach (string dashboardName in dashboardNames)
            {

                string dashboardPath = dataAccess.ConfigDictionary["DashboardsFolder"] + dashboardName + ".png";
                if (File.Exists(dashboardPath))
                {
                    this.Dispatcher.Invoke(() => dashboardImages.Add(new Model.ImageItem(dashboardName, dashboardPath, false)));
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
                    string dashboardPath = dataAccess.ConfigDictionary["DashboardsFolder"] + dash.Name + ".png";
                    this.Dispatcher.Invoke(() => dashboardImages.Add(new Model.ImageItem(dash.Name, dashboardPath, false)));
                }
            }

            if (dashboardImages.Count() > 0)
                dashboardList.AddRange(dashboardImages);
        }

        /// <summary>
        /// Busca los tableros solicitados en el servidor
        /// </summary>
        /// <param name="businessLayer">Controlador de negocio</param>
        /// <param name="dashboardNames">Array con el nombre de los tableros buscados</param>
        /// <returns>Retorna una lista con las vistas previas del tablero</returns>
        private void SearchForDashboardInServer(List<Model.ImageItem> dashboardList, Controller.BusinessLayer businessLayer, 
            string[] dashboardNames, Canvas canvas)
        {
            List<Model.ImageItem> dashboardImages = new List<Model.ImageItem>();
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
                List<string> images = new List<string>();
                List<Model.ImageItem> imagesInDashboard = new List<Model.ImageItem>();

                foreach (PictoManagementVocabulary.Image image in dash.Images)
                {
                    images.Add(image.Title);
                }
                string[] imgArray = images.ToArray();
                SearchImages(imagesInDashboard, imgArray);

                this.Dispatcher.Invoke(() => CreateDashboard(imagesInDashboard, canvas));
                canvas.Dispatcher.Invoke(() => { }, DispatcherPriority.Render);
                this.Dispatcher.Invoke(() => SaveDashboardFromCanvas(canvas, dash.Name, true));
                dataAccess.IncludeDashboardInTemporalList(dash);

                string dashboardPath = dataAccess.ConfigDictionary["DashboardsTemp"] + dash.Name + ".png";
                this.Dispatcher.Invoke(() => dashboardImages.Add(new Model.ImageItem(dash.Name, dashboardPath, false)));
            }
            if (dashboardImages != null)
            {
                dashboardList.AddRange(dashboardImages);
            }
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
                List<Model.ImageItem> imageItems = SearchForImagesLocally(title);
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
        private string[] LeftImages(List<Model.ImageItem> list, string[] requestedImages)
        {
            foreach (Model.ImageItem item in list)
            {
                if (requestedImages.Contains(item.Title))
                {
                    int aux = Array.IndexOf(requestedImages, item.Title);
                    requestedImages = requestedImages.Where(w => w != requestedImages[aux]).ToArray();
                }
            }

            if (requestedImages.Count() > 0)
            {
                return requestedImages;
            }
            return null;
        }

        /// <summary>
        /// Realiza una busqueda de imagenes en el servidor y en local
        /// </summary>
        /// <param name="itemsToShow">Lista en la que se guardaran los resultados</param>
        /// <param name="search">Textos a buscar</param>
        private void SearchImages (List<Model.ImageItem> itemsToShow, string[] search)
        {
            itemsToShow.AddRange(SearchForImagesLocally(search));
            string[] searchServer = LeftImages(itemsToShow, search);
            if (searchServer != null)
            {
                try
                {
                    Controller.BusinessLayer businessLayer = new Controller.BusinessLayer(dataAccess.ConfigDictionary["Address"],
                        Int32.Parse(dataAccess.ConfigDictionary["Port"]));
                    itemsToShow.AddRange(SearchForImagesInServer(businessLayer, searchServer));
                }  
                catch
                {
                    string messageText = "No es posible conectarse con el servidor o no se recibe respuesta";
                    string caption = "No hay conexion";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBox.Show(messageText, caption, button, icon);
                }
            }
        }

        /// <summary>
        /// Prepara un texto en funcion de ciertos caracteres especiales para iniciar una busqueda
        /// </summary>
        /// <param name="searchText">Texto a ser preparado</param>
        /// <returns>Array de string con el texto preparado</returns>
        public string[] PrepareTextForSearching (string searchText)
        {
            // Se busca el texto separado por comas, si no tiene comas el texto, se separa por espacios
            var listOfString = new List<string>();
            string[] search = listOfString.ToArray();
            if (searchText.Contains(","))
            {
                search = searchText.Split(',');
            }
            else if (searchText.Contains(" "))
            {
                search = searchText.Split(' ');
            }
            else
            {
                search = new string[1];
                search[0] = searchText;
            }

            return search;
        }

        /// <summary>
        /// Prepara y envia el contenido de un dashboard al servidor
        /// </summary>
        /// <param name="title">Nombre del dashboard</param>
        /// <param name="imagesToDashboard">Pictogramas que contiene el dashboard</param>
        public void PrepareDashboardForServer(string title, List<Model.ImageItem> imagesToDashboard)
        {
            string dashboardRequest = title;
            foreach (Model.ImageItem imageItem in imagesToDashboard)
            {
                dashboardRequest = dashboardRequest + "," + imageItem.Title;
            }

            try
            {
                Controller.BusinessLayer businessLayer = new Controller.BusinessLayer(dataAccess.ConfigDictionary["Address"],
                Int32.Parse(dataAccess.ConfigDictionary["Port"]));
                businessLayer.SendDashboard(dashboardRequest);
            }
            catch
            {
                string messageText = "No es posible conectarse con el servidor o no se recibe respuesta";
                string caption = "No hay conexion";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBox.Show(messageText, caption, button, icon);
            }
        }

        /// <summary>
        /// Obtiene un dashboard de la lista temporal de dashboards
        /// </summary>
        /// <param name="title">Titulo del dashboard a buscar</param>
        /// <returns>Dashbaord encontrado</returns>
        public Dashboard GetDashboardFromTemporalDatabase(string title)
        {
            return dataAccess.GetDashboardFromTemporalList(title);
        }

        /// <summary>
        /// Obtiene todas las imagenes de visualizacion de los tableros y los prepara como una lista de datos
        /// </summary>
        /// <returns>Lista de datos con los tableros</returns>
        public List<Model.ImageItem> GetAllDashboards()
        {
            List<Model.ImageItem> imageItems = new List<Model.ImageItem>();
            foreach(string dashboard in Directory.GetFiles(dataAccess.ConfigDictionary["DashboardsFolder"]))
            {
                int indexOfFolder = dataAccess.ConfigDictionary["DashboardsFolder"].Length;
                string fileName = dashboard.Substring(indexOfFolder);
                string fileTitle = fileName.Split('.')[0];

                this.Dispatcher.Invoke(() => imageItems.Add(new Model.ImageItem(fileTitle, dashboard, false)));
            }
            

            return imageItems;
        }

        /// <summary>
        /// Crea un dashboard en un canvas a partir de una lista de pictogramas
        /// </summary>
        /// <param name="imageItems">Lista de pictogramas a incluir en el dashboard</param>
        /// <param name="canvas">Canvas en el que dibujar los pictogramas</param>
        public void CreateDashboard(List<Model.ImageItem> imageItems, Canvas canvas)
        {
            Canvas myCanvas = canvas;
            canvas.Opacity = 0;
            int numColumns = 5;
            int numRows;

            if (imageItems.Count < 16)
            {
                numRows = 3;
            }
            else
            {
                numRows = 5;
            }

            double left;
            double top;
            left = 0;
            top = 0;

            for (int i = 0; i < imageItems.Count; i++)
            {
                string img = imageItems[i].Image;

                Rectangle rectangle = new Rectangle
                {
                    Stretch = Stretch.Uniform,
                    Width = myCanvas.ActualWidth / numColumns,
                    Height = myCanvas.ActualHeight / numRows
                };

                ImageBrush brush = new ImageBrush
                {
                    
                    ImageSource = new BitmapImage(new Uri(img, UriKind.Absolute))
                };
                brush.Stretch = Stretch.Uniform;
                brush.Freeze();
                rectangle.Fill = brush;
                if (left + rectangle.Width > myCanvas.ActualWidth)
                {
                    left = 0;
                    top += rectangle.Height;
                }
                Canvas.SetLeft(rectangle, left);
                Canvas.SetTop(rectangle, top);
                myCanvas.Children.Add(rectangle);
                left += rectangle.Width;

                dataAccess.MoveImageFromTempToDestination(imageItems[i].Title);
            }

            canvas.Opacity = 1;
        }

        /// <summary>
        /// Guarda el contenido del canvas y lo renderiza como imagen en formato png
        /// </summary>
        /// <param name="myCanvas">Canvas del que se renderizara para guardar la imagen</param>
        /// <param name="filename">Nombre del tablero</param>
        /// <param name="temp">Indica si va a la ruta de ficheros temporales o no</param>
        public void SaveDashboardFromCanvas(Canvas myCanvas, string filename, Boolean temp)
        {
            myCanvas.Opacity = 1;
            double dpi = 96;
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)(myCanvas.ActualWidth * dpi / 96.0),
                                                            (int)(myCanvas.ActualHeight * dpi / 96.0),
                                                            dpi,
                                                            dpi, PixelFormats.Default);
            rtb.Render(myCanvas);

            PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
            pngBitmapEncoder.Frames.Add(BitmapFrame.Create(rtb));

            string filePath = "";

            if (temp)
            {
                filePath = dataAccess.ConfigDictionary["DashboardsTemp"] + filename + ".png";
            }
            else
            {
                filePath = dataAccess.ConfigDictionary["DashboardsFolder"] + filename + ".png";
            }

            int aux = 0;

            while (File.Exists(filePath))
            {
                filePath = dataAccess.ConfigDictionary["DashboardsFolder"] + filename + aux + ".png";
                aux++;
            }

            using (FileStream fs = File.OpenWrite(String.Format(filePath)))
            {
                pngBitmapEncoder.Save(fs);
                fs.Flush();
                fs.Close();
            }

            myCanvas.Children.Clear();
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
            
            List<Model.ImageItem> itemsToShow = new List<Model.ImageItem>();

            bool check = ImagesOrDashboards.IsChecked.Value;
            this.IsEnabled = false;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            Task.Run(() => {
                if (check == true)
                {
                    SearchForDashboardLocally(itemsToShow, search);
                    if (itemsToShow.Count() == 0)
                    {
                        try
                        {
                            Controller.BusinessLayer businessLayer = new Controller.BusinessLayer(dataAccess.ConfigDictionary["Address"],
                                    Int32.Parse(dataAccess.ConfigDictionary["Port"]));
                            SearchForDashboardInServer(itemsToShow, businessLayer, search, searchCanvas);
                        }
                        catch
                        {
                            string messageText = "No es posible conectarse con el servidor o no se recibe respuesta";
                            string caption = "No hay conexion";
                            MessageBoxButton button = MessageBoxButton.OK;
                            MessageBoxImage icon = MessageBoxImage.Warning;
                            MessageBox.Show(messageText, caption, button, icon);
                        }
                    }
                }
                // Si el checkbox no está checkeado, se buscan imagenes
                else
                {
                    SearchImages(itemsToShow, search);
                }

                this.Dispatcher.Invoke(() =>
                {
                    itemsToShow = itemsToShow.OrderBy(o => o.Title).ToList();
                    list_Images.ItemsSource = itemsToShow;

                    searchCanvas.Children.Clear();

                    MainSearchbox.Text = "";
                    this.IsEnabled = true;
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                });
            });
        }

        /*  ------------ CREAR TABLERO DESDE CERO ------------ */

        // Muestra las imagenes buscadas en la lista
        private void NewImagesDashboard_Click(object sender, RoutedEventArgs e)
        {
            string searchText = ImagesSearchbox.Text;
            string[] search = PrepareTextForSearching(searchText);

            List<Model.ImageItem> itemsToShow = new List<Model.ImageItem>();
            this.IsEnabled = false;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            Task.Run(() =>
            {
                SearchImages(itemsToShow, search);
                this.Dispatcher.Invoke(() =>
                {
                    itemsToShow = itemsToShow.OrderBy(o => o.Title).ToList();
                    images_forNewDashboard.ItemsSource = itemsToShow;
                    CreateDashboardButton.IsEnabled = true;
                    this.IsEnabled = true;
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                });
            });
            
        }

        // Crea un dashboard y lo guarda en imagen
        private void CreateNewDashboard_Click(object sender, RoutedEventArgs e)
        {
            List<Model.ImageItem> imagesFromList = ((IEnumerable<Model.ImageItem>)this.images_forNewDashboard.ItemsSource).ToList();
            List<Model.ImageItem> imagesToDashboard = new List<Model.ImageItem>();
            string dashboardPath = dataAccess.ConfigDictionary["DashboardsFolder"] + NewDashboardTitle.Text + ".png";

            if (File.Exists(dashboardPath))
            {
                string messageText = "No es posible crear un tablero con este nombre porque ya existe";
                string caption = "Nombre duplicado";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messageText, caption, button, icon);

                if (result == MessageBoxResult.OK)
                {
                    NewDashboardTitle.Text = "";
                    ImagesSearchbox.Text = "";
                    images_forNewDashboard.ItemsSource = new List<Model.ImageItem>();
                    return;
                }
                else
                {
                    NewDashboardTitle.Text = "";
                    ImagesSearchbox.Text = "";
                    images_forNewDashboard.ItemsSource = new List<Model.ImageItem>();
                    return;
                }
            }

            this.IsEnabled = false;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            Task.Run(() =>
            {
                foreach (Model.ImageItem item in imagesFromList)
                {
                    if (this.Dispatcher.Invoke(() => item.IsIncluded.IsChecked == true))
                    {
                        imagesToDashboard.Add(item);
                    }
                }

                if (imagesToDashboard.Count > 0)
                {
                    List<PictoManagementVocabulary.Image> images = new List<PictoManagementVocabulary.Image>();

                    foreach (Model.ImageItem imageItem in imagesToDashboard)
                    {
                        images.Add(new PictoManagementVocabulary.Image(imageItem.Title, imageItem.Image));
                    }

                    this.Dispatcher.Invoke(() => CreateDashboard(imagesToDashboard, canvasCreateZero));

                    string messageText = "Este es el tablero resultante, ¿guardar?";
                    string caption = "Tablero creado";
                    MessageBoxButton button = MessageBoxButton.OKCancel;
                    MessageBoxImage icon = MessageBoxImage.Information;
                    MessageBoxResult result = MessageBox.Show(messageText, caption, button, icon);

                    if (result == MessageBoxResult.OK)
                    {
                        this.Dispatcher.Invoke(() => SaveDashboardFromCanvas(canvasCreateZero, NewDashboardTitle.Text, false));
                        Dashboard dashboard = new Dashboard(this.Dispatcher.Invoke(() => NewDashboardTitle.Text), this.Dispatcher.Invoke(() => images.ToArray()));
                        dataAccess.IncludeDashboardInList(dashboard);

                        if (this.Dispatcher.Invoke(() => ShareNewDashboard.IsChecked == true))
                        {
                            PrepareDashboardForServer(this.Dispatcher.Invoke(() => NewDashboardTitle.Text), this.Dispatcher.Invoke(() => imagesToDashboard));
                        }
                    }
                }
                this.Dispatcher.Invoke(() =>
                {
                    NewDashboardTitle.Text = "";
                    ImagesSearchbox.Text = "";
                    images_forNewDashboard.ItemsSource = new List<Model.ImageItem>();
                    CreateDashboardButton.IsEnabled = false;
                    canvasCreateZero.Children.Clear();
                    this.IsEnabled = true;
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                });

            });
            
        }

        /*  ------------ CREAR TABLERO A PARTIR DE UNO EXISTENTE ------------ */

        private void SearchExistingDashboard_Click(object sender, RoutedEventArgs e)
        {
            string searchText = DashboardSearchbox.Text;
            string[] search = PrepareTextForSearching(searchText);

            List<Model.ImageItem> itemsToShow = new List<Model.ImageItem>();

            this.IsEnabled = false;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            Task.Run(() => { 
                try
                {
                    Controller.BusinessLayer businessLayer = new Controller.BusinessLayer(dataAccess.ConfigDictionary["Address"],
                            Int32.Parse(dataAccess.ConfigDictionary["Port"]));
                    SearchForDashboardInServer(itemsToShow, businessLayer, search, canvasCreateExisting);
                }
                catch
                {
                    string messageText = "No es posible conectarse con el servidor o no se recibe respuesta";
                    string caption = "No hay conexion";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBox.Show(messageText, caption, button, icon);
                }

                this.Dispatcher.Invoke(() =>
                {
                    itemsToShow = itemsToShow.OrderBy(o => o.Title).ToList();
                    dashboards_fromServer.ItemsSource = itemsToShow;
                    EditSelectedDashboard.IsEnabled = true;

                    this.IsEnabled = true;
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                });
            });
        }

        private void NewImagesExistingDashboard_Click(object sender, RoutedEventArgs e)
        {
            List<Model.ImageItem> imagesFromList = ((IEnumerable<Model.ImageItem>)this.dashboards_fromServer.ItemsSource).ToList();
            string searchText = SearchImagesForExistingDashboard.Text;
            string[] search = PrepareTextForSearching(searchText);

            this.IsEnabled = false;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            Task.Run(() => { 
                SearchImages(imagesFromList, search);
                this.Dispatcher.Invoke(() => 
                { 
                    dashboards_fromServer.ItemsSource = imagesFromList;
                    this.IsEnabled = true;
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                });
            });
        }

        // Selecciona un dashboard y saca sus imagenes
        private void EditSelectedDashboard_Click(object sender, RoutedEventArgs e)
        {
            DashboardSearchbox.Text = "";
            List<Model.ImageItem> dashboardImagesFromList = ((IEnumerable<Model.ImageItem>)this.dashboards_fromServer.ItemsSource).ToList();
            List<Model.ImageItem> imagesFromDashboard = new List<Model.ImageItem>();
            Dashboard selectedDashboard = null;

            this.IsEnabled = false;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            Task.Run(() => {
                foreach (Model.ImageItem dashboardItem in dashboardImagesFromList)
                {
                    if (this.Dispatcher.Invoke(()=>dashboardItem.IsIncluded.IsChecked == true))
                    {
                        selectedDashboard = dataAccess.GetDashboardFromTemporalList(dashboardItem.Title);
                        break;
                    }
                }

                if (selectedDashboard != null)
                {
                    this.Dispatcher.Invoke(() => DashboardTitle.Text = selectedDashboard.Name);
                    List<string> imageTitles = new List<string>();
                    foreach (PictoManagementVocabulary.Image image in selectedDashboard.Images)
                    {
                        imageTitles.Add(image.Title);
                    }
                    string[] imageArray = imageTitles.ToArray();
                    SearchImages(imagesFromDashboard, imageArray);

                    foreach (Model.ImageItem imageItem in imagesFromDashboard)
                    {
                        this.Dispatcher.Invoke(() => imageItem.IsIncluded.IsEnabled = true);
                    }

                    this.Dispatcher.Invoke(() =>
                    {
                        dashboards_fromServer.ItemsSource = imagesFromDashboard;
                        SearchingImagesForExistingDashboard.IsEnabled = true;
                        SaveSelectedDashboard.IsEnabled = true;
                        this.IsEnabled = true;
                        Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                    });
                }
            });
        }

        private void SaveSelectedDashboard_Click(object sender, RoutedEventArgs e)
        {
            List<Model.ImageItem> imagesFromList = ((IEnumerable<Model.ImageItem>)this.dashboards_fromServer.ItemsSource).ToList();
            List<Model.ImageItem> imagesToDashboard = new List<Model.ImageItem>();

            this.IsEnabled = false;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            Task.Run(() => { 
                foreach (Model.ImageItem item in imagesFromList)
                {
                    if (this.Dispatcher.Invoke(() => item.IsIncluded.IsChecked == true))
                    {
                        imagesToDashboard.Add(item);
                    }
                }

                if (imagesToDashboard.Count > 0)
                {
                    List<PictoManagementVocabulary.Image> images = new List<PictoManagementVocabulary.Image>();

                    foreach (Model.ImageItem imageItem in imagesToDashboard)
                    {
                        images.Add(new PictoManagementVocabulary.Image(imageItem.Title, imageItem.Image));
                    }

                    this.Dispatcher.Invoke(() => CreateDashboard(imagesToDashboard, canvasCreateExisting));

                    string messageText = "Este es el tablero resultante, ¿guardar?";
                    string caption = "Tablero creado";
                    MessageBoxButton button = MessageBoxButton.OKCancel;
                    MessageBoxImage icon = MessageBoxImage.Information;
                    MessageBoxResult result = MessageBox.Show(messageText, caption, button, icon);

                    if (result == MessageBoxResult.OK)
                    {
                        this.Dispatcher.Invoke(() => SaveDashboardFromCanvas(canvasCreateExisting, DashboardTitle.Text, false));
                        Dashboard dashboard = new Dashboard(this.Dispatcher.Invoke(() => DashboardTitle.Text), images.ToArray());
                        dataAccess.IncludeDashboardInList(dashboard);
                    }
                }

                this.Dispatcher.Invoke(()=> 
                { 
                    dataAccess.CleanDashboardTemporalList();

                    canvasCreateExisting.Children.Clear();
                    DashboardTitle.Text = "";
                    SearchImagesForExistingDashboard.Text = "";
                    DashboardSearchbox.Text = "";
                    dashboards_fromServer.ItemsSource = new List<Model.ImageItem>();
                    SearchingImagesForExistingDashboard.IsEnabled = false;
                    EditSelectedDashboard.IsEnabled = false;
                    SaveSelectedDashboard.IsEnabled = false;
                    this.IsEnabled = true;
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                });
            });
        }

        /*  ------------ MODIFICAR TABLERO EXISTENTE ------------ */
        // Hay que capturar el dashboard y tenerlo guardado durante todo el proceso para luego eliminar el existente y generar uno nuevo
        private void MyDashboardsSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchText = ModifyingDashboardSearchbox.Text;
            string[] search = PrepareTextForSearching(searchText);

            List<Model.ImageItem> dashboards = new List<Model.ImageItem>();
            this.IsEnabled = false;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            Task.Run(()=> 
            { 
                SearchForDashboardLocally(dashboards, search);

                if (dashboards.Count != 0)
                {
                    this.Dispatcher.Invoke(() => 
                    {
                        dashboards = dashboards.OrderBy(o => o.Title).ToList();
                        dashboard_andImages.ItemsSource = dashboards;
                        ModifyExistingDashboard.IsEnabled = true;
                    });
                }
                else
                {
                    string messageText = "No existen tableros con el título especificado";
                    string caption = "No hay resultados";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBox.Show(messageText, caption, button, icon);
                }
            });
            this.IsEnabled = true;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }

        private void SearchImagesModifying_Click(object sender, RoutedEventArgs e)
        {
            string searchText = NewImageModifySearchbox.Text;
            string[] search = PrepareTextForSearching(searchText);
            List<Model.ImageItem> dashboardImagesFromList = ((IEnumerable<Model.ImageItem>)this.dashboard_andImages.ItemsSource).ToList();

            this.IsEnabled = false;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            Task.Run(() =>
            {
                SearchImages(dashboardImagesFromList, search);

                this.Dispatcher.Invoke(() =>
                {
                    dashboardImagesFromList = dashboardImagesFromList.OrderBy(o => o.Title).ToList();
                    dashboard_andImages.ItemsSource = dashboardImagesFromList;
                    this.IsEnabled = true;
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                });
            });
            
        }

        private void ModifyMyDashboard_Click(object sender, RoutedEventArgs e)
        {
            List<Model.ImageItem> dashboardImagesFromList = ((IEnumerable<Model.ImageItem>)this.dashboard_andImages.ItemsSource).ToList();
            List<Model.ImageItem> imagesFromDashboard = new List<Model.ImageItem>();
            Dashboard selectedDashboard = null;

            this.IsEnabled = false;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            Task.Run(() =>
            {
                foreach (Model.ImageItem dashboardItem in dashboardImagesFromList)
                {
                    // Coge el primer dashboard seleccionado
                    if (this.Dispatcher.Invoke(() => dashboardItem.IsIncluded.IsEnabled == true))
                    {
                        selectedDashboard = dataAccess.GetDashboardFromList(dashboardItem.Title);
                        break;
                    }
                }

                if (selectedDashboard != null)
                {
                    this.Dispatcher.Invoke(() => ModifiedTitle.Text = selectedDashboard.Name);
                    List<string> imageTitles = new List<string>();
                    foreach (PictoManagementVocabulary.Image image in selectedDashboard.Images)
                    {
                        imageTitles.Add(image.Title);
                    }
                    string[] imageArray = imageTitles.ToArray();
                    SearchImages(imagesFromDashboard, imageArray);

                    foreach (Model.ImageItem imageItem in imagesFromDashboard)
                    {
                        this.Dispatcher.Invoke(() => imageItem.IsIncluded.IsEnabled = true);
                    }

                    this.Dispatcher.Invoke(() =>
                    {
                        dashboard_andImages.ItemsSource = imagesFromDashboard;
                        SearchImagesModifying.IsEnabled = true;
                        SaveModifiedDashboard.IsEnabled = true;

                        this.IsEnabled = true;
                        Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                    });
                }
            });
        }

        private void SaveModifiedDashboard_Click(object sender, RoutedEventArgs e)
        {
            List<Model.ImageItem> imagesFromList = ((IEnumerable<Model.ImageItem>)this.dashboard_andImages.ItemsSource).ToList();
            List<Model.ImageItem> imagesToDashboard = new List<Model.ImageItem>();
            string dashboardPath = dataAccess.ConfigDictionary["DashboardsFolder"] + DashboardTitle.Text + ".png";

            this.IsEnabled = false;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            Task.Run(() =>
            {
                foreach (Model.ImageItem item in imagesFromList)
                {
                    if (this.Dispatcher.Invoke(() => item.IsIncluded.IsChecked == true))
                    {
                        imagesToDashboard.Add(item);
                    }
                }

                if (imagesToDashboard.Count > 0)
                {
                    List<PictoManagementVocabulary.Image> images = new List<PictoManagementVocabulary.Image>();

                    foreach (Model.ImageItem imageItem in imagesToDashboard)
                    {
                        images.Add(new PictoManagementVocabulary.Image(imageItem.Title, imageItem.Image));
                    }

                    this.Dispatcher.Invoke(() => CreateDashboard(imagesToDashboard, canvasModify));

                    string messageText = "Este es el tablero resultante, ¿guardar?";
                    string caption = "Tablero creado";
                    MessageBoxButton button = MessageBoxButton.OKCancel;
                    MessageBoxImage icon = MessageBoxImage.Information;
                    MessageBoxResult result = MessageBox.Show(messageText, caption, button, icon);

                    if (result == MessageBoxResult.OK)
                    {
                        this.Dispatcher.Invoke(() => SaveDashboardFromCanvas(canvasModify, ModifiedTitle.Text, false));
                        Dashboard dashboard = new Dashboard(this.Dispatcher.Invoke(() => ModifiedTitle.Text), images.ToArray());
                        dataAccess.IncludeDashboardInList(dashboard);
                    }

                }

                this.Dispatcher.Invoke(() =>
                {
                    canvasModify.Children.Clear();
                    ModifyingDashboardSearchbox.Text = "";
                    ModifiedTitle.Text = "";
                    NewImageModifySearchbox.Text = "";
                    dashboard_andImages.ItemsSource = new List<Model.ImageItem>();
                    SearchImagesModifying.IsEnabled = false;
                    ModifyExistingDashboard.IsEnabled = false;
                    SaveModifiedDashboard.IsEnabled = false;

                    this.IsEnabled = true;
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                });
            });
        }


        /*  ------------ VER MIS TABLEROS ------------ */

        private void SeeMyDashboards_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            Task.Run(() =>
            {
                List<Model.ImageItem> imagesToShow = GetAllDashboards();
                this.Dispatcher.Invoke(() =>
                {
                    imagesToShow = imagesToShow.OrderBy(o => o.Title).ToList();
                    own_Dashboards.ItemsSource = imagesToShow;

                    this.IsEnabled = true;
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                });
            });
        }

        /*  ------------ CIERRE DE VENTANA ------------ */

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Task.Run(() =>
            {
                dataAccess.WriteDashboardToDatabase();
                try
                {
                    dataAccess.EmptyTempDirectory();
                }
                catch
                {
                    string messageText = "Ocurrio un error intentando borrar los datos de las carpetas temporales";
                    string caption = "Error borrando ficheros temporales";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBox.Show(messageText, caption, button, icon);
                }
            });
        }
    }
}
