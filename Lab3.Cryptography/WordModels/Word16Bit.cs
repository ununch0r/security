using RC5C = Lab3.RC5Cryptography.Constants.RC5Constants;
using System;

namespace Lab3.RC5Cryptography.WordModels
{
    internal class Word16Bit : IWord
    {
        public const Int32 WordSizeInBits = BytesPerWord * RC5C.BitsPerByte;
        public const Int32 BytesPerWord = sizeof(UInt16);

        public UInt16 WordValue { get; set; }

        public void CreateFromBytes(Byte[] bytes, Int32 startFrom)
        {
            WordValue = 0;

            for (var i = startFrom + BytesPerWord - 1; i > startFrom; --i)
            {
                WordValue = (UInt16)(WordValue | bytes[i]);
                WordValue = (UInt16)(WordValue << RC5C.BitsPerByte);
            }

            WordValue = (UInt16)(WordValue | bytes[startFrom]);
        }

        public Byte[] FillBytesArray(Byte[] bytesToFill, Int32 startFrom)
        {
            var i = 0;
            for (; i < BytesPerWord - 1; ++i)
            {
                bytesToFill[startFrom + i] = (Byte)(WordValue & RC5C.ByteMask);
                WordValue = (UInt16)(WordValue >> RC5C.BitsPerByte);
            }

            bytesToFill[startFrom + i] = (Byte)(WordValue & RC5C.ByteMask);

            return bytesToFill;
        }

        public IWord ROL(Int32 offset)
        {
            offset %= BytesPerWord;
            WordValue = (UInt16)((WordValue << offset) | (WordValue >> (WordSizeInBits - offset)));

            return this;
        }

        public IWord ROR(Int32 offset)
        {
            offset %= BytesPerWord;
            WordValue = (UInt16)((WordValue >> offset) | (WordValue << (WordSizeInBits - offset)));

            return this;
        }

        public IWord Add(IWord word)
        {
            WordValue = (UInt16)(WordValue + (word as Word16Bit).WordValue);

            return this;
        }

        public IWord Add(Byte value)
        {
            WordValue = (UInt16)(WordValue + value);

            return this;
        }

        public IWord Sub(IWord word)
        {
            WordValue = (UInt16)(WordValue - (word as Word16Bit).WordValue);

            return this;
        }

        public IWord XorWith(IWord word)
        {
            WordValue = (UInt16)(WordValue ^ (word as Word16Bit).WordValue);

            return this;
        }

        public IWord Clone()
        {
            return (IWord)MemberwiseClone();
        }

        public Int32 ToInt32()
        {
            return WordValue;
        }
    }
}
