using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;
using InsaneIO.Insane.Serialization;

namespace InsaneIO.Insane.Cryptography
{
    
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
            JsonNode jsonNode = JsonNode.Parse(json)!;
            Type encoderType = Type.GetType(jsonNode[nameof(Encoder)]![nameof(IEncoder.AssemblyName)]!.GetValue<string>())!;
            return new ShaHasher
            {
                HashAlgorithm = Enum.Parse<HashAlgorithm>(jsonNode[nameof(HashAlgorithm)]!.GetValue<int>().ToString()),
                Encoder = (IEncoder)JsonSerializer.Deserialize(jsonNode[nameof(Encoder)], encoderType)!
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
