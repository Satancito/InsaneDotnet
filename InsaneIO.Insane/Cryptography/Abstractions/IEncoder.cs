using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Serialization;
using System.Reflection;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Cryptography.Abstractions
{
    public interface IEncoder : IEncoderJsonSerializable
    {
        public string Encode(byte[] data);
        public string Encode(string data);
        public byte[] Decode(string data);

        public static IEncoder DeserializeDynamic(string json)
        {
            JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(typeof(IEncoder), json);
            string assemblyName = jsonNode[nameof(IBaseSerializable.AssemblyName)]?.GetValue<string>() ?? throw new DeserializeException(typeof(IEncoder), json);
            Type encoderType = Type.GetType(assemblyName) ?? throw new DeserializeException(typeof(IEncoder), json);

            if (!typeof(IEncoder).IsAssignableFrom(encoderType))
            {
                throw new DeserializeException(typeof(IEncoder), json);
            }

            MethodInfo? deserializeMethod = encoderType.GetMethod(
                nameof(Deserialize),
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: [typeof(string)],
                modifiers: null);

            if (deserializeMethod is null || !typeof(IEncoder).IsAssignableFrom(deserializeMethod.ReturnType))
            {
                throw new DeserializeException(typeof(IEncoder), json);
            }

            return (IEncoder?)deserializeMethod.Invoke(null, [json]) ?? throw new DeserializeException(typeof(IEncoder), json);
        }
    }
}
