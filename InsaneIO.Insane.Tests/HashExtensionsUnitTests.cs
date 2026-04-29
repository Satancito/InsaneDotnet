using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Security.Cryptography;
using InsaneHashAlgorithm = InsaneIO.Insane.Cryptography.Enums.HashAlgorithm;

namespace InsaneIO.Insane.Tests
{
    [TestClass]
    public class HashExtensionsUnitTests
    {
        private const string Data = "payload";
        private static readonly byte[] DataBytes = Data.ToByteArrayUtf8();

        public static IEnumerable<object[]> HashAlgorithms =>
        [
            [InsaneHashAlgorithm.Md5],
            [InsaneHashAlgorithm.Sha1],
            [InsaneHashAlgorithm.Sha256],
            [InsaneHashAlgorithm.Sha384],
            [InsaneHashAlgorithm.Sha512]
        ];

        [TestMethod]
        [DynamicData(nameof(HashAlgorithms))]
        public void ComputeHash_ShouldMatchFrameworkHashData(InsaneHashAlgorithm algorithm)
        {
            byte[] expected = ComputeFrameworkHash(DataBytes, algorithm);

            DataBytes.ComputeHash(algorithm).Should().Equal(expected);
            Data.ComputeHash(algorithm).Should().Equal(expected);
        }

        [TestMethod]
        [DynamicData(nameof(HashAlgorithms))]
        public void VerifyHashFromEncoded_ShouldCompareEncodedHash(InsaneHashAlgorithm algorithm)
        {
            byte[] expectedBytes = Data.ComputeHash(algorithm);
            string expected = Data.ComputeHashEncoded(HexEncoder.DefaultInstance, algorithm);

            Data.VerifyHash(expectedBytes, algorithm).Should().BeTrue();
            DataBytes.VerifyHash(expectedBytes, algorithm).Should().BeTrue();
            OtherData().VerifyHash(expectedBytes, algorithm).Should().BeFalse();
            Data.ComputeHashEncoded(HexEncoder.DefaultInstance, algorithm).Should().Be(expected);
            Data.VerifyHashFromEncoded(expected, HexEncoder.DefaultInstance, algorithm).Should().BeTrue();
            DataBytes.VerifyHashFromEncoded(expected, HexEncoder.DefaultInstance, algorithm).Should().BeTrue();
            Data.VerifyHashFromEncoded(expected + "00", HexEncoder.DefaultInstance, algorithm).Should().BeFalse();
        }

        private static string OtherData()
        {
            return "other payload";
        }

        private static byte[] ComputeFrameworkHash(byte[] data, InsaneHashAlgorithm algorithm)
        {
            return algorithm switch
            {
                InsaneHashAlgorithm.Md5 => MD5.HashData(data),
                InsaneHashAlgorithm.Sha1 => SHA1.HashData(data),
                InsaneHashAlgorithm.Sha256 => SHA256.HashData(data),
                InsaneHashAlgorithm.Sha384 => SHA384.HashData(data),
                InsaneHashAlgorithm.Sha512 => SHA512.HashData(data),
                _ => throw new NotImplementedException(algorithm.ToString())
            };
        }
    }
}
