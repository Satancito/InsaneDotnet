# InsaneIO.Insane.Serialization

Parent namespace: [InsaneIO.Insane](../Insane.md)

This namespace contains the public serialization infrastructure used across cryptography and security types.

Use it when you need to:

- emit the same JSON shape as the built-in serializable classes
- validate that JSON belongs to a specific concrete type
- dynamically resolve a concrete implementation by `TypeIdentifier`
- build custom types that participate in the library's polymorphic JSON flow

## Public types

- `IJsonSerializable`
- `TypeIdentifierResolver`

## When to use this namespace

- Prefer the object-oriented APIs (`Base64Encoder`, `ShaHasher`, `TotpManager`, and so on) when you already know the concrete type.
- Prefer the dynamic deserialization APIs (`IEncoder.DeserializeDynamic`, `IHasher.DeserializeDynamic`, `IEncryptor.DeserializeDynamic`) when the concrete type is unknown at compile time.
- Use `TypeIdentifierResolver` directly inside your own custom `Deserialize(string json)` implementations.

## IJsonSerializable

`IJsonSerializable` is the base contract for public JSON serialization in this package.

### Members

- `JsonObject ToJsonObject()`
- `string Serialize(bool indented = false)`
- `static JsonSerializerOptions GetIndentOptions(bool indented)`

### Typical flow

1. Build a `JsonObject` through `ToJsonObject()`.
2. Convert it to string through `Serialize(...)`.
3. Rehydrate with the concrete type's static `Deserialize(string json)` method.

### Indentation

`GetIndentOptions(bool indented)` returns the package-configured `JsonSerializerOptions` used by the implementations. It already includes `JsonStringEnumConverter`.

## TypeIdentifierResolver

`TypeIdentifierResolver` is the public bridge between `TypeIdentifierAttribute` and dynamic deserialization.

### Public members

- `TypeIdentifierJsonPropertyName`
- `GetTypeIdentifier(Type annotatedType)`
- `MatchesSerializedType(Type annotatedType, JsonNode jsonNode)`

### How resolution works

The resolver scans loaded assemblies for non-abstract types decorated with `TypeIdentifierAttribute`. It caches the mapping from identifier string to concrete `Type`.

Resolution rules:

- the JSON must contain `TypeIdentifier`
- the identifier must map to a unique decorated type
- duplicates across different concrete types are treated as an error
- no fallback exists to CLR names, assembly names, or class names

### Using the public helper members

#### Writing JSON

```csharp
new JsonObject
{
    [TypeIdentifierResolver.TypeIdentifierJsonPropertyName] =
        TypeIdentifierResolver.GetTypeIdentifier(typeof(MyType))
};
```

#### Validating a concrete payload

```csharp
JsonNode node = JsonNode.Parse(json)!;
if (!TypeIdentifierResolver.MatchesSerializedType(typeof(MyType), node))
{
    throw new DeserializeException(typeof(MyType), json);
}
```

### Adding your own serializable type

To participate in the current public flow:

1. Decorate the class with `TypeIdentifierAttribute`.
2. Implement `IJsonSerializable` (or one of the higher-level contracts such as `IEncoder`).
3. Add `TypeIdentifier` to `ToJsonObject()`.
4. Validate the incoming payload with `MatchesSerializedType(...)`.
5. Expose a public static `Deserialize(string json)` method with the expected return type.

### Example

```csharp
[TypeIdentifier("Contoso-Cryptography-XorEncryptor")]
public sealed class XorEncryptor : IEncryptor
{
    public required byte[] Key { get; init; }

    public JsonObject ToJsonObject() => new()
    {
        [TypeIdentifierResolver.TypeIdentifierJsonPropertyName] =
            TypeIdentifierResolver.GetTypeIdentifier(typeof(XorEncryptor)),
        [nameof(Key)] = Base64Encoder.DefaultInstance.Encode(Key)
    };

    public string Serialize(bool indented = false) =>
        ToJsonObject().ToJsonString(IJsonSerializable.GetIndentOptions(indented));

    public static IEncryptor Deserialize(string json)
    {
        JsonNode node = JsonNode.Parse(json) ?? throw new DeserializeException(typeof(XorEncryptor), json);
        if (!TypeIdentifierResolver.MatchesSerializedType(typeof(XorEncryptor), node))
        {
            throw new DeserializeException(typeof(XorEncryptor), json);
        }

        return new XorEncryptor
        {
            Key = Base64Encoder.DefaultInstance.Decode(
                node[nameof(Key)]?.GetValue<string>() ?? throw new DeserializeException(typeof(XorEncryptor), json))
        };
    }

    public byte[] Encrypt(byte[] data) => throw new NotImplementedException();
    public byte[] Encrypt(string data) => throw new NotImplementedException();
    public string EncryptEncoded(byte[] data) => throw new NotImplementedException();
    public string EncryptEncoded(string data) => throw new NotImplementedException();
    public byte[] Decrypt(byte[] data) => throw new NotImplementedException();
    public byte[] DecryptEncoded(string data) => throw new NotImplementedException();
}
```
