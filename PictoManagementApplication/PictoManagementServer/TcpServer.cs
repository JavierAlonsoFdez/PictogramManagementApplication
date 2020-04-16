using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using PictoManagementVocabulary;

namespace PictoManagementServer
{
    /// <summary>
    /// Servidor de tcp concurrente multihilo
    /// </summary>
    class TcpServer
    {
        private TcpListener _tcpServer;
        private Boolean _isRunning;
        private LogSingleTon log = LogSingleTon.Instance;

        /// <summary>
        /// Constructor de la clase, lanza un servidor concurrente multihilo
        /// </summary>
        /// <param name="ipAddress">Dirección en la que escuchará el servidor</param>
        /// <param name="port">Puerto en el que escuchará el servidor</param>
        public TcpServer(System.Net.IPAddress ipAddress,int port)
        {
            _tcpServer = new TcpListener(ipAddress, port);
            _tcpServer.Start();

            log.LogMessage("TCP server started");

            _isRunning = true;

            LoopClients();
        }

        /// <summary>
        /// Acepta nuevos clientes tcp y les asigna un thread a cada uno
        /// </summary>
        public void LoopClients()
        {
            while(_isRunning)
            {
                TcpClient newClient = _tcpServer.AcceptTcpClient();

                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                log.LogMessage("Started new client");
                t.Start(newClient);
                log.LogMessage("Client finished");
            }
        }
        
        /// <summary>
        /// Manejador de clientes, genera un stream para lectura y otro para escritura para atender las peticiones del cliente
        /// </summary>
        /// <param name="obj"></param>
        public void HandleClient(object obj)
        {
            TcpClient clientTcp = (TcpClient)obj;
            Boolean clientTcpConnected;

            NetworkStream netStream = clientTcp.GetStream();
            
            clientTcpConnected = true;
            
            while(clientTcpConnected)
            {
                var rcvBuffer = new byte[clientTcp.ReceiveBufferSize];
                netStream.Read(rcvBuffer, 0, (int)clientTcp.ReceiveBufferSize);
                RequestProcessor requestProcessor = new RequestProcessor(rcvBuffer);

                // Se comprueba el tipo de petición recibida y en función de eso se realizan diferentes acciones
                if (requestProcessor.GetTypeOfRequest() == "image")
                {
                    ImageRequestProcessor imageProcessor = new ImageRequestProcessor(requestProcessor.GetBodyOfRequest());
                    foreach (Image img in imageProcessor.GetImages())
                    {
                        byte[] sndBuffer = imageProcessor.CodeImageForSending(img);
                        netStream.Write(sndBuffer, 0, sndBuffer.Length);
                    }
                }

                else if (requestProcessor.GetTypeOfRequest() == "get dashboard")
                {
                    DashboardRequestProcessor dashboardProcessor = new DashboardRequestProcessor();
                    List<string> dashboardList = dashboardProcessor.GetDataFromDashboard(requestProcessor.GetBodyOfRequest());

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

                else if (requestProcessor.GetTypeOfRequest() == "insert dashboard")
                {
                    DashboardRequestProcessor dashboardProcessor = new DashboardRequestProcessor();
                    string[] request = dashboardProcessor.PrepareRequestForInsert(requestProcessor.GetBodyOfRequest());
                    dashboardProcessor.InsertDataIntoDashboards(request[0], request[1]);
                }

                else if (requestProcessor.GetTypeOfRequest() == "disconnect")
                {
                    clientTcpConnected = false;
                    clientTcp.Close();
                }

            }
        }

        /// <summary>
        /// Para el servidor TCP
        /// </summary>
        public void StopTcpServer()
        {
            _tcpServer.Stop();
        }
    }
}
