using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictoManagementVocabulary;
using System;

namespace PictoManagementVocabularyUnitTest
{
    [TestClass]
    public class DashboardUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Image[] images = new Image[3];
            for (int i = 0; i < 3; i++)
            {
                Image image = new Image("Title-" + i, "C:\\Users\\Desktop Javier\\Desktop\\TestFoto.png");
                images[i] = image;
            }
            Dashboard dashboard = new Dashboard("Test dashboard", images);
            BinaryCodec<Dashboard> binCod = new BinaryCodec<Dashboard>();

            byte[] dashCod = binCod.Encode(dashboard);

            Dashboard dashDecod = binCod.Decode(dashCod);

            Assert.AreEqual(dashboard.Title, dashDecod.Title);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Image[] images = new Image[3];
            for (int i = 0; i < 3; i++)
            {
                Image image = new Image("Title-" + i, "C:\\Users\\Desktop Javier\\Desktop\\TestFoto.png");
                images[i] = image;
            }
            Dashboard dashboard = new Dashboard("", images);
            BinaryCodec<Dashboard> binCod = new BinaryCodec<Dashboard>();

            byte[] dashCod = binCod.Encode(dashboard);

            Dashboard dashDecod = binCod.Decode(dashCod);

            Assert.AreEqual(dashboard.Title, dashDecod.Title);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Image[] images = new Image[3];
            for (int i = 0; i < 3; i++)
            {
                Image image = new Image("Title-" + i, "C:\\Users\\Desktop Javier\\Desktop\\TestFoto.png");
                images[i] = image;
            }
            Dashboard dashboard = new Dashboard("Test dashboard", images);
            BinaryCodec<Dashboard> binCod = new BinaryCodec<Dashboard>();

            byte[] dashCod = binCod.Encode(dashboard);

            Dashboard dashDecod = binCod.Decode(dashCod);

            for (int i = 0; i < images.Length; i++)
                Assert.AreEqual(dashboard.Images[i].Title, dashDecod.Images[i].Title);
        }

        [TestMethod]
        public void TestMethod4()
        {
            Image[] images = new Image[3];
            for (int i = 0; i < 3; i++)
            {
                Image image = new Image("Title-" + i, "C:\\Users\\Desktop Javier\\Desktop\\TestFoto.png");
                images[i] = image;
            }
            Dashboard dashboard = new Dashboard("Test dashboard", images);
            BinaryCodec<Dashboard> binCod = new BinaryCodec<Dashboard>();

            byte[] dashCod = binCod.Encode(dashboard);

            Dashboard dashDecod = binCod.Decode(dashCod);

            for (int i = 0; i < images.Length; i++)
                Assert.AreEqual(dashboard.Images[i].Path, dashDecod.Images[i].Path);
        }

        [TestMethod]
        public void TestMethod5()
        {
            Image[] images = Array.Empty<Image>();
            Dashboard dashboard = new Dashboard("Test dashboard", images);
            BinaryCodec<Dashboard> binCod = new BinaryCodec<Dashboard>();

            byte[] dashCod = binCod.Encode(dashboard);

            Dashboard dashDecod = binCod.Decode(dashCod);

            for (int i = 0; i < images.Length; i++)
                Assert.AreEqual(dashboard.Images[i].Title, dashDecod.Images[i].Title);
        }

        [TestMethod]
        public void TestMethod6()
        {
            Image[] images = Array.Empty<Image>();
            Dashboard dashboard = new Dashboard("Test dashboard", images);
            BinaryCodec<Dashboard> binCod = new BinaryCodec<Dashboard>();

            byte[] dashCod = binCod.Encode(dashboard);

            Dashboard dashDecod = binCod.Decode(dashCod);

            for (int i = 0; i < images.Length; i++)
                Assert.AreEqual(dashboard.Images[i].Path, dashDecod.Images[i].Path);
        }
    }
}
