using System;
using System.Linq;
using Lab2.HashAlgorithm.Constants;
using Lab2.HashAlgorithm.Helpers;

namespace Lab2.HashAlgorithm.Concrete
{
    public class MessageDigest
    {
        internal static MessageDigest InitialValue { get; }

        static MessageDigest()
        {
            InitialValue = new MessageDigest
            {
                A = MD5C.InitialA,
                B = MD5C.InitialB,
                C = MD5C.InitialC,
                D = MD5C.InitialD
            };
        }

        public uint A { get; set; }

        public uint B { get; set; }

        public uint C { get; set; }

        public uint D { get; set; }

        public Byte[] ToByteArray()
        {
            return ArraysHelper.ConcatArrays(
                BitConverter.GetBytes(A),
                BitConverter.GetBytes(B),
                BitConverter.GetBytes(C),
                BitConverter.GetBytes(D));
        }

        public MessageDigest Clone()
        {
            return MemberwiseClone() as MessageDigest;
        }

        internal void IterationSwap(UInt32 F, UInt32[] X, UInt32 i, UInt32 k)
        {
            var tempD = D;
            D = C;
            C = B;
            B += BitsHelper.LeftRotate(A + F + X[k] + MD5C.T[i], MD5C.S[i]);
            A = tempD;
        }

        public override string ToString()
        {
            return $"{ToByteString(A)}{ToByteString(B)}{ToByteString(C)}{ToByteString(D)}";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object value)
        {
            return value is MessageDigest md
                && (GetHashCode() == md.GetHashCode() || ToString() == md.ToString());
        }

        public static MessageDigest operator+(MessageDigest left, MessageDigest right)
        {
            return new MessageDigest
            {
                A = left.A + right.A,
                B = left.B + right.B,
                C = left.C + right.C,
                D = left.D + right.D
            };
        }

        private static string ToByteString(uint x)
        {
            return string.Join(string.Empty, BitConverter.GetBytes(x).Select(y => y.ToString("x2")));
        }
    }
}
