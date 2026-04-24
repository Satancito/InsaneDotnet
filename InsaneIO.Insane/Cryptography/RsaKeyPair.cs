using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Serialization;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Cryptography
{
    public class RsaKeyPair : IJsonSerializable
    {
        public static Type SelfType => typeof(RsaKeyPair);
        public string AssemblyName { get => IBaseSerializable.GetName(SelfType); }
        public string? PublicKey { get; init; }
        public string? PrivateKey { get; init; }

        public static RsaKeyPair Deserialize(string json)
        {
            JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(SelfType, json);
            string assemblyName = jsonNode[nameof(AssemblyName)]?.GetValue<string>() ?? throw new DeserializeException(SelfType, json);

            if (assemblyName != IBaseSerializable.GetName(SelfType))
            {
                throw new DeserializeException(SelfType, json);
            }

            string? publicKey = jsonNode[nameof(PublicKey)]?.GetValue<string?>();
            string? privateKey = jsonNode[nameof(PrivateKey)]?.GetValue<string?>();

            if (string.IsNullOrWhiteSpace(publicKey) && string.IsNullOrWhiteSpace(privateKey))
            {
                throw new DeserializeException(SelfType, json);
            }

            return new RsaKeyPair
            {
                PublicKey = publicKey,
                PrivateKey = privateKey
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
                [nameof(AssemblyName)] = AssemblyName,
                [nameof(PublicKey)] = PublicKey,
                [nameof(PrivateKey)] = PrivateKey
            };
        }
    }
}
