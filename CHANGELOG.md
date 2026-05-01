# Changelog

---

## 10.5.1

This patch release tightens a few cryptography internals, keeps the public RSA documentation aligned with the current implementation, and rolls the package metadata forward to the next patch version.

### Cryptography

Changes:

- Moved RSA key-format regular expressions in `RsaExtensions` to source-generated regex factories.
- Kept the current key-format coverage for:
  - BER / Base64 body
  - PEM
  - XML
- Included the latest cryptography helper cleanup already present in the current codebase.

Impact:

- RSA key detection keeps the same public behavior while reducing regex initialization overhead.
- The package remains aligned with current .NET regex guidance for hot-path validation helpers.

### Documentation

Changes:

- Refreshed the RSA sections in the main cryptography manual.
- Refreshed the namespace-based cryptography extensions reference to reflect the current key-detection implementation.

### Package Metadata

Changes:

- Bumped `InsaneIO.Insane` to `10.5.1`.
- Updated package version snippets in `README.md`.
- Refreshed package release notes for the patch release.

### Validation

- Re-ran the full test suite after the patch updates.

---

## 10.5.0

This release expands the public documentation set into a namespace-based manual, refreshes the package metadata for the current cryptography and security surface, and documents the latest TOTP and serialization behavior in detail.

### Documentation

Changes:

- Added a namespace documentation index at `Docs/namespaces/Namespaces.md`.
- Added namespace-level reference pages for all public namespaces.
- Expanded the documentation for:
  - concrete cryptography classes
  - cryptography abstractions
  - cryptography extension families
  - security and TOTP APIs
  - serialization infrastructure
  - general-purpose extensions and helper contracts

Impact:

- Consumers now have a structured, implementation-aware manual instead of a single broad overview file.
- The documentation follows the public namespace hierarchy used by the codebase.

### Security / TOTP

Changes:

- Documented `TotpManager`, `TotpExtensions`, and `TotpTimeWindowTolerance` in detail.
- Documented current-window validation and tolerance windows:
  - `None`
  - `OneWindow`
  - `TwoWindows`
- Documented URI generation behavior, RFC algorithm names, and MD5 normalization to SHA1 for TOTP.
- Documented and aligned the current normalization behavior for unsupported TOTP hash choices:
  - `Md5 -> Sha1`
  - `Sha384 -> Sha1`
- Cleaned up TOTP counter/truncation byte-order handling to use explicit big-endian reads and writes.

Impact:

- TOTP consumers now have a clear guide for choosing between direct helpers and the manager API.
- Time-window tolerance behavior is now explicitly documented.

### Serialization

Changes:

- Expanded documentation for `IJsonSerializable` and `TypeIdentifierResolver`.
- Documented `TypeIdentifier`-based dynamic deserialization and custom type participation rules.

Impact:

- Consumers implementing custom serializable types now have a clear contract to follow.

### Package Metadata

Changes:

- Bumped `InsaneIO.Insane` to `10.5.0`.
- Updated package version snippets in `README.md`.
- Refreshed package release notes to match the current public surface.

### Validation

- Rebuilt the solution.
- Re-ran the full test suite after the documentation and metadata updates.

---

## 10.4.0

This version consolidates the TOTP surface, finishes the move to `TypeIdentifier`-only crypto deserialization, and packages the recent cryptography reorganization as a single release.

### TOTP

Changes:

- Removed `TotpGenerator`.
- Expanded `TotpManager` to cover the convenience scenarios previously handled by `TotpGenerator`.
- Added:
  - `FromSecret(...)`
  - `FromBase32Secret(...)`
  - `FromEncodedSecret(...)`
  - `GenerateTotpUri()`
  - `ComputeTotpCode(...)`
  - `VerifyTotpCode(...)`
  - `ComputeRemainingSeconds()`
  - `ComputeTotpRemainingSeconds()`

Impact:

- There is now a single main TOTP entry point.
- Consumers can migrate from `TotpGenerator` without losing convenience APIs.

### Serialization

Changes:

- Removed the remaining dependency on `AssemblyName` for crypto serialization and deserialization.
- Dynamic and concrete deserialization now rely only on `TypeIdentifier`.

Impact:

- Serialized payloads are cleaner and no longer depend on CLR type naming.
- Type resolution is more stable across refactors.

### Project Organization

Changes:

- Crypto extension classes were physically grouped under `Cryptography/Extensions`.
- Crypto enums were physically grouped under `Cryptography/Enum`.
- `TypeIdentifierResolver` was moved under `Cryptography/Internal`.
- Removed `ISecureJsonSerializable`.

Impact:

- The project layout now better matches the public namespaces.
- Internal resolution code is easier to locate and maintain.

### Documentation

Improvements:

- Updated `Docs/Cryptography.md` to reflect the TOTP consolidation and current `TypeIdentifier`-only flow.
- Updated package version examples in `README.md` to `10.4.0`.

### Testing

Added or expanded coverage for:

- `TotpManager` factory methods
- `TotpManager` compatibility aliases
- removal-safe migration coverage for the former `TotpGenerator` scenarios

### Compatibility

- This version removes `TotpGenerator`.
- TOTP consumers should migrate to `TotpManager`.

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

- Added `TypeIdentifierAttribute`.
- Concrete encoders, hashers, encryptors, and RSA key pairs now declare a stable type identifier.
- Dynamic deserialization resolves types through `TypeIdentifier`.

Impact:

- Reduces dependence on the exact CLR type name.
- Improves compatibility across namespace or assembly refactors.
- Provides a better story for custom implementations in external assemblies.

### Dynamic Deserialization

Improvements:

- `IEncoder.DeserializeDynamic`, `IHasher.DeserializeDynamic`, and `IEncryptor.DeserializeDynamic` now support stable identifier resolution.
- Added an internal static cache to avoid scanning loaded assemblies on every deserialization.
- Concrete `Deserialize(string json)` methods validate through `TypeIdentifier`.

Impact:

- Better performance in repeated dynamic deserialization scenarios.
- Better robustness for serialized payloads coming from external types.

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
- Documented the new deserialization flow using `TypeIdentifier`.
- Added complete and simple examples for implementing:
  - `IEncoder`
  - `IHasher`
  - `IEncryptor`
- `README.md` updates version examples to `10.3.0`.

### Testing

Added or expanded coverage for:

- dynamic deserialization by `TypeIdentifier`
- missing or invalid `TypeIdentifier` rejection
- concrete serializers validating the expected root type
- separate tests by class for hash, hmac, scrypt, and argon2
- reorganized crypto imports and namespaces

### Compatibility

- Existing `Deserialize(string json)` methods were not removed.
- Dynamic deserialization now depends on `TypeIdentifier`.

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

- `TotpManager.Deserialize(string json)` now validates `TypeIdentifier`.
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
- rejection of incorrect `TypeIdentifier` values in concrete deserializers
- `TotpManager.Deserialize` with invalid enums
- `DecryptAesCbc` with ciphertext shorter than the IV
- serialization/deserialization round-trips in encoders, hashers, and encryptors
