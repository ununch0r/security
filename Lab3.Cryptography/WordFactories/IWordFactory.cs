using Lab3.RC5Cryptography.WordModels;
using System;

namespace Lab3.RC5Cryptography.WordFactories
{
    internal interface IWordFactory
    {
        Int32 BytesPerWord { get; }
        Int32 BytesPerBlock { get; }
        IWord CreateQ();
        IWord CreateP();
        IWord Create();
        IWord CreateFromBytes(byte[] bytes, Int32 startFrom);
    }
}
