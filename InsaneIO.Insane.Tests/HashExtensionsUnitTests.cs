using FluentAssertions;
using InsaneIO.Insane.Cryptography;
using InsaneIO.Insane.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using InsaneHashAlgorithm = InsaneIO.Insane.Cryptography.HashAlgorithm;

namespace InsaneIO.Insane.Tests
{
    [TestClass]
    public class HashExtensionsUnitTests
    {
        private const string Data = "payload";
        private const string Key = "secret";
        private const string Salt = "salt";
        private static readonly byte[] DataBytes = Data.ToByteArrayUtf8();
        private static readonly byte[] KeyBytes = Key.ToByteArrayUtf8();
        private static readonly byte[] SaltBytes = Salt.ToByteArrayUtf8();

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
            string expected = Data.ComputeHashEncoded(HexEncoder.DefaultInstance, algorithm);

            Data.VerifyHashFromEncoded(expected, HexEncoder.DefaultInstance, algorithm).Should().BeTrue();
            DataBytes.VerifyHashFromEncoded(expected, HexEncoder.DefaultInstance, algorithm).Should().BeTrue();
            Data.VerifyHashFromEncoded(expected + "00", HexEncoder.DefaultInstance, algorithm).Should().BeFalse();
        }

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

        [TestMethod]
        public void ScryptExtensions_ShouldSupportBinaryEncodedAndVerifyOverloads()
        {
            const uint iterations = 16;
            const uint blockSize = 1;
            const uint parallelism = 1;
            const uint derivedKeyLength = 16;

            byte[] expected = DataBytes.ComputeScrypt(SaltBytes, iterations, blockSize, parallelism, derivedKeyLength);
            string encoded = Data.ComputeScryptEncoded(Salt, HexEncoder.DefaultInstance, iterations, blockSize, parallelism, derivedKeyLength);

            Data.ComputeScrypt(Salt, iterations, blockSize, parallelism, derivedKeyLength).Should().Equal(expected);
            Data.ComputeScrypt(SaltBytes, iterations, blockSize, parallelism, derivedKeyLength).Should().Equal(expected);
            DataBytes.ComputeScrypt(Salt, iterations, blockSize, parallelism, derivedKeyLength).Should().Equal(expected);
            Data.VerifyScrypt(Salt, expected, iterations, blockSize, parallelism, derivedKeyLength).Should().BeTrue();
            Data.VerifyScryptFromEncoded(Salt, encoded, HexEncoder.DefaultInstance, iterations, blockSize, parallelism, derivedKeyLength).Should().BeTrue();
            DataBytes.VerifyScryptFromEncoded(SaltBytes, encoded, HexEncoder.DefaultInstance, iterations, blockSize, parallelism, derivedKeyLength).Should().BeTrue();
            Data.VerifyScryptFromEncoded(Salt, encoded + "00", HexEncoder.DefaultInstance, iterations, blockSize, parallelism, derivedKeyLength).Should().BeFalse();
        }

        [TestMethod]
        [DataRow(Argon2Variant.Argon2d)]
        [DataRow(Argon2Variant.Argon2i)]
        [DataRow(Argon2Variant.Argon2id)]
        public void Argon2Extensions_ShouldSupportVariantsAndVerifyOverloads(Argon2Variant variant)
        {
            const uint iterations = 1;
            const uint memorySizeKiB = 1024;
            const uint parallelism = 1;
            const uint derivedKeyLength = 16;

            byte[] expected = DataBytes.ComputeArgon2(SaltBytes, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength);
            string encoded = Data.ComputeArgon2Encoded(Salt, Base64Encoder.DefaultInstance, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength);

            Data.ComputeArgon2(Salt, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength).Should().Equal(expected);
            Data.ComputeArgon2(SaltBytes, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength).Should().Equal(expected);
            DataBytes.ComputeArgon2(Salt, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength).Should().Equal(expected);
            Data.VerifyArgon2(Salt, expected, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength).Should().BeTrue();
            Data.VerifyArgon2FromEncoded(Salt, encoded, Base64Encoder.DefaultInstance, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength).Should().BeTrue();
            DataBytes.VerifyArgon2FromEncoded(SaltBytes, encoded, Base64Encoder.DefaultInstance, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength).Should().BeTrue();
            Data.VerifyArgon2FromEncoded(Salt, encoded + "A", Base64Encoder.DefaultInstance, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength).Should().BeFalse();
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
