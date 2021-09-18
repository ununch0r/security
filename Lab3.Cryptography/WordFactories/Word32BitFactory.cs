using Lab3.RC5Cryptography.Constants;
using Lab3.RC5Cryptography.WordModels;
using System;

namespace Lab3.RC5Cryptography.WordFactories
{
    internal class Word32BitFactory : IWordFactory
    {
        public Int32 BytesPerWord => Word32Bit.BytesPerWord;

        public Int32 BytesPerBlock => BytesPerWord * 2;

        public IWord Create()
        {
            return CreateConcrete();
        }

        public IWord CreateP()
        {
            return CreateConcrete(RC5Constants.P32);
        }

        public IWord CreateQ()
        {
            return CreateConcrete(RC5Constants.Q32);
        }

        public IWord CreateFromBytes(Byte[] bytes, Int32 startFromIndex)
        {
            var word = Create();
            word.CreateFromBytes(bytes, startFromIndex);

            return word;
        }

        public Word32Bit CreateConcrete(UInt32 value = 0)
        {
            return new Word32Bit
            {
                WordValue = value
            };
        }
    }
}
