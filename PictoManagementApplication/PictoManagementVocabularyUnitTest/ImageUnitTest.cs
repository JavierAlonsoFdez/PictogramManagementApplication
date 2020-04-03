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
    }
}
