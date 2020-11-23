using System;
using System.Net;
using System.Net.Sockets;

namespace PictoManagementServer
{
    class Program
    {
        /// <summary>
        /// Clase principal del servidor
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Crea un servidor TCP concurrente multihilo en la dirección de loopback en el puerto 12000
            // TODO: Pasar estos parametros a un fichero de configuración 
            var log = LogSingleTon.Instance;
            //log.LogMessage("Starting PictoManagementServer");

            TcpServer serverTcp = new TcpServer(12000);
            serverTcp.Start();
        }
    }
}
