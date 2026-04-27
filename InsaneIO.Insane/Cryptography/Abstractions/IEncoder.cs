using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Serialization;
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
            Type encoderType = TypeIdentifierResolver.ResolveSerializedType(typeof(IEncoder), jsonNode, json);
            return (IEncoder)TypeIdentifierResolver.InvokeDeserialize(typeof(IEncoder), encoderType, json);
        }
    }
}
