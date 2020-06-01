using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using PictoManagementVocabulary;
using System.Text;
using System.Threading.Tasks;

namespace PictoManagementClient.Controller
{
    /// <summary>
    /// Capa de lógica de negocio, aquí se realiza toda la carga computacional
    /// </summary>
    public class BusinessLayer
    {
        TcpClient tcpClient;
        NetworkStream netStream;
        BinaryReader binReader;
        BinaryWriter binWriter;
        string Address;
        int port;

        /// <summary>
        /// Constructor de la clase, asigna a Address y Port sus valores del fichero de configuración.
        /// </summary>
        /// <param name="ConfigDictionary">Valores del fichero de configuración en un diccionario</param>
        public BusinessLayer(Dictionary<string, string> ConfigDictionary)
        {
            Address = ConfigDictionary["address"];
            port = Int32.Parse(ConfigDictionary["port"]);
        }

        /// <summary>
        /// Abre una conexión con el servidor en la dirección y puerto indicados a través de la configuración
        /// </summary>
        public void OpenConnection()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(Address), port);

            tcpClient = new TcpClient();
            tcpClient.Connect(ipEndPoint);
            netStream = tcpClient.GetStream();

            binReader = new BinaryReader(netStream);
            binWriter = new BinaryWriter(netStream);
        }

        /// <summary>
        /// Envía una request al servidor para realizar alguna petición de las existentes
        /// </summary>
        /// <param name="request">Petición a enviar al servidor</param>
        public void SendRequest(Request request)
        {
            OpenConnection();

            BinaryCodec<Request> binCodReq = new BinaryCodec<Request>();

            byte[] sendingRequest = binCodReq.Encode(request);
            binWriter.Write(sendingRequest.Length);
            binWriter.Write(sendingRequest);

            CloseConnection();
        }

        /// <summary>
        /// Envía un dashboard, este método se usa a la hora de crear tableros, para compartirlos
        /// </summary>
        /// <param name="dashboard">Tablero a compartir con el resto de usuarios</param>
        public void SendDashboard(Dashboard dashboard)
        {
            OpenConnection();

            BinaryCodec<Dashboard> binCodDash = new BinaryCodec<Dashboard>();

            byte[] sendingDashboard = binCodDash.Encode(dashboard);
            binWriter.Write(sendingDashboard.Length);
            binWriter.Write(sendingDashboard);

            CloseConnection();
        }

        /// <summary>
        /// Recibe un array de imágenes
        /// </summary>
        /// <returns>Retorna un array de imágenes ya decodificado</returns>
        public Image[] ReceiveImages()
        {
            OpenConnection();

            int NumberOfImages = binReader.ReadInt32();
            Image[] imagesReceived = new Image[NumberOfImages];
            BinaryCodec<Image> binCodImg = new BinaryCodec<Image>();

            for (int i = 0; i<NumberOfImages;i++)
            {
                int imageSize = binReader.ReadInt32();
                imagesReceived[i] = binCodImg.Decode(binReader.ReadBytes(imageSize));
            }
            CloseConnection();

            return imagesReceived;
        }

        /// <summary>
        /// Recibe un dashboard
        /// </summary>
        /// <returns>Retorna un dashboard ya decodificado</returns>
        public Dashboard ReceiveDashboard()
        {
            OpenConnection();

            BinaryCodec<Dashboard> binCodDash = new BinaryCodec<Dashboard>();

            int dashboardSize = binReader.ReadInt32();
            Dashboard dashboardReceived = binCodDash.Decode(binReader.ReadBytes(dashboardSize));

            CloseConnection();

            return dashboardReceived;
        }

        /// <summary>
        /// Cierra la conexión con el servidor
        /// </summary>
        public void CloseConnection()
        {
            tcpClient.Dispose();
        }
    }
}
