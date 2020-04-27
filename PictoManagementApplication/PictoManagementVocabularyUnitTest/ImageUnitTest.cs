using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictoManagementVocabulary;
using System;

namespace PictoManagementVocabularyUnitTest
{
    [TestClass]
    public class ImageUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Image image = new Image("Title", "C:\\Users\\Desktop Javier\\Desktop\\TestFoto.png");
            BinaryCodec<Image> binCod = new BinaryCodec<Image>();
            byte[] imCod = binCod.Encode(image);

            Image imageDecod = binCod.Decode(imCod);

            Assert.AreEqual(image.Title, imageDecod.Title);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Image image = new Image("Title", "C:\\Users\\Desktop Javier\\Desktop\\TestFoto.png");
            BinaryCodec<Image> binCod = new BinaryCodec<Image>();
            byte[] imCod = binCod.Encode(image);

            Image imageDecod = binCod.Decode(imCod);

            Assert.AreEqual(image.Path, imageDecod.Path);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Image image = new Image("Title", "C:\\Users\\Desktop Javier\\Desktop\\TestFoto.png");
            BinaryCodec<Image> binCod = new BinaryCodec<Image>();
            byte[] imCod = binCod.Encode(image);

            Image imageDecod = binCod.Decode(imCod);

            Assert.AreEqual(image.FileBase64, imageDecod.FileBase64);
        }

        [TestMethod]
        public void TestMethod4()
        {
            string imageBase64 = "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";

            Image image = new Image("Title", "C:\\Users\\Desktop Javier\\Desktop\\TestMoto.png", imageBase64);
            BinaryCodec<Image> binCod = new BinaryCodec<Image>();
            byte[] imCod = binCod.Encode(image);

            Image imageDecod = binCod.Decode(imCod);

            Assert.AreEqual(image.FileBase64, imageDecod.FileBase64);
        }

        [TestMethod]
        public void TestMethod5()
        {
            Image image = new Image("", "C:\\Users\\Desktop Javier\\Desktop\\TestFoto.png");
            BinaryCodec<Image> binCod = new BinaryCodec<Image>();
            byte[] imCod = binCod.Encode(image);

            Image imageDecod = binCod.Decode(imCod);

            Assert.AreEqual(image.Title, imageDecod.Title);
        }

        [TestMethod]
        public void TestMethod6()
        {
            Image image = new Image("", "C:\\Users\\Desktop Javier\\Desktop\\TestFoto.png");
            BinaryCodec<Image> binCod = new BinaryCodec<Image>();
            byte[] imCod = binCod.Encode(image);

            Image imageDecod = binCod.Decode(imCod);

            Assert.AreEqual(image.Path, imageDecod.Path);
        }

        [TestMethod]
        public void TestMethod7()
        {
            Image image = new Image("", "C:\\Users\\Desktop Javier\\Desktop\\TestFoto.png");
            BinaryCodec<Image> binCod = new BinaryCodec<Image>();
            byte[] imCod = binCod.Encode(image);

            Image imageDecod = binCod.Decode(imCod);

            Assert.AreEqual(image.FileBase64, imageDecod.FileBase64);
        }

        [TestMethod]
        public void TestMethod8()
        {
            try
            {
                Image image = new Image("Title", "");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Empty path name is not legal." + Environment.NewLine + "Parameter name: path", ae.Message);
            }
            catch (Exception e)
            {
                Assert.Fail(string.Format("Unexpected exception of type {0} caught: {1}",e.GetType(), e.Message));
            }
        }
    }
}
