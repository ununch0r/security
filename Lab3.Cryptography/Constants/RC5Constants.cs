using System;

namespace Lab3.RC5Cryptography.Constants
{
    public static class RC5Constants
    {
        public const UInt16 P16 = 0xB7E1;
        public const UInt16 Q16 = 0x9E37;
        public const UInt32 P32 = 0xB7E15162;
        public const UInt32 Q32 = 0x9E3779B9;
        public const UInt64 P64 = 0xB7E151628AED2A6B;
        public const UInt64 Q64 = 0x9E3779B97F4A7C15;

        public const Int32 BitsPerByte = 8;
        public const Int32 ByteMask = 0b11111111;

        public const Int32 MinRoundCount = 0;
        public const Int32 MaxRoundCount = 255;
        public const Int32 MinSecretKeyOctetsCount = 0;
        public const Int32 MaxSecretKeyOctetsCount = 255;
        public const Int32 MinKeySizeInBytes = 0;
        public const Int32 MaxKeySizeInBytes = 2040;
    }
}
