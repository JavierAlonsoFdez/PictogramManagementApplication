using System;
using System.Drawing;
using System.IO;

namespace PictoManagementVocabulary
{
    /// <summary>
    /// Objeto imagen serializable compuesto de un título y una ruta
    /// </summary>
    [Serializable()]
    public class Image
    {
        private string _title;
        private string _path;
        private string _stringBase64;

        /// <summary>
        /// Propiedad pública para acceder a la propiedad _title
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// Propiedad pública para acceder a la propiedad _path
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
        
        /// <summary>
        /// Propiedad pública para acceder a la propiedad _file
        /// </summary>
        public string FileBase64
        {
            get { return _stringBase64; }
            set { _stringBase64 = value; }
        }

        /// <summary>
        /// Constructor de la clase Image
        /// </summary>
        /// <param name="NewTitle">Título de la imagen</param>
        /// <param name="NewPath">Ruta hasta la imagen</param>
        public Image (string NewTitle, string NewPath, string stringBase64 = null)
        {
            _title = NewTitle;
            _path = NewPath;

            if (stringBase64 != null)
            {
                FromBase64(stringBase64, NewPath);
            }

            else
            {
                _stringBase64 = ToBase64(NewPath);
            }
        }

        /// <summary>
        /// Codifica un archivo a un string en base 64
        /// </summary>
        /// <param name="FilePath">Ruta del archivo</param>
        /// <returns>Retorna el archivo en un string en base 64</returns>
        public string ToBase64(string FilePath)
        {
            byte[] bytes = File.ReadAllBytes(FilePath);
            string stringBase64 = Convert.ToBase64String(bytes);

            return stringBase64;
        }

        /// <summary>
        /// A partir del string en base 64 que representa un archivo y su path, genera el archivo
        /// </summary>
        /// <param name="stringBase64">Codificación del archivo en base 64</param>
        /// <param name="NewPath">Ruta del archivo</param>
        public void FromBase64(string stringBase64, string NewPath)
        {
            byte[] bytes = Convert.FromBase64String(stringBase64);
            File.WriteAllBytes(NewPath, bytes);
        }
    }
}
