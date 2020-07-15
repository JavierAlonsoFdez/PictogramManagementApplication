using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace PictoManagementVocabulary
{
    /// <summary>
    /// Codificador binario parametrizable
    /// </summary>
    /// <typeparam name="T">Tipo de dato</typeparam>
    public class BinaryCodec<T>: ICodec<T>
    {
        /// <summary>
        /// Codifica un objeto en una secuencia de bytes
        /// </summary>
        /// <param name="obj">Objeto a codificar</param>
        /// <returns>Retorna una secuencia de bytes que es la representación del objeto en binario</returns>
        public byte[] Encode(T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Decodifica una secuencia de bytes a un objeto
        /// </summary>
        /// <param name="source">Secuencia de bytes a decodificar</param>
        /// <returns>Retorna un objeto de la clase parametrizada</returns>
        public T Decode(byte[] source)
        {
            using (MemoryStream ms = new MemoryStream(source))
            {
                var formatter = new BinaryFormatter();
                ms.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
