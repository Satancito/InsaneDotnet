using FluentAssertions;
using InsaneIO.Insane.Security.Enums;
using InsaneIO.Insane.Security.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using InsaneHashAlgorithm = InsaneIO.Insane.Cryptography.Enums.HashAlgorithm;

namespace InsaneIO.Insane.Tests
{
    [TestClass]
    public class TotpExtensionsUnitTests
    {
        private static readonly byte[] Secret = "insaneiosecret".ToByteArrayUtf8();
        private static readonly string Base32Secret = Base32Encoder.DefaultInstance.Encode(Secret);
        private static readonly DateTimeOffset FixedTime = DateTimeOffset.FromUnixTimeMilliseconds(1676334453222);
        private static readonly DateTimeOffset RfcTime = DateTimeOffset.FromUnixTimeSeconds(59);
        private const string RfcSha1Secret = "12345678901234567890";
        private const string RfcSha256Secret = "12345678901234567890123456789012";
        private const string RfcSha512Secret = "1234567890123456789012345678901234567890123456789012345678901234";

        [TestMethod]
        public void TotpExtensions_ShouldComputeAndVerifyKnownCode()
        {
            string code = Secret.ComputeTotpCode(FixedTime, TwoFactorCodeLength.SixDigits, InsaneHashAlgorithm.Sha1);

            code.Should().Be("528272");
            code.VerifyTotpCode(Secret, FixedTime, TwoFactorCodeLength.SixDigits, InsaneHashAlgorithm.Sha1).Should().BeTrue();
            "000000".VerifyTotpCode(Secret, FixedTime, TwoFactorCodeLength.SixDigits, InsaneHashAlgorithm.Sha1).Should().BeFalse();
        }

        [TestMethod]
        public void VerifyTotpCode_ShouldOnlyCheckCurrentWindowByDefault()
        {
            string code = Secret.ComputeTotpCode(FixedTime, TwoFactorCodeLength.SixDigits, InsaneHashAlgorithm.Sha1);

            code.VerifyTotpCode(Secret, FixedTime.AddSeconds(30), TwoFactorCodeLength.SixDigits, InsaneHashAlgorithm.Sha1).Should().BeFalse();
        }

        [TestMethod]
        public void VerifyTotpCode_ShouldSupportToleranceWindows()
        {
            string code = Secret.ComputeTotpCode(FixedTime, TwoFactorCodeLength.SixDigits, InsaneHashAlgorithm.Sha1);

            code.VerifyTotpCode(Secret, FixedTime.AddSeconds(30), TotpTimeWindowTolerance.OneWindow, TwoFactorCodeLength.SixDigits, InsaneHashAlgorithm.Sha1).Should().BeTrue();
            code.VerifyTotpCode(Secret, FixedTime.AddSeconds(60), TotpTimeWindowTolerance.OneWindow, TwoFactorCodeLength.SixDigits, InsaneHashAlgorithm.Sha1).Should().BeFalse();
            code.VerifyTotpCode(Secret, FixedTime.AddSeconds(60), TotpTimeWindowTolerance.TwoWindows, TwoFactorCodeLength.SixDigits, InsaneHashAlgorithm.Sha1).Should().BeTrue();
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
        public void GenerateTotpUri_ShouldUseRfcAlgorithmNames()
        {
            Secret.GenerateTotpUri("user@example.com", "Insane IO", InsaneHashAlgorithm.Sha1).Should().Contain("algorithm=SHA1");
            Secret.GenerateTotpUri("user@example.com", "Insane IO", InsaneHashAlgorithm.Sha256).Should().Contain("algorithm=SHA256");
            Secret.GenerateTotpUri("user@example.com", "Insane IO", InsaneHashAlgorithm.Sha512).Should().Contain("algorithm=SHA512");
            Secret.GenerateTotpUri("user@example.com", "Insane IO", InsaneHashAlgorithm.Md5).Should().Contain("algorithm=SHA1");
            Secret.GenerateTotpUri("user@example.com", "Insane IO", InsaneHashAlgorithm.Sha384).Should().Contain("algorithm=SHA1");
        }

        [TestMethod]
        public void GenerateTotpUri_ShouldMatchByteAndBase32Overloads()
        {
            string fromBytes = Secret.GenerateTotpUri("user@example.com", "Insane IO");
            string fromBase32 = Base32Secret.GenerateTotpUri("user@example.com", "Insane IO");

            fromBytes.Should().Be(fromBase32);
        }

        [TestMethod]
        public void ComputeTotpRemainingSeconds_ShouldReturnWindowRemainder()
        {
            DateTimeOffset now = DateTimeOffset.FromUnixTimeSeconds(61);

            now.ComputeTotpRemainingSeconds(30).Should().Be(29);
        }

        [TestMethod]
        public void ComputeTotpRemainingSeconds_ShouldReturnFullWindowAtBoundary()
        {
            DateTimeOffset now = DateTimeOffset.FromUnixTimeSeconds(60);

            now.ComputeTotpRemainingSeconds(30).Should().Be(30);
        }

        [TestMethod]
        public void ComputeTotpCode_ShouldNormalizeMd5ToSha1()
        {
            string sha1 = Secret.ComputeTotpCode(FixedTime, TwoFactorCodeLength.SixDigits, InsaneHashAlgorithm.Sha1);
            string md5 = Secret.ComputeTotpCode(FixedTime, TwoFactorCodeLength.SixDigits, InsaneHashAlgorithm.Md5);
            string sha384 = Secret.ComputeTotpCode(FixedTime, TwoFactorCodeLength.SixDigits, InsaneHashAlgorithm.Sha384);

            md5.Should().Be(sha1);
            sha384.Should().Be(sha1);
        }

        [TestMethod]
        public void VerifyTotpCode_ShouldNormalizeMd5AndSha384ToSha1()
        {
            string sha1 = Secret.ComputeTotpCode(FixedTime, TwoFactorCodeLength.SixDigits, InsaneHashAlgorithm.Sha1);

            sha1.VerifyTotpCode(Secret, FixedTime, TwoFactorCodeLength.SixDigits, InsaneHashAlgorithm.Md5).Should().BeTrue();
            sha1.VerifyTotpCode(Secret, FixedTime, TwoFactorCodeLength.SixDigits, InsaneHashAlgorithm.Sha384).Should().BeTrue();
        }

        [TestMethod]
        public void ComputeTotpCode_ShouldMatchRfcVectorsForSupportedAlgorithms()
        {
            RfcSha1Secret.ToByteArrayUtf8().ComputeTotpCode(RfcTime, TwoFactorCodeLength.EightDigits, InsaneHashAlgorithm.Sha1).Should().Be("94287082");
            RfcSha256Secret.ToByteArrayUtf8().ComputeTotpCode(RfcTime, TwoFactorCodeLength.EightDigits, InsaneHashAlgorithm.Sha256).Should().Be("46119246");
            RfcSha512Secret.ToByteArrayUtf8().ComputeTotpCode(RfcTime, TwoFactorCodeLength.EightDigits, InsaneHashAlgorithm.Sha512).Should().Be("90693936");
        }
    }
}
