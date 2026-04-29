using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Attributes;
using InsaneIO.Insane.Extensions;
using InsaneIO.Insane.Serialization;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using InsaneIO.Insane.Cryptography.Enums;

namespace InsaneIO.Insane.Cryptography;

[TypeIdentifier("Insane-Cryptography-AesCbcEncryptor")]
public class AesCbcEncryptor : IEncryptor
{
    public string KeyString { get => Encoder.Encode(Key); init => Key = value.ToByteArrayUtf8(); }
    public byte[] KeyBytes { get => Key; init => Key = value; }

    private byte[] Key = RandomExtensions.NextBytes(AesExtensions.MaxKeyLength);

    public IEncoder Encoder { get; init; } = Base64Encoder.DefaultInstance;

    public AesCbcPadding Padding { get; init; } = AesCbcPadding.Pkcs7;

    public AesCbcEncryptor()
    {
    }

    public static IEncryptor Deserialize(string json)
    {
        try
        {
            JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(typeof(AesCbcEncryptor), json);
            if (!TypeIdentifierResolver.MatchesSerializedType(typeof(AesCbcEncryptor), jsonNode))
            {
                throw new DeserializeException(typeof(AesCbcEncryptor), json);
            }

            IEncoder encoder = IEncoder.DeserializeDynamic(jsonNode[nameof(Encoder)]?.ToJsonString() ?? throw new DeserializeException(typeof(AesCbcEncryptor), json));
            AesCbcPadding padding = (AesCbcPadding)(jsonNode[nameof(Padding)]?.GetValue<int>() ?? throw new DeserializeException(typeof(AesCbcEncryptor), json));
            if (!Enum.IsDefined(padding))
            {
                throw new DeserializeException(typeof(AesCbcEncryptor), json);
            }

            return new AesCbcEncryptor
            {
                Key = encoder.Decode(jsonNode[nameof(Key)]?.GetValue<string>() ?? throw new DeserializeException(typeof(AesCbcEncryptor), json)),
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
            throw new DeserializeException(typeof(AesCbcEncryptor), json);
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
            [TypeIdentifierResolver.TypeIdentifierJsonPropertyName] = TypeIdentifierResolver.GetTypeIdentifier(typeof(AesCbcEncryptor)),
            [nameof(Key)] = Encoder.Encode(Key),
            [nameof(Padding)] = Padding.NumberValue<int>(),
            [nameof(Encoder)] = Encoder.ToJsonObject(),
        };
    }

    public byte[] Encrypt(byte[] data)
    {
        return data.EncryptAesCbc(Key, Padding);
    }

    public byte[] Encrypt(string data)
    {
        return data.EncryptAesCbc(Key, Padding);
    }

    public string EncryptEncoded(byte[] data)
    {
        return data.EncryptAesCbcEncoded(Key, Encoder, Padding);
    }

    public string EncryptEncoded(string data)
    {
        return data.EncryptAesCbcEncoded(Key, Encoder, Padding);
    }

    public byte[] Decrypt(byte[] data)
    {
        return data.DecryptAesCbc(Key, Padding);
    }

    public byte[] DecryptEncoded(string data)
    {
        return data.DecryptAesCbcFromEncoded(Key, Encoder, Padding);
    }
}
