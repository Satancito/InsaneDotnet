using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Serialization;
using System.Reflection;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Cryptography
{
    
    public interface IHasher: IHasherJsonSerializable
    {

        public byte[] Compute(byte[] data);
        public byte[] Compute(string data);
        public string ComputeEncoded(byte[] data);
        public string ComputeEncoded(string data);

        public bool Verify(byte[] data, byte[] expected);
        public bool Verify(string data, byte[] expected);
        public bool VerifyEncoded(byte[] data, string expected);
        public bool VerifyEncoded(string data, string expected);

        public static IHasher DeserializeDynamic(string json)
        {
            JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(typeof(IHasher), json);
            string assemblyName = jsonNode[nameof(IBaseSerializable.AssemblyName)]?.GetValue<string>() ?? throw new DeserializeException(typeof(IHasher), json);
            Type hasherType = Type.GetType(assemblyName) ?? throw new DeserializeException(typeof(IHasher), json);

            if (!typeof(IHasher).IsAssignableFrom(hasherType))
            {
                throw new DeserializeException(typeof(IHasher), json);
            }

            MethodInfo? deserializeMethod = hasherType.GetMethod(
                nameof(Deserialize),
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: [typeof(string)],
                modifiers: null);

            if (deserializeMethod is null || !typeof(IHasher).IsAssignableFrom(deserializeMethod.ReturnType))
            {
                throw new DeserializeException(typeof(IHasher), json);
            }

            return (IHasher?)deserializeMethod.Invoke(null, [json]) ?? throw new DeserializeException(typeof(IHasher), json);
        }
    }
}
