# InsaneIO.Insane.Attributes

Parent namespace: [InsaneIO.Insane](../Insane.md)

This namespace contains the attribute-based metadata that participates in public serialization and dynamic type resolution.

Use it when you want your own public types to opt into the same `TypeIdentifier`-driven deserialization flow used by the built-in encoders, hashers, encryptors, and `TotpManager`.

## Public types

- `TypeIdentifierAttribute`

## TypeIdentifierAttribute

`TypeIdentifierAttribute` assigns a stable string identifier to a concrete class.

The attribute is the public-facing identity used by `TypeIdentifierResolver`. It replaces the older idea of resolving types through CLR names or assembly names.

### Constructor

```csharp
public TypeIdentifierAttribute(string identifier)
```

`identifier` must be non-null, non-empty, and non-whitespace. The constructor enforces this with `ArgumentException.ThrowIfNullOrWhiteSpace`.

### Why you would use it

Add this attribute when you want a custom class to:

- include a stable `TypeIdentifier` in JSON
- be discoverable by `TypeIdentifierResolver`
- participate in `DeserializeDynamic(...)` for the relevant abstraction

### Example

```csharp
using InsaneIO.Insane.Attributes;
using InsaneIO.Insane.Cryptography.Abstractions;
using InsaneIO.Insane.Serialization;
using System.Text.Json.Nodes;

[TypeIdentifier("Contoso-Cryptography-BinaryEncoder")]
public sealed class BinaryEncoder : IEncoder
{
    public string Encode(byte[] data) => Convert.ToString(data, 2);
    public string Encode(string data) => Encode(data.ToByteArrayUtf8());
    public byte[] Decode(string data) => throw new NotSupportedException();

    public JsonObject ToJsonObject() => new()
    {
        [TypeIdentifierResolver.TypeIdentifierJsonPropertyName] =
            TypeIdentifierResolver.GetTypeIdentifier(typeof(BinaryEncoder))
    };

    public string Serialize(bool indented = false) =>
        ToJsonObject().ToJsonString(IJsonSerializable.GetIndentOptions(indented));

    public static IEncoder Deserialize(string json)
    {
        JsonNode node = JsonNode.Parse(json)!;
        if (!TypeIdentifierResolver.MatchesSerializedType(typeof(BinaryEncoder), node))
        {
            throw new InvalidOperationException("Invalid payload.");
        }

        return new BinaryEncoder();
    }
}
```

### Guidance

- Keep the identifier stable across versions.
- Treat it as public contract data.
- Avoid reusing the same identifier across different concrete classes.
