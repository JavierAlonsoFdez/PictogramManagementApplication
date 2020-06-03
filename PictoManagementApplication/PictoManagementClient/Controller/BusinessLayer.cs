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
        private IPEndPoint ipEndPoint;
        private TcpClient tcpClient;
        private BinaryReader binReader;
        private BinaryWriter binWriter;

        /// <summary>
        /// Constructor de la clase, genera un ipEndPoint hacia el servidor
        /// </summary>
        /// <param name="newAddress">Dirección IP en la que se encuentra alojado el servidor.</param>
        /// <param name="newPort">Puerto en el que escucha el servidor.</param>
        public BusinessLayer(string newAddress, int newPort)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(newAddress), newPort);
        }

        /// <summary>
        /// Conecta el servidor y el cliente, asigna el cliente, el stream de datos, un reader y un writer
        /// </summary>
        public void Connect()
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(ipEndPoint);
            NetworkStream netStream = tcpClient.GetStream();

            binReader = new BinaryReader(netStream);
            binWriter = new BinaryWriter(netStream);
        }

        /// <summary>
        /// Cierra la conexión entre el cliente y el servidor
        /// </summary>
        public void Dispose()
        {
            tcpClient.Close();
            tcpClient.Dispose();
        }

        /// <summary>
        /// Solicita una o varias imágenes
        /// </summary>
        /// <param name="imagesRequested">Array con las imágenes solicitadas al servidor</param>
        /// <returns>Array con las imágenes que retorna el servidor</returns>
        public Image[] RequestImages(string[] imagesRequested)
        {
            Connect();

            BinaryCodec<Image> binCodImage = new BinaryCodec<Image>();
            BinaryCodec<Request> binCodReq = new BinaryCodec<Request>();
            Image[] imagesReceived = new Image[imagesRequested.Length];
            int position = 0;

            foreach (string image in imagesRequested)
            {
                Request request = new Request("Get image", image);
                byte[] sndBuffer = binCodReq.Encode(request);

                binWriter.Write(sndBuffer.Length);
                binWriter.Write(sndBuffer);

                int receiveBytes = binReader.ReadInt32();
                byte[] receivedBuffer = binReader.ReadBytes(receiveBytes);
                imagesReceived[position] = binCodImage.Decode(receivedBuffer);
                position++;
            }
            Dispose();
            return imagesReceived;
        }

        /// <summary>
        /// Envía una petición para enviar un tablero y cuando el servidor está preparado para recibirlo, lo envía
        /// </summary>
        /// <param name="dashContent">Contenido del tablero en formato string</param>
        public void SendDashboard(string dashContent)
        {
            Connect();
            BinaryCodec<Request> binCodReq = new BinaryCodec<Request>();
            Request request = new Request("Insert dashboard", dashContent);
            byte[] sndBuffer = binCodReq.Encode(request);
            using (binWriter)
            {
                binWriter.Write(sndBuffer.Length);
                binWriter.Write(sndBuffer);
            }

            Dispose();
        }

        /// <summary>
        /// Solicita un dashboard y retorna una lista con todos los tableros cuyo título coincida con ese nombre
        /// </summary>
        /// <param name="dashName">Nombre el dashboard solicitado</param>
        /// <returns>Lista de tableros cuyo título coincida con el indicado</returns>
        public List<Dashboard> GetDashboard(string dashName)
        {
            Connect();
            Dashboard newDashboard;

            BinaryCodec<Request> binCodReq = new BinaryCodec<Request>();
            BinaryCodec<Dashboard> binCodDash = new BinaryCodec<Dashboard>();
            Request request = new Request("Get dashboard", dashName);
            byte[] sndBuffer = binCodReq.Encode(request);

            binWriter.Write(sndBuffer.Length);
            binWriter.Write(sndBuffer);

            int receivingDashboards = binReader.ReadInt32();
            int receiveBytes;
            List<Dashboard> dashboardList = new List<Dashboard>();

            for (int i = 0; i < receivingDashboards; i++)
            {
                receiveBytes = binReader.ReadInt32();
                byte[] receivedBuffer = binReader.ReadBytes(receiveBytes);
                newDashboard = binCodDash.Decode(receivedBuffer);
                dashboardList.Add(newDashboard);
            }

            Dispose();

            return dashboardList;
        }
    }
}
