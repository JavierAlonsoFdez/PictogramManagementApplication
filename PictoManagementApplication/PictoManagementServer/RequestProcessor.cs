using System;
using System.Collections.Generic;
using System.Text;
using PictoManagementVocabulary;

namespace PictoManagementServer
{
    public class RequestProcessor
    {
        private BinaryCodec<Request> binaryCodec;

        public RequestProcessor()
        {
            binaryCodec = new BinaryCodec<Request>();
        }

        public Request ProcessMessage(byte[] message)
        {
            return binaryCodec.Decode(message);
        }
    }
}
