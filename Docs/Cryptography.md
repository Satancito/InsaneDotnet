# Cryptography

Este documento describe el namespace `InsaneIO.Insane.Cryptography` y las extensiones relacionadas en `InsaneIO.Insane.Extensions`.

La libreria ofrece:

- Encoders para convertir bytes a texto: Base64, Base32 y Hex.
- Extensiones de encoding general para convertir strings y bytes con `System.Text.Encoding`.
- Hashes simples: MD5, SHA-1, SHA-256, SHA-384 y SHA-512.
- HMAC con los mismos algoritmos.
- Derivacion de claves con Scrypt y Argon2.
- Cifrado simetrico AES-CBC.
- Cifrado asimetrico RSA.
- Clases configurables y serializables: encryptors, hashers y encoders.
- Metodos de interoperabilidad con Blazor WebAssembly mediante `IJSRuntime`.

Namespaces usados normalmente:

```csharp
using InsaneIO.Insane.Cryptography;
using InsaneIO.Insane.Extensions;
```

## Conceptos Base

La API suele ofrecer dos estilos de uso.

El primer estilo usa extensiones directamente sobre `string` o `byte[]`:

```csharp
string hash = "Hello".ComputeHashEncoded(HexEncoder.DefaultInstance, HashAlgorithm.Sha256);
string encrypted = "Hello".EncryptAesCbcEncoded("12345678", Base64Encoder.DefaultInstance);
```

El segundo estilo usa clases configurables:

```csharp
var hasher = new ShaHasher
{
    HashAlgorithm = HashAlgorithm.Sha256,
    Encoder = HexEncoder.DefaultInstance
};

string hash = hasher.ComputeEncoded("Hello");
```

Las sobrecargas que reciben `string` convierten el texto a bytes UTF-8 antes de operar. Las sobrecargas que terminan en `Encoded` generan texto codificado con un `IEncoder`; las que contienen `FromEncoded` reciben texto ya codificado, lo decodifican con el `IEncoder` indicado y luego verifican o descifran.

En los metodos de extension especificos de algoritmo, el orden del nombre es `Accion + Algoritmo + Formato`: por ejemplo `ComputeHashEncoded`, `VerifyHmacFromEncoded`, `ComputeScryptEncoded`, `DecryptAesCbcFromEncoded`, `EncryptAesCbcEncoded` o `EncryptRsaEncoded`. Los contratos genericos mantienen nombres sin algoritmo, como `EncryptEncoded`, `DecryptEncoded`, `ComputeEncoded` y `VerifyEncoded`.

## Interfaces

### IBaseSerializable

Define la identidad serializable minima de un tipo.

| Miembro | Tipo | Descripcion |
| --- | --- | --- |
| `SelfType` | `static abstract Type` | Tipo concreto que implementa la interfaz. |
| `AssemblyName` | `string` | Nombre completo del tipo con assembly. Se usa para reconstruir tipos concretos al deserializar. |
| `GetName(Type implementationType)` | `static string` | Devuelve el nombre en formato `FullName, AssemblyName`. |

Ejemplo de valor de `AssemblyName`:

```text
InsaneIO.Insane.Cryptography.Base64Encoder, InsaneIO.Insane
```

### IJsonSerializable

Extiende `IBaseSerializable` para objetos convertibles a JSON.

| Miembro | Tipo | Descripcion |
| --- | --- | --- |
| `ToJsonObject()` | `JsonObject` | Construye la representacion JSON del objeto. |
| `Serialize(bool indented = false)` | `string` | Devuelve el JSON como texto. |
| `GetIndentOptions(bool indented)` | `static JsonSerializerOptions` | Devuelve opciones de serializacion con o sin indentacion. |

Parametro de `Serialize`:

| Parametro | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `indented` | `bool` | `false` | Si es `true`, genera JSON formateado con saltos e indentacion. |

### IEncoder

Contrato para codificadores de bytes a texto.

| Metodo | Retorno | Descripcion |
| --- | --- | --- |
| `Encode(byte[] data)` | `string` | Codifica bytes a texto. |
| `Encode(string data)` | `string` | Convierte el texto a UTF-8 y lo codifica. |
| `Decode(string data)` | `byte[]` | Decodifica el texto al arreglo original de bytes. |

Parametros:

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `data` en `Encode(byte[])` | `byte[]` | Bytes que seran representados como texto. |
| `data` en `Encode(string)` | `string` | Texto que primero se convierte a UTF-8. |
| `data` en `Decode(string)` | `string` | Texto en el formato esperado por el encoder. |

### IEncoderJsonSerializable

Extiende `IJsonSerializable` y exige un metodo estatico de deserializacion:

```csharp
public static abstract IEncoder Deserialize(string json);
```

### IEncryptor

Contrato comun para cifradores.

| Metodo | Retorno | Descripcion |
| --- | --- | --- |
| `Encrypt(byte[] data)` | `byte[]` | Cifra bytes y devuelve bytes cifrados. |
| `Encrypt(string data)` | `byte[]` | Convierte el texto a UTF-8 y cifra los bytes. |
| `EncryptEncoded(byte[] data)` | `string` | Cifra bytes y codifica el resultado con el encoder configurado. |
| `EncryptEncoded(string data)` | `string` | Convierte texto a UTF-8, cifra y codifica. |
| `Decrypt(byte[] data)` | `byte[]` | Descifra bytes cifrados. |
| `DecryptEncoded(string data)` | `byte[]` | Decodifica el texto cifrado con el encoder configurado y luego descifra. |

### IEncryptorJsonSerializable

Extiende `IJsonSerializable` y exige:

```csharp
public static abstract IEncryptor Deserialize(string json);
```

### IHasher

Contrato comun para hashers y derivadores de claves.

| Metodo | Retorno | Descripcion |
| --- | --- | --- |
| `Compute(byte[] data)` | `byte[]` | Calcula el hash/derivacion en bytes. |
| `Compute(string data)` | `byte[]` | Convierte texto a UTF-8 y calcula. |
| `ComputeEncoded(byte[] data)` | `string` | Calcula y codifica el resultado con el encoder configurado. |
| `ComputeEncoded(string data)` | `string` | Convierte texto a UTF-8, calcula y codifica. |
| `Verify(byte[] data, byte[] expected)` | `bool` | Recalcula y compara contra bytes esperados. |
| `Verify(string data, byte[] expected)` | `bool` | Convierte texto a UTF-8, recalcula y compara. |
| `VerifyEncoded(byte[] data, string expected)` | `bool` | Recalcula, codifica y compara contra texto esperado. |
| `VerifyEncoded(string data, string expected)` | `bool` | Convierte texto a UTF-8, recalcula, codifica y compara. |

### IHasherJsonSerializable

Extiende `IJsonSerializable` y exige:

```csharp
public static abstract IHasher Deserialize(string json);
```

### ISecureJsonSerializable

Contrato previsto para serializacion protegida con clave. Actualmente define la forma, pero no hay implementaciones en las clases revisadas.

| Metodo | Retorno | Descripcion |
| --- | --- | --- |
| `Serialize(byte[] serializeKey, bool indented = false)` | `string` | Serializa usando clave binaria. |
| `Serialize(string serializeKey, bool indented = false)` | `string` | Serializa usando clave textual. |
| `ToJsonObject(byte[] serializeKey)` | `JsonObject` | Crea JSON protegido usando clave binaria. |
| `ToJsonObject(string serializeKey)` | `JsonObject` | Crea JSON protegido usando clave textual. |

## EncodingExtensions

Clase: `InsaneIO.Insane.Extensions.EncodingExtensions`.

Estas extensiones convierten texto a bytes y bytes a texto usando `System.Text.Encoding`.

### ToByteArrayUtf8

```csharp
public static byte[] ToByteArrayUtf8(this string data)
```

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `data` | `string` | Texto a convertir con `Encoding.UTF8`. |

Retorna `byte[]` con los bytes UTF-8.

Ejemplo:

```csharp
byte[] bytes = "Hola".ToByteArrayUtf8();
```

### ToStringUtf8

```csharp
public static string ToStringUtf8(this byte[] utf8Bytes)
```

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `utf8Bytes` | `byte[]` | Bytes que representan texto UTF-8. |

Retorna `string`.

Ejemplo:

```csharp
string text = bytes.ToStringUtf8();
```

### ToByteArray

```csharp
public static byte[] ToByteArray(this string data, Encoding encoding)
```

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `data` | `string` | Texto a convertir. |
| `encoding` | `Encoding` | Encoding usado para obtener los bytes. |

Ejemplo:

```csharp
byte[] ascii = "ABC".ToByteArray(Encoding.ASCII);
```

### ToString

```csharp
public static string ToString(this byte[] encodedBytes, Encoding encoding)
```

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `encodedBytes` | `byte[]` | Bytes codificados. |
| `encoding` | `Encoding` | Encoding usado para interpretar los bytes. |

Ejemplo:

```csharp
string text = ascii.ToString(Encoding.ASCII);
```

## HexEncodingExtensions

Clase: `InsaneIO.Insane.Extensions.HexEncodingExtensions`.

### EncodeToHex

Sobrecargas:

```csharp
public static string EncodeToHex(this byte[] data, bool toUpper = false)
public static string EncodeToHex(this string data, bool toUpper = false)
```

| Parametro | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `data` | `byte[]` | Requerido | Bytes a codificar como hexadecimal. |
| `data` | `string` | Requerido | Texto que se convierte a UTF-8 antes de codificar. |
| `toUpper` | `bool` | `false` | Si es `true`, usa letras `A-F`; si es `false`, usa `a-f`. |

Retorna un string hexadecimal con dos caracteres por byte.

Ejemplo:

```csharp
string lower = "Hi".EncodeToHex();          // 4869
string upper = "Hi".EncodeToHex(true);      // 4869
string hex = new byte[] { 0x0a, 0xff }.EncodeToHex(true); // 0AFF
```

### DecodeFromHex

```csharp
public static byte[] DecodeFromHex(this string data)
```

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `data` | `string` | Texto hexadecimal. Debe tener longitud par porque se procesa en pares. |

Retorna los bytes decodificados.

Ejemplo:

```csharp
byte[] bytes = "4869".DecodeFromHex();
string text = bytes.ToStringUtf8(); // Hi
```

Notas:

- Si el texto contiene caracteres no hexadecimales, `Convert.ToByte(..., 16)` lanzara excepcion.
- Si la longitud es impar, lanza `ArgumentException` porque cada byte requiere dos caracteres hexadecimales.

## Base64EncodingExtensions

Clase: `InsaneIO.Insane.Extensions.Base64EncodingExtensions`.

Constantes:

| Constante | Valor | Uso |
| --- | --- | --- |
| `NoLineBreaks` | `0` | No inserta saltos de linea. |
| `MimeLineBreaksLength` | `76` | Longitud comun para MIME. |
| `PemLineBreaksLength` | `64` | Longitud comun para PEM. |

### InsertLineBreaks

```csharp
public static string InsertLineBreaks(this string data, uint lineBreaksLength = MimeLineBreaksLength)
```

| Parametro | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `data` | `string` | Requerido | Texto al que se insertaran saltos. |
| `lineBreaksLength` | `uint` | `76` | Cantidad de caracteres por linea. Si es `0`, devuelve el texto igual. |

Retorna el string con saltos de linea usando `Environment.NewLine`.

Ejemplo:

```csharp
string wrapped = base64.InsertLineBreaks(Base64EncodingExtensions.PemLineBreaksLength);
```

### EncodeToBase64

Sobrecargas:

```csharp
public static string EncodeToBase64(this byte[] data, uint lineBreaksLength = NoLineBreaks, bool removePadding = false)
public static string EncodeToBase64(this string data, uint lineBreaksLength = NoLineBreaks, bool removePadding = false)
```

| Parametro | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `data` | `byte[]` | Requerido | Bytes a codificar. |
| `data` | `string` | Requerido | Texto convertido a UTF-8 antes de codificar. |
| `lineBreaksLength` | `uint` | `0` | Longitud de linea. `0` significa sin saltos. |
| `removePadding` | `bool` | `false` | Si es `true`, elimina los caracteres `=` del final. |

Ejemplo:

```csharp
string base64 = "Hello".EncodeToBase64();
string pemBody = bytes.EncodeToBase64(Base64EncodingExtensions.PemLineBreaksLength);
string noPadding = "Hello".EncodeToBase64(removePadding: true);
```

### EncodeToUrlSafeBase64

```csharp
public static string EncodeToUrlSafeBase64(this byte[] data)
```

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `data` | `byte[]` | Bytes a codificar. |

Convierte Base64 normal a variante segura para URL:

- `+` pasa a `-`.
- `/` pasa a `_`.
- `=` se elimina.

Ejemplo:

```csharp
string token = RandomExtensions.NextBytes(32).EncodeToUrlSafeBase64();
```

### EncodeToFilenameSafeBase64

```csharp
public static string EncodeToFilenameSafeBase64(this byte[] data)
```

Alias de `EncodeToUrlSafeBase64`.

Ejemplo:

```csharp
string filenamePart = bytes.EncodeToFilenameSafeBase64();
```

### EncodeToUrlEncodedBase64

```csharp
public static string EncodeToUrlEncodedBase64(this byte[] data)
```

Codifica en Base64 y reemplaza:

- `+` por `%2B`.
- `/` por `%2F`.
- `=` por `%3D`.

Ejemplo:

```csharp
string queryValue = bytes.EncodeToUrlEncodedBase64();
```

### DecodeFromBase64

```csharp
public static byte[] DecodeFromBase64(this string data)
```

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `data` | `string` | Texto en Base64 normal, sin padding, URL-safe, filename-safe, URL-encoded, o con saltos de linea. |

Comportamiento:

- Aplica `Trim()`.
- Convierte `%2B`, `%2F`, `%3D` a `+`, `/`, `=`.
- Convierte `-`, `_` a `+`, `/`.
- Elimina `\n` y `\r`.
- Rellena padding `=` hasta que la longitud sea multiplo de 4.
- Usa `Convert.FromBase64String`.

Ejemplo:

```csharp
byte[] decoded = token.DecodeFromBase64();
```

### EncodeBase64ToUrlSafeBase64

```csharp
public static string EncodeBase64ToUrlSafeBase64(this string base64)
```

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `base64` | `string` | Texto Base64 existente. |

Decodifica el Base64 y lo vuelve a codificar en variante URL-safe.

### EncodeBase64ToFilenameSafeBase64

```csharp
public static string EncodeBase64ToFilenameSafeBase64(this string base64)
```

Igual que el anterior, pero con alias filename-safe.

### EncodeBase64ToUrlEncodedBase64

```csharp
public static string EncodeBase64ToUrlEncodedBase64(this string base64)
```

Convierte un Base64 existente a variante URL-encoded.

## Base32EncodingExtensions

Clase: `InsaneIO.Insane.Extensions.Base32EncodingExtensions`.

### EncodeToBase32

Sobrecargas:

```csharp
public static string EncodeToBase32(this byte[] data, bool removePadding = false, bool toLower = false)
public static string EncodeToBase32(this string data, bool removePadding = false, bool toLower = false)
```

| Parametro | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `data` | `byte[]` | Requerido | Bytes a codificar. |
| `data` | `string` | Requerido | Texto convertido a UTF-8 antes de codificar. |
| `removePadding` | `bool` | `false` | Si es `true`, no agrega `=` para completar bloques. |
| `toLower` | `bool` | `false` | Si es `true`, usa letras minusculas. |

Retorna texto Base32.

Ejemplo:

```csharp
string base32 = "Hello".EncodeToBase32();
string compact = "Hello".EncodeToBase32(removePadding: true, toLower: true);
```

### DecodeFromBase32

```csharp
public static byte[] DecodeFromBase32(this string data)
```

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `data` | `string` | Texto Base32. Puede venir con padding `=` al final. |

Comportamiento:

- Aplica `Trim()`.
- Acepta valores con padding final `=` o sin padding.
- Valida que el padding este solo al final y tenga una longitud posible para Base32.
- Valida que la longitud sin padding no sea un residuo imposible de Base32.
- Acepta letras `A-Z`, `a-z` y digitos `2-7`.
- Lanza `ArgumentException` si encuentra un caracter fuera de Base32.

Ejemplo:

```csharp
byte[] decoded = base32.DecodeFromBase32();
string text = decoded.ToStringUtf8();
```

## Encoders

Los encoders implementan `IEncoder` y `IEncoderJsonSerializable`. Se usan solos o como dependencia de encryptors y hashers.

### HexEncoder

Clase: `InsaneIO.Insane.Cryptography.HexEncoder`.

Propiedades:

| Propiedad | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `ToUpper` | `bool` | `false` | Controla si `Encode` usa letras hexadecimales en mayusculas. |
| `AssemblyName` | `string` | Calculado | Identificador del tipo para serializacion. |
| `DefaultInstance` | `HexEncoder` | Instancia estatica | Instancia reutilizable con configuracion default. |

Metodos:

| Metodo | Retorno | Descripcion |
| --- | --- | --- |
| `Encode(byte[] data)` | `string` | Llama a `EncodeToHex(ToUpper)`. |
| `Encode(string data)` | `string` | Convierte a UTF-8 y codifica. |
| `Decode(string data)` | `byte[]` | Llama a `DecodeFromHex()`. |
| `ToJsonObject()` | `JsonObject` | Incluye `AssemblyName` y `ToUpper`. |
| `Serialize(bool indented = false)` | `string` | Serializa la configuracion. |
| `Deserialize(string json)` | `IEncoder` | Restaura un `HexEncoder` desde JSON. |

Ejemplo:

```csharp
var encoder = new HexEncoder { ToUpper = true };

string encoded = encoder.Encode("Hello");
byte[] decoded = encoder.Decode(encoded);
string json = encoder.Serialize(indented: true);
IEncoder restored = HexEncoder.Deserialize(json);
```

### Base32Encoder

Clase: `InsaneIO.Insane.Cryptography.Base32Encoder`.

Propiedades:

| Propiedad | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `ToLower` | `bool` | `false` | Produce letras minusculas al codificar. |
| `RemovePadding` | `bool` | `false` | Omite padding `=` si es `true`. |
| `AssemblyName` | `string` | Calculado | Identificador del tipo para serializacion. |
| `DefaultInstance` | `Base32Encoder` | Instancia estatica | Instancia reutilizable con configuracion default. |

Ejemplo:

```csharp
var encoder = new Base32Encoder
{
    RemovePadding = true,
    ToLower = true
};

string encoded = encoder.Encode("Hello");
string decoded = encoder.Decode(encoded).ToStringUtf8();
```

Serializacion:

```csharp
string json = encoder.Serialize(indented: true);
IEncoder restored = Base32Encoder.Deserialize(json);
```

### Base64Encoder

Clase: `InsaneIO.Insane.Cryptography.Base64Encoder`.

Constantes:

| Constante | Valor | Descripcion |
| --- | --- | --- |
| `NoLineBreaks` | `0` | No inserta saltos. |
| `MimeLineBreaksLength` | `76` | Lineas MIME. |
| `PemLineBreaksLength` | `64` | Lineas PEM. |

Propiedades:

| Propiedad | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `LineBreaksLength` | `uint` | `0` | Longitud de linea al codificar Base64 normal. |
| `RemovePadding` | `bool` | `false` | Elimina `=` cuando `EncodingType` es `Base64`. |
| `EncodingType` | `Base64Encoding` | `Base64` | Variante Base64 usada para codificar. |
| `AssemblyName` | `string` | Calculado | Identificador del tipo para serializacion. |
| `DefaultInstance` | `Base64Encoder` | Instancia estatica | Instancia reutilizable con configuracion default. |

`Base64Encoding`:

| Valor | Descripcion |
| --- | --- |
| `Base64` | Base64 normal. Respeta `LineBreaksLength` y `RemovePadding`. |
| `UrlSafeBase64` | Reemplaza `+` por `-`, `/` por `_` y elimina `=`. |
| `FileNameSafeBase64` | Alias de URL-safe. |
| `UrlEncodedBase64` | Reemplaza `+`, `/`, `=` por `%2B`, `%2F`, `%3D`. |

Ejemplo:

```csharp
var encoder = new Base64Encoder
{
    EncodingType = Base64Encoding.UrlSafeBase64
};

string encoded = encoder.Encode(RandomExtensions.NextBytes(32));
byte[] decoded = encoder.Decode(encoded);
```

Ejemplo PEM:

```csharp
var pemEncoder = new Base64Encoder
{
    EncodingType = Base64Encoding.Base64,
    LineBreaksLength = Base64Encoder.PemLineBreaksLength
};

string pemBody = pemEncoder.Encode(bytes);
```

Serializacion:

```csharp
string json = encoder.Serialize(indented: true);
IEncoder restored = Base64Encoder.Deserialize(json);
```

## AesExtensions

Clase: `InsaneIO.Insane.Extensions.AesExtensions`.

Constantes:

| Constante | Valor | Descripcion |
| --- | --- | --- |
| `MaxIvLength` | `16` | Tamano del IV AES usado al extraerlo del paquete cifrado. |
| `MaxKeyLength` | `32` | Tamano de clave AES usado despues de normalizar la clave. |

### Funcionamiento interno

Antes de cifrar o descifrar:

1. La clave se valida.
2. Debe tener al menos 8 bytes.
3. Se calcula SHA-512 sobre la clave original.
4. Se toman los primeros 32 bytes como clave AES.

Al cifrar:

1. Se crea una instancia `Aes`.
2. Se asigna el padding elegido.
3. Se genera un IV aleatorio.
4. Se cifra el payload.
5. Se concatena `ciphertext || iv`.

Al descifrar:

1. Se toman los ultimos 16 bytes como IV.
2. El resto se trata como ciphertext.
3. Se descifra usando la misma normalizacion de clave y padding.

Esto importa para interoperabilidad: otras librerias pueden guardar el IV al inicio o por separado.

### AesCbcPadding

| Valor | Padding .NET usado | Notas |
| --- | --- | --- |
| `None` | `PaddingMode.None` | El input debe ser multiplo del bloque AES. |
| `Zeros` | `PaddingMode.Zeros` | Rellena con ceros. Puede ser ambiguo si el texto original termina en ceros. |
| `Pkcs7` | `PaddingMode.PKCS7` | Default recomendado para datos generales. |
| `AnsiX923` | `PaddingMode.ANSIX923` | Padding ANSI X.923. |

### EncryptAesCbc

Sobrecargas:

```csharp
public static byte[] EncryptAesCbc(this byte[] data, byte[] key, AesCbcPadding padding = AesCbcPadding.Pkcs7)
public static byte[] EncryptAesCbc(this byte[] data, string key, AesCbcPadding padding = AesCbcPadding.Pkcs7)
public static byte[] EncryptAesCbc(this string data, byte[] key, AesCbcPadding padding = AesCbcPadding.Pkcs7)
public static byte[] EncryptAesCbc(this string data, string key, AesCbcPadding padding = AesCbcPadding.Pkcs7)
```

| Parametro | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `data` | `byte[]` | Requerido | Bytes a cifrar. |
| `data` | `string` | Requerido | Texto convertido a UTF-8 antes de cifrar. |
| `key` | `byte[]` | Requerido | Clave original. Debe tener al menos 8 bytes. |
| `key` | `string` | Requerido | Texto convertido a UTF-8 para usar como clave original. |
| `padding` | `AesCbcPadding` | `Pkcs7` | Padding usado por AES. |

Retorna bytes en formato `ciphertext || iv`.

Ejemplo:

```csharp
byte[] encrypted = "Hello".EncryptAesCbc("12345678", AesCbcPadding.Pkcs7);
byte[] decrypted = encrypted.DecryptAesCbc("12345678", AesCbcPadding.Pkcs7);
string text = decrypted.ToStringUtf8();
```

### DecryptAesCbc

Sobrecargas:

```csharp
public static byte[] DecryptAesCbc(this byte[] data, byte[] key, AesCbcPadding padding = AesCbcPadding.Pkcs7)
public static byte[] DecryptAesCbc(this byte[] data, string key, AesCbcPadding padding = AesCbcPadding.Pkcs7)
```

| Parametro | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `data` | `byte[]` | Requerido | Bytes cifrados en formato `ciphertext || iv`. |
| `key` | `byte[]` o `string` | Requerido | Misma clave original usada al cifrar. |
| `padding` | `AesCbcPadding` | `Pkcs7` | Mismo padding usado al cifrar. |

Retorna bytes descifrados.

### EncryptAesCbcEncoded

Sobrecargas:

```csharp
public static string EncryptAesCbcEncoded(this byte[] data, byte[] key, IEncoder encoder, AesCbcPadding padding = AesCbcPadding.Pkcs7)
public static string EncryptAesCbcEncoded(this string data, string key, IEncoder encoder, AesCbcPadding padding = AesCbcPadding.Pkcs7)
public static string EncryptAesCbcEncoded(this string data, byte[] key, IEncoder encoder, AesCbcPadding padding = AesCbcPadding.Pkcs7)
public static string EncryptAesCbcEncoded(this byte[] data, string key, IEncoder encoder, AesCbcPadding padding = AesCbcPadding.Pkcs7)
```

| Parametro | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `data` | `byte[]` o `string` | Requerido | Payload a cifrar. Si es string, se convierte a UTF-8. |
| `key` | `byte[]` o `string` | Requerido | Clave original. Si es string, se convierte a UTF-8. |
| `encoder` | `IEncoder` | Requerido | Encoder usado para convertir bytes cifrados a texto. |
| `padding` | `AesCbcPadding` | `Pkcs7` | Padding AES. |

Ejemplo:

```csharp
string encrypted = "Hello".EncryptAesCbcEncoded(
    "12345678",
    Base64Encoder.DefaultInstance,
    AesCbcPadding.Pkcs7);
```

### DecryptAesCbcFromEncoded

Sobrecargas:

```csharp
public static byte[] DecryptAesCbcFromEncoded(this string data, byte[] key, IEncoder encoder, AesCbcPadding padding = AesCbcPadding.Pkcs7)
public static byte[] DecryptAesCbcFromEncoded(this string data, string key, IEncoder encoder, AesCbcPadding padding = AesCbcPadding.Pkcs7)
```

| Parametro | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `data` | `string` | Requerido | Texto cifrado codificado por `encoder`. |
| `key` | `byte[]` o `string` | Requerido | Misma clave usada al cifrar. |
| `encoder` | `IEncoder` | Requerido | Encoder usado para decodificar `data`. |
| `padding` | `AesCbcPadding` | `Pkcs7` | Mismo padding usado al cifrar. |

Ejemplo:

```csharp
string decrypted = encrypted
    .DecryptAesCbcFromEncoded("12345678", Base64Encoder.DefaultInstance)
    .ToStringUtf8();
```

### AesExtensions con IJSRuntime

Metodos:

```csharp
EncryptAesCbcAsync(this IJSRuntime js, byte[] data, byte[] key, AesCbcPadding padding = AesCbcPadding.Pkcs7)
DecryptAesCbcAsync(this IJSRuntime js, byte[] data, byte[] key, AesCbcPadding padding = AesCbcPadding.Pkcs7)
EncryptAesCbcAsync(this IJSRuntime js, string data, string key, AesCbcPadding padding = AesCbcPadding.Pkcs7)
DecryptAesCbcAsync(this IJSRuntime js, byte[] data, string key, AesCbcPadding padding = AesCbcPadding.Pkcs7)
EncryptAesCbcEncodedAsync(this IJSRuntime js, byte[] data, byte[] key, IEncoder encoder, AesCbcPadding padding = AesCbcPadding.Pkcs7)
EncryptAesCbcEncodedAsync(this IJSRuntime js, string data, string key, IEncoder encoder, AesCbcPadding padding = AesCbcPadding.Pkcs7)
EncryptAesCbcEncodedAsync(this IJSRuntime js, string data, byte[] key, IEncoder encoder, AesCbcPadding padding = AesCbcPadding.Pkcs7)
EncryptAesCbcEncodedAsync(this IJSRuntime js, byte[] data, string key, IEncoder encoder, AesCbcPadding padding = AesCbcPadding.Pkcs7)
DecryptAesCbcFromEncodedAsync(this IJSRuntime js, string data, byte[] key, IEncoder encoder, AesCbcPadding padding = AesCbcPadding.Pkcs7)
DecryptAesCbcFromEncodedAsync(this IJSRuntime js, string data, string key, IEncoder encoder, AesCbcPadding padding = AesCbcPadding.Pkcs7)
```

Parametros adicionales:

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `js` | `IJSRuntime` | Runtime JavaScript de Blazor. |

Estos metodos generan una funcion JS temporal con nombre aleatorio, llaman a la implementacion WebAssembly de `Insane`, convierten arreglos JS a vectores internos y eliminan la funcion temporal.

Ejemplo:

```csharp
string encrypted = await js.EncryptAesCbcEncodedAsync(
    "Hello",
    "12345678",
    Base64Encoder.DefaultInstance,
    AesCbcPadding.AnsiX923);
```

## AesCbcEncryptor

Clase: `InsaneIO.Insane.Cryptography.AesCbcEncryptor`.

Implementa `IEncryptor`.

Propiedades:

| Propiedad | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `KeyString` | `string` | Generada | Setter: convierte el string a UTF-8 y lo guarda como clave. Getter: devuelve la clave codificada con `Encoder`. |
| `KeyBytes` | `byte[]` | 32 bytes aleatorios | Setter/getter para clave binaria. |
| `Encoder` | `IEncoder` | `Base64Encoder.DefaultInstance` | Encoder usado por `EncryptEncoded`, `DecryptEncoded` y serializacion de clave. |
| `Padding` | `AesCbcPadding` | `Pkcs7` | Padding AES-CBC. |
| `AssemblyName` | `string` | Calculado | Identificador para serializacion. |

Metodos:

| Metodo | Retorno | Descripcion |
| --- | --- | --- |
| `Encrypt(byte[] data)` | `byte[]` | Cifra bytes usando `Key` y `Padding`. |
| `Encrypt(string data)` | `byte[]` | Convierte a UTF-8 y cifra. |
| `EncryptEncoded(byte[] data)` | `string` | Cifra y codifica con `Encoder`. |
| `EncryptEncoded(string data)` | `string` | Convierte a UTF-8, cifra y codifica. |
| `Decrypt(byte[] data)` | `byte[]` | Descifra bytes en formato `ciphertext || iv`. |
| `DecryptEncoded(string data)` | `byte[]` | Decodifica con `Encoder` y descifra. |
| `ToJsonObject()` | `JsonObject` | Serializa `AssemblyName`, `Key`, `Padding` y `Encoder`. |
| `Serialize(bool indented = false)` | `string` | Devuelve JSON. |
| `Deserialize(string json)` | `IEncryptor` | Restaura el encryptor desde JSON. |

Ejemplo:

```csharp
var encryptor = new AesCbcEncryptor
{
    KeyString = "12345678",
    Padding = AesCbcPadding.Pkcs7,
    Encoder = Base64Encoder.DefaultInstance
};

string encrypted = encryptor.EncryptEncoded("Hello World");
string decrypted = encryptor.DecryptEncoded(encrypted).ToStringUtf8();
```

Serializacion:

```csharp
string json = encryptor.Serialize(indented: true);
IEncryptor restored = AesCbcEncryptor.Deserialize(json);
string decrypted = restored.DecryptEncoded(encrypted).ToStringUtf8();
```

Punto de seguridad:

- El JSON contiene la clave AES codificada. Debe tratarse como secreto.

## RsaExtensions

Clase: `InsaneIO.Insane.Extensions.RsaExtensions`.

### Tipos de RSA

`RsaKeyPairEncoding` controla como se exporta un par de claves:

| Valor | Descripcion |
| --- | --- |
| `Ber` | Exporta `SubjectPublicKeyInfo` y `Pkcs8PrivateKey` como Base64 sin headers. |
| `Pem` | Exporta `BEGIN PUBLIC KEY` y `BEGIN PRIVATE KEY`. |
| `Xml` | Exporta XML compatible con `RSA.ToXmlString`. |

`RsaKeyEncoding` describe el formato detectado:

| Valor | Descripcion |
| --- | --- |
| `Unknown` | Formato no reconocido o importacion fallida. |
| `BerPublic` | Clave publica BER/Base64. |
| `BerPrivate` | Clave privada PKCS#8/Base64. |
| `PemPublic` | Clave publica PEM `BEGIN PUBLIC KEY`. |
| `PemPrivate` | Clave privada PEM `BEGIN PRIVATE KEY`. |
| `XmlPublic` | Clave publica XML. |
| `XmlPrivate` | Clave privada XML. |

`RsaPadding`:

| Valor | Padding .NET |
| --- | --- |
| `Pkcs1` | `RSAEncryptionPadding.Pkcs1` |
| `OaepSha1` | `RSAEncryptionPadding.OaepSHA1` |
| `OaepSha256` | `RSAEncryptionPadding.OaepSHA256` |
| `OaepSha384` | `RSAEncryptionPadding.OaepSHA384` |
| `OaepSha512` | `RSAEncryptionPadding.OaepSHA512` |

### CreateRsaKeyPair

```csharp
public static RsaKeyPair CreateRsaKeyPair(this uint keySize, RsaKeyPairEncoding encoding = RsaKeyPairEncoding.Ber)
```

| Parametro | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `keySize` | `uint` | Requerido | Tamano de clave RSA en bits. Ejemplos: `2048`, `3072`, `4096`. |
| `encoding` | `RsaKeyPairEncoding` | `Ber` | Formato de salida del par de claves. |

Retorna `RsaKeyPair` con `PublicKey` y `PrivateKey`.

Ejemplo:

```csharp
RsaKeyPair keyPair = 2048u.CreateRsaKeyPair(RsaKeyPairEncoding.Pem);
```

### GetRsaKeyEncoding

```csharp
public static RsaKeyEncoding GetRsaKeyEncoding(this string key)
```

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `key` | `string` | Clave RSA en BER/Base64, PEM o XML. |

Retorna el formato detectado o `Unknown`.

Ejemplo:

```csharp
RsaKeyEncoding encoding = keyPair.PublicKey!.GetRsaKeyEncoding();
```

### ValidateRsaPublicKey

```csharp
public static bool ValidateRsaPublicKey(this string publicKey)
```

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `publicKey` | `string` | Clave publica a validar. |

Retorna `true` si la clave puede reconocerse e importarse como publica.

### ValidateRsaPrivateKey

```csharp
public static bool ValidateRsaPrivateKey(this string privateKey)
```

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `privateKey` | `string` | Clave privada a validar. |

Retorna `true` si la clave puede reconocerse e importarse como privada.

Ejemplo:

```csharp
bool publicOk = keyPair.PublicKey!.ValidateRsaPublicKey();
bool privateOk = keyPair.PrivateKey!.ValidateRsaPrivateKey();
```

### EncryptRsa

Sobrecargas:

```csharp
public static byte[] EncryptRsa(this byte[] data, string publicKey, RsaPadding padding = RsaPadding.OaepSha256)
public static byte[] EncryptRsa(this string data, string publicKey, RsaPadding padding = RsaPadding.OaepSha256)
```

| Parametro | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `data` | `byte[]` | Requerido | Bytes a cifrar. |
| `data` | `string` | Requerido | Texto convertido a UTF-8 antes de cifrar. |
| `publicKey` | `string` | Requerido | Clave publica en formato soportado. |
| `padding` | `RsaPadding` | `OaepSha256` | Padding RSA. Debe coincidir al descifrar. |

Retorna bytes cifrados.

Ejemplo:

```csharp
byte[] encrypted = "Hello".EncryptRsa(keyPair.PublicKey!, RsaPadding.OaepSha256);
```

### EncryptRsaEncoded

Sobrecargas:

```csharp
public static string EncryptRsaEncoded(this byte[] data, string publicKey, IEncoder encoder, RsaPadding padding = RsaPadding.OaepSha256)
public static string EncryptRsaEncoded(this string data, string publicKey, IEncoder encoder, RsaPadding padding = RsaPadding.OaepSha256)
```

| Parametro | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `data` | `byte[]` o `string` | Requerido | Payload a cifrar. |
| `publicKey` | `string` | Requerido | Clave publica. |
| `encoder` | `IEncoder` | Requerido | Encoder usado para representar el ciphertext como texto. |
| `padding` | `RsaPadding` | `OaepSha256` | Padding RSA. |

Ejemplo:

```csharp
string encrypted = "Hello".EncryptRsaEncoded(
    keyPair.PublicKey!,
    Base64Encoder.DefaultInstance,
    RsaPadding.OaepSha256);
```

### DecryptRsa

```csharp
public static byte[] DecryptRsa(this byte[] data, string privateKey, RsaPadding padding = RsaPadding.OaepSha256)
```

| Parametro | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `data` | `byte[]` | Requerido | Bytes cifrados. |
| `privateKey` | `string` | Requerido | Clave privada en formato soportado. |
| `padding` | `RsaPadding` | `OaepSha256` | Padding usado al cifrar. |

Retorna bytes descifrados.

### DecryptRsaFromEncoded

```csharp
public static byte[] DecryptRsaFromEncoded(this string data, string privateKey, IEncoder encoder, RsaPadding padding = RsaPadding.OaepSha256)
```

| Parametro | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `data` | `string` | Requerido | Ciphertext codificado. |
| `privateKey` | `string` | Requerido | Clave privada. |
| `encoder` | `IEncoder` | Requerido | Encoder usado para decodificar el ciphertext. |
| `padding` | `RsaPadding` | `OaepSha256` | Padding usado al cifrar. |

Ejemplo completo:

```csharp
RsaKeyPair keyPair = 2048u.CreateRsaKeyPair(RsaKeyPairEncoding.Pem);

string encrypted = "Hello from RSA".EncryptRsaEncoded(
    keyPair.PublicKey!,
    Base64Encoder.DefaultInstance,
    RsaPadding.OaepSha256);

string decrypted = encrypted
    .DecryptRsaFromEncoded(keyPair.PrivateKey!, Base64Encoder.DefaultInstance, RsaPadding.OaepSha256)
    .ToStringUtf8();
```

### RsaExtensions con IJSRuntime

Metodos:

```csharp
CreateRsaKeyPairAsync(this IJSRuntime js, uint keySize, RsaKeyPairEncoding encoding = RsaKeyPairEncoding.Ber)
ValidateRsaPublicKeyAsync(this IJSRuntime js, string publicKey)
ValidateRsaPrivateKeyAsync(this IJSRuntime js, string privateKey)
GetRsaKeyEncodingAsync(this IJSRuntime js, string key)
EncryptRsaAsync(this IJSRuntime js, byte[] data, string publicKey, RsaPadding padding = RsaPadding.OaepSha256)
EncryptRsaAsync(this IJSRuntime js, string data, string publicKey, RsaPadding padding = RsaPadding.OaepSha256)
EncryptRsaEncodedAsync(this IJSRuntime js, byte[] data, string publicKey, IEncoder encoder, RsaPadding padding = RsaPadding.OaepSha256)
EncryptRsaEncodedAsync(this IJSRuntime js, string data, string publicKey, IEncoder encoder, RsaPadding padding = RsaPadding.OaepSha256)
DecryptRsaAsync(this IJSRuntime js, byte[] data, string privateKey, RsaPadding padding = RsaPadding.OaepSha256)
DecryptRsaFromEncodedAsync(this IJSRuntime js, string data, string privateKey, IEncoder encoder, RsaPadding padding = RsaPadding.OaepSha256)
```

Parametro adicional:

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `js` | `IJSRuntime` | Runtime JavaScript de Blazor. |

Ejemplo:

```csharp
RsaKeyPair keyPair = await js.CreateRsaKeyPairAsync(2048, RsaKeyPairEncoding.Pem);
string encrypted = await js.EncryptRsaEncodedAsync(
    "Hello",
    keyPair.PublicKey!,
    Base64Encoder.DefaultInstance,
    RsaPadding.OaepSha256);
```

Notas:

- RSA no es adecuado para cifrar payloads grandes directamente. Para datos grandes, usa cifrado hibrido: AES para el payload y RSA para la clave AES.
- El parser actual importa PEM `BEGIN PUBLIC KEY` y `BEGIN PRIVATE KEY`. Aunque existen constantes para headers `BEGIN RSA PUBLIC KEY` y `BEGIN RSA PRIVATE KEY`, la logica actual no los importa.

## RsaKeyPair

Clase: `InsaneIO.Insane.Cryptography.RsaKeyPair`.

Propiedades:

| Propiedad | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `PublicKey` | `string?` | `null` | Clave publica serializada. |
| `PrivateKey` | `string?` | `null` | Clave privada serializada. |
| `AssemblyName` | `string` | Calculado | Identificador del tipo para JSON. |

Metodos:

| Metodo | Retorno | Descripcion |
| --- | --- | --- |
| `Serialize(bool indented = false)` | `string` | Serializa el par a JSON. |
| `ToJsonObject()` | `JsonObject` | Crea JSON con `AssemblyName`, `PublicKey` y `PrivateKey`. |
| `Deserialize(string json)` | `RsaKeyPair?` | Restaura desde JSON. |

Ejemplo:

```csharp
RsaKeyPair keyPair = 2048u.CreateRsaKeyPair(RsaKeyPairEncoding.Ber);
string json = keyPair.Serialize(indented: true);
RsaKeyPair? restored = RsaKeyPair.Deserialize(json);
```

Punto de seguridad:

- Si `PrivateKey` esta presente, el JSON contiene material privado.

## RsaEncryptor

Clase: `InsaneIO.Insane.Cryptography.RsaEncryptor`.

Implementa `IEncryptor`.

Propiedades:

| Propiedad | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `KeyPair` | `RsaKeyPair` | Requerido | Contiene clave publica para cifrar y clave privada para descifrar. |
| `Padding` | `RsaPadding` | `OaepSha256` | Padding usado en cifrado y descifrado. |
| `Encoder` | `IEncoder` | `Base64Encoder.DefaultInstance` | Encoder usado por metodos encoded y serializacion. |
| `AssemblyName` | `string` | Calculado | Identificador para serializacion. |

Metodos:

| Metodo | Retorno | Descripcion |
| --- | --- | --- |
| `Encrypt(byte[] data)` | `byte[]` | Cifra con `KeyPair.PublicKey`. |
| `Encrypt(string data)` | `byte[]` | Convierte a UTF-8 y cifra. |
| `EncryptEncoded(byte[] data)` | `string` | Cifra y codifica. |
| `EncryptEncoded(string data)` | `string` | Convierte a UTF-8, cifra y codifica. |
| `Decrypt(byte[] data)` | `byte[]` | Descifra con `KeyPair.PrivateKey`. |
| `DecryptEncoded(string data)` | `byte[]` | Decodifica y descifra. |
| `Serialize(bool indented = false)` | `string` | Serializa configuracion y claves. |
| `ToJsonObject()` | `JsonObject` | Crea JSON. |
| `Deserialize(string json)` | `IEncryptor` | Restaura desde JSON. |

Ejemplo:

```csharp
var encryptor = new RsaEncryptor
{
    KeyPair = 2048u.CreateRsaKeyPair(RsaKeyPairEncoding.Pem),
    Padding = RsaPadding.OaepSha256,
    Encoder = Base64Encoder.DefaultInstance
};

string encrypted = encryptor.EncryptEncoded("Hello");
string decrypted = encryptor.DecryptEncoded(encrypted).ToStringUtf8();
```

Serializacion:

```csharp
string json = encryptor.Serialize(indented: true);
IEncryptor restored = RsaEncryptor.Deserialize(json);
```

Punto de seguridad:

- El JSON puede contener clave privada. Protege ese contenido.

## HashExtensions

Clase: `InsaneIO.Insane.Extensions.HashExtensions`.

### HashAlgorithm

| Valor | Tamano esperado |
| --- | --- |
| `Md5` | 16 bytes |
| `Sha1` | 20 bytes |
| `Sha256` | 32 bytes |
| `Sha384` | 48 bytes |
| `Sha512` | 64 bytes |

### ComputeHash

Sobrecargas:

```csharp
public static byte[] ComputeHash(this byte[] data, HashAlgorithm algorithm = HashAlgorithm.Sha512)
public static byte[] ComputeHash(this string data, HashAlgorithm algorithm = HashAlgorithm.Sha512)
```

| Parametro | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `data` | `byte[]` o `string` | Requerido | Datos a hashear. Si es string, se convierte a UTF-8. |
| `algorithm` | `HashAlgorithm` | `Sha512` | Algoritmo de hash. |

Ejemplo:

```csharp
byte[] hash = "Hello".ComputeHash(HashAlgorithm.Sha256);
```

### ComputeHashEncoded

```csharp
public static string ComputeHashEncoded(this byte[] data, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
public static string ComputeHashEncoded(this string data, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
```

| Parametro | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `data` | `byte[]` o `string` | Requerido | Datos a hashear. |
| `encoder` | `IEncoder` | Requerido | Encoder para representar el hash. |
| `algorithm` | `HashAlgorithm` | `Sha512` | Algoritmo de hash. |

Ejemplo:

```csharp
string hash = "Hello".ComputeHashEncoded(HexEncoder.DefaultInstance, HashAlgorithm.Sha256);
```

### VerifyHash y VerifyHashFromEncoded

```csharp
public static bool VerifyHash(this byte[] data, byte[] expected, HashAlgorithm algorithm = HashAlgorithm.Sha512)
public static bool VerifyHash(this string data, byte[] expected, HashAlgorithm algorithm = HashAlgorithm.Sha512)
public static bool VerifyHashFromEncoded(this byte[] data, string expected, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
public static bool VerifyHashFromEncoded(this string data, string expected, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
```

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `data` | `byte[]` o `string` | Datos originales. |
| `expected` | `byte[]` o `string` | Hash esperado, en bytes o codificado. |
| `encoder` | `IEncoder` | Encoder usado para comparar texto. |
| `algorithm` | `HashAlgorithm` | Algoritmo usado para recalcular. |

Ejemplo:

```csharp
bool ok = "Hello".VerifyHashFromEncoded(hash, HexEncoder.DefaultInstance, HashAlgorithm.Sha256);
```

### ComputeHmac

Sobrecargas:

```csharp
public static byte[] ComputeHmac(this byte[] data, byte[] key, HashAlgorithm algorithm = HashAlgorithm.Sha512)
public static byte[] ComputeHmac(this string data, byte[] key, HashAlgorithm algorithm = HashAlgorithm.Sha512)
public static byte[] ComputeHmac(this string data, string key, HashAlgorithm algorithm = HashAlgorithm.Sha512)
public static byte[] ComputeHmac(this byte[] data, string key, HashAlgorithm algorithm = HashAlgorithm.Sha512)
```

| Parametro | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `data` | `byte[]` o `string` | Requerido | Datos a autenticar. |
| `key` | `byte[]` o `string` | Requerido | Clave secreta. Si es string, se convierte a UTF-8. |
| `algorithm` | `HashAlgorithm` | `Sha512` | Algoritmo HMAC. |

Ejemplo:

```csharp
byte[] mac = "payload".ComputeHmac("secret", HashAlgorithm.Sha256);
```

### ComputeHmacEncoded

```csharp
public static string ComputeHmacEncoded(this byte[] data, byte[] key, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
public static string ComputeHmacEncoded(this string data, string key, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
public static string ComputeHmacEncoded(this byte[] data, string key, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
public static string ComputeHmacEncoded(this string data, byte[] key, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
```

Ejemplo:

```csharp
string mac = "payload".ComputeHmacEncoded("secret", Base64Encoder.DefaultInstance, HashAlgorithm.Sha256);
```

### VerifyHmac y VerifyHmacFromEncoded

Verifican recalculando el HMAC con la misma clave y algoritmo.

Ejemplo:

```csharp
bool ok = "payload".VerifyHmacFromEncoded(
    "secret",
    mac,
    Base64Encoder.DefaultInstance,
    HashAlgorithm.Sha256);
```

Parametros:

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `data` | `byte[]` o `string` | Datos originales. |
| `key` | `byte[]` o `string` | Clave secreta. |
| `expected` | `byte[]` o `string` | HMAC esperado. |
| `encoder` | `IEncoder` | Encoder para valores codificados. |
| `algorithm` | `HashAlgorithm` | Algoritmo HMAC. |

### ComputeScrypt

Sobrecargas para `byte[]` y `string`:

```csharp
ComputeScrypt(data, salt, iterations, blockSize, parallelism, derivedKeyLength)
```

| Parametro | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `data` | `byte[]` o `string` | Requerido | Password, secreto o datos base. |
| `salt` | `byte[]` o `string` | Requerido | Salt. Si es string, se convierte a UTF-8. |
| `iterations` | `uint` | `Constants.ScryptIterations` | Parametro N/costo CPU y memoria. |
| `blockSize` | `uint` | `Constants.ScryptBlockSize` | Parametro r de Scrypt. |
| `parallelism` | `uint` | `Constants.ScryptParallelism` | Parametro p de Scrypt. |
| `derivedKeyLength` | `uint` | `Constants.ScryptDerivedKeyLength` | Bytes a derivar. |

Ejemplo:

```csharp
byte[] derived = "password".ComputeScrypt("salt");
string encoded = "password".ComputeScryptEncoded("salt", Base64Encoder.DefaultInstance);
```

### VerifyScrypt y VerifyScryptFromEncoded

Recalculan Scrypt con el mismo salt y parametros.

Ejemplo:

```csharp
bool ok = "password".VerifyScryptFromEncoded(
    "salt",
    encoded,
    Base64Encoder.DefaultInstance);
```

### ComputeArgon2

Sobrecargas para `byte[]` y `string`:

```csharp
ComputeArgon2(data, salt, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength)
```

| Parametro | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `data` | `byte[]` o `string` | Requerido | Password, secreto o datos base. |
| `salt` | `byte[]` o `string` | Requerido | Salt. Si es string, se convierte a UTF-8. |
| `iterations` | `uint` | `Constants.Argon2Iterations` | Numero de iteraciones. |
| `memorySizeKiB` | `uint` | `Constants.Argon2MemorySizeInKiB` | Memoria usada por Argon2 en KiB. |
| `parallelism` | `uint` | `Constants.Argon2DegreeOfParallelism` | Grado de paralelismo. |
| `variant` | `Argon2Variant` | `Argon2id` | Variante Argon2. |
| `derivedKeyLength` | `uint` | `Constants.Argon2DerivedKeyLength` | Bytes a derivar. |

`Argon2Variant`:

| Valor | Descripcion |
| --- | --- |
| `Argon2d` | Variante dependiente de datos. |
| `Argon2i` | Variante independiente de datos. |
| `Argon2id` | Variante hibrida, default. |

Ejemplo:

```csharp
string derived = "password".ComputeArgon2Encoded(
    "salt",
    Base64Encoder.DefaultInstance,
    iterations: 2,
    memorySizeKiB: 16384,
    parallelism: 4,
    variant: Argon2Variant.Argon2id);
```

### VerifyArgon2 y VerifyArgon2FromEncoded

Recalculan Argon2 con el mismo salt y parametros.

Ejemplo:

```csharp
bool ok = "password".VerifyArgon2FromEncoded(
    "salt",
    derived,
    Base64Encoder.DefaultInstance,
    variant: Argon2Variant.Argon2id);
```

## Hashers

Las clases hasher envuelven `HashExtensions` y guardan configuracion serializable.

### ShaHasher

Propiedades:

| Propiedad | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `HashAlgorithm` | `HashAlgorithm` | `Sha512` | Algoritmo de hash. |
| `Encoder` | `IEncoder` | `Base64Encoder.DefaultInstance` | Encoder usado para resultados encoded y serializacion. |
| `AssemblyName` | `string` | Calculado | Identificador del tipo. |

Ejemplo:

```csharp
var hasher = new ShaHasher
{
    HashAlgorithm = HashAlgorithm.Sha256,
    Encoder = HexEncoder.DefaultInstance
};

string hash = hasher.ComputeEncoded("Hello");
bool ok = hasher.VerifyEncoded("Hello", hash);
```

Serializacion:

```csharp
string json = hasher.Serialize(indented: true);
IHasher restored = ShaHasher.Deserialize(json);
```

### HmacHasher

Propiedades:

| Propiedad | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `HashAlgorithm` | `HashAlgorithm` | `Sha512` | Algoritmo HMAC. |
| `Encoder` | `IEncoder` | `Base64Encoder.DefaultInstance` | Encoder usado para resultados encoded y para codificar la clave en JSON. |
| `KeyString` | `string` | Aleatoria | Setter: convierte string a UTF-8. Getter: devuelve clave codificada con `Encoder`. |
| `KeyBytes` | `byte[]` | `Constants.HmacKeySize` bytes aleatorios | Clave secreta binaria. |
| `AssemblyName` | `string` | Calculado | Identificador del tipo. |

Ejemplo:

```csharp
var hasher = new HmacHasher
{
    KeyString = "secret",
    HashAlgorithm = HashAlgorithm.Sha256,
    Encoder = Base64Encoder.DefaultInstance
};

string mac = hasher.ComputeEncoded("payload");
bool ok = hasher.VerifyEncoded("payload", mac);
```

Serializacion:

```csharp
string json = hasher.Serialize(indented: true);
IHasher restored = HmacHasher.Deserialize(json);
```

Punto de seguridad:

- El JSON contiene la clave HMAC codificada.

### ScryptHasher

Propiedades:

| Propiedad | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `SaltString` | `string` | Aleatorio | Setter: convierte string a UTF-8. Getter: devuelve salt codificado con `Encoder`. |
| `SaltBytes` | `byte[]` | `Constants.ScryptSaltSize` bytes aleatorios | Salt binario. |
| `Iterations` | `uint` | `Constants.ScryptIterations` | Parametro N/costo. |
| `BlockSize` | `uint` | `Constants.ScryptBlockSize` | Parametro r. |
| `Parallelism` | `uint` | `Constants.ScryptParallelism` | Parametro p. |
| `DerivedKeyLength` | `uint` | `Constants.ScryptDerivedKeyLength` | Bytes a derivar. |
| `Encoder` | `IEncoder` | `Base64Encoder.DefaultInstance` | Encoder para resultados y serializacion del salt. |

Ejemplo:

```csharp
var hasher = new ScryptHasher
{
    SaltString = "app-specific-salt",
    Iterations = Constants.ScryptIterations,
    Encoder = Base64Encoder.DefaultInstance
};

string derived = hasher.ComputeEncoded("password");
bool ok = hasher.VerifyEncoded("password", derived);
```

Serializacion:

```csharp
string json = hasher.Serialize(indented: true);
IHasher restored = ScryptHasher.Deserialize(json);
```

### Argon2Hasher

Propiedades:

| Propiedad | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `SaltString` | `string` | Aleatorio | Setter: convierte string a UTF-8. Getter: devuelve salt codificado con `Encoder`. |
| `SaltBytes` | `byte[]` | `Constants.Argon2SaltSize` bytes aleatorios | Salt binario. |
| `Encoder` | `IEncoder` | `Base64Encoder.DefaultInstance` | Encoder para resultados y serializacion. |
| `Iterations` | `uint` | `Constants.Argon2Iterations` | Iteraciones Argon2. |
| `MemorySizeKiB` | `uint` | `Constants.Argon2MemorySizeInKiB` | Memoria en KiB. |
| `DegreeOfParallelism` | `uint` | `Constants.Argon2DegreeOfParallelism` | Paralelismo. |
| `DerivedKeyLength` | `uint` | `Constants.Argon2DerivedKeyLength` | Bytes a derivar. |
| `Argon2Variant` | `Argon2Variant` | `Argon2id` | Variante Argon2. |

Ejemplo:

```csharp
var hasher = new Argon2Hasher
{
    SaltString = "app-specific-salt",
    Argon2Variant = Argon2Variant.Argon2id,
    MemorySizeKiB = 16384,
    Iterations = 2,
    DegreeOfParallelism = 4,
    Encoder = Base64Encoder.DefaultInstance
};

string derived = hasher.ComputeEncoded("password");
bool ok = hasher.VerifyEncoded("password", derived);
```

Serializacion:

```csharp
string json = hasher.Serialize(indented: true);
IHasher restored = Argon2Hasher.Deserialize(json);
```

Punto de compatibilidad:

- Para verificar en el futuro, conserva los parametros y el salt. El JSON del hasher ya los incluye.

## RandomExtensions

Clase: `InsaneIO.Insane.Cryptography.RandomExtensions`.

### NextValue

```csharp
public static int NextValue(this int value)
```

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `value` | `int` | Valor usado en XOR con un entero aleatorio criptografico. |

Retorna un `int`.

### NextValue con rango

```csharp
public static int NextValue(this int min, int max)
```

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `min` | `int` | Valor minimo permitido. |
| `max` | `int` | Valor maximo permitido. |

Retorna un entero entre `min` y `max`. Lanza `ArgumentException` si `min >= max`.

### NextBytes

```csharp
public static byte[] NextBytes(this uint size)
```

| Parametro | Tipo | Descripcion |
| --- | --- | --- |
| `size` | `uint` | Cantidad de bytes aleatorios. |

Retorna bytes generados con `RandomNumberGenerator.Fill`.

Ejemplo:

```csharp
byte[] key = RandomExtensions.NextBytes(32);
byte[] salt = Constants.Argon2SaltSize.NextBytes();
```

## Serializacion

Las clases serializables escriben un JSON con `AssemblyName` y sus parametros de configuracion.

### Encoders

Ejemplo de `HexEncoder`:

```json
{
  "AssemblyName": "InsaneIO.Insane.Cryptography.HexEncoder, InsaneIO.Insane",
  "ToUpper": true
}
```

Ejemplo de uso:

```csharp
IEncoder encoder = new HexEncoder { ToUpper = true };
string json = encoder.Serialize(indented: true);
IEncoder restored = HexEncoder.Deserialize(json);
```

### Encryptors

`AesCbcEncryptor` serializa:

| Campo | Descripcion |
| --- | --- |
| `AssemblyName` | Tipo concreto. |
| `Key` | Clave codificada con `Encoder`. |
| `Padding` | Valor numerico de `AesCbcPadding`. |
| `Encoder` | JSON del encoder. |

`RsaEncryptor` serializa:

| Campo | Descripcion |
| --- | --- |
| `AssemblyName` | Tipo concreto. |
| `KeyPair` | JSON de `RsaKeyPair`. |
| `Padding` | Valor numerico de `RsaPadding`. |
| `Encoder` | JSON del encoder. |

### Hashers

`ShaHasher` serializa algoritmo y encoder.

`HmacHasher` serializa algoritmo, encoder y clave.

`ScryptHasher` serializa salt, parametros Scrypt y encoder.

`Argon2Hasher` serializa salt, parametros Argon2, variante y encoder.

### Ejemplo Polimorfico Manual

El JSON contiene `AssemblyName`, pero las llamadas de deserializacion actuales son estaticas por tipo concreto:

```csharp
string json = new HmacHasher { KeyString = "secret" }.Serialize();
IHasher hasher = HmacHasher.Deserialize(json);
```

Para deserializar de forma polimorfica, una capa superior podria leer `AssemblyName`, resolver el tipo y llamar al metodo estatico apropiado.

### Campos Sensibles

Trata como secretos los JSON que incluyan:

- `AesCbcEncryptor.Key`.
- `HmacHasher.Key`.
- `RsaKeyPair.PrivateKey`.
- `RsaEncryptor.KeyPair.PrivateKey`.

Los salts de Scrypt y Argon2 no son secretos, pero deben conservarse para verificar el mismo valor en el futuro.

## Ejemplos Integrados

### AES + HMAC

AES-CBC no autentica. Un patron mas seguro es cifrar y luego calcular HMAC del paquete cifrado.

```csharp
var aes = new AesCbcEncryptor
{
    KeyString = "12345678",
    Encoder = Base64Encoder.DefaultInstance
};

var hmac = new HmacHasher
{
    KeyString = "mac-secret",
    HashAlgorithm = HashAlgorithm.Sha256,
    Encoder = Base64Encoder.DefaultInstance
};

string encrypted = aes.EncryptEncoded("payload");
string mac = hmac.ComputeEncoded(encrypted);

bool valid = hmac.VerifyEncoded(encrypted, mac);
string decrypted = valid ? aes.DecryptEncoded(encrypted).ToStringUtf8() : throw new CryptographicException();
```

### RSA + AES Para Payload Grande

```csharp
RsaKeyPair rsaKeys = 2048u.CreateRsaKeyPair(RsaKeyPairEncoding.Pem);
byte[] aesKey = RandomExtensions.NextBytes(32);

string encryptedPayload = "large payload".EncryptAesCbcEncoded(
    aesKey,
    Base64Encoder.DefaultInstance);

string encryptedKey = aesKey.EncryptRsaEncoded(
    rsaKeys.PublicKey!,
    Base64Encoder.DefaultInstance,
    RsaPadding.OaepSha256);

byte[] restoredKey = encryptedKey.DecryptRsaFromEncoded(
    rsaKeys.PrivateKey!,
    Base64Encoder.DefaultInstance,
    RsaPadding.OaepSha256);

string payload = encryptedPayload
    .DecryptAesCbcFromEncoded(restoredKey, Base64Encoder.DefaultInstance)
    .ToStringUtf8();
```

### Password Hash Con Argon2Hasher

```csharp
var passwordHasher = new Argon2Hasher
{
    SaltBytes = RandomExtensions.NextBytes(Constants.Argon2SaltSize),
    Argon2Variant = Argon2Variant.Argon2id,
    Encoder = Base64Encoder.DefaultInstance
};

string passwordHash = passwordHasher.ComputeEncoded("user-password");
string hasherConfig = passwordHasher.Serialize();

IHasher restored = Argon2Hasher.Deserialize(hasherConfig);
bool ok = restored.VerifyEncoded("user-password", passwordHash);
```

## Puntos de Atencion

- AES-CBC cifra, pero no autentica. Usa MAC o una API autenticada si se agrega en el futuro.
- El resultado AES de esta libreria es `ciphertext || iv`, no `iv || ciphertext`.
- Las claves string se convierten a UTF-8. Si necesitas bytes exactos, usa `KeyBytes`, `SaltBytes` o sobrecargas `byte[]`.
- Las verificaciones usan `SequenceEqual` o comparaciones de string. Si el escenario requiere resistencia a timing attacks, revisa una comparacion de tiempo constante.
- MD5 y SHA-1 existen por compatibilidad, pero no deben usarse para seguridad nueva.
- RSA debe usarse con OAEP para usos nuevos; `Pkcs1` queda principalmente por compatibilidad.
- Los metodos `IJSRuntime` usan `eval` para registrar funciones temporales. Revisa politicas CSP si se ejecuta en navegadores con restricciones estrictas.
