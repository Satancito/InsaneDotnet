using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Tests
{
    internal static class TestJsonMutations
    {
        public static JsonObject ParseObject(string json)
        {
            return JsonNode.Parse(json)?.AsObject() ?? throw new InvalidOperationException("Expected JSON object.");
        }

        public static string RemoveTypeIdentifier(string json)
        {
            return RemoveProperty(json, "TypeIdentifier");
        }

        public static string ReplaceTypeIdentifier(string json, string? value)
        {
            return ReplaceProperty(json, "TypeIdentifier", value is null ? null : JsonValue.Create(value));
        }

        public static string RemoveProperty(string json, string propertyName)
        {
            JsonObject jsonObject = ParseObject(json);
            jsonObject.Remove(propertyName);
            return jsonObject.ToJsonString();
        }

        public static string ReplaceProperty(string json, string propertyName, JsonNode? value)
        {
            JsonObject jsonObject = ParseObject(json);
            jsonObject[propertyName] = value;
            return jsonObject.ToJsonString();
        }
    }
}
