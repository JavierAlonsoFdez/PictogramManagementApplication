using System;
using System.Net.Sockets;

namespace PictoManagementTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            bool runClient = true;
            string Address = "127.0.0.1";
            int port = 12000;
            TcpClient clientTest = new TcpClient(Address, port);
            NetworkStream netStream = clientTest.GetStream();
            Requestor requestor = new Requestor(clientTest, netStream);
            Receiver receiver = new Receiver(clientTest, netStream);

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
                        requestor.PrepareRequest("Get image", "TestFoto");
                        requestor.SendRequest();
                        receiver.ReceiveImage();
                        break;

                    case "2":
                        Console.WriteLine("Ha seleccionado recibir un dashboard de prueba.");
                        requestor.PrepareRequest("Get dashboard", "Test");
                        requestor.SendRequest();
                        receiver.ReceiveDashboard();
                        break;

                    case "3":
                        Console.WriteLine("Ha seleccionado enviar un dashboard de prueba.");
                        requestor.PrepareRequest("Insert dashboard", "Test1,Test2");
                        requestor.SendRequest();
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
