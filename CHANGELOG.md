# Changelog

## 10.2.0

Esta version agrega nuevas capacidades de API sin romper el funcionamiento existente. El foco principal es mejorar la deserializacion polimorfica, endurecer validaciones de payload y reforzar comparaciones criptograficas.

### IEncoder

Nuevas capacidades:

- Se agrega `IEncoder.DeserializeDynamic(string json)`.
- Permite deserializar un encoder concreto leyendo `AssemblyName` desde el JSON.
- Soporta reconstruccion dinamica de implementaciones como `HexEncoder`, `Base32Encoder` y `Base64Encoder` sin que el consumidor conozca el tipo concreto de antemano.

Mejoras relacionadas:

- `HexEncoder.Deserialize(string json)` ahora valida que el `AssemblyName` del payload corresponda realmente a `HexEncoder`.
- `Base32Encoder.Deserialize(string json)` ahora valida que el `AssemblyName` del payload corresponda realmente a `Base32Encoder`.
- `Base64Encoder.Deserialize(string json)` ahora valida que el `AssemblyName` del payload corresponda realmente a `Base64Encoder`.
- Los deserializadores de encoder ahora fallan temprano con `DeserializeException` si el JSON pertenece a otro tipo o le faltan campos requeridos.

### IHasher

Nuevas capacidades:

- Se agrega `IHasher.DeserializeDynamic(string json)`.
- Permite deserializar hashers concretos leyendo `AssemblyName` desde el JSON.
- Soporta reconstruccion dinamica de `ShaHasher`, `HmacHasher`, `ScryptHasher` y `Argon2Hasher`.

Mejoras relacionadas:

- Los `Deserialize(string json)` concretos validan el `AssemblyName` del payload antes de restaurar el objeto.
- `ShaHasher.Deserialize`, `HmacHasher.Deserialize`, `ScryptHasher.Deserialize` y `Argon2Hasher.Deserialize` usan validaciones mas estrictas y fallan con `DeserializeException` si el contenido no corresponde al tipo.
- `Argon2Hasher.Deserialize` elimina la reconstruccion redundante del `Encoder`.
- Las verificaciones de `HashExtensions` ahora usan comparacion en tiempo constante para:
  - `VerifyHash`
  - `VerifyHashFromEncoded`
  - `VerifyHmac`
  - `VerifyHmacFromEncoded`
  - `VerifyScrypt`
  - `VerifyScryptFromEncoded`
  - `VerifyArgon2`
  - `VerifyArgon2FromEncoded`

Impacto:

- Se reduce el riesgo de timing side channels al comparar valores derivados, hashes y HMACs esperados.

### IEncryptor

Nuevas capacidades:

- Se agrega `IEncryptor.DeserializeDynamic(string json)`.
- Permite deserializar encryptors concretos leyendo `AssemblyName` desde el JSON.
- Soporta reconstruccion dinamica de `AesCbcEncryptor` y `RsaEncryptor`.

Mejoras relacionadas:

- `AesCbcEncryptor.Deserialize(string json)` valida el tipo raiz del payload antes de restaurar el objeto.
- `RsaEncryptor.Deserialize(string json)` valida el tipo raiz del payload antes de restaurar el objeto.
- Los encryptors restauran su `Encoder` usando deserializacion dinamica de `IEncoder`.
- `RsaEncryptor.Deserialize` reutiliza `RsaKeyPair.Deserialize` para reconstruir el par de claves de forma consistente.

### Serializacion y Deserializacion Auxiliar

#### RsaKeyPair

Mejoras:

- `RsaKeyPair.Deserialize(string json)` deja de depender de `JsonSerializer.Deserialize<RsaKeyPair>(json)` sin validacion.
- Ahora valida `AssemblyName`.
- Ahora rechaza payloads sin `PublicKey` ni `PrivateKey`.
- Falla temprano con `DeserializeException` cuando el contenido no representa un `RsaKeyPair` valido.

#### TotpManager

Mejoras:

- `TotpManager.Deserialize(string json)` ahora valida `AssemblyName`.
- Valida que `CodeLength` y `HashAlgorithm` correspondan a valores definidos del enum.
- Falla temprano con `DeserializeException` si el payload es de otro tipo o contiene enums invalidos.

### AES

Mejoras:

- `DecryptAesCbc(byte[] data, byte[] key, AesCbcPadding padding = ...)` ahora valida que el ciphertext tenga al menos 16 bytes para contener el IV.
- Si el valor esta truncado, falla temprano con un mensaje claro en lugar de delegar a una excepcion interna menos explicita de `Aes`.

### Compatibilidad

- No se eliminaron APIs existentes.
- Los metodos concretos `Deserialize(string json)` siguen disponibles.
- La nueva deserializacion dinamica es adicional y complementaria.
- El comportamiento valido existente se mantiene; los cambios endurecen principalmente casos de payload invalido, tipo incorrecto o comparacion sensible.

### Testing

Cobertura agregada o ampliada para:

- `IEncoder.DeserializeDynamic`
- `IHasher.DeserializeDynamic`
- `IEncryptor.DeserializeDynamic`
- rechazo de `AssemblyName` incorrecto en deserializadores concretos
- `TotpManager.Deserialize` con enums invalidos
- `DecryptAesCbc` con ciphertext mas corto que el IV
- round-trip de serializacion/deserializacion en encoders, hashers y encryptors
