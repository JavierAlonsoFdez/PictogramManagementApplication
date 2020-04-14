using System;
using System.Collections.Generic;
using System.Text;

namespace PictoManagementVocabulary
{
    /// <summary>
    /// Petición que realizará el usuario contra el servidor
    /// </summary>
    [Serializable()]
    public class Request
    {
        private string _type;
        private string _requestBody;

        /// <summary>
        /// Propiedad pública para acceder a la propiedad privada _type
        /// </summary>
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Propiedad pública para acceder a la propiedad privada _requestBody
        /// </summary>
        public string RequestBody
        {
            get { return _requestBody; }
            set { _requestBody = value; }
        }

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="NewType">Tipo del objeto solicitado</param>
        /// <param name="RequestBody">Cuerpo de la petición</param>
        public Request(string NewType, string RequestBody)
        {
            _type = NewType;
            _requestBody = RequestBody;
        }
    }
}
