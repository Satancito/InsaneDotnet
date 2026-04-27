using InsaneIO.Insane.Attributes;
using InsaneIO.Insane.Cryptography.Abstractions;
using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Serialization;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Security
{
    /// <summary>
    /// Testing codes in the link - https://totp.danhersam.com/
    /// </summary>
    [TypeIdentifier("Insane-Security-TotpManager")]
    public class TotpManager : IJsonSerializable
    {
        public required byte[] Secret { get; init; } = null!;

        public required string Label { get; init; } = string.Empty;
        public required string Issuer { get; init; } = string.Empty;
        public TwoFactorCodeLength CodeLength { get; init; } = TwoFactorCodeLength.SixDigits;
        public HashAlgorithm HashAlgorithm { get; init; } = HashAlgorithm.Sha1;
        public uint TimePeriodInSeconds { get; init; } = TotpExtensions.TotpDefaultPeriod;

        public static TotpManager FromSecret(byte[] secret, string label, string issuer, TwoFactorCodeLength codeLength = TwoFactorCodeLength.SixDigits, HashAlgorithm hashAlgorithm = HashAlgorithm.Sha1, uint timePeriodInSeconds = TotpExtensions.TotpDefaultPeriod)
        {
            return new TotpManager
            {
                Secret = secret,
                Label = label,
                Issuer = issuer,
                CodeLength = codeLength,
                HashAlgorithm = hashAlgorithm,
                TimePeriodInSeconds = timePeriodInSeconds
            };
        }

        public static TotpManager FromBase32Secret(string base32EncodedSecret, string label, string issuer, TwoFactorCodeLength codeLength = TwoFactorCodeLength.SixDigits, HashAlgorithm hashAlgorithm = HashAlgorithm.Sha1, uint timePeriodInSeconds = TotpExtensions.TotpDefaultPeriod)
        {
            return FromSecret(Base32Encoder.DefaultInstance.Decode(base32EncodedSecret), label, issuer, codeLength, hashAlgorithm, timePeriodInSeconds);
        }

        public static TotpManager FromEncodedSecret(string encodedSecret, IEncoder secretDecoder, string label, string issuer, TwoFactorCodeLength codeLength = TwoFactorCodeLength.SixDigits, HashAlgorithm hashAlgorithm = HashAlgorithm.Sha1, uint timePeriodInSeconds = TotpExtensions.TotpDefaultPeriod)
        {
            return FromSecret(secretDecoder.Decode(encodedSecret), label, issuer, codeLength, hashAlgorithm, timePeriodInSeconds);
        }

        public string Serialize(bool indented = true)
        {
            return ToJsonObject().ToJsonString(IJsonSerializable.GetIndentOptions(indented));
        }

        public JsonObject ToJsonObject()
        {
            return new JsonObject
            {
                [TypeIdentifierResolver.TypeIdentifierJsonPropertyName] = TypeIdentifierResolver.GetTypeIdentifier(typeof(TotpManager)),
                [nameof(Secret)] = Base32Encoder.DefaultInstance.Encode(Secret),
                [nameof(Label)] = Label,
                [nameof(Issuer)] = Issuer,
                [nameof(CodeLength)] = CodeLength.NumberValue<int>(),
                [nameof(HashAlgorithm)] = HashAlgorithm.NumberValue<int>(),
                [nameof(TimePeriodInSeconds)] = TimePeriodInSeconds,
            };
        }

        public static TotpManager Deserialize(string json)
        {
            JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(typeof(TotpManager), json);
            if (!TypeIdentifierResolver.MatchesSerializedType(typeof(TotpManager), jsonNode))
            {
                throw new DeserializeException(typeof(TotpManager), json);
            }

            TwoFactorCodeLength codeLength = (TwoFactorCodeLength)(jsonNode[nameof(CodeLength)]?.GetValue<int>() ?? throw new DeserializeException(typeof(TotpManager), json));
            HashAlgorithm hashAlgorithm = (HashAlgorithm)(jsonNode[nameof(HashAlgorithm)]?.GetValue<int>() ?? throw new DeserializeException(typeof(TotpManager), json));

            if (!Enum.IsDefined(codeLength) || !Enum.IsDefined(hashAlgorithm))
            {
                throw new DeserializeException(typeof(TotpManager), json);
            }

            return new TotpManager
            {
                Secret = Base32Encoder.DefaultInstance.Decode(jsonNode[nameof(Secret)]?.GetValue<string>() ?? throw new DeserializeException(typeof(TotpManager), json)),
                Label = jsonNode[nameof(Label)]?.GetValue<string>() ?? throw new DeserializeException(typeof(TotpManager), json),
                Issuer = jsonNode[nameof(Issuer)]?.GetValue<string>() ?? throw new DeserializeException(typeof(TotpManager), json),
                CodeLength = codeLength,
                HashAlgorithm = hashAlgorithm,
                TimePeriodInSeconds = jsonNode[nameof(TimePeriodInSeconds)]?.GetValue<uint>() ?? throw new DeserializeException(typeof(TotpManager), json)
            };
        }

        public string ToOtpUri()
        {
            return Secret.GenerateTotpUri(Label, Issuer, HashAlgorithm, CodeLength, TimePeriodInSeconds);
        }

        public string GenerateTotpUri()
        {
            return ToOtpUri();
        }

        public bool VerifyCode(string code)
        {
            return code.VerifyTotpCode(Secret, CodeLength, HashAlgorithm, TimePeriodInSeconds);
        }

        public bool VerifyCode(string code, DateTimeOffset now)
        {
            return code.VerifyTotpCode(Secret, now, CodeLength, HashAlgorithm, TimePeriodInSeconds);
        }

        public bool VerifyTotpCode(string code)
        {
            return VerifyCode(code);
        }

        public bool VerifyTotpCode(string code, DateTimeOffset now)
        {
            return VerifyCode(code, now);
        }

        public string ComputeCode()
        {
            return Secret.ComputeTotpCode(CodeLength, HashAlgorithm, TimePeriodInSeconds);
        }

        public string ComputeCode(DateTimeOffset now)
        {
            return Secret.ComputeTotpCode(now, CodeLength, HashAlgorithm, TimePeriodInSeconds);
        }

        public string ComputeTotpCode()
        {
            return ComputeCode();
        }

        public string ComputeTotpCode(DateTimeOffset now)
        {
            return ComputeCode(now);
        }

        public long ComputeRemainingSeconds()
        {
            return DateTimeOffset.UtcNow.ComputeTotpRemainingSeconds(TimePeriodInSeconds);
        }

        public long ComputeRemainingSeconds(DateTimeOffset now)
        {
            return now.ComputeTotpRemainingSeconds(TimePeriodInSeconds);
        }

        public long ComputeTotpRemainingSeconds()
        {
            return ComputeRemainingSeconds();
        }

        public long ComputeTotpRemainingSeconds(DateTimeOffset now)
        {
            return ComputeRemainingSeconds(now);
        }
    }
}
