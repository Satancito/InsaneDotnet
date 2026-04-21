using InsaneIO.Insane.Extensions;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Text.Json.Nodes;
using InsaneIO.Insane.Serialization;

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
            JsonNode jsonNode = JsonNode.Parse(json)!;
            Type encoderType = Type.GetType(jsonNode[nameof(Encoder)]![nameof(IEncoder.AssemblyName)]!.GetValue<string>())!;
            IEncoder encoder = (IEncoder)JsonSerializer.Deserialize(jsonNode[nameof(Encoder)], encoderType)!;
            string publickey = jsonNode[nameof(KeyPair)]![nameof(RsaKeyPair.PublicKey)]!.GetValue<string?>()!;
            string privatekey = jsonNode[nameof(KeyPair)]![nameof(RsaKeyPair.PrivateKey)]!.GetValue<string?>()!;
            
            return new RsaEncryptor
            {
                KeyPair = new RsaKeyPair{ PublicKey = publickey, PrivateKey = privatekey },
                Encoder = encoder,
                Padding = Enum.Parse<RsaPadding>(jsonNode[nameof(Padding)]!.GetValue<int>().ToString())
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
            return data.DecryptRsaFromEncoded(KeyPair.PrivateKey!,Encoder, Padding);
        }
    }
}
