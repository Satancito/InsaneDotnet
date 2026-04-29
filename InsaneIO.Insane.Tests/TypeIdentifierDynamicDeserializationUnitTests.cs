using FluentAssertions;
using InsaneIO.Insane.Cryptography.Enums;
using InsaneIO.Insane.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Tests
{
    [TestClass]
    public class TypeIdentifierDynamicDeserializationUnitTests
    {
        private static readonly string EncoderJson = HexEncoder.DefaultInstance.Serialize();
        private static readonly string HasherJson = new ShaHasher
        {
            HashAlgorithm = HashAlgorithm.Sha256,
            Encoder = HexEncoder.DefaultInstance
        }.Serialize();
        private static readonly string EncryptorJson = new AesCbcEncryptor
        {
            KeyString = "12345678",
            Padding = AesCbcPadding.Pkcs7,
            Encoder = Base64Encoder.DefaultInstance
        }.Serialize();

        [TestMethod]
        public void DeserializeDynamic_ShouldRejectMissingTypeIdentifier()
        {
            string json = TestJsonMutations.RemoveTypeIdentifier(EncoderJson);

            FluentActions.Invoking(() => IEncoder.DeserializeDynamic(json)).Should().Throw<DeserializeException>();
            FluentActions.Invoking(() => IHasher.DeserializeDynamic(json)).Should().Throw<DeserializeException>();
            FluentActions.Invoking(() => IEncryptor.DeserializeDynamic(json)).Should().Throw<DeserializeException>();
        }

        [TestMethod]
        public void DeserializeDynamic_ShouldRejectBlankTypeIdentifier()
        {
            string json = TestJsonMutations.ReplaceTypeIdentifier(EncoderJson, string.Empty);

            FluentActions.Invoking(() => IEncoder.DeserializeDynamic(json)).Should().Throw<DeserializeException>();
        }

        [TestMethod]
        public void DeserializeDynamic_ShouldRejectUnknownTypeIdentifier()
        {
            string json = TestJsonMutations.ReplaceTypeIdentifier(EncoderJson, "Unknown-Type-Identifier");

            FluentActions.Invoking(() => IEncoder.DeserializeDynamic(json)).Should().Throw<DeserializeException>();
            FluentActions.Invoking(() => IHasher.DeserializeDynamic(json)).Should().Throw<DeserializeException>();
            FluentActions.Invoking(() => IEncryptor.DeserializeDynamic(json)).Should().Throw<DeserializeException>();
        }

        [TestMethod]
        public void DeserializeDynamic_ShouldRejectValidTypeIdentifierUsedWithWrongContract()
        {
            FluentActions.Invoking(() => IEncoder.DeserializeDynamic(HasherJson)).Should().Throw<DeserializeException>();
            FluentActions.Invoking(() => IEncryptor.DeserializeDynamic(HasherJson)).Should().Throw<DeserializeException>();
            FluentActions.Invoking(() => IHasher.DeserializeDynamic(EncoderJson)).Should().Throw<DeserializeException>();
            FluentActions.Invoking(() => IHasher.DeserializeDynamic(EncryptorJson)).Should().Throw<DeserializeException>();
        }
    }
}
