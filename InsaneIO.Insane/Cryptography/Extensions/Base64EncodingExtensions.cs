using System.Runtime.Versioning;

namespace InsaneIO.Insane.Cryptography.Extensions
{
    
    public static class Base64EncodingExtensions
    {
        public const uint NoLineBreaks = 0;
        public const uint MimeLineBreaksLength = 76;
        public const uint PemLineBreaksLength = 64;

        public static string InsertLineBreaks(this string data, uint lineBreaksLength = MimeLineBreaksLength)
        {
            int distance = (int)lineBreaksLength;
            if (lineBreaksLength == 0)
            {
                return data;
            }
            int Segments = data.Length / distance;
            if (Segments == 0)
            {
                return data;
            }
            else
            {
                StringBuilder sb = new();
                for (int i = 0; i < Segments; i++)
                {
                    sb.AppendLine(data.Substring(i * distance, distance));
                }
                if (Segments * lineBreaksLength < data.Length)
                {
                    sb.AppendLine(data[(Segments * distance)..]);
                }
                return sb.ToString(0, sb.Length - Environment.NewLine.Length);
            }
        }

        public static string EncodeToBase64(this byte[] data, uint lineBreaksLength = NoLineBreaks, bool removePadding = false)
        {
            var result = InsertLineBreaks(Convert.ToBase64String(data), lineBreaksLength);
            return removePadding ? result.Replace(Constants.EqualSign, string.Empty) : result;
        }

        public static string EncodeToBase64(this string data, uint lineBreaksLength = NoLineBreaks, bool removePadding = false)
        {
            return EncodeToBase64(data.ToByteArrayUtf8(), lineBreaksLength, removePadding);
        }

        public static string EncodeToUrlSafeBase64(this byte[] data)
        {
            StringBuilder sb = new(EncodeToBase64(data));
            sb.Replace(Constants.PlusSign, Constants.MinusSign).Replace(Constants.Slash, Constants.Underscore).Replace(Constants.EqualSign, string.Empty);
            return sb.ToString();
        }

        public static string EncodeToFilenameSafeBase64(this byte[] data)
        {
            return EncodeToUrlSafeBase64(data);
        }

        public static string EncodeToUrlEncodedBase64(this byte[] data)
        {
            StringBuilder sb = new(EncodeToBase64(data));
            sb.Replace(Constants.PlusSign, Constants.UrlEncodedPlusSign).Replace(Constants.Slash, Constants.UrlEncodedSlash).Replace(Constants.EqualSign, Constants.UrlEncodedEqualSign);
            return sb.ToString();
        }

        public static byte[] DecodeFromBase64(this string data)
        {
            StringBuilder sb = new(data.Trim());
            sb.Replace(Constants.UrlEncodedPlusSign, Constants.PlusSign)
                .Replace(Constants.UrlEncodedSlash, Constants.Slash)
                .Replace(Constants.UrlEncodedEqualSign, Constants.EqualSign)
                .Replace(Constants.MinusSign, Constants.PlusSign)
                .Replace(Constants.Underscore, Constants.Slash)
                .Replace(Constants.LineFeed, string.Empty)
                .Replace(Constants.CarriageReturn, string.Empty);
            int modulo = sb.Length % 4;
            sb.Append('=', modulo > 0 ? 4 - modulo : 0);
            return Convert.FromBase64String(sb.ToString());
        }

        public static string EncodeBase64ToUrlSafeBase64(this string base64)
        {
            return EncodeToUrlSafeBase64(DecodeFromBase64(base64));
        }

        public static string EncodeBase64ToFilenameSafeBase64(this string base64)
        {
            return EncodeToFilenameSafeBase64(DecodeFromBase64(base64));
        }

        public static string EncodeBase64ToUrlEncodedBase64(this string base64)
        {
            return EncodeToUrlEncodedBase64(DecodeFromBase64(base64));
        }
    }
}

