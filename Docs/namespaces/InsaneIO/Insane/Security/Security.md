# InsaneIO.Insane.Security

Parent namespace: [InsaneIO.Insane](../Insane.md)

This namespace contains the object-oriented TOTP API.

Use it when you want a reusable, serializable TOTP configuration that combines:

- the secret
- label and issuer
- digit count
- hash algorithm
- verification window size

## Public types

- `TotpManager`

## TotpManager

`TotpManager` is the main stateful TOTP entry point.

It replaces the old split between direct helpers and a separate generator object by gathering configuration, serialization, convenience aliases, and verification helpers in one place.

### Properties

- `Secret` (required)
- `Label` (required)
- `Issuer` (required)
- `CodeLength` defaults to `SixDigits`
- `HashAlgorithm` defaults to `Sha1`
- `TimePeriodInSeconds` defaults to `TotpExtensions.TotpDefaultPeriod` (`30`)

### Factory methods

- `FromSecret(...)`
- `FromBase32Secret(...)`
- `FromEncodedSecret(...)`

These are the easiest way to create equivalent managers from different secret representations.

### Serialization

`TotpManager` implements `IJsonSerializable`.

Important serialization behavior:

- the JSON includes `TypeIdentifier`
- `Secret` is serialized in Base32
- `CodeLength` and `HashAlgorithm` are serialized as numeric enum values
- `Serialize` defaults to `indented: true`

### JSON shape

```json
{
  "TypeIdentifier": "Insane-Security-TotpManager",
  "Secret": "MFXGG33EMV2W4YLMONQWG2A=",
  "Label": "user@example.com",
  "Issuer": "Insane IO",
  "CodeLength": 6,
  "HashAlgorithm": 1,
  "TimePeriodInSeconds": 30
}
```

### Validation rules during deserialization

`Deserialize(string json)` rejects:

- wrong or missing `TypeIdentifier`
- missing `Secret`
- missing `Label`
- missing `Issuer`
- undefined `CodeLength`
- undefined `HashAlgorithm`
- invalid numeric payloads such as a non-numeric `TimePeriodInSeconds`

### Main methods

- `ToOtpUri()`
- `GenerateTotpUri()` alias
- `ComputeCode()`
- `ComputeCode(DateTimeOffset now)`
- `ComputeTotpCode()` aliases
- `VerifyCode(...)`
- `VerifyTotpCode(...)` aliases
- `ComputeRemainingSeconds()`
- `ComputeTotpRemainingSeconds()` aliases

### Choosing between aliases

The alias methods exist for compatibility and readability. They are functionally equivalent:

- `GenerateTotpUri()` == `ToOtpUri()`
- `ComputeTotpCode()` == `ComputeCode()`
- `VerifyTotpCode()` == `VerifyCode()`
- `ComputeTotpRemainingSeconds()` == `ComputeRemainingSeconds()`

### Current-window verification vs tolerant verification

Without tolerance:

```csharp
bool valid = manager.VerifyCode(code);
```

This checks only the current time window.

With tolerance:

```csharp
bool valid = manager.VerifyCode(code, TotpTimeWindowTolerance.OneWindow);
```

This accepts the previous, current, and next window.

### TOTP algorithm normalization

`TotpManager` ultimately relies on `TotpExtensions` for code generation, verification, and URI generation.

That means the same normalization rules apply here:

- `Md5 -> Sha1`
- `Sha384 -> Sha1`
- `Sha1 -> Sha1`
- `Sha256 -> Sha256`
- `Sha512 -> Sha512`

### Complete example

```csharp
using InsaneIO.Insane.Cryptography.Enums;
using InsaneIO.Insane.Security;
using InsaneIO.Insane.Security.Enums;

TotpManager manager = TotpManager.FromSecret(
    secret: "insaneiosecret".ToByteArrayUtf8(),
    label: "user@example.com",
    issuer: "Insane IO",
    codeLength: TwoFactorCodeLength.SixDigits,
    hashAlgorithm: HashAlgorithm.Sha1,
    timePeriodInSeconds: 30);

string otpUri = manager.ToOtpUri();
string code = manager.ComputeCode();

bool currentWindow = manager.VerifyCode(code);
bool tolerant = manager.VerifyCode(code, TotpTimeWindowTolerance.OneWindow);

string json = manager.Serialize();
TotpManager restored = TotpManager.Deserialize(json);
```
