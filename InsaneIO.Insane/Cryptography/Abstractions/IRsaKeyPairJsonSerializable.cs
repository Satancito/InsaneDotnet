using InsaneIO.Insane.Serialization;

namespace InsaneIO.Insane.Cryptography.Abstractions
{
    public interface IRsaKeyPairJsonSerializable : IJsonSerializable
    {
        public static abstract RsaKeyPair Deserialize(string json);
    }
}
