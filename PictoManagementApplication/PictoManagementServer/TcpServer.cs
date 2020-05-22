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
        private List<ClientWorker> listClients;
        private TcpListener tcpListener;

        /// <summary>
        /// Instancia del log para generar una traza
        /// </summary>
        private LogSingleTon log;

        /// <summary>
        /// Constructor de la clase, genera un tcpListener con el puerto indicado
        /// </summary>
        /// <param name="port">Puerto donde va a escuchar el tcpListener</param>
        public TcpServer(int port)
        {
            listClients = null;
            log = LogSingleTon.Instance;
            tcpListener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
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
            while(true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                
                ClientWorker clientWorker = new ClientWorker(tcpClient);
                log.LogMessage("Client accepted.");


                listClients.Add(clientWorker);
                // clientWorker.Start(); // Not implemented
                // Recorro la lista de clientes para comprobar si alguno ya ha acabado de procesar la petición
                // Si ya se ha procesado la petición, ya ha acabado, se quita de la lista de control
                for (int i = 0; i < listClients.Count; i++)
                {
                    if (listClients[i]._t.IsAlive)
                    {
                        listClients.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }
}
