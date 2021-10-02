using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Text;
using Lab2.HashAlgorithm.Concrete;

namespace PDS.Lab2.HashingAlgorithms.Tests
{
    [TestClass]
    public class MD5Tests
    {
        [DataRow("", "D41D8CD98F00B204E9800998ECF8427E")]
        [DataRow("a", "0CC175B9C0F1B6A831C399E269772661")]
        [DataRow("abc", "900150983CD24FB0D6963F7D28E17F72")]
        [DataRow("message digest", "F96B697D7CB7938D525A2F31AAF161D0")]
        [DataRow("abcdefghijklmnopqrstuvwxyz", "C3FCD3D76192E4007DFB496CCA67E13B")]
        [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", "D174AB98D277D9F5A5611C2C9F419D9F")]
        [DataRow("12345678901234567890123456789012345678901234567890123456789012345678901234567890", "57EDF4A22BE3C955AC49DA2E2107B67A")]
        [DataTestMethod]
        public void HashesString(string input, string expectedHash)
        {
            var hasher = new MD5();
            hasher.ComputeHash(input);

            Assert.AreEqual(hasher.HashAsString.ToUpper(), expectedHash);
        }

        [DataRow("", "D41D8CD98F00B204E9800998ECF8427E")]
        [DataRow("a", "0CC175B9C0F1B6A831C399E269772661")]
        [DataRow("abc", "900150983CD24FB0D6963F7D28E17F72")]
        [DataRow("message digest", "F96B697D7CB7938D525A2F31AAF161D0")]
        [DataRow("abcdefghijklmnopqrstuvwxyz", "C3FCD3D76192E4007DFB496CCA67E13B")]
        [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", "D174AB98D277D9F5A5611C2C9F419D9F")]
        [DataRow("12345678901234567890123456789012345678901234567890123456789012345678901234567890", "57EDF4A22BE3C955AC49DA2E2107B67A")]
        [DataTestMethod]
        public void HashesBytes(string input, string expectedHash)
        {
            var hasher = new MD5();
            hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

            Assert.AreEqual(expectedHash, hasher.HashAsString.ToUpper());
        }

        [DataRow("")]
        [DataRow("a")]
        [DataRow("abc")]
        [DataRow("message digest")]
        [DataRow("abcdefghijklmnopqrstuvwxyz")]
        [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")]
        [DataRow("12345678901234567890123456789012345678901234567890123456789012345678901234567890")]
        [DataRow("SYBi4V6eGlUlSLiwvDHCH7vGylhzX6s1ZcL4lCYk2eyNUIWQ9J3WWCA9CGfRMPHqtELFPxn8erNCAiHwTY5aHHhj53gPoYSKxqlXSHTy0wv28cZWvQZPy2QAKH1X7gUnuC9BjhBahfNNBhbRdETzFMlk93CU99fRohu9ZZnk2mTYesRUfwfdiImSYDa3XRYC2bNw6FNiOU2VhBAFgq9J6BOZ0g4PbRgeUy2rXZeEV8lqH1Wdvx5NXL9BZDevOyUuJQY2t4AvYTSusYIFEgyHOORUuV3eI79VwjCsxYSXvksPJNwhf26NvSwgW8QxuW3VvwQ8GhuSL4Qu4EtVN1O5hNWuaD0E88TzzrvdhH0yUobv1okztRS9KeJfnWtYz2NdB3iNReJug5J8GrJP9viKO09BIuBHtpzbvt1sBxVk3Pe4i2NiWWvCn7P3")]
        [DataTestMethod]
        public void HashesSameAsCryptoImpl(string input)
        {
            var hasher = new MD5();
            var myHash = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
            var hash = MD5Helper.CreateMD5Bytes(input);

            Assert.AreEqual(MD5Helper.CreateMD5(input).ToUpper(), hasher.HashAsString.ToUpper());
            Assert.IsTrue(Enumerable.SequenceEqual(myHash.ToByteArray(), hash));
        }

        [DataRow("Data/Test.txt")]
        [DataRow("Data/Test2.txt")]
        [DataRow("Data/test_file")]
        [DataTestMethod]
        public void HashesFileSameAsCryptoImpl(string filePath)
        {
            var message = File.ReadAllBytes(filePath);
            var hasher = new MD5();
            hasher.HashFileAsync(filePath).Wait();

            Assert.AreEqual(MD5Helper.CreateMD5(message).ToUpper(), hasher.HashAsString.ToUpper());
        }

        [DataRow("")]
        [DataRow("a")]
        [DataRow("abc")]
        [DataRow("message digest")]
        [DataRow("abcdefghijklmnopqrstuvwxyz")]
        [DataTestMethod]
        public void ComparesMessageDigests(string input)
        {
            var hasher = new MD5();

            var firstMd = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
            var secondMd = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

            Assert.IsTrue(firstMd.Equals(secondMd));
        }
    }
}
