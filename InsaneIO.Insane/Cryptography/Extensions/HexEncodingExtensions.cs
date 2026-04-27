namespace InsaneIO.Insane.Cryptography.Extensions
{
    public static class HexEncodingExtensions
    {
        public static string EncodeToHex(this byte[] data, bool toUpper = false)
        {
            data.ThrowIfNull();
            StringBuilder ret = new(string.Empty);
            foreach (byte value in data)
            {
                ret.Append(value.ToString(toUpper? "X2": "x2"));
            }
            return ret.ToString();
        }

        public static string EncodeToHex(this string data, bool toUpper = false)
        {
            return EncodeToHex(data.ToByteArrayUtf8(), toUpper);
        }

        public static byte[] DecodeFromHex(this string data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length % 2 != 0)
            {
                throw new ArgumentException("Hex data length must be even.", nameof(data));
            }

            byte[] ret = new byte[data.Length / 2];
            for (int i = 0; i < data.Length / 2; i++)
            {
                ret[i] = Convert.ToByte(data.Substring(i * 2, 2), 16);
            }
            return ret;
        }
    }
}

