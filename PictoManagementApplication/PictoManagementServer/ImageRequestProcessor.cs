using System;
using System.Collections.Generic;
using PictoManagementVocabulary;
using System.Text;
using System.IO;

namespace PictoManagementServer
{
    /// <summary>
    /// Procesador de peticiones de tipo imagen
    /// </summary>
    class ImageRequestProcessor
    {
        private List<Image> _imageList;
        private BinaryCodec<Image> binCod;

        /// <summary>
        /// Instancia del log para generar una traza
        /// </summary>
        private LogSingleTon log;

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="imagesString">Recibe un string que es el cuerpo de la petición</param>
        public ImageRequestProcessor(string imagesString)
        {
            log = LogSingleTon.Instance;
            _imageList = new List<Image>();
            binCod = new BinaryCodec<Image>();
            string[] imagesArray = imagesString.Split(",");

            foreach (string img in imagesArray)
            {
                string firstLetter = img.Substring(0, 1);
                string secondLetter = img.Substring(1, 1);
                string[] files = Directory.GetFiles("C:\\Program Files\\PictoManagementApplication\\Images\\" + firstLetter + "\\" + secondLetter + "\\");
                foreach (string file in files)
                {
                    string filename = file.Split("C:\\Program Files\\PictoManagementApplication\\Images\\ "+ firstLetter + "\\" + secondLetter + "\\")[1];
                    if (filename.Contains(img))
                    {
                        _imageList.Add(new Image(img, file));
                    }
                }
            }
        }

        /// <summary>
        /// Retorna la lista de imágenes creada en el constructor
        /// </summary>
        /// <returns>Retorna la lista de imágenes creada en el constructor</returns>
        public List<Image> GetImages()
        {
            return _imageList;
        }

        /// <summary>
        /// Codifica una imagen a un array de bytes
        /// </summary>
        /// <param name="img">Imagen a codificar en un array de bytes</param>
        /// <returns>Conjunto de bytes que representa la imagen codificada</returns>
        public byte[] CodeImageForSending(Image img)
        {
           return binCod.Encode(img);
        }
    }
}
