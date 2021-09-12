using System.Text;

namespace PDS.Lab2.HashingAlgorithms.Tests
{
    public static class MD5Helper
    {
        public static string CreateMD5(string input)
        {
            return CreateMD5(Encoding.UTF8.GetBytes(input));
        }

        public static byte[] CreateMD5Bytes(string input)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();

            return md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        }

        public static string CreateMD5(byte[] inputBytes)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            byte[] hashedBytes = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            for (int i = 0; i < hashedBytes.Length; i++)
            {
                sb.Append(hashedBytes[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
