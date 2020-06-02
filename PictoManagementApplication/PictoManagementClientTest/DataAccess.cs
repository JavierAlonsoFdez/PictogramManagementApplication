using System.Collections.Generic;
using PictoManagementVocabulary;
using System.Xml;
using System.IO;
using Newtonsoft.Json;

namespace PictoManagementClientTest
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
            LoadConfig();
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
            using (StreamReader sr = new StreamReader("file.json"))
            {
                string json = sr.ReadToEnd();
                dashboards = JsonConvert.DeserializeObject<List<Dashboard>>(json);
            }
        }

        /// <summary>
        /// Incluye un nuevo dashboard en la base de datos de dashboards
        /// </summary>
        /// <param name="newDashboard">Nuevo dashboard a incluir en la base de datos</param>
        public void WriteDashboardToDatabase(Dashboard newDashboard)
        {
            dashboards.Add(newDashboard);
            string json = JsonConvert.SerializeObject(dashboards.ToArray());
            File.WriteAllText(@"file.json", json);
        }
    }
}
