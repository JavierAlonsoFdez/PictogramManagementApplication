using System;
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
                t.Start(newClient);
                log.LogMessage("Started new client");
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
            RequestProcessor requestProcessor = new RequestProcessor();
            
            while(clientTcpConnected)
            {
                var rcvBuffer = new byte[clientTcp.ReceiveBufferSize];
                netStream.Read(rcvBuffer, 0, (int)clientTcp.ReceiveBufferSize);

                Request request = requestProcessor.ProcessMessage(rcvBuffer);
                
                if (request.Type.ToLower() == "image")
                {
                    // The request is asking for image(s)
                }
                else if (request.Type.ToLower() == "dashboard")
                {
                    // The request is asking for dashboard(s)
                }
                else if (request.Type.ToLower() == "disconnect")
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
