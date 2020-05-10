using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using PictoManagementVocabulary;

namespace PictoManagementServer
{
    /// <summary>
    /// Servidor de tcp concurrente multihilo
    /// </summary>
    class TcpServer
    {
        /// <summary>
        /// Instancia del log para generar una traza
        /// </summary>
        private LogSingleTon log = LogSingleTon.Instance;

        /// <summary>
        /// Constructor de la clase, genera un tcpListener con el puerto indicado
        /// </summary>
        /// <param name="port">Puerto donde va a escuchar el tcpListener</param>
        public TcpServer(int port)
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Any, port);

            tcpListener.Start();
            log.LogMessage("Started TCP Server");

            log.LogMessage("Accepting clients");
            AcceptClients(tcpListener);
        }

        /// <summary>
        /// Método para aceptar clientes en hilos trabajadores y procesar peticiones
        /// </summary>
        /// <param name="tcpListener">listener que aceptará los clientes</param>
        public void AcceptClients(TcpListener tcpListener)
        {
            var tcpClientThread = new Thread(() =>
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                log.LogMessage("Client accepted");

                bool clientConnected = true;

                NetworkStream netStream = tcpClient.GetStream();

                do
                {
                    var rcvBuffer = new byte[tcpClient.ReceiveBufferSize];
                    netStream.Read(rcvBuffer, 0, (int)tcpClient.ReceiveBufferSize);
                    RequestProcessor requestProcessor = new RequestProcessor(rcvBuffer);
                }
                while (clientConnected);

                tcpClient.Close();
            });
            tcpClientThread.Start();
        }

        /// <summary>
        /// Comprueba el tipo de petición y la procesa
        /// </summary>
        /// <param name="requestType">Tipo de petición recibida.</param>
        /// <param name="bodyOfRequest">Cuerpo de la petición.</param>
        /// <param name="networkStream">Referencia al stream para enviar datos.</param>
        /// <param name="clientConnected">Referencia al booleano que se comprueba en el bucle principal.</param>
        public void CheckTypeOfRequestAndProcess(string requestType,
            string bodyOfRequest, NetworkStream networkStream,
            ref bool clientConnected)
        {
            switch (requestType)
            {
                case "image":
                    ProcessImageRequest(bodyOfRequest, ref networkStream);
                    break;
                case "get dashboard":
                    ProcessGetDashboardRequest(bodyOfRequest, ref networkStream);
                    break;
                case "insert dashboard":
                    ProcessInsertDashboardRequest(bodyOfRequest);
                    break;
                default:
                    clientConnected = false;
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
