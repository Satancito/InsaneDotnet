﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InsaneIO.Insane.Cryptography
{
    [RequiresPreviewFeatures]
    public class ShaHasher : IHasher, IHasherJsonSerialize
    {
        public static Type HasherType => typeof(ShaHasher);

        public HashAlgorithm HashAlgorithm { get; init; } = HashAlgorithm.Sha512;
        public IEncoder Encoder { get; init; } = Base64Encoder.DefaultInstance;

        private string _name = IBaseSerialize.GetName(HasherType);
        public string Name
        {
            get
            {
                return _name;
            }
            init
            {
                if (_name is not null)
                {
                    return;
                }
                _name = value;
            }
        }

        public ShaHasher()
        {
        }

        public static IHasher Deserialize(string json)
        {
            JsonNode jsonNode = JsonNode.Parse(json)!;
            Type encoderType = Type.GetType(jsonNode[nameof(Encoder)]![nameof(IEncoder.Name)]!.GetValue<string>())!;
            return new ShaHasher
            {
                HashAlgorithm = Enum.Parse<HashAlgorithm>(jsonNode[nameof(HashAlgorithm)]!.GetValue<int>().ToString()),
                Encoder = (IEncoder)JsonSerializer.Deserialize(jsonNode[nameof(Encoder)], encoderType)!
            };
        }

        public byte[] Compute(byte[] data)
        {
            return HashExtensions.ToHash(data, HashAlgorithm);
        }

        public string Compute(string data)
        {
            return Encoder.Encode(Compute(data.ToByteArrayUtf8()));
        }

        public string Serialize()
        {
            return ToJsonObject().ToJsonString();
        }

        public JsonObject ToJsonObject()
        {
            return new JsonObject()
            {
                [nameof(Name)] = Name,
                [nameof(HashAlgorithm)] = HashAlgorithm.NumberValue<int>(),
                [nameof(Encoder)] = Encoder.ToJsonObject()
            };
        }

        public bool Verify(byte[] data, byte[] expected)
        {
            return Enumerable.SequenceEqual(Compute(data), expected);
        }

        public bool Verify(string data, string expected)
        {
            return Compute(data).Equals(expected);
        }
    }
}