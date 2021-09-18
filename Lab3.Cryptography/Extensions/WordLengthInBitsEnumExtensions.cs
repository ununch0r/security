using Lab3.RC5Cryptography.Options;
using Lab3.RC5Cryptography.WordFactories;
using System;

namespace Lab3.RC5Cryptography.Extensions
{
    internal static class WordLengthInBitsEnumExtensions
    {
        internal static IWordFactory GetWordFactory(this WordLengthInBitsEnum wordLengthInBits)
        {
            IWordFactory wordFactory = null;

            switch (wordLengthInBits)
            {
                case WordLengthInBitsEnum.Bit16:
                    wordFactory = new Word16BitFactory();
                    break;
                case WordLengthInBitsEnum.Bit32:
                    wordFactory = new Word32BitFactory();
                    break;
                case WordLengthInBitsEnum.Bit64:
                    wordFactory = new Word64BitFactory();
                    break;
                default:
                    throw new ArgumentException($"Invalid {nameof(WordLengthInBitsEnum)} value.");
            }

            return wordFactory;
        }
    }
}
