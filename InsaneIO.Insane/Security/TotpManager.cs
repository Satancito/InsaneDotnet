using InsaneIO.Insane.Cryptography;
using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Serialization;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Security
{
    /// <summary>
    /// Testing codes in the link - https://totp.danhersam.com/
    /// </summary>
    public class TotpManager : IJsonSerializable
    {
        public static Type SelfType => typeof(TotpManager);
        public string AssemblyName { get => IJsonSerializable.GetName(SelfType); }

        public required byte[] Secret { get; init; } = null!;

        public required string Label { get; init; } = string.Empty;
        public required string Issuer { get; init; } = string.Empty;
        public TwoFactorCodeLength CodeLength { get; init; } = TwoFactorCodeLength.SixDigits;
        public HashAlgorithm HashAlgorithm { get; init; } = HashAlgorithm.Sha1;
        public uint TimePeriodInSeconds { get; init; } = TotpExtensions.TotpDefaultPeriod;

        public string Serialize(bool indented = true)
        {
            return ToJsonObject().ToJsonString(IJsonSerializable.GetIndentOptions(indented));
        }

        public JsonObject ToJsonObject()
        {
            return new JsonObject
            {
                [nameof(AssemblyName)] = AssemblyName,
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
            JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(SelfType, json);
            string assemblyName = jsonNode[nameof(AssemblyName)]?.GetValue<string>() ?? throw new DeserializeException(SelfType, json);

            if (assemblyName != IJsonSerializable.GetName(SelfType))
            {
                throw new DeserializeException(SelfType, json);
            }

            TwoFactorCodeLength codeLength = (TwoFactorCodeLength)(jsonNode[nameof(CodeLength)]?.GetValue<int>() ?? throw new DeserializeException(SelfType, json));
            HashAlgorithm hashAlgorithm = (HashAlgorithm)(jsonNode[nameof(HashAlgorithm)]?.GetValue<int>() ?? throw new DeserializeException(SelfType, json));

            if (!Enum.IsDefined(codeLength) || !Enum.IsDefined(hashAlgorithm))
            {
                throw new DeserializeException(SelfType, json);
            }

            return new TotpManager
            {
                Secret = Base32Encoder.DefaultInstance.Decode(jsonNode[nameof(Secret)]?.GetValue<string>() ?? throw new DeserializeException(SelfType, json)),
                Label = jsonNode[nameof(Label)]?.GetValue<string>() ?? throw new DeserializeException(SelfType, json),
                Issuer = jsonNode[nameof(Issuer)]?.GetValue<string>() ?? throw new DeserializeException(SelfType, json),
                CodeLength = codeLength,
                HashAlgorithm = hashAlgorithm,
                TimePeriodInSeconds = jsonNode[nameof(TimePeriodInSeconds)]?.GetValue<uint>() ?? throw new DeserializeException(SelfType, json)
            };
        }

        public string ToOtpUri()
        {
            return Secret.GenerateTotpUri(Label, Issuer, HashAlgorithm, CodeLength, TimePeriodInSeconds);
        }

        public bool VerifyCode(string code)
        {
            return code.VerifyTotpCode(Secret, CodeLength, HashAlgorithm, TimePeriodInSeconds);
        }

        public bool VerifyCode(string code, DateTimeOffset now)
        {
            return code.VerifyTotpCode(Secret, now, CodeLength, HashAlgorithm, TimePeriodInSeconds);
        }

        public string ComputeCode()
        {
            return Secret.ComputeTotpCode(CodeLength, HashAlgorithm, TimePeriodInSeconds);
        }

        public string ComputeCode(DateTimeOffset now)
        {
            return Secret.ComputeTotpCode(now, CodeLength, HashAlgorithm, TimePeriodInSeconds);
        }
    }
}
