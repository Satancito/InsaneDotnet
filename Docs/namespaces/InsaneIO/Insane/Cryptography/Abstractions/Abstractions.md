# InsaneIO.Insane.Cryptography.Abstractions

Parent namespace: [InsaneIO.Insane.Cryptography](../Cryptography.md)

This namespace contains the public contracts that define the cryptography object model.

Use these abstractions when you want to:

- code against encoders, hashers, and encryptors generically
- store or pass serializable cryptographic components without knowing the concrete type
- deserialize a cryptography component dynamically from JSON

## Public types

- `IEncoder`
- `IEncoderJsonSerializable`
- `IHasher`
- `IHasherJsonSerializable`
- `IEncryptor`
- `IEncryptorJsonSerializable`
- `IRsaKeyPairSerializable`

## Choosing the right API

- Prefer the concrete classes when you control the type and want explicit configuration.
- Prefer these interfaces when you want dependency injection, plug-ins, or dynamic deserialization.
- Prefer `DeserializeDynamic(...)` when the payload may represent any registered implementation for the contract.

## IEncoder

`IEncoder` converts raw bytes and strings to textual encodings and back again.

### Members

- `Encode(byte[] data)`
- `Encode(string data)`
- `Decode(string data)`
- `static DeserializeDynamic(string json)`

### Dynamic deserialization

`IEncoder.DeserializeDynamic(string json)`:

1. parses the JSON
2. resolves the concrete encoder by `TypeIdentifier`
3. invokes that concrete type's static `Deserialize(string json)`

This is how nested encoder properties are restored inside hashers and encryptors.

## IHasher

`IHasher` represents stateful hashing configuration plus convenience verification helpers.

### Members

- `Compute(...)`
- `ComputeEncoded(...)`
- `Verify(...)`
- `VerifyEncoded(...)`
- `static DeserializeDynamic(string json)`

Use this when you want a reusable configured hasher object rather than one-off extension calls.

## IEncryptor

`IEncryptor` represents a reusable configured encryption component.

### Members

- `Encrypt(...)`
- `EncryptEncoded(...)`
- `Decrypt(...)`
- `DecryptEncoded(...)`
- `static DeserializeDynamic(string json)`

Use this when your application wants a configured object rather than direct extension methods.

## Serialization-specific contracts

`IEncoderJsonSerializable`, `IHasherJsonSerializable`, `IEncryptorJsonSerializable`, and `IRsaKeyPairSerializable` exist so each concrete implementation can expose a static `Deserialize(string json)` method with the appropriate return type.

These are useful when you want compile-time enforcement that a serializable component includes its own static factory.

## Example: deserialize a configured hasher from stored JSON

```csharp
using InsaneIO.Insane.Cryptography.Abstractions;

string json = File.ReadAllText("hasher.json");
IHasher hasher = IHasher.DeserializeDynamic(json);

string digest = hasher.ComputeEncoded("payload");
```
