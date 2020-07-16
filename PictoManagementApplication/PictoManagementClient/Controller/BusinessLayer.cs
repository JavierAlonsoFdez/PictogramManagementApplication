using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using PictoManagementVocabulary;
using System.Threading;
using System.Text;
using System.Threading.Tasks;

namespace PictoManagementClient.Controller
{
    /// <summary>
    /// Capa de lógica de negocio, aquí se realiza toda la carga computacional
    /// </summary>
    public class BusinessLayer
    {
        private IPEndPoint _ipEndPoint;
        private TcpClient _tcpClient;
        private BinaryReader _binReader;
        private BinaryWriter _binWriter;

        /// <summary>
        /// Constructor de la clase, genera un ipEndPoint hacia el servidor
        /// </summary>
        /// <param name="newAddress">Dirección IP en la que se encuentra alojado el servidor.</param>
        /// <param name="newPort">Puerto en el que escucha el servidor.</param>
        public BusinessLayer(string newAddress, int newPort)
        {
            _ipEndPoint = new IPEndPoint(IPAddress.Parse(newAddress), newPort);
        }

        /// <summary>
        /// Conecta el servidor y el cliente, asigna el cliente, el stream de datos, un reader y un writer
        /// </summary>
        public TcpClient  Connect()
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.ReceiveTimeout = 15000;
            tcpClient.Connect(_ipEndPoint);
            return tcpClient;
        }

        /// <summary>
        /// Cierra la conexión entre el cliente y el servidor
        /// </summary>
        public void Dispose(TcpClient tcpClient)
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
            //BinaryCodec<Image> binCodImage = new BinaryCodec<Image>();
            //BinaryCodec<Request> binCodReq = new BinaryCodec<Request>();
            BinaryCodecRequest binCodReq = new BinaryCodecRequest();
            BinaryCodecImage binCodImage = new BinaryCodecImage();
            Image[] imagesReceived = new Image[imagesRequested.Length];
            int position = 0;

            Parallel.For(0, imagesRequested.Length,new ParallelOptions { MaxDegreeOfParallelism = 4 }, (i)=>{
                TcpClient tcpClient = Connect();
                NetworkStream netStr = tcpClient.GetStream();
                using (BinaryWriter bw = new BinaryWriter(netStr))
                using (BinaryReader br = new BinaryReader(netStr))
                {
                    string image = imagesRequested[i];
                    Request request = new Request("Get image", image);
                    byte[] sndBuffer = binCodReq.Encode(request);

                    bw.Write(sndBuffer.Length);
                    bw.Write(sndBuffer);

                    int receiveBytes = br.ReadInt32();
                    byte[] receivedBuffer = br.ReadBytes(receiveBytes);
                    imagesReceived[position] = binCodImage.Decode(receivedBuffer);
                    position++;
                }
                Dispose(tcpClient);
            });

            return imagesReceived;
        }

        /// <summary>
        /// Envía una petición para enviar un tablero y cuando el servidor está preparado para recibirlo, lo envía
        /// </summary>
        /// <param name="dashContent">Contenido del tablero en formato string</param>
        public void SendDashboard(string dashContent)
        {
            TcpClient tcpClient = Connect();
            NetworkStream netStr = tcpClient.GetStream();
            BinaryCodec<Request> binCodReq = new BinaryCodec<Request>();
            Request request = new Request("Insert dashboard", dashContent);
            byte[] sndBuffer = binCodReq.Encode(request);
            using (BinaryWriter bw = new BinaryWriter(netStr))
            {
                bw.Write(sndBuffer.Length);
                bw.Write(sndBuffer);
            }

            Dispose(tcpClient);
        }

        /// <summary>
        /// Solicita un dashboard y retorna una lista con todos los tableros cuyo título coincida con ese nombre
        /// </summary>
        /// <param name="dashName">Nombre el dashboard solicitado</param>
        /// <returns>Lista de tableros cuyo título coincida con el indicado</returns>
        public List<Dashboard> GetDashboard(string dashName)
        {
            TcpClient tcpClient = Connect();
            NetworkStream netStr = tcpClient.GetStream();
            Dashboard newDashboard;

            BinaryCodec<Request> binCodReq = new BinaryCodec<Request>();
            BinaryCodec<Dashboard> binCodDash = new BinaryCodec<Dashboard>();
            Request request = new Request("Get dashboard", dashName);
            byte[] sndBuffer = binCodReq.Encode(request);
            List<Dashboard> dashboardList = new List<Dashboard>();

            using (BinaryWriter bw = new BinaryWriter(netStr))
            using (BinaryReader br = new BinaryReader(netStr))
            {

            

                bw.Write(sndBuffer.Length);
                bw.Write(sndBuffer);

                int receivingDashboards = br.ReadInt32();
                int receiveBytes;
                

                for (int i = 0; i < receivingDashboards; i++)
                {
                    receiveBytes = br.ReadInt32();
                    byte[] receivedBuffer = br.ReadBytes(receiveBytes);
                    newDashboard = binCodDash.Decode(receivedBuffer);
                    dashboardList.Add(newDashboard);
                }
            }
            Dispose(tcpClient);
            
            return dashboardList;
        }
    }
}
