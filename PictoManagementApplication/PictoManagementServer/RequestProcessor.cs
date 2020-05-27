using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PictoManagementVocabulary;

namespace PictoManagementServer
{
    /// <summary>
    /// Procesador de las peticiones
    /// </summary>
    public class RequestProcessor
    {
        private BinaryCodec<Request> _binaryCodec;
        private Request _request;

        /// <summary>
        /// Constructor de la clase, inicializa el codec binario
        /// </summary>
        public RequestProcessor(byte[] message)
        {
            _binaryCodec = new BinaryCodec<Request>();
            DecodeMessage(message);
        }

        /// <summary>
        /// Decodifica el mensaje
        /// </summary>
        /// <param name="message">Array de bytes que contiene la petición codificada</param>
        /// <returns>Retorna un objeto Request que es el mensaje decodificado</returns>
        public void DecodeMessage(byte[] message)
        {
            _request = _binaryCodec.Decode(message);
        }

        /// <summary>
        /// Devuelve el tipo de petición en minúsculas
        /// </summary>
        /// <returns>Propiedad Tipo de la petición en minúsculas</returns>
        public string GetTypeOfRequest()
        {
            return _request.Type.ToLower();
        }

        /// <summary>
        /// Devuelve el cuerpo de la petición en forma de string
        /// </summary>
        /// <returns>Propiedad RequestBody de la petición</returns>
        public string GetBodyOfRequest()
        {
            return _request.RequestBody;
        }
    }
}
