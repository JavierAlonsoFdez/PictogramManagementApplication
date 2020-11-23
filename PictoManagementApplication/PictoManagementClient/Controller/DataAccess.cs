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

namespace PictoManagementClient.Controller
{
    /// <summary>
    /// Capa de acceso a datos
    /// </summary>
    public class DataAccess
    {
        private string _configPath;
        private Dictionary<string, string> _configDictionary;
        private List<Dashboard> _dashboards;
        private List<Dashboard> _dashboardsTemp;

        /// <summary>
        /// Constructor de la clase, carga el fichero de configuración en un diccionario de tipo String - String
        /// </summary>
        public DataAccess()
        {
            _configPath = @"D:\Documentos\JAVIER\TFG\Repositorio\PictogramManagementApplication\PictoManagementApplication\PictoManagementClientTest\Config\Config.xml"; // Necesito trabajar los path relativos
            _dashboards = new List<Dashboard>();
            _dashboardsTemp = new List<Dashboard>();
            this.LoadConfig();
            this.LoadDashboardDatabase();
        }

        /// <summary>
        /// Permite acceder al atributo configDictionary
        /// </summary>
        public Dictionary<string, string> ConfigDictionary
        {
            get { return _configDictionary; }
        }

        /// <summary>
        /// Permite acceder al atributo dashboards
        /// </summary>
        public List<Dashboard> Dashboards
        {
            get { return _dashboards; }
        }

        /// <summary>
        /// Carga la configuración del fichero de configuración al diccionario String - String de la clase
        /// </summary>
        public void LoadConfig()
        {
            _configDictionary = new Dictionary<string, string>();

            string xmlFile = File.ReadAllText(_configPath);
            XmlDocument xmlConfig = new XmlDocument();
            xmlConfig.LoadXml(xmlFile);

            XmlNodeList nodeList = xmlConfig.SelectNodes("/Config/*");

            foreach (XmlNode node in nodeList)
            {
                _configDictionary.Add(node.Name, node.InnerText);
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
                _dashboards = JsonConvert.DeserializeObject<List<Dashboard>>(json);
            }
            // Esto es para evitar que al leer un fichero vacío deje de ser una instancia de un objeto
            if (_dashboards == null)
            {
                _dashboards = new List<Dashboard>();
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

            foreach (Dashboard dashboard in _dashboards)
            {
                if (dashboard.Name == dashboardName)
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
            _dashboards.Add(ChangePathsInImages(newDashboard));
            WriteDashboardToDatabase();
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
            List<Dashboard> dashboardsToDB = new List<Dashboard>();
            foreach (Dashboard dash in _dashboards)
            {
                List<Image> imagesToDashboard = new List<Image>();
                foreach (Image img in dash.Images)
                {
                    Image imageToDB = new Image(img.Title, img.Path);
                    imageToDB.FileBase64 = null;
                    imagesToDashboard.Add(imageToDB);
                }
                dashboardsToDB.Add(new Dashboard(dash.Name, imagesToDashboard.ToArray()));
            }

            string json = JsonConvert.SerializeObject(dashboardsToDB.ToArray());
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
            return new Dashboard(dashboard.Name, newImagesList.ToArray());
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
        /// Mueve una imagen de la carpeta temporal a la carpeta destino
        /// </summary>
        /// <param name="title">Título de la imagen a cambiar de almacenamiento</param>
        public void MoveImageFromTempToDestination(string title)
        {
            string testTemporaryPath = ConfigDictionary["Temp"] + title + ".png";
            if (File.Exists(testTemporaryPath))
            {
                string destinationPath = ConfigDictionary["Images"] + title + ".png";
                if (!File.Exists(destinationPath))
                {
                    File.Copy(testTemporaryPath, destinationPath);
                }
            }
        }

        /// <summary>
        /// Borra todo el contenido de la lista de dashboards temporal
        /// </summary>
        public void CleanDashboardTemporalList()
        {
            _dashboardsTemp = new List<Dashboard>();
        }

        /// <summary>
        /// Saca una lista de dashboards comparando por su resultado
        /// </summary>
        /// <param name="content">String a buscar en los tableros</param>
        /// <returns>Lista de tableros que contienen el texto buscado</returns>
        public List<Dashboard> GetDashboardByContent(string content)
        {
            List<Dashboard> dashboardsByContent = new List<Dashboard>();
            foreach (Dashboard dash in _dashboards)
            {
                foreach (Image img in dash.Images)
                {
                    if (img.Title == content && !dashboardsByContent.Contains(dash))
                    {
                        dashboardsByContent.Add(dash);
                    }
                }
            }
            
            if (dashboardsByContent.Count() > 0)
            {
                return dashboardsByContent;
            }
            return null;
        }

        /// <summary>
        /// Borra todo el contenido de la carpeta Temp
        /// </summary>
        public void EmptyTempDirectory()
        {
            DirectoryInfo tempDirectory = new DirectoryInfo(ConfigDictionary["Temp"]);
            DirectoryInfo tempDashboardsDirectory = new DirectoryInfo(ConfigDictionary["DashboardsTemp"]);

            foreach (FileInfo file in tempDirectory.GetFiles())
            {
                file.Delete();
            }

            foreach (FileInfo file in tempDashboardsDirectory.GetFiles())
            {
                file.Delete();
            }
        }

        /// <summary>
        /// Obtiene un tablero de la lista temporal filtrando por título
        /// </summary>
        /// <param name="title">Título del tablero a buscar en la lista temporal</param>
        /// <returns>Tablero encontrado con ese título</returns>
        public Dashboard GetDashboardFromTemporalList(string title)
        {
            foreach (Dashboard dashboard in _dashboardsTemp)
            {
                if (dashboard.Name == title)
                {
                    return dashboard;
                }
            }

            return null;
        }

        /// <summary>
        /// Obtiene un tablero de la lista temporal filtrando por todo su contenido
        /// </summary>
        /// <param name="dash">Tablero a ser buscado en la lista temporal</param>
        /// <returns>Tablero encontrado con ese título y contenido</returns>
        public Dashboard GetDashboardFromTemporalList(Dashboard dash)
        {
            foreach (Dashboard dashboard in _dashboardsTemp)
            {
                if (dashboard.Name == dash.Name)
                {
                    foreach (PictoManagementVocabulary.Image img in dashboard.Images)
                    {
                        img.FileBase64 = "";
                    }
                    foreach (PictoManagementVocabulary.Image image in dash.Images)
                    {
                        image.FileBase64 = "";
                    }
                    if (dashboard.Images == dash.Images)
                    {
                        return dashboard;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Añade un tablero en la lista de tableros temporal
        /// </summary>
        /// <param name="dash">Tablero a añadir en la lista temporal</param>
        public void IncludeDashboardInTemporalList(Dashboard dash)
        {
            _dashboardsTemp.Add(dash);
        }

        /// <summary>
        /// Obtiene un tablero de la lista filtrando por todo su título
        /// </summary>
        /// <param name="dashTitle">Nombre del tablero a buscar</param>
        /// <returns>Tablero encontrado con ese título</returns>
        public Dashboard GetDashboardFromList(string dashTitle)
        {
            foreach (Dashboard dashboard in _dashboards)
            {
                if (dashboard.Name == dashTitle)
                {
                    return dashboard;
                }
            }

            return null;
        }

        /// <summary>
        /// Obtiene un tablero de la lista filtrando por todo su contenido
        /// </summary>
        /// <param name="dash">Tablero a buscar en la lista definitiva</param>
        /// <returns>Tablero encontrado con ese título y contenido</returns>
        public Dashboard GetDashboardFromList(Dashboard dash)
        {
            foreach (Dashboard dashboard in _dashboards)
            {
                if (dashboard.Name == dash.Name)
                {
                    foreach (PictoManagementVocabulary.Image img in dashboard.Images)
                    {
                        img.FileBase64 = "";
                    }
                    foreach (PictoManagementVocabulary.Image image in dash.Images)
                    {
                        image.FileBase64 = "";
                    }
                    if (dashboard.Images == dash.Images)
                    {
                        return dashboard;
                    }
                }
            }

            return null;
        }
    }
}
