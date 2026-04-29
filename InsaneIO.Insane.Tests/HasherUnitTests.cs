using FluentAssertions;
using InsaneIO.Insane.Cryptography.Enums;
using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json.Nodes;
using InsaneHashAlgorithm = InsaneIO.Insane.Cryptography.Enums.HashAlgorithm;

namespace InsaneIO.Insane.Tests
{
    [TestClass]
    public class HasherUnitTests
    {
        private const string Data = "payload";
        private const string OtherData = "other payload";
        private const string Key = "secret";
        private const string Salt = "salt";

        [TestMethod]
        public void ShaHasher_ShouldComputeVerifyAndSerialize()
        {
            var hasher = new ShaHasher
            {
                HashAlgorithm = InsaneHashAlgorithm.Sha256,
                Encoder = HexEncoder.DefaultInstance
            };

            byte[] hash = hasher.Compute(Data);
            string encoded = hasher.ComputeEncoded(Data);
            IHasher deserialized = ShaHasher.Deserialize(hasher.Serialize());

            hash.Should().Equal(Data.ComputeHash(InsaneHashAlgorithm.Sha256));
            encoded.Should().Be(Data.ComputeHashEncoded(HexEncoder.DefaultInstance, InsaneHashAlgorithm.Sha256));
            hasher.Verify(Data, hash).Should().BeTrue();
            hasher.Verify(OtherData, hash).Should().BeFalse();
            hasher.VerifyEncoded(Data, encoded).Should().BeTrue();
            hasher.VerifyEncoded(OtherData, encoded).Should().BeFalse();
            TestSerializationAssertions.AssertJsonEquals(hasher.ToJsonObject(), deserialized.ToJsonObject());
            TestSerializationAssertions.AssertJsonEquals(hasher.ToJsonObject(), IHasher.DeserializeDynamic(hasher.Serialize()).ToJsonObject());
        }

        [TestMethod]
        public void HmacHasher_ShouldComputeVerifyAndSerialize()
        {
            var hasher = new HmacHasher
            {
                KeyString = Key,
                HashAlgorithm = InsaneHashAlgorithm.Sha384,
                Encoder = Base64Encoder.DefaultInstance
            };

            byte[] hash = hasher.Compute(Data);
            string encoded = hasher.ComputeEncoded(Data);
            IHasher deserialized = HmacHasher.Deserialize(hasher.Serialize());

            hash.Should().Equal(Data.ComputeHmac(Key, InsaneHashAlgorithm.Sha384));
            encoded.Should().Be(Data.ComputeHmacEncoded(Key, Base64Encoder.DefaultInstance, InsaneHashAlgorithm.Sha384));
            hasher.Verify(Data, hash).Should().BeTrue();
            hasher.Verify(OtherData, hash).Should().BeFalse();
            hasher.VerifyEncoded(Data, encoded).Should().BeTrue();
            hasher.VerifyEncoded(OtherData, encoded).Should().BeFalse();
            TestSerializationAssertions.AssertJsonEquals(hasher.ToJsonObject(), deserialized.ToJsonObject());
            TestSerializationAssertions.AssertJsonEquals(hasher.ToJsonObject(), IHasher.DeserializeDynamic(hasher.Serialize()).ToJsonObject());
        }

        [TestMethod]
        public void ScryptHasher_ShouldComputeVerifyAndSerialize()
        {
            var hasher = new ScryptHasher
            {
                SaltString = Salt,
                Iterations = 16,
                BlockSize = 1,
                Parallelism = 1,
                DerivedKeyLength = 16,
                Encoder = HexEncoder.DefaultInstance
            };

            byte[] hash = hasher.Compute(Data);
            string encoded = hasher.ComputeEncoded(Data);
            IHasher deserialized = ScryptHasher.Deserialize(hasher.Serialize());

            hash.Should().Equal(Data.ComputeScrypt(Salt, 16, 1, 1, 16));
            encoded.Should().Be(Data.ComputeScryptEncoded(Salt, HexEncoder.DefaultInstance, 16, 1, 1, 16));
            hasher.Verify(Data, hash).Should().BeTrue();
            hasher.Verify(OtherData, hash).Should().BeFalse();
            hasher.VerifyEncoded(Data, encoded).Should().BeTrue();
            hasher.VerifyEncoded(OtherData, encoded).Should().BeFalse();
            TestSerializationAssertions.AssertJsonEquals(hasher.ToJsonObject(), deserialized.ToJsonObject());
            TestSerializationAssertions.AssertJsonEquals(hasher.ToJsonObject(), IHasher.DeserializeDynamic(hasher.Serialize()).ToJsonObject());
        }

        [TestMethod]
        public void Argon2Hasher_ShouldComputeVerifyAndSerialize()
        {
            var hasher = new Argon2Hasher
            {
                SaltString = Salt,
                Iterations = 1,
                MemorySizeKiB = 1024,
                DegreeOfParallelism = 1,
                DerivedKeyLength = 16,
                Argon2Variant = Argon2Variant.Argon2id,
                Encoder = Base64Encoder.DefaultInstance
            };

            byte[] hash = hasher.Compute(Data);
            string encoded = hasher.ComputeEncoded(Data);
            IHasher deserialized = Argon2Hasher.Deserialize(hasher.Serialize());

            hash.Should().Equal(Data.ComputeArgon2(Salt, 1, 1024, 1, Argon2Variant.Argon2id, 16));
            encoded.Should().Be(Data.ComputeArgon2Encoded(Salt, Base64Encoder.DefaultInstance, 1, 1024, 1, Argon2Variant.Argon2id, 16));
            hasher.Verify(Data, hash).Should().BeTrue();
            hasher.Verify(OtherData, hash).Should().BeFalse();
            hasher.VerifyEncoded(Data, encoded).Should().BeTrue();
            hasher.VerifyEncoded(OtherData, encoded).Should().BeFalse();
            TestSerializationAssertions.AssertJsonEquals(hasher.ToJsonObject(), deserialized.ToJsonObject());
            TestSerializationAssertions.AssertJsonEquals(hasher.ToJsonObject(), IHasher.DeserializeDynamic(hasher.Serialize()).ToJsonObject());
        }

        [TestMethod]
        public void ConcreteHasherDeserialize_ShouldRejectMismatchedSerializedType()
        {
            string shaJson = new ShaHasher
            {
                HashAlgorithm = InsaneHashAlgorithm.Sha256,
                Encoder = HexEncoder.DefaultInstance
            }.Serialize();

            string hmacJson = new HmacHasher
            {
                KeyString = Key,
                HashAlgorithm = InsaneHashAlgorithm.Sha256,
                Encoder = Base64Encoder.DefaultInstance
            }.Serialize();

            string scryptJson = new ScryptHasher
            {
                SaltString = Salt,
                Iterations = 16,
                BlockSize = 1,
                Parallelism = 1,
                DerivedKeyLength = 16,
                Encoder = HexEncoder.DefaultInstance
            }.Serialize();

            string argon2Json = new Argon2Hasher
            {
                SaltString = Salt,
                Iterations = 1,
                MemorySizeKiB = 1024,
                DegreeOfParallelism = 1,
                DerivedKeyLength = 16,
                Argon2Variant = Argon2Variant.Argon2id,
                Encoder = Base64Encoder.DefaultInstance
            }.Serialize();

            FluentActions.Invoking(() => ShaHasher.Deserialize(hmacJson)).Should().Throw<DeserializeException>();
            FluentActions.Invoking(() => HmacHasher.Deserialize(shaJson)).Should().Throw<DeserializeException>();
            FluentActions.Invoking(() => ScryptHasher.Deserialize(argon2Json)).Should().Throw<DeserializeException>();
            FluentActions.Invoking(() => Argon2Hasher.Deserialize(scryptJson)).Should().Throw<DeserializeException>();
        }

        [TestMethod]
        public void Deserialize_ShouldRejectMissingTypeIdentifierForHashers()
        {
            var hasher = new HmacHasher
            {
                KeyString = Key,
                HashAlgorithm = InsaneHashAlgorithm.Sha256,
                Encoder = Base64Encoder.DefaultInstance
            };

            JsonNode jsonNode = JsonNode.Parse(hasher.Serialize())!;
            jsonNode["TypeIdentifier"] = null;

            FluentActions.Invoking(() => HmacHasher.Deserialize(jsonNode.ToJsonString())).Should().Throw<DeserializeException>();
            FluentActions.Invoking(() => IHasher.DeserializeDynamic(jsonNode.ToJsonString())).Should().Throw<DeserializeException>();
        }

        [TestMethod]
        public void ShaHasherDeserialize_ShouldRejectMissingHashAlgorithm()
        {
            var hasher = new ShaHasher { HashAlgorithm = InsaneHashAlgorithm.Sha256, Encoder = HexEncoder.DefaultInstance };
            string json = TestJsonMutations.RemoveProperty(hasher.Serialize(), nameof(ShaHasher.HashAlgorithm));

            FluentActions.Invoking(() => ShaHasher.Deserialize(json)).Should().Throw<DeserializeException>();
        }

        [TestMethod]
        public void ShaHasherDeserialize_ShouldRejectInvalidHashAlgorithm()
        {
            var hasher = new ShaHasher { HashAlgorithm = InsaneHashAlgorithm.Sha256, Encoder = HexEncoder.DefaultInstance };
            string json = TestJsonMutations.ReplaceProperty(hasher.Serialize(), nameof(ShaHasher.HashAlgorithm), JsonValue.Create(999));

            FluentActions.Invoking(() => ShaHasher.Deserialize(json)).Should().Throw<DeserializeException>();
        }

        [TestMethod]
        public void HmacHasherDeserialize_ShouldRejectMissingKey()
        {
            var hasher = new HmacHasher { KeyString = Key, HashAlgorithm = InsaneHashAlgorithm.Sha256, Encoder = Base64Encoder.DefaultInstance };
            string json = TestJsonMutations.RemoveProperty(hasher.Serialize(), "Key");

            FluentActions.Invoking(() => HmacHasher.Deserialize(json)).Should().Throw<DeserializeException>();
        }

        [TestMethod]
        public void HmacHasherDeserialize_ShouldRejectInvalidEncoder()
        {
            var hasher = new HmacHasher { KeyString = Key, HashAlgorithm = InsaneHashAlgorithm.Sha256, Encoder = Base64Encoder.DefaultInstance };
            string json = TestJsonMutations.ReplaceProperty(hasher.Serialize(), nameof(HmacHasher.Encoder), JsonValue.Create("invalid"));

            FluentActions.Invoking(() => HmacHasher.Deserialize(json)).Should().Throw<DeserializeException>();
        }

        [TestMethod]
        public void ScryptHasherDeserialize_ShouldRejectMissingSalt()
        {
            var hasher = new ScryptHasher { SaltString = Salt, Iterations = 16, BlockSize = 1, Parallelism = 1, DerivedKeyLength = 16, Encoder = HexEncoder.DefaultInstance };
            string json = TestJsonMutations.RemoveProperty(hasher.Serialize(), "Salt");

            FluentActions.Invoking(() => ScryptHasher.Deserialize(json)).Should().Throw<DeserializeException>();
        }

        [TestMethod]
        public void ScryptHasherDeserialize_ShouldRejectInvalidIterations()
        {
            var hasher = new ScryptHasher { SaltString = Salt, Iterations = 16, BlockSize = 1, Parallelism = 1, DerivedKeyLength = 16, Encoder = HexEncoder.DefaultInstance };
            string json = TestJsonMutations.ReplaceProperty(hasher.Serialize(), nameof(ScryptHasher.Iterations), JsonValue.Create("invalid"));

            FluentActions.Invoking(() => ScryptHasher.Deserialize(json)).Should().Throw<DeserializeException>();
        }

        [TestMethod]
        public void Argon2HasherDeserialize_ShouldRejectMissingVariant()
        {
            var hasher = new Argon2Hasher { SaltString = Salt, Iterations = 1, MemorySizeKiB = 1024, DegreeOfParallelism = 1, DerivedKeyLength = 16, Argon2Variant = Argon2Variant.Argon2id, Encoder = Base64Encoder.DefaultInstance };
            string json = TestJsonMutations.RemoveProperty(hasher.Serialize(), nameof(Argon2Hasher.Argon2Variant));

            FluentActions.Invoking(() => Argon2Hasher.Deserialize(json)).Should().Throw<DeserializeException>();
        }

        [TestMethod]
        public void Argon2HasherDeserialize_ShouldRejectInvalidVariant()
        {
            var hasher = new Argon2Hasher { SaltString = Salt, Iterations = 1, MemorySizeKiB = 1024, DegreeOfParallelism = 1, DerivedKeyLength = 16, Argon2Variant = Argon2Variant.Argon2id, Encoder = Base64Encoder.DefaultInstance };
            string json = TestJsonMutations.ReplaceProperty(hasher.Serialize(), nameof(Argon2Hasher.Argon2Variant), JsonValue.Create(999));

            FluentActions.Invoking(() => Argon2Hasher.Deserialize(json)).Should().Throw<DeserializeException>();
        }
    }
}
