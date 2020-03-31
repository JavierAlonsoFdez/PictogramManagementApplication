using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PictoManagementVocabulary
{
    /// <summary>
    /// Codificador JSON parametrizable.
    /// </summary>
    /// <typeparam name="T">Objeto parametrizable.</typeparam>
    public class JsonCodec<T> : ICodec<T>
    {
        /// <summary>
        /// Codifica un objeto en secuencia de bytes.
        /// </summary>
        /// <param name="obj">Objeto parametrizable.</param>
        /// <returns>Retorna una secuencia de bytes que representa al objeto en formato JSON.</returns>
        public byte[] Encode(T obj)
        {
            string outputString = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(outputString);
        }
        /// <summary>
        /// Decodifica una secuencia de bytes a un objeto.
        /// </summary>
        /// <param name="message">Secuencia de bytes a decodificar.</param>
        /// <returns>Retorna un objeto de la clase parametrizada.</returns>
        public T Decode(byte[] message)
        {
            string stringMsg = Encoding.UTF8.GetString(message);
            return JsonConvert.DeserializeObject<T>(stringMsg);
        }
    }
}
