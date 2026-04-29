using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InsaneIO.Insane.Tests
{
    [TestClass]
    public class ScryptExtensionsUnitTests
    {
        private const string Data = "payload";
        private const string Salt = "salt";
        private static readonly byte[] DataBytes = Data.ToByteArrayUtf8();
        private static readonly byte[] SaltBytes = Salt.ToByteArrayUtf8();

        [TestMethod]
        public void ComputeAndVerifyScrypt_ShouldSupportBinaryEncodedAndVerifyOverloads()
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
            "other payload".VerifyScrypt(Salt, expected, iterations, blockSize, parallelism, derivedKeyLength).Should().BeFalse();
            Data.VerifyScryptFromEncoded(Salt, encoded, HexEncoder.DefaultInstance, iterations, blockSize, parallelism, derivedKeyLength).Should().BeTrue();
            DataBytes.VerifyScryptFromEncoded(SaltBytes, encoded, HexEncoder.DefaultInstance, iterations, blockSize, parallelism, derivedKeyLength).Should().BeTrue();
            Data.VerifyScryptFromEncoded(Salt, encoded + "00", HexEncoder.DefaultInstance, iterations, blockSize, parallelism, derivedKeyLength).Should().BeFalse();
        }
    }
}
