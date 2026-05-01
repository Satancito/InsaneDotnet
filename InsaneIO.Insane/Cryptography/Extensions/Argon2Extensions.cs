using InsaneIO.Insane.Cryptography.Enums;
using Konscious.Security.Cryptography;

namespace InsaneIO.Insane.Cryptography.Extensions;

public static class Argon2Extensions
{
    public static byte[] ComputeArgon2(this byte[] data, byte[] salt, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
    {
        switch (variant)
        {
            case Argon2Variant.Argon2d:
                using (Argon2d argon2d = new(data))
                {
                    argon2d.Salt = salt;
                    argon2d.Iterations = (int)iterations;
                    argon2d.MemorySize = (int)memorySizeKiB;
                    argon2d.DegreeOfParallelism = (int)parallelism;
                    return argon2d.GetBytes((int)derivedKeyLength);
                }
            case Argon2Variant.Argon2i:
                using (Argon2i argon2i = new(data))
                {
                    argon2i.Salt = salt;
                    argon2i.Iterations = (int)iterations;
                    argon2i.MemorySize = (int)memorySizeKiB;
                    argon2i.DegreeOfParallelism = (int)parallelism;
                    return argon2i.GetBytes((int)derivedKeyLength);
                }
            case Argon2Variant.Argon2id:
                using (Argon2id argon2id = new(data))
                {
                    argon2id.Salt = salt;
                    argon2id.Iterations = (int)iterations;
                    argon2id.MemorySize = (int)memorySizeKiB;
                    argon2id.DegreeOfParallelism = (int)parallelism;
                    return argon2id.GetBytes((int)derivedKeyLength);
                }
            default:
                throw new NotImplementedException(variant.ToString());
        }
    }

    public static byte[] ComputeArgon2(this string data, string salt, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
    {
        return ComputeArgon2(data.ToByteArrayUtf8(), salt.ToByteArrayUtf8(), iterations, memorySizeKiB, parallelism, variant, derivedKeyLength);
    }

    public static byte[] ComputeArgon2(this byte[] data, string salt, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
    {
        return ComputeArgon2(data, salt.ToByteArrayUtf8(), iterations, memorySizeKiB, parallelism, variant, derivedKeyLength);
    }

    public static byte[] ComputeArgon2(this string data, byte[] salt, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
    {
        return ComputeArgon2(data.ToByteArrayUtf8(), salt, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength);
    }

    public static string ComputeArgon2Encoded(this byte[] data, byte[] salt, IEncoder encoder, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
    {
        return encoder.Encode(ComputeArgon2(data, salt, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength));
    }

    public static string ComputeArgon2Encoded(this string data, string salt, IEncoder encoder, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
    {
        return encoder.Encode(ComputeArgon2(data, salt, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength));
    }

    public static string ComputeArgon2Encoded(this string data, byte[] salt, IEncoder encoder, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
    {
        return encoder.Encode(ComputeArgon2(data, salt, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength));
    }

    public static string ComputeArgon2Encoded(this byte[] data, string salt, IEncoder encoder, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
    {
        return encoder.Encode(ComputeArgon2(data, salt, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength));
    }

    public static bool VerifyArgon2(this byte[] data, byte[] salt, byte[] expected, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
    {
        return CryptographyComparisonExtensions.FixedTimeEquals(data.ComputeArgon2(salt, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength), expected);
    }

    public static bool VerifyArgon2(this string data, string salt, byte[] expected, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
    {
        return CryptographyComparisonExtensions.FixedTimeEquals(data.ComputeArgon2(salt, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength), expected);
    }

    public static bool VerifyArgon2(this byte[] data, string salt, byte[] expected, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
    {
        return CryptographyComparisonExtensions.FixedTimeEquals(data.ComputeArgon2(salt, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength), expected);
    }

    public static bool VerifyArgon2(this string data, byte[] salt, byte[] expected, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
    {
        return CryptographyComparisonExtensions.FixedTimeEquals(data.ComputeArgon2(salt, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength), expected);
    }

    public static bool VerifyArgon2FromEncoded(this byte[] data, byte[] salt, string expected, IEncoder encoder, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
    {
        return CryptographyComparisonExtensions.FixedTimeEqualsEncoded(data.ComputeArgon2Encoded(salt, encoder, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength), expected);
    }

    public static bool VerifyArgon2FromEncoded(this string data, string salt, string expected, IEncoder encoder, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
    {
        return CryptographyComparisonExtensions.FixedTimeEqualsEncoded(data.ComputeArgon2Encoded(salt, encoder, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength), expected);
    }

    public static bool VerifyArgon2FromEncoded(this string data, byte[] salt, string expected, IEncoder encoder, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
    {
        return CryptographyComparisonExtensions.FixedTimeEqualsEncoded(data.ComputeArgon2Encoded(salt, encoder, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength), expected);
    }

    public static bool VerifyArgon2FromEncoded(this byte[] data, string salt, string expected, IEncoder encoder, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
    {
        return CryptographyComparisonExtensions.FixedTimeEqualsEncoded(data.ComputeArgon2Encoded(salt, encoder, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength), expected);
    }
}

