using InsaneIO.Insane.Serialization;

namespace InsaneIO.Insane.Cryptography.Abstractions
{
    public interface IEncryptorJsonSerializable : IJsonSerializable
    {
        public static abstract IEncryptor Deserialize(string json);
    }
}
