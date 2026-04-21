using InsaneIO.Insane.Misc;
using InsaneIO.Insane.Serialization;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Cryptography
{
    
    public class HexEncoder : IEncoder, IDefaultInstance<HexEncoder>
    {
        public static Type SelfType => typeof(HexEncoder);
        public string AssemblyName { get => IJsonSerializable.GetName(SelfType); }

        private static readonly HexEncoder _DefaultInstance = new();
        public static HexEncoder DefaultInstance => _DefaultInstance;
        public bool ToUpper { get; init; } = false;


        public HexEncoder()
        {
        }

        public byte[] Decode(string data)
        {
            return data.DecodeFromHex();
        }

        public string Encode(byte[] data)
        {
            return data.EncodeToHex(ToUpper);
        }

        public JsonObject ToJsonObject()
        {
            return new JsonObject()
            {
                [nameof(AssemblyName)] = AssemblyName,
                [nameof(ToUpper)] = ToUpper
            };
        }

        public string Serialize(bool indented = false)
        {
            return ToJsonObject().ToJsonString(IJsonSerializable.GetIndentOptions(indented));
        }

        public static IEncoder Deserialize(string json)
        {
            JsonNode jsonNode = JsonNode.Parse(json)!;
            return new HexEncoder
            {
                ToUpper = jsonNode[nameof(ToUpper)]!.GetValue<bool>()
            };
        }

        public string Encode(string data)
        {
            return Encode(data.ToByteArrayUtf8());
        }
    }
}
