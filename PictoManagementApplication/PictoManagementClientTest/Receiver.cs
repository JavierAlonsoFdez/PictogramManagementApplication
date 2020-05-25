using System;
using System.IO;
using System.Net.Sockets;
using PictoManagementVocabulary;

namespace PictoManagementClientTest
{
    public class Receiver
    {
        TcpClient clientTest;
        BinaryReader binReader;
        public byte[] rcvBuffer;
        public BinaryCodec<Image> binCodImage = new BinaryCodec<Image>();
        public BinaryCodec<Dashboard> binCodDash = new BinaryCodec<Dashboard>();

        public Receiver(TcpClient client, BinaryReader br)
        {
            try
            {
                clientTest = client;
                binReader = br;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " at " + e.StackTrace);
                throw e;
            }
        }

        public void ReceiveImage()
        {
            using (binReader)
            {
                int bufferSize = binReader.ReadInt32();
                rcvBuffer = binReader.ReadBytes(bufferSize);
            }
            Image imageReceived = binCodImage.Decode(rcvBuffer);
        }

        public void ReceiveDashboard()
        {
            using (binReader)
            {
                int bufferSize = binReader.ReadInt32();
                rcvBuffer = binReader.ReadBytes(bufferSize);
            }
            Dashboard dashboard = binCodDash.Decode(rcvBuffer);
        }
    }
}
