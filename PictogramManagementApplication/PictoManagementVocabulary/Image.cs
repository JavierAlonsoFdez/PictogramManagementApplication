using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictoManagementVocabulary
{
    /// <summary>
    /// Clase Image.
    /// </summary>
    public class Image
    {
        private string _title;
        private string[] _tag;
        private string _path;
        /// <summary>
        /// Método público para obtener y dar valor a la propiedad privada de la clase _title.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        /// <summary>
        /// Método público para obtener y dar valor a la propiedad privada de la clase _path.
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
        /// <summary>
        /// Constructor de la clase Image.
        /// </summary>
        /// <param name="NewTitle">Título de la imagen a procesar.</param>
        /// <param name="NewPath">Ruta donde se encuentra el archivo de la imagen.</param>
        public Image(string NewTitle, string NewPath)
        {
            _title = NewTitle;
            _path = NewPath;
        }
    }
}
