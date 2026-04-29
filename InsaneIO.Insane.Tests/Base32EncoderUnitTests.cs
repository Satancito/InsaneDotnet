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
    public class Base32EncoderUnitTests
    {
        public static readonly byte[] TestBytes = [104, 101, 108, 108, 111, 119, 111, 114, 108, 100];

        public static readonly string TestString = "helloworld";
        public static readonly string UpperBase32Result = "NBSWY3DPO5XXE3DE";
        public static readonly string LowerBase32Result = "nbswy3dpo5xxe3de";

        public static readonly string TestString2 = "A";
        public static readonly byte[] TestBytes2 = [65];
        public static readonly string UpperBase32Result2 = "IE======";
        public static readonly string LowerBase32Result2 = "ie======";
        public static readonly string UpperBase32Result2NoPadding = "IE";
        public static readonly string LowerBase32Result2NoPadding = "ie";

        public static readonly Base32Encoder encoderToLowerWithPadding = new() { ToLower = true, RemovePadding = false };
        public static readonly Base32Encoder encoderToLowerNoPadding = new() { ToLower = true, RemovePadding = true };
        public static readonly Base32Encoder encoderToUpperWithPadding = new() { RemovePadding = false };
        public static readonly Base32Encoder encoderToUpperNoPadding = new() { RemovePadding = true };

        [TestMethod]
        public void Encode_ShouldReturnUppercaseWithPadding()
        {
            Assert.AreEqual(encoderToUpperWithPadding.Encode(TestBytes), UpperBase32Result);
        }

        [TestMethod]
        public void Encode_ShouldReturnLowercaseWithPadding()
        {
            Assert.AreEqual(encoderToLowerWithPadding.Encode(TestBytes), LowerBase32Result);
        }

        [TestMethod]
        public void Decode_ShouldSupportUppercaseWithPadding()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(encoderToUpperWithPadding.Decode(UpperBase32Result), TestBytes));
        }

        [TestMethod]
        public void Decode_ShouldSupportLowercaseWithPadding()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(encoderToUpperWithPadding.Decode(LowerBase32Result), TestBytes));
        }

        [TestMethod]
        public void EncodeSingleByte_ShouldReturnUppercaseWithPadding()
        {
            Assert.AreEqual(encoderToUpperWithPadding.Encode(TestBytes2), UpperBase32Result2);
        }

        [TestMethod]
        public void EncodeSingleByte_ShouldReturnLowercaseWithPadding()
        {
            Assert.AreEqual(encoderToLowerWithPadding.Encode(TestBytes2), LowerBase32Result2);
        }

        [TestMethod]
        public void DecodeSingleByte_ShouldSupportUppercaseWithPadding()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(encoderToUpperWithPadding.Decode(UpperBase32Result2), TestBytes2));
        }

        [TestMethod]
        public void DecodeSingleByte_ShouldSupportLowercaseWithPadding()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(encoderToLowerWithPadding.Decode(LowerBase32Result2), TestBytes2));
        }

        [TestMethod]
        public void EncodeSingleByte_ShouldReturnUppercaseWithoutPadding()
        {
            Assert.AreEqual(encoderToUpperNoPadding.Encode(TestBytes2), UpperBase32Result2NoPadding);
        }

        [TestMethod]
        public void EncodeSingleByte_ShouldReturnLowercaseWithoutPadding()
        {
            Assert.AreEqual(encoderToLowerNoPadding.Encode(TestBytes2), LowerBase32Result2NoPadding);
        }

        [TestMethod]
        public void DecodeSingleByte_ShouldSupportUppercaseWithoutPadding()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(encoderToUpperNoPadding.Decode(UpperBase32Result2NoPadding), TestBytes2));
        }

        [TestMethod]
        public void DecodeSingleByte_ShouldSupportLowercaseWithoutPadding()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(encoderToLowerNoPadding.Decode(LowerBase32Result2NoPadding), TestBytes2));
        }

        [TestMethod]
        public void SerializeDeserialize_ShouldRoundTripEncoder()
        {
            IEncoder encoder = Base32Encoder.DefaultInstance;
            string json = encoder.Serialize();
            JsonObject jsonObject = encoder.ToJsonObject();
            IEncoder deserialized = Base32Encoder.Deserialize(json);
            IEncoder deserializedDynamic = IEncoder.DeserializeDynamic(json);
            Assert.AreEqual(encoder.GetType().FullName, deserialized.GetType().FullName);
            Assert.IsInstanceOfType(deserialized, typeof(Base32Encoder));
            TestSerializationAssertions.AssertJsonEquals(jsonObject, deserialized.ToJsonObject());
            TestSerializationAssertions.AssertJsonEquals(jsonObject, deserializedDynamic.ToJsonObject());
        }

        [TestMethod]
        public void Deserialize_ShouldRejectMismatchedSerializedType()
        {
            string json = HexEncoder.DefaultInstance.Serialize();

            FluentActions.Invoking(() => Base32Encoder.Deserialize(json)).Should().Throw<DeserializeException>();
        }

        [TestMethod]
        public void Deserialize_ShouldRejectMissingTypeIdentifier()
        {
            string json = TestJsonMutations.RemoveTypeIdentifier(Base32Encoder.DefaultInstance.Serialize());

            FluentActions.Invoking(() => Base32Encoder.Deserialize(json)).Should().Throw<DeserializeException>();
            FluentActions.Invoking(() => IEncoder.DeserializeDynamic(json)).Should().Throw<DeserializeException>();
        }

        [TestMethod]
        public void Deserialize_ShouldRejectMissingRemovePadding()
        {
            string json = TestJsonMutations.RemoveProperty(Base32Encoder.DefaultInstance.Serialize(), nameof(Base32Encoder.RemovePadding));

            FluentActions.Invoking(() => Base32Encoder.Deserialize(json)).Should().Throw<DeserializeException>();
        }

        [TestMethod]
        public void Deserialize_ShouldRejectMissingToLower()
        {
            string json = TestJsonMutations.RemoveProperty(Base32Encoder.DefaultInstance.Serialize(), nameof(Base32Encoder.ToLower));

            FluentActions.Invoking(() => Base32Encoder.Deserialize(json)).Should().Throw<DeserializeException>();
        }
    }
}
