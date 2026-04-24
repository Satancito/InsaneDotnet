using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Serialization;
using System.Reflection;
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
            string assemblyName = jsonNode[nameof(IBaseSerializable.AssemblyName)]?.GetValue<string>() ?? throw new DeserializeException(typeof(IEncryptor), json);
            Type encryptorType = Type.GetType(assemblyName) ?? throw new DeserializeException(typeof(IEncryptor), json);

            if (!typeof(IEncryptor).IsAssignableFrom(encryptorType))
            {
                throw new DeserializeException(typeof(IEncryptor), json);
            }

            MethodInfo? deserializeMethod = encryptorType.GetMethod(
                nameof(Deserialize),
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: [typeof(string)],
                modifiers: null);

            if (deserializeMethod is null || !typeof(IEncryptor).IsAssignableFrom(deserializeMethod.ReturnType))
            {
                throw new DeserializeException(typeof(IEncryptor), json);
            }

            return (IEncryptor?)deserializeMethod.Invoke(null, [json]) ?? throw new DeserializeException(typeof(IEncryptor), json);
        }
    }
}
