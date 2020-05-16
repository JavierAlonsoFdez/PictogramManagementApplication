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
        private List<Image> _imageList = new List<Image>();
        private BinaryCodec<Image> binCod = new BinaryCodec<Image>();

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="imagesString">Recibe un string que es el cuerpo de la petición</param>
        public ImageRequestProcessor(string imagesString)
        {
            string[] imagesArray = imagesString.Split(",");

            foreach (string img in imagesArray)
            {
                string[] files = Directory.GetFiles("C:\\Users\\Desktop Javier\\Desktop\\" + img + "*.*"); // Cambiar el string al directorio real de los pictogramas
                foreach (string file in files)
                {
                    _imageList.Add(new Image(img, file));
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
