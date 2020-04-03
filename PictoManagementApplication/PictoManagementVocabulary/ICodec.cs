using System;
using System.Collections.Generic;
using System.Text;

namespace PictoManagementVocabulary
{
    public interface ICodec<T>
    {
        byte[] Encode(T obj);
        T Decode(byte[] source);
    }
}
