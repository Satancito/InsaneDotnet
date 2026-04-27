using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsaneIO.Insane.Extensions;
using InsaneIO.Insane.Cryptography;
using System.Runtime.Versioning;
using FluentAssertions;
using System.Security.Cryptography;

namespace InsaneIO.Insane.Tests
{
    
    [TestClass]
    public class AesEncryptorUnitTests
    {
        public const string Data = "Hello World!!!";
        public const string Data256bitsBlocksSizeOk = "012345678901234567890123456789ab";
        public const string Key = "12345678";
        public static readonly AesCbcEncryptor encryptor = new AesCbcEncryptor { KeyString = Key };

        public static readonly AesCbcEncryptor encryptorNoPaddingHexEncoder = new AesCbcEncryptor { KeyString = Key, Padding = AesCbcPadding.None, Encoder = HexEncoder.DefaultInstance };
        public static readonly AesCbcEncryptor encryptorPkcs7PaddingHexEncoder = new AesCbcEncryptor { KeyString = Key, Padding = AesCbcPadding.Pkcs7, Encoder = HexEncoder.DefaultInstance };
        public static readonly AesCbcEncryptor encryptorAnsiX923PaddingHexEncoder = new AesCbcEncryptor { KeyString = Key, Padding = AesCbcPadding.AnsiX923, Encoder = HexEncoder.DefaultInstance };

        public static readonly AesCbcEncryptor encryptorNoPaddingBase32Encoder = new AesCbcEncryptor { KeyString = Key, Padding = AesCbcPadding.None, Encoder = Base32Encoder.DefaultInstance };
        public static readonly AesCbcEncryptor encryptorPkcs7PaddingBase32Encoder = new AesCbcEncryptor { KeyString = Key, Padding = AesCbcPadding.Pkcs7, Encoder = Base32Encoder.DefaultInstance };
        public static readonly AesCbcEncryptor encryptorAnsiX923PaddingBase32Encoder = new AesCbcEncryptor { KeyString = Key, Padding = AesCbcPadding.AnsiX923, Encoder = Base32Encoder.DefaultInstance };

        public static readonly AesCbcEncryptor encryptorNoPaddingBase64Encoder = new AesCbcEncryptor { KeyString = Key, Padding = AesCbcPadding.None, Encoder = Base64Encoder.DefaultInstance };
        public static readonly AesCbcEncryptor encryptorPkcs7PaddingBase64Encoder = new AesCbcEncryptor { KeyString = Key, Padding = AesCbcPadding.Pkcs7, Encoder = Base64Encoder.DefaultInstance };
        public static readonly AesCbcEncryptor encryptorAnsiX923PaddingBase64Encoder = new AesCbcEncryptor { KeyString = Key, Padding = AesCbcPadding.AnsiX923, Encoder = Base64Encoder.DefaultInstance };


        [TestMethod]
        public void EncryptDecryptAes_ShouldRoundTripWithNoPaddingAndHexEncoder()
        {
            string encrypted = encryptorNoPaddingHexEncoder.EncryptEncoded(Data256bitsBlocksSizeOk);
            encryptorNoPaddingHexEncoder.DecryptEncoded(encrypted).ToStringUtf8().Should().BeEquivalentTo(Data256bitsBlocksSizeOk);
        }

        [TestMethod]
        public void EncryptDecryptAes_ShouldThrowWithInvalidLengthForNoPaddingAndHexEncoder()
        {
            FluentActions.Invoking(() =>
            {
                string encrypted = encryptorNoPaddingHexEncoder.EncryptEncoded(Data);
                string decrypted = encryptorNoPaddingHexEncoder.DecryptEncoded(encrypted).ToStringUtf8();
            }).Should().Throw<CryptographicException>();
        }

        [TestMethod]
        public void EncryptDecryptAes_ShouldRoundTripWithPkcs7PaddingAndHexEncoder()
        {
            string encrypted = encryptorPkcs7PaddingHexEncoder.EncryptEncoded(Data);
            string decrypted = encryptorPkcs7PaddingHexEncoder.DecryptEncoded(encrypted).ToStringUtf8();
            decrypted.Should().BeEquivalentTo(Data);
        }


        [TestMethod]
        public void EncryptDecryptAes_ShouldRoundTripWithAnsiX923PaddingAndHexEncoder()
        {
            string encrypted = encryptorAnsiX923PaddingHexEncoder.EncryptEncoded(Data);
            string decrypted = encryptorAnsiX923PaddingHexEncoder.DecryptEncoded(encrypted).ToStringUtf8();
            decrypted.Should().BeEquivalentTo(Data);
        }

        [TestMethod]
        public void EncryptDecryptAes_ShouldRoundTripWithNoPaddingAndBase32Encoder()
        {
            string encrypted = encryptorNoPaddingBase32Encoder.EncryptEncoded(Data256bitsBlocksSizeOk);
            encryptorNoPaddingBase32Encoder.DecryptEncoded(encrypted).ToStringUtf8().Should().BeEquivalentTo(Data256bitsBlocksSizeOk);
        }

        [TestMethod]
        public void EncryptDecryptAes_ShouldThrowWithInvalidLengthForNoPaddingAndBase32Encoder()
        {
            FluentActions.Invoking(() =>
            {
                string encrypted = encryptorNoPaddingBase32Encoder.EncryptEncoded(Data);
                string decrypted = encryptorNoPaddingBase32Encoder.DecryptEncoded(encrypted).ToStringUtf8();
            }).Should().Throw<CryptographicException>();
        }

        [TestMethod]
        public void EncryptDecryptAes_ShouldRoundTripWithPkcs7PaddingAndBase32Encoder()
        {
            string encrypted = encryptorPkcs7PaddingBase32Encoder.EncryptEncoded(Data);
            string decrypted = encryptorPkcs7PaddingBase32Encoder.DecryptEncoded(encrypted).ToStringUtf8();
            decrypted.Should().BeEquivalentTo(Data);
        }


        [TestMethod]
        public void EncryptDecryptAes_ShouldRoundTripWithAnsiX923PaddingAndBase32Encoder()
        {
            string encrypted = encryptorAnsiX923PaddingBase32Encoder.EncryptEncoded(Data);
            string decrypted = encryptorAnsiX923PaddingBase32Encoder.DecryptEncoded(encrypted).ToStringUtf8();
            decrypted.Should().BeEquivalentTo(Data);
        }

        [TestMethod]
        public void EncryptDecryptAes_ShouldRoundTripWithNoPaddingAndBase64Encoder()
        {
            string encrypted = encryptorNoPaddingBase64Encoder.EncryptEncoded(Data256bitsBlocksSizeOk);
            encryptorNoPaddingBase64Encoder.DecryptEncoded(encrypted).ToStringUtf8().Should().BeEquivalentTo(Data256bitsBlocksSizeOk);
        }

        [TestMethod]
        public void EncryptDecryptAes_ShouldThrowWithInvalidLengthForNoPaddingAndBase64Encoder()
        {
            FluentActions.Invoking(() =>
            {
                string encrypted = encryptorNoPaddingBase64Encoder.EncryptEncoded(Data);
                string decrypted = encryptorNoPaddingBase64Encoder.DecryptEncoded(encrypted).ToStringUtf8();
            }).Should().Throw<CryptographicException>();
        }

        [TestMethod]
        public void EncryptDecryptAes_ShouldRoundTripWithPkcs7PaddingAndBase64Encoder()
        {
            string encrypted = encryptorPkcs7PaddingBase64Encoder.EncryptEncoded(Data);
            string decrypted = encryptorPkcs7PaddingBase64Encoder.DecryptEncoded(encrypted).ToStringUtf8();
            decrypted.Should().BeEquivalentTo(Data);
        }


        [TestMethod]
        public void EncryptDecryptAes_ShouldRoundTripWithAnsiX923PaddingAndBase64Encoder()
        {
            string encrypted = encryptorAnsiX923PaddingBase64Encoder.EncryptEncoded(Data);
            string decrypted = encryptorAnsiX923PaddingBase64Encoder.DecryptEncoded(encrypted).ToStringUtf8();
            decrypted.Should().BeEquivalentTo(Data);
        }


        //[TestMethod]
        //public void TestEncryptDecryptAesWithHex()
        //{
        //    TestEncryptDecryptAes(Data, Key, HexEncoder.DefaultInstance);
        //}

        //[TestMethod]
        //public void TestEncryptDecryptAesWithEmptyData()
        //{
        //    TestEncryptDecryptAes(string.Empty, Key, Base64Encoder.DefaultInstance);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentException))]
        //public void TestEncryptDecryptAesWithEmptyKey()
        //{
        //    TestEncryptDecryptAes(Data, string.Empty, Base64Encoder.DefaultInstance);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void TestEncryptDecryptAesWithNullKey()
        //{
        //    TestEncryptDecryptAes(Data, null!, Base64Encoder.DefaultInstance);
        //}

        //[TestMethod]
        //public void TestEncryptDecryptAesWithNullData()
        //{
        //    Assert.ThrowsException<ArgumentNullException>(() =>
        //    {
        //        TestEncryptDecryptAes(null!, Key, Base64Encoder.DefaultInstance);
        //    });
        //}

    }
}
