using CryptSharp.Utility;
using Konscious.Security.Cryptography;
using System.Linq;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using HashAlgorithm = InsaneIO.Insane.Cryptography.HashAlgorithm;

namespace InsaneIO.Insane.Extensions
{

    
    public static class HashExtensions
    {

        public static byte[] ComputeHash(this byte[] data, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            switch (algorithm)
            {
                case HashAlgorithm.Md5:
                    {
                        using MD5 md5 = MD5.Create();
                        return md5.ComputeHash(data);
                    }
                case HashAlgorithm.Sha1:
                    {
                        using SHA1 sha1 = SHA1.Create();
                        return sha1.ComputeHash(data);
                    }
                case HashAlgorithm.Sha256:
                    {
                        using SHA256 sha256 = SHA256.Create();
                        return sha256.ComputeHash(data);
                    }
                case HashAlgorithm.Sha384:
                    {
                        using SHA384 sha384 = SHA384.Create();
                        return sha384.ComputeHash(data);
                    }
                case HashAlgorithm.Sha512:
                    {
                        using SHA512 sha512 = SHA512.Create();
                        return sha512.ComputeHash(data);
                    }
                default:
                    throw new NotImplementedException(algorithm.ToString());
            }
        }

        public static byte[] ComputeHash(this string data, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return ComputeHash(data.ToByteArrayUtf8(), algorithm);
        }

        public static string ComputeHashEncoded(this byte[] data, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return encoder.Encode(ComputeHash(data, algorithm));
        }

        public static string ComputeHashEncoded(this string data, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return encoder.Encode(ComputeHash(data, algorithm));
        }

        public static bool VerifyHash(this byte[] data, byte[] expected, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return data.ComputeHash(algorithm).SequenceEqual(expected);
        }

        public static bool VerifyHash(this string data, byte[] expected, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return data.ComputeHash(algorithm).SequenceEqual(expected);
        }

        public static bool VerifyHashFromEncoded(this byte[] data, string expected, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return data.ComputeHashEncoded(encoder,algorithm) == expected;
        }

        public static bool VerifyHashFromEncoded(this string data, string expected, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return data.ComputeHashEncoded(encoder, algorithm) == expected;
        }

        public static byte[] ComputeHmac(this byte[] data, byte[] key, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            switch (algorithm)
            {
                case HashAlgorithm.Md5:
                    {
                        using HMACMD5 hmac = new(key);
                        return hmac.ComputeHash(data);
                    }
                case HashAlgorithm.Sha1:
                    {
                        using HMACSHA1 hmac = new(key);
                        return hmac.ComputeHash(data);
                    }
                case HashAlgorithm.Sha256:
                    {
                        using HMACSHA256 hmac = new(key);
                        return hmac.ComputeHash(data);
                    }
                case HashAlgorithm.Sha384:
                    {
                        using HMACSHA384 hmac = new(key);
                        return hmac.ComputeHash(data);
                    }
                case HashAlgorithm.Sha512:
                    {
                        using HMACSHA512 hmac = new(key);
                        return hmac.ComputeHash(data);
                    }
                default:
                    throw new NotImplementedException(algorithm.ToString());
            }
        }

        public static byte[] ComputeHmac(this string data, byte[] key, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return ComputeHmac(data.ToByteArrayUtf8(), key, algorithm);
        }

        public static byte[] ComputeHmac(this string data, string key, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return ComputeHmac(data.ToByteArrayUtf8(), key.ToByteArrayUtf8(), algorithm);
        }

        public static byte[] ComputeHmac(this byte[] data, string key, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return ComputeHmac(data, key.ToByteArrayUtf8(), algorithm);
        }

        public static string ComputeHmacEncoded(this byte[] data, byte[] key, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return encoder.Encode(ComputeHmac(data, key, algorithm));
        }


        public static string ComputeHmacEncoded(this string data, string key, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return encoder.Encode(ComputeHmac(data, key, algorithm));
        }

        public static string ComputeHmacEncoded(this byte[] data, string key, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return encoder.Encode(ComputeHmac(data, key, algorithm));
        }


        public static string ComputeHmacEncoded(this string data, byte[] key, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return encoder.Encode(ComputeHmac(data, key, algorithm));
        }

        public static bool VerifyHmac(this byte[] data, byte[] key, byte[] expected, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return data.ComputeHmac(key, algorithm).SequenceEqual(expected);
        }

        public static bool VerifyHmac(this string data, byte[] key, byte[] expected, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return data.ComputeHmac(key, algorithm).SequenceEqual(expected);
        }

        public static bool VerifyHmac(this string data, string key, byte[] expected, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return data.ComputeHmac(key, algorithm).SequenceEqual(expected);
        }

        public static bool VerifyHmac(this byte[] data, string key, byte[] expected, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return data.ComputeHmac(key, algorithm).SequenceEqual(expected);
        }

        public static bool VerifyHmacFromEncoded(this byte[] data, byte[] key, string expected, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return data.ComputeHmacEncoded(key, encoder,algorithm).SequenceEqual(expected);
        }


        public static bool VerifyHmacFromEncoded(this string data, string key, string expected, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return data.ComputeHmacEncoded(key, encoder, algorithm).SequenceEqual(expected);
        }

        public static bool VerifyHmacFromEncoded(this string data, byte[] key, string expected, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return data.ComputeHmacEncoded(key, encoder, algorithm).SequenceEqual(expected);
        }


        public static bool VerifyHmacFromEncoded(this byte[] data, string key, string expected, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return data.ComputeHmacEncoded(key, encoder, algorithm).SequenceEqual(expected);
        }

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
            return data.ComputeScrypt(salt, iterations, blockSize, parallelism, derivedKeyLength).SequenceEqual(expected);
        }

        public static bool VerifyScrypt(this string data, string salt, byte[] expected, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return data.ComputeScrypt(salt, iterations, blockSize, parallelism, derivedKeyLength).SequenceEqual(expected);
        }

        public static bool VerifyScrypt(this byte[] data, string salt, byte[] expected, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return data.ComputeScrypt(salt, iterations, blockSize, parallelism, derivedKeyLength).SequenceEqual(expected);
        }

        public static bool VerifyScrypt(this string data, byte[] salt, byte[] expected, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return data.ComputeScrypt(salt, iterations, blockSize, parallelism, derivedKeyLength).SequenceEqual(expected);
        }

        public static bool VerifyScryptFromEncoded(this byte[] data, byte[] salt, string expected, IEncoder encoder, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return data.ComputeScryptEncoded(salt,encoder, iterations, blockSize, parallelism, derivedKeyLength).SequenceEqual(expected);
        }

        public static bool VerifyScryptFromEncoded(this string data, string salt, string expected, IEncoder encoder, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return data.ComputeScryptEncoded(salt, encoder, iterations, blockSize, parallelism, derivedKeyLength).SequenceEqual(expected);
        }

        public static bool VerifyScryptFromEncoded(this string data, byte[] salt, string expected, IEncoder encoder, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return data.ComputeScryptEncoded(salt, encoder, iterations, blockSize, parallelism, derivedKeyLength).SequenceEqual(expected);
        }

        public static bool VerifyScryptFromEncoded(this byte[] data, string salt, string expected, IEncoder encoder, uint iterations = Constants.ScryptIterations, uint blockSize = Constants.ScryptBlockSize, uint parallelism = Constants.ScryptParallelism, uint derivedKeyLength = Constants.ScryptDerivedKeyLength)
        {
            return data.ComputeScryptEncoded(salt, encoder, iterations, blockSize, parallelism, derivedKeyLength).SequenceEqual(expected);
        }

        public static byte[] ComputeArgon2(this byte[] data, byte[] salt, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
        {
            switch (variant)
            {
                case Argon2Variant.Argon2d:
                    {
                        using Argon2d argon2d = new(data)
                        {
                            Salt = salt,
                            Iterations = (int)iterations,
                            MemorySize = (int)memorySizeKiB,
                            DegreeOfParallelism = (int)parallelism
                        };
                        return argon2d.GetBytes((int)derivedKeyLength);
                    }
                case Argon2Variant.Argon2i:
                    {
                        using Argon2i argon2i = new(data)
                        {
                            Salt = salt,
                            Iterations = (int)iterations,
                            MemorySize = (int)memorySizeKiB,
                            DegreeOfParallelism = (int)parallelism
                        };
                        return argon2i.GetBytes((int)derivedKeyLength);
                    }
                case Argon2Variant.Argon2id:
                    {
                        using Argon2id argon2id = new(data)
                        {
                            Salt = salt,
                            Iterations = (int)iterations,
                            MemorySize = (int)memorySizeKiB,
                            DegreeOfParallelism = (int)parallelism
                        };
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
            return data.ComputeArgon2(salt, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength).SequenceEqual(expected);
        }

        public static bool VerifyArgon2(this string data, string salt, byte[] expected, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
        {
            return data.ComputeArgon2(salt, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength).SequenceEqual(expected);
        }

        public static bool VerifyArgon2(this byte[] data, string salt, byte[] expected, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
        {
            return data.ComputeArgon2(salt, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength).SequenceEqual(expected);
        }

        public static bool VerifyArgon2(this string data, byte[] salt, byte[] expected, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
        {
            return data.ComputeArgon2(salt, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength).SequenceEqual(expected);
        }


        public static bool VerifyArgon2FromEncoded(this byte[] data, byte[] salt, string expected, IEncoder encoder, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
        {
            return data.ComputeArgon2Encoded(salt,encoder, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength).SequenceEqual(expected);
        }

        public static bool VerifyArgon2FromEncoded(this string data, string salt, string expected, IEncoder encoder, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
        {
            return data.ComputeArgon2Encoded(salt, encoder, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength).SequenceEqual(expected);
        }

        public static bool VerifyArgon2FromEncoded(this string data, byte[] salt, string expected, IEncoder encoder, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
        {
            return data.ComputeArgon2Encoded(salt, encoder, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength).SequenceEqual(expected);
        }

        public static bool VerifyArgon2FromEncoded(this byte[] data, string salt, string expected, IEncoder encoder, uint iterations = Constants.Argon2Iterations, uint memorySizeKiB = Constants.Argon2MemorySizeInKiB, uint parallelism = Constants.Argon2DegreeOfParallelism, Argon2Variant variant = Argon2Variant.Argon2id, uint derivedKeyLength = Constants.Argon2DerivedKeyLength)
        {
            return data.ComputeArgon2Encoded(salt, encoder, iterations, memorySizeKiB, parallelism, variant, derivedKeyLength).SequenceEqual(expected);
        }

    }

}
