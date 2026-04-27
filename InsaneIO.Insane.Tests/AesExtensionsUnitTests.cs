using FluentAssertions;
using InsaneIO.Insane.Cryptography;
using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Tests
{
    [TestClass]
    public class AesExtensionsUnitTests
    {
        private const string Data = "Hello from AES extensions";
        private const string Key = "12345678";
        private static readonly byte[] DataBytes = Data.ToByteArrayUtf8();
        private static readonly byte[] KeyBytes = Key.ToByteArrayUtf8();

        [TestMethod]
        [DataRow(AesCbcPadding.Pkcs7)]
        [DataRow(AesCbcPadding.AnsiX923)]
        [DataRow(AesCbcPadding.Zeros)]
        public void EncryptDecryptAesCbc_ShouldRoundTripBytes(AesCbcPadding padding)
        {
            byte[] encrypted = DataBytes.EncryptAesCbc(KeyBytes, padding);

            encrypted.Length.Should().BeGreaterThan(AesExtensions.MaxIvLength);
            encrypted.TakeLast(AesExtensions.MaxIvLength).Should().HaveCount(AesExtensions.MaxIvLength);
            encrypted.DecryptAesCbc(KeyBytes, padding).ToStringUtf8().TrimEnd('\0').Should().Be(Data);
            encrypted.DecryptAesCbc(Key, padding).ToStringUtf8().TrimEnd('\0').Should().Be(Data);
        }

        [TestMethod]
        public void EncryptDecryptAesCbc_ShouldSupportStringAndByteOverloads()
        {
            Data.EncryptAesCbc(Key).DecryptAesCbc(Key).ToStringUtf8().Should().Be(Data);
            Data.EncryptAesCbc(KeyBytes).DecryptAesCbc(KeyBytes).ToStringUtf8().Should().Be(Data);
            DataBytes.EncryptAesCbc(Key).DecryptAesCbc(Key).ToStringUtf8().Should().Be(Data);
        }

        [TestMethod]
        public void EncryptDecryptAesCbcEncoded_ShouldSupportAllOverloads()
        {
            string encryptedFromBytes = DataBytes.EncryptAesCbcEncoded(KeyBytes, Base64Encoder.DefaultInstance);
            string encryptedFromStrings = Data.EncryptAesCbcEncoded(Key, Base64Encoder.DefaultInstance);
            string encryptedStringDataByteKey = Data.EncryptAesCbcEncoded(KeyBytes, HexEncoder.DefaultInstance);
            string encryptedByteDataStringKey = DataBytes.EncryptAesCbcEncoded(Key, HexEncoder.DefaultInstance);

            encryptedFromBytes.DecryptAesCbcFromEncoded(KeyBytes, Base64Encoder.DefaultInstance).ToStringUtf8().Should().Be(Data);
            encryptedFromStrings.DecryptAesCbcFromEncoded(Key, Base64Encoder.DefaultInstance).ToStringUtf8().Should().Be(Data);
            encryptedStringDataByteKey.DecryptAesCbcFromEncoded(KeyBytes, HexEncoder.DefaultInstance).ToStringUtf8().Should().Be(Data);
            encryptedByteDataStringKey.DecryptAesCbcFromEncoded(Key, HexEncoder.DefaultInstance).ToStringUtf8().Should().Be(Data);
        }

        [TestMethod]
        public void EncryptAesCbc_ShouldValidateKeys()
        {
            FluentActions.Invoking(() => Data.EncryptAesCbc("short")).Should().Throw<ArgumentException>();
            FluentActions.Invoking(() => DataBytes.EncryptAesCbc((byte[])null!)).Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void DecryptAesCbc_ShouldRejectCiphertextShorterThanIv()
        {
            byte[] invalidCiphertext = new byte[AesExtensions.MaxIvLength - 1];

            FluentActions.Invoking(() => invalidCiphertext.DecryptAesCbc(KeyBytes))
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("*IV*");
        }

        [TestMethod]
        public void AesCbcEncryptor_ShouldSerializeDeserializeAndRoundTrip()
        {
            var encryptor = new AesCbcEncryptor
            {
                KeyString = Key,
                Encoder = HexEncoder.DefaultInstance,
                Padding = AesCbcPadding.Pkcs7
            };

            string encrypted = encryptor.EncryptEncoded(Data);
            IEncryptor deserialized = AesCbcEncryptor.Deserialize(encryptor.Serialize());
            IEncryptor deserializedDynamic = IEncryptor.DeserializeDynamic(encryptor.Serialize());

            encryptor.DecryptEncoded(encrypted).ToStringUtf8().Should().Be(Data);
            deserialized.DecryptEncoded(deserialized.EncryptEncoded(Data)).ToStringUtf8().Should().Be(Data);
            TestSerializationAssertions.AssertJsonEquals(encryptor.ToJsonObject(), deserialized.ToJsonObject());
            TestSerializationAssertions.AssertJsonEquals(encryptor.ToJsonObject(), deserializedDynamic.ToJsonObject());
        }

        [TestMethod]
        public void AesCbcEncryptorDeserialize_ShouldRejectMismatchedSerializedType()
        {
            string json = new RsaEncryptor
            {
                KeyPair = 2048u.CreateRsaKeyPair(),
                Padding = RsaPadding.OaepSha256,
                Encoder = Base64Encoder.DefaultInstance
            }.Serialize();

            FluentActions.Invoking(() => AesCbcEncryptor.Deserialize(json)).Should().Throw<DeserializeException>();
        }

        [TestMethod]
        public void AesCbcEncryptorDeserialize_ShouldRejectMissingTypeIdentifier()
        {
            var encryptor = new AesCbcEncryptor
            {
                KeyString = Key,
                Encoder = HexEncoder.DefaultInstance,
                Padding = AesCbcPadding.Pkcs7
            };

            JsonNode jsonNode = JsonNode.Parse(encryptor.Serialize())!;
            jsonNode["TypeIdentifier"] = null;

            FluentActions.Invoking(() => AesCbcEncryptor.Deserialize(jsonNode.ToJsonString())).Should().Throw<DeserializeException>();
            FluentActions.Invoking(() => IEncryptor.DeserializeDynamic(jsonNode.ToJsonString())).Should().Throw<DeserializeException>();
        }
    }
}
