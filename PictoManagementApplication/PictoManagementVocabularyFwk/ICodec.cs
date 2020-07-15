using System;
using System.Collections.Generic;
using System.Text;

namespace PictoManagementVocabulary
{
    /// <summary>
    /// Interfaz de codificación
    /// </summary>
    /// <typeparam name="T">Clase parametrizable</typeparam>
    public interface ICodec<T>
    {
        /// <summary>
        /// Codifica en una secuencia de bytes
        /// </summary>
        /// <param name="obj">Objeto a codificar</param>
        /// <returns>Retorna una secuencia de bytes que representa la codificación del objeto</returns>
        byte[] Encode(T obj);
        
        /// <summary>
        /// Decodifica una secuencia de bytes en un objeto
        /// </summary>
        /// <param name="source">Secuencia de bytes a decodificar</param>
        /// <returns>Retorna un objeto de la clase parametrizada</returns>
        T Decode(byte[] source);
    }
}
