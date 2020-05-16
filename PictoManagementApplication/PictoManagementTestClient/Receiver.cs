using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using PictoManagementVocabulary;

namespace PictoManagementTestClient
{
    public class Receiver
    {
        TcpClient clientTest;
        NetworkStream netStream;
        public byte[] rcvBuffer;
        public BinaryCodec<Image> binCodImage = new BinaryCodec<Image>();
        public BinaryCodec<Dashboard> binCodDash = new BinaryCodec<Dashboard>();

        public Receiver (TcpClient client, NetworkStream ns)
        {
            try
            {
                clientTest = client;
                netStream = ns;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " at " + e.StackTrace);
                throw e;
            }   
        }

        public void ReceiveImage()
        {
            netStream.Read(rcvBuffer, 0, clientTest.ReceiveBufferSize);
            Image imageReceived = binCodImage.Decode(rcvBuffer);
        }

        public void ReceiveDashboard()
        {
            netStream.Read(rcvBuffer, 0, clientTest.ReceiveBufferSize);
            Dashboard dashboard = binCodDash.Decode(rcvBuffer);
        }
    }
}
