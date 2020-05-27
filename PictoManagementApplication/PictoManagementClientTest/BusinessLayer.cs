using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using PictoManagementVocabulary;
using System.Text;
using System.Threading;

namespace PictoManagementClientTest
{
    public class BusinessLayer
    {
        private IPEndPoint ipEndPoint;
        private TcpClient tcpClient;
        private BinaryReader binReader;
        private BinaryWriter binWriter;

        public BusinessLayer(string newAddress, int newPort)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(newAddress), newPort);
        }

        public void Connect()
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(ipEndPoint);
            NetworkStream netStream = tcpClient.GetStream();

            binReader = new BinaryReader(netStream);
            binWriter = new BinaryWriter(netStream);
        }

        public void Dispose()
        {
            tcpClient.Close();
            tcpClient.Dispose();
        }

        public Image[] RequestImages(string[] imagesRequested)
        {
            Connect();

            BinaryCodec<Image> binCodImage = new BinaryCodec<Image>();
            BinaryCodec<Request> binCodReq = new BinaryCodec<Request>();
            Image[] imagesReceived = new Image[imagesRequested.Length];
            int position = 0;

            foreach (string image in imagesRequested)
            {
                Request request = new Request("Get image", image);
                byte[] sndBuffer = binCodReq.Encode(request);
                using (binReader)
                using (binWriter)
                {
                    binWriter.Write(sndBuffer.Length);
                    binWriter.Write(sndBuffer);
                    
                    int receiveBytes = binReader.ReadInt32();
                    byte[] receivedBuffer = binReader.ReadBytes(receiveBytes);
                    imagesReceived[position] = binCodImage.Decode(receivedBuffer);
                }
                position++;
            }
            Dispose();
            return imagesReceived;
        }

        public void SendDashboard(string dashContent)
        {
            Connect();
            BinaryCodec<Request> binCodReq = new BinaryCodec<Request>();
            Request request = new Request("Insert dashboard", dashContent);
            byte[] sndBuffer = binCodReq.Encode(request);
            using (binWriter)
            {
                binWriter.Write(sndBuffer.Length);
                binWriter.Write(sndBuffer);
            }

            Dispose();
        }

        public Dashboard GetDashboard(string dashName)
        {
            Connect();
            Dashboard newDashboard;

            BinaryCodec<Request> binCodReq = new BinaryCodec<Request>();
            BinaryCodec<Dashboard> binCodDash = new BinaryCodec<Dashboard>();
            Request request = new Request("Get dashboard", dashName);
            byte[] sndBuffer = binCodReq.Encode(request);
            using (binWriter)
            using (binReader)
            {
                binWriter.Write(sndBuffer.Length);
                binWriter.Write(sndBuffer);

                int receiveBytes = binReader.ReadInt32();
                byte[] receivedBuffer = binReader.ReadBytes(receiveBytes);
                newDashboard = binCodDash.Decode(receivedBuffer);
            }


            Dispose();

            return newDashboard;
        }
    }
}
