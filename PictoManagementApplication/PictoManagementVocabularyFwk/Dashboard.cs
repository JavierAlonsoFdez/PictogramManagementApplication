using System;
using System.Collections.Generic;
using System.Text;

namespace PictoManagementVocabulary
{
    /// <summary>
    /// Objeto dashboard serializable, compuesto por un título y un grupo de imágenes
    /// </summary>
    [Serializable()]
    public class Dashboard
    {
        private string _title;
        private Image[] _images;
        
        /// <summary>
        /// Propiedad pública para acceder a la propiedad _title
        /// </summary>
        public string Name
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// Propiedad pública para acceder a la propiedad _images
        /// </summary>
        public Image[] Images
        {
            get { return _images; }
            set { _images = value; }
        }
        
        /// <summary>
        /// Constructor de la clase Dashboard
        /// </summary>
        /// <param name="NewTitle">Título del dashboard (tablero)</param>
        /// <param name="NewImages">Conjunto de imágenes que forman parte del dashboard</param>
        public Dashboard (string NewTitle, Image[] NewImages)
        {
            _title = NewTitle;
            _images = NewImages;
        }
    }
}
