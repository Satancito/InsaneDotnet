using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Attributes;
using InsaneIO.Insane.Misc;
using InsaneIO.Insane.Serialization;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Cryptography;

[TypeIdentifier("Insane-Cryptography-HexEncoder")]
public class HexEncoder : IEncoder, IDefaultInstance<HexEncoder>
{
    private static readonly HexEncoder _DefaultInstance = new();
    public static HexEncoder DefaultInstance => _DefaultInstance;
    public bool ToUpper { get; init; } = false;

    public HexEncoder()
    {
    }

    public byte[] Decode(string data)
    {
        return data.DecodeFromHex();
    }

    public string Encode(byte[] data)
    {
        return data.EncodeToHex(ToUpper);
    }

    public JsonObject ToJsonObject()
    {
        return new JsonObject()
        {
            [TypeIdentifierResolver.TypeIdentifierJsonPropertyName] = TypeIdentifierResolver.GetTypeIdentifier(typeof(HexEncoder)),
            [nameof(ToUpper)] = ToUpper
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
            JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(typeof(HexEncoder), json);
            if (!TypeIdentifierResolver.MatchesSerializedType(typeof(HexEncoder), jsonNode))
            {
                throw new DeserializeException(typeof(HexEncoder), json);
            }

            return new HexEncoder
            {
                ToUpper = jsonNode[nameof(ToUpper)]?.GetValue<bool>() ?? throw new DeserializeException(typeof(HexEncoder), json)
            };
        }
        catch (DeserializeException)
        {
            throw;
        }
        catch
        {
            throw new DeserializeException(typeof(HexEncoder), json);
        }
    }

    public string Encode(string data)
    {
        return Encode(data.ToByteArrayUtf8());
    }
}
