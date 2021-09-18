using Lab2.HashAlgorithm.Concrete;
using Lab3.RC5Cryptography.Options;
using System;
using System.Linq;

namespace Lab3.RC5Cryptography.Extensions
{
    public static class ByteArrayExtensions
    {
        public static Byte[] GetMD5HashedKeyForRC5(
            this Byte[] key,
            KeyLengthInBytesEnum keyLengthInBytes)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var hasher = new MD5();
            var bytesHash = hasher.ComputeHash(key).ToByteArray();

            if (keyLengthInBytes == KeyLengthInBytesEnum.Bytes_8)
            {
                bytesHash = bytesHash.Take(bytesHash.Length / 2).ToArray();
            }
            else if (keyLengthInBytes == KeyLengthInBytesEnum.Bytes_32)
            {
                bytesHash = bytesHash
                    .Concat(hasher.ComputeHash(bytesHash).ToByteArray())
                    .ToArray();
            }

            if (bytesHash.Length != (int)keyLengthInBytes)
            {
                throw new InvalidOperationException(
                    $"Internal error at {nameof(ByteArrayExtensions.GetMD5HashedKeyForRC5)} method, " +
                    $"hash result is not equal to {(int)keyLengthInBytes}.");
            }

            return bytesHash;
        }

        internal static void XorWith(
            this Byte[] array,
            Byte[] xorArray,
            int inStartIndex,
            int xorStartIndex,
            int length)
        {
            for (int i = 0;  i < length; ++i)
            {
                array[i + inStartIndex] ^= xorArray[i + xorStartIndex];
            }
        }
    }
}
