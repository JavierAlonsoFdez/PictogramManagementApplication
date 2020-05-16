using System;
using System.Collections.Generic;
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

        public ClientWorker(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _networkStream = tcpClient.GetStream();

            _t = new Thread(doWork);
        }

        public void doWork()
        {
            var rcvBuffer = new byte[_tcpClient.ReceiveBufferSize];
            _networkStream.Read(rcvBuffer, 0, (int)_tcpClient.ReceiveBufferSize);
            RequestProcessor requestProcessor = new RequestProcessor(rcvBuffer);

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
                    ProcessImageRequest(bodyOfRequest, ref _networkStream);
                    break;
                case "get dashboard":
                    ProcessGetDashboardRequest(bodyOfRequest, ref _networkStream);
                    break;
                case "insert dashboard":
                    ProcessInsertDashboardRequest(bodyOfRequest);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Procesa una petición para recibir imágenes
        /// </summary>
        /// <param name="bodyOfRequest">Cuerpo de la petición.</param>
        /// <param name="netStream">Referencia al stream para enviar datos.</param>
        public void ProcessImageRequest(string bodyOfRequest, ref NetworkStream netStream)
        {
            ImageRequestProcessor imageProcessor = new ImageRequestProcessor(bodyOfRequest);
            foreach (Image img in imageProcessor.GetImages())
            {
                byte[] sndBuffer = imageProcessor.CodeImageForSending(img);
                netStream.Write(sndBuffer, 0, sndBuffer.Length);
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
        public void ProcessGetDashboardRequest(string bodyOfRequest, ref NetworkStream netStream)
        {
            DashboardRequestProcessor dashboardProcessor = new DashboardRequestProcessor();
            List<string> dashboardList = dashboardProcessor.GetDataFromDashboard(bodyOfRequest);

            foreach (string dashboardImages in dashboardList)
            {
                ImageRequestProcessor imageProcessor = new ImageRequestProcessor(dashboardImages);
                foreach (Image img in imageProcessor.GetImages())
                {
                    byte[] sndBuffer = imageProcessor.CodeImageForSending(img);
                    netStream.Write(sndBuffer, 0, sndBuffer.Length);
                }
            }
        }
    }
}
