using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictoManagementVocabulary;

namespace PictoManagementVocabularyUnitTest
{
    [TestClass]
    public class ImageUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Image image = new Image("Title", "TestPath");
            BinaryCodec<Image> binCod = new BinaryCodec<Image>();
            byte[] imCod = binCod.Encode(image);

            Image imageDecod = binCod.Decode(imCod);

            Assert.AreEqual(image.Title, imageDecod.Title);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Image image = new Image("Title", "TestPath");
            BinaryCodec<Image> binCod = new BinaryCodec<Image>();
            byte[] imCod = binCod.Encode(image);

            Image imageDecod = binCod.Decode(imCod);

            Assert.AreEqual(image.Path, imageDecod.Path);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Image image = new Image("", "");
            BinaryCodec<Image> binCod = new BinaryCodec<Image>();
            byte[] imCod = binCod.Encode(image);

            Image imageDecod = binCod.Decode(imCod);

            Assert.AreEqual(image.Title, imageDecod.Title);
        }

        [TestMethod]
        public void TestMethod4()
        {
            Image image = new Image("", "");
            BinaryCodec<Image> binCod = new BinaryCodec<Image>();
            byte[] imCod = binCod.Encode(image);

            Image imageDecod = binCod.Decode(imCod);

            Assert.AreEqual(image.Path, imageDecod.Path);
        }

        [TestMethod]
        public void TestMethod5()
        {
            Image image = new Image("Title", "");
            BinaryCodec<Image> binCod = new BinaryCodec<Image>();
            byte[] imCod = binCod.Encode(image);

            Image imageDecod = binCod.Decode(imCod);

            Assert.AreEqual(image.Title, imageDecod.Title);
        }

        [TestMethod]
        public void TestMethod6()
        {
            Image image = new Image("Title", "");
            BinaryCodec<Image> binCod = new BinaryCodec<Image>();
            byte[] imCod = binCod.Encode(image);

            Image imageDecod = binCod.Decode(imCod);

            Assert.AreEqual(image.Path, imageDecod.Path);
        }

        [TestMethod]
        public void TestMethod7()
        {
            Image image = new Image("", "Path");
            BinaryCodec<Image> binCod = new BinaryCodec<Image>();
            byte[] imCod = binCod.Encode(image);

            Image imageDecod = binCod.Decode(imCod);

            Assert.AreEqual(image.Title, imageDecod.Title);
        }

        [TestMethod]
        public void TestMethod8()
        {
            Image image = new Image("", "Path");
            BinaryCodec<Image> binCod = new BinaryCodec<Image>();
            byte[] imCod = binCod.Encode(image);

            Image imageDecod = binCod.Decode(imCod);

            Assert.AreEqual(image.Path, imageDecod.Path);
        }

        [TestMethod]
        public void TestMethod9()
        {
            Image image = new Image("Title", "TestPath");
            JsonCodec<Image> jsonCod = new JsonCodec<Image>();
            byte[] imCod = jsonCod.Encode(image);

            Image imageDecod = jsonCod.Decode(imCod);

            Assert.AreEqual(image.Title, imageDecod.Title);
        }

        [TestMethod]
        public void TestMethod10()
        {
            Image image = new Image("Title", "TestPath");
            JsonCodec<Image> jsonCod = new JsonCodec<Image>();
            byte[] imCod = jsonCod.Encode(image);

            Image imageDecod = jsonCod.Decode(imCod);

            Assert.AreEqual(image.Path, imageDecod.Path);
        }

        [TestMethod]
        public void TestMethod11()
        {
            Image image = new Image("", "");
            JsonCodec<Image> jsonCod = new JsonCodec<Image>();
            byte[] imCod = jsonCod.Encode(image);

            Image imageDecod = jsonCod.Decode(imCod);

            Assert.AreEqual(image.Title, imageDecod.Title);
        }

        [TestMethod]
        public void TestMethod12()
        {
            Image image = new Image("", "");
            JsonCodec<Image> jsonCod = new JsonCodec<Image>();
            byte[] imCod = jsonCod.Encode(image);

            Image imageDecod = jsonCod.Decode(imCod);

            Assert.AreEqual(image.Path, imageDecod.Path);
        }

        [TestMethod]
        public void TestMethod13()
        {
            Image image = new Image("Title", "");
            JsonCodec<Image> jsonCod = new JsonCodec<Image>();
            byte[] imCod = jsonCod.Encode(image);

            Image imageDecod = jsonCod.Decode(imCod);

            Assert.AreEqual(image.Title, imageDecod.Title);
        }

        [TestMethod]
        public void TestMethod14()
        {
            Image image = new Image("Title", "");
            JsonCodec<Image> jsonCod = new JsonCodec<Image>();
            byte[] imCod = jsonCod.Encode(image);

            Image imageDecod = jsonCod.Decode(imCod);

            Assert.AreEqual(image.Path, imageDecod.Path);
        }

        [TestMethod]
        public void TestMethod15()
        {
            Image image = new Image("", "Path");
            JsonCodec<Image> jsonCod = new JsonCodec<Image>();
            byte[] imCod = jsonCod.Encode(image);

            Image imageDecod = jsonCod.Decode(imCod);

            Assert.AreEqual(image.Title, imageDecod.Title);
        }

        [TestMethod]
        public void TestMethod16()
        {
            Image image = new Image("", "Path");
            JsonCodec<Image> jsonCod = new JsonCodec<Image>();
            byte[] imCod = jsonCod.Encode(image);

            Image imageDecod = jsonCod.Decode(imCod);

            Assert.AreEqual(image.Path, imageDecod.Path);
        }
    }
}
