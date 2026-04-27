using FluentAssertions;
using InsaneIO.Insane.Cryptography;
using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Tests
{
    [TestClass]
    public class HexEncoderUnitTests
    {
        private readonly byte[] testTytes = [0xff, 0x0a, 1, 0x22];
        private const string hexStringUppercase = "FF0A0122";
        private const string hexStringLowercase = "ff0a0122";
        private static readonly HexEncoder encoderToUpper = new() { ToUpper = true };
        private static readonly HexEncoder encoderToLower = new() { ToUpper = false };

        [TestMethod]
        public void Decode_ShouldSupportUppercaseHex()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(encoderToUpper.Decode(hexStringUppercase), testTytes));
        }

        [TestMethod]
        public void Decode_ShouldSupportLowercaseHex()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(encoderToLower.Decode(hexStringLowercase), testTytes));
        }

        [TestMethod]
        public void Encode_ShouldReturnUppercaseHex()
        {
            Assert.AreEqual(hexStringUppercase, encoderToUpper.Encode(testTytes));
        }

        [TestMethod]
        public void Encode_ShouldReturnLowercaseHex()
        {
            Assert.AreEqual(hexStringLowercase, encoderToLower.Encode(testTytes));
        }

        [TestMethod]
        public void SerializeDeserialize_ShouldRoundTripEncoder()
        {
            IEncoder encoder = HexEncoder.DefaultInstance;
            string json = encoder.Serialize();
            JsonObject jsonObject = encoder.ToJsonObject();
            IEncoder deserialized = HexEncoder.Deserialize(json);
            IEncoder deserializedDynamic = IEncoder.DeserializeDynamic(json);
            Assert.AreEqual(encoder.GetType().FullName, deserialized.GetType().FullName);
            Assert.IsInstanceOfType(deserialized, typeof(HexEncoder));
            TestSerializationAssertions.AssertJsonEquals(jsonObject, deserialized.ToJsonObject());
            TestSerializationAssertions.AssertJsonEquals(jsonObject, deserializedDynamic.ToJsonObject());
        }

        [TestMethod]
        public void Deserialize_ShouldRejectMismatchedSerializedType()
        {
            string json = Base32Encoder.DefaultInstance.Serialize();

            FluentActions.Invoking(() => HexEncoder.Deserialize(json)).Should().Throw<DeserializeException>();
        }

        [TestMethod]
        public void Deserialize_ShouldRejectMissingTypeIdentifier()
        {
            JsonNode jsonNode = JsonNode.Parse(HexEncoder.DefaultInstance.Serialize())!;
            jsonNode["TypeIdentifier"] = null;

            FluentActions.Invoking(() => HexEncoder.Deserialize(jsonNode.ToJsonString())).Should().Throw<DeserializeException>();
            FluentActions.Invoking(() => IEncoder.DeserializeDynamic(jsonNode.ToJsonString())).Should().Throw<DeserializeException>();
        }
    }
}
