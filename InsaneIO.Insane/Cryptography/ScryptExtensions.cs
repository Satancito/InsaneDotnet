using CryptSharp.Utility;

namespace InsaneIO.Insane.Extensions
{
    public static class ScryptExtensions
    {
        public static byte[] ComputeScrypt(this byte[] data, byte[] salt, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return SCrypt.ComputeDerivedKey(data, salt, (int)iterations, (int)blockSize, (int)parallelism, null, (int)derivedKeyLength);
        }

        public static byte[] ComputeScrypt(this string data, string salt, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return ComputeScrypt(data.ToByteArrayUtf8(), salt.ToByteArrayUtf8(), iterations, blockSize, parallelism, derivedKeyLength);
        }

        public static byte[] ComputeScrypt(this byte[] data, string salt, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return ComputeScrypt(data, salt.ToByteArrayUtf8(), iterations, blockSize, parallelism, derivedKeyLength);
        }

        public static byte[] ComputeScrypt(this string data, byte[] salt, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return ComputeScrypt(data.ToByteArrayUtf8(), salt, iterations, blockSize, parallelism, derivedKeyLength);
        }

        public static string ComputeScryptEncoded(this byte[] data, byte[] salt, IEncoder encoder, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return encoder.Encode(ComputeScrypt(data, salt, iterations, blockSize, parallelism, derivedKeyLength));
        }

        public static string ComputeScryptEncoded(this string data, string salt, IEncoder encoder, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return encoder.Encode(ComputeScrypt(data, salt, iterations, blockSize, parallelism, derivedKeyLength));
        }

        public static string ComputeScryptEncoded(this string data, byte[] salt, IEncoder encoder, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return encoder.Encode(ComputeScrypt(data, salt, iterations, blockSize, parallelism, derivedKeyLength));
        }

        public static string ComputeScryptEncoded(this byte[] data, string salt, IEncoder encoder, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return encoder.Encode(ComputeScrypt(data, salt, iterations, blockSize, parallelism, derivedKeyLength));
        }

        public static bool VerifyScrypt(this byte[] data, byte[] salt, byte[] expected, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return CryptographyComparisonExtensions.FixedTimeEquals(data.ComputeScrypt(salt, iterations, blockSize, parallelism, derivedKeyLength), expected);
        }

        public static bool VerifyScrypt(this string data, string salt, byte[] expected, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return CryptographyComparisonExtensions.FixedTimeEquals(data.ComputeScrypt(salt, iterations, blockSize, parallelism, derivedKeyLength), expected);
        }

        public static bool VerifyScrypt(this byte[] data, string salt, byte[] expected, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return CryptographyComparisonExtensions.FixedTimeEquals(data.ComputeScrypt(salt, iterations, blockSize, parallelism, derivedKeyLength), expected);
        }

        public static bool VerifyScrypt(this string data, byte[] salt, byte[] expected, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return CryptographyComparisonExtensions.FixedTimeEquals(data.ComputeScrypt(salt, iterations, blockSize, parallelism, derivedKeyLength), expected);
        }

        public static bool VerifyScryptFromEncoded(this byte[] data, byte[] salt, string expected, IEncoder encoder, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return CryptographyComparisonExtensions.FixedTimeEqualsEncoded(data.ComputeScryptEncoded(salt, encoder, iterations, blockSize, parallelism, derivedKeyLength), expected);
        }

        public static bool VerifyScryptFromEncoded(this string data, string salt, string expected, IEncoder encoder, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return CryptographyComparisonExtensions.FixedTimeEqualsEncoded(data.ComputeScryptEncoded(salt, encoder, iterations, blockSize, parallelism, derivedKeyLength), expected);
        }

        public static bool VerifyScryptFromEncoded(this string data, byte[] salt, string expected, IEncoder encoder, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return CryptographyComparisonExtensions.FixedTimeEqualsEncoded(data.ComputeScryptEncoded(salt, encoder, iterations, blockSize, parallelism, derivedKeyLength), expected);
        }

        public static bool VerifyScryptFromEncoded(this byte[] data, string salt, string expected, IEncoder encoder, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return CryptographyComparisonExtensions.FixedTimeEqualsEncoded(data.ComputeScryptEncoded(salt, encoder, iterations, blockSize, parallelism, derivedKeyLength), expected);
        }
    }
}
