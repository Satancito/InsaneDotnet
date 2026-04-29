using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Attributes;
using InsaneIO.Insane.Extensions;
using InsaneIO.Insane.Serialization;
using System.Linq;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Cryptography;

[TypeIdentifier("Insane-Cryptography-ScryptHasher")]
public class ScryptHasher : IHasher
{
    public string SaltString { get => Encoder.Encode(Salt); init => Salt = value.ToByteArrayUtf8(); }

    public byte[] SaltBytes { get => Salt; init => Salt = value; }

    private byte[] Salt = RandomExtensions.NextBytes(Constants.ScryptSaltSize);
    public uint Iterations { get; init; } = Constants.ScryptIterations;
    public uint BlockSize { get; init; } = Constants.ScryptBlockSize;
    public uint Parallelism { get; init; } = Constants.ScryptParallelism;
    public uint DerivedKeyLength { get; init; } = Constants.ScryptDerivedKeyLength;
    public IEncoder Encoder { get; init; } = Base64Encoder.DefaultInstance;

    public ScryptHasher()
    {
    }

    public static IHasher Deserialize(string json)
    {
        try
        {
            JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(typeof(ScryptHasher), json);
            if (!TypeIdentifierResolver.MatchesSerializedType(typeof(ScryptHasher), jsonNode))
            {
                throw new DeserializeException(typeof(ScryptHasher), json);
            }

            IEncoder encoder = IEncoder.DeserializeDynamic(jsonNode[nameof(Encoder)]?.ToJsonString() ?? throw new DeserializeException(typeof(ScryptHasher), json));

            return new ScryptHasher
            {
                Salt = encoder.Decode(jsonNode[nameof(Salt)]?.GetValue<string>() ?? throw new DeserializeException(typeof(ScryptHasher), json)),
                Iterations = jsonNode[nameof(Iterations)]?.GetValue<uint>() ?? throw new DeserializeException(typeof(ScryptHasher), json),
                BlockSize = jsonNode[nameof(BlockSize)]?.GetValue<uint>() ?? throw new DeserializeException(typeof(ScryptHasher), json),
                Parallelism = jsonNode[nameof(Parallelism)]?.GetValue<uint>() ?? throw new DeserializeException(typeof(ScryptHasher), json),
                DerivedKeyLength = jsonNode[nameof(DerivedKeyLength)]?.GetValue<uint>() ?? throw new DeserializeException(typeof(ScryptHasher), json),
                Encoder = encoder,
            };
        }
        catch (DeserializeException)
        {
            throw;
        }
        catch
        {
            throw new DeserializeException(typeof(ScryptHasher), json);
        }
    }

    public string Serialize(bool indented = false)
    {
        return ToJsonObject().ToJsonString(IJsonSerializable.GetIndentOptions(indented));
    }

    public JsonObject ToJsonObject()
    {
        return new JsonObject
        {
            [TypeIdentifierResolver.TypeIdentifierJsonPropertyName] = TypeIdentifierResolver.GetTypeIdentifier(typeof(ScryptHasher)),
            [nameof(Salt)] = Encoder.Encode(Salt),
            [nameof(Iterations)] = Iterations,
            [nameof(BlockSize)] = BlockSize,
            [nameof(Parallelism)] = Parallelism,
            [nameof(DerivedKeyLength)] = DerivedKeyLength,
            [nameof(Encoder)] = Encoder.ToJsonObject(),
        };
    }

    public byte[] Compute(byte[] data)
    {
        return data.ComputeScrypt(Salt, Iterations, BlockSize, Parallelism, DerivedKeyLength);
    }

    public byte[] Compute(string data)
    {
        return data.ComputeScrypt(Salt, Iterations, BlockSize, Parallelism, DerivedKeyLength);
    }

    public string ComputeEncoded(byte[] data)
    {
        return data.ComputeScryptEncoded(Salt, Encoder, Iterations, BlockSize, Parallelism, DerivedKeyLength);
    }

    public string ComputeEncoded(string data)
    {
        return data.ComputeScryptEncoded(Salt, Encoder, Iterations, BlockSize, Parallelism, DerivedKeyLength);
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
