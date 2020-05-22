using System;
using System.Net.Sockets;
using PictoManagementVocabulary;

namespace PictoManagementTestClient
{
    public class Requestor
    {
        public TcpClient clientTest;
        public NetworkStream netStream;
        public BinaryCodec<Request> binaryCodecReq = new BinaryCodec<Request>();
        public byte[] sndBuffer;

        public Requestor(TcpClient client, NetworkStream ns)
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

        public void PrepareRequest(string Type, string Body)
        {
            Request req = new Request(Type, Body);
            sndBuffer = binaryCodecReq.Encode(req);
        }

        public void SendRequest()
        {
            try
            {
                // Meter binarywritter, todo a un metodo y quitar el buffer
                netStream.Write(sndBuffer, 0, sndBuffer.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " at " + e.StackTrace);
                throw e;
            }
        }
    }
}
