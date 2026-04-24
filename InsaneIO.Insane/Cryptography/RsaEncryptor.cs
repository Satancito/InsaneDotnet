using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Extensions;
using InsaneIO.Insane.Serialization;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Cryptography
{
    public class RsaEncryptor : IEncryptor
    {
        public static Type SelfType => typeof(RsaEncryptor);
        public string AssemblyName { get => IBaseSerializable.GetName(SelfType); }

        public required RsaKeyPair KeyPair { get; init; }
        public RsaPadding Padding { get; init; } = RsaPadding.OaepSha256;
        public IEncoder Encoder { get; init; } = Base64Encoder.DefaultInstance;

        public RsaEncryptor()
        {
        }

        public static IEncryptor Deserialize(string json)
        {
            JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(SelfType, json);
            string assemblyName = jsonNode[nameof(AssemblyName)]?.GetValue<string>() ?? throw new DeserializeException(SelfType, json);

            if (assemblyName != IBaseSerializable.GetName(SelfType))
            {
                throw new DeserializeException(SelfType, json);
            }

            IEncoder encoder = IEncoder.DeserializeDynamic(jsonNode[nameof(Encoder)]?.ToJsonString() ?? throw new DeserializeException(SelfType, json));
            RsaKeyPair keyPair = RsaKeyPair.Deserialize(jsonNode[nameof(KeyPair)]?.ToJsonString() ?? throw new DeserializeException(SelfType, json))
                ?? throw new DeserializeException(SelfType, json);

            return new RsaEncryptor
            {
                KeyPair = keyPair,
                Encoder = encoder,
                Padding = (RsaPadding)(jsonNode[nameof(Padding)]?.GetValue<int>() ?? throw new DeserializeException(SelfType, json))
            };
        }

        public string Serialize(bool indented = false)
        {
            return ToJsonObject().ToJsonString(IJsonSerializable.GetIndentOptions(indented));
        }

        public JsonObject ToJsonObject()
        {
            return new JsonObject
            {
                [nameof(AssemblyName)] = AssemblyName,
                [nameof(KeyPair)] = KeyPair.ToJsonObject(),
                [nameof(Padding)] = Padding.NumberValue<int>(),
                [nameof(Encoder)] = Encoder.ToJsonObject(),
            };
        }

        public byte[] Encrypt(byte[] data)
        {
            return data.EncryptRsa(KeyPair.PublicKey!, Padding);
        }

        public byte[] Encrypt(string data)
        {
            return data.EncryptRsa(KeyPair.PublicKey!, Padding);
        }

        public string EncryptEncoded(byte[] data)
        {
            return data.EncryptRsaEncoded(KeyPair.PublicKey!, Encoder, Padding);
        }

        public string EncryptEncoded(string data)
        {
            return data.EncryptRsaEncoded(KeyPair.PublicKey!, Encoder, Padding);
        }

        public byte[] Decrypt(byte[] data)
        {
            return data.DecryptRsa(KeyPair.PrivateKey!, Padding);
        }

        public byte[] DecryptEncoded(string data)
        {
            return data.DecryptRsaFromEncoded(KeyPair.PrivateKey!, Encoder, Padding);
        }
    }
}
