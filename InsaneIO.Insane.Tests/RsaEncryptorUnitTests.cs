using FluentAssertions;
using InsaneIO.Insane.Cryptography;
using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InsaneIO.Insane.Tests
{
    [TestClass]
    public class RsaEncryptorUnitTests
    {
        private const string Data = "Hello from RSA encryptor";

        [TestMethod]
        [DataRow(RsaKeyPairEncoding.Ber, RsaKeyEncoding.BerPublic, RsaKeyEncoding.BerPrivate)]
        [DataRow(RsaKeyPairEncoding.Pem, RsaKeyEncoding.PemPublic, RsaKeyEncoding.PemPrivate)]
        [DataRow(RsaKeyPairEncoding.Xml, RsaKeyEncoding.XmlPublic, RsaKeyEncoding.XmlPrivate)]
        public void CreateRsaKeyPair_ShouldCreateValidKeysForEveryEncoding(
            RsaKeyPairEncoding pairEncoding,
            RsaKeyEncoding expectedPublicEncoding,
            RsaKeyEncoding expectedPrivateEncoding)
        {
            RsaKeyPair keyPair = 2048u.CreateRsaKeyPair(pairEncoding);

            keyPair.PublicKey.Should().NotBeNullOrWhiteSpace();
            keyPair.PrivateKey.Should().NotBeNullOrWhiteSpace();
            keyPair.PublicKey!.GetRsaKeyEncoding().Should().Be(expectedPublicEncoding);
            keyPair.PrivateKey!.GetRsaKeyEncoding().Should().Be(expectedPrivateEncoding);
            keyPair.PublicKey.ValidateRsaPublicKey().Should().BeTrue();
            keyPair.PrivateKey.ValidateRsaPrivateKey().Should().BeTrue();
        }

        [TestMethod]
        [DataRow(RsaPadding.Pkcs1)]
        [DataRow(RsaPadding.OaepSha1)]
        [DataRow(RsaPadding.OaepSha256)]
        [DataRow(RsaPadding.OaepSha384)]
        [DataRow(RsaPadding.OaepSha512)]
        public void RsaExtensions_ShouldRoundTripWithEveryPadding(RsaPadding padding)
        {
            RsaKeyPair keyPair = 2048u.CreateRsaKeyPair();

            byte[] encrypted = Data.EncryptRsa(keyPair.PublicKey!, padding);
            string encryptedEncoded = Data.EncryptRsaEncoded(keyPair.PublicKey!, Base64Encoder.DefaultInstance, padding);

            encrypted.DecryptRsa(keyPair.PrivateKey!, padding).ToStringUtf8().Should().Be(Data);
            encryptedEncoded.DecryptRsaFromEncoded(keyPair.PrivateKey!, Base64Encoder.DefaultInstance, padding).ToStringUtf8().Should().Be(Data);
        }

        [TestMethod]
        public void RsaEncryptor_ShouldSerializeDeserializeAndRoundTrip()
        {
            var encryptor = new RsaEncryptor
            {
                KeyPair = 2048u.CreateRsaKeyPair(),
                Padding = RsaPadding.OaepSha256,
                Encoder = Base64Encoder.DefaultInstance
            };

            string encrypted = encryptor.EncryptEncoded(Data);
            IEncryptor deserialized = RsaEncryptor.Deserialize(encryptor.Serialize());
            IEncryptor deserializedDynamic = IEncryptor.DeserializeDynamic(encryptor.Serialize());

            encryptor.DecryptEncoded(encrypted).ToStringUtf8().Should().Be(Data);
            deserialized.DecryptEncoded(deserialized.EncryptEncoded(Data)).ToStringUtf8().Should().Be(Data);
            TestSerializationAssertions.AssertJsonEquals(encryptor.ToJsonObject(), deserialized.ToJsonObject());
            TestSerializationAssertions.AssertJsonEquals(encryptor.ToJsonObject(), deserializedDynamic.ToJsonObject());
        }

        [TestMethod]
        public void RsaKeyPair_ShouldSerializeDeserialize()
        {
            RsaKeyPair keyPair = 2048u.CreateRsaKeyPair(RsaKeyPairEncoding.Pem);

            RsaKeyPair deserialized = RsaKeyPair.Deserialize(keyPair.Serialize());

            TestSerializationAssertions.AssertJsonEquals(keyPair.ToJsonObject(), deserialized.ToJsonObject());
        }

        [TestMethod]
        public void RsaKeyPairDeserialize_ShouldRejectMismatchedAssemblyName()
        {
            string json = new RsaEncryptor
            {
                KeyPair = 2048u.CreateRsaKeyPair(),
                Padding = RsaPadding.OaepSha256,
                Encoder = Base64Encoder.DefaultInstance
            }.Serialize();

            FluentActions.Invoking(() => RsaKeyPair.Deserialize(json)).Should().Throw<DeserializeException>();
        }

        [TestMethod]
        public void RsaEncryptorDeserialize_ShouldRejectMismatchedAssemblyName()
        {
            string json = new AesCbcEncryptor
            {
                KeyString = "12345678",
                Encoder = HexEncoder.DefaultInstance,
                Padding = AesCbcPadding.Pkcs7
            }.Serialize();

            FluentActions.Invoking(() => RsaEncryptor.Deserialize(json)).Should().Throw<DeserializeException>();
        }
    }
}
