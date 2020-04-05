using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PictoManagementVocabulary
{
    /// <summary>
    /// Codificador JSON parametrizable
    /// </summary>
    /// <typeparam name="T">Tipo de dato</typeparam>
    public class JsonCodec<T> : ICodec<T>
    {
        /// <summary>
        /// Codifica un objeto en una secuencia de bytes
        /// </summary>
        /// <param name="obj">Objeto a codificar</param>
        /// <returns>Retorna una secuencia de bytes que es la representación del objeto en formato JSON</returns>
        public byte[] Encode(T obj)
        {
            string outputString = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(outputString);
        }

        /// <summary>
        /// Decodifica una secuencia de bytes a un objeto
        /// </summary>
        /// <param name="source">Secuencia de bytes a decodificar</param>
        /// <returns>Retorna un objeto de la clase parametrizada</returns>
        public T Decode(byte[] source)
        {
            string inputString = Encoding.UTF8.GetString(source);
            return JsonConvert.DeserializeObject<T>(inputString);
        }
    }
}
