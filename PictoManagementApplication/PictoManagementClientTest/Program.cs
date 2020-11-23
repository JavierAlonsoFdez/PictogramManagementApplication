using System;
using System.Collections.Generic;
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
            DataAccess dataAccess = new DataAccess();
            Dictionary<string, string> configDictionary = dataAccess.ConfigDictionary;

            // string Address = "127.0.0.1";
            string Address = configDictionary["Address"];
            //int port = 12000;
            int port = Int32.Parse(configDictionary["Port"]);

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
                        imagesRequested[0] = "abuela";
                        Image[] imagesReceived = businessLayer.RequestImages(imagesRequested);
                        foreach (Image img in imagesReceived)
                        {
                            dataAccess.SaveNewImage(img.Title, img.FileBase64);
                        }
                        break;

                    case "2":
                        Console.WriteLine("Ha seleccionado recibir un dashboard de prueba.");
                        List<Dashboard> dashboardsReceived = businessLayer.GetDashboard("TestDashboard");
                        foreach (Dashboard dashboard in dashboardsReceived)
                        {
                            dataAccess.IncludeDashboardInList(dashboard);
                        }
                        dataAccess.WriteDashboardToDatabase();
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
