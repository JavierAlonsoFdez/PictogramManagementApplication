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

        public BusinessLayer()
        {
            string Address = "127.0.0.1"; // Fichero de configuración
            int port = 12000; // Fichero de configuración
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
            BinaryCodec<Request> binCodReq = new BinaryCodec<Request>();

            byte[] sendingRequest = binCodReq.Encode(request);
            binWriter.Write(sendingRequest.Length);
            binWriter.Write(sendingRequest);
        }

        /// <summary>
        /// Envía un dashboard, este método se usa a la hora de crear tableros, para compartirlos
        /// </summary>
        /// <param name="dashboard">Tablero a compartir con el resto de usuarios</param>
        public void SendDashboard(Dashboard dashboard)
        {
            BinaryCodec<Dashboard> binCodDash = new BinaryCodec<Dashboard>();

            byte[] sendingDashboard = binCodDash.Encode(dashboard);
            binWriter.Write(sendingDashboard.Length);
            binWriter.Write(sendingDashboard);
        }

        /// <summary>
        /// Recibe un array de imágenes
        /// </summary>
        /// <returns>Retorna un array de imágenes ya decodificado</returns>
        public Image[] ReceiveImages()
        {
            
            int NumberOfImages = binReader.ReadInt32();
            Image[] imagesReceived = new Image[NumberOfImages];
            BinaryCodec<Image> binCodImg = new BinaryCodec<Image>();

            for (int i = 0; i<NumberOfImages;i++)
            {
                imagesReceived[i] = binCodImg.Decode(binReader.ReadBytes(binReader.ReadInt32()));
            }

            return imagesReceived;
        }

        /// <summary>
        /// Recibe un dashboard
        /// </summary>
        /// <returns>Retorna un dashboard ya decodificado</returns>
        public Dashboard ReceiveDashboard()
        {
            BinaryCodec<Dashboard> binCodDash = new BinaryCodec<Dashboard>();

            Dashboard dashboardReceived = binCodDash.Decode(binReader.ReadBytes(binReader.ReadInt32()));

            return dashboardReceived;
        }
    }
}
