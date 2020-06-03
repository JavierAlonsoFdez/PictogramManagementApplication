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

namespace PictoManagementClient.DataAccessLayer
{
    public class DataAccess
    {
        private string configPath;
        private Dictionary<string, string> configDictionary;
        private List<Dashboard> dashboards;

        /// <summary>
        /// Constructor de la clase, carga el fichero de configuración en un diccionario de tipo String - String
        /// </summary>
        public DataAccess()
        {
            configPath = @"D:\Documentos\JAVIER\TFG\Repositorio\PictogramManagementApplication\PictoManagementApplication\PictoManagementClientTest\Config\Config.xml"; // Necesito trabajar los path relativos
            dashboards = new List<Dashboard>();
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
        /// Obtiene todas las imagenes que componen un determinado dashboard
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
        /// Lee los dashboards guardados en el json que sirve como base de datos
        /// </summary>
        public void LoadDashboardDatabase()
        {
            using (StreamReader sr = new StreamReader(ConfigDictionary["Dashboards"]))
            {
                string json = sr.ReadToEnd();
                dashboards = JsonConvert.DeserializeObject<List<Dashboard>>(json);
            }
            // Estp es para evitar que al leer un fichero vacío deje de ser una instancia de un objeto
            if (dashboards == null)
            {
                dashboards = new List<Dashboard>();
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

        public System.Drawing.Image GetImageFromFolder(string newTitle)
        {
            string folderPath = ConfigDictionary["Images"] + newTitle + ".png";
            if (Directory.Exists(folderPath))
            {
                return System.Drawing.Image.FromFile(folderPath);
            }

            return null;
        }

        public Dashboard GetDashboardByName(string dashboardName)
        {
            foreach (Dashboard dashboard in dashboards)
            {
                if (dashboard.Title == dashboardName)
                    return dashboard;
            }
            return null;
        }
    }
}
