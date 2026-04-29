using InsaneIO.Insane.Cryptography.Abstractions;
using InsaneIO.Insane.Attributes;
using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Serialization;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Cryptography;

[TypeIdentifier("Insane-Cryptography-RsaKeyPair")]
public class RsaKeyPair : IRsaKeyPairSerializable
{
    public string? PublicKey { get; init; }
    public string? PrivateKey { get; init; }

    public static RsaKeyPair Deserialize(string json)
    {
        try
        {
            JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(typeof(RsaKeyPair), json);
            if (!TypeIdentifierResolver.MatchesSerializedType(typeof(RsaKeyPair), jsonNode))
            {
                throw new DeserializeException(typeof(RsaKeyPair), json);
            }

            string? publicKey = jsonNode[nameof(PublicKey)]?.GetValue<string?>();
            string? privateKey = jsonNode[nameof(PrivateKey)]?.GetValue<string?>();

            if (string.IsNullOrWhiteSpace(publicKey) || string.IsNullOrWhiteSpace(privateKey))
            {
                throw new DeserializeException(typeof(RsaKeyPair), json);
            }

            return new RsaKeyPair
            {
                PublicKey = publicKey,
                PrivateKey = privateKey
            };
        }
        catch (DeserializeException)
        {
            throw;
        }
        catch
        {
            throw new DeserializeException(typeof(RsaKeyPair), json);
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
            [TypeIdentifierResolver.TypeIdentifierJsonPropertyName] = TypeIdentifierResolver.GetTypeIdentifier(typeof(RsaKeyPair)),
            [nameof(PublicKey)] = PublicKey,
            [nameof(PrivateKey)] = PrivateKey
        };
    }
}
