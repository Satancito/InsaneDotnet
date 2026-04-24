using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Cryptography.Attributes;
using InsaneIO.Insane.Misc;
using InsaneIO.Insane.Serialization;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Cryptography
{
    [CryptographyType("Insane-Cryptography-Base32Encoder")]
    public class Base32Encoder : IEncoder, IDefaultInstance<Base32Encoder>
    {
        public static Type SelfType => typeof(Base32Encoder);
        public string AssemblyName { get => IJsonSerializable.GetName(SelfType); }

        private static readonly Base32Encoder _DefaultInstance = new();
        public static Base32Encoder DefaultInstance => _DefaultInstance;

        public bool ToLower { get; set; } = false;
        public bool RemovePadding { get; set; } = false;

        public Base32Encoder()
        {
        }

        public byte[] Decode(string data)
        {
            return data.DecodeFromBase32();
        }

        public string Encode(byte[] data)
        {
            return data.EncodeToBase32(RemovePadding, ToLower);
        }

        public string Encode(string data)
        {
            return Encode(data.ToByteArrayUtf8());
        }

        public JsonObject ToJsonObject()
        {
            return new JsonObject()
            {
                [CryptographyTypeResolver.JsonPropertyName] = CryptographyTypeResolver.GetTypeId(SelfType),
                [nameof(AssemblyName)] = AssemblyName,
                [nameof(RemovePadding)] = RemovePadding,
                [nameof(ToLower)] = ToLower,
            };
        }

        public string Serialize(bool indented = false)
        {
            return ToJsonObject().ToJsonString(IJsonSerializable.GetIndentOptions(indented));
        }

        public static IEncoder Deserialize(string json)
        {
            JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(SelfType, json);
            if (!CryptographyTypeResolver.MatchesSerializedType(SelfType, jsonNode))
            {
                throw new DeserializeException(SelfType, json);
            }

            return new Base32Encoder
            {
                RemovePadding = jsonNode[nameof(RemovePadding)]?.GetValue<bool>() ?? throw new DeserializeException(SelfType, json),
                ToLower = jsonNode[nameof(ToLower)]?.GetValue<bool>() ?? throw new DeserializeException(SelfType, json)
            };
        }
    }
}
