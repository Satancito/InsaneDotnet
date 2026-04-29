using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Attributes;
using InsaneIO.Insane.Extensions;
using InsaneIO.Insane.Serialization;
using System.Text.Json.Nodes;
using InsaneIO.Insane.Cryptography.Enums;

namespace InsaneIO.Insane.Cryptography;

[TypeIdentifier("Insane-Cryptography-RsaEncryptor")]
public class RsaEncryptor : IEncryptor
{
    public required RsaKeyPair KeyPair { get; init; }
    public RsaPadding Padding { get; init; } = RsaPadding.OaepSha256;
    public IEncoder Encoder { get; init; } = Base64Encoder.DefaultInstance;

    public RsaEncryptor()
    {
    }

    public static IEncryptor Deserialize(string json)
    {
        try
        {
            JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(typeof(RsaEncryptor), json);
            if (!TypeIdentifierResolver.MatchesSerializedType(typeof(RsaEncryptor), jsonNode))
            {
                throw new DeserializeException(typeof(RsaEncryptor), json);
            }

            IEncoder encoder = IEncoder.DeserializeDynamic(jsonNode[nameof(Encoder)]?.ToJsonString() ?? throw new DeserializeException(typeof(RsaEncryptor), json));
            RsaKeyPair keyPair = RsaKeyPair.Deserialize(jsonNode[nameof(KeyPair)]?.ToJsonString() ?? throw new DeserializeException(typeof(RsaEncryptor), json))
                ?? throw new DeserializeException(typeof(RsaEncryptor), json);
            RsaPadding padding = (RsaPadding)(jsonNode[nameof(Padding)]?.GetValue<int>() ?? throw new DeserializeException(typeof(RsaEncryptor), json));
            if (!Enum.IsDefined(padding))
            {
                throw new DeserializeException(typeof(RsaEncryptor), json);
            }

            return new RsaEncryptor
            {
                KeyPair = keyPair,
                Encoder = encoder,
                Padding = padding
            };
        }
        catch (DeserializeException)
        {
            throw;
        }
        catch
        {
            throw new DeserializeException(typeof(RsaEncryptor), json);
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
            [TypeIdentifierResolver.TypeIdentifierJsonPropertyName] = TypeIdentifierResolver.GetTypeIdentifier(typeof(RsaEncryptor)),
            [nameof(KeyPair)] = KeyPair.ToJsonObject(),
            [nameof(Padding)] = Padding.NumberValue<int>(),
            [nameof(Encoder)] = Encoder.ToJsonObject(),
        };
    }

    public byte[] Encrypt(byte[] data)
    {
        return data.EncryptRsa(KeyPair.PublicKey!, Padding);
    }

    public byte[] Encrypt(string data)
    {
        return data.EncryptRsa(KeyPair.PublicKey!, Padding);
    }

    public string EncryptEncoded(byte[] data)
    {
        return data.EncryptRsaEncoded(KeyPair.PublicKey!, Encoder, Padding);
    }

    public string EncryptEncoded(string data)
    {
        return data.EncryptRsaEncoded(KeyPair.PublicKey!, Encoder, Padding);
    }

    public byte[] Decrypt(byte[] data)
    {
        return data.DecryptRsa(KeyPair.PrivateKey!, Padding);
    }

    public byte[] DecryptEncoded(string data)
    {
        return data.DecryptRsaFromEncoded(KeyPair.PrivateKey!, Encoder, Padding);
    }
}
