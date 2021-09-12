using System;
using Lab2.HashAlgorithm.Constants;

namespace Lab2.HashAlgorithm.Helpers
{
    internal static class BitsHelper
    {
        /// <summary>
        /// Extracts <see langword="UInt32"/> words array
        /// </summary>
        public static UInt32[] Extract32BitWords(Byte[] message, UInt32 blockNo, UInt32 blockSizeInBytes)
        {
            var messageStartIndex = blockNo * blockSizeInBytes;
            var extractedArray = new UInt32[blockSizeInBytes / MD5C.BytesPer32BitWord];

            for (UInt32 i = 0; i < blockSizeInBytes; i += MD5C.BytesPer32BitWord)
            {
                var j = messageStartIndex + i;

                extractedArray[i / MD5C.BytesPer32BitWord] = // form 32-bit word from four bytes
                      message[j]                                                   // first byte
                    | (((UInt32)message[j + 1]) << ((Int32)MD5C.BitsPerByte * 1))  // second byte
                    | (((UInt32)message[j + 2]) << ((Int32)MD5C.BitsPerByte * 2))  // third byte
                    | (((UInt32)message[j + 3]) << ((Int32)MD5C.BitsPerByte * 3)); // fourth byte
            }

            return extractedArray;
        }

        /// <summary>
        /// Shifts value bits for <paramref name="shiftValue"/>
        /// </summary>
        public static UInt32 LeftRotate(UInt32 value, Int32 shiftValue)
        {
            return (value << shiftValue)
                 | (value >> (Int32)(MD5C.BitsPerByte * MD5C.BytesPer32BitWord - shiftValue));
        }

        /// <summary>
        /// Elementary function F(B, C, D)
        /// </summary>
        public static UInt32 FuncF(UInt32 B, UInt32 C, UInt32 D) =>  (B & C) | (~B & D);

        /// <summary>
        /// Elementary function G(B, C, D)
        /// </summary>
        public static UInt32 FuncG(UInt32 B, UInt32 C, UInt32 D) => (D & B) | (C & ~D);

        /// <summary>
        /// Elementary function H(B, C, D)
        /// </summary>
        public static UInt32 FuncH(UInt32 B, UInt32 C, UInt32 D) => B ^ C ^ D;

        /// <summary>
        /// Elementary function I(B, C, D)
        /// </summary>
        public static UInt32 FuncI(UInt32 B, UInt32 C, UInt32 D) => C ^ (B | ~D);
    }
}
