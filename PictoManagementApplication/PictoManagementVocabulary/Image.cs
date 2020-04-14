using System;

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
        /// Constructor de la clase Image
        /// </summary>
        /// <param name="NewTitle">Título de la imagen</param>
        /// <param name="NewPath">Ruta hasta la imagen</param>
        public Image (string NewTitle, string NewPath)
        {
            _title = NewTitle;
            _path = NewPath;
        }
    }
}
