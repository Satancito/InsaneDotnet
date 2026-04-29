using InsaneIO.Insane.Serialization;

namespace InsaneIO.Insane.Cryptography.Abstractions;

public interface IEncoderJsonSerializable : IJsonSerializable
{
    public static abstract IEncoder Deserialize(string json);
}
