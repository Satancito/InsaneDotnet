using InsaneIO.Insane.Serialization;

namespace InsaneIO.Insane.Cryptography.Abstractions
{
    public interface IHasherJsonSerializable : IJsonSerializable
    {
        public static abstract IHasher Deserialize(string json);
    }
}
