using InsaneIO.Insane.Extensions;
using InsaneIO.Insane.Serialization;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Cryptography
{


    
    public class AesCbcEncryptor : IEncryptor
    {
        public static Type SelfType => typeof(AesCbcEncryptor);
        public string AssemblyName { get => IBaseSerializable.GetName(SelfType); }

        public string KeyString { get => Encoder.Encode(Key); init => Key = value.ToByteArrayUtf8(); }
        public byte[] KeyBytes { get => Key; init => Key = value; }

        private byte[] Key = RandomExtensions.NextBytes(AesExtensions.MaxKeyLength);

        public IEncoder Encoder { get; init; } = Base64Encoder.DefaultInstance;

        public AesCbcPadding Padding { get; init; } = AesCbcPadding.Pkcs7;

        public AesCbcEncryptor()
        {
        } 

        public static IEncryptor Deserialize(string json)
        {
            JsonNode jsonNode = JsonNode.Parse(json)!;
            Type encoderType = Type.GetType(jsonNode[nameof(Encoder)]![nameof(IEncoder.AssemblyName)]!.GetValue<string>())!;
            IEncoder encoder = (IEncoder)JsonSerializer.Deserialize(jsonNode[nameof(Encoder)], encoderType)!;
            return new AesCbcEncryptor {
                Key = encoder.Decode(jsonNode[nameof(Key)]!.GetValue<string>()),
                Encoder = encoder,
                Padding = Enum.Parse<AesCbcPadding>(jsonNode[nameof(Padding)]!.GetValue<int>().ToString())
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
                [nameof(Key)] = Encoder.Encode(Key),
                [nameof(Padding)] = Padding.NumberValue<int>(),
                [nameof(Encoder)] = Encoder.ToJsonObject(),
            };
        }

        public byte[] Encrypt(byte[] data)
        {
            return data.EncryptAesCbc(Key, Padding);
        }

        public byte[] Encrypt(string data)
        {
            return data.EncryptAesCbc(Key, Padding);
        }

        public string EncryptEncoded(byte[] data)
        {
            return data.EncryptAesCbcEncoded(Key, Encoder, Padding);
        }

        public string EncryptEncoded(string data)
        {
            return data.EncryptAesCbcEncoded(Key, Encoder, Padding);
        }

        public byte[] Decrypt(byte[] data)
        {
            return data.DecryptAesCbc(Key, Padding);
        }

        public byte[] DecryptEncoded(string data)
        {
            return data.DecryptAesCbcFromEncoded(Key, Encoder, Padding);
        }
    }
}
