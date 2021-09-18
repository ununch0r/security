﻿using Lab3.RC5Cryptography.Constants;
using Lab3.RC5Cryptography.WordModels;
using System;

namespace Lab3.RC5Cryptography.WordFactories
{
    internal class Word16BitFactory : IWordFactory
    {
        public Int32 BytesPerWord => Word16Bit.BytesPerWord;

        public Int32 BytesPerBlock => BytesPerWord * 2;

        public IWord Create()
        {
            return CreateConcrete();
        }

        public IWord CreateP()
        {
            return CreateConcrete(RC5Constants.P16);
        }

        public IWord CreateQ()
        {
            return CreateConcrete(RC5Constants.Q16);
        }

        public IWord CreateFromBytes(Byte[] bytes, Int32 startFromIndex)
        {
            var word = Create();
            word.CreateFromBytes(bytes, startFromIndex);

            return word;
        }

        private Word16Bit CreateConcrete(UInt16 value = 0)
        {
            return new Word16Bit
            {
                WordValue = value
            };
        }
    }
}
