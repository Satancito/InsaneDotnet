using InsaneIO.Insane.Serialization;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Cryptography
{

    public class HmacHasher : IHasher
    {
        public static Type SelfType => typeof(HmacHasher);
        public string AssemblyName { get => IJsonSerializable.GetName(SelfType); }

        public HashAlgorithm HashAlgorithm { get; init; } = HashAlgorithm.Sha512;
        public IEncoder Encoder { get; init; } = Base64Encoder.DefaultInstance;

        public string KeyString { get => Encoder.Encode(Key); init => Key = value.ToByteArrayUtf8(); }

        public byte[] KeyBytes { get => Key; init => Key = value; }

        private byte[] Key = RandomExtensions.NextBytes(Constants.HmacKeySize);

        public HmacHasher()
        {
        }

        public static IHasher Deserialize(string json)
        {
            JsonNode jsonNode = JsonNode.Parse(json)!;
            Type encoderType = Type.GetType(jsonNode[nameof(Encoder)]![nameof(IEncoder.AssemblyName)]!.GetValue<string>())!;
            IEncoder encoder = (IEncoder)JsonSerializer.Deserialize(jsonNode[nameof(Encoder)], encoderType)!;
            return new HmacHasher
            {
                HashAlgorithm = Enum.Parse<HashAlgorithm>(jsonNode[nameof(HashAlgorithm)]!.GetValue<int>().ToString()),
                Encoder = encoder,
                Key = encoder.Decode(jsonNode[nameof(Key)]!.GetValue<string>())
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
                [nameof(Key)] = Encoder.Encode(Key),
                [nameof(HashAlgorithm)] = HashAlgorithm.NumberValue<int>(),
                [nameof(Encoder)] = Encoder.ToJsonObject(),
            };
        }

        public byte[] Compute(byte[] data)
        {
            return data.ComputeHmac(Key, HashAlgorithm);
        }

        public byte[] Compute(string data)
        {
            return data.ComputeHmac(Key, HashAlgorithm);
        }

        public string ComputeEncoded(byte[] data)
        {
            return data.ComputeHmacEncoded(Key, Encoder, HashAlgorithm);
        }

        public string ComputeEncoded(string data)
        {
            return data.ComputeHmacEncoded(Key, Encoder, HashAlgorithm);
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
