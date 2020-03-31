using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictoManagementVocabulary
{
    /// <summary>
    /// Interfaz de codificación.
    /// </summary>
    /// <typeparam name="T">Clase parametrizable.</typeparam>
    interface ICodec<T>
    {
        /// <summary>
        /// Codifica un objeto T.
        /// </summary>
        /// <param name="obj">Objeto a codificar de la clase parametrizada.</param>
        /// <returns>Retorna un conjunto de bytes que representa al objeto codificado.</returns>
        byte[] Encode(T obj);
        /// <summary>
        /// Decodifica un array de bytes en un objeto parametrizada.
        /// </summary>
        /// <param name="message">Conjunto de bytes que representa al objeto codificado.</param>
        /// <returns>Retorna un objeto de la clase parametrizada.</returns>
        T Decode(byte[] message);
    }
}
