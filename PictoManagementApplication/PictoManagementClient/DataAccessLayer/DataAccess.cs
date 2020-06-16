using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using PictoManagementVocabulary;
using System.Drawing.Imaging;

namespace PictoManagementClient.DataAccessLayer
{
    /// <summary>
    /// Capa de acceso a datos
    /// </summary>
    public class DataAccess
    {
        private string configPath;
        private Dictionary<string, string> configDictionary;
        private List<Dashboard> dashboards;
        private List<Dashboard> dashboardsTemp;

        /// <summary>
        /// Constructor de la clase, carga el fichero de configuración en un diccionario de tipo String - String
        /// </summary>
        public DataAccess()
        {
            configPath = @"D:\Documentos\JAVIER\TFG\Repositorio\PictogramManagementApplication\PictoManagementApplication\PictoManagementClientTest\Config\Config.xml"; // Necesito trabajar los path relativos
            dashboards = new List<Dashboard>();
            dashboardsTemp = new List<Dashboard>();
            LoadConfig();
            LoadDashboardDatabase();
        }

        /// <summary>
        /// Permite acceder al atributo configDictionary
        /// </summary>
        public Dictionary<string, string> ConfigDictionary
        {
            get { return configDictionary; }
        }

        /// <summary>
        /// Permite acceder al atributo dashboards
        /// </summary>
        public List<Dashboard> Dashboards
        {
            get { return dashboards; }
        }

        /// <summary>
        /// Carga la configuración del fichero de configuración al diccionario String - String de la clase
        /// </summary>
        public void LoadConfig()
        {
            configDictionary = new Dictionary<string, string>();

            string xmlFile = File.ReadAllText(configPath);
            XmlDocument xmlConfig = new XmlDocument();
            xmlConfig.LoadXml(xmlFile);

            XmlNodeList nodeList = xmlConfig.SelectNodes("/Config/*");

            foreach (XmlNode node in nodeList)
            {
                configDictionary.Add(node.Name, node.InnerText);
            }
        }

        /// <summary>
        /// Lee los dashboards guardados en el json que sirve como base de datos
        /// </summary>
        public void LoadDashboardDatabase()
        {
            using (StreamReader sr = new StreamReader(ConfigDictionary["Dashboards"]))
            {
                string json = sr.ReadToEnd();
                dashboards = JsonConvert.DeserializeObject<List<Dashboard>>(json);
            }
            // Esto es para evitar que al leer un fichero vacío deje de ser una instancia de un objeto
            if (dashboards == null)
            {
                dashboards = new List<Dashboard>();
            }
        }

        /// <summary>
        /// Obtiene todos los nombres de las imagenes que componen un determinado dashboard
        /// </summary>
        /// <param name="dashboardName">Nombre del dashboard del que se quieren obtener las imágenes</param>
        /// <returns>Array de string con el título de cada imagen</returns>
        public string[] GetImagesOfDashboardInDatabase(string dashboardName)
        {
            string[] imagesName;

            foreach (Dashboard dashboard in dashboards)
            {
                if (dashboard.Title == dashboardName)
                {
                    imagesName = new string[dashboard.Images.Length];
                    int i = 0;
                    foreach (Image image in dashboard.Images)
                    {
                        imagesName[i] = image.Title;
                        i++;
                    }
                    return imagesName;
                }
            }

            return null;
        }

        /// <summary>
        /// Incluye un tablero en la lista de tableros previamente cargada
        /// </summary>
        /// <param name="newDashboard">Tablero a incluir en la lista</param>
        public void IncludeDashboardInList(Dashboard newDashboard)
        {
            dashboards.Add(ChangePathsInImages(newDashboard));
        }

        /// <summary>
        /// Incluye múltiples tablero en la lista de tableros previamente cargada
        /// </summary>
        /// <param name="newDashboardList">Lista de tableros a incluir en la lista existente</param>
        public void IncludeMultipleDashboardInList(List<Dashboard> newDashboardList)
        {
            foreach (Dashboard newDashboard in newDashboardList)
            {
                IncludeDashboardInList(newDashboard);
            }
        }

        /// <summary>
        /// Incluye un nuevo dashboard en la base de datos de dashboards
        /// </summary>
        /// <param name="newDashboard">Nuevo dashboard a incluir en la base de datos</param>
        public void WriteDashboardToDatabase()
        {
            string json = JsonConvert.SerializeObject(dashboards.ToArray());
            File.WriteAllText(ConfigDictionary["Dashboards"], json);
        }

        /// <summary>
        /// Cambia la ruta de acceso a las imágenes y la sustituye por la que tienen en el cliente
        /// </summary>
        /// <param name="dashboard">Tablero cuyas imágenes van a ser redirigidas</param>
        /// <returns>Retorna el mismo tablero pero con las rutas de acceso cambiadas</returns>
        public Dashboard ChangePathsInImages(Dashboard dashboard)
        {
            List<Image> newImagesList = new List<Image>();
            foreach (Image img in dashboard.Images)
            {
                Image newImage = new Image(img.Title, ConfigDictionary["Images"] + img.Title + ".png");
                newImagesList.Add(newImage);
            }
            return new Dashboard(dashboard.Title, newImagesList.ToArray());
        }

        /// <summary>
        /// Guarda una imagen en la ruta seleccionada
        /// </summary>
        /// <param name="newTitle">Título con el que se guardará la imagen</param>
        /// <param name="FileBase64">Codificación del archivo en base64</param>
        public void SaveNewImage(string newTitle, string FileBase64)
        {
            byte[] bytes = Convert.FromBase64String(FileBase64);
            string destinationPath = ConfigDictionary["Images"] + newTitle + ".png";
            File.WriteAllBytes(destinationPath, bytes);
        }

        /// <summary>
        /// Guarda una imagen en la ruta temporal
        /// </summary>
        /// <param name="newTitle">Título con el que se guardará la imagen</param>
        /// <param name="FileBase64">Codificación del archivo en base64</param>
        public void SaveNewTemporalImage(string newTitle, string FileBase64)
        {
            byte[] bytes = Convert.FromBase64String(FileBase64);
            string destinationPath = ConfigDictionary["Temp"] + newTitle + ".png";
            File.WriteAllBytes(destinationPath, bytes);
        }

        /// <summary>
        /// Guarda una imagen en un archivo PNG en la carpeta de imágenes
        /// </summary>
        /// <param name="newTitle">Título con el que se guardará la imagen</param>
        /// <param name="image">Objeto que representa la imagen</param>
        public void SaveImageFile(string newTitle, System.Drawing.Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {                
                image.Save(ms, ImageFormat.Png);
                byte[] imageBytes = ms.ToArray();

                string base64String = Convert.ToBase64String(imageBytes);
                SaveNewImage(newTitle, base64String);
            }
        }

        /// <summary>
        /// Obtiene una imagen del directorio donde se guardan si existe una con dicho nombre
        /// </summary>
        /// <param name="newTitle">Nombre de la imagen a buscar</param>
        /// <returns>Retorna el archivo de imagen cuyo título coincide con el buscado</returns>
        public System.Drawing.Image GetImageFromFolder(string newTitle)
        {
            string folderPath = ConfigDictionary["Images"] + newTitle + ".png";
            if (Directory.Exists(folderPath))
            {
                return System.Drawing.Image.FromFile(folderPath);
            }

            return null;
        }

        /// <summary>
        /// Obtiene un tablero de los existentes a través de su nombre
        /// </summary>
        /// <param name="dashboardName">Nombre del tablero a mostrar</param>
        /// <returns></returns>
        public Dashboard GetDashboardByName(string dashboardName)
        {
            foreach (Dashboard dashboard in dashboards)
            {
                if (dashboard.Title.Contains(dashboardName))
                    return dashboard;
            }
            return null;
        }

        /// <summary>
        /// Mueve una imagen de la carpeta temporal a la carpeta destino
        /// </summary>
        /// <param name="title">Título de la imagen a cambiar de almacenamiento</param>
        public void MoveImageFromTempToDestination(string title)
        {
            string testTemporaryPath = ConfigDictionary["Temp"] + title + ".png";
            if (Directory.Exists(testTemporaryPath))
            {
                string destinationPath = ConfigDictionary["Images"] + title + ".png";
                File.Move(testTemporaryPath, destinationPath);
            }
        }

        /// <summary>
        /// Guarda la imagen que forma el tablero en la carpeta destinada a ello
        /// </summary>
        /// <param name="newTitle">Título del archivo que va a ser guardado</param>
        /// <param name="dashboardPreview">Imagen que conforma el tablero</param>
        public void SaveDashboardPreview(string newTitle, System.Drawing.Image dashboardPreview)
        {
            string path = ConfigDictionary["DashboardsFolder"] + newTitle;
            dashboardPreview.Save(path, ImageFormat.Png);
        }

        /// <summary>
        /// Borra todo el contenido de la lista de dashboards temporal
        /// </summary>
        public void CleanDashboardTemporalList()
        {
            dashboardsTemp = new List<Dashboard>();
        }

        /// <summary>
        /// Guarda un tablero de los almacenados en la lista temporal en la lista definitiva
        /// </summary>
        /// <param name="dashboardName">Nombre del tablero a ser transferido a la lista definitiva</param>
        public void SaveTemporalDashboard(string dashboardName)
        {
            foreach (Dashboard tempDashboard in dashboardsTemp)
            {
                if (tempDashboard.Title == dashboardName)
                    dashboards.Add(tempDashboard);
            }
        }

        /// <summary>
        /// Saca todas las imágenes que contiene un dashboard (si están descargadas en el cliente)
        /// </summary>
        /// <returns>Lista de imágenes que contiene el dashboard</returns>
        public List<System.Drawing.Image> GetDashboardsImages(string dashboardName)
        {
            List<System.Drawing.Image> dashboardsImages = new List<System.Drawing.Image>();
            foreach (Dashboard dash in dashboards)
            {
                if (dash.Title == dashboardName)
                {
                    string folderPath = ConfigDictionary["Dashboards"] + dash.Title + ".png";
                    if (Directory.Exists(folderPath))
                    {
                        dashboardsImages.Add(System.Drawing.Image.FromFile(folderPath));
                    }
                }
            }

            return dashboardsImages;
        }

        /// <summary>
        /// Borra todo el contenido de la carpeta Temp
        /// </summary>
        public void EmptyTempDirectory()
        {
            DirectoryInfo tempDirectory = new DirectoryInfo(ConfigDictionary["Temp"]);

            foreach (FileInfo file in tempDirectory.GetFiles())
            {
                file.Delete();
            }
        }
    }
}
