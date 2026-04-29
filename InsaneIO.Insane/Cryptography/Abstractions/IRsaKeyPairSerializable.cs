using InsaneIO.Insane.Serialization;

namespace InsaneIO.Insane.Cryptography.Abstractions;

public interface IRsaKeyPairSerializable : IJsonSerializable
{
    public static abstract RsaKeyPair Deserialize(string json);
}
