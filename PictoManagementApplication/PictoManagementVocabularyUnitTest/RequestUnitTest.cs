using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictoManagementVocabulary;
using System;

namespace PictoManagementVocabularyUnitTest
{
    [TestClass]
    public class RequestUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Request request = new Request("Image", "Payload for testing");
            BinaryCodec<Request> binCod = new BinaryCodec<Request>();

            byte[] reqCod = binCod.Encode(request);

            Request reqDecod = binCod.Decode(reqCod);

            Assert.AreEqual(request.Type, reqDecod.Type);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Request request = new Request("Image", "Payload for testing");
            BinaryCodec<Request> binCod = new BinaryCodec<Request>();

            byte[] reqCod = binCod.Encode(request);

            Request reqDecod = binCod.Decode(reqCod);

            Assert.AreEqual(request.RequestBody, reqDecod.RequestBody);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Request request = new Request("", "");
            BinaryCodec<Request> binCod = new BinaryCodec<Request>();

            byte[] reqCod = binCod.Encode(request);

            Request reqDecod = binCod.Decode(reqCod);

            Assert.AreEqual(request.Type, reqDecod.Type);
        }

        [TestMethod]
        public void TestMethod4()
        {
            Request request = new Request("", "");
            BinaryCodec<Request> binCod = new BinaryCodec<Request>();

            byte[] reqCod = binCod.Encode(request);

            Request reqDecod = binCod.Decode(reqCod);

            Assert.AreEqual(request.RequestBody, reqDecod.RequestBody);
        }

        [TestMethod]
        public void TestMethod5()
        {
            Request request = new Request("Image", "");
            BinaryCodec<Request> binCod = new BinaryCodec<Request>();

            byte[] reqCod = binCod.Encode(request);

            Request reqDecod = binCod.Decode(reqCod);

            Assert.AreEqual(request.Type, reqDecod.Type);
        }

        [TestMethod]
        public void TestMethod6()
        {
            Request request = new Request("Image", "");
            BinaryCodec<Request> binCod = new BinaryCodec<Request>();

            byte[] reqCod = binCod.Encode(request);

            Request reqDecod = binCod.Decode(reqCod);

            Assert.AreEqual(request.RequestBody, reqDecod.RequestBody);
        }

        [TestMethod]
        public void TestMethod7()
        {
            Request request = new Request("", "Payload for testing");
            BinaryCodec<Request> binCod = new BinaryCodec<Request>();

            byte[] reqCod = binCod.Encode(request);

            Request reqDecod = binCod.Decode(reqCod);

            Assert.AreEqual(request.Type, reqDecod.Type);
        }

        [TestMethod]
        public void TestMethod8()
        {
            Request request = new Request("", "Payload for testing");
            BinaryCodec<Request> binCod = new BinaryCodec<Request>();

            byte[] reqCod = binCod.Encode(request);

            Request reqDecod = binCod.Decode(reqCod);

            Assert.AreEqual(request.RequestBody, reqDecod.RequestBody);
        }

        [TestMethod]
        public void TestMethod9()
        {
            Request request = new Request("Image", "Payload for testing");
            JsonCodec<Request> jsonCod = new JsonCodec<Request>();

            byte[] reqCod = jsonCod.Encode(request);

            Request reqDecod = jsonCod.Decode(reqCod);

            Assert.AreEqual(request.Type, reqDecod.Type);
        }

        [TestMethod]
        public void TestMethod10()
        {
            Request request = new Request("Image", "Payload for testing");
            JsonCodec<Request> jsonCod = new JsonCodec<Request>();

            byte[] reqCod = jsonCod.Encode(request);

            Request reqDecod = jsonCod.Decode(reqCod);

            Assert.AreEqual(request.RequestBody, reqDecod.RequestBody);
        }

        [TestMethod]
        public void TestMethod11()
        {
            Request request = new Request("", "");
            JsonCodec<Request> jsonCod = new JsonCodec<Request>();

            byte[] reqCod = jsonCod.Encode(request);

            Request reqDecod = jsonCod.Decode(reqCod);

            Assert.AreEqual(request.Type, reqDecod.Type);
        }

        [TestMethod]
        public void TestMethod12()
        {
            Request request = new Request("", "");
            JsonCodec<Request> jsonCod = new JsonCodec<Request>();

            byte[] reqCod = jsonCod.Encode(request);

            Request reqDecod = jsonCod.Decode(reqCod);

            Assert.AreEqual(request.RequestBody, reqDecod.RequestBody);
        }

        [TestMethod]
        public void TestMethod13()
        {
            Request request = new Request("Image", "");
            JsonCodec<Request> jsonCod = new JsonCodec<Request>();

            byte[] reqCod = jsonCod.Encode(request);

            Request reqDecod = jsonCod.Decode(reqCod);

            Assert.AreEqual(request.Type, reqDecod.Type);
        }

        [TestMethod]
        public void TestMethod14()
        {
            Request request = new Request("Image", "");
            JsonCodec<Request> jsonCod = new JsonCodec<Request>();

            byte[] reqCod = jsonCod.Encode(request);

            Request reqDecod = jsonCod.Decode(reqCod);

            Assert.AreEqual(request.RequestBody, reqDecod.RequestBody);
        }

        [TestMethod]
        public void TestMethod15()
        {
            Request request = new Request("", "Payload for testing");
            JsonCodec<Request> jsonCod = new JsonCodec<Request>();

            byte[] reqCod = jsonCod.Encode(request);

            Request reqDecod = jsonCod.Decode(reqCod);

            Assert.AreEqual(request.Type, reqDecod.Type);
        }

        [TestMethod]
        public void TestMethod16()
        {
            Request request = new Request("", "Payload for testing");
            JsonCodec<Request> jsonCod = new JsonCodec<Request>();

            byte[] reqCod = jsonCod.Encode(request);

            Request reqDecod = jsonCod.Decode(reqCod);

            Assert.AreEqual(request.RequestBody, reqDecod.RequestBody);
        }
    }
}
