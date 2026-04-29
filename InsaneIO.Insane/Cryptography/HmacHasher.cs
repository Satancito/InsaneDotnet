using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Attributes;
using InsaneIO.Insane.Serialization;
using System.Text.Json.Nodes;
using InsaneIO.Insane.Cryptography.Enums;

namespace InsaneIO.Insane.Cryptography;

[TypeIdentifier("Insane-Cryptography-HmacHasher")]
public class HmacHasher : IHasher
{
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
        try
        {
            JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(typeof(HmacHasher), json);
            if (!TypeIdentifierResolver.MatchesSerializedType(typeof(HmacHasher), jsonNode))
            {
                throw new DeserializeException(typeof(HmacHasher), json);
            }

            HashAlgorithm hashAlgorithm = (HashAlgorithm)(jsonNode[nameof(HashAlgorithm)]?.GetValue<int>() ?? throw new DeserializeException(typeof(HmacHasher), json));
            if (!Enum.IsDefined(hashAlgorithm))
            {
                throw new DeserializeException(typeof(HmacHasher), json);
            }

            IEncoder encoder = IEncoder.DeserializeDynamic(jsonNode[nameof(Encoder)]?.ToJsonString() ?? throw new DeserializeException(typeof(HmacHasher), json));

            return new HmacHasher
            {
                HashAlgorithm = hashAlgorithm,
                Encoder = encoder,
                Key = encoder.Decode(jsonNode[nameof(Key)]?.GetValue<string>() ?? throw new DeserializeException(typeof(HmacHasher), json))
            };
        }
        catch (DeserializeException)
        {
            throw;
        }
        catch
        {
            throw new DeserializeException(typeof(HmacHasher), json);
        }
    }

    public string Serialize(bool indented = false)
    {
        return ToJsonObject().ToJsonString(IJsonSerializable.GetIndentOptions(indented));
    }

    public JsonObject ToJsonObject()
    {
        return new JsonObject()
        {
            [TypeIdentifierResolver.TypeIdentifierJsonPropertyName] = TypeIdentifierResolver.GetTypeIdentifier(typeof(HmacHasher)),
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
