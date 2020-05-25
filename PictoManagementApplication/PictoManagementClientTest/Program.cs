using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace PictoManagementClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            bool runClient = true;
            string Address = "127.0.0.1";
            int port = 12000;
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(Address), port);
            TcpClient clientTest = new TcpClient();
            clientTest.Connect(ipEndPoint);
            NetworkStream netStream = clientTest.GetStream();
            BinaryReader binReader = new BinaryReader(netStream);
            BinaryWriter binWriter = new BinaryWriter(netStream);
            Requestor requestor = new Requestor(clientTest, binWriter);
            Receiver receiver = new Receiver(clientTest, binReader);

            Console.WriteLine("--- Bienvenido al PictoManagementTestClient ---");

            do
            {

                Console.WriteLine("Elige entre las siguientes opciones para realizar un test:");
                Console.WriteLine("1.- Recibir una imagen de prueba.");
                Console.WriteLine("2.- Recibir un dashboard de prueba.");
                Console.WriteLine("3.- Enviar un dashboard de prueba.");
                Console.WriteLine("Para salir, pulsa cualquier otra tecla.");
                string selection = Console.ReadLine();

                switch (selection)
                {
                    case "1":
                        Console.WriteLine("Ha seleccionado recibir una imagen de prueba.");
                        requestor.PrepareAndSendRequest("Get image", "TestFoto");
                        receiver.ReceiveImage();
                        break;

                    case "2":
                        Console.WriteLine("Ha seleccionado recibir un dashboard de prueba.");
                        requestor.PrepareAndSendRequest("Get dashboard", "Test");
                        receiver.ReceiveDashboard();
                        break;

                    case "3":
                        Console.WriteLine("Ha seleccionado enviar un dashboard de prueba.");
                        requestor.PrepareAndSendRequest("Insert dashboard", "Test1,Test2");
                        break;

                    default:
                        runClient = false;
                        break;
                }
            }
            while (runClient);

            Console.WriteLine("--- PictoManagementTestClient ha finalizado. Gracias ---");
            Console.ReadKey();
        }
    }
}
