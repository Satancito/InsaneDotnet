using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InsaneIO.Insane.Serialization;

public interface IJsonSerializable
{
    private static JsonSerializerOptions Indented = new JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter() } };
    private static JsonSerializerOptions NotIndented = new JsonSerializerOptions { WriteIndented = false, Converters = { new JsonStringEnumConverter() } };
    
    public static JsonSerializerOptions GetIndentOptions(bool indented) { return indented? Indented: NotIndented; }
    public JsonObject ToJsonObject();
    public string Serialize(bool indented = false);
}
