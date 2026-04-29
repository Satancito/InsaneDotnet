using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Attributes;
using InsaneIO.Insane.Serialization;
using System.Linq;
using System.Text.Json.Nodes;
using InsaneIO.Insane.Cryptography.Enums;

namespace InsaneIO.Insane.Cryptography;

[TypeIdentifier("Insane-Cryptography-ShaHasher")]
public class ShaHasher : IHasher
{
    public HashAlgorithm HashAlgorithm { get; init; } = HashAlgorithm.Sha512;
    public IEncoder Encoder { get; init; } = Base64Encoder.DefaultInstance;

    public ShaHasher()
    {
    }

    public static IHasher Deserialize(string json)
    {
        try
        {
            JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(typeof(ShaHasher), json);
            if (!TypeIdentifierResolver.MatchesSerializedType(typeof(ShaHasher), jsonNode))
            {
                throw new DeserializeException(typeof(ShaHasher), json);
            }

            HashAlgorithm hashAlgorithm = (HashAlgorithm)(jsonNode[nameof(HashAlgorithm)]?.GetValue<int>() ?? throw new DeserializeException(typeof(ShaHasher), json));
            if (!Enum.IsDefined(hashAlgorithm))
            {
                throw new DeserializeException(typeof(ShaHasher), json);
            }

            return new ShaHasher
            {
                HashAlgorithm = hashAlgorithm,
                Encoder = IEncoder.DeserializeDynamic(jsonNode[nameof(Encoder)]?.ToJsonString() ?? throw new DeserializeException(typeof(ShaHasher), json))
            };
        }
        catch (DeserializeException)
        {
            throw;
        }
        catch
        {
            throw new DeserializeException(typeof(ShaHasher), json);
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
            [TypeIdentifierResolver.TypeIdentifierJsonPropertyName] = TypeIdentifierResolver.GetTypeIdentifier(typeof(ShaHasher)),
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
