using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;


namespace PictoManagementVocabulary
{
    public class BinaryCodec<T> : ICodec<T>
    {
        public byte[] Encode(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public T Decode(byte[] message)
        {
            using (MemoryStream ms = new MemoryStream(message))
            {
                var formatter = new BinaryFormatter();
                ms.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
