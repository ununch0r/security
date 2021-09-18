using RC5C = Lab3.RC5Cryptography.Constants.RC5Constants;
using System;

namespace Lab3.RC5Cryptography.WordModels
{
    internal class Word32Bit : IWord
    {
        public const Int32 WordSizeInBits = BytesPerWord * RC5C.BitsPerByte;
        public const Int32 BytesPerWord = sizeof(UInt32);

        public UInt32 WordValue { get; set; }

        public void CreateFromBytes(Byte[] bytes, Int32 startFrom)
        {
            WordValue = 0;

            for (var i = startFrom + BytesPerWord - 1; i > startFrom; --i)
            {
                WordValue |= bytes[i];
                WordValue <<= RC5C.BitsPerByte;
            }

            WordValue |= bytes[startFrom];
        }

        public Byte[] FillBytesArray(Byte[] bytesToFill, Int32 startFrom)
        {
            var i = 0;
            for (; i < BytesPerWord - 1; ++i)
            {
                bytesToFill[startFrom + i] = (Byte)(WordValue & RC5C.ByteMask);
                WordValue >>= RC5C.BitsPerByte;
            }

            bytesToFill[startFrom + i] = (Byte)(WordValue & RC5C.ByteMask);

            return bytesToFill;
        }

        public IWord ROL(Int32 offset)
        {
            WordValue = (WordValue << offset) | (WordValue >> (WordSizeInBits - offset));

            return this;
        }

        public IWord ROR(Int32 offset)
        {
            WordValue = (WordValue >> offset) | (WordValue << (WordSizeInBits - offset));

            return this;
        }

        public IWord Add(IWord word)
        {
            WordValue += (word as Word32Bit).WordValue;

            return this;
        }

        public IWord Add(Byte value)
        {
            WordValue += value;

            return this;
        }

        public IWord Sub(IWord word)
        {
            WordValue -= (word as Word32Bit).WordValue;

            return this;
        }

        public IWord XorWith(IWord word)
        {
            WordValue ^= (word as Word32Bit).WordValue;

            return this;
        }

        public IWord Clone()
        {
            return (IWord)MemberwiseClone();
        }

        public Int32 ToInt32()
        {
            return (Int32)WordValue;
        }
    }
}
