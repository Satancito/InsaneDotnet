using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Attributes;
using InsaneIO.Insane.Misc;
using InsaneIO.Insane.Serialization;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Cryptography;

[TypeIdentifier("Insane-Cryptography-Base32Encoder")]
public class Base32Encoder : IEncoder, IDefaultInstance<Base32Encoder>
{
    private static readonly Base32Encoder _DefaultInstance = new();
    public static Base32Encoder DefaultInstance => _DefaultInstance;

    public bool ToLower { get; set; } = false;
    public bool RemovePadding { get; set; } = false;

    public Base32Encoder()
    {
    }

    public byte[] Decode(string data)
    {
        return data.DecodeFromBase32();
    }

    public string Encode(byte[] data)
    {
        return data.EncodeToBase32(RemovePadding, ToLower);
    }

    public string Encode(string data)
    {
        return Encode(data.ToByteArrayUtf8());
    }

    public JsonObject ToJsonObject()
    {
        return new JsonObject()
        {
            [TypeIdentifierResolver.TypeIdentifierJsonPropertyName] = TypeIdentifierResolver.GetTypeIdentifier(typeof(Base32Encoder)),
            [nameof(RemovePadding)] = RemovePadding,
            [nameof(ToLower)] = ToLower,
        };
    }

    public string Serialize(bool indented = false)
    {
        return ToJsonObject().ToJsonString(IJsonSerializable.GetIndentOptions(indented));
    }

    public static IEncoder Deserialize(string json)
    {
        try
        {
            JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(typeof(Base32Encoder), json);
            if (!TypeIdentifierResolver.MatchesSerializedType(typeof(Base32Encoder), jsonNode))
            {
                throw new DeserializeException(typeof(Base32Encoder), json);
            }

            return new Base32Encoder
            {
                RemovePadding = jsonNode[nameof(RemovePadding)]?.GetValue<bool>() ?? throw new DeserializeException(typeof(Base32Encoder), json),
                ToLower = jsonNode[nameof(ToLower)]?.GetValue<bool>() ?? throw new DeserializeException(typeof(Base32Encoder), json)
            };
        }
        catch (DeserializeException)
        {
            throw;
        }
        catch
        {
            throw new DeserializeException(typeof(Base32Encoder), json);
        }
    }
}
