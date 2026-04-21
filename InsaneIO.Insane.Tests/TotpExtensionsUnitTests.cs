using FluentAssertions;
using InsaneIO.Insane.Cryptography;
using InsaneIO.Insane.Extensions;
using InsaneIO.Insane.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using InsaneHashAlgorithm = InsaneIO.Insane.Cryptography.HashAlgorithm;

namespace InsaneIO.Insane.Tests
{
    [TestClass]
    public class TotpExtensionsUnitTests
    {
        private static readonly byte[] Secret = "insaneiosecret".ToByteArrayUtf8();
        private static readonly string Base32Secret = Base32Encoder.DefaultInstance.Encode(Secret);
        private static readonly DateTimeOffset FixedTime = DateTimeOffset.FromUnixTimeMilliseconds(1676334453222);

        [TestMethod]
        public void TotpExtensions_ShouldComputeAndVerifyKnownCode()
        {
            string code = Secret.ComputeTotpCode(FixedTime, TwoFactorCodeLength.SixDigits, InsaneHashAlgorithm.Sha1);

            code.Should().Be("528272");
            code.VerifyTotpCode(Secret, FixedTime, TwoFactorCodeLength.SixDigits, InsaneHashAlgorithm.Sha1).Should().BeTrue();
            "000000".VerifyTotpCode(Secret, FixedTime, TwoFactorCodeLength.SixDigits, InsaneHashAlgorithm.Sha1).Should().BeFalse();
        }

        [TestMethod]
        public void TotpExtensions_ShouldSupportBase32EncodedSecret()
        {
            string code = Base32Secret.ComputeTotpCode();

            code.Should().HaveLength(6);
            code.VerifyTotpCode(Base32Secret).Should().BeTrue();
        }

        [TestMethod]
        public void TotpExtensions_ShouldGenerateOtpUri()
        {
            string uri = Secret.GenerateTotpUri("user@example.com", "Insane IO", InsaneHashAlgorithm.Sha256, TwoFactorCodeLength.EightDigits, 60);

            uri.Should().StartWith("otpauth://totp/user%40example.com?");
            uri.Should().Contain($"secret={Secret.EncodeToBase32(removePadding: true)}");
            uri.Should().Contain("issuer=Insane+IO");
            uri.Should().Contain("algorithm=SHA256");
            uri.Should().Contain("digits=8");
            uri.Should().Contain("period=60");
        }

        [TestMethod]
        public void ComputeTotpRemainingSeconds_ShouldReturnWindowRemainder()
        {
            DateTimeOffset now = DateTimeOffset.FromUnixTimeSeconds(61);

            now.ComputeTotpRemainingSeconds(30).Should().Be(29);
        }
    }
}
