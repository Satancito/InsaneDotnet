using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InsaneIO.Insane.Tests
{
    [TestClass]
    public class Argon2ExtensionsUnitTests
    {
        private const string Data = "payload";
        private const string Salt = "salt";
        private static readonly byte[] DataBytes = Data.ToByteArrayUtf8();
        private static readonly byte[] SaltBytes = Salt.ToByteArrayUtf8();

        [TestMethod]
        [DataRow(Argon2Variant.Argon2d)]
        [DataRow(Argon2Variant.Argon2i)]
        [DataRow(Argon2Variant.Argon2id)]
        public void ComputeAndVerifyArgon2_ShouldSupportVariantsAndVerifyOverloads(Argon2Variant variant)
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
    }
}
