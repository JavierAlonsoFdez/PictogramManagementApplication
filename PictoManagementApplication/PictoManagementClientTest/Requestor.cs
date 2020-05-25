using System;
using System.IO;
using System.Net.Sockets;
using PictoManagementVocabulary;

namespace PictoManagementClientTest
{
    public class Requestor
    {
        public TcpClient clientTest;
        public BinaryWriter binWriter;
        public BinaryCodec<Request> binaryCodecReq = new BinaryCodec<Request>();
        public byte[] sndBuffer;

        public Requestor(TcpClient client, BinaryWriter bw)
        {
            try
            {
                clientTest = client;
                binWriter = bw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " at " + e.StackTrace);
                throw e;
            }
        }

        public void PrepareAndSendRequest(string Type, string Body)
        {
            Request req = new Request(Type, Body);
            sndBuffer = binaryCodecReq.Encode(req);
            using (binWriter)
            {
                binWriter.Write(sndBuffer.Length);
                binWriter.Write(sndBuffer);
            }

        }
    }
}
