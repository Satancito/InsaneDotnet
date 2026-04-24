using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Security.Cryptography;
using InsaneHashAlgorithm = InsaneIO.Insane.Cryptography.HashAlgorithm;

namespace InsaneIO.Insane.Tests
{
    [TestClass]
    public class HmacExtensionsUnitTests
    {
        private const string Data = "payload";
        private const string Key = "secret";
        private static readonly byte[] DataBytes = Data.ToByteArrayUtf8();
        private static readonly byte[] KeyBytes = Key.ToByteArrayUtf8();

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
        public void ComputeHmac_ShouldMatchFrameworkHmac(InsaneHashAlgorithm algorithm)
        {
            byte[] expected = ComputeFrameworkHmac(DataBytes, KeyBytes, algorithm);

            DataBytes.ComputeHmac(KeyBytes, algorithm).Should().Equal(expected);
            Data.ComputeHmac(KeyBytes, algorithm).Should().Equal(expected);
            Data.ComputeHmac(Key, algorithm).Should().Equal(expected);
            DataBytes.ComputeHmac(Key, algorithm).Should().Equal(expected);
        }

        [TestMethod]
        [DynamicData(nameof(HashAlgorithms))]
        public void VerifyHmacFromEncoded_ShouldSupportEveryKeyOverload(InsaneHashAlgorithm algorithm)
        {
            string expected = Data.ComputeHmacEncoded(Key, Base64Encoder.DefaultInstance, algorithm);

            Data.VerifyHmacFromEncoded(Key, expected, Base64Encoder.DefaultInstance, algorithm).Should().BeTrue();
            Data.VerifyHmacFromEncoded(KeyBytes, expected, Base64Encoder.DefaultInstance, algorithm).Should().BeTrue();
            DataBytes.VerifyHmacFromEncoded(Key, expected, Base64Encoder.DefaultInstance, algorithm).Should().BeTrue();
            DataBytes.VerifyHmacFromEncoded(KeyBytes, expected, Base64Encoder.DefaultInstance, algorithm).Should().BeTrue();
            Data.VerifyHmacFromEncoded(Key, expected + "A", Base64Encoder.DefaultInstance, algorithm).Should().BeFalse();
        }

        private static byte[] ComputeFrameworkHmac(byte[] data, byte[] key, InsaneHashAlgorithm algorithm)
        {
            using HMAC hmac = algorithm switch
            {
                InsaneHashAlgorithm.Md5 => new HMACMD5(key),
                InsaneHashAlgorithm.Sha1 => new HMACSHA1(key),
                InsaneHashAlgorithm.Sha256 => new HMACSHA256(key),
                InsaneHashAlgorithm.Sha384 => new HMACSHA384(key),
                InsaneHashAlgorithm.Sha512 => new HMACSHA512(key),
                _ => throw new NotImplementedException(algorithm.ToString())
            };

            return hmac.ComputeHash(data);
        }
    }
}
