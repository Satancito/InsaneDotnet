using FluentAssertions;
using InsaneIO.Insane.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Linq;
using InsaneIO.Insane.Cryptography.Enums;

namespace InsaneIO.Insane.Tests
{
    [TestClass]
    public class EncodingExtensionsUnitTests
    {
        [TestMethod]
        public void Utf8Helpers_ShouldRoundTripUnicodeText()
        {
            const string value = "InsaneIO - prueba ñ";

            value.ToByteArrayUtf8().ToStringUtf8().Should().Be(value);
        }

        [TestMethod]
        public void EncodingHelpers_ShouldUseProvidedEncoding()
        {
            const string value = "abc";

            byte[] ascii = value.ToByteArray(Encoding.ASCII);

            ascii.Should().Equal(97, 98, 99);
            ascii.ToString(Encoding.ASCII).Should().Be(value);
        }

        [TestMethod]
        public void ByteArrayGuards_ShouldThrowForInvalidValues()
        {
            byte[]? nullBytes = null;
            byte[] emptyBytes = Array.Empty<byte>();

            FluentActions.Invoking(() => nullBytes!.ThrowIfNull()).Should().Throw<ArgumentException>();
            FluentActions.Invoking(() => nullBytes!.ThrowIfNullOrEmpty()).Should().Throw<ArgumentException>();
            FluentActions.Invoking(() => emptyBytes.ThrowIfEmpty()).Should().Throw<ArgumentException>();
            FluentActions.Invoking(() => emptyBytes.ThrowIfNullOrEmpty()).Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void HexEncodingExtensions_ShouldEncodeAndDecodeStringsAndBytes()
        {
            byte[] bytes = { 0x00, 0x0a, 0xff };

            bytes.EncodeToHex().Should().Be("000aff");
            bytes.EncodeToHex(toUpper: true).Should().Be("000AFF");
            "Hi".EncodeToHex().Should().Be("4869");
            "000AFF".DecodeFromHex().Should().Equal(bytes);
        }

        [TestMethod]
        public void HexEncodingExtensions_ShouldRejectOddLengthInput()
        {
            FluentActions.Invoking(() => "ABC".DecodeFromHex())
                .Should()
                .Throw<ArgumentException>();
        }

        [TestMethod]
        public void HexEncodingExtensions_ShouldRejectInvalidCharacters()
        {
            FluentActions.Invoking(() => "GG".DecodeFromHex())
                .Should()
                .Throw<ArgumentException>();
        }

        [TestMethod]
        public void Base32EncodingExtensions_ShouldRespectPaddingAndCase()
        {
            byte[] bytes = "A".ToByteArrayUtf8();

            bytes.EncodeToBase32(removePadding: false, toLower: false).Should().Be("IE======");
            bytes.EncodeToBase32(removePadding: true, toLower: true).Should().Be("ie");
            "ie".DecodeFromBase32().Should().Equal(bytes);
            "IE======".DecodeFromBase32().Should().Equal(bytes);
            string.Empty.DecodeFromBase32().Should().BeEmpty();
        }

        [TestMethod]
        [DataRow("A")]
        [DataRow("ABC")]
        [DataRow("ABCDEF")]
        [DataRow("IE=")]
        [DataRow("IE=====")]
        [DataRow("I=E=====")]
        [DataRow("IE======A")]
        public void Base32EncodingExtensions_ShouldRejectInvalidLengthOrPadding(string value)
        {
            FluentActions.Invoking(() => value.DecodeFromBase32())
                .Should()
                .Throw<ArgumentException>();
        }

        [TestMethod]
        public void Base32EncodingExtensions_ShouldRejectInvalidCharacters()
        {
            FluentActions.Invoking(() => "IE$=====".DecodeFromBase32())
                .Should()
                .Throw<ArgumentException>();
        }

        [TestMethod]
        public void Base64EncodingExtensions_ShouldSupportBase64Variants()
        {
            byte[] bytes = { 0xfb, 0xff, 0xee };

            string base64 = bytes.EncodeToBase64();

            base64.Should().Be("+//u");
            bytes.EncodeToUrlSafeBase64().Should().Be("-__u");
            bytes.EncodeToFilenameSafeBase64().Should().Be("-__u");
            bytes.EncodeToUrlEncodedBase64().Should().Be("%2B%2F%2Fu");
            "-__u".DecodeFromBase64().Should().Equal(bytes);
            "%2B%2F%2Fu".DecodeFromBase64().Should().Equal(bytes);
            base64.EncodeBase64ToUrlSafeBase64().Should().Be("-__u");
            base64.EncodeBase64ToFilenameSafeBase64().Should().Be("-__u");
            base64.EncodeBase64ToUrlEncodedBase64().Should().Be("%2B%2F%2Fu");
        }

        [TestMethod]
        public void Base64EncodingExtensions_ShouldSupportMimeAndPemLineBreaks()
        {
            byte[] bytes = Enumerable.Range(0, 90).Select(value => (byte)value).ToArray();

            string mime = bytes.EncodeToBase64(Base64EncodingExtensions.MimeLineBreaksLength);
            string pem = bytes.EncodeToBase64(Base64EncodingExtensions.PemLineBreaksLength);

            mime.Should().Contain(Environment.NewLine);
            pem.Should().Contain(Environment.NewLine);
            mime.DecodeFromBase64().Should().Equal(bytes);
            pem.DecodeFromBase64().Should().Equal(bytes);
        }

        [TestMethod]
        public void InsertLineBreaks_ShouldUseEnvironmentNewLine()
        {
            "abcdefgh".InsertLineBreaks(4).Should().Be($"abcd{Environment.NewLine}efgh");
            "abc".InsertLineBreaks(0).Should().Be("abc");
        }

        [TestMethod]
        public void EnumExtensions_ShouldConvertNumericValues()
        {
            AesCbcPadding.Pkcs7.IntValue().Should().Be((int)AesCbcPadding.Pkcs7);
            ((int)AesCbcPadding.AnsiX923).ParseEnum<AesCbcPadding, int>().Should().Be(AesCbcPadding.AnsiX923);
            ((uint)Base64Encoding.UrlSafeBase64).ParseEnum<Base64Encoding, uint>().Should().Be(Base64Encoding.UrlSafeBase64);
        }
    }
}
