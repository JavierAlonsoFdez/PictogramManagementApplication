using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PictoManagementServer
{
    /// <summary>
    /// Servidor de tcp concurrente multihilo
    /// </summary>
    class TcpServer
    {
        private TcpListener _tcpServer;
        private Boolean _isRunning;

        /// <summary>
        /// Constructor de la clase, lanza un servidor concurrente multihilo
        /// </summary>
        /// <param name="ipAddress">Dirección en la que escuchará el servidor</param>
        /// <param name="port">Puerto en el que escuchará el servidor</param>
        public TcpServer(System.Net.IPAddress ipAddress,int port)
        {
            _tcpServer = new TcpListener(ipAddress, port);
            _tcpServer.Start();

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

            StreamWriter streamWriter = new StreamWriter(clientTcp.GetStream(), Encoding.ASCII);
            StreamReader streamReader = new StreamReader(clientTcp.GetStream(), Encoding.ASCII);

            clientTcpConnected = true;

            while(clientTcpConnected)
            {
                // PARTE PRINCIPAL DEL PROGRAMA
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
