﻿using System.Runtime.Versioning;

namespace InsaneIO.Insane.Converter
{
    [RequiresPreviewFeatures]
    public class AesStringValueConverter : IValueConverter<string>
    {
        private readonly AesCbcEncryptor encryptor;

        public AesStringValueConverter(AesCbcEncryptor encryptor)
        {
            this.encryptor = encryptor;
        }

        public string Convert(string value)
        {
            return encryptor.Encrypt(value);
        }

        public string Deconvert(string value)
        {
            return encryptor.Decrypt(value);
        }
    }
}