using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Cryptography.Attributes;
using InsaneIO.Insane.Serialization;
using System.Linq;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Cryptography
{
    [CryptographyType("Insane-Cryptography-ShaHasher")]
    public class ShaHasher : IHasher
    {
        public static Type SelfType => typeof(ShaHasher);
        public string AssemblyName { get => IBaseSerializable.GetName(SelfType); }

        public HashAlgorithm HashAlgorithm { get; init; } = HashAlgorithm.Sha512;
        public IEncoder Encoder { get; init; } = Base64Encoder.DefaultInstance;

        public ShaHasher()
        {
        }

        public static IHasher Deserialize(string json)
        {
            JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(SelfType, json);
            if (!CryptographyTypeResolver.MatchesSerializedType(SelfType, jsonNode))
            {
                throw new DeserializeException(SelfType, json);
            }

            return new ShaHasher
            {
                HashAlgorithm = (HashAlgorithm)(jsonNode[nameof(HashAlgorithm)]?.GetValue<int>() ?? throw new DeserializeException(SelfType, json)),
                Encoder = IEncoder.DeserializeDynamic(jsonNode[nameof(Encoder)]?.ToJsonString() ?? throw new DeserializeException(SelfType, json))
            };
        }

        public string Serialize(bool indented = false)
        {
            return ToJsonObject().ToJsonString(IJsonSerializable.GetIndentOptions(indented));
        }

        public JsonObject ToJsonObject()
        {
            return new JsonObject()
            {
                [CryptographyTypeResolver.JsonPropertyName] = CryptographyTypeResolver.GetTypeId(SelfType),
                [nameof(AssemblyName)] = AssemblyName,
                [nameof(HashAlgorithm)] = HashAlgorithm.NumberValue<int>(),
                [nameof(Encoder)] = Encoder.ToJsonObject()
            };
        }

        public byte[] Compute(byte[] data)
        {
            return data.ComputeHash(HashAlgorithm);
        }

        public byte[] Compute(string data)
        {
            return data.ComputeHash(HashAlgorithm);
        }

        public string ComputeEncoded(byte[] data)
        {
            return data.ComputeHashEncoded(Encoder, HashAlgorithm);
        }

        public string ComputeEncoded(string data)
        {
            return data.ComputeHashEncoded(Encoder, HashAlgorithm);
        }

        public bool Verify(byte[] data, byte[] expected)
        {
            return Compute(data).SequenceEqual(expected);
        }

        public bool Verify(string data, byte[] expected)
        {
            return Compute(data).SequenceEqual(expected);
        }

        public bool VerifyEncoded(byte[] data, string expected)
        {
            return ComputeEncoded(data).SequenceEqual(expected);
        }

        public bool VerifyEncoded(string data, string expected)
        {
            return ComputeEncoded(data).SequenceEqual(expected);
        }
    }
}
