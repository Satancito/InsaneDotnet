using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Serialization;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Cryptography.Abstractions
{
    public interface IHasher : IHasherJsonSerializable
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
            Type hasherType = TypeIdentifierResolver.ResolveSerializedType(typeof(IHasher), jsonNode, json);
            return (IHasher)TypeIdentifierResolver.InvokeDeserialize(typeof(IHasher), hasherType, json);
        }
    }
}
