using System;
using System.Threading;
using PictoManagementVocabulary;

namespace PictoManagementClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(2000);
            bool runClient = true;
            string Address = "127.0.0.1";
            int port = 12000;
            BusinessLayer businessLayer = new BusinessLayer(Address, port);

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
                        string[] imagesRequested = new string[1];
                        imagesRequested[0] = "TestFoto";
                        Image[] imagesReceived = businessLayer.RequestImages(imagesRequested);
                        break;

                    case "2":
                        Console.WriteLine("Ha seleccionado recibir un dashboard de prueba.");
                        Dashboard dashboardReceived = businessLayer.GetDashboard("TestDashboard");
                        break;

                    case "3":
                        Console.WriteLine("Ha seleccionado enviar un dashboard de prueba.");
                        businessLayer.SendDashboard("TestDashboard,TestFoto");
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
