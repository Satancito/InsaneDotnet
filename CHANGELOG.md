# Changelog

---

## 10.3.0

This version expands the public cryptography API without breaking compatibility. The main focus is improving extensibility for custom implementations, decoupling deserialization from the exact CLR type name, and better organizing the public crypto surface.

### Namespaces

Changes:

- Cryptography extensions are consolidated under `InsaneIO.Insane.Cryptography.Extensions`.
- Crypto interfaces were moved to `InsaneIO.Insane.Cryptography.Abstractions`.

Impact:

- The API is more organized and expressive for direct consumption.
- Contracts, implementations, and cryptography extensions are better separated.

### Extensibility

New capabilities:

- Added `CryptographyTypeAttribute`.
- Concrete encoders, hashers, and encryptors now declare a stable type identifier.
- Dynamic deserialization now tries to resolve types through `CryptographyType` first.
- If it cannot resolve by identifier, it falls back to `AssemblyName`.

Impact:

- Reduces dependence on the exact CLR type name.
- Improves compatibility across namespace or assembly refactors.
- Provides a better story for custom implementations in external assemblies.

### Dynamic Deserialization

Improvements:

- `IEncoder.DeserializeDynamic`, `IHasher.DeserializeDynamic`, and `IEncryptor.DeserializeDynamic` now support stable identifier resolution.
- Added an internal static cache to avoid scanning loaded assemblies on every deserialization.
- Concrete `Deserialize(string json)` methods validate through `CryptographyType` first and `AssemblyName` as fallback.

Impact:

- Better performance in repeated dynamic deserialization scenarios.
- Better robustness for serialized payloads in future versions or coming from external types.

### Algorithms and Extensions

Changes:

- `HashExtensions` was split by responsibility into:
  - `HashExtensions`
  - `HmacExtensions`
  - `ScryptExtensions`
  - `Argon2Extensions`
- Added an internal shared helper for cryptographic comparisons.

Impact:

- The API is clearer and easier to navigate.
- Per-algorithm organization improves maintainability and documentation.

### SCrypt

Changes:

- `InsaneIO.CryptSharp.SCrypt` was added to the solution.
- `InsaneIO.Insane` now uses a `ProjectReference` to `InsaneIO.CryptSharp.SCrypt` instead of a `PackageReference`.

Impact:

- Better local integration between projects in the repository.
- Better debugging and version consistency during development.

### Documentation

Improvements:

- `Docs/Cryptography.md` was updated to reflect the new namespaces for extensions and interfaces.
- Documented the new deserialization flow using `CryptographyType` with `AssemblyName` fallback.
- Added complete and simple examples for implementing:
  - `IEncoder`
  - `IHasher`
  - `IEncryptor`
- `README.md` now points to a public URL for the cryptography documentation and updates version examples to `10.3.0`.

### Testing

Added or expanded coverage for:

- dynamic deserialization by `CryptographyType`
- `AssemblyName` fallback
- concrete serializers with altered `AssemblyName` and correct `CryptographyType`
- separate tests by class for hash, hmac, scrypt, and argon2
- reorganized crypto imports and namespaces

### Compatibility

- Existing `Deserialize(string json)` methods were not removed.
- `AssemblyName` support remains available as a fallback.
- The new `CryptographyType` capability is additive and compatible with previous payloads.

---

## 10.2.0

This version adds new API capabilities without breaking existing behavior. The main focus is improving polymorphic deserialization, tightening payload validation, and strengthening cryptographic comparisons.

### IEncoder

New capabilities:

- Added `IEncoder.DeserializeDynamic(string json)`.
- Allows deserializing a concrete encoder by reading `AssemblyName` from JSON.
- Supports dynamic reconstruction of implementations such as `HexEncoder`, `Base32Encoder`, and `Base64Encoder` without the consumer knowing the concrete type in advance.

Related improvements:

- `HexEncoder.Deserialize(string json)` now validates that the payload `AssemblyName` really corresponds to `HexEncoder`.
- `Base32Encoder.Deserialize(string json)` now validates that the payload `AssemblyName` really corresponds to `Base32Encoder`.
- `Base64Encoder.Deserialize(string json)` now validates that the payload `AssemblyName` really corresponds to `Base64Encoder`.
- Encoder deserializers now fail early with `DeserializeException` if the JSON belongs to another type or is missing required fields.

### IHasher

New capabilities:

- Added `IHasher.DeserializeDynamic(string json)`.
- Allows deserializing concrete hashers by reading `AssemblyName` from JSON.
- Supports dynamic reconstruction of `ShaHasher`, `HmacHasher`, `ScryptHasher`, and `Argon2Hasher`.

Related improvements:

- Concrete `Deserialize(string json)` methods validate the payload `AssemblyName` before restoring the object.
- `ShaHasher.Deserialize`, `HmacHasher.Deserialize`, `ScryptHasher.Deserialize`, and `Argon2Hasher.Deserialize` use stricter validation and fail with `DeserializeException` when the content does not match the expected type.
- `Argon2Hasher.Deserialize` removes the redundant reconstruction of `Encoder`.
- `HashExtensions` verification helpers now use constant-time comparison for:
  - `VerifyHash`
  - `VerifyHashFromEncoded`
  - `VerifyHmac`
  - `VerifyHmacFromEncoded`
  - `VerifyScrypt`
  - `VerifyScryptFromEncoded`
  - `VerifyArgon2`
  - `VerifyArgon2FromEncoded`

Impact:

- Reduces the risk of timing side channels when comparing derived values, hashes, and expected HMACs.

### IEncryptor

New capabilities:

- Added `IEncryptor.DeserializeDynamic(string json)`.
- Allows deserializing concrete encryptors by reading `AssemblyName` from JSON.
- Supports dynamic reconstruction of `AesCbcEncryptor` and `RsaEncryptor`.

Related improvements:

- `AesCbcEncryptor.Deserialize(string json)` validates the root payload type before restoring the object.
- `RsaEncryptor.Deserialize(string json)` validates the root payload type before restoring the object.
- Encryptors restore their `Encoder` through dynamic `IEncoder` deserialization.
- `RsaEncryptor.Deserialize` reuses `RsaKeyPair.Deserialize` to reconstruct the key pair consistently.

### Auxiliary Serialization and Deserialization

#### RsaKeyPair

Improvements:

- `RsaKeyPair.Deserialize(string json)` no longer depends on `JsonSerializer.Deserialize<RsaKeyPair>(json)` without validation.
- It now validates `AssemblyName`.
- It now rejects payloads that contain neither `PublicKey` nor `PrivateKey`.
- It fails early with `DeserializeException` when the content does not represent a valid `RsaKeyPair`.

#### TotpManager

Improvements:

- `TotpManager.Deserialize(string json)` now validates `AssemblyName`.
- It validates that `CodeLength` and `HashAlgorithm` match defined enum values.
- It fails early with `DeserializeException` when the payload belongs to another type or contains invalid enums.

### AES

Improvements:

- `DecryptAesCbc(byte[] data, byte[] key, AesCbcPadding padding = ...)` now validates that the ciphertext has at least 16 bytes to contain the IV.
- If the value is truncated, it fails early with a clear message instead of delegating to a less explicit internal `Aes` exception.

### Compatibility

- No existing APIs were removed.
- Concrete `Deserialize(string json)` methods remain available.
- The new dynamic deserialization is additional and complementary.
- Existing valid behavior is preserved; the changes mainly harden invalid payload, wrong-type, or sensitive-comparison cases.

### Testing

Added or expanded coverage for:

- `IEncoder.DeserializeDynamic`
- `IHasher.DeserializeDynamic`
- `IEncryptor.DeserializeDynamic`
- rejection of incorrect `AssemblyName` values in concrete deserializers
- `TotpManager.Deserialize` with invalid enums
- `DecryptAesCbc` with ciphertext shorter than the IV
- serialization/deserialization round-trips in encoders, hashers, and encryptors
