using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Serialization;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Cryptography.Abstractions
{
    public interface IEncryptor : IEncryptorJsonSerializable
    {
        public byte[] Encrypt(byte[] data);
        public byte[] Encrypt(string data);
        public string EncryptEncoded(byte[] data);
        public string EncryptEncoded(string data);
        public byte[] Decrypt(byte[] data);
        public byte[] DecryptEncoded(string data);

        public static IEncryptor DeserializeDynamic(string json)
        {
            JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(typeof(IEncryptor), json);
            Type encryptorType = CryptographyTypeResolver.ResolveSerializedType(typeof(IEncryptor), jsonNode, json);
            return (IEncryptor)CryptographyTypeResolver.InvokeDeserialize(typeof(IEncryptor), encryptorType, json);
        }
    }
}
