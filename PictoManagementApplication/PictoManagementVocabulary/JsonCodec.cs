using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PictoManagementVocabulary
{
    public class JsonCodec<T> : ICodec<T>
    {
        public byte[] Encode(T obj)
        {
            string outputString = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(outputString);
        }

        public T Decode(byte[] source)
        {
            string inputString = Encoding.UTF8.GetString(source);
            return JsonConvert.DeserializeObject<T>(inputString);
        }
    }
}
