using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Lab2.HashAlgorithm.Constants;
using Lab2.HashAlgorithm.Helpers;

namespace Lab2.HashAlgorithm.Concrete
{
    /// <summary>
    /// Class with methods to compute hash of input strings,
    /// byte arrays and files
    /// </summary>
    public sealed class MD5
    {
        private const int OptimalChunkSizeMultiplier = 100_000; 
        private const uint OptimalChunkSize = MD5C.BytesCountPerBits512Block * OptimalChunkSizeMultiplier;

        /// <summary>
        /// Computed hash as <see cref="string"/>
        /// </summary>
        public string HashAsString => Hash.ToString();

        /// <summary>
        /// Computed hash as <see cref="MessageDigest"/> model
        /// </summary>
        public MessageDigest Hash { get; private set; }

        /// <summary>
        /// Computes MD5 hash of input message
        /// </summary>
        /// <param name="message">Input message</param>
        /// <returns>Returns hash as <see cref="MessageDigest"/> model</returns>
        public void ComputeHash(string message)
        {
            ComputeHash(Encoding.ASCII.GetBytes(message));
        }

        /// <summary>
        /// Computes MD5 hash of input message
        /// </summary>
        /// <param name="message">Input message, byte array</param>
        /// <returns>Returns hash as <see cref="MessageDigest"/> model</returns>
        public MessageDigest ComputeHash(byte[] message)
        {
            Hash = MessageDigest.InitialValue;

            var paddedMessage = JoinArrays(message, GetMessagePadding((uint)message.Length));

            for (uint blockNumber = 0; blockNumber < paddedMessage.Length / MD5C.BytesCountPerBits512Block; ++blockNumber)
            {
                var bitWords = BitsHelper.ConvertToWords(paddedMessage, blockNumber, MD5C.Words32BitArraySize * MD5C.BytesPer32BitWord);

                Hash += CalculateHashForBlock(bitWords);
            }

            return Hash;
        }

        /// <summary>
        /// Computes MD5 hash of input file
        /// </summary>
        /// <param name="filePath">Path to file to be hashed</param>
        /// <returns></returns>
        public async Task<MessageDigest> HashFileAsync(String filePath)
        {
            Hash = MessageDigest.InitialValue;

            using (var fs = File.OpenRead(filePath))
            {
                UInt64 totalBytesRead = 0;
                Int32 currentBytesRead = 0;
                bool isFileEnd = false;

                do
                {
                    var chunk = new Byte[OptimalChunkSize];

                    currentBytesRead = await fs.ReadAsync(chunk, 0, chunk.Length);
                    totalBytesRead += (UInt64)currentBytesRead;


                    if (currentBytesRead < chunk.Length)
                    {
                        Byte[] lastChunk;

                        if (currentBytesRead == 0)
                        {
                            lastChunk = GetMessagePadding(totalBytesRead);
                        }
                        else
                        {
                            lastChunk = new Byte[currentBytesRead];
                            Array.Copy(chunk, lastChunk, currentBytesRead);

                            lastChunk = JoinArrays(lastChunk, GetMessagePadding(totalBytesRead));
                        }

                        chunk = lastChunk;
                        isFileEnd = true;
                    }

                    for (UInt32 bNo = 0; bNo < chunk.Length / MD5C.BytesCountPerBits512Block; ++bNo)
                    {
                        UInt32[] X = BitsHelper.ConvertToWords(
                            chunk,
                            bNo,
                            MD5C.Words32BitArraySize * MD5C.BytesPer32BitWord);

                        Hash += CalculateHashForBlock(X);
                    }
                }
                while (isFileEnd == false);
            }

            return Hash;
        }

        private MessageDigest CalculateHashForBlock(UInt32[] X)
        {
            uint F, i, k;
            var blockSize = MD5C.BytesCountPerBits512Block;
            var temporaryHash = Hash.Clone();

            // first round
            for (i = 0; i < blockSize / 4; ++i)
            {
                k = i;
                F = BitsHelper.FuncF(temporaryHash.B, temporaryHash.C, temporaryHash.D);

                temporaryHash.IterationSwap(F, X, i, k);
            }
            // second round
            for (; i < blockSize / 2; ++i)
            {
                k = (1 + (5 * i)) % (blockSize / 4);
                F = BitsHelper.FuncG(temporaryHash.B, temporaryHash.C, temporaryHash.D);

                temporaryHash.IterationSwap(F, X, i, k);
            }
            // third round
            for (; i < blockSize / 4 * 3; ++i)
            {
                k = (5 + (3 * i)) % (blockSize / 4);
                F = BitsHelper.FuncH(temporaryHash.B, temporaryHash.C, temporaryHash.D);

                temporaryHash.IterationSwap(F, X, i, k);
            }
            // fourth round
            for (; i < blockSize; ++i)
            {
                k = 7 * i % (blockSize / 4);
                F = BitsHelper.FuncI(temporaryHash.B, temporaryHash.C, temporaryHash.D);

                temporaryHash.IterationSwap(F, X, i, k);
            }

            return temporaryHash;
        }

        private static byte[] GetMessagePadding(ulong messageLength)
        {
            uint paddingLengthInBytes = default;
            var mod = (uint)(messageLength * MD5C.BitsPerByte % MD5C.Bits512BlockSize);

            // Append Padding Bits
            if (mod == MD5C.BITS_448)
            {
                paddingLengthInBytes = MD5C.Bits512BlockSize / MD5C.BitsPerByte;
            }
            else if (mod > MD5C.BITS_448)
            {
                paddingLengthInBytes = (MD5C.Bits512BlockSize - mod + MD5C.BITS_448) / MD5C.BitsPerByte;
            }
            else if (mod < MD5C.BITS_448)
            {
                paddingLengthInBytes = (MD5C.BITS_448 - mod) / MD5C.BitsPerByte;
            }

            var padding = new byte[paddingLengthInBytes + MD5C.BitsPerByte];
            padding[0] = MD5C.BITS_128;

            // Append Length
            var messageLength64Bit = messageLength * MD5C.BitsPerByte;

            for (var i = 0; i < MD5C.BitsPerByte; ++i)
            {
                padding[paddingLengthInBytes + i] = (byte)(messageLength64Bit >> (int)(i * MD5C.BitsPerByte) & MD5C.BITS_255);
            }

            return padding;
        }

        private static T[] JoinArrays<T>(T[] first, T[] second)
        {
            T[] joinedArray = new T[first.Length + second.Length];
            Array.Copy(first, joinedArray, first.Length);
            Array.Copy(second, 0, joinedArray, first.Length, second.Length);

            return joinedArray;
        }
    }
}
