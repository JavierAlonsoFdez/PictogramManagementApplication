using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using PictoManagementVocabulary;

namespace PictoManagementServer
{
    public class ClientWorker
    {
        private NetworkStream _networkStream;
        private TcpClient _tcpClient;
        public Thread _t;
        private byte[] receiveBuffer;
        private BinaryReader binReader;
        private BinaryWriter binWriter;
        /// <summary>
        /// Instancia del log para generar una traza
        /// </summary>
        private LogSingleTon log;

        public ClientWorker(TcpClient tcpClient)
        {
            log = LogSingleTon.Instance;
            _tcpClient = tcpClient;
            _networkStream = tcpClient.GetStream();
            binReader = new BinaryReader(_networkStream);
            binWriter = new BinaryWriter(_networkStream);
            _t = new Thread(doWork);

            _t.Start();
        }

        public void doWork()
        {
            int bufferSize = binReader.ReadInt32();
            receiveBuffer = binReader.ReadBytes(bufferSize);

            log.LogMessage("Request recibida, comenzando procesado.");

            RequestProcessor requestProcessor = new RequestProcessor(receiveBuffer);
            CheckTypeOfRequestAndProcess(requestProcessor.GetTypeOfRequest(), requestProcessor.GetBodyOfRequest());
        }

        /// <summary>
        /// Comprueba el tipo de petición y la procesa
        /// </summary>
        /// <param name="requestType">Tipo de petición recibida.</param>
        /// <param name="bodyOfRequest">Cuerpo de la petición.</param>
        /// <param name="networkStream">Referencia al stream para enviar datos.</param>
        /// <param name="clientConnected">Referencia al booleano que se comprueba en el bucle principal.</param>
        public void CheckTypeOfRequestAndProcess(string requestType,
            string bodyOfRequest)
        {
            switch (requestType)
            {
                case "get image":
                    ProcessImageRequest(bodyOfRequest);
                    log.LogMessage("Imagenes procesadas.");
                    break;
                case "get dashboard":
                    ProcessGetDashboardRequest(bodyOfRequest);
                    log.LogMessage("Dashboard enviado.");
                    break;
                case "insert dashboard":
                    ProcessInsertDashboardRequest(bodyOfRequest);
                    log.LogMessage("Dashboard recibido.");
                    break;
                default:
                    log.LogWarning("Petición inválida.");
                    break;
            }
        }

        /// <summary>
        /// Procesa una petición para recibir imágenes
        /// </summary>
        /// <param name="bodyOfRequest">Cuerpo de la petición.</param>
        /// <param name="netStream">Referencia al stream para enviar datos.</param>
        public void ProcessImageRequest(string bodyOfRequest)
        {
            ImageRequestProcessor imageProcessor = new ImageRequestProcessor(bodyOfRequest);
            foreach (Image img in imageProcessor.GetImages())
            {
                byte[] sndBuffer = imageProcessor.CodeImageForSending(img);
                binWriter.Write(sndBuffer.Length);
                binWriter.Write(sndBuffer);
            }
        }

        /// <summary>
        /// Procesa la petición para insertar un dashboard en la base de datos del servidor.
        /// </summary>
        /// <param name="bodyOfRequest">Cuerpo de la petición.</param>
        public void ProcessInsertDashboardRequest(string bodyOfRequest)
        {
            DashboardRequestProcessor dashboardProcessor = new DashboardRequestProcessor();
            string[] request = dashboardProcessor.PrepareRequestForInsert(bodyOfRequest);
            dashboardProcessor.InsertDataIntoDashboards(request[0], request[1]);
        }

        /// <summary>
        /// Procesa la petición para recibir dashboards
        /// </summary>
        /// <param name="bodyOfRequest">Cuerpo de la petición.</param>
        /// <param name="netStream">Referencia al stream para enviar datos.</param>
        public void ProcessGetDashboardRequest(string bodyOfRequest)
        {
            DashboardRequestProcessor dashboardProcessor = new DashboardRequestProcessor();
            List<Dashboard> dashboardList = dashboardProcessor.GetDataFromDashboard(bodyOfRequest);
            BinaryCodec<Dashboard> dashBinCod = new BinaryCodec<Dashboard>();

            if (dashboardList != null)
            {
                binWriter.Write(dashboardList.Count);

                foreach (Dashboard dashboardToSend in dashboardList)
                {
                    byte[] sndBuffer = dashBinCod.Encode(dashboardToSend);
                    binWriter.Write(sndBuffer.Length);
                    binWriter.Write(sndBuffer);
                }
            }
        }

        ~ClientWorker()
        {
            binReader.Close();
            binWriter.Close();
            _tcpClient.Close();
        }
    }
}
