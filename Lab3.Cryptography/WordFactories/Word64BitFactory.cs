using Lab3.RC5Cryptography.Constants;
using Lab3.RC5Cryptography.WordModels;
using System;

namespace Lab3.RC5Cryptography.WordFactories
{
    internal class Word64BitFactory : IWordFactory
    {
        public Int32 BytesPerWord => Word64Bit.BytesPerWord;

        public Int32 BytesPerBlock => BytesPerWord * 2;

        public IWord Create()
        {
            return CreateConcrete();
        }

        public IWord CreateP()
        {
            return CreateConcrete(RC5Constants.P64);
        }

        public IWord CreateQ()
        {
            return CreateConcrete(RC5Constants.Q64);
        }

        public IWord CreateFromBytes(Byte[] bytes, Int32 startFromIndex)
        {
            var word = Create();
            word.CreateFromBytes(bytes, startFromIndex);

            return word;
        }

        private Word64Bit CreateConcrete(UInt64 value = 0)
        {
            return new Word64Bit
            {
                WordValue = value
            };
        }
    }
}
