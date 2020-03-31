using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictoManagementVocabulary
{
    /// <summary>
    /// Clase Dashboard (conjunto de imagenes bajo un mismo título).
    /// </summary>
    public class Dashboard
    {
        private string _title;
        private Image[] _pictograms;
        /// <summary>
        /// Método público para obtener y dar valor a la propiedad privada de la clase _title.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        /// <summary>
        /// Método público para obtener el valor de la propiedad privada de la clase _pictograms.
        /// </summary>
        public Image[] Images
        {
            get { return _pictograms; }
        }
        /// <summary>
        /// Constructor de la clase Dashboard
        /// </summary>
        /// <param name="NewTitle">Título que se quiere dar al tablero.</param>
        /// <param name="NewSize">Número de pictogramas que contiene el tablero.</param>
        /// <param name="NewImages">Pictogramas contenidos en el tablero.</param>
        public Dashboard(string NewTitle, Image[] NewImages)
        {
            _title = NewTitle;
            _pictograms = NewImages;
        }
    }
}
